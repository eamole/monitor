namespace Monitor
{
    partial class Form1
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
            this.label1 = new System.Windows.Forms.Label();
            this.wb = new System.Windows.Forms.WebBrowser();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnSelectDatabase = new System.Windows.Forms.Button();
            this.ReadAdoDbData = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "RM Plus Monitor";
            // 
            // wb
            // 
            this.wb.Location = new System.Drawing.Point(26, 41);
            this.wb.MinimumSize = new System.Drawing.Size(20, 20);
            this.wb.Name = "wb";
            this.wb.Size = new System.Drawing.Size(734, 378);
            this.wb.TabIndex = 1;
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(325, 4);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(107, 23);
            this.btnConnect.TabIndex = 2;
            this.btnConnect.Text = "Connect to Db";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnSelectDatabase
            // 
            this.btnSelectDatabase.Location = new System.Drawing.Point(144, 4);
            this.btnSelectDatabase.Name = "btnSelectDatabase";
            this.btnSelectDatabase.Size = new System.Drawing.Size(175, 23);
            this.btnSelectDatabase.TabIndex = 3;
            this.btnSelectDatabase.Text = "Select RM Database";
            this.btnSelectDatabase.UseVisualStyleBackColor = true;
            this.btnSelectDatabase.Click += new System.EventHandler(this.btnSelectDatabase_Click);
            // 
            // ReadAdoDbData
            // 
            this.ReadAdoDbData.Location = new System.Drawing.Point(438, 4);
            this.ReadAdoDbData.Name = "ReadAdoDbData";
            this.ReadAdoDbData.Size = new System.Drawing.Size(107, 23);
            this.ReadAdoDbData.TabIndex = 4;
            this.ReadAdoDbData.Text = "Read AdoDb Data";
            this.ReadAdoDbData.UseVisualStyleBackColor = true;
            this.ReadAdoDbData.Click += new System.EventHandler(this.ReadAdoDbData_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.ReadAdoDbData);
            this.Controls.Add(this.btnSelectDatabase);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.wb);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.WebBrowser wb;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnSelectDatabase;
        private System.Windows.Forms.Button ReadAdoDbData;
    }
}

