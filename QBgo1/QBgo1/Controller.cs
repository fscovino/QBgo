using System;
using System.Collections.Generic;
using System.Timers;
using System.Xml;
using System.IO;

namespace QBgo1
{
    public class Controller
    {
        private FormMain view;
        private Timer timer;
		private QBParser qb;

        // Constructor
        public Controller()
        {
			qb = new QBParser ();
        }

        // Load Preferences saved on settings.xml file
        public string[] getPreferences()
        {
            string[] settings = new string[4];
            XmlDocument xmlDoc;
            XmlNodeList nodes;

            try
            {
                xmlDoc = new XmlDocument();
                xmlDoc.Load("settings.xml");
                nodes = xmlDoc.GetElementsByTagName("data");

                foreach (XmlNode node in nodes)
                {
                    settings[0] = node.SelectSingleNode("qbFile").InnerText;
                    settings[1] = node.SelectSingleNode("ordersFolder").InnerText;
                    settings[2] = node.SelectSingleNode("inventoryFolder").InnerText;
                    settings[3] = node.SelectSingleNode("refreshRate").InnerText; 
                }
            }
            finally
            {
                xmlDoc = null;
                nodes = null;
            }

            return settings;
        }

        // Save Preferences to settings.xml file
        public void savePreferences(string[] settings)
        {
            XmlWriter writer = null;
            try
            {
                // Create an XmlWriterSettings object with the correct options.
                XmlWriterSettings xmlSettings = new XmlWriterSettings();
                xmlSettings.Indent = true;
                xmlSettings.IndentChars = ("\t");
                xmlSettings.OmitXmlDeclaration = true;

                // Create the XmlWriter object and write data
                writer = XmlWriter.Create("settings.xml", xmlSettings);
                writer.WriteStartElement("data");
                writer.WriteElementString("qbFile", settings[0]);
                writer.WriteElementString("ordersFolder", settings[1]);
                writer.WriteElementString("inventoryFolder", settings[2]);
                writer.WriteElementString("refreshRate", settings[3]);
                writer.WriteEndElement();
                writer.Flush();
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        // Subscribes a view to the controller
        public void subscribeView(FormMain view)
        {
            this.view = view;
        }

        // Start Service
        public void startService(int minutes)
        {
            // Create a timer
            timer = new Timer(minutes * 60 * 1000);

            // Hook up elapse event
            timer.Elapsed += RunProcesses;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        // Run Service Processes
        private void RunProcesses(object sender, ElapsedEventArgs e)
        {
            timer.Stop();

			// Get the list of orders in folder
            string[] list = getOrdersList();

			// Process Orders batch if there is at least one order in folder
			if (list.Length > 0)
            	processOrdersBatch(list);

            // Update view informaction
            updateView("Svc Running. Updated " + e.SignalTime);

            timer.Start();
        }

        // Stop Service 
        public void stopService()
        {
            timer.Stop();
            timer.Dispose();
        }

        // Update View
        private void updateView(string message)
        {
            view.updateView(message);
        }

		// Load the list of files from selected folder
		private string[] getOrdersList()
		{
			// Get orders folder path from view
			string path = view.getOrdersFolder();

			// Get all files from selected folder
			string[] fileList = Directory.GetFiles(path);

			// Verify each order's extension to only process .csv files
			List<string> orderList = new List<string>();

			foreach (string item in fileList) 
			{
				if (item.ToLower ().EndsWith (".csv")) 
				{
					orderList.Add (item);
				}
			}
			return orderList.ToArray ();
		}

		// Process all orders pending one by one
		private void processOrdersBatch(string[] ordersList)
		{
			string qbFile = view.getQbFile();						// Quickbooks File
			string ticket = "";										// Ticket obtained when open qb session

            // Begin Quickbooks Session
            ticket = qb.openSession(qbFile);

			// Make sure connection was succcesfull and there is a ticket
			if (ticket != "") 
			{
				// Obtain validation data Request
				string valRequest = qb.getXmlValidationDataRq();

				// Submit validation data Request
				string valReponse = qb.submitRequest (ticket, valRequest);

                // Convert validation data into an array
                string[] valData = qb.xml2arrayValidationData (valReponse);

				// Revise and Convert the order files into a xml request
				string request = qb.getXmlEstimatesAddRq(ordersList, valData);

				// Submit request only if is not empty
				if (request != "")
				{
                    // Close previous session and get a new ticket
                    ticket = qb.issueNewSessionTicket(qbFile, ticket);
					string response = qb.submitRequest(ticket, request);

                    // If there is no response (response = "") qb should have encountered an
                    // exception and close session and connection in the previous statement
                    if (response != "")
                    {
                        // End Quickbooks Session
                        qb.endSession(ticket);

                        // Close Quickbooks Connection
                        qb.closeConnection();
                    }
				}	
			}
        }

		// Export a.csv Inventory Report
		public int exportInventoryReport(string qbFile, string folder)
		{
			string ticket = "";
			int count = 0;

			// Begin Quickbooks Session
			ticket = qb.openSession(qbFile);

			// Make sure connection was succcesfull and there is a ticket
			if (ticket != "") 
			{
				// Convert the file into a xml request
				string request = qb.getXmlInventoryRq();
				// Submit request and obtained xml response
				string response = qb.submitRequest(ticket, request);
				// End Quickbooks Session
				qb.endSession(ticket);
				// Close Quickbooks Connection
				qb.closeConnection();
                // Export response to a .csv file
                count = qb.xml2csvInventory(response, folder);
			}

			return count;
		}

		// Export a .csv Sales Report
		public int exportSalesReport(string qbFile, string folder, string fromDate, string toDate)
		{
			string ticket = "";
			int count = 0;

			// Begin Quickbooks Session
			ticket = qb.openSession(qbFile);

			// Make sure connection was succcesfull and there is a ticket
			if (ticket != "") 
			{
				// Convert the file into a xml request
				string request = qb.getXmlSalesReportRq(fromDate, toDate);
				// Submit request and obtained xml response
				string response = qb.submitRequest(ticket, request);
				// End Quickbooks Session
				qb.endSession(ticket);
				// Close Quickbooks Connection
				qb.closeConnection();
				// Export response to a .csv file
				count = qb.xml2csvSalesReport(response, folder);
			}

			return count;
		}

		// Export a .csv Sales Report
		public int exportProductReport(string qbFile, string folder, string product, string fromDate)
		{
			string ticket = "";
			int count = 0;

			// Begin Quickbooks Session
			ticket = qb.openSession(qbFile);

			// Make sure connection was succcesfull and there is a ticket
			if (ticket != "") 
			{
                // Convert the file into a xml request
                string request = qb.getXmlProductReportRq(fromDate);
				// Submit request and obtained xml response
				string response = qb.submitRequest(ticket, request);
				// End Quickbooks Session
				qb.endSession(ticket);
				// Close Quickbooks Connection
				qb.closeConnection();
				// Export response to a .csv file
				count = qb.xml2csvProductReport (response, product, folder);
			}

			return count;
		}
    }
}
