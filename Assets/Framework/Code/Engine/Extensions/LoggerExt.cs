using System.Collections.Generic;
using System.Linq;

namespace Jape
{
    public static class LoggerExt
    {
        private static List<Logger> loggers = new();

        public static Logger Log(this object instance)
        {
            if (loggers.Any(l => l.HasInstance(instance))) { return loggers.First(l => l.HasInstance(instance)); }

            Logger logger = new(instance);
            loggers.Add(logger);
            return logger;
        }
    }
}