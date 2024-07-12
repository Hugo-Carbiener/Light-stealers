using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class SerializableDictionary<TKey, TValue>: IEnumerable<KeyValuePair<TKey, TValue>>
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

    public TValue this[TKey key]
    {
        get
        {
            return GetValue(key);
        }
    }

    public TKey this[TValue value]
    {
        get
        {
            return GetKey(value);
        }
    }

    private TValue GetValue(TKey key)
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

    private TKey GetKey(TValue value)
    {
        foreach (DictionaryItem item in elements)
        {
            if (item.value.Equals(value))
            {
                return item.key;
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
        return elements.Exists(elem => elem.key.Equals(key));
    }

    public bool ContainsValue(TValue value)
    {
        return elements.Exists(elem => elem.value.Equals(value));
    }

    public List<TKey> GetKeys() { return elements.Select(elem => elem.key).ToList(); }
    public List<TValue> GetValues() { return elements.Select(elem => elem.value).ToList(); }

    public Dictionary<TKey, TValue> ToDictionnary()
    {
        Dictionary<TKey, TValue> dictionnary = new Dictionary<TKey, TValue>();
        elements.ForEach(element => dictionnary.Add(element.key, element.value));

        return dictionnary;
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        foreach (DictionaryItem element in elements)
        {
            yield return new KeyValuePair<TKey, TValue>(element.key, element.value);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}