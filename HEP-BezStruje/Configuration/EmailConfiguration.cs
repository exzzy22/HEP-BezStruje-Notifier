namespace PowerOutageNotifier.Configuration;

public class EmailConfiguration
{
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string Host { get; set; } = null!;
    public int Port { get; set; }
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}
