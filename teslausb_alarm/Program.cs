using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace teslausb_alarm
{
    class Program
    {
        static void Main(string[] args)
        {

            string watcherPath;
            FileSystemWatcher watcher;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) == true)
            {
                // windows test code
                watcherPath = @"media/SentryClips";
            }
            else
            {
                watcherPath = @"/mutable/TeslaCam/SentryClips";
            }

            watcher = new FileSystemWatcher(watcherPath);
            watcher.Created += Watcher_Created;
        }

        static List<string> WatcherFileList = new List<string>();

        private static void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            //ArchiveCallCount = 0;

            if (WatcherFileList.Contains(e.FullPath))
                return;
            WatcherFileList.Add(e.FullPath);

            string message = $"event sentry file : {e.FullPath} ";

            GetSnsNCall(message);

        }


        public static void GetSnsNCall(string message)
        {
            Console.WriteLine(message);

            //var url = "https://discordapp.com/api/webhooks/885042624581484594/4GyiWg6HyFHWPyucWLLcF-MGmaKxT--sQaTWRrZ7csXK3tnelAc7IzcfSiaWcfkJzQvP";
            // set your url
            var url = GetSns();
            
            if (url != null && 0 < url.Length)
                WebCall.Call(url, message);

        }
        public static string GetConfigPath(string filename)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) == true)
            {
                return filename;
            }
            return "/mutable/" + filename;
        }


        public static string GetSns()
        {
            try
            {
                return System.IO.File.ReadAllText(GetConfigPath("sns.bin"));
            }
            catch (Exception)
            {

            }
            return string.Empty;
        }
    }
}
