using RevengineEditor.Components;
using RevengineEditor.EngineAPIStructs;
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
    class GameGrievanceDescriptor
    {
        public TransformMotivator Transform = new TransformMotivator();
    }
}

namespace RevengineEditor.DLLWrappers
{
    static class EngineAPI
    {
        // Find DLL file
        private const string _dllName = "RevengineDLL.dll";

        // Import functions
        [DllImport(_dllName)]
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

            // Call DLL function to create the Grievance in the Engine
            return CreateGrievance(desc);
        }

        [DllImport(_dllName)]
        private static extern void RemoveGrievance(int id);
        public static void RemoveGrievance(Grievance grievance)
        {
            RemoveGrievance(grievance.GrievanceID);
        }
    }
}
