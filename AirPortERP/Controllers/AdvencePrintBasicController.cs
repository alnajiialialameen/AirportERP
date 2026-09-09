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
using System.Web.Mvc;
using static System.Web.Razor.Parser.SyntaxConstants;

namespace AirPortERP.Controllers
{
    public class AdvencePrintBasicController : Controller
    {
        List<CurrencyInfo> currencies = new List<CurrencyInfo>();
        private Entities db = new Entities();
        private CommonUsed CommonUsed = new CommonUsed();
        private RptExtention Ex = new RptExtention();
        private int Format, Month, Year;
        static decimal ExchangeRate;
        private Calculation cal = new Calculation();
        private DateTime currentDate = DateTime.UtcNow.AddHours(2);
        List<int> blackListAirlines = new List<int>() { 176, 107, 52 };

        public AdvencePrintBasicController(){
            
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Sudan));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Dollar));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Syria));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.UAE));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.SaudiArabia));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Tunisia));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Gold));
            
            
            
            }
        
        [Authorize(Roles = "Revenues,Super-Admin,Account, Account-Admin, Account, Account-Admin")]
        public ActionResult PrintDataFinalInvoice(int? AirLineId, DateTime? DateFrom, DateTime? DateTo, decimal ExcRate)
        {
            var userId = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(userId);
            FinalInvoiceNumber finalInvoiceNumber;
            var id = User.Identity.GetUserId();
            RoleController role = new RoleController();
            
            int CurrencyType = Convert.ToInt32(db.AirLines.Where(x => x.Id == AirLineId).Single().CurrencyType);
            
            //int GetCompanyId = role.GetCompanyId(id);
            //var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);

            string UserName = CommonUsed.GetUserName(id);
            int InvoiceNo = 0;
           // string txtInvoiceNo = "";
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
            else
            {
                return RedirectToAction("Error");
            }

            //اذا عملة الدفع هي الجنيه السوداني
            if (CurrencyType == 1)
            {
                if (ExcRate <= 0)
                {
                    ExchangeRate = CommonUsed.getExchangeRateByLandingDate(Month, Year);
                }
                else
                {
                    ExchangeRate = ExcRate;
                }
            }
            else
            {
                ExchangeRate = 0;
            }


            string SearchAirLineId = "غير محدد";
            string SearchDateFrom = "غير محدد";
            string SearchDateTo = "غير محدد";
            string Name = "غير محدد";

            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date).Where(x=>x.OfficerData.TowerData.CompanyInfoId == companyId);

            if (AirLineId > 0)
            {
                Revenue = Revenue.Where(x => x.OfficerData.TowerData.AirLineId == AirLineId);
                SearchAirLineId = db.AirLines.Where(x => x.Id == AirLineId).Single().Name;
                Name = SearchAirLineId;
            }

            if (DateFrom != null && DateTo != null)
            {
                Revenue = Revenue.Where(t => DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) >= DbFunctions.TruncateTime(DateFrom) && DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) <= DbFunctions.TruncateTime(DateTo));
                SearchDateFrom = CommonUsed.ToDisplayDate(DateFrom).ToString();
                SearchDateTo = CommonUsed.ToDisplayDate(DateTo).ToString();
            }
            if (Revenue.Count() <= 0)
            {
                return RedirectToAction("Error");
            }
            ///***********************************************************************************
            var RevenueList = new List<PrintFinalInvoiceViewModel>();

            PrintFinalInvoiceViewModel F = new PrintFinalInvoiceViewModel();

            F.Name = db.AirLines.Where(x => x.Id == AirLineId).Single().Code + "/ " + db.AirLines.Where(x => x.Id == AirLineId).Single().Name;
            F.LandingFees = Convert.ToDouble(Revenue.Sum(x => x.LandingFees));
            F.ParkingFees = Convert.ToDouble(Revenue.Sum(x => x.ParkingFees));
            F.NightOperationFees = Convert.ToDouble(Revenue.Sum(x => x.NightOperationFees));
            F.NavigationFees = Convert.ToDouble(Revenue.Sum(x => x.NavigationFees));

            F.SecurityFees = Convert.ToDouble(Revenue.Sum(x => x.SecurityFees));
            F.FirstClassFees = Convert.ToDouble(Revenue.Sum(x => x.FirstClassFees));

            //F.InterCUPPSFees = Convert.ToDouble(Revenue.Where(x=>x.OfficerData.TowerData.FlightTypeD==1).Sum(x => x.InterCUPPSFees));
            F.InterCUPPSFees = Convert.ToDouble(Revenue.Sum(x => x.InterCUPPSFees));

            //F.InterCUPPSFees = Convert.ToInt32(Revenue.Where(x=> x.OfficerData.TowerData.FlightTypeD == 1).Sum(x => x.OfficerData.Pax + x.OfficerData.TRNYCUPPS)) * 2.5;
            F.DomeCUPPSFees = Convert.ToDouble(Revenue.Sum(x => x.DomeCUPPSFees));

            if (companyId != 11 || !blackListAirlines.Contains(AirLineId ?? 0))
            {
                F.InterNYFees = Convert.ToDouble(Revenue.Sum(x => x.InterNYFees));
                F.DomeNYFees = Convert.ToDouble(Revenue.Sum(x => x.DomeNYFees));
            }
            else
            {
                F.InterNYFees = 0;
                F.DomeNYFees = 0;
            }

            //F.InterNYFees = Convert.ToDouble(Revenue.Sum(x => x.InterNYFees));
            //if (ExchangeRate > 0)
            //{
            ////    old
            ////    F.DomeNYFees = Convert.ToDouble(Revenue.Sum(x => x.DomeNYFees) / ExchangeRate);
            ////    F.DomeNYFees = Convert.ToDouble(Revenue.Where(x => x.OfficerData.TowerData.FlightTypeD == 2).Sum(x => x.DomeNYFees) / ExchangeRate);

            //    F.DomeNYFees = Convert.ToDouble(Revenue.Sum(x => x.DomeNYFees));
            //    F.DomeNYFees = Convert.ToDouble(Revenue.Where(x => x.OfficerData.TowerData.FlightTypeD == 2).Sum(x => x.DomeNYFees));
            //}
            //else
            //{
            //    F.DomeNYFees = Convert.ToDouble(Revenue.Sum(x => x.DomeNYFees));
            //    F.DomeNYFees = Convert.ToDouble(Revenue.Where(x => x.OfficerData.TowerData.FlightTypeD == 2).Sum(x => x.DomeNYFees));
            //}


            F.PushBackFees = Convert.ToDouble(Revenue.Sum(x => x.PushBackFees));
            F.ServiceCars = Convert.ToDouble(Revenue.Sum(x => x.AmbolanceFees))
                + Convert.ToDouble(Revenue.Sum(x => x.FireFightingFees));

            F.NoiseFees = Convert.ToDouble(Revenue.Sum(x => x.NoiseFees));
            F.ExpMailFees = Convert.ToDouble(Revenue.Sum(x => x.ExpMailFees));
            F.ImpMailFees = Convert.ToDouble(Revenue.Sum(x => x.ImpMailFees));
            F.FreightFees = Convert.ToDouble(Revenue.Sum(x => x.FreightFees));

            F.SubTotalDollar = F.LandingFees + F.ParkingFees + F.NightOperationFees + F.NavigationFees + F.SecurityFees +
                F.FirstClassFees + F.FreightFees + F.ServiceCars + F.InterCUPPSFees + F.DomeCUPPSFees + /*F.InterNYFees +
                F.DomeNYFees*/ + F.PushBackFees + F.ExpMailFees + F.ImpMailFees // + F.OpenAireportOvertimeFees
            + F.NoiseFees;

            F.SubTotalSDG = Convert.ToDouble(F.SubTotalDollar * Convert.ToDouble(ExchangeRate));

            F.TaxDollar = (F.LandingFees + F.ParkingFees + F.NightOperationFees + F.NavigationFees + F.SecurityFees +
                F.FirstClassFees + F.FreightFees + F.ServiceCars + F.InterCUPPSFees + F.DomeCUPPSFees /*+ F.InterNYFees*/ +
               /* F.DomeNYFees +*/ F.PushBackFees + F.ExpMailFees + F.ImpMailFees // + F.OpenAireportOvertimeFees
            + F.NoiseFees) * 0.05;
            F.TaxSDG = F.TaxDollar * Convert.ToDouble(ExchangeRate);



            F.GrandTotalDollar = F.SubTotalDollar + F.TaxDollar;

            F.GrandTotalSDG = F.SubTotalSDG + F.TaxSDG;


            RevenueList.Add(F);

            if (db.FinalInvoiceNumbers.Any(x => x.Month == Month && x.Year == Year))
            {
                if (db.FinalInvoiceNumbers.Any(x => x.AirLineId == AirLineId && x.Month == Month && x.Year == Year))
                {
                    InvoiceNo = Convert.ToInt32(db.FinalInvoiceNumbers.FirstOrDefault(x => x.AirLineId == AirLineId && x.Month == Month && x.Year == Year).InvoiceNo);
                }
                else
                {
                    InvoiceNo = db.FinalInvoiceNumbers.Where(x => x.Month == Month && x.Year == Year).OrderBy(b => b.InvoiceNo).Max(n => n.InvoiceNo).Value + 1;

                    finalInvoiceNumber = new FinalInvoiceNumber();
                    finalInvoiceNumber.Month = Month;
                    finalInvoiceNumber.Year = Year;
                    finalInvoiceNumber.InvoiceNo = InvoiceNo;
                    finalInvoiceNumber.AirLineId = AirLineId;
                    finalInvoiceNumber.IsFinal = false;
                    finalInvoiceNumber.CreatedBy = id;
                    finalInvoiceNumber.CreationDate = DateTime.UtcNow.ToShortDateString();
                    db.FinalInvoiceNumbers.Add(finalInvoiceNumber);
                    db.SaveChanges();

                }
            }
            else
            {
                finalInvoiceNumber = new FinalInvoiceNumber();
                finalInvoiceNumber.Month = Month;
                finalInvoiceNumber.Year = Year;
                finalInvoiceNumber.InvoiceNo = 1;
                finalInvoiceNumber.AirLineId = AirLineId;
                finalInvoiceNumber.IsFinal = false;
                finalInvoiceNumber.CreatedBy = id;
                finalInvoiceNumber.CreationDate = DateTime.UtcNow.ToShortDateString();
                db.FinalInvoiceNumbers.Add(finalInvoiceNumber);
                db.SaveChanges();

                InvoiceNo = 1;
            }
 
            F.txtInvoiceNo = "#" + Month.ToString("00") + Year.ToString() + "00" + InvoiceNo.ToString();

            ////*************************************************************************************
            F.HeaderDate = Convert.ToDateTime(DateFrom).ToString("MMM-yyyy");
            F.DateFrom = SearchDateFrom;
            F.DateTo = SearchDateTo;
            F.ExchangeRate = Convert.ToDouble(ExchangeRate);
            F.DateNow = DateTime.Now.ToString() ;
           

            if (CurrencyType == 1)
            {
                double NumberSDGRound = Convert.ToDouble(F.GrandTotalSDG);
                double SDGRounded = Math.Round(NumberSDGRound, 2);


                F.NumberWord = CurrencyFormat.ConvertToWords(SDGRounded.ToString(),"SDG");

                ViewBag.Header = "Final-" + Name + "-" + DateTime.Now.ToShortDateString();
                return View(F);
            }
            else
            {
                double NumberDollarRound = Convert.ToDouble(F.GrandTotalDollar);
                double DollarRounded = Math.Round(NumberDollarRound, 2);
                F.NumberWord = CurrencyFormat.ConvertToWords(DollarRounded.ToString(),"Dollar");

                ViewBag.Header = "Final-" + Name + "-" + DateTime.Now.ToShortDateString();
                return View("PrintDataFinalInvoiceNotSDG", F);
            }


          
        }

        public ActionResult PrintDataFinalInvoiceDaily(int? TowerDataId, decimal ExcRate)
        {
            if (TowerDataId == null)
                return RedirectToAction("Error");

            var id = User.Identity.GetUserId();

            TowerData towerData = db.TowerDatas.Find(TowerDataId);
            FinancialClaim1 financialClaim = db.FinancialClaim1.Where(x => x.TowerDataId == TowerDataId).FirstOrDefault();
            string InoviceDate = CommonUsed.ToDisplayDate(financialClaim.Date);
            ViewBag.InoviceDate = InoviceDate;


            if (towerData == null)
                return RedirectToAction("Error");

            int AirLineId = Convert.ToInt32(towerData.AirLineId);
            int CurrencyType = Convert.ToInt32(towerData.AirLine.CurrencyType);

            int Month = towerData.Date.Value.Month;
            int Year = towerData.Date.Value.Year;

            decimal ExchangeRate;

            // ===== تحديد سعر الصرف =====
            if (CurrencyType == 1)
            {
                ExchangeRate = ExcRate > 0
                    ? ExcRate
                    : CommonUsed.getExchangeRateByLandingDate(Month, Year);
            }
            else
            {
                ExchangeRate = 0;
            }

            // ===== جلب Revenue الخاص بهذه الرحلة فقط =====
            var revenue = db.Revenues
                .FirstOrDefault(x => x.OfficerData.TowerDataId == TowerDataId);

            if (revenue == null)
                return RedirectToAction("Error");

            PrintFinalInvoiceViewModel F = new PrintFinalInvoiceViewModel();

            var airline = towerData.AirLine;

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

            // ===== SubTotal =====

            F.SubTotalDollar =
                F.LandingFees + F.ParkingFees + F.NightOperationFees + F.NavigationFees +
                F.SecurityFees + F.FirstClassFees + F.FreightFees + F.ServiceCars +
                F.InterCUPPSFees + F.DomeCUPPSFees + F.InterNYFees + F.DomeNYFees +
                F.PushBackFees + F.ExpMailFees + F.ImpMailFees + F.NoiseFees;

            F.SubTotalSDG = F.SubTotalDollar * Convert.ToDouble(ExchangeRate);

            // ===== الضريبة =====

            F.TaxDollar = (
                F.LandingFees + F.ParkingFees + F.NightOperationFees +
                F.NavigationFees + F.SecurityFees + F.FirstClassFees +
                F.FreightFees + F.ServiceCars + F.InterCUPPSFees +
                F.DomeCUPPSFees + F.PushBackFees +
                F.ExpMailFees + F.ImpMailFees + F.NoiseFees
            ) * 0.05;

            F.TaxSDG = F.TaxDollar * Convert.ToDouble(ExchangeRate);

            // ===== الإجمالي =====

            F.GrandTotalDollar = F.SubTotalDollar + F.TaxDollar;
            F.GrandTotalSDG = F.SubTotalSDG + F.TaxSDG;

            // ===== رقم الفاتورة =====

            int InvoiceNo;
            FinalInvoiceNumber finalInvoiceNumber;

            if (db.FinalInvoiceNumbers.Any(x => x.Month == Month && x.Year == Year))
            {
                if (db.FinalInvoiceNumbers.Any(x => x.AirLineId == AirLineId && x.Month == Month && x.Year == Year))
                {
                    InvoiceNo = db.FinalInvoiceNumbers
                        .First(x => x.AirLineId == AirLineId && x.Month == Month && x.Year == Year)
                        .InvoiceNo.Value;
                }
                else
                {
                    InvoiceNo = db.FinalInvoiceNumbers
                        .Where(x => x.Month == Month && x.Year == Year)
                        .Max(n => n.InvoiceNo).Value + 1;

                    finalInvoiceNumber = new FinalInvoiceNumber()
                    {
                        Month = Month,
                        Year = Year,
                        InvoiceNo = InvoiceNo,
                        AirLineId = AirLineId,
                        IsFinal = false,
                        CreatedBy = id,
                        CreationDate = DateTime.UtcNow.ToShortDateString()
                    };

                    db.FinalInvoiceNumbers.Add(finalInvoiceNumber);
                    db.SaveChanges();
                }
            }
            else
            {
                finalInvoiceNumber = new FinalInvoiceNumber()
                {
                    Month = Month,
                    Year = Year,
                    InvoiceNo = 1,
                    AirLineId = AirLineId,
                    IsFinal = false,
                    CreatedBy = id,
                    CreationDate = DateTime.UtcNow.ToShortDateString()
                };

                db.FinalInvoiceNumbers.Add(finalInvoiceNumber);
                db.SaveChanges();

                InvoiceNo = 1;
            }

            F.txtInvoiceNo = "#" + Month.ToString("00") + Year.ToString() + "00" + InvoiceNo.ToString();

            // ===== Header =====

            F.HeaderDate = towerData.Date.Value.ToString("MMM-yyyy", CultureInfo.InvariantCulture);


            F.ExchangeRate = Convert.ToDouble(ExchangeRate);
            F.DateNow = DateTime.Now.ToString();
            F.DateFrom = CommonUsed.ToDisplayDate(towerData.Date);
            F.DateTo = CommonUsed.ToDisplayDate(towerData.Date);

            if (CurrencyType == 1)
            {
                double SDGRounded = Math.Round(F.GrandTotalSDG, 2);
                F.NumberWord = CurrencyFormat.ConvertToWords(SDGRounded.ToString(), "SDG");

                ViewBag.Header = "Final-" + airline.Name + "-" + DateTime.Now.ToShortDateString();
                return View(F);
            }
            else
            {
                double DollarRounded = Math.Round(F.GrandTotalDollar, 2);
                F.NumberWord = CurrencyFormat.ConvertToWords(DollarRounded.ToString(), "Dollar");

                ViewBag.Header = "Final-" + airline.Name + "-" + DateTime.Now.ToShortDateString();
               
                return View("PrintDataFinalInvoiceNotSDG", F);
            }
        }

        public ActionResult PrintDataLandingDom() {
            return View();
        }
        
        [Authorize(Roles = "Revenues,Super-Admin,Landing,Mail,Freight,Landing-Admin, Account, Account-Admin")]
        public ActionResult PrintDataLanding(int? AirLineId, DateTime? DateFrom, DateTime? DateTo, int? FlightTypeId)
        {
            var id = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(id);
            RoleController role = new RoleController();
            //int GetCompanyId = role.GetCompanyId(id);
            //var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);
            string TitleDiscount = "";
            string UserName = CommonUsed.GetUserName(id);
            bool IsLocalCompany = false;

            string Name = "غير محدد";
            string SearchDateFrom = "غير محدد";
            string SearchDateTo = "غير محدد";
            string MessageDiscount = "";
            string Discount = "";

         //   ReportDocument rd = new ReportDocument();

            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date).Where(x => x.OfficerData.TowerData.CompanyInfoId == companyId);

            if (FlightTypeId > 0)
            {
                if (FlightTypeId == 1)
                {
                    Revenue = Revenue.Where(x => x.OfficerData.TowerData.FlightTypeL == 1);
                   
                }
                else
                {
                    Revenue = Revenue.Where(x => x.OfficerData.TowerData.FlightTypeL == 2);
                   
                }
            }

            if (AirLineId > 0)
            {
                Revenue = Revenue.Where(x => x.OfficerData.TowerData.AirLineId == AirLineId);
                 Name = db.AirLines.Where(x => x.Id == AirLineId).Single().Name;

                IsLocalCompany = Convert.ToBoolean(db.AirLines.Where(x => x.Id == AirLineId).Single().IsLocalCompany);

                if (IsLocalCompany)
                {
                    GeneralService gs = db.GeneralServices.FirstOrDefault(x => x.Id == 5);
                    if (gs.Value > 0)
                    {
                        int Type = Convert.ToInt32(gs.Type);
                        decimal Value = 0;
                        if (Type == 2)//means It's a value Not Percent
                        {
                            Value = Convert.ToDecimal(gs.Value);
                            MessageDiscount = "*Discount   " + Convert.ToDouble(Value) + "$ from Security Fees";
                            Discount = "(- " + Convert.ToDouble(Value) + "$)";
                            TitleDiscount = "Discount-" + Convert.ToDouble(Value) + "$";
                           
                        }
                        else //means It's a Percent Not value
                        {
                            Value = Convert.ToDecimal(gs.Value);
                            MessageDiscount = " *Discount " + Convert.ToDouble(Value) + "% from Security Fees";
                            Discount = "(- " + Convert.ToDouble(Value) + "%)";
                            TitleDiscount = "Discount-" + Convert.ToDouble(Value) + "%";
                           
                        }
                    }
                }
            }

            ViewBag.TitleDiscount = TitleDiscount;
            if (DateFrom != null && DateTo != null)
            {
                Revenue = Revenue.Where(t => DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) >= DbFunctions.TruncateTime(DateFrom) && DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) <= DbFunctions.TruncateTime(DateTo));
                SearchDateFrom = CommonUsed.ToDisplayDate(DateFrom).ToString();
                SearchDateTo = CommonUsed.ToDisplayDate(DateTo).ToString();
            }
            if (Revenue.Count() <= 0)
            {
                return RedirectToAction("Error");
            }
            ///***********************************************************************************
            var RevenueList = new List<PrintLandingViewModel>();
            int N = 0;
            foreach (var item in Revenue)
            {
                PrintLandingViewModel L = new PrintLandingViewModel();
                L.Id = item.Id;
                N = N + 1;
                L.N = N;
                // L.Name = item?.OfficerData.TowerData.AirLine.Code + "/ " + item?.OfficerData.TowerData.AirLine.Name;
                L.FlightNo = item?.OfficerData.TowerData.AirLinesFlightTrip.FlightNo;
                L.MaxTakoffWieght = item?.OfficerData.TowerData.AircraftRegistration.MaxTakoffWieght.ToString();
                L.LandingDate = CommonUsed.ToDisplayDate(item?.OfficerData.TowerData.LandingDate);
                L.Wt = Convert.ToDouble(item.OfficerData.TowerData.AircraftRegistration.MaxTakoffWieght).ToString();
                L.Type = item.OfficerData.TowerData.AircraftRegistration.AircraftType.Type.ToString();
                L.RegNo = item.OfficerData.TowerData.AircraftRegistration.Registration.ToString();
                L.LandingFees = Convert.ToDecimal(item?.LandingFees);
                L.ParkingFees = Convert.ToDecimal(item?.ParkingFees);
                L.NightOperationFees = Convert.ToDecimal(item?.NightOperationFees);
                L.NavigationFees = Convert.ToDecimal(item?.NavigationFees);
                L.SecurityFees = Convert.ToDecimal(item?.SecurityFees);
                L.FreightFees = Convert.ToDecimal(item?.FreightFees);
                L.NoiseFees = Convert.ToDecimal(item?.NoiseFees);
                L.ImpMailFees = Convert.ToDecimal(item?.ImpMailFees);
                L.ExpMailFees = Convert.ToDecimal(item?.ExpMailFees);
                L.Total = L.ParkingFees + L.LandingFees + L.NightOperationFees + L.NavigationFees + L.FreightFees + L.SecurityFees
                    + L.NoiseFees + L.ExpMailFees + L.ImpMailFees;

                RevenueList.Add(L);
            }
               
           

            ViewBag.DateFrom = SearchDateFrom.ToString();
            ViewBag.DateTo = SearchDateTo.ToString();

            double SumLandingFees = Convert.ToDouble(RevenueList.Sum(x => x.LandingFees));
            ViewBag.SumLandingFees = SumLandingFees;

            double SumNightOperationFees = Convert.ToDouble(RevenueList.Sum(x => x.NightOperationFees));
            ViewBag.SumNightOperationFees = SumNightOperationFees;

            double SumNavigationFees = Convert.ToDouble(RevenueList.Sum(x => x.NavigationFees));
            ViewBag.SumNavigationFees = SumNavigationFees;

            double SumSecurityFees = Convert.ToDouble(RevenueList.Sum(x => x.SecurityFees));
            ViewBag.SumSecurityFees = SumSecurityFees;

            double SumParkingFees = Convert.ToDouble(RevenueList.Sum(x => x.ParkingFees));
            ViewBag.SumParkingFees = SumParkingFees;

            double SumFreightFees = Convert.ToDouble(RevenueList.Sum(x => x.FreightFees));
            ViewBag.SumFreightFees = SumFreightFees;

            double SumImpMailFees = Convert.ToDouble(RevenueList.Sum(x => x.ImpMailFees));
            ViewBag.SumImpMailFees = SumImpMailFees;

            double SumExpMailFees = Convert.ToDouble(RevenueList.Sum(x => x.ExpMailFees));
            ViewBag.SumExpMailFees = SumExpMailFees;

            double SumNoiseFees = Convert.ToDouble(RevenueList.Sum(x => x.NoiseFees));
            ViewBag.SumNoiseFees = SumNoiseFees;


            double SumTotal = Convert.ToDouble(RevenueList.Sum(x => x.Total));
            ViewBag.SumTotal = SumTotal;

            ///////////////////////////////////////////
            double Tax = SumTotal * 0.05;
            ViewBag.Tax = Tax;

            double GrandTotal = Tax + SumTotal;
            ViewBag.GrandTotal = GrandTotal;




            if (AirLineId > 0)
            {
                AirLine airline = db.AirLines.Find(AirLineId);
                ViewBag.Name = airline.Name + "/" + airline.Code;
            }
            //************************************************************************************/
            
                if (FlightTypeId == 1)
                {
                ViewBag.Header = "Landing-" + Name + "-" + DateTime.Now.ToShortDateString();
                return View(RevenueList);

            }
                else
            {
                ViewBag.Header = "Landing-" + Name + "-" + DateTime.Now.ToShortDateString();
                return View("PrintDataLandingDom", RevenueList);

            }
            




           
        }
        
        public ActionResult PrintDataGrafDollar() {

            return View();
        }

        [Authorize(Roles = "Revenues,Super-Admin, Account, Account-Admin")]
        public ActionResult PrintDataGraf(int? AirLineId, DateTime? DateFrom, DateTime? DateTo, decimal ExcRate)
        {
            var id = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(id);
            RoleController role = new RoleController();

            int AirLineAccId = 0;
            int GetCompanyId = role.GetCompanyId(id);
            var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);
            
            string RevenuesManager = CompanyInfo.RevenuesManager;
            ViewBag.RevenuesManager = RevenuesManager;
            ViewBag.companyId = GetCompanyId;


            string DateNow = System.DateTime.Now.ToString("yyyy/MM/dd") + "م";
            ViewBag.DateNow = DateNow;

            string Serial = Guid.NewGuid().ToString();

            string UserName = CommonUsed.GetUserName(id);

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
            else
            {
                //Month = Convert.ToDateTime(currentDate).Month;
                //Year = Convert.ToDateTime(currentDate).Year;
                return RedirectToAction("Error");
            }

            int CurrencyType = Convert.ToInt32(db.AirLines.Where(x => x.Id == AirLineId).Single().CurrencyType);

            //اذا عملة الدفع هي الجنيه السوداني
            if (CurrencyType == 1)
            {
                if (ExcRate <= 0)
                {
                    ExchangeRate = CommonUsed.getExchangeRateByLandingDate(Month, Year);
                }
                else
                {
                    ExchangeRate = ExcRate;
                }
            }
            else
            {
                ExchangeRate = 0;
            }


            //ده كود لاستجلاب سعر الصرف و بقى ما شغال بعد شغل الحسابات
            //ExchangeRate = CommonUsed.getExchangeRateByLandingDate(Month, Year);
            //ViewBag.ExchangeRate = ExchangeRate;




            //string DemandMouth = Convert.ToDateTime(DateFrom).ToString("MMMM");
            string DemandMouth = Convert.ToDateTime(DateFrom).ToString("MMMM", new CultureInfo("ar-EG"));
       

            //string DemandMouth = db.MouthWorkOns.Where(x => x.Id == Month).FirstOrDefault().Mouth;
            //  string DemandYear = db.WorkOns.Where(x => x.UserId == id).FirstOrDefault().YearWorkOn.Year.ToString();




            string DemandDate = "مطالبة شهر " + DemandMouth + " " + Year + "م";
            ViewBag.DemandDate = DemandDate;

            string ArName = "غير محدد";
            string Name = "غير محدد";

            string SearchDateFrom = "غير محدد";
            string SearchDateTo = "غير محدد";

          
            


            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date).Where(x => x.OfficerData.TowerData.CompanyInfoId == companyId);


            if (AirLineId > 0)
            {
                Revenue = Revenue.Where(x => x.OfficerData.TowerData.AirLineId == AirLineId);
                ArName = db.AirLines.Where(x => x.Id == AirLineId).Single().ArName;
                AirLineAccId = (int)db.AirLines.Where(x => x.Id == AirLineId).Single().AccId;

                Name = db.AirLines.Where(x => x.Id == AirLineId).Single().Name;
            }

            ViewBag.ArName = ArName;
            if (DateFrom != null && DateTo != null)
            {
                Revenue = Revenue.Where(t => DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) >= DbFunctions.TruncateTime(DateFrom) && DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) <= DbFunctions.TruncateTime(DateTo));
                SearchDateFrom = CommonUsed.ToDisplayDate(DateFrom).ToString();
                SearchDateTo = CommonUsed.ToDisplayDate(DateTo).ToString();
            }
            //if (Revenue.Count() <= 0)
            //{
            //    return RedirectToAction("Error");
            //}
            ///***********************************************************************************
          //  var RevenueList = new List<PrintFinalInvoiceViewModel>();

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
            F.DomeNYFees = Convert.ToDouble(Revenue.Sum(x => x.DomeNYFees));

            F.PushBackFees = Convert.ToDouble(Revenue.Sum(x => x.PushBackFees));
            F.ServiceCars = Convert.ToDouble(Revenue.Sum(x => x.AmbolanceFees))
                + Convert.ToDouble(Revenue.Sum(x => x.FireFightingFees));

            F.NoiseFees = Convert.ToDouble(Revenue.Sum(x => x.NoiseFees));
            F.ExpMailFees = Convert.ToDouble(Revenue.Sum(x => x.ExpMailFees));
            F.ImpMailFees = Convert.ToDouble(Revenue.Sum(x => x.ImpMailFees));
            F.FreightFees = Convert.ToDouble(Revenue.Sum(x => x.FreightFees));

            if (companyId != 11 || !blackListAirlines.Contains(AirLineId??0))
            {
                F.SubTotalDollar = F.LandingFees + F.ParkingFees + F.NightOperationFees + F.NavigationFees + F.SecurityFees +
                F.FirstClassFees + F.FreightFees + F.ServiceCars + F.InterCUPPSFees + F.DomeCUPPSFees + F.InterNYFees +
                F.DomeNYFees + F.PushBackFees + F.ExpMailFees + F.ImpMailFees // + F.OpenAireportOvertimeFees
            + F.NoiseFees;
            }
            else
            {
                F.SubTotalDollar = F.LandingFees + F.ParkingFees + F.NightOperationFees + F.NavigationFees + F.SecurityFees +
                    F.FirstClassFees + F.FreightFees + F.ServiceCars + F.InterCUPPSFees + F.DomeCUPPSFees + 
                    F.PushBackFees + F.ExpMailFees + F.ImpMailFees // + F.OpenAireportOvertimeFees
                + F.NoiseFees;
            }

            F.TaxDollar = ((F.LandingFees + F.ParkingFees + F.NightOperationFees + F.NavigationFees + F.SecurityFees +
                F.FirstClassFees + F.FreightFees + F.ServiceCars + F.InterCUPPSFees + F.DomeCUPPSFees +
                F.PushBackFees + F.ExpMailFees + F.ImpMailFees // + F.OpenAireportOvertimeFees
                + F.NoiseFees) * 0.05);





            F.GrandTotalDollar = (F.SubTotalDollar + F.TaxDollar);
            F.GrandTotalSDG = (F.GrandTotalDollar) * Convert.ToDouble(ExchangeRate);




            ///-------------------------------------SDG------------------------------------------------------------
            double SDGRound = Convert.ToDouble(Math.Round(F.GrandTotalSDG, 2));

                ToWord toWord = new ToWord(Convert.ToDecimal(SDGRound), currencies[Convert.ToInt32(0)]);

                F.GrandTotalSDGTxt = toWord.ConvertToArabic().ToString();

            ///--------------------------------------Dollar-----------------------------------------------------------

            double DollarRound = Convert.ToDouble(Math.Round(F.GrandTotalDollar, 2));
            ToWord toWord1 = new ToWord(Convert.ToDecimal(DollarRound), currencies[Convert.ToInt32(1)]);


            F.GrandTotalDollarTxt = toWord1.ConvertToArabic().ToString();


            ////*************************************************************************************
            ViewBag.HeaderDate = Convert.ToDateTime(DateFrom).ToString("MMM-yyyy");

            if (CurrencyType == 1)
            {
                ViewBag.Header = "Graf-" + Name + "-" + DateTime.Now.ToShortDateString();
                return View(F);
            }
            else
            {
                ViewBag.Header = "Graf-" + Name + "-" + DateTime.Now.ToShortDateString();
                return View("PrintDataGrafDollar", F);
            }
        }

        [Authorize(Roles = "Revenues,Super-Admin,Car,Ny&&Car, Account, Account-Admin")]
        public ActionResult PrintDataServiceCar(int? AirLineId, DateTime? DateFrom, DateTime? DateTo, int? FlightTypeId)
        {
            var id = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(id);
            RoleController role = new RoleController();
            int GetCompanyId = role.GetCompanyId(id);
            var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);
            int AirLineAccId = 0;
            string UserName = CommonUsed.GetUserName(id);

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
            else
            {
                //Month = Convert.ToDateTime(currentDate).Month;
                //Year = Convert.ToDateTime(currentDate).Year;
                return RedirectToAction("Error", "Revenues");
            }
            
            //ده كود لاستجلاب سعر الصرف و بقى ما شغال بعد شغل الحسابات
            //ExchangeRate = CommonUsed.getExchangeRateByLandingDate(Month, Year);
            //ViewBag.ExchangeRate = ExchangeRate;

            string Name = "غير محدد";
            string SearchAirLineId = "غير محدد";
            string SearchDateFrom = "غير محدد";
            string SearchDateTo = "غير محدد";
           // ReportDocument rd = new ReportDocument();

           // rd.Load(Path.Combine(Server.MapPath("~/Reports/Revenue/RptServiceCarFees.rpt")));

            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date).Where(x => (x.AmbolanceFees > 0 || x.FireFightingFees > 0)
            && x.OfficerData.TowerData.CompanyInfoId == companyId);

            if (AirLineId > 0)
            {
                Revenue = Revenue.Where(x => x.OfficerData.TowerData.AirLineId == AirLineId);
                SearchAirLineId = db.AirLines.Where(x => x.Id == AirLineId).Single().Name;
                AirLineAccId = (int)db.AirLines.Where(x => x.Id == AirLineId).Single().AccId;
               

                ViewBag.Name = SearchAirLineId;
                Name = SearchAirLineId;
            }


            ////هنا مفترض نستجلب سعر الصرف من جدول المطالبة وليس من جدول تفاصيل العملات
            //if (db.FinancialClaims.Any(x => (DbFunctions.TruncateTime(x.DateFrom) >= DateFrom & DbFunctions.TruncateTime(x.DateTo) >= DateFrom) |
            //     (DbFunctions.TruncateTime(x.DateFrom) <= DateTo & DbFunctions.TruncateTime(x.DateTo) >= DateTo) && x.CompanyInfoId == companyId))
            //{
            //    var obj = db.FinancialClaims.Where(x => (DbFunctions.TruncateTime(x.DateFrom) >= DateFrom & DbFunctions.TruncateTime(x.DateTo) >= DateFrom) |
            //     (DbFunctions.TruncateTime(x.DateFrom) <= DateTo & DbFunctions.TruncateTime(x.DateTo) >= DateTo) && x.CompanyInfoId == companyId).LastOrDefault();

            //    ExchangeRate = Convert.ToDecimal(obj.ExchangeRateAgainstPrimaryCurrency);

            //    ViewBag.ExchangeRate = ExchangeRate;
            //}
            //else
            //{
            //    return RedirectToAction("Error", "Revenues");
            //}





            if (db.FinancialClaim1.Any(x => x.AccIdDebit == AirLineAccId && (DbFunctions.TruncateTime(x.DateFrom) <= DateFrom & DbFunctions.TruncateTime(x.DateTo) >= DateFrom) |
                (DbFunctions.TruncateTime(x.DateFrom) <= DateTo & DbFunctions.TruncateTime(x.DateTo) >= DateTo) && x.CompanyInfoId == companyId))
            {
                var obj = db.FinancialClaim1.Where(x => x.AccIdDebit == AirLineAccId && (DbFunctions.TruncateTime(x.DateFrom) <= DateFrom & DbFunctions.TruncateTime(x.DateTo) >= DateFrom) |
                 (DbFunctions.TruncateTime(x.DateFrom) <= DateTo & DbFunctions.TruncateTime(x.DateTo) >= DateTo) && x.CompanyInfoId == companyId).FirstOrDefault();
                ExchangeRate = Convert.ToDecimal(obj.ExchangeRateAgainstPrimaryCurrency);

                ViewBag.ExchangeRate = ExchangeRate;
            }
            else
            {
                return RedirectToAction("Error", "Revenues");
            }







            if (DateFrom != null && DateTo != null)
            {
                Revenue = Revenue.Where(t => DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) >= DbFunctions.TruncateTime(DateFrom) && DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) <= DbFunctions.TruncateTime(DateTo));
                SearchDateFrom = CommonUsed.ToDisplayDate(DateFrom).ToString();
                SearchDateTo = CommonUsed.ToDisplayDate(DateTo).ToString();

                ViewBag.DateFrom = SearchDateFrom;
                ViewBag.DateTo = SearchDateTo;

            }


            if (Revenue.Count() <= 0)
            {
                return RedirectToAction("Error");
            }
            ///***********************************************************************************
            var RevenueList = new List<PrintCarServiceViewModel>();
            int N = 0;
            foreach (var item in Revenue)
            {
                PrintCarServiceViewModel C = new PrintCarServiceViewModel();
                C.Id = item.Id;
              
                N = N + 1;
                C.N = N;
                C.Name = item?.OfficerData.TowerData.AirLine.Code + "/ " + item?.OfficerData.TowerData.AirLine.Name;
                C.FlightNo = item?.OfficerData.TowerData.AirLinesFlightTrip.FlightNo;
                C.LandingDate = CommonUsed.ToDisplayDate(item?.OfficerData.TowerData.LandingDate);
                C.AmbulanceNo = Convert.ToInt32(item?.OfficerData.AmbulanceCarCount);
                C.FeesAmbulanceAmount = Convert.ToDecimal(item?.AmbolanceFees);

                if (C.FeesAmbulanceAmount == 0)
                {
                    C.FeesAmbulanceAmount = 0;
                }
                else
                {
                    C.FeesAmbulance = Convert.ToDecimal(C.FeesAmbulanceAmount / C.AmbulanceNo);
                }
                C.FireNo = Convert.ToInt32(item?.OfficerData.FireFightingCarCount);
                C.FeesFireAmount = Convert.ToDecimal(item?.FireFightingFees);
                if (C.FeesFireAmount == 0)
                {
                    C.FeesFire = 0;
                }
                else
                {
                    C.FeesFire = Convert.ToDecimal(C.FeesFireAmount / C.FireNo);
                }
                C.AmbulanceSDG = C.FeesAmbulanceAmount * ExchangeRate;
                C.FireSDG = C.FeesFireAmount * ExchangeRate;

                RevenueList.Add(C);
            }


            double SumFeesAmbulanceAmount = Convert.ToDouble(RevenueList.Sum(x => x.FeesAmbulanceAmount));
            ViewBag.SumFeesAmbulanceAmount = SumFeesAmbulanceAmount;

            double SumAmbulanceSDG = Convert.ToDouble(RevenueList.Sum(x => x.AmbulanceSDG));
            ViewBag.SumAmbulanceSDG = SumAmbulanceSDG;

            double SumFeesFireAmount = Convert.ToDouble(RevenueList.Sum(x => x.FeesFireAmount));
            ViewBag.SumFeesFireAmount = SumFeesFireAmount;

            double SumFireSDG = Convert.ToDouble(RevenueList.Sum(x => x.FireSDG));
            ViewBag.SumFireSDG = SumFireSDG;


           


            double TaxADollar = (SumFeesAmbulanceAmount) * .05;
            double TaxASDG = (SumAmbulanceSDG) * .05;

            double TaxFDollar = (SumFeesFireAmount) * .05;
            double TaxFSDG = (SumFireSDG) * .05;

            ViewBag.TaxADollar = TaxADollar;
            ViewBag.TaxASDG = TaxASDG;
            ViewBag.TaxFDollar = TaxFDollar;
            ViewBag.TaxFSDG = TaxFSDG;




            double GrandATotalDollar= SumFeesAmbulanceAmount + TaxADollar;
            double GrandATotalSDG = GrandATotalDollar * Convert.ToDouble(ExchangeRate);

            double GrandFTotalDollar = SumFeesFireAmount + TaxFDollar;
            double GrandFTotalSDG = GrandFTotalDollar * Convert.ToDouble(ExchangeRate);


            ViewBag.GrandATotalDollar = GrandATotalDollar;
            ViewBag.GrandATotalSDG = GrandATotalSDG;

            ViewBag.GrandFTotalDollar = GrandFTotalDollar;
            ViewBag.GrandFTotalSDG = GrandFTotalSDG;
            /************************************************************************************/
            ViewBag.Header = "ServiceCar-" + Name + "-" + DateTime.Now.ToShortDateString();
            return View(RevenueList);
        }
        
        [Authorize(Roles = "Revenues,Super-Admin,PushPack, Account, Account-Admin")]
        public ActionResult PrintPushPack(int? AirLineId, DateTime? DateFrom, DateTime? DateTo)
        {
            var id = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(id);
            RoleController role = new RoleController();
            int GetCompanyId = role.GetCompanyId(id);
            var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);
            int AirLineAccId = 0;
            string UserName = CommonUsed.GetUserName(id);

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
            else
            {
                //Month = Convert.ToDateTime(currentDate).Month;
                //Year = Convert.ToDateTime(currentDate).Year;
                return RedirectToAction("Error");
            }

            //ده كود لاستجلاب سعر الصرف و بقى ما شغال بعد شغل الحسابات
            //ExchangeRate = CommonUsed.getExchangeRateByLandingDate(Month, Year);
            //ViewBag.ExchangeRate = ExchangeRate;

            string Name = "غير محدد";

            string SearchDateFrom = "غير محدد";
            string SearchDateTo = "غير محدد";

           // ReportDocument rd = new ReportDocument();

           // rd.Load(Path.Combine(Server.MapPath("~/Reports/Revenue/RptPushPackFees.rpt")));

            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date).Where(x => x.PushBackFees != null && x.PushBackFees > 0
            && x.OfficerData.TowerData.CompanyInfoId == companyId);

            if (AirLineId > 0)
            {
                Revenue = Revenue.Where(x => x.OfficerData.TowerData.AirLineId == AirLineId);
                Name = db.AirLines.Where(x => x.Id == AirLineId).Single().Name;
                AirLineAccId = (int)db.AirLines.Where(x => x.Id == AirLineId).Single().AccId;

                ViewBag.Name = Name;
            }


            //هنا مفترض نستجلب سعر الصرف من جدول المطالبة وليس من جدول تفاصيل العملات
            if (db.FinancialClaim1.Any(x => x.AccIdDebit == AirLineAccId && (DbFunctions.TruncateTime(x.DateFrom) <= DateFrom & DbFunctions.TruncateTime(x.DateTo) >= DateFrom) |
                 (DbFunctions.TruncateTime(x.DateFrom) <= DateTo & DbFunctions.TruncateTime(x.DateTo) >= DateTo)))
            {
                var obj = db.FinancialClaim1.Where(x => (DbFunctions.TruncateTime(x.DateFrom) <= DateFrom & DbFunctions.TruncateTime(x.DateTo) >= DateFrom) |
                 (DbFunctions.TruncateTime(x.DateFrom) <= DateTo & DbFunctions.TruncateTime(x.DateTo) >= DateTo)).FirstOrDefault();
                ExchangeRate = Convert.ToDecimal(obj.ExchangeRateAgainstPrimaryCurrency);

                ViewBag.ExchangeRate = ExchangeRate;
            }
            else
            {
                return RedirectToAction("Error", "Revenues");
            }


            if (DateFrom != null && DateTo != null)
            {
                Revenue = Revenue.Where(t => DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) >= DbFunctions.TruncateTime(DateFrom) && DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) <= DbFunctions.TruncateTime(DateTo));
                SearchDateFrom = CommonUsed.ToDisplayDate(DateFrom).ToString();
                SearchDateTo = CommonUsed.ToDisplayDate(DateTo).ToString();

                ViewBag.DateFrom = SearchDateFrom;
                ViewBag.DateTo = SearchDateTo;
            }

            if (Revenue.Count() <= 0)
            {
                return RedirectToAction("Error");
            }
            ///***********************************************************************************
            var RevenueList = new List<PrintPushPackViewModel>();

            int N = 0;
            foreach (var item in Revenue)
            {
                PrintPushPackViewModel C = new PrintPushPackViewModel();

                N = N + 1;
                C.N = N;
                C.Id = item.Id;
                C.Name = item?.OfficerData.TowerData.AirLine.Name;
                C.LandingDate = CommonUsed.ToDisplayDate(item?.OfficerData.TowerData.LandingDate);
                C.FlightNo = item?.OfficerData.TowerData.AirLinesFlightTrip.FlightNo;
                C.AirPortTo = item?.OfficerData.TowerData.AirPort.Code;

                C.RegNo = item?.OfficerData.TowerData.AircraftRegistration.Registration;
                C.Type = item?.OfficerData.TowerData.AircraftRegistration.AircraftType.Type;
                C.PlaneSize = item?.OfficerData.TowerData.AircraftRegistration.AircraftType.AircraftSize.Size;
                C.Wt = item?.OfficerData.TowerData.AircraftRegistration.MaxTakoffWieght.ToString();
                C.PushPackFees = Convert.ToDecimal(item?.PushBackFees);

                C.AmountSDG = ExchangeRate * C.PushPackFees;
                RevenueList.Add(C);
            }

            ////*************************************************************************************


            double SumPushPackFees = Convert.ToDouble(RevenueList.Sum(x => x.PushPackFees));
            ViewBag.SumPushPackFees = SumPushPackFees;

            double SumAmountSDG = Convert.ToDouble(RevenueList.Sum(x => x.AmountSDG));
            ViewBag.SumAmountSDG = SumAmountSDG;

            double TaxDollar = SumPushPackFees * 0.05;
            ViewBag.TaxDollar = TaxDollar;

            double TaxSDG = SumAmountSDG * 0.05;
            ViewBag.TaxSDG = TaxSDG;

            // Grand Total
            double GrandTotalDollar = SumPushPackFees + TaxDollar;
            ViewBag.GrandTotalDollar = GrandTotalDollar;

            double GrandTotalSDG = SumAmountSDG + TaxSDG;
            ViewBag.GrandTotalSDG = GrandTotalSDG;






            ViewBag.Header ="PushPack-"+ Name +"-"+ DateTime.Now.ToShortDateString();
            return View(RevenueList);
        }
        
        [Authorize(Roles = "Revenues,Super-Admin,FirstClass, Account, Account-Admin")]
        public ActionResult PrintDataFirstClass(int? AirLineId, DateTime? DateFrom, DateTime? DateTo/*, int? FlightTypeId*/)
        {
            var id = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(id);
            RoleController role = new RoleController();
            int GetCompanyId = role.GetCompanyId(id);
            var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);
            int AirLineAccId = 0;
            string UserName = CommonUsed.GetUserName(id);

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
            else
            {
                //Month = Convert.ToDateTime(currentDate).Month;
                //Year = Convert.ToDateTime(currentDate).Year;
                return RedirectToAction("Error");
            }

            //ده كود لاستجلاب سعر الصرف و بقى ما شغال بعد شغل الحسابات
            //ExchangeRate = CommonUsed.getExchangeRateByLandingDate(Month, Year);
            //ViewBag.ExchangeRate = ExchangeRate;


            string Name = "غير محدد";
            string SearchDateFrom = "غير محدد";
            string SearchDateTo = "غير محدد";
           // ReportDocument rd = new ReportDocument();

           // rd.Load(Path.Combine(Server.MapPath("~/Reports/Revenue/RptFirstClassFees.rpt")));

            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date).Where(x => x.FirstClassFees > 0
            && x.OfficerData.TowerData.CompanyInfoId == companyId);
            //if (FlightTypeId > 0)
            //{
            //    Revenue = Revenue.Where(x => x.OfficerData.TowerData.FlightType == FlightTypeId);
            //    SearchFlightTypeId = CommonUsed.GetFlightTypeName(FlightTypeId);
            //}

            if (AirLineId > 0)
            {
                Revenue = Revenue.Where(x => x.OfficerData.TowerData.AirLineId == AirLineId);
                Name = db.AirLines.Where(x => x.Id == AirLineId).Single().Name;
                AirLineAccId = (int)db.AirLines.Where(x => x.Id == AirLineId).Single().AccId;

                ViewBag.Name = Name;
            }



            //هنا مفترض نستجلب سعر الصرف من جدول المطالبة وليس من جدول تفاصيل العملات
            if (db.FinancialClaim1.Any(x => x.AccIdDebit == AirLineAccId && (DbFunctions.TruncateTime(x.DateFrom) <= DateFrom & DbFunctions.TruncateTime(x.DateTo) >= DateFrom) |
                 (DbFunctions.TruncateTime(x.DateFrom) <= DateTo & DbFunctions.TruncateTime(x.DateTo) >= DateTo)))
            {
                var obj = db.FinancialClaim1.Where(x => (DbFunctions.TruncateTime(x.DateFrom) <= DateFrom & DbFunctions.TruncateTime(x.DateTo) >= DateFrom) |
                 (DbFunctions.TruncateTime(x.DateFrom) <= DateTo & DbFunctions.TruncateTime(x.DateTo) >= DateTo)).FirstOrDefault();
                ExchangeRate = Convert.ToDecimal(obj.ExchangeRateAgainstPrimaryCurrency);

                ViewBag.ExchangeRate = ExchangeRate;
            }
            else
            {
                return RedirectToAction("Error", "Revenues");
            }


            if (DateFrom != null && DateTo != null)
            {
                Revenue = Revenue.Where(t => DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) >= DbFunctions.TruncateTime(DateFrom) && DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) <= DbFunctions.TruncateTime(DateTo));
                SearchDateFrom = CommonUsed.ToDisplayDate(DateFrom).ToString();
                SearchDateTo = CommonUsed.ToDisplayDate(DateTo).ToString();

                ViewBag.DateFrom = SearchDateFrom;
                ViewBag.DateTo = SearchDateTo;


            }

            if (Revenue.Count() <= 0)
            {
                return RedirectToAction("Error");
            }
            ///***********************************************************************************
            var RevenueList = new List<PrintFirstClassViewModel>();

            int N = 0;
            foreach (var item in Revenue)
            {

                PrintFirstClassViewModel F = new PrintFirstClassViewModel();

                F.Id = item.Id;

                N = N + 1;
                F.N = N;

                F.Name = item?.OfficerData.TowerData.AirLine.Code + "/ " + item?.OfficerData.TowerData.AirLine.Name;
                F.FlightNo = item?.OfficerData.TowerData.AirLinesFlightTrip.FlightNo;
                //L.MaxTakoffWieght = item?.OfficerData.TowerData.AircraftRegistration.AircraftType.MaxTakoffWieght.ToString();
                F.TakeOffDate = CommonUsed.ToDisplayDate(item?.OfficerData.TowerData.TakeOffDate);

                // L.MaxTakoffWieght = item?.OfficerData.TowerData.AircraftRegistration.AircraftType.Type.ToString();

                F.FirstClassCount = Convert.ToInt32(item?.OfficerData.FirstClassCount);

                F.FirstClassFees = Convert.ToDecimal(item?.FirstClassFees);

                F.FirstClassFeesSDG = Convert.ToDecimal(item?.FirstClassFees) * Convert.ToDecimal(ExchangeRate);
                if (F.FirstClassFees == 0)
                {
                    F.Fees = 0;
                }
                else
                {
                    F.Fees = (F.FirstClassFees / F.FirstClassCount);

                }

                RevenueList.Add(F);
            }

            /*************************************************************************************/
            double SumFirstClassCount = Convert.ToDouble(RevenueList.Sum(x => x.FirstClassCount));
            ViewBag.SumFirstClassCount = SumFirstClassCount;






            double SumFirstClassFees = Convert.ToDouble(RevenueList.Sum(x => x.FirstClassFees));
            ViewBag.SumFirstClassFees = SumFirstClassFees;

            double SumFirstClassFeesSDG = Convert.ToDouble(RevenueList.Sum(x => x.FirstClassFeesSDG));
            ViewBag.SumFirstClassFeesSDG = SumFirstClassFeesSDG;

            double TaxDollar = SumFirstClassFees * 0.05;
            ViewBag.TaxDollar = TaxDollar;

            double TaxSDG = SumFirstClassFeesSDG * 0.05;
            ViewBag.TaxSDG = TaxSDG;

            // Grand Total
            double GrandTotalDollar = SumFirstClassFees + TaxDollar;
            ViewBag.GrandTotalDollar = GrandTotalDollar;

            double GrandTotalSDG = SumFirstClassFeesSDG + TaxSDG;
            ViewBag.GrandTotalSDG = GrandTotalSDG;









            ViewBag.Header = "FirstClass-" + Name + "-" + DateTime.Now.ToShortDateString();
            return View(RevenueList);
        }
        
        [Authorize(Roles = "Revenues,Super-Admin,Cupps,Ny,Ny&&Car, Account, Account-Admin")]
        public ActionResult PrintDataNy1(int? AirLineId, DateTime? DateFrom, DateTime? DateTo, int? FlightTypeId)
        {
            string TitleDiscount = "";
            int C = 0, AirLineAccId = 0;
            var id = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(id);
            RoleController role = new RoleController();
            int GetCompanyId = role.GetCompanyId(id);
            var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);
            string Discount = "";
          
            string UserName = CommonUsed.GetUserName(id);

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
            else
            {
                return RedirectToAction("Error");
            }

            //ده كود لاستجلاب سعر الصرف و بقى ما شغال بعد شغل الحسابات
            //ExchangeRate = CommonUsed.getExchangeRateByLandingDate(Month, Year);
            //ViewBag.ExchangeRate = ExchangeRate;


            string Name = "غير محدد";

            string SearchDateFrom = "غير محدد";
            string SearchDateTo = "غير محدد";
            string MessageDiscount = "";
            bool IsLocalCompany = false;
            ///////////////NYFeesDiscount//////////////////
            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.TakeOffDate).Where(x => x.OfficerData.Pax > 0
            && x.OfficerData.TowerData.CompanyInfoId == companyId);
            if (FlightTypeId > 0)
            {
                if (FlightTypeId == 1)
                {
                    Revenue = Revenue.Where(x => x.OfficerData.TowerData.FlightTypeD == 1 /*|| x.OfficerData.TowerData.FlightTypeL == 1*/);
                    Revenue = Revenue.Where(x => x.InterNYFees != null);
                }
                else
                {
                    Revenue = Revenue.Where(x => x.OfficerData.TowerData.FlightTypeD == 2/* && x.OfficerData.TowerData.FlightTypeL == 2*/);
                    Revenue = Revenue.Where(x => x.DomeNYFees != null);
                }
            }

            if (AirLineId > 0)
            {
                Revenue = Revenue.Where(x => x.OfficerData.TowerData.AirLineId == AirLineId);
                IsLocalCompany = Convert.ToBoolean(db.AirLines.Where(x => x.Id == AirLineId).Single().IsLocalCompany);
                Name = db.AirLines.Where(x => x.Id == AirLineId).Single().Name;
                AirLineAccId = (int)db.AirLines.Where(x => x.Id == AirLineId).Single().AccId;


                ViewBag.Name = Name;


                if (IsLocalCompany)
                {
                    GeneralService gs = db.GeneralServices.FirstOrDefault(x => x.Id == 6);
                    int Type = Convert.ToInt32(gs.Type);
                    decimal Value = 0;

                    if (gs.Value > 0)
                    {
                        if (Type == 2)//means It's a value Not Percent
                        {
                            Value = Convert.ToDecimal(gs.Value);
                            MessageDiscount = "*NY Fees Discount  " + Convert.ToDouble(Value) + "$";
                            Discount = "(- " + Convert.ToDouble(Value) + "$)";
                            TitleDiscount = "Discount" + Convert.ToDouble(Value) + "$)";
                        }
                        else //means It's a Percent Not value
                        {
                            Value = Convert.ToDecimal(gs.Value);
                            MessageDiscount = "*NY Fees Discount " + Convert.ToDouble(Value) + " %";
                            TitleDiscount = "Discount" + Convert.ToDouble(Value) + "%";
                        }
                    }
                }
            }


            //هنا مفترض نستجلب سعر الصرف من جدول المطالبة وليس من جدول تفاصيل العملات
            if (db.FinancialClaim1.Any(x => x.AccIdDebit == AirLineAccId && (DbFunctions.TruncateTime(x.DateFrom) <= DateFrom & DbFunctions.TruncateTime(x.DateTo) >= DateFrom) |
                 (DbFunctions.TruncateTime(x.DateFrom) <= DateTo & DbFunctions.TruncateTime(x.DateTo) >= DateTo) && x.CompanyInfoId ==companyId))
            {
                var obj = db.FinancialClaim1.Where(x => x.AccIdDebit == AirLineAccId && (DbFunctions.TruncateTime(x.DateFrom) <= DateFrom & DbFunctions.TruncateTime(x.DateTo) >= DateFrom) |
                 (DbFunctions.TruncateTime(x.DateFrom) <= DateTo & DbFunctions.TruncateTime(x.DateTo) >= DateTo) && x.CompanyInfoId == companyId).FirstOrDefault();
                ExchangeRate = Convert.ToDecimal(obj.ExchangeRateAgainstPrimaryCurrency);

                ViewBag.ExchangeRate = ExchangeRate;
            }
            else
            {
                return RedirectToAction("Error", "Revenues");
            }


            ViewBag.TitleDiscount = TitleDiscount;
            if (DateFrom != null && DateTo != null)
            {
                Revenue = Revenue.Where(t => DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) >= DbFunctions.TruncateTime(DateFrom) && DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) <= DbFunctions.TruncateTime(DateTo));
                SearchDateFrom = CommonUsed.ToDisplayDate(DateFrom).ToString();
                SearchDateTo = CommonUsed.ToDisplayDate(DateTo).ToString();

                ViewBag.DateFrom = SearchDateFrom;
                ViewBag.DateTo = SearchDateTo;
            }

            if (Revenue.Count() <= 0)
            {
                return RedirectToAction("Error");
            }


            // ReportDocument rd = new ReportDocument();
            if (FlightTypeId == 1)
            {
                   var RevenueList = new List<PrintNyViewModel>();
                   foreach (var item in Revenue)
                    {
                        PrintNyViewModel N = new PrintNyViewModel();
                        C = C + 1;
                        N.N = C;
                        N.Pax = Convert.ToInt32(item?.OfficerData.Pax);
                        N.Id = item.Id;
                        N.Name = item?.OfficerData.TowerData.AirLine.Code + "/ " + item?.OfficerData.TowerData.AirLine.Name;
                        N.AirPortTo = item?.OfficerData.TowerData.AirPort.Code;
                        //N.TakeOffDate = CommonUsed.ToDisplayDate(item?.OfficerData.TowerData.TakeOffDate.ToString());
                        N.TakeOffDate = CommonUsed.ToDisplayDate(item?.OfficerData.TowerData.LandingDate);//تم تعديلها نسبة لان بعض الرحلات تكون بايتة وتظهر بتاريخ الشهر الجديد ودي مشكلة
                        N.FlightNo = item?.OfficerData.TowerData.AirLinesFlightTrip.FlightNo;
                        N.InterNYFees = Convert.ToDecimal(item?.InterNYFees);
                        if (N.InterNYFees == 0)
                        {
                            N.InterFees = 0;
                        }
                        else
                        {
                            N.InterFees = (N.InterNYFees / N.Pax);
                        }

                        N.AmountSDG = N.InterNYFees * ExchangeRate;
                        RevenueList.Add(N);
                    }
                //InterNational && Domestic

                ///**********************************************InterNational && Local Company****************************************
                double SumPax = Convert.ToDouble(RevenueList.Sum(x => x.Pax));
                ViewBag.SumPax = SumPax;

                double SumInterFees = Convert.ToDouble(RevenueList.Sum(x => x.InterFees));
                ViewBag.SumInterFees = SumInterFees;

                double SumInterNYFees = Convert.ToDouble(RevenueList.Sum(x => x.InterNYFees));
                ViewBag.SumInterNYFees = SumInterNYFees;

                double SumInterNYFeesSDG = SumInterNYFees * Convert.ToDouble(ExchangeRate);
                ViewBag.SumInterNYFeesSDG = SumInterNYFeesSDG;


                ViewBag.Header = "Ny-" + Name + "-" + DateTime.Now.ToShortDateString();
                return View("PrintDataNyInter", RevenueList);
            }
                
            else
            {//Domestic
              
                var RevenueList = new List<PrintNyViewModel>();

              
                    foreach (var item in Revenue)
                    {
                        PrintNyViewModel N = new PrintNyViewModel();

                        C = C + 1;
                        N.N = C;
                    if (item.OfficerData.PaxInter > 0)
                    {
                        N.Pax2 = Convert.ToInt32(item?.OfficerData.Pax).ToString() + "+" + Convert.ToInt32(item?.OfficerData.PaxInter).ToString();
                        N.PaxInter = Convert.ToInt32(item?.OfficerData.PaxInter);
                        ViewBag.PaxInter = N.PaxInter;
                    }
                    else
                    {
                        N.Pax2 = Convert.ToInt32(item?.OfficerData.Pax).ToString();
                    }
                    N.Pax = Convert.ToInt32(item?.OfficerData.Pax);
                        N.Id = item.Id;
                        N.Name = item?.OfficerData.TowerData.AirLine.Code + "/ " + item?.OfficerData.TowerData.AirLine.Name;
                        N.AirPortTo = item?.OfficerData.TowerData.AirPort.Code;
                        N.TakeOffDate = CommonUsed.ToDisplayDate(item?.OfficerData.TowerData.LandingDate);
                        N.FlightNo = item?.OfficerData.TowerData.AirLinesFlightTrip.FlightNo;
                    // N.DomeNYFeesSDG = Convert.ToDecimal(item?.DomeNYFees); // this is old one(KHA)
                    if (item.OfficerData.PaxInter > 0)
                    {
                        N.DomeNYFeesSDG = Convert.ToDecimal(item?.DomeNYFees) * Convert.ToDecimal(ExchangeRate) +
                            Convert.ToDecimal(item?.InterNYFees) * Convert.ToDecimal(ExchangeRate);
                    }
                    else { N.DomeNYFeesSDG = Convert.ToDecimal(item?.DomeNYFees) * Convert.ToDecimal(ExchangeRate); }
                       
                        

                        if (N.DomeNYFeesSDG == 0)
                        {
                            N.DomeFees = 0;
                        }
                        else
                        {
                        // N.DomeFees = (N.DomeNYFeesSDG / N.Pax);
                        N.DomeFees = Convert.ToDecimal(item?.DomeNYFees / N.Pax);
                    }

                        N.AmountDollar = N.DomeNYFeesSDG / ExchangeRate;
                        RevenueList.Add(N);
                    }
                //************************************ //Domestic && LocalCompany***************************************
                string SumPax;
                if (RevenueList.Any(x => x.PaxInter > 0))
                {
                    SumPax = Convert.ToDouble(RevenueList.Sum(x => x.Pax)).ToString() + "+" + Convert.ToDouble(RevenueList.Sum(x => x.PaxInter)).ToString();
                }
                else
                {
                     SumPax = Convert.ToDouble(RevenueList.Sum(x => x.Pax)).ToString();
                }
                ViewBag.SumPax = SumPax;

               

                double SumDomeNYFeesSDG = Convert.ToDouble(RevenueList.Sum(x => x.DomeNYFeesSDG));
                ViewBag.SumDomeNYFeesSDG = SumDomeNYFeesSDG;



                double SumAmountDollar = Convert.ToDouble(RevenueList.Sum(x => x.AmountDollar));
                ViewBag.SumAmountDollar = SumAmountDollar;









                ViewBag.Header = "Ny-" + Name + "-" + DateTime.Now.ToShortDateString();
                return View("PrintDataNyDom", RevenueList);





            }
        }

        public ActionResult PrintDataNy(int? AirLineId, DateTime? DateFrom, DateTime? DateTo, int? FlightTypeId)
        {
            string TitleDiscount = "";
            int C = 0, AirLineAccId = 0;
            var id = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(id);
            RoleController role = new RoleController();
            int GetCompanyId = role.GetCompanyId(id);
            var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);
            string Discount = "";

            string UserName = CommonUsed.GetUserName(id);

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
            else
            {
                return RedirectToAction("Error");
            }

            //ده كود لاستجلاب سعر الصرف و بقى ما شغال بعد شغل الحسابات
            //ExchangeRate = CommonUsed.getExchangeRateByLandingDate(Month, Year);
            //ViewBag.ExchangeRate = ExchangeRate;


            string Name = "غير محدد";

            string SearchDateFrom = "غير محدد";
            string SearchDateTo = "غير محدد";
            string MessageDiscount = "";
            bool IsLocalCompany = false;
            ///////////////NYFeesDiscount//////////////////
            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.TakeOffDate).Where(x => x.OfficerData.Pax > 0
            && x.OfficerData.TowerData.CompanyInfoId == companyId);
            if (FlightTypeId > 0)
            {
                if (FlightTypeId == 1)
                {
                    Revenue = Revenue.Where(x => x.OfficerData.TowerData.FlightTypeD == 1 /*|| x.OfficerData.TowerData.FlightTypeL == 1*/);
                    Revenue = Revenue.Where(x => x.InterNYFees != null);
                }
                else
                {
                    Revenue = Revenue.Where(x => x.OfficerData.TowerData.FlightTypeD == 2/* && x.OfficerData.TowerData.FlightTypeL == 2*/);
                    Revenue = Revenue.Where(x => x.DomeNYFees != null);
                }
            }

            if (AirLineId > 0)
            {
                Revenue = Revenue.Where(x => x.OfficerData.TowerData.AirLineId == AirLineId);
                IsLocalCompany = Convert.ToBoolean(db.AirLines.Where(x => x.Id == AirLineId).Single().IsLocalCompany);
                Name = db.AirLines.Where(x => x.Id == AirLineId).Single().Name;
                AirLineAccId = (int)db.AirLines.Where(x => x.Id == AirLineId).Single().AccId;


                ViewBag.Name = Name;


                if (IsLocalCompany)
                {
                    GeneralService gs = db.GeneralServices.FirstOrDefault(x => x.Id == 6);
                    int Type = Convert.ToInt32(gs.Type);
                    decimal Value = 0;

                    if (gs.Value > 0)
                    {
                        if (Type == 2)//means It's a value Not Percent
                        {
                            Value = Convert.ToDecimal(gs.Value);
                            MessageDiscount = "*NY Fees Discount  " + Convert.ToDouble(Value) + "$";
                            Discount = "(- " + Convert.ToDouble(Value) + "$)";
                            TitleDiscount = "Discount" + Convert.ToDouble(Value) + "$)";
                        }
                        else //means It's a Percent Not value
                        {
                            Value = Convert.ToDecimal(gs.Value);
                            MessageDiscount = "*NY Fees Discount " + Convert.ToDouble(Value) + " %";
                            TitleDiscount = "Discount" + Convert.ToDouble(Value) + "%";
                        }
                    }
                }
            }


            //هنا مفترض نستجلب سعر الصرف من جدول المطالبة وليس من جدول تفاصيل العملات
            if (db.FinancialClaim1.Any(x => x.AccIdDebit == AirLineAccId && (DbFunctions.TruncateTime(x.DateFrom) <= DateFrom & DbFunctions.TruncateTime(x.DateTo) >= DateFrom) |
                 (DbFunctions.TruncateTime(x.DateFrom) <= DateTo & DbFunctions.TruncateTime(x.DateTo) >= DateTo) && x.CompanyInfoId == companyId))
            {
                var obj = db.FinancialClaim1.Where(x => x.AccIdDebit == AirLineAccId && (DbFunctions.TruncateTime(x.DateFrom) <= DateFrom & DbFunctions.TruncateTime(x.DateTo) >= DateFrom) |
                 (DbFunctions.TruncateTime(x.DateFrom) <= DateTo & DbFunctions.TruncateTime(x.DateTo) >= DateTo) && x.CompanyInfoId == companyId).FirstOrDefault();
                ExchangeRate = Convert.ToDecimal(obj.ExchangeRateAgainstPrimaryCurrency);

                ViewBag.ExchangeRate = ExchangeRate;
            }
            else
            {
                return RedirectToAction("Error", "Revenues");
            }


            ViewBag.TitleDiscount = TitleDiscount;
            if (DateFrom != null && DateTo != null)
            {
                Revenue = Revenue.Where(t => DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) >= DbFunctions.TruncateTime(DateFrom) && DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) <= DbFunctions.TruncateTime(DateTo));
                SearchDateFrom = CommonUsed.ToDisplayDate(DateFrom).ToString();
                SearchDateTo = CommonUsed.ToDisplayDate(DateTo).ToString();

                ViewBag.DateFrom = SearchDateFrom;
                ViewBag.DateTo = SearchDateTo;
            }

            if (Revenue.Count() <= 0)
            {
                return RedirectToAction("Error");
            }


            // ReportDocument rd = new ReportDocument();
            if (FlightTypeId == 1)
            {
                var RevenueList = new List<PrintNyViewModel>();
                foreach (var item in Revenue)
                {
                    PrintNyViewModel N = new PrintNyViewModel();
                    C = C + 1;
                    N.N = C;
                    N.Pax = Convert.ToInt32(item?.OfficerData.Pax);
                    N.Id = item.Id;
                    N.Name = item?.OfficerData.TowerData.AirLine.Code + "/ " + item?.OfficerData.TowerData.AirLine.Name;
                    N.AirPortTo = item?.OfficerData.TowerData.AirPort.Code;
                    //N.TakeOffDate = CommonUsed.ToDisplayDate(item?.OfficerData.TowerData.TakeOffDate.ToString());
                    N.TakeOffDate = CommonUsed.ToDisplayDate(item?.OfficerData.TowerData.LandingDate);//تم تعديلها نسبة لان بعض الرحلات تكون بايتة وتظهر بتاريخ الشهر الجديد ودي مشكلة
                    N.FlightNo = item?.OfficerData.TowerData.AirLinesFlightTrip.FlightNo;
                    N.InterNYFees = Convert.ToDecimal(item?.InterNYFees);
                    if (N.InterNYFees == 0)
                    {
                        N.InterFees = 0;
                    }
                    else
                    {
                        N.InterFees = (N.InterNYFees / N.Pax);
                    }

                    N.AmountSDG = N.InterNYFees * ExchangeRate;
                    RevenueList.Add(N);
                }
                //InterNational && Domestic

                ///**********************************************InterNational && Local Company****************************************
                double SumPax = Convert.ToDouble(RevenueList.Sum(x => x.Pax));
                ViewBag.SumPax = SumPax;

                double SumInterFees = Convert.ToDouble(RevenueList.Sum(x => x.InterFees));
                ViewBag.SumInterFees = SumInterFees;

                double SumInterNYFees = Convert.ToDouble(RevenueList.Sum(x => x.InterNYFees));
                ViewBag.SumInterNYFees = SumInterNYFees;

                double SumInterNYFeesSDG = SumInterNYFees * Convert.ToDouble(ExchangeRate);
                ViewBag.SumInterNYFeesSDG = SumInterNYFeesSDG;


                ViewBag.Header = "Ny-" + Name + "-" + DateTime.Now.ToShortDateString();
                return View("PrintDataNyInter", RevenueList);
            }

            else
            {//Domestic

                var RevenueList = new List<PrintNyViewModel>();


                foreach (var item in Revenue)
                {
                    PrintNyViewModel N = new PrintNyViewModel();

                    C = C + 1;
                    N.N = C;
                    N.Pax = Convert.ToInt32(item?.OfficerData.Pax);
                    N.Id = item.Id;
                    N.Name = item?.OfficerData.TowerData.AirLine.Code + "/ " + item?.OfficerData.TowerData.AirLine.Name;
                    N.AirPortTo = item?.OfficerData.TowerData.AirPort.Code;
                    N.TakeOffDate = CommonUsed.ToDisplayDate(item?.OfficerData.TowerData.LandingDate);
                    N.FlightNo = item?.OfficerData.TowerData.AirLinesFlightTrip.FlightNo;
                    // N.DomeNYFeesSDG = Convert.ToDecimal(item?.DomeNYFees); // this is old one(KHA)
                    N.DomeNYFeesSDG = Convert.ToDecimal(item?.DomeNYFees) * Convert.ToDecimal(ExchangeRate); // this is a new one(PZU)

                    if (N.DomeNYFeesSDG == 0)
                    {
                        N.DomeFees = 0;
                    }
                    else
                    {
                        // N.DomeFees = (N.DomeNYFeesSDG / N.Pax);
                        N.DomeFees = Convert.ToDecimal(item?.DomeNYFees / N.Pax);
                    }

                    N.AmountDollar = N.DomeNYFeesSDG / ExchangeRate;
                    RevenueList.Add(N);
                }
                //************************************ //Domestic && LocalCompany***************************************

                double SumPax = Convert.ToDouble(RevenueList.Sum(x => x.Pax));
                ViewBag.SumPax = SumPax;



                double SumDomeNYFeesSDG = Convert.ToDouble(RevenueList.Sum(x => x.DomeNYFeesSDG));
                ViewBag.SumDomeNYFeesSDG = SumDomeNYFeesSDG;



                double SumAmountDollar = Convert.ToDouble(RevenueList.Sum(x => x.AmountDollar));
                ViewBag.SumAmountDollar = SumAmountDollar;


                ViewBag.Header = "Ny-" + Name + "-" + DateTime.Now.ToShortDateString();
                return View("PrintDataNyDom", RevenueList);





            }
        }

        public ActionResult PrintDataNyForDom(int? AirLineId, DateTime? DateFrom, DateTime? DateTo, int? FlightTypeId)
        {
            string TitleDiscount = "";
            int C = 0, AirLineAccId = 0;
            var id = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(id);
            RoleController role = new RoleController();
            int GetCompanyId = role.GetCompanyId(id);
            var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);
            string Discount = "";

            string UserName = CommonUsed.GetUserName(id);

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
            else
            {
                return RedirectToAction("Error");
            }

         
            string Name = "غير محدد";

            string SearchDateFrom = "غير محدد";
            string SearchDateTo = "غير محدد";
            string MessageDiscount = "";
            bool IsLocalCompany = false;
            ///////////////NYFeesDiscount//////////////////
            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.TakeOffDate).Where(x => x.OfficerData.PaxInter > 0
            && x.OfficerData.TowerData.CompanyInfoId == companyId);
           
            Revenue = Revenue.Where(x => x.OfficerData.TowerData.FlightTypeD == 2/* && x.OfficerData.TowerData.FlightTypeL == 2*/);
            Revenue = Revenue.Where(x => x.InterNYFees != null);
                
            

            if (AirLineId > 0)
            {
                Revenue = Revenue.Where(x => x.OfficerData.TowerData.AirLineId == AirLineId);
                IsLocalCompany = Convert.ToBoolean(db.AirLines.Where(x => x.Id == AirLineId).Single().IsLocalCompany);
                Name = db.AirLines.Where(x => x.Id == AirLineId).Single().Name;
                AirLineAccId = (int)db.AirLines.Where(x => x.Id == AirLineId).Single().AccId;


                ViewBag.Name = Name;


                if (IsLocalCompany)
                {
                    GeneralService gs = db.GeneralServices.FirstOrDefault(x => x.Id == 6);
                    int Type = Convert.ToInt32(gs.Type);
                    decimal Value = 0;

                    if (gs.Value > 0)
                    {
                        if (Type == 2)//means It's a value Not Percent
                        {
                            Value = Convert.ToDecimal(gs.Value);
                            MessageDiscount = "*NY Fees Discount  " + Convert.ToDouble(Value) + "$";
                            Discount = "(- " + Convert.ToDouble(Value) + "$)";
                            TitleDiscount = "Discount" + Convert.ToDouble(Value) + "$)";
                        }
                        else //means It's a Percent Not value
                        {
                            Value = Convert.ToDecimal(gs.Value);
                            MessageDiscount = "*NY Fees Discount " + Convert.ToDouble(Value) + " %";
                            TitleDiscount = "Discount" + Convert.ToDouble(Value) + "%";
                        }
                    }
                }
            }


            //هنا مفترض نستجلب سعر الصرف من جدول المطالبة وليس من جدول تفاصيل العملات
            if (db.FinancialClaim1.Any(x => x.AccIdDebit == AirLineAccId && (DbFunctions.TruncateTime(x.DateFrom) <= DateFrom & DbFunctions.TruncateTime(x.DateTo) >= DateFrom) |
                 (DbFunctions.TruncateTime(x.DateFrom) <= DateTo & DbFunctions.TruncateTime(x.DateTo) >= DateTo) && x.CompanyInfoId == companyId))
            {
                var obj = db.FinancialClaim1.Where(x => x.AccIdDebit == AirLineAccId && (DbFunctions.TruncateTime(x.DateFrom) <= DateFrom & DbFunctions.TruncateTime(x.DateTo) >= DateFrom) |
                 (DbFunctions.TruncateTime(x.DateFrom) <= DateTo & DbFunctions.TruncateTime(x.DateTo) >= DateTo) && x.CompanyInfoId == companyId).FirstOrDefault();
                ExchangeRate = Convert.ToDecimal(obj.ExchangeRateAgainstPrimaryCurrency);

                ViewBag.ExchangeRate = ExchangeRate;
            }
            else
            {
                return RedirectToAction("Error", "Revenues");
            }


            ViewBag.TitleDiscount = TitleDiscount;
            if (DateFrom != null && DateTo != null)
            {
                Revenue = Revenue.Where(t => DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) >= DbFunctions.TruncateTime(DateFrom) && DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) <= DbFunctions.TruncateTime(DateTo));
                SearchDateFrom = CommonUsed.ToDisplayDate(DateFrom).ToString();
                SearchDateTo = CommonUsed.ToDisplayDate(DateTo).ToString();

                ViewBag.DateFrom = SearchDateFrom;
                ViewBag.DateTo = SearchDateTo;
            }

            if (Revenue.Count() <= 0)
            {
                return RedirectToAction("Error");
            }


            // ReportDocument rd = new ReportDocument();
            if (FlightTypeId == 2)
            {
                var RevenueList = new List<PrintNyViewModel>();
                foreach (var item in Revenue)
                {
                    PrintNyViewModel N = new PrintNyViewModel();
                    C = C + 1;
                    N.N = C;
                    N.Pax = Convert.ToInt32(item?.OfficerData.PaxInter);
                    N.Id = item.Id;
                    N.Name = item?.OfficerData.TowerData.AirLine.Code + "/ " + item?.OfficerData.TowerData.AirLine.Name;
                    N.AirPortTo = item?.OfficerData.TowerData.AirPort.Code;
                    //N.TakeOffDate = CommonUsed.ToDisplayDate(item?.OfficerData.TowerData.TakeOffDate.ToString());
                    N.TakeOffDate = CommonUsed.ToDisplayDate(item?.OfficerData.TowerData.LandingDate);//تم تعديلها نسبة لان بعض الرحلات تكون بايتة وتظهر بتاريخ الشهر الجديد ودي مشكلة
                    N.FlightNo = item?.OfficerData.TowerData.AirLinesFlightTrip.FlightNo;
                    N.InterNYFees = Convert.ToDecimal(item?.InterNYFees);
                    if (N.InterNYFees == 0)
                    {
                        N.InterFees = 0;
                    }
                    else
                    {
                        N.InterFees = (N.InterNYFees / N.Pax);
                    }

                    N.AmountSDG = N.InterNYFees * ExchangeRate;
                    RevenueList.Add(N);
                }
                //InterNational && Domestic

                ///**********************************************InterNational && Local Company****************************************
                double SumPax = Convert.ToDouble(RevenueList.Sum(x => x.Pax));
                ViewBag.SumPax = SumPax;

                double SumInterFees = Convert.ToDouble(RevenueList.Sum(x => x.InterFees));
                ViewBag.SumInterFees = SumInterFees;

                double SumInterNYFees = Convert.ToDouble(RevenueList.Sum(x => x.InterNYFees));
                ViewBag.SumInterNYFees = SumInterNYFees;

                double SumInterNYFeesSDG = SumInterNYFees * Convert.ToDouble(ExchangeRate);
                ViewBag.SumInterNYFeesSDG = SumInterNYFeesSDG;


                ViewBag.Header = "Ny-" + Name + "-" + DateTime.Now.ToShortDateString();
                return View("PrintDataNyForDom", RevenueList);
            }

            else { return View("Error"); }
        }

        [Authorize(Roles = "Revenues,Super-Admin,Cupps,Ny,Ny&&Car, Account, Account-Admin")]
        public ActionResult PrintDataCupps1(int? AirLineId, DateTime? DateFrom, DateTime? DateTo, int? FlightTypeId)
        {
            var id = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(id);
            RoleController role = new RoleController();
            int GetCompanyId = role.GetCompanyId(id);
            var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);
            int AirLineAccId = 0;

            int N = 0;


            // decimal ExchangeRate = Convert.ToDecimal(db.Currencies.Single(x => x.Id == 1).ExchangeRateAgainstPrimaryCurrency);

            string UserName = CommonUsed.GetUserName(id);
            string SearchAirLineId = "غير محدد";

            string SearchDateFrom = "غير محدد";
            string SearchDateTo = "غير محدد";
            string Name = "غير محدد";

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
            else
            {
                return RedirectToAction("Error");
            }

            IQueryable<Revenue> Revenue;
            //Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.TakeOffDate).Where(x => x.OfficerData.Pax > 0 | x.OfficerData.Pax != null | x.OfficerData.TRNYCUPPS != null);
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.TakeOffDate).Where(x => x.OfficerData.Pax > 0 | x.OfficerData.TRNYCUPPS > 0
            && x.OfficerData.TowerData.CompanyInfoId == companyId);

            if (AirLineId > 0)
            {
                Revenue = Revenue.Where(x => x.OfficerData.TowerData.AirLineId == AirLineId);
                SearchAirLineId = db.AirLines.Where(x => x.Id == AirLineId).Single().Name;
                AirLineAccId = (int)db.AirLines.Where(x => x.Id == AirLineId).Single().AccId;

                ViewBag.Name = SearchAirLineId;
                Name = SearchAirLineId;

            }

            if (DateFrom != null && DateTo != null)
            {
                Revenue = Revenue.Where(t => DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) >= DbFunctions.TruncateTime(DateFrom) && DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) <= DbFunctions.TruncateTime(DateTo));
                SearchDateFrom = CommonUsed.ToDisplayDate(DateFrom).ToString();
                SearchDateTo = CommonUsed.ToDisplayDate(DateTo).ToString();

                ViewBag.DateFrom = SearchDateFrom;
                ViewBag.DateTo = SearchDateTo;
            }


            //هنا مفترض نستجلب سعر الصرف من جدول المطالبة وليس من جدول تفاصيل العملات
            if (db.FinancialClaim1.Any(x => x.AccIdDebit == AirLineAccId && (DbFunctions.TruncateTime(x.DateFrom) <= DateFrom & DbFunctions.TruncateTime(x.DateTo) >= DateFrom) |
                 (DbFunctions.TruncateTime(x.DateFrom) <= DateTo & DbFunctions.TruncateTime(x.DateTo) >= DateTo) && x.CompanyInfoId == companyId))
            {
                var obj = db.FinancialClaim1.Where(x => x.AccIdDebit == AirLineAccId && (DbFunctions.TruncateTime(x.DateFrom) <= DateFrom & DbFunctions.TruncateTime(x.DateTo) >= DateFrom) |
                 (DbFunctions.TruncateTime(x.DateFrom) <= DateTo & DbFunctions.TruncateTime(x.DateTo) >= DateTo) && x.CompanyInfoId == companyId).FirstOrDefault();
                ExchangeRate = Convert.ToDecimal(obj.ExchangeRateAgainstPrimaryCurrency);

                ViewBag.ExchangeRate = ExchangeRate;
            }
            else
            {
                return RedirectToAction("Error", "Revenues");
            }

            //ده كود لاستجلاب سعر الصرف و بقى ما شغال بعد شغل الحسابات
            //ExchangeRate = CommonUsed.getExchangeRateByLandingDate(Month, Year);
            //ViewBag.ExchangeRate = ExchangeRate;



            if (FlightTypeId == 1)
            {


                Revenue = Revenue.Where(x => x.OfficerData.TowerData.FlightTypeD == 1 /*|| x.OfficerData.TowerData.FlightTypeL == 1*/);
                Revenue = Revenue.Where(x => x.InterCUPPSFees != null);


                if (Revenue.Count() <= 0)
                {
                    return RedirectToAction("Error");
                }
                //**********************************************************************************
                var RevenueListInter = new List<PrintCuppsViewModel>();
                foreach (var item in Revenue)
                {
                    PrintCuppsViewModel CInter = new PrintCuppsViewModel();
                    //545228

                    N = N + 1;
                    CInter.N = N;
                    //  CInter.Pax = Convert.ToInt32(item?.OfficerData.DISEMB + item.OfficerData.DisEmbInft + item?.OfficerData.TRNYCUPPS);
                    CInter.Pax = Convert.ToInt32(item?.OfficerData.Pax);
                    CInter.TR = Convert.ToInt32(item?.OfficerData.TRNYCUPPS);
                    CInter.Id = item.Id;
                    CInter.Name = item?.OfficerData.TowerData.AirLine.Code + "/ " + item?.OfficerData.TowerData.AirLine.Name;
                    CInter.AirPortTo = item?.OfficerData.TowerData.AirPort.Code;
                    //CInter.TakeOffDate = CommonUsed.ToDisplayDate(item?.OfficerData.TowerData.TakeOffDate.ToString());
                    CInter.TakeOffDate = CommonUsed.ToDisplayDate(item?.OfficerData.TowerData.LandingDate);//تم تعديلها نسبة لان بعض الرحلات تكون بايتة وتظهر بتاريخ الشهر الجديد ودي مشكلة
                    CInter.FlightNo = item?.OfficerData.TowerData.AirLinesFlightTrip.FlightNo;

                    CInter.InterCuppsFees = Convert.ToDecimal(item?.InterCUPPSFees);
                    if (CInter.InterCuppsFees == 0)
                    {
                        CInter.InterFees = 0;
                    }
                    else
                    {
                        CInter.InterFees = (CInter.InterCuppsFees / (CInter.Pax + CInter.TR));
                    }

                    CInter.CUPPSSDG = CInter.InterCuppsFees * ExchangeRate;

                    RevenueListInter.Add(CInter);
                }
                //InterNational
                ///////////////////////////////////////////////////////////////////////////////////////////////////////
                double SumPax = Convert.ToDouble(RevenueListInter.Sum(x => x.Pax));
                ViewBag.SumPax = SumPax;

                double SumTR = Convert.ToDouble(RevenueListInter.Sum(x => x.TR));
                ViewBag.SumTR = SumTR;


                double SumInterCuppsFees = Convert.ToDouble(RevenueListInter.Sum(x => x.InterCuppsFees));
                ViewBag.SumInterCuppsFees = SumInterCuppsFees;

                double SumCUPPSSDG = Convert.ToDouble(RevenueListInter.Sum(x => x.CUPPSSDG));
                ViewBag.SumCUPPSSDG = SumCUPPSSDG;


                double TaxDollar = SumInterCuppsFees * 0.05;
                ViewBag.TaxDollar = TaxDollar;

                double TaxSDG = SumCUPPSSDG * 0.05;
                ViewBag.TaxSDG = TaxSDG;


                double GrandTotalDollar = SumInterCuppsFees + TaxDollar;
                ViewBag.GrandTotalDollar = GrandTotalDollar;

                double GrandTotalSDG = SumCUPPSSDG + TaxSDG;
                ViewBag.GrandTotalSDG = GrandTotalSDG;




                ViewBag.Header = "Cupps-" + Name + "-" + DateTime.Now.ToShortDateString();
                return View("PrintDataCuppsInter", RevenueListInter);
            }
            else
            {


                Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date).Where(x => x.OfficerData.TowerData.CompanyInfoId == companyId);
                Revenue = Revenue.Where(x => x.OfficerData.TowerData.FlightTypeD == 2 /*&& x.OfficerData.TowerData.FlightTypeL == 2*/);
                Revenue = Revenue.Where(x => x.DomeCUPPSFees != null);

                if (AirLineId > 0)
                {
                    Revenue = Revenue.Where(x => x.OfficerData.TowerData.AirLineId == AirLineId);
                    SearchAirLineId = db.AirLines.Where(x => x.Id == AirLineId).Single().Name;
                }

                if (DateFrom != null && DateTo != null)
                {
                    Revenue = Revenue.Where(t => DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) >= DbFunctions.TruncateTime(DateFrom) && DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) <= DbFunctions.TruncateTime(DateTo));
                    SearchDateFrom = CommonUsed.ToDisplayDate(DateFrom).ToString();
                    SearchDateTo = CommonUsed.ToDisplayDate(DateTo).ToString();
                }
                if (Revenue.Count() <= 0)
                {
                    return RedirectToAction("Error");
                }
                ///***********************************************************************************
                var RevenueList = new List<PrintCuppsViewModel>();
                foreach (var item in Revenue)
                {
                    PrintCuppsViewModel C = new PrintCuppsViewModel();
                    N = N + 1;
                    C.N = N;
                    if (item.OfficerData.PaxInter > 0)
                    {
                        C.Pax2 = Convert.ToInt32(item?.OfficerData.Pax).ToString() + "+" + Convert.ToInt32(item?.OfficerData.PaxInter).ToString();
                        C.PaxInter = Convert.ToInt32(item?.OfficerData.PaxInter);
                        ViewBag.PaxInter = C.PaxInter;
                    }
                    else
                    {
                        C.Pax2 = Convert.ToInt32(item?.OfficerData.Pax).ToString();


                    }

                    C.Pax = Convert.ToInt32(item?.OfficerData.Pax);
                    C.TR = Convert.ToInt32(item?.OfficerData.TRNYCUPPS);
                    C.Id = item.Id;
                    C.Name = item?.OfficerData.TowerData.AirLine.Code + "/ " + item?.OfficerData.TowerData.AirLine.Name;
                    C.AirPortTo = item?.OfficerData.TowerData.AirPort.Code;
                    C.TakeOffDate = CommonUsed.ToDisplayDate(item?.OfficerData.TowerData.TakeOffDate);
                    C.FlightNo = item?.OfficerData.TowerData.AirLinesFlightTrip.FlightNo;

                    if (item.OfficerData.PaxInter > 0)
                    {
                        C.DomeCuppsFees = Convert.ToDecimal(item?.DomeCUPPSFees) + Convert.ToDecimal(item?.InterCUPPSFees);
                    }
                    else { C.DomeCuppsFees = Convert.ToDecimal(item?.DomeCUPPSFees); }



                    if (item.DomeCUPPSFees == 0) { C.DomeFees = 0; }
                    else
                    {
                        C.DomeFees = (Convert.ToDecimal(item.DomeCUPPSFees) / (C.Pax + C.TR));
                    }
                    C.CUPPSSDG = C.DomeCuppsFees * ExchangeRate;

                    RevenueList.Add(C);
                }

                //Domestic
                ///////////////////////////////////////////////////////////////////////////////////////////////////////
                //double SumPax = Convert.ToDouble(RevenueList.Sum(x => x.Pax));
                //ViewBag.SumPax = SumPax;

                string SumPax;
                if (RevenueList.Any(x => x.PaxInter > 0))
                {
                    SumPax = Convert.ToDouble(RevenueList.Sum(x => x.Pax)).ToString() + "+" + Convert.ToDouble(RevenueList.Sum(x => x.PaxInter)).ToString();
                }
                else
                {
                    SumPax = Convert.ToDouble(RevenueList.Sum(x => x.Pax)).ToString();
                }
                ViewBag.SumPax = SumPax;


                double SumTR = Convert.ToDouble(RevenueList.Sum(x => x.TR));
                ViewBag.SumTR = SumTR;


                double SumDomeCuppsFees = Convert.ToDouble(RevenueList.Sum(x => x.DomeCuppsFees));
                ViewBag.SumDomeCuppsFees = SumDomeCuppsFees;

                double SumCUPPSSDG = Convert.ToDouble(RevenueList.Sum(x => x.CUPPSSDG));
                ViewBag.SumCUPPSSDG = SumCUPPSSDG;


                double TaxDollar = SumDomeCuppsFees * 0.05;
                ViewBag.TaxDollar = TaxDollar;

                double TaxSDG = SumCUPPSSDG * 0.05;
                ViewBag.TaxSDG = TaxSDG;


                double GrandTotalDollar = SumDomeCuppsFees + TaxDollar;
                ViewBag.GrandTotalDollar = GrandTotalDollar;

                double GrandTotalSDG = SumCUPPSSDG + TaxSDG;
                ViewBag.GrandTotalSDG = GrandTotalSDG;









                ViewBag.Header = "Cupps-" + Name + "-" + DateTime.Now.ToShortDateString();
                return View("PrintDataCuppsDome", RevenueList);
            }
        }

        [Authorize(Roles = "Revenues,Super-Admin,Cupps,Ny,Ny&&Car, Account, Account-Admin")]
        public ActionResult PrintDataCupps(int? AirLineId, DateTime? DateFrom, DateTime? DateTo, int? FlightTypeId)
        {
            var id = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(id);
            RoleController role = new RoleController();
            int GetCompanyId = role.GetCompanyId(id);
            var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);
            int AirLineAccId = 0;

            int N = 0;


            // decimal ExchangeRate = Convert.ToDecimal(db.Currencies.Single(x => x.Id == 1).ExchangeRateAgainstPrimaryCurrency);

            string UserName = CommonUsed.GetUserName(id);
            string SearchAirLineId = "غير محدد";

            string SearchDateFrom = "غير محدد";
            string SearchDateTo = "غير محدد";
            string Name = "غير محدد";

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
            else
            {
                return RedirectToAction("Error");
            }


            IQueryable<Revenue> Revenue;
            //Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.TakeOffDate).Where(x => x.OfficerData.Pax > 0 | x.OfficerData.Pax != null | x.OfficerData.TRNYCUPPS != null);
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.TakeOffDate).Where(x => x.OfficerData.Pax > 0 | x.OfficerData.TRNYCUPPS > 0
            && x.OfficerData.TowerData.CompanyInfoId == companyId);

            if (AirLineId > 0)
            {
                Revenue = Revenue.Where(x => x.OfficerData.TowerData.AirLineId == AirLineId);
                SearchAirLineId = db.AirLines.Where(x => x.Id == AirLineId).Single().Name;
                AirLineAccId = (int)db.AirLines.Where(x => x.Id == AirLineId).Single().AccId;

                ViewBag.Name = SearchAirLineId;
                Name = SearchAirLineId;

            }

            if (DateFrom != null && DateTo != null)
            {
                Revenue = Revenue.Where(t => DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) >= DbFunctions.TruncateTime(DateFrom) && DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) <= DbFunctions.TruncateTime(DateTo));
                SearchDateFrom = CommonUsed.ToDisplayDate(DateFrom).ToString();
                SearchDateTo = CommonUsed.ToDisplayDate(DateTo).ToString();

                ViewBag.DateFrom = SearchDateFrom;
                ViewBag.DateTo = SearchDateTo;
            }


            //هنا مفترض نستجلب سعر الصرف من جدول المطالبة وليس من جدول تفاصيل العملات
            if (db.FinancialClaim1.Any(x => x.AccIdDebit == AirLineAccId && (DbFunctions.TruncateTime(x.DateFrom) <= DateFrom & DbFunctions.TruncateTime(x.DateTo) >= DateFrom) |
                 (DbFunctions.TruncateTime(x.DateFrom) <= DateTo & DbFunctions.TruncateTime(x.DateTo) >= DateTo) && x.CompanyInfoId == companyId))
            {
                var obj = db.FinancialClaim1.Where(x => x.AccIdDebit == AirLineAccId && (DbFunctions.TruncateTime(x.DateFrom) <= DateFrom & DbFunctions.TruncateTime(x.DateTo) >= DateFrom) |
                 (DbFunctions.TruncateTime(x.DateFrom) <= DateTo & DbFunctions.TruncateTime(x.DateTo) >= DateTo) && x.CompanyInfoId == companyId).FirstOrDefault();
                ExchangeRate = Convert.ToDecimal(obj.ExchangeRateAgainstPrimaryCurrency);

                ViewBag.ExchangeRate = ExchangeRate;
            }
            else
            {
                return RedirectToAction("Error", "Revenues");
            }

            //ده كود لاستجلاب سعر الصرف و بقى ما شغال بعد شغل الحسابات
            //ExchangeRate = CommonUsed.getExchangeRateByLandingDate(Month, Year);
            //ViewBag.ExchangeRate = ExchangeRate;



            if (FlightTypeId == 1)
            {


                Revenue = Revenue.Where(x => x.OfficerData.TowerData.FlightTypeD == 1 /*|| x.OfficerData.TowerData.FlightTypeL == 1*/);
                Revenue = Revenue.Where(x => x.InterCUPPSFees != null);


                if (Revenue.Count() <= 0)
                {
                    return RedirectToAction("Error");
                }
                //**********************************************************************************
                var RevenueListInter = new List<PrintCuppsViewModel>();
                foreach (var item in Revenue)
                {
                    PrintCuppsViewModel CInter = new PrintCuppsViewModel();
                    //545228

                    N = N + 1;
                    CInter.N = N;
                    //  CInter.Pax = Convert.ToInt32(item?.OfficerData.DISEMB + item.OfficerData.DisEmbInft + item?.OfficerData.TRNYCUPPS);
                    CInter.Pax = Convert.ToInt32(item?.OfficerData.Pax);
                    CInter.TR = Convert.ToInt32(item?.OfficerData.TRNYCUPPS);
                    CInter.Id = item.Id;
                    CInter.Name = item?.OfficerData.TowerData.AirLine.Code + "/ " + item?.OfficerData.TowerData.AirLine.Name;
                    CInter.AirPortTo = item?.OfficerData.TowerData.AirPort.Code;
                    //CInter.TakeOffDate = CommonUsed.ToDisplayDate(item?.OfficerData.TowerData.TakeOffDate.ToString());
                    CInter.TakeOffDate = CommonUsed.ToDisplayDate(item?.OfficerData.TowerData.LandingDate);//تم تعديلها نسبة لان بعض الرحلات تكون بايتة وتظهر بتاريخ الشهر الجديد ودي مشكلة
                    CInter.FlightNo = item?.OfficerData.TowerData.AirLinesFlightTrip.FlightNo;

                    CInter.InterCuppsFees = Convert.ToDecimal(item?.InterCUPPSFees);
                    if (CInter.InterCuppsFees == 0)
                    {
                        CInter.InterFees = 0;
                    }
                    else
                    {
                        CInter.InterFees = (CInter.InterCuppsFees / (CInter.Pax + CInter.TR));
                    }

                    CInter.CUPPSSDG = CInter.InterCuppsFees * ExchangeRate;

                    RevenueListInter.Add(CInter);
                }
                //InterNational
                ///////////////////////////////////////////////////////////////////////////////////////////////////////
                double SumPax = Convert.ToDouble(RevenueListInter.Sum(x => x.Pax));
                ViewBag.SumPax = SumPax;

                double SumTR = Convert.ToDouble(RevenueListInter.Sum(x => x.TR));
                ViewBag.SumTR = SumTR;


                double SumInterCuppsFees = Convert.ToDouble(RevenueListInter.Sum(x => x.InterCuppsFees));
                ViewBag.SumInterCuppsFees = SumInterCuppsFees;

                double SumCUPPSSDG = Convert.ToDouble(RevenueListInter.Sum(x => x.CUPPSSDG));
                ViewBag.SumCUPPSSDG = SumCUPPSSDG;


                double TaxDollar = SumInterCuppsFees * 0.05;
                ViewBag.TaxDollar = TaxDollar;

                double TaxSDG = SumCUPPSSDG * 0.05;
                ViewBag.TaxSDG = TaxSDG;


                double GrandTotalDollar = SumInterCuppsFees + TaxDollar;
                ViewBag.GrandTotalDollar = GrandTotalDollar;

                double GrandTotalSDG = SumCUPPSSDG + TaxSDG;
                ViewBag.GrandTotalSDG = GrandTotalSDG;




                ViewBag.Header = "Cupps-" + Name + "-" + DateTime.Now.ToShortDateString();
                return View("PrintDataCuppsInter", RevenueListInter);
            }
            else
            {


            

                Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date).Where(x => x.OfficerData.TowerData.CompanyInfoId == companyId);
                Revenue = Revenue.Where(x => x.OfficerData.TowerData.FlightTypeD == 2 /*&& x.OfficerData.TowerData.FlightTypeL == 2*/);
                Revenue = Revenue.Where(x => x.DomeCUPPSFees != null);

                if (AirLineId > 0)
                {
                    Revenue = Revenue.Where(x => x.OfficerData.TowerData.AirLineId == AirLineId);
                    SearchAirLineId = db.AirLines.Where(x => x.Id == AirLineId).Single().Name;
                }

                if (DateFrom != null && DateTo != null)
                {
                    Revenue = Revenue.Where(t => DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) >= DbFunctions.TruncateTime(DateFrom) && DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) <= DbFunctions.TruncateTime(DateTo));
                    SearchDateFrom = CommonUsed.ToDisplayDate(DateFrom).ToString();
                    SearchDateTo = CommonUsed.ToDisplayDate(DateTo).ToString();
                }
                if (Revenue.Count() <= 0)
                {
                    return RedirectToAction("Error");
                }
                ///***********************************************************************************
                var RevenueList = new List<PrintCuppsViewModel>();
                foreach (var item in Revenue)
                {
                    PrintCuppsViewModel C = new PrintCuppsViewModel();
                    N = N + 1;
                    C.N = N;
                    C.Pax = Convert.ToInt32(item?.OfficerData.Pax);
                    C.TR = Convert.ToInt32(item?.OfficerData.TRNYCUPPS);
                    C.Id = item.Id;
                    C.Name = item?.OfficerData.TowerData.AirLine.Code + "/ " + item?.OfficerData.TowerData.AirLine.Name;
                    C.AirPortTo = item?.OfficerData.TowerData.AirPort.Code;
                    C.TakeOffDate = CommonUsed.ToDisplayDate(item?.OfficerData.TowerData.TakeOffDate);
                    C.FlightNo = item?.OfficerData.TowerData.AirLinesFlightTrip.FlightNo;
                    C.DomeCuppsFees = Convert.ToDecimal(item?.DomeCUPPSFees);
                    if (C.DomeCuppsFees == 0) { C.DomeFees = 0; }
                    else
                    {
                        C.DomeFees = (C.DomeCuppsFees / (C.Pax + C.TR));
                    }
                    C.CUPPSSDG = C.DomeCuppsFees * ExchangeRate;

                    RevenueList.Add(C);
                }

                //Domestic
                ///////////////////////////////////////////////////////////////////////////////////////////////////////
                double SumPax = Convert.ToDouble(RevenueList.Sum(x => x.Pax));
                ViewBag.SumPax = SumPax;

                double SumTR = Convert.ToDouble(RevenueList.Sum(x => x.TR));
                ViewBag.SumTR = SumTR;


                double SumDomeCuppsFees = Convert.ToDouble(RevenueList.Sum(x => x.DomeCuppsFees));
                ViewBag.SumDomeCuppsFees = SumDomeCuppsFees;

                double SumCUPPSSDG = Convert.ToDouble(RevenueList.Sum(x => x.CUPPSSDG));
                ViewBag.SumCUPPSSDG = SumCUPPSSDG;


                double TaxDollar = SumDomeCuppsFees * 0.05;
                ViewBag.TaxDollar = TaxDollar;

                double TaxSDG = SumCUPPSSDG * 0.05;
                ViewBag.TaxSDG = TaxSDG;


                double GrandTotalDollar = SumDomeCuppsFees + TaxDollar;
                ViewBag.GrandTotalDollar = GrandTotalDollar;

                double GrandTotalSDG = SumCUPPSSDG + TaxSDG;
                ViewBag.GrandTotalSDG = GrandTotalSDG;









                ViewBag.Header = "Cupps-" + Name + "-" + DateTime.Now.ToShortDateString();
                return View("PrintDataCuppsDome", RevenueList);
            }
        }

        public ActionResult PrintDataCuppForDom(int? AirLineId, DateTime? DateFrom, DateTime? DateTo, int? FlightTypeId)
        {
            var id = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(id);
            RoleController role = new RoleController();
            int GetCompanyId = role.GetCompanyId(id);
            var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);
            int AirLineAccId = 0;
            int N = 0;
            string UserName = CommonUsed.GetUserName(id);
            string SearchAirLineId = "غير محدد";

            string SearchDateFrom = "غير محدد";
            string SearchDateTo = "غير محدد";
            string Name = "غير محدد";

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
            else
            {
                return RedirectToAction("Error");
            }

            IQueryable<Revenue> Revenue;
            //Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.TakeOffDate).Where(x => x.OfficerData.Pax > 0 | x.OfficerData.Pax != null | x.OfficerData.TRNYCUPPS != null);
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.TakeOffDate).Where(x => x.OfficerData.PaxInter > 0 | x.OfficerData.TRNYCUPPS > 0
            && x.OfficerData.TowerData.CompanyInfoId == companyId);

            if (AirLineId > 0)
            {
                Revenue = Revenue.Where(x => x.OfficerData.TowerData.AirLineId == AirLineId);
                SearchAirLineId = db.AirLines.Where(x => x.Id == AirLineId).Single().Name;
                AirLineAccId = (int)db.AirLines.Where(x => x.Id == AirLineId).Single().AccId;

                ViewBag.Name = SearchAirLineId;
                Name = SearchAirLineId;

            }

            if (DateFrom != null && DateTo != null)
            {
                Revenue = Revenue.Where(t => DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) >= DbFunctions.TruncateTime(DateFrom) && DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) <= DbFunctions.TruncateTime(DateTo));
                SearchDateFrom = CommonUsed.ToDisplayDate(DateFrom).ToString();
                SearchDateTo = CommonUsed.ToDisplayDate(DateTo).ToString();

                ViewBag.DateFrom = SearchDateFrom;
                ViewBag.DateTo = SearchDateTo;
            }


            //هنا مفترض نستجلب سعر الصرف من جدول المطالبة وليس من جدول تفاصيل العملات
            if (db.FinancialClaim1.Any(x => x.AccIdDebit == AirLineAccId && (DbFunctions.TruncateTime(x.DateFrom) <= DateFrom & DbFunctions.TruncateTime(x.DateTo) >= DateFrom) |
                 (DbFunctions.TruncateTime(x.DateFrom) <= DateTo & DbFunctions.TruncateTime(x.DateTo) >= DateTo) && x.CompanyInfoId == companyId))
            {
                var obj = db.FinancialClaim1.Where(x => x.AccIdDebit == AirLineAccId && (DbFunctions.TruncateTime(x.DateFrom) <= DateFrom & DbFunctions.TruncateTime(x.DateTo) >= DateFrom) |
                 (DbFunctions.TruncateTime(x.DateFrom) <= DateTo & DbFunctions.TruncateTime(x.DateTo) >= DateTo) && x.CompanyInfoId == companyId).FirstOrDefault();
                ExchangeRate = Convert.ToDecimal(obj.ExchangeRateAgainstPrimaryCurrency);

                ViewBag.ExchangeRate = ExchangeRate;
            }
            else
            {
                return RedirectToAction("Error", "Revenues");
            }


            if (FlightTypeId == 2)
            {


                Revenue = Revenue.Where(x => x.OfficerData.TowerData.FlightTypeD == 2 /*|| x.OfficerData.TowerData.FlightTypeL == 1*/);
                Revenue = Revenue.Where(x => x.InterCUPPSFees != null);




                if (Revenue.Count() <= 0)
                {
                    return RedirectToAction("Error");
                }
                //**********************************************************************************
                var RevenueListInter = new List<PrintCuppsViewModel>();
                foreach (var item in Revenue)
                {
                    PrintCuppsViewModel CInter = new PrintCuppsViewModel();
                    //545228

                    N = N + 1;
                    CInter.N = N;
                    CInter.Pax = Convert.ToInt32(item?.OfficerData.PaxInter);
                    CInter.TR = Convert.ToInt32(item?.OfficerData.TRNYCUPPS);
                    CInter.Id = item.Id;
                    CInter.Name = item?.OfficerData.TowerData.AirLine.Code + "/ " + item?.OfficerData.TowerData.AirLine.Name;
                    CInter.AirPortTo = item?.OfficerData.TowerData.AirPort.Code;
                    //CInter.TakeOffDate = CommonUsed.ToDisplayDate(item?.OfficerData.TowerData.TakeOffDate.ToString());
                    CInter.TakeOffDate = CommonUsed.ToDisplayDate(item?.OfficerData.TowerData.LandingDate);//تم تعديلها نسبة لان بعض الرحلات تكون بايتة وتظهر بتاريخ الشهر الجديد ودي مشكلة
                    CInter.FlightNo = item?.OfficerData.TowerData.AirLinesFlightTrip.FlightNo;

                    CInter.InterCuppsFees = Convert.ToDecimal(item?.InterCUPPSFees);
                    if (CInter.InterCuppsFees == 0)
                    {
                        CInter.InterFees = 0;
                    }
                    else
                    {
                        CInter.InterFees = (CInter.InterCuppsFees / (CInter.Pax + CInter.TR));
                    }

                    CInter.CUPPSSDG = CInter.InterCuppsFees * ExchangeRate;

                    RevenueListInter.Add(CInter);
                }
                //InterNational
                ///////////////////////////////////////////////////////////////////////////////////////////////////////
                double SumPax = Convert.ToDouble(RevenueListInter.Sum(x => x.Pax));
                ViewBag.SumPax = SumPax;

                double SumTR = Convert.ToDouble(RevenueListInter.Sum(x => x.TR));
                ViewBag.SumTR = SumTR;


                double SumInterCuppsFees = Convert.ToDouble(RevenueListInter.Sum(x => x.InterCuppsFees));
                ViewBag.SumInterCuppsFees = SumInterCuppsFees;

                double SumCUPPSSDG = Convert.ToDouble(RevenueListInter.Sum(x => x.CUPPSSDG));
                ViewBag.SumCUPPSSDG = SumCUPPSSDG;


                double TaxDollar = SumInterCuppsFees * 0.05;
                ViewBag.TaxDollar = TaxDollar;

                double TaxSDG = SumCUPPSSDG * 0.05;
                ViewBag.TaxSDG = TaxSDG;


                double GrandTotalDollar = SumInterCuppsFees + TaxDollar;
                ViewBag.GrandTotalDollar = GrandTotalDollar;

                double GrandTotalSDG = SumCUPPSSDG + TaxSDG;
                ViewBag.GrandTotalSDG = GrandTotalSDG;




                ViewBag.Header = "Cupps-" + Name + "-" + DateTime.Now.ToShortDateString();
                return View("PrintDataCuppForDom", RevenueListInter);
            }
            else {
                return View("error");
            }
           
        }

        public ActionResult PrintDataCuppsDistribution(int? AirLineId, DateTime? DateFrom, DateTime? DateTo, int? FlightTypeId)
        {
            var id = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(id);
            RoleController role = new RoleController();
            int GetCompanyId = role.GetCompanyId(id);
            var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);
            int AirLineAccId = 0;

            int N = 0;


            // decimal ExchangeRate = Convert.ToDecimal(db.Currencies.Single(x => x.Id == 1).ExchangeRateAgainstPrimaryCurrency);

            string UserName = CommonUsed.GetUserName(id);
            string SearchAirLineId = "غير محدد";

            string SearchDateFrom = "غير محدد";
            string SearchDateTo = "غير محدد";
            string Name = "غير محدد";

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
            else
            {
                return RedirectToAction("Error");
            }

            IQueryable<Revenue> Revenue;
            //Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.TakeOffDate).Where(x => x.OfficerData.Pax > 0 | x.OfficerData.Pax != null | x.OfficerData.TRNYCUPPS != null);
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.TakeOffDate).Where(x => x.OfficerData.Pax > 0 | x.OfficerData.TRNYCUPPS > 0
            && x.OfficerData.TowerData.CompanyInfoId == companyId);

            if (AirLineId > 0)
            {
                Revenue = Revenue.Where(x => x.OfficerData.TowerData.AirLineId == AirLineId);
                SearchAirLineId = db.AirLines.Where(x => x.Id == AirLineId).Single().Name;
                AirLineAccId = (int)db.AirLines.Where(x => x.Id == AirLineId).Single().AccId;

                ViewBag.Name = SearchAirLineId;
                Name = SearchAirLineId;

            }

            if (DateFrom != null && DateTo != null)
            {
                Revenue = Revenue.Where(t => DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) >= DbFunctions.TruncateTime(DateFrom) && DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) <= DbFunctions.TruncateTime(DateTo));
                SearchDateFrom = CommonUsed.ToDisplayDate(DateFrom).ToString();
                SearchDateTo = CommonUsed.ToDisplayDate(DateTo).ToString();

                ViewBag.DateFrom = SearchDateFrom;
                ViewBag.DateTo = SearchDateTo;
            }


            //هنا مفترض نستجلب سعر الصرف من جدول المطالبة وليس من جدول تفاصيل العملات
            if (db.FinancialClaim1.Any(x => x.AccIdDebit == AirLineAccId && (DbFunctions.TruncateTime(x.DateFrom) <= DateFrom & DbFunctions.TruncateTime(x.DateTo) >= DateFrom) |
                 (DbFunctions.TruncateTime(x.DateFrom) <= DateTo & DbFunctions.TruncateTime(x.DateTo) >= DateTo) && x.CompanyInfoId == companyId))
            {
                var obj = db.FinancialClaim1.Where(x => x.AccIdDebit == AirLineAccId && (DbFunctions.TruncateTime(x.DateFrom) <= DateFrom & DbFunctions.TruncateTime(x.DateTo) >= DateFrom) |
                 (DbFunctions.TruncateTime(x.DateFrom) <= DateTo & DbFunctions.TruncateTime(x.DateTo) >= DateTo) && x.CompanyInfoId == companyId).FirstOrDefault();
                ExchangeRate = Convert.ToDecimal(obj.ExchangeRateAgainstPrimaryCurrency);

                ViewBag.ExchangeRate = ExchangeRate;
            }
            else
            {
                return RedirectToAction("Error", "Revenues");
            }

            //ده كود لاستجلاب سعر الصرف و بقى ما شغال بعد شغل الحسابات
            //ExchangeRate = CommonUsed.getExchangeRateByLandingDate(Month, Year);
            //ViewBag.ExchangeRate = ExchangeRate;



            if (FlightTypeId == 1)
            {


                Revenue = Revenue.Where(x => x.OfficerData.TowerData.FlightTypeD == 1 /*|| x.OfficerData.TowerData.FlightTypeL == 1*/);
                Revenue = Revenue.Where(x => x.InterCUPPSFees != null);


                if (Revenue.Count() <= 0)
                {
                    return RedirectToAction("Error");
                }
                //**********************************************************************************
                var RevenueListInter = new List<PrintCuppsViewModel>();
                foreach (var item in Revenue)
                {
                    PrintCuppsViewModel CInter = new PrintCuppsViewModel();
                    //545228

                    N = N + 1;
                    CInter.N = N;
                    CInter.ExchangeRate = item?.ExchangeRate;
                    //  CInter.Pax = Convert.ToInt32(item?.OfficerData.DISEMB + item.OfficerData.DisEmbInft + item?.OfficerData.TRNYCUPPS);
                    CInter.Pax = Convert.ToInt32(item?.OfficerData.Pax);
                    CInter.TR = Convert.ToInt32(item?.OfficerData.TRNYCUPPS);
                    CInter.Id = item.Id;
                    CInter.Name = item?.OfficerData.TowerData.AirLine.Code + "/ " + item?.OfficerData.TowerData.AirLine.Name;
                    CInter.AirPortTo = item?.OfficerData.TowerData.AirPort.Code;
                    //CInter.TakeOffDate = CommonUsed.ToDisplayDate(item?.OfficerData.TowerData.TakeOffDate.ToString());
                    CInter.TakeOffDate = CommonUsed.ToDisplayDate(item?.OfficerData.TowerData.LandingDate);//تم تعديلها نسبة لان بعض الرحلات تكون بايتة وتظهر بتاريخ الشهر الجديد ودي مشكلة
                    CInter.FlightNo = item?.OfficerData.TowerData.AirLinesFlightTrip.FlightNo;
                    CInter.ExchangeRate = item?.ExchangeRate;
                    CInter.InterCuppsFees = Convert.ToDecimal(item?.InterCUPPSFees);
                    if (CInter.InterCuppsFees == 0)
                    {
                        CInter.InterFees = 0;
                    }
                    else
                    {
                        CInter.InterFees = (CInter.InterCuppsFees / (CInter.Pax + CInter.TR));
                    }

                    CInter.CUPPSSDG = Convert.ToDecimal(CInter.InterCuppsFees * CInter.ExchangeRate);

                    RevenueListInter.Add(CInter);
                }
                //InterNational
                ///////////////////////////////////////////////////////////////////////////////////////////////////////
                double SumPax = Convert.ToDouble(RevenueListInter.Sum(x => x.Pax));
                ViewBag.SumPax = SumPax;

                double SumTR = Convert.ToDouble(RevenueListInter.Sum(x => x.TR));
                ViewBag.SumTR = SumTR;


                double SumInterCuppsFees = Convert.ToDouble(RevenueListInter.Sum(x => x.InterCuppsFees));
                ViewBag.SumInterCuppsFees = SumInterCuppsFees;

                double SumCUPPSSDG = Convert.ToDouble(RevenueListInter.Sum(x => x.CUPPSSDG));
                ViewBag.SumCUPPSSDG = SumCUPPSSDG;


                double TaxDollar = SumInterCuppsFees * 0.05;
                ViewBag.TaxDollar = TaxDollar;

                double TaxSDG = SumCUPPSSDG * 0.05;
                ViewBag.TaxSDG = TaxSDG;


                double GrandTotalDollar = SumInterCuppsFees + TaxDollar;
                ViewBag.GrandTotalDollar = GrandTotalDollar;

                double GrandTotalSDG = SumCUPPSSDG + TaxSDG;
                ViewBag.GrandTotalSDG = GrandTotalSDG;




                ViewBag.Header = "Cupps-" + Name + "-" + DateTime.Now.ToShortDateString();
                return View("PrintDataCuppsDistributionInter", RevenueListInter);
            }
            else
            {


                Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date).Where(x => x.OfficerData.TowerData.CompanyInfoId == companyId);
                Revenue = Revenue.Where(x => x.OfficerData.TowerData.FlightTypeD == 2 /*&& x.OfficerData.TowerData.FlightTypeL == 2*/);
                Revenue = Revenue.Where(x => x.DomeCUPPSFees != null);

                if (AirLineId > 0)
                {
                    Revenue = Revenue.Where(x => x.OfficerData.TowerData.AirLineId == AirLineId);
                    SearchAirLineId = db.AirLines.Where(x => x.Id == AirLineId).Single().Name;
                }

                if (DateFrom != null && DateTo != null)
                {
                    Revenue = Revenue.Where(t => DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) >= DbFunctions.TruncateTime(DateFrom) && DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) <= DbFunctions.TruncateTime(DateTo));
                    SearchDateFrom = CommonUsed.ToDisplayDate(DateFrom).ToString();
                    SearchDateTo = CommonUsed.ToDisplayDate(DateTo).ToString();
                }
                if (Revenue.Count() <= 0)
                {
                    return RedirectToAction("Error");
                }
                ///***********************************************************************************
                var RevenueList = new List<PrintCuppsViewModel>();
                foreach (var item in Revenue)
                {
                    PrintCuppsViewModel C = new PrintCuppsViewModel();
                    N = N + 1;
                    C.N = N;
                    C.ExchangeRate = item?.ExchangeRate;
                    //if (item.OfficerData.PaxInter > 0)
                    //{
                    //    C.Pax2 = Convert.ToInt32(item?.OfficerData.Pax).ToString() + "+" + Convert.ToInt32(item?.OfficerData.PaxInter).ToString();
                    //    C.PaxInter = Convert.ToInt32(item?.OfficerData.PaxInter);
                    //    ViewBag.PaxInter = C.PaxInter;
                    //}
                    //else
                    //{
                        C.Pax2 = Convert.ToInt32(item?.OfficerData.Pax).ToString();


                    //}

                    C.Pax = Convert.ToInt32(item?.OfficerData.Pax);
                    C.TR = Convert.ToInt32(item?.OfficerData.TRNYCUPPS);
                    C.Id = item.Id;
                    C.Name = item?.OfficerData.TowerData.AirLine.Code + "/ " + item?.OfficerData.TowerData.AirLine.Name;
                    C.AirPortTo = item?.OfficerData.TowerData.AirPort.Code;
                    C.TakeOffDate = CommonUsed.ToDisplayDate(item?.OfficerData.TowerData.TakeOffDate);
                    C.FlightNo = item?.OfficerData.TowerData.AirLinesFlightTrip.FlightNo;
                   
                    //if (item.OfficerData.PaxInter > 0)
                    //{
                    //    C.DomeCuppsFees = Convert.ToDecimal(item?.DomeCUPPSFees) + Convert.ToDecimal(item?.InterCUPPSFees);
                    //}
                   /* else {*/ C.DomeCuppsFees = Convert.ToDecimal(item?.DomeCUPPSFees); /*}*/



                    if (item.DomeCUPPSFees == 0) { C.DomeFees = 0; }
                    else
                    {
                        C.DomeFees = (Convert.ToDecimal(item.DomeCUPPSFees) / (C.Pax + C.TR));
                    }
                    C.CUPPSSDG = Convert.ToDecimal(C.DomeCuppsFees * C.ExchangeRate);

                    RevenueList.Add(C);
                }

                //Domestic
                ///////////////////////////////////////////////////////////////////////////////////////////////////////
                //double SumPax = Convert.ToDouble(RevenueList.Sum(x => x.Pax));
                //ViewBag.SumPax = SumPax;

                string SumPax;
                //if (RevenueList.Any(x => x.PaxInter > 0))
                //{
                //    SumPax = Convert.ToDouble(RevenueList.Sum(x => x.Pax)).ToString() + "+" + Convert.ToDouble(RevenueList.Sum(x => x.PaxInter)).ToString();
                //}
                //else
                //{
                    SumPax = Convert.ToDouble(RevenueList.Sum(x => x.Pax)).ToString();
                //}
                ViewBag.SumPax = SumPax;


                double SumTR = Convert.ToDouble(RevenueList.Sum(x => x.TR));
                ViewBag.SumTR = SumTR;


                double SumDomeCuppsFees = Convert.ToDouble(RevenueList.Sum(x => x.DomeCuppsFees));
                ViewBag.SumDomeCuppsFees = SumDomeCuppsFees;

                double SumCUPPSSDG = Convert.ToDouble(RevenueList.Sum(x => x.CUPPSSDG));
                ViewBag.SumCUPPSSDG = SumCUPPSSDG;


                double TaxDollar = SumDomeCuppsFees * 0.05;
                ViewBag.TaxDollar = TaxDollar;

                double TaxSDG = SumCUPPSSDG * 0.05;
                ViewBag.TaxSDG = TaxSDG;


                double GrandTotalDollar = SumDomeCuppsFees + TaxDollar;
                ViewBag.GrandTotalDollar = GrandTotalDollar;

                double GrandTotalSDG = SumCUPPSSDG + TaxSDG;
                ViewBag.GrandTotalSDG = GrandTotalSDG;









                ViewBag.Header = "Cupps-" + Name + "-" + DateTime.Now.ToShortDateString();
                return View("PrintDataCuppsDistributionDome", RevenueList);
            }
        }

        /*---------------------------------------------------- New Reports -----------------------------------------------------------------------------*/
        public ActionResult PrintDataGHGrafDollar()
        {

            return View();
        }

        [Authorize(Roles = "Revenues,Super-Admin, Account, Account-Admin")]
        public ActionResult PrintDataGHGraf(int? HandlingCompanyId, DateTime? DateFrom, DateTime? DateTo)
        {
            var id = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(id);
            RoleController role = new RoleController();

            int GetCompanyId = role.GetCompanyId(id);
            var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);

            string RevenuesManager = CompanyInfo.RevenuesManager;
            ViewBag.RevenuesManager = RevenuesManager;

            string DateNow = System.DateTime.Now.ToString("yyyy/MM/dd") + "م";
            ViewBag.DateNow = DateNow;

            string Serial = Guid.NewGuid().ToString();

            string UserName = CommonUsed.GetUserName(id);

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
            else
            {
                //Month = Convert.ToDateTime(currentDate).Month;
                //Year = Convert.ToDateTime(currentDate).Year;
                return RedirectToAction("Error");
            }
            ExchangeRate = CommonUsed.getExchangeRateByLandingDate(Month, Year);
            //string DemandMouth = db.MouthWorkOns.Where(x => x.Id == Month).FirstOrDefault().Mouth;
            //string DemandMouth = Convert.ToDateTime(DateFrom).ToString("MMMM");
            string DemandMouth = Convert.ToDateTime(DateFrom)
                    .ToString("MMMM", new CultureInfo("ar-EG"));

            string DemandDate = "مطالبة شهر " + DemandMouth + " " + Year + "م";
            ViewBag.DemandDate = DemandDate;

            string ArName = "غير محدد";
            string Name = "غير محدد";

            string SearchDateFrom = "غير محدد";
            string SearchDateTo = "غير محدد";

            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date).Where(x => x.OfficerData.TowerData.CompanyInfoId == companyId);
            ;

            if (HandlingCompanyId > 0)
            {
                Revenue = Revenue.Where(x => x.OfficerData.HandlingAgentId == HandlingCompanyId);
                ArName = db.HandlingAgentsCompanies.Find(HandlingCompanyId).NameAr;
                Name = db.HandlingAgentsCompanies.Find(HandlingCompanyId).NameEn;
            }
            ViewBag.ArName = ArName;
            if (DateFrom != null && DateTo != null)
            {
                Revenue = Revenue.Where(t => DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) >= DbFunctions.TruncateTime(DateFrom) && DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) <= DbFunctions.TruncateTime(DateTo));
                SearchDateFrom = CommonUsed.ToDisplayDate(DateFrom).ToString();
                SearchDateTo = CommonUsed.ToDisplayDate(DateTo).ToString();

                ViewBag.SearchDateFrom = SearchDateFrom;
                ViewBag.SearchDateTo = SearchDateTo;

            }
            /***********************************************************************************/

            PrintFinalInvoiceViewModel F = new PrintFinalInvoiceViewModel();

            F.Name = db.HandlingAgentsCompanies.Find(HandlingCompanyId).NameAr;

            F.FreightFees = Convert.ToDouble(Revenue.Sum(x => x.GroundHandlingFees));

            F.TaxDollar = F.FreightFees * 0.05;

            F.GrandTotalDollar = (F.FreightFees + F.TaxDollar);
            F.GrandTotalSDG = (F.GrandTotalDollar) * Convert.ToDouble(ExchangeRate);

            /*-------------------------------------SDG------------------------------------------------------------*/

            double SDGRound = Convert.ToDouble(Math.Round(F.GrandTotalSDG, 2));

            ToWord toWord = new ToWord(Convert.ToDecimal(SDGRound), currencies[Convert.ToInt32(0)]);

            F.GrandTotalSDGTxt = toWord.ConvertToArabic().ToString();

            /*--------------------------------------Dollar-----------------------------------------------------------*/

            double DollarRound = Convert.ToDouble(Math.Round(F.GrandTotalDollar, 2));
            ToWord toWord1 = new ToWord(Convert.ToDecimal(DollarRound), currencies[Convert.ToInt32(1)]);

            F.GrandTotalDollarTxt = toWord1.ConvertToArabic().ToString();

            /*************************************************************************************/
            ViewBag.HeaderDate = Convert.ToDateTime(DateFrom).ToString("MMM-yyyy");

            int CurrencyType = Convert.ToInt32(db.HandlingAgentsCompanies.Find(HandlingCompanyId).CurrencyId);

            if (CurrencyType == 1)
            {
                ViewBag.Header = "Graf-" + Name + "-" + DateTime.Now.ToShortDateString();
                return View(F);
            }
            else
            {
                ViewBag.Header = "Graf-" + Name + "-" + DateTime.Now.ToShortDateString();
                return View("PrintDataGHGrafDollar", F);
            }

        }
        
        public ActionResult PrintDataFuelGrafDollar()
        {

            return View();
        }

        [Authorize(Roles = "Revenues,Super-Admin, Account, Account-Admin")]
        public ActionResult PrintDataFuelGraf(int? FuelCompanyId, DateTime? DateFrom, DateTime? DateTo)
        {
            var id = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(id);
            RoleController role = new RoleController();

            int GetCompanyId = role.GetCompanyId(id);
            var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);

            string RevenuesManager = CompanyInfo.RevenuesManager;
            ViewBag.RevenuesManager = RevenuesManager;

            string DateNow = System.DateTime.Now.ToString("yyyy/MM/dd") + "م";
            ViewBag.DateNow = DateNow;

            string Serial = Guid.NewGuid().ToString();

            string UserName = CommonUsed.GetUserName(id);

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
            else
            {
                //Month = Convert.ToDateTime(currentDate).Month;
                //Year = Convert.ToDateTime(currentDate).Year;
                return RedirectToAction("Error");
            }
            ExchangeRate = CommonUsed.getExchangeRateByLandingDate(Month, Year);
            //string DemandMouth = db.MouthWorkOns.Where(x => x.Id == Month).FirstOrDefault().Mouth;
            //string DemandMouth = Convert.ToDateTime(DateFrom).ToString("MMMM");
            string DemandMouth = Convert.ToDateTime(DateFrom)
                    .ToString("MMMM", new CultureInfo("ar-EG"));

            string DemandDate = this.GetDemandDate(DateFrom.Value, DateTo.Value);

           

            string ArName = "غير محدد";
            string Name = "غير محدد";

            string SearchDateFrom = "غير محدد";
            string SearchDateTo = "غير محدد";

            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date).Where(x => x.OfficerData.TowerData.CompanyInfoId == companyId); ;


            if (FuelCompanyId > 0)
            {
                Revenue = Revenue.Where(x => x.OfficerData.FuelCompanyId == FuelCompanyId);
                ArName = db.FuelCompanies.Find(FuelCompanyId).NameAr;
                Name = db.FuelCompanies.Find(FuelCompanyId).NameEn;
            }

            ViewBag.ArName = ArName;
            if (DateFrom != null && DateTo != null)
            {
                Revenue = Revenue.Where(t => DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) >= DbFunctions.TruncateTime(DateFrom) && DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) <= DbFunctions.TruncateTime(DateTo));
                SearchDateFrom = CommonUsed.ToDisplayDate(DateFrom).ToString();
                SearchDateTo = CommonUsed.ToDisplayDate(DateTo).ToString();

                ViewBag.SearchDateFrom = SearchDateFrom;
                ViewBag.SearchDateTo = SearchDateTo;

            }

            /***********************************************************************************/

            PrintFinalInvoiceViewModel F = new PrintFinalInvoiceViewModel();

            F.Name = db.FuelCompanies.Find(FuelCompanyId).NameEn;

            F.FreightFees = Convert.ToDouble(Revenue.Sum(x => x.FuelFees));

            F.TaxDollar = F.FreightFees * 0.05;

            F.GrandTotalDollar = (F.FreightFees + F.TaxDollar);
            F.GrandTotalSDG = (F.GrandTotalDollar) * Convert.ToDouble(ExchangeRate);

            ///-------------------------------------SDG------------------------------------------------------------
            double SDGRound = Convert.ToDouble(Math.Round(F.GrandTotalSDG, 2));

            ToWord toWord = new ToWord(Convert.ToDecimal(SDGRound), currencies[Convert.ToInt32(0)]);

            F.GrandTotalSDGTxt = toWord.ConvertToArabic().ToString();

            ///--------------------------------------Dollar-----------------------------------------------------------

            double DollarRound = Convert.ToDouble(Math.Round(F.GrandTotalDollar, 2));
            ToWord toWord1 = new ToWord(Convert.ToDecimal(DollarRound), currencies[Convert.ToInt32(1)]);


            F.GrandTotalDollarTxt = toWord1.ConvertToArabic().ToString();


            ////*************************************************************************************
            ViewBag.HeaderDate = Convert.ToDateTime(DateFrom).ToString("MMM-yyyy");
            ViewBag.DemandDate = DemandDate;
            int CurrencyType = Convert.ToInt32(db.FuelCompanies.Find(FuelCompanyId).CurrencyId);

            if (CurrencyType == 1)
            {
                ViewBag.Header = "Graf-" + Name + "-" + DateTime.Now.ToShortDateString();
                return View(F);
            }
            else
            {
                ViewBag.Header = "Graf-" + Name + "-" + DateTime.Now.ToShortDateString();
                return View("PrintDataFuelGrafDollar", F);
            }

        }

        public string GetDemandDate(DateTime dateFrom, DateTime dateTo)
        {
            int year = dateFrom.Year;
            string monthName = dateFrom.ToString("MMMM", new CultureInfo("ar-EG"));

            // إذا الشهر أو السنة مختلفين
            if (dateFrom.Month != dateTo.Month || dateFrom.Year != dateTo.Year)
            {
                return $"مطالبة من ({dateFrom:dd/MM/yyyy} إلى {dateTo:dd/MM/yyyy})";
            }

            int lastDay = DateTime.DaysInMonth(dateFrom.Year, dateFrom.Month);

            // شهر كامل
            if (dateFrom.Day == 1 && dateTo.Day == lastDay)
                return $"مطالبة شهر {monthName} {year}م";

            int fromDay = dateFrom.Day;
            int toDay = dateTo.Day;

            if (fromDay == 1 && toDay == 7)
                return $"مطالبة الأسبوع الأول من شهر {monthName} {year}م";

            if (fromDay == 8 && toDay == 14)
                return $"مطالبة الأسبوع الثاني من شهر {monthName} {year}م";

            if (fromDay == 15 && toDay == 21)
                return $"مطالبة الأسبوع الثالث من شهر {monthName} {year}م";

            if (fromDay == 22 && toDay == lastDay)
                return $"مطالبة الأسبوع الرابع من شهر {monthName} {year}م";

            return $"مطالبة من ({fromDay}-{toDay}) من شهر {monthName} {year}م";
        }

        [Authorize(Roles = "Revenues,Super-Admin, Account, Account-Admin")]
        public ActionResult PrintDataHandlingComp(int? HandlingCompanyId, DateTime? DateFrom, DateTime? DateTo)
        {
            var id = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(id);
            RoleController role = new RoleController();
            int GetCompanyId = role.GetCompanyId(id);
            var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);

            string UserName = CommonUsed.GetUserName(id);

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
            //else
            //{
            //    return RedirectToAction("Error");
            //}

            ExchangeRate = CommonUsed.getExchangeRateByLandingDate(Month, Year);
            ViewBag.ExchangeRate = ExchangeRate;

            string Name = "غير محدد";
            string SearchDateFrom = "غير محدد";
            string SearchDateTo = "غير محدد";

            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date).Where(x => x.GroundHandlingFees > 0
             && x.OfficerData.TowerData.CompanyInfoId == companyId);

            if (HandlingCompanyId > 0)
            {
                Revenue = Revenue.Where(x => x.OfficerData.HandlingAgentId == HandlingCompanyId);
                Name = db.HandlingAgentsCompanies.Find(HandlingCompanyId).NameEn;

                ViewBag.Name = Name;
            }

            if (DateFrom != null && DateTo != null)
            {
                Revenue = Revenue.Where(t => DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) >= DbFunctions.TruncateTime(DateFrom) && DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) <= DbFunctions.TruncateTime(DateTo));
                SearchDateFrom = CommonUsed.ToDisplayDate(DateFrom).ToString();
                SearchDateTo = CommonUsed.ToDisplayDate(DateTo).ToString();

                ViewBag.DateFrom = SearchDateFrom;
                ViewBag.DateTo = SearchDateTo;
            }

            //if (Revenue.Count() <= 0)
            //{
            //    return RedirectToAction("Error");
            //}

            /**************************************************************************************************************/

            var RevenueList = new List<PrintGroundHandlingViewModel>();

            int N = 0;
            foreach (var item in Revenue)
            {
                PrintGroundHandlingViewModel F = new PrintGroundHandlingViewModel();

                F.Id = item.Id;

                N = N + 1;
                F.N = N;

                F.Name = item?.OfficerData.HandlingAgentsCompany.NameEn;

                F.FlightNo = item?.OfficerData.TowerData.AirLinesFlightTrip.FlightNo;

                F.TakeOffDate = CommonUsed.ToDisplayDate(item?.OfficerData.TowerData.TakeOffDate);

                F.AirlineCode = item?.OfficerData.TowerData.AirLine.Code;

                F.AirCraftReg = item?.OfficerData.TowerData.AircraftRegistration.Registration;
                F.AirCraftType = item?.OfficerData.TowerData.AircraftRegistration.AircraftType.Type;
                F.AirCraftWieght = item?.OfficerData.TowerData.AircraftRegistration.MaxTakoffWieght.ToString();

                F.GroundHandlingFees = Convert.ToDecimal(item?.GroundHandlingFees);

                F.GroundHandlingFeeSDG = Convert.ToDecimal(item?.GroundHandlingFees) * Convert.ToDecimal(ExchangeRate);

                //if (F.GroundHandlingFeeSDG == 0)
                //{
                //    F.Fees = 0;
                //}
                //else
                //{
                //    F.Fees = (F.GroundHandlingFeeSDG / F.FirstClassCount);
                //}

                RevenueList.Add(F);
            }

            /**************************************************************************************************************/
            //double SumFirstClassCount = Convert.ToDouble(RevenueList.Sum(x => x.FirstClassCount));
            //ViewBag.SumFirstClassCount = SumFirstClassCount;

            double SumGroundHandlingFees = Convert.ToDouble(RevenueList.Sum(x => x.GroundHandlingFees));
            ViewBag.SumGroundHandlingFees = SumGroundHandlingFees;

            double SumGroundHandlingFeesSDG = Convert.ToDouble(RevenueList.Sum(x => x.GroundHandlingFeeSDG));
            ViewBag.SumGroundHandlingFeesSDG = SumGroundHandlingFeesSDG;

            double TaxDollar = SumGroundHandlingFees * 0.05;
            ViewBag.TaxDollar = TaxDollar;

            double TaxSDG = SumGroundHandlingFeesSDG * 0.05;
            ViewBag.TaxSDG = TaxSDG;

            // Grand Total
            double GrandTotalDollar = SumGroundHandlingFees + TaxDollar;
            ViewBag.GrandTotalDollar = GrandTotalDollar;

            double GrandTotalSDG = SumGroundHandlingFeesSDG + TaxSDG;
            ViewBag.GrandTotalSDG = GrandTotalSDG;

            ViewBag.Header = "Ground Handling Fees-" + Name + "-" + DateTime.Now.ToShortDateString();
            return View(RevenueList);
        }

        [Authorize(Roles = "Revenues,Super-Admin, Account, Account-Admin")]
        public ActionResult PrintDataFuelFees(int? FuelCompanyId, DateTime? DateFrom, DateTime? DateTo)
        {
            var id = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(id);
            RoleController role = new RoleController();
            int GetCompanyId = role.GetCompanyId(id);
            var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);

            string UserName = CommonUsed.GetUserName(id);

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

            ExchangeRate = CommonUsed.getExchangeRateByLandingDate(Month, Year);
            ViewBag.ExchangeRate = ExchangeRate;

            string Name = "غير محدد";
            string SearchDateFrom = "غير محدد";
            string SearchDateTo = "غير محدد";

            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date).Where(x => x.FuelFees > 0
            && x.OfficerData.TowerData.CompanyInfoId == companyId);

            if (FuelCompanyId > 0)
            {
                Revenue = Revenue.Where(x => x.OfficerData.FuelCompanyId == FuelCompanyId);
                Name = db.FuelCompanies.Find(FuelCompanyId).NameEn;

                ViewBag.Name = Name;
            }

            if (DateFrom != null && DateTo != null)
            {
                Revenue = Revenue.Where(t => DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) >= DbFunctions.TruncateTime(DateFrom) && DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) <= DbFunctions.TruncateTime(DateTo));
                SearchDateFrom = CommonUsed.ToDisplayDate(DateFrom).ToString();
                SearchDateTo = CommonUsed.ToDisplayDate(DateTo).ToString();

                ViewBag.DateFrom = SearchDateFrom;
                ViewBag.DateTo = SearchDateTo;
            }

            //if (Revenue.Count() <= 0)
            //{
            //    return RedirectToAction("Error");
            //}

            /**************************************************************************************************************/

            var RevenueList = new List<PrintGroundHandlingViewModel>();

            int N = 0;
            foreach (var item in Revenue)
            {
                PrintGroundHandlingViewModel F = new PrintGroundHandlingViewModel();

                F.Id = item.Id;

                N = N + 1;
                F.N = N;

                F.Name = item?.OfficerData.TowerData.AirLine.Name;

                F.FlightNo = item?.OfficerData.TowerData.AirLinesFlightTrip.FlightNo;

                F.TakeOffDate = CommonUsed.ToDisplayDate(item?.OfficerData.TowerData.TakeOffDate);

                F.AirlineCode = item?.OfficerData.TowerData.AirLine.Code;

                F.AirCraftReg = item?.OfficerData.TowerData.AircraftRegistration.Registration;
                F.AirCraftType = item?.OfficerData.TowerData.AircraftRegistration.AircraftType.Type;
                F.AirCraftWieght = item?.OfficerData.FuelLiter.ToString();

                F.GroundHandlingFees = Convert.ToDecimal(item?.FuelFees);

                F.GroundHandlingFeeSDG = Convert.ToDecimal(item?.FuelFees) * Convert.ToDecimal(ExchangeRate);

                //if (F.GroundHandlingFeeSDG == 0)
                //{
                //    F.Fees = 0;
                //}
                //else
                //{
                //    F.Fees = (F.GroundHandlingFeeSDG / F.FirstClassCount);
                //}

                RevenueList.Add(F);
            }

            /**************************************************************************************************************/
            //double SumFirstClassCount = Convert.ToDouble(RevenueList.Sum(x => x.FirstClassCount));
            //ViewBag.SumFirstClassCount = SumFirstClassCount;

            double SumFuelFees = Convert.ToDouble(RevenueList.Sum(x => x.GroundHandlingFees));
            ViewBag.SumFuelFees = SumFuelFees;

            double SumFuelFeesSDG = Convert.ToDouble(RevenueList.Sum(x => x.GroundHandlingFeeSDG));
            ViewBag.SumFuelFeesSDG = SumFuelFeesSDG;

            double TaxDollar = SumFuelFees * 0.05;
            ViewBag.TaxDollar = TaxDollar;

            double TaxSDG = SumFuelFeesSDG * 0.05;
            ViewBag.TaxSDG = TaxSDG;

            // Grand Total
            double GrandTotalDollar = SumFuelFees + TaxDollar;
            ViewBag.GrandTotalDollar = GrandTotalDollar;

            double GrandTotalSDG = SumFuelFeesSDG + TaxSDG;
            ViewBag.GrandTotalSDG = GrandTotalSDG;

            ViewBag.Header = "Fuel Fees-" + Name + "-" + DateTime.Now.ToShortDateString();
            return View(RevenueList);
        }
        
        [Authorize(Roles = "Revenues,Super-Admin,Landing,Mail,Freight,Landing-Admin, Account, Account-Admin")]
        public ActionResult PrintDataTripInfo(int Id)
        {
            var id = User.Identity.GetUserId();
            string UserName = CommonUsed.GetUserName(id);

            ViewBag.UserName = UserName;

            PrintTripInfoViewModel model = new PrintTripInfoViewModel();

            if (db.TowerDatas.Any(x => x.Id == Id))
            {
                TowerData towerData = db.TowerDatas.Find(Id);

                model.Id = Id;
                model.Airline = towerData?.AirLine.Code + "/ " + towerData?.AirLine.Name;
                model.FlightDate = CommonUsed.ToDisplayDate(towerData.TakeOffDate);
                model.LandingDate = CommonUsed.ToDisplayDate(towerData.LandingDate);
                model.ATD = towerData.ATD.ToString().Substring(0, 5);
                model.ATA = towerData.ATA.ToString().Substring(0, 5);
                model.FlightNo = towerData.AirLinesFlightTrip.FlightNo;
                model.MaxTakoffWieght = towerData.AircraftRegistration.MaxTakoffWieght.ToString();
                model.RegNo = towerData.AircraftRegistration.Registration;
                model.Type = towerData.AircraftRegistration.AircraftType.Type;
                model.From = towerData.AirPort.NameEn + " / " + towerData.AirPort.Code;
                model.To = towerData.AirPort1.NameEn + " / " + towerData.AirPort.Code;

                model.InterPax = towerData.OfficerDatas.FirstOrDefault(x => x.TowerDataId == towerData.Id).Pax > 0 ? towerData.OfficerDatas.FirstOrDefault(x => x.TowerDataId == towerData.Id).Pax.ToString() : "Not Found";
                //model.DomesticPax = towerData.OfficerDatas.FirstOrDefault(x => x.TowerDataId == towerData.Id).Pax.ToString();
                model.TRNPax = towerData.OfficerDatas.FirstOrDefault(x => x.TowerDataId == towerData.Id).TRNYCUPPS > 0 ? towerData.OfficerDatas.FirstOrDefault(x => x.TowerDataId == towerData.Id).TRNYCUPPS.ToString() : "Not Found";

                model.AmbulanceCarCount = towerData.OfficerDatas.FirstOrDefault(x => x.TowerDataId == towerData.Id).AmbulanceCarCount > 0 ? towerData.OfficerDatas.FirstOrDefault(x => x.TowerDataId == towerData.Id).AmbulanceCarCount.ToString() : "Not Found";
                model.FireTruckCount = towerData.OfficerDatas.FirstOrDefault(x => x.TowerDataId == towerData.Id).FireFightingCarCount > 0 ? towerData.OfficerDatas.FirstOrDefault(x => x.TowerDataId == towerData.Id).FireFightingCarCount.ToString() : "Not Found";

                model.MailExp = towerData.OfficerDatas.FirstOrDefault(x => x.TowerDataId == towerData.Id).NormalMailLoadingExp > 0 | towerData.OfficerDatas.FirstOrDefault(x => x.TowerDataId == towerData.Id).RapidMailLoadingExp > 0 ? (towerData.OfficerDatas.FirstOrDefault(x => x.TowerDataId == towerData.Id).NormalMailLoadingExp + towerData.OfficerDatas.FirstOrDefault(x => x.TowerDataId == towerData.Id).RapidMailLoadingExp).ToString() : "Not Found";
                model.MailImp = towerData.OfficerDatas.FirstOrDefault(x => x.TowerDataId == towerData.Id).NormalMailLoadingImp > 0 | towerData.OfficerDatas.FirstOrDefault(x => x.TowerDataId == towerData.Id).RapidMailLoadingImp > 0 ? (towerData.OfficerDatas.FirstOrDefault(x => x.TowerDataId == towerData.Id).NormalMailLoadingImp  + towerData.OfficerDatas.FirstOrDefault(x => x.TowerDataId == towerData.Id).RapidMailLoadingImp).ToString() : "Not Found";
                model.FrieghtExp = towerData.OfficerDatas.FirstOrDefault(x => x.TowerDataId == towerData.Id).FreightLoadingExp > 0 ? towerData.OfficerDatas.FirstOrDefault(x => x.TowerDataId == towerData.Id).FreightLoadingExp.ToString() : "Not Found";
                model.FrieghtImp = towerData.OfficerDatas.FirstOrDefault(x => x.TowerDataId == towerData.Id).FreightLoadingImp > 0 ? towerData.OfficerDatas.FirstOrDefault(x => x.TowerDataId == towerData.Id).FreightLoadingImp.ToString() : "Not Found";
                
                model.GroundHandlingComp = towerData.OfficerDatas.FirstOrDefault(x => x.TowerDataId == towerData.Id).HandlingAgentId > 0 ? towerData.OfficerDatas.FirstOrDefault(x => x.TowerDataId == towerData.Id).HandlingAgentsCompany.NameEn : "Not Found";
                model.FuelComp = towerData.OfficerDatas.FirstOrDefault(x => x.TowerDataId == towerData.Id).FuelCompanyId > 0 ? towerData.OfficerDatas.FirstOrDefault(x => x.TowerDataId == towerData.Id).FuelCompany.NameEn : "Not Found";
                model.FuelLiters = towerData.OfficerDatas.FirstOrDefault(x => x.TowerDataId == towerData.Id).FuelLiter > 0 ? towerData.OfficerDatas.FirstOrDefault(x => x.TowerDataId == towerData.Id).FuelLiter.ToString() : "Not Found";
            }
            
            return View(model);
        }
        
        public ActionResult GetRepOld(int Year)
        {
            var data = db.TowerDatas.Where(x => x.LandingDate.Value.Year == Year && x.TripTypeId == 1).ToList();

            var dataList = new List<dynamic>();

            foreach(var item in data)
            {
                var obj = new
                {
                    Month = item.LandingDate.Value.Month,
                    Id = item.Id,
                    Pax = item.OfficerDatas.FirstOrDefault(x => x.TowerDataId == item.Id).Pax,
                    FreightIn = item.OfficerDatas.FirstOrDefault(x => x.TowerDataId == item.Id).FreightLoadingImp,
                    FreightOut = item.OfficerDatas.FirstOrDefault(x => x.TowerDataId == item.Id).FreightLoadingExp
                };

                dataList.Add(obj);
            }

            return Json(dataList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRep(int year, List<int?> TripTypeIdList)
        {
            // جلب البيانات وجمعها حسب الشهر
            var data = db.TowerDatas
                .Where(x => x.LandingDate.HasValue && x.LandingDate.Value.Year == year && TripTypeIdList.Contains(x.TripTypeId))
                .SelectMany(item => item.OfficerDatas, (towerData, officerData) => new
                {
                    Month = towerData.LandingDate.Value.Month,
                    Pax = officerData.Pax ?? 0,
                    FreightIn = officerData.FreightLoadingImp ?? 0,
                    FreightOut = officerData.FreightLoadingExp ?? 0
                })
                .GroupBy(x => x.Month)
                .Select(group => new
                {
                    Month = group.Key,
                    TotalPax = group.Sum(x => x.Pax),
                    TotalFreightIn = group.Sum(x => x.FreightIn),
                    TotalFreightOut = group.Sum(x => x.FreightOut)
                })
                .OrderBy(x => x.Month)
                .ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PrintDataGrafSingle(int? TowerDataId, decimal ExcRate)
        {
            var id = User.Identity.GetUserId();
            RoleController role = new RoleController();
            TowerData towerData = db.TowerDatas.Find(TowerDataId);



            //int AirLineAccId = 0;
            int GetCompanyId = role.GetCompanyId(id);
            var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);

            string RevenuesManager = CompanyInfo.RevenuesManager;
            ViewBag.RevenuesManager = RevenuesManager;

            string DateNow = DateTime.Now.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture) + "م";
            ViewBag.DateNow = DateNow;

            string Serial = Guid.NewGuid().ToString();

            string UserName = CommonUsed.GetUserName(id);

           

            int CurrencyType = Convert.ToInt32(db.AirLines.Where(x => x.Id == towerData.AirLineId).Single().CurrencyType);

            //اذا عملة الدفع هي الجنيه السوداني
            if (CurrencyType == 1)
            {
                if (ExcRate <= 0)
                {
                    ExchangeRate = CommonUsed.getExchangeRateByLandingDate(Month, Year);
                }
                else
                {
                    ExchangeRate = ExcRate;
                }
            }
            else
            {
                ExchangeRate = 0;
            }


            string DemandDate = "مطالبة يوم " + Convert.ToDateTime(towerData.Date).ToString("yyyy/MM/dd", CultureInfo.InvariantCulture) + "م" + "  للرحلة رقم" + "(" + towerData.AirLinesFlightTrip.FlightNo + ")";
            ViewBag.DemandDate = DemandDate;

            string ArName = "غير محدد";
            string Name = "غير محدد";

          
            Revenue Revenue;
            OfficerData officerData = db.OfficerDatas.Where(x => x.TowerDataId == TowerDataId).FirstOrDefault();

            Revenue = db.Revenues.Where(x => x.OfficerDataId == officerData.Id).FirstOrDefault();
            
           ArName = db.AirLines.Where(x => x.Id == towerData.AirLineId).Single().ArName;
            ViewBag.ArName = ArName;

            //if (Revenue.Count() <= 0)
            //{
            //    return RedirectToAction("Error");
            //}
            ///***********************************************************************************
            //  var RevenueList = new List<PrintFinalInvoiceViewModel>();

            PrintFinalInvoiceViewModel F = new PrintFinalInvoiceViewModel();

            var airLine = db.AirLines.Single(x => x.Id == towerData.AirLineId);

            F.Name = airLine.Code + "/ " + airLine.Name;

            F.LandingFees = Convert.ToDouble(Revenue.LandingFees);
            F.ParkingFees = Convert.ToDouble(Revenue.ParkingFees);
            F.NightOperationFees = Convert.ToDouble(Revenue.NightOperationFees);
            F.NavigationFees = Convert.ToDouble(Revenue.NavigationFees);

            F.SecurityFees = Convert.ToDouble(Revenue.SecurityFees);
            F.FirstClassFees = Convert.ToDouble(Revenue.FirstClassFees);

            F.InterCUPPSFees = Convert.ToDouble(Revenue.InterCUPPSFees);
            F.DomeCUPPSFees = Convert.ToDouble(Revenue.DomeCUPPSFees);

            F.InterNYFees = Convert.ToDouble(Revenue.InterNYFees);
           // F.DomeNYFees = Convert.ToDouble(Revenue.DomeNYFees) / Convert.ToDouble(ExchangeRate);
            F.DomeNYFees = Convert.ToDouble(Revenue.DomeNYFees);


            F.PushBackFees = Convert.ToDouble(Revenue.PushBackFees);

            F.ServiceCars =
                Convert.ToDouble(Revenue.AmbolanceFees) +
                Convert.ToDouble(Revenue.FireFightingFees);

            F.NoiseFees = Convert.ToDouble(Revenue.NoiseFees);
            F.ExpMailFees = Convert.ToDouble(Revenue.ExpMailFees);
            F.ImpMailFees = Convert.ToDouble(Revenue.ImpMailFees);
            F.FreightFees = Convert.ToDouble(Revenue.FreightFees);


            F.SubTotalDollar = F.LandingFees + F.ParkingFees + F.NightOperationFees + F.NavigationFees + F.SecurityFees +
                F.FirstClassFees + F.FreightFees + F.ServiceCars + F.InterCUPPSFees + F.DomeCUPPSFees + F.InterNYFees +
                F.DomeNYFees + F.PushBackFees + F.ExpMailFees + F.ImpMailFees // + F.OpenAireportOvertimeFees
            + F.NoiseFees;


            F.TaxDollar = ((F.LandingFees + F.ParkingFees + F.NightOperationFees + F.NavigationFees + F.SecurityFees +
            F.FirstClassFees + F.FreightFees + F.ServiceCars + F.InterCUPPSFees + F.DomeCUPPSFees +
            F.PushBackFees + F.ExpMailFees + F.ImpMailFees // + F.OpenAireportOvertimeFees
            + F.NoiseFees) * 0.05);





            F.GrandTotalDollar = (F.SubTotalDollar + F.TaxDollar);
            F.GrandTotalSDG = (F.GrandTotalDollar) * Convert.ToDouble(ExchangeRate);




            ///-------------------------------------SDG------------------------------------------------------------
            double SDGRound = Convert.ToDouble(Math.Round(F.GrandTotalSDG, 2));

            ToWord toWord = new ToWord(Convert.ToDecimal(SDGRound), currencies[Convert.ToInt32(0)]);

            F.GrandTotalSDGTxt = toWord.ConvertToArabic().ToString();

            ///--------------------------------------Dollar-----------------------------------------------------------

            double DollarRound = Convert.ToDouble(Math.Round(F.GrandTotalDollar, 2));
            ToWord toWord1 = new ToWord(Convert.ToDecimal(DollarRound), currencies[Convert.ToInt32(1)]);


            F.GrandTotalDollarTxt = toWord1.ConvertToArabic().ToString();


            ////*************************************************************************************
            ViewBag.HeaderDate = Convert.ToDateTime(DateTime.Now).ToString("MMM-yyyy");

            if (CurrencyType == 1)
            {
                ViewBag.Header = "Graf-" + Name + "-" + DateTime.Now.ToShortDateString();
                return View("PrintDataGrafSingle", F);
            }
            else
            {
                ViewBag.Header = "Graf-" + Name + "-" + DateTime.Now.ToShortDateString();
                return View("PrintDataGrafDollar", F);
            }
        }
        
        [Authorize(Roles = "Revenues,Super-Admin, Account, Account-Admin")]
        public ActionResult PrintDataNYGraf(int? AirLineId, DateTime? DateFrom, DateTime? DateTo, decimal ExcRate)
        {
            var id = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(id);
            RoleController role = new RoleController();

            int AirLineAccId = 0;
            int GetCompanyId = role.GetCompanyId(id);
            var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);

            string RevenuesManager = CompanyInfo.RevenuesManager;
            ViewBag.RevenuesManager = RevenuesManager;
            ViewBag.companyId = GetCompanyId;


            string DateNow = System.DateTime.Now.ToString("yyyy/MM/dd") + "م";
            ViewBag.DateNow = DateNow;

            string Serial = Guid.NewGuid().ToString();

            string UserName = CommonUsed.GetUserName(id);

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
            else
            {
                return RedirectToAction("Error");
            }

            int CurrencyType = Convert.ToInt32(db.AirLines.Where(x => x.Id == AirLineId).Single().CurrencyType);

            //اذا عملة الدفع هي الجنيه السوداني
            if (CurrencyType == 1)
            {
                if (ExcRate <= 0)
                {
                    ExchangeRate = CommonUsed.getExchangeRateByLandingDate(Month, Year);
                }
                else
                {
                    ExchangeRate = ExcRate;
                }
            }
            else
            {
                ExchangeRate = 0;
            }

            string DemandMouth = Convert.ToDateTime(DateFrom).ToString("MMMM", new CultureInfo("ar-EG"));

            string DemandDate = "مطالبة رسوم ال (NY)عن شهر " + DemandMouth + " " + Year + "م";
            ViewBag.DemandDate = DemandDate;

            string ArName = "غير محدد";
            string Name = "غير محدد";

            string SearchDateFrom = "غير محدد";
            string SearchDateTo = "غير محدد";

            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues
                                        .OrderBy(x => x.OfficerData.TowerData.Date)
                                        .Where(x => x.OfficerData.TowerData.CompanyInfoId == companyId);

            if (AirLineId > 0)
            {
                Revenue = Revenue.Where(x => x.OfficerData.TowerData.AirLineId == AirLineId);
                ArName = db.AirLines.Where(x => x.Id == AirLineId).FirstOrDefault().ArName;
                AirLineAccId = (int)db.AirLines.Where(x => x.Id == AirLineId).FirstOrDefault().AccId;

                Name = db.AirLines.Where(x => x.Id == AirLineId).FirstOrDefault().Name;
            }

            ViewBag.ArName = ArName;
            if (DateFrom != null && DateTo != null)
            {
                Revenue = Revenue.Where(t => DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) >= DbFunctions.TruncateTime(DateFrom) && 
                                                                     DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) <= DbFunctions.TruncateTime(DateTo));
                SearchDateFrom = CommonUsed.ToDisplayDate(DateFrom).ToString();
                SearchDateTo = CommonUsed.ToDisplayDate(DateTo).ToString();
            }

            PrintFinalInvoiceViewModel F = new PrintFinalInvoiceViewModel();

            F.Name = db.AirLines.Where(x => x.Id == AirLineId).FirstOrDefault().Code + "/ " + db.AirLines.Where(x => x.Id == AirLineId).FirstOrDefault().Name;
            
            F.InterNYFees = Convert.ToDouble(Revenue.Sum(x => x.InterNYFees));
            F.DomeNYFees = Convert.ToDouble(Revenue.Sum(x => x.DomeNYFees));

            F.GrandTotalDollar = (F.InterNYFees + F.DomeNYFees);
            F.GrandTotalSDG = (F.GrandTotalDollar) * Convert.ToDouble(ExchangeRate);

            ///-------------------------------------SDG------------------------------------------------------------
            double SDGRound = Convert.ToDouble(Math.Round(F.GrandTotalSDG, 2));

            ToWord toWord = new ToWord(Convert.ToDecimal(SDGRound), currencies[Convert.ToInt32(0)]);

            F.GrandTotalSDGTxt = toWord.ConvertToArabic().ToString();

            ///--------------------------------------Dollar-----------------------------------------------------------

            double DollarRound = Convert.ToDouble(Math.Round(F.GrandTotalDollar, 2));
            ToWord toWord1 = new ToWord(Convert.ToDecimal(DollarRound), currencies[Convert.ToInt32(1)]);

            F.GrandTotalDollarTxt = toWord1.ConvertToArabic().ToString();

            ////*************************************************************************************
            ViewBag.HeaderDate = Convert.ToDateTime(DateFrom).ToString("MMM-yyyy");

            if (CurrencyType == 1)
            {
                ViewBag.Header = "Graf-" + Name + "-" + DateTime.Now.ToShortDateString();
                return View(F);
            }
            else
            {
                ViewBag.Header = "Graf-" + Name + "-" + DateTime.Now.ToShortDateString();
                return View("PrintDataGrafDollar", F);
            }
        }

    }
}