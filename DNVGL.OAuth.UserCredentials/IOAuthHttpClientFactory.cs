namespace DNVGL.OAuth.Api.HttpClient
{
    public interface IOAuthHttpClientFactory
    {
        System.Net.Http.HttpClient Create(string name);
    }
}
