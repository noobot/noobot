using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace Noobot.Runner.Logging
{
    public class TextWriterDistributor : TextWriter
    {
        private readonly TextWriter[] _writers;

        public TextWriterDistributor(params TextWriter[] writers)
        {
            _writers = writers;
        }

        public override void WriteLine()
        {
            foreach (var textWriter in _writers)
            {
                textWriter.WriteLine();
            }
        }

        public override void WriteLine(string value)
        {
            foreach (var textWriter in _writers)
            {
                textWriter.WriteLine(value);
            }
        }

        public override void WriteLine(object value)
        {
            foreach (var textWriter in _writers)
            {
                textWriter.WriteLine(value);
            }
        }

        public override void Close()
        {
            foreach (var textWriter in _writers)
            {
                textWriter.Close();
            }
        }

        protected override void Dispose(bool disposing)
        {
            foreach (var textWriter in _writers)
            {
                textWriter.Dispose();
            }
        }

        public override void Flush()
        {
            foreach (var textWriter in _writers)
            {
                textWriter.Flush();
            }
        }

        public override void Write(char value)
        {
            foreach (var textWriter in _writers)
            {
                textWriter.Write(value);
            }
        }

        public override void Write(char[] buffer)
        {
            foreach (var textWriter in _writers)
            {
                textWriter.Write(buffer);
            }
        }

        public override void Write(char[] buffer, int index, int count)
        {
            foreach (var textWriter in _writers)
            {
                textWriter.Write(buffer, index, count);
            }
        }

        public override void Write(bool value)
        {
            foreach (var textWriter in _writers)
            {
                textWriter.Write(value);
            }
        }

        public override void Write(int value)
        {
            foreach (var textWriter in _writers)
            {
                textWriter.Write(value);
            }
        }

        public override void Write(uint value)
        {
            foreach (var textWriter in _writers)
            {
                textWriter.Write(value);
            }
        }

        public override void Write(long value)
        {
            foreach (var textWriter in _writers)
            {
                textWriter.Write(value);
            }
        }

        public override void Write(ulong value)
        {
            foreach (var textWriter in _writers)
            {
                textWriter.Write(value);
            }
        }

        public override void Write(float value)
        {
            foreach (var textWriter in _writers)
            {
                textWriter.Write(value);
            }
        }

        public override void Write(double value)
        {
            foreach (var textWriter in _writers)
            {
                textWriter.Write(value);
            }
        }

        public override void Write(decimal value)
        {
            foreach (var textWriter in _writers)
            {
                textWriter.Write(value);
            }
        }

        public override void Write(string value)
        {
            foreach (var textWriter in _writers)
            {
                textWriter.Write(value);
            }
        }

        public override void Write(object value)
        {
            foreach (var textWriter in _writers)
            {
                textWriter.Write(value);
            }
        }

        public override void Write(string format, object arg0)
        {
            foreach (var textWriter in _writers)
            {
                textWriter.Write(format, arg0);
            }
        }

        public override void Write(string format, object arg0, object arg1)
        {
            foreach (var textWriter in _writers)
            {
                textWriter.Write(format, arg0, arg1);
            }
        }

        public override void Write(string format, object arg0, object arg1, object arg2)
        {
            foreach (var textWriter in _writers)
            {
                textWriter.Write(format, arg0, arg1, arg2);
            }
        }

        public override void Write(string format, params object[] arg)
        {
            foreach (var textWriter in _writers)
            {
                textWriter.Write(format, arg);
            }
        }

        public override void WriteLine(char value)
        {
            foreach (var textWriter in _writers)
            {
                textWriter.WriteLine(value);
            }
        }

        public override void WriteLine(char[] buffer)
        {
            foreach (var textWriter in _writers)
            {
                textWriter.WriteLine(buffer);
            }
        }

        public override void WriteLine(char[] buffer, int index, int count)
        {
            foreach (var textWriter in _writers)
            {
                textWriter.WriteLine(buffer, index, count);
            }
        }

        public override void WriteLine(bool value)
        {
            foreach (var textWriter in _writers)
            {
                textWriter.WriteLine(value);
            }
        }

        public override void WriteLine(int value)
        {
            foreach (var textWriter in _writers)
            {
                textWriter.WriteLine(value);
            }
        }

        public override void WriteLine(uint value)
        {
            foreach (var textWriter in _writers)
            {
                textWriter.WriteLine(value);
            }
        }

        public override void WriteLine(long value)
        {
            foreach (var textWriter in _writers)
            {
                textWriter.WriteLine(value);
            }
        }

        public override void WriteLine(ulong value)
        {
            foreach (var textWriter in _writers)
            {
                textWriter.WriteLine(value);
            }
        }

        public override void WriteLine(float value)
        {
            foreach (var textWriter in _writers)
            {
                textWriter.WriteLine(value);
            }
        }

        public override void WriteLine(double value)
        {
            foreach (var textWriter in _writers)
            {
                textWriter.WriteLine(value);
            }
        }

        public override void WriteLine(decimal value)
        {
            foreach (var textWriter in _writers)
            {
                textWriter.WriteLine(value);
            }
        }

        public override void WriteLine(string format, object arg0)
        {
            foreach (var textWriter in _writers)
            {
                textWriter.WriteLine(format, arg0);
            }
        }

        public override void WriteLine(string format, object arg0, object arg1)
        {
            foreach (var textWriter in _writers)
            {
                textWriter.WriteLine(format, arg0, arg1);
            }
        }

        public override void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            foreach (var textWriter in _writers)
            {
                textWriter.WriteLine(format, arg0, arg1, arg2);
            }
        }

        public override void WriteLine(string format, params object[] arg)
        {
            foreach (var textWriter in _writers)
            {
                textWriter.WriteLine(format, arg);
            }
        }

        public override Task WriteAsync(char value)
        {
            List<Task> tasks = _writers.Select(textWriter => textWriter.WriteAsync(value)).ToList();

            return Task.WhenAll(tasks);
        }

        public override Task WriteAsync(string value)
        {
            List<Task> tasks = _writers.Select(textWriter => textWriter.WriteAsync(value)).ToList();

            return Task.WhenAll(tasks);
        }

        public override Task WriteAsync(char[] buffer, int index, int count)
        {
            List<Task> tasks = _writers.Select(textWriter => textWriter.WriteAsync(buffer, index, count)).ToList();

            return Task.WhenAll(tasks);
        }

        public override Task WriteLineAsync(char value)
        {
            List<Task> tasks = _writers.Select(textWriter => textWriter.WriteLineAsync(value)).ToList();

            return Task.WhenAll(tasks);
        }

        public override Task WriteLineAsync(string value)
        {
            List<Task> tasks = _writers.Select(textWriter => textWriter.WriteLineAsync(value)).ToList();

            return Task.WhenAll(tasks);
        }

        public override Task WriteLineAsync(char[] buffer, int index, int count)
        {
            List<Task> tasks = _writers.Select(textWriter => textWriter.WriteLineAsync(buffer, index, count)).ToList();

            return Task.WhenAll(tasks);
        }

        public override Task WriteLineAsync()
        {
            List<Task> tasks = _writers.Select(textWriter => textWriter.WriteLineAsync()).ToList();

            return Task.WhenAll(tasks);
        }

        public override Task FlushAsync()
        {
            List<Task> tasks = _writers.Select(textWriter => textWriter.FlushAsync()).ToList();

            return Task.WhenAll(tasks);
        }

        public override Encoding Encoding { get; }
    }
}