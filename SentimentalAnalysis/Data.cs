using Microsoft.ML.Data;

namespace SentimentalAnalysis;

public class Data
{
    [LoadColumn(0)] public string Text { get; set; } = string.Empty;
    [LoadColumn(1), ColumnName("Label")] public bool Sentiment { get; set; }
}