using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VsSolutionGenerator.DevSCR
{

    internal class FolderItem : INotifyPropertyChanged
    {
        private string _name;
        public string Name {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
                }
            }
        }

        public bool IsExpanded { get; set; }

        public string Path { get; set; }

        private bool _isFolder = false;
        public bool IsFolder {
            get => _isFolder;
            set
            {
                if (_isFolder != value)
                {
                    _isFolder = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLeaf)));
                }
            }
        }

        public bool IsLeaf => !IsFolder && SubEntries.Count == 0;

        public FolderItem Parent { get; set; }

        public ObservableCollection<FolderItem> SubEntries { get; }

        public FolderItem()
        {
            SubEntries = new ObservableCollection<FolderItem>();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
