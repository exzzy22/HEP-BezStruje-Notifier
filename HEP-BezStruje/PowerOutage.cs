namespace PowerOutageNotifier;

public class PowerOutage
{
    public string Place { get; set; } = null!;
    public List<string> Streets { get; set; } = null!;
    public string Comment { get; set; } = "";
    public string Time { get; set; } = null!;
    public DateTime Date { get; set; }
    public string Url { get; set; } = null!;
}
