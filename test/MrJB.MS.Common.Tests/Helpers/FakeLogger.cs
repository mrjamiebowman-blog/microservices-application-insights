using Microsoft.Extensions.Logging;
using System;
using System.Text;

namespace MrJB.MS.Common.Tests.Helpers
{
    public class FakeLogger<T> : ILogger<T>
    {
        public StringBuilder messages = new StringBuilder();

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            throw new NotImplementedException();
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            // append message
            var message = formatter.Invoke(state, exception);
            messages.AppendLine(message);
        }
    }
}
