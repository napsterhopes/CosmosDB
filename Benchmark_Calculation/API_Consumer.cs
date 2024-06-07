using APIConsumer;
using Polly;
using Polly.Timeout;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace HttpClientSample
{
    public class Benchmark
    {
        public string TenantId { get; set; }
        public string SiteId { get; set; }
    }

    class Program
    {
       // static readonly HttpClient client = new();

        //static async Task<HttpStatusCode> WriteTelemetry(Benchmark product)
        //{
            
        //}

       
        static void Main()
        {
            RunAsync().GetAwaiter().GetResult();
        }

        static async Task RunAsync()
        {
            #region Dev
            HttpClient client;
            var EndPoint = "https://192.168.0.1/api";
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) =>
            {
                return true;
            };
            client = new HttpClient(httpClientHandler) { BaseAddress = new Uri(EndPoint) };
            client.BaseAddress = new Uri("https://weiot-platform-benchmark-api.p710192975.uswest-dev-az-003.cluster.k8s.westus2.us.walmart.net/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            #endregion

            //#region localhost
            //client.BaseAddress = new Uri("https://localhost:7027/");
            //client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //#endregion

            try
            {
                for (short j = 6; j <= 15; j++)
                {
                    for (short i = 80; i <=100; i++)
                    {
                        Benchmark product = new()
                        {
                            SiteId = i.ToString(),
                            TenantId = TenantsList.tenantIDs[j]
                        };
                        //await WriteTelemetry(product);

                        var pollyContext = new Context("Retry 503");
                        var policy = Policy
                            .Handle<HttpRequestException>(ex => ex.Message.Contains("503"))
                            .WaitAndRetryAsync(
                                5,
                                _ => TimeSpan.FromMilliseconds(10000),
                                (result, timespan, retryNo, context) =>
                                {
                                    Console.WriteLine($"{context.OperationKey}: Retry number {retryNo} within " +
                                        $"{timespan.TotalMilliseconds}ms. Original status code: 503");
                                }
                            );

                        var response = await policy.ExecuteAsync(async ctx =>
                        {
                            HttpResponseMessage response = await client.PostAsJsonAsync(
                             "Benchmark/DevWriteBulkTelemetry", product);
                            response.EnsureSuccessStatusCode();

                            return response.StatusCode;
                        }, pollyContext);
                        //return HttpStatusCode.OK;

                        Console.WriteLine(i);
                        Thread.Sleep(30000);
                    }
                    Thread.Sleep(180000);
                }
            }
            
            catch (TimeoutRejectedException e)
            {
                Console.WriteLine(e.Message);
                RunAsync().GetAwaiter().GetResult();
            }

            Console.ReadLine();
        }
    }
}