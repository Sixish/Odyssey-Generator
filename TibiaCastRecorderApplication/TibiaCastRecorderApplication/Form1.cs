using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace TibiaCastRecorderApplication
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnAddToRead_Click(object sender, EventArgs e)
        {
            int i;
            for (i = lstIgnore.Items.Count - 1; i >= 0; i--)
            {
                if (lstIgnore.GetSelected(i))
                {
                    lstRead.Items.Add(lstIgnore.Items[i]);
                    lstIgnore.Items.RemoveAt(i);
                }
            }
        }

        private void btnAddToIgnore_Click(object sender, EventArgs e)
        {
            int i;
            for (i = lstRead.Items.Count - 1; i >= 0; i--)
            {
                if (lstRead.GetSelected(i))
                {
                    lstIgnore.Items.Add(lstRead.Items[i]);
                    lstRead.Items.RemoveAt(i);
                }
            }
        }

        private void chkSelectAllRead_CheckedChanged(object sender, EventArgs e)
        {
            int i, len = lstRead.Items.Count;
            bool isChecked = chkSelectAllRead.Checked;
            for (i = 0; i < len; i++)
            {
                lstRead.SetSelected(i, isChecked);
            }
        }

        private void chkSelectAllIgnore_CheckedChanged(object sender, EventArgs e)
        {
            int i, len = lstIgnore.Items.Count;
            bool isChecked = chkSelectAllIgnore.Checked;
            for (i = 0; i < len; i++)
            {
                lstIgnore.SetSelected(i, isChecked);
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            // Obtain the list of recordings.
            List<int> recordings = new List<int>();

            int val = 0;

            // Set the progress to 0.
            pbCompletion.Value = 0;
            // Allow the UI to update.
            Application.DoEvents();

            // Copy the items from the UI recording read list.
            for (int i = 0, len = lstRead.Items.Count; i < len; i++)
            {
                if (int.TryParse(lstRead.Items[i].ToString(), out val))
                {
                    recordings.Add(val);
                }
            }
            // If there are no recordings, output to status log and exit.
            if (recordings.Count < 1)
            {
                lstStatus.Items.Add("No recordings to read.");
                return;
            }

            // Output to status log the amount of recordings to be read.
            lstStatus.Items.Add("Reading " + recordings.Count.ToString() + " recordings.");

            // Iterate over all the recordings.
            for (int i = 0, len = recordings.Count; i < len; i++)
            {
                // Output to status log the recording being logged.
                lstStatus.Items.Add("Reading recording #" + recordings[i].ToString());

                // Read the recording and extract the packet data.
                // TODO

                // Output to status log that we are done with this recording.
                lstStatus.Items.Add("Finished reading recording.");

                // Update the completion percentage.
                pbCompletion.Value = (int)(100 * (i + 1) / recordings.Count);

                // Allow the UI to update.
                Application.DoEvents();
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            string path;
            if (dlgSelectFolder.ShowDialog() == DialogResult.OK)
            {
                path = dlgSelectFolder.SelectedPath;
                tbFolderPath.Text = path;
            }
        }

        private void tbFolderPath_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
