using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinUIEx;

namespace WinUI.Method
{
     public static class Initialize
    {
        public static void InitWindowSize(Window window, double scale = 0.55)
        {
            var appWindow = window.AppWindow;
            var displayArea = DisplayArea.GetFromWindowId(
                appWindow.Id,
                DisplayAreaFallback.Primary
            );
            var workArea = displayArea.WorkArea;
            int windowWidth = (int)(workArea.Width * scale);
            int windowHeight = (int)(workArea.Height * scale);
            windowWidth = Math.Clamp(windowWidth, 900, 1600);
            windowHeight = Math.Clamp(windowHeight, 650, 1100);
            window.SetWindowSize(windowWidth, windowHeight);
            window.CenterOnScreen();
        }

        /// <summary>
        /// 设置窗口最小尺寸
        /// </summary>
        public static void SetMinSize(Window window, int width, int height)
        {
            if (window.AppWindow.Presenter is OverlappedPresenter presenter)
            {
                presenter.PreferredMinimumWidth = width;
                presenter.PreferredMinimumHeight = height;
            }
        }
    }
}
