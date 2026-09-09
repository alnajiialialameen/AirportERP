using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AirPortERP.Models;
using AirPortERP.ViewModels;
using AirPortERP.Extenstion;
using System.IO;
using System.Threading;
namespace AirPortERP.Controllers
{
    [Authorize(Roles = "Landing-Admin,Revenues,Super-Admin,Account")]
    public class AirLinesController : Controller
    {
        RptExtention ex = new RptExtention();
        private Entities db = new Entities();
        CommonUsed CommonUsed = new CommonUsed();
        ClsAccount ClsAccount = new ClsAccount();
        // GET: Airlines
        public ActionResult Index()
        {
            return View();
        }

        //public ActionResult GetAllAirlines()
        //{
        //    db.Configuration.ProxyCreationEnabled = false;
        //    var AirlinesList = db.Airlines.ToList();


        //    return Json(new { data = AirlinesList }, JsonRequestBehavior.AllowGet);

        //}


        public ActionResult GetAllAirlines()
        {

            var AirlinesFromDb = db.AirLines.ToList();
            var AirlineList = new List<AirLinesViewMosels>();
            foreach (var item in AirlinesFromDb)
            {
                // ExecuteCourseViewModels.Add(new AirLinesViewMosels
                // {
                //  DateTime.Now.Date.ToString("MM/dd/yyyy");
                AirLinesViewMosels r = new AirLinesViewMosels();
                r.Id = item.Id;
                r.Name = item.Name;
                r.Phone = item.Phone;
                r.Email = item.Email;
                //  StartDate = Convert.ToDateTime(item.StartDate).ToShortDateString(),
                ///  EndDate = Convert.ToDateTime(item.StartDate).ToShortDateString(),
                r.Address = item.Address;
                r.Code = item.Code;
                r.Islocal = GetIslocalName(Convert.ToBoolean(item.IsLocalCompany));
                if (item.AccId > 0) { r.AccNo = ClsAccount.GetAccountNumber(Convert.ToDecimal(item.AccId)); }


                AirlineList.Add(r);

              //  });

            }

            return Json(new { data = AirlineList }, JsonRequestBehavior.AllowGet);

        }
        
        public ActionResult GetAirlineParial(int? id)
        {
           
            AirlineViewModel model = new AirlineViewModel();
            AirLine Airline = new AirLine();
            if (id > 0)
            {

                Airline = db.AirLines.Find(id);
                model.Id = Airline.Id;
                model.Name = Airline.Name;
                model.ArName = Airline.ArName;
                model.Address = Airline.Address;
                model.Code = Airline.Code;
                model.CurrencyType = Airline.CurrencyType;
                model.IsLocalCompany = Airline.IsLocalCompany;

                model.Phone = Airline.Phone;
                model.Email = Airline.Email;
            }
            // id of Airline of in AccountTree 6
            ViewBag.AccId = new SelectList(db.AccountTrees.Where(x => x.Id == 6), "Id", "AccName", 6);

         




            return PartialView("_AddEditAirline", model);

        }
        
        public ActionResult CreateOrUpdateAirline(AirlineViewModel model)
        {
           
            if (ModelState.IsValid)
            {
                AirLine AirLine = new AirLine();
                if (model.Id > 0)
                {

                    if (db.AirLines.Any(x =>(x.Code == model.Code || x.Name == model.Name) && x.Id != model.Id))
                    {
                        return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
                    }


                    AirLine = db.AirLines.Find(model.Id);
                    //-----------------------------Account Start----------------------------
                    AirLine.Id = model.Id;
                    AirLine.Name = model.Name;
                    AirLine.ArName = model.ArName;
                    AirLine.Address = model.Address;
                    AirLine.Code = model.Code;

                    AirLine.CurrencyType = model.CurrencyType;
                    AirLine.IsLocalCompany = model.IsLocalCompany;

                    AirLine.Phone = model.Phone;
                    AirLine.Email = model.Email;
                    //------------------------------Account End-----------------------------

                    //*********************************************************************************************
                    if (AirLine.AccId == null)
                    {
                        AccountTree AccountTreeParent = db.AccountTrees.Find(model.AccId);

                        int Id = Convert.ToInt32(db.SpLastAccountTree().FirstOrDefault());
                        decimal AccNumber = Convert.ToInt32(db.GetAccNumber(AccountTreeParent.Id).FirstOrDefault());
                        AccountTree AccountTree = new AccountTree();

                        AirLine.AccId = Id;
                        AccountTree.Id = Id;
                        AccountTree.AccNumber = AccNumber;
                        AccountTree.AccName = model.Name;
                        AccountTree.AccParent = AccountTreeParent.Id;
                        AccountTree.AccType = 2;
                        AccountTree.AccGroupId = 1;

                        AccountTree.AccLevel = ClsAccount.GetAccParentLevel(AccountTreeParent.Id) + 1;
                        db.AccountTrees.Add(AccountTree);


                    }
                    //***************************************************************************************


                    db.Entry(AirLine).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Update" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (db.AirLines.Any(x => x.Code == model.Code || x.Name == model.Name))
                    {
                        return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
                    }

                    AirLine.Id = model.Id;
                    AirLine.Name = model.Name;
                    AirLine.ArName = model.ArName;
                    AirLine.Address = model.Address;
                    AirLine.Code = model.Code;
                    AirLine.CurrencyType = model.CurrencyType;
                    AirLine.IsLocalCompany = model.IsLocalCompany;

                    AirLine.Phone = model.Phone;
                    AirLine.Email = model.Email;

                    //*********************************************************************************************
                   
                        AccountTree AccountTreeParent = db.AccountTrees.Find(model.AccId);

                        int Id = Convert.ToInt32(db.SpLastAccountTree().FirstOrDefault());
                        decimal AccNumber = Convert.ToInt32(db.GetAccNumber(AccountTreeParent.AccNumber).FirstOrDefault());
                        AccountTree AccountTree = new AccountTree();

                        AirLine.AccId = Id;
                        AccountTree.Id = Id;
                        AccountTree.AccNumber = AccNumber;
                        AccountTree.AccName = model.Name;
                        AccountTree.AccParent = AccountTreeParent.Id;
                        AccountTree.AccLevel = ClsAccount.GetAccParentLevel(AccountTreeParent.Id) + 1;
                        AccountTree.AccType = 2;
                       AccountTree.AccGroupId = 1;
                    db.AccountTrees.Add(AccountTree);


                    
                    //*******


                    db.AirLines.Add(AirLine);            
                    db.SaveChanges();
                    int lastId = AirLine.Id;
                    return Json(new { success = true, message = "Add" , LastId = lastId }, JsonRequestBehavior.AllowGet);
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
            AirLine Airline = db.AirLines.Find(id);
            if (Airline == null)
            {
                return HttpNotFound();
            }

            if (db.AirLinesFlightTrips.Any(x => x.AirLineId == Airline.Id))
            {
                return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
            }

            if (Airline.AccId > 0)
            {
                bool IsHasAccount = db.AccountTrees.Any(x => x.Id == Airline.AccId);
                if (IsHasAccount)
                {
                    AccountTree AccountTree = db.AccountTrees.Find(Airline.AccId);
                    db.AccountTrees.Remove(AccountTree);
                }


            }
            var AirlineDelete = db.AirLines.Find(id);
            db.AirLines.Remove(AirlineDelete);
            db.SaveChanges();
            return Json(new { success = true, message = "Delete" }, JsonRequestBehavior.AllowGet);
        }


        public string GetIslocalName(bool Islocal)
        {
            string StatusName;
            if (Islocal == false)
            {
                StatusName = "Foreign";
            }
            else
            {
                StatusName = "Local";
            }
            return StatusName;
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
