using BulkyBook.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HistoryController : Controller
    {
        

            private readonly IUnitOfWork _unitOfWork;

            public HistoryController(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }
            public IActionResult Index()
            {
                var histories = _unitOfWork.History.GetAll(includeProperties: "Product");
                return View(histories);
            }
        }
    }
