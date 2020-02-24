using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Smart_Hold.IO;
using Smart_Hold.ViewModels;

namespace Smart_Hold.Models
{
    public class FolderBackup
    {
        //private int totalFiles = -1;

        public DirectoryInfo DirectoryInfo;
        public string Path { get { return DirectoryInfo.FullName; } }
        public bool Enabled { get; set; }
        public double ProgressValue { get; set; }
        public int TotalFiles { get; private set; }
        public int IndexFile { get; private set; }

        public FolderBackup(string folderPath)
        {
            Enabled = true;
            DirectoryInfo = new DirectoryInfo(folderPath);
            SetTotalFilesAsync();            
        }

        private void SetTotalFilesAsync()
        {
            var task = new Task<int>(() => { return Explorer.TotalFiles(DirectoryInfo); });
            task.ContinueWith(antecendent =>
            {
                System.Threading.Thread.Sleep(200);
                var viewModel = SmartHoldViewModel.Instance?.Paths?.Single(x => x.FolderBackup == this);
                viewModel.TotalFiles = viewModel.FolderBackup.TotalFiles = antecendent.Result;
            });
            task.Start();
        }
    }
}
