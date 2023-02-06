using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace teslausb_alarm
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 0)
            {
                if (SetSns(args[0]))
                    Console.WriteLine($"sns addr save success : {args[0]}");
                else
                    Console.WriteLine($"sns addr save fail : {args[0]}");
                return;
            }
            string watcherPath;
            FileSystemWatcher watcher;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) == true)
            {
                // windows test code
                watcherPath = $"{System.IO.Directory.GetCurrentDirectory()}/SentryClips";

            }
            else
            {
                watcherPath = @"/mutable/TeslaCam/SentryClips";
            }
            
            Console.WriteLine($"filesystem watcher : {watcherPath}");
            if(System.IO.Directory.Exists(watcherPath) == false)
                System.IO.Directory.CreateDirectory(watcherPath);

            watcher = new FileSystemWatcher(watcherPath);
            watcher.Created += Watcher_Created;
            watcher.Filter = "*.*";
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;

            Console.WriteLine("Press ctrl-c to exit.");

            // Ctrl-c will gracefully exit the call at any point.
            ManualResetEvent exitMre = new ManualResetEvent(false);
            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e)
            {
                e.Cancel = true;
                exitMre.Set();
            };

            // Wait for a signal saying the call failed, was cancelled with ctrl-c or completed.
            exitMre.WaitOne();


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

        public static bool SetSns(string addr)
        {
            try
            {
                System.IO.File.WriteAllText(GetConfigPath("sns.bin"),addr);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }
    }
}
