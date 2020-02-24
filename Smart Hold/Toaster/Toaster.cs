using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Smart_Hold.Toaster
{
    public static class Toaster
    {
        private static NotifyIcon nIcon;

        public static void Initialize(NotifyIcon notifyIcon) => nIcon = notifyIcon;

        public static void Toast(string title, string message, ToolTipIcon icon, ToastLength length)
        {
            if (nIcon == null)
                throw new WarningException("Toaster must be initialized with Initialize() method.");

            nIcon.ShowBalloonTip((int)length, title, message, icon);
        }
    }

    public enum ToastLength
    {
        LONG = 5000,
        SHORT = 2000
    }
}
