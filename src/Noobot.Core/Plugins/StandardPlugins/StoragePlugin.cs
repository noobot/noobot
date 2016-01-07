using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FlatFile.Core;
using FlatFile.Delimited.Attributes;
using FlatFile.Delimited.Implementation;
using Noobot.Core.Logging;

namespace Noobot.Core.Plugins.StandardPlugins
{
    public class StoragePlugin : IPlugin
    {
        private readonly ILog _log;
        private string _directory;

        public StoragePlugin(ILog log)
        {
            _log = log;
        }

        public void Start()
        {
            _directory = Path.Combine(Environment.CurrentDirectory, "data");
            if (!Directory.Exists(_directory))
            {
                Directory.CreateDirectory(_directory);
            }
        }

        public void Stop()
        { }

        public T[] ReadFile<T>(string fileName) where T : class, new()
        {
            string filePath = GetFilePath(fileName);
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Dispose();
            }

            IFlatFileEngine engine = GetFlatFileEngine<T>();
            MethodInfo decorateMethod = engine.GetType().GetMethod("Read");
            MethodInfo generic = decorateMethod.MakeGenericMethod(typeof(T));

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    var results = generic.Invoke(engine, new object[] { stream }) as IEnumerable<T>;
                    return results.ToArray();
                }
            }
            catch (FormatException ex)
            {
                _log.Log($"Error while loading file {filePath}, deleting file to ensure it doesn't happen again.");
                _log.Log(ex.ToString());

                File.Delete(filePath);
                return new T[0];
            }
        }

        public void SaveFile<T>(string fileName, T[] objects) where T : class, new()
        {
            string filePath = GetFilePath(fileName);

            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                IFlatFileEngine engine = GetFlatFileEngine<T>();

                MethodInfo decorateMethod = engine.GetType().GetMethod("Write");
                MethodInfo generic = decorateMethod.MakeGenericMethod(typeof(T));
                generic.Invoke(engine, new object[] { stream, objects });
            }
        }

        private static IFlatFileEngine GetFlatFileEngine<T>() where T : class, new()
        {
            var factory = new DelimitedFileEngineFactory();

            MethodInfo method = typeof(FlatFileEngineFactoryExtensions).GetMethod("GetEngine");

            MethodInfo generic = method.MakeGenericMethod(typeof(T));
            return generic.Invoke(factory, new object[] { factory, null }) as IFlatFileEngine;
        }

        public void DeleteFile(string fileName)
        {
            string filePath = GetFilePath(fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        private string GetFilePath(string fileName)
        {
            return Path.Combine(_directory, fileName + ".txt");
        }
    }
}