using Newtonsoft.Json;
using Search.Exception;

namespace Search.Utilities;

public class FileReader {

    private static readonly Lazy<FileReader> Instance = new(() => new());

    public static FileReader GetInstance()
    {
        return Instance.Value;
    }

    public T Load<T>(string fileName) {
        try
        {
            using StreamReader reader = new(fileName);

            string content = reader.ReadToEnd();

            return JsonConvert.DeserializeObject<T>(content) ??
                throw new NoDataException($"{fileName} had no data to read");
        }
        catch (IOException e)
        {
            throw new FileParseException($"{fileName} could not be read", e);
        }
    }
}