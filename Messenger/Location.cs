using System;
using System.Text;

public class Location {
    private User user;
    private string latitude { get; set; }
    private string longitude { get; set; }
    private string accuracy { get; set; }
    private DateTime lastUpdated { get; set; }

    public Location(string email,
                    string first,
                    string last,
                    string latitude,
                    string longitude,
                    string accuracy,
                    DateTime lastUpdated) {
        user = new User(email, first, last);
        this.latitude = latitude;
        this.longitude = longitude;
        this.accuracy = accuracy;
        this.lastUpdated = lastUpdated;
    }

    public override string ToString() {
        return "User: " + user + "\n" +
               "Location: (" + latitude + "," + longitude + ")\n" +
               "Last Updated: " + lastUpdated;
    }
}
