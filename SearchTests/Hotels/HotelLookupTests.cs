using Search.Hotels;

namespace SearchTests.Hotels;

public class HotelLookupTests
{

    private HotelLookup subject;

    [SetUp]
    public void Setup()
    {
        subject = HotelLookup.GetInstance();
    }


    [Test]
    public void Search_ReturnsExpectedHotels_WhenAirportTFSOnNovemberFifthTwentyTwoForSevenNights_OrderedByPriceAscending()
    {
        string airport = "TFS";
        DateOnly date = new(2022, 11, 5);
        int numberOfNights = 7;

        List<Hotel> hotels = subject.Search(airport, date, numberOfNights);
        
        Assert.That(hotels, Has.Count.EqualTo(2), "Only 2 hotels expected");
        Hotel hotel_1 = hotels[0];

        Assert.Multiple(() =>
        {
            Assert.That(hotel_1.Id, Is.EqualTo(2), "Id not equal");
            Assert.That(hotel_1.Name, Is.EqualTo("Laguna Park 2"), "Name not equal");
            Assert.That(hotel_1.ArrivalDate, Is.EqualTo(date), "ArrivalDate not equal");
            Assert.That(hotel_1.PricePerNight, Is.EqualTo(50), "PricePerNight not equal");
            Assert.That(hotel_1.LocalAirports, Has.Count.EqualTo(1), "Only 1 LocalAirport expected");
            Assert.That(hotel_1.LocalAirports[0], Is.EqualTo(airport), "Airport not equal");
            Assert.That(hotel_1.Nights, Is.EqualTo(numberOfNights), "Nights not equal");
            Assert.That(hotel_1.TotalPrice, Is.EqualTo(350), "TotalPrice not equal");
        });

        Hotel hotel_2 = hotels[1];

        Assert.Multiple(() =>
        {
            Assert.That(hotel_2.Id, Is.EqualTo(1), "Id not equal");
            Assert.That(hotel_2.Name, Is.EqualTo("Iberostar Grand Portals Nous"), "Name not equal");
            Assert.That(hotel_2.ArrivalDate, Is.EqualTo(date), "ArrivalDate not equal");
            Assert.That(hotel_2.PricePerNight, Is.EqualTo(100), "PricePerNight not equal");
            Assert.That(hotel_2.LocalAirports, Has.Count.EqualTo(1), "Only 1 LocalAirport expected");
            Assert.That(hotel_2.LocalAirports[0], Is.EqualTo(airport), "Airport not equal");
            Assert.That(hotel_2.Nights, Is.EqualTo(numberOfNights), "Nights not equal");
            Assert.That(hotel_2.TotalPrice, Is.EqualTo(700), "TotalPrice not equal");
        });
    }

    [Test]
    public void Search_ReturnsExpectedHotels_WhenMultipleResultsWithSameData()
    {
        string airport = "PMI";
        DateOnly date = new(2023, 6, 15);
        int numberOfNights = 14;

        List<Hotel> hotels = subject.Search(airport, date, numberOfNights);
        
        Assert.That(hotels, Has.Count.EqualTo(2), "Only 2 hotels expected");
        Hotel hotel_1 = hotels[0];

        Assert.Multiple(() =>
        {
            Assert.That(hotel_1.Id, Is.EqualTo(3), "Id not equal");
            Assert.That(hotel_1.Name, Is.EqualTo("Sol Katmandu Park & Resort"), "Name not equal");
            Assert.That(hotel_1.ArrivalDate, Is.EqualTo(date), "ArrivalDate not equal");
            Assert.That(hotel_1.PricePerNight, Is.EqualTo(59), "PricePerNight not equal");
            Assert.That(hotel_1.LocalAirports, Has.Count.EqualTo(1), "Only 1 LocalAirport expected");
            Assert.That(hotel_1.LocalAirports[0], Is.EqualTo(airport), "Airport not equal");
            Assert.That(hotel_1.Nights, Is.EqualTo(numberOfNights), "Nights not equal");
            Assert.That(hotel_1.TotalPrice, Is.EqualTo(826), "TotalPrice not equal");
        });

        Hotel hotel_2 = hotels[1];

        Assert.Multiple(() =>
        {
            Assert.That(hotel_2.Id, Is.EqualTo(4), "Id not equal");
            Assert.That(hotel_2.Name, Is.EqualTo("Sol Katmandu Park & Resort"), "Name not equal");
            Assert.That(hotel_2.ArrivalDate, Is.EqualTo(date), "ArrivalDate not equal");
            Assert.That(hotel_2.PricePerNight, Is.EqualTo(59), "PricePerNight not equal");
            Assert.That(hotel_2.LocalAirports, Has.Count.EqualTo(1), "Only 1 LocalAirport expected");
            Assert.That(hotel_2.LocalAirports[0], Is.EqualTo(airport), "Airport not equal");
            Assert.That(hotel_2.Nights, Is.EqualTo(numberOfNights), "Nights not equal");
            Assert.That(hotel_2.TotalPrice, Is.EqualTo(826), "TotalPrice not equal");
        });
    }
}