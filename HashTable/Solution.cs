using System;
using System.Collections;
using System.Collections.Generic;

namespace RIT_CS {
public class LinkedHashTable<Key, Value> : Table<Key, Value> {
    private int capacity;
    private int size;
    private double loadThreshold;
    private List<Pair<Key, Value> >[] table;

    public LinkedHashTable(int capacity, double loadThreshold) {
        this.capacity = capacity;
        this.loadThreshold = loadThreshold;
        table = new List<Pair<Key, Value> >[capacity];

        for (int i = 0; i < capacity; ++i) {
            table[i] = new List<Pair<Key, Value> >();
        }
    }

    public void Put(Key k, Value v) {
        int hash = Math.Abs(k.GetHashCode()) % capacity;
        List<Pair<Key, Value> > row = table[hash];

        bool exists = false;
        foreach (Pair<Key, Value> entry in row) {
            if (entry.First.Equals(k)) {
                entry.Second = v;
                exists = true;
                break;
            }
        }

        if (!exists) {
            if ((double) size / capacity > loadThreshold) {
                Rehash((int)(capacity * 1.5));
            }
            row.Add(new Pair<Key, Value>(k, v));
            ++size;
        }
    }

    public bool Contains(Key k) {
        int hash = Math.Abs(k.GetHashCode()) % capacity;
        List<Pair<Key, Value> > row = table[hash];
        
        bool exists = false;
        foreach (Pair<Key, Value> entry in row) {
            if (entry.First.Equals(k)) {
                exists = true;
                break;
            }
        }

        return exists;
    }

    public Value Get(Key k) {
        int hash = Math.Abs(k.GetHashCode()) % capacity;
        List<Pair<Key, Value> > row = table[hash];
        
        foreach (Pair<Key, Value> entry in row) {
            if (entry.First.Equals(k)) {
                return entry.Second;
            }
        }

        throw new NonExistentKey<Key>(k);
    }

    public IEnumerator<Key> GetEnumerator() {
        List<Key> keys = new List<Key>(size);

        foreach (List<Pair<Key, Value> > row in table) {
            foreach(Pair<Key, Value> entry in row) {
                keys.Add(entry.First);
            }
        }

        return keys.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }

    private void Rehash(int capacity) {
        Console.WriteLine("Resizing table to: " + capacity);
        // Create new table
        List<Pair<Key, Value> >[] newTable = 
            new List<Pair<Key, Value> >[capacity];
        for (int i = 0; i < capacity; ++i) {
            newTable[i] = new List<Pair<Key, Value> >();
        }

        foreach (List<Pair<Key, Value> > row in table) {
            foreach(Pair<Key, Value> entry in row) {
                int hash = Math.Abs(entry.First.GetHashCode()) % capacity;
                newTable[hash].Add(entry);
            }
        }

        table = newTable;
        this.capacity = capacity;
    }
}

public class Pair<T1, T2> {
    public Pair(T1 first, T2 second) {
        First = first;
        Second = second;
    }
    public T1 First { get; set; }
    public T2 Second { get; set; }
}

public class TestTable {
    public static void test() {
        Random r = new Random();
        LinkedHashTable<String, String> ht =
            new LinkedHashTable<String, String>(100, .75);

        string baseStr = "Key";

        for (int i = 0; i < 10000; ++i) {
            string curStr = baseStr + i.ToString();
            ht.Put(curStr, r.Next().ToString());
        }
    }
}
}
