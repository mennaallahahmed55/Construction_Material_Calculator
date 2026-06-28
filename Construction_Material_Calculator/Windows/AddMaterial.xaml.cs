using Construction_Material_Calculator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Construction_Material_Calculator.Helpers;

namespace Construction_Material_Calculator.Windows
{
    
    public partial class AddMaterial : Window
    {
        public List<string> MaterialType { get; set; }
        private AppData AppData { get; set; }   
        public AddMaterial(AppData data)
        {
            InitializeComponent();
            AppData = data;
            BtnSave.IsEnabled = false; // Disable the Save button until valid input is provided
            MaterialType = AppData.Materials.Select(m => m.Category).Distinct().ToList(); //convert (observable collection to list)
            DataContext = this;

        }

        private void Button_Save(object sender, RoutedEventArgs e)
        {
           
            Material newMaterial = new Material
            { //assign value from txtbox,combox
                Name = MaterialNameTxt.Text, 
                Category = CategoryBox.SelectedItem.ToString(),
                Unit = UnitTxt.Text,
                UnitPrice = decimal.Parse(UnitPriceTxt.Text)
            };
            
            AppData.Materials.Add(newMaterial);
            Helper.SaveToJson(AppData);
        }
        private void Button_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

       
        private void TurnOnOff()
        {
            bool isNameValid = !string.IsNullOrWhiteSpace(MaterialNameTxt.Text) && 
                !double.TryParse(MaterialNameTxt.Text, out _); //validate name is not empty & not number
            bool isUnitValid = !string.IsNullOrWhiteSpace(UnitTxt.Text) && 
                !double.TryParse(UnitTxt.Text, out _); //validate unit is not empty & not number
            bool isPriceValid = double.TryParse(UnitPriceTxt.Text, out double price) && price > 0; //validate price +

            if (BtnSave != null) // Check if BtnSave is initialized
                BtnSave.IsEnabled = isNameValid && isUnitValid && isPriceValid;
        }

     
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            { 
                bool isValid = true;
                string errorMessage = "";

                if (textBox.Name == "UnitPriceTxt")
                {
                    isValid = double.TryParse(textBox.Text, out _);
                    errorMessage = "Please enter a valid number";
                }
                else
                {
                    bool isNumber = double.TryParse(textBox.Text, out _);
                    if (string.IsNullOrWhiteSpace(textBox.Text) || isNumber)
                    {
                        isValid = false;
                        errorMessage = "This field cannot be empty";
                    }
                }


                if (!isValid && !string.IsNullOrEmpty(textBox.Text))
                { 
                    textBox.BorderBrush = Brushes.Red;
                    textBox.BorderThickness = new Thickness(1.5);
                    textBox.ToolTip = errorMessage;
                }
                else
                {
                    textBox.ClearValue(BorderBrushProperty);
                    textBox.ToolTip = null;
                }
            }
            TurnOnOff();
        }
    }
    
}
