using Newtonsoft.Json;
using Search.Exception;

namespace Search.Flights;

public class FlightLookup
{

    private static readonly Lazy<FlightLookup> Instance = new(() => new());

    private List<Flight> Flights = [];

    public static FlightLookup GetInstance()
    {
        return Instance.Value;
    }

    public List<Flight> Search(string from, string to, DateOnly date)
    {
        if(Flights.Count == 0)
        {
            Load();
        }

        return Flights.FindAll(flight => flight.From == from && flight.To == to && flight.DepartureDate == date)
            .OrderBy(flight => flight.Price)
            .ToList();
    }

    private void Load()
    {
        string fileName = "flights.json";
        try
        {
            using StreamReader reader = new("flights.json");

            string content = reader.ReadToEnd();

            List<Flight>? flights = JsonConvert.DeserializeObject<List<Flight>>(content) ?? throw new NoDataException($"{fileName} had no data to read");
            this.Flights = flights;
        }
        catch (IOException e)
        {
            throw new FileParseException($"{fileName} could not be read", e);
        }
    }
}

public record Flight(
    [JsonProperty("id")] int Id,
    [JsonProperty("airline")] string Airline,
    [JsonProperty("from")] string From,
    [JsonProperty("to")] string To,
    [JsonProperty("price")] decimal Price,
    [JsonProperty("departure_date")] DateOnly DepartureDate
)
{
}