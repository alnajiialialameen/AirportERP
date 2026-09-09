using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AirPortERP.Models;
using System.IO;
using AirPortERP.ViewModels;
using AirPortERP.Extenstion;

namespace AirPortERP.Controllers
{
    public class FuelCompaniesController : Controller
    {
        private Entities db = new Entities();
        private CommonUsed CommonUsed = new CommonUsed();
        ClsAccount ClsAccount = new ClsAccount();
        
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult GetAllFuelCompanies()
        {
            var FuelCompaniesList = db.FuelCompanies.Where(x => x.IsActive != false).ToList();
            var HAViewModels = new List<HandlingAgentsCompaniesViewModel>();
            foreach (var item in FuelCompaniesList)
            {
                HandlingAgentsCompaniesViewModel obj = new HandlingAgentsCompaniesViewModel();

                obj.Id = item.Id;
                obj.NameEn = item.NameEn;
                obj.NameAr = item.NameAr;
                obj.Phone = item.Phone;
                obj.Email = item.Email;
                obj.Address = item.Address;
                obj.IsActive = GetName(Convert.ToBoolean(item.IsActive));
                if (item.AccId > 0) { obj.AccId = ClsAccount.GetAccountNumber(Convert.ToDecimal(item.AccId)); }

                HAViewModels.Add(obj);
            }

            return Json(new { data = HAViewModels }, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult GetFuelCompanyParial(int? id)
        {
            FuelCompany FuelCompany = new FuelCompany();
            if (id > 0)
            {
                FuelCompany = db.FuelCompanies.Find(id);
                
            }

            ViewBag.AccId = new SelectList(db.AccountTrees, "Id", "AccName", 6);

            return PartialView("_FuelCompanyParial", FuelCompany);
        }
        
        public ActionResult CreateOrUpdateFuelCompany(FuelCompany model)
        {
            if (ModelState.IsValid)
            {
                FuelCompany fuelCompany = new FuelCompany();
                if (model.Id > 0)
                {
                    fuelCompany = db.FuelCompanies.Find(model.Id);
                    //-----------------------------Account Start----------------------------
                    fuelCompany.Id = model.Id;
                    fuelCompany.NameEn = model.NameEn;
                    fuelCompany.NameAr = model.NameAr;
                    fuelCompany.Address = model.Address;
                    fuelCompany.Phone = model.Phone;
                    fuelCompany.Email = model.Email;
                    fuelCompany.IsActive = true;

                    fuelCompany.CurrencyId = model.CurrencyId;
                    //------------------------------Account End-----------------------------

                    //*********************************************************************************************
                    if (fuelCompany.AccId == null)
                    {
                        AccountTree AccountTreeParent = db.AccountTrees.Find(model.AccId);

                        int Id = Convert.ToInt32(db.SpLastAccountTree().FirstOrDefault());
                        decimal AccNumber = Convert.ToInt32(db.GetAccNumber(AccountTreeParent.AccNumber).FirstOrDefault());
                        AccountTree AccountTree = new AccountTree();

                        fuelCompany.AccId = Id;
                        AccountTree.Id = Id;
                        AccountTree.AccNumber = AccNumber;
                        AccountTree.AccName = model.NameEn;
                        AccountTree.AccParent = AccountTreeParent.Id;
                        AccountTree.AccType = 2;
                        AccountTree.AccGroupId = 1;

                        AccountTree.AccLevel = ClsAccount.GetAccParentLevel(AccountTreeParent.Id) + 1;
                        db.AccountTrees.Add(AccountTree);

                    }

                    db.Entry(fuelCompany).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Update" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    fuelCompany.Id = model.Id;
                    fuelCompany.NameEn = model.NameEn;
                    fuelCompany.NameAr = model.NameAr;
                    fuelCompany.Address = model.Address;
                    fuelCompany.CurrencyId = model.CurrencyId;
                    fuelCompany.Phone = model.Phone;
                    fuelCompany.Email = model.Email;

                    //*********************************************************************************************

                    AccountTree AccountTreeParent = db.AccountTrees.Find(model.AccId);

                    int Id = Convert.ToInt32(db.SpLastAccountTree().FirstOrDefault());
                    decimal AccNumber = Convert.ToInt32(db.GetAccNumber(AccountTreeParent.AccNumber).FirstOrDefault());
                    AccountTree AccountTree = new AccountTree();

                    fuelCompany.AccId = Id;
                    AccountTree.Id = Id;
                    AccountTree.AccNumber = AccNumber;
                    AccountTree.AccName = model.NameEn;
                    AccountTree.AccParent = AccountTreeParent.Id;
                    AccountTree.AccLevel = ClsAccount.GetAccParentLevel(AccountTreeParent.Id) + 1;
                    AccountTree.AccType = 2;
                    AccountTree.AccGroupId = 1;

                    db.AccountTrees.Add(AccountTree);
                    db.FuelCompanies.Add(fuelCompany);
                    db.SaveChanges();

                    return Json(new { success = true, message = "Add" }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { success = false, message = "error" }, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            FuelCompany fuelCompany = db.FuelCompanies.Find(id);
            if (fuelCompany == null)
            {
                return HttpNotFound();
            }

            if (db.OfficerDatas.Any(x => x.FuelCompanyId == id))
            {
                return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
            }

            if (fuelCompany.AccId > 0)
            {
                bool IsHasAccount = db.AccountTrees.Any(x => x.Id == fuelCompany.AccId);
                if (IsHasAccount)
                {
                    AccountTree AccountTree = db.AccountTrees.Find(fuelCompany.AccId);
                    db.AccountTrees.Remove(AccountTree);
                }
            }

            var fuelCompanyDelete = db.FuelCompanies.Find(id);
            db.FuelCompanies.Remove(fuelCompanyDelete);
            db.SaveChanges();
            return Json(new { success = true, message = "Delete" }, JsonRequestBehavior.AllowGet);
        }
        
        public string GetName(bool IsActive)
        {
            string StatusName;
            if (IsActive == false)
            {
                StatusName = "Not Active";
            }
            else
            {
                StatusName = "Active";
            }
            return StatusName;
        }
        
        /*---------------------------------------------------------------------------------------------------------------*/
        
        // GET: HandlingAgentsCompanies/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HandlingAgentsCompany handlingAgentsCompany = db.HandlingAgentsCompanies.Find(id);
            if (handlingAgentsCompany == null)
            {
                return HttpNotFound();
            }
            return View(handlingAgentsCompany);
        }

        // GET: HandlingAgentsCompanies/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: HandlingAgentsCompanies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,NameAr,NameEn,Address,IsActive")] HandlingAgentsCompany handlingAgentsCompany)
        {
            if (ModelState.IsValid)
            {
                db.HandlingAgentsCompanies.Add(handlingAgentsCompany);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(handlingAgentsCompany);
        }

        // GET: HandlingAgentsCompanies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HandlingAgentsCompany handlingAgentsCompany = db.HandlingAgentsCompanies.Find(id);
            if (handlingAgentsCompany == null)
            {
                return HttpNotFound();
            }
            return View(handlingAgentsCompany);
        }

        // POST: HandlingAgentsCompanies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,NameAr,NameEn,Address,IsActive")] HandlingAgentsCompany handlingAgentsCompany)
        {
            if (ModelState.IsValid)
            {
                db.Entry(handlingAgentsCompany).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(handlingAgentsCompany);
        }

        // GET: HandlingAgentsCompanies/Delete/5
     

        // POST: HandlingAgentsCompanies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            HandlingAgentsCompany handlingAgentsCompany = db.HandlingAgentsCompanies.Find(id);
            db.HandlingAgentsCompanies.Remove(handlingAgentsCompany);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
