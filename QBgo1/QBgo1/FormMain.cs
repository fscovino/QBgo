using System;
using System.Windows.Forms;

namespace QBgo1
{
    public partial class FormMain : Form
    {
        Controller controller;
        bool isServiceRunning;
        string[] settings;

        public FormMain(Controller controller)
        {
            InitializeComponent();
            this.controller = controller;
            controller.subscribeView(this);
            settings = new string[4];
            getPreferences();
            isServiceRunning = false;
        }

        private void btnQbFile_Click(object sender, EventArgs e)
        {
            DialogResult result = ofdQbFile.ShowDialog();
            
            if (result == DialogResult.OK)
            {
                txbQbFile.Text = ofdQbFile.FileName;
                setPreferences();
            }
        }

        private void btnOrdersFolder_Click(object sender, EventArgs e)
        {
            DialogResult result = fbdFolder.ShowDialog();

            if (result == DialogResult.OK)
            {
                txbOrdersFolder.Text = fbdFolder.SelectedPath;
                setPreferences();
            }
        }

        private void btnInvFolder_Click(object sender, EventArgs e)
        {
            DialogResult result = fbdFolder.ShowDialog();

            if (result == DialogResult.OK)
            {
                txbInvFolder.Text = fbdFolder.SelectedPath;
                setPreferences();
            }
        }

        private void cboRefreshRate_SelectedIndexChanged(object sender, EventArgs e)
        {
            setPreferences();
        }

        private void btnInventory_Click(object sender, EventArgs e)
        {
            int count = controller.exportInventoryReport(txbQbFile.Text, txbInvFolder.Text);
            MessageBox.Show(count + " Records Exported.", "QBgo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (isServiceRunning)
            {
                controller.stopService();
                isServiceRunning = false;
                this.Text = "QBgo - Svc Not Running";
                notifyIcon.Text = "QBgo - Svc Not Running";
                btnStart.Text = "Start";
                btnClose.Text = "Close";
            }
            else
            {
                controller.startService(Int16.Parse(cboRefreshRate.Text));
                isServiceRunning = true;
                this.Text = "QBgo - Svc Running";
                notifyIcon.Text = "QBgo - Svc Running";
                btnStart.Text = "Stop";
                btnClose.Text = "Hide";
            }
        }

        // Hidden method to access different reports
        private void lblQbFile_DoubleClick(object sender, EventArgs e)
        {
            FormDates frmDates = new FormDates(this.controller, txbQbFile.Text, txbInvFolder.Text);
            frmDates.Show();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (isServiceRunning)
            {
                this.Hide();
            }
            else
            {
                this.Close();
            }
        }

        private void getPreferences()
        {
            string[] settings = controller.getPreferences();
            txbQbFile.Text = settings[0];
            txbOrdersFolder.Text = settings[1];
            txbInvFolder.Text = settings[2];
            cboRefreshRate.Text = settings[3];
        }

        private void setPreferences()
        {
            settings[0] = txbQbFile.Text;
            settings[1] = txbOrdersFolder.Text;
            settings[2] = txbInvFolder.Text;
            settings[3] = cboRefreshRate.Text;
            controller.savePreferences(settings);
        }

		public string getQbFile()
		{
			return txbQbFile.Text;
		}

		public string getOrdersFolder()
		{
			return txbOrdersFolder.Text;
		}

        // Method to update form with service info
        // Securely called from another threat
        delegate void updateViewDelegate(string message);
        public void updateView(string message)
        {
            if (this.InvokeRequired)
            {
                updateViewDelegate d = new updateViewDelegate(updateView);
                this.Invoke(d, new object[] { message });
            }
            else
            {
                this.Text = "QBgo - " + message;
                notifyIcon.Text = "QBgo - " + message;
            }
        }

        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
        }
    }
}
