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
        [Required]
        public string CustomerName { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string CustomerEmail { get; set; }
        [Required]
        public string CustomerNo { get; set; }
        [Required]
        public string CustomerAddress { get; set; }

        public DateTime TransactionDate { get; set; }
    }
}
