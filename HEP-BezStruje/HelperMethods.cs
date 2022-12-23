namespace PowerOutageNotifier;

public static class HelperMethods
{
    public static PowerOutage GeneratePowerOutageObject(List<string> location, List<string> time, DateTime date, string url)
    {
        location[0] = location[0].Replace("Mjesto:", "").TrimStart();
        location[1] = location[1].Replace("Ulica:", "").TrimStart();
        location[2] = location[2].Replace("Napomena:", "").TrimStart();
        ParseSreet(location[1]);

        PowerOutage powerOutage = new()
        {
            Place = location[0],
            Streets = ParseSreet(location[1]),
            Comment = location[2],
            Time = time[1],
            Date = date,
            Url = url
        };

        return powerOutage;
    }

    private static List<string> ParseSreet(string adressLine)
    {
        List<string> streetList = new();
        string? street = null;
        List<string> splited = adressLine.Split(",").ToList().TrimAll();

        foreach (var line in splited)
        {
            if (line.Take(3).All(Char.IsLetter))
            {
                if(street is not null)
                    streetList.Add(street);

                street = line;
            }
            else
            {
                if (street is not null)
                {
                    street = street + ", " + line;
                }
            }
        }

        return streetList;
    }

    public static List<string> TrimAll(this List<string> stringList)
    {
        stringList.ForEach(s => s = s.Trim());

        return stringList;
    }
}
