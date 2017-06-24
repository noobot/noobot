using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Common.Logging;
using FlatFile.Core;
using FlatFile.Delimited.Attributes;
using FlatFile.Delimited.Implementation;
using Noobot.Core.Plugins;

namespace Noobot.Toolbox.Plugins
{
    public class StoragePlugin : IPlugin
    {
        private const string DATA_DIRECTORY = "data";
        private readonly ILog _log;
        private string _directory;

        public StoragePlugin(ILog log)
        {
            _log = log;
        }

        public void Start()
        {
            _directory = Path.Combine(Environment.CurrentDirectory, DATA_DIRECTORY);
            if (!Directory.Exists(_directory))
            {
                Directory.CreateDirectory(_directory);
            }
        }

        public void Stop() { }

        public T[] ReadFile<T>(string fileName) where T : class, new()
        {
            string filePath = GetFilePath(fileName);
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Dispose();
            }

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    return GetFlatFileEngine<T>().Read<T>(stream)?.ToArray();
                }
            }
            catch (FormatException ex)
            {
                _log.Info($"Error while loading file {filePath}, deleting file to ensure it doesn't happen again.");
                _log.Info(ex.ToString());

                File.Delete(filePath);
                return new T[0];
            }
        }

        public void SaveFile<T>(string fileName, T[] objects) where T : class, new()
        {
            string filePath = GetFilePath(fileName);

            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                GetFlatFileEngine<T>().Write(stream, objects);
            }
        }

        public void DeleteFile(string fileName)
        {
            string filePath = GetFilePath(fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        private static IFlatFileEngine GetFlatFileEngine<T>() where T : class, new() => 
            new DelimitedFileEngineFactory().GetEngine<T>();

        private string GetFilePath(string fileName) => 
            Path.Combine(_directory, fileName + ".txt");
    }
}