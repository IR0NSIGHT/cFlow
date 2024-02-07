using System.Windows.Forms;

namespace WinFormsApp1
{
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            heightPictureBox = new PictureBox();
            genFlowButton = new Button();
            genManyRiverButton = new Button();
            numericRiverSpacingX = new NumericUpDown();
            numericRiverSpacingY = new NumericUpDown();
            xRiverSpacing = new Label();
            labelCurrentScaleDescription = new Label();
            spawnSingleRiverButton = new Button();
            labelCurrentScale = new Label();
            label2 = new Label();
            progressBar1 = new ProgressBar();
            loading_spinner_box = new PictureBox();
            ImportHeightmapButton = new Button();
            ((System.ComponentModel.ISupportInitialize)heightPictureBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericRiverSpacingX).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericRiverSpacingY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)loading_spinner_box).BeginInit();
            SuspendLayout();
            // 
            // heightPictureBox
            // 
            heightPictureBox.Location = new System.Drawing.Point(12, 12);
            heightPictureBox.Name = "heightPictureBox";
            heightPictureBox.Size = new Size(1523, 1017);
            heightPictureBox.TabIndex = 0;
            heightPictureBox.TabStop = false;
            // 
            // genFlowButton
            // 
            genFlowButton.Location = new System.Drawing.Point(1541, 747);
            genFlowButton.Name = "genFlowButton";
            genFlowButton.Size = new Size(160, 90);
            genFlowButton.TabIndex = 4;
            genFlowButton.Text = "Generate Flow";
            genFlowButton.UseVisualStyleBackColor = true;
            genFlowButton.Click += onGenerateFlowButton;
            // 
            // genManyRiverButton
            // 
            genManyRiverButton.Location = new System.Drawing.Point(1541, 843);
            genManyRiverButton.Name = "genManyRiverButton";
            genManyRiverButton.Size = new Size(160, 90);
            genManyRiverButton.TabIndex = 5;
            genManyRiverButton.Text = "Generate Rivers";
            genManyRiverButton.UseVisualStyleBackColor = true;
            // 
            // numericRiverSpacingX
            // 
            numericRiverSpacingX.Location = new System.Drawing.Point(1707, 843);
            numericRiverSpacingX.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            numericRiverSpacingX.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericRiverSpacingX.Name = "numericRiverSpacingX";
            numericRiverSpacingX.Size = new Size(107, 23);
            numericRiverSpacingX.TabIndex = 6;
            numericRiverSpacingX.Value = new decimal(new int[] { 100, 0, 0, 0 });
            numericRiverSpacingX.ValueChanged += riverSpacingNumericChanged;
            // 
            // numericRiverSpacingY
            // 
            numericRiverSpacingY.Location = new System.Drawing.Point(1707, 872);
            numericRiverSpacingY.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            numericRiverSpacingY.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericRiverSpacingY.Name = "numericRiverSpacingY";
            numericRiverSpacingY.Size = new Size(107, 23);
            numericRiverSpacingY.TabIndex = 7;
            numericRiverSpacingY.Value = new decimal(new int[] { 100, 0, 0, 0 });
            numericRiverSpacingY.ValueChanged += riverSpacingNumericChanged;
            // 
            // xRiverSpacing
            // 
            xRiverSpacing.AutoSize = true;
            xRiverSpacing.Location = new System.Drawing.Point(1820, 845);
            xRiverSpacing.Name = "xRiverSpacing";
            xRiverSpacing.Size = new Size(74, 15);
            xRiverSpacing.TabIndex = 8;
            xRiverSpacing.Text = "spacing on x";
            // 
            // labelCurrentScaleDescription
            // 
            labelCurrentScaleDescription.AutoSize = true;
            labelCurrentScaleDescription.Location = new System.Drawing.Point(1541, 95);
            labelCurrentScaleDescription.Name = "labelCurrentScaleDescription";
            labelCurrentScaleDescription.Size = new Size(47, 15);
            labelCurrentScaleDescription.TabIndex = 9;
            labelCurrentScaleDescription.Text = "scale = ";
            // 
            // spawnSingleRiverButton
            // 
            spawnSingleRiverButton.BackColor = SystemColors.Control;
            spawnSingleRiverButton.Location = new System.Drawing.Point(1541, 939);
            spawnSingleRiverButton.Name = "spawnSingleRiverButton";
            spawnSingleRiverButton.Size = new Size(160, 90);
            spawnSingleRiverButton.TabIndex = 10;
            spawnSingleRiverButton.Text = "Spawn single river";
            spawnSingleRiverButton.UseVisualStyleBackColor = false;
            spawnSingleRiverButton.Click += OnSpawnRiverButtonClick;
            // 
            // labelCurrentScale
            // 
            labelCurrentScale.AutoSize = true;
            labelCurrentScale.Location = new System.Drawing.Point(1594, 95);
            labelCurrentScale.Name = "labelCurrentScale";
            labelCurrentScale.Size = new Size(13, 15);
            labelCurrentScale.TabIndex = 12;
            labelCurrentScale.Text = "1";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(1820, 874);
            label2.Name = "label2";
            label2.Size = new Size(74, 15);
            label2.TabIndex = 11;
            label2.Text = "spacing on y";
            // 
            // progressBar1
            // 
            progressBar1.Location = new System.Drawing.Point(1541, 622);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(160, 23);
            progressBar1.TabIndex = 13;
            // 
            // loading_spinner_box
            // 
            loading_spinner_box.Image = (Image)resources.GetObject("loading_spinner_box.Image");
            loading_spinner_box.Location = new System.Drawing.Point(1541, 12);
            loading_spinner_box.Name = "loading_spinner_box";
            loading_spinner_box.Size = new Size(80, 80);
            loading_spinner_box.SizeMode = PictureBoxSizeMode.StretchImage;
            loading_spinner_box.TabIndex = 14;
            loading_spinner_box.TabStop = false;
            // 
            // ImportHeightmapButton
            // 
            ImportHeightmapButton.Location = new System.Drawing.Point(1541, 651);
            ImportHeightmapButton.Name = "ImportHeightmapButton";
            ImportHeightmapButton.Size = new Size(160, 90);
            ImportHeightmapButton.TabIndex = 15;
            ImportHeightmapButton.Text = "Import Heightmap";
            ImportHeightmapButton.UseVisualStyleBackColor = true;
            ImportHeightmapButton.Click += OnImportHeightmapButtonClick;
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1904, 1041);
            Controls.Add(ImportHeightmapButton);
            Controls.Add(loading_spinner_box);
            Controls.Add(progressBar1);
            Controls.Add(labelCurrentScale);
            Controls.Add(label2);
            Controls.Add(spawnSingleRiverButton);
            Controls.Add(labelCurrentScaleDescription);
            Controls.Add(xRiverSpacing);
            Controls.Add(numericRiverSpacingY);
            Controls.Add(numericRiverSpacingX);
            Controls.Add(genManyRiverButton);
            Controls.Add(genFlowButton);
            Controls.Add(heightPictureBox);
            Name = "MainWindow";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)heightPictureBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericRiverSpacingX).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericRiverSpacingY).EndInit();
            ((System.ComponentModel.ISupportInitialize)loading_spinner_box).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox heightPictureBox;
        private Button genFlowButton;
        private Button genManyRiverButton;
        private NumericUpDown numericRiverSpacingX;
        private NumericUpDown numericRiverSpacingY;
        private Label xRiverSpacing;
        private Label labelCurrentScaleDescription;
        private Button spawnSingleRiverButton;
        private Label labelCurrentScale;
        private Label label2;
        private ProgressBar progressBar1;
        private PictureBox loading_spinner_box;
        private Button ImportHeightmapButton;
    }
}