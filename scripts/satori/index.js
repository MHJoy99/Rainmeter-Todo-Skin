import fs from 'fs';
import satori from 'satori';
import { html } from 'satori-html';
import { Resvg } from '@resvg/resvg-js';

const generateOgImage = async () => {
  const fontBold = fs.readFileSync('Roboto-Bold.ttf');
  const fontRegular = fs.readFileSync('Roboto-Regular.ttf');

  const markup = html`
    <div style="display: flex; height: 100%; width: 100%; align-items: center; justify-content: center; flex-direction: column; background-image: linear-gradient(to bottom right, #0d1117, #161b22, #0d1117); font-family: Roboto;">
      
      <!-- Glowing background accent -->
      <div style="display: flex; position: absolute; width: 600px; height: 600px; background: radial-gradient(circle, rgba(56, 139, 253, 0.15) 0%, transparent 70%); top: 50%; left: 50%; transform: translate(-50%, -50%);"></div>

      <!-- Main Container -->
      <div style="display: flex; flex-direction: column; align-items: center; justify-content: center; z-index: 10;">
        
        <!-- App Title & Badge -->
        <div style="display: flex; align-items: center; gap: 20px; margin-bottom: 24px;">
          <div style="display: flex; align-items: center; justify-content: center; width: 80px; height: 80px; background: linear-gradient(135deg, #1f6feb, #238636); border-radius: 20px; box-shadow: 0 8px 24px rgba(0,0,0,0.4); border: 1px solid rgba(255,255,255,0.1);">
            <svg width="40" height="40" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
              <path d="M9 16.2L4.8 12L3.4 13.4L9 19L21 7L19.6 5.6L9 16.2Z" fill="white"/>
            </svg>
          </div>
          <h1 style="color: #ffffff; font-size: 84px; font-weight: 700; margin: 0; letter-spacing: -2px;">Rainmeter Todo</h1>
        </div>

        <div style="display: flex; font-size: 32px; color: #8b949e; margin-bottom: 60px; font-weight: 400; letter-spacing: -0.5px;">
          The ultimate productivity skin for your desktop.
        </div>

        <!-- Feature Pills -->
        <div style="display: flex; gap: 24px;">
          <div style="display: flex; align-items: center; padding: 12px 24px; background: rgba(255,255,255,0.05); border: 1px solid rgba(255,255,255,0.1); border-radius: 100px; color: #c9d1d9; font-size: 24px; font-weight: 600;">
            <svg width="24" height="24" viewBox="0 0 24 24" fill="none" style="margin-right: 12px;">
              <path d="M12 2C6.48 2 2 6.48 2 12C2 17.52 6.48 22 12 22C17.52 22 22 17.52 22 12C22 6.48 17.52 2 12 2ZM11 19.93C7.06 19.43 4 16.05 4 12C4 7.95 7.06 4.57 11 4.07V19.93ZM13 4.07C16.94 4.57 20 7.95 20 12C20 16.05 16.94 19.43 13 19.93V4.07Z" fill="#58a6ff"/>
            </svg>
            Offline First
          </div>

          <div style="display: flex; align-items: center; padding: 12px 24px; background: rgba(255,255,255,0.05); border: 1px solid rgba(255,255,255,0.1); border-radius: 100px; color: #c9d1d9; font-size: 24px; font-weight: 600;">
            <svg width="24" height="24" viewBox="0 0 24 24" fill="none" style="margin-right: 12px;">
              <path d="M19 4H5C3.89 4 3 4.9 3 6V18C3 19.1 3.89 20 5 20H19C20.1 20 21 19.1 21 18V6C21 4.9 20.1 4 19 4ZM19 18H5V8H19V18ZM17 10H7V12H17V10ZM14 14H7V16H14V14Z" fill="#3fb950"/>
            </svg>
            Google Tasks Sync
          </div>

          <div style="display: flex; align-items: center; padding: 12px 24px; background: rgba(255,255,255,0.05); border: 1px solid rgba(255,255,255,0.1); border-radius: 100px; color: #c9d1d9; font-size: 24px; font-weight: 600;">
            <svg width="24" height="24" viewBox="0 0 24 24" fill="none" style="margin-right: 12px;">
              <path d="M12 2L15.09 8.26L22 9.27L17 14.14L18.18 21.02L12 17.77L5.82 21.02L7 14.14L2 9.27L8.91 8.26L12 2Z" fill="#d2a8ff"/>
            </svg>
            CalDAV Support
          </div>
        </div>

      </div>
      
      <!-- Footer GitHub logo -->
      <div style="position: absolute; bottom: 40px; display: flex; align-items: center; color: #8b949e; font-size: 24px; font-weight: 600;">
        <svg width="32" height="32" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg" style="margin-right: 12px;">
          <path fill-rule="evenodd" clip-rule="evenodd" d="M12 2C6.477 2 2 6.484 2 12.017C2 16.435 4.865 20.185 8.835 21.505C9.335 21.597 9.516 21.288 9.516 21.025C9.516 20.79 9.508 20.165 9.503 19.336C6.722 19.941 6.136 17.994 6.136 17.994C5.681 16.839 5.025 16.531 5.025 16.531C4.118 15.912 5.093 15.924 5.093 15.924C6.096 15.995 6.623 16.953 6.623 16.953C7.514 18.49 8.966 18.046 9.553 17.79C9.643 17.118 9.914 16.674 10.215 16.422C7.997 16.17 5.666 15.312 5.666 11.496C5.666 10.408 6.054 9.517 6.689 8.826C6.587 8.574 6.246 7.558 6.787 6.191C6.787 6.191 7.621 5.923 9.499 7.195C10.291 6.974 11.144 6.864 11.992 6.86C12.839 6.864 13.692 6.974 14.485 7.195C16.362 5.923 17.195 6.191 17.195 6.191C17.737 7.558 17.396 8.574 17.294 8.826C17.931 9.517 18.316 10.408 18.316 11.496C18.316 15.321 15.981 16.166 13.755 16.413C14.133 16.741 14.471 17.391 14.471 18.414C14.471 19.88 14.453 21.064 14.453 21.42C14.453 21.685 14.632 22.001 15.141 21.503C19.11 20.181 21.996 16.434 21.996 12.017C21.996 6.484 17.523 2 12 2Z" fill="#8b949e"/>
        </svg>
        MHJoy99/Rainmeter-Todo-Skin
      </div>
    </div>
  `;

  const svg = await satori(markup, {
    width: 1200,
    height: 630,
    fonts: [
      {
        name: 'Roboto',
        data: fontRegular,
        weight: 400,
        style: 'normal',
      },
      {
        name: 'Roboto',
        data: fontBold,
        weight: 700,
        style: 'normal',
      },
      {
        name: 'Roboto',
        data: fontBold,
        weight: 600,
        style: 'normal',
      },
    ],
  });

  const resvg = new Resvg(svg, {
    background: '#0d1117',
    fitTo: {
      mode: 'width',
      value: 1200,
    },
  });
  const pngData = resvg.render();
  const pngBuffer = pngData.asPng();

  fs.writeFileSync('../../img/satori-banner.png', pngBuffer);
  console.log('Successfully generated Satori banner!');
};

generateOgImage().catch(console.error);