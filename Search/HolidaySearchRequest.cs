namespace Search;

public record class HolidaySearchRequest(Airport? From, Airport? To, DateOnly Date, Duration Duration)
{
}

public record class Airport(Iata? Iata, City? City)
{
}

public class Iata(string name)
{
    public string Name { get; } = name;

    public static readonly Iata AGP = new("AGP");
    public static readonly Iata LGW = new("LGW");
    public static readonly Iata LPA = new("LPA");
    public static readonly Iata LTN = new("LTN");
    public static readonly Iata MAN = new("MAN");
    public static readonly Iata PMI = new("PMI");
    public static readonly Iata TFS = new("TFS");

    public static Iata Lookup(string name) {
        return name switch
        {
            "AGP" => AGP,
            "LGW" => LGW,
            "LPA" => LPA,
            "LTN" => LTN,
            "MAN" => MAN,
            "PMI" => PMI,
            "TFS" => TFS,
            _ => throw new System.Exception(),
        };
    }
}

public class City(string name, List<Iata> iatas)
{
    public string Name { get; } = name;
    public List<Iata> Iatas { get; } = iatas;

    public static readonly City London = new("London", [Iata.LGW, Iata.LTN]);
}

public record class Duration(int Days)
{
}