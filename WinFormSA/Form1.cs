using Microsoft.ML;
using SentimentalAnalysis;

namespace WinFormSA;

public partial class Form1 : Form
{
    private readonly Model _model;
    private readonly MLContext _mlContext;
    private PredictionEngine<Data, Results>? _predictionEngine;
    private string _lastPredictedText = string.Empty;
    private const string ModelPath = "model.zip";
    private const string DataPath = "yelp_labelled.txt";
    private const string TrainingLog = "trainingLog.txt";
    private const string LastEvaluationPath = "lastEvaluation.txt";


    public Form1()
    {
        InitializeComponent();

        try
        {
            _mlContext = new MLContext(seed: 1);
            _model = new Model(ModelPath, DataPath);
            var loadedModel = _model.LoadModel(_mlContext);
            _predictionEngine = _mlContext.Model.CreatePredictionEngine<Data, Results>(loadedModel);

            var originalMetrics = _model.GetOriginalEvaluation(_mlContext, loadedModel);
            OriginalEvaluation.Text = originalMetrics;
            File.WriteAllText(LastEvaluationPath, originalMetrics);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error initializing model: {ex.Message}", "Error", MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void SubmitButton_Click(object sender, EventArgs e)
    {
        try
        {
            if (_predictionEngine == null)
            {
                MessageBox.Show("Prediction engine not initialized.", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            var inputText = InputTextBox.Text.Trim();
            if (string.IsNullOrEmpty(inputText))
            {
                MessageBox.Show("Input text is empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var result = _predictionEngine.Predict(new Data { Text = inputText });
            _lastPredictedText = inputText;

            var output = $"{inputText}\n" + Display.GetPrediction(result.Prediction, result.Probability, result.Score);
            OutputTextBox.Text = output + OutputTextBox.Text;
            InputTextBox.Clear();

            AccurateRadio.Enabled = true;
            InaccurateRadio.Enabled = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error making prediction: {ex.Message}", "Error", MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void RetrainButton_Click(object sender, EventArgs e)
    {
        try
        {
            if (_predictionEngine == null)
            {
                MessageBox.Show("Prediction engine not initialized.", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            var text = _lastPredictedText;

            var predictedSentiment = _predictionEngine.Predict(new Data { Text = text }).Prediction;
            var sentiment = AccurateRadio.Checked ? (predictedSentiment ? "1" : "0") : (predictedSentiment ? "0" : "1");
            File.AppendAllText(DataPath, $"{text}\t{sentiment}\n");

            var dataView = _model.LoadData(_mlContext);
            var splitData = _model.SplitData(dataView);
            var trainedModel = _model.BuildAndTrain(splitData.TrainSet);
            _model.Save(dataView);
            _predictionEngine = _mlContext.Model.CreatePredictionEngine<Data, Results>(trainedModel);
            // var testData = _model.LoadData(_mlContext);
            // var metrics = _model.Evaluate(_mlContext, trainedModel, testData);
            var metrics = _model.Evaluate(_mlContext, trainedModel, splitData.TestSet);
            Display.SaveMetricsToFile(metrics, TrainingLog, append: true);
            Display.SaveMetricsToFile(metrics, LastEvaluationPath, append: false);
            RecentEvaluation.Text = Display.GetMetrics(metrics, showChanges: true);

            RetrainButton.Enabled = false;
            AccurateRadio.Checked = false;
            InaccurateRadio.Checked = false;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error retraining model: {ex.Message}", "Error", MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void AccurateRadio_CheckedChanged(object sender, EventArgs e)
    {
        RetrainButton.Enabled = AccurateRadio.Checked || InaccurateRadio.Checked;
    }

    private void InaccurateRadio_CheckedChanged(object sender, EventArgs e)
    {
        RetrainButton.Enabled = AccurateRadio.Checked || InaccurateRadio.Checked;
    }

    private void InputTextBox_TextChanged(object sender, EventArgs e)
    {
        SubmitButton.Enabled = !string.IsNullOrWhiteSpace(InputTextBox.Text);
    }
}