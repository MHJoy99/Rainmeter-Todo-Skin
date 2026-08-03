using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using RainmeterBackend;

internal static partial class TodoApp
{
    private const string GitHubRepository = "MHJoy99/Rainmeter-Todo-Skin";
    private static string ResourceDir = AppDomain.CurrentDomain.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar);
    private static readonly string AppVersion = LoadAppVersion();
    private const string AppEditionName = "Unified";
    private static string StatePath { get { return Path.Combine(ResourceDir, "tasks.json"); } }
    private static string IncludePath { get { return Path.Combine(ResourceDir, "Generated.inc"); } }
    private static string GuardPath { get { return Path.Combine(ResourceDir, ".refresh-guard"); } }
    private static string UpdaterScript { get { return Path.Combine(ResourceDir, "Updater", "RainmeterDesktopWidgetsUpdater.ps1"); } }
    private static string PaperCache { get { return Path.Combine(ResourceDir, "PaperCache"); } }
    private static string PaperSyncSecret { get { return Path.Combine(ResourceDir, "paper-sync.secret"); } }
    private static string TranslationSecret { get { return Path.Combine(ResourceDir, "translation.secret"); } }

    private static string LoadAppVersion()
    {
        string versionPath = Path.Combine(ResourceDir, "app-version.txt");
        if (File.Exists(versionPath))
        {
            string value = File.ReadAllText(versionPath, Encoding.UTF8).Trim();
            if (value != "") return value;
        }
        return "0.0.0";
    }

    [STAThread]
    private static int Main(string[] args)
    {
        UiScale.EnableDpiAwareness();
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        string action = args.Length > 0 ? args[0] : "Render";
        string id = args.Length > 1 ? args[1] : "";
        bool force = args.Any(x => String.Equals(x, "Force", StringComparison.OrdinalIgnoreCase));
        if (action == "Add") return AddInteractive();
        if (action == "Edit") return EditInteractive(id);
        if (action == "Manage") return ManageInteractive();
        if (action == "Settings") return SettingsInteractive();
        if (action == "SignIn") return GoogleTasksSignInInteractive();
        if (action == "PaperWorker") return RunPaperWorker(id);
        if (action == "PaperSelfTest") return RunPaperSelfTests();
        using (Mutex mutex = OpenStateMutex())
        {
            bool held = false;
            Dictionary<string, object> state = null;
            try
            {
                held = mutex.WaitOne(TimeSpan.FromSeconds(15));
                if (!held) return 4;
                state = LoadState();
                int rolled = Normalize(state);
                bool refresh = rolled > 0;
                if (rolled > 0) Meta(state)["status"] = "Auto-archived " + rolled + " papers from yesterday";
                switch (action)
                {
                    case "Startup":
                        bool guarded = ConsumeGuard();
                        SyncArxiv(state, false, "");
                        Save(state);
                        refresh |= Render(state) && !guarded;
                        break;
                    case "Rollover": refresh |= Render(state); break;
                    case "Refresh":
                        SyncArxiv(state, true, "");
                        Save(state); Render(state); refresh = true; break;
                    case "Render": Render(state); break;
                    case "Delete": Delete(state, id, ref refresh); break;
                    case "Toggle": Toggle(state, id, ref refresh); break;
                    case "Open": Open(state, id, ref refresh); break;
                    case "ClearArxiv":
                        Tasks(state).RemoveAll(t => S(t, "source") == "arxiv");
                        Meta(state)["last_arxiv_sync_date"] = "";
                        Meta(state)["status"] = "Cleared paper tasks";
                        Commit(state); refresh = true; break;
                    case "SyncArxiv":
                        SyncArxiv(state, true, ""); Commit(state); refresh = true; break;
                }
                if (refresh) Refresh();
                return 0;
            }
            catch (Exception ex)
            {
                if (state != null)
                {
                    Meta(state)["status"] = "Operation failed: " + ex.Message;
                    try { Commit(state); Refresh(); } catch { }
                }
                return 1;
            }
            finally { if (held) mutex.ReleaseMutex(); }
        }
    }





    private static string TimeLabel(Dictionary<string, object> task, DateTimeOffset now)
    {
        DateTimeOffset? due = RuntimeUtil.Date(task, "due_at"), available = RuntimeUtil.Date(task, "available_from");
        if (due.HasValue && now > due.Value) return "Overdue · due " + due.Value.ToString("MMM d HH:mm");
        if (due.HasValue) return (due.Value.Date == now.Date ? "Today" : due.Value.Date == now.Date.AddDays(1) ? "Tomorrow" : due.Value.ToString("MMM d")) + " " + due.Value.ToString("HH:mm") + " due";
        return available.HasValue ? available.Value.ToString("MMM d HH:mm") + " start" : "";
    }






    private static Mutex OpenStateMutex()
    {
        try { return new Mutex(false, @"Global\RainmeterTodoState"); }
        catch (UnauthorizedAccessException) { return new Mutex(false, @"Local\RainmeterTodoState"); }
    }

    private static bool ConsumeGuard(){if(!File.Exists(GuardPath))return false;try{bool fresh=(DateTime.Now-File.GetLastWriteTime(GuardPath)).TotalSeconds<20;File.Delete(GuardPath);return fresh;}catch{return true;}}
    private static void Refresh(){File.WriteAllText(GuardPath,RuntimeUtil.Iso(DateTimeOffset.Now),RuntimeUtil.Utf8NoBom);RuntimeUtil.Refresh("Todo");string calendar=Path.GetFullPath(Path.Combine(ResourceDir,"..","..","Calendar","Calendar.ini"));if(File.Exists(calendar))RuntimeUtil.Refresh("Calendar");}
}

