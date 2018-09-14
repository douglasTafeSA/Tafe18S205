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
            conn.CreateTable<ShoppingLists>();
            var query1 = conn.Table<ShoppingLists>();
            ShoppingListView.ItemsSource = query1.ToList();
        }

        private async void AddShoppingList_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (tbShopName.Text.ToString() == "" || tbShopDate.ToString() == "" || tbItemName.Text.ToString() == "" || tbPriceQuote.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("No value entered", "Must enter a Shop Name, Item name and price..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    double priceQuote = Convert.ToDouble(tbPriceQuote.Text);
                    DateTime date = tbShopDate.Date.DateTime;
                    conn.CreateTable<ShoppingLists>();
                    conn.Insert(new ShoppingLists
                    {
                        nameOfItem = tbItemName.Text.ToString(),
                        priceQuoted = priceQuote,
                        shopName = tbShopName.Text.ToString(),
                        shoppingDate = date
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
                    MessageDialog dialog = new MessageDialog("ShoppingLists could no tbe created, Try again with new values", "Oops..!");
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
                int listSelection;
                try
                {
                    listSelection = ((ShoppingLists)ShoppingListView.SelectedItem).shoppingItemID;
                }
                catch
                {
                    MessageDialog dialog = new MessageDialog("No selected Item", "Oops..!");
                    await dialog.ShowAsync();
                    return;
                }
                
                conn.CreateTable<ShoppingLists>();
                var query1 = conn.Table<ShoppingLists>();
                var query3 = conn.Query<ShoppingLists>("DELETE FROM ShoppingLists WHERE shoppingItemID = " + listSelection);
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

        private async void SaveItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ShoppingLists listSelection;
                try
                {
                    listSelection = conn.Find<ShoppingLists>(((ShoppingLists)ShoppingListView.SelectedItem).shoppingItemID);
                    listSelection.shopName = tbShopName.Text;
                    listSelection.shoppingDate = tbShopDate.Date.DateTime;
                    listSelection.nameOfItem = tbItemName.Text;
                    listSelection.priceQuoted = double.Parse(tbPriceQuote.Text);
                    try
                    {
                        
                        conn.Update(listSelection);
                    }
                    catch
                    {
                        MessageDialog dialog = new MessageDialog("Could not update Shopping List", "Please try again!!");
                        await dialog.ShowAsync();
                        return;
                    }
                    
                }
                catch
                {
                    MessageDialog dialog = new MessageDialog("No selected Item or input invalid", "Oops..!");
                    await dialog.ShowAsync();
                    return;
                }

                Results();
               
            }
            catch (NullReferenceException)
            {
                MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                await dialog.ShowAsync();
            }
        }

        private void ShoppingListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ShoppingLists sList = (ShoppingLists)e.ClickedItem;
            tbShopName.Text = sList.shopName;
            DateTime date = sList.shoppingDate;
            tbShopDate.Date = date;
            tbItemName.Text = sList.nameOfItem;
            tbPriceQuote.Text = sList.priceQuoted.ToString();
        }
    }
}
