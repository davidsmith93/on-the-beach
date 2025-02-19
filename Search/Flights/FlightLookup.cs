using Newtonsoft.Json;

namespace Search.Flights;

public class FlightLookup
{

    private static readonly Lazy<FlightLookup> Instance = new(() => new());

    private List<Flight> Flights;

    public static FlightLookup GetInstance() {
        return Instance.Value;
    }

    public FlightLookup() {
        Load();
    }

    private void Load()
    {
        try
        {
            using StreamReader reader = new("flights.json");

            string content = reader.ReadToEnd();

            List<Flight>? flights = JsonConvert.DeserializeObject<List<Flight>>(content) ?? throw new Exception();
            this.Flights = flights;
        }
        catch (IOException e)
        {
            Console.WriteLine("The file could not be read:");
            Console.WriteLine(e.Message);
            
            throw new Exception();
        }
    }

    public List<Flight> Search(string from, string to, DateOnly date) {
        return Flights.FindAll(flight => flight.From == from && flight.To == to && flight.DepartureDate == date);
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