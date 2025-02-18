using System.Runtime.ConstrainedExecution;
using Microsoft.VisualBasic;
using NUnit.Framework;
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
    public void FlightDataLoadedCorrectly()
    {
        string from = "MAN";
        string to = "AGP";
        DateOnly date = new(2023, 7, 1);

        List<FlightResult> results = subject.Search(from, to, date);
        
        Assert.Pass();
    }

}
