using AirPortERP.Extenstion;
using AirPortERP.Models;
using AirPortERP.ViewModels;
using Microsoft.AspNet.Identity;
using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AirPortERP.Controllers
{
    public class FuelsController : Controller
    {
        DateTime now = System.DateTime.Now;
        private CommonUsed CommonUsed = new CommonUsed();
        private Entities db = new Entities();
        RoleController Role = new RoleController();
        private Calculation cal = new Calculation();
        int WorkMouth;
        int WorkYear;
        static decimal ExchangeRate;

        // GET: Fuels
        public ActionResult Index()
        {
            string val = db.GeneralServices.Find(7).Value.ToString();
            ViewBag.SettingsFeesVal = val;
            return View();
        }

        public ActionResult GetAllTowerDatas()
        {
            IQueryable<Revenue> Revenue;
            var id = User.Identity.GetUserId();

            //var companyInfo = Role.GetCompanyId(id);
            var companyId = CommonUsed.getCompanyId(id);

            WorkMouth = CommonUsed.GetMouth(id);
            WorkYear = CommonUsed.GetYear(id);

            Revenue = db.Revenues.OrderBy(x => x.Id).Where(x => x.OfficerData.TowerData.Date.Value.Month == WorkMouth
            && x.OfficerData.TowerData.Date.Value.Year == WorkYear & (x.FuelFees < 0 | x.FuelFees == null));

            Revenue = Revenue.Where(x => x.OfficerData.TowerData.CompanyInfoId == companyId);
            var TowerDatasList = new List<GroundHandlingViewModels>();

            // var TowerDatasList = new List<TowerDataViewModels>();
            foreach (var item in Revenue)
            {
                GroundHandlingViewModels tdm = new GroundHandlingViewModels();

                tdm.Id = item.OfficerData.TowerData.Id;
                tdm.AirLineCode = item?.OfficerData.TowerData.AirLine.Code;
                tdm.AirLineName = item?.OfficerData.TowerData.AirLine.Name;
                tdm.TripType = CommonUsed.GetFlightTypeNameLanding(item.OfficerData.TowerData.FlightTypeL);
                tdm.FlightNo = item?.OfficerData.TowerData.AirLinesFlightTrip.FlightNo;
                tdm.AircraftRegName = item?.OfficerData.TowerData.AircraftRegistration.Registration;
                tdm.Type = item?.OfficerData.TowerData.AircraftRegistration.AircraftType.Type;
                tdm.MaxTaKeOffWeight = item?.OfficerData.TowerData.AircraftRegistration.MaxTakoffWieght.ToString();

                //from --------------------------------------------------------------------------------------
                tdm.AirportIdFrom = CommonUsed.CheckAirportCode(Convert.ToInt32(item?.OfficerData.TowerData.AirportIdFrom));
                tdm.FromArabicName = item.OfficerData.TowerData.AirPort1.NameAr;

                if (Convert.ToInt32(item.OfficerData.TowerData.AirPort1.CountryId) > 0)
                {
                    tdm.FromArabicCountry = item.OfficerData.TowerData.AirPort1.Country.NameAr;
                }
                else
                {
                    tdm.FromArabicCountry = "غير محدد";
                }

                //To --------------------------------------------------------------------------------------
                tdm.AirportIdTo = CommonUsed.CheckAirportCode(Convert.ToInt32(item?.OfficerData.TowerData.AirportIdTo));
                tdm.ToArabicName = item.OfficerData.TowerData.AirPort.NameAr;

                if (Convert.ToInt32(item.OfficerData.TowerData.AirPort.CountryId) > 0)
                {
                    tdm.ToArabicCountry = item.OfficerData.TowerData.AirPort.Country.NameAr;
                }
                else
                {
                    tdm.ToArabicCountry = "غير محدد";
                }

                tdm.Date = Convert.ToDateTime(item.OfficerData.TowerData.Date).ToString("MM/dd/yyyy");
                tdm.ATA = item.OfficerData.TowerData.ATA.ToString();
                tdm.ATD = item.OfficerData.TowerData.ATD.ToString();

                tdm.CreatedBy = CommonUsed.GetUserName(item.OfficerData.TowerData.CreatedBy);

                tdm.LandingDate = CommonUsed.CheckDate(item.OfficerData.TowerData.LandingDate.ToString());
                tdm.TakeOffDate = CommonUsed.CheckDate(item.OfficerData.TowerData.TakeOffDate.ToString());

                tdm.Size = item?.OfficerData.TowerData.AircraftRegistration.AircraftType.AircraftSize.Size.ToString();
                tdm.LandingFees = item.LandingFees.ToString();

                tdm.ParkingFees = item.ParkingFees.ToString();
                tdm.NightOperationFees = item.NightOperationFees.ToString();
                tdm.NavigationFees = item.NavigationFees.ToString();
                tdm.NoiseFees = item.NoiseFees.ToString();
                tdm.SecurityFees = item.SecurityFees.ToString();

                tdm.GroundHandlingFees = item.FuelFees.ToString();
                //tdm.GroundHandlingCompId = Convert.ToInt32(item.OfficerData.HandlingAgentId);
                
                TowerDatasList.Add(tdm);
            }

            return Json(new { data = TowerDatasList.ToList() }, JsonRequestBehavior.AllowGet);
        }
        // GET: Fuels
        public ActionResult Completed()
        {
            string val = db.GeneralServices.Find(7).Value.ToString();
            ViewBag.SettingsFeesVal = val;

            return View();
        }

        public ActionResult GetAllForCompleted()
        {
            IQueryable<Revenue> Revenue;
            var id = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(id);
            var companyInfo = Role.GetCompanyId(id);

            WorkMouth = CommonUsed.GetMouth(id);
            WorkYear = CommonUsed.GetYear(id);

            Revenue = db.Revenues.OrderBy(x => x.Id).Where(x => x.OfficerData.TowerData.Date.Value.Month == WorkMouth
            && x.OfficerData.TowerData.Date.Value.Year == WorkYear & x.FuelFees > 0);
            Revenue = Revenue.Where(x => x.OfficerData.TowerData.CompanyInfoId == companyId);
            var TowerDatasList = new List<GroundHandlingViewModels>();

            // var TowerDatasList = new List<TowerDataViewModels>();
            foreach (var item in Revenue)
            {
                GroundHandlingViewModels tdm = new GroundHandlingViewModels();

                tdm.Id = item.OfficerData.TowerData.Id;
                tdm.AirLineCode = item?.OfficerData.TowerData.AirLine.Code;
                tdm.AirLineName = item?.OfficerData.TowerData.AirLine.Name;
                tdm.TripType = CommonUsed.GetFlightTypeNameLanding(item.OfficerData.TowerData.FlightTypeL);
                tdm.FlightNo = item?.OfficerData.TowerData.AirLinesFlightTrip.FlightNo;
                tdm.AircraftRegName = item?.OfficerData.TowerData.AircraftRegistration.Registration;
                tdm.Type = item?.OfficerData.TowerData.AircraftRegistration.AircraftType.Type;
                tdm.MaxTaKeOffWeight = item?.OfficerData.TowerData.AircraftRegistration.MaxTakoffWieght.ToString();

                //from --------------------------------------------------------------------------------------
                tdm.AirportIdFrom = CommonUsed.CheckAirportCode(Convert.ToInt32(item?.OfficerData.TowerData.AirportIdFrom));
                tdm.FromArabicName = item.OfficerData.TowerData.AirPort1.NameAr;

                if (Convert.ToInt32(item.OfficerData.TowerData.AirPort1.CountryId) > 0)
                {
                    tdm.FromArabicCountry = item.OfficerData.TowerData.AirPort1.Country.NameAr;
                }
                else
                {
                    tdm.FromArabicCountry = "غير محدد";
                }

                //To --------------------------------------------------------------------------------------
                tdm.AirportIdTo = CommonUsed.CheckAirportCode(Convert.ToInt32(item?.OfficerData.TowerData.AirportIdTo));
                tdm.ToArabicName = item.OfficerData.TowerData.AirPort.NameAr;

                if (Convert.ToInt32(item.OfficerData.TowerData.AirPort.CountryId) > 0)
                {
                    tdm.ToArabicCountry = item.OfficerData.TowerData.AirPort.Country.NameAr;
                }
                else
                {
                    tdm.ToArabicCountry = "غير محدد";
                }

                tdm.Date = Convert.ToDateTime(item.OfficerData.TowerData.Date).ToString("MM/dd/yyyy");
                tdm.ATA = item.OfficerData.TowerData.ATA.ToString();
                tdm.ATD = item.OfficerData.TowerData.ATD.ToString();

                tdm.CreatedBy = CommonUsed.GetUserName(item.OfficerData.TowerData.CreatedBy);

                tdm.LandingDate = CommonUsed.CheckDate(item.OfficerData.TowerData.LandingDate.ToString());
                tdm.TakeOffDate = CommonUsed.CheckDate(item.OfficerData.TowerData.TakeOffDate.ToString());

                tdm.Size = item?.OfficerData.TowerData.AircraftRegistration.AircraftType.AircraftSize.Size.ToString();
                tdm.LandingFees = item.LandingFees.ToString();

                tdm.ParkingFees = item.ParkingFees.ToString();
                tdm.NightOperationFees = item.NightOperationFees.ToString();
                tdm.NavigationFees = item.NavigationFees.ToString();
                tdm.NoiseFees = item.NoiseFees.ToString();
                tdm.SecurityFees = item.SecurityFees.ToString();

                tdm.GroundHandlingFees = item.FuelFees.ToString();
                tdm.HandlingCompName = item.OfficerData.FuelCompany.NameEn;


                TowerDatasList.Add(tdm);
            }

            return Json(new { data = TowerDatasList.ToList() }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CreateOrUpdate(int FuelCompanyId, int Id, decimal FuelLiter)
        {
            if (db.OfficerDatas.Any(x => x.TowerDataId == Id))
            {
                OfficerData data = db.OfficerDatas.FirstOrDefault(m => m.TowerDataId == Id);
                data.FuelCompanyId = FuelCompanyId;
                data.FuelLiter = FuelLiter;

                decimal Fees = Calculation.FuelPerLiterFees(FuelLiter);

                if (Fees > 0)
                {
                    Revenue Rev = db.Revenues.FirstOrDefault(x => x.OfficerDataId == data.Id);
                    Rev.FuelFees = Fees;

                    db.Entry(data).State = EntityState.Modified;
                    db.Entry(Rev).State = EntityState.Modified;
                    db.SaveChanges();

                    return Json("Success", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("Error", JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Delete(int id)
        {
            if (id > 0)
            {
                if (db.OfficerDatas.Any(x => x.TowerDataId == id))
                {
                    OfficerData data = db.OfficerDatas.FirstOrDefault(m => m.TowerDataId == id);
                    data.FuelLiter = null;
                    data.FuelCompanyId = null;

                    Revenue Rev = db.Revenues.FirstOrDefault(m => m.OfficerDataId == data.Id);
                    Rev.FuelFees = null;

                    db.Entry(data).State = EntityState.Modified;
                    db.Entry(Rev).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return Json("Delete", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }


        }

        public ActionResult FuelsReport()
        {
            return View();
        }

        /******************************************************** First Class ************************************************************************/
        public ActionResult GetDataFuel(int? page, int? Get, int? Search, int? Count, int? FuelCompanyId, DateTime? DateFrom, DateTime? DateTo)
        {
            var id = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(id);

            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date).Where(x => x.FuelFees > 0
            && x.OfficerData.TowerData.CompanyInfoId == companyId);

            if (Get > 0)
            {
                if (FuelCompanyId > 0)
                {
                    Revenue = Revenue.Where(x => x.OfficerData.FuelCompanyId == FuelCompanyId);
                }

                if (DateFrom != null && DateTo != null)
                {
                    Revenue = Revenue.Where(t => DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) >= DbFunctions.TruncateTime(DateFrom) && DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) <= DbFunctions.TruncateTime(DateTo));
                }

                if (Search == 1)
                {
                    if (Revenue != null)
                    {
                        int ItemCount = 0;
                        ItemCount = Revenue.Count();
                        ViewBag.ItemCount = ItemCount;
                    }
                }
                else
                {
                    ViewBag.ItemCount = Count;
                }
            }
            else
            {
                Revenue = null;
                ViewBag.ItemCount = 0;
            }

            int pageSize = 5;
            int PageNumber = (page ?? 1);

            //return Json(Revenue.ToList(), JsonRequestBehavior.AllowGet);
            return PartialView("_GetDataFuel", Revenue.ToPagedList(PageNumber, pageSize));
        }
        
        public ActionResult PrintDataHandlingComp(int? FuelCompanyId, DateTime? DateFrom, DateTime? DateTo)
        {
            var id = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(id);
            RoleController role = new RoleController();
            int GetCompanyId = role.GetCompanyId(id);
            var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);

            string UserName = CommonUsed.GetUserName(id);

            if (DateTo != null)
            {
                WorkMouth = Convert.ToDateTime(DateTo).Month;
                WorkYear = Convert.ToDateTime(DateTo).Year;
            }
            else if (DateFrom != null)
            {
                WorkMouth = Convert.ToDateTime(DateFrom).Month;
                WorkYear = Convert.ToDateTime(DateFrom).Year;
            }
            else
            {
                return RedirectToAction("Error");
            }
            ExchangeRate = CommonUsed.getExchangeRateByLandingDate(WorkMouth, WorkYear);

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
                Name = db.AirLines.Find(FuelCompanyId).Name;

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

            if (Revenue.Count() <= 0)
            {
                return RedirectToAction("Error");
            }
            /***********************************************************************************/
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

                F.TakeOffDate = CommonUsed.CheckDate(item?.OfficerData.TowerData.TakeOffDate.ToString());

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



        //// ---------------------------------print fueil graf-------------------------------------------


        //public ActionResult PrintDataFuelsGraf(int? HandlingCompanyId, DateTime? DateFrom, DateTime? DateTo)
        //{
        //    var id = User.Identity.GetUserId();
        //    var companyId = CommonUsed.getCompanyId(id);
        //    RoleController role = new RoleController();

        //    int GetCompanyId = role.GetCompanyId(id);
        //    var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);

        //    string RevenuesManager = CompanyInfo.RevenuesManager;
        //    ViewBag.RevenuesManager = RevenuesManager;

        //    string DateNow = System.DateTime.Now.ToString("yyyy/MM/dd") + "م";
        //    ViewBag.DateNow = DateNow;

        //    string Serial = Guid.NewGuid().ToString();

        //    string UserName = CommonUsed.GetUserName(id);

        //    if (DateTo != null)
        //    {
        //        Month = Convert.ToDateTime(DateTo).Month;
        //        Year = Convert.ToDateTime(DateTo).Year;
        //    }
        //    else if (DateFrom != null)
        //    {
        //        Month = Convert.ToDateTime(DateFrom).Month;
        //        Year = Convert.ToDateTime(DateFrom).Year;
        //    }
        //    else
        //    {
        //        //Month = Convert.ToDateTime(currentDate).Month;
        //        //Year = Convert.ToDateTime(currentDate).Year;
        //        return RedirectToAction("Error");
        //    }
        //    ExchangeRate = CommonUsed.getExchangeRateByLandingDate(Month, Year);
        //    string DemandMouth = db.MouthWorkOns.Where(x => x.Id == Month).FirstOrDefault().Mouth;

        //    string DemandDate = "مطالبة شهر " + DemandMouth + " " + Year + "م";
        //    ViewBag.DemandDate = DemandDate;

        //    string ArName = "غير محدد";
        //    string Name = "غير محدد";

        //    string SearchDateFrom = "غير محدد";
        //    string SearchDateTo = "غير محدد";

        //    IQueryable<Revenue> Revenue;
        //    Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date).Where(x => x.OfficerData.TowerData.CompanyInfoId == companyId);
        //    ;

        //    if (HandlingCompanyId > 0)
        //    {
        //        Revenue = Revenue.Where(x => x.OfficerData.HandlingAgentId == HandlingCompanyId);
        //        ArName = db.HandlingAgentsCompanies.Find(HandlingCompanyId).NameAr;
        //        Name = db.HandlingAgentsCompanies.Find(HandlingCompanyId).NameEn;
        //    }
        //    ViewBag.ArName = ArName;
        //    if (DateFrom != null && DateTo != null)
        //    {
        //        Revenue = Revenue.Where(t => DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) >= DbFunctions.TruncateTime(DateFrom) && DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) <= DbFunctions.TruncateTime(DateTo));
        //        SearchDateFrom = CommonUsed.ToDisplayDate(DateFrom).ToString();
        //        SearchDateTo = CommonUsed.ToDisplayDate(DateTo).ToString();

        //        ViewBag.SearchDateFrom = SearchDateFrom;
        //        ViewBag.SearchDateTo = SearchDateTo;

        //    }
        //    /***********************************************************************************/

        //    PrintFinalInvoiceViewModel F = new PrintFinalInvoiceViewModel();

        //    F.Name = db.HandlingAgentsCompanies.Find(HandlingCompanyId).NameAr;

        //    F.FreightFees = Convert.ToDouble(Revenue.Sum(x => x.GroundHandlingFees));

        //    F.TaxDollar = F.FreightFees * 0.05;

        //    F.GrandTotalDollar = (F.FreightFees + F.TaxDollar);
        //    F.GrandTotalSDG = (F.GrandTotalDollar) * Convert.ToDouble(ExchangeRate);

        //    /*-------------------------------------SDG------------------------------------------------------------*/

        //    double SDGRound = Convert.ToDouble(Math.Round(F.GrandTotalSDG, 2));

        //    ToWord toWord = new ToWord(Convert.ToDecimal(SDGRound), currencies[Convert.ToInt32(0)]);

        //    F.GrandTotalSDGTxt = toWord.ConvertToArabic().ToString();

        //    /*--------------------------------------Dollar-----------------------------------------------------------*/

        //    double DollarRound = Convert.ToDouble(Math.Round(F.GrandTotalDollar, 2));
        //    ToWord toWord1 = new ToWord(Convert.ToDecimal(DollarRound), currencies[Convert.ToInt32(1)]);

        //    F.GrandTotalDollarTxt = toWord1.ConvertToArabic().ToString();

        //    /*************************************************************************************/
        //    ViewBag.HeaderDate = Convert.ToDateTime(DateFrom).ToString("MMM-yyyy");

        //    int CurrencyType = Convert.ToInt32(db.HandlingAgentsCompanies.Find(HandlingCompanyId).CurrencyId);

        //    if (CurrencyType == 1)
        //    {
        //        ViewBag.Header = "Graf-" + Name + "-" + DateTime.Now.ToShortDateString();
        //        return View(F);
        //    }
        //    else
        //    {
        //        ViewBag.Header = "Graf-" + Name + "-" + DateTime.Now.ToShortDateString();
        //        return View("PrintDataGHGrafDollar", F);
        //    }

        //}



    }
}