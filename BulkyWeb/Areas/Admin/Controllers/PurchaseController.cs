using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.RegularExpressions;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PurchaseController : Controller
    {


        private readonly IUnitOfWork _unitOfWork;

        public PurchaseController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var purchases = _unitOfWork.PurchaseMaster.GetAll(includeProperties : "PurchaseDetails.Product");

            var purchaseIndexVM = new PurchaseIndexVM
            {
                PurchaseMasters = purchases.ToList(),
                PurchaseDetail = purchases.SelectMany(p => p.PurchaseDetails).ToList()
            };
            return View(purchaseIndexVM);
        }

        public IActionResult Create()
        {
            var viewModel = new PurchaseVM
            {
                PurchaseMaster = new PurchaseMaster { TransactionDate = DateTime.Now },
                PurchaseDetail = new List<PurchaseDetail> { new PurchaseDetail()},
                Products = _unitOfWork.Product.GetAll().Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Title,
                    Group = new SelectListGroup { Name = p.ListPrice.ToString() }
                }).ToList()
            };
            return View(viewModel);
        }
        [HttpPost]
        public IActionResult Create(PurchaseVM purchaseVM)
        {
             _unitOfWork.PurchaseMaster.Add(purchaseVM.PurchaseMaster);
                _unitOfWork.Save();
 
            foreach (var detail in purchaseVM.PurchaseDetail)
            {
                detail.MasterId = purchaseVM.PurchaseMaster.Id;
                detail.Total = detail.Quantity * detail.Rate;
                _unitOfWork.PurchaseDetail.Add(detail);
            }
            _unitOfWork.Save();

                return RedirectToAction("Index");
            }

        }
    }
