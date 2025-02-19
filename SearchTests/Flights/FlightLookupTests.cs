using Search.Flights;

namespace SearchTests.Flights;

public class FlightLookupTests
{

    private FlightLookup subject;

    [SetUp]
    public void Setup()
    {
        subject = FlightLookup.GetInstance();
    }

    [Test]
    public void ExpectedFlightReturned_WhenFromMANToAGPOnJulyFirst()
    {
        string from = "MAN";
        string to = "AGP";
        DateOnly date = new(2023, 7, 1);

        List<Flight> flights = subject.Search(from, to, date);
        
        Assert.That(flights, Has.Count.EqualTo(1), "Only 1 flight expected");
        Flight flight = flights[0];

        Assert.Multiple(() =>
        {
            Assert.That(flight.Id, NUnit.Framework.Is.EqualTo(2), "Id not equal");
            Assert.That(flight.From, NUnit.Framework.Is.EqualTo(from), "From not equal");
            Assert.That(flight.To, NUnit.Framework.Is.EqualTo(to), "To not equal");
            Assert.That(flight.Airline, NUnit.Framework.Is.EqualTo("Oceanic Airlines"), "Airline not equal");
            Assert.That(flight.Price, NUnit.Framework.Is.EqualTo(245), "Price not equal");
            Assert.That(flight.DepartureDate, NUnit.Framework.Is.EqualTo(date), "DepartureDate not equal");
        });
    }

    [Test]
    public void ExpectedFlightsReturned_WhenFromLGWToAGPOnJulyFirst_OrderedByPriceAscending()
    {
        string from = "LGW";
        string to = "AGP";
        DateOnly date = new(2023, 7, 1);

        List<Flight> flights = subject.Search(from, to, date);
        
        Assert.That(flights, Has.Count.EqualTo(2), "Only 2 flights expected");
        Flight flight_1 = flights[0];

        Assert.Multiple(() =>
        {
            Assert.That(flight_1.Id, NUnit.Framework.Is.EqualTo(11), "Id not equal");
            Assert.That(flight_1.From, NUnit.Framework.Is.EqualTo(from), "From not equal");
            Assert.That(flight_1.To, NUnit.Framework.Is.EqualTo(to), "To not equal");
            Assert.That(flight_1.Airline, NUnit.Framework.Is.EqualTo("First Class Air"), "Airline not equal");
            Assert.That(flight_1.Price, NUnit.Framework.Is.EqualTo(155), "Price not equal");
            Assert.That(flight_1.DepartureDate, NUnit.Framework.Is.EqualTo(date), "DepartureDate not equal");
        });

        Flight flight_2 = flights[1];
        Assert.Multiple(() =>
        {
            Assert.That(flight_2.Id, NUnit.Framework.Is.EqualTo(10), "Id not equal");
            Assert.That(flight_2.From, NUnit.Framework.Is.EqualTo(from), "From not equal");
            Assert.That(flight_2.To, NUnit.Framework.Is.EqualTo(to), "To not equal");
            Assert.That(flight_2.Airline, NUnit.Framework.Is.EqualTo("First Class Air"), "Airline not equal");
            Assert.That(flight_2.Price, NUnit.Framework.Is.EqualTo(225), "Price not equal");
            Assert.That(flight_2.DepartureDate, NUnit.Framework.Is.EqualTo(date), "DepartureDate not equal");
        });
    }

    [Test]
    public void ExpectedFlightsReturned_WhenFromMANToLPAOnNovemberTenthTwentyThree()
    {
        string from = "MAN";
        string to = "LPA";
        DateOnly date = new(2023, 11, 10);

        List<Flight> flights = subject.Search(from, to, date);
        
        Assert.That(flights, Has.Count.EqualTo(1), "Only 1 flight expected");
        Flight flight = flights[0];

        Assert.Multiple(() =>
        {
            Assert.That(flight.Id, NUnit.Framework.Is.EqualTo(8), "Id not equal");
            Assert.That(flight.From, NUnit.Framework.Is.EqualTo(from), "From not equal");
            Assert.That(flight.To, NUnit.Framework.Is.EqualTo(to), "To not equal");
            Assert.That(flight.Airline, NUnit.Framework.Is.EqualTo("Fresh Airways"), "Airline not equal");
            Assert.That(flight.Price, NUnit.Framework.Is.EqualTo(175), "Price not equal");
            Assert.That(flight.DepartureDate, NUnit.Framework.Is.EqualTo(date), "DepartureDate not equal");
        });
    }

}
