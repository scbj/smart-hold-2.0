using Smart_Hold.IO;
using Smart_Hold.ViewModels;
using Smart_Hold.Views;
using Smart_Hold.Xml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Smart_Hold
{
    public class SmartHold
    {

        private static SmartHold instance;

        private Task LaunchWatcher;

        public static SmartHold Instance
        {
            get
            {
                if (instance == null)
                    instance = new SmartHold();
                return instance;
            }
        }
        public static FolderBackupViewModel CurrentFolderBackup { get; set; }

        public XmlConfig Config { get; set; }
        public bool Running { set { SmartHoldViewModel.Instance.Running = value; } }
        public string BackupDrive { get; set; }
        public string BackupFolder { get { return Path.Combine(BackupDrive, "SmartHold"); } }


        public SmartHold()
        {
            Config = new XmlConfig("config.xml");
            Config.Initialize();
            Config.SetRoot("//" + nameof(SmartHold));
        }

        internal void Start()
        {
            LaunchWatcher = new Task(new Action(() =>
            {
                // Loop until Activated is True.
                while (SmartHoldViewModel.Instance.Activated)
                {
                    if (SmartHoldViewModel.Instance.OldBackups.Count == 0 && HardDriveReady())
                        Run(SmartHoldViewModel.Instance.Paths);
                    else
                    {
                        var lastBackup = SmartHoldViewModel.Instance.OldBackups.Last();
                        var backupFrequency = (double)SmartHoldViewModel.Instance.BackupFrequency;
                        var ts = DateTime.Now - lastBackup;
                        if (ts.TotalHours > backupFrequency && HardDriveReady())
                            Run(SmartHoldViewModel.Instance.Paths);
                    }

                    System.Threading.Thread.Sleep(TimeSpan.FromMinutes(15));
                }
            }));
            LaunchWatcher.Start();
        }

        public void Run(IEnumerable<FolderBackupViewModel> paths)
        {
            if (!HardDriveReady())
                return;

            Running = true;

            if (!Directory.Exists(BackupFolder))
                Directory.CreateDirectory(BackupFolder);

            foreach (FolderBackupViewModel fb in paths.Where(fb => fb.Enabled))
            {
                CurrentFolderBackup = fb;

                var diSource = fb.FolderBackup.DirectoryInfo;
                var diTarget = new DirectoryInfo(Path.Combine(BackupFolder, diSource.Name));

                Explorer.Clone(diSource, diTarget);
                Explorer.CleanTarget(diSource, diTarget);
            }

            new DirectoryInfo(BackupFolder).EnumerateDirectories().ToList().ForEach(di =>
            {
                try
                {
                    if (!paths.ToList().Exists(fb => fb.FolderBackup.DirectoryInfo.Name == di.Name))
                        di.Delete(true);
                }
                catch (Exception ex)
                {
                    Logger.Logger.LogError(ex, di);
                }
            });

            Logger.Logger.Save("debug_errors.log");
            SmartHoldViewModel.Instance.OldBackups.Add(DateTime.Now);
            DateTime next = DateTime.Now.AddHours((double)SmartHoldViewModel.Instance.BackupFrequency);
            string message = $"Sauvegarde effectuée avec succès, prochaine sauvegarde à partir du {next.ToShortDateString()} à {next.ToShortTimeString()}.";
            Toaster.Toaster.Toast(nameof(SmartHold), message, System.Windows.Forms.ToolTipIcon.Info, Toaster.ToastLength.LONG);
            Running = false;
        }

        internal void Stop()
        {
            //throw new NotImplementedException();
        }

        private bool HardDriveReady()
        {
            BackupDrive = String.Empty;
            var drives = DriveInfo.GetDrives()
                .Where(d => d.IsReady && (d.DriveType == DriveType.Fixed || d.DriveType == DriveType.Removable));
            foreach (var d in drives)
                if (File.Exists(Path.Combine(d.RootDirectory.ToString(), "IdentificatorFile.ext")))
                    BackupDrive = d.RootDirectory.ToString();

            return !String.IsNullOrWhiteSpace(BackupDrive);
        }
    }
}
