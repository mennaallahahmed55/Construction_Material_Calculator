using Construction_Material_Calculator.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Construction_Material_Calculator.Helpers
{
    public class Helper
    { 
        private static string filePath =
             Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MaterialDataBase.json"); //to global path of json file

        public static void SaveToJson(AppData data)
        {
            string dataconverter = JsonConvert.SerializeObject(data, Formatting.Indented); //convert data to json format 
            File.WriteAllText(filePath, dataconverter); //write json data to file
        }

        public static AppData LoadFromJson()
        {
            AppData appData = null; 

            if (File.Exists(filePath))
            {
                try
                {
                    string jsonData = File.ReadAllText(filePath); 
                    appData = JsonConvert.DeserializeObject<AppData>(jsonData); //convert json data back to AppData object
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error loading JSON: {ex.Message}");
                    appData = null;
                }
            }

            if (appData == null)
            {
                appData = new AppData
                {  //ensure that Collections are initialized even if JSON loading fails
                    Materials = new System.Collections.ObjectModel.ObservableCollection<Material>(), 
                    Orders = new System.Collections.ObjectModel.ObservableCollection<Order>()
                };
            }
            else
            { //to avoid null exp if json is missed
                if (appData.Materials == null)
                    appData.Materials = new System.Collections.ObjectModel.ObservableCollection<Material>();

                if (appData.Orders == null)
                    appData.Orders = new System.Collections.ObjectModel.ObservableCollection<Order>();
            }

            return appData;
        }

        public static double GetNumValue(TextBox textBox)
        { 
            double.TryParse(textBox.Text, out double value);
            return value;
        }
    }
}