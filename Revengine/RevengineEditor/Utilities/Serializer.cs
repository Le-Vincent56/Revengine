using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RevengineEditor.Utilities
{
    public static class Serializer
    {
        public static void ToFile<T>(T instance, string path)
        {
            try
            {
                // Create a new fileStream from path with FileMode.Create
                using FileStream fs = new FileStream(path, FileMode.Create);

                // Create a serializer
                DataContractSerializer serializer = new DataContractSerializer(typeof(T));

                // Write to an object using the fileStream and the given instance
                serializer.WriteObject(fs, instance);
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);

                // Log error
                Logger.Log(MessageType.Error, $"Failed to serialize {instance} to {path}");

                throw;
            }
        }

        public static T FromFile<T>(string path)
        {
            try
            {
                // Create a new filestream from path with FileMode.Open
                using FileStream fs = new FileStream(path, FileMode.Open);

                // Create a serializer
                DataContractSerializer serializer = new DataContractSerializer(typeof(T));

                // Read from the fileStream and set it to a Generic instance
                T instance = (T)serializer.ReadObject(fs);

                // Return the serialized object
                return instance;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                // Log error
                Logger.Log(MessageType.Error, $"Failed to deserialize {path}");

                throw;
            }
        }
    }
}
