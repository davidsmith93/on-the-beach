using Newtonsoft.Json;
using Search.Exception;
using Search.Utilities;

namespace Search.Flights;

public class FlightLookup(IFileReader fileReader) : IFlightLookup
{
    private static readonly string FILE_NAME = "flights.json";

    private readonly IFileReader fileReader = fileReader;
    private List<FlightEntity> Flights = [];

    public List<Flight> Search(Airport? from, Airport? to, DateOnly date)
    {
        if(Flights.Count == 0)
        {
            Load();
        }

        return Flights.FindAll(flight => CompareFlight(from, to, date, flight))
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

    private bool CompareFlight(Airport? from, Airport? to, DateOnly date, FlightEntity flight) {
        return CompareFlight(from, Iata.Lookup(flight.From)) &&
            CompareFlight(to, Iata.Lookup(flight.To)) &&
            flight.DepartureDate == date;
    }

    private bool CompareFlight(Airport? airport, Iata iata) {
        if(airport == null) {
            return true;
        } else if(airport.Iata != null && airport.City == null) {
            return airport.Iata.Name == iata.Name;
        } else if(airport.Iata == null && airport.City != null) {
            return airport.City.Iatas.Contains(iata);
        } else {
            throw new ValidationException("Provided airport is invalid. Iata or City must be provided.");
        }
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
