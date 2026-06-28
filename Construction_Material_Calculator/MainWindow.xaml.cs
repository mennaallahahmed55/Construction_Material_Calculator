using Construction_Material_Calculator.Helpers;
using Construction_Material_Calculator.Models;
using Construction_Material_Calculator.Windows;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.IO;

namespace Construction_Material_Calculator
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<Material> Materials { get; set; }
        public ObservableCollection<Order> Orders { get; set; }

        private AppData appData;

        public int OrdersCount => Orders?.Count ?? 0; 

        public int PendingOrdersCount => Orders?.Count(o => o.Status == "Pending") ?? 0; //check if status is pending, if Orders is null return 0

        public decimal TotalSpent => Orders?.Sum(o => o.Total) ?? 0; 

        public MainWindow()
        {
            InitializeComponent();

            appData = Helper.LoadFromJson(); 

            Materials = appData.Materials; 
            Orders = appData.Orders; 

            DataContext = this; // Set the DataContext to the MainWindow instance

            Orders.CollectionChanged += Orders_CollectionChanged; //  if there are any changes to the Orders collection, it will trigger the event handler to update the UI 
        }

        private void Orders_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Dispatcher.Invoke(() => // Update the DataContext to refresh the UI when the Orders collection changes (clear,reset)
            {
                DataContext = null; 
                DataContext = this; 
            });
        }

        private void AddMaterial_Click(object sender, RoutedEventArgs e)
        {
            AddMaterial addMaterial = new AddMaterial(appData); 
            addMaterial.Show();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Order_Click(object sender, RoutedEventArgs e)
        {
            OrdersWindow order = new OrdersWindow(appData);
            order.Show();
        }

        private void Calculator_Click(object sender, RoutedEventArgs e)
        {
            CalculatorWindow calculatorWindow = new CalculatorWindow(appData);
            calculatorWindow.Show();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

       
        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
                var dialog = new SaveFileDialog { Filter = "JSON|*.json" };
                if (dialog.ShowDialog() == true)
                {
                    var data = new { Materials = Materials, Orders = Orders }; // Include both Materials and Orders in the export
                    File.WriteAllText(dialog.FileName,
                        JsonConvert.SerializeObject(data, Formatting.Indented)); // Use JsonConvert for serialization
            }

        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog { Filter = "JSON|*.json" }; // Open a file dialog to select the JSON file to import
            if (dialog.ShowDialog() == true)
            {
                var data = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(dialog.FileName)); // Deserialize the JSON file into a dynamic object
                Materials.Clear();
                Orders.Clear();
                foreach (var m in data.Materials) Materials.Add(new Material
                { // Add each material from the imported data to the Materials collection
                    Name = m.Name,
                    Category = m.Category,
                    Unit = m.Unit,
                    UnitPrice = m.UnitPrice 
                });
                foreach (var o in data.Orders) Orders.Add(new Order
                {// Add each order from the imported data to the Orders collection
                    MaterialName = o.MaterialName,
                    Quantity = o.Quantity,
                    Status = o.Status 
                });
            }
        }
    }
}