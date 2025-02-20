using Moq;
using Search.Hotels;
using Search.Utilities;

namespace SearchTests.Hotels;

public class HotelLookupTests
{

    private HotelLookup subject;
    private Mock<IFileReader> fileReader;

    [SetUp]
    public void Setup()
    {
        fileReader = new Mock<IFileReader>();
        subject = new(fileReader.Object);
    }

    [Test]
    public void Search_WillReload_WhenNoHotelsAreLoaded()
    {
        string airport = "MAN";
        DateOnly date = DateOnly.MaxValue;
        int numberOfNights = 10;

        fileReader.Setup(m => m.Load<List<HotelEntity>>(It.IsAny<string>()))
            .Returns([]);

        List<Hotel> hotels = subject.Search(airport, date, numberOfNights);
        Assert.That(hotels, Has.Count.EqualTo(0));

        subject.Search(airport, date, numberOfNights);

        fileReader.Verify(m => m.Load<List<HotelEntity>>("hotels.json"), Times.Exactly(2));
    }

        [Test]
    public void Search_WillNotReload_WhenHotelsAreLoaded()
    {
        string airport = "MAN";
        DateOnly date = DateOnly.MaxValue;
        int numberOfNights = 10;

        fileReader.Setup(m => m.Load<List<HotelEntity>>(It.IsAny<string>()))
            .Returns([
                new (1, "Hotel 1", date, 10, [airport], numberOfNights)
            ]);

        subject.Search(airport, date, numberOfNights);
        subject.Search(airport, date, numberOfNights);

        fileReader.Verify(m => m.Load<List<HotelEntity>>("hotels.json"), Times.Once);
    }

    [Test]
    public void Search_ReturnsExpectedHotel_WhenDateAirportAndNightsMatches()
    {
        string airport = "MAN";
        DateOnly date = DateOnly.MaxValue;
        int numberOfNights = 10;

        fileReader.Setup(m => m.Load<List<HotelEntity>>(It.IsAny<string>()))
            .Returns([
                new (1, "Hotel 1", date, 10, ["LPL"], numberOfNights),
                new (2, "Hotel 2", date, 10, [airport], numberOfNights),
                new (3, "Hotel 3", DateOnly.MinValue, 10, [airport], numberOfNights),
                new (3, "Hotel 4", date, 10, [airport], numberOfNights + 1)
            ]);

        List<Hotel> hotels = subject.Search(airport, date, numberOfNights);
        
        Assert.That(hotels, Has.Count.EqualTo(1), "Only 1 hotel expected");
        
        Hotel hotel_1 = hotels[0];
        Assert.Multiple(() =>
        {
            Assert.That(hotel_1.Id, Is.EqualTo(2), "Id not equal");
            Assert.That(hotel_1.Name, Is.EqualTo("Hotel 2"), "Name not equal");
            Assert.That(hotel_1.ArrivalDate, Is.EqualTo(date), "ArrivalDate not equal");
            Assert.That(hotel_1.PricePerNight, Is.EqualTo(10), "PricePerNight not equal");
            Assert.That(hotel_1.LocalAirports, Has.Count.EqualTo(1), "Only 1 LocalAirport expected");
            Assert.That(hotel_1.LocalAirports[0], Is.EqualTo(airport), "Airport not equal");
            Assert.That(hotel_1.Nights, Is.EqualTo(numberOfNights), "Nights not equal");
            Assert.That(hotel_1.TotalPrice, Is.EqualTo(100), "TotalPrice not equal");
        });
    }

    [Test]
    public void Search_ReturnsExpectedHotelsWithDifferentPrices_InOrder()
    {
        string airport = "MAN";
        DateOnly date = DateOnly.MaxValue;
        int numberOfNights = 10;

        fileReader.Setup(m => m.Load<List<HotelEntity>>(It.IsAny<string>()))
            .Returns([
                new (1, "Hotel 1", date, 20, [airport], numberOfNights),
                new (2, "Hotel 2", date, 30, [airport], numberOfNights),
                new (3, "Hotel 3", date, 10, [airport], numberOfNights)
            ]);

        List<Hotel> hotels = subject.Search(airport, date, numberOfNights);
        
        Assert.That(hotels, Has.Count.EqualTo(3), "Only 3 hotels expected");
        
        Hotel hotel_1 = hotels[0];
        Assert.Multiple(() =>
        {
            Assert.That(hotel_1.Id, Is.EqualTo(3), "Id not equal");
            Assert.That(hotel_1.PricePerNight, Is.EqualTo(10), "PricePerNight not equal");
            Assert.That(hotel_1.Nights, Is.EqualTo(numberOfNights), "Nights not equal");
            Assert.That(hotel_1.TotalPrice, Is.EqualTo(100), "TotalPrice not equal");
        });

        Hotel hotel_2 = hotels[1];
        Assert.Multiple(() =>
        {
            Assert.That(hotel_2.Id, Is.EqualTo(1), "Id not equal");
            Assert.That(hotel_2.PricePerNight, Is.EqualTo(20), "PricePerNight not equal");
            Assert.That(hotel_2.Nights, Is.EqualTo(numberOfNights), "Nights not equal");
            Assert.That(hotel_2.TotalPrice, Is.EqualTo(200), "TotalPrice not equal");
        });

        Hotel hotel_3 = hotels[2];
        Assert.Multiple(() =>
        {
            Assert.That(hotel_3.Id, Is.EqualTo(2), "Id not equal");
            Assert.That(hotel_3.PricePerNight, Is.EqualTo(30), "PricePerNight not equal");
            Assert.That(hotel_3.Nights, Is.EqualTo(numberOfNights), "Nights not equal");
            Assert.That(hotel_3.TotalPrice, Is.EqualTo(300), "TotalPrice not equal");
        });
    }

    [Test]
    public void Search_ReturnsExpectedHotelsWithWhenSamePrice_InOrder()
    {
        string airport = "MAN";
        DateOnly date = DateOnly.MaxValue;
        int numberOfNights = 10;

        fileReader.Setup(m => m.Load<List<HotelEntity>>(It.IsAny<string>()))
            .Returns([
                new (3, "Hotel 3", date, 10, [airport], numberOfNights),
                new (2, "Hotel 2", date, 10, [airport], numberOfNights),
                new (1, "Hotel 1", date, 10, [airport], numberOfNights)
            ]);

        List<Hotel> hotels = subject.Search(airport, date, numberOfNights);
        
        Assert.That(hotels, Has.Count.EqualTo(3), "Only 3 hotels expected");
        
        Hotel hotel_1 = hotels[0];
        Assert.That(hotel_1.Id, Is.EqualTo(1), "Id not equal");

        Hotel hotel_2 = hotels[1];
        Assert.That(hotel_2.Id, Is.EqualTo(2), "Id not equal");

        Hotel hotel_3 = hotels[2];
        Assert.That(hotel_3.Id, Is.EqualTo(3), "Id not equal");
    }
}