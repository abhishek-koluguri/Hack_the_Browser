using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Hack_the_Browser;
using Hack_the_Browser.Models;
using Microsoft.Owin.Testing;
using Newtonsoft.Json;
using NUnit.Framework;

namespace HackTheBrowserIntegrationTests
{
    [TestFixture]
    public class ImagesControllerTests : IntegrationTestFramework
    {
        private TestServer _server;
        private Process _mongod;
        private static Stopwatch Timer = new Stopwatch();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _mongod = StartMongoDbServer();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _mongod.Kill();
        }

        [SetUp]
        public void FixtureInit()
        {
            ClearDatabase();
            _server = TestServer.Create<ApiStartup>();
        }

        [TearDown]
        public void FixtureDispose()
        {
            _server.Dispose();
        }

        [Test]
        public void GetImage()
        {
            var referenceIdTif = UploadTif();
            var referenceIdPdf = UploadPdf();

            Timer.Restart();
            var oldResponse = SendGetRequest($"api/images/{referenceIdTif}");
            Timer.Stop();
            Console.WriteLine("Time taken per request for a TIF file in the old way: " + Timer.ElapsedMilliseconds);
            Assert.IsTrue(oldResponse.IsSuccessStatusCode);
            var stream = oldResponse.Content.ReadAsStreamAsync().Result;
            Assert.IsNotNull(stream);

            Timer.Restart();
            var hackTheBrowserResponse = SendGetRequest($"api/images/{referenceIdTif}", oldResponse.Headers.ETag.Tag);
            Timer.Stop();
            Console.WriteLine("Time taken per request for a TIF file in the new way: " + Timer.ElapsedMilliseconds);

            Timer.Restart();
            var oldResponse2 = SendGetRequest($"api/images/{referenceIdPdf}");
            Timer.Stop();
            Console.WriteLine("Time taken per request for a PDF file in the old way: " + Timer.ElapsedMilliseconds);;

            Timer.Restart();
            var hackTheBrowserResponse2 = SendGetRequest($"api/images/{referenceIdPdf}", oldResponse2.Headers.ETag.Tag);
            Timer.Stop();
            Console.WriteLine("Time taken per request for a PDF file in the new way: " + Timer.ElapsedMilliseconds);
        }

        private string UploadTif(bool includeAnnotations = false)
        {
            var imageUploadData = new ImageModel
            {
                ExternalId = "tif_1234",
                Extension = ".tif",
                ExternalData = "{\"PageVersion\":\"0\",\"RotationAngle\":0}",
                RotationAngle = 0
            };

            var xmlAnnotation = includeAnnotations ? File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData\\MultiAnnotationVF.txt")) : null;

            var binaryDataStream = File.OpenRead("C:\\Users\\koluguab\\Downloads\\TestTif.tif");
            var uploadRequestPayload = CreateUploadRequestPayload(imageUploadData, binaryDataStream, "image/tiff", "testTif.tif", xmlAnnotation);

            var response = SendPostRequest("api/images", uploadRequestPayload);
            var result = JsonConvert.DeserializeObject<Image>(response.Content.ReadAsStringAsync().Result);

            return result.ReferenceId.ToString();
        }

        private string UploadPdf(bool includeAnnotations = false)
        {
            var imageUploadData = new ImageModel
            {
                ExternalId = "pdf_1234",
                Extension = ".pdf",
                ExternalData = "{\"PageVersion\":\"0\",\"RotationAngle\":0}",
                RotationAngle = 0
            };

            var xmlAnnotation = includeAnnotations ? File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData\\MultiAnnotationVF.txt")) : null;

            var binaryDataStream = File.OpenRead("C:\\Users\\koluguab\\Downloads\\TestPdf.pdf");
            var uploadRequestPayload = CreateUploadRequestPayload(imageUploadData, binaryDataStream, "image/pdf", "testPdf.pdf", xmlAnnotation);

            var response = SendPostRequest("api/images", uploadRequestPayload);
            var result = JsonConvert.DeserializeObject<Image>(response.Content.ReadAsStringAsync().Result);

            return result.ReferenceId.ToString();
        }

        private static MultipartContent CreateUploadRequestPayload(ImageModel imageUploadData, Stream imageData, string mimeType, string filename, string annotationXml)
        {
            var multipartContent = new MultipartFormDataContent();

            // Create metadata section
            var jsonData = JsonConvert.SerializeObject(imageUploadData);
            multipartContent.Add(new StringContent(jsonData), "Data");

            // Add the image section
            var filecontent = new ByteArrayContent(ConvertStreamToByteArray(imageData));
            filecontent.Headers.ContentType = new MediaTypeHeaderValue(mimeType.Split(',')[0]);
            filecontent.Headers.ContentDisposition =
                new ContentDispositionHeaderValue("attachment")
                {
                    FileName = filename,
                    Name = "Image",
                };
            multipartContent.Add(filecontent);

            // Add the annotation section
            if (!string.IsNullOrEmpty(annotationXml))
            {
                var annFile = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes(annotationXml)));
                annFile.Headers.ContentType = new MediaTypeHeaderValue("text/xml");
                annFile.Headers.ContentDisposition =
                    new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = filename + ".ann",
                        Name = "Annotation"
                    };
                multipartContent.Add(annFile);
            }

            return multipartContent;
        }

        private HttpResponseMessage SendGetRequest(string route, string eTag = "")
        {
            var request = new HttpRequestMessage
            {
                Method = new HttpMethod(HttpMethod.Get.Method),
                RequestUri = new Uri(string.Format("{0}/{1}", _server.HttpClient.BaseAddress.ToString().TrimEnd('/'), route))
            };
            request.Headers.Add("ETag", eTag);
            

            var response = SendHttpRequest(request).Result;
            return response;
        }

        private HttpResponseMessage SendPostRequest(string route, HttpContent content)
        {
            var request = new HttpRequestMessage
            {
                Method = new HttpMethod(HttpMethod.Post.Method),
                RequestUri = new Uri(string.Format("{0}/{1}", _server.HttpClient.BaseAddress.ToString().TrimEnd('/'), route)),
                Content = content
            };

            var response = SendHttpRequest(request).Result;
            return response;
        }

        private async Task<HttpResponseMessage> SendHttpRequest(HttpRequestMessage request)
        {
            return await _server.HttpClient.SendAsync(request).ConfigureAwait(false);
        }
    }
}
