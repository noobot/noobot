namespace Noobot.Domain.Storage
{
    public interface IStorageHelper
    {
        T[] ReadFile<T>(string fileName) where T : class;
        void SaveFile<T>(string fileName, T[] objects) where T : class;
        void DeleteFile(string fileName);
    }
}