using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models.ViewModels
{
    public class PurchaseProductVM
    {
        public int Id { get; set; }
        public String Name { get; set; }

        public double Rate { get; set; }

        public int Stock { get; set; }
    }
}
