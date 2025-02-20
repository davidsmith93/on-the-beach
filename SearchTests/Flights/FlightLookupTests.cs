using Moq;
using Search.Flights;
using Search.Utilities;

namespace SearchTests.Flights;

public class FlightLookupTests
{

    private FlightLookup subject;
    private Mock<IFileReader> fileReader;

    [SetUp]
    public void Setup()
    {
        fileReader = new Mock<IFileReader>();
        subject = new(fileReader.Object);
    }

    [Test]
    public void Search_WillReload_WhenNoFlightsAreLoaded()
    {
        string from = "MAN";
        string to = "LPL";
        DateOnly date = DateOnly.MaxValue;

        fileReader.Setup(m => m.Load<List<FlightEntity>>(It.IsAny<string>()))
            .Returns([]);

        List<Flight> flights = subject.Search(from, to, date);
        Assert.That(flights, Has.Count.EqualTo(0));

        subject.Search(from, to, date);

        fileReader.Verify(m => m.Load<List<FlightEntity>>("flights.json"), Times.Exactly(2));
    }

    [Test]
    public void Search_WillNotReload_WhenHotelsAreLoaded()
    {
        string from = "MAN";
        string to = "LPL";
        DateOnly date = DateOnly.MaxValue;

        fileReader.Setup(m => m.Load<List<FlightEntity>>(It.IsAny<string>()))
            .Returns([
                new (1, "Flight 1", from, to, 10, date)
            ]);

        subject.Search(from, to, date);
        subject.Search(from, to, date);

        fileReader.Verify(m => m.Load<List<FlightEntity>>("flights.json"), Times.Once);
    }

    [Test]
    public void Search_ReturnsExpectedFlight_WhenDateFromAndToMatches()
    {
        string from = "MAN";
        string to = "LPL";
        DateOnly date = DateOnly.MaxValue;

        fileReader.Setup(m => m.Load<List<FlightEntity>>(It.IsAny<string>()))
            .Returns([
                new (1, "Flight 1", "JFK", to, 10, date),
                new (2, "Flight 2", from, to, 10, date),
                new (3, "Flight 3", from, "JFK", 10, date),
                new (4, "Flight 4", from, to, 10, DateOnly.MinValue)
            ]);

        List<Flight> flights = subject.Search(from, to, date);
        
        Assert.That(flights, Has.Count.EqualTo(1), "Only 1 flight expected");
        
        Flight flight_1 = flights[0];
        Assert.Multiple(() =>
        {
            Assert.That(flight_1.Id, Is.EqualTo(2), "Id not equal");
            Assert.That(flight_1.Airline, Is.EqualTo("Flight 2"), "Name not equal");
            Assert.That(flight_1.From, Is.EqualTo(from), "From not equal");
            Assert.That(flight_1.To, Is.EqualTo(to), "To not equal");
            Assert.That(flight_1.Price, Is.EqualTo(10), "Price not equal");
            Assert.That(flight_1.DepartureDate, Is.EqualTo(date), "DepartureDate not equal");
        });
    }

    [Test]
    public void Search_ReturnsExpectedFlightsWithDifferentPrices_InOrder()
    {
        string from = "MAN";
        string to = "LPL";
        DateOnly date = DateOnly.MaxValue;

        fileReader.Setup(m => m.Load<List<FlightEntity>>(It.IsAny<string>()))
            .Returns([
                new (1, "Flight 1", from, to, 20, date),
                new (2, "Flight 2", from, to, 30, date),
                new (3, "Flight 3", from, to, 10, date)
            ]);

        List<Flight> flights = subject.Search(from, to, date);
        
        Assert.That(flights, Has.Count.EqualTo(3), "Only 3 flights expected");
        
        Flight flight_1 = flights[0];
        Assert.Multiple(() =>
        {
            Assert.That(flight_1.Id, Is.EqualTo(3), "Id not equal");
            Assert.That(flight_1.Price, Is.EqualTo(10), "Price not equal");
        });

        Flight flight_2 = flights[1];
        Assert.Multiple(() =>
        {
            Assert.That(flight_2.Id, Is.EqualTo(1), "Id not equal");
            Assert.That(flight_2.Price, Is.EqualTo(20), "Price not equal");
        });

        Flight flight_3 = flights[2];
        Assert.Multiple(() =>
        {
            Assert.That(flight_3.Id, Is.EqualTo(2), "Id not equal");
            Assert.That(flight_3.Price, Is.EqualTo(30), "Price not equal");
        });
    }

    [Test]
    public void Search_ReturnsExpectedHotelsWithWhenSamePrice_InOrder()
    {
        string from = "MAN";
        string to = "LPL";
        DateOnly date = DateOnly.MaxValue;

        fileReader.Setup(m => m.Load<List<FlightEntity>>(It.IsAny<string>()))
            .Returns([
                new (3, "Flight 3", from, to, 10, date),
                new (2, "Flight 2", from, to, 10, date),
                new (1, "Flight 1", from, to, 10, date)
            ]);

        List<Flight> flights = subject.Search(from, to, date);
        
        Assert.That(flights, Has.Count.EqualTo(3), "Only 3 flights expected");
        
        Flight flight_1 = flights[0];
        Assert.That(flight_1.Id, Is.EqualTo(1), "Id not equal");

        Flight flight_2 = flights[1];
        Assert.That(flight_2.Id, Is.EqualTo(2), "Id not equal");

        Flight flight_3 = flights[2];
        Assert.That(flight_3.Id, Is.EqualTo(3), "Id not equal");
    }

}
