using Microsoft.Extensions.Logging;

namespace TicTacToe.Server
{
    /// <summary>
    /// Helper class to allow Fenrir to log via Microsoft.Extensions.Logging
    /// </summary>
    class FenrirLogger : Fenrir.Multiplayer.ILogger
    {
        private readonly ILogger _logger;

        public FenrirLogger(ILogger<FenrirLogger> logger)
        {
            _logger = logger;
        }

        public void Critical(string format, params object[] arguments) => _logger.LogCritical(format, arguments);
        public void Debug(string format, params object[] arguments) => _logger.LogDebug(format, arguments);
        public void Error(string format, params object[] arguments) => _logger.LogError(format, arguments);
        public void Info(string format, params object[] arguments) => _logger.LogInformation(format, arguments);
        public void Trace(string format, params object[] arguments) => _logger.LogTrace(format, arguments);
        public void Warning(string format, params object[] arguments) => _logger.LogWarning(format, arguments);
    }
}
