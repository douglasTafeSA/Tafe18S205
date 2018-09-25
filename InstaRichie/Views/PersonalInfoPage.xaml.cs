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
using Windows.Globalization.DateTimeFormatting;
using Windows.Globalization;
using System.Globalization;


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

        PersonalInfo PInfo;

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
             
                    var PInfoDOB = DOBDatePicker.Date.Date;

                    PInfo = new PersonalInfo(Convert.ToInt32(PersonalIDTextBox.Text), FirstNameTextBox.Text,
                        LastNameTextBox.Text, PInfoDOB, GenderTextBox.Text.ToUpper(),
                        EmailAddressTextBox.Text, MobilePhoneTextBox.Text);
                   
                    PInfo.convertDateToShortDate();
                    conn.Insert(PInfo);
                    
                    Results();


                    //Reset fields
                    PersonalIDTextBox.Text = "";
                    FirstNameTextBox.Text = "";
                    LastNameTextBox.Text = "";
                    DOBDatePicker.Date = System.DateTime.Today.Date;
                    GenderTextBox.Text = "";
                    EmailAddressTextBox.Text = "";
                    MobilePhoneTextBox.Text = "";
                    PersonalInfoList.SelectedItem = null;
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
            //Disable delete button 
            DeleteBarButton.IsEnabled = false;

            //Enable text boxes 
            PersonalIDTextBox.IsEnabled = true;
            FirstNameTextBox.IsEnabled = true;
            LastNameTextBox.IsEnabled = true;
            DOBDatePicker.IsEnabled = true;
            GenderTextBox.IsEnabled = true;
            EmailAddressTextBox.IsEnabled = true;
            MobilePhoneTextBox.IsEnabled = true;

            //Disable ListView
            PersonalInfoList.IsItemClickEnabled = false;

            //Enable Update button
            UpdateBarButton.IsEnabled = true;
        }

        private async void UpdatePInfo_Click(object sender, RoutedEventArgs e)
        {           
            //Update Record
            MessageDialog ShowConf = new MessageDialog("Update Record? Overwritten information will be lost", "Important");
            ShowConf.Commands.Add(new UICommand("Yes, Update")
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
                    //Labels
                    int PersonalInfoLabel = ((PersonalInfo)PersonalInfoList.SelectedItem).PersonalID;
                    
                    //Data inputs
                    int PersonalInfoInput = Convert.ToInt32(PersonalIDTextBox.Text);
                    string FirstNameInput = FirstNameTextBox.Text;
                    string LastNameInput = LastNameTextBox.Text;

                    var PInfoDOB = DOBDatePicker.Date.Date;
                    DateTime DOBInput = PInfoDOB;
                    string GenderInput = GenderTextBox.Text.ToUpper();
                    string EmailAddressInput = EmailAddressTextBox.Text;
                    string MobilePhoneInput = MobilePhoneTextBox.Text;

                    var queryupdate = conn.Query<PersonalInfo>("UPDATE PersonalInfo SET  PersonalID = '" +
                        PersonalInfoInput + "' , FirstName = '" + FirstNameInput + "' , LastName = '" +
                        LastNameInput + "' , DOB = '" + DOBInput + "' , Gender = '" + GenderInput + "' , EmailAddress = '" +
                        EmailAddressInput + "' , MobilePhone = '" + MobilePhoneInput + "'" +
                        " WHERE PersonalID = '" + PersonalInfoLabel + "';");
                    ((PersonalInfo)PersonalInfoList.SelectedItem).convertDateToShortDate();
                    Results();           
                    conn.CreateTable<PersonalInfo>();
                    Reset();
                    
                }
                catch (Exception ex)
                {          
                    // Exception to display when amount is invalid or not numbers
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
                    //Exception if nothing is selected
                    else if (ex is NullReferenceException)
                    {
                        MessageDialog ClearDialog = new MessageDialog("Please select the item to Update", "Oops..!");
                        await ClearDialog.ShowAsync();
                    }

                }

            }
            else
            {

            }
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

                    Reset();

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
                
                var dateString = ((PersonalInfo)PersonalInfoList.SelectedItem).DOBString;
                var dateParse = DateTime.Parse(dateString);
                DOBDatePicker.Date = dateParse;

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

                //Disable Add button
                AddBarButton.IsEnabled = false;
                //Enable Edit button
                EditBarButton.IsEnabled = true;
            }
        }

        private void ClearSelectionButton_Click(object sender, RoutedEventArgs e)
        {
            Reset();
        }


        public void Reset()
        {
            //Resets enabling and disabling
            PersonalIDTextBox.IsEnabled = true;
            FirstNameTextBox.IsEnabled = true;
            LastNameTextBox.IsEnabled = true;
            DOBDatePicker.IsEnabled = true;
            GenderTextBox.IsEnabled = true;
            EmailAddressTextBox.IsEnabled = true;
            MobilePhoneTextBox.IsEnabled = true;

            PersonalInfoList.IsItemClickEnabled = true;

            AddBarButton.IsEnabled = true;
            EditBarButton.IsEnabled = false;
            UpdateBarButton.IsEnabled = false;
            DeleteBarButton.IsEnabled = true;

            //Resets fields and selections
            PersonalIDTextBox.Text = "";
            FirstNameTextBox.Text = "";
            LastNameTextBox.Text = "";
            DOBDatePicker.Date = System.DateTime.Today.Date;
            GenderTextBox.Text = "";
            EmailAddressTextBox.Text = "";
            MobilePhoneTextBox.Text = "";

            PersonalInfoList.SelectedItem = null;
        }
    }
}
