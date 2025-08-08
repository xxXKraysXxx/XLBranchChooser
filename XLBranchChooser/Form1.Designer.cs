namespace XLBranchChooser
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
            label1 = new Label();
            label2 = new Label();
            save_button = new Button();
            textBoxKind = new TextBox();
            textBoxKey = new TextBox();
            pictureBox = new PictureBox();
            inject_button = new Button();
            groupBox1 = new GroupBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(334, 12);
            label1.Name = "label1";
            label1.Size = new Size(102, 15);
            label1.TabIndex = 1;
            label1.Text = "DalamudBetaKind";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(334, 75);
            label2.Name = "label2";
            label2.Size = new Size(97, 15);
            label2.TabIndex = 2;
            label2.Text = "DalamudBetaKey";
            // 
            // save_button
            // 
            save_button.Location = new Point(446, 357);
            save_button.Name = "save_button";
            save_button.Size = new Size(133, 50);
            save_button.TabIndex = 3;
            save_button.Text = "Сохранить и выйти";
            save_button.UseVisualStyleBackColor = true;
            save_button.Click += save_button_Click;
            // 
            // textBoxKind
            // 
            textBoxKind.Location = new Point(334, 30);
            textBoxKind.Name = "textBoxKind";
            textBoxKind.Size = new Size(245, 23);
            textBoxKind.TabIndex = 4;
            textBoxKind.KeyDown += textBoxKind_KeyDown;
            // 
            // textBoxKey
            // 
            textBoxKey.Location = new Point(334, 93);
            textBoxKey.Name = "textBoxKey";
            textBoxKey.Size = new Size(245, 23);
            textBoxKey.TabIndex = 5;
            textBoxKey.KeyDown += textBoxKey_KeyDown;
            // 
            // pictureBox
            // 
            pictureBox.Image = Properties.Resources.hamster_wheel;
            pictureBox.Location = new Point(334, 136);
            pictureBox.Name = "pictureBox";
            pictureBox.Size = new Size(245, 200);
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.TabIndex = 7;
            pictureBox.TabStop = false;
            // 
            // inject_button
            // 
            inject_button.Location = new Point(307, 357);
            inject_button.Name = "inject_button";
            inject_button.Size = new Size(133, 23);
            inject_button.TabIndex = 8;
            inject_button.Text = "INJECT РУКАМИ";
            inject_button.UseVisualStyleBackColor = true;
            inject_button.Click += inject_button_Click;
            // 
            // groupBox1
            // 
            groupBox1.Location = new Point(12, 16);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(204, 320);
            groupBox1.TabIndex = 10;
            groupBox1.TabStop = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(591, 414);
            Controls.Add(groupBox1);
            Controls.Add(inject_button);
            Controls.Add(pictureBox);
            Controls.Add(textBoxKey);
            Controls.Add(textBoxKind);
            Controls.Add(save_button);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "Form1";
            Text = "Hampter...";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label label1;
        private Label label2;
        private Button save_button;
        private TextBox textBoxKind;
        private TextBox textBoxKey;
        private PictureBox pictureBox;
        private Button inject_button;
        private GroupBox groupBox1;
    }
}
