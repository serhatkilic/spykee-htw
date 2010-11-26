namespace SpykeeVision.Sandbox {
    partial class TestForm {
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
            this.SuspendLayout();
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.imageBox1 = new Emgu.CV.UI.ImageBox();
            this.ClientSize = new System.Drawing.Size(466, 453);
            this.Name = "TestForm";
            this.Text = "TestForm";
            this.ResumeLayout(false);

            // 
            // imageBox1
            // 
            this.imageBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imageBox1.Image = null;
            this.imageBox1.Location = new System.Drawing.Point(0, 52);
            this.imageBox1.Name = "imageBox1";
            this.imageBox1.Size = new System.Drawing.Size(520, 393);
            this.imageBox1.TabIndex = 5;
        }

        private Emgu.CV.UI.ImageBox imageBox1;
        #endregion
    }
}