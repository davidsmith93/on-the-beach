namespace Search;

public interface IHolidaySearchService
{
    HolidaySearchResponse Search(HolidaySearchRequest request);
}