using RevengineEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices;
using System.IO;

namespace RevengineEditor.Classes
{
    static class VisualStudio
    {
        private static EnvDTE80.DTE2 _vsInstance = null;
        private static readonly string _progID = "VisualStudio.DTE"; // Visual Studio 2019

        [DllImport("Ole32.dll")]
        private static extern int GetRunningObjectTable(uint reserved, out IRunningObjectTable pprot);

        [DllImport("Ole32.dll")]
        private static extern int CreateBindCtx(uint reserved, out IBindCtx ppbc);

        /// <summary>
        /// Open the Visual Studio Instance
        /// </summary>
        /// <param name="solutionPath">The path of the Game Project's Solution</param>
        public static void OpenVisualStudio(string solutionPath)
        {
            IRunningObjectTable rot = null;
            IEnumMoniker monikerTable = null;
            IBindCtx bindCtx = null;

            try
            {
                // Check if there is a connection to Visual Studio
                if (_vsInstance == null)
                {
                    // Get the RunningObjectTable
                    int hResult = GetRunningObjectTable(0, out rot);

                    // Throw an excpetion if retrieving the RunningObjectTable fails
                    if (hResult < 0 || rot == null)
                        throw new COMException($"GetRunningObjectTable() returned HRESULT: {hResult:X8}");

                    // Get a table for enumeration
                    rot.EnumRunning(out monikerTable);

                    // Start at the first item
                    monikerTable.Reset();

                    // Create a Binding Context
                    hResult = CreateBindCtx(0, out bindCtx);

                    // Throw an excpetion if creating the BindingContext fails
                    if (hResult < 0 || bindCtx == null)
                        throw new COMException($"CreateBindCtx() returned HRESULT: {hResult:X8}");

                    // To enumerate through a MonikerTable, you need a moniker Array
                    // to put the next item of the table into
                    IMoniker[] currentMoniker = new IMoniker[1];
                    while(monikerTable.Next(1, currentMoniker, IntPtr.Zero) == 0)
                    {
                        string name = string.Empty;
                        currentMoniker[0].GetDisplayName(bindCtx, null, out name);

                        // If the name is the same as the program ID, then
                        // we have found a Visual Studio instance
                        if(name.Contains(_progID))
                        {
                            // Check if the correct Visual Studio instance is open
                            // (the one that has our game solution open)
                            hResult = rot.GetObject(currentMoniker[0], out object obj);

                            // Throw an exception if getting an object fails
                            if (hResult < 0 || obj == null)
                                throw new COMException($"RunningObjectTable's GetObject() returned HRESULT: {hResult:X8}");

                            // Cast the object to an instance of Visual Studio
                            EnvDTE80.DTE2 dte = obj as EnvDTE80.DTE2;

                            // Compare the name of the solution with the open solution in Visual Studio
                            string solutionName = dte.Solution.FullName;
                            if(solutionName == solutionPath)
                            {
                                // If it's equal, set the Visual Studio instance
                                // to the object
                                _vsInstance = dte;
                                break;
                            }
                        }
                    }
                    
                    if (_vsInstance == null)
                    {
                        // Get the Visual Studio type
                        Type visualStudioType = Type.GetTypeFromProgID(_progID, true);

                        // Create an instance of the Visual Studio type
                        _vsInstance = Activator.CreateInstance(visualStudioType) as EnvDTE80.DTE2;
                    }
                }
            } catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Logger.Log(MessageType.Error, "Failed to open Visual Studio");
            } finally
            {
                // Release the interfaces
                if (monikerTable != null) 
                    Marshal.ReleaseComObject(monikerTable);

                if (rot != null) 
                    Marshal.ReleaseComObject(rot);

                if (bindCtx != null) 
                    Marshal.ReleaseComObject(bindCtx);
            }
        }

        /// <summary>
        /// Close the Visual Studio Instance
        /// </summary>
        public static void CloseVisualStudio()
        {
            // Check if the Visual Studio Instance has an open solution
            if(_vsInstance?.Solution.IsOpen == true)
            {
                // Save the files and close the solution
                _vsInstance.ExecuteCommand("File.SaveAll");
                _vsInstance.Solution.Close(true);
            }

            // Close the Visual Studio Instance
            _vsInstance?.Quit();
        }

        public static bool AddFilesToSolution(string solution, string projectName, string[] files)
        {
            Debug.Assert(files?.Length > 0);

            // Open the Visual Studio solution
            OpenVisualStudio(solution);
            try
            {
                // Check if the Visual Studio Instance was set
                if(_vsInstance != null)
                {
                    // Open the Visual Studio solution if not yet open
                    if (!_vsInstance.Solution.IsOpen)
                    {
                        _vsInstance.Solution.Open(solution);
                    } else
                    {
                        // If it is open, save before adding
                        _vsInstance.ExecuteCommand("File.SaveAll");
                    }

                    // Go through all of the projects within the solution
                    foreach(EnvDTE.Project project in _vsInstance.Solution.Projects)
                    {
                        // Check if the project name is the same as the Game Project's name
                        if(project.UniqueName.Contains(projectName))
                        {
                            // If the project is the same, add the files
                            foreach(string file in files)
                            {
                                project.ProjectItems.AddFromFile(file);
                            }
                        }
                    }

                    // Find the .cpp file
                    string? cpp = files.FirstOrDefault(x => Path.GetExtension(x) == ".cpp");
                    
                    // Attempt to open the file if it exists
                    if(!string.IsNullOrEmpty(cpp))
                    {
                        // using ViewKind:EnvDTE.Constants.vsViewKindTextView
                        _vsInstance.ItemOperations.OpenFile(cpp, "{7651A703-06E5-11D1-8EBD-00A0C90F26EA}").Visible = true;
                    }

                    // Activate Visual Studio
                    _vsInstance.MainWindow.Activate();
                    _vsInstance.MainWindow.Visible = true;
                }
            } catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("Failed to add files to Visual Studio project");
                return false;
            }

            return true;
        }
    }
}
