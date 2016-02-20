﻿using System;
using System.Collections.Generic;

namespace JPEG.ExtensionsMethods
{
    internal static class EnumerableExtensions
	{
		public static T MinOrDefault<T>(this IEnumerable<T> enumerable, Func<T, int> selector) => 
            BestOrDefault(enumerable, (arg1, arg2) => selector(arg1) < selector(arg2));

        public static T BestOrDefault<T>(this IEnumerable<T> enumerable, Func<T, T, bool> firstIsBetter)
		{
			var enumerator = enumerable.GetEnumerator();
			if(!enumerator.MoveNext())
				return default(T);
			var best = enumerator.Current;
			while(enumerator.MoveNext())
			{
				var item = enumerator.Current;
				if(firstIsBetter(item, best))
					best = item;
			}
			return best;
		}
	}
}