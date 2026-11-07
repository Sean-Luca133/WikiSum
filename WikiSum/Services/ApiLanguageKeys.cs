using System;
using System.Collections.Generic;
using System.Text;

namespace WikiSum.Services
{
    public static class ApiLanguageKeys
    {
        public static string LanguageKey {get;} = Environment.GetEnvironmentVariable("LANGUAGE_KEY") ;
        public  static string LanguageEndpoint { get; } = Environment.GetEnvironmentVariable("LANGUAGE_ENDPOINT");
    }
}
