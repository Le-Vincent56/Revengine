using RevengineEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RevengineEditor.GameProject
{
    [DataContract(Name = "Game")]
    public class Project : ViewModelBase
    {
        public static string Extension { get; } = ".revengine";

        [DataMember]
        public string Name { get; private set; } = "New Project";

        [DataMember]
        public string Path { get; private set; }

        public string FullPath { get { return $"{Path}{Name}{Extension}"; } }

        [DataMember(Name = "Scenes")]
        private ObservableCollection<Scene> _scenes = new ObservableCollection<Scene>();
        public ReadOnlyObservableCollection<Scene> Scenes { get; private set; }
        private Scene _activeScene;
        public Scene ActiveScene { 
            get { return _activeScene; }
            set
            {
                if(_activeScene != value )
                {
                    _activeScene = value;
                    OnPropertyChanged(nameof(ActiveScene));
                }
            }
        }
        public static Project Current { get { return Application.Current.MainWindow.DataContext as Project; } }

        public Project(string name, string path)
        {
            Name = name;
            Path = path;

            OnDeserialized(new StreamingContext());
        }

        /// <summary>
        /// Load a project from a file path
        /// </summary>
        /// <param name="file">The file path of the project</param>
        /// <returns>A Project</returns>
        public static Project LoadProject(string filePath)
        {
            Debug.Assert(File.Exists(filePath));
            return Serializer.FromFile<Project>(filePath);
        }

        /// <summary>
        /// Save a project
        /// </summary>
        /// <param name="project">The project to save</param>
        public static void Save(Project project)
        {
            Serializer.ToFile(project, project.FullPath);
        }

        /// <summary>
        /// Handle the rest of the deserialization after Save()
        /// </summary>
        /// <param name="context"></param>
        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
        {
            // Construct scenes if there are scenes
            if(_scenes != null)
            {
                Scenes = new ReadOnlyObservableCollection<Scene>(_scenes);
                OnPropertyChanged(nameof(Scenes));
            }
            ActiveScene = Scenes.FirstOrDefault(x => x.IsActive);
        }

        public void UnloadProject()
        {

        }
    }
}
