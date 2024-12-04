using Microsoft.ML;
using SentimentalAnalysis;

namespace WinFormSA;

public partial class Form1 : Form
{
    private readonly Model? _model;
    private readonly MLContext? _mlContext;
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

            try
            {
                var loadedModel = _model.LoadModel(_mlContext);
                _predictionEngine = _mlContext.Model.CreatePredictionEngine<Data, Results>(loadedModel);

                var originalMetrics = _model.GetOriginalEvaluation(_mlContext, loadedModel);
                OriginalEvaluation.Text = originalMetrics;
                File.WriteAllText(LastEvaluationPath, originalMetrics);
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show($"Model or data file not found: {ex.Message}", "File Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
            catch (IOException ex)
            {
                MessageBox.Show($"Error accessing model or data file: {ex.Message}", "File Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show($"Model error: {ex.Message}", "Model Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error initializing model: {ex.Message}", "Error", MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private async void SubmitButton_Click(object sender, EventArgs e)
    {
        try
        {
            if (_predictionEngine == null || _mlContext == null)
            {
                MessageBox.Show("Model not properly initialized.", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            var inputText = InputTextBox.Text.Trim();
            if (string.IsNullOrEmpty(inputText))
            {
                MessageBox.Show("Input text is empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SubmitButton.Enabled = false;
            try
            {
                var result = await Task.Run(() => _predictionEngine.Predict(new Data { Text = inputText }));
                _lastPredictedText = inputText;

                var output = $"{inputText}\n" +
                             Display.GetPrediction(result.Prediction, result.Probability, result.Score);
                OutputTextBox.Text = output + OutputTextBox.Text;
                InputTextBox.Clear();

                AccurateRadio.Enabled = true;
                InaccurateRadio.Enabled = true;
            }
            finally
            {
                SubmitButton.Enabled = true;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error making prediction: {ex.Message}", "Error", MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private async void RetrainButton_Click(object sender, EventArgs e)
    {
        try
        {
            if (_predictionEngine == null || _mlContext == null || _model == null)
            {
                MessageBox.Show("Model not properly initialized.", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            var text = _lastPredictedText;
            var predictedSentiment = _predictionEngine.Predict(new Data { Text = text }).Prediction;
            var sentiment = AccurateRadio.Checked ? (predictedSentiment ? "1" : "0") : (predictedSentiment ? "0" : "1");

            RetrainButton.Enabled = false;
            try
            {
                await Task.Run(() =>
                {
                    try
                    {
                        File.AppendAllText(DataPath, $"{text}\t{sentiment}\n");
                    }
                    catch (IOException ex)
                    {
                        throw new IOException($"Error writing to training data file: {ex.Message}", ex);
                    }

                    try
                    {
                        var dataView = _model.LoadData(_mlContext);
                        var splitData = _model.SplitData(dataView);
                        var trainedModel = _model.BuildAndTrain(splitData.TrainSet);
                        _model.Save(dataView);

                        this.Invoke(() =>
                        {
                            _predictionEngine =
                                _mlContext.Model.CreatePredictionEngine<Data, Results>(trainedModel);
                        });

                        var metrics = _model.Evaluate(_mlContext, trainedModel, splitData.TestSet);
                        Display.SaveMetricsToFile(metrics, TrainingLog, append: true);
                        Display.SaveMetricsToFile(metrics, LastEvaluationPath, append: false);

                        this.Invoke(() =>
                        {
                            RecentEvaluation.Text = Display.GetMetrics(metrics, showChanges: true);
                            RetrainButton.Enabled = false;
                            AccurateRadio.Checked = false;
                            InaccurateRadio.Checked = false;
                        });
                    }
                    catch (FileNotFoundException ex)
                    {
                        throw new FileNotFoundException($"Training data file not found: {ex.Message}", ex);
                    }
                    catch (InvalidOperationException ex)
                    {
                        throw new InvalidOperationException($"Model error: {ex.Message}", ex);
                    }
                    catch (IOException ex)
                    {
                        throw new IOException($"Error writing evaluation results: {ex.Message}", ex);
                    }
                });
            }
            finally
            {
                if (!this.IsDisposed && this.Created)
                {
                    this.Invoke(() => RetrainButton.Enabled = true);
                }
            }
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