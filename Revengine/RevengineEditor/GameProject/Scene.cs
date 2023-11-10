using RevengineEditor.Components;
using RevengineEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Input;

namespace RevengineEditor.GameProject
{
    [DataContract]
    internal class Scene : ViewModelBase
    {
        private string _name;
        private bool _isActive;

        [DataMember]
        public string Name
        {
            get { return _name; }
            set
            {
                if(_name != value)
                {
                    _name = value;
                }
                OnPropertyChanged(nameof(Name));
            }
        }

        [DataMember]
        public Project Project { get; private set; }

        [DataMember]
        public bool IsActive { 
            get { return _isActive; }
            set
            {
                if(_isActive != value)
                {
                    _isActive = value;
                    OnPropertyChanged(nameof(IsActive));
                }
            }
        }

        [DataMember(Name = nameof(GameEntities))]
        private readonly ObservableCollection<GameEntity> _gameEntities = new ObservableCollection<GameEntity>();
        public ReadOnlyObservableCollection<GameEntity> GameEntities { get; private set; }

        public ICommand AddGameEntityCommand { get; private set; }
        public ICommand RemoveGameEntityCommand { get; private set; }

        public Scene(string name, Project project)
        {
            Name = name;

            Debug.Assert(project != null);
            Project = project;

            OnDeserialized(new StreamingContext());
        }

        /// <summary>
        /// Add a Game Entity to the Scene
        /// </summary>
        /// <param name="entity">The Game Entity to add</param>
        private void AddGameEntity(GameEntity entity)
        {
            Debug.Assert(!_gameEntities.Contains(entity));
            _gameEntities.Add(entity);
        }

        /// <summary>
        /// Remove a Game Entity from the Scene
        /// </summary>
        /// <param name="entity">The Game Entity to remove</param>
        private void RemoveGameEntity(GameEntity entity)
        {
            Debug.Assert(_gameEntities.Contains(entity));
            _gameEntities.Remove(entity);
        }

        /// <summary>
        /// Handle the rest of the deserialization after Save()
        /// </summary>
        /// <param name="context"></param>
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            // Construct scenes if there are scenes
            if (_gameEntities != null)
            {
                GameEntities = new ReadOnlyObservableCollection<GameEntity>(_gameEntities);
                OnPropertyChanged(nameof(GameEntities));
            }

            // Initialize Add Game Entity Command
            AddGameEntityCommand = new RelayCommand<GameEntity>(x =>
            {
                // Add the scene
                AddGameEntity(x);

                // Remember the index
                int entityIndex = _gameEntities.Count - 1;

                // Add the Add Scene Undo and Redo commands
                Project.UndoRedo.Add(new UndoRedoAction(
                    () => RemoveGameEntity(x),
                    () => _gameEntities.Insert(entityIndex, x),
                    $"Add {x.Name} to {Name}"));
            });

            // Initialize Remove Game Entity Command
            RemoveGameEntityCommand = new RelayCommand<GameEntity>(x =>
            {
                // Get the scene index of the scene to remove
                int gameEntityIndex = _gameEntities.IndexOf(x);

                // Remove the scene
                RemoveGameEntity(x);

                // Add the Remove Scene Undo and Redo commands
                Project.UndoRedo.Add(new UndoRedoAction(
                    () => _gameEntities.Insert(gameEntityIndex, x),
                    () => RemoveGameEntity(x),
                    $"Removed {x.Name} from {Name}"));
            });
        }
    }
}
