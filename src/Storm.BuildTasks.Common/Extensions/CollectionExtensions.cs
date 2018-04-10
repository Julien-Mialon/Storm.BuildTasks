using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Storm.BuildTasks.Common.Extensions
{
	public static class CollectionExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue value)
		{
			if (source.ContainsKey(key))
			{
				return false;
			}

			source.Add(key, value);
			return true;
		}
	}
}