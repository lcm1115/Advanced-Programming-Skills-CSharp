using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

public class T9 {
    private DAFSA dafsa;
    private int index;
    private List<char> entry;
    private List<string> values;
    private List<string> words;

    public T9(string db_filename) {
        string[] lines = File.ReadAllLines(db_filename);
        List<Tuple<string, string>> entries = new List<Tuple<string, string>>();
        foreach (string s in lines) {
            entries.Add(new Tuple<string, string>(ToNum(s), s));
        }
        dafsa = new DAFSA(entries);
        values = new List<string>();
        words = new List<string>();
        entry = new List<char>();
    }

    public void clearEntry() {
        entry.Clear();
        values = new List<string>();
    }

    public string PushKey(char c) {
        if (c >= '2' && c <= '9') {
            index = 0;
            entry.Add(c);
            values = dafsa.GetTerminals(new string(entry.ToArray()));
        } else if (c == '*') {
            if (entry.Count == 0 && words.Count > 0) {
                words.RemoveAt(words.Count - 1);
            }
            index = 0;
            if (entry.Count > 0) {
                entry.RemoveAt(entry.Count - 1);
            }
            values = dafsa.GetTerminals(new string(entry.ToArray()));
        } else if (c == '0') {
            if (values.Count > 0) {
                ++index;
                index %= values.Count;
            }
        } else if (c == '#') {
            if (values.Count != 0) {
                words.Add(values[index]);
            }
            index = 0;
            clearEntry();
        }

        if (values.Count == 0) {
            return GetHyphens();
        } else {
            return values[index];
        }
    }

    public string GetWords() {
        return string.Join(" ", words.ToArray());
    }

    public string BuildSentence() {
        string sentence = GetWords();
        if (values.Count == 0) {
            if (sentence.Length == 0) 
                return GetHyphens();
            else {
                return sentence + " " + GetHyphens();
            }
        } else {
            if (sentence.Length == 0) {
                return values[index];
            } else {
                return sentence + " " + values[index];
            }
        }
    }

    public string GetHyphens() {
        return String.Concat(Enumerable.Repeat("-", entry.Count));
    }

    public static string ToNum(string s) {
        StringBuilder sb = new StringBuilder();
        foreach (char c in s) {
            sb.Append(ToNum(c));
        }
        return sb.ToString();
    }

    public static char ToNum(char c) {
        int i = c - 'a';
        if (i > 17) {
            i -= 1;
        }
        i /= 3;
        if (i < 8) {
            ++i;
        }
        return Convert.ToChar(i + '1');
    }

    public static void Main(string[] args) {
        T9 t9 = new T9("english-words.txt");
        while (true) {
            string c = Console.ReadLine();
            Console.WriteLine(t9.PushKey(c[0]));
            Console.WriteLine(t9.BuildSentence());
        }
    }
}
