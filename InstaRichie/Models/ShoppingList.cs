using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Attributes;

namespace StartFinance.Models
{
    public class ShoppingLists
    {
        [PrimaryKey, AutoIncrement]
        public int shoppingItemID { get; set; }
        [NotNull]
        public string shopName { get; set; }
        [NotNull]
        public string nameOfItem { get; set; }
        [NotNull]
        public DateTime shoppingDate { get; set; }
        [NotNull]
        public double priceQuoted { get; set; }
    }
}
