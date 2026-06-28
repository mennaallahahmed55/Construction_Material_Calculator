using Construction_Material_Calculator.Helpers;
using Construction_Material_Calculator.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Construction_Material_Calculator.Windows
{
    public partial class OrdersWindow : Window
    {
        private AppData appData { get; set; }

        public ObservableCollection<Order> OrdersGrid { get; set; }

        public List<string> CategoryOrdersCombox { get; set; }

        private ICollectionView collectionView;  //create view to filter & sort on it, not on original collection

        public OrdersWindow(AppData data)
        { 
            InitializeComponent(); 

            try
            {
                if (data == null)
                {
                    MessageBox.Show("Error: AppData is null", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                    return;
                }

                appData = data; 

                if (appData.Orders == null)
                {
                    appData.Orders = new ObservableCollection<Order>();
                }

                if (appData.Materials == null)
                {
                    appData.Materials = new ObservableCollection<Material>();
                }

                OrdersGrid = appData.Orders; //Binding orders to grid

                try
                {
                    //get category from material without duplicate
                    CategoryOrdersCombox = appData.Materials 
                        .Select(m => m.Category) 
                        .Distinct()
                        .ToList(); 
                }
                catch
                {
                    CategoryOrdersCombox = new List<string>();
                }

                CategoryOrdersCombox.Insert(0, "All"); //add all to list

                try
                {
                    collectionView = CollectionViewSource.GetDefaultView(OrdersGrid); //create view to filter

                    if (collectionView == null)
                    {
                        collectionView = new ListCollectionView(OrdersGrid);  
                    }

                    collectionView.Filter = null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error creating collection view: {ex.Message}");
                    collectionView = new ListCollectionView(OrdersGrid);
                }

                OrdersDataGrid.ItemsSource = collectionView; //Binding collection view to filter

                DataContext = this; 

                try
                {
                    if (appData.Orders != null && appData.Orders.Count > 0)
                    {
                        UpdateSummary(appData.Orders); 
                    }
                    else
                    {
                        UpdateSummary(new List<Order>()); 
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error updating summary: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing OrdersWindow: {ex.Message}");
                Close();
            }
        }

        private void ApplyFilters()
        {
            try
            {
                if (collectionView == null)
                {
                    return; 
                }

                collectionView.Filter = (item) => 
                {
                    Order order = item as Order; //check if item is order
                    if (order == null) return false; 

                    // Category Filter
                    if (CategoryComboBox.SelectedItem != null) 
                    {
                        string selectedCategory = CategoryComboBox.SelectedItem.ToString(); 
                        if (!string.IsNullOrWhiteSpace(selectedCategory) && selectedCategory != "All")
                        {
                            if (order.Category == null || order.Category != selectedCategory)
                                return false;
                        }
                    }

                    // Status Filter
                    if (StatusComboBox.SelectedItem != null)
                    {
                        ComboBoxItem statusItem = StatusComboBox.SelectedItem as ComboBoxItem;  
                        if (statusItem != null)
                        {
                            string selectedStatus = statusItem.Content?.ToString();
                            if (!string.IsNullOrWhiteSpace(selectedStatus) && selectedStatus != "All")
                            {
                                if (order.Status == null || order.Status != selectedStatus)
                                    return false;
                            }
                        }
                    }

                    // Search Filter
                    if (!string.IsNullOrWhiteSpace(SearchTextBox.Text))
                    {
                        string searchText = SearchTextBox.Text.ToLower(); 
                        if (string.IsNullOrWhiteSpace(order.MaterialName) || !order.MaterialName.ToLower().Contains(searchText))
                            return false;
                    }

                    // From Date
                    if (FromDatePicker.SelectedDate.HasValue)
                    {
                        if (order.Date < FromDatePicker.SelectedDate.Value)
                            return false;
                    }

                    // To Date
                    if (ToDatePicker.SelectedDate.HasValue)
                    {
                        if (order.Date > ToDatePicker.SelectedDate.Value)
                            return false;
                    }

                    return true;
                };

                collectionView.Refresh();

                try
                {
                    var filteredOrders = collectionView.Cast<Order>().ToList(); 
                    UpdateSummary(filteredOrders);
                }
                catch
                {
                    UpdateSummary(new List<Order>());
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error applying filters: {ex.Message}");
            }
        }

        private void UpdateSummary(IEnumerable<Order> orders)
        {
            try
            {
                if (orders == null)
                {
                    OrdersCountTextBlock.Text = "0";
                    TotalAmountTextBlock.Text = "0.00";
                    PendingCountTextBlock.Text = "0";
                    return;
                }

                OrdersCountTextBlock.Text = orders.Count().ToString();

                TotalAmountTextBlock.Text = orders.Sum(o => o.Total).ToString("N2");

                PendingCountTextBlock.Text =
                    orders.Count(o => o.Status == "Pending").ToString(); //consider status is pending
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating summary: {ex.Message}");
                OrdersCountTextBlock.Text = "0";
                TotalAmountTextBlock.Text = "0.00";
                PendingCountTextBlock.Text = "0";
            }
        }

        private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ApplyFilters();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in CategoryComboBox_SelectionChanged: {ex.Message}");
            }
        }

        private void StatusComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ApplyFilters();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in StatusComboBox_SelectionChanged: {ex.Message}");
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                ApplyFilters();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in SearchTextBox_TextChanged: {ex.Message}");
            }
        }

        private void FromDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ApplyFilters();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in FromDatePicker_SelectedDateChanged: {ex.Message}");
            }
        }

        private void ToDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ApplyFilters();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ToDatePicker_SelectedDateChanged: {ex.Message}");
            }
        }

        private void MarkDeliveredButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (OrdersDataGrid.SelectedItem is Order order)
                {
                    order.Status = "Delivered";
                    ApplyFilters();
                }
                else
                {
                    MessageBox.Show("Please select an order first", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error marking order as delivered: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (OrdersDataGrid.SelectedItem is Order order)
                {
                    if (appData.Orders.Contains(order))
                    {
                        appData.Orders.Remove(order);
                        ApplyFilters();
                    }
                }
                else
                {
                    MessageBox.Show("Please select an order first", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting order: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}