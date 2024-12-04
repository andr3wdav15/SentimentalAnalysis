using Microsoft.ML.Data;
using System.IO;

namespace SentimentalAnalysis;

public static class Display
{
    public static string GetPrediction(bool prediction, float probability, float score)
    {
        return $"Prediction: {(prediction ? "Positive" : "Negative")}\n" +
               $"Probability: {probability:P2}\n" +
               $"Score: {score}\n";
    }

    private static CalibratedBinaryClassificationMetrics? _lastMetrics = null;

    private static string GetChange(double current, double? previous, bool isPercentage = true,
        bool invertImprovement = false)
    {
        if (previous == null) return "";
        var change = current - previous.Value;
        if (invertImprovement) change = -change;
        var arrow = change > 0 ? "↑" : "↓";
        var absChange = Math.Abs(change);
        var changeValue = isPercentage ? (absChange * 100).ToString("F2") + "%" : absChange.ToString("F4");
        return $" ({arrow}{changeValue})";
    }

    public static string GetMetrics(CalibratedBinaryClassificationMetrics metrics, bool showChanges = false)
    {
        if (metrics == null)
            throw new ArgumentNullException(nameof(metrics), "Metrics cannot be null");

        var currentMetrics = metrics;
        var previousMetrics = _lastMetrics;

        if (showChanges)
        {
            _lastMetrics = metrics;
        }

        return
            $"Accuracy: {metrics.Accuracy:P2}{(showChanges ? GetChange(metrics.Accuracy, previousMetrics?.Accuracy) : "")}\n" +
            $"Area Under ROC Curve: {metrics.AreaUnderRocCurve:P2}{(showChanges ? GetChange(metrics.AreaUnderRocCurve, previousMetrics?.AreaUnderRocCurve) : "")}\n" +
            $"F1 Score: {metrics.F1Score:P2}{(showChanges ? GetChange(metrics.F1Score, previousMetrics?.F1Score) : "")}\n" +
            $"Log Loss: {metrics.LogLoss:F4}{(showChanges ? GetChange(metrics.LogLoss, previousMetrics?.LogLoss, false, true) : "")}\n" +
            $"Positive Precision: {metrics.PositivePrecision:P2}{(showChanges ? GetChange(metrics.PositivePrecision, previousMetrics?.PositivePrecision) : "")}\n" +
            $"Positive Recall: {metrics.PositiveRecall:P2}{(showChanges ? GetChange(metrics.PositiveRecall, previousMetrics?.PositiveRecall) : "")}\n" +
            $"Negative Precision: {metrics.NegativePrecision:P2}{(showChanges ? GetChange(metrics.NegativePrecision, previousMetrics?.NegativePrecision) : "")}\n" +
            $"Negative Recall: {metrics.NegativeRecall:P2}{(showChanges ? GetChange(metrics.NegativeRecall, previousMetrics?.NegativeRecall) : "")}\n";
    }

    public static void SaveMetricsToFile(CalibratedBinaryClassificationMetrics metrics, string filePath,
        bool append = true)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

        if (metrics == null)
            throw new ArgumentNullException(nameof(metrics), "Metrics cannot be null");

        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        var metricsText = $"\n=== Evaluation at {timestamp} ===\n" + GetMetrics(metrics);

        try
        {
            if (append)
            {
                File.AppendAllText(filePath, metricsText);
            }
            else
            {
                File.WriteAllText(filePath, metricsText);
            }
        }
        catch (IOException ex)
        {
            throw new IOException($"Error writing to file {filePath}: {ex.Message}", ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            throw new UnauthorizedAccessException($"Access denied to file {filePath}: {ex.Message}", ex);
        }
    }
}