using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.RegularExpressions;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]/[action]")]
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

      
        public IActionResult View(int id)
        {
            // Retrieve the PurchaseMaster for the specific customer by ID
            var purchaseMaster = _unitOfWork.PurchaseMaster.GetFirstOrDefault(
                p => p.Id == id,
                includeProperties: "PurchaseDetails.Product");

            if (purchaseMaster == null)
            {
                return NotFound(); // Return a not found error if no purchase master is found
            }

            // Create the ViewModel to display purchase details
            var purchaseIndexVM = new PurchaseIndexVM
            {
                PurchaseMasters = new List<PurchaseMaster> { purchaseMaster },
                PurchaseDetail = purchaseMaster.PurchaseDetails.ToList()
            };

            // Return the View with the populated ViewModel
            return View(purchaseIndexVM);
        }

        public IActionResult Create()
        {
            var viewModel = new PurchaseVM
            {
                PurchaseMaster = new PurchaseMaster { TransactionDate = DateTime.Now },
                PurchaseDetail = new List<PurchaseDetail> { new PurchaseDetail() },
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
            // Add PurchaseMaster
            _unitOfWork.PurchaseMaster.Add(purchaseVM.PurchaseMaster);
            _unitOfWork.Save();

            foreach (var detail in purchaseVM.PurchaseDetail)
            {
                detail.MasterId = purchaseVM.PurchaseMaster.Id;
                detail.Total = detail.Quantity * detail.Rate;

                _unitOfWork.PurchaseDetail.Add(detail);

                var history = new History
                {
                    TransactionType = TransactionType.Purchase,
                    StockCheckOut = StockCheckOut.In,
                    ItemId = detail.ItemId,
                    TransactionDate = DateTime.Now
                };

                _unitOfWork.History.Add(history);
            }

            _unitOfWork.Save();

            return RedirectToAction("Index");
        }


        public IActionResult Edit(int id)
        {
            var purchaseMaster = _unitOfWork.PurchaseMaster.GetFirstOrDefault(
                p => p.Id == id, includeProperties: "PurchaseDetails.Product");
            if (purchaseMaster == null)
            {
                return NotFound();
            }

            var purchaseVM = new PurchaseVM
            {
                PurchaseMaster = purchaseMaster,
                PurchaseDetail = purchaseMaster.PurchaseDetails.ToList(),
                Products = _unitOfWork.Product.GetAll().Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Title,
                    Group = new SelectListGroup { Name = p.ListPrice.ToString() }
                }).ToList()
            };

            return View(purchaseVM);
        }


        [HttpPost]
        public IActionResult Edit(PurchaseVM purchaseVM)
        {
            var purchaseMaster = _unitOfWork.PurchaseMaster.GetFirstOrDefault(p => p.Id == purchaseVM.PurchaseMaster.Id);
            if (purchaseMaster == null)
            {
                return NotFound();
            }

            // Update PurchaseMaster
            purchaseMaster.CustomerName = purchaseVM.PurchaseMaster.CustomerName;
            purchaseMaster.CustomerEmail = purchaseVM.PurchaseMaster.CustomerEmail;
            purchaseMaster.CustomerNo = purchaseVM.PurchaseMaster.CustomerNo;
            purchaseMaster.CustomerAddress = purchaseVM.PurchaseMaster.CustomerAddress;
            purchaseMaster.TransactionDate = purchaseVM.PurchaseMaster.TransactionDate;

            _unitOfWork.PurchaseMaster.Update(purchaseMaster);

            // Remove existing details
            var existingDetails = _unitOfWork.PurchaseDetail.GetAll(d => d.MasterId == purchaseMaster.Id);
            foreach (var detail in existingDetails)
            {
                _unitOfWork.PurchaseDetail.Remove(detail);
            }

            // Add new details
            foreach (var detail in purchaseVM.PurchaseDetail)
            {
                detail.MasterId = purchaseMaster.Id;
                detail.Total = detail.Quantity * detail.Rate;
                _unitOfWork.PurchaseDetail.Add(detail);
            }

            _unitOfWork.Save();

            return RedirectToAction("Index");
        }


        public IActionResult Delete(int id)
        {
            var purchaseMaster = _unitOfWork.PurchaseMaster.GetFirstOrDefault(p => p.Id == id, includeProperties: "PurchaseDetails");
            if (purchaseMaster == null)
            {
                return NotFound();
            }

            return View(purchaseMaster);
        }

        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var purchaseMaster = _unitOfWork.PurchaseMaster.GetFirstOrDefault(p => p.Id == id, includeProperties: "PurchaseDetails");
            if (purchaseMaster == null)
            {
                return NotFound();
            }

            // Remove associated details
            foreach (var detail in purchaseMaster.PurchaseDetails)
            {
                _unitOfWork.PurchaseDetail.Remove(detail);
            }

            // Remove master
             _unitOfWork.PurchaseMaster.Remove(purchaseMaster);
             _unitOfWork.Save();

             return RedirectToAction("Index");
        }


    }
}

