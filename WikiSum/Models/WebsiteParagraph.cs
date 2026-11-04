using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace WikiSum.Models
{
    public class WebsiteParagraph: ObservableObject
    {
        private string header  = string.Empty;
        private string paragraphSummary  = string.Empty;

        public string Header
        {
            get => header;
            set => SetProperty(ref header, value);
        }

        public string ParagraphSummary
        {
            get => paragraphSummary;
            set => SetProperty(ref paragraphSummary, value);
        }

        public WebsiteParagraph()
        {
        }

        public WebsiteParagraph(string header, string paragraphSummary)
        {
            Header = header;
            ParagraphSummary = paragraphSummary;
        }
    }
}
