using Microsoft.ML;
using SentimentalAnalysis;
using Microsoft.ML.Data;

namespace CreateModel
{
    internal static class Program
    {
        private const string DataPath = "yelp_labelled.txt";
        private const string ModelPath = "model.zip";
        private const string EvaluationPath = "evaluation.txt";

        private static void Main()
        {
            try
            {
                var model = new Model(ModelPath, DataPath);
                var mlContext = new MLContext(seed: 1);
                var dataView = model.LoadData(mlContext);
                var splitData = model.SplitData(dataView);
                var trainedModel = model.BuildAndTrain(splitData.TrainSet);
                model.Save(dataView);
                var metrics = model.Evaluate(mlContext, trainedModel, splitData.TestSet);
                SaveMetrics(metrics);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void SaveMetrics(CalibratedBinaryClassificationMetrics metrics)
        {
            try
            {
                using var writer = new StreamWriter(EvaluationPath);
                writer.WriteLine(Display.GetMetrics(metrics));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}