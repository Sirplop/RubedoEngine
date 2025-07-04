using System.Collections.Generic;
using System.Text;

namespace Rubedo.Lib;

/// <summary>
/// Offers some nicely formatted print functions.
/// </summary>
public static class PrintUtil
{
    public static string Print<T>(this List<T> list)
    {
        if (list.Count == 0)
            return "[ ]";
        StringBuilder sb = new StringBuilder();
        sb.Append("[ ");
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == null)
                sb.Append("null");
            else
                sb.Append(list[i].ToString());
            if (i != list.Count - 1)
                sb.Append(", ");
        }
        sb.Append(" ]");
        return sb.ToString();
    }

    public static string Print<T>(this T[] arr)
    {
        if (arr.Length == 0)
            return "[ ]";
        StringBuilder sb = new StringBuilder();
        sb.Append("[ ");
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] == null)
                sb.Append("null");
            else
                sb.Append(arr[i].ToString());
            if (i != arr.Length - 1)
                sb.Append(", ");
        }
        sb.Append(" ]");
        return sb.ToString();
    }
    public static string Print<T>(this HashSet<T> set)
    {
        if (set.Count == 0)
            return "{ }";
        StringBuilder sb = new StringBuilder();
        sb.Append("{ ");
        var en = set.GetEnumerator();
        while (en.MoveNext())
        {
            if (en.Current == null)
                sb.Append("null, ");
            else
            {
                sb.Append(en.Current.ToString());
                sb.Append(", ");
            }
        }
        sb.Remove(sb.Length - 2, 2);
        sb.Append(" }");
        return sb.ToString();
    }
}