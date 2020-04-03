using System;

using System.Collections.ObjectModel;

using System.IO;
using System.Windows;
using HomeWork.Model;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MVVMbasics.Attributes;

namespace HomeWork.ViewModel
{
    public class  SnapshotsViewModel : BaseModel
    {
        public FileSystemWatcher FieFileSystemWatcher { get; }

        private MetroWindow MainWindow => Application.Current.MainWindow as MetroWindow;


        private bool _isRecordedStarted { get; set; }

        public bool IsRecordingStarted
        {
            get => _isRecordedStarted;
            set
            {
                _isRecordedStarted = value;

                OnPropertyChanged(nameof(IsRecordingStarted));
            }
        }

        private bool _isDirectoryChoosed => !string.IsNullOrEmpty(_directoryChoosed);

        private string _directoryChoosed { get; set; }

        public string DirectoryChoosed
        {
            get => _directoryChoosed;
            set
            {
                _directoryChoosed = value;
                OnPropertyChanged(nameof(DirectoryChoosed));
            }
        }


        private bool _isRecordedFinished { get; set; } 

        public bool IsRecordingFinished
        {
            get => _isRecordedFinished;
            set
            {
                _isRecordedFinished = value;

                OnPropertyChanged(nameof(IsRecordingFinished));
            }
        }

        public SnapshotsViewModel()
        {
            Snapshots = new ObservableCollection<Snapshot>();
            
            FieFileSystemWatcher = new FileSystemWatcher
            {
                NotifyFilter = NotifyFilters.LastWrite
                               | NotifyFilters.FileName
                               | NotifyFilters.DirectoryName
                               | NotifyFilters.CreationTime
            };

            FieFileSystemWatcher.Changed += OnChanged;
            FieFileSystemWatcher.Created += OnChanged;
            FieFileSystemWatcher.Deleted += OnChanged;
            FieFileSystemWatcher.Renamed += OnRenamed;

            _isRecordedFinished = true;
        }


        private Snapshot currentSnapshot { get; set; }

        public Snapshot CurrentSnapshot
        {
            get => currentSnapshot;
            set => currentSnapshot = value;
        }


        private ObservableCollection<Snapshot> _snapshots { get; set; }

        public ObservableCollection<Snapshot> Snapshots
        {
            get => _snapshots;
            set
            {
                _snapshots = value;
                OnPropertyChanged(nameof(Snapshots));
            }
        }

        private BaseCommand _startCommand;
        public BaseCommand StartCommand
        {
            get
            {
                return _startCommand ??
                       (_startCommand = new BaseCommand(async obj =>
                       {
                           try
                           {
                               IsRecordingStarted = true;
                               IsRecordingFinished = false;
                               CurrentSnapshot = new Snapshot();
                               FieFileSystemWatcher.Path = _directoryChoosed;
                               FieFileSystemWatcher.EnableRaisingEvents = true;
                               FieFileSystemWatcher.IncludeSubdirectories = true;
                           }
                           catch (Exception e)
                           {
                               IsRecordingStarted = false;
                               IsRecordingFinished = true;
                               FieFileSystemWatcher.EnableRaisingEvents = false;
                               await MainWindow.ShowMessageAsync("Error", e.Message);
                           }
                       }, b => _isRecordedFinished && _isDirectoryChoosed));
            }
        }


        private BaseCommand _stopCommand;
        public BaseCommand StopCommand
        {
            get
            {
                return _stopCommand ??
                       (_stopCommand = new BaseCommand(async obj
                           =>
                       {
                           IsRecordingStarted = false;
                           IsRecordingFinished = true;
                           FieFileSystemWatcher.EnableRaisingEvents = false;

                           var text = await MainWindow.ShowInputAsync("Snapshot name", "Enter name");

                            CurrentSnapshot.Name = text ?? "Random name";
                            CurrentSnapshot.DateCreating = DateTime.Now;
                           Snapshots.Add(CurrentSnapshot);
                       }, b => _isRecordedStarted && _isDirectoryChoosed));

            }
        }

        



        private void OnChanged(object source, FileSystemEventArgs e)
        {
            if(e.Name.ToLower().Contains("$recycle.bin"))
                return;

            try 
            {
                var type = e.ChangeType == WatcherChangeTypes.Deleted
                    ? !Path.HasExtension(e.FullPath) ? ElemetnType.Directory : ElemetnType.File
                    : (File.GetAttributes(e.FullPath) & FileAttributes.Directory) == FileAttributes.Directory
                        ? ElemetnType.Directory
                        : ElemetnType.File;

                if(type == ElemetnType.Directory && e.ChangeType == WatcherChangeTypes.Changed)
                    return;

                currentSnapshot.Changes.Add(new Changes
                {
                    ChangeType = e.ChangeType,
                    Type = type,
                    DateTimeAction = DateTime.Now,
                    Name = Path.GetFileName(e.Name),
                    ParentName = Path.GetDirectoryName(e.FullPath),
                    FullPath = e.FullPath
                });
            }
            catch (Exception)
            {
                //ignore
            }
        }
           

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            if (e.Name.ToLower().Contains("$recycle.bin"))
                return;

            try
            {
                currentSnapshot.Changes.Add(new Changes
                {
                    ChangeType = e.ChangeType,
                    Type = (File.GetAttributes(e.FullPath) & FileAttributes.Directory) == FileAttributes.Directory
                        ? ElemetnType.Directory
                        : ElemetnType.File,
                    DateTimeAction = DateTime.Now,
                    OldName = e.OldName,
                    OldFullPath = e.OldFullPath,
                    Name = Path.GetFileName(e.Name),
                    ParentName = Path.GetDirectoryName(e.FullPath),
                    FullPath = e.FullPath
                });
            }
            catch (Exception)
            {
                //ignore
            }
        }
    }
}
