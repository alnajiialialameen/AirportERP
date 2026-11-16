using AirPortERP.Extenstion;
using AirPortERP.Models;
using AirPortERP.ViewModels;
using Microsoft.AspNet.Identity;
using NumberToWord;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebGrease.Css.Extensions;

namespace AirPortERP.Controllers
{
    public class AccountFreightClaimController : Controller
    {
        private int Format, Month, Year;
        static decimal ExchangeRate;
        private CommonUsed CommonUsed = new CommonUsed();
        private RoleController Role = new RoleController();
        ClsAccount cls = new ClsAccount();
        Entities db = new Entities();
        List<CurrencyInfo> currencies = new List<CurrencyInfo>();

        // GET: AccountClaim

        public ActionResult Index()
        {
            var id = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(id);
            ViewBag.CompanyId = companyId;

            return View();
        }
        
        public ActionResult ConfirmClaimTransaction()
        {
            return View();
        }
        
        public AccountFreightClaimController()
        {

            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Sudan));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Dollar));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Syria));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.UAE));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.SaudiArabia));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Tunisia));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Gold));



        }
        
        public ActionResult FreightClaimParial(int? id)
        
        {
            ClaimAddViewModel model = new ClaimAddViewModel();
            FinancialClaim1 Claim = new FinancialClaim1();

            if (id > 0)
            {

                Claim = db.FinancialClaim1.Find(id);
                model.Id = Convert.ToInt32(id);
                model.AccId = Convert.ToInt32(Claim.AccIdDebit);

                model.DateFrom = Convert.ToDateTime(Claim.DateFrom)
                    .ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);                //model.DateFrom = cls.ConvertToddMMyyyy(Claim.DateFrom.ToString());
                //model.DateTo = cls.ConvertToddMMyyyy(Claim.DateTo.ToString());
                model.DateTo = Convert.ToDateTime(Claim.DateTo).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                model.ExchangeRate = Convert.ToDecimal(Claim.ExchangeRateAgainstPrimaryCurrency);
            }



            List<AccountTree> Acclist = new List<AccountTree>();
            Acclist = db.AccountTrees.Where(x => x.AccGroupId == 1).ToList();
            Acclist.Insert(0, new AccountTree() { AccName = "--Select Account--", Id = 0 });
            ViewBag.AccId = new SelectList(Acclist, "Id", "AccName", model.AccId);


          ///  ViewBag.AccId = new SelectList(db.AccountTrees.Where(x => x.AccGroupId == 1), "Id", "AccName", model.AccId);
            return PartialView("AddEditFreightClaim", model);

        }

        public ActionResult GetAllFreightClaim()
        {
            var id = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(id);

            IQueryable<FinancialClaim1> FinancialClaim = db.FinancialClaim1.Where(x => x.IsTransfer == false || x.IsTransfer == null
             && x.CompanyInfoId == companyId && x.TowerDataId == null && x.OperationTypeId == 6);
           // SandDetail = SandDetail.Where(x => x.SandMain.ISDeleted == false || x.SandMain.ISDeleted == null);

            var ClaimList = new List<ClaimViewModel>();
            foreach (var item in FinancialClaim)
            {

                ClaimViewModel r = new ClaimViewModel();
                r.Id = item.Id;
              
                r.AccId = Convert.ToInt32(item.AccIdDebit);
                if (item.AccIdDebit > 0)
                {
                    r.AccName = item?.AccountTree.AccName;
                    r.TreeAccountNumber = cls.GetAccountNumber(Convert.ToDecimal(item.AccIdDebit));
                }
                r.TotalAmountText = GetAmountTextString(Convert.ToInt32(item.CurrencyId), Convert.ToDecimal(item.Amount));
                r.Date = CommonUsed.ToDisplayDate(item.Date);
                r.Statement = item?.Statement;
                r.CurrencyName = item?.Currency.CurrencyName;
                r.IsConfirm = Convert.ToBoolean(item.IsConfirm);

                r.DateFrom = CommonUsed.ToDisplayDate(item.DateFrom);
                r.DateTo = CommonUsed.ToDisplayDate(item.DateTo);

                r.TotalAmountCredit = Convert.ToDecimal(item?.Amount).ToString("N");
                r.ExchangeRate = Convert.ToDecimal(item?.ExchangeRateAgainstPrimaryCurrency).ToString("N");
                //r.AirLineId = db.AirLines.FirstOrDefault(x => x.AccId == r.AccId).Id;
                r.AirLineId = db.AirLines.FirstOrDefault(x => x.AccId == r.AccId).Id;
                r.Year = Convert.ToInt32(item?.Year);
                r.Month = Convert.ToInt32(item?.Month);

                //});
                ClaimList.Add(r);
            }

            return Json(new { data = ClaimList.ToList() }, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult GetAllFuelClaimTransfer()
        {
            var id = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(id);

            IQueryable<FinancialClaim1> FinancialClaim = db.FinancialClaim1.Where(x => x.IsTransfer == true
            && x.CompanyInfoId == companyId && x.TowerDataId == null && x.OperationTypeId == 6);
            // SandDetail = SandDetail.Where(x => x.SandMain.ISDeleted == false || x.SandMain.ISDeleted == null);

            var ClaimList = new List<ClaimViewModel>();
            foreach (var item in FinancialClaim)
            {

                ClaimViewModel r = new ClaimViewModel();
                r.Id = item.Id;
                r.AccId = Convert.ToInt32(item.AccIdDebit);
                if (item.AccIdDebit > 0)
                {
                    r.AccName = item?.AccountTree.AccName;
                    r.TreeAccountNumber = cls.GetAccountNumber(Convert.ToDecimal(item.AccIdDebit));
                }
                r.TotalAmountText = GetAmountTextString(Convert.ToInt32(item.CurrencyId), Convert.ToDecimal(item.Amount));
                r.Date = CommonUsed.ToDisplayDate(item.Date);
                r.Statement = item?.Statement;
                r.CurrencyName = item?.Currency.CurrencyName;
                r.IsConfirm = Convert.ToBoolean(item.IsConfirm);

                r.DateFrom = CommonUsed.ToDisplayDate(item.DateFrom);
                r.DateTo = CommonUsed.ToDisplayDate(item.DateTo);

                r.TotalAmountCredit = Convert.ToDecimal(item?.Amount).ToString("N");
                r.ExchangeRate = Convert.ToDecimal(item?.ExchangeRateAgainstPrimaryCurrency).ToString("N");
                r.Year = Convert.ToInt32(item?.Year);
                r.Month = Convert.ToInt32(item?.Month);
                //});

                r.Role = Role.GetRole(id);
                ClaimList.Add(r);
            }

            return Json(new { data = ClaimList.ToList() }, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult GetClaimToConfirm(int id)
        {
            string MoneyText = "";
            FinancialClaim1 Claim = db.FinancialClaim1.Find(id);

            MoneyText = GetAmountTextString(Convert.ToInt32(Claim.CurrencyId), Convert.ToDecimal(Claim.Amount));

            double value = Convert.ToDouble(Claim.Amount);
            string Message = value + "بقيمة" + Claim.AccountTree.AccName + "مطالبة مالية من حساب";

            return Json(new { success = true, message = Message, moneytext = MoneyText }, JsonRequestBehavior.AllowGet);


            ///--------------------------------------Dollar-----------------------------------------------------------
        }
        
        public ActionResult AddFreightToConfrim(int id)
        {
            // string MoneyText = "";
            var userId = User.Identity.GetUserId();

            var companyId = CommonUsed.getCompanyId(userId);
            FinancialClaim1 Claim = db.FinancialClaim1.Find(id);
            Claim.IsConfirm = true;

            ///----------------------------------عمل قيد مزدوج------------------------------------------
            AccountOperation OperationDebit = new AccountOperation();
            AccountOperation OperationCredit = new AccountOperation();
            int Id = Convert.ToInt32(db.SpLastAccountOperationId().FirstOrDefault());

            OperationDebit.Id = Id;
            OperationDebit.AccId = Claim.AccIdDebit;
            OperationDebit.OperationDebit = Claim.Amount;
            OperationDebit.OperationCredit = 0;
            OperationDebit.OperationDate = Claim.DateFrom;
            OperationDebit.OperationID = Claim.Id;
            OperationDebit.CurrencyId = Claim.CurrencyId;
            OperationDebit.OperationStatement = Claim.Statement;
            OperationDebit.OperationTypeId = Claim.OperationTypeId;
            OperationDebit.CreatedBy = userId;
            OperationDebit.CreationDate = DateTime.Now;
            OperationDebit.CompanyInfoId = companyId;
            ///----------------------------------------------------------------------------
            int Id2 = Convert.ToInt32(db.SpLastAccountOperationId().FirstOrDefault() + 1);
            OperationCredit.Id = Id2;
            OperationCredit.AccId = 7;
            OperationCredit.OperationDebit = 0;
            OperationCredit.OperationCredit = Claim.Amount;
            OperationCredit.OperationDate = Claim.DateFrom;
            OperationCredit.OperationID = Claim.Id;
            OperationCredit.CurrencyId = Claim.CurrencyId;
            OperationCredit.OperationStatement = Claim.Statement;
            OperationCredit.OperationTypeId = Claim.OperationTypeId;
            OperationCredit.CreatedBy = userId;
            OperationCredit.CreationDate = DateTime.Now;
            OperationCredit.CompanyInfoId = companyId;


            db.AccountOperations.Add(OperationDebit);
            db.AccountOperations.Add(OperationCredit);
            db.SaveChanges();

            return Json(new { success = true, message = "Add" }, JsonRequestBehavior.AllowGet);


            ///--------------------------------------Dollar-----------------------------------------------------------
        }

        public ActionResult GetSandParial(int? id)
        {

            SandDetails1 SandDetail = new SandDetails1();
            SandAddViewModel model = new SandAddViewModel();


            if (id > 0)
            {

                SandDetail = db.SandDetails1.Find(id);

                model.Id = SandDetail.Id;
                model.AccIdDetail = Convert.ToInt32(SandDetail.AccId);
                model.AccIdMain = Convert.ToInt32(SandDetail.SandMain1.AccId);

                model.Id = Convert.ToInt32(SandDetail.Id);
                model.MainId = Convert.ToInt32(SandDetail.SandMain1.Id);

                model.ISCash = Convert.ToBoolean(SandDetail.ISCash);
                model.ShakNumber = SandDetail.ShakNumber;
                model.Statement = SandDetail.Statement;

                model.TotalAmountCredit = Convert.ToDecimal(SandDetail.SandMain1.TotalAmount);

                model.CurrencyId = Convert.ToInt32(SandDetail.CurrencyId);

                //model.Date = Convert.ToDateTime(SandDetail.SandMain.SandDate);
                //  model.Note = Convert.ToDecimal(OpenAccBalance.Credit);
                if (model.ISCash == true)
                {
                   /// ViewBag.AccIdMain = new SelectList(db.AccountTrees.Where(x => x.AccGroupId == 3), "Id", "AccName", model.AccIdMain);

                    List<AccountTree> AccountTreelist = new List<AccountTree>();
                    AccountTreelist = db.AccountTrees.Where(x => x.AccGroupId == 3).ToList();
                    AccountTreelist.Insert(0, new AccountTree() { AccName = "--Select Main Account--", Id = 0 });
                    ViewBag.AccIdMain = new SelectList(AccountTreelist, "Id", "AccName", model.AccIdMain);
                }
                else
                {
                    ViewBag.AccIdMain = new SelectList(db.AccountTrees.Where(x => x.AccGroupId == 2), "Id", "AccName", model.AccIdMain);
                }
                ViewBag.AccIdDetail = new SelectList(db.AccountTrees.Where(x => x.AccGroupId == 1), "Id", "AccName", model.AccIdDetail);
                ViewBag.CurrencyId = new SelectList(db.Currencies, "Id", "CurrencyName", model.CurrencyId);

            }
            else
            {
                ViewBag.AccIdMain = new SelectList(db.AccountTrees.Where(x => x.AccGroupId == 2), "Id", "AccName", model.AccIdMain);

                ViewBag.AccIdDetail = new SelectList(db.AccountTrees.Where(x => x.AccGroupId == 1), "Id", "AccName", model.AccIdDetail);
                ViewBag.CurrencyId = new SelectList(db.Currencies, "Id", "CurrencyName", model.CurrencyId);
            }
            return PartialView("_AddEditSand", model);

        }
        
        public ActionResult CreateOrUpdateClaim(ClaimAddViewModel model)
        {
            var userId = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(userId);
            int Month;
            int Year;
            DateTime DateFrom = CommonUsed.ConvertToDbDate(model.DateFrom);
            DateTime DateTo = CommonUsed.ConvertToDbDate(model.DateTo);
            int AccId = Convert.ToInt32(model.AccId);
           

            // Config DateFrom - DateTo
            //if (DateFrom.Month != DateTo.Month)
            //{
            //    return Json(new { success = true, message = "InvaildDate" }, JsonRequestBehavior.AllowGet);
            //}
            //if (DateFrom.Year != DateTo.Year)
            //{
            //    return Json(new { success = true, message = "InvaildDateYear" }, JsonRequestBehavior.AllowGet);
            //}
            //هنا التاريخ ما لاوم يبدا من يوم 1 لانه الفواتير بقت اسبوعية عشان كده ح تتلغي ال حته دي
            //if (DateFrom.Day!=1)
            //{

            //    return Json(new { success = true, message = "InvaildStartDate" }, JsonRequestBehavior.AllowGet);
            //}

            Month = Convert.ToDateTime(DateFrom).Month;
            Year = Convert.ToDateTime(DateFrom).Year;

            FinancialClaim1 FinancialClaim = new FinancialClaim1();
            FinancialClaimViewModel Model = new FinancialClaimViewModel();
           
            /*---------------------------------------------- Get ExchangeRate & AirLine -------------------------------------------------------------*/
           
            decimal ExchangeRate = CommonUsed.getExchangeRateByLandingDate(Month, Year);
            AirLine airlineCompany = db.AirLines.Where(x => x.AccId == AccId).FirstOrDefault();

            /*---------------------------------------------- Start Create Or Update -------------------------------------------------------------*/
            // update claim
            if (model.Id > 0)
            {
                //دي مفترض تتلغي عشان يقدر يدخل اكتر من مطالبة في الشهر
                //if (db.FinancialClaims.Any(x => x.Month == Month && x.Year == Year && x.AccIdDebit == model.AccId && x.Id != model.Id)) 
                //{
                //    return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
                //}

                FinancialClaim = db.FinancialClaim1.Find(model.Id);
                ExchangeRate = Convert.ToDecimal(model.ExchangeRate);
                Model = ClacuFreightClaim(airlineCompany.Id, DateFrom, DateTo, ExchangeRate);
                
                bool IsOperation = db.AccountOperations.Any(x => x.OperationID == model.Id && x.OperationTypeId == 6);
                if (IsOperation)
                {
                    //delete old operation
                    List<AccountOperation> Operation = db.AccountOperations.Where(x => x.OperationTypeId == 6 && x.OperationID == model.Id).ToList();
                    
                    foreach (var item in Operation)
                    {
                        db.AccountOperations.Remove(item);
                    }

                    //Add Opertion  operation
                    //--------------------------------------------------عمل قيد مزدوج------------------------------------------
                    AccountOperation OperationDebit = new AccountOperation();
                    AccountOperation OperationCredit = new AccountOperation();

                    int Id = Convert.ToInt32(db.SpLastAccountOperationId().FirstOrDefault());
                    OperationDebit.Id = Id;
                    OperationDebit.AccId = model.AccId;
                    OperationDebit.OperationDebit = Convert.ToDecimal(Model.MonthingClaim);
                    OperationDebit.OperationCredit = 0;
                    OperationDebit.OperationDate = FinancialClaim.DateFrom;
                    OperationDebit.OperationID = model.Id;
                    OperationDebit.CurrencyId = FinancialClaim.CurrencyId;
                    OperationDebit.CurrencyValue = Convert.ToDouble(model.ExchangeRate);
                    OperationDebit.OperationStatement = FinancialClaim.Statement;
                    OperationDebit.OperationTypeId =  6;
                    OperationDebit.CreatedBy = userId;
                    OperationDebit.CreationDate = DateTime.Now;
                    OperationDebit.CompanyInfoId = companyId;
                    ///----------------------------------------------------------------------------

                    int Id2 = Convert.ToInt32(db.SpLastAccountOperationId().FirstOrDefault() + 1);
                    OperationCredit.Id = Id2;
                    OperationCredit.AccId = 7;
                    OperationCredit.OperationDebit = 0;
                    OperationCredit.OperationCredit = Convert.ToDecimal(Model.MonthingClaim);
                    OperationCredit.OperationDate = FinancialClaim.DateFrom;
                    OperationCredit.OperationID = model.Id;
                    OperationCredit.CurrencyId = FinancialClaim.CurrencyId;
                    OperationCredit.CurrencyValue = Convert.ToDouble(Convert.ToDouble(model.ExchangeRate));
                    OperationCredit.OperationStatement = FinancialClaim.Statement;
                    OperationCredit.OperationTypeId = 6;
                    OperationCredit.CreatedBy = userId;
                    OperationCredit.CreationDate = DateTime.Now;
                    OperationCredit.CompanyInfoId = companyId;
                    db.AccountOperations.Add(OperationDebit);
                    db.AccountOperations.Add(OperationCredit);

                    /*------------------------------------------------------------------------------------------------------------------------------------------*/  
                }

                FinancialClaim.AccIdDebit = AccId;
                FinancialClaim.Year = Year;
                FinancialClaim.Month = Month;
                FinancialClaim.Statement = Model.Statement;
                FinancialClaim.Amount = Convert.ToDecimal(Model.MonthingClaim);
                FinancialClaim.CurrencyId = Model.CurrencyId;
                FinancialClaim.OperationTypeId = 6;
                FinancialClaim.DateFrom = DateFrom;
                FinancialClaim.DateTo = DateTo;
                FinancialClaim.UpdatedBy = userId;
                FinancialClaim.UpdatingDate = DateTime.Now;
                FinancialClaim.CompanyInfoId = companyId;
                FinancialClaim.ExchangeRateAgainstPrimaryCurrency = model.ExchangeRate;

                ///-----------------------------------------------------------------------

                //var revenues = db.Revenues
                //        .Where(x =>
                //            x.OfficerData.TowerData.AirLineId == AirLine.Id &&
                //            x.OfficerData.TowerData.Date.HasValue && DbFunctions.TruncateTime(x.OfficerData.TowerData.Date) >= DateFrom.Date &&
                //             DbFunctions.TruncateTime(x.OfficerData.TowerData.Date) <= DateTo.Date &&
                //            x.OfficerData.TowerData.CompanyInfoId == companyId
                //        )
                //        .ToList();

                //revenues = revenues.ForEach(item => { item.ExchangeRate = ExchangeRate; item.CompanyInfoId = companyId; });
                //revenues.ForEach(item =>
                //{
                //    item.ExchangeRate = ExchangeRate;
                //    item.CompanyInfoId = companyId;
                //    //db.Entry(item).State = EntityState.Modified;
                //});

                //db.Entry(revenues).State = EntityState.Modified;
                db.Entry(FinancialClaim).State = EntityState.Modified;
                db.SaveChanges();

                return Json(new { success = true, message = "Update" }, JsonRequestBehavior.AllowGet);
            }
            else //create claim
            {
                //دي مفترض تتلغي عشان يقدر يدخل اكتر من مطالبة في الشهر
                //if (db.FinancialClaims.Any(x => x.Month == Month && x.Year == Year && x.AccIdDebit == model.AccId))
                //{
                //    return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
                //}


                //              if (db.FinancialClaims.Any(x =>
                //    x.AccIdDebit == AirLine.AccId &&
                //    (DbFunctions.TruncateTime(x.DateFrom) <= DateFrom &
                //     DbFunctions.TruncateTime(x.DateTo) >= DateFrom)
                //    |
                //    (DbFunctions.TruncateTime(x.DateFrom) <= DateTo &
                //     DbFunctions.TruncateTime(x.DateTo) >= DateTo &&  x.CompanyInfoId == companyId &&
                // x.TowerDataId == null)
                //))
                //              {
                //                  return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
                //              }


                if (db.FinancialClaim1.Any(x =>
                    x.AccIdDebit == airlineCompany.AccId &&
                    x.CompanyInfoId == companyId &&
                    x.OperationTypeId == 6 &&
                    DbFunctions.TruncateTime(x.DateFrom) <= DateTo &&
                    DbFunctions.TruncateTime(x.DateTo) >= DateFrom
                ))
                {
                    return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
                }

                Model = ClacuFreightClaim(airlineCompany.Id, DateFrom, DateTo, ExchangeRate);

                int Id = Convert.ToInt32(db.SpLastClaimid().FirstOrDefault());
                FinancialClaim.Id = Id;
                FinancialClaim.AccIdDebit = AccId;
                FinancialClaim.Year = Year;
                FinancialClaim.Month = Month;
                FinancialClaim.Statement = Model.Statement;
                FinancialClaim.Amount = Convert.ToDecimal(Model.MonthingClaim);
                FinancialClaim.CurrencyId = Model.CurrencyId;
                FinancialClaim.OperationTypeId = 6;
                FinancialClaim.DateFrom = DateFrom;
                FinancialClaim.DateTo = DateTo;
                FinancialClaim.ExchangeRateAgainstPrimaryCurrency = ExchangeRate;
                FinancialClaim.Date = DateTime.Now;
                FinancialClaim.CreatedBy = userId;
                FinancialClaim.CreationDate = DateTime.Now;

                /*
                    يوجد مشكلة :
                    انه بعد ما يتم طباعة المطالبة بتحث تعديلات في الرحلات
                    مفترض لم نتطلع المطالبة لايتم تعديل بيانات الرحلات الا عن طريق المحاسب المختص
                    منع النظام من ادخال رحلة جديدة في الفترة المحددة بين تاريخين وال تم استخراج المطالبة خلالها
                    منع النظام لاي تعديل يح\دث خلال هذه الفترة الا التعديلات التي تاتي من قبل الموظف المعني

                الحل:
                الحل المقترح هو عمل حق في جدول ال بيانات البرج يتم تحديث هذا الحقل تلقائيا من هنا
                بمعني ان اي رحلة تم ادخال مطالبة لها من هنا يقوم اللنظام بتعديل الحقل
                :في المقابل اي رحلة حددت بانها مقفولة من التعديل لا يمكن تعديل بياناتها في سواء في
                البيانات الاساسية 
                بيانات الركاب
                بيانات عربات الخدمة
                بيانات الوقود
                بيانات المناولة الارضية
                بيانات الغرامات
                وغيرها من البيانات

                التنفيذ:
                تحديث الدالة التي هذه لتعديل قيمة الحقل الذي تم اضافته في قاعدة البيانات
                تحديث باقي الشاشة لاختبار هذه الرحلة مغلقة من التعديل (و) الموظف ليس له دور يسمح له بالتعديل بالتالي يتم منعه من التعديل

                اسم الجدول ال ح يتم فيه اضافة الحقل هو TowerData
                اسم الحقل ال ح يتم اضافتة هو isClosed

                */

                ///-----------------------------------------------------------------------

                //var revenues = db.Revenues
                //       .Where(x =>
                //           x.OfficerData.TowerData.AirLineId == AirLine.Id &&
                //           x.OfficerData.TowerData.Date.HasValue && DbFunctions.TruncateTime(x.OfficerData.TowerData.Date) >= DateFrom.Date &&
                //            DbFunctions.TruncateTime(x.OfficerData.TowerData.Date) <= DateTo.Date &&
                //           x.OfficerData.TowerData.CompanyInfoId == companyId
                //       )
                //       .ToList();

                //revenues = revenues.ForEach(item => { item.ExchangeRate = ExchangeRate; item.CompanyInfoId = companyId; });
                //revenues.ForEach(item =>
                //{
                //    item.ExchangeRate = ExchangeRate;
                //    item.CompanyInfoId = companyId;
                //    //db.Entry(item).State = EntityState.Modified;
                //});


                //db.Entry(revenues).State = EntityState.Modified;
                FinancialClaim.CompanyInfoId = companyId;
                db.FinancialClaim1.Add(FinancialClaim);
                db.SaveChanges();
                return Json(new { success = true, message = "Add" }, JsonRequestBehavior.AllowGet);
            }
        }
        
        public FinancialClaimViewModel ClacuFreightClaim(int? AirlineId, DateTime? DateFrom, DateTime? DateTo, decimal myExchangeRate)
        {
            var userId = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(userId);
            FinancialClaimViewModel Model =new FinancialClaimViewModel();
            
            if (DateTo != null)
            {
                Month = Convert.ToDateTime(DateTo).Month;
                Year = Convert.ToDateTime(DateTo).Year;
            }
            else if (DateFrom != null)
            {
                Month = Convert.ToDateTime(DateFrom).Month;
                Year = Convert.ToDateTime(DateFrom).Year;
            }

           AirLine airLinecompany = db.AirLines.Find(AirlineId);
            //ExchangeRate = CommonUsed.getExchangeRateByLandingDate(Month, Year);

            //string DemandMouth = db.MouthWorkOns.Where(x => x.Id == Month).FirstOrDefault().Mouth;
            //string DemandMouth = Convert.ToDateTime(DateFrom).ToString("MMMM");
            string DemandMouth = Convert.ToDateTime(DateFrom)
            .ToString("MMMM", new CultureInfo("ar-EG"));
 
            string DemandDate = "مطالبة الشحن " + airLinecompany.ArName +" لشهر "+ DemandMouth + " " + Year + "م";
            Model.Statement = DemandDate;


            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date).Where(x => x.OfficerData.TowerData.CompanyInfoId == companyId);

            // هنا حددنا الشركة او الخطوط
            if (AirlineId > 0)
            {
                Revenue = Revenue.Where(x => x.OfficerData.TowerData.AirLineId == AirlineId);
              
            }
           // هنا حددنا انه عاوزين نطلع مطالبة من يوم كم لحدي يوم كم
            if (DateFrom != null && DateTo != null)
            {
                Revenue = Revenue.Where(t => DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) >= DbFunctions.TruncateTime(DateFrom) && DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) <= DbFunctions.TruncateTime(DateTo));
              
            }

            PrintFuelFinalInvoiceViewModel F = new PrintFuelFinalInvoiceViewModel();

            F.Name = db.AirLines.Where(x => x.Id == AirlineId).Single().Name;
            F.FreightFees = Convert.ToDouble(Revenue.Sum(x => x.FreightFees));


            //if (companyId != 11 || !blackListAirlines.Contains(AirLineId ?? 0))
            //{
            //    F.InterNYFees = Convert.ToDouble(Revenue.Sum(x => x.InterNYFees));
            //    F.DomeNYFees = Convert.ToDouble(Revenue.Sum(x => x.DomeNYFees));
            //}
            //else
            //{
            //    F.InterNYFees = 0;
            //    F.DomeNYFees = 0;
            //}



            //if (companyId != 11 || !blackListAirlines.Contains(AirLineId ?? 0))
            //{
            //    F.SubTotalDollar = F.LandingFees + F.ParkingFees + F.NightOperationFees + F.NavigationFees + F.SecurityFees +
            //    F.FirstClassFees + F.FreightFees + F.ServiceCars + F.InterCUPPSFees + F.DomeCUPPSFees + F.InterNYFees +
            //    F.DomeNYFees + F.PushBackFees + F.ExpMailFees + F.ImpMailFees // + F.OpenAireportOvertimeFees
            //    + F.NoiseFees;
            //}else
            //{
            //    F.SubTotalDollar = F.LandingFees + F.ParkingFees + F.NightOperationFees + F.NavigationFees + F.SecurityFees +
            //    F.FirstClassFees + F.FreightFees + F.ServiceCars + F.InterCUPPSFees + F.DomeCUPPSFees +
            //    F.PushBackFees + F.ExpMailFees + F.ImpMailFees // + F.OpenAireportOvertimeFees
            //    + F.NoiseFees;
            //}

            F.SubTotalDollar = F.FreightFees;


            F.TaxDollar = (F.SubTotalDollar) * 0.05;





            F.GrandTotalDollar = (F.SubTotalDollar + F.TaxDollar);
            F.GrandTotalSDG = (F.GrandTotalDollar) * Convert.ToDouble(myExchangeRate);



            int CurrencyType = Convert.ToInt32(db.AirLines.Where(x => x.Id == AirlineId).Single().CurrencyType);
            ///-------------------------------------SDG------------------------------------------------------------

            if (CurrencyType == 2)
            {
                double SDGRound = Convert.ToDouble(Math.Round(F.GrandTotalSDG, 2));

                ToWord toWord = new ToWord(Convert.ToDecimal(SDGRound), currencies[Convert.ToInt32(0)]);

                F.GrandTotalSDGTxt = toWord.ConvertToArabic().ToString();

                Model.CurrencyId = 2;
                Model.MonthingClaim = SDGRound;
            }
            else
            {
                ///--------------------------------------Dollar-----------------------------------------------------------

                double DollarRound = Convert.ToDouble(Math.Round(F.GrandTotalDollar, 2));
                ToWord toWord1 = new ToWord(Convert.ToDecimal(DollarRound), currencies[Convert.ToInt32(1)]);


                F.GrandTotalDollarTxt = toWord1.ConvertToArabic().ToString();

                Model.CurrencyId = 1;
                Model.MonthingClaim = DollarRound;
            }
            ////*************************************************************************************

            return Model;
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
        
        public ActionResult Delete(int id)
        {
            var userId = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(userId);
            FinancialClaim1 Claim = db.FinancialClaim1.Find(id);


            //if (db.SandDetails.Any(x => x.AccId == Claim.AccIdDebit & x.MonthId == Claim.Month && x.YearWorkOn.Year == Claim.Year
            //&& x.SandMain.CompanyInfoId == companyId))
            //{
            //    return Json(new { success = true, message = "SandFound" }, JsonRequestBehavior.AllowGet);
            //}


            if (db.AccountOperations.Any(x => x.OperationTypeId == 3 && x.OperationID == Claim.Id))
            {
                List<AccountOperation> Operation = db.AccountOperations.Where(x => x.OperationTypeId == 3 && x.OperationID == Claim.Id).ToList();


                foreach (var item in Operation)
                {
                    db.AccountOperations.Remove(item);
                }
            }

            db.FinancialClaim1.Remove(Claim);
            db.SaveChanges();


            return Json(new { success = true, message = "Delete" }, JsonRequestBehavior.AllowGet);
        }
        
        //public ActionResult DeleteConfirm(int id)
        //{
        //    FinancialClaim Claim = db.FinancialClaims.Find(id);
             
        //    List<AccountOperation> Operation = db.AccountOperations.Where(x => x.OperationTypeId == 3 && x.OperationID == Claim.Id).ToList();


        //    foreach (var item in Operation)
        //    {
        //        db.AccountOperations.Remove(item);
        //    }


        //    db.FinancialClaims.Remove(Claim);
        //    db.SaveChanges();


        //    return Json(new { success = true, message = "Delete" }, JsonRequestBehavior.AllowGet);



        //}
        
        public ActionResult Transfer(int id)
        {
            FinancialClaim1 Claim = db.FinancialClaim1.Find(id);
            Claim.IsTransfer = true;


            db.Entry(Claim).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return Json(new { success = true, message = "Add" }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult ReTransfer(int id)
        {
            FinancialClaim1 Claim = db.FinancialClaim1.Find(id);
            Claim.IsTransfer = false;


            db.Entry(Claim).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return Json(new { success = true, message = "Add" }, JsonRequestBehavior.AllowGet);

        }
        
    }
}