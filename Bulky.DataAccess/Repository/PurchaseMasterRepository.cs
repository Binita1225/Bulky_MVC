using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public PurchaseMaster GetFirstOrDefault(Expression<Func<PurchaseMaster, bool>> filter = null, string includeProperties = null)
        {
            IQueryable<PurchaseMaster> query = _db.PurchaseMasters;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            return query.FirstOrDefault();
        }

    }
}
