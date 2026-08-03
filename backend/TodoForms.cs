using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using RainmeterBackend;

internal static partial class TodoApp
{
    private const int EmGetFirstVisibleLine = 0x00CE;
    private const int EmGetLineCount = 0x00BA;

    [DllImport("user32.dll")]
    private static extern IntPtr SendMessage(IntPtr handle, int message, IntPtr wParam, IntPtr lParam);

    private sealed class EditorResult { public string Title, Target, Note, Available, Due; public List<string> Labels; }
    private static EditorResult ShowEditor(Dictionary<string, object> task)
    {
        bool editing = task != null;
        Form f = LightUi.Form(editing ? "Edit task" : "New task", 560, 840); int x = 28, w = 504;
        LightUi.Heading(f, editing ? "Edit task" : "New task", editing ? "Adjust your task, set a clear goal, and get it done" : "Create a new task, set a clear goal, and get it done", "todo.svg");
        Button close = LightUi.Button("×", 500, 22, 34, DialogResult.Cancel); close.Height = 34; f.Controls.Add(close);

        TextBox title = Field(f, "Title *", x, 112, w, editing ? S(task, "title") : "");
        TextBox target = FieldWithButton(f, "Open target", x, 204, w, editing ? S(task, "target") : "", "Browse");
        TextBox available = DateField(f, "Start time", x, 304, 230, RuntimeUtil.Date(task, "available_from"));
        TextBox due = DateField(f, "Due time", 302, 304, 230, RuntimeUtil.Date(task, "due_at"));

        Panel shortcuts = DateShortcuts(available, due, x, 362, w);
        f.Controls.Add(shortcuts);

        HashSet<string> selectedLabels = new HashSet<string>(editing ? Labels(task) : Enumerable.Empty<string>());
        Panel labelPanel = LabelSelector("Labels", x, 420, w, CommonLabels(task), selectedLabels);
        f.Controls.Add(labelPanel);

        f.Controls.Add(LightUi.Label("Notes", x, 510, w));
        Panel noteSurface = new Panel { Left = x, Top = 532, Width = w, Height = 144, BackColor = LightUi.Panel };
        LightUi.Round(noteSurface, 10);
        TextBox note = new TextBox { Left = 14, Top = 14, Width = w - 28, Height = 116, Text = editing ? S(task, "note") : "", Multiline = true, ScrollBars = ScrollBars.Vertical, AcceptsReturn = true, BorderStyle = BorderStyle.None, BackColor = LightUi.Panel, ForeColor = LightUi.Text, Font = new Font("Microsoft YaHei UI", 10F) };
        noteSurface.Controls.Add(note); f.Controls.Add(noteSurface);
        labelPanel.BringToFront();
        Label hint = LightUi.Label("Title is required. Due time cannot be earlier than start time.", x, 682, 340); f.Controls.Add(hint);
        Button cancel = LightUi.Button("Cancel", 210, 722, 112, DialogResult.Cancel), save = LightUi.PrimaryButton(editing ? "+ Save changes" : "+ Add task", 338, 722, 194, DialogResult.OK);
        f.Controls.Add(cancel); f.Controls.Add(save); f.AcceptButton = save; f.CancelButton = cancel;
        while (f.ShowDialog() == DialogResult.OK)
        {
            if (String.IsNullOrWhiteSpace(title.Text)) { LightUi.Error("Title cannot be empty"); continue; }
            DateTimeOffset a = default(DateTimeOffset), d = default(DateTimeOffset); string av = "", du = "";
            if (!String.IsNullOrWhiteSpace(available.Text) && !TryEditorDate(available.Text, out a)) { LightUi.Error("Start time format should be YYYY-MM-DD HH:mm"); continue; } else if (!String.IsNullOrWhiteSpace(available.Text)) av = RuntimeUtil.Iso(a);
            if (!String.IsNullOrWhiteSpace(due.Text) && !TryEditorDate(due.Text, out d)) { LightUi.Error("Due time format should be YYYY-MM-DD HH:mm"); continue; } else if (!String.IsNullOrWhiteSpace(due.Text)) du = RuntimeUtil.Iso(d);
            if (av != "" && du != "" && d < a) { LightUi.Error("Due time cannot be earlier than start time"); continue; }
            return new EditorResult { Title = title.Text.Trim(), Target = target.Text.Trim(), Note = note.Text, Available = av, Due = du, Labels = selectedLabels.Where(v => v != "").Distinct().ToList() };
        }
        return null;
    }

    private static void ShowSettings()
    {
        Dictionary<string, object> credentials = ReadTranslationCredentials();
        PaperSettings settings = LoadPaperSettings();
        Form f = LightUi.Form("Todo settings", 930, 760);
        LightUi.Heading(f, "Todo settings", "Configure paper recommendations, DeepSeek scoring, file sync, title translation, and version updates.", "settings.svg");
        Button close = LightUi.Button("×", 870, 22, 34, DialogResult.Cancel);
        close.Height = 34;
        f.Controls.Add(close);

        CheckBox enabled = new CheckBox { Left = 28, Top = 102, Width = 220, Height = 28, Text = "Enable paper recommendations", Checked = settings.Enabled, ForeColor = LightUi.Text, BackColor = Color.Transparent, Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold) };
        f.Controls.Add(enabled);

        string[] pageNames = { "Papers", "Google Tasks", "DeepSeek API", "Filter & Score", "File Sync", "Translation", "About" };
        Panel navigation = new Panel { Left = 28, Top = 148, Width = 150, Height = 492, BackColor = Color.FromArgb(235, 245, 253) };
        LightUi.Round(navigation, 12);
        Panel content = new Panel { Left = 194, Top = 148, Width = 708, Height = 492, BackColor = Color.Transparent };
        f.Controls.AddRange(new Control[] { navigation, content });
        List<Button> tabs = new List<Button>();
        List<Panel> pages = new List<Panel>();
        for (int i = 0; i < pageNames.Length; i++)
        {
            Button tab = LightUi.Button(pageNames[i], 8, 8 + i * 56, 134, DialogResult.None);
            tab.Height = 46; tab.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
            navigation.Controls.Add(tab); tabs.Add(tab);
            Panel page = new Panel { Left = 0, Top = 0, Width = 708, Height = 492, BackColor = Color.Transparent, AutoScroll = true, Visible = false };
            content.Controls.Add(page); pages.Add(page);
        }

        int w = 660;
        TextBox importCount = Field(pages[0], "Papers to import per day (1-20)", 12, 12, 310, settings.ImportCount.ToString(CultureInfo.InvariantCulture));
        TextBox cacheDays = Field(pages[0], "Cache retention days (1-90)", 342, 12, 306, settings.CacheDays.ToString(CultureInfo.InvariantCulture));
        Panel jobCard = new Panel { Left = 12, Top = 126, Width = 636, Height = 104, BackColor = LightUi.Surface };
        LightUi.Round(jobCard, 10);
        Label jobState = new Label { Left = 14, Top = 10, Width = 522, Height = 44, Text = "Status: no background scoring job", ForeColor = LightUi.Text, BackColor = Color.Transparent, Font = new Font("Microsoft YaHei UI", 10F) };
        Label jobPercent = new Label { Left = 542, Top = 10, Width = 78, Height = 28, Text = "0%", TextAlign = ContentAlignment.TopRight, ForeColor = LightUi.Accent, BackColor = Color.Transparent, Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold) };
        Panel progressTrack = new Panel { Left = 14, Top = 72, Width = 608, Height = 12, BackColor = Color.FromArgb(211, 228, 242) };
        Panel progressFill = new Panel { Left = 0, Top = 0, Width = 0, Height = 12, BackColor = LightUi.AccentFill };
        LightUi.Round(progressTrack, 6);
        LightUi.Round(progressFill, 6);
        progressTrack.Controls.Add(progressFill);
        jobCard.Controls.AddRange(new Control[] { jobState, jobPercent, progressTrack });
        Label generalHint = LightUi.Label("On startup, only reads local or remote complete files between 08:00-20:00; DeepSeek is only called after a manual refresh and confirmation.", 12, 246, 636);
        generalHint.Height = 48;
        Label defaultsHint = LightUi.Label("Non-sensitive settings have built-in defaults; API keys and server credentials must be entered by you. Updates and deploys preserve saved settings.", 12, 306, 636);
        defaultsHint.Height = 42;
        Label rescoreHint = LightUi.Label("After changing filters, thresholds, or prompts, you can re-fetch and re-score with the current settings.", 12, 380, 430);
        rescoreHint.Height = 40;
        Button rescore = LightUi.PrimaryButton("Re-fetch and score", 472, 372, 176, DialogResult.None);
        pages[0].Controls.AddRange(new Control[] { jobCard, generalHint, defaultsHint, rescoreHint, rescore });

        TextBox apiUrl = Field(pages[2], "Chat Completions URL", 12, 12, w, settings.ApiBaseUrl);
        TextBox apiModel = Field(pages[2], "Model", 12, 106, w, settings.Model);
        TextBox apiKey = PasswordField(pages[2], "API Key", 12, 200, w, settings.ApiKey);
        TextBox concurrency = Field(pages[2], "Max concurrency (1-32, default 8)", 12, 294, 310, settings.MaxConcurrency.ToString(CultureInfo.InvariantCulture));
        TextBox timeout = Field(pages[2], "Request timeout seconds (30-600)", 342, 294, 330, settings.TimeoutSeconds.ToString(CultureInfo.InvariantCulture));
        Button testApi = LightUi.Button("Test DeepSeek", 502, 408, 170, DialogResult.None);
        pages[2].Controls.Add(testApi);

        TextBox categories = Field(pages[3], "Include categories (comma-separated; empty = all CS categories)", 12, 12, w, settings.Categories);
        TextBox excludes = Field(pages[3], "Exclude categories (comma-separated; empty = none)", 12, 106, w, settings.ExcludeCategories);
        TextBox threshold = Field(pages[3], "Title threshold for abstract scoring (0-10)", 12, 200, 206, settings.TitleThreshold.ToString(CultureInfo.InvariantCulture));
        TextBox titleBatch = Field(pages[3], "Title batch size (1-50)", 230, 200, 206, settings.TitleBatchSize.ToString(CultureInfo.InvariantCulture));
        TextBox abstractBatch = Field(pages[3], "Abstract batch size (1-20)", 448, 200, 224, settings.AbstractBatchSize.ToString(CultureInfo.InvariantCulture));
        TextBox titlePrompt = PromptField(pages[3], "Title scoring prompt", 12, 294, w, 160, settings.TitlePrompt);
        TextBox abstractPrompt = PromptField(pages[3], "Abstract scoring prompt", 12, 490, w, 190, settings.AbstractPrompt);
        pages[3].AutoScrollMinSize = new Size(0, 710);

        CheckBox fileEnabled = new CheckBox { Left = 12, Top = 12, Width = 220, Height = 26, Text = "Enable file server sync", Checked = settings.FileServerEnabled, ForeColor = LightUi.Text, BackColor = Color.Transparent, Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold) };
        TextBox fileUrl = Field(pages[4], "File Browser URL", 12, 58, w, settings.FileBaseUrl);
        TextBox fileAccount = Field(pages[4], "Account", 12, 152, w, settings.FileAccount);
        TextBox filePassword = PasswordField(pages[4], "Password", 12, 246, w, settings.FilePassword);
        Button testFile = LightUi.Button("Test file server", 502, 374, 170, DialogResult.None);
        pages[4].Controls.AddRange(new Control[] { fileEnabled, testFile });

        TextBox secretId = Field(pages[5], "Tencent Cloud SecretId", 12, 12, w, S(credentials, "SecretId"));
        TextBox secretKey = PasswordField(pages[5], "Tencent Cloud SecretKey", 12, 106, w, S(credentials, "SecretKey"));
        Label translationStatus = LightUi.Label(File.Exists(TranslationSecret) ? "Translation credentials saved" : "No translation credentials configured; paper titles stay in English.", 12, 214, 636);
        Button clearTranslation = LightUi.DangerButton("Clear translation", 350, 266, 140, DialogResult.None);
        Button testTranslation = LightUi.Button("Test translation", 502, 266, 140, DialogResult.None);
        pages[5].Controls.AddRange(new Control[] { translationStatus, clearTranslation, testTranslation });

        Label aboutTitle = new Label { Text = "Rainmeter Desktop Widgets", Left = 12, Top = 18, Width = 636, Height = 36, ForeColor = LightUi.Text, BackColor = Color.Transparent, Font = new Font("Microsoft YaHei UI", 15F, FontStyle.Bold) };
        Label aboutVersion = new Label { Text = "Version: " + AppVersion + " (" + AppEditionName + ")", Left = 12, Top = 72, Width = 636, Height = 26, ForeColor = LightUi.Text, BackColor = Color.Transparent, Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold) };
        Label aboutRepo = LightUi.Label("Source: github.com/MHJoy99/Rainmeter-Todo-Skin", 12, 112, 636);
        Label updateStatus = LightUi.Label("Not checked for updates", 12, 158, 420);
        Button checkUpdate = LightUi.PrimaryButton("Check for updates", 502, 146, 146, DialogResult.None);
        Label uiScaleLabel = LightUi.Label("UI scale (tile and windows)", 12, 214, 300);
        string[] uiScaleLabels = { "Auto (currently " + UiScale.Percent + "%)", "50% (Tiny)", "75% (Small)", "90% (Compact)", "100% (Normal)", "110% (Medium)", "125% (Large)", "150% (Extra Large)", "175% (Huge)", "200% (Massive)" };
        string[] uiScaleValues = { "auto", "0.50", "0.75", "0.90", "1.00", "1.10", "1.25", "1.50", "1.75", "2.00" };
        ComboBox uiScale = new ComboBox { Left = 12, Top = 244, Width = 260, Height = 36, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Microsoft YaHei UI", 10F) };
        uiScale.Items.AddRange(uiScaleLabels.Cast<object>().ToArray());
        string currentUiScaleMode = UiScale.Mode;
        int currentUiScaleIndex = Array.FindIndex(uiScaleValues, value => String.Equals(value, currentUiScaleMode, StringComparison.OrdinalIgnoreCase));
        uiScale.SelectedIndex = currentUiScaleIndex < 0 ? 0 : currentUiScaleIndex;
        Button applyUiScale = LightUi.PrimaryButton("Apply scale", 292, 242, 128, DialogResult.None);
        Label uiScaleHint = LightUi.Label("The scale controls both the tile and windows; windows also adapt to Windows display scaling. Reopen the window after applying.", 12, 296, 620);
        uiScaleHint.Height = 48;
        pages[6].Controls.AddRange(new Control[] { aboutTitle, aboutVersion, aboutRepo, updateStatus, checkUpdate, uiScaleLabel, uiScale, applyUiScale, uiScaleHint });

        Label googleStatus = LightUi.Label(GoogleTasksSignedIn() ? "Signed in to Google Tasks" : "Not signed in to Google Tasks", 12, 12, 636);
        googleStatus.Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold);
        Label googleHint = LightUi.Label("Clicking a todo without a custom link creates a real Google Task. Sign in once below and every new task is pushed to your Google account. Your token is stored encrypted with Windows DPAPI; you can replace the built-in OAuth client by placing your own gtasks-client.json in @Resources.", 12, 60, 636);
        googleHint.Height = 110;
        Button googleSignIn = LightUi.PrimaryButton("Sign in with Google", 12, 196, 190, DialogResult.None);
        Button googleSignOut = LightUi.Button("Sign out", 216, 196, 110, DialogResult.None);
        Label googleSecure = LightUi.Label("Uses the official Google Tasks API with OAuth 2.0 (scope: https://www.googleapis.com/auth/tasks).", 12, 258, 636);
        pages[1].Controls.AddRange(new Control[] { googleStatus, googleHint, googleSignIn, googleSignOut, googleSecure });

        googleSignIn.Click += delegate {
            try
            {
                googleSignIn.Enabled = false;
                googleSignIn.Text = "Waiting for authorization...";
                googleSignIn.Refresh();
                Application.DoEvents();
                string message = GoogleTasksSignIn();
                googleStatus.Text = "Signed in to Google Tasks";
                googleStatus.ForeColor = Color.FromArgb(63, 178, 119);
            }
            catch (Exception ex)
            {
                googleStatus.Text = "Sign-in failed: " + ex.Message;
                googleStatus.ForeColor = LightUi.Danger;
                LightUi.Error("Google Tasks sign-in failed: " + ex.Message);
            }
            finally { googleSignIn.Enabled = true; googleSignIn.Text = "Sign in with Google"; }
        };

        googleSignOut.Click += delegate {
            try
            {
                if (!GoogleTasksSignedIn()) return;
                if (!LightUi.Confirm("Remove the saved Google account and sign out?", "Sign out of Google Tasks")) return;
                GoogleTasksSignOut();
                googleStatus.Text = "Not signed in to Google Tasks";
                googleStatus.ForeColor = LightUi.Muted;
            }
            catch (Exception ex) { LightUi.Error(ex.Message); }
        };

        Label saveStatus = LightUi.Label(File.Exists(PaperSyncSecret) ? "Settings saved" : "Settings not saved", 194, 666, 580);
        saveStatus.Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold);
        Button saveAll = LightUi.PrimaryButton("Save settings", 790, 654, 112, DialogResult.None);
        f.Controls.AddRange(new Control[] { saveStatus, saveAll });

        Action<int> showPage = delegate(int selected) {
            for (int i = 0; i < pages.Count; i++) { pages[i].Visible = i == selected; PaintTabButton(tabs[i], i == selected); }
        };
        for (int i = 0; i < tabs.Count; i++) { int selected = i; tabs[i].Click += delegate { showPage(selected); }; }
        showPage(0);

        applyUiScale.Click += delegate {
            try
            {
                applyUiScale.Enabled = false;
                UiScale.SaveMode(uiScaleValues[Math.Max(0, uiScale.SelectedIndex)]);
                RenderUiScaleSkins();
                uiScaleLabels[0] = "Auto (currently " + UiScale.Percent + "%)";
                uiScale.Items[0] = uiScaleLabels[0];
                saveStatus.Text = "UI scale applied; reopen the window to see it";
                saveStatus.ForeColor = Color.FromArgb(63, 178, 119);
            }
            catch (Exception ex)
            {
                saveStatus.Text = "Failed to apply UI scale";
                saveStatus.ForeColor = LightUi.Danger;
                LightUi.Error(ex.Message);
            }
            finally { applyUiScale.Enabled = true; }
        };

        Action refreshPaperProgress = delegate {
            string message = "No background scoring job";
            string state = "";
            int completed = 0;
            int total = 0;
            try
            {
                if (File.Exists(PaperJobPath))
                {
                    Dictionary<string, object> job = JsonUtil.LoadObject(PaperJobPath);
                    message = JsonUtil.String(job, "message", message);
                    state = JsonUtil.String(job, "state", "");
                    completed = Math.Max(0, JsonUtil.Int(job, "completed", 0));
                    total = Math.Max(0, JsonUtil.Int(job, "total", 0));
                }
            }
            catch { }
            int percent = total > 0 ? Math.Max(0, Math.Min(100, (int)Math.Round(completed * 100D / total))) : 0;
            if (state == "completed") percent = 100;
            jobState.Text = "Status: " + message;
            jobPercent.Text = percent.ToString(CultureInfo.InvariantCulture) + "%";
            progressFill.Width = Math.Max(0, Math.Min(progressTrack.ClientSize.Width, progressTrack.ClientSize.Width * percent / 100));
            Color progressColor = state == "failed" ? LightUi.Danger : state == "completed" ? Color.FromArgb(63, 178, 119) : LightUi.AccentFill;
            progressFill.BackColor = progressColor;
            jobPercent.ForeColor = progressColor;
            rescore.Enabled = enabled.Checked && !IsPaperJobRunning();
        };
        refreshPaperProgress();
        System.Windows.Forms.Timer paperProgressTimer = new System.Windows.Forms.Timer { Interval = 500 };
        paperProgressTimer.Tick += delegate { refreshPaperProgress(); };
        paperProgressTimer.Start();

        Action updatePaperEnabled = delegate {
            for (int i = 0; i < pages.Count; i++)
            {
                if (i == 1) continue;
                SetChildrenEnabled(pages[i], enabled.Checked);
            }
            enabled.Enabled = true;
            refreshPaperProgress();
        };
        enabled.CheckedChanged += delegate { updatePaperEnabled(); };
        updatePaperEnabled();

        Func<PaperSettings> collect = delegate {
            PaperSettings value = new PaperSettings();
            value.Enabled = enabled.Checked;
            value.ApiBaseUrl = apiUrl.Text;
            value.ApiKey = apiKey.Text;
            value.Model = apiModel.Text;
            value.MaxConcurrency = ParseSettingInt(concurrency.Text, 1, 32, "Max concurrency");
            value.TimeoutSeconds = ParseSettingInt(timeout.Text, 30, 600, "Request timeout");
            value.FileServerEnabled = fileEnabled.Checked;
            value.FileBaseUrl = fileUrl.Text;
            value.FileAccount = fileAccount.Text;
            value.FilePassword = filePassword.Text;
            value.Categories = categories.Text;
            value.ExcludeCategories = excludes.Text;
            value.TitleThreshold = ParseSettingInt(threshold.Text, 0, 10, "Title threshold");
            value.TitleBatchSize = ParseSettingInt(titleBatch.Text, 1, 50, "Title batch size");
            value.AbstractBatchSize = ParseSettingInt(abstractBatch.Text, 1, 20, "Abstract batch size");
            value.TitlePrompt = titlePrompt.Text;
            value.AbstractPrompt = abstractPrompt.Text;
            value.ImportCount = ParseSettingInt(importCount.Text, 1, 20, "Import count");
            value.CacheDays = ParseSettingInt(cacheDays.Text, 1, 90, "Cache days");
            return value;
        };

        saveAll.Click += delegate {
            try
            {
                PaperSettings value = collect();
                SavePaperSettings(value);
                if (!String.IsNullOrWhiteSpace(secretId.Text) || !String.IsNullOrWhiteSpace(secretKey.Text))
                    SaveTranslationCredentials(secretId.Text, secretKey.Text);
                saveStatus.Text = "Settings saved";
                saveStatus.ForeColor = Color.FromArgb(63, 178, 119);
            }
            catch (Exception ex) { saveStatus.Text = "Save failed"; saveStatus.ForeColor = LightUi.Danger; LightUi.Error(ex.Message); }
        };
        rescore.Click += delegate {
            try
            {
                rescore.Enabled = false;
                PaperSettings value = collect();
                if (StartPaperRescore(value))
                {
                    saveStatus.Text = "Settings saved; rescoring started";
                    saveStatus.ForeColor = Color.FromArgb(63, 178, 119);
                    refreshPaperProgress();
                }
            }
            catch (Exception ex)
            {
                saveStatus.Text = "Rescoring not started";
                saveStatus.ForeColor = LightUi.Danger;
                LightUi.Error(ex.Message);
            }
            finally { refreshPaperProgress(); }
        };
        testApi.Click += delegate {
            try { testApi.Enabled = false; testApi.Text = "Testing..."; Application.DoEvents(); TestDeepSeekConnection(collect()); saveStatus.Text = "DeepSeek test succeeded"; saveStatus.ForeColor = Color.FromArgb(63, 178, 119); }
            catch (Exception ex) { LightUi.Error("DeepSeek test failed: " + ex.Message); }
            finally { testApi.Enabled = true; testApi.Text = "Test DeepSeek"; }
        };
        testFile.Click += delegate {
            try { TestFileServerConnection(collect()); saveStatus.Text = "File server login succeeded"; saveStatus.ForeColor = Color.FromArgb(63, 178, 119); }
            catch (Exception ex) { LightUi.Error("File server test failed: " + ex.Message); }
        };
        testTranslation.Click += delegate {
            try { string result = TestTranslationCredentials(secretId.Text, secretKey.Text); translationStatus.Text = "Connected: " + result; translationStatus.ForeColor = Color.FromArgb(63, 178, 119); }
            catch (Exception ex) { LightUi.Error("Translation test failed: " + ex.Message); }
        };
        clearTranslation.Click += delegate {
            try
            {
                if (File.Exists(TranslationSecret)) File.Delete(TranslationSecret);
                secretId.Text = "";
                secretKey.Text = "";
                translationStatus.Text = "No translation credentials; paper titles will stay in English";
                translationStatus.ForeColor = LightUi.Muted;
            }
            catch (Exception ex) { LightUi.Error(ex.Message); }
        };

        checkUpdate.Click += delegate {
            try
            {
                checkUpdate.Enabled = false;
                updateStatus.Text = "Checking GitHub...";
                updateStatus.ForeColor = LightUi.Muted;
                updateStatus.Refresh();
                Application.DoEvents();
                UpdateCheckResult info = CheckLatestUpdate();
                if (!info.IsNewer)
                {
                    updateStatus.Text = info.CompareResult == 0 ? "Already up to date: " + info.Tag : "Current version is newer than latest tag: " + info.Tag;
                    updateStatus.ForeColor = Color.FromArgb(63, 178, 119);
                    return;
                }
                updateStatus.Text = "New version found: " + info.Tag;
                updateStatus.ForeColor = LightUi.Accent;
                DialogResult update = MessageBox.Show(
                    "New version " + info.Tag + " (Unified) found.\r\n\r\nDownload and deploy now? The deploy script will restart Rainmeter.",
                    "Check for updates",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                if (update != DialogResult.Yes)
                {
                    updateStatus.Text = "Update cancelled: " + info.Tag;
                    updateStatus.ForeColor = LightUi.Muted;
                    return;
                }
                updateStatus.Text = "Starting standalone updater...";
                updateStatus.ForeColor = LightUi.Muted;
                updateStatus.Refresh();
                Application.DoEvents();
                StartExternalUpdater();
                updateStatus.Text = "Standalone updater started";
                updateStatus.ForeColor = Color.FromArgb(63, 178, 119);
                f.BeginInvoke(new Action(f.Close));
            }
            catch (Exception ex)
            {
                updateStatus.Text = "Failed to check for updates";
                updateStatus.ForeColor = LightUi.Danger;
                LightUi.Error("Failed to check for updates: " + ex.Message);
            }
            finally { checkUpdate.Enabled = true; }
        };

        f.CancelButton = close;
        f.ShowDialog();
        paperProgressTimer.Stop();
        paperProgressTimer.Dispose();
    }

    private static void RenderUiScaleSkins()
    {
        string todoExe = Application.ExecutablePath;
        using (Process todo = Process.Start(new ProcessStartInfo(todoExe, "Render") { UseShellExecute = false, CreateNoWindow = true }))
        {
            if (todo != null && !todo.WaitForExit(15000)) throw new Exception("Todo tile refresh timed out");
            if (todo != null && todo.ExitCode != 0) throw new Exception("Todo tile refresh failed");
        }
        string calendarExe = Path.GetFullPath(Path.Combine(ResourceDir, "..", "..", "Calendar", "@Resources", "CalendarHost.exe"));
        if (File.Exists(calendarExe))
        {
            using (Process calendar = Process.Start(new ProcessStartInfo(calendarExe, "Render") { UseShellExecute = false, CreateNoWindow = true }))
            {
                if (calendar != null && !calendar.WaitForExit(15000)) throw new Exception("Calendar tile reload timed out");
                if (calendar != null && calendar.ExitCode != 0) throw new Exception("Calendar tile refresh failed");
            }
        }
        // Both generated includes are ready at this point. Refresh the Rainmeter
        // app once so the two tiles cannot remain on different/previous scales.
        RuntimeUtil.RefreshAll();
    }

    private static string ShowPaperScoringConsent(string message)
    {
        Form form = LightUi.Form("Local paper scoring", 560, 300);
        LightUi.Heading(form, "Use DeepSeek scoring?", "", "ai-score.svg");
        Label detail = new Label {
            Left = 28, Top = 86, Width = 504, Height = 118,
            Text = message, ForeColor = LightUi.Text, BackColor = LightUi.Surface,
            Padding = new Padding(14), Font = new Font("Microsoft YaHei UI", 9.5F)
        };
        LightUi.Round(detail, 10);
        Button skipToday = LightUi.Button("Don't ask today", 188, 226, 140, DialogResult.None);
        Button cancel = LightUi.Button("Cancel", 340, 226, 80, DialogResult.None);
        Button use = LightUi.PrimaryButton("Use", 432, 226, 100, DialogResult.None);
        use.TextAlign = ContentAlignment.MiddleCenter;
        use.Padding = new Padding(0, 3, 0, 0);
        string result = "cancel";
        skipToday.Click += delegate { result = "skip_today"; form.Close(); };
        cancel.Click += delegate { result = "cancel"; form.Close(); };
        use.Click += delegate { result = "use"; form.Close(); };
        form.Controls.AddRange(new Control[] { detail, skipToday, cancel, use });
        form.CancelButton = cancel;
        form.ShowDialog();
        return result;
    }

    private static bool ShowPaperRescoreConsent()
    {
        Form form = LightUi.Form("Re-fetch and score", 560, 300);
        LightUi.Heading(form, "Re-fetch and score?", "", "ai-score.svg");
        Label detail = new Label {
            Left = 28, Top = 86, Width = 504, Height = 118,
            Text = "This will save the current paper settings, clear today's local paper cache, and call DeepSeek again, which may incur costs.\r\n\r\nToday's existing paper recommendations will only be replaced once the new scoring completes successfully.",
            ForeColor = LightUi.Text, BackColor = LightUi.Surface,
            Padding = new Padding(14), Font = new Font("Microsoft YaHei UI", 9.5F)
        };
        LightUi.Round(detail, 10);
        Button cancel = LightUi.Button("Cancel", 340, 226, 80, DialogResult.None);
        Button start = LightUi.PrimaryButton("Rescore", 432, 226, 100, DialogResult.None);
        start.TextAlign = ContentAlignment.MiddleCenter;
        start.Padding = new Padding(0, 3, 0, 0);
        bool confirmed = false;
        cancel.Click += delegate { form.Close(); };
        start.Click += delegate { confirmed = true; form.Close(); };
        form.Controls.AddRange(new Control[] { detail, cancel, start });
        form.CancelButton = cancel;
        form.ShowDialog();
        return confirmed;
    }

    private static bool ShowPaperOverwriteConsent(string fileName)
    {
        Form form = LightUi.Form("Remote paper file exists", 560, 300);
        LightUi.Heading(form, "Overwrite remote file?", "", "ai-score.svg");
        Label detail = new Label {
            Left = 28, Top = 86, Width = 504, Height = 118,
            Text = "The file server already contains:\r\n" + fileName + "\r\n\r\nOverwrite the remote file with this local scoring result?",
            ForeColor = LightUi.Text, BackColor = LightUi.Surface,
            Padding = new Padding(14), Font = new Font("Microsoft YaHei UI", 9.5F)
        };
        LightUi.Round(detail, 10);
        Button keep = LightUi.Button("Keep", 340, 226, 80, DialogResult.None);
        Button overwrite = LightUi.PrimaryButton("Overwrite", 432, 226, 100, DialogResult.None);
        overwrite.TextAlign = ContentAlignment.MiddleCenter;
        overwrite.Padding = new Padding(0, 3, 0, 0);
        bool confirmed = false;
        keep.Click += delegate { form.Close(); };
        overwrite.Click += delegate { confirmed = true; form.Close(); };
        form.Controls.AddRange(new Control[] { detail, keep, overwrite });
        form.CancelButton = keep;
        form.ShowDialog();
        return confirmed;
    }

    private static int ParseSettingInt(string text, int minimum, int maximum, string name)
    {
        int value;
        if (!Int32.TryParse((text ?? "").Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out value) || value < minimum || value > maximum)
            throw new Exception(name + " must be an integer between " + minimum + " and " + maximum);
        return value;
    }

    private static void SetChildrenEnabled(Control parent, bool enabled)
    {
        foreach (Control child in parent.Controls)
        {
            child.Enabled = enabled;
            if (child.HasChildren) SetChildrenEnabled(child, enabled);
        }
    }

    private static TextBox PromptField(ScrollableControl parent, string label, int x, int y, int width, int height, string text)
    {
        parent.Controls.Add(LightUi.Label(label, x, y, width));
        Button insert = LightUi.PrimaryButton("Insert paper at cursor", x + width - 164, y - 4, 164, DialogResult.None);
        insert.Height = 30;
        Panel surface = new Panel { Left = x, Top = y + 26, Width = width, Height = height, BackColor = LightUi.Panel };
        LightUi.Round(surface, 10);
        TextBox box = new TextBox { Left = 14, Top = 12, Width = width - 28, Height = height - 24, Text = text ?? "", Multiline = true, ScrollBars = ScrollBars.Vertical, AcceptsReturn = true, BorderStyle = BorderStyle.None, BackColor = LightUi.Panel, ForeColor = LightUi.Text, Font = new Font("Microsoft YaHei UI", 9F) };
        insert.Click += delegate {
            int caret = Math.Max(0, Math.Min(box.SelectionStart, box.TextLength));
            string value = box.Text;
            int found;
            while ((found = value.IndexOf(PaperListPlaceholder, StringComparison.Ordinal)) >= 0)
            {
                value = value.Remove(found, PaperListPlaceholder.Length);
                if (found < caret) caret = Math.Max(found, caret - PaperListPlaceholder.Length);
            }
            box.Text = value.Insert(caret, PaperListPlaceholder);
            box.SelectionStart = caret + PaperListPlaceholder.Length;
            box.SelectionLength = 0;
            box.Focus();
        };
        box.MouseWheel += delegate(object sender, MouseEventArgs e) {
            HandledMouseEventArgs handled = e as HandledMouseEventArgs;
            int firstVisibleLine = SendMessage(box.Handle, EmGetFirstVisibleLine, IntPtr.Zero, IntPtr.Zero).ToInt32();
            int lineCount = Math.Max(1, SendMessage(box.Handle, EmGetLineCount, IntPtr.Zero, IntPtr.Zero).ToInt32());
            int visibleLines = Math.Max(1, box.ClientSize.Height / Math.Max(1, box.Font.Height));
            bool atTop = firstVisibleLine <= 0;
            bool atBottom = firstVisibleLine + visibleLines >= lineCount;
            bool forwardToOuter = (e.Delta > 0 && atTop) || (e.Delta < 0 && atBottom);
            if (!forwardToOuter) return;
            if (handled != null) handled.Handled = true;
            int current = Math.Max(0, -parent.AutoScrollPosition.Y);
            int maximum = Math.Max(0, parent.DisplayRectangle.Height - parent.ClientSize.Height);
            int target = Math.Max(0, Math.Min(maximum, current - e.Delta));
            parent.AutoScrollPosition = new Point(0, target);
        };
        surface.Controls.Add(box); parent.Controls.Add(surface); parent.Controls.Add(insert);
        insert.BringToFront();
        return box;
    }

    private static IEnumerable<string> CommonLabels(Dictionary<string, object> task)
    {
        string[] defaults = { "Work", "Personal", "Important", "Errand", "Study", "Health", "Home" };
        return defaults.Concat(task == null ? Enumerable.Empty<string>() : Labels(task)).Where(x => !String.IsNullOrWhiteSpace(x)).Distinct();
    }

    private static Panel LabelSelector(string title, int x, int y, int width, IEnumerable<string> options, HashSet<string> selected)
    {
        Panel panel = new Panel { Left = x, Top = y, Width = width, Height = 86, BackColor = Color.Transparent };
        panel.Controls.Add(LightUi.Label(title, 0, 0, width));
        Panel surface = new Panel { Left = 0, Top = 28, Width = width, Height = 56, BackColor = LightUi.Panel };
        LightUi.Round(surface, 10);
        panel.Controls.Add(surface);
        Button expand = LightUi.Button("Expand", width - 90, 12, 80, DialogResult.None);
        expand.Height = 32;
        expand.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
        expand.UseVisualStyleBackColor = false;
        expand.BackColor = LightUi.AccentFill;
        expand.ForeColor = Color.White;
        expand.FlatAppearance.BorderColor = Color.FromArgb(31, 103, 201);
        expand.FlatAppearance.MouseOverBackColor = Color.FromArgb(31, 116, 224);
        expand.FlatAppearance.MouseDownBackColor = Color.FromArgb(22, 88, 176);
        expand.MouseEnter += delegate { if (expand.Enabled) expand.BackColor = Color.FromArgb(31, 116, 224); };
        expand.MouseLeave += delegate { expand.BackColor = LightUi.AccentFill; };
        surface.Controls.Add(expand);
        HashSet<Button> firstRowChips = new HashSet<Button>();
        int left = 12, top = 13;
        foreach (string label in options)
        {
            int buttonWidth = Math.Max(58, Math.Min(104, TextRenderer.MeasureText(label, new Font("Microsoft YaHei UI", 9F)).Width + 28));
            if (left + buttonWidth > width - 90) { left = 12; top += 32; }
            Button button = LightUi.Button(label, left, top, buttonWidth, DialogResult.None);
            button.Height = 28;
            button.Tag = label;
            button.Visible = top == 13;
            if (top == 13) firstRowChips.Add(button);
            PaintLabelChoice(button, selected.Contains(label));
            button.Click += delegate(object sender, EventArgs e) {
                Button current = (Button)sender;
                string value = Convert.ToString(current.Tag);
                if (selected.Contains(value)) selected.Remove(value); else selected.Add(value);
                PaintLabelChoice(current, selected.Contains(value));
            };
            button.MouseEnter += delegate(object sender, EventArgs e) {
                Button current = (Button)sender;
                PaintLabelChoice(current, selected.Contains(Convert.ToString(current.Tag)));
            };
            button.MouseLeave += delegate(object sender, EventArgs e) {
                Button current = (Button)sender;
                PaintLabelChoice(current, selected.Contains(Convert.ToString(current.Tag)));
            };
            surface.Controls.Add(button);
            left += buttonWidth + 8;
        }
        int expandedSurfaceLogicalHeight = Math.Max(94, surface.Controls.Cast<Control>().Where(control => control != expand).Select(control => control.Bottom).DefaultIfEmpty(42).Max() + 12);
        int expandedPanelLogicalHeight = expandedSurfaceLogicalHeight + 32;
        bool expanded = false;
        expand.Click += delegate {
            expanded = !expanded;
            surface.Height = UiScale.Logical(surface, expanded ? expandedSurfaceLogicalHeight : 56);
            panel.Height = UiScale.Logical(panel, expanded ? expandedPanelLogicalHeight : 86);
            expand.Text = expanded ? "Collapse" : "Expand";
            expand.BackColor = LightUi.AccentFill;
            expand.ForeColor = Color.White;
            if (expanded) panel.BringToFront();
            foreach (Control control in surface.Controls)
            {
                Button chip = control as Button;
                if (chip != null && chip != expand) chip.Visible = expanded || firstRowChips.Contains(chip);
            }
        };
        return panel;
    }

    private static void PaintLabelChoice(Button button, bool active)
    {
        button.BackColor = active ? Color.FromArgb(220, 238, 255) : LightUi.Panel;
        button.ForeColor = active ? LightUi.Accent : LightUi.Text;
        button.FlatAppearance.BorderColor = button.BackColor;
        button.FlatAppearance.BorderSize = 0;
    }

    private static TextBox Field(Control f, string label, int x, int y, int width, string text)
    {
        f.Controls.Add(LightUi.Label(label, x, y, width));
        Panel surface = new Panel { Left = x, Top = y + 26, Width = width, Height = 50, BackColor = LightUi.Panel };
        LightUi.Round(surface, 10);
        TextBox box = new TextBox { Left = 14, Top = 15, Width = width - 28, Height = 24, AutoSize = false, Text = text ?? "", BackColor = LightUi.Panel, ForeColor = LightUi.Text, BorderStyle = BorderStyle.None, Font = new Font("Microsoft YaHei UI", 10F) };
        surface.Controls.Add(box);
        f.Controls.Add(surface);
        return box;
    }

    private static TextBox PasswordField(Control f, string label, int x, int y, int width, string text)
    {
        f.Controls.Add(LightUi.Label(label, x, y, width));
        Panel surface = new Panel { Left = x, Top = y + 26, Width = width, Height = 50, BackColor = LightUi.Panel };
        LightUi.Round(surface, 10);
        TextBox box = new TextBox { Left = 14, Top = 15, Width = width - 92, Height = 24, AutoSize = false, Text = text ?? "", UseSystemPasswordChar = true, BackColor = LightUi.Panel, ForeColor = LightUi.Text, BorderStyle = BorderStyle.None, Font = new Font("Microsoft YaHei UI", 10F) };
        Button reveal = LightUi.Button("Show", width - 70, 8, 56, DialogResult.None);
        reveal.Height = 34;
        reveal.Click += delegate {
            box.UseSystemPasswordChar = !box.UseSystemPasswordChar;
            reveal.Text = box.UseSystemPasswordChar ? "Show" : "Hide";
        };
        surface.Controls.Add(box);
        surface.Controls.Add(reveal);
        f.Controls.Add(surface);
        return box;
    }

    private static TextBox FieldWithButton(Form f, string label, int x, int y, int width, string text, string buttonText)
    {
        f.Controls.Add(LightUi.Label(label, x, y, width));
        Panel surface = new Panel { Left = x, Top = y + 26, Width = width, Height = 50, BackColor = LightUi.Panel };
        LightUi.Round(surface, 10);
        TextBox box = new TextBox { Left = 14, Top = 15, Width = width - 98, Height = 24, AutoSize = false, Text = text ?? "", BackColor = LightUi.Panel, ForeColor = LightUi.Text, BorderStyle = BorderStyle.None, Font = new Font("Microsoft YaHei UI", 10F) };
        Button browse = LightUi.Button(buttonText, width - 78, 8, 64, DialogResult.None);
        browse.Height = 34;
        browse.Click += delegate {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = "Choose open target";
                dialog.CheckFileExists = false;
                dialog.CheckPathExists = true;
                dialog.Filter = "All files (*.*)|*.*";
                if (dialog.ShowDialog() == DialogResult.OK) box.Text = dialog.FileName;
            }
        };
        surface.Controls.Add(box); surface.Controls.Add(browse); f.Controls.Add(surface);
        return box;
    }

    private static TextBox SearchField(Form f, int x, int y, int width)
    {
        Panel surface = new Panel { Left = x, Top = y, Width = width, Height = 42, BackColor = LightUi.Panel };
        LightUi.Round(surface, 10);
        Label icon = new Label { Left = 12, Top = 11, Width = 22, Height = 22, Text = "\xE721", Font = new Font("Segoe Fluent Icons", 9F), ForeColor = LightUi.Muted, BackColor = Color.Transparent };
        TextBox box = new TextBox { Left = 38, Top = 12, Width = width - 50, Height = 22, AutoSize = false, Text = "", BackColor = LightUi.Panel, ForeColor = LightUi.Text, BorderStyle = BorderStyle.None, Font = new Font("Microsoft YaHei UI", 9F) };
        surface.Controls.Add(icon); surface.Controls.Add(box); f.Controls.Add(surface);
        return box;
    }

    private static TextBox DateField(Form f, string label, int x, int y, int width, DateTimeOffset? value)
    {
        f.Controls.Add(LightUi.Label(label, x, y, width));
        Panel surface = new Panel { Left = x, Top = y + 26, Width = width, Height = 50, BackColor = LightUi.Panel };
        LightUi.Round(surface, 10);
        TextBox box = new TextBox { Left = 14, Top = 15, Width = width - 58, Height = 24, AutoSize = false, Text = DateEdit(value), ReadOnly = true, BackColor = LightUi.Panel, ForeColor = LightUi.Text, BorderStyle = BorderStyle.None, Font = new Font("Microsoft YaHei UI", 10F) };
        Button choose = LightUi.Button("\xE787", width - 42, 8, 30, DialogResult.None);
        choose.Height = 34;
        choose.Font = new Font("Segoe Fluent Icons", 9F);
        choose.Click += delegate {
            string picked = PickDateTime(box.Text);
            if (picked != null) box.Text = picked;
        };
        surface.Controls.Add(box); surface.Controls.Add(choose); f.Controls.Add(surface);
        return box;
    }

    private static Panel DateShortcuts(TextBox available, TextBox due, int x, int y, int width)
    {
        Panel panel = new Panel { Left = x, Top = y, Width = width, Height = 46, BackColor = Color.Transparent };
        string[] labels = { "Today 5pm", "Tomorrow 9am", "+1 Hour", "Clear Due" };
        Action[] actions = {
            delegate { due.Text = DateTime.Today.AddHours(17).ToString("yyyy-MM-dd HH:mm"); },
            delegate { due.Text = DateTime.Today.AddDays(1).AddHours(9).ToString("yyyy-MM-dd HH:mm"); },
            delegate { DateTime cur; if (!DateTime.TryParseExact(due.Text, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out cur)) cur = DateTime.Now; due.Text = cur.AddHours(1).ToString("yyyy-MM-dd HH:mm"); },
            delegate { due.Text = ""; }
        };
        int curX = 0;
        for (int i = 0; i < labels.Length; i++)
        {
            Button btn = LightUi.Button(labels[i], curX, 4, 110, DialogResult.None);
            btn.Height = 32;
            btn.Font = new Font("Microsoft YaHei UI", 8.5F, FontStyle.Regular);
            Action act = actions[i];
            btn.Click += delegate { act(); };
            panel.Controls.Add(btn);
            curX += 118;
        }
        return panel;
    }

    private static string PickDateTime(string current)
    {
        DateTime initial;
        if (!DateTime.TryParseExact(current, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out initial)) initial = DateTime.Now;
        Form dialog = LightUi.Form("Pick time", 360, 210);
        LightUi.Heading(dialog, "Pick time", "Pick a date and time; clear to leave unlimited.");
        DateTimePicker picker = new DateTimePicker { Left = 26, Top = 92, Width = 308, Height = 32, Format = DateTimePickerFormat.Custom, CustomFormat = "yyyy-MM-dd HH:mm", Value = initial, Font = new Font("Microsoft YaHei UI", 10F) };
        Button clear = LightUi.Button("Clear", 82, 150, 76, DialogResult.Retry);
        Button cancel = LightUi.Button("Cancel", 168, 150, 76, DialogResult.Cancel);
        Button ok = LightUi.PrimaryButton("OK", 254, 150, 80, DialogResult.OK);
        dialog.Controls.AddRange(new Control[] { picker, clear, cancel, ok });
        DialogResult result = dialog.ShowDialog();
        if (result == DialogResult.OK) return picker.Value.ToString("yyyy-MM-dd HH:mm");
        if (result == DialogResult.Retry) return "";
        return null;
    }
    private static string DateEdit(DateTimeOffset? value) { return value.HasValue ? value.Value.ToLocalTime().ToString("yyyy-MM-dd HH:mm") : ""; }
    private static bool TryEditorDate(string text, out DateTimeOffset result)
    {
        DateTime local; if (!DateTime.TryParseExact(text.Trim(), "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out local)) { result = default(DateTimeOffset); return false; }
        result = new DateTimeOffset(local, TimeZoneInfo.Local.GetUtcOffset(local)); return true;
    }


    private static void Manage(Dictionary<string, object> state, ref bool refresh)
    {
        bool managerChanged = false;
        Form f = LightUi.Form("All tasks", 1120, 760); LightUi.Heading(f, "All tasks", "Manage all your tasks with batch operations", "all-tasks.svg");
        Button close = LightUi.Button("×", 1054, 22, 36, DialogResult.Cancel); close.Height = 34; f.Controls.Add(close);
        TextBox search = SearchField(f, 610, 38, 378);
        int filter = 0;
        CheckBox onlyOpen = new CheckBox { Left = 944, Top = 128, Width = 130, Height = 24, Text = "Open only", ForeColor = LightUi.Text, BackColor = Color.Transparent, Font = new Font("Microsoft YaHei UI", 9F) };
        f.Controls.Add(onlyOpen);
        Button allTab = LightUi.Button("All  0", 32, 118, 96, DialogResult.None), overdueTab = LightUi.Button("Overdue  0", 138, 118, 96, DialogResult.None), futureTab = LightUi.Button("Upcoming  0", 244, 118, 104, DialogResult.None), pendingTab = LightUi.Button("Pending  0", 358, 118, 96, DialogResult.None), doneTab = LightUi.Button("Done  0", 464, 118, 96, DialogResult.None);
        f.Controls.AddRange(new Control[]{allTab,overdueTab,futureTab,pendingTab,doneTab});
        Panel table = new Panel { Left = 32, Top = 180, Width = 1056, Height = 470, BackColor = Color.FromArgb(247, 251, 255), AutoScroll = true };
        LightUi.EnableDoubleBuffer(table);
        LightUi.Round(table, 12); f.Controls.Add(table);
        Panel footer = new Panel { Left = 32, Top = 682, Width = 1056, Height = 54, BackColor = Color.FromArgb(245, 251, 255) };
        LightUi.EnableDoubleBuffer(footer);
        LightUi.Round(footer, 12); f.Controls.Add(footer);
        Label selectionHint = LightUi.Label("0 selected", 18, 16, 240); footer.Controls.Add(selectionHint);
        Button edit = LightUi.Button("Edit selected", 604, 8, 112, DialogResult.None), toggle = LightUi.Button("Complete selected", 728, 8, 112, DialogResult.None), delete = LightUi.DangerButton("Delete", 852, 8, 76, DialogResult.None), add = LightUi.PrimaryButton("+ New task", 940, 8, 100, DialogResult.None);
        footer.Controls.AddRange(new Control[]{edit,toggle,delete,add}); f.CancelButton = close;
        List<CheckBox> rowChecks = new List<CheckBox>();
        Dictionary<string, Panel> rowPanels = new Dictionary<string, Panel>();
        string selectedId = "";
        Action paintTabs = delegate {
            Button[] tabs = { allTab, overdueTab, futureTab, pendingTab, doneTab };
            for (int i = 0; i < tabs.Length; i++) PaintTabButton(tabs[i], filter == i);
        };
        Action paintRows = delegate {
            foreach (KeyValuePair<string, Panel> pair in rowPanels)
                pair.Value.BackColor = pair.Key == selectedId ? Color.FromArgb(232, 244, 255) : Color.FromArgb(247, 251, 255);
        };
        MouseEventHandler selectRow = delegate(object sender, MouseEventArgs e) {
            if (e.Button != MouseButtons.Left) return;
            Control control = sender as Control;
            while (control != null && !(control.Tag is string)) control = control.Parent;
            if (control == null) return;
            selectedId = Convert.ToString(control.Tag);
            paintRows();
        };
        Action<bool> reload = null;
        reload = delegate(bool preserveScroll) {
            int previousScrollY = preserveScroll ? Math.Max(0, -table.AutoScrollPosition.Y) : 0;
            // Clearing rows while the panel is still scrolled leaves its negative display
            // offset in the next layout pass. Reset first, then restore the clamped offset
            // after the rebuilt controls have established the new scroll range.
            table.AutoScrollPosition = Point.Empty;
            List<Dictionary<string, object>> all = Tasks(state);
            DateTimeOffset now = DateTimeOffset.Now;
            allTab.Text = "All  " + all.Count;
            overdueTab.Text = "Overdue  " + all.Count(t => !B(t, "completed") && RuntimeUtil.Date(t, "due_at").HasValue && now > RuntimeUtil.Date(t, "due_at").Value);
            futureTab.Text = "Upcoming  " + all.Count(t => !B(t, "completed") && RuntimeUtil.Date(t, "available_from").HasValue && now < RuntimeUtil.Date(t, "available_from").Value);
            pendingTab.Text = "Pending  " + all.Count(t => !B(t, "completed") && (!RuntimeUtil.Date(t, "due_at").HasValue || now <= RuntimeUtil.Date(t, "due_at").Value) && (!RuntimeUtil.Date(t, "available_from").HasValue || now >= RuntimeUtil.Date(t, "available_from").Value));
            doneTab.Text = "Done  " + all.Count(t => B(t, "completed"));
            string query = search.Text.Trim();
            table.SuspendLayout(); LightUi.SetRedraw(table, false); table.Controls.Clear(); rowChecks.Clear(); rowPanels.Clear();
            AddCellLabel(table, "Status", 50, 14, 70, LightUi.Muted, FontStyle.Bold);
            AddCellLabel(table, "Title", 128, 14, 350, LightUi.Muted, FontStyle.Bold);
            AddCellLabel(table, "Labels", 486, 14, 140, LightUi.Muted, FontStyle.Bold);
            AddCellLabel(table, "Start", 640, 14, 132, LightUi.Muted, FontStyle.Bold);
            AddCellLabel(table, "Due", 788, 14, 132, LightUi.Muted, FontStyle.Bold);
            AddCellLabel(table, "Actions", 936, 14, 86, LightUi.Muted, FontStyle.Bold);
            int y = 42;
            foreach (Dictionary<string, object> t in all.Where(t => TaskMatchesFilter(t, filter, now)).Where(t => !onlyOpen.Checked || !B(t, "completed")).Where(t => query == "" || S(t, "title").IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0 || String.Join("、", Labels(t)).IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0).OrderBy(t => B(t,"completed") ? 3 : (RuntimeUtil.Date(t,"due_at").HasValue && now > RuntimeUtil.Date(t,"due_at").Value ? 0 : RuntimeUtil.Date(t,"available_from").HasValue && now < RuntimeUtil.Date(t,"available_from").Value ? 1 : 2)).ThenByDescending(t => RuntimeUtil.Date(t,"created_at") ?? DateTimeOffset.MinValue)) {
                string id = S(t, "id");
                Panel row = new Panel { Left = 12, Top = y, Width = 1016, Height = 42, BackColor = Color.FromArgb(247, 251, 255), Tag = id, Cursor = Cursors.Hand };
                LightUi.EnableDoubleBuffer(row);
                LightUi.Round(row, 8);
                row.MouseDown += selectRow;
                CheckBox check = new CheckBox { Left = 6, Top = 11, Width = 20, Height = 20, BackColor = Color.Transparent, Tag = id };
                check.CheckedChanged += delegate { selectionHint.Text = rowChecks.Count(c => c.Checked) + " selected"; };
                row.Controls.Add(check); rowChecks.Add(check);
                AddCellLabel(row, TaskStatusText(t, now), 36, 11, 70, TaskStatusColor(t, now), FontStyle.Regular).MouseDown += selectRow;
                AddCellLabel(row, S(t,"title"), 114, 11, 350, LightUi.Text, FontStyle.Regular).MouseDown += selectRow;
                AddCellLabel(row, String.Join("  ", Labels(t)), 472, 11, 140, LightUi.Accent, FontStyle.Regular).MouseDown += selectRow;
                AddCellLabel(row, DateEdit(RuntimeUtil.Date(t,"available_from")) == "" ? "—" : DateEdit(RuntimeUtil.Date(t,"available_from")), 626, 11, 132, LightUi.Text, FontStyle.Regular).MouseDown += selectRow;
                AddCellLabel(row, DateEdit(RuntimeUtil.Date(t,"due_at")) == "" ? "—" : DateEdit(RuntimeUtil.Date(t,"due_at")), 774, 11, 132, LightUi.Text, FontStyle.Regular).MouseDown += selectRow;
                Button openBtn = RowIcon("\xE72A", 914, 5);
                Button editBtn = RowIcon("\xE70F", 948, 5);
                Button deleteBtn = RowIcon("\xE74D", 982, 5);
                openBtn.Click += delegate { bool changed=false; Open(state, id, ref changed); managerChanged |= changed; if (changed) reload(true); };
                editBtn.Click += delegate { bool changed=false; Edit(state, id, ref changed); managerChanged |= changed; if (changed) reload(true); };
                deleteBtn.Click += delegate { bool changed=false; Delete(state, id, ref changed); managerChanged |= changed; if (changed) reload(true); };
                row.Controls.Add(openBtn); row.Controls.Add(editBtn); row.Controls.Add(deleteBtn);
                table.Controls.Add(row); rowPanels[id] = row; y += 42;
            }
            table.ResumeLayout();
            int maxScrollY = Math.Max(0, table.DisplayRectangle.Height - table.ClientSize.Height);
            if (previousScrollY > 0) table.AutoScrollPosition = new Point(0, Math.Min(previousScrollY, maxScrollY));
            LightUi.SetRedraw(table, true); paintTabs(); paintRows(); selectionHint.Text = rowChecks.Count(c => c.Checked) + " selected";
        };
        allTab.Click += delegate { filter = 0; reload(false); };
        overdueTab.Click += delegate { filter = 1; reload(false); };
        futureTab.Click += delegate { filter = 2; reload(false); };
        pendingTab.Click += delegate { filter = 3; reload(false); };
        doneTab.Click += delegate { filter = 4; reload(false); };
        search.TextChanged += delegate { reload(false); };
        onlyOpen.CheckedChanged += delegate { reload(false); };
        search.Parent.BringToFront();
        search.BringToFront();
        close.BringToFront();
        reload(false);
        edit.Click += delegate { if (selectedId == "") { selectionHint.Text="Select a task to edit first."; selectionHint.ForeColor=LightUi.Danger; return; } bool changed=false; Edit(state, selectedId, ref changed); managerChanged |= changed; selectionHint.ForeColor=LightUi.Muted; if (changed) reload(true); };
        toggle.Click += delegate { List<string> selected=rowChecks.Where(c=>c.Checked).Select(c=>Convert.ToString(c.Tag)).ToList(); if(selected.Count==0){selectionHint.Text="Check the tasks to complete or restore first.";selectionHint.ForeColor=LightUi.Danger;return;} foreach (string id in selected) { bool changed=false; Toggle(state,id,ref changed); managerChanged |= changed; } selectionHint.ForeColor=LightUi.Muted; reload(true); };
        delete.Click += delegate { List<string> selected=rowChecks.Where(c=>c.Checked).Select(c=>Convert.ToString(c.Tag)).ToList(); if(selected.Count==0){selectionHint.Text="Check the tasks to delete first.";selectionHint.ForeColor=LightUi.Danger;return;} if(!LightUi.Confirm("Delete the "+selected.Count+" selected task(s)?","Batch delete"))return; foreach (string id in selected) Tasks(state).RemoveAll(t => S(t, "id") == id); Meta(state)["status"]="Batch deleted";Commit(state);managerChanged=true;selectionHint.ForeColor=LightUi.Muted;reload(true); };
        add.Click += delegate { bool changed=false; Add(state, ref changed); managerChanged |= changed; if (changed) reload(false); };
        table.DoubleClick += delegate { edit.PerformClick(); }; f.ShowDialog(); refresh |= managerChanged;
    }

    private static Label AddCellLabel(Control parent, string text, int x, int y, int width, Color color, FontStyle style)
    {
        Label label = new Label { Left = x, Top = y, Width = width, Height = 22, Text = text, ForeColor = color, BackColor = Color.Transparent, AutoEllipsis = true, Font = new Font("Microsoft YaHei UI", 9F, style) };
        parent.Controls.Add(label);
        return label;
    }

    private static Button RowIcon(string text, int x, int y)
    {
        Button button = LightUi.Button(text, x, y, 28, DialogResult.None);
        button.Height = 30;
        button.Font = new Font("Segoe Fluent Icons", 9F);
        button.BackColor = Color.FromArgb(247, 251, 255);
        button.FlatAppearance.BorderColor = button.BackColor;
        button.FlatAppearance.BorderSize = 0;
        return button;
    }

    private static void PaintTabButton(Button button, bool active)
    {
        button.BackColor = active ? Color.FromArgb(220, 238, 255) : LightUi.Panel;
        button.ForeColor = active ? LightUi.Accent : LightUi.Text;
        button.FlatAppearance.BorderColor = button.BackColor;
        button.FlatAppearance.BorderSize = 0;
    }

    private static string TaskStatusText(Dictionary<string, object> task, DateTimeOffset now)
    {
        bool completed = B(task, "completed");
        bool overdue = !completed && RuntimeUtil.Date(task, "due_at").HasValue && now > RuntimeUtil.Date(task, "due_at").Value;
        bool future = !completed && RuntimeUtil.Date(task, "available_from").HasValue && now < RuntimeUtil.Date(task, "available_from").Value;
        return completed ? "Done" : overdue ? "Overdue" : future ? "Upcoming" : "Pending";
    }

    private static Color TaskStatusColor(Dictionary<string, object> task, DateTimeOffset now)
    {
        string status = TaskStatusText(task, now);
        if (status == "Done") return Color.FromArgb(28, 145, 82);
        if (status == "Overdue") return LightUi.Danger;
        if (status == "Upcoming") return Color.FromArgb(145, 96, 28);
        return LightUi.Accent;
    }

}

