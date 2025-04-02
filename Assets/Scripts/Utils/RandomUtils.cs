using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class RandomUtils {
    /// <summary>
    /// Return random bool value.
    /// </summary>
    public static bool NextBool() {
        return Random.Range(0, 2) == 0;
    }

    /// <summary>
    /// Return random item from item1 and item2 set.
    /// </summary>
    public static T Next<T>(T item1, T item2) {
        return NextBool() ? item1 : item2;
    }

    /// <summary>
    /// Return random item from item1, item2 and item3 set.
    /// </summary>
    public static T Next<T>(T item1, T item2, T item3) {
        int n = Random.Range(0, 3);
        return n == 0 ? item1 : (n == 1 ? item2 : item3);
    }

    /// <summary>
    /// Return random item from array.
    /// </summary>
    public static T NextItem<T>(this T[] array) {
        return array[Random.Range(0, array.Length)];
    }

    /// <summary>
    /// Return random item from list.
    /// </summary>
    public static T NextItem<T>(this List<T> list) {
        return list[Random.Range(0, list.Count)];
    }

    /// <summary>
    /// Return random enum item.
    /// </summary>
    public static T NextEnum<T>() {
        var values = Enum.GetValues(typeof(T));
        return (T)values.GetValue(Random.Range(0, values.Length));
    }

    /// <summary>
    /// Return random index of passed array. Index random selection is based on array weights.
    /// </summary>
    public static int NextWeightedInd(int[] weights) {
        int randomPoint = Random.Range(0, weights.Sum()) + 1;
        int sum = 0;
        for (int i = 0; i < weights.Length; i++) {
            sum += weights[i];
            if (randomPoint <= sum)
                return i;
        }
        throw new Exception("Logic error!");
    }

    /// <summary>
    /// Return random index of passed array. Index random selection is based on array weights.
    /// </summary>
    public static int NextWeightedInd(float[] weights) {
        float randomPoint = Random.Range(0f, weights.Sum());
        float sum = 0f;
        for (int i = 0; i < weights.Length; i++) {
            sum += weights[i];
            if (randomPoint <= sum)
                return i;
        }
        throw new Exception("Logic error!");
    }

    /// <summary>
    /// Return sub-list of random items from origin list without repeating.
    /// </summary>
    public static List<T> Take<T>(this IList<T> list, int count) {
        List<T> items = new(count);
        List<int> remainedIndexes = Enumerable.Range(0, list.Count).ToList();
        for (int i = 0; i < count; i++) {
            int selectedIndex = NextItem(remainedIndexes);
            remainedIndexes.Remove(selectedIndex);
            items.Add(list[selectedIndex]);
        }
        return items;
    }

    /// <summary>
    /// Return sub-list of random items from origin list without repeating.
    /// </summary>
    public static T[] Take<T>(this T[] array, int count) {
        T[] items = new T[count];
        List<int> remainedIndexes = Enumerable.Range(0, array.Length).ToList();
        for (int i = 0; i < count; i++) {
            int selectedIndex = NextItem(remainedIndexes);
            remainedIndexes.Remove(selectedIndex);
            items[i] = array[selectedIndex];
        }
        return items;
    }

    /// <summary>
    /// Return sub-list of random items from origin list without repeating. It also removes the selected items of the
    /// generated sublist
    /// </summary>
    public static List<T> TakeAndRemove<T>(this IList<T> list, int count) {
        List<T> items = new();
        List<int> selectedIndexes = new();
        List<int> remainedIndexes = Enumerable.Range(0, list.Count).ToList();
        for (int i = 0; i < count; i++) {
            int selectedIndex = NextItem(remainedIndexes);
            remainedIndexes.Remove(selectedIndex);
            items.Add(list[selectedIndex]);

            selectedIndexes.Add(selectedIndex);
        }

        List<T> temp = new();
        for (int i = 0; i < list.Count; i++) {
            if (!selectedIndexes.Contains(i)) {
                temp.Add(list[i]);
            }
        }

        list.Clear();
        foreach (var item in temp) {
            list.Add(item);
        }

        return items;
    }

    /// <summary>
    /// Shuffle list of items.
    /// </summary>
    public static IList<T> Shuffle<T>(this IList<T> list) {
        for (int i = 1; i < list.Count; i++) {
            int indRnd = Random.Range(0, i + 1);
            (list[indRnd], list[i]) = (list[i], list[indRnd]);
        }

        return list;
    }

    /// <summary>
    /// Shuffle array of items.
    /// </summary>
    public static T[] Shuffle<T>(this T[] array) {
        for (int i = 1; i < array.Length; i++) {
            int indRnd = Random.Range(0, i + 1);
            (array[indRnd], array[i]) = (array[i], array[indRnd]);
        }

        return array;
    }

    /// <summary>
    /// Shuffles a list using the Fisher-Yates shuffle algorithm. Taken from https://stackoverflow.com/questions/273313/randomize-a-listt
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    public static IList<T> SimpleShuffle<T>(this IList<T> list) {
        int n = list.Count;
        var random = new System.Random();
        while (n > 1) {
            n--;
            int k = random.Next(n + 1);
            (list[n], list[k]) = (list[k], list[n]);
        }

        return list;
    }

    /// <summary>
    /// Shuffles a list using the Fisher-Yates shuffle algorithm. Taken from https://stackoverflow.com/questions/273313/randomize-a-listt
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    public static T[] SimpleShuffle<T>(this T[] array) {
        int n = array.Length;
        var random = new System.Random();
        while (n > 1) {
            n--;
            int k = random.Next(n + 1);
            (array[n], array[k]) = (array[k], array[n]);
        }

        return array;
    }

    /// <summary>
    /// Shuffles a list using the Fisher-Yates shuffle algorithm. Taken from https://stackoverflow.com/questions/5807128/an-extension-method-on-ienumerable-needed-for-shuffling
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumerable"></param>
    public static IEnumerable<T> SimpleShuffle<T>(this IEnumerable<T> enumerable) {
        var random = new System.Random();
        var buffer = enumerable.ToList();
        int n = enumerable.Count();
        for (int i = 0; i < n; i++) {
            int j = random.Next(i, n);
            yield return buffer[j];

            buffer[j] = buffer[i];
        }
    }

    /// <summary>
    /// Return random point on line.
    /// </summary>
    public static Vector2 NextPointOnLine(Vector2 point1, Vector2 point2) {
        float t = Random.Range(0f, 1f);
        return new Vector2(Mathf.Lerp(point1.x, point2.x, t), Mathf.Lerp(point1.y, point2.y, t));
    }

    /// <summary>
    /// Return random point on line.
    /// </summary>
    public static Vector3 NextPointOnLine(Vector3 point1, Vector3 point2) {
        float t = Random.Range(0f, 1f);
        return new Vector3(Mathf.Lerp(point1.x, point2.x, t), Mathf.Lerp(point1.y, point2.y, t), Mathf.Lerp(point1.z, point2.z, t));
    }

    /// <summary>
    /// Get a chance with given percentage. If percentage is 25 it will return true each 4th time on an average.
    /// </summary>
    public static bool GetChance(int percentage) {
        return Random.Range(0, 100) + 1 <= percentage;
    }

    /// <summary>
    /// Gets a chance with give probability. If probability is 0.25 it will return true each 4th time on an average.
    /// </summary>
    public static bool GetChance(float probability) {
        return Random.value < probability;
    }

    /// <summary>
    /// Get random normalized 2D direction as Vector2.
    /// </summary>
    public static Vector2 NextDirection() {
        return Random.insideUnitCircle.normalized;
    }

    /// <summary>
    /// Get random normalized 2D direction as Vector2.
    /// </summary>
    public static Vector3 NextDirection3D() {
        return Random.insideUnitCircle.normalized;
    }

    /// <summary>
    /// Return Random.Range between two values which are stored in pairMinMax Vector2.
    /// Where pairMinMax.x is min value and pairMinMax.y is max value.
    /// </summary>
    public static float Range(Vector2 pairMinMax) {
        return Random.Range(pairMinMax.x, pairMinMax.y);
    }

    /// <summary>
    /// Return random point from rect bound (inside rect).
    /// </summary>
    public static Vector2 NextPointOnRect(Rect rect) {
        return new Vector2(Random.Range(rect.xMin, rect.xMax), Random.Range(rect.yMin, rect.yMax));
    }

    /// <summary>
    /// Return random point on rect border (perimeter of rect).
    /// </summary>
    public static Vector2 NextPointOnRectBorder(Rect rect) {
        float perimeterLength = (rect.width + rect.height) * 2f;
        float pointOnPerimeter = Random.Range(0f, perimeterLength);

        if (pointOnPerimeter < rect.width)//top border
            return new Vector2(rect.xMin + pointOnPerimeter, rect.yMax);

        pointOnPerimeter -= rect.width;

        if (pointOnPerimeter < rect.height)//right border
            return new Vector2(rect.xMax, rect.yMin + pointOnPerimeter);

        pointOnPerimeter -= rect.height;

        if (pointOnPerimeter < rect.width)//bottom border
            return new Vector2(rect.xMin + pointOnPerimeter, rect.yMin);

        pointOnPerimeter -= rect.width;

        //left border
        return new Vector2(rect.xMin, rect.yMin + pointOnPerimeter);
    }
}
