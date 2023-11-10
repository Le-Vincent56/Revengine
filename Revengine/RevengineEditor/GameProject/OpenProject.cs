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
using System.Windows.Media;

namespace RevengineEditor.GameProject
{
    [DataContract]
    public class ProjectData
    {
        [DataMember]
        public string ProjectName { get; set; }
        [DataMember]
        public string ProjectPath { get; set; }
        [DataMember]
        public DateTime Date { get; set; }
        public string FullPath { get { return $"{ProjectPath}{ProjectName}{Project.Extension}"; } }
        public byte[] Icon { get; set; }
        public byte[] Screenshot { get; set; }
    }

    [DataContract]
    public class ProjectDataList
    {
        [DataMember]
        public List<ProjectData> Projects { get; set; }
    }

    public class OpenProject : ViewModelBase
    {
        private static readonly string _applicationDataPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\RevengineEditor\";
        private static readonly string _projectDataPath;
        private static readonly ObservableCollection<ProjectData> _projects = new ObservableCollection<ProjectData>();
        public static ReadOnlyObservableCollection<ProjectData> Projects { get; }

        static OpenProject()
        {
            try
            {
                // Create the AppData file path if nonexistent
                if(!Directory.Exists(_applicationDataPath))
                    Directory.CreateDirectory(_applicationDataPath);

                // Set the project data path
                _projectDataPath = $@"{_applicationDataPath}ProjectData.xml";

                // Initialize the Projects collection
                Projects = new ReadOnlyObservableCollection<ProjectData>(_projects);

                // Read project data
                ReadProjectData();

            } catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                // TODO: log errors
            }
        }

        private static void ReadProjectData()
        {
            if(File.Exists(_projectDataPath))
            {
                // Read the list of projects
                List<ProjectData> projects = Serializer.FromFile<ProjectDataList>(_projectDataPath).Projects;

                // Order the projects by descending dates
                projects.OrderByDescending(x => x.Date);

                // Clear the _projects list
                _projects.Clear();

                // Go through the list of ProjectData
                foreach(ProjectData project in projects)
                {
                    // Check if the file exists at the given full path
                    if(File.Exists(project.FullPath))
                    {
                        // Set the project images for display
                        project.Icon = File.ReadAllBytes($@"{project.ProjectPath}\.Revengine\Icon.png");
                        project.Screenshot = File.ReadAllBytes($@"{project.ProjectPath}\.Revengine\Screenshot.png");

                        // Add to the list of projects
                        _projects.Add(project);
                    }
                }
            }
        }

        private static void WriteProjectData()
        {
            // Create a list of projectData
            List<ProjectData> projects = _projects.OrderBy(x => x.Date).ToList();

            // Serialize the list to a file
            Serializer.ToFile(new ProjectDataList() { Projects = projects }, _projectDataPath);
        }

        public static Project Open(ProjectData projectData)
        {
            // Read project data
            ReadProjectData();

            // Check if the project is the same
            ProjectData project = _projects.FirstOrDefault(x => x.FullPath == projectData.FullPath);
            if(project != null)
            {
                // Set the date
                project.Date = DateTime.Now;
            } else
            {
                // Set the project to projectData
                project = projectData;

                // Set the date
                project.Date = DateTime.Now;

                // Add the project to the collection
                _projects.Add(project);
            }

            // Save the project data
            WriteProjectData();

            // Load the project
            return Project.LoadProject(project.FullPath);
        }
    }
}
