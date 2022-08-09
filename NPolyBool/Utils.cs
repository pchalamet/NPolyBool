﻿// The MIT License (MIT)

// Original source code Copyright (c) 2016 Sean Connelly(@voidqk, web: syntheti.cc)
// Ported source code Copyright (c) 2018 - 2022 Pierre Chalamet

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Collections.Generic;

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