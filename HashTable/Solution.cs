// File: Solution.cs
// Author: Liam Morris
// Description: Implements a hash table that uses chaining in C#.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace RIT_CS {
    /// <summary>
    /// A hash table implementation that uses chaining to resolve hashing
    /// collisions.
    /// </summary>
    /// <param name="Key">the type of the table's keys</param>
    /// <param name="Value"> the type of the table's values</param>
    public class LinkedHashTable<Key, Value> : Table<Key, Value> {
        public int Count { get; set; }
        private int Capacity { get; set; }
        private double LoadThreshold;
        private List<Pair<Key, Value> >[] table;

        /// <summary>
        /// Constructor for LinkedHashTable. Reads in initial capacity and load
        /// threshold for determining when rehashing should occur.
        /// </summary>
        /// <param name="capacity">initial size of hash table</param>
        /// <param name="loadThreshold">
        /// maximum load of hash table before rehashing occurs
        /// </param>
        public LinkedHashTable(int capacity, double loadThreshold) {
            Capacity = capacity;
            LoadThreshold = loadThreshold;
            table = new List<Pair<Key, Value> >[Capacity];

            for (int i = 0; i < Capacity; ++i) {
                table[i] = new List<Pair<Key, Value> >();
            }
        }

        /// <summary>
        /// Add a new entry in the hash table. If an entry with the
        /// given key already exists, it is replaced without error.
        /// put() always succeeds.
        /// (Details left to implementing classes.)
        /// </summary>
        /// <param name="k">the key for the new or existing entry</param>
        /// <param name="v">the (new) value for the key</param>
        public void Put(Key k, Value v) {
            if (k == null) {
                throw new ArgumentNullException();
            }

            int hash = Math.Abs(k.GetHashCode()) % Capacity;
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
                if ((double) Count / Capacity > LoadThreshold) {
                    Rehash((int)(Capacity * 1.5));
                }
                hash = Math.Abs(k.GetHashCode()) % Capacity;
                row = table[hash];
                row.Add(new Pair<Key, Value>(k, v));
                ++Count;
            }
        }

        /// <summary>
        /// Does an entry with the given key exist?
        /// </summary>
        /// <param name="k">the key being sought</param>
        /// <returns>true iff the key exists in the table</returns>
        public bool Contains(Key k) {
            if (k == null) {
                return false;
            }

            int hash = Math.Abs(k.GetHashCode()) % Capacity;
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

        /// <summary>
        /// Fetch the value associated with the given key.
        /// </summary>
        /// <param name="k">The key to be looked up in the table</param>
        /// <returns>the value associated with the given key</returns>
        /// <exception cref="NonExistentKey">if Contains(key) is false</exception>
        public Value Get(Key k) {
            if (k == null) {
                throw new NonExistentKey<Key>(k);
            }

            int hash = Math.Abs(k.GetHashCode()) % Capacity;
            List<Pair<Key, Value> > row = table[hash];

            foreach (Pair<Key, Value> entry in row) {
                if (entry.First.Equals(k)) {
                    return entry.Second;
                }
            }

            throw new NonExistentKey<Key>(k);
        }

        /// <summary>
        /// Returns an enumerator over the keys of the hash table.
        /// </summary>
        /// <returns> Enumerator over hash table's keys
        public IEnumerator<Key> GetEnumerator() {
            List<Key> keys = GetKeys();
            return keys.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator over the keys of the hash table.
        /// </summary>
        /// <returns> Enumerator over hash table's keys
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        /// <summary>
        /// Constructs a list of the hash table's keys.
        /// </summary>
        /// <returns> The list of keys in the hash table
        public List<Key> GetKeys() {
            List<Key> keys = new List<Key>(Count);

            foreach (List<Pair<Key, Value> > row in table) {
                foreach(Pair<Key, Value> entry in row) {
                    keys.Add(entry.First);
                }
            }

            return keys;
        }

        /// <summary>
        /// Resizes the underlying list in the hash table to a specified
        /// capacity.
        /// </summary>
        /// <param name="capacity">the new capacity for hash table</param>
        private void Rehash(int capacity) {
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
            Capacity = capacity;
        }
    }

    /// <summary>
    /// Stores a pair of objects.
    /// </summary>
    /// <param name="T1">the first type in the pair</param>
    /// <param name="T2">the second type in the pair</param>
    class Pair<T1, T2> {
        public Pair(T1 first, T2 second) {
            First = first;
            Second = second;
        }
        public T1 First { get; set; }
        public T2 Second { get; set; }
    }

    class TestTable {
        public static void test() {
            Console.Write("Running tests.. ");
            Random r = new Random();

            // C# Hash Table
            Dictionary<string, int> golden;

            // Implementation to test
            LinkedHashTable<string, int> ht;

            // Insert 10000 values to same key
            string baseStr = "Key";
            {
                ht = new LinkedHashTable<string, int>(100, .75);
                for (int i = 0; i < 10000; ++i) {
                    ht.Put(baseStr, i);
                }
                
                Debug.Assert(ht.Count == 1);
                Debug.Assert(ht.Get(baseStr) == 9999);
            }

            // Insert 10000 random values to different keys
            {
                ht = new LinkedHashTable<string, int>(100, .75);
                golden = new Dictionary<string, int>();

                for (int i = 0; i < 10000; ++i) {
                    string key = baseStr + i.ToString();
                    int val = r.Next();
                    ht.Put(key, val);
                    golden.Add(key, val);
                }

                // Verify that keys are in table
                for (int i = 0; i < 10000; ++i) {
                    string key = baseStr + i.ToString();
                    Debug.Assert(ht.Contains(key));
                }

                // Verify that unknown key does not exist in table
                Debug.Assert(!ht.Contains("Key100000"));

                try {
                    ht.Get("Key100000");
                    Debug.Assert(false);
                } catch (NonExistentKey<string>) {
                }

                // Verify that contents are the same
                foreach (string key in golden.Keys) {
                    try {
                        Debug.Assert(golden[key].Equals(ht.Get(key)));
                    } catch (NonExistentKey<string>) {
                        Debug.Assert(false);
                    }
                }

                List<string> htKeys = ht.GetKeys();
                List<string> goldenKeys = new List<string>(golden.Keys);

                // Verify that keys are the same
                Debug.Assert((htKeys.Count == goldenKeys.Count));
                htKeys.Sort();
                goldenKeys.Sort();
                for (int i = 0; i < htKeys.Count; ++i) {
                    Debug.Assert(htKeys[i].Equals(goldenKeys[i]));
                }
            }

            // Tests with null values
            {
                ht = new LinkedHashTable<string, int>(100, .75);

                Debug.Assert(!ht.Contains(null));

                try {
                    ht.Get(null);
                    Debug.Assert(false);
                } catch(NonExistentKey<string>) {
                }

                try {
                    ht.Put(null, 0);
                    Debug.Assert(false);
                } catch (ArgumentNullException) {
                }
            }

            Console.WriteLine("PASSED");
        }
    }

    /// <summary>
    /// An exception used to indicate a problem with how
    /// a HashTable instance is being accessed
    /// </summary>
    public class NonExistentKey< Key >: Exception
    {
        /// <summary>
        /// The key that caused this exception to be raised
        /// </summary>
        public Key BadKey { get; private set; }

        /// <summary>
        /// Create a new instance and save the key that
        /// caused the problem.
        /// </summary>
        /// <param name="k">
        /// The key that was not found in the hash table
        /// </param>
        public NonExistentKey( Key k ):
            base( "Non existent key in HashTable: " + k )
        {
            BadKey = k;
        }

    }

    /// <summary>
    /// An associative (key-value) data structure.
    /// A given key may not appear more than once in the table,
    /// but multiple keys may have the same value associated with them.
    /// Tables are assumed to be of limited size are expected to automatically
    /// expand if too many entries are put in them.
    /// </summary>
    /// <param name="Key">the types of the table's keys (uses Equals())</param>
    /// <param name="Value">the types of the table's values</param>
    interface Table< Key, Value >: IEnumerable< Key >
    {
        /// <summary>
        /// Add a new entry in the hash table. If an entry with the
        /// given key already exists, it is replaced without error.
        /// put() always succeeds.
        /// (Details left to implementing classes.)
        /// </summary>
        /// <param name="k">the key for the new or existing entry</param>
        /// <param name="v">the (new) value for the key</param>
        void Put( Key k, Value v );

        /// <summary>
        /// Does an entry with the given key exist?
        /// </summary>
        /// <param name="k">the key being sought</param>
        /// <returns>true iff the key exists in the table</returns>
        bool Contains( Key k );

        /// <summary>
        /// Fetch the value associated with the given key.
        /// </summary>
        /// <param name="k">The key to be looked up in the table</param>
        /// <returns>the value associated with the given key</returns>
        /// <exception cref="NonExistentKey">if Contains(key) is false</exception>
        Value Get( Key k );
    }

    class TableFactory {
        /// <summary>
        /// Create a Table.
        /// (The student is to put a line of code in this method corresponding to
        /// the name of the Table implementor s/he has designed.)
        /// </summary>
        /// <param name="K">the key type</param>
        /// <param name="V">the value type</param>
        /// <param name="capacity">The initial maximum size of the table</param>
        /// <param name="loadThreshold">
        /// The fraction of the table's capacity that when
        /// reached will cause a rebuild of the table to a 50% larger size
        /// </param>
        /// <returns>A new instance of Table</returns>
        public static Table< K, V > Make< K, V >( int capacity = 100, double loadThreshold = 0.75 ) {
            return new LinkedHashTable<K, V>(capacity, loadThreshold);
        }
    }

    class MainClass
    {
        public static void Main( string[] args )
        {
            Table< string, string> ht = TableFactory.Make< string, string >( 4, 0.5 );
            ht.Put( "Joe", "Doe" );
            ht.Put( "Jane", "Brain" );
            ht.Put( "Chris", "Swiss" );
            try
            {
                foreach ( string first in ht )
                {
                    Console.WriteLine( first + " -> " + ht.Get( first ) );
                }
                Console.WriteLine( "=========================" );

                ht.Put( "Wavy", "Gravy" );
                ht.Put( "Chris", "Bliss" );
                foreach ( string first in ht )
                {
                    Console.WriteLine( first + " -> " + ht.Get( first ) );
                }
                Console.WriteLine( "=========================" );

                Console.Write( "Jane -> " );
                Console.WriteLine( ht.Get( "Jane" ) );
                Console.Write( "John -> " );
                Console.WriteLine( ht.Get( "John" ) );
            }
            catch ( NonExistentKey< string > nek )
            {
                Console.WriteLine( nek.Message );
                Console.WriteLine( nek.StackTrace );
            }

            Console.ReadLine();
            TestTable.test();
        }
    }
}
