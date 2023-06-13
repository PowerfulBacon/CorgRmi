using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Linq
{
    internal static class LinqExtensions
    {

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> func)
        {
            foreach (var item in source)
                func(item);
        }

    }
}
