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
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnSelectDatabase = new System.Windows.Forms.Button();
            this.btnReadAdoDbData = new System.Windows.Forms.Button();
            this.txtDatabase = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tabs = new System.Windows.Forms.TabControl();
            this.tabTables = new System.Windows.Forms.TabPage();
            this.dgvTables = new System.Windows.Forms.DataGridView();
            this.tabFields = new System.Windows.Forms.TabPage();
            this.dgvFields = new System.Windows.Forms.DataGridView();
            this.tabStats = new System.Windows.Forms.TabPage();
            this.dgvStats = new System.Windows.Forms.DataGridView();
            this.tabData = new System.Windows.Forms.TabPage();
            this.dgvData = new System.Windows.Forms.DataGridView();
            this.label3 = new System.Windows.Forms.Label();
            this.btnReadMetadata = new System.Windows.Forms.Button();
            this.btnComputeFieldStats = new System.Windows.Forms.Button();
            this.txtTablename = new System.Windows.Forms.TextBox();
            this.btnTakeDataSnapshot = new System.Windows.Forms.Button();
            this.btnAddKeys = new System.Windows.Forms.Button();
            this.btnGenFieldLists = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.tabs.SuspendLayout();
            this.tabTables.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTables)).BeginInit();
            this.tabFields.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFields)).BeginInit();
            this.tabStats.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStats)).BeginInit();
            this.tabData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).BeginInit();
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
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(126, 4);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(107, 23);
            this.btnConnect.TabIndex = 2;
            this.btnConnect.Text = "Connect to Db";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnSelectDatabase
            // 
            this.btnSelectDatabase.Location = new System.Drawing.Point(26, 33);
            this.btnSelectDatabase.Name = "btnSelectDatabase";
            this.btnSelectDatabase.Size = new System.Drawing.Size(129, 23);
            this.btnSelectDatabase.TabIndex = 3;
            this.btnSelectDatabase.Text = "Select RM Database";
            this.btnSelectDatabase.UseVisualStyleBackColor = true;
            this.btnSelectDatabase.Click += new System.EventHandler(this.btnSelectDatabase_Click);
            // 
            // btnReadAdoDbData
            // 
            this.btnReadAdoDbData.Location = new System.Drawing.Point(161, 33);
            this.btnReadAdoDbData.Name = "btnReadAdoDbData";
            this.btnReadAdoDbData.Size = new System.Drawing.Size(107, 23);
            this.btnReadAdoDbData.TabIndex = 4;
            this.btnReadAdoDbData.Text = "Read AdoDb Data";
            this.btnReadAdoDbData.UseVisualStyleBackColor = true;
            this.btnReadAdoDbData.Click += new System.EventHandler(this.ReadAdoDbData_Click);
            // 
            // txtDatabase
            // 
            this.txtDatabase.Location = new System.Drawing.Point(85, 57);
            this.txtDatabase.Name = "txtDatabase";
            this.txtDatabase.Size = new System.Drawing.Size(378, 20);
            this.txtDatabase.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Database";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // tabs
            // 
            this.tabs.Controls.Add(this.tabTables);
            this.tabs.Controls.Add(this.tabFields);
            this.tabs.Controls.Add(this.tabStats);
            this.tabs.Controls.Add(this.tabData);
            this.tabs.Location = new System.Drawing.Point(29, 83);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(736, 366);
            this.tabs.TabIndex = 7;
            this.tabs.SelectedIndexChanged += new System.EventHandler(this.tabs_SelectedIndexChanged);
            // 
            // tabTables
            // 
            this.tabTables.Controls.Add(this.dgvTables);
            this.tabTables.Location = new System.Drawing.Point(4, 22);
            this.tabTables.Name = "tabTables";
            this.tabTables.Padding = new System.Windows.Forms.Padding(3);
            this.tabTables.Size = new System.Drawing.Size(728, 340);
            this.tabTables.TabIndex = 0;
            this.tabTables.Text = "Tables";
            this.tabTables.UseVisualStyleBackColor = true;
            // 
            // dgvTables
            // 
            this.dgvTables.AllowUserToOrderColumns = true;
            this.dgvTables.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTables.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvTables.Location = new System.Drawing.Point(3, 3);
            this.dgvTables.Name = "dgvTables";
            this.dgvTables.Size = new System.Drawing.Size(722, 334);
            this.dgvTables.TabIndex = 0;
            this.dgvTables.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTables_CellContentClick);
            // 
            // tabFields
            // 
            this.tabFields.Controls.Add(this.dgvFields);
            this.tabFields.Location = new System.Drawing.Point(4, 22);
            this.tabFields.Name = "tabFields";
            this.tabFields.Padding = new System.Windows.Forms.Padding(3);
            this.tabFields.Size = new System.Drawing.Size(728, 340);
            this.tabFields.TabIndex = 1;
            this.tabFields.Text = "Fields";
            this.tabFields.UseVisualStyleBackColor = true;
            // 
            // dgvFields
            // 
            this.dgvFields.AllowUserToOrderColumns = true;
            this.dgvFields.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvFields.Location = new System.Drawing.Point(3, 3);
            this.dgvFields.Name = "dgvFields";
            this.dgvFields.Size = new System.Drawing.Size(722, 334);
            this.dgvFields.TabIndex = 0;
            // 
            // tabStats
            // 
            this.tabStats.Controls.Add(this.dgvStats);
            this.tabStats.Location = new System.Drawing.Point(4, 22);
            this.tabStats.Name = "tabStats";
            this.tabStats.Padding = new System.Windows.Forms.Padding(3);
            this.tabStats.Size = new System.Drawing.Size(728, 340);
            this.tabStats.TabIndex = 2;
            this.tabStats.Text = "Stats";
            this.tabStats.UseVisualStyleBackColor = true;
            // 
            // dgvStats
            // 
            this.dgvStats.AllowUserToOrderColumns = true;
            this.dgvStats.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvStats.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvStats.Location = new System.Drawing.Point(3, 3);
            this.dgvStats.Name = "dgvStats";
            this.dgvStats.Size = new System.Drawing.Size(722, 334);
            this.dgvStats.TabIndex = 0;
            // 
            // tabData
            // 
            this.tabData.AutoScroll = true;
            this.tabData.Controls.Add(this.dgvData);
            this.tabData.Location = new System.Drawing.Point(4, 22);
            this.tabData.Name = "tabData";
            this.tabData.Padding = new System.Windows.Forms.Padding(3);
            this.tabData.Size = new System.Drawing.Size(728, 340);
            this.tabData.TabIndex = 3;
            this.tabData.Text = "Data";
            this.tabData.UseVisualStyleBackColor = true;
            this.tabData.GotFocus += new System.EventHandler(this.tabData_GotFocus);
            // 
            // dgvData
            // 
            this.dgvData.AllowUserToOrderColumns = true;
            this.dgvData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvData.Location = new System.Drawing.Point(3, 3);
            this.dgvData.Name = "dgvData";
            this.dgvData.Size = new System.Drawing.Size(722, 334);
            this.dgvData.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(478, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Table";
            // 
            // btnReadMetadata
            // 
            this.btnReadMetadata.Location = new System.Drawing.Point(239, 4);
            this.btnReadMetadata.Name = "btnReadMetadata";
            this.btnReadMetadata.Size = new System.Drawing.Size(104, 23);
            this.btnReadMetadata.TabIndex = 9;
            this.btnReadMetadata.Text = "Read Metadata";
            this.btnReadMetadata.UseVisualStyleBackColor = true;
            this.btnReadMetadata.Click += new System.EventHandler(this.btnReadMetadata_Click);
            // 
            // btnComputeFieldStats
            // 
            this.btnComputeFieldStats.Location = new System.Drawing.Point(274, 33);
            this.btnComputeFieldStats.Name = "btnComputeFieldStats";
            this.btnComputeFieldStats.Size = new System.Drawing.Size(132, 23);
            this.btnComputeFieldStats.TabIndex = 11;
            this.btnComputeFieldStats.Text = "Compute Field Stats";
            this.btnComputeFieldStats.UseVisualStyleBackColor = true;
            this.btnComputeFieldStats.Click += new System.EventHandler(this.btnComputeFieldStats_Click);
            // 
            // txtTablename
            // 
            this.txtTablename.Location = new System.Drawing.Point(518, 57);
            this.txtTablename.Name = "txtTablename";
            this.txtTablename.Size = new System.Drawing.Size(182, 20);
            this.txtTablename.TabIndex = 12;
            this.txtTablename.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // btnTakeDataSnapshot
            // 
            this.btnTakeDataSnapshot.Location = new System.Drawing.Point(632, 33);
            this.btnTakeDataSnapshot.Name = "btnTakeDataSnapshot";
            this.btnTakeDataSnapshot.Size = new System.Drawing.Size(104, 23);
            this.btnTakeDataSnapshot.TabIndex = 13;
            this.btnTakeDataSnapshot.Text = "Data Snapshot";
            this.btnTakeDataSnapshot.UseVisualStyleBackColor = true;
            this.btnTakeDataSnapshot.Click += new System.EventHandler(this.btnTakeDataSnapshot_Click);
            // 
            // btnAddKeys
            // 
            this.btnAddKeys.Location = new System.Drawing.Point(412, 33);
            this.btnAddKeys.Name = "btnAddKeys";
            this.btnAddKeys.Size = new System.Drawing.Size(104, 23);
            this.btnAddKeys.TabIndex = 14;
            this.btnAddKeys.Text = "Add Keys";
            this.btnAddKeys.UseVisualStyleBackColor = true;
            this.btnAddKeys.Click += new System.EventHandler(this.btnAddKeys_Click);
            // 
            // btnGenFieldLists
            // 
            this.btnGenFieldLists.Location = new System.Drawing.Point(522, 33);
            this.btnGenFieldLists.Name = "btnGenFieldLists";
            this.btnGenFieldLists.Size = new System.Drawing.Size(104, 23);
            this.btnGenFieldLists.TabIndex = 15;
            this.btnGenFieldLists.Text = "Field Lists";
            this.btnGenFieldLists.UseVisualStyleBackColor = true;
            this.btnGenFieldLists.Click += new System.EventHandler(this.btnGenFieldLists_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(384, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(104, 23);
            this.button1.TabIndex = 16;
            this.button1.Text = "Test";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(494, 4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(104, 23);
            this.button2.TabIndex = 17;
            this.button2.Text = "Gen JSON";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnGenFieldLists);
            this.Controls.Add(this.btnAddKeys);
            this.Controls.Add(this.btnTakeDataSnapshot);
            this.Controls.Add(this.txtTablename);
            this.Controls.Add(this.btnComputeFieldStats);
            this.Controls.Add(this.btnReadMetadata);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tabs);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtDatabase);
            this.Controls.Add(this.btnReadAdoDbData);
            this.Controls.Add(this.btnSelectDatabase);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.tabs.ResumeLayout(false);
            this.tabTables.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTables)).EndInit();
            this.tabFields.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvFields)).EndInit();
            this.tabStats.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvStats)).EndInit();
            this.tabData.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnSelectDatabase;
        private System.Windows.Forms.Button btnReadAdoDbData;
        private System.Windows.Forms.TextBox txtDatabase;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.TabPage tabTables;
        private System.Windows.Forms.DataGridView dgvTables;
        private System.Windows.Forms.TabPage tabFields;
        private System.Windows.Forms.DataGridView dgvFields;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnReadMetadata;
        private System.Windows.Forms.Button btnComputeFieldStats;
        private System.Windows.Forms.TabPage tabStats;
        private System.Windows.Forms.DataGridView dgvStats;
        private System.Windows.Forms.TabPage tabData;
        private System.Windows.Forms.DataGridView dgvData;
        private System.Windows.Forms.TextBox txtTablename;
        private System.Windows.Forms.Button btnTakeDataSnapshot;
        private System.Windows.Forms.Button btnAddKeys;
        private System.Windows.Forms.Button btnGenFieldLists;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}

