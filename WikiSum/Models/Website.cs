using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace WikiSum.Models
{
    class Website: ObservableObject
    {
        private string url;
        private ObservableCollection<WebsiteParagraph> websiteParagraphs = new ObservableCollection<WebsiteParagraph>();

        public string Url
        {
            get =>url;
            set => SetProperty(ref url, value);
        }

        public ObservableCollection<WebsiteParagraph> WebsiteParagraphs
        {
            get => websiteParagraphs;
            set => SetProperty(ref websiteParagraphs, value);
        }

        public Website()
        {
        }
    }
}
