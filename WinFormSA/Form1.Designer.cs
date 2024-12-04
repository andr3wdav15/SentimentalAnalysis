namespace WinFormSA
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            OutputTextBox = new RichTextBox();
            InputTextBox = new TextBox();
            SubmitButton = new Button();
            AccurateRadio = new RadioButton();
            InaccurateRadio = new RadioButton();
            RetrainButton = new Button();
            OriginalEvaluation = new RichTextBox();
            RecentEvaluation = new RichTextBox();
            PredictionLabel = new Label();
            OriginalEvalLabel = new Label();
            RecentEvalLabel = new Label();
            SuspendLayout();
            // 
            // OutputTextBox
            // 
            OutputTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            OutputTextBox.BackColor = SystemColors.ControlLightLight;
            OutputTextBox.Location = new Point(133, 67);
            OutputTextBox.MaximumSize = new Size(452, 344);
            OutputTextBox.Name = "OutputTextBox";
            OutputTextBox.ReadOnly = true;
            OutputTextBox.ScrollBars = RichTextBoxScrollBars.Vertical;
            OutputTextBox.Size = new Size(452, 344);
            OutputTextBox.TabIndex = 0;
            OutputTextBox.TabStop = false;
            OutputTextBox.Text = "";
            // 
            // InputTextBox
            // 
            InputTextBox.Location = new Point(133, 415);
            InputTextBox.Multiline = true;
            InputTextBox.Name = "InputTextBox";
            InputTextBox.PlaceholderText = "Enter the comment you would like a prediction for here...";
            InputTextBox.ScrollBars = ScrollBars.Vertical;
            InputTextBox.Size = new Size(452, 55);
            InputTextBox.TabIndex = 7;
            InputTextBox.TabStop = false;
            // 
            // SubmitButton
            // 
            SubmitButton.Location = new Point(516, 472);
            SubmitButton.Name = "SubmitButton";
            SubmitButton.Size = new Size(69, 23);
            SubmitButton.TabIndex = 0;
            SubmitButton.TabStop = false;
            SubmitButton.Text = "Submit";
            SubmitButton.UseVisualStyleBackColor = true;
            SubmitButton.Click += SubmitButton_Click;
            // 
            // AccurateRadio
            // 
            AccurateRadio.AutoSize = true;
            AccurateRadio.Location = new Point(143, 472);
            AccurateRadio.Name = "AccurateRadio";
            AccurateRadio.Size = new Size(72, 19);
            AccurateRadio.TabIndex = 0;
            AccurateRadio.Text = "Accurate";
            AccurateRadio.UseVisualStyleBackColor = true;
            AccurateRadio.CheckedChanged += AccurateRadio_CheckedChanged;
            // 
            // InaccurateRadio
            // 
            InaccurateRadio.AutoSize = true;
            InaccurateRadio.Location = new Point(221, 472);
            InaccurateRadio.Name = "InaccurateRadio";
            InaccurateRadio.Size = new Size(80, 19);
            InaccurateRadio.TabIndex = 0;
            InaccurateRadio.Text = "Inaccurate";
            InaccurateRadio.UseVisualStyleBackColor = true;
            InaccurateRadio.CheckedChanged += InaccurateRadio_CheckedChanged;
            // 
            // RetrainButton
            // 
            RetrainButton.Location = new Point(307, 472);
            RetrainButton.Name = "RetrainButton";
            RetrainButton.Size = new Size(75, 23);
            RetrainButton.TabIndex = 0;
            RetrainButton.TabStop = false;
            RetrainButton.Text = "Retrain";
            RetrainButton.UseVisualStyleBackColor = true;
            RetrainButton.Click += RetrainButton_Click;
            // 
            // OriginalEvaluation
            // 
            OriginalEvaluation.BackColor = SystemColors.ControlLightLight;
            OriginalEvaluation.Location = new Point(601, 67);
            OriginalEvaluation.Name = "OriginalEvaluation";
            OriginalEvaluation.ReadOnly = true;
            OriginalEvaluation.ScrollBars = RichTextBoxScrollBars.Vertical;
            OriginalEvaluation.Size = new Size(452, 188);
            OriginalEvaluation.TabIndex = 0;
            OriginalEvaluation.TabStop = false;
            OriginalEvaluation.Text = "";
            // 
            // RecentEvaluation
            // 
            RecentEvaluation.BackColor = SystemColors.ControlLightLight;
            RecentEvaluation.Location = new Point(601, 282);
            RecentEvaluation.Name = "RecentEvaluation";
            RecentEvaluation.ReadOnly = true;
            RecentEvaluation.ScrollBars = RichTextBoxScrollBars.Vertical;
            RecentEvaluation.Size = new Size(452, 188);
            RecentEvaluation.TabIndex = 0;
            RecentEvaluation.TabStop = false;
            RecentEvaluation.Text = "";
            // 
            // PredictionLabel
            // 
            PredictionLabel.AutoSize = true;
            PredictionLabel.Location = new Point(133, 49);
            PredictionLabel.Name = "PredictionLabel";
            PredictionLabel.Size = new Size(66, 15);
            PredictionLabel.TabIndex = 0;
            PredictionLabel.Text = "Predictions";
            // 
            // OriginalEvalLabel
            // 
            OriginalEvalLabel.AutoSize = true;
            OriginalEvalLabel.Location = new Point(601, 49);
            OriginalEvalLabel.Name = "OriginalEvalLabel";
            OriginalEvalLabel.Size = new Size(107, 15);
            OriginalEvalLabel.TabIndex = 0;
            OriginalEvalLabel.Text = "Original Evaluation";
            // 
            // RecentEvalLabel
            // 
            RecentEvalLabel.AutoSize = true;
            RecentEvalLabel.Location = new Point(601, 264);
            RecentEvalLabel.Name = "RecentEvalLabel";
            RecentEvalLabel.Size = new Size(106, 15);
            RecentEvalLabel.TabIndex = 0;
            RecentEvalLabel.Text = "Recent Evaluations";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1184, 562);
            Controls.Add(RecentEvalLabel);
            Controls.Add(OriginalEvalLabel);
            Controls.Add(PredictionLabel);
            Controls.Add(RecentEvaluation);
            Controls.Add(OriginalEvaluation);
            Controls.Add(RetrainButton);
            Controls.Add(InaccurateRadio);
            Controls.Add(AccurateRadio);
            Controls.Add(SubmitButton);
            Controls.Add(InputTextBox);
            Controls.Add(OutputTextBox);
            MinimumSize = new Size(1200, 600);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private RichTextBox OutputTextBox;
        private TextBox InputTextBox;
        private Button SubmitButton;
        private RadioButton AccurateRadio;
        private RadioButton InaccurateRadio;
        private Button RetrainButton;
        private RichTextBox OriginalEvaluation;
        private RichTextBox RecentEvaluation;
        private Label PredictionLabel;
        private Label OriginalEvalLabel;
        private Label RecentEvalLabel;
    }
}
