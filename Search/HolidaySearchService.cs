using Search.Flights;
using Search.Hotels;

namespace Search;

public class HolidaySearchService(IFlightLookup flightLookup, IHotelLookup hotelLookup) : IHolidaySearchService
{

    private readonly IFlightLookup flightLookup = flightLookup;
    private readonly IHotelLookup hotelLookup = hotelLookup;

    public HolidaySearchResponse Search(HolidaySearchRequest request) {
        var flights = flightLookup.Search(request.From, request.To, request.Date);
        var hotels = hotelLookup.Search(request.To, request.Date, request.Duration.Days);

        List<HolidaySearchResult> results = [];

        flights.ForEach(flight => {
            hotels.ForEach(hotel => {
                results.Add(CreateHolidaySearchResult(flight, hotel));
            });
        });

        return new (results.OrderBy(result => result.TotalPrice.Value).ToList());
    }

    private static HolidaySearchResult CreateHolidaySearchResult(Flights.Flight flight, Hotels.Hotel hotel) {
        return new(
            new(flight.Price + hotel.TotalPrice, Currency.GBP),
            CreateFlight(flight),
            CreateHotel(hotel)
        );
    }

    private static Flight CreateFlight(Flights.Flight flight) {
        return new(
            flight.Id,
            flight.Airline,
            flight.From,
            flight.To,
            new (flight.Price, Currency.GBP),
            flight.DepartureDate
        );
    }

    private static Hotel CreateHotel(Hotels.Hotel hotel) {
        return new(
            hotel.Id,
            hotel.Name,
            hotel.ArrivalDate,
            new (hotel.PricePerNight, Currency.GBP),
            hotel.LocalAirports,
            hotel.Nights
        );
    }
}
