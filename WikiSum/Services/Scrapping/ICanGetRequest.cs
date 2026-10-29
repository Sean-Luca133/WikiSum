namespace WikiSum.Services.Scrapping
{
    public interface ICanGetRequest
    {
        Task<List<(string, string)>?> GetRequestAsync(Uri requestUri);
    }
}
