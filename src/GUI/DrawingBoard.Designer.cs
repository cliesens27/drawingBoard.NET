namespace drawingBoard.GUI {
	partial class DrawingBoard {
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
			((System.ComponentModel.ISupportInitialize)(this.mainPictureBox)).BeginInit();
			this.SuspendLayout();
			// 
			// mainPictureBox
			// 
			this.mainPictureBox.Location = new System.Drawing.Point(0, 0);
			this.mainPictureBox.Name = "mainPictureBox";
			this.mainPictureBox.Size = new System.Drawing.Size(100, 50);
			this.mainPictureBox.TabIndex = 0;
			this.mainPictureBox.TabStop = false;
			this.mainPictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.mainPictureBox_Paint);
			// 
			// DrawingBoard
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.mainPictureBox);
			this.Name = "DrawingBoard";
			this.Text = "Program";
			((System.ComponentModel.ISupportInitialize)(this.mainPictureBox)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox mainPictureBox;
	}
}

