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
using System.IO;
using AirPortERP.ViewModels;
using System.ComponentModel.Design;

namespace AirPortERP.Controllers
{
    public class RevenuesController : Controller
    {
        private Entities db = new Entities();
        private CommonUsed CommonUsed = new CommonUsed();
        private RptExtention Ex = new RptExtention();
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
            var id = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(id);

            string SearchDateFrom = "غير محدد";
            string SearchDateTo = "غير محدد";

            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date).Where(x => x.OfficerData.TowerData.CompanyInfoId == companyId);

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

        /******************************************************* CUPPS *************************************************************************/
        public ActionResult GetDataCupps(int? page, int? Get, int? Search, int? Count, int? AirLineId, DateTime? DateFrom, DateTime? DateTo
                                        , int? FlightTypeId)
        {
            IQueryable<Revenue> Revenue;
            var id = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(id);
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date).Where(x => x.OfficerData.TowerData.CompanyInfoId == companyId);

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
        /***************************************************** Service Car ***************************************************************************/
        public ActionResult GetDataServiceCar(int? page, int? Get, int? Search, int? Count, int? AirLineId
            , DateTime? DateFrom, DateTime? DateTo)
        {
            var id = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(id);
            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.Include(q => q.OfficerData).Include(q => q.OfficerData.TowerData).OrderBy(x => x.OfficerData.TowerData.Date).Where(x => (x.AmbolanceFees > 0 || x.FireFightingFees > 0) && x.OfficerData.TowerData.CompanyInfoId == companyId);

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
        /******************************************************** Landing ************************************************************************/
        public ActionResult GetDataLanding(int? page, int? Get, int? Search, int? Count, int? AirLineId
           , DateTime? DateFrom, DateTime? DateTo, int? FlightTypeId)
        {
            var id = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(id);
            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date).Where(x => x.OfficerData.TowerData.CompanyInfoId == companyId);

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
        /************************************************************ Mail & Frieght ********************************************************************/
        public ActionResult GetDataMail(int? page, int? Get, int? Search, int? Count, int? AirLineId
           , DateTime? DateFrom, DateTime? DateTo)
        {
            var id = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(id);
            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date).Where(x => x.FreightFees > 0 || x.ExpMailFees > 0 ||
             x.ImpMailFees > 0 && x.OfficerData.TowerData.CompanyInfoId == companyId);

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
        /******************************************************** First Class ************************************************************************/
        public ActionResult GetDataFirstClass(int? page, int? Get, int? Search, int? Count, int? AirLineId
          , DateTime? DateFrom, DateTime? DateTo)
        //, int? FlightTypeId)
        {
            var id = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(id);
            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date).Where(x => x.FirstClassFees > 0 && x.OfficerData.TowerData.CompanyInfoId == companyId);

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


        /************************************************************ PushBack ********************************************************************/
        public ActionResult GetPushPack(int? page, int? Get, int? Search, int? Count, int? AirLineId
         , DateTime? DateFrom, DateTime? DateTo
        )
        {
            var id = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(id);
            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date).Where(x => x.PushBackFees != null && x.PushBackFees > 0 && x.OfficerData.TowerData.CompanyInfoId == companyId);

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

        /********************************************************* Final Invoice *******************************************************************/
        public ActionResult GetDataFinalInvoice(int? page, int? Get, int? Search, int? Count, int? AirLineId
          , DateTime? DateFrom, DateTime? DateTo)
        //   , int? FlightTypeId)
        {
            var id = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(id);
            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date).Where(x => x.OfficerData.TowerData.CompanyInfoId == companyId);

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



        /********************************************************* New Report **********************************************************************/
        public ActionResult NewReport()
        {
            return View();
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
            Revenue = db.Revenues.Where(m => m.OfficerData.IsCurrentAccount == true).OrderBy(x => x.OfficerData.TowerData.Date);

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




        /********************************************************* Inter CUPPS Details ***********************************************************************/
        public ActionResult TotalInterCUPPSDetails()
        {
            return View();
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
    
    }
}