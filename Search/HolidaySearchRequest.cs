namespace Search;

public record class HolidaySearchRequest(Airport From, Airport To, DateOnly Date, Duration Duration)
{
}

public record class Airport(String Iata)
{
}

public record class Duration(int Days)
{
}