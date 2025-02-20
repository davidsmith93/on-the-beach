namespace Search.Flights;

public interface IFlightLookup
{
    List<Flight> Search(Airport? from, Airport? to, DateOnly date);
}
