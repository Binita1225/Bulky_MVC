using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models
{
    public class PurchaseDetail
    {
        [Key]
        public int Id { get; set; }
        public int ItemId {  get; set; }
        [ForeignKey("ItemId")]
        public virtual Product Product { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        [Range(1, 1000)]
        public int Rate { get; set; }                           
        public int RateUpto50 { get; set; }                           
        public int RateAbove50 { get; set; }                           
        public int RateAbove100 { get; set; }                           
        public int Stock { get; set; }                           
        public int MasterId {  get; set; }
        [ForeignKey("MasterId")]
        public PurchaseMaster PurchaseMaster { get; set; }
        public int Total { get; set; }
    }
}
