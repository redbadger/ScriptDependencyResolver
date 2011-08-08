namespace RedBadger.Web.ScriptDependencyResolver.Extensions
{
    using System;
    using System.Collections.Generic;

    public static class DictionaryExtensions
    {
        public static TValue GetOrAdd<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> factory)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary");
            }

            if (factory == null)
            {
                throw new ArgumentNullException("factory");
            }

            lock (dictionary)
            {
                TValue value;
                if (!dictionary.TryGetValue(key, out value))
                {
                    value = factory(key);
                    dictionary.Add(key, value);
                }

                return value;
            }
        }
    }
}