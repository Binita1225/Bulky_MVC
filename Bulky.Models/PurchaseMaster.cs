using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models
{
    public class PurchaseMaster
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Customer name is required")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Customer email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string CustomerEmail { get; set; }
        [Required(ErrorMessage = "Customer phone number is required")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits")]
        public string CustomerNo { get; set; }
        [Required(ErrorMessage = "Customer address is required")]
        public string CustomerAddress { get; set; }
        [Required(ErrorMessage = "Transaction date is required")]
        [DataType(DataType.Date)]

        public DateTime TransactionDate { get; set; }

        public ICollection<PurchaseDetail> PurchaseDetails { get; set; }
    }
}
