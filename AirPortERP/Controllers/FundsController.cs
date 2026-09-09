using AirPortERP.Extenstion;
using AirPortERP.Models;
using AirPortERP.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace AirPortERP.Controllers
{
    public class FundsController : Controller
    {
        private Entities db = new Entities();
        ClsAccount ClsAccount = new ClsAccount();
        // GET: Funds
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetAllFunds()
        {

            IQueryable<Fund> funds = db.Funds;


            var FundsList = new List<FundViewModel>();
            foreach (var item in funds)
            {
                //AirPortsList.Add(new AirPortsViewMosels
                //{
                FundViewModel r = new FundViewModel();
                r.Id = item.Id;
                r.Name = item?.Name;
                r.FundAccountNumber = ClsAccount.GetAccountNumber(Convert.ToDecimal(item.AccId));

                //});
                FundsList.Add(r);
            }

            return Json(new { data = FundsList.ToList() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetFundParial(int? id)
        {

            FundAddViewModel BankAddViewModel = new FundAddViewModel();



            Fund Fund = new Fund();

            if (id > 0)
            {

                Fund = db.Funds.Find(id);
                BankAddViewModel.Id = Fund.Id;
                BankAddViewModel.Name = Fund.Name;
                BankAddViewModel.AccId = Convert.ToInt32(Fund.AccId);
            }

             /// id of Found in TreeAccount 4
            ViewBag.AccParentId = new SelectList(db.AccountTrees.Where(x => x.Id == 4), "Id", "AccName", 4);
            return PartialView("_AddEditFund", BankAddViewModel);

        }

        public ActionResult CreateOrUpdateFund(FundAddViewModel model)
        {
            if (ModelState.IsValid)
            {
                Fund Fund = new Fund();
                Fund.Id = model.Id;
                Fund.AccId = model.AccId;
                Fund.Name = model.Name;


                if (Fund.Id > 0)
                {
                    if (db.Funds.Any(x => x.Name == Fund.Name && x.Id != Fund.Id))
                    {
                        return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
                    }

                    AccountTree AccountTree = db.AccountTrees.Find(model.AccId);
                    AccountTree.AccName = Fund.Name;
                    AccountTree.AccType = 2;
                    AccountTree.AccGroupId = 3;


                    db.Entry(AccountTree).State = System.Data.Entity.EntityState.Modified;
                    db.Entry(Fund).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Update" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (db.Funds.Any(x => x.Name == Fund.Name))
                    {
                        return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
                    }
                    AccountTree AccountTreeParent = db.AccountTrees.Find(model.AccParentId);
                    AccountTree AccountTree = new AccountTree();

                    AccountTree.Id = Convert.ToInt32(db.SpLastAccountTree().FirstOrDefault());
                    AccountTree.AccNumber = db.GetAccNumber(AccountTreeParent.AccNumber).FirstOrDefault();
                    AccountTree.AccName = Fund.Name;
                    AccountTree.AccLevel = (AccountTreeParent.AccLevel) + 1;
                    AccountTree.AccParent = AccountTreeParent.Id;
                    AccountTree.AccType = 2;
                    AccountTree.AccGroupId = 3;


                    Fund.AccId = AccountTree.Id;
                    db.AccountTrees.Add(AccountTree);
                    int Id = Convert.ToInt32(db.SpLastfund().FirstOrDefault());
                    Fund.Id = Id;

                    db.Funds.Add(Fund);
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


            Fund Fund = db.Funds.Find(id);
            if (Fund == null)
            {
                return HttpNotFound();
            }

            //if (db.AirLinesFlightTrips.Any(x => x.AirPortId == id))
            //{
            //    return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
            //}
            var AccountTreeDelete = db.AccountTrees.Find(Fund.AccId);
            if (AccountTreeDelete != null)
            {
                db.AccountTrees.Remove(AccountTreeDelete);
            }

            var FundDelete = db.Funds.Find(id);
            db.Funds.Remove(FundDelete);
            db.SaveChanges();
            return Json(new { success = true, message = "Delete" }, JsonRequestBehavior.AllowGet);
        }






    }
}