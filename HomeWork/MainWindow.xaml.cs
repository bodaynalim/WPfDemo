using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using HomeWork.Model;
using HomeWork.View;
using HomeWork.ViewModel;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using SaveFileDialog = System.Windows.Forms.SaveFileDialog;

namespace HomeWork
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new SnapshotsViewModel();
        }

        private void ButtonDeleteBase_OnClick(object sender, RoutedEventArgs e)
        {
            var obj = ((FrameworkElement)sender).DataContext as Snapshot;

            ((SnapshotsViewModel)DataContext).Snapshots.Remove(obj);
        }


        private void ButtonReviewBase_OnClick(object sender, RoutedEventArgs e)
        {
            var obj = ((FrameworkElement)sender).DataContext as Snapshot;

            var reviewWindows = new SnapshotOverview()
            {
                DataContext = new SnapshotOverviewViewModel(obj)
            };

            reviewWindows.ShowDialog();
        }

        private void ButtonChooseDirectory_OnClick(object sender, RoutedEventArgs e)
        {
            var openFolderDialog = new FolderBrowserDialog();
            
            if (openFolderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                ((SnapshotsViewModel)DataContext).DirectoryChoosed = openFolderDialog.SelectedPath;
        }

        private async void ButtonOpenSnapshot_OnClick(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog {Filter = @"JSON files (*.json)|*.json" };

            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    using (var reader = new StreamReader(fileDialog.OpenFile(), Encoding.UTF8))
                    {
                        var snapshot = JsonConvert.DeserializeObject<Snapshot>(reader.ReadToEnd());

                        if(snapshot != null)
                            ((SnapshotsViewModel)DataContext).Snapshots.Add(snapshot);
                    }
                    
                }
                catch (Exception exception)
                {
                    await this.ShowMessageAsync("Error", exception.Message);
                }
                
            }
        }

        private void MenuItemReview_OnClick(object sender, RoutedEventArgs e)
        {
            ButtonReviewBase_OnClick(((FrameworkElement)sender).DataContext, e);
        }


        private async  void MenuItemSave_OnClick(object sender, RoutedEventArgs e)
        {
            var obj = ((DataGridRow)((FrameworkElement)sender).DataContext).DataContext as Snapshot;

            var save = new SaveFileDialog {
                DefaultExt = "json", 
                AddExtension = true,
                Filter = @"JSON files (*.json)|*.json"
            };

            if (save.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var progress = await this.ShowProgressAsync("Saving", "Wait for saving");
                
                progress.SetIndeterminate();

                await Task.Delay(5000);

                File.WriteAllText(save.FileName, JsonConvert.SerializeObject(obj));

                await progress.CloseAsync();
            }

        }
    }
}
