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
            heightPictureBox = new PictureBox();
            flowPicturBox = new PictureBox();
            riverPictureBox = new PictureBox();
            genFlowButton = new Button();
            genManyRiverButton = new Button();
            numericRiverSpacingX = new NumericUpDown();
            numericRiverSpacingY = new NumericUpDown();
            xRiverSpacing = new Label();
            label1 = new Label();
            ((System.ComponentModel.ISupportInitialize)heightPictureBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)flowPicturBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)riverPictureBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericRiverSpacingX).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericRiverSpacingY).BeginInit();
            SuspendLayout();
            // 
            // heightPictureBox
            // 
            heightPictureBox.Location = new System.Drawing.Point(91, 12);
            heightPictureBox.Name = "heightPictureBox";
            heightPictureBox.Size = new Size(836, 452);
            heightPictureBox.TabIndex = 0;
            heightPictureBox.TabStop = false;
            // 
            // flowPicturBox
            // 
            flowPicturBox.Location = new System.Drawing.Point(933, 12);
            flowPicturBox.Name = "flowPicturBox";
            flowPicturBox.Size = new Size(836, 452);
            flowPicturBox.TabIndex = 1;
            flowPicturBox.TabStop = false;
            // 
            // riverPictureBox
            // 
            riverPictureBox.Location = new System.Drawing.Point(91, 470);
            riverPictureBox.Name = "riverPictureBox";
            riverPictureBox.Size = new Size(836, 452);
            riverPictureBox.TabIndex = 2;
            riverPictureBox.TabStop = false;
            // 
            // genFlowButton
            // 
            genFlowButton.Location = new System.Drawing.Point(933, 470);
            genFlowButton.Name = "genFlowButton";
            genFlowButton.Size = new Size(160, 90);
            genFlowButton.TabIndex = 4;
            genFlowButton.Text = "Generate Flow";
            genFlowButton.UseVisualStyleBackColor = true;
            genFlowButton.Click += onGenerateFlowButton;
            // 
            // genManyRiverButton
            // 
            genManyRiverButton.Location = new System.Drawing.Point(933, 566);
            genManyRiverButton.Name = "genManyRiverButton";
            genManyRiverButton.Size = new Size(160, 90);
            genManyRiverButton.TabIndex = 5;
            genManyRiverButton.Text = "Generate Rivers";
            genManyRiverButton.UseVisualStyleBackColor = true;
            // 
            // numericRiverSpacingX
            // 
            numericRiverSpacingX.Location = new System.Drawing.Point(1099, 566);
            numericRiverSpacingX.Name = "numericRiverSpacingX";
            numericRiverSpacingX.Size = new Size(107, 23);
            numericRiverSpacingX.TabIndex = 6;
            numericRiverSpacingX.Value = new decimal(new int[] { 100, 0, 0, 0 });
            numericRiverSpacingX.Maximum = 100000;
            numericRiverSpacingX.Minimum = 1;
            numericRiverSpacingX.ValueChanged += riverSpacingNumericChanged;
            // 
            // numericRiverSpacingY
            // 
            numericRiverSpacingY.Location = new System.Drawing.Point(1099, 595);
            numericRiverSpacingY.Name = "numericRiverSpacingY";
            numericRiverSpacingY.Size = new Size(107, 23);
            numericRiverSpacingY.TabIndex = 7;
            numericRiverSpacingY.Value = new decimal(new int[] { 100, 0, 0, 0 });
            numericRiverSpacingY.Maximum = 100000;
            numericRiverSpacingY.Minimum = 1;
            numericRiverSpacingY.ValueChanged += riverSpacingNumericChanged;

            // 
            // xRiverSpacing
            // 
            xRiverSpacing.AutoSize = true;
            xRiverSpacing.Location = new System.Drawing.Point(1212, 568);
            xRiverSpacing.Name = "xRiverSpacing";
            xRiverSpacing.Size = new Size(74, 15);
            xRiverSpacing.TabIndex = 8;
            xRiverSpacing.Text = "spacing on x";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(1212, 597);
            label1.Name = "label1";
            label1.Size = new Size(74, 15);
            label1.TabIndex = 9;
            label1.Text = "spacing on y";
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1904, 1041);
            Controls.Add(label1);
            Controls.Add(xRiverSpacing);
            Controls.Add(numericRiverSpacingY);
            Controls.Add(numericRiverSpacingX);
            Controls.Add(genManyRiverButton);
            Controls.Add(genFlowButton);
            Controls.Add(riverPictureBox);
            Controls.Add(flowPicturBox);
            Controls.Add(heightPictureBox);
            Name = "MainWindow";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)heightPictureBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)flowPicturBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)riverPictureBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericRiverSpacingX).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericRiverSpacingY).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox heightPictureBox;
        private PictureBox flowPicturBox;
        private PictureBox riverPictureBox;
        private Button genFlowButton;
        private Button genManyRiverButton;
        private NumericUpDown numericRiverSpacingX;
        private NumericUpDown numericRiverSpacingY;
        private Label xRiverSpacing;
        private Label label1;
    }
}