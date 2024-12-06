using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class HistoryRepository : Repository<History>, IHistoryRepository
    {

        private ApplicationDbContext _db;
        public HistoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public IEnumerable<History> GetByItemId(int itemId)
        {
            return _db.Histories.Where(h => h.ItemId == itemId).Include(h => h.Product);
        }
    }
}
