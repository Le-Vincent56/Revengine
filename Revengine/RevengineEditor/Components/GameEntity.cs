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
    internal class GameEntity : ViewModelBase
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
        }
    }

    internal abstract class MSEntity : ViewModelBase
    {
        private bool _updatesEnabled = true;
        private string _name;
        private bool? _isEnabled = true;
        private readonly ObservableCollection<IMSMotivator> _motivators = new ObservableCollection<IMSMotivator>();
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
        public bool? IsEnabled
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
        public ReadOnlyObservableCollection<IMSMotivator> Motivators { get; private set; }
        public List<GameEntity> SelectedEntities { get; }

        public MSEntity(List<GameEntity> entities)
        {
            Debug.Assert(entities?.Any() == true);
            Motivators = new ReadOnlyObservableCollection<IMSMotivator>(_motivators);
            SelectedEntities = entities;
            PropertyChanged += (s, e) =>
            {
                if(_updatesEnabled)
                    UpdateGameEntities(e.PropertyName);
            };

        }

        public static float? GetMixedValue(List<GameEntity> entities, Func<GameEntity, float> getProperty)
        {
            float value = getProperty(entities.First());

            // If we find an Entity with a non-uniform value, return null
            foreach (GameEntity entity in entities.Skip(1))
            {
                // Use custom comparison method for precision
                if (value.IsTheSameAs(getProperty(entity)))
                {
                    return null;
                }
            }

            // Return the value
            return value;
        }

        public static bool? GetMixedValue(List<GameEntity> entities, Func<GameEntity, bool> getProperty)
        {
            bool value = getProperty(entities.First());

            // If we find an Entity with a non-uniform value, return null
            foreach (GameEntity entity in entities.Skip(1))
            {
                // Use custom comparison method for precision
                if (value != getProperty(entity))
                {
                    return null;
                }
            }

            // Return the value
            return value;
        }

        public static string GetMixedValue(List<GameEntity> entities, Func<GameEntity, string> getProperty)
        {
            string value = getProperty(entities.First());

            // If we find an Entity with a non-uniform value, return null
            foreach (GameEntity entity in entities.Skip(1))
            {
                // Use custom comparison method for precision
                if (value != getProperty(entity))
                {
                    return null;
                }
            }

            // Return the value
            return value;
        }

        /// <summary>
        /// Refresh and update the selected game entities
        /// </summary>
        public void Refresh()
        {
            // Disable updates
            _updatesEnabled = false;

            // Update entities
            UpdateMSGameEntity();

            // Enable updates
            _updatesEnabled = true;
        }

        /// <summary>
        /// Update the values of each selected Game Entity
        /// </summary>
        /// <param name="propertyName">The property to update</param>
        /// <returns>True if the property updates, false if not</returns>
        protected virtual bool UpdateGameEntities(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(IsEnabled):
                    SelectedEntities.ForEach(x => x.IsEnabled = IsEnabled.Value);
                    return true;

                case nameof(Name):
                    SelectedEntities.ForEach(x => x.Name = Name);
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Update the multiselected game entity
        /// </summary>
        /// <returns>True if updated completed</returns>
        protected virtual bool UpdateMSGameEntity()
        {
            IsEnabled = GetMixedValue(SelectedEntities, new Func<GameEntity, bool>(x => x.IsEnabled));
            Name = GetMixedValue(SelectedEntities, new Func<GameEntity, string>(x => x.Name));

            return true;
        }
    }

    internal class MSGameEntity : MSEntity
    {
        public MSGameEntity(List<GameEntity> entities) : base(entities)
        {
            // Get all the selected data from the selected entities
            Refresh();
        }
    }
}
