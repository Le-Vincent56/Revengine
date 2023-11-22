using RevengineEditor.Classes;
using RevengineEditor.DLLWrappers;
using RevengineEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RevengineEditor.GameProject
{
    enum BuildConfiguration
    {
        Debug,
        DebugEditor,
        Release,
        ReleaseEditor
    }

    [DataContract(Name = "Game")]
    internal class Project : ViewModelBase
    {
        public static string Extension { get; } = ".revengine";

        [DataMember]
        public string Name { get; private set; } = "New Project";

        [DataMember]
        public string Path { get; private set; }

        public string FullPath { get { return $@"{Path}{Name}{Extension}"; } }
        public string Solution { get { return $@"{Path}{Name}.sln"; } }

        private static readonly string[] _buildConfigurationNames = new string[] 
            { "Debug", "DebugEditor", "Release", "ReleaseEditor" };

        private int _buildConfig;
        [DataMember]
        public int BuildConfig
        {
            get { return _buildConfig; }
            set
            {
                if(_buildConfig != value)
                {
                    _buildConfig = value;
                    OnPropertyChanged(nameof(BuildConfig));
                }
            }
        }

        public BuildConfiguration StandaloneBuildConfig => BuildConfig == 0 ? BuildConfiguration.Debug : BuildConfiguration.Release;
        public BuildConfiguration DLLBuildConfig => BuildConfig == 0 ? BuildConfiguration.DebugEditor : BuildConfiguration.ReleaseEditor;

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
        public ICommand UndoCommand { get; private set; }
        public ICommand RedoCommand { get; private set; }

        public ICommand AddSceneCommand { get; private set; }
        public ICommand RemoveSceneCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand BuildCommand { get; private set; }

        public Project(string name, string path)
        {
            Name = name;
            Path = path;

            OnDeserialized(new StreamingContext());
        }

        /// <summary>
        /// Set all project commands
        /// </summary>
        private void SetCommands()
        {
            AddSceneCommand = new RelayCommand<object>(x =>
            {
                // Add the scene
                AddScene($"New Scene {_scenes.Count}");

                // Get a reference to the new scene
                Scene newScene = _scenes.Last();
                int sceneIndex = _scenes.Count - 1;

                // Add the Add Scene Undo and Redo commands
                UndoRedo.Add(new UndoRedoAction(
                    () => RemoveScene(newScene),
                    () => _scenes.Insert(sceneIndex, newScene),
                    $"Add {newScene.Name}"));
            });

            RemoveSceneCommand = new RelayCommand<Scene>(x =>
            {
                // Get the scene index of the scene to remove
                int sceneIndex = _scenes.IndexOf(x);

                // Remove the scene
                RemoveScene(x);

                // Add the Remove Scene Undo and Redo commands
                UndoRedo.Add(new UndoRedoAction(
                    () => _scenes.Insert(sceneIndex, x),
                    () => RemoveScene(x),
                    $"Remove {x.Name}"));
            }, x => !x.IsActive);

            // Assign Undo and Redo
            UndoCommand = new RelayCommand<object>(x => UndoRedo.Undo(), x => UndoRedo.UndoList.Any());
            RedoCommand = new RelayCommand<object>(x => UndoRedo.Redo(), x => UndoRedo.RedoList.Any());

            // Assign a Save command
            SaveCommand = new RelayCommand<object>(x => Save(this));

            // Assign a Build command
            BuildCommand = new RelayCommand<bool>(async x => await BuildGameCodeDLL(x), x => !VisualStudio.IsDebugging() && VisualStudio.BuildDone);

            OnPropertyChanged(nameof(AddSceneCommand));
            OnPropertyChanged(nameof(RemoveSceneCommand));
            OnPropertyChanged(nameof(UndoCommand));
            OnPropertyChanged(nameof(RedoCommand));
            OnPropertyChanged(nameof(SaveCommand));
            OnPropertyChanged(nameof(BuildCommand));
        }

        /// <summary>
        /// Add a scene to the project
        /// </summary>
        /// <param name="sceneName">The scene name</param>
        private void AddScene(string sceneName)
        {
            Debug.Assert(!string.IsNullOrEmpty(sceneName.Trim()));
            _scenes.Add(new Scene(sceneName, this));
        }

        /// <summary>
        /// Remove a scene from the project
        /// </summary>
        /// <param name="scene"></param>
        private void RemoveScene(Scene scene)
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
            Logger.Log(MessageType.Info, $"{project.Name} saved to {project.FullPath}");
        }

        /// <summary>
        /// Handle the rest of the deserialization after Save()
        /// </summary>
        /// <param name="context"></param>
        [OnDeserialized]
        public async void OnDeserialized(StreamingContext context)
        {
            // Construct scenes if there are scenes
            if(_scenes != null)
            {
                Scenes = new ReadOnlyObservableCollection<Scene>(_scenes);
                OnPropertyChanged(nameof(Scenes));
            }
            ActiveScene = Scenes.FirstOrDefault(x => x.IsActive);

            // Build the game code without showing the editor
            await BuildGameCodeDLL(false);

            // Set commands
            SetCommands();
        }

        /// <summary>
        /// Unload the Project
        /// </summary>
        public void UnloadProject()
        {
            // Close Visual Studio
            VisualStudio.CloseVisualStudio();

            // Reset the UndoRedo class, as they are no longer relevant
            UndoRedo.Reset();
        }

        /// <summary>
        /// Get the name of the build for the certain type of configuration
        /// </summary>
        /// <param name="config">The build configuration type</param>
        /// <returns>A string that is the name of the build configuration</returns>
        private static string GetConfigurationName(BuildConfiguration config)
        {
            return _buildConfigurationNames[(int)config];
        }

        /// <summary>
        /// Build the Game Code DLL
        /// </summary>
        /// <param name="showWindow">Whether to show Visual Studio or not</param>
        private async Task BuildGameCodeDLL(bool showWindow = true)
        {
            try
            {
                // Need to unload the game code first
                UnloadGameCodeDLL();

                // Build the solution from Visual Studio
                await Task.Run(() => VisualStudio.BuildSolution(this, GetConfigurationName(DLLBuildConfig), showWindow));

                if (VisualStudio.BuildSucceeded)
                {
                    LoadGameCodeDLL();
                }
            } catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Load the Game Code DLL
        /// </summary>
        private void LoadGameCodeDLL()
        {
            // Get the name of the build and craft the DLL path
            string configName = GetConfigurationName(DLLBuildConfig);
            string dll = $@"{Path}\x64\{configName}\{Name}.dll";

            // Check if the file exists and if the loading is successful
            if(File.Exists(dll) && EngineAPI.LoadGameCodeDLL(dll) != 0)
            {
                Logger.Log(MessageType.Info, "Game code DLL loaded successfully!");
            } else
            {
                Logger.Log(MessageType.Warning, "Failed to load game code DLL file. Try building the project first.");
            }
        }

        /// <summary>
        /// Unload the Game Code DLL
        /// </summary>
        private void UnloadGameCodeDLL()
        {
            // Unload the game code through the EngineAPI
            if (EngineAPI.UnloadGameCodeDLL() != 0)
            {
                Logger.Log(MessageType.Info, "Game code DLL unloaded successfully!");
            }
        }
    }
}
