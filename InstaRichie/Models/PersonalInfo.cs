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

        private string dateString;
        private DateTime dateDate;
        private int personalID;
        private String firstName;
        private String lastName;
        private DateTime dob;
        private String gender;
        private String emailAddress;
        private String mobilePhone;

        [PrimaryKey, Unique]
        public int PersonalID
        {
            get { return personalID; }
            set { personalID = value; }
        }
        [NotNull]
        public String FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }
        [NotNull]
        public String LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }
        [NotNull]
        public DateTime DOB
        {
            get { return dateDate; }
            set { dateDate = value; }         
        }

        public string DOBString
        {
            get { return dateString; }
            set { dateString = value; }
        }

        [NotNull]
        public String Gender
        {
            get { return gender; }
            set { gender = value; }
        }
        [NotNull]
        public String EmailAddress
        {
            get { return emailAddress; }
            set { emailAddress = value; }
        }
        [NotNull]
        public String MobilePhone
        {
            get { return mobilePhone; }
            set { mobilePhone = value; }
        }
        public void convertDateToShortDate()
        {
            dateString = dateDate.ToString("d");
        }    
        
        public PersonalInfo(int personalID, String firstName, String lastName, DateTime dob,
                       String gender, String emailAddress, String mobilePhone)
        {
            PersonalID = personalID;
            FirstName = firstName;
            LastName = lastName;
            DOB = dob;
            Gender = gender;
            EmailAddress = emailAddress;
            MobilePhone = mobilePhone;
        }   

        public PersonalInfo()
        {

        }

    }
}
