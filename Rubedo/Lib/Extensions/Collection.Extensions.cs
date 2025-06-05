using System.Collections;
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
}