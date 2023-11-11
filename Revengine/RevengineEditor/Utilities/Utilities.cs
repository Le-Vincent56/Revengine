using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevengineEditor.Utilities
{
    public static class ID {
        public static int INVALID_ID => -1;
        public static bool Isvalid(int id) => id != INVALID_ID;
    }

    public static class MathUtil
    {
        public static float Epsilon => 0.00001f;

        /// <summary>
        /// Compare floating point values
        /// </summary>
        /// <param name="value">The value to compare</param>
        /// <param name="other">The value to compare to</param>
        /// <returns>True if both floating point values are the same, false if not</returns>
        public static bool IsTheSameAs(this float value, float other)
        {
            return Math.Abs(value - other) < Epsilon;
        }

        /// <summary>
        /// Compare nullable floating point values
        /// </summary>
        /// <param name="value">The nullable value to compare</param>
        /// <param name="other">The nullable value to compare to</param>
        /// <returns>True if both nullable floating point values are the same, false if not</returns>
        public static bool IsTheSameAs(this float? value, float? other)
        {
            if(!value.HasValue || !other.HasValue) return false;
            return Math.Abs(value.Value - other.Value) < Epsilon;
        }
    }
}
