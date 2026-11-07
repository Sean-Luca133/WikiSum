using System;
using System.Collections.Generic;
using System.Text;

namespace WikiSum.Services.Navigation
{
    public interface INavigationService
    {
        Task NavigateToSummaryAsync();
        Task NavigateToDetailAsync();
        Task NavigateBack();
    }
}
