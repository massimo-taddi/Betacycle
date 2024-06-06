
using System.Runtime.CompilerServices;

namespace BetaCycleAPI.Models.Cache
{
    /// <summary>
    /// In-memory LRU cache implementation.
    /// </summary>
    /// <typeparam name="K">cache item key type</typeparam>
    /// <typeparam name="V">cache item type</typeparam>
    public class InMemoryCache<K, V>
    {
        private int capacity;
        private Dictionary<K, LinkedListNode<CacheItem<K, V>>> cacheMap = new();
        // List of items in the cache, ordered by times accessed
        private LinkedList<CacheItem<K, V>> lruList = new LinkedList<CacheItem<K, V>>();

        public InMemoryCache(int capacity)
        {
            this.capacity = capacity;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public V? Get(K key)
        {
            LinkedListNode<CacheItem<K, V>> node;
            // if value is found in cachemap, return it and move it to the end of the list
            if (cacheMap.TryGetValue(key, out node))
            {
                V value = node.Value.value;
                lruList.Remove(node);
                lruList.AddLast(node);
                return value;
            }
            return default;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Add(K key, V val)
        {
            // If item already exists in cache, update it and move to the end of the list
            if (cacheMap.TryGetValue(key, out var existingNode))
            {
                lruList.Remove(existingNode);
            }
            // If cache is full, remove the first item to make space for this one
            else if (cacheMap.Count >= capacity)
            {
                RemoveFirst();
            }

            // create new cache item, add to end of list and add to cache map for quick lookup
            CacheItem<K, V> cacheItem = new CacheItem<K, V>(key, val);
            LinkedListNode<CacheItem<K, V>> node = new LinkedListNode<CacheItem<K, V>>(cacheItem);
            lruList.AddLast(node);
            cacheMap[key] = node;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool HasItem(K key)
        {
            return cacheMap.ContainsKey(key);
        }

        private void RemoveFirst()
        {
            // select item from lruList, remove it from the list and the cache
            LinkedListNode<CacheItem<K, V>> node = lruList.First;
            lruList.RemoveFirst();

            cacheMap.Remove(node.Value.key);
        }
    }

    class CacheItem<K, V>
    {
        public CacheItem(K k, V v)
        {
            key = k;
            value = v;
        }
        public K key;
        public V value;
    }
}