using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models.ViewModels
{
    public class PurchaseVM
    {
        public PurchaseMaster PurchaseMaster { get; set; }      
        public List<PurchaseDetail> PurchaseDetail { get; set; } = new List<PurchaseDetail>();

        public List<PurchaseProductVM> Products { get; set; }


    }
}
