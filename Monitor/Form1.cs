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

namespace Monitor
{
    public partial class Form1 : Form
    {
        string filename;
        Db db;

        public Form1()
        {
            InitializeComponent();

            filename = Properties.Settings.Default.DatabaseFilename;

            wb.DocumentText =
                "<html><body>Please enter your name:<br/>" +
                "<input type='text' name='userName'/><br/>" +
                "<a href='http://www.microsoft.com'>continue</a>" +
                "</body></html>";

            wb.Navigating +=
                new WebBrowserNavigatingEventHandler(wb_Navigating);
        }

        private void wb_Navigating(object sender,
                   WebBrowserNavigatingEventArgs e)
        {
            System.Windows.Forms.HtmlDocument document =
                wb.Document;

            //if (document != null && document.all["username"] != null &&
            //    string.isnullorempty(
            //    document.all["username"].getattribute("value")))
            //{
            //    e.cancel = true;
            //    system.windows.forms.messagebox.show(
            //        "you must enter your name before you can navigate to " +
            //        e.url.tostring());
            //}
        }

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

                    wb.DocumentText = "Using Database " + filename;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            db = new Db();
            db.tablesToJson();
        }
    }
}
