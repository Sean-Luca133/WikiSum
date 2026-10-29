using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace WikiSum.Services.Scrapping
{
    public class ScrapeService : IScrapeService
    {
        private Queue<AngleSharp.Dom.IElement> headings = new();
        private List<string> headingsList = new();
        private List<string> tags = new();

        private readonly HttpClient _httpClient;
        public ScrapeService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
                   "MyScraper/1.0 (contact: jouw-email@example.com)"
            );
        }

        public async Task<List<(string, string)>?> GetRequestAsync(Uri requestUri)
        {
            Debug.WriteLine($"[ScrapService]: Getting URI {requestUri}.");

            try
            {
                using HttpResponseMessage response = await _httpClient.GetAsync(requestUri);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                string html = responseBody;
                var context = BrowsingContext.New(Configuration.Default);
                var document = await context.OpenAsync(req => req.Content(html));
                GetHeadings(document);
                SetTags(document);
                return PrepareResult();
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine("\nException Caught!");
                Debug.WriteLine("Message :{0} ", e.Message);
                return null;
            }
        }

        private void GetHeadings(IDocument document)
        {
            var links = document.QuerySelectorAll("nav[aria-label='Contents'] a");
            foreach (var link in links)
            {
                string content = link.TextContent.Trim();
                if (content.Split("\t")[0].Contains("."))
                {
                    continue;
                }
                string? url = link.GetAttribute("href");
                Debug.WriteLine($"Tekst: {content} | URL: {url}");
                headings.Enqueue(link);
            }
        }
        private void SetTags(IDocument document)
        {
            string? startId = GetId();
            headingsList.Add(startId);
            while (startId != null)
            {
                string? endId = GetId();
                Debug.WriteLine($"StartId: {startId} & EndId: {endId}");
                if (endId == null)
                {
                    break;
                }
                string content = GetContentBetweenHeadings(document, startId, endId);
                if (startId != "start")
                {
                    tags.Add(content);
                }

                startId = endId;
                headingsList.Add(startId);

            }
        }

        private string? GetId()
        {
            if (headings.Count == 0) return null;
            string? href = headings.Dequeue().GetAttribute("href");

            if (string.IsNullOrEmpty(href)) return null;

            int hashIndex = href.IndexOf('#');
            if (hashIndex < 0) return href;

            // Pak alles na de '#'
            string id = href.Substring(hashIndex + 1);

            // Als de href alleen '#' is, is de id leeg. We geven dan een specifieke aanduiding terug.
            return string.IsNullOrEmpty(id) ? "start" : id;

        }
        private string GetContentBetweenHeadings(IDocument document, string startId, string endId)
        {
            var startElement = document.GetElementById(startId);

            if (startId == "start")
            {
                startElement = document.QuerySelector("[data-mw-section-id='0']");
                tags.Add(startElement.OuterHtml);

            }
            var endElement = document.GetElementById(endId);

            if (startElement == null) return string.Empty;

            List<string> content = new();

            // Pak het omringende element van de kop (vaak de <div class="mw-heading"> op Wikipedia)
            // Zo niet, dan vallen we terug op het startElement zelf.
            var current = startElement.ParentElement?.ClassName?.Contains("mw-heading") == true
                          ? startElement.ParentElement.NextElementSibling
                          : startElement.NextElementSibling;

            // Bepaal ook het eindpunt (als er een endId is)
            var endTarget = endElement?.ParentElement?.ClassName?.Contains("mw-heading") == true
                                           ? endElement.ParentElement
                                           : endElement;

            while (current != null && current != endTarget)
            {
                content.Add(current.OuterHtml);
                current = current.NextElementSibling;
            }

            return string.Join(" ", content);
        }

        private List<(string, string)> PrepareResult()
        {
            var parser = new HtmlParser();
            List<string> result = new();
            foreach (var tag in tags)
            {
                IDocument tagDocument = parser.ParseDocument(tag);
                result.Add(ExtractUselessTags(tagDocument));
            }
            return headingsList.Zip(result, (title, summary) => (title, summary)).ToList();
        }
        private string ExtractUselessTags(IDocument document)
        {
            // Verwijder elementen die geen nuttige tekst bevatten
            document.QuerySelectorAll("script, style, noscript, template,iframe,svg,canvas,header,footer,nav, aside")
                .ToList()
                .ForEach(element => element.Remove());

            // Haal alle tekst uit het document
            var text = document.Body?.TextContent ?? string.Empty;

            text = Regex.Replace(text, @"\s+", " ").Trim();

            Debug.WriteLine($"[ScrapeService]: Cleaned tekst: {text}");
            return text;
        }
    }
}
