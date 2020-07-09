using System.Net.Http;

namespace DNVGL.OAuth.UserCredentials
{
    public interface IOAuthHttpClientFactory
    {
        HttpClient Create(string name);
    }
}
