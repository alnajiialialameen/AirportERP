using AirPortERP.Extenstion;
using AirPortERP.Models;
using AirPortERP.ViewModels;
using CrystalDecisions.CrystalReports.Engine;
using Microsoft.AspNet.Identity;
using NumberToWord;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
        //private int companyInfoId = 10; // companyInfoId
        //  private String userId = "111"; // createdById + updatedById
        private DateTime currentDate = DateTime.UtcNow.AddHours(2);


        
        public AdvencePrintBasicController(){
            
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Sudan));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Dollar));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Syria));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.UAE));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.SaudiArabia));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Tunisia));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Gold));
            
            
            
            }
        
        [Authorize(Roles = "Revenues,Super-Admin,Account, Account-Admin")]
        public ActionResult PrintDataFinalInvoice(int? AirLineId, DateTime? DateFrom, DateTime? DateTo, decimal ExcRate)
        {
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
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date);

            if (AirLineId > 0)
            {
                Revenue = Revenue.Where(x => x.OfficerData.TowerData.AirLineId == AirLineId);
                SearchAirLineId = db.AirLines.Where(x => x.Id == AirLineId).Single().Name;
                Name = SearchAirLineId;
            }

            if (DateFrom != null && DateTo != null)
            {
                Revenue = Revenue.Where(t => DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) >= DbFunctions.TruncateTime(DateFrom) && DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) <= DbFunctions.TruncateTime(DateTo));
                SearchDateFrom = CommonUsed.CheckDate(DateFrom.ToString()).ToString();
                SearchDateTo = CommonUsed.CheckDate(DateTo.ToString()).ToString();
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

            F.InterCUPPSFees = Convert.ToDouble(Revenue.Where(x=>x.OfficerData.TowerData.FlightTypeD==1).Sum(x => x.InterCUPPSFees));
            //F.InterCUPPSFees = Convert.ToInt32(Revenue.Where(x=> x.OfficerData.TowerData.FlightTypeD == 1).Sum(x => x.OfficerData.Pax + x.OfficerData.TRNYCUPPS)) * 2.5;
            F.DomeCUPPSFees = Convert.ToDouble(Revenue.Sum(x => x.DomeCUPPSFees));

            //   F.InterNYFees = Convert.ToDouble(Revenue.Where(x => x.OfficerData.TowerData.FlightTypeD == 1).Sum(x => x.InterNYFees));
            //   F.DomeNYFees = Convert.ToDouble(Revenue.Where(x => x.OfficerData.TowerData.FlightTypeD == 2).Sum(x => x.DomeNYFees) / ExchangeRate);

            F.InterNYFees = Convert.ToDouble(Revenue.Sum(x => x.InterNYFees));
            if (ExchangeRate > 0)
            {
                F.DomeNYFees = Convert.ToDouble(Revenue.Sum(x => x.DomeNYFees) / ExchangeRate);
                F.DomeNYFees = Convert.ToDouble(Revenue.Where(x => x.OfficerData.TowerData.FlightTypeD == 2).Sum(x => x.DomeNYFees) / ExchangeRate);
            }
            else
            {
                F.DomeNYFees = Convert.ToDouble(Revenue.Sum(x => x.DomeNYFees));
                F.DomeNYFees = Convert.ToDouble(Revenue.Where(x => x.OfficerData.TowerData.FlightTypeD == 2).Sum(x => x.DomeNYFees));
            }


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
                    InvoiceNo = Convert.ToInt32(db.FinalInvoiceNumbers.SingleOrDefault(x => x.AirLineId == AirLineId && x.Month == Month && x.Year == Year).InvoiceNo);
                    
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

        public ActionResult PrintDataLandingDom() {
            return View();
        }
        
        [Authorize(Roles = "Revenues,Super-Admin,Landing,Mail,Freight,Landing-Admin")]
        public ActionResult PrintDataLanding(int? AirLineId, DateTime? DateFrom, DateTime? DateTo, int? FlightTypeId)
        {
            var id = User.Identity.GetUserId();
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
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date);

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
                SearchDateFrom = CommonUsed.CheckDate(DateFrom.ToString()).ToString();
                SearchDateTo = CommonUsed.CheckDate(DateTo.ToString()).ToString();
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
                L.LandingDate = CommonUsed.CheckDate(item?.OfficerData.TowerData.LandingDate.ToString());
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

        [Authorize(Roles = "Revenues,Super-Admin")]
        public ActionResult PrintDataGraf(int? AirLineId, DateTime? DateFrom, DateTime? DateTo, decimal ExcRate)
        {
            var id = User.Identity.GetUserId();
            RoleController role = new RoleController();

            int AirLineAccId = 0;
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





            string DemandMouth = db.MouthWorkOns.Where(x => x.Id == Month).FirstOrDefault().Mouth;
            //  string DemandYear = db.WorkOns.Where(x => x.UserId == id).FirstOrDefault().YearWorkOn.Year.ToString();




            string DemandDate = "مطالبة شهر " + DemandMouth + " " + Year + "م";
            ViewBag.DemandDate = DemandDate;

            string ArName = "غير محدد";
            string Name = "غير محدد";

            string SearchDateFrom = "غير محدد";
            string SearchDateTo = "غير محدد";

           // ReportDocument rd = new ReportDocument();


            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date);


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
                SearchDateFrom = CommonUsed.CheckDate(DateFrom.ToString()).ToString();
                SearchDateTo = CommonUsed.CheckDate(DateTo.ToString()).ToString();
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
            F.DomeNYFees = Convert.ToDouble(Revenue.Sum(x => x.DomeNYFees) / ExchangeRate);

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
        
        [Authorize(Roles = "Revenues,Super-Admin,Car,Ny&&Car")]
        public ActionResult PrintDataServiceCar(int? AirLineId, DateTime? DateFrom, DateTime? DateTo, int? FlightTypeId)
        {
            var id = User.Identity.GetUserId();
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
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date).Where(x => x.AmbolanceFees > 0 || x.FireFightingFees > 0);

            if (AirLineId > 0)
            {
                Revenue = Revenue.Where(x => x.OfficerData.TowerData.AirLineId == AirLineId);
                SearchAirLineId = db.AirLines.Where(x => x.Id == AirLineId).Single().Name;
                AirLineAccId = (int)db.AirLines.Where(x => x.Id == AirLineId).Single().AccId;
               

                ViewBag.Name = SearchAirLineId;
                Name = SearchAirLineId;
            }


            //هنا مفترض نستجلب سعر الصرف من جدول المطالبة وليس من جدول تفاصيل العملات
            if (db.FinancialClaims.Any(x => x.AccIdDebit == AirLineId && (DbFunctions.TruncateTime(x.DateFrom) <= DateFrom & DbFunctions.TruncateTime(x.DateTo) >= DateFrom) |
                 (DbFunctions.TruncateTime(x.DateFrom) <= DateTo & DbFunctions.TruncateTime(x.DateTo) >= DateTo)))
            {
                var obj = db.FinancialClaims.Where(x => (DbFunctions.TruncateTime(x.DateFrom) <= DateFrom & DbFunctions.TruncateTime(x.DateTo) >= DateFrom) |
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
                SearchDateFrom = CommonUsed.CheckDate(DateFrom.ToString()).ToString();
                SearchDateTo = CommonUsed.CheckDate(DateTo.ToString()).ToString();

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
                C.LandingDate = CommonUsed.CheckDate(item?.OfficerData.TowerData.LandingDate.ToString());
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
        
        [Authorize(Roles = "Revenues,Super-Admin,PushPack")]
        public ActionResult PrintPushPack(int? AirLineId, DateTime? DateFrom, DateTime? DateTo)
        {
            var id = User.Identity.GetUserId();
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
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date).Where(x => x.PushBackFees != null && x.PushBackFees > 0);

            if (AirLineId > 0)
            {
                Revenue = Revenue.Where(x => x.OfficerData.TowerData.AirLineId == AirLineId);
                Name = db.AirLines.Where(x => x.Id == AirLineId).Single().Name;
                AirLineAccId = (int)db.AirLines.Where(x => x.Id == AirLineId).Single().AccId;

                ViewBag.Name = Name;
            }


            //هنا مفترض نستجلب سعر الصرف من جدول المطالبة وليس من جدول تفاصيل العملات
            if (db.FinancialClaims.Any(x => x.AccIdDebit == AirLineAccId && (DbFunctions.TruncateTime(x.DateFrom) <= DateFrom & DbFunctions.TruncateTime(x.DateTo) >= DateFrom) |
                 (DbFunctions.TruncateTime(x.DateFrom) <= DateTo & DbFunctions.TruncateTime(x.DateTo) >= DateTo)))
            {
                var obj = db.FinancialClaims.Where(x => (DbFunctions.TruncateTime(x.DateFrom) <= DateFrom & DbFunctions.TruncateTime(x.DateTo) >= DateFrom) |
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
                SearchDateFrom = CommonUsed.CheckDate(DateFrom.ToString()).ToString();
                SearchDateTo = CommonUsed.CheckDate(DateTo.ToString()).ToString();

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
                C.LandingDate = CommonUsed.CheckDate(item?.OfficerData.TowerData.LandingDate.ToString());
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
        
        [Authorize(Roles = "Revenues,Super-Admin,FirstClass")]
        public ActionResult PrintDataFirstClass(int? AirLineId, DateTime? DateFrom, DateTime? DateTo/*, int? FlightTypeId*/)
        {
            var id = User.Identity.GetUserId();
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
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date).Where(x => x.FirstClassFees > 0);
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
            if (db.FinancialClaims.Any(x => x.AccIdDebit == AirLineAccId && (DbFunctions.TruncateTime(x.DateFrom) <= DateFrom & DbFunctions.TruncateTime(x.DateTo) >= DateFrom) |
                 (DbFunctions.TruncateTime(x.DateFrom) <= DateTo & DbFunctions.TruncateTime(x.DateTo) >= DateTo)))
            {
                var obj = db.FinancialClaims.Where(x => (DbFunctions.TruncateTime(x.DateFrom) <= DateFrom & DbFunctions.TruncateTime(x.DateTo) >= DateFrom) |
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
                SearchDateFrom = CommonUsed.CheckDate(DateFrom.ToString()).ToString();
                SearchDateTo = CommonUsed.CheckDate(DateTo.ToString()).ToString();

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
                F.TakeOffDate = CommonUsed.CheckDate(item?.OfficerData.TowerData.TakeOffDate.ToString());

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
        
        [Authorize(Roles = "Revenues,Super-Admin,Cupps,Ny,Ny&&Car")]
        public ActionResult PrintDataNy(int? AirLineId, DateTime? DateFrom, DateTime? DateTo, int? FlightTypeId)
        {
            string TitleDiscount = "";
            int C = 0, AirLineAccId = 0;
            var id = User.Identity.GetUserId();
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
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.TakeOffDate).Where(x => x.OfficerData.Pax > 0);
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
            if (db.FinancialClaims.Any(x => x.AccIdDebit == AirLineAccId && (DbFunctions.TruncateTime(x.DateFrom) <= DateFrom & DbFunctions.TruncateTime(x.DateTo) >= DateFrom) |
                 (DbFunctions.TruncateTime(x.DateFrom) <= DateTo & DbFunctions.TruncateTime(x.DateTo) >= DateTo)))
            {
                var obj = db.FinancialClaims.Where(x => x.AccIdDebit == AirLineAccId && (DbFunctions.TruncateTime(x.DateFrom) <= DateFrom & DbFunctions.TruncateTime(x.DateTo) >= DateFrom) |
                 (DbFunctions.TruncateTime(x.DateFrom) <= DateTo & DbFunctions.TruncateTime(x.DateTo) >= DateTo)).FirstOrDefault();
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
                SearchDateFrom = CommonUsed.CheckDate(DateFrom.ToString()).ToString();
                SearchDateTo = CommonUsed.CheckDate(DateTo.ToString()).ToString();

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
                        //N.TakeOffDate = CommonUsed.CheckDate(item?.OfficerData.TowerData.TakeOffDate.ToString());
                        N.TakeOffDate = CommonUsed.CheckDate(item?.OfficerData.TowerData.LandingDate.ToString());//تم تعديلها نسبة لان بعض الرحلات تكون بايتة وتظهر بتاريخ الشهر الجديد ودي مشكلة
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
                        N.TakeOffDate = CommonUsed.CheckDate(item?.OfficerData.TowerData.LandingDate.ToString());
                        N.FlightNo = item?.OfficerData.TowerData.AirLinesFlightTrip.FlightNo;
                        N.DomeNYFeesSDG = Convert.ToDecimal(item?.DomeNYFees);
                        if (N.DomeNYFeesSDG == 0)
                        {
                            N.DomeFees = 0;
                        }
                        else
                        {
                            N.DomeFees = (N.DomeNYFeesSDG / N.Pax);
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
        
        [Authorize(Roles = "Revenues,Super-Admin,Cupps,Ny,Ny&&Car")]
        public ActionResult PrintDataCupps(int? AirLineId, DateTime? DateFrom, DateTime? DateTo, int? FlightTypeId)
        {
            var id = User.Identity.GetUserId();
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
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.TakeOffDate).Where(x => x.OfficerData.Pax != null | x.OfficerData.TRNYCUPPS != null);

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
                SearchDateFrom = CommonUsed.CheckDate(DateFrom.ToString()).ToString();
                SearchDateTo = CommonUsed.CheckDate(DateTo.ToString()).ToString();

                ViewBag.DateFrom = SearchDateFrom;
                ViewBag.DateTo = SearchDateTo;
            }


            //هنا مفترض نستجلب سعر الصرف من جدول المطالبة وليس من جدول تفاصيل العملات
            if (db.FinancialClaims.Any(x =>  x.AccIdDebit == AirLineAccId && (DbFunctions.TruncateTime(x.DateFrom) <= DateFrom & DbFunctions.TruncateTime(x.DateTo) >= DateFrom) |
                 (DbFunctions.TruncateTime(x.DateFrom) <= DateTo & DbFunctions.TruncateTime(x.DateTo) >= DateTo)))
            {
                var obj = db.FinancialClaims.Where(x => x.AccIdDebit == AirLineAccId && (DbFunctions.TruncateTime(x.DateFrom) <= DateFrom & DbFunctions.TruncateTime(x.DateTo) >= DateFrom) |
                 (DbFunctions.TruncateTime(x.DateFrom) <= DateTo & DbFunctions.TruncateTime(x.DateTo) >= DateTo)).FirstOrDefault();
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
                    //CInter.TakeOffDate = CommonUsed.CheckDate(item?.OfficerData.TowerData.TakeOffDate.ToString());
                    CInter.TakeOffDate = CommonUsed.CheckDate(item?.OfficerData.TowerData.LandingDate.ToString());//تم تعديلها نسبة لان بعض الرحلات تكون بايتة وتظهر بتاريخ الشهر الجديد ودي مشكلة
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
               

                Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date);
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
                    SearchDateFrom = CommonUsed.CheckDate(DateFrom.ToString()).ToString();
                    SearchDateTo = CommonUsed.CheckDate(DateTo.ToString()).ToString();
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
                    C.TakeOffDate = CommonUsed.CheckDate(item?.OfficerData.TowerData.TakeOffDate.ToString());
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


        /*---------------------------------------------------- New Reports -----------------------------------------------------------------------------*/
        public ActionResult PrintDataGHGrafDollar()
        {

            return View();
        }

        [Authorize(Roles = "Revenues,Super-Admin")]
        public ActionResult PrintDataGHGraf(int? HandlingCompanyId, DateTime? DateFrom, DateTime? DateTo)
        {
            var id = User.Identity.GetUserId();
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
            string DemandMouth = db.MouthWorkOns.Where(x => x.Id == Month).FirstOrDefault().Mouth;

            string DemandDate = "مطالبة شهر " + DemandMouth + " " + Year + "م";
            ViewBag.DemandDate = DemandDate;

            string ArName = "غير محدد";
            string Name = "غير محدد";

            string SearchDateFrom = "غير محدد";
            string SearchDateTo = "غير محدد";

            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date);

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
                SearchDateFrom = CommonUsed.CheckDate(DateFrom.ToString()).ToString();
                SearchDateTo = CommonUsed.CheckDate(DateTo.ToString()).ToString();

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

        [Authorize(Roles = "Revenues,Super-Admin")]
        public ActionResult PrintDataFuelGraf(int? FuelCompanyId, DateTime? DateFrom, DateTime? DateTo)
        {
            var id = User.Identity.GetUserId();
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
            string DemandMouth = db.MouthWorkOns.Where(x => x.Id == Month).FirstOrDefault().Mouth;

            string DemandDate = "مطالبة شهر " + DemandMouth + " " + Year + "م";
            ViewBag.DemandDate = DemandDate;

            string ArName = "غير محدد";
            string Name = "غير محدد";

            string SearchDateFrom = "غير محدد";
            string SearchDateTo = "غير محدد";

            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date);


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
                SearchDateFrom = CommonUsed.CheckDate(DateFrom.ToString()).ToString();
                SearchDateTo = CommonUsed.CheckDate(DateTo.ToString()).ToString();

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
        
        [Authorize(Roles = "Revenues,Super-Admin")]
        public ActionResult PrintDataHandlingComp(int? HandlingCompanyId, DateTime? DateFrom, DateTime? DateTo)
        {
            var id = User.Identity.GetUserId();
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
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date).Where(x => x.GroundHandlingFees > 0);

            if (HandlingCompanyId > 0)
            {
                Revenue = Revenue.Where(x => x.OfficerData.HandlingAgentId == HandlingCompanyId);
                Name = db.HandlingAgentsCompanies.Find(HandlingCompanyId).NameEn;

                ViewBag.Name = Name;
            }

            if (DateFrom != null && DateTo != null)
            {
                Revenue = Revenue.Where(t => DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) >= DbFunctions.TruncateTime(DateFrom) && DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) <= DbFunctions.TruncateTime(DateTo));
                SearchDateFrom = CommonUsed.CheckDate(DateFrom.ToString()).ToString();
                SearchDateTo = CommonUsed.CheckDate(DateTo.ToString()).ToString();

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

                F.TakeOffDate = CommonUsed.CheckDate(item?.OfficerData.TowerData.TakeOffDate.ToString());

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

        [Authorize(Roles = "Revenues,Super-Admin")]
        public ActionResult PrintDataFuelFees(int? FuelCompanyId, DateTime? DateFrom, DateTime? DateTo)
        {
            var id = User.Identity.GetUserId();
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
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date).Where(x => x.FuelFees > 0);

            if (FuelCompanyId > 0)
            {
                Revenue = Revenue.Where(x => x.OfficerData.FuelCompanyId == FuelCompanyId);
                Name = db.FuelCompanies.Find(FuelCompanyId).NameEn;

                ViewBag.Name = Name;
            }

            if (DateFrom != null && DateTo != null)
            {
                Revenue = Revenue.Where(t => DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) >= DbFunctions.TruncateTime(DateFrom) && DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) <= DbFunctions.TruncateTime(DateTo));
                SearchDateFrom = CommonUsed.CheckDate(DateFrom.ToString()).ToString();
                SearchDateTo = CommonUsed.CheckDate(DateTo.ToString()).ToString();

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

                F.TakeOffDate = CommonUsed.CheckDate(item?.OfficerData.TowerData.TakeOffDate.ToString());

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
        
        [Authorize(Roles = "Revenues,Super-Admin,Landing,Mail,Freight,Landing-Admin")]
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
                model.FlightDate = CommonUsed.CheckDate(towerData.TakeOffDate.ToString());
                model.LandingDate = CommonUsed.CheckDate(towerData.LandingDate.ToString());
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
        
    }
}