using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using StartFinance.Models;
using SQLite.Net;
using Windows.UI.Popups;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace StartFinance.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PersonalInfoPage : Page
    {
        SQLiteConnection conn; // adding an SQLite connection
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");

        public PersonalInfoPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
            Results();
        }

        public void Results()
        {     
            conn.CreateTable<PersonalInfo>();
            var query = conn.Table<PersonalInfo>();
            PersonalInfoList.ItemsSource = query.ToList();

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Results();
        }

        private bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        private bool IsValidGenderCheck(string str)
        {
            if ((str == "M") || (str == "m") || (str == "F") || (str == "f"))
            {
                return true;
            }
            return false;
        }

        private async void AddPInfo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Null Check
                if (PersonalIDTextBox.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("PersonalID not Entered", "Oops..!");
                    await dialog.ShowAsync();                    
                }
                else if (FirstNameTextBox.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("First Name not Entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (LastNameTextBox.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Last Name not Entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (GenderTextBox.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Gender not Entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (EmailAddressTextBox.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Email Address not Entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (MobilePhoneTextBox.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Mobile Phone not Entered", "Oops..!");
                    await dialog.ShowAsync();
                }

                // Input Validation
                else if (IsDigitsOnly(PersonalIDTextBox.Text.ToString()) == false)
                {
                    MessageDialog dialog = new MessageDialog("Personal ID field invalid.", "Please re-enter your Personal ID.");
                    await dialog.ShowAsync();
                }
                else if (IsDigitsOnly(MobilePhoneTextBox.Text.ToString()) == false)
                {
                    MessageDialog dialog = new MessageDialog("Mobile Phone field invalid.", "Please re-enter your Mobile Phone Number.");
                    await dialog.ShowAsync();
                }
                else if (IsValidGenderCheck(GenderTextBox.Text.ToString()) == false)
                {
                    MessageDialog dialog = new MessageDialog("Gender field invalid.", "Please re-enter your Gender (M/F).");
                    await dialog.ShowAsync();
                }

                // Data Insertion
                else
                {   
                    DateTimeOffset PInfoDOB = DOBDatePicker.Date;
                    var DOB = PInfoDOB.Date;
                    
                    conn.Insert(new PersonalInfo()
                    {
                        PersonalID = Convert.ToInt16(PersonalIDTextBox.Text),
                        FirstName = FirstNameTextBox.Text,
                        LastName = LastNameTextBox.Text,
                        DOB = DOB,
                        Gender = GenderTextBox.Text.ToUpper(),
                        EmailAddress = EmailAddressTextBox.Text,
                        MobilePhone = MobilePhoneTextBox.Text,                                             
                    });
                    Results();
                }

            }
            catch (Exception ex)
            {   // Exception to display when amount is invalid or not numbers
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("Entered an invalid field data", "Oops..!");
                    await dialog.ShowAsync();
                }   
                // Exception handling when SQLite contraints are violated
                else if (ex is SQLiteException)
                {
                    MessageDialog dialog = new MessageDialog("Personal ID already exist, Try Different Personal ID", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                   
                }

            }
        }

        private void EditPInfo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UpdatePInfo_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void DeletePInfo_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog ShowConf = new MessageDialog("Deleting this Person will delete all information of this person", "Important");
            ShowConf.Commands.Add(new UICommand("Yes, Delete")
            {
                Id = 0
            });
            ShowConf.Commands.Add(new UICommand("Cancel")
            {
                Id = 1
            });
            ShowConf.DefaultCommandIndex = 0;
            ShowConf.CancelCommandIndex = 1;

            var result = await ShowConf.ShowAsync();
            if ((int)result.Id == 0)
            {
                try
                {
                    int PersonalInfoLabel = ((PersonalInfo)PersonalInfoList.SelectedItem).PersonalID;
                    var querydel = conn.Query<PersonalInfo>("DELETE FROM PersonalInfo WHERE PersonalID ='" + PersonalInfoLabel + "'");
                    Results();
                    conn.CreateTable<PersonalInfo>();
                }
                catch (NullReferenceException)
                {
                    MessageDialog ClearDialog = new MessageDialog("Please select the item to Delete", "Oops..!");
                    await ClearDialog.ShowAsync();
                }
            }
            else
            {
                
            }
        }

        private void PersonalInfoList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PersonalInfoList.SelectedIndex != -1)
            {
                // Personal Info fields population
                PersonalIDTextBox.Text = Convert.ToString(((PersonalInfo)PersonalInfoList.SelectedItem).PersonalID);
                FirstNameTextBox.Text = ((PersonalInfo)PersonalInfoList.SelectedItem).FirstName;
                LastNameTextBox.Text = ((PersonalInfo)PersonalInfoList.SelectedItem).LastName;
                DOBDatePicker.Date = ((PersonalInfo)PersonalInfoList.SelectedItem).DOB;                              
                GenderTextBox.Text = ((PersonalInfo)PersonalInfoList.SelectedItem).Gender;
                EmailAddressTextBox.Text = ((PersonalInfo)PersonalInfoList.SelectedItem).EmailAddress;
                MobilePhoneTextBox.Text = ((PersonalInfo)PersonalInfoList.SelectedItem).MobilePhone;

                //Disable text boxes so details are only editable when using edit button
                PersonalIDTextBox.IsEnabled = false;
                FirstNameTextBox.IsEnabled = false;
                LastNameTextBox.IsEnabled = false;
                DOBDatePicker.IsEnabled = false;
                GenderTextBox.IsEnabled = false;
                EmailAddressTextBox.IsEnabled = false;
                MobilePhoneTextBox.IsEnabled = false;
            }
        }

        
    }
}
