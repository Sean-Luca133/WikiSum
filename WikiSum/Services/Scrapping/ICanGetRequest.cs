namespace WikiSum.Services.Scrapping
{
    public interface ICanGetRequest
    {
        Task<HttpResponseMessage> GetRequestAsync(Uri requestUri);
    }
}
