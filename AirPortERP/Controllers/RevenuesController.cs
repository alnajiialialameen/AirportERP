using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AirPortERP.Models;
using PagedList;
using AirPortERP.Extenstion;
using Microsoft.AspNet.Identity;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using AirPortERP.ViewModels;

namespace AirPortERP.Controllers
{
    public class RevenuesController : Controller
    {
        private Entities db = new Entities();
        private CommonUsed CommonUsed = new CommonUsed();
        private RptExtention Ex =new RptExtention();
        private int Format, Month, Year;
        static decimal ExchangeRate;

        private Calculation cal = new Calculation();
        //private int companyInfoId = 10; // companyInfoId
      //  private String userId = "111"; // createdById + updatedById
        private DateTime currentDate = DateTime.UtcNow.AddHours(2);

                /************************************************** Start Revenues *****************************/
        /************************************************** Reporting Section *****************************************************/
        public RevenuesController()
        {
            //ExchangeRate = Convert.ToDecimal(db.Currencies.Where(x => x.Id == 1).Single().ExchangeRateAgainstPrimaryCurrency);
            Format = Convert.ToInt32(db.PrintFormats.Single(x => x.Id == 1).FormatTypeId);
        }

        public ActionResult Error()
        {
            return View();
        }

        public ActionResult NyReport()
        {
            return View();
        }

        public ActionResult FirstClassReport()
        {
            return View();
        }

        public ActionResult PushPackReport()
        {
            return View();
        }

        public ActionResult LandingReport()
        {
            return View();
        }

        public ActionResult CuppsReportDistribtion()
        {
            return View();
        }
        public ActionResult CuppsReport()
        {
            return View();
        }

        public ActionResult CuppsReportNoData()
        {
            return View();
        }
        
        public ActionResult ServiceCarReport()
        {
            return View();
        }

        public ActionResult FinalInvoiceReport()
        {
            return View();
        }

        public ActionResult ServiceMailReport()
        {
            return View();
        }
        /******************************************************* NY *************************************************************************/
        public ActionResult GetDataNy(int? page, int? Get, int? Search, int? Count, int? AirLineId
           , DateTime? DateFrom, DateTime? DateTo
          , int? FlightTypeId)
        {

            string SearchDateFrom = "غير محدد";
            string SearchDateTo = "غير محدد";

            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date);

            ViewBag.AirLineId = AirLineId;


            ViewBag.DateFrom = DateFrom;
            ViewBag.DateTo = DateTo;

            ViewBag.FlightTypeId = FlightTypeId;

            if (Get > 0)
            {

                if (FlightTypeId > 0)
                {
                    if (FlightTypeId == 1)
                    {
                      //  Revenue = Revenue.Where(x => x.OfficerData.TowerData.FlightTypeD == 1 || x.OfficerData.TowerData.FlightTypeL == 1);
                        Revenue = Revenue.Where(x => x.OfficerData.TowerData.FlightTypeD == 1);
                        Revenue = Revenue.Where(x => x.InterNYFees != null);
                    }
                    else
                    {
                      //  Revenue = Revenue.Where(x => x.OfficerData.TowerData.FlightTypeD == 2 && x.OfficerData.TowerData.FlightTypeL == 2);
                        Revenue = Revenue.Where(x => x.OfficerData.TowerData.FlightTypeD == 2);
                        Revenue = Revenue.Where(x => x.DomeNYFees != null);
                    }
                }

                if (AirLineId > 0)
                {
                    Revenue = Revenue.Where(x => x.OfficerData.TowerData.AirLineId == AirLineId);
                }

                if (DateFrom != null && DateTo != null)
                {
                    SearchDateFrom = CommonUsed.CheckDate(DateFrom.ToString());
                    SearchDateTo = CommonUsed.CheckDate(DateTo.ToString());
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




            return PartialView("_GetNy", Revenue.ToPagedList(PageNumber, pageSize));
        }

        public ActionResult PrintDataNy(int? AirLineId, DateTime? DateFrom, DateTime? DateTo, int? FlightTypeId)
        {
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
            ExchangeRate = CommonUsed.getExchangeRateByLandingDate(Month, Year);

            string SearchDateFrom = "غير محدد";
            string SearchDateTo = "غير محدد";
            string MessageDiscount = "";
            bool IsLocalCompany = false;
            ///////////////NYFeesDiscount//////////////////
            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.TakeOffDate);
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
                        }
                        else //means It's a Percent Not value
                        {
                            Value = Convert.ToDecimal(gs.Value);
                            MessageDiscount = "*NY Fees Discount " + Convert.ToDouble(Value) + " %";
                            Discount = "(- " + Convert.ToDouble(Value) + " %)";
                        }
                    }
                }
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
            ReportDocument rd = new ReportDocument();
            if (FlightTypeId == 1)
            {
                if (IsLocalCompany)
                {
                    var RevenueList = new List<NyRptViewModel>();

                    rd.Load(Path.Combine(Server.MapPath("~/Reports/Revenue/RptInterNYFeesDiscount.rpt")));
                    foreach (var item in Revenue)
                    {
                        NyRptViewModel N = new NyRptViewModel();
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
                    //InterNational
                    rd.SetDataSource(RevenueList.Select(p => new
                    {
                        Id = p.Id,
                        Name = p.Name,
                        AirPortTo = p.AirPortTo,
                        InterNYFees = p.InterNYFees,
                        Pax = p.Pax,
                        TakeOffDate = p.TakeOffDate,
                        DomeNYFees = p.DomeNYFees,
                        FlightNo = p.FlightNo,
                        InterFees = p.InterFees,
                        SearchDateFrom = SearchDateFrom,
                        SearchDateTo = SearchDateTo,
                        AmountSDG = p.AmountSDG,
                        ExchangeRate = ExchangeRate,
                        MessageDiscount = MessageDiscount,
                        Discount = Discount,
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
                        return File(Stream, "application/xls", "RptInterNYFees.xls");
                    }
                    else
                    {
                        Response.Buffer = false;
                        Response.ClearContent();
                        Response.ClearHeaders();
                        Stream Stream = rd.ExportToStream
                        //WordForWindows
                        (CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                        Stream.Seek(0, SeekOrigin.Begin);//application/doc
                        return File(Stream, "application/pdf");
                    }
                }
                else
                {
                    var RevenueList = new List<NyRptViewModel>();

                    rd.Load(Path.Combine(Server.MapPath("~/Reports/Revenue/RptInterNYFees.rpt")));
                    foreach (var item in Revenue)
                    {
                        NyRptViewModel N = new NyRptViewModel();
                        N.Pax = Convert.ToInt32(item?.OfficerData.Pax);
                        N.Id = item.Id;
                        N.Name = item?.OfficerData.TowerData.AirLine.Code + "/  " + item?.OfficerData.TowerData.AirLine.Name;
                        N.AirPortTo = item?.OfficerData.TowerData.AirPort.Code;
                        N.TakeOffDate = CommonUsed.CheckDate(item?.OfficerData.TowerData.LandingDate.ToString());
                        N.FlightNo = item?.OfficerData.TowerData.AirLinesFlightTrip.FlightNo;
                        /////IsLocalCompany
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
                    //InterNational
                    rd.SetDataSource(RevenueList.Select(p => new
                    {
                        Id = p.Id,
                        Name = p.Name,
                        AirPortTo = p.AirPortTo,
                        InterNYFees = p.InterNYFees,
                        Pax = p.Pax,
                        TakeOffDate = p.TakeOffDate,
                        DomeNYFees = p.DomeNYFees,
                        FlightNo = p.FlightNo,
                        InterFees = p.InterFees,
                        SearchDateFrom = SearchDateFrom,
                        SearchDateTo = SearchDateTo,
                        AmountSDG = p.AmountSDG,
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
                        return File(Stream, "application/xls", "RptInterNYFees.xls");
                    }
                    else
                    {
                        Response.Buffer = false;
                        Response.ClearContent();
                        Response.ClearHeaders();
                        Stream Stream = rd.ExportToStream
                            //WordForWindows
                            (CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                        Stream.Seek(0, SeekOrigin.Begin);//application/doc
                        return File(Stream, "application/pdf", "RptInterNYFees.pdf");
                    }
                }
            }
            else
            {//Domestic
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Revenue/RptDomeNYFeesDiscount.rpt")));
                var RevenueList = new List<NyRptViewModel>();

                if (IsLocalCompany) //Domestic && LocalCompany
                {
                    foreach (var item in Revenue)
                    {
                        NyRptViewModel N = new NyRptViewModel();
                        N.Pax = Convert.ToInt32(item?.OfficerData.Pax);
                        N.Id = item.Id;
                        N.Name = item?.OfficerData.TowerData.AirLine.Code + "/ " + item?.OfficerData.TowerData.AirLine.Name;
                        N.AirPortTo = item?.OfficerData.TowerData.AirPort.Code;
                        N.TakeOffDate = CommonUsed.CheckDate(item?.OfficerData.TowerData.LandingDate.ToString());
                        N.FlightNo = item?.OfficerData.TowerData.AirLinesFlightTrip.FlightNo;
                        N.DomeNYFees = Convert.ToDecimal(item?.DomeNYFees);
                        if (N.DomeNYFees == 0)
                        {
                            N.DomeFees = 0;
                        }
                        else
                        {
                            N.DomeFees = (N.DomeNYFees / N.Pax);
                        }

                        N.AmountDollar = N.DomeNYFees / ExchangeRate;
                        RevenueList.Add(N);
                    }

                    rd.SetDataSource(RevenueList.Select(p => new
                    {
                        Id = p.Id,
                        Name = p.Name,
                        AirPortTo = p.AirPortTo,
                        InterNYFees = p.InterNYFees,
                        Pax = p.Pax,
                        TakeOffDate = p.TakeOffDate,
                        DomeNYFees = p.DomeNYFees,
                        FlightNo = p.FlightNo,
                        SearchDateFrom = SearchDateFrom,
                        SearchDateTo = SearchDateTo,
                        DomeFees = p.DomeFees,
                        AmountSDG = p.AmountSDG,
                        ExchangeRate = ExchangeRate,
                        AmountDollar = p.AmountDollar,
                        MessageDiscount = MessageDiscount,
                        Discount = Discount

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
                        return File(Stream, "application/xls", "NYFees.xls");
                    }
                    else
                    {
                        Response.Buffer = false;
                        Response.ClearContent();
                        Response.ClearHeaders();
                        Stream Stream = rd.ExportToStream
                            //WordForWindows
                            (CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                        Stream.Seek(0, SeekOrigin.Begin);//application/doc
                        return File(Stream, "application/pdf", "NYFees.pdf");
                    }
                }
                else
                {
                    foreach (var item in Revenue)
                    {
                        NyRptViewModel N = new NyRptViewModel();
                        N.Pax = Convert.ToInt32(item?.OfficerData.Pax);
                        N.Id = item.Id;
                        N.Name = item?.OfficerData.TowerData.AirLine.Name + "/ " + item?.OfficerData.TowerData.AirLine.Name;
                        N.AirPortTo = item?.OfficerData.TowerData.AirPort.Code;
                        N.TakeOffDate = CommonUsed.CheckDate(item?.OfficerData.TowerData.LandingDate.ToString());
                        N.FlightNo = item?.OfficerData.TowerData.AirLinesFlightTrip.FlightNo;
                        N.DomeNYFees = Convert.ToDecimal(item?.DomeNYFees);
                        if (N.DomeNYFees == 0)
                        {
                            N.DomeFees = 0;
                        }
                        else
                        {
                            N.DomeFees = (N.DomeNYFees / N.Pax);
                        }

                        N.AmountDollar = N.DomeNYFees / ExchangeRate;
                        RevenueList.Add(N);
                    }

                    rd.SetDataSource(RevenueList.Select(p => new
                    {
                        Id = p.Id,
                        Name = p.Name,
                        AirPortTo = p.AirPortTo,
                        InterNYFees = p.InterNYFees,
                        Pax = p.Pax,
                        TakeOffDate = p.TakeOffDate,
                        DomeNYFees = p.DomeNYFees,
                        FlightNo = p.FlightNo,
                        SearchDateFrom = SearchDateFrom,
                        SearchDateTo = SearchDateTo,
                        DomeFees = p.DomeFees,
                        AmountSDG = p.AmountSDG,
                        ExchangeRate = ExchangeRate,
                        AmountDollar = p.AmountDollar,

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
                        return File(Stream, "application/xls", "NYFees.xls");
                    }
                    else
                    {
                        Response.Buffer = false;
                        Response.ClearContent();
                        Response.ClearHeaders();
                        Stream Stream = rd.ExportToStream
                            //WordForWindows
                            (CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                        Stream.Seek(0, SeekOrigin.Begin);//application/doc
                        return File(Stream, "application/pdf", "NYFees.pdf");
                    }
                }
            }
        }
        /******************************************************* CUPPS *************************************************************************/
        public ActionResult GetDataCupps(int? page, int? Get, int? Search, int? Count, int? AirLineId, DateTime? DateFrom, DateTime? DateTo
                                        , int? FlightTypeId)
        {
            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date);

            ViewBag.AirLineId = AirLineId;


            ViewBag.DateFrom = DateFrom;
            ViewBag.DateTo = DateTo;

            ViewBag.FlightTypeId = FlightTypeId;

            if (Get > 0)
            {

                if (FlightTypeId > 0)
                {
                    if (FlightTypeId == 1)
                    {
                        Revenue = Revenue.Where(x => x.OfficerData.TowerData.FlightTypeD == 1 /*|| x.OfficerData.TowerData.FlightTypeL == 1*/);
                        Revenue = Revenue.Where(x => x.InterCUPPSFees != null);
                    }
                    else
                    {
                        Revenue = Revenue.Where(x => x.OfficerData.TowerData.FlightTypeD == 2 /*&& x.OfficerData.TowerData.FlightTypeL == 2*/);
                        Revenue = Revenue.Where(x => x.DomeCUPPSFees != null);
                    }
                }
                if (AirLineId > 0)
                {
                    Revenue = Revenue.Where(x => x.OfficerData.TowerData.AirLineId == AirLineId);
                }

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




            return PartialView("_GetCupps", Revenue.ToPagedList(PageNumber, pageSize));
        }
        //Cupps
        public ActionResult PrintDataCupps(int? AirLineId, DateTime? DateFrom, DateTime? DateTo, int? FlightTypeId)
        {
            var id = User.Identity.GetUserId();
            RoleController role = new RoleController();
            int GetCompanyId = role.GetCompanyId(id);
            var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);
            // decimal ExchangeRate = Convert.ToDecimal(db.Currencies.Single(x => x.Id == 1).ExchangeRateAgainstPrimaryCurrency);

            string UserName = CommonUsed.GetUserName(id);
            string SearchAirLineId = "غير محدد";

            string SearchDateFrom = "غير محدد";
            string SearchDateTo = "غير محدد";


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
            ExchangeRate = CommonUsed.getExchangeRateByLandingDate(Month, Year);

            ReportDocument rd = new ReportDocument();
            IQueryable<Revenue> Revenue;
            if (FlightTypeId == 1)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Revenue/RptInterCuppsFees.rpt")));
                Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.TakeOffDate);
                Revenue = Revenue.Where(x => x.OfficerData.TowerData.FlightTypeD == 1 /*|| x.OfficerData.TowerData.FlightTypeL == 1*/);
                Revenue = Revenue.Where(x => x.InterCUPPSFees != null);

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
                //**********************************************************************************
                var RevenueListInter = new List<CuppsRptViewModel>();
                foreach (var item in Revenue)
                {
                    CuppsRptViewModel CInter = new CuppsRptViewModel();
                    //545228

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
                rd.SetDataSource(RevenueListInter.Select(p => new
                {
                    Id = p.Id,
                    Name = p.Name,
                    AirPortTo = p.AirPortTo,
                    InterCuppsFees = p.InterCuppsFees,
                    Pax = p.Pax,
                    TakeOffDate = p.TakeOffDate,
                    TR = p.TR,
                    FlightNo = p.FlightNo,
                    InterFees = p.InterFees,
                    SearchDateFrom = SearchDateFrom,
                    SearchDateTo = SearchDateTo,
                    CUPPSSDG = p.CUPPSSDG,
                    ExchangeRate = ExchangeRate,
                }).ToList());
                if (Format == 2) ///excel
                {
                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();
                    Stream Stream = rd.ExportToStream
                        //WordForWindows //PortableDocFormat //PortableDocFormat
                        (CrystalDecisions.Shared.ExportFormatType.Excel);
                    Stream.Seek(0, SeekOrigin.Begin);//application/doc //
                    return File(Stream, "application/xls ", "InterCuppsFees.xls");
                }
                else
                {
                    //pdf
                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();
                    Stream Stream = rd.ExportToStream
                    //WordForWindows //PortableDocFormat //PortableDocFormat
                    (CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    Stream.Seek(0, SeekOrigin.Begin);//application/doc //
                    return File(Stream, "application/doc ");
                }
            }
            else
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Revenue/RptDomeCuppsFees.rpt")));

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
                var RevenueList = new List<CuppsRptViewModel>();
                foreach (var item in Revenue)
                {
                    CuppsRptViewModel C = new CuppsRptViewModel();
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
                rd.SetDataSource(RevenueList.Select(p => new
                {
                    Id = p.Id,
                    Name = p.Name,
                    AirPortTo = p.AirPortTo,
                    DomeCuppsFees = p.DomeCuppsFees,
                    Pax = p.Pax,
                    TR = p.TR,
                    TakeOffDate = p.TakeOffDate,
                    FlightNo = p.FlightNo,
                    DomeFees = p.DomeFees,
                    SearchDateFrom = SearchDateFrom,
                    SearchDateTo = SearchDateTo,
                    CUPPSSDG = p.CUPPSSDG,
                    ExchangeRate = ExchangeRate,
                }).ToList());

                if (Format == 2) ///excel
                {
                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();
                    Stream Stream = rd.ExportToStream
                    //WordForWindows //PortableDocFormat //PortableDocFormat
                    (CrystalDecisions.Shared.ExportFormatType.Excel);
                    Stream.Seek(0, SeekOrigin.Begin);//application/doc //
                    return File(Stream, "application/xls ", "CuppsFees.xls");
                }
                else
                {
                    //pdf
                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();
                    Stream Stream = rd.ExportToStream
                    //WordForWindows //PortableDocFormat //PortableDocFormat
                    (CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    Stream.Seek(0, SeekOrigin.Begin);//application/doc //
                    return File(Stream, "application/doc ");
                }
            }
        }
        /***************************************************** Service Car ***************************************************************************/
        public ActionResult GetDataServiceCar(int? page, int? Get, int? Search, int? Count, int? AirLineId
            , DateTime? DateFrom, DateTime? DateTo)
        {
            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date).Where(x => x.AmbolanceFees > 0 || x.FireFightingFees > 0);

            ViewBag.AirLineId = AirLineId;


            ViewBag.DateFrom = DateFrom;
            ViewBag.DateTo = DateTo;

            if (Get > 0)
            {
                if (AirLineId > 0)
                {
                    Revenue = Revenue.Where(x => x.OfficerData.TowerData.AirLineId == AirLineId);
                }

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

                        ItemCount = Revenue.Count();
                        ViewBag.ItemCount = ItemCount;

                    }
                }
                else
                {
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


            return PartialView("_GetServiceCar", Revenue.ToPagedList(PageNumber, pageSize));
        }

        //ServiceCar
        public ActionResult PrintDataServiceCar(int? AirLineId, DateTime? DateFrom, DateTime? DateTo, int? FlightTypeId)
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

            rd.Load(Path.Combine(Server.MapPath("~/Reports/Revenue/RptServiceCarFees.rpt")));

            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date).Where(x => x.AmbolanceFees > 0 || x.FireFightingFees > 0);

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
            var RevenueList = new List<CarServiceRptViewModel>();
            foreach (var item in Revenue)
            {
                CarServiceRptViewModel C = new CarServiceRptViewModel();
                C.Id = item.Id;
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

            /************************************************************************************/

            rd.SetDataSource(RevenueList.Select(p => new
            {
                Id = p.Id,
                Name = p.Name,
                FlightNo = p.FlightNo,
                LandingDate = p.LandingDate,
                FeesAmbulance = p.FeesAmbulance,
                FeesAmbulanceAmount = p.FeesAmbulanceAmount,
                AmbulanceNo = p.AmbulanceNo,
                FeesFire = p.FeesFire,
                FeesFireAmount = p.FeesFireAmount,
                SearchDateFrom = SearchDateFrom,
                SearchDateTo = SearchDateTo,
                FireNo = p.FireNo,
                FireSDG = p.FireSDG,
                AmbulanceSDG = p.AmbulanceSDG,
                ExchangeRate = ExchangeRate
            }).ToList());
            if (Format == 2) ///excel
            {
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                Stream Stream = rd.ExportToStream
                    //WordForWindows //PortableDocFormat //PortableDocFormat
                    (CrystalDecisions.Shared.ExportFormatType.Excel);
                Stream.Seek(0, SeekOrigin.Begin);//application/doc //
                return File(Stream, "application/xls ", "ServiceCar.xls");
            }
            else
            {
                //pdf
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                Stream Stream = rd.ExportToStream
                    //WordForWindows //PortableDocFormat //PortableDocFormat
                    (CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                Stream.Seek(0, SeekOrigin.Begin);//application/doc //
                return File(Stream, "application/doc ");
            }
        }
        /******************************************************** Landing ************************************************************************/
        public ActionResult GetDataLanding(int? page, int? Get, int? Search, int? Count, int? AirLineId
           , DateTime? DateFrom, DateTime? DateTo, int? FlightTypeId)
        {
            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date);

            ViewBag.AirLineId = AirLineId;
            ViewBag.DateFrom = DateFrom;
            ViewBag.DateTo = DateTo;
            ViewBag.FlightTypeId = FlightTypeId;

            if (Get > 0)
            {

                if (FlightTypeId > 0)
                {
                    if (FlightTypeId == 1)
                    {
                      //  Revenue = Revenue.Where(x => x.OfficerData.TowerData.FlightTypeD == 1 || x.OfficerData.TowerData.FlightTypeL == 1);
                        Revenue = Revenue.Where(x => x.OfficerData.TowerData.FlightTypeL == 1);

                    }
                    else
                    {
                        //   Revenue = Revenue.Where(x => x.OfficerData.TowerData.FlightTypeD == 2 && x.OfficerData.TowerData.FlightTypeL == 2);
                        Revenue = Revenue.Where(x => x.OfficerData.TowerData.FlightTypeL == 2);

                    }
                }

                if (AirLineId > 0)
                {
                    Revenue = Revenue.Where(x => x.OfficerData.TowerData.AirLineId == AirLineId);
                }

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

            int pageSize = 20;
            int PageNumber = (page ?? 1);

            return PartialView("_GetLanding", Revenue.ToPagedList(PageNumber, pageSize));
        }

        //ServiceCar
        public ActionResult PrintDataLanding(int? AirLineId, DateTime? DateFrom, DateTime? DateTo, int? FlightTypeId)
        {
            var id = User.Identity.GetUserId();
            RoleController role = new RoleController();
            int GetCompanyId = role.GetCompanyId(id);
            var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);

            string UserName = CommonUsed.GetUserName(id);
            bool IsLocalCompany = false;

            string SearchDateFrom = "غير محدد";
            string SearchDateTo = "غير محدد";
            string MessageDiscount = "";
            string Discount = "";

            ReportDocument rd = new ReportDocument();

            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date);

            if (FlightTypeId > 0)
            {
                if (FlightTypeId == 1)
                {
                    // Revenue = Revenue.Where(x => x.OfficerData.TowerData.FlightTypeD == 1 || x.OfficerData.TowerData.FlightTypeL == 1);
                    Revenue = Revenue.Where(x => x.OfficerData.TowerData.FlightTypeL == 1);
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/Revenue/RptLandingFees.rpt")));
                }
                else
                {
                    Revenue = Revenue.Where(x => x.OfficerData.TowerData.FlightTypeL == 2);
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/Revenue/RptLandingFeesDom.rpt")));
                }
            }

            if (AirLineId > 0)
            {
                Revenue = Revenue.Where(x => x.OfficerData.TowerData.AirLineId == AirLineId);
                // SearchAirLineId = db.AirLines.Where(x => x.Id == AirLineId).Single().Name;

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
                        }
                        else //means It's a Percent Not value
                        {
                            Value = Convert.ToDecimal(gs.Value);
                            MessageDiscount = " *Discount " + Convert.ToDouble(Value) + "% from Security Fees";
                            Discount = "(- " + Convert.ToDouble(Value) + "%)";
                        }
                    }
                }
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
            var RevenueList = new List<LandingRptViewModel>();
            foreach (var item in Revenue)
            {
                LandingRptViewModel L = new LandingRptViewModel();
                L.Id = item.Id;
                L.Name = item?.OfficerData.TowerData.AirLine.Code + "/ " + item?.OfficerData.TowerData.AirLine.Name;
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
            //************************************************************************************/
            rd.SetDataSource(RevenueList.Select(p => new
            {
                Id = p.Id,
                Name = p.Name,
                FlightNo = p.FlightNo,
                LandingDate = p.LandingDate,

                ParkingFees = p.ParkingFees,
                LandingFees = p.LandingFees,
                SecurityFees = p.SecurityFees,

                NavigationFees = p.NavigationFees,
                NightOperationFees = p.NightOperationFees,

                FreightFees = p.FreightFees,
                NoiseFees = p.NoiseFees,
                ImpMailFees = p.ImpMailFees,
                ExpMailFees = p.ExpMailFees,
                SearchDateFrom = SearchDateFrom,
                SearchDateTo = SearchDateTo,
                Total = p.Total,

                Wt = p.Wt,
                RegNo = p.RegNo,
                Type = p.Type,
                MessageDiscount = MessageDiscount,
                Discount = Discount,
            }).ToList());

            if (Format == 2) ///excel
            {
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                Stream Stream = rd.ExportToStream
                    //WordForWindows //PortableDocFormat //PortableDocFormat
                    (CrystalDecisions.Shared.ExportFormatType.Excel);
                Stream.Seek(0, SeekOrigin.Begin);//application/doc //
                return File(Stream, "application/xls ", "LandingFees.xls");
            }
            else
            {
                //pdf
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                Stream Stream = rd.ExportToStream
                    //WordForWindows //PortableDocFormat //PortableDocFormat
                    (CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                Stream.Seek(0, SeekOrigin.Begin);//application/doc //
                return File(Stream, "application/doc ");
            }
        }
        /************************************************************ Mail & Frieght ********************************************************************/
        public ActionResult GetDataMail(int? page, int? Get, int? Search, int? Count, int? AirLineId
           , DateTime? DateFrom, DateTime? DateTo)
        {
            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date).Where(x => x.FreightFees > 0 || x.ExpMailFees > 0 ||
             x.ImpMailFees > 0);

            ViewBag.AirLineId = AirLineId;


            ViewBag.DateFrom = DateFrom;
            ViewBag.DateTo = DateTo;

            if (Get > 0)
            {
                if (AirLineId > 0)
                {
                    Revenue = Revenue.Where(x => x.OfficerData.TowerData.AirLineId == AirLineId);
                }

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

                        ItemCount = Revenue.Count();
                        ViewBag.ItemCount = ItemCount;

                    }
                }
                else
                {
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


            return PartialView("_GetMail", Revenue.ToPagedList(PageNumber, pageSize));
        }

        //ServiceCar
        public ActionResult PrintDataMail(int? AirLineId, DateTime? DateFrom, DateTime? DateTo)
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

            rd.Load(Path.Combine(Server.MapPath("~/Reports/Revenue/RptMailFees.rpt")));

            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date).Where(x => x.FreightFees > 0 || x.ExpMailFees > 0 ||
            x.ImpMailFees > 0);

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
            /***********************************************************************************/
            var RevenueList = new List<MailRptViewModel>();
            foreach (var item in Revenue)
            {
                MailRptViewModel C = new MailRptViewModel();
                C.Id = item.Id;
                C.Name = item?.OfficerData.TowerData.AirLine.Code + "/ " + item?.OfficerData.TowerData.AirLine.Name;
                C.FlightNo = item?.OfficerData.TowerData.AirLinesFlightTrip.FlightNo;
                C.LandingDate = CommonUsed.CheckDate(item?.OfficerData.TowerData.LandingDate.ToString());
                C.NormalMailLoadingExp = Convert.ToInt32(item?.OfficerData.NormalMailLoadingExp);
                C.RapidMailLoadingExp = Convert.ToInt32(item?.OfficerData.RapidMailLoadingExp);
                C.NormalMailLoadingImp = Convert.ToInt32(item?.OfficerData.NormalMailLoadingImp);
                C.RapidMailLoadingImp = Convert.ToInt32(item?.OfficerData.RapidMailLoadingImp);
                C.FreightLoadingExp = Convert.ToInt32(item?.OfficerData.FreightLoadingExp);
                C.FreightLoadingImp = Convert.ToInt32(item?.OfficerData.FreightLoadingImp);
                C.FreightFees = Convert.ToDecimal(item?.FreightFees);
                C.ExpMailFees = Convert.ToDecimal(item?.ExpMailFees);
                C.ImpMailFees = Convert.ToDecimal(item?.ImpMailFees);

                RevenueList.Add(C);
            }
            /************************************************************************************/
            rd.SetDataSource(RevenueList.Select(p => new
            {
                Id = p.Id,
                Name = p.Name,
                FlightNo = p.FlightNo,
                LandingDate = p.LandingDate,
                NormalMailLoadingExp = p.NormalMailLoadingExp,
                RapidMailLoadingExp = p.RapidMailLoadingExp,

                NormalMailLoadingImp = p.NormalMailLoadingImp,
                RapidMailLoadingImp = p.RapidMailLoadingImp,

                FreightLoadingExp = p.FreightLoadingExp,
                FreightLoadingImp = p.FreightLoadingImp,

                FreightFees = p.FreightFees,

                ExpMailFees = p.ExpMailFees,
                ImpMailFees = p.ImpMailFees,

                SearchDateFrom = SearchDateFrom,
                SearchDateTo = SearchDateTo,

                ExchangeRate = ExchangeRate,

            }).ToList());
            if (Format == 2) ///excel
            {
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                Stream Stream = rd.ExportToStream
                    //WordForWindows //PortableDocFormat //PortableDocFormat
                    (CrystalDecisions.Shared.ExportFormatType.Excel);
                Stream.Seek(0, SeekOrigin.Begin);//application/doc //
                return File(Stream, "application/xls ", "ServiceCar.xls");
            }
            else
            {//pdf

                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                Stream Stream = rd.ExportToStream
                    //WordForWindows //PortableDocFormat //PortableDocFormat
                    (CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                Stream.Seek(0, SeekOrigin.Begin);//application/doc //
                return File(Stream, "application/doc ");
            }
        }
        /******************************************************** First Class ************************************************************************/
        public ActionResult GetDataFirstClass(int? page, int? Get, int? Search, int? Count, int? AirLineId
          , DateTime? DateFrom, DateTime? DateTo)
        //, int? FlightTypeId)
        {
            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date).Where(x => x.FirstClassFees > 0);

            ViewBag.AirLineId = AirLineId;


            ViewBag.DateFrom = DateFrom;
            ViewBag.DateTo = DateTo;

            //  ViewBag.FlightTypeId = FlightTypeId;

            if (Get > 0)
            {

                //if (FlightTypeId > 0)
                //{
                //    Revenue = Revenue.Where(x => x.OfficerData.TowerData.FlightType == FlightTypeId);
                //}

                if (AirLineId > 0)
                {
                    Revenue = Revenue.Where(x => x.OfficerData.TowerData.AirLineId == AirLineId);
                }

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




            return PartialView("_GetFirstClass", Revenue.ToPagedList(PageNumber, pageSize));
        }

        public ActionResult PrintDataFirstClass(int? AirLineId, DateTime? DateFrom, DateTime? DateTo/*, int? FlightTypeId*/)
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

            rd.Load(Path.Combine(Server.MapPath("~/Reports/Revenue/RptFirstClassFees.rpt")));

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

            if (Format == 2) ///excel
            {
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                Stream Stream = rd.ExportToStream
                    //WordForWindows //PortableDocFormat //PortableDocFormat
                    (CrystalDecisions.Shared.ExportFormatType.Excel);
                Stream.Seek(0, SeekOrigin.Begin);//application/doc //
                return File(Stream, "application/xls ", "FirstClass.xls");
            }
            else
            {
                //pdf
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders(); 
                 Stream Stream = rd.ExportToStream
                     //WordForWindows //PortableDocFormat //PortableDocFormat
                     (CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                Stream.Seek(0, SeekOrigin.Begin);//application/doc //
                return File(Stream, "application/doc ");
            }
        }

        /************************************************************ PushBack ********************************************************************/
        public ActionResult GetPushPack(int? page, int? Get, int? Search, int? Count, int? AirLineId
         , DateTime? DateFrom, DateTime? DateTo
        )
        {
            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date).Where(x => x.PushBackFees != null && x.PushBackFees > 0);

            ViewBag.AirLineId = AirLineId;
            ViewBag.DateFrom = DateFrom;
            ViewBag.DateTo = DateTo;

            if (Get > 0)
            {
                if (AirLineId > 0)
                {
                    Revenue = Revenue.Where(x => x.OfficerData.TowerData.AirLineId == AirLineId);
                }

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

            return PartialView("_GetPushPack", Revenue.ToPagedList(PageNumber, pageSize));
        }
        public ActionResult PrintPushPack(int? AirLineId, DateTime? DateFrom, DateTime? DateTo)
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

            rd.Load(Path.Combine(Server.MapPath("~/Reports/Revenue/RptPushPackFees.rpt")));

            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date).Where(x => x.PushBackFees != null && x.PushBackFees > 0);

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
            var RevenueList = new List<PushPackRptViewModel>();
            foreach (var item in Revenue)
            {
                PushPackRptViewModel N = new PushPackRptViewModel();

                N.Id = item.Id;
                N.Name = item?.OfficerData.TowerData.AirLine.Name;
                N.LandingDate = CommonUsed.CheckDate(item?.OfficerData.TowerData.LandingDate.ToString());
                N.FlightNo = item?.OfficerData.TowerData.AirLinesFlightTrip.FlightNo;
                N.AirPortTo = item?.OfficerData.TowerData.AirPort.Code;

                N.RegNo = item?.OfficerData.TowerData.AircraftRegistration.Registration;
                N.Type = item?.OfficerData.TowerData.AircraftRegistration.AircraftType.Type;
                N.PlaneSize = item?.OfficerData.TowerData.AircraftRegistration.AircraftType.AircraftSize.Size;
                N.Wt = item?.OfficerData.TowerData.AircraftRegistration.MaxTakoffWieght.ToString();
                N.PushPackFees = Convert.ToDecimal(item?.PushBackFees);
                
                N.AmountSDG = ExchangeRate * N.PushPackFees;
                RevenueList.Add(N);
            }

            ////*************************************************************************************

            rd.SetDataSource(RevenueList.Select(p => new
            {
                Id = p.Id,
                Name = p.Name,
                AirPortTo = p.AirPortTo,

                LandingDate = p.LandingDate,

                FlightNo = p.FlightNo,

                PushPackFees = p.PushPackFees,

                SearchDateFrom = SearchDateFrom,
                SearchDateTo = SearchDateTo,

                Wt = p.Wt,
                RegNo = p.RegNo,
                Type = p.Type,
                PlaneSize = p.PlaneSize,

                AmountSDG = p.AmountSDG,
                ExchangeRate = ExchangeRate,
            }).ToList());

            if (Format == 2) ///excel
            {
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                Stream Stream = rd.ExportToStream
                    //WordForWindows //PortableDocFormat //PortableDocFormat
                    (CrystalDecisions.Shared.ExportFormatType.Excel);
                Stream.Seek(0, SeekOrigin.Begin);//application/doc //
                return File(Stream, "application/xls ", "pushpack.xls");
            }
            else
            {//pdf

                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                Stream Stream = rd.ExportToStream
                    //WordForWindows //PortableDocFormat //PortableDocFormat
                    (CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                Stream.Seek(0, SeekOrigin.Begin);//application/doc //
                return File(Stream, "application/doc ");
            }
        }

        /********************************************************* Final Invoice *******************************************************************/
        public ActionResult GetDataFinalInvoice(int? page, int? Get, int? Search, int? Count, int? AirLineId
          , DateTime? DateFrom, DateTime? DateTo)
        //   , int? FlightTypeId)
        {
            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date);

            ViewBag.AirLineId = AirLineId;


            ViewBag.DateFrom = DateFrom;
            ViewBag.DateTo = DateTo;

            //  ViewBag.FlightTypeId = FlightTypeId;

            if (Get > 0)
            {

                //if (FlightTypeId > 0)
                //{
                //    Revenue = Revenue.Where(x => x.OfficerData.TowerData.FlightType == FlightTypeId);
                //}

                if (AirLineId > 0)
                {
                    Revenue = Revenue.Where(x => x.OfficerData.TowerData.AirLineId == AirLineId);
                }

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




            return PartialView("_GetFinalInvoice", Revenue.ToPagedList(PageNumber, pageSize));
        }

        public ActionResult PrintDataFinalInvoice(int? AirLineId, DateTime? DateFrom, DateTime? DateTo)
        {
            FinalInvoiceNumber finalInvoiceNumber;
            var id = User.Identity.GetUserId();
            RoleController role = new RoleController();
            int GetCompanyId = role.GetCompanyId(id);
            var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);

            string UserName = CommonUsed.GetUserName(id);
            int InvoiceNo = 0;
            string txtInvoiceNo = "";
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
            ExchangeRate = CommonUsed.getExchangeRateByLandingDate(Month, Year);


            string SearchAirLineId = "غير محدد";
            string SearchDateFrom = "غير محدد";
            string SearchDateTo = "غير محدد";
            ReportDocument rd = new ReportDocument();

            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date);

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
            var RevenueList = new List<FinalInvoiceRptViewModel>();

            FinalInvoiceRptViewModel F = new FinalInvoiceRptViewModel();

            F.Name = db.AirLines.Where(x => x.Id == AirLineId).Single().Code + "/ " + db.AirLines.Where(x => x.Id == AirLineId).Single().Name;
            F.LandingFees = Convert.ToDecimal(Revenue.Sum(x => x.LandingFees));
            F.ParkingFees = Convert.ToDecimal(Revenue.Sum(x => x.ParkingFees));
            F.NightOperationFees = Convert.ToDecimal(Revenue.Sum(x => x.NightOperationFees));
            F.NavigationFees = Convert.ToDecimal(Revenue.Sum(x => x.NavigationFees));

            F.SecurityFees = Convert.ToDecimal(Revenue.Sum(x => x.SecurityFees));
            F.FirstClassFees = Convert.ToDecimal(Revenue.Sum(x => x.FirstClassFees));

            F.InterCUPPSFees = Convert.ToDecimal(Revenue.Sum(x => x.InterCUPPSFees));
            F.DomeCUPPSFees = Convert.ToDecimal(Revenue.Sum(x => x.DomeCUPPSFees));

            F.InterNYFees = Convert.ToDecimal(Revenue.Sum(x => x.InterNYFees));
            F.DomeNYFees = Convert.ToDecimal(Revenue.Sum(x => x.DomeNYFees) / ExchangeRate);

            F.PushBackFees = Convert.ToDecimal(Revenue.Sum(x => x.PushBackFees));
            F.ServiceCars = Convert.ToDecimal(Revenue.Sum(x => x.AmbolanceFees))
                + Convert.ToDecimal(Revenue.Sum(x => x.FireFightingFees));

            F.NoiseFees = Convert.ToDecimal(Revenue.Sum(x => x.NoiseFees));
            F.ExpMailFees = Convert.ToDecimal(Revenue.Sum(x => x.ExpMailFees));
            F.ImpMailFees = Convert.ToDecimal(Revenue.Sum(x => x.ImpMailFees));
            F.FreightFees = Convert.ToDecimal(Revenue.Sum(x => x.FreightFees));

            F.SubTotal = F.LandingFees + F.ParkingFees + F.NightOperationFees + F.NavigationFees + F.SecurityFees +
                F.FirstClassFees + F.FreightFees + F.ServiceCars + F.InterCUPPSFees + F.DomeCUPPSFees + F.InterNYFees +
                F.DomeNYFees + F.PushBackFees + F.ExpMailFees + F.ImpMailFees // + F.OpenAireportOvertimeFees
            + F.NoiseFees;

            RevenueList.Add(F);

            int CurrencyType = Convert.ToInt32(db.AirLines.Where(x => x.Id == AirLineId).Single().CurrencyType);
            if (CurrencyType == 1)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Revenue/RptFinalInvoice.rpt")));
            }
            else
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Revenue/RptFinalInvoiceNotSDG.rpt")));
            }

            if (db.FinalInvoiceNumbers.Any(x => x.Month == Month && x.Year == Year))
            {
                if (db.FinalInvoiceNumbers.Any(x => x.AirLineId == AirLineId && x.Month == Month && x.Year == Year))
                {
                    InvoiceNo = Convert.ToInt32(db.FinalInvoiceNumbers.SingleOrDefault(x => x.AirLineId == AirLineId && x.Month == Month && x.Year == Year).InvoiceNo);
                    //FinalInvoiceNumber = db.FinalInvoiceNumbers.SingleOrDefault(x => x.AirLineId == AirLineId && x.Month == Month && x.Year == Year).InvoiceNo;
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

            txtInvoiceNo = "#" + Month.ToString("00") + Year.ToString() + "00" + InvoiceNo.ToString();

            ////*************************************************************************************
            String HeaderDate = Convert.ToDateTime(DateFrom).ToString("MMM-yyyy");
            rd.SetDataSource(RevenueList.Select(p => new
            {
                Id = p.Id,
                Name = p.Name,
                ParkingFees = p.ParkingFees,

                LandingFees = p.LandingFees,

                NavigationFees = p.NavigationFees,
                NightOperationFees = p.NightOperationFees,

                SecurityFees = p.SecurityFees,

                FreightFees = p.FreightFees,
                FirstClassFees = p.FirstClassFees,

                FireFightingFees = p.FireFightingFees,
                AmbolanceFees = p.AmbolanceFees,

                InterCUPPSFees = p.InterCUPPSFees,
                DomeCUPPSFees = p.DomeCUPPSFees,

                InterNYFees = p.InterNYFees,
                DomeNYFees = p.DomeNYFees,

                ExpMailFees = p.ExpMailFees,
                ImpMailFees = p.ImpMailFees,

                OpenAireportOvertimeFees = p.OpenAireportOvertimeFees,
                NoiseFees = p.NoiseFees,
                SubTotal = p.SubTotal,

                SearchDateFrom = SearchDateFrom,
                SearchDateTo = SearchDateTo,

                ExchangeRate = ExchangeRate,

                ServiceCars = p.ServiceCars,
                PushBackFees = p.PushBackFees,
                HeaderDate = HeaderDate,

                txtInvoiceNo = txtInvoiceNo

            }).ToList());

            if (Format == 2)
            {
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                Stream Stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
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

        public ActionResult PrintDataTax(DateTime? DateFrom, DateTime? DateTo)
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
            //string SearchFlightNo = "غير محدد";
            //string SearchAirPortIdFrom = "غير محدد";
            //string SearchAirPortIdTo = "غير محدد";
            string SearchDateFrom = "غير محدد";
            string SearchDateTo = "غير محدد";
            //string SearchRegId = "غير محدد";
            //  string SearchFlightTypeId = "غير محدد";
            ReportDocument rd = new ReportDocument();

            rd.Load(Path.Combine(Server.MapPath("~/Reports/Revenue/RptFinalTax.rpt")));
            IQueryable<Revenue> Revenue;
            IQueryable<Revenue> RevenueTax;

            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date);


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
            /// 

            var RevenueList = new List<FinalRptViewModelTax>();
            foreach (var AirlineItem in db.vwTopMaxAirLines.ToList().Where(x => x.Count >= 1))
            {
                RevenueTax = Revenue.Where(x => x.OfficerData.TowerData.AirLine.Id == AirlineItem.Id);

                FinalRptViewModelTax R = new FinalRptViewModelTax();


                R.Name = AirlineItem.Name;
                R.LandingFees = Convert.ToDecimal(RevenueTax.Sum(x => x.LandingFees));
                R.ParkingFees = Convert.ToDecimal(RevenueTax.Sum(x => x.ParkingFees));
                R.NightOperationFees = Convert.ToDecimal(RevenueTax.Sum(x => x.NightOperationFees));
                R.NavigationFees = Convert.ToDecimal(RevenueTax.Sum(x => x.NavigationFees));

                R.SecurityFees = Convert.ToDecimal(RevenueTax.Sum(x => x.SecurityFees));
                R.FirstClassFees = Convert.ToDecimal(RevenueTax.Sum(x => x.FirstClassFees));

                R.InterCUPPSFees = Convert.ToDecimal(RevenueTax.Sum(x => x.InterCUPPSFees));
                R.DomeCUPPSFees = Convert.ToDecimal(RevenueTax.Sum(x => x.DomeCUPPSFees));

                R.InterNYFees = Convert.ToDecimal(RevenueTax.Sum(x => x.InterNYFees));
                R.DomeNYFees = Convert.ToDecimal(RevenueTax.Sum(x => x.DomeNYFees));

                R.PushBackFees = Convert.ToDecimal(RevenueTax.Sum(x => x.PushBackFees));
                R.ServiceCars = Convert.ToDecimal(RevenueTax.Sum(x => x.AmbolanceFees))
                    + Convert.ToDecimal(RevenueTax.Sum(x => x.FireFightingFees));

                R.NoiseFees = Convert.ToDecimal(RevenueTax.Sum(x => x.NoiseFees));
                R.ExpMailFees = Convert.ToDecimal(RevenueTax.Sum(x => x.ExpMailFees));
                R.ImpMailFees = Convert.ToDecimal(RevenueTax.Sum(x => x.ImpMailFees));
                R.FreightFees = Convert.ToDecimal(RevenueTax.Sum(x => x.FreightFees));

                R.SubTotal = R.LandingFees + R.ParkingFees + R.NightOperationFees + R.NavigationFees + R.SecurityFees +
                    R.FirstClassFees + R.FreightFees + R.ServiceCars + R.InterCUPPSFees + R.DomeCUPPSFees + R.InterNYFees +
                    R.DomeNYFees + R.PushBackFees + R.ExpMailFees + R.ImpMailFees // + F.OpenAireportOvertimeFees
                + R.NoiseFees;
                R.Tax = (R.SubTotal * 5) / 100;


                RevenueList.Add(R);
            }

            ////*************************************************************************************
            String HeaderDate = Convert.ToDateTime(DateFrom).ToString("MMM-yyyy");
            rd.SetDataSource(RevenueList.Select(p => new
            {
                Id = p.Id,
                Name = p.Name,
                Tax = p.Tax,
                SubTotal = p.SubTotal,
                SearchDateTo = SearchDateTo,
                ExchangeRate = ExchangeRate,
                SearchDateFrom = SearchDateFrom,

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
                return File(Stream, "application/xls", "Total Tax Report.xls");
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
                return File(Stream, "application/pdf", "Total Tax Report.pdf");
            }
        }

        /********************************************************* New Report **********************************************************************/
        public ActionResult NewReport()
        {
            return View();
        }

        public ActionResult PrintDataTotalCompaniesInterCUPPS(DateTime DateFrom, String[] SelectedAirLineIds)
        {
            var id = User.Identity.GetUserId();
            RoleController role = new RoleController();
            int GetCompanyId = role.GetCompanyId(id);
            var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);

            string UserName = CommonUsed.GetUserName(id);

            if (DateFrom != null)
            {
                ExchangeRate = CommonUsed.getExchangeRateByLandingDate(DateFrom.Month, DateFrom.Year);
            }
            else
            {
                //ExchangeRate = CommonUsed.getExchangeRateByLandingDate(currentDate.Month, currentDate.Year);
                return RedirectToAction("Error");
            }

            List<TotalCompaniesInterCUPPS_Result> data = getTotalCompaniesInterCUPPSData(DateFrom);
            /***********************************************************************************/
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports/Revenue/RptTotalCompaniesInterCUPPS.rpt")));

            var ReportList = new List<TotalCompaniesInterCUPPSModel>();
            foreach (var item in data)
            {
                TotalCompaniesInterCUPPSModel C = new TotalCompaniesInterCUPPSModel();
                if (SelectedAirLineIds.Contains(item.Code))
                {
                    C.Id = Convert.ToInt32(item.Id);
                    C.Name = item.Name;
                    C.Code = item.Code;
                    C.Pax = item.Pax;
                    C.ExchRate = ExchangeRate;
                    C.TotalAmountDollar = item.TotalAmountDollar;
                    C.TaxDollar = item.TaxDollar;
                    C.GrandTotalDollar = item.GrandTotalDollar;

                    ReportList.Add(C);
                }
            }
            /*************************************************************************************/
            rd.SetDataSource(ReportList.Select(p => new
            {
                Id = p.Id,
                Name = p.Name,
                Code = p.Code,
                Pax = p.Pax,
                ExchRate = p.ExchRate,
                TotalAmountDollar = p.TotalAmountDollar,
                TaxDollar = p.TaxDollar,
                GrandTotalDollar = p.GrandTotalDollar,
                txtDate = DateFrom.ToString("MMMM") + "/" + DateFrom.Year

                //SearchDateTo = SearchDateTo,
                //ExchangeRate = ExchangeRate,
            }).ToList());

            if (Format == 2) ///excel
            {
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                Stream Stream = rd.ExportToStream
                    //WordForWindows //PortableDocFormat //PortableDocFormat
                    (CrystalDecisions.Shared.ExportFormatType.Excel);
                Stream.Seek(0, SeekOrigin.Begin);//application/excel
                return File(Stream, "application/xls ", "Nyfees.xls");
            }
            else
            {//pdf
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                Stream Stream = rd.ExportToStream
                    //WordForWindows //PortableDocFormat //PortableDocFormat
                    (CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                Stream.Seek(0, SeekOrigin.Begin);//application/pdf
                return File(Stream, "application/doc ", "Nyfees.pdf");
            }
        }

     
        public ActionResult SearchDataTotalCompaniesInterCUPPS(int? page, int? Get, int? Search, int? Count, DateTime? DateFrom)
        {
            List<TotalCompaniesInterCUPPS_Result> Revenue;
            Revenue = getTotalCompaniesInterCUPPSData(DateTime.Now);
            ViewBag.DateFrom = DateFrom;

            if (Get > 0)
            {
                if (DateFrom != null)
                {
                    Revenue = getTotalCompaniesInterCUPPSData(Convert.ToDateTime(DateFrom));
                }

                if (Search == 1 && Revenue != null)
                {
                    if (Revenue.Count() > 0)
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

            int pageSize = 40;
            int PageNumber = (page ?? 1);
            if (Revenue == null)
            {
                return PartialView("_PleaseSearch");
            }
            else
            {
                return PartialView("_GetTotalCompaniesInterCUPPS", Revenue.ToPagedList(PageNumber, pageSize));
            }
        }

        private List<TotalCompaniesInterCUPPS_Result> getTotalCompaniesInterCUPPSData(DateTime DateFrom)
        {
            int Year = DateFrom.Year;
            int Month = DateFrom.Month;
            List<TotalCompaniesInterCUPPS_Result> data = db.TotalCompaniesInterCUPPS(Month, Year).ToList();

            if (data.Count() <= 0)
            {
                return null;
            }

            return data;
        }

      
        /********************************************************* CUUPS Distribtion ***********************************************************************/
        public ActionResult GetDataCuppsDistribtion(int? page, int? Get, int? Search, int? Count
                , DateTime? DateFrom, DateTime? DateTo, int? FlightTypeId)
        {
            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.Where(m=>m.OfficerData.IsCurrentAccount==true).OrderBy(x => x.OfficerData.TowerData.Date);

            //     ViewBag.AirLineId = AirLineId;


            ViewBag.DateFrom = DateFrom;
            ViewBag.DateTo = DateTo;

            ViewBag.FlightTypeId = FlightTypeId;

            if (Get > 0)
            {

                if (FlightTypeId > 0)
                {
                    if (FlightTypeId == 1)
                    {
                        Revenue = Revenue.Where(x => x.OfficerData.TowerData.FlightTypeD == 1 || x.OfficerData.TowerData.FlightTypeL == 1);
                        Revenue = Revenue.Where(x => x.InterCUPPSFees != null);
                    }
                    else
                    {
                        Revenue = Revenue.Where(x => x.OfficerData.TowerData.FlightTypeD == 2 && x.OfficerData.TowerData.FlightTypeL == 2);
                        Revenue = Revenue.Where(x => x.DomeCUPPSFees != null);
                    }
                }
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




            return PartialView("_GetCupps", Revenue.ToPagedList(PageNumber, pageSize));
        }

        public ActionResult PrintDataCuppsDistribtion(/*int? AirLineId,*/ DateTime? DateFrom, DateTime? DateTo, int? FlightTypeId)
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


            string SearchDateFrom = "غير محدد";
            string SearchDateTo = "غير محدد";



            IQueryable<Revenue> Revenue;            
            Revenue = db.Revenues.Where(x => x.OfficerData.IsNotUsedCUPPS == false || x.OfficerData.IsNotUsedCUPPS == null && x.OfficerData.IsCurrentAccount == true).OrderBy(x => x.OfficerData.TowerData.Date);
            if (FlightTypeId > 0)
            {
                if (FlightTypeId == 1)
                {
                    Revenue = Revenue.Where(x => x.OfficerData.TowerData.FlightTypeD == 1 || x.OfficerData.TowerData.FlightTypeL == 1);
                    Revenue = Revenue.Where(x => x.InterCUPPSFees != null);

                }
                else
                {
                    Revenue = Revenue.Where(x => x.OfficerData.TowerData.FlightTypeD == 2 && x.OfficerData.TowerData.FlightTypeL == 2);
                    Revenue = Revenue.Where(x => x.DomeCUPPSFees != null);
                }
            }
            //if (AirLineId > 0)
            //{
            //    Revenue = Revenue.Where(x => x.OfficerData.TowerData.AirLineId == AirLineId);
            //    SearchAirLineId = db.AirLines.Where(x => x.Id == AirLineId).Single().Name;
            //}

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
            //**********************************************************************************

            ReportDocument rd = new ReportDocument();
            if (FlightTypeId == 1)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Revenue/Dist/RptCUPPSInterDistribtion.rpt")));
            }
            else
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Revenue/Dist/RptCUPPSDomeDistribtion.rpt")));
            }


            var RevenueList = new List<CuppsRptViewModelD>();
            //  foreach (var item in Revenue)
            //  {
            CuppsRptViewModelD C = new CuppsRptViewModelD();




            if (FlightTypeId == 1)
            {
                C.Pax = Convert.ToInt32(Revenue.Sum(x => x.OfficerData.Pax));
                C.TR = Convert.ToInt32(Revenue.Sum(x => x.OfficerData.TRNYCUPPS));
                C.AmountDollar = Convert.ToDecimal(Revenue.Sum(x => x.InterCUPPSFees));
                C.AmountSDG = C.AmountDollar * ExchangeRate;
              //  C.TaxDollar = Convert.ToDecimal(C.AmountDollar * Convert.ToDecimal(0.05));
                C.TaxSDG = (Convert.ToDecimal(C.AmountDollar * Convert.ToDecimal(0.05))) * ExchangeRate;
               // C.GrandTotalDollar = C.AmountDollar + C.TaxDollar;
                C.GrandTotalSDG = C.AmountSDG + C.TaxSDG;
                RevenueList.Add(C);
            }
            if (FlightTypeId == 2)
            {
                C.Pax = Convert.ToInt32(Revenue.Sum(x => x.OfficerData.Pax));
                C.TR = Convert.ToInt32(Revenue.Sum(x => x.OfficerData.TRNYCUPPS));
                C.AmountDollar = Convert.ToDecimal(Revenue.Sum(x => x.DomeCUPPSFees));
                  C.AmountSDG = C.AmountDollar * ExchangeRate;
                //    C.TaxDollar = Convert.ToDecimal(C.AmountDollar * Convert.ToDecimal(0.05));
                C.TaxSDG = (Convert.ToDecimal(C.AmountDollar * Convert.ToDecimal(0.05))) * ExchangeRate;
                //    C.GrandTotalDollar = C.AmountDollar + C.TaxDollar;
                C.GrandTotalSDG = C.AmountSDG + C.TaxSDG;


                //  }
                RevenueList.Add(C);

            }

            ///************************************************************************************
            if (FlightTypeId == 1)
            {  //InterNational
                rd.SetDataSource(RevenueList.Select(p => new
                {
                    Pax = p.Pax,
                    TR = p.TR,
                    AmountDollar = p.AmountDollar,
                    AmountSDG = p.AmountSDG,
                   // TaxDollar = p.TaxDollar,

                    TaxSDG = p.TaxSDG,
                   // GrandTotalDollar = p.GrandTotalDollar,
                    GrandTotalSDG = p.GrandTotalSDG,
                    SearchDateFrom = SearchDateFrom,

                    SearchDateTo = SearchDateTo,
                    ExchangeRate = ExchangeRate,

                }).ToList());
                if (Format == 2) ///excel
                {
                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();
                    Stream Stream = rd.ExportToStream
                        //WordForWindows //PortableDocFormat //PortableDocFormat
                        (CrystalDecisions.Shared.ExportFormatType.Excel);
                    Stream.Seek(0, SeekOrigin.Begin);//application/doc //
                    return File(Stream, "application/xls ", "Nyfees.xls");
                }
                else
                {//pdf

                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();
                    Stream Stream = rd.ExportToStream
                        //WordForWindows //PortableDocFormat //PortableDocFormat
                        (CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    Stream.Seek(0, SeekOrigin.Begin);//application/doc //
                    return File(Stream, "application/doc ", "bvbvbbb.pdf");
                }
            }
            else
            {
                //Domestic
                rd.SetDataSource(RevenueList.Select(p => new
                {
                    Pax = p.Pax,
                    TR = p.TR,
                    AmountDollar = p.AmountDollar,
                    AmountSDG = p.AmountSDG,
                    TaxDollar = p.TaxDollar,

                    TaxSDG = p.TaxSDG,
                    GrandTotalDollar = p.GrandTotalDollar,
                    GrandTotalSDG = p.GrandTotalSDG,
                    ExchangeRate = ExchangeRate,
                    SearchDateFrom = SearchDateFrom,

                    SearchDateTo = SearchDateTo,
                }).ToList());

                if (Format == 2) ///excel
                {
                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();
                    Stream Stream = rd.ExportToStream
                        //WordForWindows //PortableDocFormat //PortableDocFormat
                        (CrystalDecisions.Shared.ExportFormatType.Excel);
                    Stream.Seek(0, SeekOrigin.Begin);//application/doc //
                    return File(Stream, "application/xls ", "NyFees.xls");
                }
                else
                {//pdf

                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();
                    Stream Stream = rd.ExportToStream
                        //WordForWindows //PortableDocFormat //PortableDocFormat
                        (CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    Stream.Seek(0, SeekOrigin.Begin);//application/doc //
                    return File(Stream, "application/doc ", "NyFees.pdf");
                }
            }



        }

        public ActionResult PrintDataCuppsDetails(int? AirLineId, DateTime? DateFrom, DateTime? DateTo, int? FlightTypeId)
        {
            var id = User.Identity.GetUserId();
            RoleController role = new RoleController();
            int GetCompanyId = role.GetCompanyId(id);
            var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);
            // decimal ExchangeRate = Convert.ToDecimal(db.Currencies.Single(x => x.Id == 1).ExchangeRateAgainstPrimaryCurrency);

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
            IQueryable<Revenue> Revenue;
            if (FlightTypeId == 1)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Revenue/RptInterCuppsFees.rpt")));

                Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date);

                Revenue = Revenue.Where(x => x.OfficerData.TowerData.FlightTypeD == 1 || x.OfficerData.TowerData.FlightTypeL == 1);
                Revenue = Revenue.Where(x => x.InterCUPPSFees != null);

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
                //**********************************************************************************
                var RevenueListInter = new List<CuppsRptViewModel>();
                foreach (var item in Revenue)
                {
                    CuppsRptViewModel CInter = new CuppsRptViewModel();
                    //545228

                    //  CInter.Pax = Convert.ToInt32(item?.OfficerData.DISEMB + item.OfficerData.DisEmbInft + item?.OfficerData.TRNYCUPPS);
                    CInter.Pax = Convert.ToInt32(item?.OfficerData.Pax);
                    CInter.TR = Convert.ToInt32(item?.OfficerData.TRNYCUPPS);
                    CInter.Id = item.Id;
                    CInter.Name = item?.OfficerData.TowerData.AirLine.Name;
                    CInter.AirPortTo = item?.OfficerData.TowerData.AirPort.Code;
                    CInter.TakeOffDate = CommonUsed.CheckDate(item?.OfficerData.TowerData.TakeOffDate.ToString());
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
                rd.SetDataSource(RevenueListInter.Select(p => new
                {
                    Id = p.Id,
                    Name = p.Name,
                    AirPortTo = p.AirPortTo,
                    InterCuppsFees = p.InterCuppsFees,
                    //  DomeCuppsFees = p.DomeCuppsFees,
                    Pax = p.Pax,
                    TakeOffDate = p.TakeOffDate,
                    TR = p.TR,

                    FlightNo = p.FlightNo,

                    InterFees = p.InterFees,
                    //  DomeFees = p.DomeFees,
                    SearchDateFrom = SearchDateFrom,
                    SearchDateTo = SearchDateTo,

                    CUPPSSDG = p.CUPPSSDG,
                    ExchangeRate = ExchangeRate,
                }).ToList());
                if (Format == 2) ///excel
                {
                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();
                    Stream Stream = rd.ExportToStream
                        //WordForWindows //PortableDocFormat //PortableDocFormat
                        (CrystalDecisions.Shared.ExportFormatType.Excel);
                    Stream.Seek(0, SeekOrigin.Begin);//application/doc //
                    return File(Stream, "application/xls ", "InterCuppsFees.xls");
                }
                else
                {//pdf

                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();
                    Stream Stream = rd.ExportToStream
                        //WordForWindows //PortableDocFormat //PortableDocFormat
                        (CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    Stream.Seek(0, SeekOrigin.Begin);//application/doc //
                    return File(Stream, "application/doc ", "InterCuppsFees.pdf");
                }

            }
            else
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Revenue/RptDomeCuppsFees.rpt")));


                Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date);

                Revenue = Revenue.Where(x => x.OfficerData.TowerData.FlightTypeD == 2 && x.OfficerData.TowerData.FlightTypeL == 2);
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
                var RevenueList = new List<CuppsRptViewModel>();
                foreach (var item in Revenue)
                {
                    CuppsRptViewModel C = new CuppsRptViewModel();

                    C.Pax = Convert.ToInt32(item?.OfficerData.Pax);
                    C.TR = Convert.ToInt32(item?.OfficerData.TRNYCUPPS);
                    C.Id = item.Id;
                    C.Name = item?.OfficerData.TowerData.AirLine.Name;
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

                ////*************************************************************************************

                //Domestic
                rd.SetDataSource(RevenueList.Select(p => new
                {
                    Id = p.Id,
                    Name = p.Name,
                    AirPortTo = p.AirPortTo,
                    DomeCuppsFees = p.DomeCuppsFees,
                    Pax = p.Pax,
                    TR = p.TR,
                    TakeOffDate = p.TakeOffDate,
                    FlightNo = p.FlightNo,
                    DomeFees = p.DomeFees,
                    SearchDateFrom = SearchDateFrom,
                    SearchDateTo = SearchDateTo,
                    CUPPSSDG = p.CUPPSSDG,
                    ExchangeRate = ExchangeRate,

                }).ToList());

                if (Format == 2) ///excel
                {
                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();
                    Stream Stream = rd.ExportToStream
                        //WordForWindows //PortableDocFormat //PortableDocFormat
                        (CrystalDecisions.Shared.ExportFormatType.Excel);
                    Stream.Seek(0, SeekOrigin.Begin);//application/doc //
                    return File(Stream, "application/xls ", "CuppsFees.xls");
                }
                else
                {//pdf

                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();
                    Stream Stream = rd.ExportToStream
                        //WordForWindows //PortableDocFormat //PortableDocFormat
                        (CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    Stream.Seek(0, SeekOrigin.Begin);//application/doc //
                    return File(Stream, "application/doc ", "CuppsFees.pdf");
                }
            }

        }


        /********************************************************* Inter CUPPS Details ***********************************************************************/
        public ActionResult TotalInterCUPPSDetails()
        {
            return View();
        }

        public ActionResult PrintDataTotalInterCUPPSDetails(DateTime DateFrom)
        {
            var id = User.Identity.GetUserId();
            RoleController role = new RoleController();
            int GetCompanyId = role.GetCompanyId(id);
            var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);

            decimal Amount = Convert.ToDecimal(db.CUPPSServiceFees.SingleOrDefault(x => x.Id == 1).AmountInDollar);

            string UserName = CommonUsed.GetUserName(id);
            String ManagerName = CompanyInfo.RevenuesManager;
            String SupervisorName = CompanyInfo.InterCUUPSSupervisor;

            if (DateFrom != null)
            {
                ExchangeRate = CommonUsed.getExchangeRateByLandingDate(DateFrom.Month, DateFrom.Year);
            }
            else
            {
                //ExchangeRate = CommonUsed.getExchangeRateByLandingDate(currentDate.Month, currentDate.Year);
                return RedirectToAction("Error");
            }

            List<spTotalCompaniesCUPPSForInter_Result> data = getTotalInterCUPPSDetails(DateFrom);
            /***********************************************************************************/
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports/Revenue/RptTotalCUPPSByCompanyInter.rpt")));

            var ReportList = new List<TotalCompaniesInterCUPPSModel>();
            foreach (var item in data)
            {
                TotalCompaniesInterCUPPSModel C = new TotalCompaniesInterCUPPSModel();

                C.Name = item.ArName;
                C.TotalAmountDollar = Convert.ToInt32(item.TotalAmountDollar);
                C.GrandTotalDollar  = Convert.ToInt32(item.GrandTotalDollar);
                C.TaxDollar         = Convert.ToInt32(item.TaxDollar);

                C.Pax = Convert.ToInt32(item.Pax);

                ReportList.Add(C);
            }
            /*************************************************************************************/
            rd.SetDataSource(ReportList.Select(p => new
            {                
                Amount = Amount,
                ExchRate = ExchangeRate,
                Name = p.Name,
                Pax = p.Pax,
                TotalAmountDollar = p.TotalAmountDollar,
                TaxDollar = p.TaxDollar,
                GrandTotalDollar = p.GrandTotalDollar * ExchangeRate,
                SupervisorName = SupervisorName,
                ManagerName = ManagerName,
                txtDate = DateFrom.Year + " - " + DateFrom.ToString("MMMM")
            }).ToList());

            if (Format == 2) ///excel
            {
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                Stream Stream = rd.ExportToStream
                    //WordForWindows //PortableDocFormat //PortableDocFormat
                    (CrystalDecisions.Shared.ExportFormatType.Excel);
                Stream.Seek(0, SeekOrigin.Begin);//application/excel
                return File(Stream, "application/xls ", "Nyfees.xls");
            }
            else
            {//pdf
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                Stream Stream = rd.ExportToStream
                    //WordForWindows //PortableDocFormat //PortableDocFormat
                    (CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                Stream.Seek(0, SeekOrigin.Begin);//application/pdf
                return File(Stream, "application/doc ", "Nyfees.pdf");
            }
        }

        public ActionResult PrintDataJournalCUPPSInter(DateTime DateFrom)
        {
            var id = User.Identity.GetUserId();
            RoleController role = new RoleController();
            int GetCompanyId = role.GetCompanyId(id);
            var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);

            decimal Amount = Convert.ToDecimal(db.CUPPSServiceFees.SingleOrDefault(x => x.Id == 1).AmountInDollar);

            string UserName = CommonUsed.GetUserName(id);
            String ManagerName = CompanyInfo.RevenuesManager;
            String SupervisorName = CompanyInfo.InterCUUPSSupervisor;
            decimal TotalSum = 0;

            if (DateFrom != null)
            {
                ExchangeRate = CommonUsed.getExchangeRateByLandingDate(DateFrom.Month, DateFrom.Year);
            }
            else
            {
                return RedirectToAction("Error");
            }

            /***********************************************************************************/
            ReportDocument rd = new ReportDocument();

            rd.Load(Path.Combine(Server.MapPath("~/Reports/Revenue/RptJournalCUPPSInter.rpt")));
            //List<vwJournalCUPPSInter> data = db.spTotalCompaniesCUPPSForInter.Where(t => DbFunctions.TruncateTime(t.Date).Value.Month == DateFrom.Month && DbFunctions.TruncateTime(t.Date).Value.Year == DateFrom.Year).ToList();
            List<spTotalCompaniesCUPPSForInter_Result> data = getTotalInterCUPPSDetails(DateFrom);

            var ReportList = new List<int>();
            foreach (var item in data)
            {
                ReportList.Add(Convert.ToInt32(item.Pax));
            }

            JournalViewModel journalViewModel = new JournalViewModel();
            int totalPax = ReportList.Sum();

            rd.SetDataSource(ReportList.Select(p => new
            {               
                SupervisorName = SupervisorName,
                ManagerName = ManagerName,
                Pax = totalPax,
                ExchRate = ExchangeRate,
                Amount = Amount,
                txtDate = DateFrom.Year + " - " + DateFrom.ToString("MMMM")
            }).ToList());

            if (Format == 2) ///excel
            {
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                Stream Stream = rd.ExportToStream
                    //WordForWindows //PortableDocFormat //PortableDocFormat
                    (CrystalDecisions.Shared.ExportFormatType.Excel);
                Stream.Seek(0, SeekOrigin.Begin);//application/excel
                return File(Stream, "application/xls ", "Nyfees.xls");
            }
            else
            {//pdf
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                Stream Stream = rd.ExportToStream
                    //WordForWindows //PortableDocFormat //PortableDocFormat
                    (CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                Stream.Seek(0, SeekOrigin.Begin);//application/pdf
                return File(Stream, "application/doc ", "Nyfees.pdf");
            }
        }

        //For Search Button
        //public ActionResult SearchDataTotalCompaniesInterCUPPS(DateTime DateFrom)
        //{
        //    var id = User.Identity.GetUserId();
        //    RoleController role = new RoleController();
        //    int GetCompanyId = role.GetCompanyId(id);
        //    var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);

        //    string UserName = CommonUsed.GetUserName(id);

        //    List<TotalCompaniesInterCUPPS_Result> data = getTotalCompaniesInterCUPPSData(DateFrom);
        //    /***********************************************************************************/

        //    var ReportList = new List<TotalCompaniesInterCUPPSModel>();
        //    foreach (var item in data)
        //    {
        //        TotalCompaniesInterCUPPSModel C = new TotalCompaniesInterCUPPSModel();

        //        C.Id = Convert.ToInt32(item.Id);
        //        C.Name = item.Name;
        //        C.Code = item.Code;
        //        C.Pax = item.Pax;
        //        C.ExchRate = Convert.ToDecimal(item.ExchRate);
        //        C.TotalAmountDollar = item.TotalAmountDollar;
        //        C.TaxDollar = item.TaxDollar;
        //        C.GrandTotalDollar = item.GrandTotalDollar;

        //        ReportList.Add(C);
        //    }

        //    return PartialView("_GetCupps", ReportList.ToPagedList(PageNumber, pageSize));
        //}

        //For Search Button
        public ActionResult SearchDataTotalInterCUPPSDetails(int? page, int? Get, int? Search, int? Count, DateTime? DateFrom)
        {
            List<spTotalCompaniesCUPPSForInter_Result> Revenue;
            Revenue = getTotalInterCUPPSDetails(DateTime.Now);
            ViewBag.DateFrom = DateFrom;

            if (Get > 0)
            {
                if (DateFrom != null)
                {
                    Revenue = getTotalInterCUPPSDetails(Convert.ToDateTime(DateFrom));
                }

                if (Search == 1 && Revenue != null)
                {
                    if (Revenue.Count() > 0)
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

            int pageSize = 40;
            int PageNumber = (page ?? 1);
            if (Revenue == null)
            {
                return PartialView("_PleaseSearch");
            }
            else
            {
                return PartialView("_GetTotalCompaniesInterCUPPS", Revenue.ToPagedList(PageNumber, pageSize));
            }
        }

        private List<spTotalCompaniesCUPPSForInter_Result> getTotalInterCUPPSDetails(DateTime DateFrom)
        {
            int Year = DateFrom.Year;
            int Month = DateFrom.Month;
            List<spTotalCompaniesCUPPSForInter_Result> data = db.spTotalCompaniesCUPPSForInter(Month, Year).ToList();

            if (data.Count() <= 0)
            {
                return null;
            }

            return data;
        }

        /********************************************************* Domestic CUPPS Details ***********************************************************************/
        public ActionResult TotalDomesticCUPPSDetails()
        {
            return View();
        }

        public ActionResult PrintDataTotalDomesticCUPPSDetails(DateTime DateFrom)
        {
            var id = User.Identity.GetUserId();
            RoleController role = new RoleController();
            int GetCompanyId = role.GetCompanyId(id);
            var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);

            decimal Amount = Convert.ToDecimal(db.CUPPSServiceFees.SingleOrDefault(x => x.Id == 2).AmountInDollar);

            String ManagerName = CompanyInfo.RevenuesManager;
            String SupervisorName = CompanyInfo.DomeCUUPSSupervisor;

            string UserName = CommonUsed.GetUserName(id);

            if (DateFrom != null)
            {
                ExchangeRate = CommonUsed.getExchangeRateByLandingDate(DateFrom.Month, DateFrom.Year);
            }
            else
            {
                //ExchangeRate = CommonUsed.getExchangeRateByLandingDate(currentDate.Month, currentDate.Year);
                return RedirectToAction("Error");
            }

            List<spTotalCompaniesCUPPSForDomestic_Result> data = getTotalDomesticCUPPSDetails(DateFrom);
            /***********************************************************************************/
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports/Revenue/RptTotalCUPPSByCompanyDom.rpt")));

            var ReportList = new List<TotalCompaniesInterCUPPSModel>();
            foreach (var item in data)
            {
                TotalCompaniesInterCUPPSModel C = new TotalCompaniesInterCUPPSModel();
                C.Name = item.ArName;
                C.Pax = Convert.ToInt32(item.Pax);
                C.TotalAmountDollar = Convert.ToInt32(item.TotalAmountDollar);
                C.TaxDollar = Convert.ToInt32(item.TaxDollar);
                C.GrandTotalDollar = Convert.ToInt32(item.GrandTotalDollar);

                ReportList.Add(C);
            }
            /*************************************************************************************/
            rd.SetDataSource(ReportList.Select(p => new
            {
                Name = p.Name,
                Pax = p.Pax,
                ExchRate = ExchangeRate,
                Amount = Amount,
                SupervisorName = SupervisorName,
                ManagerName = ManagerName,
                TotalAmountDollar = p.TotalAmountDollar * ExchangeRate,
                TaxDollar = p.TaxDollar * ExchangeRate,
                GrandTotalDollar = p.GrandTotalDollar * ExchangeRate,
                txtDate = DateFrom.Year + " - " + DateFrom.ToString("MMMM")
            }).ToList());

            if (Format == 2) //excel
            {
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                Stream Stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                Stream.Seek(0, SeekOrigin.Begin);//application/excel
                return File(Stream, "application/xls ", "Nyfees.xls");
            }
            else
            {//pdf
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                Stream Stream = rd.ExportToStream
                    //WordForWindows //PortableDocFormat //PortableDocFormat
                    (CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                Stream.Seek(0, SeekOrigin.Begin);//application/pdf
                return File(Stream, "application/doc ", "Nyfees.pdf");
            }
        }

        public ActionResult PrintDataJournalCUPPSDome(DateTime DateFrom)
        {
            var id = User.Identity.GetUserId();
            RoleController role = new RoleController();
            int GetCompanyId = role.GetCompanyId(id);
            var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);

            decimal Amount = Convert.ToDecimal(db.CUPPSServiceFees.SingleOrDefault(x => x.Id != 1).AmountInDollar);

            string UserName = CommonUsed.GetUserName(id);
            String ManagerName = CompanyInfo.RevenuesManager;
            String SupervisorName = CompanyInfo.DomeCUUPSSupervisor;
            decimal TotalSum = 0;

            if (DateFrom != null)
            {
                ExchangeRate = CommonUsed.getExchangeRateByLandingDate(DateFrom.Month, DateFrom.Year);
            }
            else
            {
                return RedirectToAction("Error");
            }

            /***********************************************************************************/
            ReportDocument rd = new ReportDocument();

            rd.Load(Path.Combine(Server.MapPath("~/Reports/Revenue/RptJournalCUPPSDome.rpt")));
            List<spTotalCompaniesCUPPSForDomestic_Result> data = getTotalDomesticCUPPSDetails(DateFrom);

            var ReportList = new List<int>();
            foreach (var item in data)
            {
                ReportList.Add(Convert.ToInt32(item.Pax));
            }

            JournalViewModel journalViewModel = new JournalViewModel();
            int totalPax = ReportList.Sum();

            rd.SetDataSource(ReportList.Select(p => new
            {                
                SupervisorName = SupervisorName,
                ManagerName = ManagerName,
                Pax = totalPax,
                Amount = Amount,
                ExchRate = ExchangeRate,
                txtDate = DateFrom.Year + " - " + DateFrom.ToString("MMMM")
            }).ToList());



            if (Format == 2) ///excel
            {
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                Stream Stream = rd.ExportToStream
                    //WordForWindows //PortableDocFormat //PortableDocFormat
                    (CrystalDecisions.Shared.ExportFormatType.Excel);
                Stream.Seek(0, SeekOrigin.Begin);//application/excel
                return File(Stream, "application/xls ", "Nyfees.xls");
            }
            else
            {//pdf
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                Stream Stream = rd.ExportToStream
                    //WordForWindows //PortableDocFormat //PortableDocFormat
                    (CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                Stream.Seek(0, SeekOrigin.Begin);//application/pdf
                return File(Stream, "application/doc ", "Nyfees.pdf");
            }
        }
     
        public ActionResult SearchDataTotalDomesticCUPPSDetails(int? page, int? Get, int? Search, int? Count, DateTime? DateFrom)
        {
            List<spTotalCompaniesCUPPSForDomestic_Result> Revenue;
            Revenue = getTotalDomesticCUPPSDetails(DateTime.Now);
            ViewBag.DateFrom = DateFrom;

            if (Get > 0)
            {
                if (DateFrom != null)
                {
                    Revenue = getTotalDomesticCUPPSDetails(Convert.ToDateTime(DateFrom));
                }

                if (Search == 1 && Revenue != null)
                {
                    if (Revenue.Count() > 0)
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

            int pageSize = 40;
            int PageNumber = (page ?? 1);
            if (Revenue == null)
            {
                return PartialView("_PleaseSearch");
            }
            else
            {
                return PartialView("_GetTotalCompaniesDomeCUPPS", Revenue.ToPagedList(PageNumber, pageSize));
            }
        }

        private List<spTotalCompaniesCUPPSForDomestic_Result> getTotalDomesticCUPPSDetails(DateTime DateFrom)
        {
            int Year = DateFrom.Year;
            int Month = DateFrom.Month;
            List<spTotalCompaniesCUPPSForDomestic_Result> data = db.spTotalCompaniesCUPPSForDomestic(Month, Year).ToList();

            if (data.Count() <= 0)
            {
                return null;
            }

            return data;
        }

        /********************************************************* Inter NY Details ***********************************************************************/
        public ActionResult TotalInterNYDetails()
        {
            return View();
        }

        public ActionResult PrintDataTotalInterNYDetails(DateTime DateFrom)
        {
            var id = User.Identity.GetUserId();
            RoleController role = new RoleController();
            int GetCompanyId = role.GetCompanyId(id);
            var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);

            decimal Amount = Convert.ToDecimal(db.NYFees.SingleOrDefault(x => x.Id == 1).AmountInDollar);

            string UserName = CommonUsed.GetUserName(id);
            String ManagerName = CompanyInfo.RevenuesManager;
            String SupervisorName = CompanyInfo.InterNYSupervisor;

            if (DateFrom != null)
            {
                ExchangeRate = CommonUsed.getExchangeRateByLandingDate(DateFrom.Month, DateFrom.Year);
            }
            else
            {
                //ExchangeRate = CommonUsed.getExchangeRateByLandingDate(currentDate.Month, currentDate.Year);
                return RedirectToAction("Error");
            }

            List<spTotalCompaniesNYForInter_Result> data = getTotalInterNYDetails(DateFrom);
            /***********************************************************************************/
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports/Revenue/RptTotalNYByCompanyInter.rpt")));

            var ReportList = new List<TotalCompaniesInterCUPPSModel>();
            foreach (var item in data)
            {
                TotalCompaniesInterCUPPSModel C = new TotalCompaniesInterCUPPSModel();
                C.Name = item.Name;
                C.Pax = Convert.ToInt32(item.Pax);

                ReportList.Add(C);
            }
            /*************************************************************************************/
            rd.SetDataSource(ReportList.Select(p => new
            {
                Name = p.Name,
                Pax = p.Pax,
                ExchRate = ExchangeRate,
                Amount = Amount,
                SupervisorName = SupervisorName,
                ManagerName = ManagerName,
                //TotalAmountDollar = p.TotalAmountDollar * ExchangeRate,
                //TaxDollar = p.TaxDollar * ExchangeRate,
                //GrandTotalDollar = p.GrandTotalDollar * ExchangeRate,
                txtDate = DateFrom.Year + " - " + DateFrom.ToString("MMMM")

                //SearchDateTo = SearchDateTo,
                //ExchangeRate = ExchangeRate,
            }).ToList());

            if (Format == 2) ///excel
            {
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                Stream Stream = rd.ExportToStream
                    //WordForWindows //PortableDocFormat //PortableDocFormat
                    (CrystalDecisions.Shared.ExportFormatType.Excel);
                Stream.Seek(0, SeekOrigin.Begin);//application/excel
                return File(Stream, "application/xls ", "Nyfees.xls");
            }
            else
            {//pdf
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                Stream Stream = rd.ExportToStream
                    //WordForWindows //PortableDocFormat //PortableDocFormat
                    (CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                Stream.Seek(0, SeekOrigin.Begin);//application/pdf
                return File(Stream, "application/doc ", "Nyfees.pdf");
            }
        }

        public ActionResult PrintDataJournalNYInter(DateTime DateFrom)
        {
            var id = User.Identity.GetUserId();
            RoleController role = new RoleController();
            int GetCompanyId = role.GetCompanyId(id);
            var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);

            decimal Amount = Convert.ToDecimal(db.NYFees.SingleOrDefault(x => x.Id == 1).AmountInDollar);

            string UserName = CommonUsed.GetUserName(id);
            String ManagerName = CompanyInfo.RevenuesManager;
            String SupervisorName = CompanyInfo.InterNYSupervisor;
            decimal TotalSum = 0;

            if (DateFrom != null)
            {
                ExchangeRate = CommonUsed.getExchangeRateByLandingDate(DateFrom.Month, DateFrom.Year);
            }
            else
            {
                return RedirectToAction("Error");
            }

            /***********************************************************************************/
            ReportDocument rd = new ReportDocument();

            rd.Load(Path.Combine(Server.MapPath("~/Reports/Revenue/RptJournalNYInter.rpt")));
            //List<vwJournalNYInter> data = db.vwJournalNYInters.Where(t => DbFunctions.TruncateTime(t.Date).Value.Month ==  && DbFunctions.TruncateTime(t.Date).Value.Year == 2022).ToList();
            List<spTotalCompaniesNYForInter_Result> data = getTotalInterNYDetails(DateFrom);

            var ReportList = new List<int>();
            foreach (var item in data)
            {
                ReportList.Add(Convert.ToInt32(item.Pax));
            }

            JournalViewModel journalViewModel = new JournalViewModel();
            int totalPax = ReportList.Sum();

            rd.SetDataSource(ReportList.Select(p => new
            {
                SupervisorName = SupervisorName,
                ManagerName = ManagerName,
                ExchRate = ExchangeRate,
                Amount = Amount,
                Pax = totalPax,
                txtDate = DateFrom.Year + " - " + DateFrom.ToString("MMMM")
            }).ToList());



            if (Format == 2) ///excel
            {
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                Stream Stream = rd.ExportToStream
                    //WordForWindows //PortableDocFormat //PortableDocFormat
                    (CrystalDecisions.Shared.ExportFormatType.Excel);
                Stream.Seek(0, SeekOrigin.Begin);//application/excel
                return File(Stream, "application/xls ", "Nyfees.xls");
            }
            else
            {//pdf
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                Stream Stream = rd.ExportToStream
                    //WordForWindows //PortableDocFormat //PortableDocFormat
                    (CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                Stream.Seek(0, SeekOrigin.Begin);//application/pdf
                return File(Stream, "application/doc ", "Nyfees.pdf");
            }
        }

        //For Search Button
        //public ActionResult SearchDataTotalCompaniesInterCUPPS(DateTime DateFrom)
        //{
        //    var id = User.Identity.GetUserId();
        //    RoleController role = new RoleController();
        //    int GetCompanyId = role.GetCompanyId(id);
        //    var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);

        //    string UserName = CommonUsed.GetUserName(id);

        //    List<TotalCompaniesInterCUPPS_Result> data = getTotalCompaniesInterCUPPSData(DateFrom);
        //    /***********************************************************************************/

        //    var ReportList = new List<TotalCompaniesInterCUPPSModel>();
        //    foreach (var item in data)
        //    {
        //        TotalCompaniesInterCUPPSModel C = new TotalCompaniesInterCUPPSModel();

        //        C.Id = Convert.ToInt32(item.Id);
        //        C.Name = item.Name;
        //        C.Code = item.Code;
        //        C.Pax = item.Pax;
        //        C.ExchRate = Convert.ToDecimal(item.ExchRate);
        //        C.TotalAmountDollar = item.TotalAmountDollar;
        //        C.TaxDollar = item.TaxDollar;
        //        C.GrandTotalDollar = item.GrandTotalDollar;

        //        ReportList.Add(C);
        //    }

        //    return PartialView("_GetCupps", ReportList.ToPagedList(PageNumber, pageSize));
        //}

        //For Search Button
        public ActionResult SearchDataTotalInterNYDetails(int? page, int? Get, int? Search, int? Count, DateTime? DateFrom)
        {
            List<spTotalCompaniesNYForInter_Result> Revenue;
            Revenue = getTotalInterNYDetails(DateTime.Now);
            ViewBag.DateFrom = DateFrom;

            if (Get > 0)
            {
                if (DateFrom != null)
                {
                    Revenue = getTotalInterNYDetails(Convert.ToDateTime(DateFrom));
                }

                if (Search == 1 && Revenue != null)
                {
                    if (Revenue.Count() > 0)
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

            int pageSize = 40;
            int PageNumber = (page ?? 1);
            if (Revenue == null)
            {
                return PartialView("_PleaseSearch");
            }
            else
            {
                return PartialView("_GetTotalCompaniesInterNY", Revenue.ToPagedList(PageNumber, pageSize));
            }
        }

        private List<spTotalCompaniesNYForInter_Result> getTotalInterNYDetails(DateTime DateFrom)
        {
            int Year = DateFrom.Year;
            int Month = DateFrom.Month;
            List<spTotalCompaniesNYForInter_Result> data = db.spTotalCompaniesNYForInter(Month, Year).ToList();

            if (data.Count() <= 0)
            {
                return null;
            }

            return data;
        }

        /********************************************************* Domestic NY Details ***********************************************************************/
        public ActionResult TotalDomesticNYDetails()
        {
            return View();
        }

        public ActionResult PrintDataTotalDomesticNYDetails(DateTime DateFrom)
        {
            var id = User.Identity.GetUserId();
            RoleController role = new RoleController();
            int GetCompanyId = role.GetCompanyId(id);
            var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);

            decimal Amount = Convert.ToDecimal(db.NYFees.SingleOrDefault(x => x.Id != 1).AmountInSDG);

            string UserName = CommonUsed.GetUserName(id);

            String ManagerName = CompanyInfo.RevenuesManager;
            String SupervisorName = CompanyInfo.DomeNYSupervisor;

            if (DateFrom != null)
            {
                ExchangeRate = CommonUsed.getExchangeRateByLandingDate(DateFrom.Month, DateFrom.Year);
            }
            else
            {
                //ExchangeRate = CommonUsed.getExchangeRateByLandingDate(currentDate.Month, currentDate.Year);
                return RedirectToAction("Error");
            }

            List<spTotalCompaniesNYForDomestic_Result> data = getTotalDomesticNYDetails(DateFrom);
            /***********************************************************************************/
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports/Revenue/RptTotalNYByCompanyDom.rpt")));

            var ReportList = new List<TotalCompaniesInterCUPPSModel>();
            foreach (var item in data)
            {
                TotalCompaniesInterCUPPSModel C = new TotalCompaniesInterCUPPSModel();
                C.Name = item.ArName;
                C.Pax = Convert.ToInt32(item.Pax);
                C.Amount = Amount;
                C.ExchRate = ExchangeRate;

                ReportList.Add(C);
            }
            /*************************************************************************************/
            rd.SetDataSource(ReportList.Select(p => new
            {
                Name = p.Name,
                Pax = p.Pax,
                ExchRate = ExchangeRate,
                Amount = Amount,
                SupervisorName = SupervisorName,
                ManagerName = ManagerName,
                TotalAmountDollar = p.TotalAmountDollar,
                TaxDollar = p.TaxDollar,
                GrandTotalDollar = p.GrandTotalDollar,
                txtDate = DateFrom.Year + " - " + DateFrom.ToString("MMMM")

            }).ToList());

            if (Format == 2) ///excel
            {
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                Stream Stream = rd.ExportToStream
                    //WordForWindows //PortableDocFormat //PortableDocFormat
                    (CrystalDecisions.Shared.ExportFormatType.Excel);
                Stream.Seek(0, SeekOrigin.Begin);//application/excel
                return File(Stream, "application/xls ", "Nyfees.xls");
            }
            else
            {//pdf
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                Stream Stream = rd.ExportToStream
                    //WordForWindows //PortableDocFormat //PortableDocFormat
                    (CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                Stream.Seek(0, SeekOrigin.Begin);//application/pdf
                return File(Stream, "application/doc ", "Nyfees.pdf");
            }
        }

        public ActionResult PrintDataJournalNYDome(DateTime DateFrom)
        {
            var id = User.Identity.GetUserId();
            RoleController role = new RoleController();
            int GetCompanyId = role.GetCompanyId(id);
            var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);

            decimal Amount = Convert.ToDecimal(db.NYFees.SingleOrDefault(x => x.Id != 1).AmountInSDG);

            string UserName = CommonUsed.GetUserName(id);
            String ManagerName = CompanyInfo.RevenuesManager;
            String SupervisorName = CompanyInfo.DomeNYSupervisor;
            decimal TotalSum = 0;

            if (DateFrom != null)
            {
                ExchangeRate = CommonUsed.getExchangeRateByLandingDate(DateFrom.Month, DateFrom.Year);
            }
            else
            {
                return RedirectToAction("Error");
            }

            /***********************************************************************************/
            ReportDocument rd = new ReportDocument();

            rd.Load(Path.Combine(Server.MapPath("~/Reports/Revenue/RptJournalNYDome.rpt")));
            List<spTotalCompaniesNYForDomestic_Result> data = getTotalDomesticNYDetails(DateFrom);

            var ReportList = new List<int>();
            foreach (var item in data)
            {
                ReportList.Add(Convert.ToInt32(item.Pax));
            }

            int totalPax = ReportList.Sum();
            rd.SetDataSource(ReportList.Select(p => new
            {
                Pax = totalPax,
                Amount = Amount,
                ExchRate = ExchangeRate,
                SupervisorName = SupervisorName,
                ManagerName = ManagerName,
                txtDate = DateFrom.Year + " - " + DateFrom.ToString("MMMM")
            }).ToList());



            if (Format == 2) ///excel
            {
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                Stream Stream = rd.ExportToStream
                    //WordForWindows //PortableDocFormat //PortableDocFormat
                    (CrystalDecisions.Shared.ExportFormatType.Excel);
                Stream.Seek(0, SeekOrigin.Begin);//application/excel
                return File(Stream, "application/xls ", "Nyfees.xls");
            }
            else
            {//pdf
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                Stream Stream = rd.ExportToStream
                    //WordForWindows //PortableDocFormat //PortableDocFormat
                    (CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                Stream.Seek(0, SeekOrigin.Begin);//application/pdf
                return File(Stream, "application/doc ", "Nyfees.pdf");
            }
        }

        //For Search Button
        //public ActionResult SearchDataTotalCompaniesInterCUPPS(DateTime DateFrom)
        //{
        //    var id = User.Identity.GetUserId();
        //    RoleController role = new RoleController();
        //    int GetCompanyId = role.GetCompanyId(id);
        //    var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);

        //    string UserName = CommonUsed.GetUserName(id);

        //    List<TotalCompaniesInterCUPPS_Result> data = getTotalCompaniesInterCUPPSData(DateFrom);
        //    /***********************************************************************************/

        //    var ReportList = new List<TotalCompaniesInterCUPPSModel>();
        //    foreach (var item in data)
        //    {
        //        TotalCompaniesInterCUPPSModel C = new TotalCompaniesInterCUPPSModel();

        //        C.Id = Convert.ToInt32(item.Id);
        //        C.Name = item.Name;
        //        C.Code = item.Code;
        //        C.Pax = item.Pax;
        //        C.ExchRate = Convert.ToDecimal(item.ExchRate);
        //        C.TotalAmountDollar = item.TotalAmountDollar;
        //        C.TaxDollar = item.TaxDollar;
        //        C.GrandTotalDollar = item.GrandTotalDollar;

        //        ReportList.Add(C);
        //    }

        //    return PartialView("_GetCupps", ReportList.ToPagedList(PageNumber, pageSize));
        //}

        //For Search Button
        public ActionResult SearchDataTotalDomesticNYDetails(int? page, int? Get, int? Search, int? Count, DateTime? DateFrom)
        {
            List<spTotalCompaniesNYForDomestic_Result> Revenue;
            Revenue = getTotalDomesticNYDetails(DateTime.Now);
            ViewBag.DateFrom = DateFrom;

            if (Get > 0)
            {
                if (DateFrom != null)
                {
                    Revenue = getTotalDomesticNYDetails(Convert.ToDateTime(DateFrom));
                }

                if (Search == 1 && Revenue != null)
                {
                    if (Revenue.Count() > 0)
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

            int pageSize = 40;
            int PageNumber = (page ?? 1);
            if (Revenue == null)
            {
                return PartialView("_PleaseSearch");
            }
            else
            {
                return PartialView("_GetTotalCompaniesDomeNY", Revenue.ToPagedList(PageNumber, pageSize));
            }
        }

        private List<spTotalCompaniesNYForDomestic_Result> getTotalDomesticNYDetails(DateTime DateFrom)
        {
            int Year = DateFrom.Year;
            int Month = DateFrom.Month;
            List<spTotalCompaniesNYForDomestic_Result> data = db.spTotalCompaniesNYForDomestic(Month, Year).ToList();

            if (data.Count() <= 0)
            {
                return null;
            }

            return data;
        }
        /********************************************************* Pushback Details ***********************************************************************/
        public ActionResult TotalPushbackDetails()
        {
            return View();
        }

        public ActionResult PrintDataTotalPushbackDetails(DateTime DateFrom, decimal LastRequestedAmountSDG)
        {
            var id = User.Identity.GetUserId();
            RoleController role = new RoleController();
            int GetCompanyId = role.GetCompanyId(id);
            var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);

            decimal FinalAmountForSudanAirWays = 0;

            string UserName = CommonUsed.GetUserName(id);
            String ManagerName = CompanyInfo.RevenuesManager;
            String SupervisorName = CompanyInfo.PushBackSupervisor;

            if (DateFrom != null)
            {
                ExchangeRate = CommonUsed.getExchangeRateByLandingDate(DateFrom.Month, DateFrom.Year);
                FinalAmountForSudanAirWays = cal.FinalAmountForSudanAirWays(DateFrom);
            }
            else
            {
                //ExchangeRate = CommonUsed.getExchangeRateByLandingDate(currentDate.Month, currentDate.Year);
                return RedirectToAction("Error");
            }

            List<spTotalCompaniesPushbackFees_Result> data = getTotalPushbackDetails(DateFrom);
            /***********************************************************************************/
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports/Revenue/RptTotalPushbackDetails.rpt")));

            var ReportList = new List<TotalCompaniesInterCUPPSModel>();
            foreach (var item in data)
            {
                TotalCompaniesInterCUPPSModel C = new TotalCompaniesInterCUPPSModel();
                C.Name = item.ArName;
                C.TotalAmountDollar = Convert.ToInt32(item.PushBackFees);

                ReportList.Add(C);
            }
            /*************************************************************************************/
            rd.SetDataSource(ReportList.Select(p => new
            {
                Name = p.Name,
                ExchRate = ExchangeRate,
                SupervisorName = SupervisorName,
                ManagerName = ManagerName,
                LastRequestedAmountSDG = LastRequestedAmountSDG,
                Amount = FinalAmountForSudanAirWays,
                TotalAmountDollar = p.TotalAmountDollar * ExchangeRate,
                TaxDollar = ((p.TotalAmountDollar * 5) / 100) * ExchangeRate,
                GrandTotalDollar = (p.TotalAmountDollar + ((p.TotalAmountDollar * 5) / 100)) * ExchangeRate,
                txtDate = DateFrom.Year + " - " + DateFrom.ToString("MMMM")

            }).ToList());

            if (Format == 2) ///excel
            {
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                Stream Stream = rd.ExportToStream
                    //WordForWindows //PortableDocFormat //PortableDocFormat
                    (CrystalDecisions.Shared.ExportFormatType.Excel);
                Stream.Seek(0, SeekOrigin.Begin);//application/excel
                return File(Stream, "application/xls ", "Pushback Details.xls");
            }
            else
            {//pdf
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                Stream Stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                Stream.Seek(0, SeekOrigin.Begin);//application/pdf
                return File(Stream, "application/doc ", "Pushback Details.pdf");
            }
        }

        //For Search Button
        //public ActionResult SearchDataTotalCompaniesInterCUPPS(DateTime DateFrom)
        //{
        //    var id = User.Identity.GetUserId();
        //    RoleController role = new RoleController();
        //    int GetCompanyId = role.GetCompanyId(id);
        //    var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);

        //    string UserName = CommonUsed.GetUserName(id);

        //    List<TotalCompaniesInterCUPPS_Result> data = getTotalCompaniesInterCUPPSData(DateFrom);
        //    /***********************************************************************************/

        //    var ReportList = new List<TotalCompaniesInterCUPPSModel>();
        //    foreach (var item in data)
        //    {
        //        TotalCompaniesInterCUPPSModel C = new TotalCompaniesInterCUPPSModel();

        //        C.Id = Convert.ToInt32(item.Id);
        //        C.Name = item.Name;
        //        C.Code = item.Code;
        //        C.Pax = item.Pax;
        //        C.ExchRate = Convert.ToDecimal(item.ExchRate);
        //        C.TotalAmountDollar = item.TotalAmountDollar;
        //        C.TaxDollar = item.TaxDollar;
        //        C.GrandTotalDollar = item.GrandTotalDollar;

        //        ReportList.Add(C);
        //    }

        //    return PartialView("_GetCupps", ReportList.ToPagedList(PageNumber, pageSize));
        //}

        //For Search Button
        public ActionResult SearchDataTotalPushbackDetails(int? page, int? Get, int? Search, int? Count, DateTime? DateFrom)
        {
            List<spTotalCompaniesPushbackFees_Result> Revenue;
            Revenue = getTotalPushbackDetails(DateTime.Now);
            ViewBag.DateFrom = DateFrom;

            if (Get > 0)
            {
                if (DateFrom != null)
                {
                    Revenue = getTotalPushbackDetails(Convert.ToDateTime(DateFrom));
                }

                if (Search == 1 && Revenue != null)
                {
                    if (Revenue.Count() > 0)
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

            int pageSize = 40;
            int PageNumber = (page ?? 1);
            if (Revenue == null)
            {
                return PartialView("_PleaseSearch");
            }
            else
            {
                return PartialView("_GetTotalCompaniesPushback", Revenue.ToPagedList(PageNumber, pageSize));
            }
        }

        private List<spTotalCompaniesPushbackFees_Result> getTotalPushbackDetails(DateTime DateFrom)
        {
            int Year = DateFrom.Year;
            int Month = DateFrom.Month;
            List<spTotalCompaniesPushbackFees_Result> data = db.spTotalCompaniesPushbackFees(Month, Year).ToList();

            if (data.Count() <= 0)
            {
                return null;
            }

            return data;
        }
        /********************************************************* Service Car Details ***********************************************************************/
        public ActionResult TotalServiceCarDetails()
        {
            return View();
        }

        public ActionResult PrintDataTotalServiceCarDetails(DateTime DateFrom, DateTime DateTo)
        {
            var id = User.Identity.GetUserId();
            RoleController role = new RoleController();
            int GetCompanyId = role.GetCompanyId(id);
            var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);

            decimal FinalAmountForSudanAirWays = 0;

            string UserName = CommonUsed.GetUserName(id);
            String ManagerName = CompanyInfo.RevenuesManager;
            String SupervisorName = CompanyInfo.ServiceCarSupervisor;

            if (DateFrom != null)
            {
                ExchangeRate = CommonUsed.getExchangeRateByLandingDate(DateFrom.Month, DateFrom.Year);
                FinalAmountForSudanAirWays = cal.FinalAmountForSudanAirWays(DateFrom);
            }
            else
            {
                //ExchangeRate = CommonUsed.getExchangeRateByLandingDate(currentDate.Month, currentDate.Year);
                return RedirectToAction("Error");
            }

            List<spTotalCompaniesServiceCarFees_Result> data = getTotalServiceCarDetails(DateFrom, DateTo);
            /***********************************************************************************/
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports/Revenue/RptTotalServiceCarByCompanyDetails.rpt")));

            var ReportList = new List<ServiceCarViewModel>();
            foreach (var item in data)
            {
                ServiceCarViewModel C = new ServiceCarViewModel();
                
                C.ArName = item.ArName;                
                C.TotalACarCount = Convert.ToInt32(item.TotalACarCount);
                C.TotalFCarCount = Convert.ToInt32(item.TotalFCarCount);
                C.AmbolanceFees = Convert.ToDecimal(item.AmbolanceFees);
                C.FireFightingFees = Convert.ToDecimal(item.FireFightingFees);

                ReportList.Add(C);
            }
            /*************************************************************************************/
            rd.SetDataSource(ReportList.Select(p => new
            {
                Name = p.ArName,
                ExchRate = ExchangeRate,
                SupervisorName = SupervisorName,
                ManagerName = ManagerName,
                ACount = p.TotalACarCount,
                FCount = p.TotalFCarCount,
                DateFrom = DateFrom.Date.ToShortDateString(),
                DateTo = DateTo.Date.ToShortDateString(),
                TotalAmountDollar = p.AmbolanceFees + p.FireFightingFees,
                Amount = (p.AmbolanceFees + p.FireFightingFees) * ExchangeRate,
                txtDate = DateFrom.Year + " - " + DateFrom.ToString("MMMM")

            }).ToList());

            if (Format == 2) ///excel
            {
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                Stream Stream = rd.ExportToStream
                    //WordForWindows //PortableDocFormat //PortableDocFormat
                    (CrystalDecisions.Shared.ExportFormatType.Excel);
                Stream.Seek(0, SeekOrigin.Begin);//application/excel
                return File(Stream, "application/xls ", "ServiceCar Details.xls");
            }
            else
            {//pdf
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                Stream Stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                Stream.Seek(0, SeekOrigin.Begin);//application/pdf
                return File(Stream, "application/doc ", "ServiceCar Details.pdf");
            }
        }

        //For Search Button
        //public ActionResult SearchDataTotalCompaniesInterCUPPS(DateTime DateFrom)
        //{
        //    var id = User.Identity.GetUserId();
        //    RoleController role = new RoleController();
        //    int GetCompanyId = role.GetCompanyId(id);
        //    var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);

        //    string UserName = CommonUsed.GetUserName(id);

        //    List<TotalCompaniesInterCUPPS_Result> data = getTotalCompaniesInterCUPPSData(DateFrom);
        //    /***********************************************************************************/

        //    var ReportList = new List<TotalCompaniesInterCUPPSModel>();
        //    foreach (var item in data)
        //    {
        //        TotalCompaniesInterCUPPSModel C = new TotalCompaniesInterCUPPSModel();

        //        C.Id = Convert.ToInt32(item.Id);
        //        C.Name = item.Name;
        //        C.Code = item.Code;
        //        C.Pax = item.Pax;
        //        C.ExchRate = Convert.ToDecimal(item.ExchRate);
        //        C.TotalAmountDollar = item.TotalAmountDollar;
        //        C.TaxDollar = item.TaxDollar;
        //        C.GrandTotalDollar = item.GrandTotalDollar;

        //        ReportList.Add(C);
        //    }

        //    return PartialView("_GetCupps", ReportList.ToPagedList(PageNumber, pageSize));
        //}

        //For Search Button
        public ActionResult SearchDataTotalServiceCarDetails(int? page, int? Get, int? Search, int? Count, DateTime? DateFrom, DateTime? DateTo)
        {
            List<spTotalCompaniesServiceCarFees_Result> Revenue;
            Revenue = getTotalServiceCarDetails(DateTime.Now, DateTime.Now);
            ViewBag.DateFrom = DateFrom;

            if (Get > 0)
            {
                if (DateFrom != null && DateTo != null)
                {
                    Revenue = getTotalServiceCarDetails(Convert.ToDateTime(DateFrom), Convert.ToDateTime(DateTo));
                }

                if (Search == 1 && Revenue != null)
                {
                    if (Revenue.Count() > 0)
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

            int pageSize = 40;
            int PageNumber = (page ?? 1);
            if (Revenue == null)
            {
                return PartialView("_PleaseSearch");
            }
            else
            {
                return PartialView("_GetTotalCompaniesServiceCar", Revenue.ToPagedList(PageNumber, pageSize));
            }
        }

        private List<spTotalCompaniesServiceCarFees_Result> getTotalServiceCarDetails(DateTime DateFrom, DateTime? DateTo)
        {
            int Year = DateFrom.Year;
            int Month = DateFrom.Month;
            List<spTotalCompaniesServiceCarFees_Result> data = db.spTotalCompaniesServiceCarFees(DateFrom, DateTo).ToList();

            if (data.Count() <= 0)
            {
                return null;
            }

            return data;
        }

        ////////////////////////////////////////////// End Revenue Section///////////////////////////////////////////////////////////////                  
        public ActionResult Index()
        {
            var revenues = db.Revenues.Include(r => r.CompanyInfo).Include(r => r.OfficerData);
            return View(revenues.ToList());
        }

        // GET: Revenues/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Revenue revenue = db.Revenues.Find(id);
            if (revenue == null)
            {
                return HttpNotFound();
            }
            return View(revenue);
        }

        // GET: Revenues/Create
        public ActionResult Create()
        {
            ViewBag.CompanyInfoId = new SelectList(db.CompanyInfoes, "Id", "NameAr");
            ViewBag.OfficerDataId = new SelectList(db.OfficerDatas, "Id", "CreatedBy");
            return View();
        }

        // POST: Revenues/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,OfficerDataId,LandingFees,OpenAireportOvertimeFees,NavigationFees,NightOperationFees,ParkingFees,PushBackFees,CUPPSFees,FireFightingFees,AmbolanceFees,FrightFees,MailFees,SecurityFees,NYFees,NoiseFees,CompanyInfoId,CreatedBy,CreationDate,UpdatedBy,UpdatingDate")] Revenue revenue)
        {
            if (ModelState.IsValid)
            {
                db.Revenues.Add(revenue);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CompanyInfoId = new SelectList(db.CompanyInfoes, "Id", "NameAr", revenue.CompanyInfoId);
            ViewBag.OfficerDataId = new SelectList(db.OfficerDatas, "Id", "CreatedBy", revenue.OfficerDataId);
            return View(revenue);
        }

        // GET: Revenues/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Revenue revenue = db.Revenues.Find(id);
            if (revenue == null)
            {
                return HttpNotFound();
            }
            ViewBag.CompanyInfoId = new SelectList(db.CompanyInfoes, "Id", "NameAr", revenue.CompanyInfoId);
            ViewBag.OfficerDataId = new SelectList(db.OfficerDatas, "Id", "CreatedBy", revenue.OfficerDataId);
            return View(revenue);
        }

        // POST: Revenues/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,OfficerDataId,LandingFees,OpenAireportOvertimeFees,NavigationFees,NightOperationFees,ParkingFees,PushBackFees,CUPPSFees,FireFightingFees,AmbolanceFees,FrightFees,MailFees,SecurityFees,NYFees,NoiseFees,CompanyInfoId,CreatedBy,CreationDate,UpdatedBy,UpdatingDate")] Revenue revenue)
        {
            if (ModelState.IsValid)
            {
                db.Entry(revenue).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CompanyInfoId = new SelectList(db.CompanyInfoes, "Id", "NameAr", revenue.CompanyInfoId);
            ViewBag.OfficerDataId = new SelectList(db.OfficerDatas, "Id", "CreatedBy", revenue.OfficerDataId);
            return View(revenue);
        }

        // GET: Revenues/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Revenue revenue = db.Revenues.Find(id);
            if (revenue == null)
            {
                return HttpNotFound();
            }
            return View(revenue);
        }

        // POST: Revenues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Revenue revenue = db.Revenues.Find(id);
            db.Revenues.Remove(revenue);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public ActionResult PrintDataCuppsNoData(int? AirLineId, DateTime? DateFrom, DateTime? DateTo, int? FlightTypeId)
        {
            var id = User.Identity.GetUserId();
            RoleController role = new RoleController();
            int GetCompanyId = role.GetCompanyId(id);
            var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);

            string UserName = CommonUsed.GetUserName(id);
            string SearchAirLineId = "غير محدد";
            string SearchDateFrom = "غير محدد";
            string SearchDateTo = "غير محدد";

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

            ReportDocument rd = new ReportDocument();
            IQueryable<Revenue> Revenue;

            //rd.Load(Path.Combine(Server.MapPath("~/Reports/Revenue/RptCuppsNoDataFees.rpt")));

            Revenue = db.Revenues.Where(x => x.OfficerData.TowerData.AirLineId == AirLineId);

            if (FlightTypeId > 0)
            {
                if (FlightTypeId == 1)
                {
                    Revenue = Revenue.Where(x => (x.OfficerData.TowerData.FlightTypeD == 1 || x.OfficerData.TowerData.FlightTypeL == 1)
                    && (x.InterCUPPSFees == null || x.InterCUPPSFees <= 0));

                    rd.Load(Path.Combine(Server.MapPath("~/Reports/Revenue/RptCuppsNoDataIntFees.rpt")));
                }
                else
                {
                    Revenue = Revenue.Where(x => x.OfficerData.TowerData.FlightTypeD == 2 && x.OfficerData.TowerData.FlightTypeL == 2
                    && x.DomeCUPPSFees == null);
                    rd.Load(Path.Combine(Server.MapPath("~/Reports/Revenue/RptCuppsNoDataDomFees.rpt")));
                }
            }

            if (DateFrom != null && DateTo != null)
            {
                Revenue = Revenue.Where(t => DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) >= DbFunctions.TruncateTime(DateFrom) &&
                DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) <= DbFunctions.TruncateTime(DateTo));

                SearchDateFrom = CommonUsed.CheckDate(DateFrom.ToString()).ToString();
                SearchDateTo = CommonUsed.CheckDate(DateTo.ToString()).ToString();
            }

            if (Revenue.Count() <= 0)
            {
                return RedirectToAction("Error");
            }
            //********************************************************************************

            var RevenueListInter = new List<CuppsRptViewModel>();
            foreach (var item in Revenue.OrderBy(x=>x.OfficerData.TowerData.Date))
            {
                CuppsRptViewModel CInter = new CuppsRptViewModel();
                CInter.Id = item.Id;
                CInter.Name = item?.OfficerData.TowerData.AirLine.Code + "/ " + item?.OfficerData.TowerData.AirLine.Name;
                CInter.Registration = item?.OfficerData.TowerData.AircraftRegistration.Registration;
                CInter.AirPortTo = item?.OfficerData.TowerData.AirPort.Code;
                CInter.TakeOffDate = CommonUsed.CheckDate(item?.OfficerData.TowerData.TakeOffDate.ToString());
                CInter.FlightNo = item?.OfficerData.TowerData.AirLinesFlightTrip.FlightNo;
                
                RevenueListInter.Add(CInter);
            }
            //InterNational
            rd.SetDataSource(RevenueListInter.Select(p => new
            {
                Id = p.Id,
                Registration=p.Registration,
                Name = p.Name,
                AirPortTo = p.AirPortTo,
                TakeOffDate = p.TakeOffDate,
                FlightNo = p.FlightNo,
                SearchDateFrom = SearchDateFrom,
                SearchDateTo = SearchDateTo,
            }).ToList());

            if (Format == 2) ///excel
            {
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                Stream Stream = rd.ExportToStream
                    //WordForWindows //PortableDocFormat //PortableDocFormat
                    (CrystalDecisions.Shared.ExportFormatType.Excel);
                Stream.Seek(0, SeekOrigin.Begin);//application/doc //
                return File(Stream, "application/xls ", "CuppsFeesNoData.xls");
            }
            else
            {//pdf

                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                Stream Stream = rd.ExportToStream
                    //WordForWindows //PortableDocFormat //PortableDocFormat
                    (CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                Stream.Seek(0, SeekOrigin.Begin);//application/doc //
                return File(Stream, "application/doc ", "CuppsFeesNoData.pdf");
            }
        }    
        public ActionResult PrintDataGraf(int? AirLineId, DateTime? DateFrom, DateTime? DateTo)
        {
            var id = User.Identity.GetUserId();
            RoleController role = new RoleController();
            int GetCompanyId = role.GetCompanyId(id);
            var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);

            string RevenuesManager = CompanyInfo.RevenuesManager;

            string DateNow = System.DateTime.Now.ToString("yyyy/MM/dd") +"م";

           
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
            string DemandMouth = db.MouthWorkOns.Where(x =>x.Id== Month).FirstOrDefault().Mouth;
          //  string DemandYear = db.WorkOns.Where(x => x.UserId == id).FirstOrDefault().YearWorkOn.Year.ToString();

          


            string DemandDate = "مطالبة شهر " + DemandMouth +" "+ Year + "م";


            string ArName = "غير محدد";
       
            string SearchDateFrom = "غير محدد";
            string SearchDateTo = "غير محدد";
       
            ReportDocument rd = new ReportDocument();

          
            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date);
         

            if (AirLineId > 0)
            {
                Revenue = Revenue.Where(x => x.OfficerData.TowerData.AirLineId == AirLineId);
                ArName = db.AirLines.Where(x => x.Id == AirLineId).Single().ArName;
            }

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
            var RevenueList = new List<FinalInvoiceRptViewModel>();

            FinalInvoiceRptViewModel F = new FinalInvoiceRptViewModel();

            F.Name = db.AirLines.Where(x => x.Id == AirLineId).Single().Code + "/ " + db.AirLines.Where(x => x.Id == AirLineId).Single().Name;
            F.LandingFees = Convert.ToDecimal(Revenue.Sum(x => x.LandingFees));
            F.ParkingFees = Convert.ToDecimal(Revenue.Sum(x => x.ParkingFees));
            F.NightOperationFees = Convert.ToDecimal(Revenue.Sum(x => x.NightOperationFees));
            F.NavigationFees = Convert.ToDecimal(Revenue.Sum(x => x.NavigationFees));

            F.SecurityFees = Convert.ToDecimal(Revenue.Sum(x => x.SecurityFees));
            F.FirstClassFees = Convert.ToDecimal(Revenue.Sum(x => x.FirstClassFees));

            F.InterCUPPSFees = Convert.ToDecimal(Revenue.Sum(x => x.InterCUPPSFees));
            F.DomeCUPPSFees = Convert.ToDecimal(Revenue.Sum(x => x.DomeCUPPSFees));

            F.InterNYFees = Convert.ToDecimal(Revenue.Sum(x => x.InterNYFees));
            F.DomeNYFees = Convert.ToDecimal(Revenue.Sum(x => x.DomeNYFees) / ExchangeRate);

            F.PushBackFees = Convert.ToDecimal(Revenue.Sum(x => x.PushBackFees));
            F.ServiceCars = Convert.ToDecimal(Revenue.Sum(x => x.AmbolanceFees))
                + Convert.ToDecimal(Revenue.Sum(x => x.FireFightingFees));

            F.NoiseFees = Convert.ToDecimal(Revenue.Sum(x => x.NoiseFees));
            F.ExpMailFees = Convert.ToDecimal(Revenue.Sum(x => x.ExpMailFees));
            F.ImpMailFees = Convert.ToDecimal(Revenue.Sum(x => x.ImpMailFees));
            F.FreightFees = Convert.ToDecimal(Revenue.Sum(x => x.FreightFees));

            F.SubTotal = F.LandingFees + F.ParkingFees + F.NightOperationFees + F.NavigationFees + F.SecurityFees +
                F.FirstClassFees + F.FreightFees + F.ServiceCars + F.InterCUPPSFees + F.DomeCUPPSFees + F.InterNYFees +
                F.DomeNYFees + F.PushBackFees + F.ExpMailFees + F.ImpMailFees // + F.OpenAireportOvertimeFees
            + F.NoiseFees;


            F.Tax = ((F.LandingFees + F.ParkingFees + F.NightOperationFees + F.NavigationFees + F.SecurityFees +
            F.FirstClassFees + F.FreightFees + F.ServiceCars + F.InterCUPPSFees + F.DomeCUPPSFees +
            F.PushBackFees + F.ExpMailFees + F.ImpMailFees // + F.OpenAireportOvertimeFees
            + F.NoiseFees) * 5 / 100);





            F.GrandTotalDollar = (F.SubTotal + F.Tax);
            F.GrandTotalSDG = (F.SubTotal + F.Tax) * ExchangeRate;

            F.GrandTotalSDGTxt = "فقط "+ N2C.ConvertN2C.ConvertNow(Convert.ToDouble(F.GrandTotalSDG), "جنية", "قرش") + " لاغير";

            F.GrandTotalDollarTxt = "فقط " + N2C.ConvertN2C.ConvertNow(Convert.ToDouble(F.GrandTotalDollar), "دولار", "سنت") + " لاغير";

            RevenueList.Add(F);

            int CurrencyType = Convert.ToInt32(db.AirLines.Where(x => x.Id == AirLineId).Single().CurrencyType);
            if (CurrencyType == 1)
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Revenue/Graf/RptGraf.rpt")));
            }
            else
            {
                rd.Load(Path.Combine(Server.MapPath("~/Reports/Revenue/Graf/RptGrafNotSDG.rpt")));
            }
            //}

            ////*************************************************************************************
            String HeaderDate = Convert.ToDateTime(DateFrom).ToString("MMM-yyyy");
            rd.SetDataSource(RevenueList.Select(p => new
            {

                ArName = ArName,

                GrandTotalSDG = p.GrandTotalSDG,

                GrandTotalSDGTxt = p.GrandTotalSDGTxt,

                GrandTotalDollar = p.GrandTotalDollar,

                GrandTotalDollarTxt = p.GrandTotalDollarTxt,

                SearchDateFrom = SearchDateFrom,
                SearchDateTo = SearchDateTo,

                ExchangeRate = ExchangeRate,
                DemandDate = DemandDate,
                RevenuesManager = RevenuesManager,
                DateNow = DateNow,
                Serial= Serial

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
                return File(Stream, "application/pdf");
            }
        }
    }
}