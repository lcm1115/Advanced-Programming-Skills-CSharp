using System;
using System.Text;

public class User {
    private string email { get; set; }
    private string first { get; set; }
    private string last { get; set; }

    public User(string email,
                string first,
                string last) {
        this.email = email;
        this.first = first;
        this.last = last;
    }

    public override string ToString() {
        return first + " " + last + " (" + email + ")";
    }
}
