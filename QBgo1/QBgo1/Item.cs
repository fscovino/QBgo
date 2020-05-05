using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QBgo1
{
    class Item
    {
        //Attributes Used on the estimates
        public string CustomerCode { get; set; }
        public string PartNumber { get; set; }  //Used on both Estimates & Inventory
        public int Quantity { get; set; }
        public string Rate { get; set; }

        //Attributes used on the Inventory
        public string TimeModified { get; set; }
        public string BarCodeValue { get; set; }
        public string ManufacturerPartNumber { get; set; }
        public string SalesDesc { get; set; }
        public string SalesPrice { get; set; }
        public string PurchaseCost { get; set; }
        public string AverageCost { get; set; }
        public string QuantityOnHand { get; set; }
        public string QuantityOnOrder { get; set; }
        public string QuantityOnSalesOrder { get; set; }

        // Field to holding error notes
        public string errorMessage { get; set; }
    }
}
