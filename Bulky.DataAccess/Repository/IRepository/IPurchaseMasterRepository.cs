using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository.IRepository
{
    public interface IPurchaseMasterRepository : IRepository<PurchaseMaster>
    {
        PurchaseMaster GetFirstOrDefault(Expression<Func<PurchaseMaster, bool>> filter = null, string includeProperties = null);
    }
}
