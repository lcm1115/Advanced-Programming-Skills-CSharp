using System;
using System.Collections.Generic;

public class DAFSA {
    private static readonly List<string> NONE = new List<string>();
    private Node root;

    public DAFSA(ICollection<Tuple<string, string>> entries) {
        root = new Node();
        Node current;
        foreach (Tuple<string, string> t in entries) {
            current = root;
            foreach (char c in t.Item1) {
                current = current.GetChild(c);
                current.AddTerminal(t.Item2);
            }
        }
    }

    public List<string> GetTerminals(string s) {
        Node current = root;
        foreach (char c in s) {
            if (!current.HasChild(c)) {
                return NONE;
            }

            current = current.GetChild(c);
        }

        return current.GetTerminals();
    }

    private class Node {
        private char identifier { get; set; }
        private List<string> terminals;
        private Dictionary<char, Node> children;

        public Node() {
            children = new Dictionary<char, Node>();
            terminals = new List<string>();
        }

        public void AddTerminal(string s) {
            terminals.Add(s);
        }

        public List<string> GetTerminals() {
            return terminals;
        }

        public bool HasChild(char identifier) {
            return children.ContainsKey(identifier);
        }

        public Node GetChild(char identifier) {
            if (!children.ContainsKey(identifier)) {
                children[identifier] = new Node();
            }

            return children[identifier];
        }
    }
}
