using System.Collections.Generic;
using System.Linq;

namespace BerserkPixel.Prata
{
    public static class InteractionExt
    {
        public static T Random<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                throw new System.ArgumentNullException(nameof(enumerable));
            }
            
            var r = new System.Random();
            var list = enumerable as IList<T> ?? enumerable.ToList();
            return list.Count == 0 ? default(T) : list[r.Next(0, list.Count)];
        }
    }
}