using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Attributes;

namespace StartFinance.Models
{
    public class ShoppingList
    {
        [PrimaryKey, AutoIncrement]
        public int productID { get; set; }
        [NotNull]
        public string productName { get; set; }
        [NotNull]
        public int productQty {get; set;}
        [NotNull]
        public double productPrice { get; set; }
        public double productTotal { get; set; }
    }
}
