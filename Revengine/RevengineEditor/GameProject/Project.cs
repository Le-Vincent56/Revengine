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
using System.Windows.Input;

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

        public static UndoRedo UndoRedo { get; } = new UndoRedo();
        public ICommand Undo { get; private set; }
        public ICommand Redo { get; private set; }

        public ICommand AddScene { get; private set; }
        public ICommand RemoveScene { get; private set; }

        public Project(string name, string path)
        {
            Name = name;
            Path = path;

            OnDeserialized(new StreamingContext());
        }

        /// <summary>
        /// Add a scene to the project
        /// </summary>
        /// <param name="sceneName">The scene name</param>
        private void AddSceneInternal(string sceneName)
        {
            Debug.Assert(!string.IsNullOrEmpty(sceneName.Trim()));
            _scenes.Add(new Scene(sceneName, this));
        }

        /// <summary>
        /// Remove a scene from the project
        /// </summary>
        /// <param name="scene"></param>
        private void RemoveSceneInternal(Scene scene)
        {
            Debug.Assert(_scenes.Contains(scene));
            _scenes.Remove(scene);
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

            AddScene = new RelayCommand<object>(x =>
            {
                // Add the scene
                AddSceneInternal($"New Scene {_scenes.Count}");

                // Get a reference to the new scene
                Scene newScene = _scenes.Last();
                int sceneIndex = _scenes.Count - 1;

                // Add the Add Scene Undo and Redo commands
                UndoRedo.Add(new UndoRedoAction(
                    () => RemoveSceneInternal(newScene),
                    () => _scenes.Insert(sceneIndex, newScene),
                    $"Add {newScene.Name}"));
            });

            RemoveScene = new RelayCommand<Scene>(x =>
            {
                // Get the scene index of the scene to remove
                int sceneIndex = _scenes.IndexOf(x);

                // Remove the scene
                RemoveSceneInternal(x);

                // Add the Remove Scene Undo and Redo commands
                UndoRedo.Add(new UndoRedoAction(
                    () => _scenes.Insert(sceneIndex, x),
                    () => RemoveSceneInternal(x),
                    $"Remove {x.Name}"));
            }, x => !x.IsActive);

            // Assign Undo and Redo
            Undo = new RelayCommand<object>(x => UndoRedo.Undo());
            Redo = new RelayCommand<object>(x => UndoRedo.Redo());
        }

        public void UnloadProject()
        {

        }
    }
}
