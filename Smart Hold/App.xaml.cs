using Smart_Hold.ViewModels;
using Smart_Hold.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Smart_Hold
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        // Fields
        private System.Windows.Forms.NotifyIcon nIcon;
        private Mutex mutexSingleInstance;
        private bool isExit;

        public App()
        {
            bool isNew;
            mutexSingleInstance = new Mutex(true, "8F6F0AC4-B9A1-45fd-A8CF-72F04E6BDE8F", out isNew);
            if (!isNew)
                Environment.Exit(0);
        }

        ~App()
        {
            Logger.Logger.Save("debug_errors.log");
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainWindow = new SmartHoldView();
            (MainWindow as SmartHoldView).Initialize();
            MainWindow.StateChanged += (s, args) => { if (MainWindow.WindowState == WindowState.Minimized) MainWindow.Hide(); };
            MainWindow.Closing += MainWindow_Closing;
            Microsoft.Win32.SystemEvents.SessionEnding += (s, args) => ExitApplication();

            nIcon = new System.Windows.Forms.NotifyIcon();
            nIcon.Click += (s, args) => ShowMainWindow();
            nIcon.BalloonTipClicked += (s, args) => ShowMainWindow();
            nIcon.Icon = Smart_Hold.Properties.Resources.Notify_Icon;
            nIcon.Text = MainWindow.Title;
            nIcon.Visible = true;
            Toaster.Toaster.Initialize(nIcon);

            CreateContextMenu();
        }

        private void CreateContextMenu()
        {
            nIcon.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
            nIcon.ContextMenuStrip.Items.Add("Ouvrir").Click += (s, e) => ShowMainWindow();
            nIcon.ContextMenuStrip.Items.Add("Sauvegarder maintenant").Click += (s, e) =>
            {
                if (!SmartHoldViewModel.Instance.Running)
                    new Task(new Action(() => SmartHoldViewModel.Instance.SmartHold.Run(SmartHoldViewModel.Instance.Paths))).Start();
            };
            nIcon.ContextMenuStrip.Items.Add("Quitter").Click += (s, e) => ExitApplication();
        }

        private void ExitApplication()
        {
            isExit = true;
            (MainWindow as SmartHoldView).ViewModel.Exit();
            MainWindow.Close();
            nIcon.Dispose();
            nIcon = null;
        }

        private void ShowMainWindow()
        {
            if (MainWindow.IsVisible)
                MainWindow.Activate();
            else
                MainWindow.Show();

            if (MainWindow.WindowState == WindowState.Minimized)
                MainWindow.WindowState = WindowState.Normal;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!isExit)
            {
                e.Cancel = true;
                MainWindow.Hide();
            }
        }
    }
}
