using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;

namespace RevengineEditor.GameProject
{
    [DataContract]
    public class Scene : ViewModelBase
    {
        private string _name;

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

        public Scene(string name, Project project)
        {
            Name = name;

            Debug.Assert(project != null);
            Project = project;
        }
    }
}
