using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using QBXMLRP2Lib;

namespace QBgo1
{
    public class QBParser
    {
        private const string APP_CODE = "qbgo1";
        private const string APP_NAME = "QBgo";
        private RequestProcessor2 rp;

		// Constructor
        public QBParser()
        {
            
        }

		// Open Quickbooks Connection, Begin Session and return ticket
        public string openSession(string qbFile)
        {
			string ticket = "";

			try {
				//Obtain Request Processor Object
				rp = new RequestProcessor2();

				//Open Connection
				rp.OpenConnection(APP_CODE, APP_NAME);

				//Begin Session
				ticket = rp.BeginSession(qbFile, QBFileMode.qbFileOpenDoNotCare);

			} catch (Exception ex) {
				
				// Send Error to the Error Log
				string excep = DateTime.Now.ToString() + " [QBParser-openSession] " + ex.Message;
				File.AppendAllText("ErrorLog.txt", excep + Environment.NewLine);

				if (rp != null) 
				{
					rp.CloseConnection ();
					rp = null;
				}
			}
				
            return ticket;
        }

		// Submit request into Quickbooks and return response
		public string submitRequest(string ticket, string request)
		{
			string response = "";

			try {
				response = rp.ProcessRequest(ticket, request);

			} catch (Exception ex) {
				// Send Error to the Error Log
				string excep = DateTime.Now.ToString() + " [QBParser-submitRequest] " + ex.Message;
                File.AppendAllText("ErrorLog.txt", excep + Environment.NewLine);

                response = "";
				rp.EndSession (ticket);
				rp.CloseConnection ();
			}

			return response;
		}

        // End used Session and Issue a new one
        public string issueNewSessionTicket(string qbFile, string oldTicket)
        {
            String newTicket = "";

            try
            {
                // End Previous Used Session
                rp.EndSession(oldTicket);

                //Begin New Session
                newTicket = rp.BeginSession(qbFile, QBFileMode.qbFileOpenDoNotCare);
            }
            catch (Exception ex)
            {
				// Send Error to the Error Log
				string excep = DateTime.Now.ToString() + " [QBParser-issueNewSessionTicket] " + ex.Message;
                File.AppendAllText("ErrorLog.txt", excep + Environment.NewLine);

                if (rp != null)
                {
                    rp.CloseConnection();
                    rp = null;
                }
            }

            return newTicket;
        }

        // End Quickbooks Session
        public void endSession(string ticket)
        {
			rp.EndSession (ticket);
        }

		// Close Quickbooks Connection
		public void closeConnection()
		{
			rp.CloseConnection ();
		}

		// Build and return a xml validation data request string
		public string getXmlValidationDataRq()
		{
			//Create XML Docuemnt
			XmlDocument doc = new XmlDocument ();

			//Add prolog processing instructions
			doc.AppendChild (doc.CreateXmlDeclaration ("1.0", null, null));
			doc.AppendChild (doc.CreateProcessingInstruction ("qbxml", "version=\"7.0\""));

			//Add Main elements of the request
			XmlElement qbxml = doc.CreateElement ("QBXML");
			doc.AppendChild (qbxml);

			XmlElement qbXmlMsgsRq = doc.CreateElement ("QBXMLMsgsRq");
			qbxml.AppendChild (qbXmlMsgsRq);
			qbXmlMsgsRq.SetAttribute ("onError", "stopOnError");

			// Add a Customer Request to obtain Customer Code
			XmlElement customerQueryRq = doc.CreateElement ("CustomerQueryRq");
			qbXmlMsgsRq.AppendChild (customerQueryRq);

			// Add an Item Request to obtain Item Code
			XmlElement itemInventoryQueryRq = doc.CreateElement ("ItemInventoryQueryRq");
			qbXmlMsgsRq.AppendChild (itemInventoryQueryRq);

			return doc.OuterXml;
		}

		// Build and return a xml estimate request string for multiple files
		public string getXmlEstimatesAddRq(string[] files, string[] data)
		{
			List<Item> tempOrder;								// Temporary order to hold current processing file
			string row;                                 		// Complete line not splitted yet
			string[] itemField;                         		// Complete line splitted: [0] customer, [1] part#, [2] qty, [3] price
			StreamReader reader;								// File Reader
			StringBuilder sb;                                   // File Writer
            int count = 0;              						// Verified is there is at least one estimate to submit, also used to enumerate requestID

            // Create XML Docuemnt
            XmlDocument doc = new XmlDocument ();

			//Add prolog processing instructions
			doc.AppendChild (doc.CreateXmlDeclaration ("1.0", null, null));
			doc.AppendChild (doc.CreateProcessingInstruction ("qbxml", "version=\"7.0\""));

			//Add Main elements of the request
			XmlElement qbxml = doc.CreateElement ("QBXML");
			doc.AppendChild (qbxml);

			XmlElement qbXmlMsgsRq = doc.CreateElement ("QBXMLMsgsRq");
			qbxml.AppendChild (qbXmlMsgsRq);
			qbXmlMsgsRq.SetAttribute ("onError", "stopOnError");

            // Validate each file and include it on the request
            Item item;
			bool isOrderValid;
            XmlElement estimateAddRq;

            // Validate Orders one by one
            foreach (var file in files)
			{
				tempOrder = new List<Item>();	
				isOrderValid = true;
                
                try
                {
                    reader = new StreamReader(file);
                    do
                    {
                        row = reader.ReadLine();
                        if (row != "")
                        {
                            // Initialize Variables
                            item = new Item();
                            item.errorMessage = "Err: ";

                            // Separate line into columns
                            itemField = row.Split(',');

                            // Validate Customer Code Exists
                            item.CustomerCode = itemField[0].ToUpper();

							if (data.Contains(item.CustomerCode))
							{
								item.errorMessage += "0";
							}
							else
							{
								item.errorMessage += "1";
								isOrderValid = false;
							}

                            // Validate Part Number Exists
                            item.PartNumber = itemField[1].ToUpper();

							if (data.Contains(item.PartNumber))
							{
								item.errorMessage += "0";
							}
							else
							{
								item.errorMessage += "1";
								isOrderValid = false;
							}

                            // Validate Quantity
                            int n;
                            if (int.TryParse(itemField[2], out n))
                            {
                                item.Quantity = n;
                                item.errorMessage += "0";
                            }
                            else
                            {
                                item.Quantity = 0;
                                item.errorMessage += "1";
								isOrderValid = false;
                            }

                            // Price gets Validated somewhere else
                            item.Rate = itemField[3];
                            item.errorMessage += "0";

                            // Clear Error Message field on lines that are OK
                            item.errorMessage = item.errorMessage == "Err: 0000" ? "" : item.errorMessage;

                            // Add the item to a temporary list
                            tempOrder.Add(item);
                        }

                    } while (reader.Peek() != -1);

					// Close Stream Reader
					reader.Close();

                    // Include or Reject Order
					if (isOrderValid)
					{
						// Process a good order
                        count++;

                        // Add an Estimate Request node
                        estimateAddRq = doc.CreateElement("EstimateAddRq");
                        qbXmlMsgsRq.AppendChild(estimateAddRq);
                        estimateAddRq.SetAttribute("requestID", count.ToString());

                        XmlElement estimateAdd = doc.CreateElement("EstimateAdd");
                        estimateAddRq.AppendChild(estimateAdd);

                        XmlElement customerRef = doc.CreateElement ("CustomerRef");
                        estimateAdd.AppendChild (customerRef);
						customerRef.AppendChild (doc.CreateElement ("FullName")).InnerText = tempOrder[0].CustomerCode;

						// Add Product lines
						XmlElement estimateLineAdd;
						XmlElement itemRef;
						XmlElement priceLevelRef;

						foreach (var line in tempOrder) 
						{
							// Add new line of item
							estimateLineAdd = doc.CreateElement("EstimateLineAdd");
                            estimateAdd.AppendChild (estimateLineAdd);

							//Add Item PartNumber element
							itemRef = doc.CreateElement ("ItemRef");
							estimateLineAdd.AppendChild (itemRef);
							itemRef.AppendChild (doc.CreateElement ("FullName")).InnerText = line.PartNumber;

                            //Add Item Quantity element
                            estimateLineAdd.AppendChild(doc.CreateElement("Quantity")).InnerText = line.Quantity.ToString();

							//Condition to select the item rate
							if (line.Rate.ToLower().StartsWith ("pl:")) 
							{
								//Set the item rate to a specific Price Level value
								priceLevelRef = doc.CreateElement ("PriceLevelRef");
								estimateLineAdd.AppendChild (priceLevelRef);
								//Get the FullNAme of the price Level from the item eliminating the first 3 characters which are pl:
								priceLevelRef.AppendChild (doc.CreateElement ("FullName")).InnerText = line.Rate.Substring(3);
							} 
							else if (line.Rate.ToLower () == "cost" || line.Rate.ToLower () == "c" || line.Rate.ToLower () == "low" || line.Rate.ToLower () == "left" || line.Rate.ToLower () == "l") 
							{
								//Set the item rate to the Cost value (low, left)
								estimateLineAdd.AppendChild (doc.CreateElement ("MarkupRatePercent")).InnerText = "0";
							}
							else if (line.Rate.ToLower () == "" || line.Rate.ToLower () == "price" || line.Rate.ToLower () == "p" || line.Rate.ToLower () == "high" || line.Rate.ToLower () == "h" || line.Rate.ToLower () == "right" || line.Rate.ToLower () == "r") 
							{
								//Do nothing to set the price rate to the default Price Value (high, right)
							}
							else
							{
                                //Set the item rate to the value assigned on the order
                                estimateLineAdd.AppendChild (doc.CreateElement ("Rate")).InnerText = line.Rate;
                            }
						}

                        // Delete the file from folder
                        File.Delete(file);
                    }
					else
					{
						// Handle a bad order
						// Initilize the stream builder
						sb = new StringBuilder();

						// Array to hold items from one line
						string[] outLine = new string[5];

						// Process each line of the order
						foreach (var line in tempOrder)
						{
							outLine[0] = line.CustomerCode;
							outLine[1] = line.PartNumber;
							outLine[2] = line.Quantity.ToString();
							outLine[3] = line.Rate;
							outLine[4] = line.errorMessage;

							// Add line to the streambuider separated by a comma
							sb.AppendLine(string.Join(",", outLine));
						}

						// Save the whole stream "revised order" into a .csv file keeping the same name
						File.WriteAllText(file, sb.ToString());
						sb.Clear();
					}

                }
                catch (Exception ex)
                {
					// Send Error to the Error Log
					string excep = DateTime.Now.ToString() + " [QBParser-getXmlEstimatesAddRq] " + ex.Message;
                    File.AppendAllText("ErrorLog.txt", excep + Environment.NewLine);
                }

			}

			return count > 0 ? doc.OuterXml : "";
		}

		// Build and return a xml inventory request string
        public string getXmlInventoryRq()
        {
			//Create XML Docuemnt
			XmlDocument doc = new XmlDocument ();

			//Add prolog processing instructions
			doc.AppendChild (doc.CreateXmlDeclaration ("1.0", null, null));
			doc.AppendChild (doc.CreateProcessingInstruction ("qbxml", "version=\"7.0\""));

			//Add Main elements of the request
			XmlElement qbxml = doc.CreateElement ("QBXML");
			doc.AppendChild (qbxml);

			XmlElement qbXmlMsgsRq = doc.CreateElement ("QBXMLMsgsRq");
			qbxml.AppendChild (qbXmlMsgsRq);
			qbXmlMsgsRq.SetAttribute ("onError", "stopOnError");

			// Add an Item Inventory Query Request
			XmlElement itemInventoryQueryRq = doc.CreateElement ("ItemInventoryQueryRq");
			qbXmlMsgsRq.AppendChild (itemInventoryQueryRq);

            return doc.OuterXml;
        }

		// Build and return a xml sales report request string
		public string getXmlSalesReportRq(string fromDate, string toDate)
		{
			//Create XML Docuemnt
			XmlDocument doc = new XmlDocument ();

			//Add prolog processing instructions
			doc.AppendChild (doc.CreateXmlDeclaration ("1.0", null, null));
			doc.AppendChild (doc.CreateProcessingInstruction ("qbxml", "version=\"7.0\""));

			//Add Main elements of the request
			XmlElement qbxml = doc.CreateElement ("QBXML");
			doc.AppendChild (qbxml);

			XmlElement qbXmlMsgsRq = doc.CreateElement ("QBXMLMsgsRq");
			qbxml.AppendChild (qbXmlMsgsRq);
			qbXmlMsgsRq.SetAttribute ("onError", "stopOnError");

			// Add an Item Inventory Query Request
			XmlElement itemInventoryQueryRq = doc.CreateElement ("ItemInventoryQueryRq");
			qbXmlMsgsRq.AppendChild (itemInventoryQueryRq);

			// Add an Invoice Query Request
			XmlElement invoiceQueryRq = doc.CreateElement ("InvoiceQueryRq");
			qbXmlMsgsRq.AppendChild (invoiceQueryRq);

			// Add dates gap to the Invoices Request
			XmlElement txnDateRangeFilter = doc.CreateElement("TxnDateRangeFilter");
			invoiceQueryRq.AppendChild (txnDateRangeFilter);

			txnDateRangeFilter.AppendChild (doc.CreateElement ("FromTxnDate")).InnerText = fromDate;
			txnDateRangeFilter.AppendChild (doc.CreateElement ("ToTxnDate")).InnerText = toDate;

            // Include Line Items
            XmlElement includeLineItems = doc.CreateElement("IncludeLineItems");
            invoiceQueryRq.AppendChild(includeLineItems).InnerText = "1";

            return doc.OuterXml;
		}

		// Build and return a xml Product report request string
		public string getXmlProductReportRq(string fromDate)
		{
			//Create XML Docuemnt
			XmlDocument doc = new XmlDocument ();

			//Add prolog processing instructions
			doc.AppendChild (doc.CreateXmlDeclaration ("1.0", null, null));
			doc.AppendChild (doc.CreateProcessingInstruction ("qbxml", "version=\"7.0\""));

			//Add Main elements of the request
			XmlElement qbxml = doc.CreateElement ("QBXML");
			doc.AppendChild (qbxml);

			XmlElement qbXmlMsgsRq = doc.CreateElement ("QBXMLMsgsRq");
			qbxml.AppendChild (qbXmlMsgsRq);
			qbXmlMsgsRq.SetAttribute ("onError", "stopOnError");

			// Add an Item Inventory Query Request
			XmlElement purchaseOrderQueryRq = doc.CreateElement ("PurchaseOrderQueryRq");
			qbXmlMsgsRq.AppendChild (purchaseOrderQueryRq);

			// Add dates gap to the Pruchase Order Request
			XmlElement poDateRangeFilter = doc.CreateElement("TxnDateRangeFilter");
			purchaseOrderQueryRq.AppendChild (poDateRangeFilter);

            poDateRangeFilter.AppendChild (doc.CreateElement ("FromTxnDate")).InnerText = fromDate;
            poDateRangeFilter.AppendChild(doc.CreateElement("ToTxnDate")).InnerText = DateTime.Today.ToString ("yyyy-MM-dd");

			// Include Line Items
			XmlElement poIncludeLineItems = doc.CreateElement("IncludeLineItems");
			purchaseOrderQueryRq.AppendChild(poIncludeLineItems).InnerText = "1";

			// Add an Invoice Query Request
			XmlElement invoiceQueryRq = doc.CreateElement ("InvoiceQueryRq");
			qbXmlMsgsRq.AppendChild (invoiceQueryRq);

			// Add dates gap to the Invoices Request
			XmlElement invDateRangeFilter = doc.CreateElement("TxnDateRangeFilter");
			invoiceQueryRq.AppendChild (invDateRangeFilter);

            invDateRangeFilter.AppendChild(doc.CreateElement("FromTxnDate")).InnerText = fromDate;
            invDateRangeFilter.AppendChild(doc.CreateElement("ToTxnDate")).InnerText = DateTime.Today.ToString ("yyyy-MM-dd");

            // Include Line Items
            XmlElement invIncludeLineItems = doc.CreateElement("IncludeLineItems");
            invoiceQueryRq.AppendChild(invIncludeLineItems).InnerText = "1";

			return doc.OuterXml;
		}

		// Convert the xml data response into an array
		public string[] xml2arrayValidationData(string xmlData)
		{
			List<string> data = new List<string> ();

			//Create XML Document
			XmlDocument doc = new XmlDocument();

			try
			{
				doc.LoadXml(xmlData);

				// Process Item Code list
				//Create main node list
				XmlNodeList itemInventoryQueryRs = doc.GetElementsByTagName("ItemInventoryQueryRs");

				if (itemInventoryQueryRs.Count == 1)
				{
					//Get the Status value
					string status = itemInventoryQueryRs.Item(0).Attributes.GetNamedItem("statusCode").Value;

					//Read nodes if status is OK = 0
					if (Convert.ToInt32(status) == 0)
					{
						// Get All Items from the response
						XmlNodeList itemsList = itemInventoryQueryRs.Item(0).ChildNodes;

						// Add each item part number to the data list
						foreach (XmlNode item in itemsList)
						{
							data.Add(item.SelectSingleNode("FullName").InnerText.ToUpper());
						}
					}
				}

				// Process Customer Code list
				//Create main node list
				XmlNodeList customerQueryRs = doc.GetElementsByTagName("CustomerQueryRs");

				if (customerQueryRs.Count == 1)
				{
					//Get the Status value
					string status = customerQueryRs.Item(0).Attributes.GetNamedItem("statusCode").Value;

					//Read nodes if status is OK = 0
					if (Convert.ToInt32(status) == 0)
					{
						// Get All Customers from the response
						XmlNodeList customerList = customerQueryRs.Item(0).ChildNodes;

						// Add each Customer Code to the data list
						foreach (XmlNode customer in customerList)
						{
							data.Add(customer.SelectSingleNode("Name").InnerText.ToUpper());
						}
					}
				}
			}
			catch (Exception ex) 
			{
				// Send Error to the Error Log
				string excep = DateTime.Now.ToString() + " [QBParser-xml2arrayValidationData] " + ex.Message;
                File.AppendAllText("ErrorLog.txt", excep + Environment.NewLine);
            }

            return data.ToArray();
		}

		// Export an Inventory report to a .csv file
        public int xml2csvInventory(string response, string folder)
        {
			int count = 0;

            //Create XML Document
            XmlDocument doc = new XmlDocument();

            try
            {
                //Load the Xml response
                doc.LoadXml(response);

                //Create main node list
                XmlNodeList itemInventoryQueryRs = doc.GetElementsByTagName("ItemInventoryQueryRs");

                if (itemInventoryQueryRs.Count == 1)
                {

                    //Get the Status value
                    string status = itemInventoryQueryRs.Item(0).Attributes.GetNamedItem("statusCode").Value;

                    //Read nodes if status is OK = 0
                    if (Convert.ToInt32(status) == 0)
                    {
                        XmlNodeList itemsList = itemInventoryQueryRs.Item(0).ChildNodes;

                        string[] row = new string[13];
                        StringBuilder sb = new StringBuilder();

                        // Insert the first row with header
                        string[] header = Filter.headerInventoryReport();
                        sb.AppendLine(string.Join(",", header));

                        // Insert Item rows
                        foreach (XmlNode item in itemsList)
                        {

                            // Verify some nodes exist for each item to avoid null exception
                            string desc = item.SelectNodes("SalesDesc").Count == 1 ? item.SelectSingleNode("SalesDesc").InnerText : "";
                            string mpn = item.SelectNodes("ManufacturerPartNumber").Count == 1 ? item.SelectSingleNode("ManufacturerPartNumber").InnerText : "";

                            // Load the array with the current item data
                            row[0] = Filter.brand(desc);                                                            // Brand
                            row[1] = item.SelectSingleNode("FullName").InnerText;                                   // Part Number
                            row[2] = Filter.description(desc);                                                      // Description
                            row[3] = Filter.available(item.SelectSingleNode("QuantityOnHand").InnerText, item.SelectSingleNode("QuantityOnSalesOrder").InnerText);// Available
                            row[4] = item.SelectSingleNode("QuantityOnOrder").InnerText;                            // Coming
                            row[5] = Filter.image(item.SelectSingleNode("FullName").InnerText);                     // Image
                            row[6] = "0.00";                                                                        // Level 0
							row[7] = item.SelectSingleNode("PurchaseCost").InnerText;                               // Level 1
                            row[8] = item.SelectSingleNode("SalesPrice").InnerText;                                 // Level 2
                            row[9] = Filter.level3(item.SelectSingleNode("SalesPrice").InnerText);                  // Level 3
                            row[10] = mpn;                                                                          // MPN (upc)
							row[11] = item.SelectSingleNode("AverageCost").InnerText;
                            row[12] = item.SelectSingleNode("TimeModified").InnerText;

                            // Apend line into streambuilder
                            sb.AppendLine(string.Join(",", row));
                            count++;
                        }
                        // Export streambuilder to Inv_yyyymmdd.csv file
                        string fileName = "\\Inv_" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + ".csv";
                        File.WriteAllText(folder + fileName, sb.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
				// Send Error to the Error Log
				string excep = DateTime.Now.ToString() + " [QBParser-xml2csvInventory] " + ex.Message;
                File.AppendAllText("ErrorLog.txt", excep + Environment.NewLine);
            }

			return count;
        }

		// Export a sales report to a .csv file
        public int xml2csvSalesReport(string response, string folder)
        {
			List<string[]> productsList = new List<string[]> ();	// PartNumber, AvgCost
            List<string> salesRepsList= new List<string>();			// Sales Rep Names
			List<string[]> ordersList = new List<string[]> ();		// [0]SalesRep, [1]Customer, [2]invNumber [3]invDate, [4]Amount, [5]Profit, [6]#Items, [7]Percentage

			// Global Totals
            int count = 0;
			double globalAmount = 0.00;
			double globalProfit = 0.00;

			//Create XML Document
			XmlDocument doc = new XmlDocument();

			// Make a list of products with its Average Cost
			try
			{
				//Load the Xml response
				doc.LoadXml(response);

				// Make a list of products with its Average Cost
				//Create main node list
				XmlNodeList itemInventoryQueryRs = doc.GetElementsByTagName("ItemInventoryQueryRs");

				if (itemInventoryQueryRs.Count == 1)
				{
					//Get the Status value
					string status = itemInventoryQueryRs.Item(0).Attributes.GetNamedItem("statusCode").Value;

					//Read nodes if status is OK = 0
					if (Convert.ToInt32(status) == 0)
					{
						XmlNodeList itemsList = itemInventoryQueryRs.Item(0).ChildNodes;

                        string[] row;

						// Insert Item rows
						foreach (XmlNode item in itemsList)
						{
                            row = new string[2];
                            row[0] = item.SelectSingleNode("FullName").InnerText;
							row[1] = item.SelectSingleNode("AverageCost").InnerText;

							// Add Product to list
							productsList.Add(row);
						}
					}
				}

				// Read data from the Invoice Response
				//Create main node list
				XmlNodeList invoiceQueryRs = doc.GetElementsByTagName("InvoiceQueryRs");

				if (invoiceQueryRs.Count == 1)
				{
					//Get the Status value
					string status = invoiceQueryRs.Item(0).Attributes.GetNamedItem("statusCode").Value;

					//Read nodes if status is OK = 0
					if (Convert.ToInt32(status) == 0)
					{
						XmlNodeList invoiceList = invoiceQueryRs.Item(0).ChildNodes;

						// Build Sales Rep list from Invoices
                        string rep = "";

                        foreach (XmlNode invoice in invoiceList)
						{
                            // Add every Sales Rep from the invoice list to the sales rep list if is not already
                            // Verify Sales Rep nodes exist for each invoice to avoid null exception
                            rep = invoice.SelectNodes("SalesRepRef/FullName").Count == 1 ? invoice.SelectSingleNode("SalesRepRef/FullName").InnerText : "N/A";
                            if (!salesRepsList.Contains(rep))
                            {
                                salesRepsList.Add(rep);
                            }
						}
						// Sort the list alphabetically
						salesRepsList.Sort();

                        // Build Invoices on the order list
						// Temporal variables for Invoice
						string[] order;		                    // [0]SalesRep, [1]Customer, [2]invNumber, [3]invDate, [4]Amount, [5]Profit, [6]#Items, [7]Percentage
						double totAmount = 0.00;
						double totProfit = 0.00;
						double totItems = 0.00;
						// Temporal variables for ItemLine
						string name = "";
						double qty = 0.00;
						double rate = 0.00;
						double cost = 0.00;

                        foreach (XmlNode invoice in invoiceList)
                        {
                            // Clear data on temp variables
                            order = new string[8];
                            cost = 0.00;
                            totAmount = 0.00;
							totProfit = 0.00;
							totItems = 0;
							// Populate temporary order
							order[0] = invoice.SelectNodes("SalesRepRef/FullName").Count == 1 ? invoice.SelectSingleNode("SalesRepRef/FullName").InnerText : "N/A";
                            order[1] = invoice.SelectSingleNode("CustomerRef/FullName").InnerText;
							order[2] = invoice.SelectSingleNode("TxnDate").InnerText;
							order[3] = invoice.SelectSingleNode("RefNumber").InnerText;

							// Get the list of items from current invoice
							XmlNodeList itemsRetList = invoice.SelectNodes("InvoiceLineRet");
							// Totalize each row of the invoice
							foreach (XmlNode item in itemsRetList)
							{
                                name = item.SelectNodes("ItemRef/FullName").Count == 1 ? item.SelectSingleNode("ItemRef/FullName").InnerText : "";
                                qty = item.SelectNodes("Quantity").Count == 1 ? double.Parse(item.SelectSingleNode("Quantity").InnerText) : 0.00;
                                rate = item.SelectNodes("Rate").Count == 1 ? double.Parse(item.SelectSingleNode("Rate").InnerText) : 0.00;
                                // Look for the cost of the product on the product list
                                foreach (string[] product in productsList)
                                {
                                    if (product[0] == name)
                                    {
                                        cost = double.Parse(product[1]);
                                    }
                                }
                                // Add the values of each row to the totals
                                totAmount += qty * rate;
								totProfit += qty * (rate - cost);
                                totItems += qty;
							}
							order[4] = totAmount.ToString("F2");
							order[5] = totProfit.ToString("F2");
							order[6] = totItems.ToString("F2");
							order[7] = ((totProfit / (totAmount - totProfit)) * 100).ToString("F2");
							// Add the temporary order to the order list
							ordersList.Add(order);
							// Add Amount and Profit to the Global counters
							globalAmount += totAmount;
							globalProfit += totProfit;
                        }

						// Print the Report to a file ordered by sales rep
						// Create a string builder
						StringBuilder sb = new StringBuilder();

						// Temp variable to hold totals for a specific sales rep
						//[0]Empty, [1]Empty, [2]Empty, [3]Empty, [4] Amount, [5]Profit, [6] #Items, [7] Contribution
						double[] totalRep;
						string[] repFooter;

						foreach (string salesrep in salesRepsList)
						{
                            // Clear temp varibles
                            totalRep = new double[8];
                            repFooter = new string[8];
                            totalRep[4] = 0.00;
							totalRep[5] = 0.00;
							totalRep[6] = 0.00;
							totalRep[7] = 0.00;

							// Add Header
							string[] header = Filter.headerSalesReport();
							sb.AppendLine(string.Join(",", header));

							// Run all invoices to find the ones that match current sales rep
							foreach (string[] invoice in ordersList)
							{

								if (invoice[0] == salesrep)
								{
									// Apend line into stringmbuilder
									sb.AppendLine(string.Join(",", invoice));

									// Add invoice to the totals by rep
									totalRep[4] += double.Parse(invoice[4]);
									totalRep[5] += double.Parse(invoice[5]);
									totalRep[6] += double.Parse(invoice[6]);
									// Increment the count. Just to expose a number of invoices processed
									count++;
								}
							}
							//Calculate the sales rep's profit contribution compare to the global profit
							totalRep[7] = totalRep[5] / globalProfit;
							// Set Footer for sales rep
							repFooter[0] = "";
							repFooter[1] = "";
							repFooter[2] = "";
							repFooter[3] = "";
							repFooter[4] = totalRep[4].ToString("F2");
							repFooter[5] = totalRep[5].ToString("F2");
							repFooter[6] = totalRep[6].ToString("F2");
                            repFooter[7] = "Ptn: " + totalRep[7].ToString("P");
							// Add Totals by Rep into stringbuilder
							sb.AppendLine(string.Join(",", repFooter));
							sb.AppendLine();
						}
						// Add the Global Amount and Profit at the end of the report
						String[] globalTotals = new string[]{"", "", "", "", globalAmount.ToString("F2"), globalProfit.ToString("F2"), "", ""};
						// Add Global Totals to the stringbuilder
						sb.AppendLine(string.Join(",", globalTotals));
						// Export streambuilder to sr_yyyymmdd.csv file
						string fileName = "\\sr_" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + ".csv";
						File.WriteAllText(folder + fileName, sb.ToString());
					}
				}
			}
			catch (Exception ex)
			{
				// Send Error to the Error Log
				string excep = DateTime.Now.ToString() + " [QBParser-xml2csvSalesReport] " + ex.Message;
                File.AppendAllText("ErrorLog.txt", excep + Environment.NewLine);
            }

			return count;
        }

		// Export a Product report to a .csv file
		public int xml2csvProductReport(string response, string product, string folder)
		{
			List<string[]> purchaseLine = new List<string[]> ();	//[0]Date, [1]Vendor, [2]Quantity, [3]Price
			List<string[]> invoiceLine = new List<string[]> ();		//[0]Date, [1]Customer, [2]Quantity, [3]Price
			int count = 0;

			//Create XML Document
			XmlDocument doc = new XmlDocument();

			try {
				//Load the Xml response
				doc.LoadXml(response);

				// Make a list of purchase orders to filter the selected product from
				//Create main node list
				XmlNodeList purchaseOrderQueryRs = doc.GetElementsByTagName("PurchaseOrderQueryRs");

				if (purchaseOrderQueryRs.Count == 1)
				{
					//Get the Status value
					string status = purchaseOrderQueryRs.Item(0).Attributes.GetNamedItem("statusCode").Value;

					//Read nodes if status is OK = 0
					if (Convert.ToInt32(status) == 0)
					{
                        // Get all the purchase orders already filtered by date range
						XmlNodeList purchaseList = purchaseOrderQueryRs.Item(0).ChildNodes;

						string[] row;
						string item = "";

						// Run every purchase order at a time
						foreach (XmlNode purchase in purchaseList)
						{
							// Get the list of items from current purchase order
							XmlNodeList itemsRetList = purchase.SelectNodes("PurchaseOrderLineRet");
							// Filter the specific item from the purchase order
							foreach (XmlNode line in itemsRetList)
							{
								item = line.SelectNodes("ItemRef/FullName").Count == 1 ? line.SelectSingleNode("ItemRef/FullName").InnerText : "";
                                // Select only the lines that coontain the specific product
								if (item == product)
								{
									row = new string[4];
									row[0] = purchase.SelectSingleNode("TxnDate").InnerText;
									row[1] = purchase.SelectSingleNode("VendorRef/FullName").InnerText;
                                    row[2] = int.Parse(line.SelectSingleNode("Quantity").InnerText).ToString("D");
                                    row[3] = double.Parse(line.SelectSingleNode("Rate").InnerText).ToString("F2");
                                    // Add Product to list
                                    purchaseLine.Add(row);
								}
							}
						}
					}
				}

				// Make a list of invoices to filter the selected product from
				//Create main node list
				XmlNodeList invoiceQueryRs = doc.GetElementsByTagName("InvoiceQueryRs");

				if (invoiceQueryRs.Count == 1)
				{
					//Get the Status value
					string status = invoiceQueryRs.Item(0).Attributes.GetNamedItem("statusCode").Value;

					//Read nodes if status is OK = 0
					if (Convert.ToInt32(status) == 0)
					{
						XmlNodeList invoiceList = invoiceQueryRs.Item(0).ChildNodes;

						string[] row;
						string item = "";

                        // Run every invoice at a time
                        foreach (XmlNode invoice in invoiceList)
						{
							// Get the list of items from current invoice
							XmlNodeList itemsRetList = invoice.SelectNodes("InvoiceLineRet");
							// Filter the specific item from the invoices
							foreach (XmlNode line in itemsRetList)
							{
								item = line.SelectNodes("ItemRef/FullName").Count == 1 ? line.SelectSingleNode("ItemRef/FullName").InnerText : "";
                                // Select only the lines that coontain the specific product
                                if (item == product)
								{
									row = new string[4];
									row[0] = invoice.SelectSingleNode("TxnDate").InnerText;
									row[1] = invoice.SelectSingleNode("CustomerRef/FullName").InnerText;
									row[2] = int.Parse(line.SelectSingleNode("Quantity").InnerText).ToString("D");
									row[3] = double.Parse(line.SelectSingleNode("Rate").InnerText).ToString("F2");
									// Add Product to list
									invoiceLine.Add(row);
								}
							}
						}
					}
				}

				// Print the items to a .csv file
				if (purchaseLine.Count > 0 || invoiceLine.Count > 0)
				{
					// Create a string builder
					StringBuilder sb = new StringBuilder();

                    // Create Name of the report, 1st rows
                    sb.AppendLine("Product Report for " + product);

                    // Create Header
                    string[] header = Filter.headerProductReport();

                    if (purchaseLine.Count > 0)
                    {
                        // Add header
                        sb.AppendLine(string.Join(",", header));

                        // Print Purchases for product
                        foreach (string[] line in purchaseLine)
                        {
                            sb.AppendLine(string.Join(",", line));
                            count++;
                        }
                    }

                    if (invoiceLine.Count > 0)
                    {
                        //Add an extra line for separation
                        sb.AppendLine();

                        // Add header
                        sb.AppendLine(string.Join(",", header));

                        // Print Invoices for product
                        foreach (string[] line in invoiceLine)
                        {
                            sb.AppendLine(string.Join(",", line));
                            count++;
                        }
                    }

                    // Export streambuilder to product_yyyymmdd.csv file
                    string fileName = "\\" + product + "_" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + ".csv";
                    File.WriteAllText(folder + fileName, sb.ToString());
                }
			}
			catch (Exception ex) 
			{
                // Send Error to the Error Log
                string excep = DateTime.Now.ToString() + " [QBParser-xml2csvProductReport] " + ex.Message;
				File.AppendAllText("ErrorLog.txt", excep + Environment.NewLine);
			}

			return count;
		}
    }
}
