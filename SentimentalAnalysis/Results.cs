using Microsoft.ML.Data;

namespace SentimentalAnalysis;

public class Results
{
    // https://learn.microsoft.com/en-us/dotnet/api/microsoft.ml.data.columnnameattribute.-ctor?view=ml-dotnet&viewFallbackFrom=netstandard-2.0
    [ColumnName("PredictedLabel")] public bool Prediction { get; set; }
    public float Probability { get; set; }
    public float Score { get; set; }
}