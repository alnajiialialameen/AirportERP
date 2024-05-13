using AirPortERP.Extenstion;
using AirPortERP.Models;
using AirPortERP.ViewModels;
using Microsoft.AspNet.Identity;
using NumberToWord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AirPortERP.Controllers
{
    public class AccountSandSarfController : Controller
    {
        ClsAccount cls = new ClsAccount();
        Entities db = new Entities();
        CommonUsed CommonUsed = new CommonUsed();
        RoleController Role = new RoleController();
        List<CurrencyInfo> currencies = new List<CurrencyInfo>();
        // GET: AccountSand
        
        public AccountSandSarfController()
        {

            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Sudan));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Dollar));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Syria));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.UAE));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.SaudiArabia));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Tunisia));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Gold));



        }


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ConfirmSandSarfTransation()
        {
            return View();
        }


        public ActionResult GetAllSandSarfConfirm()
        {
            var id = User.Identity.GetUserId();


            IQueryable<SandDetail> SandDetail = db.SandDetails.Where(x => x.SandMain.OperationTypeId == 1);
            SandDetail = SandDetail.Where(x => x.SandMain.IsTransfer == true);

            //IQueryable<SandDetail> SandDetail = db.SandDetails.Where(x => x.SandMain.IsTransfer == true);
            //SandDetail = SandDetail.Where(x => x.SandMain.OperationTypeId == 1);
            //SandDetail = SandDetail.Where(x => x.SandMain.ISDeleted == false || x.SandMain.ISDeleted == null);

            var SandList = new List<SadViewModel>();
            foreach (var item in SandDetail)
            {

                SadViewModel r = new SadViewModel();
                r.Id = item.Id;

                r.AccId = Convert.ToInt32(item.AccId);
                if (item.AccId > 0)
                {
                    r.TreeAccountNumber = cls.GetAccountNumber(Convert.ToDecimal(item.AccId));
                    r.AccName = item?.AccountTree.AccName;
                }
                r.TotalAmountText = GetAmountTextString(Convert.ToInt32(item.CurrencyId), Convert.ToDecimal(item.Amount));
                r.Date = cls.GetDateddMMyyyy(item.SandMain.SandDate.ToString());
                r.Statement = item?.Statement;
                r.CurrencyName = item?.Currency.CurrencyName;
                r.IsConfirm = Convert.ToBoolean(item.SandMain.IsConfirm);

                if (item.YearId > 0)
                {
                    r.Year = item?.YearWorkOn.Year.ToString();
                }
                else
                {
                    r.Year = "غير معروف";
                }
                if (item.YearId > 0)
                {
                    r.Month = item?.MouthWorkOn.Mouth.ToString();
                }
                else
                {
                    r.Month = "غير معروف";
                }



                r.TotalAmountCredit = Convert.ToDecimal(item?.Amount).ToString("N");
                r.Role = Role.GetRole(id);
                //});
                SandList.Add(r);
            }

            return Json(new { data = SandList.ToList() }, JsonRequestBehavior.AllowGet);
        }





        public ActionResult GetAllSandSarf()
        {
            IQueryable<SandDetail> SandDetail = db.SandDetails.Where(x => x.SandMain.OperationTypeId == 1);
            SandDetail = SandDetail.Where(x => x.SandMain.IsTransfer == false || x.SandMain.IsTransfer == null);
            //  SandDetail = SandDetail.Where(x => x.SandMain.ISDeleted == false || x.SandMain.ISDeleted == null);

            var SandList = new List<SadViewModelSarf>();
            foreach (var item in SandDetail)
            {

                SadViewModelSarf r = new SadViewModelSarf();
                r.Id = item.Id;
                r.AccId = Convert.ToInt32(item.AccId);
                if (item.AccId > 0)
                {
                    r.TreeAccountNumber = cls.GetAccountNumber(Convert.ToDecimal(item.AccId));
                    r.AccName = item?.AccountTree.AccName;
                }
                r.TotalAmountText = GetAmountTextString(Convert.ToInt32(item.CurrencyId), Convert.ToDecimal(item.Amount));
                r.Date = cls.GetDateddMMyyyy(item.SandMain.SandDate.ToString());
                r.Statement = item?.Statement;
                r.CurrencyName = item?.Currency.CurrencyName;
                r.IsConfirm = Convert.ToBoolean(item.SandMain.IsConfirm);
                if (item.YearId > 0)
                {
                    r.Year = item?.YearWorkOn.Year.ToString();
                }
                else
                {
                    r.Year = "غير معروف";
                }
                if (item.YearId > 0)
                {
                    r.Month = item?.MouthWorkOn.Mouth.ToString();
                }
                else
                {
                    r.Month = "غير معروف";
                }


                r.TotalAmountCredit = Convert.ToDecimal(item?.Amount).ToString("N");

                //});
                SandList.Add(r);
            }

            return Json(new { data = SandList.ToList() }, JsonRequestBehavior.AllowGet);
        }



        public ActionResult GetSandParialSarf(int? id)
        {

            SandDetail SandDetail = new SandDetail();
            SandAddViewModelSarf model = new SandAddViewModelSarf();


            model.Date = DateTime.Today.Date;

            if (id > 0)
            {

                SandDetail = db.SandDetails.Find(id);

                model.Id = SandDetail.Id;
                model.AccIdDetail = Convert.ToInt32(SandDetail.AccId);
                model.AccIdMain = Convert.ToInt32(SandDetail.SandMain.AccId);

                model.Id = Convert.ToInt32(SandDetail.Id);
                model.MainId = Convert.ToInt32(SandDetail.SandMain.Id);

               // model.ISCash = Convert.ToBoolean(SandDetail.ISCash);
               // model.ShakNumber = SandDetail.ShakNumber;
                model.Statement = SandDetail.Statement;

                model.TotalAmountCredit = SandDetail.SandMain.TotalAmount;

                model.CurrencyId = Convert.ToInt32(SandDetail.CurrencyId);


                model.YearId = Convert.ToInt32(SandDetail.YearId);
                model.MonthId = Convert.ToInt32(SandDetail.MonthId);

                model.Date = Convert.ToDateTime(SandDetail.SandMain.SandDate);
                //  model.Note = Convert.ToDecimal(OpenAccBalance.Credit);
              //  ViewBag.AccIdMain = new SelectList(db.AccountTrees.Where(x => x.AccGroupId == 3 || x.AccGroupId == 2), "Id", "AccName", model.AccIdMain);
              //  ViewBag.AccIdDetail = new SelectList(db.AccountTrees.Where(x => x.AccGroupId == 1), "Id", "AccName", model.AccIdDetail);
              //  ViewBag.CurrencyId = new SelectList(db.Currencies, "Id", "CurrencyName", model.CurrencyId);


                //---------------------------------------Year && Month---------------------------------------------------------


            }
         


            List<AccountTree> Mainlist = new List<AccountTree>();
            Mainlist = db.AccountTrees.Where(x => x.AccGroupId == 2 || x.AccGroupId == 3).ToList();
            Mainlist.Insert(0, new AccountTree() { AccName = "--Select Main Account--", Id = 0 });
            ViewBag.AccIdMain = new SelectList(Mainlist, "Id", "AccName", model.AccIdMain);

            List<AccountTree> Detaillist = new List<AccountTree>();
            Detaillist = db.AccountTrees.ToList();
            Detaillist.Insert(0, new AccountTree() { AccName = "--Select Sub Account--", Id = 0, AccGroupId = 4 });
            ViewBag.AccIdDetail = new SelectList(Detaillist.Where(x=>x.AccGroupId==4), "Id", "AccName", model.AccIdDetail);
            //ViewBag.AccIdDetail = new SelectList(db.AccountTrees.Where(x => x.AccGroupId == 1), "Id", "AccName", model.AccIdDetail);


            ViewBag.CurrencyId = new SelectList(db.Currencies, "Id", "CurrencyName", model.CurrencyId);


            IQueryable<YearWorkOn> Years = db.YearWorkOns;
            var YearList = new List<YearViewModel>();
            foreach (var item in Years)
            {
                YearViewModel r = new YearViewModel();
                r.Id = item.Id;
                r.Year = item.Year.ToString();
                YearList.Add(r);
            }

            YearList.Insert(0, new YearViewModel() { Year = "--Select Year Claim--", Id = 0 });
            ViewBag.YearId = new SelectList(YearList, "Id", "Year", model.YearId);


            List<MouthWorkOn> MouthList = new List<MouthWorkOn>();
            MouthList = db.MouthWorkOns.ToList();
            MouthList.Insert(0, new MouthWorkOn() { Mouth = "--Select Mouth Claim--", Id = 0 });
            ViewBag.MonthId = new SelectList(MouthList, "Id", "Mouth", model.MonthId);





            return PartialView("_AddEditSandSarf", model);

        }


        public ActionResult CreateOrUpdateSandSarf(SandAddViewModelSarf model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                SandDetail SandDetail = new SandDetail();
                SandMain SandMain = new SandMain();
                AccountTree Tree = db.AccountTrees.Find(model.AccIdMain);

                var MouthObj = db.MouthWorkOns.FirstOrDefault(x => x.Id == model.Date.Month);
                var YearObj = db.YearWorkOns.FirstOrDefault(x => x.Year == model.Date.Year);

                int Mouth = Convert.ToInt32(MouthObj.Id);
                int Year = Convert.ToInt32(YearObj.Year);

                model.MonthId = Mouth;
                model.YearId = Convert.ToInt32(YearObj.Id);


                if (model.Id > 0)
                {
                    //if (db.OpenAccBalances.Any(x => x.AccId == model.AccId && x.Id != model.Id))
                    //{
                    //    return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
                    //}
                    SandMain = db.SandMains.Find(model.MainId);
                    SandMain.AccId = model.AccIdMain;
                    SandMain.SandDate = model.Date;
                    SandMain.TotalAmount = model.TotalAmountCredit;
                    SandMain.UpdatedBy = userId;
                    SandMain.UpdatingDate = DateTime.Now;

                    //----------------------Deatils  ------------------------------------------------------------------------------------------------------
                    SandDetail = db.SandDetails.Find(model.Id);

                    SandDetail.AccId = model.AccIdDetail;
                    //SandDetail.ISCash = model.ISCash;
                    SandDetail.CurrencyId = model.CurrencyId;
                    SandDetail.YearId = model.YearId;
                    SandDetail.MonthId = model.MonthId;
                    SandDetail.MonthId = model.MonthId;


                    if (Tree.AccGroupId ==3)
                    {
                        SandDetail.ISCash = true;
                    }
                    else
                    {
                        SandDetail.ISCash = false;
                    }


                    SandDetail.Statement = model.Statement;
                    SandDetail.Amount = model.TotalAmountCredit;


                    ///-------------------------------------------------------------------------------------------------------------------------
                    bool IsOperation = db.AccountOperations.Any(x => x.OperationID == model.Id && x.OperationTypeId == 1);
                    if (IsOperation)
                    {
                        //delete old operation
                        List<AccountOperation> Operation = db.AccountOperations.Where(x => x.OperationTypeId == 1 && x.OperationID == model.Id).ToList();


                        foreach (var item in Operation)
                        {
                            db.AccountOperations.Remove(item);
                        }

                        //Add Opertion  operation
                        //----------------------------------------------------------------------------------------------------------------------------------------
                        ///----------------------------------عمل قيد مزدوج------------------------------------------
                        AccountOperation OperationDebit = new AccountOperation();
                        AccountOperation OperationCredit = new AccountOperation();

                        int Id = Convert.ToInt32(db.SpLastAccountOperationId().FirstOrDefault());
                        OperationDebit.Id = Id;

                        OperationDebit.AccountNumber = model.AccIdMain;
                        OperationDebit.OperationDebit = 0;
                        OperationDebit.OperationCredit = model.TotalAmountCredit;
                        OperationDebit.OperationDate = model.Date;
                        //OperationDebit.OperationDate = SandMain.SandDate;
                        OperationDebit.OperationID = model.Id;
                        OperationDebit.CurrencyId = model.CurrencyId;
                        OperationDebit.OperationStatement = model.Statement;
                        OperationDebit.OperationTypeId = 1;
                        OperationDebit.CreatedBy = userId;
                        OperationDebit.CreationDate = DateTime.Now;
                        ///----------------------------------------------------------------------------
                        int Id2 = Convert.ToInt32(db.SpLastAccountOperationId().FirstOrDefault() + 1);
                        OperationCredit.Id = Id2;

                        OperationCredit.AccountNumber = model.AccIdDetail;
                        OperationCredit.OperationDebit = model.TotalAmountCredit;
                        OperationCredit.OperationCredit = 0;
                        OperationCredit.OperationDate = model.Date;
                        //OperationCredit.OperationDate = SandMain.SandDate;
                        OperationCredit.OperationID = model.Id;
                        OperationCredit.CurrencyId = model.CurrencyId;
                        OperationCredit.OperationStatement = model.Statement;
                        OperationCredit.OperationTypeId = 1;
                        OperationCredit.CreatedBy = userId;
                        OperationCredit.CreationDate = DateTime.Now;

                        db.AccountOperations.Add(OperationDebit);
                        db.AccountOperations.Add(OperationCredit);

                        //------------------------------------------------------------------------------------------------------------------------------------------
                    }





                    db.Entry(SandMain).State = System.Data.Entity.EntityState.Modified;
                    db.Entry(SandDetail).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Update" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //if (db.OpenAccBalances.Any(x => x.AccId == model.AccId))
                    //{
                    //    return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
                    //}
                    // Balance.Note = cls.GetAccountName(model.AccId) + "حساب افتتاحي ل";
                    int SandMainId = Convert.ToInt32(db.SpLastSandMain().FirstOrDefault());
                    SandMain.Id = SandMainId;
                    SandMain.AccId = model.AccIdMain;
                    SandMain.OperationTypeId = 1;
                    SandMain.TotalAmount = model.TotalAmountCredit;
                    /// SandMain.ISDeleted = false;
                    SandMain.SandDate = model.Date;
                    SandMain.CreatedBy = userId;
                    SandMain.CreationDate = DateTime.Now;


                    ///-------------------------details -------------------------------------------------------------------------------

                    int SandDetailId = Convert.ToInt32(db.SpLastSandDetail().FirstOrDefault());
                    SandDetail.Id = SandDetailId;
                    SandDetail.AccId = model.AccIdDetail;

                    SandDetail.CurrencyId = model.CurrencyId;
                    SandDetail.YearId = model.YearId;

                    SandDetail.MonthId = model.MonthId;

                    SandDetail.SandMainId = SandMainId;
                    SandDetail.Statement = model.Statement;
                    SandDetail.Amount = model.TotalAmountCredit;

                    if (Tree.AccGroupId == 3)
                    {
                        SandDetail.ISCash = true;
                    }
                    else
                    {
                        SandDetail.ISCash = false;
                    }

                    db.SandMains.Add(SandMain);
                    db.SandDetails.Add(SandDetail);
                    db.SaveChanges();
                    return Json(new { success = true, message = "Add" }, JsonRequestBehavior.AllowGet);
                }

            }
            return Json(new { success = false, message = "error" }, JsonRequestBehavior.AllowGet);
        }





       







        public ActionResult GetAcc(bool yes)
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<AccountTree> lstAccountTree = new List<AccountTree>();
            lstAccountTree = db.AccountTrees.ToList();
            if (yes)
            {
                lstAccountTree = lstAccountTree.Where(x => x.AccGroupId == 3).ToList();
            }
            else
            {
                lstAccountTree = lstAccountTree.Where(x => x.AccGroupId == 2).ToList();
            }



            lstAccountTree.Insert(0, new AccountTree() { AccName = "--Select Account--", Id = 0 });
            return Json(lstAccountTree, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetAmountText(int CurrencyId, decimal Statement)
        {
            string MoneyText = "";
            if (CurrencyId == 2)
            {
                double SDGRound = Convert.ToDouble(Math.Round(Statement, 2));

                ToWord toWord = new ToWord(Convert.ToDecimal(SDGRound), currencies[Convert.ToInt32(0)]);

                MoneyText = toWord.ConvertToArabic().ToString();

                return Json(MoneyText, JsonRequestBehavior.AllowGet);


                ///--------------------------------------Dollar-----------------------------------------------------------
            }
            else
            {
                double DollarRound = Convert.ToDouble(Math.Round(Statement, 2));
                ToWord toWord1 = new ToWord(Convert.ToDecimal(DollarRound), currencies[Convert.ToInt32(1)]);
                MoneyText = toWord1.ConvertToArabic().ToString();


                return Json(MoneyText, JsonRequestBehavior.AllowGet);
            }
        }


        public string GetAmountTextString(int CurrencyId, decimal Statement)
        {
            string MoneyText = "";
            if (CurrencyId == 2)
            {
                double SDGRound = Convert.ToDouble(Math.Round(Statement, 2));

                ToWord toWord = new ToWord(Convert.ToDecimal(SDGRound), currencies[Convert.ToInt32(0)]);

                MoneyText = toWord.ConvertToArabic().ToString();

                return MoneyText;


                ///--------------------------------------Dollar-----------------------------------------------------------
            }
            else
            {
                double DollarRound = Convert.ToDouble(Math.Round(Statement, 2));
                ToWord toWord1 = new ToWord(Convert.ToDecimal(DollarRound), currencies[Convert.ToInt32(1)]);
                MoneyText = toWord1.ConvertToArabic().ToString();


                return MoneyText;
            }
        }


        // ----------------------------------------------------------------------------------------------------------------------
     


        public ActionResult GetSandToConfirm(int id)
        {
            string MoneyText = "";
            SandDetail Sand = db.SandDetails.Find(id);

            MoneyText = GetAmountTextString(Convert.ToInt32(Sand.CurrencyId), Convert.ToDecimal(Sand.Amount));

            double value = Convert.ToDouble(Sand.Amount);
            string Message = " بقيمة " + value + cls.GetAccountNamePlus(Sand.AccId) + " سند (صرف) الي حساب";

            return Json(new { success = true, message = Message, moneytext = MoneyText }, JsonRequestBehavior.AllowGet);


            ///--------------------------------------Dollar-----------------------------------------------------------
        }




        public ActionResult GetSandToConfirmDelete(int id)
        {
            string MoneyText = "";
            SandDetail Sand = db.SandDetails.Find(id);

            MoneyText = GetAmountTextString(Convert.ToInt32(Sand.CurrencyId), Convert.ToDecimal(Sand.Amount));

            double value = Convert.ToDouble(Sand.Amount);
            string Message = value + "بقيمة" + Sand.AccountTree.AccName + "  هل تريد حذف سند قبض من حساب  ";

            return Json(new { success = true, message = Message, moneytext = MoneyText }, JsonRequestBehavior.AllowGet);


            ///--------------------------------------Dollar-----------------------------------------------------------
        }









        public ActionResult AddToConfrimSarf(int id)
        {
            // string MoneyText = "";
            SandDetail Sand = db.SandDetails.Find(id);
            Sand.SandMain.IsConfirm = true;

            ///----------------------------------عمل قيد مزدوج------------------------------------------
            AccountOperation OperationDebit = new AccountOperation();
            AccountOperation OperationCredit = new AccountOperation();

            int Id = Convert.ToInt32(db.SpLastAccountOperationId().FirstOrDefault());
            OperationDebit.Id = Id;
            OperationDebit.AccountNumber = Sand.SandMain.AccId;
            OperationDebit.OperationDebit = 0;
            OperationDebit.OperationCredit = Sand.Amount;
            OperationDebit.OperationDate = Sand.SandMain.SandDate;
            OperationDebit.OperationID = Sand.Id;
            OperationDebit.CurrencyId = Sand.CurrencyId;
            OperationDebit.OperationStatement = Sand.Statement;
            OperationDebit.OperationTypeId = Sand.SandMain.OperationTypeId;
            ///----------------------------------------------------------------------------
            int Id2 = Convert.ToInt32(db.SpLastAccountOperationId().FirstOrDefault() + 1);
            OperationCredit.Id = Id2;
            OperationCredit.AccountNumber = Sand.AccId;
            OperationCredit.OperationDebit = Sand.Amount;
            OperationCredit.OperationCredit = 0;
            OperationCredit.OperationDate = Sand.SandMain.SandDate;
            OperationCredit.OperationID = Sand.Id;
            OperationCredit.CurrencyId = Sand.CurrencyId;
            OperationCredit.OperationStatement = Sand.Statement;
            OperationCredit.OperationTypeId = Sand.SandMain.OperationTypeId;

            db.AccountOperations.Add(OperationDebit);
            db.AccountOperations.Add(OperationCredit);
            db.SaveChanges();

            return Json(new { success = true, message = "Add" }, JsonRequestBehavior.AllowGet);


            ///--------------------------------------Dollar-----------------------------------------------------------
        }




        public ActionResult Transfer(int id)
        {
            SandDetail SandDetail = db.SandDetails.Find(id);
            SandDetail.SandMain.IsTransfer = true;


            db.Entry(SandDetail).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return Json(new { success = true, message = "Add" }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult ReTransfer(int id)
        {
            SandDetail SandDetail = db.SandDetails.Find(id);
            SandDetail.SandMain.IsTransfer = false;


            db.Entry(SandDetail).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return Json(new { success = true, message = "Add" }, JsonRequestBehavior.AllowGet);

        }


        public ActionResult DeleteConfirm(int id)
        {
            SandDetail SandDetail = db.SandDetails.Find(id);
            SandMain SandMain = db.SandMains.Find(SandDetail.SandMainId);


            List<AccountOperation> Operation = db.AccountOperations.Where(x => x.OperationTypeId == 2 && x.OperationID == SandDetail.Id).ToList();

            foreach (var item in Operation)
            {
                db.AccountOperations.Remove(item);
            }

            db.SandDetails.Remove(SandDetail);
            db.SandMains.Remove(SandMain);
            db.SaveChanges();

            return Json(new { success = true, message = "Delete" }, JsonRequestBehavior.AllowGet);



        }



        //public ActionResult Delete(int id)
        //{
        //    SandDetail SandDetail = db.SandDetails.Find(id);
        //    SandMain SandMain = db.SandMains.Find(SandDetail.SandMainId);




        //    db.SandDetails.Remove(SandDetail);
        //    db.SandMains.Remove(SandMain);
        //    db.SaveChanges();


        //    return Json(new { success = true, message = "Delete" }, JsonRequestBehavior.AllowGet);



        //}

        public ActionResult Delete(int id)
        {
            SandDetail SandDetail = db.SandDetails.Find(id);
            SandMain SandMain = db.SandMains.Find(SandDetail.SandMainId);

            if (db.AccountOperations.Any(x => x.OperationTypeId == 1 && x.OperationID == SandDetail.Id))
            {
                List<AccountOperation> Operation = db.AccountOperations.Where(x => x.OperationTypeId == 1 && x.OperationID == SandDetail.Id).ToList();


                foreach (var item in Operation)
                {
                    db.AccountOperations.Remove(item);
                }
            }

            db.SandDetails.Remove(SandDetail);
            db.SandMains.Remove(SandMain);
            db.SaveChanges();


            return Json(new { success = true, message = "Delete" }, JsonRequestBehavior.AllowGet);



        }





        public ActionResult getDeleteReasonParial(int? id)
        {

            DeleteReasonAddViewModel DeleteReasonAddViewModel = new DeleteReasonAddViewModel();
            if (id > 0)
            {

                // Country = db.Countries.Find(id);

            }


            return PartialView("_AddDeleteReason", DeleteReasonAddViewModel);

        }




        public ActionResult AddDeleteReason(DeleteReasonAddViewModel model)
        {

            if (ModelState.IsValid)
            {


                SandDetail Sand = db.SandDetails.Find(model.Id);
                /// Sand.SandMain.ISDeleted = true;
                model.DeleteReason = model.DeleteReason;

                List<AccountOperation> Operation = db.AccountOperations.Where(x => x.OperationTypeId == 2 && x.OperationID == model.Id).ToList();
                foreach (var item in Operation)
                {
                    db.AccountOperations.Remove(item);

                }


                db.Entry(Sand).State = System.Data.Entity.EntityState.Modified;

                db.SaveChanges();


                return Json(new { success = true, message = "Delete" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = true, message = "error" }, JsonRequestBehavior.AllowGet);
            }


        }









    }
}