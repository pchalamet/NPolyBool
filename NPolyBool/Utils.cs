﻿using System.Collections.Generic;

namespace PolyBool
{
    internal static class Utils
    {
        public static void Shift<T>(this List<T> lst)
        {
            lst.RemoveAt(0);
        }

        public static void Pop<T>(this List<T> lst)
        {
            lst.RemoveAt(lst.Count-1);
        }

        public static void Splice<T>(this List<T> source, int index, int count)
        {
            source.RemoveRange(index, count);
        }

        public static void Push<T>(this List<T> source, T elem)
        {
            source.Add(elem);
        }

        public static void Unshift<T>(this List<T> source, T elem)
        {
            source.Insert(0, elem);
        }
    }
}