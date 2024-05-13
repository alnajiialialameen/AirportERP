using AirPortERP.Extenstion;
using AirPortERP.Models;
using AirPortERP.ViewModels;
using Microsoft.AspNet.Identity;
using NumberToWord;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AirPortERP.Controllers
{
    public class AccountClaimController : Controller
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
            return View();
        }
        public ActionResult ConfirmClaimTransaction()
        {
            return View();
        }
        public AccountClaimController()
        {

            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Sudan));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Dollar));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Syria));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.UAE));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.SaudiArabia));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Tunisia));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Gold));



        }
        
        public ActionResult ClaimParial(int? id)
        {

            ClaimAddViewModel model = new ClaimAddViewModel();



            FinancialClaim Claim = new FinancialClaim();

            if (id > 0)
            {

                Claim = db.FinancialClaims.Find(id);
                model.Id = Convert.ToInt32(id);
                model.AccId = Convert.ToInt32(Claim.AccIdDebit);
                model.DateFrom = cls.ConvertToddMMyyyy(Claim.DateFrom.ToString());
                model.DateTo = cls.ConvertToddMMyyyy(Claim.DateTo.ToString());
              
            }



            List<AccountTree> Acclist = new List<AccountTree>();
            Acclist = db.AccountTrees.Where(x => x.AccGroupId == 1).ToList();
            Acclist.Insert(0, new AccountTree() { AccName = "--Select Account--", Id = 0 });
            ViewBag.AccId = new SelectList(Acclist, "Id", "AccName", model.AccId);


          ///  ViewBag.AccId = new SelectList(db.AccountTrees.Where(x => x.AccGroupId == 1), "Id", "AccName", model.AccId);
            return PartialView("_AddEditClaim", model);

        }

        public ActionResult GetAllClaim()
        {

            IQueryable<FinancialClaim> FinancialClaim = db.FinancialClaims.Where(x => x.IsTransfer == false || x.IsTransfer == null);
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
                r.Date = cls.GetDateddMMyyyy(item.Date.ToString());
                r.Statement = item?.Statement;
                r.CurrencyName = item?.Currency.CurrencyName;
                r.IsConfirm = Convert.ToBoolean(item.IsConfirm);

                r.DateFrom = Convert.ToDateTime(item.DateFrom).ToString("dd/MM/yyyy");
                r.DateTo = Convert.ToDateTime(item.DateTo).ToString("dd/MM/yyyy");

                r.TotalAmountCredit = Convert.ToDecimal(item?.Amount).ToString("N");
                //r.AirLineId = db.AirLines.FirstOrDefault(x => x.AccId == r.AccId).Id;
                r.AirLineId = db.AirLines.FirstOrDefault(x => x.AccId == r.AccId).Id;
                r.Year = Convert.ToInt32(item?.Year);
                r.Month = Convert.ToInt32(item?.Month);
                //});
                ClaimList.Add(r);
            }

            return Json(new { data = ClaimList.ToList() }, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult GetAllClaimTransfer()
        {
            var id = User.Identity.GetUserId();
            IQueryable<FinancialClaim> FinancialClaim = db.FinancialClaims.Where(x => x.IsTransfer == true);
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
                r.Date = cls.GetDateddMMyyyy(item.Date.ToString());
                r.Statement = item?.Statement;
                r.CurrencyName = item?.Currency.CurrencyName;
                r.IsConfirm = Convert.ToBoolean(item.IsConfirm);

                r.DateFrom = Convert.ToDateTime(item.DateFrom).ToString("dd/MM/yyyy");
                r.DateTo = Convert.ToDateTime(item.DateTo).ToString("dd/MM/yyyy");

                r.TotalAmountCredit = Convert.ToDecimal(item?.Amount).ToString("N");

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
            FinancialClaim Claim = db.FinancialClaims.Find(id);

            MoneyText = GetAmountTextString(Convert.ToInt32(Claim.CurrencyId), Convert.ToDecimal(Claim.Amount));

            double value = Convert.ToDouble(Claim.Amount);
            string Message = value + "بقيمة" + Claim.AccountTree.AccName + "مطالبة مالية من حساب";

            return Json(new { success = true, message = Message, moneytext = MoneyText }, JsonRequestBehavior.AllowGet);


            ///--------------------------------------Dollar-----------------------------------------------------------
        }
        
        public ActionResult AddToConfrim(int id)
        {
            // string MoneyText = "";
            var userId = User.Identity.GetUserId();
            FinancialClaim Claim = db.FinancialClaims.Find(id);
            Claim.IsConfirm = true;

            ///----------------------------------عمل قيد مزدوج------------------------------------------
            AccountOperation OperationDebit = new AccountOperation();
            AccountOperation OperationCredit = new AccountOperation();
            int Id = Convert.ToInt32(db.SpLastAccountOperationId().FirstOrDefault());

            OperationDebit.Id = Id;
            OperationDebit.AccountNumber = Claim.AccIdDebit;
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
            OperationCredit.AccountNumber = 7;
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
        
        public ActionResult GetSandParial(int? id)
        {

            SandDetail SandDetail = new SandDetail();
            SandAddViewModel model = new SandAddViewModel();


            if (id > 0)
            {

                SandDetail = db.SandDetails.Find(id);

                model.Id = SandDetail.Id;
                model.AccIdDetail = Convert.ToInt32(SandDetail.AccId);
                model.AccIdMain = Convert.ToInt32(SandDetail.SandMain.AccId);

                model.Id = Convert.ToInt32(SandDetail.Id);
                model.MainId = Convert.ToInt32(SandDetail.SandMain.Id);

                model.ISCash = Convert.ToBoolean(SandDetail.ISCash);
                model.ShakNumber = SandDetail.ShakNumber;
                model.Statement = SandDetail.Statement;

                model.TotalAmountCredit = Convert.ToDecimal(SandDetail.SandMain.TotalAmount);

                model.CurrencyId = Convert.ToInt32(SandDetail.CurrencyId);

                model.Date = Convert.ToDateTime(SandDetail.SandMain.SandDate);
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
            int Month;
            int Year;
            DateTime DateFrom = cls.GetDateD(model.DateFrom);
            DateTime DateTo = cls.GetDateD(model.DateTo);
            int AccId = Convert.ToInt32(model.AccId);
            var userId = User.Identity.GetUserId();

            // Config DateFrom - DateTo
            if (DateFrom.Month != DateTo.Month)
            {
                return Json(new { success = true, message = "InvaildDate" }, JsonRequestBehavior.AllowGet);
            }
            if (DateFrom.Year != DateTo.Year)
            {
                return Json(new { success = true, message = "InvaildDateYear" }, JsonRequestBehavior.AllowGet);
            }
            //هنا التاريخ ما لاوم يبدا من يوم 1 لانه الفواتير بقت اسبوعية عشان كده ح تتلغي ال حته دي
            //if (DateFrom.Day!=1)
            //{

            //    return Json(new { success = true, message = "InvaildStartDate" }, JsonRequestBehavior.AllowGet);
            //}


            Month = Convert.ToDateTime(DateFrom).Month;
            Year = Convert.ToDateTime(DateFrom).Year;

            FinancialClaim FinancialClaim = new FinancialClaim();
            FinancialClaimViewModel Model = new FinancialClaimViewModel();


            /*---------------------------------------------- Get ExchangeRate & AirLine -------------------------------------------------------------*/
            decimal ExchangeRate = CommonUsed.getExchangeRateByLandingDate(Month, Year);
            AirLine AirLine = db.AirLines.Where(x => x.AccId == AccId).FirstOrDefault();

            /*---------------------------------------------- Start Create Or Update -------------------------------------------------------------*/
            // update claim
            if (model.Id > 0)
            {
                //دي مفترض تتلغي عشان يقدر يدخل اكتر من مطالبة في الشهر
                //if (db.FinancialClaims.Any(x => x.Month == Month && x.Year == Year && x.AccIdDebit == model.AccId && x.Id != model.Id)) 
                //{
                //    return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
                //}

                FinancialClaim = db.FinancialClaims.Find(model.Id);
                ExchangeRate = Convert.ToDecimal(FinancialClaim.ExchangeRateAgainstPrimaryCurrency);
                Model = Claim(AirLine.Id, DateFrom, DateTo, ExchangeRate);
                
                bool IsOperation = db.AccountOperations.Any(x => x.OperationID == model.Id && x.OperationTypeId == 3);
                if (IsOperation)
                {
                    //delete old operation
                    List<AccountOperation> Operation = db.AccountOperations.Where(x => x.OperationTypeId == 3 && x.OperationID == model.Id).ToList();
                    
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
                    OperationDebit.AccountNumber = model.AccId;
                    OperationDebit.OperationDebit = Convert.ToDecimal(Model.MonthingClaim);
                    OperationDebit.OperationCredit = 0;
                    OperationDebit.OperationDate = FinancialClaim.DateFrom;
                    OperationDebit.OperationID = model.Id;
                    OperationDebit.CurrencyId = FinancialClaim.CurrencyId;
                    OperationDebit.CurrencyValue = Convert.ToDouble(FinancialClaim.ExchangeRateAgainstPrimaryCurrency);
                    OperationDebit.OperationStatement = FinancialClaim.Statement;
                    OperationDebit.OperationTypeId =  3;
                    OperationDebit.CreatedBy = userId;
                    OperationDebit.CreationDate = DateTime.Now;
                    ///----------------------------------------------------------------------------

                    int Id2 = Convert.ToInt32(db.SpLastAccountOperationId().FirstOrDefault() + 1);
                    OperationCredit.Id = Id2;
                    OperationCredit.AccountNumber = 7;
                    OperationCredit.OperationDebit = 0;
                    OperationCredit.OperationCredit = Convert.ToDecimal(Model.MonthingClaim);
                    OperationCredit.OperationDate = FinancialClaim.DateFrom;
                    OperationCredit.OperationID = model.Id;
                    OperationCredit.CurrencyId = FinancialClaim.CurrencyId;
                    OperationCredit.CurrencyValue = Convert.ToDouble(FinancialClaim.ExchangeRateAgainstPrimaryCurrency);
                    OperationCredit.OperationStatement = FinancialClaim.Statement;
                    OperationCredit.OperationTypeId = 3;
                    OperationCredit.CreatedBy = userId;
                    OperationCredit.CreationDate = DateTime.Now;

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
                FinancialClaim.OperationTypeId = 3;
                FinancialClaim.DateFrom = DateFrom;
                FinancialClaim.DateTo = DateTo;
                FinancialClaim.UpdatedBy = userId;
                FinancialClaim.UpdatingDate = DateTime.Now;

                //FinancialClaim.ExchangeRateAgainstPrimaryCurrency = ExchangeRate;


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

                if(db.FinancialClaims.Any(x => x.AccIdDebit == AirLine.AccId && (DbFunctions.TruncateTime(x.DateFrom) <= DateFrom & DbFunctions.TruncateTime(x.DateTo) >= DateFrom) | 
                (DbFunctions.TruncateTime(x.DateFrom) <= DateTo & DbFunctions.TruncateTime(x.DateTo) >= DateTo)))
                {
                    return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
                }
                
                Model = Claim(AirLine.Id, DateFrom, DateTo, ExchangeRate);

                int Id = Convert.ToInt32(db.SpLastClaimid().FirstOrDefault());
                FinancialClaim.Id = Id;
                FinancialClaim.AccIdDebit = AccId;
                FinancialClaim.Year = Year;
                FinancialClaim.Month = Month;
                FinancialClaim.Statement = Model.Statement;
                FinancialClaim.Amount = Convert.ToDecimal(Model.MonthingClaim);
                FinancialClaim.CurrencyId = Model.CurrencyId;
                FinancialClaim.OperationTypeId = 3;
                FinancialClaim.DateFrom = DateFrom;
                FinancialClaim.DateTo = DateTo;
                FinancialClaim.ExchangeRateAgainstPrimaryCurrency = ExchangeRate;
                FinancialClaim.Date = DateTime.Now;
                FinancialClaim.CreatedBy = userId;
                FinancialClaim.CreationDate = DateTime.Now;

                db.FinancialClaims.Add(FinancialClaim);
                db.SaveChanges();
                return Json(new { success = true, message = "Add" }, JsonRequestBehavior.AllowGet);
            }
        }
        
        public FinancialClaimViewModel Claim(int? AirLineId, DateTime? DateFrom, DateTime? DateTo, decimal myExchangeRate)
        {
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

            AirLine Airline = db.AirLines.Find(AirLineId);
            //ExchangeRate = CommonUsed.getExchangeRateByLandingDate(Month, Year);

            string DemandMouth = db.MouthWorkOns.Where(x => x.Id == Month).FirstOrDefault().Mouth;
            
            string DemandDate = "مطالبة " + Airline.ArName +" لشهر "+ DemandMouth + " " + Year + "م";
            Model.Statement = DemandDate;


            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date);


            if (AirLineId > 0)
            {
                Revenue = Revenue.Where(x => x.OfficerData.TowerData.AirLineId == AirLineId);
              
            }
           
            if (DateFrom != null && DateTo != null)
            {
                Revenue = Revenue.Where(t => DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) >= DbFunctions.TruncateTime(DateFrom) && DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) <= DbFunctions.TruncateTime(DateTo));
              
            }
            
            PrintFinalInvoiceViewModel F = new PrintFinalInvoiceViewModel();

            F.Name = db.AirLines.Where(x => x.Id == AirLineId).Single().Code + "/ " + db.AirLines.Where(x => x.Id == AirLineId).Single().Name;
            F.LandingFees = Convert.ToDouble(Revenue.Sum(x => x.LandingFees));
            F.ParkingFees = Convert.ToDouble(Revenue.Sum(x => x.ParkingFees));
            F.NightOperationFees = Convert.ToDouble(Revenue.Sum(x => x.NightOperationFees));
            F.NavigationFees = Convert.ToDouble(Revenue.Sum(x => x.NavigationFees));

            F.SecurityFees = Convert.ToDouble(Revenue.Sum(x => x.SecurityFees));
            F.FirstClassFees = Convert.ToDouble(Revenue.Sum(x => x.FirstClassFees));

            F.InterCUPPSFees = Convert.ToDouble(Revenue.Sum(x => x.InterCUPPSFees));
            F.DomeCUPPSFees = Convert.ToDouble(Revenue.Sum(x => x.DomeCUPPSFees));

            F.InterNYFees = Convert.ToDouble(Revenue.Sum(x => x.InterNYFees));
            F.DomeNYFees = Convert.ToDouble(Revenue.Sum(x => x.DomeNYFees) / myExchangeRate);

            F.PushBackFees = Convert.ToDouble(Revenue.Sum(x => x.PushBackFees));
            F.ServiceCars = Convert.ToDouble(Revenue.Sum(x => x.AmbolanceFees))
                + Convert.ToDouble(Revenue.Sum(x => x.FireFightingFees));

            F.NoiseFees = Convert.ToDouble(Revenue.Sum(x => x.NoiseFees));
            F.ExpMailFees = Convert.ToDouble(Revenue.Sum(x => x.ExpMailFees));
            F.ImpMailFees = Convert.ToDouble(Revenue.Sum(x => x.ImpMailFees));
            F.FreightFees = Convert.ToDouble(Revenue.Sum(x => x.FreightFees));

            F.SubTotalDollar = F.LandingFees + F.ParkingFees + F.NightOperationFees + F.NavigationFees + F.SecurityFees +
                F.FirstClassFees + F.FreightFees + F.ServiceCars + F.InterCUPPSFees + F.DomeCUPPSFees + F.InterNYFees +
                F.DomeNYFees + F.PushBackFees + F.ExpMailFees + F.ImpMailFees // + F.OpenAireportOvertimeFees
            + F.NoiseFees;


            F.TaxDollar = ((F.LandingFees + F.ParkingFees + F.NightOperationFees + F.NavigationFees + F.SecurityFees +
            F.FirstClassFees + F.FreightFees + F.ServiceCars + F.InterCUPPSFees + F.DomeCUPPSFees +
            F.PushBackFees + F.ExpMailFees + F.ImpMailFees // + F.OpenAireportOvertimeFees
            + F.NoiseFees) * 0.05);





            F.GrandTotalDollar = (F.SubTotalDollar + F.TaxDollar);
            F.GrandTotalSDG = (F.GrandTotalDollar) * Convert.ToDouble(myExchangeRate);



            int CurrencyType = Convert.ToInt32(db.AirLines.Where(x => x.Id == AirLineId).Single().CurrencyType);
            ///-------------------------------------SDG------------------------------------------------------------

            if (CurrencyType == 1)
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
            FinancialClaim Claim = db.FinancialClaims.Find(id);


            if (db.SandDetails.Any(x => x.AccId == Claim.AccIdDebit & x.MonthId == Claim.Month && x.YearWorkOn.Year == Claim.Year))
            {
                return Json(new { success = true, message = "SandFound" }, JsonRequestBehavior.AllowGet);
            }


            if (db.AccountOperations.Any(x => x.OperationTypeId == 3 && x.OperationID == Claim.Id))
            {
                List<AccountOperation> Operation = db.AccountOperations.Where(x => x.OperationTypeId == 3 && x.OperationID == Claim.Id).ToList();


                foreach (var item in Operation)
                {
                    db.AccountOperations.Remove(item);
                }
            }

            db.FinancialClaims.Remove(Claim);
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
            FinancialClaim Claim = db.FinancialClaims.Find(id);
            Claim.IsTransfer = true;


            db.Entry(Claim).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return Json(new { success = true, message = "Add" }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult ReTransfer(int id)
        {
            FinancialClaim Claim = db.FinancialClaims.Find(id);
            Claim.IsTransfer = false;


            db.Entry(Claim).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return Json(new { success = true, message = "Add" }, JsonRequestBehavior.AllowGet);

        }
        
    }
}