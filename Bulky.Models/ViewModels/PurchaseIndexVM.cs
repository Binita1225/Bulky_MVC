using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models.ViewModels
{
    public class PurchaseIndexVM
    {
        public List<PurchaseMaster> PurchaseMasters { get; set; } = new List<PurchaseMaster>();
        public List<PurchaseDetail> PurchaseDetail { get; set; } = new List<PurchaseDetail>();
    }
}
