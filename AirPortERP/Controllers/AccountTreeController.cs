using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AirPortERP.Models;
using AirPortERP.ViewModels;
using AirPortERP.Extenstion;
using System.Net;

namespace AirPortERP.Controllers
{
    public class AccountTreeController : Controller
    {

        Entities db = new Entities();
        ClsAccount ClsAccount = new ClsAccount();
        // GET: AccountTree
        public ActionResult Index()
        {
            return View();
        }



        public ActionResult GetAccountTree()
        {
            IQueryable<AccountTree> Tree = db.AccountTrees;
            var AccountTreeList = new List<TreeViewModel>();
            foreach (var item in Tree)
            {
                //AirPortsList.Add(new AirPortsViewMosels
                //{
                TreeViewModel r = new TreeViewModel();
                r.Id = item.Id;
                r.Name = item?.AccName;

                if (item.AccParent > 0 || item.AccParent != null) 
                {
                    r.ParentName = ClsAccount.GetAccParentName(item.AccParent);
                }
                else
                {
                    r.ParentName = "Not Define";
                }
                if (item.AccGroupId > 0)
                {
                    r.GroupName = item.AccountGroup.Name;
                }
                else
                {
                    r.GroupName = "حساب رئيسي";
                }
                if (item.AccType == 1)
                {
                    r.AccTypeName = "رئيسي";
                }
                else
                {
                    r.AccTypeName = "فرعي";
                }
                r.ParentNameList = ClsAccount.GetParentNameList(Convert.ToDecimal(item.AccParent));
                r.AccNumber = Convert.ToDecimal(item.AccNumber);

                //});
                AccountTreeList.Add(r);
            }

            return Json(new { data = AccountTreeList.ToList() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAccountTreeParial(int? id)
        {

            TreeAddViewModel model = new TreeAddViewModel();
            //ViewBag.AccGroupId = new SelectList(db.AccountGroups, "Id", "Name");

            List<AccountGroup> AccountGrouplist = new List<AccountGroup>();
            AccountGrouplist = db.AccountGroups.ToList();
            AccountGrouplist.Insert(0, new AccountGroup() { Name = "--Select Account Group--", Id = 0 });
            ViewBag.AccGroupId = new SelectList(AccountGrouplist, "Id", "Name");

            //=============================================================================

            List<AccountTree> AccTreelist = new List<AccountTree>();
            AccTreelist = db.AccountTrees.ToList();
            AccTreelist.Insert(0, new AccountTree() { AccName = "--Select Account Parent--", Id = 0 });
            ViewBag.AccParent = new SelectList(AccTreelist, "Id", "AccName");


            return PartialView("_AddTree", model);

        }

        public ActionResult CreateTreeNode(TreeAddViewModel model)
        {
            if (ModelState.IsValid)
            {

                //bool IsHasAirline = db.AirLines.Any(x => x.AccId == model.AccParent);
                //if (IsHasAirline)
                //{
                  
                //    return Json(new { success = true, message = "UsedAirline" }, JsonRequestBehavior.AllowGet);
                //}

                //------------------------check is Account is prevent--------------------------------------------------------------

                //AirLine AirLine = db.AirLines.Where(x => x.AccId != null).FirstOrDefault();
                //if (AirLine != null)
                //{
                //    bool IsHasAirline = db.AccountTrees.Any(x => x.Id == AirLine.AccId);
                //    if (IsHasAirline)
                //    {
                //        return Json(new { success = true, message = "UsedAirline" }, JsonRequestBehavior.AllowGet);
                //    }


                //}

                if (model.AccParent == 6)
                {
                       return Json(new { success = true, message = "UsedAirline" }, JsonRequestBehavior.AllowGet);
                }
                if (model.AccGroupId == 1)
                {
                    return Json(new { success = true, message = "UsedAirline" }, JsonRequestBehavior.AllowGet);
                }


                //------------------------------------------------------------------------------------



                if (db.AccountTrees.Any(x => x.AccName == model.AccName))
                {
                    return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
                }
                decimal AccNumberParent;
                AccountTree AccountTreeParent = db.AccountTrees.Find(model.AccParent);
                if (AccountTreeParent == null)
                {
                     AccNumberParent = 0;
                }
                else {
                    AccNumberParent =  Convert.ToDecimal(AccountTreeParent.AccNumber);
                }
                int Id = Convert.ToInt32(db.SpLastAccountTree().FirstOrDefault());
                decimal AccNumber = Convert.ToInt32(db.GetAccNumber(AccNumberParent).FirstOrDefault());
                AccountTree AccountTree = new AccountTree();
                AccountTree.Id = Id;

                AccountTree.AccNumber = AccNumber;
                AccountTree.AccName = model.AccName;
                AccountTree.AccParent = ClsAccount.GetAccNumberForParent(model.AccParent);
                AccountTree.AccLevel = ClsAccount.GetAccParentLevel(model.AccParent) + 1;

                if (model.AccGroupId == 2)
                {
                    Bank bank = new Bank();
                    int BankId = Convert.ToInt32(db.SpLastBank().FirstOrDefault());
                    bank.Id = BankId;
                    bank.Name = model.AccName;
                    bank.AccId = Id;

                    AccountTree.AccGroupId = 2;
                    db.Banks.Add(bank);
                }
                if (model.AccGroupId == 3)
                {
                    Fund Fund = new Fund();
                    int FundId = Convert.ToInt32(db.SpLastfund().FirstOrDefault());
                    Fund.Id = FundId;
                    Fund.Name = model.AccName;
                    Fund.AccId = Id;

                    AccountTree.AccGroupId = 3;
                    db.Funds.Add(Fund);
                  
                }
                if (model.AccGroupId == 4)
                {
                    AccountTree.AccGroupId = 4;
                }
                if (model.AccGroupId == 5)
                {
                    AccountTree.AccGroupId = 5;
                }

                if (model.AccGroupId == 0)
                {
                    AccountTree.AccType = 1;
                }
                else {
                    AccountTree.AccType = 2;
                }
                    db.AccountTrees.Add(AccountTree);
               db.SaveChanges();

               


               return Json(new { success = true, message = "Add" }, JsonRequestBehavior.AllowGet);
                }

            
            return Json(new { success = false, message = "error" }, JsonRequestBehavior.AllowGet);
        }



        public ActionResult GetAccountTreeUpdateParial(int? id)
        {
            TreeUpdateViewModel model = new TreeUpdateViewModel();
            if (id > 0)
            {
                AccountTree AccountTree = db.AccountTrees.Find(id);
                model.Id = AccountTree.Id;
                model.AccName = AccountTree.AccName;
                model.AccParentName = ClsAccount.GetAccParentName(AccountTree.AccParent);
                model.AccNumber = Convert.ToDecimal(AccountTree.AccNumber);
                //model.OldAccGroupId = Convert.ToInt32(AccountTree.AccGroupId);



               // ViewBag.AccGroupId = new SelectList(db.AccountGroups, "Id", "Name");
                //ViewBag.AccParent = new SelectList(db.AccountTrees, "Id", "AccName");

            }

            return PartialView("_EditTree", model);

        }


        public ActionResult UpdateTreeNode(TreeUpdateViewModel model)
        {
            if (ModelState.IsValid)
            {



                if (db.AccountTrees.Any(x => x.AccName == model.AccName && x.Id != model.Id))
                {
                    return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
                }
                AccountTree AccountTree = db.AccountTrees.Find(model.Id);
                AccountTree.AccName = model.AccName;

                if (AccountTree.AccType == 2)
                {
                    // اذا الحساب الفرعي حساب تابع لشركات الطيران
                    if (AccountTree.AccGroupId == 1)
                    {
                        AirLine AirLine = db.AirLines.Where(x => x.AccId == AccountTree.Id).FirstOrDefault();
                        AirLine.ArName = model.AccName;
                    }
                    // اذا الحساب الفرعي حساب تابع  للبنوك
                    if (AccountTree.AccGroupId == 2)
                    {
                        Bank Bank = db.Banks.Where(x => x.AccId == AccountTree.Id).FirstOrDefault();
                        Bank.Name = model.AccName;
                    }

                    // اذا الحساب الفرعي حساب تابع  للصناديق او الخزيتة
                    if (AccountTree.AccGroupId == 3)
                    {
                        Fund Fund = db.Funds.Where(x => x.AccId == AccountTree.Id).FirstOrDefault();
                        Fund.Name = model.AccName;
                    }
                }

               



                db.Entry(AccountTree).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, message = "Update" }, JsonRequestBehavior.AllowGet);
            }


            return Json(new { success = false, message = "error" }, JsonRequestBehavior.AllowGet);
        }






        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountTree AccountTree = db.AccountTrees.Find(id);
            if (AccountTree == null)
            {
                return HttpNotFound();
            }

            if (db.AirLines.Any(x => x.AccId == AccountTree.Id))
            {
                return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
            }

            //check is Has child
            if (db.AccountTrees.Any(x => x.AccParent == AccountTree.AccNumber))
            {
               
                return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
            }

            if (db.Banks.Any(x => x.AccId == AccountTree.Id))
            {
                return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
            }

            //check is Has child
            if (db.Funds.Any(x => x.AccId == AccountTree.Id))
            {

                return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
            }



            var AccountTreeDelete = db.AccountTrees.Find(id);
            db.AccountTrees.Remove(AccountTreeDelete);
            db.SaveChanges();
            return Json(new { success = true, message = "Delete" }, JsonRequestBehavior.AllowGet);
        }








        public ActionResult GetTreeParial(int? id)
        {
            return PartialView("_Tree");

        }














    }
}