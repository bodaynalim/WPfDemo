using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using HomeWork.Model;
using MVVMbasics.Attributes;

namespace HomeWork.ViewModel
{
    public class FileTree
    {
        public string RealName { get; set; }
        public string RealPath { get; set; }

        public ElemetnType? Type { get; set; }

        public WatcherChangeTypes? ChangeType { get; set; }

        public List<FileTree> TreeView { get; set; }
    }

    public class SnapshotOverviewViewModel : BaseModel
    {
        private ObservableCollection<Changes> _filteredList { get; set; }

        private ObservableCollection<FileTree> _treeView { get; set; }

        public List<WatcherChangeTypes> ChangeTypes =>
            typeof(WatcherChangeTypes).GetEnumValues().Cast<WatcherChangeTypes>().ToList();

        public ObservableCollection<FileTree> TreeView
        {
            get => _treeView;
            set
            {
                _treeView = value;
                OnPropertyChanged(nameof(TreeView));
            }
        }

        public ObservableCollection<Changes> FilteredList
        {
            get => _filteredList;
            set => _filteredList = value;
        }

        public SnapshotOverviewViewModel(Snapshot snapshot)
        {
            CurrentSnapshot = snapshot;
            FilteredList = new ObservableCollection<Changes>(CurrentSnapshot.Changes);

            TreeView = new ObservableCollection<FileTree>(ParseRecursive(FilteredList));

            if(TreeView.Count == 0)
                TreeView.Add(new FileTree()
                {
                    RealName = "Nothing to show"
                });

        }

        public  static List<FileTree> ParseRecursive(IEnumerable<Changes> files, string root = "", char separator = '\\')
        {
            return
                files.Select(s => new { s, split = s.FullPath.Split(separator) })
                    .GroupBy(t => t.s.FullPath.Split(separator)[0], t => t.s)
                    .Select(g =>
                    {
                        var element = g.FirstOrDefault(s => s.Name.Equals(g.Key, StringComparison.OrdinalIgnoreCase));

                        return new FileTree
                        {
                            RealName = g.Key,
                            RealPath = $"{root}{(!string.IsNullOrEmpty(root) ? separator.ToString() : "")}{g.Key}",
                            TreeView = ParseRecursive(
                                g.Where(s => s.FullPath.Length > g.Key.Length + 1).Select(s =>
                                    new Changes
                                    {
                                        FullPath = s.FullPath.Substring(g.Key.Length + 1),
                                        OldFullPath = s.OldFullPath,
                                        OldName = s.OldName,
                                        Type = s.Type,
                                        ChangeType = s.ChangeType,
                                        Name = s.Name,
                                        DateTimeAction = s.DateTimeAction,
                                        ParentName = s.ParentName
                                    }),
                                $"{root}{(!string.IsNullOrEmpty(root) ? separator.ToString() : "")}{g.Key}"
                            ),
                            Type = element?.Type,
                            ChangeType = element?.ChangeType
                        };
                    }).ToList();
        }

        private Snapshot currentSnapshot { get; set; }

        public Snapshot CurrentSnapshot
        {
            get => currentSnapshot;
            set => currentSnapshot = value;
        }


    }
}
