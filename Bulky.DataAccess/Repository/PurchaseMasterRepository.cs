using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class PurchaseMasterRepository : Repository<PurchaseMaster>, IPurchaseMasterRepository
    {
        private ApplicationDbContext _db;
        public PurchaseMasterRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    
    }
}
