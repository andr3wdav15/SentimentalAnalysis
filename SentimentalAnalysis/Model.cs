using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using Microsoft.ML.Calibrators;
using static Microsoft.ML.DataOperationsCatalog;

namespace SentimentalAnalysis;

public class Model
{
    private TransformerChain<BinaryPredictionTransformer<CalibratedModelParametersBase<LinearBinaryModelParameters,
        PlattCalibrator>>>? _trainedModel;

    private readonly MLContext _mlContext;
    private readonly string _modelPath;
    private readonly string _dataPath;

    public Model(string modelPath, string dataPath)
    {
        _mlContext = new MLContext();
        _modelPath = modelPath;
        _dataPath = dataPath;
    }

    public IDataView LoadData(MLContext mlContext)
    {
        var lines = File.ReadAllLines(_dataPath);
        if (lines.Length == 0)
            throw new InvalidOperationException("Training data file is empty");

        var allData = lines
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(line => 
            {
                var parts = line.Split('\t');
                return new Data { Text = parts[0], Sentiment = parts[1] == "1" };
            })
            .ToList();

        return mlContext.Data.LoadFromEnumerable(allData);
    }

    public TrainTestData SplitData(IDataView dataView, double testFraction = 0.2)
    {
        return _mlContext.Data.TrainTestSplit(dataView, testFraction);
    }

    public ITransformer BuildAndTrain(IDataView trainSet)
    {
        var pipeline = BuildPipeline();
        _trainedModel = pipeline.Fit(trainSet);

        return _trainedModel;
    }

    public CalibratedBinaryClassificationMetrics Evaluate(MLContext mlContext, ITransformer model, IDataView testSet)
    {
        var predictions = model.Transform(testSet);
        return mlContext.BinaryClassification.Evaluate(
            predictions,
            labelColumnName: "Label",
            scoreColumnName: "Score"
        );
    }

    public void Save(IDataView dataView)
    {
        if (_trainedModel == null)
            throw new InvalidOperationException("Model has not been trained yet, cannot save.");

        _mlContext.Model.Save(_trainedModel, dataView.Schema, _modelPath);
    }

    public PredictionEngine<Data, Results> CreatePredictionEngine(MLContext mlContext)
    {
        return mlContext.Model.CreatePredictionEngine<Data, Results>(_trainedModel);
    }

    private EstimatorChain<BinaryPredictionTransformer<
        CalibratedModelParametersBase<LinearBinaryModelParameters, PlattCalibrator>>> BuildPipeline()
    {
        var pipeline = _mlContext.Transforms.Text
            .FeaturizeText(
                outputColumnName: "Features",
                inputColumnName: nameof(Data.Text))
            .Append(_mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(
                labelColumnName: "Label",
                featureColumnName: "Features"
            ));

        return pipeline;
    }

    public ITransformer LoadModel(MLContext mlContext)
    {
        var loadedModel = mlContext.Model.Load(_modelPath, out _);
        _trainedModel = loadedModel as TransformerChain<BinaryPredictionTransformer<
            CalibratedModelParametersBase<LinearBinaryModelParameters, PlattCalibrator>>>;
        return loadedModel;
    }

    private CalibratedBinaryClassificationMetrics LoadEvaluation(MLContext mlContext, ITransformer model)
    {
        var dataView = LoadData(mlContext);
        var splitData = SplitData(dataView, testFraction: 0.2);
        return Evaluate(mlContext, model, splitData.TestSet);
    }

    public string GetOriginalEvaluation(MLContext mlContext, ITransformer model)
    {
        var metrics = LoadEvaluation(mlContext, model);

        Display.GetMetrics(metrics, showChanges: true);
        return Display.GetMetrics(metrics, showChanges: false);
    }
}