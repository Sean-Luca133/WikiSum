using Azure.AI.TextAnalytics;
using System;
using System.Collections.Generic;
using System.Text;

namespace WikiSum.Services.EntityReconition
{
    public interface IEntityRecognitionService
    {
        CategorizedEntityCollection GetNamedEntities(string document);
    }
}
