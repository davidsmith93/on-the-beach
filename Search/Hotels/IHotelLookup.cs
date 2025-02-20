namespace Search.Hotels;

//We would use dependency injection to provide the FileReader to this
public interface IHotelLookup
{
    List<Hotel> Search(Airport? localAirport, DateOnly date, int numberOfNights);
}