using AirPortERP.Extenstion;
using AirPortERP.Models;
using AirPortERP.ViewModels;
using Microsoft.AspNet.Identity;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AirPortERP.Controllers
{
    public class ReportExtenstionController : Controller
    {
        private Entities db = new Entities();
        private CommonUsed CommonUsed = new CommonUsed();
        private RptExtention Ex = new RptExtention();
        private int Format, Month, Year;

        static decimal ExchangeRate;

        private Calculation cal = new Calculation();
        // GET: ReportExtenstion

        public ReportExtenstionController()
        {
            //ExchangeRate = Convert.ToDecimal(db.Currencies.Where(x => x.Id == 1).Single().ExchangeRateAgainstPrimaryCurrency);
            Format = Convert.ToInt32(db.PrintFormats.Single(x => x.Id == 1).FormatTypeId);
        }
        
        public ActionResult CuppsInternal()
        {
            return View();
        }
        
        /********************************************************* Final Invoice ***********************************************************************/
        public ActionResult GetInternalCupps(int? page, int? Get, int? Search, int? Count, int? AirLineId
          , DateTime? DateFrom, DateTime? DateTo)
        //   , int? FlightTypeId)
        {
          



            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date).Where(x=>x.OfficerData.TowerData.AirLine.IsLocalCompany==true);

            ViewBag.AirLineId = AirLineId;


            ViewBag.DateFrom = DateFrom;
            ViewBag.DateTo = DateTo;

            //  ViewBag.FlightTypeId = FlightTypeId;

            if (Get > 0)
            {



                //if (AirLineId > 0)
                //{
                //    Revenue = Revenue.Where(x => x.OfficerData.TowerData.AirLineId == AirLineId);
                //}

                if (DateFrom != null && DateTo != null)
                {

                    Revenue = Revenue.Where(t => DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) >= DbFunctions.TruncateTime(DateFrom) && DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) <= DbFunctions.TruncateTime(DateTo));
                }


                //////////////////////////////////////////////////////
                if (Search == 1)
                {

                    if (Revenue != null)
                    {

                        int ItemCount = 0;
                        //List<decimal> Amountlist = new List<decimal>();
                        //List<decimal> Allowancelist = new List<decimal>();


                        ItemCount = Revenue.Count();
                        ViewBag.ItemCount = ItemCount;

                    }
                }
                else
                {

                    //ViewBag.Amount = Amount;
                    //ViewBag.Allowance = Allowance;
                    ViewBag.ItemCount = Count;
                }

                //////////////////////////////////////////////////////////////////////

            }
            else
            {
                Revenue = null;
                ViewBag.ItemCount = 0;
            }

            int pageSize = 5;
            int PageNumber = (page ?? 1);




            return PartialView("_GetCuppsInternal", Revenue.ToPagedList(PageNumber, pageSize));
        }









        public ActionResult PaxAndFrieghtReport()
        {
            return View();
        }

        // دي للبحث
        public ActionResult GetDataPAxAndFrieght(
    int? page, int? Get, int? Search, int? Count, DateTime? DateFrom, DateTime? DateTo)
        {
            IQueryable<TowerData> TowerDataList = db.TowerDatas.OrderBy(x => x.LandingDate);

            ViewBag.DateFrom = DateFrom;
            ViewBag.DateTo = DateTo;

            if (Get > 0)
            {
                if (DateFrom != null && DateTo != null)
                {
                    TowerDataList = TowerDataList
                        .Where(t => DbFunctions.TruncateTime(t.LandingDate) >= DbFunctions.TruncateTime(DateFrom) &&
                                    DbFunctions.TruncateTime(t.LandingDate) <= DbFunctions.TruncateTime(DateTo));

                    TowerDataList = TowerDataList.Where(x => x.LandingDate.Value.Year >= DateFrom.Value.Year && x.LandingDate.Value.Year <= DateTo.Value.Year);
                }

                if (Search == 1)
                {
                    ViewBag.ItemCount = TowerDataList?.Count() ?? 0;
                }
                else
                {
                    ViewBag.ItemCount = Count;
                }
            }
            else
            {
                TowerDataList = Enumerable.Empty<TowerData>().AsQueryable(); // تجنب الخطأ في العمليات التالية
                ViewBag.ItemCount = 0;
            }

            // جلب البيانات وتجميعها حسب الشهر
            //var aggregatedData = TowerDataList
            //    .SelectMany(item => item.OfficerDatas, (towerData, officerData) => new
            //    {
            //        Id = towerData.Id,
            //        Month = towerData.LandingDate.Value.Month,
            //        TotalPax = officerData.Pax ?? 0,
            //        TotalFreightIn = officerData.FreightLoadingImp ?? 0,
            //        TotalFreightOut = officerData.FreightLoadingExp ?? 0
            //    })
            //    .GroupBy(x => x.Month)
            //    .Select(group => new RepoObj
            //    {
            //        Month = group.Key,
            //        TripCount = group.Sum(x => x.Id),
            //        TotalPax = group.Sum(x => x.TotalPax),
            //        TotalFreightIn = group.Sum(x => x.TotalFreightIn),
            //        TotalFreightOut = group.Sum(x => x.TotalFreightOut)
            //    })
            //    .OrderBy(x => x.Month)
            //    .ToList();

            //// جلب البيانات وتجميعها حسب الشهر
            var aggregatedData = TowerDataList
                     .Select(item => new
                     {
                         Id = item.Id,
                         Month = item.LandingDate.Value.Month,
                         Year = item.LandingDate.Value.Year,
                         TotalPax = item.OfficerDatas.FirstOrDefault(x => x.TowerDataId == item.Id).Pax ?? 0,
                         TotalFreightIn = item.OfficerDatas.FirstOrDefault(x => x.TowerDataId == item.Id).FreightLoadingImp ?? 0,
                         TotalFreightOut = item.OfficerDatas.FirstOrDefault(x => x.TowerDataId == item.Id).FreightLoadingExp ?? 0
                     })
                     .GroupBy(x => new { x.Year, x.Month }) // Group by Year and Month
                     .Select(group => new RepoObj
                     {
                         Month = group.Key.Month,
                         TripCount = group.Count(), // Count trips instead of summing Ids
                         TotalPax = group.Where(x => x.Year == group.Key.Year).Sum(x => x.TotalPax),
                         TotalFreightIn = group.Where(x => x.Year == group.Key.Year).Sum(x => x.TotalFreightIn),
                         TotalFreightOut = group.Where(x => x.Year == group.Key.Year).Sum(x => x.TotalFreightOut)
                     }) // Ensure sorting by Year and Month
                     .OrderBy(x => x.Month)
                     .ToList();

            int pageSize = 5;
            int pageNumber = (page ?? 1);

            // تقسيم النتائج إلى صفحات
            var pagedResult = aggregatedData.ToPagedList(pageNumber, pageSize);

            return PartialView("_GetData", pagedResult);
        }
        
        // دي للطباعة
        public ActionResult PrintPaxAndFrieghtReport(DateTime? DateFrom, DateTime? DateTo)
        {
            IQueryable<TowerData> TowerDataList = db.TowerDatas.OrderBy(x => x.LandingDate);

            ViewBag.DateFrom = DateFrom.Value.ToString("dd/MM/yyyy");
            ViewBag.DateTo = DateTo.Value.ToString("dd/MM/yyyy");

            if (DateFrom != null && DateTo != null)
            {
                TowerDataList = TowerDataList
                    .Where(t => DbFunctions.TruncateTime(t.LandingDate) >= DbFunctions.TruncateTime(DateFrom) &&
                                DbFunctions.TruncateTime(t.LandingDate) <= DbFunctions.TruncateTime(DateTo));

                TowerDataList = TowerDataList.Where(x=> x.LandingDate.Value.Year >= DateFrom.Value.Year && x.LandingDate.Value.Year <= DateTo.Value.Year);
            }

            //// جلب البيانات وتجميعها حسب الشهر
            var aggregatedData = TowerDataList
                     .Select(item => new
                     {
                         Id = item.Id,
                         Month = item.LandingDate.Value.Month,
                         Year = item.LandingDate.Value.Year,
                         TotalPax = item.OfficerDatas.FirstOrDefault(x => x.TowerDataId == item.Id).Pax ?? 0,
                         TotalFreightIn = item.OfficerDatas.FirstOrDefault(x => x.TowerDataId == item.Id).FreightLoadingImp ?? 0,
                         TotalFreightOut = item.OfficerDatas.FirstOrDefault(x => x.TowerDataId == item.Id).FreightLoadingExp ?? 0
                     })
                     .GroupBy(x => new { x.Year, x.Month }) // Group by Year and Month
                     .Select(group => new RepoObj
                     {
                         Month = group.Key.Month,
                         TripCount = group.Count(), // Count trips instead of summing Ids
                         TotalPax = group.Where(x => x.Year == group.Key.Year).Sum(x => x.TotalPax),
                         TotalFreightIn = group.Where(x => x.Year == group.Key.Year).Sum(x => x.TotalFreightIn),
                         TotalFreightOut = group.Where(x => x.Year == group.Key.Year).Sum(x => x.TotalFreightOut)
                     }) // Ensure sorting by Year and Month
                     .OrderBy(x => x.Month)
                     .ToList();

            return View(aggregatedData);
            // return Json(aggregatedData, JsonRequestBehavior.AllowGet);
        }
    }
}