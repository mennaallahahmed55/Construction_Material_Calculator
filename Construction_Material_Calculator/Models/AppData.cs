using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Construction_Material_Calculator.Models
{
    public class AppData
    {
        // Observ use Inotify to update automatically when data changes
        public ObservableCollection<Material> Materials { get; set; } 
        public ObservableCollection<Order> Orders { get; set; } 

        
        public AppData()
        { // Initialize the collections to avoid null reference issues
            Materials = new ObservableCollection<Material>();
            Orders = new ObservableCollection<Order>();
        }
    }

}
