using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webscraper
{
    public static class CollectionExtensions
    {
        public static void RemoveIf<T>(this IList<T> list, Func<T, bool> predicate)
        {
            for (int i = 0; i < list.Count; )
            {
                if (predicate(list[i]))
                    list.RemoveAt(i);
                else
                    i++;
            }
        }
    }
}
