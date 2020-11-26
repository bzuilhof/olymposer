using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Refit;

namespace Olymposer
{
    public class ReserveSlot
    {
        private readonly IOlymposApi _olymposApi;
        public ReserveSlot()
        {
            var olymposUrl = Environment.GetEnvironmentVariable("OLYMPOS_URL");
            _olymposApi = RestService.For<IOlymposApi>(olymposUrl);   
        }
        [FunctionName("ReserveSlot")]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "reserve-slot")]
            HttpRequest req, ILogger log)
        {
            
            log.LogInformation("C# HTTP trigger function processed a request.");

            var timeslot = "23-11-20 10:00:00"; //GetTimeslot();
            var cookie = await GetCookie();

            var reserveSlotQueryParams = new ReserveSlotQueryParams{ NavisionSubCode = timeslot};

            var response = await _olymposApi.ReserveSlot(reserveSlotQueryParams, cookie);

            var jsonResponse = response.Content;
            
            return new OkResult();
        }

        private async Task<string> GetCookie()
        {
            var username = Environment.GetEnvironmentVariable("USERNAME");
            var password = Environment.GetEnvironmentVariable("PASSWORD");
            
            var queryParams = new GetSessionCookieQueryParams { Username = username};
            var body = new GetSessionCookieQueryBody { Password = password };
            var response = await _olymposApi.GetSessionCookie(queryParams, body);
            var headers = response.Headers.ToList();

            var setCookieHeader = headers.First(pair => pair.Key == "Set-Cookie").Value.ToList();
            var sessionIdHeader = setCookieHeader.First(x => x.Contains("SessionId")).Split(' ')[0];
            var sessionAuthHeader = setCookieHeader.First(x => x.Contains("ASPXANONYMOUS")).Split(' ')[0];

            var cookie = $"{sessionAuthHeader} {sessionIdHeader}";

            return cookie;
        }

        private string GetTimeslot()
        {
            var twoDaysAhead = DateTime.Today.AddDays(2);
            
            var dateString = twoDaysAhead.ToString("dd-MM-yy");

            var time = twoDaysAhead.DayOfWeek == DayOfWeek.Saturday || twoDaysAhead.DayOfWeek == DayOfWeek.Sunday
                ? "9:00:00"
                : "20:30:00";
            
            var timeslot = $"{dateString} {time}";
            
            return timeslot;
        }
    }
}