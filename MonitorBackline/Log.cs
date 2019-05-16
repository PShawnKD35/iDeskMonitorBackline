using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorBackline
{
    static class Log
    {
        const string DATETIME_FORMAT = "yyyy-MM-dd HH:mm:ss";
        /// <summary>
        /// Write Log with a Timestamp
        /// </summary>
        /// <param name="log"></param>
        public static void WriteLine(string log)
        {
            Console.WriteLine(string.Format("{0} - {1}", DateTime.Now.ToString(DATETIME_FORMAT), log));
        }

    }
}
