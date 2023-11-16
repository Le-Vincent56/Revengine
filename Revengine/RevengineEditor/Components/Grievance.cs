using RevengineEditor.DLLWrappers;
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
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;

namespace RevengineEditor.Components
{
    [DataContract]
    [KnownType(typeof(Transform))]
    internal class Grievance : ViewModelBase
    {
        private int _grievanceId = ID.INVALID_ID;
        public int GrievanceID
        {
            get { return _grievanceId; }
            set
            {
                if(_grievanceId != value)
                {
                    _grievanceId = value;
                    OnPropertyChanged(nameof(GrievanceID));
                }
            }
        }
        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    if(_isActive)
                    {
                        GrievanceID = EngineAPI.CreateGrievance(this);
                        Debug.Assert(ID.Isvalid(_grievanceId));
                    } else if(ID.Isvalid(GrievanceID))
                    {
                        EngineAPI.RemoveGrievance(this);
                        GrievanceID = ID.INVALID_ID;
                    }
                    OnPropertyChanged(nameof(IsActive));
                }
            }
        }
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

        /// <summary>
        /// Get a Motivator of a certain Type in the Grievance
        /// </summary>
        /// <param name="type">The Type of Motivator to retrieve</param>
        /// <returns>A Motivator of the asked Type, null if it doesn't exist</returns>
        public Motivator GetMotivator(Type type)
        {
            return Motivators.FirstOrDefault(m => m.GetType() == type);
        }

        /// <summary>
        /// Get a Motivator of a certain Type and automatically cast it
        /// </summary>
        /// <typeparam name="T">The Type of Motivator to retreive and cast to</typeparam>
        /// <returns>A Motivator of Type T, null if it doesn't exist</returns>
        public T GetMotivator<T>() where T : Motivator
        {
            return GetMotivator(typeof(T)) as T;
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

        public T GetMSComponent<T>() where T : IMSMotivator
        {
            return (T)Motivators.FirstOrDefault(x => x.GetType() == typeof(T));
        }

        public static float? GetMixedValue<T>(List<T> objects, Func<T, float> getProperty)
        {
            float value = getProperty(objects.First());

            // If we find an object with a non-uniform value, return null, otherwise, return the value
            return objects.Skip(1).Any(x => !getProperty(x).IsTheSameAs(value)) ? (float?)null : value;
        }

        public static bool? GetMixedValue<T>(List<T> objects, Func<T, bool> getProperty)
        {
            bool value = getProperty(objects.First());

            // If we find an object with a non-uniform value, return null, otherwise, return the value
            return objects.Skip(1).Any(x => value != getProperty(x)) ? (bool?)null : value;
        }

        public static string GetMixedValue<T>(List<T> objects, Func<T, string> getProperty)
        {
            string value = getProperty(objects.First());

            // If we find an object with a non-uniform value, return null, otherwise, return the value
            return objects.Skip(1).Any(x => value != getProperty(x)) ? null : value;
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

            // Create the component list
            MakeMotivatorList();

            // Enable updates
            _updatesEnabled = true;
        }

        /// <summary>
        /// Compile all the Motivators to be included in the Motivator List
        /// </summary>
        private void MakeMotivatorList()
        {
            // Clear the list
            _motivators.Clear();

            // Select the first selected Grieevance
            Grievance firstGrievance = SelectedGrievances.FirstOrDefault();

            // If null, return
            if (firstGrievance == null) return;
            
            // Go through its list of Motivators
            foreach(Motivator motivator in firstGrievance.Motivators)
            {
                // Select the Motivator's type
                Type type = motivator.GetType();

                // Check if any of the other Grievances have the same Motivator Type
                if(!SelectedGrievances.Skip(1).Any(grievance => grievance.GetMotivator(type) == null))
                {
                    // If so, retrieve it and add it to the list of Motivators
                    Debug.Assert(Motivators.FirstOrDefault(x => x.GetType() == type) == null);

                    // Need to include the MuultiSelection variant
                    _motivators.Add(motivator.GetMultiSelectionMotivator(this));
                }
            }

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
