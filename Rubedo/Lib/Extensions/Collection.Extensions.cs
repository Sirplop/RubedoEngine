using System;
using System.Collections.Generic;

namespace Rubedo.Lib.Extensions;

/// <summary>
/// Extensions to Lists and probably other collection types.
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    /// Sets the element at <paramref name="index"/> to the last element in the list, then removes the last element.<br></br>
    /// This removes the element at the index without needing to move elements around.
    /// </summary>
    /// <param name="list">The list</param>
    /// <param name="index">The index to remove</param>
    public static void SwapAndRemove<T>(this IList<T> list, int index)
    {
        list[index] = list[^1];
        list.RemoveAt(index);
    }
    /// <summary>
    /// Populates an array with a given default value.
    /// </summary>
    public static void Populate<T>(this T[] arr, T value)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = value;
        }
    }
    /// <summary>
    /// Populates an array via the given factory function.
    /// </summary>
    public static void Populate<T>(this T[] arr, Func<T> valFactory)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = valFactory();
        }
    }
    /// <summary>
    /// O(n) Fisher-Yates shuffle.
    /// </summary>
    public static List<T> FYShuffle<T>(this List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int k = Random.Range(0, i + 1);
            (list[i], list[k]) = (list[k], list[i]);
        }
        return list;
    }
    /// <summary>
    /// O(n) Fisher-Yates shuffle.
    /// </summary>
    public static T[] FYShuffle<T>(this T[] arr)
    {
        for (int i = arr.Length - 1; i > 0; i--)
        {
            int k = Random.Range(0, i + 1);
            (arr[i], arr[k]) = (arr[k], arr[i]);
        }
        return arr;
    }
    /// <summary>
    /// Fisher-Yates shuffle on a subsection of the array.
    /// </summary>
    public static T[] FYSubShuffle<T>(this T[] arr, int start, int length)
    {
        for (int i = start + length - 1; i > start; i--)
        {
            int k = Random.Range(start, i + 1);
            (arr[i], arr[k]) = (arr[k], arr[i]);
        }
        return arr;
    }
    /// <summary>
    /// Fisher-Yates shuffle on a rectangular cut of the array.
    /// </summary>
    public static T[] FYRectShuffle<T>(this T[] arr, int x, int y, int width, int height)
    {
        for (int mY = y + height - 1; mY >= y; mY--)
        {
            for (int mX = x + width - 1; mX >= x; mX--)
            {
                int rY = Random.Range(y, mY + 1);
                int rX = Random.Range(x, mX + 1);
                (arr[mY * width + mX], arr[rY * width + rX])
                    = (arr[rY * width + rX], arr[mY * width + mX]);
            }
        }
        return arr;
    }
}