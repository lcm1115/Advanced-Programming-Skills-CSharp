using System;
using System.Text;

public class Message : IComparable {
    private string fromEmail { get; set; }
    private string fromFirst { get; set; }
    private string fromLast { get; set; }
    private string body { get; set; }
    private DateTime timestamp { get; set; }

    public Message(string fromEmail,
                   string fromFirst,
                   string fromLast,
                   string body,
                   DateTime timestamp) {
        this.fromEmail = fromEmail;
        this.fromFirst = fromFirst;
        this.fromLast = fromLast;
        this.body = body;
        this.timestamp = timestamp;
    }

    public override string ToString() {
        return "From: " + fromFirst + " " + fromLast + " (" + fromEmail + ")\n"
               + "Time: " + timestamp + "\n"
               + "Message: " + body;
    }

    public int CompareTo(object obj) {
        if (obj == null) return 1;

        Message otherMessage = obj as Message;
        if (otherMessage != null) {
            return timestamp.CompareTo(otherMessage.timestamp);
        } else {
            throw new ArgumentException("Object is not a Message");
        }
    }
}
