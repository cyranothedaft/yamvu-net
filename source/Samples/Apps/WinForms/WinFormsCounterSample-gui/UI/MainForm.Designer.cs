namespace WinFormsCounterSample.gui.UI {
   partial class MainForm {
      /// <summary>
      ///  Required designer variable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      /// <summary>
      ///  Clean up any resources being used.
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
      ///  Required method for Designer support - do not modify
      ///  the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent() {
         Program1GroupBox = new System.Windows.Forms.GroupBox();
         Program2GroupBox = new System.Windows.Forms.GroupBox();
         SuspendLayout();
         // 
         // Program1GroupBox
         // 
         Program1GroupBox.Location = new System.Drawing.Point(12, 12);
         Program1GroupBox.Name = "Program1GroupBox";
         Program1GroupBox.Size = new System.Drawing.Size(372, 361);
         Program1GroupBox.TabIndex = 0;
         Program1GroupBox.TabStop = false;
         Program1GroupBox.Text = "Program 1";
         // 
         // Program2GroupBox
         // 
         Program2GroupBox.Location = new System.Drawing.Point(399, 12);
         Program2GroupBox.Name = "Program2GroupBox";
         Program2GroupBox.Size = new System.Drawing.Size(372, 361);
         Program2GroupBox.TabIndex = 1;
         Program2GroupBox.TabStop = false;
         Program2GroupBox.Text = "Program 2";
         // 
         // MainForm
         // 
         AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
         AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         ClientSize = new System.Drawing.Size(785, 389);
         Controls.Add(Program2GroupBox);
         Controls.Add(Program1GroupBox);
         DoubleBuffered = true;
         FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
         MaximizeBox = false;
         Name = "MainForm";
         StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
         Text = "MainForm";
         ResumeLayout(false);
      }

      #endregion

      private System.Windows.Forms.GroupBox Program1GroupBox;
      private System.Windows.Forms.GroupBox Program2GroupBox;
   }
}
