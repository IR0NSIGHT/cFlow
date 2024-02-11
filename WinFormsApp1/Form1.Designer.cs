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
            loading_spinner_box = new PictureBox();
            LayerTogglePanel = new FlowLayoutPanel();
            button_toogle_floodTool = new Button();
            maxLakeSurfaceInfo = new Label();
            maxLakeSurfaceNumeric = new NumericUpDown();
            maxLakeSurfaceLabel = new Label();
            maxLakeDepthInfoLabel = new Label();
            maxLakeDepthNumeric = new NumericUpDown();
            maxLakeDepthText = new Label();
            label3 = new Label();
            riverSplitProbInfo = new Label();
            riverSplitProbNumeric = new NumericUpDown();
            flowLayoutPanel1 = new FlowLayoutPanel();
            metroSetControlBox1 = new MetroSet_UI.Controls.MetroSetControlBox();
            tableLayoutPanel2 = new TableLayoutPanel();
            flowLayoutPanel2 = new FlowLayoutPanel();
            styledButton1 = new cFlowForms.StyledButton();
            ((System.ComponentModel.ISupportInitialize)heightPictureBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericRiverSpacingX).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericRiverSpacingY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)loading_spinner_box).BeginInit();
            ((System.ComponentModel.ISupportInitialize)maxLakeSurfaceNumeric).BeginInit();
            ((System.ComponentModel.ISupportInitialize)maxLakeDepthNumeric).BeginInit();
            ((System.ComponentModel.ISupportInitialize)riverSplitProbNumeric).BeginInit();
            flowLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            flowLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // heightPictureBox
            // 
            heightPictureBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            heightPictureBox.Location = new System.Drawing.Point(2, 2);
            heightPictureBox.Margin = new Padding(2);
            heightPictureBox.Name = "heightPictureBox";
            heightPictureBox.Size = new Size(1249, 691);
            heightPictureBox.TabIndex = 0;
            heightPictureBox.TabStop = false;
            // 
            // genFlowButton
            // 
            genFlowButton.Location = new System.Drawing.Point(234, 2);
            genFlowButton.Margin = new Padding(2);
            genFlowButton.Name = "genFlowButton";
            genFlowButton.Size = new Size(112, 68);
            genFlowButton.TabIndex = 4;
            genFlowButton.Text = "Generate Flow";
            genFlowButton.UseVisualStyleBackColor = true;
            genFlowButton.Click += onGenerateFlowButton;
            // 
            // genManyRiverButton
            // 
            genManyRiverButton.Location = new System.Drawing.Point(2, 2);
            genManyRiverButton.Margin = new Padding(2);
            genManyRiverButton.Name = "genManyRiverButton";
            genManyRiverButton.Size = new Size(112, 68);
            genManyRiverButton.TabIndex = 5;
            genManyRiverButton.Text = "Generate Rivers";
            genManyRiverButton.UseVisualStyleBackColor = true;
            // 
            // numericRiverSpacingX
            // 
            numericRiverSpacingX.Location = new System.Drawing.Point(466, 2);
            numericRiverSpacingX.Margin = new Padding(2);
            numericRiverSpacingX.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            numericRiverSpacingX.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericRiverSpacingX.Name = "numericRiverSpacingX";
            numericRiverSpacingX.Size = new Size(75, 23);
            numericRiverSpacingX.TabIndex = 6;
            numericRiverSpacingX.Value = new decimal(new int[] { 100, 0, 0, 0 });
            numericRiverSpacingX.ValueChanged += riverSpacingNumericChanged;
            // 
            // numericRiverSpacingY
            // 
            numericRiverSpacingY.Location = new System.Drawing.Point(545, 2);
            numericRiverSpacingY.Margin = new Padding(2);
            numericRiverSpacingY.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            numericRiverSpacingY.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericRiverSpacingY.Name = "numericRiverSpacingY";
            numericRiverSpacingY.Size = new Size(75, 23);
            numericRiverSpacingY.TabIndex = 7;
            numericRiverSpacingY.Value = new decimal(new int[] { 100, 0, 0, 0 });
            numericRiverSpacingY.ValueChanged += riverSpacingNumericChanged;
            // 
            // xRiverSpacing
            // 
            xRiverSpacing.AutoSize = true;
            xRiverSpacing.Location = new System.Drawing.Point(216, 0);
            xRiverSpacing.Margin = new Padding(2, 0, 2, 0);
            xRiverSpacing.Name = "xRiverSpacing";
            xRiverSpacing.Size = new Size(74, 15);
            xRiverSpacing.TabIndex = 8;
            xRiverSpacing.Text = "spacing on x";
            // 
            // labelCurrentScaleDescription
            // 
            labelCurrentScaleDescription.AutoSize = true;
            labelCurrentScaleDescription.Location = new System.Drawing.Point(106, 120);
            labelCurrentScaleDescription.Margin = new Padding(2, 0, 2, 0);
            labelCurrentScaleDescription.Name = "labelCurrentScaleDescription";
            labelCurrentScaleDescription.Size = new Size(47, 15);
            labelCurrentScaleDescription.TabIndex = 9;
            labelCurrentScaleDescription.Text = "scale = ";
            // 
            // spawnSingleRiverButton
            // 
            spawnSingleRiverButton.BackColor = SystemColors.Control;
            spawnSingleRiverButton.Location = new System.Drawing.Point(350, 2);
            spawnSingleRiverButton.Margin = new Padding(2);
            spawnSingleRiverButton.Name = "spawnSingleRiverButton";
            spawnSingleRiverButton.Size = new Size(112, 68);
            spawnSingleRiverButton.TabIndex = 10;
            spawnSingleRiverButton.Text = "Spawn single river";
            spawnSingleRiverButton.UseVisualStyleBackColor = false;
            spawnSingleRiverButton.Click += OnSpawnRiverButtonClick;
            // 
            // labelCurrentScale
            // 
            labelCurrentScale.AutoSize = true;
            labelCurrentScale.Location = new System.Drawing.Point(235, 120);
            labelCurrentScale.Margin = new Padding(2, 0, 2, 0);
            labelCurrentScale.Name = "labelCurrentScale";
            labelCurrentScale.Size = new Size(13, 15);
            labelCurrentScale.TabIndex = 12;
            labelCurrentScale.Text = "1";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(157, 120);
            label2.Margin = new Padding(2, 0, 2, 0);
            label2.Name = "label2";
            label2.Size = new Size(74, 15);
            label2.TabIndex = 11;
            label2.Text = "spacing on y";
            // 
            // loading_spinner_box
            // 
            loading_spinner_box.Image = (Image)resources.GetObject("loading_spinner_box.Image");
            loading_spinner_box.Location = new System.Drawing.Point(1255, 697);
            loading_spinner_box.Margin = new Padding(2);
            loading_spinner_box.Name = "loading_spinner_box";
            loading_spinner_box.Size = new Size(35, 38);
            loading_spinner_box.SizeMode = PictureBoxSizeMode.StretchImage;
            loading_spinner_box.TabIndex = 14;
            loading_spinner_box.TabStop = false;
            // 
            // LayerTogglePanel
            // 
            LayerTogglePanel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            LayerTogglePanel.Location = new System.Drawing.Point(2, 2);
            LayerTogglePanel.Margin = new Padding(2);
            LayerTogglePanel.Name = "LayerTogglePanel";
            LayerTogglePanel.Size = new Size(210, 116);
            LayerTogglePanel.TabIndex = 17;
            // 
            // button_toogle_floodTool
            // 
            button_toogle_floodTool.BackColor = SystemColors.Control;
            button_toogle_floodTool.Location = new System.Drawing.Point(118, 2);
            button_toogle_floodTool.Margin = new Padding(2);
            button_toogle_floodTool.Name = "button_toogle_floodTool";
            button_toogle_floodTool.Size = new Size(112, 68);
            button_toogle_floodTool.TabIndex = 18;
            button_toogle_floodTool.Text = "Flood Tool";
            button_toogle_floodTool.UseVisualStyleBackColor = false;
            button_toogle_floodTool.Click += OnToggleFloodToolButtonClick;
            // 
            // maxLakeSurfaceInfo
            // 
            maxLakeSurfaceInfo.AutoSize = true;
            maxLakeSurfaceInfo.Location = new System.Drawing.Point(2, 149);
            maxLakeSurfaceInfo.Margin = new Padding(2, 0, 2, 0);
            maxLakeSurfaceInfo.Name = "maxLakeSurfaceInfo";
            maxLakeSurfaceInfo.Size = new Size(95, 15);
            maxLakeSurfaceInfo.TabIndex = 20;
            maxLakeSurfaceInfo.Text = "max lake surface";
            // 
            // maxLakeSurfaceNumeric
            // 
            maxLakeSurfaceNumeric.Location = new System.Drawing.Point(252, 122);
            maxLakeSurfaceNumeric.Margin = new Padding(2);
            maxLakeSurfaceNumeric.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            maxLakeSurfaceNumeric.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            maxLakeSurfaceNumeric.Name = "maxLakeSurfaceNumeric";
            maxLakeSurfaceNumeric.Size = new Size(34, 23);
            maxLakeSurfaceNumeric.TabIndex = 19;
            maxLakeSurfaceNumeric.Value = new decimal(new int[] { 100, 0, 0, 0 });
            // 
            // maxLakeSurfaceLabel
            // 
            maxLakeSurfaceLabel.AutoSize = true;
            maxLakeSurfaceLabel.Location = new System.Drawing.Point(101, 149);
            maxLakeSurfaceLabel.Margin = new Padding(2, 0, 2, 0);
            maxLakeSurfaceLabel.Name = "maxLakeSurfaceLabel";
            maxLakeSurfaceLabel.Size = new Size(48, 15);
            maxLakeSurfaceLabel.TabIndex = 21;
            maxLakeSurfaceLabel.Text = "x 100 m";
            // 
            // maxLakeDepthInfoLabel
            // 
            maxLakeDepthInfoLabel.AutoSize = true;
            maxLakeDepthInfoLabel.Location = new System.Drawing.Point(191, 149);
            maxLakeDepthInfoLabel.Margin = new Padding(2, 0, 2, 0);
            maxLakeDepthInfoLabel.Name = "maxLakeDepthInfoLabel";
            maxLakeDepthInfoLabel.Size = new Size(88, 15);
            maxLakeDepthInfoLabel.TabIndex = 23;
            maxLakeDepthInfoLabel.Text = "max lake depth";
            // 
            // maxLakeDepthNumeric
            // 
            maxLakeDepthNumeric.Location = new System.Drawing.Point(153, 151);
            maxLakeDepthNumeric.Margin = new Padding(2);
            maxLakeDepthNumeric.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            maxLakeDepthNumeric.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            maxLakeDepthNumeric.Name = "maxLakeDepthNumeric";
            maxLakeDepthNumeric.Size = new Size(34, 23);
            maxLakeDepthNumeric.TabIndex = 22;
            maxLakeDepthNumeric.Value = new decimal(new int[] { 10, 0, 0, 0 });
            // 
            // maxLakeDepthText
            // 
            maxLakeDepthText.AutoSize = true;
            maxLakeDepthText.Location = new System.Drawing.Point(283, 149);
            maxLakeDepthText.Margin = new Padding(2, 0, 2, 0);
            maxLakeDepthText.Name = "maxLakeDepthText";
            maxLakeDepthText.Size = new Size(18, 15);
            maxLakeDepthText.TabIndex = 24;
            maxLakeDepthText.Text = "m";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(162, 176);
            label3.Margin = new Padding(2, 0, 2, 0);
            label3.Name = "label3";
            label3.Size = new Size(119, 30);
            label3.TabIndex = 27;
            label3.Text = "blocks between splits\r\non average";
            label3.Click += label3_Click;
            // 
            // riverSplitProbInfo
            // 
            riverSplitProbInfo.AutoSize = true;
            riverSplitProbInfo.Location = new System.Drawing.Point(40, 176);
            riverSplitProbInfo.Margin = new Padding(2, 0, 2, 0);
            riverSplitProbInfo.Name = "riverSplitProbInfo";
            riverSplitProbInfo.Size = new Size(118, 15);
            riverSplitProbInfo.TabIndex = 26;
            riverSplitProbInfo.Text = "River split probability";
            // 
            // riverSplitProbNumeric
            // 
            riverSplitProbNumeric.Location = new System.Drawing.Point(2, 178);
            riverSplitProbNumeric.Margin = new Padding(2);
            riverSplitProbNumeric.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            riverSplitProbNumeric.Minimum = new decimal(new int[] { 1, 0, 0, int.MinValue });
            riverSplitProbNumeric.Name = "riverSplitProbNumeric";
            riverSplitProbNumeric.Size = new Size(34, 23);
            riverSplitProbNumeric.TabIndex = 25;
            riverSplitProbNumeric.Value = new decimal(new int[] { 10, 0, 0, 0 });
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            flowLayoutPanel1.Controls.Add(LayerTogglePanel);
            flowLayoutPanel1.Controls.Add(xRiverSpacing);
            flowLayoutPanel1.Controls.Add(metroSetControlBox1);
            flowLayoutPanel1.Controls.Add(labelCurrentScaleDescription);
            flowLayoutPanel1.Controls.Add(label2);
            flowLayoutPanel1.Controls.Add(labelCurrentScale);
            flowLayoutPanel1.Controls.Add(maxLakeSurfaceNumeric);
            flowLayoutPanel1.Controls.Add(maxLakeSurfaceInfo);
            flowLayoutPanel1.Controls.Add(maxLakeSurfaceLabel);
            flowLayoutPanel1.Controls.Add(maxLakeDepthNumeric);
            flowLayoutPanel1.Controls.Add(maxLakeDepthInfoLabel);
            flowLayoutPanel1.Controls.Add(maxLakeDepthText);
            flowLayoutPanel1.Controls.Add(riverSplitProbNumeric);
            flowLayoutPanel1.Controls.Add(riverSplitProbInfo);
            flowLayoutPanel1.Controls.Add(label3);
            flowLayoutPanel1.Location = new System.Drawing.Point(1255, 2);
            flowLayoutPanel1.Margin = new Padding(2);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(310, 691);
            flowLayoutPanel1.TabIndex = 28;
            // 
            // metroSetControlBox1
            // 
            metroSetControlBox1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            metroSetControlBox1.CloseHoverBackColor = Color.FromArgb(183, 40, 40);
            metroSetControlBox1.CloseHoverForeColor = Color.White;
            metroSetControlBox1.CloseNormalForeColor = Color.Gray;
            metroSetControlBox1.DisabledForeColor = Color.DimGray;
            metroSetControlBox1.IsDerivedStyle = true;
            metroSetControlBox1.Location = new System.Drawing.Point(2, 122);
            metroSetControlBox1.Margin = new Padding(2);
            metroSetControlBox1.MaximizeBox = true;
            metroSetControlBox1.MaximizeHoverBackColor = Color.FromArgb(238, 238, 238);
            metroSetControlBox1.MaximizeHoverForeColor = Color.Gray;
            metroSetControlBox1.MaximizeNormalForeColor = Color.Gray;
            metroSetControlBox1.MinimizeBox = true;
            metroSetControlBox1.MinimizeHoverBackColor = Color.FromArgb(238, 238, 238);
            metroSetControlBox1.MinimizeHoverForeColor = Color.Gray;
            metroSetControlBox1.MinimizeNormalForeColor = Color.Gray;
            metroSetControlBox1.Name = "metroSetControlBox1";
            metroSetControlBox1.Size = new Size(100, 25);
            metroSetControlBox1.Style = MetroSet_UI.Enums.Style.Light;
            metroSetControlBox1.StyleManager = null;
            metroSetControlBox1.TabIndex = 19;
            metroSetControlBox1.Text = "metroSetControlBox1";
            metroSetControlBox1.ThemeAuthor = "Narwin";
            metroSetControlBox1.ThemeName = "MetroLite";
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 80F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tableLayoutPanel2.Controls.Add(flowLayoutPanel1, 1, 0);
            tableLayoutPanel2.Controls.Add(flowLayoutPanel2, 0, 1);
            tableLayoutPanel2.Controls.Add(heightPictureBox, 0, 0);
            tableLayoutPanel2.Controls.Add(loading_spinner_box, 1, 1);
            tableLayoutPanel2.Location = new System.Drawing.Point(8, 9);
            tableLayoutPanel2.Margin = new Padding(2);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 2;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 82.5454559F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 17.454546F));
            tableLayoutPanel2.Size = new Size(1567, 843);
            tableLayoutPanel2.TabIndex = 29;
            // 
            // flowLayoutPanel2
            // 
            flowLayoutPanel2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            flowLayoutPanel2.Controls.Add(genManyRiverButton);
            flowLayoutPanel2.Controls.Add(button_toogle_floodTool);
            flowLayoutPanel2.Controls.Add(genFlowButton);
            flowLayoutPanel2.Controls.Add(spawnSingleRiverButton);
            flowLayoutPanel2.Controls.Add(numericRiverSpacingX);
            flowLayoutPanel2.Controls.Add(numericRiverSpacingY);
            flowLayoutPanel2.Controls.Add(styledButton1);
            flowLayoutPanel2.Location = new System.Drawing.Point(2, 697);
            flowLayoutPanel2.Margin = new Padding(2);
            flowLayoutPanel2.Name = "flowLayoutPanel2";
            flowLayoutPanel2.Size = new Size(1249, 144);
            flowLayoutPanel2.TabIndex = 29;
            // 
            // styledButton1
            // 
            styledButton1.Location = new System.Drawing.Point(625, 3);
            styledButton1.Name = "styledButton1";
            styledButton1.Size = new Size(220, 60);
            styledButton1.TabIndex = 19;
            styledButton1.Text = "styledButton1";
            styledButton1.UseVisualStyleBackColor = false;
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            BackColor = SystemColors.ActiveCaption;
            ClientSize = new Size(1584, 861);
            Controls.Add(tableLayoutPanel2);
            Margin = new Padding(2);
            MinimumSize = new Size(565, 347);
            Name = "MainWindow";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)heightPictureBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericRiverSpacingX).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericRiverSpacingY).EndInit();
            ((System.ComponentModel.ISupportInitialize)loading_spinner_box).EndInit();
            ((System.ComponentModel.ISupportInitialize)maxLakeSurfaceNumeric).EndInit();
            ((System.ComponentModel.ISupportInitialize)maxLakeDepthNumeric).EndInit();
            ((System.ComponentModel.ISupportInitialize)riverSplitProbNumeric).EndInit();
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            flowLayoutPanel2.ResumeLayout(false);
            ResumeLayout(false);
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
        private PictureBox loading_spinner_box;
        private FlowLayoutPanel LayerTogglePanel;
        private Button button_toogle_floodTool;
        private Label maxLakeSurfaceInfo;
        private NumericUpDown maxLakeSurfaceNumeric;
        private Label maxLakeSurfaceLabel;
        private Label label1;
        private Label maxLakeDepthInfoLabel;
        private NumericUpDown maxLakeDepthNumeric;
        private Label maxLakeDepthText;
        private Label label3;
        private Label riverSplitProbInfo;
        private NumericUpDown riverSplitProbNumeric;
        private FlowLayoutPanel flowLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel2;
        private FlowLayoutPanel flowLayoutPanel2;
        private MetroSet_UI.Controls.MetroSetControlBox metroSetControlBox1;
        private cFlowForms.StyledButton styledButton1;
    }
}