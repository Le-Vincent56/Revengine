﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RevengineEditor.GameProject
{
    [DataContract(Name = "Game")]
    public class Project : ViewModelBase
    {
        public static string Extension { get; } = ".revengine";

        [DataMember]
        public string Name { get; private set; }

        [DataMember]
        public string Path { get; private set; }

        public string FullPath { get { return $"{Path}{Name}{Extension}"; } }

        [DataMember(Name = "Scenes")]
        private ObservableCollection<Scene> _scenes = new ObservableCollection<Scene>();
        private ReadOnlyObservableCollection<Scene> Scenes { get; }

        public Project(string name, string path)
        {
            Name = name;
            Path = path;

            _scenes.Add(new Scene("Default Scene", this));
        }
    }
}
