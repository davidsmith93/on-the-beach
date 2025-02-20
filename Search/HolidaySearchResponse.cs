namespace Search;

public record class HolidaySearchResponse(List<HolidaySearchResult> Results)
{

}

public record class HolidaySearchResult(Money TotalPrice, Flight Flight, Hotel Hotel) {

}

public record class Money(decimal Value, Currency Currency)
{

}

public enum Currency
{
    GBP
}

public record class Flight(
    int Id,
    string Airline,
    string DepartingFrom,
    string TravelingTo,
    Money Price,
    DateOnly DepartureDate
) {

}

public record class Hotel(
    int Id,
    string Name,
    DateOnly ArrivalDate,
    Money PricePerNight,
    List<string> LocalAirports,
    int Nights
) {

}
