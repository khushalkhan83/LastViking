using System;
using System.Collections.Generic;
using System.Linq;

namespace Helpers
{
    public static class EnumsHelper
    {
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }
}
