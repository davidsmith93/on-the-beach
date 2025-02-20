namespace Search.Flights;

public interface IFlightLookup
{
    List<Flight> Search(string from, string to, DateOnly date);
}
