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
    public class BanksController : Controller
    {
        private Entities db = new Entities();
        ClsAccount ClsAccount = new ClsAccount();
        // GET: Bank
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetAllBanks()
        {

            IQueryable<Bank> banks = db.Banks;


            var banksList = new List<BankViewModel>();
            foreach (var item in banks)
            {
                //AirPortsList.Add(new AirPortsViewMosels
                //{
                BankViewModel r = new BankViewModel();
                r.Id = item.Id;
                r.Name = item?.Name;
                r.BankAccountNumber = ClsAccount.GetAccountNumber(Convert.ToDecimal(item.AccId));

                //});
                banksList.Add(r);
            }

            return Json(new { data = banksList.ToList() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBankParial(int? id)
        {

            BankAddViewModel BankAddViewModel = new BankAddViewModel();



            Bank Bank = new Bank();
          
            if (id > 0)
            {

                Bank = db.Banks.Find(id);
                BankAddViewModel.Id = Bank.Id;
                BankAddViewModel.Name = Bank.Name;
                BankAddViewModel.AccId = Convert.ToInt32(Bank.AccId);
            }
            // Id of Bank 
            ViewBag.AccParentId = new SelectList(db.AccountTrees.Where(x => x.Id == 5), "Id", "AccName", 5);
            return PartialView("_AddEditBank", BankAddViewModel);

        }

        public ActionResult CreateOrUpdateBank(BankAddViewModel model)
        {
            if (ModelState.IsValid)
            {
                Bank bank = new Bank();
                bank.Id = model.Id;
                bank.AccId = model.AccId;
                bank.Name = model.Name;


                if (bank.Id > 0)
                {
                    if (db.Banks.Any(x => x.Name == bank.Name && x.Id != bank.Id))
                    {
                        return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
                    }

                    AccountTree AccountTree = db.AccountTrees.Find(model.AccId);
                    AccountTree.AccName = bank.Name;
                    AccountTree.AccType = 2;
                    AccountTree.AccGroupId = 2;

                    db.Entry(AccountTree).State = System.Data.Entity.EntityState.Modified;
                    db.Entry(bank).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Update" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (db.Banks.Any(x => x.Name == bank.Name))
                    {
                        return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
                    }
                    AccountTree AccountTreeParent = db.AccountTrees.Find(model.AccParentId);
                    AccountTree AccountTree = new AccountTree();

                    AccountTree.Id = Convert.ToInt32(db.SpLastAccountTree().FirstOrDefault());     
                    AccountTree.AccNumber = db.GetAccNumber(AccountTreeParent.AccNumber).FirstOrDefault();
                    AccountTree.AccName = bank.Name;
                    AccountTree.AccLevel = (AccountTreeParent.AccLevel) + 1;
                    AccountTree.AccParent = AccountTreeParent.Id;
                    AccountTree.AccType = 2;
                    AccountTree.AccGroupId = 2;




                    bank.AccId = AccountTree.Id;
                    db.AccountTrees.Add(AccountTree);
                    int Id = Convert.ToInt32(db.SpLastBank().FirstOrDefault());
                    bank.Id = Id;

                    db.Banks.Add(bank);
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


            Bank Bank = db.Banks.Find(id);
            if (Bank == null)
            {
                return HttpNotFound();
            }

            //if (db.AirLinesFlightTrips.Any(x => x.AirPortId == id))
            //{
            //    return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
            //}
            var AccountTreeDelete = db.AccountTrees.Find(Bank.AccId);
            if (AccountTreeDelete != null)
            {
                db.AccountTrees.Remove(AccountTreeDelete);
            }

            var BankDelete = db.Banks.Find(id);
            db.Banks.Remove(BankDelete);
            db.SaveChanges();
            return Json(new { success = true, message = "Delete" }, JsonRequestBehavior.AllowGet);
        }








    }
}