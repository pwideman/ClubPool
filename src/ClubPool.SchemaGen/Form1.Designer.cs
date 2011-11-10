namespace ClubPool.SchemaGen
{
  partial class SchemaGen
  {
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
      this.OutputTextBox = new System.Windows.Forms.TextBox();
      this.importIPDataSQLButton = new System.Windows.Forms.Button();
      this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
      this.SuspendLayout();
      // 
      // OutputTextBox
      // 
      this.OutputTextBox.Location = new System.Drawing.Point(12, 41);
      this.OutputTextBox.Multiline = true;
      this.OutputTextBox.Name = "OutputTextBox";
      this.OutputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      this.OutputTextBox.Size = new System.Drawing.Size(727, 247);
      this.OutputTextBox.TabIndex = 1;
      // 
      // importIPDataSQLButton
      // 
      this.importIPDataSQLButton.Location = new System.Drawing.Point(12, 12);
      this.importIPDataSQLButton.Name = "importIPDataSQLButton";
      this.importIPDataSQLButton.Size = new System.Drawing.Size(146, 23);
      this.importIPDataSQLButton.TabIndex = 3;
      this.importIPDataSQLButton.Text = "Import IP Data";
      this.importIPDataSQLButton.UseVisualStyleBackColor = true;
      this.importIPDataSQLButton.Click += new System.EventHandler(this.importIPDataSQLButton_Click);
      // 
      // backgroundWorker1
      // 
      this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
      this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
      // 
      // SchemaGen
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(751, 300);
      this.Controls.Add(this.importIPDataSQLButton);
      this.Controls.Add(this.OutputTextBox);
      this.Name = "SchemaGen";
      this.Text = "SchemaGen";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox OutputTextBox;
    private System.Windows.Forms.Button importIPDataSQLButton;
    private System.ComponentModel.BackgroundWorker backgroundWorker1;
  }
}

