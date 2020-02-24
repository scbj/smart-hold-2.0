using Smart_Hold.Models;
using Smart_Hold.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Smart_Hold.Views
{
    /// <summary>
    /// Logique d'interaction pour SmartHoldView.xaml
    /// </summary>
    public partial class SmartHoldView : Window, IView<SmartHoldViewModel>
    {

        // Properties
        public SmartHoldViewModel ViewModel { get; set; }
        object IView.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (SmartHoldViewModel)value; }
        }


        // Constructors
        public SmartHoldView()
        {
            DataContext = ViewModel = SmartHoldViewModel.Instance;
            InitializeComponent();
            ViewModel.View = this;
        }

        public void Initialize()
        {
            if (ViewModel.Paths.Count > 0)
                WindowState = WindowState.Minimized;
            ViewModel.Intialize();
            CheckActivate.IsChecked = ViewModel.Activated;
        }

        private void FolderBackupCheckedChanged(object sender, RoutedEventArgs e) => ViewModel.EnabledCount = -1;
        private void CheckBox_CheckedChanged(object sender, RoutedEventArgs e) => ViewModel.Activated = CheckActivate.IsChecked ?? false;
        private void ListBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (!ViewModel.Activated && e.Key == Key.Delete)
                ViewModel.RemoveCommand.Execute(null);
        }
        private void Hyperlink_Click(object sender, RoutedEventArgs e) => MessageBox.Show(String.Join("\n", ViewModel.OldBackups.Skip(ViewModel.OldBackups.Count - 5)));
    }
}
