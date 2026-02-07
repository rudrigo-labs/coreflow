namespace CoreFlow.Shared.Extensions
{
    public static class DapperExtensions
    {
        public static void Map<TFirst, TSecond, TKey>(this IEnumerable<TFirst> list, IEnumerable<TSecond> child, Func<TFirst, TKey> firstKey, Func<TSecond, TKey> secondKey, Action<TFirst, IEnumerable<TSecond>> addChildren)
        {
            Func<TFirst, TKey> firstKey2 = firstKey;
            Action<TFirst, IEnumerable<TSecond>> addChildren2 = addChildren;
            Dictionary<TKey, IEnumerable<TSecond>> childMap = child.GroupBy(secondKey).ToDictionary((IGrouping<TKey, TSecond> g) => g.Key, (IGrouping<TKey, TSecond> g) => g.AsEnumerable());
            Parallel.ForEach<TFirst>(list, delegate (TFirst item)
            {
                if (childMap.Any())
                {
                    TKey val = firstKey2(item);
                    if (val != null && childMap.TryGetValue(val, out var value))
                    {
                        addChildren2(item, value);
                    }
                }
            });
        }
    }
}

