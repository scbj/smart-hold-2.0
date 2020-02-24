using Smart_Hold.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smart_Hold.ViewModels
{
    public class FolderBackupViewModel : ViewModelBase
    {
        public string Path { get { return FolderBackup.Path; } }
        public FolderBackup FolderBackup { get; }
        public bool Enabled
        {
            get { return this.FolderBackup.Enabled; }
            set
            {
                this.FolderBackup.Enabled = value;
                OnPropertyChanged("Enabled");
            }
        }
        public double ProgressValue
        {
            get { return this.FolderBackup.ProgressValue; }
            set
            {
                this.FolderBackup.ProgressValue = value;
                OnPropertyChanged("ProgressValue");
            }
        }
        public int TotalFiles
        {
            get { return this.FolderBackup.TotalFiles; }
            set { OnPropertyChanged("TotalFiles"); }
        }
        public int IndexFile
        {
            get { return this.FolderBackup.IndexFile; }
            set { OnPropertyChanged("IndexFile"); }
        }


        public FolderBackupViewModel(FolderBackup folderBackup)
        {
            this.FolderBackup = folderBackup;
        }

        public static FolderBackupViewModel Parse(string folderPath) => new FolderBackupViewModel(new FolderBackup(folderPath));
    }
}
