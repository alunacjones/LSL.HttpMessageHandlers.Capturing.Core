using System.Net.Http;

namespace LSL.HttpMessageHandlers.Capturing.Core.Tests;

internal class MyOtherTestClient(HttpClient httpClient) : MyTestClient(httpClient);