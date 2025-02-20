using Search;
using Search.Flights;
using Search.Hotels;
using Search.Utilities;

namespace SearchTests;

public class HolidaySearchIT
{
    private HolidaySearchService Subject;

    [SetUp]
    public void Setup()
    {
        var fileReader = new FileReader();
        var flightLookup = new FlightLookup(fileReader);
        var hotelLookup = new HotelLookup(fileReader);
        
        Subject = new(flightLookup, hotelLookup);
    }

    [Test]
    //Test case 1 from spec
    public void Search_FromMANToAGPOnJulyFirstTwentyThreeForSevenNights_ReturnsExpectedResults()
    {
        var request = new HolidaySearchRequest(
            new(Iata.MAN, null),
            new(Iata.AGP, null),
            new(2023, 7, 1),
            new(7)
        );
        var response = Subject.Search(request);

        Assert.That(response.Results, Has.Count.EqualTo(1));

        var result = response.Results[0];
        Assert.Multiple(() => {
            Assert.That(result.Flight.Id, Is.EqualTo(2));
            Assert.That(result.Hotel.Id, Is.EqualTo(9));
        });
    }

    [Test]
    //Test case 2 from spec
    public void Search_FromLondonToPMIOnJuneFiftheenthTwentyThreeForTenNights_ReturnsExpectedResults()
    {
        var request = new HolidaySearchRequest(
            new(null, City.London),
            new(Iata.PMI, null),
            new(2023, 6, 15),
            new(10)
        );
        var response = Subject.Search(request);

        Assert.That(response.Results, Has.Count.EqualTo(4));
        Assert.Multiple(() => {
            Assert.That(response.Results[0].Flight.Id, Is.EqualTo(6));
            Assert.That(response.Results[0].Hotel.Id, Is.EqualTo(5));
            Assert.That(response.Results[0].TotalPrice.Value, Is.EqualTo(675));
        });

        Assert.Multiple(() => {
            Assert.That(response.Results[1].Flight.Id, Is.EqualTo(4));
            Assert.That(response.Results[1].Hotel.Id, Is.EqualTo(5));
            Assert.That(response.Results[1].TotalPrice.Value, Is.EqualTo(753));
        });

        Assert.Multiple(() => {
            Assert.That(response.Results[2].Flight.Id, Is.EqualTo(6));
            Assert.That(response.Results[2].Hotel.Id, Is.EqualTo(13));
            Assert.That(response.Results[2].TotalPrice.Value, Is.EqualTo(3025));
        });

        Assert.Multiple(() => {
            Assert.That(response.Results[3].Flight.Id, Is.EqualTo(4));
            Assert.That(response.Results[3].Hotel.Id, Is.EqualTo(13));
            Assert.That(response.Results[3].TotalPrice.Value, Is.EqualTo(3103));
        });
    }

    [Test]
    //Test case 3 from spec
    public void Search_FromAnywhereToLPAOnNovemberEleventhTwentyTwoForFourteenNights_ReturnsExpectedResults()
    {
        var request = new HolidaySearchRequest(
            null,
            new(Iata.LPA, null),
            new(2022, 11, 10),
            new(14)
        );
        var response = Subject.Search(request);

        Assert.That(response.Results, Has.Count.EqualTo(1));

        var result = response.Results[0];
        Assert.Multiple(() => {
            Assert.That(result.Flight.Id, Is.EqualTo(7));
            Assert.That(result.Hotel.Id, Is.EqualTo(6));
        });
    }

    [Test]
    //Could be used to display the best deals available
    public void Search_FromAnywhereToAnywhereOnJulyFirstTwentyThreeForFourteenNights_ReturnsExpectedResults()
    {
        var request = new HolidaySearchRequest(
            null,
            null,
            new(2023, 7, 1),
            new(14)
        );
        var results = Subject.Search(request).Results;

        Assert.That(results, Has.Count.EqualTo(3));

        Assert.Multiple(() => {
            Assert.That(results[0].Flight.Id, Is.EqualTo(11));
            Assert.That(results[0].Hotel.Id, Is.EqualTo(12));
            Assert.That(results[0].TotalPrice.Value, Is.EqualTo(785));
        });

        Assert.Multiple(() => {
            Assert.That(results[1].Flight.Id, Is.EqualTo(10));
            Assert.That(results[1].Hotel.Id, Is.EqualTo(12));
            Assert.That(results[1].TotalPrice.Value, Is.EqualTo(855));
        });

        Assert.Multiple(() => {
            Assert.That(results[2].Flight.Id, Is.EqualTo(2));
            Assert.That(results[2].Hotel.Id, Is.EqualTo(12));
            Assert.That(results[2].TotalPrice.Value, Is.EqualTo(875));
        });
    }
}
