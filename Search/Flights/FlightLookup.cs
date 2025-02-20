using Newtonsoft.Json;
using Search.Utilities;

namespace Search.Flights;

public class FlightLookup(IFileReader fileReader) : IFlightLookup
{
    private static readonly string FILE_NAME = "flights.json";

    private readonly IFileReader fileReader = fileReader;
    private List<FlightEntity> Flights = [];

    public List<Flight> Search(string from, string to, DateOnly date)
    {
        if(Flights.Count == 0)
        {
            Load();
        }

        return Flights.FindAll(flight => flight.From == from && flight.To == to && flight.DepartureDate == date)
            .ConvertAll(flight => new Flight(
                flight.Id,
                flight.Airline,
                flight.From,
                flight.To,
                flight.Price,
                flight.DepartureDate
            ))
            .OrderBy(flight => flight.Price)
            .ThenBy(flight => flight.Id)
            .ToList();
    }

    private void Load()
    {
        Flights = fileReader.Load<List<FlightEntity>>(FILE_NAME);
    }
}

public record FlightEntity(
    [JsonProperty("id")] int Id,
    [JsonProperty("airline")] string Airline,
    [JsonProperty("from")] string From,
    [JsonProperty("to")] string To,
    [JsonProperty("price")] decimal Price,
    [JsonProperty("departure_date")] DateOnly DepartureDate
)
{
}

public record Flight(
    int Id,
    string Airline,
    string From,
    string To,
    decimal Price,
    DateOnly DepartureDate
)
{
}
