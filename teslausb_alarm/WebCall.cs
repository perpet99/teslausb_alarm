using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace teslausb_alarm
{

    class FileWacher
    {
        private PhysicalFileProvider _fileProvider;
        private IChangeToken _fileChangeToken;
        public Action _action;

        public FileWacher(string path)
        {
            _fileProvider = new PhysicalFileProvider(path);
            WatchForFileChanges();

        }

        private void WatchForFileChanges()
        {
            _fileChangeToken = _fileProvider.Watch("*.*");
            _fileChangeToken.RegisterChangeCallback(Notify, default);
        }

        private void Notify(object state)
        {
            Console.WriteLine("File change detected");
            _action();

            WatchForFileChanges();
        }

    }


    class WebCall
    {

        public static void Call(string url, string message)
        {
            CallWebAPIAsync(url, message).Wait();
        }

        public class Data
        {
            public string content;
        }

        static async Task CallWebAPIAsync(string url, string message)
        {
            try
            {
                var data = new Data() { content = message };

                var json = JsonConvert.SerializeObject(data);
                var c = new StringContent(json, Encoding.UTF8, "application/json");

                //var url = "https://discordapp.com/api/webhooks/885042624581484594/4GyiWg6HyFHWPyucWLLcF-MGmaKxT--sQaTWRrZ7csXK3tnelAc7IzcfSiaWcfkJzQvP";
                using var client = new HttpClient();

                var response = await client.PostAsync(url, c);

                string result = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine(result);
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }

        }
    }
}
