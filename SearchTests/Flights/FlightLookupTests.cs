using Search.Flights;

namespace SearchTests.Flights;

public class FlightLookupTests
{

    private FlightLookup subject;

    [SetUp]
    public void Setup()
    {
        subject = new();
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
            Assert.That(flight.Id, Is.EqualTo(2), "Id not equal");
            Assert.That(flight.From, Is.EqualTo(from), "From not equal");
            Assert.That(flight.To, Is.EqualTo(to), "To not equal");
            Assert.That(flight.Airline, Is.EqualTo("Oceanic Airlines"), "Airline not equal");
            Assert.That(flight.Price, Is.EqualTo(245), "Price not equal");
            Assert.That(flight.DepartureDate, Is.EqualTo(date), "DepartureDate not equal");
        });
    }

}
