using System;

namespace Search.Flights;

public class FlightLookup
{

    private static Lazy<FlightLookup> Instance = new(() => new());

    public static FlightLookup GetInstance() {
        return Instance.Value;
    }

    public List<FlightResult> Search(String from, String to, DateOnly date) {
        return [];
    }

}

public record FlightResult()
{
}