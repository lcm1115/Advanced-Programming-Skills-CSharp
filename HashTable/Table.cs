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
        int hash = k.GetHashCode() % capacity;
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
            row.Add(new Pair<Key, Value>(k, v));
            ++size;
        }
    }

    public bool Contains(Key k) {
        int hash = k.GetHashCode() % capacity;
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
        if ((double) size / capacity > loadThreshold) {
            Rehash(2 * capacity + 1);
        }

        int hash = k.GetHashCode() % capacity;
        List<Pair<Key, Value> > row = table[hash];
        
        bool exists = false;
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
        return null;
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
                int hash = entry.First.GetHashCode() % capacity;
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
}
