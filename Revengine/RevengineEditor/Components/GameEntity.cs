using RevengineEditor.GameProject;
using RevengineEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RevengineEditor.Components
{
    [DataContract]
    [KnownType(typeof(Transform))]
    public class GameEntity : ViewModelBase
    {
        private string _name;
        [DataMember]
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        private bool _isEnabled = true;
        [DataMember]
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged(nameof(IsEnabled));
                }
            }
        }

        [DataMember]
        public Scene ParentScene { get; private set; }
        [DataMember(Name = nameof(Motivators))]

        private readonly ObservableCollection<Motivator> _motivators = new ObservableCollection<Motivator>();
        public ReadOnlyObservableCollection<Motivator> Motivators { get; private set; }

        public ICommand RenameCommand { get; private set; }
        public ICommand EnableCommand { get; private set; }

        public GameEntity(Scene scene)
        {
            Debug.Assert(scene != null);
            ParentScene = scene;
            _motivators.Add(new Transform(this));

            OnDeserialized(new StreamingContext());
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if(_motivators != null)
            {
                Motivators = new ReadOnlyObservableCollection<Motivator>(_motivators);
                OnPropertyChanged(nameof(Motivators));
            }

            // Initialize the Rename Command to be undoable
            RenameCommand = new RelayCommand<string>(x =>
            {
                // Store the old name and set a new name
                string oldName = _name;
                Name = x;

                Project.UndoRedo.Add(new UndoRedoAction(nameof(Name), this, oldName, x, $"Rename entity '{oldName}' to '{x}'"));
            }, x => x != _name);
        }
    }
}
