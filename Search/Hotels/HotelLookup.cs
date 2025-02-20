using Newtonsoft.Json;
using Search.Utilities;

namespace Search.Hotels;

public class HotelLookup(IFileReader fileReader) : IHotelLookup {
    private static readonly string FILE_NAME = "hotels.json";

    private readonly IFileReader fileReader = fileReader;
    private List<HotelEntity> Hotels = [];

    public List<Hotel> Search(string localAirport, DateOnly date, int numberOfNights)
    {
        if(Hotels.Count == 0)
        {
            Load();
        }

        return Hotels.FindAll(hotel => hotel.LocalAirports.Contains(localAirport) && hotel.ArrivalDate == date && hotel.Nights == numberOfNights)
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