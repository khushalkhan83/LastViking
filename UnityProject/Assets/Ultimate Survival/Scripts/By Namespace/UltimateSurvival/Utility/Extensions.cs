﻿using System.Collections.Generic;
using UnityEngine;

namespace UltimateSurvival 
{
	public static class Extensions
	{
		/// <summary>
		/// 
		/// </summary>
		public static Transform FindDeepChild(this Transform parent, string childName) 
		{
			var result = parent.Find(childName);

			if(result) 
				return result;

			for(int i = 0;i < parent.childCount;i ++)
			{
				result = parent.GetChild(i).FindDeepChild(childName);
				if(result)
					return result;
			}

			return null;
		}

		/// <summary>
		/// Checks if the index is inside the list's bounds.
		/// </summary>
		public static bool IndexIsValid<T>(this List<T> list, int index)
		{
			return index >= 0 && index < list.Count;
		}
		/// <summary>
		/// Checks if the index is inside the array's bounds.
		/// </summary>
		public static bool IndexIsValid<T>(this T[] array, int index)
		{
			return index >= 0 && index < array.Length;
		}

        public static List<T> CopyOther<T>(this List<T> list, List<T> toCopy)
        {
            if (toCopy == null || toCopy.Count == 0)
                return null;

            list = new List<T>();

            for (int i = 0; i < toCopy.Count; i++)
                list.Add(toCopy[i]);

            return list;
        }

        public static bool IsInRangeLimitsExcluded(this float f, float l1, float l2)
        {
            return f > l1 && f < l2;
        }

		public static bool IsInRangeLimitsIncluded(this float f, float l1, float l2)
		{
			return f >= l1 && f <= l2;
		}
    }
}