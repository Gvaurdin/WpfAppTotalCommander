using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using WpfAppTotalCommander.Core;
using WpfAppTotalCommander.MVVM.Model;
using WpfAppUsersChat.MVVM.Core;

namespace WpfAppTotalCommander.MVVM.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private ObservableCollection<DriveItem> _drives;
        private ObservableCollection<FileSystemItem> _leftPanelItems;
        private ObservableCollection<FileSystemItem> _rightPanelItems;
        private ObservableCollection<FileSystemItem> _copyItems;
        private string _currentLeftDirectory;
        private string _currentRightDirectory;
        private string _defaultEditor = "notepad.exe";
        private bool _hidden = true;
        private bool _isLeftPanel = true;
        private bool _isCutFile = false;
        private string cutPath;

        // Свойства
        public ObservableCollection<DriveItem> Drives
        {
            get { return _drives; }
            set
            {
                _drives = value;
                OnPropertyChanged();
            }
        }


        public ObservableCollection<FileSystemItem> LeftPanelItems
        {
            get { return _leftPanelItems; }
            set
            {
                _leftPanelItems = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<FileSystemItem> RightPanelItems
        {
            get { return _rightPanelItems; }
            set
            {
                _rightPanelItems = value;
                OnPropertyChanged();
            }
        }

        public string CurrentLeftDirectory
        {
            get { return _currentLeftDirectory; }
            set
            {
                _currentLeftDirectory = value;
                OnPropertyChanged();
            }
        }

        public string CurrentRightDirectory
        {
            get { return _currentRightDirectory; }
            set
            {
                _currentRightDirectory = value;
                OnPropertyChanged();
            }
        }

        public string DefaultEditor
        {
            get { return _defaultEditor; }
            set
            {
                _defaultEditor = value;
                OnPropertyChanged();
            }
        }

        public bool Hidden
        {
            get { return _hidden; }
            set
            {
                _hidden = value;
                OnPropertyChanged();
            }
        }

        public bool IsLeftPanel
        {
            get { return _isLeftPanel; }
            set
            {
                _isLeftPanel = value;
                OnPropertyChanged();
            }
        }

        private DriveItem _selectedLeftDrive;
        public DriveItem SelectedLeftDrive
        {
            get { return _selectedLeftDrive; }
            set
            {
                _selectedLeftDrive = value;
                OnPropertyChanged();
            }
        }

        private DriveItem _selectedRightDrive;
        public DriveItem SelectedRightDrive
        {
            get { return _selectedRightDrive; }
            set
            {
                _selectedRightDrive = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<string> _driveLabels;
        public ObservableCollection<string> DriveLabels
        {
            get { return _driveLabels; }
            set
            {
                _driveLabels = value;
                OnPropertyChanged();
            }
        }

        private Stack<string> _historyLeft = new Stack<string>();
        private Stack<string> _historyRight = new Stack<string>();

        private FileSystemItem _selectedLeftItem;
        public FileSystemItem SelectedLeftItem
        {
            get { return _selectedLeftItem; }
            set
            {
                _selectedLeftItem = value;
                OnPropertyChanged();
            }
        }

        private FileSystemItem _selectedRightItem;
        public FileSystemItem SelectedRightItem
        {
            get { return _selectedRightItem; }
            set
            {
                _selectedRightItem = value;
                OnPropertyChanged();
            }
        }

        // Конструктор
        public MainViewModel()
        {
            Drives = new ObservableCollection<DriveItem>();
            LeftPanelItems = new ObservableCollection<FileSystemItem>();
            RightPanelItems = new ObservableCollection<FileSystemItem>();
            DriveLabels = new ObservableCollection<string>();
            _copyItems = new ObservableCollection<FileSystemItem>();
            LoadDrives();
            ViewFileCommand = new RelayTypeCommand<FileSystemItem>(ViewFile);
            DeleteCommand = new RelayTypeCommand<string>(Delete);
            ReturnPreviousPath = new RelayCommand(GoBack);
            CopyCommand = new RelayCommand(CopySelectedItem);
            PasteCommand = new RelayCommand(PasteSelectedItem);
            CutCommand = new RelayCommand(CutSelectedItem);
            LoadDirectoryCommand = new RelayTypeCommand<string>(LoadDirectory);
            SelectLeftDriveCommand = new RelayTypeCommand<object>(SelectLeftDrive);
            SelectRightDriveCommand = new RelayTypeCommand<object>(SelectRightDrive);
            FocusControlLeftCommand = new RelayCommand(FocusLeftControl);
            FocusControlRightCommand = new RelayCommand(FocusRightControl);

        }

        // Методы
        private void LoadDrives()
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                Drives.Add(new DriveItem(drive));
                DriveLabels.Add(drive.Name);
            }
        }

        public async void FocusLeftControl()
        {
            await Task.Run(() =>
            {
                _isLeftPanel = true;
            });
        }

        public async void FocusRightControl()
        {
            await Task.Run(() =>
            {
                _isLeftPanel = false;
            });
        }

        private async void SelectLeftDrive(object selectedItem)
        {
            string selectedDriveLabel = selectedItem.ToString();
            DriveItem selectedDrive = Drives.FirstOrDefault(d => d.Drive.Name == selectedDriveLabel);
            SelectedLeftDrive = selectedDrive;
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                LoadDirectory(SelectedLeftDrive.Drive.RootDirectory.FullName);
            });

        }

        private async void SelectRightDrive(object selectedItem)
        {
            string selectedDriveLabel = selectedItem.ToString();
            DriveItem selectedDrive = Drives.FirstOrDefault(d => d.Drive.Name == selectedDriveLabel);
            SelectedRightDrive = selectedDrive;
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                LoadDirectory(SelectedRightDrive.Drive.RootDirectory.FullName);
            });

        }

        public ICommand LoadDriveCommand { get; }
        public ICommand LoadDirectoryCommand { get; }
        public ICommand ViewFileCommand { get; }
        public RelayTypeCommand<string> DeleteCommand { get; }
        public ICommand ToggleHiddenFilesCommand { get; }
        public ICommand SetDefaultEditorCommand { get; }
        public ICommand LoadSelectedLeftDriveCommand { get; }
        public ICommand LoadSelectedRightDriveCommand { get; }
        public ICommand ReturnPreviousPath { get; }
        public ICommand CopyCommand { get; }
        public ICommand PasteCommand { get; }
        public ICommand CutCommand { get; }
        public ICommand SelectLeftDriveCommand { get; }
        public ICommand SelectRightDriveCommand { get; }
        public ICommand FocusControlLeftCommand { get; }
        public ICommand FocusControlRightCommand { get; }


        public void LoadDirectory(string path)
        {

            ObservableCollection<FileSystemItem> items;
            if (IsLeftPanel)
            {
                _historyLeft.Push(path);
                CurrentLeftDirectory = path;
                LeftPanelItems.Clear();
                items = LeftPanelItems;
            }
            else
            {
                _historyRight.Push(path);
                CurrentRightDirectory = path;
                RightPanelItems.Clear();
                items = RightPanelItems;
            }

            try
            {
                DirectoryInfo directory = new DirectoryInfo(path);
                if (Directory.Exists(directory.FullName))
                {
                    foreach (var dir in directory.GetDirectories())
                    {
                        if (!Hidden || (Hidden && (dir.Attributes & FileAttributes.Hidden) == 0))
                        {
                            AddFileSystemItem(dir, items);
                        }
                    }

                    foreach (var file in directory.GetFiles())
                    {
                        if (!Hidden || (Hidden && (file.Attributes & FileAttributes.Hidden) == 0))
                        {
                            AddFileSystemItem(file, items);
                        }
                    }
                }

                else
                {
                    FileInfo info = new FileInfo(path);
                    if (File.Exists(info.FullName))
                    {
                        FileSystemItem fileSystemItem = new FileSystemItem(info);
                        ViewFile(fileSystemItem);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error :{ex.Message}");
            }
        }

        private void AddFileSystemItem(FileSystemInfo info, ObservableCollection<FileSystemItem> items)
        {
            if (items != null)
            {
                items.Add(new FileSystemItem(info));
            }
        }

        private void GoBack()
        {
            string previousPath = null;
            if (IsLeftPanel)
            {
                if (_historyLeft.Count > 1)
                {
                    _historyLeft.Pop();
                    previousPath = _historyLeft.Pop();
                }
            }
            else
            {
                if (_historyRight.Count > 1)
                {
                    _historyRight.Pop();
                    previousPath = _historyRight.Pop();
                }
            }

            if (previousPath != null)
            {
                LoadDirectory(previousPath);
            }
        }

        private FileSystemItem SelectItem(string path)
        {
            FileSystemItem systemItem = null;

            if (!string.IsNullOrEmpty(path) && _isCutFile)
            {
                FileInfo info = new FileInfo(path);
                systemItem = new FileSystemItem(info);
            }
            else
            {
                systemItem = IsLeftPanel ? SelectedLeftItem : SelectedRightItem;
            }

            return systemItem;
        }

        private void Delete(object path)
        {

            FileSystemItem selectedItem = SelectItem((string)path);
            if (selectedItem != null)


            {
                try
                {
                    if (Directory.Exists(selectedItem.RootPath))
                    {
                        // Если элемент является папкой, проверяем, содержит ли она файлы или другие подпапки
                        DirectoryInfo directoryInfo = new DirectoryInfo(selectedItem.Info.FullName);
                        bool hasFiles = directoryInfo.EnumerateFiles().Any();
                        bool hasDirectories = directoryInfo.EnumerateDirectories().Any();

                        if (hasFiles || hasDirectories)
                        {
                            // Если папка содержит файлы или подпапки, выводим предупреждающее сообщение
                            var result = MessageBox.Show("The folder contains files or subfolders. Deleting it will also delete its contents. Are you sure you want to continue?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                            if (result == MessageBoxResult.Yes)
                            {
                                // Если пользователь подтвердил удаление, удаляем папку со всем содержимым
                                directoryInfo.Delete(true);
                                if (IsLeftPanel)
                                {
                                    LeftPanelItems.Remove(selectedItem);
                                }
                                else
                                {
                                    RightPanelItems.Remove(selectedItem);
                                }
                            }
                            // Если пользователь отменил удаление, ничего не делаем
                        }
                        else
                        {
                            // Если папка пуста, удаляем ее без дополнительных предупреждений
                            directoryInfo.Delete();
                            if (IsLeftPanel)
                            {
                                LeftPanelItems.Remove(selectedItem);
                            }
                            else
                            {
                                RightPanelItems.Remove(selectedItem);
                            }
                        }
                    }
                    else
                    {
                        // Если элемент - файл, удаляем его без дополнительных проверок
                        selectedItem.Info.Delete();
                        if (IsLeftPanel)
                        {
                            LeftPanelItems.Remove(selectedItem);
                        }
                        else
                        {
                            RightPanelItems.Remove(selectedItem);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }

        private void CopySelectedItem()
        {
            try
            {
                FileSystemItem selectedItem = IsLeftPanel ? SelectedLeftItem : SelectedRightItem;
                if (_isCutFile)
                {
                    cutPath = selectedItem.RootPath;
                }
                if (selectedItem != null)
                {
                    _copyItems.Clear();
                    _copyItems.Add(selectedItem);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void CopyFolder(string sourceDir, string destDir)
        {
            if (!Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }

            foreach (string file in Directory.GetFiles(sourceDir, "*.*"))
            {
                string destFile = Path.Combine(destDir, Path.GetFileName(file));
                File.Copy(file, destFile, true);
            }

            foreach (string folder in Directory.GetDirectories(sourceDir))
            {
                string destFolder = Path.Combine(destDir, Path.GetFileName(folder));
                CopyFolder(folder, destFolder);
            }
        }

        private async void CutSelectedItem()
        {
            _isCutFile = true;
            await Task.Run(() =>
            {
                CopySelectedItem();
            });

        }

        private async void PasteSelectedItem()
        {
            try
            {
                string destinationPath = IsLeftPanel ? CurrentLeftDirectory : CurrentRightDirectory;
                if (!string.IsNullOrEmpty(destinationPath) && _copyItems.Count > 0)
                {
                    foreach (var item in _copyItems)
                    {
                        string sourcePath = item.Info.FullName;
                        string destinationFile = Path.Combine(destinationPath, item.Name);
                        if (Directory.Exists(item.RootPath))
                        {
                            // копирование папки
                            if (Directory.Exists(destinationFile))
                            {
                                var result = MessageBox.Show($"A folder with the same name already exists in the destination folder. Do you want to replace it?", "Folder Exists", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                                if (result == MessageBoxResult.Yes)
                                {
                                    // копирование с заменой
                                    CopyFileOrFolder(sourcePath, destinationFile);
                                }
                                else if (result == MessageBoxResult.No)
                                {
                                    continue;
                                }
                                else
                                {
                                    return;
                                }
                            }
                            else
                            {
                                CopyFileOrFolder(sourcePath, destinationFile);
                            }
                        }
                        else
                        {
                            // копирование файла
                            if (File.Exists(destinationFile))
                            {
                                var result = MessageBox.Show($"A file with the same name already exists in the destination folder. Do you want to replace it?", "File Exists", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                                if (result == MessageBoxResult.Yes)
                                {
                                    File.Copy(sourcePath, destinationFile, true);
                                }
                                else if (result == MessageBoxResult.No)
                                {
                                    continue;
                                }
                                else
                                {
                                    return;
                                }
                            }
                            else
                            {
                                File.Copy(sourcePath, destinationFile, true);
                            }
                        }
                    }
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        if (_isCutFile)
                        {
                            Delete(cutPath);
                            Refresh(LeftPanelItems, CurrentLeftDirectory);
                            Refresh(RightPanelItems, CurrentRightDirectory);
                            _copyItems.Clear();
                            _isCutFile = false;
                        }
                        else
                        {
                            _copyItems.Clear();
                            if (IsLeftPanel) Refresh(LeftPanelItems, CurrentLeftDirectory);
                            else Refresh(RightPanelItems, CurrentRightDirectory);
                        }
                    });
                }
                else
                {
                    MessageBox.Show("Please specify the destination folder or select an item to copy.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void CopyFileOrFolder(string sourcePath, string destinationPath)
        {
            try
            {
                FileAttributes attr = File.GetAttributes(sourcePath);
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    CopyFolder(sourcePath, destinationPath);
                }
                else
                {
                    File.Copy(sourcePath, destinationPath, true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        // Методы для обновления и изменения директорий
        public void Refresh(ObservableCollection<FileSystemItem> items, string path)
        {
            try
            {
                if (items != null)
                {
                    items.Clear();
                    DirectoryInfo directory = new DirectoryInfo(path);
                    if (Directory.Exists(directory.FullName))
                    {
                        foreach (var dir in directory.GetDirectories())
                        {
                            if (!Hidden || (Hidden && (dir.Attributes & FileAttributes.Hidden) == 0))
                            {
                                AddFileSystemItem(dir, items);
                            }
                        }

                        foreach (var file in directory.GetFiles())
                        {
                            if (!Hidden || (Hidden && (file.Attributes & FileAttributes.Hidden) == 0))
                            {
                                AddFileSystemItem(file, items);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error : {ex.Message}");
            }

        }


        private void SetDefaultEditor()
        {
            string editorPath = "C:\\Windows\\System32\\notepad.exe";
            DefaultEditor = editorPath;
        }

        private void ViewFile(FileSystemItem fileSystemItem)
        {
            if (fileSystemItem != null && fileSystemItem.Info is FileInfo fileInfo)
            {
                try
                {
                    SetDefaultEditor();
                    Process.Start(DefaultEditor, fileInfo.FullName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }
    }
}
