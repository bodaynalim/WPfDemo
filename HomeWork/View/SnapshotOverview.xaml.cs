using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HomeWork.ViewModel;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace HomeWork.View
{
    /// <summary>
    /// Interaction logic for SnapshotOverview.xaml
    /// </summary>
    public partial class SnapshotOverview : MetroWindow
    {
        private WatcherChangeTypes CurrentChangeType { get; set; } = WatcherChangeTypes.All;

        private string Search { get; set; }


        public SnapshotOverview()
        {
            InitializeComponent();
        }

        private void TextBoxSearch_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var text = (sender as TextBox)?.Text.ToLower();

            var dataContext = ((SnapshotOverviewViewModel) DataContext);

            Search = text;

            if (!string.IsNullOrEmpty(text))
            {
                dataContext.TreeView = new ObservableCollection<FileTree>(
                   SnapshotOverviewViewModel.ParseRecursive(dataContext.FilteredList.Where(s => CurrentChangeType.HasFlag(s.ChangeType) 
                                                                                                && s.FullPath.ToLower().Contains(Search))));
            }
            else
                dataContext.TreeView = new ObservableCollection<FileTree>(
                    SnapshotOverviewViewModel.ParseRecursive(dataContext.FilteredList.Where(s => CurrentChangeType.HasFlag(s.ChangeType))));

            SetEmptyValue(dataContext);

        }

        private void ComboBox_OnTypeChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = ((ComboBox) sender);

            if (comboBox.SelectedItem is WatcherChangeTypes item)
            {
                CurrentChangeType = item;

                if (!(comboBox.DataContext is SnapshotOverviewViewModel dataContext)) return;

                dataContext.TreeView = new ObservableCollection<FileTree>(
                    SnapshotOverviewViewModel.ParseRecursive(dataContext.FilteredList.Where(s => (string.IsNullOrEmpty(Search) 
                                                                                                  || s.FullPath.ToLower().Contains(Search)) 
                                                                                                 && CurrentChangeType.HasFlag(s.ChangeType))));

                SetEmptyValue(dataContext);
            }
        }


        private void SetEmptyValue(SnapshotOverviewViewModel dataContext)
        {
            if (dataContext.TreeView.Count == 0)
            {
                dataContext.TreeView.Add(new FileTree
                {
                    RealName = "Nothing to show"
                });
            }
        }

        private async void TreeItemDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = ((TreeViewItem) sender);

            var dataContext = item?.DataContext as FileTree;

            if (item?.IsSelected == true && dataContext?.RealPath != null)
            {
                try
                {

                    var attr = File.GetAttributes(dataContext.RealPath);

                    //detect whether its a directory or file
                    if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                        Process.Start(dataContext.RealPath);
                    else
                        Process.Start("explorer.exe",$"/select, {dataContext.RealPath}");
                }
                catch (Exception exception)
                {
                    await this.ShowMessageAsync("Error", exception.Message);
                }

                e.Handled = true;
            }
        }
    }
}
