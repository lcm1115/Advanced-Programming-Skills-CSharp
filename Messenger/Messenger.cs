using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;

public class Messenger {
    const string HOST = "http://www.cs.rit.edu/~jsb/2135/ProgSkills/Labs/Messenger/api.php";
    private WebClient client;

    static void Main(String[] args) {
        Messenger m = new Messenger("lcm1115@rit.edu", "thisisthebestpassword1");
        //m.sendMessage("lcm1115@rit.edu", Console.ReadLine());
        foreach (Object o in m.getUsers()) {
            Console.WriteLine(o);
        }
    }

    public Messenger(string email, string password) {
        client = new WebClient();
        client.QueryString.Add("email", email);
        client.QueryString.Add("password", password);
    }

    public string createAccount(string first, string last) {
        client.QueryString.Add("command", "createAccount");
        client.QueryString.Add("first_name", first);
        client.QueryString.Add("last_name", last);
        string response = client.DownloadString(HOST);
        client.QueryString.Remove("first_name");
        client.QueryString.Remove("last_name");
        return response;
    }

    public List<Location> getLocations() {
        client.QueryString.Add("command", "getLocations");
        string response = client.DownloadString(HOST);
        List<Dictionary<string, string>> messagesJson = JsonConvert
            .DeserializeObject<List<Dictionary<string, string>>>(response);
        List<Location> locations = new List<Location>();
        foreach (Dictionary<string, string> d in messagesJson) {
            string email = d["email"];
            string first = d["first_name"];
            string last = d["last_name"];
            string latitude = d["latitude"];
            string longitude = d["longitude"];
            string accuracy = d["accuracy"];
            DateTime lastUpdated;
            try {
                lastUpdated = DateTime.Parse(d["lastUpdated"]);
            } catch (System.FormatException) {
                lastUpdated = new DateTime();
            }
            locations.Add(new Location(email,
                                       first,
                                       last,
                                       latitude,
                                       longitude,
                                       accuracy,
                                       lastUpdated));
        }
        return locations;
    }

    public List<Message> getMessages() {
        client.QueryString.Add("command", "getMessages");
        string response = client.DownloadString(HOST);
        List<Dictionary<string, string>> messagesJson = JsonConvert
            .DeserializeObject<List<Dictionary<string, string>>>(response);
        List<Message> messages = new List<Message>();
        foreach (Dictionary<string, string> d in messagesJson) {
            string fromEmail = d["email"];
            string fromFirst = d["first_name"];
            string fromLast = d["last_name"];
            string body = d["message"];
            DateTime timestamp = DateTime.Parse(d["ts"]);
            messages.Add(new Message(
                            fromEmail, fromFirst, fromLast, body, timestamp));
        }
        messages.Sort();
        return messages;
    }

    public List<User> getUsers() {
        client.QueryString.Add("command", "getUsers");
        string response = client.DownloadString(HOST);
        List<Dictionary<string, string>> messagesJson = JsonConvert
            .DeserializeObject<List<Dictionary<string, string>>>(response);
        List<User> users = new List<User>();
        foreach (Dictionary<string, string> d in messagesJson) {
            string email = d["email"];
            string first = d["first_name"];
            string last = d["last_name"];
            users.Add(new User(email, first, last));
        }
        return users;
    }

    public string sendMessage(string to, string message) {
        client.QueryString.Add("command", "sendMessage");
        client.QueryString.Add("to", to);
        client.QueryString.Add("message", message);
        string response = client.DownloadString(HOST);
        client.QueryString.Remove("to");
        client.QueryString.Remove("remove");
        return response;
    }

    public string setLocation(string latitude, string longitude, string accuracy) {
        client.QueryString.Add("command", "setLocation");
        client.QueryString.Add("latitude", latitude);
        client.QueryString.Add("longitude", longitude);
        client.QueryString.Add("accuracy", accuracy);
        string response = client.DownloadString(HOST);
        client.QueryString.Remove("latitude");
        client.QueryString.Remove("longitude");
        client.QueryString.Remove("accuracy");
        return response;
    }

    public string setPush(string pushUrl) {
        client.QueryString.Add("command", "setPush");
        client.QueryString.Add("pushUrl", pushUrl);
        string response = client.DownloadString(HOST);
        client.QueryString.Remove("pushUrl");
        return response;
    }
}
