using System.Net.Http;
using System.Threading.Tasks;

namespace LSL.HttpMessageHandlers.Capturing.Core.Tests;

internal class MyTestClient(HttpClient httpClient)
{
    public async Task SendRequest() => await httpClient.GetAsync("http://nowhere.com");
}