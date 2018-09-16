using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Attributes;

namespace StartFinance.Models
{
    class PersonalInfo
    {
        [PrimaryKey, Unique]
        public int PersonalID { get; set; }

        [NotNull]
        public String FirstName { get; set; }

        [NotNull]
        public String LastName { get; set; }

        [NotNull]
        public DateTime DOB { get; set; }

        [NotNull]
        public String Gender { get; set; }

        [NotNull]
        public String EmailAddress { get; set; }

        [NotNull]
        public String MobilePhone { get; set; }

    }
}
