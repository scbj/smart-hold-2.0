using Smart_Hold.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smart_Hold.Logger
{
    public static class Logger
    {
        static readonly object _lockObject = new object();
        private static List<string> _logs = null;

        private static List<string> Logs
        {
            get
            {
                lock (_lockObject)
                {
                    if (_logs == null)
                        _logs = new List<string>();
                    return _logs;
                }
            }
        }

        public static void LogError(Exception ex, FileSystemInfo fileSystem)
        {
            lock(_lockObject)
            {
                try
                {
                    Logs.Add(ex.GetType().Name + " : " + fileSystem.FullName);
                }
                catch
                {
                    Logs.Add(ex.GetType().Name + " => " + ex.TargetSite?.Name);
                }
            }
        }

        public static void Save(string name)
        {
            lock (_lockObject)
            {
                File.WriteAllLines(name, Logs);
                _logs = null;
            }
        }
    }
}
