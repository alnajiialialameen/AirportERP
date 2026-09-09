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
    public class HandlingAgentsCompaniesController : Controller
    {
        private Entities db = new Entities();
        private CommonUsed CommonUsed = new CommonUsed();
        private ClsAccount clsAccount = new ClsAccount();
        
        public ActionResult Index()
        {
            return View();
        }

        //public ActionResult GetAllHandlingAgentsCompanies()
        //{
        //    db.Configuration.ProxyCreationEnabled = false;
        //    var HandlingAgentsCompaniesList = db.HandlingAgentsCompanies.ToList();


        //    return Json(new { data = HandlingAgentsCompaniesList }, JsonRequestBehavior.AllowGet);

        //}


        public ActionResult GetAllHandlingAgentsCompanies()
        {

            var HandlingAgentsCompaniesList = db.HandlingAgentsCompanies.Include(q=>q.AccountTree).ToList();
            var ExecuteCourseViewModels = new List<HandlingAgentsCompaniesViewModels>();
            foreach (var item in HandlingAgentsCompaniesList)
            {
                ExecuteCourseViewModels.Add(new HandlingAgentsCompaniesViewModels
                {
                    //  DateTime.Now.Date.ToString("MM/dd/yyyy");

                    Id = item.Id,
                    AccId = item.AccId,
                    NameEn = item.NameEn,
                    NameAr = item.NameAr,
                    Email = item.Email,
                    Phone = item.Phone,
                    Address = item.Address,
                   



                    IsActive = GetName(Convert.ToBoolean(item.IsActive)),
                });

            }

            return Json(new { data = ExecuteCourseViewModels }, JsonRequestBehavior.AllowGet);

        }











        public ActionResult GetHandlingAgentsCompanyParial(int? id)
        {

            HandlingAgentsCompany HandlingAgentsCompany = new HandlingAgentsCompany();
            if (id > 0)
            {

                HandlingAgentsCompany = db.HandlingAgentsCompanies.Find(id);
                
            }

            ViewBag.AccId = new SelectList(db.AccountTrees.Where(x => x.Id == 35), "Id", "AccName", 35);

            return PartialView("_HandlingAgentsCompanyParial", HandlingAgentsCompany);

        }





        public ActionResult CreateOrUpdateHandlingAgentsCompany(HandlingAgentsCompany model)
        {
            if (ModelState.IsValid)
            {

                if (model.Id > 0)
                {
                    if (db.HandlingAgentsCompanies.Any(x => x.NameEn == model.NameEn && x.NameEn!=model.NameEn))
                        
                    {
                        return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
                    }

                    //---------------------------------------------------
                    var handlingAgentCompany = db.HandlingAgentsCompanies.AsNoTracking().FirstOrDefault(q=> q.Id == model.Id);
                    if (handlingAgentCompany.AccId == null)
                    {
                        AccountTree AccountTreeParent = db.AccountTrees.Find(model.AccId);

                        int Id = Convert.ToInt32(db.SpLastAccountTree().FirstOrDefault());
                        decimal AccNumber = Convert.ToInt32(db.GetAccNumber(AccountTreeParent.AccNumber).FirstOrDefault());
                        AccountTree AccountTree = new AccountTree();

                        model.AccId = Id;
                        AccountTree.Id = Id;
                        AccountTree.AccNumber = AccNumber;
                        AccountTree.AccName = model.NameEn;
                        AccountTree.AccParent = AccountTreeParent.Id;
                        AccountTree.AccType = 2;
                        AccountTree.AccGroupId = 5;

                        AccountTree.AccLevel = clsAccount.GetAccParentLevel(AccountTreeParent.Id) + 1;
                        db.AccountTrees.Add(AccountTree);
                    }

                    ///----------------------------------------------------------------
                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Update" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (db.HandlingAgentsCompanies.Any(x => x.NameEn == model.NameEn))
                    {
                        return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
                    }


                    AccountTree AccountTreeParent = db.AccountTrees.Find(model.AccId);

                    int Id = Convert.ToInt32(db.SpLastAccountTree().FirstOrDefault());
                    decimal AccNumber = Convert.ToInt32(db.GetAccNumber(AccountTreeParent.AccNumber).FirstOrDefault());
                    AccountTree AccountTree = new AccountTree();

                    model.AccId = Id;

                    AccountTree.Id = Id;
                    AccountTree.AccNumber = AccNumber;
                    AccountTree.AccName = model.NameEn;
                    AccountTree.AccParent = AccountTreeParent.Id;
                    AccountTree.AccLevel = clsAccount.GetAccParentLevel(AccountTreeParent.Id) + 1;
                    AccountTree.AccType = 2;
                    AccountTree.AccGroupId = 5;

                    db.AccountTrees.Add(AccountTree);

                    db.HandlingAgentsCompanies.Add(model);
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
            HandlingAgentsCompany HandlingAgentsCompany = db.HandlingAgentsCompanies.Find(id);
            if (HandlingAgentsCompany == null)
            {
                return HttpNotFound();
            }

            if (db.OfficerDatas.Any(x => x.HandlingAgentId == id))
            {
                return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
            }

            var HandlingAgentsCompanyDelete = db.HandlingAgentsCompanies.Find(id);
            db.HandlingAgentsCompanies.Remove(HandlingAgentsCompanyDelete);
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
