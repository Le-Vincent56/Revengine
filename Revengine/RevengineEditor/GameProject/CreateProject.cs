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

namespace RevengineEditor.GameProject
{
    [DataContract]
    public class ProjectTemplate
    {
        [DataMember]
        public string ProjectType { get; set; }
        [DataMember]
        public string ProjectFile { get; set; }
        [DataMember]
        public List<string> Folders { get; set; }

        public byte[] Icon { get; set; }
        public byte[] Screenshot { get; set; }
        public string IconFilePath { get; set; }
        public string ScreenshotFilePath { get; set; }
        public string ProjectFilePath { get; set; }
        public string TemplatePath { get; set; }
    }

    public class CreateProject : ViewModelBase
    {
        // TODO: get the path from the installation location
        private readonly string _templatePath = @"..\..\RevengineEditor\ProjectTemplates\";
        private string _projectName = "New Project";
        private string _projectPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\RevengineProjects";
        private bool _isValid = false;
        private string _errorMessage;
        private ObservableCollection<ProjectTemplate> _projectTemplates = new ObservableCollection<ProjectTemplate>();

        public string ProjectName { 
            get { return _projectName; } 
            set 
            {   
                if (_projectName != value)
                {
                    _projectName = value;
                    ValidateProjectPath();
                    OnPropertyChanged(nameof(ProjectName));
                }
            } 
        }

        public string ProjectPath
        {
            get { return _projectPath; }
            set
            {
                if (_projectPath != value)
                {
                    _projectPath = value;
                    ValidateProjectPath();
                    OnPropertyChanged(nameof(ProjectPath));
                }
            }
        }

        public bool IsValid
        {
            get { return _isValid; }
            set
            {
                if(_isValid != value)
                {
                    _isValid = value;
                    OnPropertyChanged(nameof(IsValid));
                }
            }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                if (_errorMessage != value)
                {
                    _errorMessage = value;
                    OnPropertyChanged(nameof(ErrorMessage));
                }
            }
        }

        public ReadOnlyObservableCollection<ProjectTemplate> ProjectTemplates { get; }

        public CreateProject()
        {
            // Set project templates
            ProjectTemplates = new ReadOnlyObservableCollection<ProjectTemplate>(_projectTemplates);

            try
            {
                // Find all templates within the directory
                string[] templateFiles = Directory.GetFiles(_templatePath, "template.xml", SearchOption.AllDirectories);
                Debug.Assert(templateFiles.Any());

                // Read project templates from each file
                foreach (string templateFile in templateFiles)
                {
                    // Read in template
                    ProjectTemplate template = Serializer.FromFile<ProjectTemplate>(templateFile);

                    // Assign data
                    template.TemplatePath = Path.GetDirectoryName(templateFile);
                    template.IconFilePath = Path.GetFullPath(Path.Combine(template.TemplatePath, "Icon.png"));
                    template.Icon = File.ReadAllBytes(template.IconFilePath);
                    template.ScreenshotFilePath = Path.GetFullPath(Path.Combine(template.TemplatePath, "Screenshot.png"));
                    template.Screenshot = File.ReadAllBytes(template.ScreenshotFilePath);
                    template.ProjectFilePath = Path.GetFullPath(Path.Combine(template.TemplatePath, template.ProjectFile));
                    

                    // Add each template to the list
                    _projectTemplates.Add(template);
                }

                // Validate project path
                ValidateProjectPath();
            }
            catch (Exception ex)
            {
                // Notify exceptions
                Debug.WriteLine(ex.Message);

                Logger.Log(MessageType.Error, $"Failed to read project templates");
            }

        }

        /// <summary>
        /// Validate Project Name and Path
        /// </summary>
        /// <returns>True if ProjectName and ProjectPath are valid, false otherwise </returns>
        private bool ValidateProjectPath()
        {
            // Set path name
            string path = ProjectPath;

            // Sanitize path name
            if (!Path.EndsInDirectorySeparator(path))
                path += @"\";

            path += $@"{ProjectName}\";

            // Set IsValid to false initially
            IsValid = false;

            // Check false cases and set error messages
            if(string.IsNullOrWhiteSpace(ProjectName.Trim())) // Check for empty string or white space
            {
                ErrorMessage = "Please provide a project name";
            } else if(ProjectName.IndexOfAny(Path.GetInvalidFileNameChars()) != -1) // Check for invalid filename characters
            {
                ErrorMessage = "Please use valid characters in your project name";
            } else if(string.IsNullOrWhiteSpace(ProjectPath.Trim())) // Check for empty path file
            {
                ErrorMessage = "Please provide a valid project folder";
            } else if(ProjectPath.IndexOfAny(Path.GetInvalidPathChars()) != -1) // Check for invalid path characters
            {
                ErrorMessage = "Please use valid characters in your project path"; 
            } else if(Directory.Exists(path) && Directory.EnumerateFileSystemEntries(path).Any()) // If the path exists and is already populated
            {
                ErrorMessage = "Please use an empty project folder";
            } else // No false case found
            {
                ErrorMessage = string.Empty;
                IsValid = true;
            }

            // Return IsValid
            return IsValid;
        }

        /// <summary>
        /// Create a New Project
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public string CreateNewProject(ProjectTemplate template)
        {
            // Validate the project path
            ValidateProjectPath();

            // If the path is invalid, return an empty string
            if(!IsValid)
            {
                return string.Empty;
            }

            // Sanitize path name
            if (!Path.EndsInDirectorySeparator(ProjectPath))
                ProjectPath += @"\";

            string path = $@"{ProjectPath}{ProjectName}\";

            try
            {
                // Create the folder if it doesn't exist
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                // Create necessary folders
                foreach(string folder in template.Folders)
                {
                    Directory.CreateDirectory(Path.GetFullPath(Path.Combine(Path.GetDirectoryName(path), folder)));
                }

                // Set hidden directories
                DirectoryInfo dirInfo = new DirectoryInfo(path + @".Revengine");
                dirInfo.Attributes |= FileAttributes.Hidden;
                File.Copy(template.IconFilePath, Path.GetFullPath(Path.Combine(dirInfo.FullName, "Icon.png")));
                File.Copy(template.ScreenshotFilePath, Path.GetFullPath(Path.Combine(dirInfo.FullName, "Screenshot.png")));

                // Read the tempalte project file and format the projectPath with it
                string projectXML = File.ReadAllText(template.ProjectFilePath);
                projectXML = string.Format(projectXML, ProjectName, path);
                string projectPath = Path.GetFullPath(Path.Combine(path, $"{ProjectName}{Project.Extension}"));

                // Write the formatted file at the given project path
                File.WriteAllText(projectPath, projectXML);

                // Create the MSVC Solution
                CreateMSVCSolution(template, path);

                return path;
            }
            catch(Exception ex)
            {
                // Notify exceptions
                Debug.WriteLine(ex.Message);

                // Log error
                Logger.Log(MessageType.Error, $"Failed to create {ProjectName}");

                return string.Empty;
            }
        }

        private void CreateMSVCSolution(ProjectTemplate template, string projectPath)
        {
            Debug.Assert(File.Exists(Path.Combine(template.TemplatePath, "MSVCSolution")));
            Debug.Assert(File.Exists(Path.Combine(template.TemplatePath, "MSVCProject")));

            string engineAPIPath = Path.Combine(MainWindow.RevenginePath, @"Engine\EngineAPI\");
            Debug.Assert(Directory.Exists(engineAPIPath));

            // Project name parameter
            string _0 = ProjectName;

            // Project GUID parameter
            string _1 = "{" + Guid.NewGuid().ToString().ToUpper() + "}";

            // Solution GUID paramter
            string _2s = "{" + Guid.NewGuid().ToString().ToUpper() + "}";

            // Construct the string from the templates
            string solution = File.ReadAllText(Path.Combine(template.TemplatePath, "MSVCSolution"));
            solution = string.Format(solution, _0, _1, _2s);

            // Write the text to a solution
            File.WriteAllText(Path.GetFullPath(Path.Combine(projectPath, $"{_0}.sln")), solution);

            // Include Path parameter
            string _2p = engineAPIPath;
            
            // Additional Libraries parameter
            string _3 = MainWindow.RevenginePath;

            // Construct the string from the templates
            string project = File.ReadAllText(Path.Combine(template.TemplatePath, "MSVCProject"));
            project = string.Format(project, _0, _1, _2p, _3);

            // Write the text to a project
            File.WriteAllText(Path.GetFullPath(Path.Combine(projectPath, $@"GameCode\{_0}.vcxproj")), project);
        }
    }
}
