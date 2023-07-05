using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgRmi.Logging
{
    public interface ILogger
    {

        /// <summary>
        /// Log a log message to the console
        /// </summary>
        /// <param name="message"></param>
        public void LogMessage(object target, string message);

        /// <summary>
        /// Log a warning message to the console
        /// </summary>
        /// <param name="message"></param>
        public void LogWarning(object target, string message);

        /// <summary>
        /// Log an error to the console
        /// </summary>
        /// <param name="message"></param>
        public void LogError(object target, string message);

    }
}
