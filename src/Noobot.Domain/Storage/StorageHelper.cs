namespace Noobot.Domain.Storage
{
    public class StorageHelper : IStorageHelper
    {
        public T[] ReadFile<T>(string fileName) where T : class
        {
            throw new System.NotImplementedException();
        }

        public void SaveFile<T>(string fileName, T[] objects) where T : class
        {
            throw new System.NotImplementedException();
        }

        public void DeleteFile(string fileName)
        {
            throw new System.NotImplementedException();
        }
    }
}