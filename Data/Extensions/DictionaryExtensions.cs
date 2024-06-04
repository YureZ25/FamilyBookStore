namespace Data.Extensions
{
    public static class DictionaryExtensions
    {
        public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue valueForCreating)
        {
            if (!dict.TryGetValue(key, out var value))
            {
                value = valueForCreating;
                dict.Add(key, value);
            }
            return value;
        }
    }
}
