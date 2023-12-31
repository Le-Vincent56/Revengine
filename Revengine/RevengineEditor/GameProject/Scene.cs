﻿using RevengineEditor.Components;
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

        [DataMember(Name = nameof(Grievances))]
        private readonly ObservableCollection<Grievance> _grievances = new ObservableCollection<Grievance>();
        public ReadOnlyObservableCollection<Grievance> Grievances { get; private set; }

        public ICommand AddGrievanceCommand { get; private set; }
        public ICommand RemoveGrievanceCommand { get; private set; }

        public Scene(string name, Project project)
        {
            Name = name;

            Debug.Assert(project != null);
            Project = project;

            OnDeserialized(new StreamingContext());
        }

        /// <summary>
        /// Add a Grievance to the Scene
        /// </summary>
        /// <param name="grievance">The Grievance to add</param>
        private void AddGrievance(Grievance grievance, int index = -1)
        {
            Debug.Assert(!_grievances.Contains(grievance));

            // Set the Grievance to Active if the Scene is Active
            grievance.IsActive = IsActive;

            // Check if an index is given
            if(index == -1)
            {
                // If not, add the Grievance to the end of the collection of Grievances
                _grievances.Add(grievance);
            } else
            {
                // Otherwise, insert the Grievance into the collection of Grievances
                // at the given index
                _grievances.Insert(index, grievance);
            }
        }

        /// <summary>
        /// Remove a Grievance from the Scene
        /// </summary>
        /// <param name="grievance">The Grievance to remove</param>
        private void RemoveGrievance(Grievance grievance)
        {
            Debug.Assert(_grievances.Contains(grievance));

            // Deactivate the Grievance
            grievance.IsActive = false;

            // Remove the Grievance from the collection
            _grievances.Remove(grievance);
        }

        /// <summary>
        /// Handle the rest of the deserialization after Save()
        /// </summary>
        /// <param name="context"></param>
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            // Construct scenes if there are scenes
            if (_grievances != null)
            {
                Grievances = new ReadOnlyObservableCollection<Grievance>(_grievances);
                OnPropertyChanged(nameof(Grievances));
            }

            // Activate all Grievances so they can load into the engine
            foreach(Grievance grievance in _grievances)
            {
                grievance.IsActive = true;
            }

            // Initialize Add Grievance Command
            AddGrievanceCommand = new RelayCommand<Grievance>(x =>
            {
                // Add the scene
                AddGrievance(x);

                // Remember the index
                int grievanceIndex = _grievances.Count - 1;

                // Add the Add Scene Undo and Redo commands
                Project.UndoRedo.Add(new UndoRedoAction(
                    () => RemoveGrievance(x),
                    () => AddGrievance(x, grievanceIndex),
                    $"Add {x.Name} to {Name}"));
            });

            // Initialize Remove Grievance Command
            RemoveGrievanceCommand = new RelayCommand<Grievance>(x =>
            {
                // Get the scene index of the scene to remove
                int grievanceIndex = _grievances.IndexOf(x);

                // Remove the scene
                RemoveGrievance(x);

                // Add the Remove Scene Undo and Redo commands
                Project.UndoRedo.Add(new UndoRedoAction(
                    () => AddGrievance(x, grievanceIndex),
                    () => RemoveGrievance(x),
                    $"Removed {x.Name} from {Name}"));
            });
        }
    }
}
