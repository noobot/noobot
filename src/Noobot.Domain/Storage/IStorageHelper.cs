namespace Noobot.Domain.Storage
{
    public interface IStorageHelper
    {
        T[] ReadFile<T>(string fileName) where T : class, new();
        void SaveFile<T>(string fileName, T[] objects) where T : class, new();
        void DeleteFile(string fileName);
    }
}