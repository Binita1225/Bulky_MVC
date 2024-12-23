﻿using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
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
           // ViewBag.Greeting = "Welcome to Index Page";

            var viewModel = new PurchaseVM
            {
                PurchaseMaster = new PurchaseMaster { TransactionDate = DateTime.Now },
                PurchaseDetail = new List<PurchaseDetail> { new PurchaseDetail() },
                Products = _unitOfWork.Product.GetAll().Select(p => new PurchaseProductVM
                {
                    Id = p.Id,
                    Name = p.Title,
                    Rate = p.ListPrice,
                    RateUpto50 = p.Price,
                    RateAbove50 = p.Price50,
                    RateAbove100 = p.Price100,
                    Stock = p.Stock1
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

            // to fill the product if not product displays null
            purchaseVM.Products = _unitOfWork.Product.GetAll().Select(p => new PurchaseProductVM
            {
                Id = p.Id,
                Name = p.Title,
                Rate = p.ListPrice,
                RateUpto50 = p.Price,
                RateAbove50 = p.Price50,
                RateAbove100 = p.Price100,
                Stock = p.Stock1
            }).ToList();
            foreach (var detail in purchaseVM.PurchaseDetail)
            {
                //to validate stock
                var product = _unitOfWork.Product.Get(x => x.Id == detail.ItemId);
                
                if (product.Stock1 < detail.Quantity)
                {
                    ModelState.AddModelError("", "Invalid product or insufficient stock.");
                    return View(purchaseVM);
                }

                // Reduce stock
                product.Stock1 -= detail.Quantity;
                _unitOfWork.Product.Update(product);

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
            TempData["success"] = "Created successfully";

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

            var products = _unitOfWork.Product.GetAll();

            var purchaseVM = new PurchaseVM
            {
                PurchaseMaster = purchaseMaster,
                PurchaseDetail = purchaseMaster.PurchaseDetails.ToList(),
                Products = products.Select(p => new PurchaseProductVM
                {
                    Id = p.Id,
                    Name = p.Title,
                    Rate = p.ListPrice,
                    RateUpto50 = p.Price,
                    RateAbove50 = p.Price50,
                    RateAbove100 = p.Price100,
                    Stock = p.Stock1
                    // Set data-rate to ListPrice in the SelectListItem Text or Group
                    // We pass rate as a data-attribute directly in the view.
                }).ToList()
            };



            return View(purchaseVM);
        }

        [HttpPost]
        public IActionResult Edit(PurchaseVM purchaseVM)
        {

            // if there is null this section of code will not work
            //if (!ModelState.IsValid)
            //{
            //    purchaseVM.Products = _unitOfWork.Product.GetAll().Select(p => new SelectListItem
            //    {
            //        Value = p.Id.ToString(),
            //        Text = p.Title
            //    }).ToList();
            //    return View(purchaseVM);
            //}

            // to fill the product if not product displays null
            purchaseVM.Products = _unitOfWork.Product.GetAll().Select(p => new PurchaseProductVM
            {
                Id = p.Id,
                Name = p.Title,
                Rate = p.ListPrice,
                RateUpto50 = p.Price,
                RateAbove50 = p.Price50,
                RateAbove100 = p.Price100,
                Stock = p.Stock1
            }).ToList();

            var purchaseMaster = _unitOfWork.PurchaseMaster.GetFirstOrDefault(p => p.Id == purchaseVM.PurchaseMaster.Id);
            if (purchaseMaster == null)
            {
                return NotFound();
            }

            // Update PurchaseMaster
            purchaseMaster.CustomerName = purchaseVM.PurchaseMaster.CustomerName;
            purchaseMaster.CustomerEmail = purchaseVM.PurchaseMaster.CustomerEmail;
            purchaseMaster.TransactionDate = purchaseVM.PurchaseMaster.TransactionDate;

            _unitOfWork.PurchaseMaster.Update(purchaseMaster);

            // Remove existing details and validate new ones
            var existingDetails = _unitOfWork.PurchaseDetail.GetAll(d => d.MasterId == purchaseMaster.Id);
            foreach (var detail in existingDetails)
            {
                _unitOfWork.PurchaseDetail.Remove(detail);
            }

            // Add new PurchaseDetail items
            foreach (var detail in purchaseVM.PurchaseDetail)
            {
                var product = _unitOfWork.Product.Get(x => x.Id == detail.ItemId);

                //if (product.Stock1 < detail.Quantity)
                //{
                //    ModelState.AddModelError("", "Invalid product or insufficient stock.");
                //    return View(purchaseVM);
                //}

                //to add previous quantity to stock and decrease new quantity from it
                var previousDetail = _unitOfWork.PurchaseDetail.Get(a  => a.Id == detail.Id);

                if (previousDetail != null)
                {
                    product.Stock1 = product.Stock1 + previousDetail.Quantity;

                    if (product.Stock1 < detail.Quantity)
                    {
                        ModelState.AddModelError("", "Invalid product or insufficient stock.");
                        return View(purchaseVM);
                    }
                    
                }
                else
                {
                    if (product.Stock1 < detail.Quantity)
                    {
                        return View(purchaseVM);
                    }


                }

                // Reduce stock
                product.Stock1 -= detail.Quantity;
                _unitOfWork.Product.Update(product);

                //product.Stock1 -= detail.Quantity;
                //_unitOfWork.Product.Update(product);

                if (detail.ItemId > 0 && detail.Quantity > 0 && detail.Rate > 0)
                {
                    detail.MasterId = purchaseMaster.Id;
                    detail.Total = detail.Quantity * detail.Rate;
                    _unitOfWork.PurchaseDetail.Add(detail);
                }
            }

            _unitOfWork.Save();
            TempData["success"] = "Purchase Updated";
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
                var product = _unitOfWork.Product.Get(x => x.Id == detail.ItemId);
                var previousDetail = _unitOfWork.PurchaseDetail.GetAll(a => a.Id == detail.Id);
                if (product != null) 
                {
                    product.Stock1 += detail.Quantity;
                    _unitOfWork.Product.Update(product);
                }

                _unitOfWork.PurchaseDetail.Remove(detail);
            }

            // Remove master
             _unitOfWork.PurchaseMaster.Remove(purchaseMaster);
             _unitOfWork.Save();
            TempData["success"] = "Deleted Successfully";

             return RedirectToAction("Index");
        }


    }
}

