using RevengineEditor.Components;
using RevengineEditor.EngineAPIStructs;
using RevengineEditor.GameProject;
using RevengineEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

// Anything within here should be laid out sequentially so we can access it in order
// in the C++ engine
namespace RevengineEditor.EngineAPIStructs
{
    [StructLayout(LayoutKind.Sequential)]
    class TransformMotivator
    {
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale = new Vector3(1, 1, 1);
    }

    [StructLayout(LayoutKind.Sequential)]
    class ScriptMotivator
    {
        public IntPtr ScriptCreator;
    }

    [StructLayout(LayoutKind.Sequential)]
    class GameGrievanceDescriptor
    {
        public TransformMotivator Transform = new TransformMotivator();
        public ScriptMotivator Script = new ScriptMotivator();
    }
}

namespace RevengineEditor.DLLWrappers
{
    public static class EngineAPI
    {
        // Find DLL file
        private const string _engineDLL = "RevengineDLL.dll";

        [DllImport(_engineDLL, CharSet = CharSet.Ansi)]
        public static extern int LoadGameCodeDLL(string dllPath);

        [DllImport(_engineDLL)]
        public static extern int UnloadGameCodeDLL();

        [DllImport(_engineDLL)]
        public static extern IntPtr GetScriptCreator(string name);

        [DllImport(_engineDLL)]
        [return: MarshalAs(UnmanagedType.SafeArray)]
        public static extern string[] GetScriptNames();

        internal static class GrievanceAPI
        {
            // Import functions
            [DllImport(_engineDLL)]
            private static extern int CreateGrievance(GameGrievanceDescriptor desc);
            public static int CreateGrievance(Grievance grievance)
            {
                GameGrievanceDescriptor desc = new GameGrievanceDescriptor();

                // Transform Motivator
                {
                    // Copy over Transform data
                    Transform transform = grievance.GetMotivator<Transform>();
                    desc.Transform.Position = transform.Position;
                    desc.Transform.Rotation = transform.Rotation;
                    desc.Transform.Scale = transform.Scale;
                }

                // Script Motivator
                {
                    Script script = grievance.GetMotivator<Script>();

                    // Check if the script exits and if the project is loaded
                    // as the script might not load if the game code DLl has not loaded
                    if(script != null && Project.Current != null)
                    {
                        // Check the if the script name is available
                        if(Project.Current.AvailableScripts.Contains(script.Name))
                        {
                            // Get the corresponding script creator
                            desc.Script.ScriptCreator = GetScriptCreator(script.Name);
                        } else
                        {
                            Logger.Log(MessageType.Error, $"Unable to find script with name {script.Name}. Grievance will" +
                                $"be created without the {script.Name} script motivator");
                        }
                    }
                }

                // Call DLL function to create the Grievance in the Engine
                return CreateGrievance(desc);
            }

            [DllImport(_engineDLL)]
            private static extern void RemoveGrievance(int id);
            public static void RemoveGrievance(Grievance grievance)
            {
                RemoveGrievance(grievance.GrievanceID);
            }
        }
    }
}
