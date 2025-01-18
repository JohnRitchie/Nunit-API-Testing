using NUnit.Framework;
using Allure.Net.Commons;
using Allure.NUnit.Attributes;
using Allure.NUnit;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace NunitAPITesting
{
    [AllureNUnit]
    [TestFixture]
    [AllureSuite("Simple tests to verify the RESTful API of the JSONPlaceholder site")]
    public class ApiTests
    {
        private HttpClient _client;
        private const string BaseUrl = "https://jsonplaceholder.typicode.com";
        private const string PostEndpointUrl = $"{BaseUrl}/posts";
        private const string GetEndpointUrl = $"{BaseUrl}/posts/1";
        private const string PutEndpointUrl = $"{BaseUrl}/posts/1";
        private const string DeleteEndpointUrl = $"{BaseUrl}/posts/1";

        [SetUp]
        public void SetUp()
        {
            _client = new HttpClient();
        }

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
        }
        
        private static string CreateTempFile(string content)
        {
            var tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, content);
            return tempFile;
        }

        private async Task<HttpResponseMessage> SendRequestWithLogging(HttpMethod method, string url, HttpContent? payload = null)
        {
            var requestDetails = $"Request Method: {method.Method}\nRequest URL: {url}\nPayload: {payload?.ReadAsStringAsync().GetAwaiter().GetResult()}";
            AllureApi.AddAttachment("Request", "text/plain", CreateTempFile(requestDetails));

            return method.Method switch
            {
                "GET" => await _client.GetAsync(url),
                "POST" => await _client.PostAsync(url, payload),
                "PUT" => await _client.PutAsync(url, payload),
                "DELETE" => await _client.DeleteAsync(url),
                _ => throw new ArgumentException("Unsupported HTTP method.")
            };
        }
        
        private static void VerifyResponseStatusCode(HttpResponseMessage response, HttpStatusCode expectedStatusCode)
        {
            Assert.That(response, Is.Not.Null, "Response is null. Request failed.");
            var responseStatusCode = (int)response.StatusCode;
            AllureApi.AddAttachment("Response status code", "text/plain", CreateTempFile($"Status code: {responseStatusCode}"));
            Assert.That(response.StatusCode, Is.EqualTo(expectedStatusCode), $"Expected status code {expectedStatusCode}, but got {responseStatusCode}.");
        }

        private static void VerifyResponseBody(string responseBody, params string[] expectedFields)
        {
            foreach (var field in expectedFields)
            {
                Assert.That(responseBody, Is.Not.Empty, "Response body is unexpectedly empty.");
                Assert.That(responseBody, Does.Contain(field).IgnoreCase, $"Response body should contain '{field}'.");
            }
        }

        [Test]
        [AllureDescription("Test verifies that the GET /posts/1 endpoint returns 200 status code and the response body contains the expected post data")]
        [AllureTag("API", "SmokeTest")]
        [AllureSeverity(SeverityLevel.critical)]
        public async Task GetPosts_ShouldReturnExpectedData()
        {
            var response = await AllureApi.Step("Send GET request", async () => await SendRequestWithLogging(HttpMethod.Get, GetEndpointUrl));

            AllureApi.Step("Log and verify response status code", () => VerifyResponseStatusCode(response, HttpStatusCode.OK));

            var responseBody = await AllureApi.Step("Log response body", async () =>
            {
                var body = await response.Content.ReadAsStringAsync();
                AllureApi.AddAttachment("Response body", "application/json", CreateTempFile(body));
                return body;
            });

            AllureApi.Step("Verify response body contains expected fields", () =>
                VerifyResponseBody(responseBody, "\"userId\"", "\"id\"", "\"title\"", "\"body\""));
        }

        [Test]
        [AllureDescription("Test verifies that the POST /posts endpoint returns 201 status code and the response body contains the details of the newly created post")]
        [AllureTag("API", "SmokeTest")]
        [AllureSeverity(SeverityLevel.critical)]
        public async Task PostPosts_ShouldCreateResource()
        {
            var payload = new StringContent(
                JsonConvert.SerializeObject(new { title = "foo", body = "bar", userId = 1 }),
                Encoding.UTF8,
                "application/json");

            var response = await AllureApi.Step("Send POST request", async () => await SendRequestWithLogging(HttpMethod.Post, PostEndpointUrl, payload));

            AllureApi.Step("Log and verify response status code", () => VerifyResponseStatusCode(response, HttpStatusCode.Created));

            var responseBody = await AllureApi.Step("Log response body", async () =>
            {
                var body = await response.Content.ReadAsStringAsync();
                AllureApi.AddAttachment("Response body", "application/json", CreateTempFile(body));
                return body;
            });

            AllureApi.Step("Verify response body contains 'id' of the newly created post", () =>
                Assert.That(responseBody, Does.Contain("\"id\":").IgnoreCase, "Response body should contain 'id' indicating a created resource."));
        }

        [Test]
        [AllureDescription("Test verifies that the PUT /posts/1 endpoint returns 200 status code and the response body contains the updated post details")]
        [AllureTag("API", "SmokeTest")]
        [AllureSeverity(SeverityLevel.critical)]
        public async Task PutPosts_ShouldUpdateResource()
        {
            var payload = new StringContent(
                JsonConvert.SerializeObject(new { id = 1, title = "new_foo", body = "new_bar", userId = 1 }),
                Encoding.UTF8,
                "application/json");

            var response = await AllureApi.Step("Send PUT request", async () => await SendRequestWithLogging(HttpMethod.Put, PutEndpointUrl, payload));

            AllureApi.Step("Log and verify response status code", () => VerifyResponseStatusCode(response, HttpStatusCode.OK));

            var responseBody = await AllureApi.Step("Log response body", async () =>
            {
                var body = await response.Content.ReadAsStringAsync();
                AllureApi.AddAttachment("Response body", "application/json", CreateTempFile(body));
                return body;
            });

            AllureApi.Step("Verify response body contains updated fields", () =>
                VerifyResponseBody(responseBody, "\"id\"", "\"title\": \"new_foo\"", "\"body\": \"new_bar\""));
        }
        
        [Test]
        [AllureDescription("Test verifies that the DELETE /posts/1 endpoint returns 200 or 204 status code and the response body is empty or contains '{}'"),]
        [AllureTag("API", "SmokeTest")]
        [AllureSeverity(SeverityLevel.critical)]
        public async Task DeletePosts_ShouldRemoveResource()
        {
            var response = await AllureApi.Step("Send DELETE request", async () => await SendRequestWithLogging(HttpMethod.Delete, DeleteEndpointUrl));

            AllureApi.Step("Log and verify response status code", () =>
            {
                Assert.That(response, Is.Not.Null, "Response is null. Request failed.");
                var responseStatusCode = (int)response.StatusCode;
                AllureApi.AddAttachment("Response status code", "text/plain", CreateTempFile($"Status code: {responseStatusCode}"));
                Assert.That(response.StatusCode, Is.AnyOf(HttpStatusCode.OK, HttpStatusCode.NoContent), "Response status code is not 200 or 204.");
            });

            var responseBody = await AllureApi.Step("Log response body", async () =>
            {
                var body = await response.Content.ReadAsStringAsync();
                AllureApi.AddAttachment("Response body", "application/json", CreateTempFile(body));
                return body;
            });

            AllureApi.Step("Verify response body is empty or contains '{}'", () =>
                Assert.That(responseBody, Is.EqualTo("{}").Or.Empty, "Response body should be empty or contain '{}'"));
        }
    }
}
