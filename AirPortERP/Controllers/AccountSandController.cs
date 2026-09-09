//using AirPortERP.Extenstion;
//using AirPortERP.Models;
//using AirPortERP.Reports.Ds;
//using AirPortERP.ViewModels;
//using Microsoft.AspNet.Identity;
//using Newtonsoft.Json.Linq;
//using NumberToWord;
//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;

//namespace AirPortERP.Controllers
//{
//    public class AccountSandController : Controller
//    {
//        ClsAccount cls = new ClsAccount();
//        Entities db = new Entities();
//        CommonUsed CommonUsed = new CommonUsed();
//        RoleController Role = new RoleController();
//        List<CurrencyInfo> currencies = new List<CurrencyInfo>();
//        // GET: AccountSand
      

//        public AccountSandController()
//        {

//            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Sudan));
//            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Dollar));
//            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Syria));
//            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.UAE));
//            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.SaudiArabia));
//            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Tunisia));
//            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Gold));



//        }
        
//        public ActionResult Index()
//        {
//            return View();
//        }

//        public ActionResult ConfirmSandTransation()
//        {
//            return View();
//        }
        
//        public ActionResult GetAllSandSarfConfirm()
//        {
//            var id = User.Identity.GetUserId();       
//            int? companyInfoId = db.WorkOns.Where(x => x.UserId == id).FirstOrDefault().CompanyInfoId;
//            IQueryable<SandDetail> SandDetail = db.SandDetails.Where(x => x.SandMain.IsTransfer == true && x.SandMain.CompanyInfoId == companyInfoId);
//            SandDetail = SandDetail.Where(x => x.SandMain.OperationTypeId == 2);
//            //SandDetail = SandDetail.Where(x => x.SandMain.ISDeleted == false || x.SandMain.ISDeleted == null);

//            var SandList = new List<SadSarfViewModel>();
//            foreach (var item in SandDetail)
//            {

//                SadSarfViewModel r = new SadSarfViewModel();
//                r.Id = item.Id;
             
//                r.AccId = Convert.ToInt32(item.AccId);
//                if (item.AccId > 0)
//                {
//                    r.TreeAccountNumber = cls.GetAccountNumber(Convert.ToDecimal(item.AccId));
//                    r.AccName = item?.AccountTree.AccName;
//                }
//                //r.TotalAmountText = GetAmountTextString(Convert.ToInt32(item.CurrencyId), Convert.ToDecimal(item.Amount));
//                r.TotalAmountText = GetAmountTextString((int)item.CurrencyId, Convert.ToDecimal(item.Amount));
//                r.Date = cls.GetDateddMMyyyy(item.SandMain.SandDate.ToString());
//                r.Statement = item?.Statement;
//                r.CurrencyName = item?.Currency.CurrencyName;
//                r.IsConfirm = Convert.ToBoolean(item.SandMain.IsConfirm);

//                if (item.YearId > 0)
//                {
//                    r.Year = item?.YearWorkOn.Year.ToString();
//                }
//                else {
//                    r.Year = "غير معروف";
//                }
//                if (item.YearId > 0)
//                {
//                    r.Month = item?.MouthWorkOn.Mouth.ToString();
//                }
//                else
//                {
//                    r.Month = "غير معروف";
//                }
               


//                r.TotalAmountCredit = Convert.ToDecimal(item?.Amount).ToString("N");
//                r.Role = Role.GetRole(id);
//                //});
//                SandList.Add(r);
//            }

//            return Json(new { data = SandList.ToList() }, JsonRequestBehavior.AllowGet);
//        }
        
//        public ActionResult GetAllSand()
//        {
//            var id = User.Identity.GetUserId();
//            int? companyInfoId = db.WorkOns.Where(x => x.UserId == id).FirstOrDefault().CompanyInfoId;
//            IQueryable<SandDetail> SandDetail = db.SandDetails.Where(x => x.SandMain.IsTransfer == false || x.SandMain.IsTransfer == null && x.SandMain.CompanyInfoId == companyInfoId);
//            SandDetail = SandDetail.Where(x => x.SandMain.OperationTypeId == 2);

//            var SandList = new List<SadViewModel>();
//            foreach (var item in SandDetail)
//            {

//                SadViewModel r = new SadViewModel();
//                r.Id = item.Id;
//                r.AccId = Convert.ToInt32(item.AccId);
//                if (item.AccId > 0)
//                {
//                    r.TreeAccountNumber = cls.GetAccountNumber(Convert.ToDecimal(item.AccId));
//                    r.AccName = item?.AccountTree.AccName;
//                }
//                //r.TotalAmountText = GetAmountTextString(Convert.ToInt32(item.CurrencyId), Convert.ToDecimal(item.Amount));
//                r.TotalAmountText = GetAmountTextString((int)item.CurrencyId, Convert.ToDecimal(item.Amount));
//                //r.Date = cls.GetDateddMMyyyy(item.SandMain.SandDate.ToString());
//                r.Date = CommonUsed.ToDisplayDate(item.SandMain.SandDate);
//                r.Statement = item?.Statement;
//                r.CurrencyName = item?.Currency.CurrencyName;
//                r.IsConfirm = Convert.ToBoolean(item.SandMain.IsConfirm);
//                if (item.YearId > 0)
//                {
//                    r.Year = item?.YearWorkOn.Year.ToString();
//                }
//                else
//                {
//                    r.Year = "غير معروف";
//                }
//                if (item.YearId > 0)
//                {
//                    r.Month = item?.MouthWorkOn.Mouth.ToString();
//                }
//                else
//                {
//                    r.Month = "غير معروف";
//                }


//                r.TotalAmountCredit = Convert.ToDecimal(item?.Amount).ToString("N");

//                //});
//                SandList.Add(r);
//            }

//            return Json(new { data = SandList.ToList() }, JsonRequestBehavior.AllowGet);
//        }
        
//        public ActionResult GetAllConfirmSandTransation()
//        {

//            var id = User.Identity.GetUserId();
//            int? companyInfoId = db.WorkOns.Where(x => x.UserId == id).FirstOrDefault().CompanyInfoId;
//            IQueryable<SandDetail> SandDetail = db.SandDetails.Where(x => x.SandMain.IsTransfer == true && x.SandMain.CompanyInfoId == companyInfoId);
//            SandDetail = SandDetail.Where(x => x.SandMain.OperationTypeId == 2);

//            var SandList = new List<SadViewModel>();
//            foreach (var item in SandDetail)
//            {

//                SadViewModel r = new SadViewModel();
//                r.Id = item.Id;
//                r.AccId = Convert.ToInt32(item.AccId);
//                if (item.AccId > 0)
//                {
//                    r.TreeAccountNumber = cls.GetAccountNumber(Convert.ToDecimal(item.AccId));
//                    r.AccName = item?.AccountTree.AccName;
//                }
//                //r.TotalAmountText = GetAmountTextString(Convert.ToInt32(item.CurrencyId), Convert.ToDecimal(item.Amount));
//                r.TotalAmountText = GetAmountTextString((int)item.CurrencyId, Convert.ToDecimal(item.Amount));
//                r.Date = CommonUsed.ToDisplayDate(item.SandMain.SandDate);
//                r.Statement = item?.Statement;
//                r.CurrencyName = item?.Currency.CurrencyName;
//                r.IsConfirm = Convert.ToBoolean(item.SandMain.IsConfirm);
              
//                if (item.YearId > 0)
//                {
//                    r.Year = item?.YearWorkOn.Year.ToString();
//                }
//                else
//                {
//                    r.Year = "غير معروف";
//                }
//                if (item.YearId > 0)
//                {
//                    r.Month = item?.MouthWorkOn.Mouth.ToString();
//                }
//                else
//                {
//                    r.Month = "غير معروف";
//                }


//                r.TotalAmountCredit = Convert.ToDecimal(item?.Amount).ToString("N");
//                r.Role = Role.GetRole(id);
//                //});
//                SandList.Add(r);
//            }

//            return Json(new { data = SandList.ToList() }, JsonRequestBehavior.AllowGet);
//        }
        
//        public ActionResult GetSandParial(int? id)
//        {
//            SandDetail SandDetail = new SandDetail();
//            SandAddViewModel model = new SandAddViewModel();
            
//            if (id > 0)
//            {
//                SandDetail = db.SandDetails.Find(id);

//                model.Id = SandDetail.Id;
//                model.AccIdDetail = Convert.ToInt32(SandDetail.AccId);
//                model.AccIdMain = Convert.ToInt32(SandDetail.SandMain.AccId);

//                model.Id = Convert.ToInt32(SandDetail.Id);
//                model.MainId = Convert.ToInt32(SandDetail.SandMain.Id);

//                model.ISCash = Convert.ToBoolean(SandDetail.ISCash);
//                model.ShakNumber = SandDetail.ShakNumber;
//                model.Statement = SandDetail.Statement;

//                model.TotalAmountCredit = SandDetail.SandMain.TotalAmount;

//                model.CurrencyId = Convert.ToInt32(SandDetail.CurrencyId);


//                model.YearId = Convert.ToInt32(SandDetail.YearId);
//                model.MonthId = Convert.ToInt32(SandDetail.MonthId);
              
//                model.Date = SandDetail.SandMain.SandDate.HasValue ? SandDetail.SandMain.SandDate.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : null;

//                //model.Date = Convert.ToDateTime(SandDetail.SandMain.SandDate);
//                //  model.Note = Convert.ToDecimal(OpenAccBalance.Credit);
//                if (model.ISCash == true)
//                {
//                    ViewBag.AccIdMain = new SelectList(db.AccountTrees.Where(x => x.AccGroupId == 3), "Id", "AccName", model.AccIdMain);

              
//                }
//                else
//                {
//                    ViewBag.AccIdMain = new SelectList(db.AccountTrees.Where(x => x.AccGroupId == 2), "Id", "AccName", model.AccIdMain);
//                }
//                ViewBag.AccIdDetail = new SelectList(db.AccountTrees.Where(x => x.AccGroupId == 1), "Id", "AccName", model.AccIdDetail);
//                ViewBag.CurrencyId = new SelectList(db.Currencies, "Id", "CurrencyName", model.CurrencyId);


//                //---------------------------------------Year && Month---------------------------------------------------------
              

//            }
//            else
//            {
//                List<AccountTree> Mainlist = new List<AccountTree>();
//                Mainlist = db.AccountTrees.Where(x => x.AccGroupId == 2).ToList();
//                Mainlist.Insert(0, new AccountTree() { AccName = "--Select Main Account--", Id = 0 });
//                ViewBag.AccIdMain = new SelectList(Mainlist, "Id", "AccName", model.AccIdMain);
//                model.Date = DateTime.Today.Date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);


//                List<AccountTree> Detaillist = new List<AccountTree>();
//                Detaillist = db.AccountTrees.Where(x => x.AccGroupId == 1).ToList();
//                Detaillist.Insert(0, new AccountTree() { AccName = "--Select Sub Account--", Id = 0 });
//                ViewBag.AccIdDetail = new SelectList(Detaillist, "Id", "AccName", model.AccIdDetail);
//               /// ViewBag.AccIdDetail = new SelectList(db.AccountTrees.Where(x => x.AccGroupId == 1), "Id", "AccName", model.AccIdDetail);


//                ViewBag.CurrencyId = new SelectList(db.Currencies, "Id", "CurrencyName", model.CurrencyId);
//            }





//            IQueryable<YearWorkOn> Years = db.YearWorkOns;
//            var YearList = new List<YearViewModel>();
//            foreach (var item in Years)
//            {
//                YearViewModel r = new YearViewModel();
//                r.Id = item.Id;
//                r.Year = item.Year.ToString();
//                YearList.Add(r);
//            }

//            YearList.Insert(0, new YearViewModel() { Year = "--Select Year Claim--", Id = 0 });
//            ViewBag.YearId = new SelectList(YearList, "Id", "Year", model.YearId);


//            List<MouthWorkOn> MouthList = new List<MouthWorkOn>();
//            MouthList = db.MouthWorkOns.ToList();
//            MouthList.Insert(0, new MouthWorkOn() { Mouth = "--Select Mouth Claim--", Id = 0 });
//            ViewBag.MonthId = new SelectList(MouthList, "Id", "Mouth", model.MonthId);





//            return PartialView("_AddEditSand", model);

//        }
        
//        public ActionResult CreateOrUpdateBalance(SandAddViewModel model)
//        {
//            var id = User.Identity.GetUserId();
//            int? companyInfoId = db.WorkOns.Where(x => x.UserId == id).FirstOrDefault().CompanyInfoId;
//            if (!companyInfoId.HasValue || companyInfoId <= 0) { return Json(new { success = true, message = "CompanyInfoIdNotFound" }, JsonRequestBehavior.AllowGet); }
//            DateTime sanddate = CommonUsed.ConvertToDbDate(model.Date);
          
//            int Mouth, Year;
//            if (ModelState.IsValid)
//            {
//                var MouthObj = db.MouthWorkOns.FirstOrDefault(x => x.Id == sanddate.Date.Month);
//                var YearObj = db.YearWorkOns.FirstOrDefault(x => x.Year == sanddate.Date.Year);

//                Mouth = Convert.ToInt32(MouthObj.Id);
//                Year = Convert.ToInt32(YearObj.Year);

//                model.MonthId = Mouth;
//                model.YearId = Convert.ToInt32(YearObj.Id);

//                //Mouth = Convert.ToInt32(model.MonthId);
//                //Year = Convert.ToInt32(db.YearWorkOns.Find(model.YearId).Year);

//                //if (!db.FinancialClaims.Any(x => x.Month == Mouth && x.Year == Year && x.AccIdDebit == model.AccIdDetail))
//                //{
//                //    return Json(new { success = true, message = "NoClaim" }, JsonRequestBehavior.AllowGet);
//                //}
//                //else
//                //{
//                    //FinancialClaim Claim = db.FinancialClaims.Where(x => x.Month == Mouth && x.Year == Year && x.AccIdDebit == model.AccIdDetail).FirstOrDefault();
//                    //if (Claim.CurrencyId != model.CurrencyId)
//                    //{
//                    //    return Json(new { success = true, message = "CurrencyDifference" }, JsonRequestBehavior.AllowGet);
//                    //}

//                    //if (Claim.Amount != model.TotalAmountCredit)
//                    //{
//                    //    if (!model.ISConfirmAmount)
//                    //    {
//                    //        return Json(new { success = true, message = "AmountDifference" }, JsonRequestBehavior.AllowGet);
//                    //    }
//                    //}
//                ///}

//                SandDetail SandDetail = new SandDetail();
//                SandMain SandMain = new SandMain();


//                decimal ExchangeRate = CommonUsed.getExchangeRateByLandingDate(Mouth, Year);
//                if (model.Id > 0)
//                {
//                    //if (db.OpenAccBalances.Any(x => x.AccId == model.AccId && x.Id != model.Id))
//                    //{
//                    //    return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
//                    //}
//                    SandMain = db.SandMains.Find(model.MainId);
//                    SandMain.AccId = model.AccIdMain;
//                    SandMain.SandDate = sanddate.Date;
//                    SandMain.TotalAmount = model.TotalAmountCredit;
//                    SandMain.UpdatingDate = DateTime.Now;
//                    SandMain.UpdatedBy = id;
//                    SandMain.CompanyInfoId = companyInfoId;

//                    //----------------------Deatils  ------------------------------------------------------------------------------------------------------
//                    SandDetail = db.SandDetails.Find(model.Id);

//                    SandDetail.AccId = model.AccIdDetail;
//                    SandDetail.ISCash = model.ISCash;
//                    SandDetail.CurrencyId = model.CurrencyId;
//                    SandDetail.YearId = model.YearId;
//                    SandDetail.MonthId = model.MonthId;
//                    if (Convert.ToBoolean(SandDetail.ISCash))
//                    {
//                        SandDetail.ShakNumber = null;
//                    }
//                    else
//                    {
//                        SandDetail.ShakNumber = model.ShakNumber;
//                    }


//                    SandDetail.Statement = model.Statement;
//                    SandDetail.Amount = model.TotalAmountCredit;



//                    ///-------------------------------------------------------------------------------------------------------------------------
//                    bool IsOperation = db.AccountOperations.Any(x => x.OperationID == model.Id && x.OperationTypeId == 2);
//                    if (IsOperation)
//                    {
//                        //delete old operation
//                        List<AccountOperation> Operation = db.AccountOperations.Where(x => x.OperationTypeId == 2 && x.OperationID == model.Id).ToList();


//                        foreach (var item in Operation)
//                        {
//                            db.AccountOperations.Remove(item);
//                        }

//                        //Add Opertion  operation
//                        //----------------------------------------------------------------------------------------------------------------------------------------
//                        ///----------------------------------عمل قيد مزدوج------------------------------------------
//                        AccountOperation OperationDebit = new AccountOperation();
//                        AccountOperation OperationCredit = new AccountOperation();

//                        int Id = Convert.ToInt32(db.SpLastAccountOperationId().FirstOrDefault());
//                        OperationDebit.Id = Id;

//                        OperationDebit.AccountNumber = model.AccIdMain;
//                        OperationDebit.OperationDebit = model.TotalAmountCredit;
//                        OperationDebit.OperationCredit = 0;
//                        OperationDebit.OperationDate = sanddate.Date;
//                        OperationDebit.OperationID = model.Id;
//                        OperationDebit.CurrencyId = model.CurrencyId;
//                        OperationDebit.CurrencyValue = Convert.ToDouble(ExchangeRate);
//                        OperationDebit.OperationStatement = model.Statement;
//                        OperationDebit.OperationTypeId = 2;
//                        OperationDebit.CreatedBy = id;
//                        OperationDebit.CreationDate = DateTime.Now;
//                        OperationDebit.CompanyInfoId = companyInfoId;
//                        ///----------------------------------------------------------------------------
//                        int Id2 = Convert.ToInt32(db.SpLastAccountOperationId().FirstOrDefault() + 1);
//                        OperationCredit.Id = Id2;

//                        OperationCredit.AccountNumber = model.AccIdDetail;
//                        OperationCredit.OperationDebit = 0;
//                        OperationCredit.OperationCredit = model.TotalAmountCredit;
//                        OperationCredit.OperationDate = sanddate.Date;
//                        OperationCredit.OperationID = model.Id;
//                        OperationCredit.CurrencyId = model.CurrencyId;
//                        OperationCredit.CurrencyValue = Convert.ToDouble(ExchangeRate);
//                        OperationCredit.OperationStatement = model.Statement;
//                        OperationCredit.OperationTypeId = 2;
//                        OperationCredit.CreatedBy = id;
//                        OperationCredit.CreationDate = DateTime.Now;
//                        OperationCredit.CompanyInfoId = companyInfoId;

//                        db.AccountOperations.Add(OperationDebit);
//                        db.AccountOperations.Add(OperationCredit);

//                        //------------------------------------------------------------------------------------------------------------------------------------------
//                    }

//                    db.Entry(SandMain).State = System.Data.Entity.EntityState.Modified;
//                    db.Entry(SandDetail).State = System.Data.Entity.EntityState.Modified;
//                    db.SaveChanges();
//                    return Json(new { success = true, message = "Update" }, JsonRequestBehavior.AllowGet);
//                }
//                else //create
//                {
//                    //if (db.OpenAccBalances.Any(x => x.AccId == model.AccId))
//                    //{
//                    //    return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
//                    //}
//                    // Balance.Note = cls.GetAccountName(model.AccId) + "حساب افتتاحي ل";
//                    int SandMainId = Convert.ToInt32(db.SpLastSandMain().FirstOrDefault());
//                    SandMain.Id = SandMainId;
//                    SandMain.AccId = model.AccIdMain;
//                    SandMain.OperationTypeId = 2;
//                    SandMain.TotalAmount = model.TotalAmountCredit;
//                    /// SandMain.ISDeleted = false;
//                    SandMain.SandDate = sanddate.Date;
//                    SandMain.CreationDate = DateTime.Now;
//                    SandMain.CreatedBy = id;

//                    ///-------------------------details -------------------------------------------------------------------------------
//                    int SandDetailId = Convert.ToInt32(db.SpLastSandDetail().FirstOrDefault());
//                    SandDetail.Id = SandDetailId;
//                    SandDetail.AccId = model.AccIdDetail;

//                    SandDetail.CurrencyId = model.CurrencyId;
//                    SandDetail.YearId = model.YearId;

//                    SandDetail.MonthId = model.MonthId;

//                    SandDetail.SandMainId = SandMainId;
//                    SandDetail.Statement = model.Statement;
//                    SandDetail.Amount = model.TotalAmountCredit;

//                    SandDetail.ISCash = model.ISCash;

//                    if (Convert.ToBoolean(SandDetail.ISCash))
//                    {
//                        SandDetail.ShakNumber = null;
//                    }
//                    else
//                    {
//                        SandDetail.ShakNumber = model.ShakNumber;
//                    }

//                    SandMain.CompanyInfoId = companyInfoId;
//                    db.SandMains.Add(SandMain);
//                    db.SandDetails.Add(SandDetail);
//                    db.SaveChanges();
//                    return Json(new { success = true, message = "Add" }, JsonRequestBehavior.AllowGet);
//                }

//            }
//            return Json(new { success = false, message = "error" }, JsonRequestBehavior.AllowGet);
//            }
        
//        //public ActionResult CreateOrUpdateBalanceOld(SandAddViewModel model)
//        //{
//        //    var userId = User.Identity.GetUserId();
//        //    int Mouth, Year;
//        //    if (ModelState.IsValid)
//        //    {
//        //        Mouth = Convert.ToInt32(model.MonthId);
//        //        Year = Convert.ToInt32(db.YearWorkOns.Find(model.YearId).Year);

//        //        if (!db.FinancialClaims.Any(x => x.Month == Mouth && x.Year == Year && x.AccIdDebit == model.AccIdDetail))
//        //        {
//        //            return Json(new { success = true, message = "NoClaim" }, JsonRequestBehavior.AllowGet);
//        //        }
//        //        else
//        //        {
//        //            FinancialClaim Claim = db.FinancialClaims.Where(x => x.Month == Mouth && x.Year == Year && x.AccIdDebit == model.AccIdDetail).FirstOrDefault();
//        //            if (Claim.CurrencyId != model.CurrencyId)
//        //            {
//        //                return Json(new { success = true, message = "CurrencyDifference" }, JsonRequestBehavior.AllowGet);
//        //            }

//        //            //if (Claim.Amount != model.TotalAmountCredit)
//        //            //{
//        //            //    if (!model.ISConfirmAmount)
//        //            //    {
//        //            //        return Json(new { success = true, message = "AmountDifference" }, JsonRequestBehavior.AllowGet);
//        //            //    }
//        //            //}
//        //        }

//        //        SandDetail SandDetail = new SandDetail();
//        //        SandMain SandMain = new SandMain();


//        //        decimal ExchangeRate = CommonUsed.getExchangeRateByLandingDate(Mouth, Year);
//        //        if (model.Id > 0)
//        //        {
//        //            //if (db.OpenAccBalances.Any(x => x.AccId == model.AccId && x.Id != model.Id))
//        //            //{
//        //            //    return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
//        //            //}
//        //            SandMain = db.SandMains.Find(model.MainId);
//        //            SandMain.AccId = model.AccIdMain;
//        //            SandMain.TotalAmount = model.TotalAmountCredit;

//        //            //----------------------Deatils  ------------------------------------------------------------------------------------------------------
//        //            SandDetail = db.SandDetails.Find(model.Id);

//        //            SandDetail.AccId = model.AccIdDetail;
//        //            SandDetail.ISCash = model.ISCash;
//        //            SandDetail.CurrencyId = model.CurrencyId;
//        //            SandDetail.YearId = model.YearId;
//        //            SandDetail.MonthId = model.MonthId;
//        //            if (Convert.ToBoolean(SandDetail.ISCash))
//        //            {
//        //                SandDetail.ShakNumber = null;
//        //            }
//        //            else
//        //            {
//        //                SandDetail.ShakNumber = model.ShakNumber;
//        //            }


//        //            SandDetail.Statement = model.Statement;
//        //            SandDetail.Amount = model.TotalAmountCredit;


//        //            ///-------------------------------------------------------------------------------------------------------------------------
//        //            bool IsOperation = db.AccountOperations.Any(x => x.OperationID == model.Id && x.OperationTypeId == 2);
//        //            if (IsOperation)
//        //            {
//        //                //delete old operation
//        //                List<AccountOperation> Operation = db.AccountOperations.Where(x => x.OperationTypeId == 2 && x.OperationID == model.Id).ToList();


//        //                foreach (var item in Operation)
//        //                {
//        //                    db.AccountOperations.Remove(item);
//        //                }

//        //                //Add Opertion  operation
//        //                //----------------------------------------------------------------------------------------------------------------------------------------
//        //                ///----------------------------------عمل قيد مزدوج------------------------------------------
//        //                AccountOperation OperationDebit = new AccountOperation();
//        //                AccountOperation OperationCredit = new AccountOperation();

//        //                int Id = Convert.ToInt32(db.SpLastAccountOperationId().FirstOrDefault());
//        //                OperationDebit.Id = Id;

//        //                OperationDebit.AccountNumber = model.AccIdMain;
//        //                OperationDebit.OperationDebit = model.TotalAmountCredit;
//        //                OperationDebit.OperationCredit = 0;
//        //                OperationDebit.OperationDate = model.Date;
//        //                OperationDebit.OperationID = model.Id;
//        //                OperationDebit.CurrencyId = model.CurrencyId;
//        //                OperationDebit.CurrencyValue = Convert.ToDouble(ExchangeRate);
//        //                OperationDebit.OperationStatement = model.Statement;
//        //                OperationDebit.OperationTypeId = 2;
//        //                OperationDebit.CreatedBy = userId;
//        //                OperationDebit.CreationDate = DateTime.Now;
//        //                ///----------------------------------------------------------------------------
//        //                int Id2 = Convert.ToInt32(db.SpLastAccountOperationId().FirstOrDefault() + 1);
//        //                OperationCredit.Id = Id2;

//        //                OperationCredit.AccountNumber = model.AccIdDetail;
//        //                OperationCredit.OperationDebit = 0;
//        //                OperationCredit.OperationCredit = model.TotalAmountCredit;
//        //                OperationCredit.OperationDate = model.Date;
//        //                OperationCredit.OperationID = model.Id;
//        //                OperationCredit.CurrencyId = model.CurrencyId;
//        //                OperationCredit.CurrencyValue = Convert.ToDouble(ExchangeRate);
//        //                OperationCredit.OperationStatement = model.Statement;
//        //                OperationCredit.OperationTypeId = 2;
//        //                OperationCredit.CreatedBy = userId;
//        //                OperationCredit.CreationDate = DateTime.Now;

//        //                db.AccountOperations.Add(OperationDebit);
//        //                db.AccountOperations.Add(OperationCredit);

//        //                //------------------------------------------------------------------------------------------------------------------------------------------
//        //            }

//        //            db.Entry(SandMain).State = System.Data.Entity.EntityState.Modified;
//        //            db.Entry(SandDetail).State = System.Data.Entity.EntityState.Modified;
//        //            db.SaveChanges();
//        //            return Json(new { success = true, message = "Update" }, JsonRequestBehavior.AllowGet);
//        //        }
//        //        else
//        //        {
//        //            //if (db.OpenAccBalances.Any(x => x.AccId == model.AccId))
//        //            //{
//        //            //    return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
//        //            //}
//        //            // Balance.Note = cls.GetAccountName(model.AccId) + "حساب افتتاحي ل";
//        //            int SandMainId = Convert.ToInt32(db.SpLastSandMain().FirstOrDefault());
//        //            SandMain.Id = SandMainId;
//        //            SandMain.AccId = model.AccIdMain;
//        //            SandMain.OperationTypeId = 2;
//        //            SandMain.TotalAmount = model.TotalAmountCredit;
//        //            /// SandMain.ISDeleted = false;
//        //            SandMain.SandDate = DateTime.Now;


//        //            ///-------------------------details -------------------------------------------------------------------------------
//        //            int SandDetailId = Convert.ToInt32(db.SpLastSandDetail().FirstOrDefault());
//        //            SandDetail.Id = SandDetailId;
//        //            SandDetail.AccId = model.AccIdDetail;

//        //            SandDetail.CurrencyId = model.CurrencyId;
//        //            SandDetail.YearId = model.YearId;

//        //            SandDetail.MonthId = model.MonthId;

//        //            SandDetail.SandMainId = SandMainId;
//        //            SandDetail.Statement = model.Statement;
//        //            SandDetail.Amount = model.TotalAmountCredit;

//        //            SandDetail.ISCash = model.ISCash;

//        //            if (Convert.ToBoolean(SandDetail.ISCash))
//        //            {
//        //                SandDetail.ShakNumber = null;
//        //            }
//        //            else
//        //            {
//        //                SandDetail.ShakNumber = model.ShakNumber;
//        //            }

//        //            db.SandMains.Add(SandMain);
//        //            db.SandDetails.Add(SandDetail);
//        //            db.SaveChanges();
//        //            return Json(new { success = true, message = "Add" }, JsonRequestBehavior.AllowGet);
//        //        }

//        //    }
//        //    return Json(new { success = false, message = "error" }, JsonRequestBehavior.AllowGet);
//        //}
        
//        public ActionResult GetAcc(bool yes)
//        {
//            db.Configuration.ProxyCreationEnabled = false;
//            List<AccountTree> lstAccountTree = new List<AccountTree>();
//            lstAccountTree = db.AccountTrees.ToList();
//            if (yes)
//            {
//                lstAccountTree = lstAccountTree.Where(x => x.AccGroupId == 3).ToList();
//            }
//            else
//            {
//                lstAccountTree = lstAccountTree.Where(x => x.AccGroupId == 2).ToList();
//            }



//            lstAccountTree.Insert(0, new AccountTree() { AccName = "--Select Account--", Id = 0 });
//            return Json(lstAccountTree, JsonRequestBehavior.AllowGet);
//        }
        
//        public ActionResult GetAmountText(int CurrencyId, decimal Statement)
//        {
//            string MoneyText = "";
//            if (CurrencyId == 2)
//            {
//                double SDGRound = Convert.ToDouble(Math.Round(Statement, 2));

//                ToWord toWord = new ToWord(Convert.ToDecimal(SDGRound), currencies[Convert.ToInt32(0)]);

//                MoneyText = toWord.ConvertToArabic().ToString();

//                return Json(MoneyText, JsonRequestBehavior.AllowGet);


//                ///--------------------------------------Dollar-----------------------------------------------------------
//            }
//            else
//            {
//                double DollarRound = Convert.ToDouble(Math.Round(Statement, 2));
//                ToWord toWord1 = new ToWord(Convert.ToDecimal(DollarRound), currencies[Convert.ToInt32(1)]);
//                MoneyText = toWord1.ConvertToArabic().ToString();


//                return Json(MoneyText, JsonRequestBehavior.AllowGet);
//            }
//        }
        
//        public string GetAmountTextString(int CurrencyId, decimal Statement)
//        {
//            string MoneyText = "";
//            if (CurrencyId == 2)
//            {
//                //double SDGRound = Convert.ToDouble(Math.Round(Statement, 2));
//                decimal SDGRound = Math.Round(Statement, 2);

//                ToWord toWord = new ToWord(SDGRound, currencies[Convert.ToInt32(0)]);

//                MoneyText = toWord.ConvertToArabic().ToString();

//                return MoneyText;


//                ///--------------------------------------Dollar-----------------------------------------------------------
//            }
//            else
//            {
//                //double DollarRound = Convert.ToDouble(Math.Round(Statement, 2));
//                decimal DollarRound = Math.Round(Statement, 2);
//                ToWord toWord1 = new ToWord(DollarRound, currencies[Convert.ToInt32(1)]);
//                MoneyText = toWord1.ConvertToArabic().ToString();


//                return MoneyText;
//            }
//        }
        
//        // ----------------------------------------------------------------------------------------------------------------------
//        public ActionResult GetAmountBallText(DateTime sanadDate, int? AccId)
//        {
//            string MoneyBallText;
//            decimal MoneyBall;
//            if (AccId > 0 && sanadDate != null)
//            {

//                int Mouth, Year;
//                Mouth = Convert.ToInt32(sanadDate.Month);
//                Year = Convert.ToInt32(db.YearWorkOns.FirstOrDefault(x => x.Year == sanadDate.Year).Year);
                
//                if (db.FinancialClaims.Any(x => x.AccIdDebit == AccId && x.Month == Mouth && x.Year == Year))
//                {
//                    FinancialClaim claim = db.FinancialClaims.Where(x => x.AccIdDebit == AccId && x.Month == Mouth && x.Year == Year).OrderByDescending(x=>x.Id).FirstOrDefault();
//                    MoneyBall = Convert.ToDecimal(claim.Amount);

//                    MoneyBallText = " مبلغ الفاتوره الذي يجب سداده: " + MoneyBall +" "+ claim.Currency.CurrencyName;               
//                }
//                else
//                {
//                    MoneyBallText = "لم يتم استخراج فاتورة لهذا الشهر";       
//                }                     
//            }
//            else
//            {
//                MoneyBallText = "";
//            }

//            return Json(MoneyBallText, JsonRequestBehavior.AllowGet);
//        }

//        public ActionResult GetAmountBallTextOld(int? MonthId, int? YearId, int? AccId)
//        {
//            string MoneyBallText;
//            decimal MoneyBall;
//            if (AccId > 0 && MonthId > 0 && YearId > 0)
//            {

//                int Mouth, Year;
//                Mouth = Convert.ToInt32(MonthId);
//                Year = Convert.ToInt32(db.YearWorkOns.Find(YearId).Year);
//                if (db.FinancialClaims.Any(x => x.AccIdDebit == AccId && x.Month == Mouth && x.Year == Year))
//                {
//                    FinancialClaim claim = db.FinancialClaims.Where(x => x.AccIdDebit == AccId && x.Month == Mouth && x.Year == Year).OrderByDescending(x => x.Id).FirstOrDefault();
//                    MoneyBall = Convert.ToDecimal(claim.Amount);

//                    MoneyBallText = " مبلغ الفاتوره الذي يجب سداده: " + MoneyBall + " " + claim.Currency.CurrencyName;
//                }
//                else
//                {
//                    MoneyBallText = "لم يتم استخراج فاتورة لهذا الشهر";
//                }
//            }
//            else
//            {
//                MoneyBallText = "";
//            }

//            return Json(MoneyBallText, JsonRequestBehavior.AllowGet);
//        }
        
//        public ActionResult GetWritingStatementAutomatic(int? MonthId, int? YearId, int? AccId,bool? value, decimal TotalAmountCredit)
//        {
//            string StatementText;
//            decimal MoneyBall;
//            if (AccId > 0 && MonthId > 0 && YearId > 0 && value == true) 
//            {

//                int Mouth, Year;
//                Mouth = Convert.ToInt32(MonthId);
//                AirLine Airline = db.AirLines.Where(x => x.AccId == AccId).FirstOrDefault();
//                string MouthName = db.MouthWorkOns.Find(MonthId).Mouth;
//                Year = Convert.ToInt32(db.YearWorkOns.Find(YearId).Year);
//                if (db.FinancialClaims.Any(x => x.AccIdDebit == AccId && x.Month == Mouth && x.Year == Year))
//                {
//                   FinancialClaim claim = db.FinancialClaims.Where(x => x.AccIdDebit == AccId && x.Month == Mouth && x.Year == Year).FirstOrDefault();
//                    if (TotalAmountCredit >= claim.Amount)
//                    {
//                        StatementText = " سداد مطالبة " + Airline.ArName + " لشهر " + MouthName + " لعام " + " " + Year + "م";
//                    }
//                    else
//                    {
//                        StatementText = " سداد (جزء) من مطالبة " + Airline.ArName + " لشهر " + MouthName + " لعام " + " " + Year + "م";
//                    }
//                }
//                else
//                {
//                    StatementText = "";
//                }
//            }
//            else
//            {
//                StatementText = "";
//            }

//            return Json(StatementText, JsonRequestBehavior.AllowGet);
//        }
        
//        public ActionResult GetAmountBallTextR(int? MonthId, int? YearId, int? AccId, decimal? TotalAmountCredit) 
//        {
//            string Value= "False";
//            decimal MoneyBall;
//            if (AccId > 0 && MonthId > 0 && YearId > 0 && TotalAmountCredit > 0) 
//            {

//                int Mouth, Year;
//                Mouth = Convert.ToInt32(MonthId);
//                Year = Convert.ToInt32(db.YearWorkOns.Find(YearId).Year);
//                if (db.FinancialClaims.Any(x => x.AccIdDebit == AccId && x.Month == Mouth && x.Year == Year))
//                {
//                    FinancialClaim claim = db.FinancialClaims.Where(x => x.AccIdDebit == AccId && x.Month == Mouth && x.Year == Year).FirstOrDefault();
//                    MoneyBall = Convert.ToDecimal(claim.Amount);

//                    if (MoneyBall == TotalAmountCredit)
//                    {
//                        Value = "True";
//                    }
//                }
//                else
//                {
//                    Value = "False";
//                }
//            }
//            else
//            {
//                Value = "False";
//            }

//            return Json(Value, JsonRequestBehavior.AllowGet);
//        }
//        // ----------------------------------------------------------------------------------------------------------
        
//        public ActionResult GetSandToConfirm(int id)
//        {
//            string MoneyText = "";
//            SandDetail Sand = db.SandDetails.Find(id);

//            //MoneyText = GetAmountTextString(Convert.ToInt32(Sand.CurrencyId), Convert.ToDecimal(Sand.Amount));
//            MoneyText = GetAmountTextString((int)Sand.CurrencyId, Convert.ToDecimal(Sand.Amount));

//            double value = Convert.ToDouble(Sand.Amount);
//            string Message = value + "بقيمة" + Sand.AccountTree.AccName + "سند قبض من حساب";

//            return Json(new { success = true, message = Message, moneytext = MoneyText }, JsonRequestBehavior.AllowGet);


//            ///--------------------------------------Dollar-----------------------------------------------------------
//        }
        
//        public ActionResult GetSandToConfirmDelete(int id)
//        {
//            string MoneyText = "";
//            SandDetail Sand = db.SandDetails.Find(id);

//            MoneyText = GetAmountTextString((int)Sand.CurrencyId, Convert.ToDecimal(Sand.Amount));

//            double value = Convert.ToDouble(Sand.Amount);
//            string Message =      value + "بقيمة" + Sand.AccountTree.AccName + "  هل تريد حذف سند قبض من حساب  ";

//            return Json(new { success = true, message = Message, moneytext = MoneyText }, JsonRequestBehavior.AllowGet);


//            ///--------------------------------------Dollar-----------------------------------------------------------
//        }
        
//        //add To Operations
//        public ActionResult AddToConfrim(int id)
//        {
//            var Userid = User.Identity.GetUserId();
//            int? companyInfoId = db.WorkOns.Where(x => x.UserId == Userid).FirstOrDefault().CompanyInfoId;
//            if (!companyInfoId.HasValue || companyInfoId <= 0) { return Json(new { success = true, message = "CompanyInfoIdNotFound" }, JsonRequestBehavior.AllowGet); }
//            // string MoneyText = "";
          
//            SandDetail Sand = db.SandDetails.Find(id);
//            Sand.SandMain.IsConfirm = true;
            
//            //----------------------------------عمل قيد مزدوج------------------------------------------
//            AccountOperation OperationDebit = new AccountOperation();
//            AccountOperation OperationCredit = new AccountOperation();

//            int Id = Convert.ToInt32(db.SpLastAccountOperationId().FirstOrDefault());
//            OperationDebit.Id = Id;
//            OperationDebit.AccountNumber = Sand.SandMain.AccId;
//            OperationDebit.OperationDebit = Sand.Amount;
//            OperationDebit.OperationCredit = 0;
//            OperationDebit.OperationDate = Sand.SandMain.SandDate;
//            OperationDebit.OperationID = Sand.Id;
//            OperationDebit.CurrencyId = Sand.CurrencyId;
//            OperationDebit.OperationStatement = Sand.Statement;
//            OperationDebit.OperationTypeId = Sand.SandMain.OperationTypeId;
//            OperationDebit.CreatedBy = Userid;
//            OperationDebit.CreationDate = DateTime.Now;
//            OperationDebit.CompanyInfoId = companyInfoId;
//            //----------------------------------------------------------------------------

//            int Id2 = Convert.ToInt32(db.SpLastAccountOperationId().FirstOrDefault() + 1);
//            OperationCredit.Id = Id2;
//            OperationCredit.AccountNumber = Sand.AccId;
//            OperationCredit.OperationDebit = 0;
//            OperationCredit.OperationCredit = Sand.Amount;
//            OperationCredit.OperationDate = Sand.SandMain.SandDate;
//            OperationCredit.OperationID = Sand.Id;
//            OperationCredit.CurrencyId = Sand.CurrencyId;
//            OperationCredit.OperationStatement = Sand.Statement;
//            OperationCredit.OperationTypeId = Sand.SandMain.OperationTypeId;
//            OperationCredit.CreatedBy = Userid;
//            OperationCredit.CreationDate = DateTime.Now;
//            OperationCredit.CompanyInfoId = companyInfoId;
//            db.AccountOperations.Add(OperationDebit);
//            db.AccountOperations.Add(OperationCredit);
//            db.SaveChanges();

//            return Json(new { success = true, message = "Add" }, JsonRequestBehavior.AllowGet);

//            ///--------------------------------------Dollar-----------------------------------------------------------
//        }




//        public ActionResult Transfer(int id)
//        {
//            SandDetail SandDetail = db.SandDetails.Find(id);
//            SandDetail.SandMain.IsTransfer = true;


//            db.Entry(SandDetail).State = System.Data.Entity.EntityState.Modified;
//            db.SaveChanges();

//            return Json(new { success = true, message = "Add" }, JsonRequestBehavior.AllowGet);

//        }

//        public ActionResult ReTransfer(int id)
//        {
//            SandDetail SandDetail = db.SandDetails.Find(id);
//            SandDetail.SandMain.IsTransfer = false;


//            db.Entry(SandDetail).State = System.Data.Entity.EntityState.Modified;
//            db.SaveChanges();

//            return Json(new { success = true, message = "Add" }, JsonRequestBehavior.AllowGet);

//        }


//        public ActionResult DeleteConfirm(int id)
//        {
//            SandDetail SandDetail = db.SandDetails.Find(id);
//            SandMain SandMain = db.SandMains.Find(SandDetail.SandMainId);


//            List<AccountOperation> Operation = db.AccountOperations.Where(x => x.OperationTypeId == 2 && x.OperationID == SandDetail.Id).ToList();

//            foreach (var item in Operation)
//            {
//                db.AccountOperations.Remove(item);
//            }

//            db.SandDetails.Remove(SandDetail);
//            db.SandMains.Remove(SandMain);
//            db.SaveChanges();

//            return Json(new { success = true, message = "Delete" }, JsonRequestBehavior.AllowGet);



//        }



//        public ActionResult Delete(int id)
//        {
//            SandDetail SandDetail = db.SandDetails.Find(id);
//            SandMain SandMain = db.SandMains.Find(SandDetail.SandMainId);



           
//            db.SandDetails.Remove(SandDetail);
//            db.SandMains.Remove(SandMain);
//            db.SaveChanges();
          

//            return Json(new { success = true, message = "Delete" }, JsonRequestBehavior.AllowGet);



//        }







//        public ActionResult getDeleteReasonParial(int? id)
//        {

//            DeleteReasonAddViewModel DeleteReasonAddViewModel = new DeleteReasonAddViewModel();
//            if (id > 0)
//            {

//               // Country = db.Countries.Find(id);

//            }


//            return PartialView("_AddDeleteReason", DeleteReasonAddViewModel);

//        }


        

//         public ActionResult AddDeleteReason(DeleteReasonAddViewModel model)
//        {

//            if (ModelState.IsValid)
//            {


//                SandDetail Sand = db.SandDetails.Find(model.Id);
//               /// Sand.SandMain.ISDeleted = true;
//                model.DeleteReason = model.DeleteReason;

//                List<AccountOperation> Operation = db.AccountOperations.Where(x => x.OperationTypeId == 2 && x.OperationID == model.Id).ToList();
//                foreach (var item in Operation)
//                {
//                    db.AccountOperations.Remove(item);
                   
//                }


//                db.Entry(Sand).State = System.Data.Entity.EntityState.Modified;
             
//                db.SaveChanges();


//                return Json(new { success = true, message = "Delete" }, JsonRequestBehavior.AllowGet);
//            }
//            else {
//                return Json(new { success = true, message = "error" }, JsonRequestBehavior.AllowGet);
//            }

            
//        }









//    }
//}