namespace Search.Hotels;

//We would use dependency injection to provide the FileReader to this
public interface IHotelLookup
{
    List<Hotel> Search(string localAirport, DateOnly date, int numberOfNights);
}