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
      this.createDummyDataButton = new System.Windows.Forms.Button();
      this.OutputTextBox = new System.Windows.Forms.TextBox();
      this.importIPDataSQLButton = new System.Windows.Forms.Button();
      this.createSchemaButton = new System.Windows.Forms.Button();
      this.createSpecialUsersButton = new System.Windows.Forms.Button();
      this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
      this.testButton = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // createDummyDataButton
      // 
      this.createDummyDataButton.Location = new System.Drawing.Point(318, 12);
      this.createDummyDataButton.Name = "createDummyDataButton";
      this.createDummyDataButton.Size = new System.Drawing.Size(122, 23);
      this.createDummyDataButton.TabIndex = 0;
      this.createDummyDataButton.Text = "Create Dummy Data";
      this.createDummyDataButton.UseVisualStyleBackColor = true;
      this.createDummyDataButton.Click += new System.EventHandler(this.createDummyDataButton_Click);
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
      this.importIPDataSQLButton.Location = new System.Drawing.Point(446, 12);
      this.importIPDataSQLButton.Name = "importIPDataSQLButton";
      this.importIPDataSQLButton.Size = new System.Drawing.Size(146, 23);
      this.importIPDataSQLButton.TabIndex = 3;
      this.importIPDataSQLButton.Text = "Import IP Data SQL";
      this.importIPDataSQLButton.UseVisualStyleBackColor = true;
      this.importIPDataSQLButton.Click += new System.EventHandler(this.importIPDataSQLButton_Click);
      // 
      // createSchemaButton
      // 
      this.createSchemaButton.Location = new System.Drawing.Point(12, 12);
      this.createSchemaButton.Name = "createSchemaButton";
      this.createSchemaButton.Size = new System.Drawing.Size(110, 23);
      this.createSchemaButton.TabIndex = 4;
      this.createSchemaButton.Text = "Create Schema";
      this.createSchemaButton.UseVisualStyleBackColor = true;
      this.createSchemaButton.Click += new System.EventHandler(this.createSchemaButton_Click);
      // 
      // createSpecialUsersButton
      // 
      this.createSpecialUsersButton.Location = new System.Drawing.Point(128, 12);
      this.createSpecialUsersButton.Name = "createSpecialUsersButton";
      this.createSpecialUsersButton.Size = new System.Drawing.Size(184, 23);
      this.createSpecialUsersButton.TabIndex = 5;
      this.createSpecialUsersButton.Text = "Create Special Users and Roles";
      this.createSpecialUsersButton.UseVisualStyleBackColor = true;
      this.createSpecialUsersButton.Click += new System.EventHandler(this.createSpecialUsersButton_Click);
      // 
      // backgroundWorker1
      // 
      this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
      this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
      // 
      // testButton
      // 
      this.testButton.Location = new System.Drawing.Point(598, 12);
      this.testButton.Name = "testButton";
      this.testButton.Size = new System.Drawing.Size(110, 23);
      this.testButton.TabIndex = 6;
      this.testButton.Text = "Test";
      this.testButton.UseVisualStyleBackColor = true;
      this.testButton.Click += new System.EventHandler(this.testButton_Click);
      // 
      // SchemaGen
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(751, 300);
      this.Controls.Add(this.testButton);
      this.Controls.Add(this.createSpecialUsersButton);
      this.Controls.Add(this.createSchemaButton);
      this.Controls.Add(this.importIPDataSQLButton);
      this.Controls.Add(this.OutputTextBox);
      this.Controls.Add(this.createDummyDataButton);
      this.Name = "SchemaGen";
      this.Text = "SchemaGen";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button createDummyDataButton;
    private System.Windows.Forms.TextBox OutputTextBox;
    private System.Windows.Forms.Button importIPDataSQLButton;
    private System.Windows.Forms.Button createSchemaButton;
    private System.Windows.Forms.Button createSpecialUsersButton;
    private System.ComponentModel.BackgroundWorker backgroundWorker1;
    private System.Windows.Forms.Button testButton;
  }
}

