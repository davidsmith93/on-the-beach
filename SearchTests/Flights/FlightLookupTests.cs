using Moq;
using Search;
using Search.Exception;
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
        var from = new Airport(Iata.MAN, null);
        var to = new Airport(Iata.LGW, null);
        var date = DateOnly.MaxValue;

        fileReader.Setup(m => m.Load<List<FlightEntity>>(It.IsAny<string>()))
            .Returns([]);

        var flights = subject.Search(from, to, date);
        Assert.That(flights, Has.Count.EqualTo(0));

        subject.Search(from, to, date);

        fileReader.Verify(m => m.Load<List<FlightEntity>>("flights.json"), Times.Exactly(2));
    }

    [Test]
    public void Search_WillNotReload_WhenFlightsAreLoaded()
    {
        var from = new Airport(Iata.MAN, null);
        var to = new Airport(Iata.LGW, null);
        var date = DateOnly.MaxValue;

        fileReader.Setup(m => m.Load<List<FlightEntity>>(It.IsAny<string>()))
            .Returns([
                new (1, "Flight 1", from.Iata.Name, to.Iata.Name, 10, date)
            ]);

        subject.Search(from, to, date);
        subject.Search(from, to, date);

        fileReader.Verify(m => m.Load<List<FlightEntity>>("flights.json"), Times.Once);
    }

    [Test]
    public void Search_ReturnsExpectedFlights_WhenNullFromAirport() {
        var to = new Airport(Iata.LGW, null);
        var date = DateOnly.MaxValue;

        fileReader.Setup(m => m.Load<List<FlightEntity>>(It.IsAny<string>()))
            .Returns([
                new (1, "Flight 1", Iata.AGP.Name, to.Iata.Name, 10, date),
                new (2, "Flight 2", Iata.MAN.Name, to.Iata.Name, 10, date)
            ]);

        var flights = subject.Search(null, to, date);
        
        Assert.That(flights, Has.Count.EqualTo(2), "Only 2 flights expected");
        
        Assert.Multiple(() => {
            Assert.That(flights[0].Id, Is.EqualTo(1), "Id not equal");
            Assert.That(flights[1].Id, Is.EqualTo(2), "Id not equal");
        });
    }

    [Test]
    public void Search_ReturnsExpectedFlights_WhenFromAirportHasNullIataAndHasCity() {
        var from = new Airport(null, City.London);
        var to = new Airport(Iata.LGW, null);
        var date = DateOnly.MaxValue;

        fileReader.Setup(m => m.Load<List<FlightEntity>>(It.IsAny<string>()))
            .Returns([
                new (1, "Flight 1", Iata.LGW.Name, to.Iata.Name, 10, date),
                new (2, "Flight 2", Iata.MAN.Name, to.Iata.Name, 10, date),
                new (3, "Flight 3", Iata.LTN.Name, to.Iata.Name, 10, date)
            ]);

        var flights = subject.Search(from, to, date);
        
        Assert.That(flights, Has.Count.EqualTo(2), "Only 2 flights expected");
        
        Assert.Multiple(() => {
            Assert.That(flights[0].Id, Is.EqualTo(1), "Id not equal");
            Assert.That(flights[1].Id, Is.EqualTo(3), "Id not equal");
        });
    }

    [Test]
    public void Search_ThrowsException_WhenFromAirportHasIataNullAndCityNull() {
        var from = new Airport(null, null);
        var to = new Airport(Iata.LGW, null);
        var date = DateOnly.MaxValue;

        fileReader.Setup(m => m.Load<List<FlightEntity>>(It.IsAny<string>()))
            .Returns([
                new (1, "Flight 1", Iata.LGW.Name, to.Iata.Name, 10, date)
            ]);

        Assert.Throws<ValidationException>(() => subject.Search(from, to, date));
    }

    [Test]
    public void Search_ThrowsException_WhenFromAirportIataNotNullAndCityNotNull() {
        var from = new Airport(Iata.LGW, City.London);
        var to = new Airport(Iata.LGW, null);
        var date = DateOnly.MaxValue;

        fileReader.Setup(m => m.Load<List<FlightEntity>>(It.IsAny<string>()))
            .Returns([
                new (1, "Flight 1", Iata.LGW.Name, to.Iata.Name, 10, date)
            ]);

        Assert.Throws<ValidationException>(() => subject.Search(from, to, date));
    }

    [Test]
    public void Search_ReturnsExpectedFlights_WhenNullToAirport() {
        var from = new Airport(Iata.MAN, null);
        var date = DateOnly.MaxValue;

        fileReader.Setup(m => m.Load<List<FlightEntity>>(It.IsAny<string>()))
            .Returns([
                new (1, "Flight 1", from.Iata.Name, Iata.AGP.Name, 10, date),
                new (2, "Flight 2", from.Iata.Name, Iata.MAN.Name, 10, date)
            ]);

        var flights = subject.Search(from, null, date);
        
        Assert.That(flights, Has.Count.EqualTo(2), "Only 2 flights expected");
        
        Assert.Multiple(() => {
            Assert.That(flights[0].Id, Is.EqualTo(1), "Id not equal");
            Assert.That(flights[1].Id, Is.EqualTo(2), "Id not equal");
        });
    }

    [Test]
    public void Search_ReturnsExpectedFlights_WhenToAirportHasNullIataAndHasCity() {
        var from = new Airport(Iata.MAN, null);
        var to = new Airport(null, City.London);
        var date = DateOnly.MaxValue;

        fileReader.Setup(m => m.Load<List<FlightEntity>>(It.IsAny<string>()))
            .Returns([
                new (1, "Flight 1", from.Iata.Name, Iata.LGW.Name, 10, date),
                new (2, "Flight 2", from.Iata.Name, Iata.MAN.Name, 10, date),
                new (3, "Flight 3", from.Iata.Name, Iata.LTN.Name, 10, date)
            ]);

        var flights = subject.Search(from, to, date);
        
        Assert.That(flights, Has.Count.EqualTo(2), "Only 2 flights expected");
        
        Assert.Multiple(() => {
            Assert.That(flights[0].Id, Is.EqualTo(1), "Id not equal");
            Assert.That(flights[1].Id, Is.EqualTo(3), "Id not equal");
        });
    }

    [Test]
    public void Search_ThrowsException_WhenToAirportHasIataNullAndCityNull() {
        var from = new Airport(Iata.MAN, null);
        var to = new Airport(null, null);
        var date = DateOnly.MaxValue;

        fileReader.Setup(m => m.Load<List<FlightEntity>>(It.IsAny<string>()))
            .Returns([
                new (1, "Flight 1", from.Iata.Name, Iata.LGW.Name, 10, date)
            ]);

        Assert.Throws<ValidationException>(() => subject.Search(from, to, date));
    }

    [Test]
    public void Search_ThrowsException_WhenToAirportIataNotNullAndCityNotNull() {
        var from = new Airport(Iata.MAN, null);
        var to = new Airport(Iata.LGW, City.London);
        var date = DateOnly.MaxValue;

        fileReader.Setup(m => m.Load<List<FlightEntity>>(It.IsAny<string>()))
            .Returns([
                new (1, "Flight 1", from.Iata.Name, Iata.LGW.Name, 10, date)
            ]);

        Assert.Throws<ValidationException>(() => subject.Search(from, to, date));
    }

    [Test]
    public void Search_ReturnsExpectedFlight_WhenDateFromAndToMatches()
    {
        var from = new Airport(Iata.MAN, null);
        var to = new Airport(Iata.LGW, null);
        var date = DateOnly.MaxValue;

        fileReader.Setup(m => m.Load<List<FlightEntity>>(It.IsAny<string>()))
            .Returns([
                new (1, "Flight 1", Iata.AGP.Name, to.Iata.Name, 10, date),
                new (2, "Flight 2", from.Iata.Name, to.Iata.Name, 10, date),
                new (3, "Flight 3", from.Iata.Name, Iata.AGP.Name, 10, date),
                new (4, "Flight 4", from.Iata.Name, to.Iata.Name, 10, DateOnly.MinValue)
            ]);

        var flights = subject.Search(from, to, date);
        
        Assert.That(flights, Has.Count.EqualTo(1), "Only 1 flight expected");
        
        var flight_1 = flights[0];
        Assert.Multiple(() =>
        {
            Assert.That(flight_1.Id, Is.EqualTo(2), "Id not equal");
            Assert.That(flight_1.Airline, Is.EqualTo("Flight 2"), "Name not equal");
            Assert.That(flight_1.From, Is.EqualTo(from.Iata.Name), "From not equal");
            Assert.That(flight_1.To, Is.EqualTo(to.Iata.Name), "To not equal");
            Assert.That(flight_1.Price, Is.EqualTo(10), "Price not equal");
            Assert.That(flight_1.DepartureDate, Is.EqualTo(date), "DepartureDate not equal");
        });
    }

    [Test]
    public void Search_ReturnsExpectedFlightsWithDifferentPrices_InOrder()
    {
        var from = new Airport(Iata.MAN, null);
        var to = new Airport(Iata.LGW, null);
        var date = DateOnly.MaxValue;

        fileReader.Setup(m => m.Load<List<FlightEntity>>(It.IsAny<string>()))
            .Returns([
                new (1, "Flight 1", from.Iata.Name, to.Iata.Name, 20, date),
                new (2, "Flight 2", from.Iata.Name, to.Iata.Name, 30, date),
                new (3, "Flight 3", from.Iata.Name, to.Iata.Name, 10, date)
            ]);

        var flights = subject.Search(from, to, date);
        
        Assert.That(flights, Has.Count.EqualTo(3), "Only 3 flights expected");
        
        var flight_1 = flights[0];
        Assert.Multiple(() =>
        {
            Assert.That(flight_1.Id, Is.EqualTo(3), "Id not equal");
            Assert.That(flight_1.Price, Is.EqualTo(10), "Price not equal");
        });

        var flight_2 = flights[1];
        Assert.Multiple(() =>
        {
            Assert.That(flight_2.Id, Is.EqualTo(1), "Id not equal");
            Assert.That(flight_2.Price, Is.EqualTo(20), "Price not equal");
        });

        var flight_3 = flights[2];
        Assert.Multiple(() =>
        {
            Assert.That(flight_3.Id, Is.EqualTo(2), "Id not equal");
            Assert.That(flight_3.Price, Is.EqualTo(30), "Price not equal");
        });
    }

    [Test]
    public void Search_ReturnsExpectedFlightsWithWhenSamePrice_InOrder()
    {
        var from = new Airport(Iata.MAN, null);
        var to = new Airport(Iata.LGW, null);
        var date = DateOnly.MaxValue;

        fileReader.Setup(m => m.Load<List<FlightEntity>>(It.IsAny<string>()))
            .Returns([
                new (3, "Flight 3", from.Iata.Name, to.Iata.Name, 10, date),
                new (2, "Flight 2", from.Iata.Name, to.Iata.Name, 10, date),
                new (1, "Flight 1", from.Iata.Name, to.Iata.Name, 10, date)
            ]);

        var flights = subject.Search(from, to, date);
        
        Assert.That(flights, Has.Count.EqualTo(3), "Only 3 flights expected");
        
        var flight_1 = flights[0];
        Assert.That(flight_1.Id, Is.EqualTo(1), "Id not equal");

        var flight_2 = flights[1];
        Assert.That(flight_2.Id, Is.EqualTo(2), "Id not equal");

        var flight_3 = flights[2];
        Assert.That(flight_3.Id, Is.EqualTo(3), "Id not equal");
    }

}
