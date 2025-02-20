using Newtonsoft.Json;
using Search.Exception;
using Search.Utilities;

namespace Search.Hotels;

public class HotelLookup(IFileReader fileReader) : IHotelLookup {
    private static readonly string FILE_NAME = "hotels.json";

    private readonly IFileReader fileReader = fileReader;
    private List<HotelEntity> Hotels = [];

    public List<Hotel> Search(Airport? localAirport, DateOnly date, int numberOfNights)
    {
        if(Hotels.Count == 0)
        {
            Load();
        }

        return Hotels.FindAll(hotel => CompareHotel(localAirport, date, numberOfNights, hotel))
            .ConvertAll(hotel => new Hotel(
                hotel.Id,
                hotel.Name,
                hotel.ArrivalDate,
                hotel.PricePerNight,
                hotel.LocalAirports,
                hotel.Nights,
                hotel.PricePerNight * hotel.Nights
            ))
            .OrderBy(hotel => hotel.PricePerNight)
            .ThenBy(hotel => hotel.Id)
            .ToList();
    }

    private bool CompareHotel(Airport? localAirport, DateOnly date, int numberOfNights, HotelEntity hotel) {
        return CompareHotel(localAirport, hotel) &&
        hotel.ArrivalDate == date &&
        hotel.Nights == numberOfNights;
    }

    private bool CompareHotel(Airport? localAirport, HotelEntity hotel) {
        if(localAirport == null) {
            return true;
        } else if(
            (localAirport.Iata == null && localAirport.City == null) ||
            (localAirport.Iata != null && localAirport.City != null)
        ) {
            throw new ValidationException("Provided airport is invalid. Iata or City must be provided.");
        } 
        
        var iatas = hotel.LocalAirports.ConvertAll(Iata.Lookup);
        
        if(localAirport.Iata != null && localAirport.City == null) {
            return iatas.Contains(localAirport.Iata);
        } else if(localAirport.Iata == null && localAirport.City != null) {
            return localAirport.City.Iatas.Select(iata => iata)
                .Intersect(iatas)
                .Any();
        }

        return false;
    }

    private void Load()
    {
        Hotels = fileReader.Load<List<HotelEntity>>(FILE_NAME);
    }
}

public record HotelEntity(
    [JsonProperty("id")] int Id,
    [JsonProperty("name")] string Name,
    [JsonProperty("arrival_date")] DateOnly ArrivalDate,
    [JsonProperty("price_per_night")] decimal PricePerNight,
    [JsonProperty("local_airports")] List<string> LocalAirports,
    [JsonProperty("nights")] int Nights
)
{
}

public record Hotel(
    int Id,
    string Name,
    DateOnly ArrivalDate,
    decimal PricePerNight,
    List<string> LocalAirports,
    int Nights,
    decimal TotalPrice
)
{
}