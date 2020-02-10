namespace drawingBoard.Drawing.Constants.Render {
	partial class MainForm {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.mainPictureBox = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize) (this.mainPictureBox)).BeginInit();
			this.SuspendLayout();
			// 
			// mainPictureBox
			// 
			this.mainPictureBox.BackColor = System.Drawing.Color.White;
			this.mainPictureBox.Location = new System.Drawing.Point(0, 0);
			this.mainPictureBox.Name = "mainPictureBox";
			this.mainPictureBox.Size = new System.Drawing.Size(121, 73);
			this.mainPictureBox.TabIndex = 0;
			this.mainPictureBox.TabStop = false;
			this.mainPictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.mainPictureBox_Paint);
			this.mainPictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mainPictureBox_MouseDown);
			this.mainPictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.mainPictureBox_MouseUp);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(120, 73);
			this.Controls.Add(this.mainPictureBox);
			this.Name = "MainForm";
			this.Text = "Program";
			this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.MainForm_KeyPress);
			((System.ComponentModel.ISupportInitialize) (this.mainPictureBox)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox mainPictureBox;
	}
}

