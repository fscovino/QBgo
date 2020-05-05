using System;

namespace QBgo1
{
    public static class Filter
    {
        private const string WEBSITE = "http://www.cmhunited.com/images/";
        private const string IMG_EXT = ".jpg";


        // *************************************************************************************
        //                                 CUSTOM FILTERS
        // *************************************************************************************

		public static string[] headerSalesReport()
		{
			string[] header = new string[] { "Sales Rep", "Customer", "Date", "Invoice", "Amount", "Profit", "No Items", "Percentage"};

			return header;
		}

        public static string[] headerProductReport()
        {
            string[] header = new string[] { "Date", "Company", "Quantity", "Price"};

            return header;
        }


        public static string[] headerInventoryReport()
        {
            string[] header = new string[] { "Brand", "Part#", "Description", "Available", "Coming", "Image", "Level 0", "Level 1", "Level 2", "Level 3", "MPN", "Cost", "Modified"};

            return header;
        }

        public static string description(string description)
        {

            string result = spaceReducer(description);
            result = replaceCharacters("*", "Apple", result);
            result = replaceCharacters(",", " /", result);

            return result;
        }

        public static string available(string onHand, string onSalesOrder)
        {
            int val1 =  Int32.Parse(onHand);
            int val2 = Int32.Parse(onSalesOrder);
            int total = val1 - val2;

            return total.ToString();
        }

        public static string image(string partNumber)
        {
            return WEBSITE + partNumber + IMG_EXT;
        }

        public static string brand(string description)
        {
            string desc = Filter.description(description);

            return selectBrand(desc);
        }

        public static string level3(string price)
        {
            return addPercentage("5", price);
        }

        // *************************************************************************************
        //                     Common Methods invoke on different filters
        // *************************************************************************************

        // Reduce extra space on a line to single spaces
        private static string spaceReducer(string line)
        {
            return line.Replace(" ", "{}").Replace("}{", "").Replace("{}", " ");
        }

        // Replace a character on a line
        private static string replaceCharacters(string oldCharacters, string newCharacters, string line)
        {
            return line.Replace(oldCharacters, newCharacters);
        }

        // Create a brand value from a line
        private static string selectBrand(string line)
        {
            string brand = "NA";

            // Separate line into words on an array
            char[] delimiter = { ' ' };
            string[] words = line.Split(delimiter);
            
            // Select the second word if first word = original
             if (words[0].ToLower() == "original")
            {
                brand = words[1].ToUpper();
            }

             switch (brand)
            {
                case "AGENT":
                    brand = "AGENT 18";
                    break;

                case "BODY":
                    brand = "BODY GLOVE";
                    break;

                case "BLACK":
                    brand = "BLACK ROCK";
                    break;

                case "PURE":
                    brand = "PURE GEAR";
                    break;

                case "WHITE":
                    brand = "WHITE DIAMONDS";
                    break;

                default:
                    break;
            }

            return brand;
        }

        // Create an url for an image
        private static string createUrl(string name)
        {
            return WEBSITE + name + IMG_EXT;
        }

        // Add a percantage to a number
        private static string addPercentage(string percentage, string number)
        {
            double rate = double.Parse(percentage);
            double value = double.Parse(number);
            double total = value + (value * (rate / 100));

            return total.ToString("N1");
        }

    }
}
