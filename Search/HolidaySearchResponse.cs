namespace Search;

public record class HolidaySearchResponse(List<HolidaySearchResult> Results)
{

}

public record class HolidaySearchResult() {

}

public record class Money(decimal Value, Currency Currency)
{

}

public enum Currency
{
    GBP
}