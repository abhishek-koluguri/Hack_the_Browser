using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using MongoDB.Driver;
using NUnit.Framework;

namespace HackTheBrowserIntegrationTests
{
    public class IntegrationTestFramework
    {
        public static void ClearDatabase()
        {
            var databaseConnectionString = "mongodb://localhost:27017/ImageService";
            var client = new MongoClient(databaseConnectionString);
            client.DropDatabase("ImageService");
        }

       
        public static byte[] ConvertStreamToByteArray(Stream stream)
        {
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }

        public static Process StartMongoDbServer()
        {
            Task.Delay(TimeSpan.FromSeconds(10)).Wait();
            var mongoDataPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "mongodb\\data\\db");
            if (!Directory.Exists(mongoDataPath))
            {
                Directory.CreateDirectory(mongoDataPath);
            }

            var start = new ProcessStartInfo
            {
                FileName = @"C:\Users\koluguab\OneDrive - Vertafore, Inc\Abhi_Vertafore\Hackathon\Hack_the_Browser\HackTheBrowserIntegrationTests\Resources\MongoDB\mongod.exe",
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = $"--dbpath {mongoDataPath}",
                UseShellExecute = false
            };

            return Process.Start(start);
        }
    }
}
