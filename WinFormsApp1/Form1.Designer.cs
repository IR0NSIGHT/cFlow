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
            LayerTogglePanel = new FlowLayoutPanel();
            button_toogle_floodTool = new Button();
            maxLakeSurfaceInfo = new Label();
            maxLakeSurfaceNumeric = new NumericUpDown();
            maxLakeSurfaceLabel = new Label();
            maxLakeDepthInfoLabel = new Label();
            maxLakeDepthNumeric = new NumericUpDown();
            maxLakeDepthText = new Label();
            ((System.ComponentModel.ISupportInitialize)heightPictureBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericRiverSpacingX).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericRiverSpacingY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)loading_spinner_box).BeginInit();
            ((System.ComponentModel.ISupportInitialize)maxLakeSurfaceNumeric).BeginInit();
            ((System.ComponentModel.ISupportInitialize)maxLakeDepthNumeric).BeginInit();
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
            genFlowButton.Location = new System.Drawing.Point(1541, 398);
            genFlowButton.Name = "genFlowButton";
            genFlowButton.Size = new Size(160, 90);
            genFlowButton.TabIndex = 4;
            genFlowButton.Text = "Generate Flow";
            genFlowButton.UseVisualStyleBackColor = true;
            genFlowButton.Click += onGenerateFlowButton;
            // 
            // genManyRiverButton
            // 
            genManyRiverButton.Location = new System.Drawing.Point(1541, 494);
            genManyRiverButton.Name = "genManyRiverButton";
            genManyRiverButton.Size = new Size(160, 90);
            genManyRiverButton.TabIndex = 5;
            genManyRiverButton.Text = "Generate Rivers";
            genManyRiverButton.UseVisualStyleBackColor = true;
            // 
            // numericRiverSpacingX
            // 
            numericRiverSpacingX.Location = new System.Drawing.Point(1707, 494);
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
            numericRiverSpacingY.Location = new System.Drawing.Point(1707, 523);
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
            xRiverSpacing.Location = new System.Drawing.Point(1820, 496);
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
            spawnSingleRiverButton.Location = new System.Drawing.Point(1541, 590);
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
            label2.Location = new System.Drawing.Point(1820, 525);
            label2.Name = "label2";
            label2.Size = new Size(74, 15);
            label2.TabIndex = 11;
            label2.Text = "spacing on y";
            // 
            // progressBar1
            // 
            progressBar1.Location = new System.Drawing.Point(1541, 273);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(353, 23);
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
            ImportHeightmapButton.Location = new System.Drawing.Point(1541, 302);
            ImportHeightmapButton.Name = "ImportHeightmapButton";
            ImportHeightmapButton.Size = new Size(160, 90);
            ImportHeightmapButton.TabIndex = 15;
            ImportHeightmapButton.Text = "Import Heightmap";
            ImportHeightmapButton.UseVisualStyleBackColor = true;
            ImportHeightmapButton.Click += OnImportHeightmapButtonClick;
            // 
            // LayerTogglePanel
            // 
            LayerTogglePanel.Location = new System.Drawing.Point(1541, 113);
            LayerTogglePanel.Name = "LayerTogglePanel";
            LayerTogglePanel.Size = new Size(351, 154);
            LayerTogglePanel.TabIndex = 17;
            // 
            // button_toogle_floodTool
            // 
            button_toogle_floodTool.BackColor = SystemColors.Control;
            button_toogle_floodTool.Location = new System.Drawing.Point(1541, 686);
            button_toogle_floodTool.Name = "button_toogle_floodTool";
            button_toogle_floodTool.Size = new Size(160, 90);
            button_toogle_floodTool.TabIndex = 18;
            button_toogle_floodTool.Text = "Flood Tool";
            button_toogle_floodTool.UseVisualStyleBackColor = false;
            button_toogle_floodTool.Click += OnToggleFloodToolButtonClick;
            // 
            // maxLakeSurfaceInfo
            // 
            maxLakeSurfaceInfo.AutoSize = true;
            maxLakeSurfaceInfo.Location = new System.Drawing.Point(1707, 686);
            maxLakeSurfaceInfo.Name = "maxLakeSurfaceInfo";
            maxLakeSurfaceInfo.Size = new Size(95, 15);
            maxLakeSurfaceInfo.TabIndex = 20;
            maxLakeSurfaceInfo.Text = "max lake surface";
            // 
            // maxLakeSurfaceNumeric
            // 
            maxLakeSurfaceNumeric.Location = new System.Drawing.Point(1707, 704);
            maxLakeSurfaceNumeric.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            maxLakeSurfaceNumeric.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            maxLakeSurfaceNumeric.Name = "maxLakeSurfaceNumeric";
            maxLakeSurfaceNumeric.Size = new Size(49, 23);
            maxLakeSurfaceNumeric.TabIndex = 19;
            maxLakeSurfaceNumeric.Value = new decimal(new int[] { 100, 0, 0, 0 });
            // 
            // maxLakeSurfaceLabel
            // 
            maxLakeSurfaceLabel.AutoSize = true;
            maxLakeSurfaceLabel.Location = new System.Drawing.Point(1762, 706);
            maxLakeSurfaceLabel.Name = "maxLakeSurfaceLabel";
            maxLakeSurfaceLabel.Size = new Size(48, 15);
            maxLakeSurfaceLabel.TabIndex = 21;
            maxLakeSurfaceLabel.Text = "x 100 m";
            // 
            // maxLakeDepthInfoLabel
            // 
            maxLakeDepthInfoLabel.AutoSize = true;
            maxLakeDepthInfoLabel.Location = new System.Drawing.Point(1707, 735);
            maxLakeDepthInfoLabel.Name = "maxLakeDepthInfoLabel";
            maxLakeDepthInfoLabel.Size = new Size(88, 15);
            maxLakeDepthInfoLabel.TabIndex = 23;
            maxLakeDepthInfoLabel.Text = "max lake depth";
            // 
            // maxLakeDepthNumeric
            // 
            maxLakeDepthNumeric.Location = new System.Drawing.Point(1707, 753);
            maxLakeDepthNumeric.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            maxLakeDepthNumeric.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            maxLakeDepthNumeric.Name = "maxLakeDepthNumeric";
            maxLakeDepthNumeric.Size = new Size(49, 23);
            maxLakeDepthNumeric.TabIndex = 22;
            maxLakeDepthNumeric.Value = new decimal(new int[] { 10, 0, 0, 0 });
            // 
            // maxLakeDepthText
            // 
            maxLakeDepthText.AutoSize = true;
            maxLakeDepthText.Location = new System.Drawing.Point(1762, 755);
            maxLakeDepthText.Name = "maxLakeDepthText";
            maxLakeDepthText.Size = new Size(18, 15);
            maxLakeDepthText.TabIndex = 24;
            maxLakeDepthText.Text = "m";
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1904, 1041);
            Controls.Add(maxLakeDepthText);
            Controls.Add(maxLakeDepthInfoLabel);
            Controls.Add(maxLakeDepthNumeric);
            Controls.Add(maxLakeSurfaceLabel);
            Controls.Add(maxLakeSurfaceInfo);
            Controls.Add(maxLakeSurfaceNumeric);
            Controls.Add(button_toogle_floodTool);
            Controls.Add(LayerTogglePanel);
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
            ((System.ComponentModel.ISupportInitialize)maxLakeSurfaceNumeric).EndInit();
            ((System.ComponentModel.ISupportInitialize)maxLakeDepthNumeric).EndInit();
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
        private FlowLayoutPanel LayerTogglePanel;
        private Button button_toogle_floodTool;
        private Label maxLakeSurfaceInfo;
        private NumericUpDown maxLakeSurfaceNumeric;
        private Label maxLakeSurfaceLabel;
        private Label label1;
        private Label maxLakeDepthInfoLabel;
        private NumericUpDown maxLakeDepthNumeric;
        private Label maxLakeDepthText;
    }
}