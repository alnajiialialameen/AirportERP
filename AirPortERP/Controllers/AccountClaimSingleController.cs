using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AirPortERP.Models;
using AirPortERP.Extenstion;
using AirPortERP.ViewModels;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Drawing;
using NumberToWord;
using static NumberToWord.CurrencyInfo;
namespace AirPortERP.Controllers
{
     
    public class AccountClaimSingleController : Controller
    {

        DateTime now = System.DateTime.Now;
        private CommonUsed CommonUsed = new CommonUsed();
        private Entities db = new Entities();
        RoleController Role = new RoleController();
        int WorkMouth;
        int WorkYear;
  
        List<CurrencyInfo> currencies = new List<CurrencyInfo>();
        public AccountClaimSingleController()
        {

            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Sudan));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Dollar));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Syria));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.UAE));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.SaudiArabia));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Tunisia));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Gold));



        }




        public ActionResult ClaimParial(int? towerDataId)
        {

            ClaimAddViewModel2 model = new ClaimAddViewModel2();
            int? accid = db.TowerDatas.Find(towerDataId).AirLine.AccId;


            FinancialClaim1 Claim = new FinancialClaim1();
            bool ishas = db.FinancialClaim1.Any(x => x.TowerDataId == towerDataId);
            if (ishas)
            {

                Claim = db.FinancialClaim1.Where(x => x.TowerDataId == towerDataId).FirstOrDefault();
                model.Id = Convert.ToInt32(Claim.Id);
                model.AccId = Convert.ToInt32(Claim.AccIdDebit);
              


            }
            


            List<AccountTree> Acclist = new List<AccountTree>();
            Acclist = db.AccountTrees.Where(x => x.Id == accid).ToList();
            model.TowerDataId = Convert.ToInt32(towerDataId);
            //Acclist.Insert(0, new AccountTree() { AccName = "--Select Account--", Id = 0 });
            ViewBag.AccId = new SelectList(Acclist, "Id", "AccName", model.AccId);


            ///  ViewBag.AccId = new SelectList(db.AccountTrees.Where(x => x.AccGroupId == 1), "Id", "AccName", model.AccId);
            return PartialView("_AddEditClaimDaily", model);

        }




        public ActionResult Index()
        {
            var id = User.Identity.GetUserId();

            if (db.WorkOns.Any(x => x.UserId == id))
            {
                WorkMouth = Convert.ToInt32(db.WorkOns.Where(x => x.UserId == id).Single().MonthWorkOn);
            }
            if (WorkMouth > 0)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "WorkOns");
            }

        }


        public ActionResult ConfirmClaimTransaction()
        {
            var id = User.Identity.GetUserId();

            if (db.WorkOns.Any(x => x.UserId == id))
            {
                WorkMouth = Convert.ToInt32(db.WorkOns.Where(x => x.UserId == id).Single().MonthWorkOn);
            }
            if (WorkMouth > 0)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "WorkOns");
            }

        }


        
        public ActionResult GetAllTowerDatas()
        {
            IQueryable<Revenue> Revenue;
            var id = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(id);
            //var companyInfo = Role.GetCompanyId(id);

            WorkMouth = CommonUsed.GetMouth(id);
            WorkYear = CommonUsed.GetYear(id);

            // جلب الريفنيو حسب الشهر والسنة
            Revenue = db.Revenues
    .Where(x =>
        x.OfficerData.TowerData.Date.HasValue &&
        x.OfficerData.TowerData.Date.Value.Month == WorkMouth &&
        x.OfficerData.TowerData.Date.Value.Year == WorkYear &&
        x.OfficerData.TowerData.CompanyInfoId == companyId &&
        (
            !db.FinancialClaim1.Any(fc =>
                fc.TowerDataId == x.OfficerData.TowerDataId
            )
            ||
            db.FinancialClaim1.Any(fc =>
                fc.TowerDataId == x.OfficerData.TowerDataId &&
                (fc.IsTransfer == false || fc.IsTransfer == null)
            )
        )
    );



            //// 🔥 جلب كل TowerDataId التي لديها مطالبة (FinancialClaim)
            //var consumedTowerIds = db.FinancialClaims
            //    .Select(x => x.TowerDataId)
            //    .ToList();

            var consumedTowerIds = db.FinancialClaim1
    //.Where(x => x.IsTransfer == false || x.IsTransfer==null)
    .Select(x => x.TowerDataId)
    .ToList();

            ///FinancialClaims هنا اريد ايضاجلب المطالبات بناءا علي جدول 
            // جلبها بناءا علي الشرط   db.FinancialClaims.Where(x=>x.IsTransfer ==true)





            var TowerDatasList = new List<TowerDataViewModels>();

            foreach (var item in Revenue)
            {
                TowerDataViewModels tdm = new TowerDataViewModels();

                var tower = item.OfficerData.TowerData;

                tdm.Id = tower.Id;
                tdm.AirLineCode = tower.AirLine.Code;
                tdm.AirLineName = tower.AirLine.Name;
                tdm.TripType = CommonUsed.GetFlightTypeNameLanding(tower.FlightTypeL);
                tdm.FlightNo = tower.AirLinesFlightTrip.FlightNo;
                tdm.AircraftRegName = tower.AircraftRegistration.Registration;
                tdm.Type = tower.AircraftRegistration.AircraftType.Type;
                tdm.MaxTaKeOffWeight = tower.AircraftRegistration.MaxTakoffWieght.ToString();

                // From
                tdm.AirportIdFrom = CommonUsed.CheckAirportCode(Convert.ToInt32(tower.AirportIdFrom));
                tdm.FromArabicName = tower.AirPort1.NameAr;

                if (Convert.ToInt32(tower.AirPort1.CountryId) > 0)
                    tdm.FromArabicCountry = tower.AirPort1.Country.NameAr;
                else
                    tdm.FromArabicCountry = "غير محدد";

                // To
                tdm.AirportIdTo = CommonUsed.CheckAirportCode(Convert.ToInt32(tower.AirportIdTo));
                tdm.ToArabicName = tower.AirPort.NameAr;

                if (Convert.ToInt32(tower.AirPort.CountryId) > 0)
                    tdm.ToArabicCountry = tower.AirPort.Country.NameAr;
                else
                    tdm.ToArabicCountry = "غير محدد";

                tdm.Date = CommonUsed.ToDisplayDate(tower.Date);
                tdm.ATA = tower.ATA.ToString();
                tdm.ATD = tower.ATD.ToString();

                //tdm.CreatedBy = CommonUsed.GetUserName(tower.CreatedBy);

                tdm.LandingDate = CommonUsed.ToDisplayDate(tower.LandingDate);
                tdm.TakeOffDate = CommonUsed.ToDisplayDate(tower.TakeOffDate);

                tdm.Size = tower.AircraftRegistration.AircraftType.AircraftSize.Size.ToString();

                tdm.LandingFees = Convert.ToDecimal(item.LandingFees).ToString("#,0.##");
                tdm.ParkingFees = Convert.ToDecimal(item.ParkingFees).ToString("#,0.##");
                tdm.NightOperationFees = Convert.ToDecimal(item.NightOperationFees).ToString("#,0.##");
                tdm.NavigationFees = Convert.ToDecimal(item.NavigationFees).ToString("#,0.##");
                tdm.NoiseFees = Convert.ToDecimal(item.NoiseFees).ToString("#,0.##");
                tdm.SecurityFees = Convert.ToDecimal(item.SecurityFees).ToString("#,0.##");

                tdm.DomeNYFees = Convert.ToDecimal(item.DomeNYFees).ToString("#,0.##");
                tdm.InterNYFees = Convert.ToDecimal(item.InterNYFees).ToString("#,0.##");
                tdm.DomeCUPPSFees = Convert.ToDecimal(item.DomeCUPPSFees).ToString("#,0.##");
                tdm.InterCUPPSFees = Convert.ToDecimal(item.InterCUPPSFees).ToString("#,0.##");
                tdm.ImpMailFees = Convert.ToDecimal(item.ImpMailFees).ToString("#,0.##");
                tdm.ExpMailFees = Convert.ToDecimal(item.ExpMailFees).ToString("#,0.##");

                tdm.ServiceCar = (
                    Convert.ToDecimal(item.FireFightingFees) +
                    Convert.ToDecimal(item.AmbolanceFees)
                ).ToString("N0");

                tdm.FirstClassFees = Convert.ToDecimal(item.SecurityFees).ToString("N0");
                tdm.FreightFees = Convert.ToDecimal(item.FreightFees).ToString("N0");
                tdm.AirportName = CommonUsed.getCompanyName(item.OfficerData.TowerData.CompanyInfoId);


                //tdm.IsStop = CommonUsed.IsEntryDateinRange(
                //    Convert.ToDateTime(tower.Date),
                //    Role.GetRole(id),
                //    Convert.ToInt32(tower.AirLineId));

                // 🔥 تحديد هل الرحلة تم استهلاكها (لديها مطالبة)
                tdm.Isconsumed = consumedTowerIds.Contains(tower.Id);

                TowerDatasList.Add(tdm);
            }

            return Json(new { data = TowerDatasList.ToList() }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetAllTowerDatasTransfer()
        {
            IQueryable<Revenue> Revenue;
            var id = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(id);
            //var companyInfo = Role.GetCompanyId(id);

            WorkMouth = CommonUsed.GetMouth(id);
            WorkYear = CommonUsed.GetYear(id);

            // جلب الريفنيو حسب الشهر والسنة
            Revenue = db.Revenues
           .Where(x =>
            x.OfficerData.TowerData.CreationDate.HasValue &&
            x.OfficerData.TowerData.CreationDate.Value.Month == WorkMouth &&
            x.OfficerData.TowerData.CreationDate.Value.Year == WorkYear &&
            x.OfficerData.TowerData.CompanyInfoId == companyId &&
            db.FinancialClaim1.Any(fc =>
                fc.TowerDataId == x.OfficerData.TowerDataId &&
                (fc.IsTransfer == true)
            )
        )
        .OrderBy(x => x.Id);

            //// 🔥 جلب كل TowerDataId التي لديها مطالبة (FinancialClaim)
            //var consumedTowerIds = db.FinancialClaims
            //    .Select(x => x.TowerDataId)
            //    .ToList();

            var consumedTowerIds = db.FinancialClaim1
    .Where(x => x.IsTransfer == true)
    .Select(x => x.TowerDataId)
    .ToList();

            ///FinancialClaims هنا اريد ايضاجلب المطالبات بناءا علي جدول 
            // جلبها بناءا علي الشرط   db.FinancialClaims.Where(x=>x.IsTransfer ==true)





            var TowerDatasList = new List<TowerDataViewModels>();

            foreach (var item in Revenue)
            {
                TowerDataViewModels tdm = new TowerDataViewModels();

                var tower = item.OfficerData.TowerData;

                tdm.Id = tower.Id;
                tdm.AirLineCode = tower.AirLine.Code;
                tdm.AirLineName = tower.AirLine.Name;
                tdm.TripType = CommonUsed.GetFlightTypeNameLanding(tower.FlightTypeL);
                tdm.FlightNo = tower.AirLinesFlightTrip.FlightNo;
                tdm.AircraftRegName = tower.AircraftRegistration.Registration;
                tdm.Type = tower.AircraftRegistration.AircraftType.Type;
                tdm.MaxTaKeOffWeight = tower.AircraftRegistration.MaxTakoffWieght.ToString();

                // From
                tdm.AirportIdFrom = CommonUsed.CheckAirportCode(Convert.ToInt32(tower.AirportIdFrom));
                tdm.FromArabicName = tower.AirPort1.NameAr;

                if (Convert.ToInt32(tower.AirPort1.CountryId) > 0)
                    tdm.FromArabicCountry = tower.AirPort1.Country.NameAr;
                else
                    tdm.FromArabicCountry = "غير محدد";

                // To
                tdm.AirportIdTo = CommonUsed.CheckAirportCode(Convert.ToInt32(tower.AirportIdTo));
                tdm.ToArabicName = tower.AirPort.NameAr;

                if (Convert.ToInt32(tower.AirPort.CountryId) > 0)
                    tdm.ToArabicCountry = tower.AirPort.Country.NameAr;
                else
                    tdm.ToArabicCountry = "غير محدد";

                tdm.Date = Convert.ToDateTime(tower.Date).ToString("MM/dd/yyyy");
                tdm.ATA = tower.ATA.ToString();
                tdm.ATD = tower.ATD.ToString();

                //tdm.CreatedBy = CommonUsed.GetUserName(tower.CreatedBy);

                tdm.LandingDate = CommonUsed.ToDisplayDate(tower.LandingDate);
                tdm.TakeOffDate = CommonUsed.ToDisplayDate(tower.TakeOffDate);

                tdm.Size = tower.AircraftRegistration.AircraftType.AircraftSize.Size.ToString();

                tdm.LandingFees = Convert.ToDecimal(item.LandingFees).ToString("#,0.##");
                tdm.ParkingFees = Convert.ToDecimal(item.ParkingFees).ToString("#,0.##");
                tdm.NightOperationFees = Convert.ToDecimal(item.NightOperationFees).ToString("#,0.##");
                tdm.NavigationFees = Convert.ToDecimal(item.NavigationFees).ToString("#,0.##");
                tdm.NoiseFees = Convert.ToDecimal(item.NoiseFees).ToString("#,0.##");
                tdm.SecurityFees = Convert.ToDecimal(item.SecurityFees).ToString("#,0.##");

                tdm.DomeNYFees = Convert.ToDecimal(item.DomeNYFees).ToString("#,0.##");
                tdm.InterNYFees = Convert.ToDecimal(item.InterNYFees).ToString("#,0.##");
                tdm.DomeCUPPSFees = Convert.ToDecimal(item.DomeCUPPSFees).ToString("#,0.##");
                tdm.InterCUPPSFees = Convert.ToDecimal(item.InterCUPPSFees).ToString("#,0.##");
                tdm.ImpMailFees = Convert.ToDecimal(item.ImpMailFees).ToString("#,0.##");
                tdm.ExpMailFees = Convert.ToDecimal(item.ExpMailFees).ToString("#,0.##");

                tdm.ServiceCar = (
                    Convert.ToDecimal(item.FireFightingFees) +
                    Convert.ToDecimal(item.AmbolanceFees)
                ).ToString("N0");

                tdm.FirstClassFees = Convert.ToDecimal(item.SecurityFees).ToString("N0");
                tdm.FreightFees = Convert.ToDecimal(item.FreightFees).ToString("N0");
                tdm.AirportName = CommonUsed.getCompanyName(item.OfficerData.TowerData.CompanyInfoId);

                //tdm.IsStop = CommonUsed.IsEntryDateinRange(
                //    Convert.ToDateTime(tower.Date),
                //    Role.GetRole(id),
                //    Convert.ToInt32(tower.AirLineId));

                // 🔥 تحديد هل الرحلة تم استهلاكها (لديها مطالبة)
                tdm.Isconsumed = consumedTowerIds.Contains(tower.Id);

                TowerDatasList.Add(tdm);
            }

            return Json(new { data = TowerDatasList.ToList() }, JsonRequestBehavior.AllowGet);
        }



        public ActionResult CreateOrUpdateClaim(ClaimAddViewModel2 model)
        {
            var userId = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(userId);
            int AccId = Convert.ToInt32(model.AccId);

            // جلب بيانات الرحلة
            TowerData towerData = db.TowerDatas.Find(model.TowerDataId);
            if (towerData == null)
                return Json(new { success = false, message = "InvalidTower" }, JsonRequestBehavior.AllowGet);

            int Month = towerData.Date.Value.Month;
            int Year = towerData.Date.Value.Year;

            FinancialClaim1 FinancialClaim = new FinancialClaim1();
            FinancialClaimViewModel Model = new FinancialClaimViewModel();

           
            decimal ExchangeRate = CommonUsed.getExchangeRateByLandingDateForSingle(DateTime.Now);
            if (ExchangeRate == 0) 
            {
                return Json(new { success = false, message = "ExchangeRateNotFound" }, JsonRequestBehavior.AllowGet);
            }

            AirLine AirLine = db.AirLines.FirstOrDefault(x => x.AccId == AccId);

            if (AirLine == null)
                return Json(new { success = false, message = "InvalidAirline" }, JsonRequestBehavior.AllowGet);

            // ======================== UPDATE ========================

            if (model.Id > 0)
            {
                FinancialClaim = db.FinancialClaim1.Find(model.Id);

                ExchangeRate = Convert.ToDecimal(FinancialClaim.ExchangeRateAgainstPrimaryCurrency);

                // 🔥 حساب المطالبة حسب الرحلة فقط
                Model = Claim(model.TowerDataId, ExchangeRate);

                bool IsOperation = db.AccountOperations
                    .Any(x => x.OperationID == model.Id && x.OperationTypeId == 3);

                if (IsOperation)
                {
                    // حذف العمليات القديمة
                    var operations = db.AccountOperations
                        .Where(x => x.OperationTypeId == 3 && x.OperationID == model.Id)
                        .ToList();

                    foreach (var item in operations)
                        db.AccountOperations.Remove(item);

                    // ===== القيد المزدوج =====

                    int Id = Convert.ToInt32(db.SpLastAccountOperationId().FirstOrDefault());

                    AccountOperation OperationDebit = new AccountOperation()
                    {
                        Id = Id,
                        AccId = model.AccId,
                        OperationDebit = Convert.ToDecimal(Model.MonthingClaim),
                        OperationCredit = 0,
                        OperationDate = towerData.Date,
                        OperationID = model.Id,
                        CurrencyId = FinancialClaim.CurrencyId,
                        CurrencyValue = Convert.ToDouble(FinancialClaim.ExchangeRateAgainstPrimaryCurrency),
                        OperationStatement = FinancialClaim.Statement,
                        OperationTypeId = 3,
                        CreatedBy = userId,
                        CreationDate = DateTime.Now,
                        CompanyInfoId = companyId,
                       
                    };

                    AccountOperation OperationCredit = new AccountOperation()
                    {
                        Id = Id + 1,
                        AccId = 7,
                        OperationDebit = 0,
                        OperationCredit = Convert.ToDecimal(Model.MonthingClaim),
                        OperationDate = towerData.Date,
                        OperationID = model.Id,
                        CurrencyId = FinancialClaim.CurrencyId,
                        CurrencyValue = Convert.ToDouble(FinancialClaim.ExchangeRateAgainstPrimaryCurrency),
                        OperationStatement = FinancialClaim.Statement,
                        OperationTypeId = 3,
                        CreatedBy = userId,
                        CreationDate = DateTime.Now,
                        CompanyInfoId = companyId,
                    };

                    db.AccountOperations.Add(OperationDebit);
                    db.AccountOperations.Add(OperationCredit);
                }

                FinancialClaim.AccIdDebit = AccId;
                FinancialClaim.Year = Year;
                FinancialClaim.Month = Month;
                FinancialClaim.Statement = Model.Statement;
                FinancialClaim.Amount = Convert.ToDecimal(Model.MonthingClaim);
                FinancialClaim.CurrencyId = Model.CurrencyId;
                FinancialClaim.OperationTypeId = 3;
                FinancialClaim.UpdatedBy = userId;
                FinancialClaim.UpdatingDate = DateTime.Now;
                FinancialClaim.CompanyInfoId = companyId;
                db.Entry(FinancialClaim).State = EntityState.Modified;
                db.SaveChanges();

                return Json(new { success = true, message = "Update" }, JsonRequestBehavior.AllowGet);
            }

            // ======================== CREATE ========================

            else
            {
                // منع تكرار مطالبة لنفس الرحلة
                if (db.FinancialClaim1.Any(x => x.TowerDataId == model.TowerDataId))
                {
                    return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
                }

                Model = Claim(model.TowerDataId, ExchangeRate);

                int Id = Convert.ToInt32(db.SpLastClaimid().FirstOrDefault());

                FinancialClaim.Id = Id;
                FinancialClaim.AccIdDebit = AccId;
                FinancialClaim.Year = Year;
                FinancialClaim.Month = Month;
                FinancialClaim.Statement = Model.Statement;
                FinancialClaim.Amount = Convert.ToDecimal(Model.MonthingClaim);
                FinancialClaim.CurrencyId = Model.CurrencyId;
                FinancialClaim.OperationTypeId = 3;
                FinancialClaim.ExchangeRateAgainstPrimaryCurrency = ExchangeRate;
                FinancialClaim.Date = DateTime.Now;
                FinancialClaim.CreatedBy = userId;
                FinancialClaim.CreationDate = DateTime.Now;

                // 🔥 ربط المطالبة بالرحلة
                FinancialClaim.TowerDataId = model.TowerDataId;

                ///---------------------------------------------------------------------------------------------



                FinancialClaim.IsConfirm = true;
                FinancialClaim.CompanyInfoId = companyId;
                ///----------------------------------عمل قيد مزدوج------------------------------------------
                AccountOperation OperationDebit = new AccountOperation();
                AccountOperation OperationCredit = new AccountOperation();
                int OperationId = Convert.ToInt32(db.SpLastAccountOperationId().FirstOrDefault());

                OperationDebit.Id = OperationId;
                OperationDebit.AccId = FinancialClaim.AccIdDebit;
                OperationDebit.OperationDebit = FinancialClaim.Amount;
                OperationDebit.OperationCredit = 0;
                OperationDebit.OperationDate = towerData.Date;
                OperationDebit.OperationID = Id;
                OperationDebit.CurrencyId = FinancialClaim.CurrencyId;
                OperationDebit.OperationStatement = FinancialClaim.Statement;
                OperationDebit.OperationTypeId = FinancialClaim.OperationTypeId;
                OperationDebit.CreatedBy = userId;
                OperationDebit.CreationDate = DateTime.Now;
                OperationDebit.CompanyInfoId = companyId;
                ///----------------------------------------------------------------------------
                int Id2 = Convert.ToInt32(db.SpLastAccountOperationId().FirstOrDefault() + 1);
                OperationCredit.Id = Id2;
                OperationCredit.AccId = 7;
                OperationCredit.OperationDebit = 0;
                OperationCredit.OperationCredit = FinancialClaim.Amount;
                OperationCredit.OperationDate = towerData.Date;
                OperationCredit.OperationID = Id;
                OperationCredit.CurrencyId = FinancialClaim.CurrencyId;
                OperationCredit.OperationStatement = FinancialClaim.Statement;
                OperationCredit.OperationTypeId = FinancialClaim.OperationTypeId;
                OperationCredit.CreatedBy = userId;
                OperationCredit.CreationDate = DateTime.Now;
                OperationCredit.CompanyInfoId = companyId;
                db.AccountOperations.Add(OperationDebit);
                db.AccountOperations.Add(OperationCredit);

                db.FinancialClaim1.Add(FinancialClaim);
                db.SaveChanges();

                return Json(new { success = true, message = "Add" }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult AddToConfrim(int id)
        {
            var userId = User.Identity.GetUserId();
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

            db.AccountOperations.Add(OperationDebit);
            db.AccountOperations.Add(OperationCredit);
            db.SaveChanges();

            return Json(new { success = true, message = "Add" }, JsonRequestBehavior.AllowGet);


            ///--------------------------------------Dollar-----------------------------------------------------------
        }


        public FinancialClaimViewModel Claim(int? TowerDataId, decimal myExchangeRate)
        {
            FinancialClaimViewModel Model = new FinancialClaimViewModel();

            if (TowerDataId == null)
                return Model;

            // جلب بيانات الرحلة
            TowerData towerData = db.TowerDatas.Find(TowerDataId);
            if (towerData == null)
                return Model;

            // جلب بيانات الخطوط
            AirLine airline = db.AirLines.Find(towerData.AirLineId);
            if (airline == null)
                return Model;

            // جلب الريفنيو الخاص بهذه الرحلة فقط
            Revenue revenue = db.Revenues
                .FirstOrDefault(x => x.OfficerData.TowerDataId == TowerDataId);

            if (revenue == null)
                return Model;

            // تجهيز النص
            string DemandDate = "مطالبة " + airline.ArName + " ليوم " + CommonUsed.ToDisplayDate(towerData.CreationDate) + "م";
            Model.Statement = DemandDate;

            PrintFinalInvoiceViewModel F = new PrintFinalInvoiceViewModel();

            F.Name = airline.Code + "/ " + airline.Name;

            // ===== الحسابات =====
            F.LandingFees = Convert.ToDouble(revenue.LandingFees ?? 0);
            F.ParkingFees = Convert.ToDouble(revenue.ParkingFees ?? 0);
            F.NightOperationFees = Convert.ToDouble(revenue.NightOperationFees ?? 0);
            F.NavigationFees = Convert.ToDouble(revenue.NavigationFees ?? 0);
            F.SecurityFees = Convert.ToDouble(revenue.SecurityFees ?? 0);
            F.FirstClassFees = Convert.ToDouble(revenue.FirstClassFees ?? 0);
            F.InterCUPPSFees = Convert.ToDouble(revenue.InterCUPPSFees ?? 0);
            F.DomeCUPPSFees = Convert.ToDouble(revenue.DomeCUPPSFees ?? 0);
            F.InterNYFees = Convert.ToDouble(revenue.InterNYFees ?? 0);
            F.DomeNYFees = Convert.ToDouble(revenue.DomeNYFees ?? 0);
            F.PushBackFees = Convert.ToDouble(revenue.PushBackFees ?? 0);

            F.ServiceCars =
                Convert.ToDouble(revenue.AmbolanceFees ?? 0) +
                Convert.ToDouble(revenue.FireFightingFees ?? 0);

            F.NoiseFees = Convert.ToDouble(revenue.NoiseFees ?? 0);
            F.ExpMailFees = Convert.ToDouble(revenue.ExpMailFees ?? 0);
            F.ImpMailFees = Convert.ToDouble(revenue.ImpMailFees ?? 0);
            F.FreightFees = Convert.ToDouble(revenue.FreightFees ?? 0);

            // ===== المجموع =====
            F.SubTotalDollar =
                F.LandingFees + F.ParkingFees + F.NightOperationFees + F.NavigationFees +
                F.SecurityFees + F.FirstClassFees + F.FreightFees + F.ServiceCars +
                F.InterCUPPSFees + F.DomeCUPPSFees + F.InterNYFees + F.DomeNYFees +
                F.PushBackFees + F.ExpMailFees + F.ImpMailFees + F.NoiseFees;

            F.TaxDollar = F.SubTotalDollar * 0.05;

            F.GrandTotalDollar = F.SubTotalDollar + F.TaxDollar;
            F.GrandTotalSDG = F.GrandTotalDollar * Convert.ToDouble(myExchangeRate);

            int CurrencyType = Convert.ToInt32(airline.CurrencyType);

            // ===== تحديد العملة =====
            if (CurrencyType == 1)
            {
                double SDGRound = Math.Round(F.GrandTotalSDG, 2);

                ToWord toWord = new ToWord(Convert.ToDecimal(SDGRound), currencies[0]);
                F.GrandTotalSDGTxt = toWord.ConvertToArabic();

                Model.CurrencyId = 2;
                Model.MonthingClaim = SDGRound;
            }
            else
            {
                double DollarRound = Math.Round(F.GrandTotalDollar, 2);

                ToWord toWord1 = new ToWord(Convert.ToDecimal(DollarRound), currencies[1]);
                F.GrandTotalDollarTxt = toWord1.ConvertToArabic();

                Model.CurrencyId = 1;
                Model.MonthingClaim = DollarRound;
            }

            return Model;
        }





        public ActionResult Transfer(int id)
        {
            FinancialClaim1 Claim = db.FinancialClaim1.Where(x => x.TowerDataId == id).FirstOrDefault();
            Claim.IsTransfer = true;


            db.Entry(Claim).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return Json(new { success = true, message = "Add" }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult ReTransfer(int id)
        {
            FinancialClaim1 Claim = db.FinancialClaim1.Where(x => x.TowerDataId == id).FirstOrDefault();
            Claim.IsTransfer = false;


            db.Entry(Claim).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return Json(new { success = true, message = "Add" }, JsonRequestBehavior.AllowGet);

        }


        public ActionResult Delete(int id)
        {
            FinancialClaim1 Claim = db.FinancialClaim1.Where(x => x.TowerDataId == id).FirstOrDefault();


            //if (db.SandDetails.Any(x => x.AccId == Claim.AccIdDebit & x.MonthId == Claim.Month && x.YearWorkOn.Year == Claim.Year))
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







    }
}