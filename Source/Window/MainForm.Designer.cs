namespace DrawingBoardNET.Window
{
	partial class MainForm
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
            mainPictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize) mainPictureBox).BeginInit();
            SuspendLayout();
            // 
            // mainPictureBox
            // 
            mainPictureBox.BackColor = System.Drawing.Color.White;
            mainPictureBox.Location = new System.Drawing.Point(0, 0);
            mainPictureBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            mainPictureBox.Name = "mainPictureBox";
            mainPictureBox.Size = new System.Drawing.Size(141, 84);
            mainPictureBox.TabIndex = 0;
            mainPictureBox.TabStop = false;
            mainPictureBox.Paint += mainPictureBox_Paint;
            mainPictureBox.MouseDown += mainPictureBox_MouseDown;
            mainPictureBox.MouseUp += mainPictureBox_MouseUp;
            mainPictureBox.MouseWheel += mainPictureBox_MouseWheel;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.SystemColors.ControlDarkDark;
            ClientSize = new System.Drawing.Size(140, 84);
            Controls.Add(mainPictureBox);
            ForeColor = System.Drawing.Color.White;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "MainForm";
            Text = "Program";
            KeyDown += MainForm_KeyDown;
            KeyUp += MainForm_KeyUp;
            ((System.ComponentModel.ISupportInitialize) mainPictureBox).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.PictureBox mainPictureBox;
	}
}

