namespace WikiSum.Services.Scrapping
{
    public interface ICanGetRequest
    {
        Task<string?> GetRequestAsync(Uri requestUri);
    }
}
