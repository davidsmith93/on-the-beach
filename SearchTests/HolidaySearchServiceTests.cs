namespace SearchTests;

using Moq;
using Search;
using Search.Flights;
using Search.Hotels;

public class HolidaySearchServiceTests
{

    private HolidaySearchService subject;
    private Mock<IFlightLookup> flightLookup;
    private Mock<IHotelLookup> hotelLookup;

    [SetUp]
    public void Setup()
    {
        flightLookup = new();
        hotelLookup = new();
        subject = new(flightLookup.Object, hotelLookup.Object);
    }

    [Test]
    public void Search_CallsFlightLookupWithExpectedData()
    {
        flightLookup.Setup(m => m.Search(It.IsAny<Airport>(), It.IsAny<Airport>(), It.IsAny<DateOnly>()))
            .Returns(() => []);

        hotelLookup.Setup(m => m.Search(It.IsAny<Airport>(), It.IsAny<DateOnly>(), It.IsAny<int>()))
            .Returns(() => []);

        var from = new Airport(Iata.MAN, null);
        var to = new Airport(null, null);
        var request = new HolidaySearchRequest(from, to, DateOnly.MaxValue, new Duration(10));

        subject.Search(request);

        flightLookup.Verify(m => m.Search(from, to, request.Date));
    }

    [Test]
    public void Search_CallsHotelLookupWithExpectedData()
    {
        flightLookup.Setup(m => m.Search(It.IsAny<Airport>(), It.IsAny<Airport>(), It.IsAny<DateOnly>()))
            .Returns(() => []);

        hotelLookup.Setup(m => m.Search(It.IsAny<Airport>(), It.IsAny<DateOnly>(), It.IsAny<int>()))
            .Returns(() => []);

        var from = new Airport(Iata.MAN, null);
        var to = new Airport(null, null);
        var request = new HolidaySearchRequest(from, to, DateOnly.MaxValue, new Duration(10));

        subject.Search(request);

        hotelLookup.Verify(m => m.Search(to, request.Date, request.Duration.Days));
    }

    [Test]
    public void Search_ReturnsExpectedResults_OrderedByTotalPrice()
    {
        flightLookup.Setup(m => m.Search(It.IsAny<Airport>(), It.IsAny<Airport>(), It.IsAny<DateOnly>()))
            .Returns(() => [
                new(1, "Flight 1", "MAN", "LPL", 100, DateOnly.MaxValue),
                new(2, "Flight 2", "MAN", "LPL", 50, DateOnly.MaxValue),
                new(3, "Flight 3", "MAN", "LPL", 25, DateOnly.MaxValue)
            ]);

        hotelLookup.Setup(m => m.Search(It.IsAny<Airport>(), It.IsAny<DateOnly>(), It.IsAny<int>()))
            .Returns(() => [
                new(1, "Hotel 1", DateOnly.MaxValue, 100, ["LPL"], 10, 1000),
                new(2, "Hotel 2", DateOnly.MaxValue, 50, ["LPL"], 10, 500),
                new(3, "Hotel 3", DateOnly.MaxValue, 200, ["LPL"], 10, 2000)
            ]);

        var from = new Airport(Iata.MAN, null);
        var to = new Airport(null, null);
        var request = new HolidaySearchRequest(from, to, DateOnly.MaxValue, new Duration(10));

        var results = subject.Search(request).Results;
        Assert.That(results, Has.Count.EqualTo(9));

        AssertResult(results[0], 3, 2, 525);
        AssertResult(results[1], 2, 2, 550);
        AssertResult(results[2], 1, 2, 600);
        AssertResult(results[3], 3, 1, 1025);
        AssertResult(results[4], 2, 1, 1050);
        AssertResult(results[5], 1, 1, 1100);
        AssertResult(results[6], 3, 3, 2025);
        AssertResult(results[7], 2, 3, 2050);
        AssertResult(results[8], 1, 3, 2100);
    }

    private void AssertResult(HolidaySearchResult result, int flightId, int hotelId, decimal totalPrice)
    {
        Assert.Multiple(() => {
            Assert.That(result.Flight.Id, Is.EqualTo(flightId));
            Assert.That(result.Hotel.Id, Is.EqualTo(hotelId));
            Assert.That(result.TotalPrice.Value, Is.EqualTo(totalPrice));
        });
    }

}
