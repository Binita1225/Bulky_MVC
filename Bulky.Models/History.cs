using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models
{

    public enum TransactionType
    {
        Purchase,
        Sales
    } 
    public enum StockCheckOut
    {
        In,
        Out
    }

    public partial class History
    {
        [Key]
        public int Id { get; set; }
        public TransactionType TransactionType { get; set; }
        public StockCheckOut StockCheckOut { get; set; }
        public int ItemId { get; set; }
        [ForeignKey("ItemId")]
        public Product Product { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
