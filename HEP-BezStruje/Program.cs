using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using PowerOutageNotifier.Configuration;
using System.Text;
using System.Text.Json;
using static PowerOutageNotifier.HelperMethods;

namespace PowerOutageNotifier;

internal class Program
{
    static List<PowerOutage> _alreadySend = new();

    static InfoConfiguration InfoConfiguration { get; set; } = null!;
    static EmailConfiguration EmailConfiguration { get; set; } = null!;
    static EmailList EmailList { get; set; } = null!;

    static async Task Main()
    {
        // Configure app
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false);

        IConfiguration config = builder.Build();

        InfoConfiguration = config.GetSection("Info").Get<InfoConfiguration>();
        EmailConfiguration = config.GetSection("Email").Get<EmailConfiguration>();
        EmailList = config.GetSection("EmailList").Get<EmailList>();

        if (File.Exists("AlreadySend.json"))
        {
            string json = File.ReadAllText("AlreadySend.json");
            if (!string.IsNullOrEmpty(json))
            {
                _alreadySend = JsonSerializer.Deserialize<List<PowerOutage>>(json) ?? throw new ArgumentNullException(nameof(json));
            }
        }

        List<DateTime> dateList = GenerateDateList();

        List<PowerOutage> powerOutages = new();


        foreach (var date in dateList)
        {
            powerOutages.AddRange(await PowerOutageForDate(date));
        }

        await SendEmail(powerOutages);
    }

    private static async Task<List<PowerOutage>> PowerOutageForDate(DateTime date)
    {
        string url = $"{InfoConfiguration.BaseUrl}{date:dd.MM.yyyy}";
        HtmlWeb web = new();
        HtmlDocument htmlDoc = await web.LoadFromWebAsync(url);


        List<string> nodes = htmlDoc.DocumentNode.Descendants()
            .Where(n => n.HasClass("radwrap"))
            .First().ChildNodes.Where(n => n.NodeType == HtmlNodeType.Element)
            .Select(n => n.InnerText)
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .ToList();

        List<PowerOutage> powerOutages = new();

        while (nodes.Count > 0)
        {
            IEnumerable<string> item = nodes.Take(2);

            List<string> location = item
                .First()
                .Replace("\t", "")
                .Split("\r\n")
                .Where(i => !string.IsNullOrWhiteSpace(i) && !string.IsNullOrEmpty(i))
                .ToList();

            List<string> time = item
                .Skip(1)
                .Take(1)
                .First()
                .Replace("\t", "")
                .Split("\r\n")
                .Where(i => !string.IsNullOrWhiteSpace(i) && !string.IsNullOrEmpty(i))
                .ToList();

            powerOutages.Add(GeneratePowerOutageObject(location, time, date, url));
            nodes.RemoveRange(0, 2);
        }

        return powerOutages;
    }

    private static async Task SendEmail(List<PowerOutage> outages)
    {
        EmailService emailService = new(EmailConfiguration);
        StringBuilder sb = new();

        bool send = false;
        
        foreach (PowerOutage outage in outages)
        {
            if (outage.Place.Contains(InfoConfiguration.Place,StringComparison.OrdinalIgnoreCase) && outage.Streets.Any(s => s.Contains(InfoConfiguration.Street,StringComparison.OrdinalIgnoreCase)))
            {
                bool exits = _alreadySend
                    .Exists(p =>
                    p.Date.Date.Equals(outage.Date.Date) &&
                    p.Place.Equals(outage.Place) && 
                    p.Streets.SequenceEqual(outage.Streets));

                if(exits)
                    return;

                sb.Append($"<br><b>{outage.Date.ToString("dd.MM.yyyy")}</b><br>");
                sb.AppendLine($"<p><b>Mjesto: </b>{outage.Place}</p><br>");
                sb.AppendLine($"<p><b>Ulica: </b>{outage.Streets.First(s => s.Contains(InfoConfiguration.Street, StringComparison.OrdinalIgnoreCase))}</p>");
                sb.AppendLine($"<p><b>Napomena: </b>{outage.Comment}</p><br>");
                sb.AppendLine($" <a href={outage.Url}>Više</a>");
                sb.AppendLine();

                _alreadySend.Add(outage);

                send = true;
            }
        }

        if (send)
        {
            foreach (var email in EmailList.Emails)
            {
                await emailService.SendEmail(email, sb.ToString());
            }
            string jsonString = JsonSerializer.Serialize(_alreadySend,new JsonSerializerOptions { WriteIndented = true});
            File.WriteAllText(@"AlreadySend.json", jsonString);
        }
    }

    private static List<DateTime> GenerateDateList()
    {
        var dateList = new List<DateTime>
        {
            DateTime.Now
        };

        for (int i = 0; i < 2; i++)
        {
            dateList.Add(dateList.Last().AddDays(1));
        }

        return dateList;
    }
}