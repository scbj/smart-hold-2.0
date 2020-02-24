using Smart_Hold.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace Smart_Hold.ViewModels
{
    public class SmartHoldViewModel : ViewModelBase, IViewModel<SmartHoldView>
    {
        private static SmartHoldViewModel instance;
        private decimal backupFrequency;
        private bool activated;
        private bool running;
        private string currentFile;
        private readonly ObservableCollection<FolderBackupViewModel> paths;
        private readonly ICollectionView collectionView;
        private ICommand browseCommand;
        private ICommand removeCommand;
        private ICommand activatedCommand;



        public static SmartHoldViewModel Instance
        {
            get
            {
                if (instance == null)
                    instance = new SmartHoldViewModel();
                return instance;
            }
        }
        public SmartHold SmartHold
        {
            get { return SmartHold.Instance; }
        }
        public SmartHoldView View { get; set; }
        object IViewModel.View
        {
            get { return View; }
            set { View = (SmartHoldView)value; }
        }
        public Version Version { get { return new Version(2, 0, 3); } }
        public List<DateTime> OldBackups { get; set; }
        public decimal BackupFrequency
        {
            get { return this.backupFrequency; }
            set
            {
                this.backupFrequency = value;
                OnPropertyChanged("BackupFrequency");
            }
        }
        public bool Activated
        {
            get { return this.activated; }
            set
            {
                this.activated = value;
                OnPropertyChanged("Activated");
                if (value) SmartHold.Start();
                else SmartHold.Stop();
            }
        }
        public bool Running
        {
            get { return this.running; }
            set
            {
                this.running = value;
                OnPropertyChanged("Running");
            }
        }
        public string CurrentFile
        {
            get { return this.currentFile; }
            set
            {
                this.currentFile = value;
                OnPropertyChanged("CurrentFile");
            }
        }

        public int EnabledCount
        {
            get { return this.paths.Where(fb => fb.Enabled).Count(); }
            set { OnPropertyChanged("EnabledCount"); }
        }
        public ObservableCollection<FolderBackupViewModel> Paths { get { return this.paths; } }
        public FolderBackupViewModel SelectedFolderBackup
        {
            get { return this.collectionView.CurrentItem as FolderBackupViewModel; }
        }
        public ICommand BrowseCommand
        {
            get
            {
                if (this.browseCommand == null)
                    this.browseCommand = new RelayCommand(() => Browse(), () => !Activated);

                return this.browseCommand;
            }
        }
        public ICommand RemoveCommand
        {
            get
            {
                if (this.removeCommand == null)
                    this.removeCommand = new RelayCommand(() => this.paths.Remove(SelectedFolderBackup), () => !Activated && SelectedFolderBackup != null);

                return this.removeCommand;
            }
        }
        public ICommand ActivatedCommand
        {
            get
            {
                if (this.activatedCommand == null)
                    this.activatedCommand = new RelayCommand(() => Activated = View.CheckActivate.IsChecked ?? false, () => !Running && this.paths.Count > 0);

                return this.activatedCommand;
            }
        }


        public SmartHoldViewModel()
        {
            this.paths = new ObservableCollection<FolderBackupViewModel>();
            this.collectionView = CollectionViewSource.GetDefaultView(this.paths);
            if (this.collectionView == null)
                throw new NullReferenceException("collectionView");
            this.collectionView.CurrentChanged += (s, e) => OnPropertyChanged("SelectedFolderBackup");
            this.collectionView.CollectionChanged += (s, e) => OnPropertyChanged("EnabledCount");
        }

        public void Intialize()
        {
            OldBackups = SmartHold.Config.GetObjects<string>("oldBackups").Select(s => DateTime.Parse(s)).ToList();
            SmartHold.Config.GetPaths("paths").ToList().ForEach(fb => this.paths.Add(fb));
            var value = SmartHold.Config.GetDecimal("backupFrequency");
            BackupFrequency = value > 0 ? value : 24;
            Activated = SmartHold.Config.GetBool("activated");
        }

        private void Browse()
        {
            var fbd = new System.Windows.Forms.FolderBrowserDialog()
            {
                ShowNewFolderButton = false,
                Description = "Sélectionnez un dossier que vous désirez sauvegarder."
            };
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK && !this.paths.ToList().Exists(fb => fb.Path == fbd.SelectedPath))
                this.paths.Add(FolderBackupViewModel.Parse(fbd.SelectedPath));
        }

        public void Exit()
        {
            SmartHold.Config.SetObjects("oldBackups", OldBackups);
            SmartHold.Config.SetPaths("paths", this.paths);
            SmartHold.Config.SetBool("activated", Activated);
            SmartHold.Config.SetDecimal("backupFrequency", BackupFrequency);
        }
    }
}
