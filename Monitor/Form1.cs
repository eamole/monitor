using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Globalization;
using System.Timers;

namespace Monitor
{
    public partial class Form1 : Form
    {
        string filename;
        Db db;
        Snapshot snapshot;
        System.Timers.Timer timer;

        string tableName;
        DataTable tableData;

        public Form1()
        {
            InitializeComponent();


            filename = Properties.Settings.Default.DatabaseFilename;
            Connect();

            //wb.DocumentText =
            //    "<html><body>Please enter your name:<br/>" +
            //    "<input type='text' name='userName'/><br/>" +
            //    "<a href='http://www.microsoft.com'>continue</a>" +
            //    "</body></html>";

            //wb.Navigating +=
            //    new WebBrowserNavigatingEventHandler(wb_Navigating);
        }

        //private void wb_Navigating(object sender,
        //           WebBrowserNavigatingEventArgs e)
        //{
        //    System.Windows.Forms.HtmlDocument document =
        //        wb.Document;

        //    //if (document != null && document.all["username"] != null &&
        //    //    string.isnullorempty(
        //    //    document.all["username"].getattribute("value")))
        //    //{
        //    //    e.cancel = true;
        //    //    system.windows.forms.messagebox.show(
        //    //        "you must enter your name before you can navigate to " +
        //    //        e.url.tostring());
        //    //}
        //}

        private void btnSelectDatabase_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = filename;
            openFileDialog1.Filter = "Access Database files (*.mdb)|*.mdb";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    filename = openFileDialog1.FileName;
                    Properties.Settings.Default.DatabaseFilename = filename;
                    Properties.Settings.Default.Save();
                    txtDatabase.Text = filename;

                    //wb.DocumentText = "Using Database " + filename;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            // the primary Db has a connection to the snapshot db for timings
            //db = new Db( null , new Db("E:\\ResMgr\\amdex\\resmanager_snapshot.mdb"));
            //db.tablesToJson();
            Connect();            
        }

        private void Connect()
        {
            txtDatabase.Text = Properties.Settings.Default.DatabaseFilename;
            snapshot = Snapshot.getInstance(Properties.Settings.Default.DatabaseFilename);
            db = snapshot.originalDb;

            App.snapshotDb = snapshot;
            App.originalDb = db;

            dgvTables.DataSource = snapshot.fromSnapshot();
            //dgvTables.DataSource = snapshot.boundTables();

            //timer = new System.Timers.Timer(5 * App.queryTimerInterval);
            //timer.Elapsed += OnTimedEvent;
            //timer.AutoReset = true;
            //timer.Start();

        }

        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            App.log("refresh Tables");
            // this is throwing an error
            // dgvTables.Refresh();
            dgvTables.DataSource = snapshot.fromSnapshot();
            dgvTables.Update();
        }

        private void ReadAdoDbData_Click(object sender, EventArgs e)
        {
            snapshot.fromAdoDb();


        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void btnReadMetadata_Click(object sender, EventArgs e)
        {
            dgvTables.DataSource = snapshot.fromSnapshot();
        }

        private void dgvTables_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // need the table name
            int index = dgvTables.SelectedCells[0].RowIndex;

            DataGridViewRow selectedRow = dgvTables.Rows[index];

            tableName = selectedRow.Cells["name"].Value.ToString();
            txtTablename.Text = tableName;
            tableData = null;

            DataTable fields = Table.getFields(tableName);

            dgvFields.DataSource = fields;

            tabs.SelectedTab = tabFields;

            dgvStats.DataSource = Table.getFieldStats(tableName);

        }

        private void btnComputeFieldStats_Click(object sender, EventArgs e)
        {
            snapshot.computeFieldStats();
        }

        private void tabData_GotFocus(Object sender, EventArgs e)
        {
            Console.WriteLine("data selected");
        }

        private void dgvData_GotFocus(object sender, DataGridViewCellEventArgs e)
        {

        }


        private void tabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabs.SelectedTab == tabData && tableName !=null)
            {
                if (tableData == null)
                {
                    tableData = Table.getData(snapshot, tableName);
                    dgvData.DataSource = tableData;
                }
            }
        }

        private void btnGetFieldStats_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnTakeDataSnapshot_Click(object sender, EventArgs e)
        {
            //if(tableName != null)
            //{
            //    Table.checkForInserts(snapshot, tableName);
            //}
        }

        private void btnAddKeys_Click(object sender, EventArgs e)
        {
            snapshot.insertIdFields();
        }

        private void btnDeleteKeys_Click(object sender, EventArgs e)
        {
            if (tableName != null)
            {
                Table.deleteIdField(snapshot, tableName);
            }
        }

        private void btnGenFieldLists_Click(object sender, EventArgs e)
        {
            //if (tableName != null)
            //{
            //    Table.genFieldLists(tableName);
            //}
            snapshot.genFieldLists();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (tableName != null)
            {
                // snapshot.tables[tableName].getNewRecordIds();
                Table.checkForInserts(1,tableName);
                //   Table.genFieldLists(tableName);
            }
        }

        private void btnStartMonitor_Click(object sender, EventArgs e)
        {
            //if (tableName != null)
            //{
            //    // snapshot.tables[tableName].getNewRecordIds();
            //    Table.genJson(tableName);
            //    //   Table.genFieldLists(tableName);
            //}
            Queue.loadQueues();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Snapshot.checkForDeltas();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Table table = snapshot.tables["Agents"];
            // [inactive]  & chr(124) &  [account_agent]  & chr(124) &  [active]  & chr(124) &  [agent_name]  & chr(124) &  [direct_debit]  & chr(124) &  [language]  & chr(124) &  [Override_Commission]  & chr(124) &  [override_terms]  & chr(124) &  [use_ticket_stock]  & chr(124) &  [use_voucher_stock]  & chr(124) &  [Vat_Liable]  & chr(124) &  [Direct_Booking]  & chr(124) &  [agent_id]  & chr(124) &  [comm_band]  & chr(124) &  [Address]  & chr(124) &  Str( [Preferred_Contact_Method] ) & chr(124) &  [Street]  & chr(124) &  [agent_tel]  & chr(124) &  [City]  & chr(124) &  [agent_email]  & chr(124) &  [Country]  & chr(124) &  [mobile_no]  & chr(124) &  [agent_fax]  & chr(124) &  [bank_name]  & chr(124) &  [Region]  & chr(124) &  [sales_initial]  & chr(124) &  [sales_title]  & chr(124) &  [sales_surname]  & chr(124) &  [web_site_address]  & chr(124) &  [Postal_Code] 
            string values = "0|-1|0|RJA|0|Eng|-1|0|0|0|-1|0|RJA|02|Floor 2, 3 ,4| 1|Dunbar House, 14 Georges Quay|4703299|Cork|andrew.duggan@rja.ie|Ireland|086 6070038|4703298|||||||";
            string json = JSON.encode(table.parseSnapshotFieldListValues(values, 1));
            System.IO.File.WriteAllText(App.deltaPath + "\\delta.json", json);
        }
    }
}
