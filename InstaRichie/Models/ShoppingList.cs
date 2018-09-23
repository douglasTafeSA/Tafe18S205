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
        public int shoppingItemID { get; set; }
        [NotNull]
        public string shopName { get; set; }
        [NotNull]
        public string nameOfItem { get; set; }
        [NotNull]
        public DateTime shoppingDate { get; set; }
        [NotNull]
        public double priceQuoted { get; set; }

        
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(shopName);
            sb.Append("\t\t");
            sb.Append(nameOfItem);
            sb.Append("\t\t");
            sb.Append(shoppingDate.Date.ToString("dd/MM/yyyy"));
            sb.Append("\t\t");
            sb.Append(priceQuoted);

            return sb.ToString();
        }
    }
}
