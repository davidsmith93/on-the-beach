using Search.Exception;
using Search.Utilities;

namespace SearchTests.Utilities;

public class fileReaderTests {
    private FileReader subject;

    [SetUp]
    public void Setup()
    {
        subject = new();
    }

    [Test]
    public void Load_ThrowsNoDataException_WhenFileEmpty()
    {
        Assert.Throws<NoDataException>(() => subject.Load<string>("empty.json"));
    }

    [Test]
    public void Load_ThrowsNoDataException_WhenFileNotFound()
    {
        Assert.Throws<FileParseException>(() => subject.Load<string>("missing.json"));
    }
}