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
using SQLite;
using StartFinance.Models;
using Windows.UI.Popups;
using SQLite.Net;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace StartFinance.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShoppingListPage : Page
    {
        SQLiteConnection conn; // adding an SQLite connection
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");

        public ShoppingListPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            /// Initializing a database
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
            // Creating table
            Results();
        }

        public void Results()
        {
            conn.CreateTable<ShoppingList>();
            var query1 = conn.Table<ShoppingList>();
            ShoppingListView.ItemsSource = query1.ToList();
        }

        private async void AddShoppingList_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_ShoppingListName.Text.ToString() == "" || QtyInput.Text.ToString() == "" || MoneyIn.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("No value entered", "Must enter a name, quantity and price..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    double TempMoney = Convert.ToDouble(MoneyIn.Text);
                    int quantity = Convert.ToInt32(QtyInput.Text);
                    conn.CreateTable<ShoppingList>();
                    conn.Insert(new ShoppingList
                    {
                        productName = _ShoppingListName.Text.ToString(),
                        productPrice = TempMoney,
                        productQty = quantity,
                        productTotal = TempMoney * quantity
                    });
                    // Creating table
                    Results();
                }
            }
            catch (Exception ex)
            {
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("You forgot to enter the Amount or entered an invalid Amount", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (ex is SQLiteException)
                {
                    MessageDialog dialog = new MessageDialog("ShoppingList could no tbe created, Try again with new values", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    /// no idea
                }
            }
        }

        private async void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int AccSelection;
                try
                {
                    AccSelection = ((ShoppingList)ShoppingListView.SelectedItem).productID;
                }
                catch
                {
                    MessageDialog dialog = new MessageDialog("No selected Item", "Oops..!");
                    await dialog.ShowAsync();
                    return;
                }
                
                conn.CreateTable<ShoppingList>();
                var query1 = conn.Table<ShoppingList>();
                var query3 = conn.Query<ShoppingList>("DELETE FROM ShoppingList WHERE productID = " + AccSelection);
                ShoppingListView.ItemsSource = query1.ToList();
            }
            catch (NullReferenceException)
            {
                MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                await dialog.ShowAsync();
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Results();
        }
    }
}
