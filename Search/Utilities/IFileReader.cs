namespace Search.Utilities;

public interface IFileReader
{
    T Load<T>(string fileName);
}