using System;
using System.Text.Json.Serialization;

namespace FireBrowserWinUi3MultiCore;

public class User
{
    private User user;
    public User(User user)
    {
        this.user = user;
    }

    public User() { }

    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }

    public bool IsFirstLaunch { get; set; }

    [JsonPropertyName("UserSettings")]
    public Settings UserSettings { get; set; }
}