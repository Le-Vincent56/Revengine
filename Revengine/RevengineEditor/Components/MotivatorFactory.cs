using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevengineEditor.Components
{
    enum MotivatorType
    {
        Transform,
        Script
    }

    static class MotivatorFactory
    {
        private static readonly Func<Grievance, object, Motivator>[] _function =
            new Func<Grievance, object, Motivator>[]
            {
                (grievance, data) => new Transform(grievance),
                (grievance, data) => new Script(grievance){Name = (string)data}
            };

        public static Func<Grievance, object, Motivator> GetCreationFunction(MotivatorType motivatorType)
        {
            Debug.Assert((int)motivatorType < _function.Length);

            // Because we establish the array in a way that is similar to how the enum is presented,
            // we can just index into the array to get the Motivator type
            return _function[(int)motivatorType];
        }
    }
}
