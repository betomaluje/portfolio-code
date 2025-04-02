using System;
using System.Collections.Generic;
using EditorTool.Models;

public static class ListExt {
    public static SerializableList<T> ToSerializable<T>(this List<T> list) {
        return new SerializableList<T>() { list = list };
    }

    public static void AddRange<T>(this IList<T> list, IEnumerable<T> items) {
        if (list == null) throw new ArgumentNullException(nameof(list));
        if (items == null) throw new ArgumentNullException(nameof(items));

        if (list is List<T> asList) {
            asList.AddRange(items);
        }
        else {
            foreach (var item in items) {
                list.Add(item);
            }
        }
    }
}