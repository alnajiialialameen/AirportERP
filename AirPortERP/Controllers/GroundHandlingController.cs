using AirPortERP.Extenstion;
using AirPortERP.Models;
using AirPortERP.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AirPortERP.Extenstion;
using PagedList;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;

namespace AirPortERP.Controllers
{
    public class GroundHandlingController : Controller
    {
        DateTime now = System.DateTime.Now;
        private CommonUsed CommonUsed = new CommonUsed();
        private Entities db = new Entities();
        RoleController Role = new RoleController();
        private Calculation cal = new Calculation();
        int WorkMouth;
        int WorkYear;
        static decimal ExchangeRate;

        // GET: GroundHandling
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetAllTowerDatas()
        {
            IQueryable<Revenue> Revenue;
            var id = User.Identity.GetUserId();

            var companyInfo = Role.GetCompanyId(id);

            WorkMouth = CommonUsed.GetMouth(id);
            WorkYear = CommonUsed.GetYear(id);

            Revenue = db.Revenues.OrderBy(x => x.Id).Where(x => x.OfficerData.TowerData.Date.Value.Month == WorkMouth
            && x.OfficerData.TowerData.Date.Value.Year == WorkYear & (x.GroundHandlingFees < 0 | x.GroundHandlingFees == null));

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

                tdm.GroundHandlingFees = item.GroundHandlingFees.ToString();
                tdm.GroundHandlingCompId = Convert.ToInt32(item.OfficerData.HandlingAgentId);

                if (tdm.GroundHandlingCompId > 0)
                {
                    tdm.HandlingCompName = item.OfficerData.HandlingAgentsCompany.NameEn;
                }

                TowerDatasList.Add(tdm);
            }

            return Json(new { data = TowerDatasList.ToList() }, JsonRequestBehavior.AllowGet);
        }
        // GET: GroundHandling
        public ActionResult Completed()
        {
            return View();
        }

        public ActionResult GetAllForCompleted()
        {
            IQueryable<Revenue> Revenue;
            var id = User.Identity.GetUserId();

            var companyInfo = Role.GetCompanyId(id);

            WorkMouth = CommonUsed.GetMouth(id);
            WorkYear = CommonUsed.GetYear(id);

            Revenue = db.Revenues.OrderBy(x => x.Id).Where(x => x.OfficerData.TowerData.Date.Value.Month == WorkMouth
            && x.OfficerData.TowerData.Date.Value.Year == WorkYear & x.GroundHandlingFees > 0);

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

                tdm.GroundHandlingFees = item.GroundHandlingFees.ToString();
                tdm.GroundHandlingCompId = Convert.ToInt32(item.OfficerData.HandlingAgentId);

                if (tdm.GroundHandlingCompId > 0)
                {
                    tdm.HandlingCompName = item.OfficerData.HandlingAgentsCompany.NameEn;
                }

                TowerDatasList.Add(tdm);
            }

            return Json(new { data = TowerDatasList.ToList() }, JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        public ActionResult CreateOrUpdate(int TowerDataId, int GroundHandlingCompId)
        {
            if (db.OfficerDatas.Any(x => x.TowerDataId == TowerDataId))
            {
                OfficerData data = db.OfficerDatas.FirstOrDefault(m => m.TowerDataId == TowerDataId);
                data.HandlingAgentId = GroundHandlingCompId;

                decimal Fees = Calculation.GroundHandlingFees(TowerDataId, data.Id);

                if (Fees > 0)
                {
                    Revenue Rev = db.Revenues.FirstOrDefault(x => x.OfficerDataId == data.Id);
                    Rev.GroundHandlingFees = Fees;

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
                    data.HandlingAgentId = null;

                    Revenue Rev = db.Revenues.FirstOrDefault(m => m.OfficerDataId == data.Id);
                    Rev.GroundHandlingFees = 0;

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

        public ActionResult GroundHandlingReport()
        {
            return View();
        }

        /******************************************************** First Class ************************************************************************/
        public ActionResult GetDataHandlingComp(int? page, int? Get, int? Search, int? Count, int? HandlingCompanyId, DateTime? DateFrom, DateTime? DateTo)
        {
            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date).Where(x => x.GroundHandlingFees > 0);

            if (Get > 0)
            {
                if (HandlingCompanyId > 0)
                {
                    Revenue = Revenue.Where(x => x.OfficerData.HandlingAgentId == HandlingCompanyId);
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
            return PartialView("_GetDataHandlingComp", Revenue.ToPagedList(PageNumber, pageSize));
        }

        public ActionResult PrintDataHandlingCompCR(int? HandlingCompanyId, DateTime? DateFrom, DateTime? DateTo)
        {
            var id = User.Identity.GetUserId();
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
                //Month = Convert.ToDateTime(currentDate).Month;
                //Year = Convert.ToDateTime(currentDate).Year;
                return RedirectToAction("Error");
            }
            ExchangeRate = CommonUsed.getExchangeRateByLandingDate(WorkMouth, WorkYear);

            string SearchAirLineId = "غير محدد";
            string SearchDateFrom = "غير محدد";
            string SearchDateTo = "غير محدد";
            ReportDocument rd = new ReportDocument();

            rd.Load(Path.Combine(Server.MapPath("~/Reports/Revenue/RptFirstClassFees.rpt")));

            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date).Where(x => x.FirstClassFees > 0);
            //if (FlightTypeId > 0)
            //{
            //    Revenue = Revenue.Where(x => x.OfficerData.TowerData.FlightType == FlightTypeId);
            //    SearchFlightTypeId = CommonUsed.GetFlightTypeName(FlightTypeId);
            //}

            if (HandlingCompanyId > 0)
            {
                Revenue = Revenue.Where(x => x.OfficerData.HandlingAgentId == HandlingCompanyId);
                SearchAirLineId = db.HandlingAgentsCompanies.Find(HandlingCompanyId).NameEn;
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
            var RevenueList = new List<FirstClassRptViewModel>();

            foreach (var item in Revenue)
            {

                FirstClassRptViewModel F = new FirstClassRptViewModel();

                F.Id = item.Id;
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
            rd.SetDataSource(RevenueList.Select(p => new
            {
                Id = p.Id,
                Name = p.Name,
                FlightNo = p.FlightNo,
                TakeOffDate = p.TakeOffDate,

                FirstClassCount = p.FirstClassCount,
                FirstClassFees = p.FirstClassFees,
                Fees = p.Fees,

                SearchDateFrom = SearchDateFrom,
                SearchDateTo = SearchDateTo,
                FirstClassFeesSDG = p.FirstClassFeesSDG,
                ExchangeRate = ExchangeRate,
            }).ToList());

            //if (Format == 2) ///excel
            //{
            //    Response.Buffer = false;
            //    Response.ClearContent();
            //    Response.ClearHeaders();
            //    Stream Stream = rd.ExportToStream
            //        //WordForWindows //PortableDocFormat //PortableDocFormat
            //        (CrystalDecisions.Shared.ExportFormatType.Excel);
            //    Stream.Seek(0, SeekOrigin.Begin);//application/doc //
            //    return File(Stream, "application/xls ", "FirstClass.xls");
            //}
            //else
            //{
            //    //pdf
            //    Response.Buffer = false;
            //    Response.ClearContent();
            //    Response.ClearHeaders();
            //    Stream Stream = rd.ExportToStream
            //        //WordForWindows //PortableDocFormat //PortableDocFormat
            //        (CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            //    Stream.Seek(0, SeekOrigin.Begin);//application/doc //
            //    return File(Stream, "application/doc ");
            //}

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Stream Stream = rd.ExportToStream
                //WordForWindows //PortableDocFormat //PortableDocFormat
                (CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            Stream.Seek(0, SeekOrigin.Begin);//application/doc //
            return File(Stream, "application/doc ");
        }

        public ActionResult PrintDataHandlingComp(int? HandlingCompanyId, DateTime? DateFrom, DateTime? DateTo)
        {
            var id = User.Identity.GetUserId();
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
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date).Where(x => x.FirstClassFees > 0);
            
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
    }
}