namespace WikiSum.Services.Scrapping
{
    public interface ICanGetRequest
    {
        Task<List<string>?> GetRequestAsync(Uri requestUri);
    }
}
