using AirPortERP.Extenstion;
using AirPortERP.Models;
using AirPortERP.ViewModels;
using CrystalDecisions.CrystalReports.Engine;
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

        public ActionResult PrintDataInternalCupps(int? AirLineId, DateTime? DateFrom, DateTime? DateTo)
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
            else
            {
                //Month = Convert.ToDateTime(currentDate).Month;
                //Year = Convert.ToDateTime(currentDate).Year;
                return RedirectToAction("Error");
            }
            ExchangeRate = CommonUsed.getExchangeRateByLandingDate(Month, Year);


            string SearchAirLineId = "غير محدد";
          
            string SearchDateFrom = "غير محدد";
            string SearchDateTo = "غير محدد";
          
            ReportDocument rd = new ReportDocument();

            
            IQueryable<Revenue> Revenue ;
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date).Where(x => x.OfficerData.TowerData.AirLine.IsLocalCompany == true 
            && x.OfficerData.Pax > 0);
           
           

           

            if (DateFrom != null && DateTo != null)
            {
                Revenue = Revenue.Where(t => DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) >= DbFunctions.TruncateTime(DateFrom) && DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) <= DbFunctions.TruncateTime(DateTo));
                SearchDateFrom = CommonUsed.CheckDate(DateFrom.ToString()).ToString();
                SearchDateTo = CommonUsed.CheckDate(DateTo.ToString()).ToString();
            }


            //////////////////////////////////////////////////////////
           /// IQueryable<AirLine> AirLines = db.AirLines.Where(x => x.IsLocalCompany == true);

            var RevenueList = new List<RptExtenstionInternalCupps>();

            //foreach (var item in ))
            //{
              //  Revenue = db.Revenues.Where(x => x.OfficerData.TowerData.AirLineId == AirLine.Id && x.OfficerData.Pax > 0);

                RptExtenstionInternalCupps N = new RptExtenstionInternalCupps();
               // N.Airline = Revenue.;
                N.Pax = Convert.ToInt32(Revenue.Sum(x => x.OfficerData.Pax));

                N.CuppsFees = Convert.ToInt32(Revenue.Sum(x => x.DomeCUPPSFees));

                /// N.AmountSDG = N.InterNYFees * ExchangeRate;
                RevenueList.Add(N);


            //}


            ////////////////////////////////////////////////////////////////////////////



           




            if (Revenue.Count() <= 0)
            {
                return RedirectToAction("Error");
            }
            ///***********************************************************************************
          

          
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Revenue/ReportExtenstion/RptCuppsInternal.rpt")));
            
           

            ////*************************************************************************************
            String HeaderDate = Convert.ToDateTime(DateFrom).ToString("MMM-yyyy");
            rd.SetDataSource(RevenueList.Select(p => new
            {
               // Id = p.Id,
                Pax = p.Pax,
                CuppsFees = p.CuppsFees,

            

                ExchangeRate = ExchangeRate,

             

            }).ToList());


            if (Format == 2)
            {
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                Stream Stream = rd.ExportToStream
                    //WordForWindows
                    (CrystalDecisions.Shared.ExportFormatType.Excel);
                Stream.Seek(0, SeekOrigin.Begin);//application/doc
                return File(Stream, "application/xls", "FinalInvoice.xls");
            }
            else
            {
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                Stream Stream = rd.ExportToStream
                    //WordForWindows //PortableDocFormat //PortableDocFormat
                    (CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                Stream.Seek(0, SeekOrigin.Begin);//application/doc //
                return File(Stream, "application/pdf", "FinalReportSum.pdf");
            }
        }














    }
}