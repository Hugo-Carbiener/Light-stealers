using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableDictionary<TKey, TValue>
{
    [System.Serializable]
    public class DictionaryItem
    {
        public TKey key;
        public TValue value;

        public DictionaryItem(TKey key, TValue value)
        {
            this.key = key;
            this.value = value;
        }
    }

    public List<DictionaryItem> elements = new List<DictionaryItem>();

    public TValue At(TKey key)
    {
        foreach(DictionaryItem item in elements)
        {
            if (item.key.Equals(key))
            {
                return item.value;
            }
        }
        return default;
    }

    public void Put(TKey key, TValue value)
    {
        foreach(DictionaryItem item in elements)
        {
            if (item.key.Equals(key))
            {
                item.value = value;
                return;
            }
        }
        DictionaryItem newItem = new DictionaryItem(key, value);
        elements.Add(newItem);
    }

    public int Count()
    {
        return elements.Count;
    }

    public bool ContainsKey(TKey key)
    {
        foreach (DictionaryItem item in elements)
        {
            if (item.key.Equals(key))
            {
                return true;
            }
        }
        return false;
    }

    public bool ContainsValue(TValue value)
    {
        foreach (DictionaryItem item in elements)
        {
            if (item.value.Equals(value))
            {
                return true;
            }
        }
        return false;
    }

    public Dictionary<TKey, TValue> ToDictionnary()
    {
        Dictionary<TKey, TValue> dictionnary = new Dictionary<TKey, TValue>();
        elements.ForEach(element => dictionnary.Add(element.key, element.value));

        return dictionnary;
    }
}