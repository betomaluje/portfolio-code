using System;
using System.Collections.Generic;
using System.Linq;

namespace Powerups {
    // Taken from https://stackoverflow.com/questions/4171140/how-to-iterate-over-values-of-an-enum-having-flags
    /// <summary>
    /// Extension methods for <see cref="Enum"/> to get individual and all flags
    /// Example usage:
    /// var value = Items.Bar | Items.Baz;
    /// value.GetFlags();           // Boo
    /// value.GetIndividualFlags(); // Bar, Baz
    /// </summary>
    public static class EnumExt {
        public static IEnumerable<T> GetUniqueFlags<T>(this T flags) where T : Enum {
            foreach (Enum value in Enum.GetValues(flags.GetType()))
                if (flags.HasFlag(value))
                    yield return (T)value;
        }

        public static IEnumerable<Enum> GetFlags(this Enum value) {
            return GetFlags(value, Enum.GetValues(value.GetType()).Cast<Enum>().ToArray());
        }

        public static IEnumerable<Enum> GetIndividualFlags(this Enum value) {
            return GetFlags(value, GetFlagValues(value.GetType()).ToArray());
        }

        private static IEnumerable<Enum> GetFlags(Enum value, Enum[] values) {
            ulong bits = Convert.ToUInt64(value);
            List<Enum> results = new();
            for (int i = values.Length - 1; i >= 0; i--) {
                ulong mask = Convert.ToUInt64(values[i]);
                if (i == 0 && mask == 0L)
                    break;
                if ((bits & mask) == mask) {
                    results.Add(values[i]);
                    bits -= mask;
                }
            }
            if (bits != 0L)
                return Enumerable.Empty<Enum>();
            if (Convert.ToUInt64(value) != 0L)
                return results.Reverse<Enum>();
            if (bits == Convert.ToUInt64(value) && values.Length > 0 && Convert.ToUInt64(values[0]) == 0L)
                return values.Take(1);
            return Enumerable.Empty<Enum>();
        }

        private static IEnumerable<Enum> GetFlagValues(Type enumType) {
            ulong flag = 0x1;
            foreach (var value in Enum.GetValues(enumType).Cast<Enum>()) {
                ulong bits = Convert.ToUInt64(value);
                if (bits == 0L)
                    //yield return value;
                    continue; // skip the zero value
                while (flag < bits) flag <<= 1;
                if (flag == bits)
                    yield return value;
            }
        }
    }
}