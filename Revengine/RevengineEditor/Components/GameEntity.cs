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
    internal class Grievance : ViewModelBase
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

        public Grievance(Scene scene)
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

    internal abstract class MSObject : ViewModelBase
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
        public List<Grievance> SelectedGrievances { get; }

        public MSObject(List<Grievance> grievances)
        {
            Debug.Assert(grievances?.Any() == true);
            Motivators = new ReadOnlyObservableCollection<IMSMotivator>(_motivators);
            SelectedGrievances = grievances;
            PropertyChanged += (s, e) =>
            {
                if(_updatesEnabled)
                    UpdateGrievances(e.PropertyName);
            };

        }

        public static float? GetMixedValue(List<Grievance> grievances, Func<Grievance, float> getProperty)
        {
            float value = getProperty(grievances.First());

            // If we find a Grievance with a non-uniform value, return null
            foreach (Grievance grievance in grievances.Skip(1))
            {
                // Use custom comparison method for precision
                if (value.IsTheSameAs(getProperty(grievance)))
                {
                    return null;
                }
            }

            // Return the value
            return value;
        }

        public static bool? GetMixedValue(List<Grievance> grievances, Func<Grievance, bool> getProperty)
        {
            bool value = getProperty(grievances.First());

            // If we find a Grievance with a non-uniform value, return null
            foreach (Grievance grievance in grievances.Skip(1))
            {
                // Use custom comparison method for precision
                if (value != getProperty(grievance))
                {
                    return null;
                }
            }

            // Return the value
            return value;
        }

        public static string GetMixedValue(List<Grievance> grievances, Func<Grievance, string> getProperty)
        {
            string value = getProperty(grievances.First());

            // If we find an Grievance with a non-uniform value, return null
            foreach (Grievance grievance in grievances.Skip(1))
            {
                // Use custom comparison method for precision
                if (value != getProperty(grievance))
                {
                    return null;
                }
            }

            // Return the value
            return value;
        }

        /// <summary>
        /// Refresh and update the selected Grievances
        /// </summary>
        public void Refresh()
        {
            // Disable updates
            _updatesEnabled = false;

            // Update grievances
            UpdateMSGrievance();

            // Enable updates
            _updatesEnabled = true;
        }

        /// <summary>
        /// Update the values of each selected Grievance
        /// </summary>
        /// <param name="propertyName">The property to update</param>
        /// <returns>True if the property updates, false if not</returns>
        protected virtual bool UpdateGrievances(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(IsEnabled):
                    SelectedGrievances.ForEach(x => x.IsEnabled = IsEnabled.Value);
                    return true;

                case nameof(Name):
                    SelectedGrievances.ForEach(x => x.Name = Name);
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Update the multiselected Grievance
        /// </summary>
        /// <returns>True if updated completed</returns>
        protected virtual bool UpdateMSGrievance()
        {
            IsEnabled = GetMixedValue(SelectedGrievances, new Func<Grievance, bool>(x => x.IsEnabled));
            Name = GetMixedValue(SelectedGrievances, new Func<Grievance, string>(x => x.Name));

            return true;
        }
    }

    internal class MSGrievance : MSObject
    {
        public MSGrievance(List<Grievance> grievances) : base(grievances)
        {
            // Get all the selected data from the selected Grievances
            Refresh();
        }
    }
}
