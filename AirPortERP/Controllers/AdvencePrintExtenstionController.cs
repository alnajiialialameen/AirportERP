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
    public class AdvencePrintExtenstionController : Controller
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













        // GET: AdvencePrintExtenstion
        public ActionResult PrintDataCuppsNoData(int? AirLineId, DateTime? DateFrom, DateTime? DateTo, int? FlightTypeId)
        {
            var id = User.Identity.GetUserId();
            RoleController role = new RoleController();
            int GetCompanyId = role.GetCompanyId(id);
            var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);

            string UserName = CommonUsed.GetUserName(id);
            string Name = "غير محدد";
            string SearchDateFrom = "غير محدد";
            string SearchDateTo = "غير محدد";

            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.Where(x => x.OfficerData.TowerData.AirLineId == AirLineId);


            if (AirLineId > 0)
            {
                Revenue = Revenue.Where(x => x.OfficerData.TowerData.AirLineId == AirLineId);
                Name = db.AirLines.Where(x => x.Id == AirLineId).Single().Name;

                ViewBag.Name = Name;
            }


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







            if (FlightTypeId > 0)
            {
                if (FlightTypeId == 1)
                {
                    Revenue = Revenue.Where(x => (x.OfficerData.TowerData.FlightTypeD == 1 || x.OfficerData.TowerData.FlightTypeL == 1)
                    && (x.InterCUPPSFees == null || x.InterCUPPSFees <= 0));


                }
                else
                {
                    Revenue = Revenue.Where(x => x.OfficerData.TowerData.FlightTypeD == 2 && x.OfficerData.TowerData.FlightTypeL == 2
                    && x.DomeCUPPSFees == null);

                }
            }

            if (DateFrom != null && DateTo != null)
            {
                Revenue = Revenue.Where(t => DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) >= DbFunctions.TruncateTime(DateFrom) &&
                DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) <= DbFunctions.TruncateTime(DateTo));

                SearchDateFrom = CommonUsed.CheckDate(DateFrom.ToString()).ToString();
                SearchDateTo = CommonUsed.CheckDate(DateTo.ToString()).ToString();

                ViewBag.DateFrom = SearchDateFrom;
                ViewBag.DateTo = SearchDateTo;
            }

            if (Revenue.Count() <= 0)
            {
                return RedirectToAction("CuppsNoDataNotFound");
            }
            //********************************************************************************

            var RevenueListInter = new List<PrintCuppsNoDataViewModel>();
            int N = 0;

            foreach (var item in Revenue.OrderBy(x => x.OfficerData.TowerData.Date))
            {
                PrintCuppsNoDataViewModel CInter = new PrintCuppsNoDataViewModel();

                N = N + 1;
                CInter.N = N;


                CInter.Id = item.Id;
                CInter.Name = item?.OfficerData.TowerData.AirLine.Code + "/ " + item?.OfficerData.TowerData.AirLine.Name;
                CInter.Registration = item?.OfficerData.TowerData.AircraftRegistration.Registration;
                CInter.AirPortTo = item?.OfficerData.TowerData.AirPort.Code;
                CInter.TakeOffDate = CommonUsed.CheckDate(item?.OfficerData.TowerData.TakeOffDate.ToString());
                CInter.FlightNo = item?.OfficerData.TowerData.AirLinesFlightTrip.FlightNo;

                RevenueListInter.Add(CInter);
            }
            //InterNational



            if (FlightTypeId == 1)
            {
                return View("PrintDataCuppsNoDataInter", RevenueListInter);


            }
            else
            {
                return View("PrintDataCuppsNoDataDom", RevenueListInter);

            }
        }


        //////////PrintDataMail//////////////////////////////////////////////////////////////////////////////////
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
            //ReportDocument rd = new ReportDocument();

           // rd.Load(Path.Combine(Server.MapPath("~/Reports/Revenue/RptMailFees.rpt")));

            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date).Where(x => x.FreightFees > 0 || x.ExpMailFees > 0 ||
            x.ImpMailFees > 0);

            if (AirLineId > 0)
            {
                Revenue = Revenue.Where(x => x.OfficerData.TowerData.AirLineId == AirLineId);
                SearchAirLineId = db.AirLines.Where(x => x.Id == AirLineId).Single().Name;

                ViewBag.Name = SearchAirLineId;
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

            int N = 0;
            /***********************************************************************************/
            var RevenueList = new List<PrintMailViewModel>();
            foreach (var item in Revenue)
            {
                PrintMailViewModel C = new PrintMailViewModel();

                N = N + 1;
                C.N = N;



                C.Id = item.Id;
                C.Name = item?.OfficerData.TowerData.AirLine.Code + "/ " + item?.OfficerData.TowerData.AirLine.Name;
                C.FlightNo = item?.OfficerData.TowerData.AirLinesFlightTrip.FlightNo;
                C.LandingDate = CommonUsed.CheckDate(item?.OfficerData.TowerData.LandingDate.ToString());
                C.NormalMailLoadingExp = Convert.ToDouble(item?.OfficerData.NormalMailLoadingExp);
                C.RapidMailLoadingExp = Convert.ToDouble(item?.OfficerData.RapidMailLoadingExp);
                C.NormalMailLoadingImp = Convert.ToDouble(item?.OfficerData.NormalMailLoadingImp);
                C.RapidMailLoadingImp = Convert.ToDouble(item?.OfficerData.RapidMailLoadingImp);
                C.FreightLoadingExp = Convert.ToInt32(item?.OfficerData.FreightLoadingExp);
                C.FreightLoadingImp = Convert.ToInt32(item?.OfficerData.FreightLoadingImp);
                C.FreightFees = Convert.ToDecimal(item?.FreightFees);
                C.ExpMailFees = Convert.ToDecimal(item?.ExpMailFees);
                C.ImpMailFees = Convert.ToDecimal(item?.ImpMailFees);

                RevenueList.Add(C);
            }


            double SumFreightFees = Convert.ToDouble(RevenueList.Sum(x => x.FreightFees));
            ViewBag.SumFreightFees = SumFreightFees;

            double SumExpMailFees = Convert.ToDouble(RevenueList.Sum(x => x.ExpMailFees));
            ViewBag.SumExpMailFees = SumExpMailFees;

            double SumImpMailFees = Convert.ToDouble(RevenueList.Sum(x => x.ImpMailFees));
            ViewBag.SumImpMailFees = SumImpMailFees;

            return View(RevenueList);
        }



        public ActionResult PrintDataTotalInterCUPPSDetails(DateTime DateFrom)
        {
            var id = User.Identity.GetUserId();
            RoleController role = new RoleController();
            int GetCompanyId = role.GetCompanyId(id);
            var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);

            double Amount = Convert.ToDouble(db.CUPPSServiceFees.SingleOrDefault(x => x.Id == 1).AmountInDollar);
            ViewBag.Amount = Amount;

            string txtDate;
            string UserName = CommonUsed.GetUserName(id);
            string ManagerName = CompanyInfo.RevenuesManager;
            
            string SupervisorName = CompanyInfo.InterCUUPSSupervisor;

            ViewBag.ManagerName = ManagerName;
            ViewBag.SupervisorName = SupervisorName;




            if (DateFrom != null)
            {
                ExchangeRate = CommonUsed.getExchangeRateByLandingDate(DateFrom.Month, DateFrom.Year);
                ViewBag.ExchangeRate = ExchangeRate;
            }
            else
            {
                //ExchangeRate = CommonUsed.getExchangeRateByLandingDate(currentDate.Month, currentDate.Year);
                return RedirectToAction("Error");
            }

            List<spTotalCompaniesCUPPSForInter_Result> data = getTotalInterCUPPSDetails(DateFrom);
            /***********************************************************************************/
          //  ReportDocument rd = new ReportDocument();
           // rd.Load(Path.Combine(Server.MapPath("~/Reports/Revenue/RptTotalCUPPSByCompanyInter.rpt")));

            int N = 0;
            var ReportList = new List<PrintTotalCompaniesInterCUPPSModel>();
            foreach (var item in data)
            {
                PrintTotalCompaniesInterCUPPSModel C = new PrintTotalCompaniesInterCUPPSModel();

                N = N + 1;
                C.N = N;
                C.Name = item.ArName;
                C.Pax = Convert.ToInt32(item.Pax);

                //  C.TotalAmountDollar = Convert.ToDouble(item.TotalAmountDollar);
                C.TotalAmountSDG = (Convert.ToDouble(item.Pax) * Convert.ToDouble(Amount) * Convert.ToDouble(ExchangeRate));


              //  C.TaxDollar = (Convert.ToDouble(item.TaxDollar) * Convert.ToDouble(ExchangeRate) * 0.05);
                C.TaxSDG = (C.TotalAmountSDG * 0.05);



              //  C.GrandTotalDollar = Convert.ToInt32(item.GrandTotalDollar) * Convert.ToDouble(ExchangeRate);
                C.GrandTotalSDG = Convert.ToDouble(C.TotalAmountSDG) + Convert.ToDouble(C.TaxSDG);
                ReportList.Add(C);
            }
            /*************************************************************************************/
            txtDate = DateFrom.Year + " - " + DateFrom.ToString("MMMM");
            ViewBag.txtDate = txtDate;




            double SumPax = Convert.ToDouble(ReportList.Sum(x => x.Pax));
            ViewBag.SumPax = SumPax;

            double SumTotalAmountSDG = Convert.ToDouble(ReportList.Sum(x => x.TotalAmountSDG));
            ViewBag.SumTotalAmountSDG = SumTotalAmountSDG;

            double SumTaxSDG = Convert.ToDouble(ReportList.Sum(x => x.TaxSDG));
            ViewBag.SumTaxSDG = SumTaxSDG;

            double SumGrandTotalSDG = Convert.ToDouble(ReportList.Sum(x => x.GrandTotalSDG));
            ViewBag.SumGrandTotalSDG = SumGrandTotalSDG;



            double RightPart = SumTotalAmountSDG * 0.4;

            double KhartoumPart = SumTotalAmountSDG * 0.6;

            ViewBag.RightPart = RightPart;
            ViewBag.KhartoumPart = KhartoumPart;


            return View(ReportList);
        }

        public ActionResult PrintDataJournalCUPPSInter(DateTime DateFrom)
        {
            var id = User.Identity.GetUserId();
            RoleController role = new RoleController();
            int GetCompanyId = role.GetCompanyId(id);
            var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);

            string txtDate;

            decimal Amount = Convert.ToDecimal(db.CUPPSServiceFees.SingleOrDefault(x => x.Id == 1).AmountInDollar);

            string UserName = CommonUsed.GetUserName(id);
            string ManagerName = CompanyInfo.RevenuesManager;
            string SupervisorName = CompanyInfo.InterCUUPSSupervisor;

            ViewBag.ManagerName = ManagerName;
            ViewBag.SupervisorName = SupervisorName;



            if (DateFrom != null)
            {
                ExchangeRate = CommonUsed.getExchangeRateByLandingDate(DateFrom.Month, DateFrom.Year);
            }
            else
            {
                return RedirectToAction("Error");
            }

            /***********************************************************************************/
          //  ReportDocument rd = new ReportDocument();

           // rd.Load(Path.Combine(Server.MapPath("~/Reports/Revenue/RptJournalCUPPSInter.rpt")));
            //List<vwJournalCUPPSInter> data = db.spTotalCompaniesCUPPSForInter.Where(t => DbFunctions.TruncateTime(t.Date).Value.Month == DateFrom.Month && DbFunctions.TruncateTime(t.Date).Value.Year == DateFrom.Year).ToList();
            List<spTotalCompaniesCUPPSForInter_Result> data = getTotalInterCUPPSDetails(DateFrom);

            var ReportList = new List<int>();
            foreach (var item in data)
            {
                ReportList.Add(Convert.ToInt32(item.Pax));
            }

            txtDate = DateFrom.Year + " - " + DateFrom.ToString("MMMM");
            ViewBag.txtDate = txtDate;

            PrintJournalViewModel journalViewModel = new PrintJournalViewModel();
            int totalPax = ReportList.Sum();

            double TotalAmountSDG = Convert.ToDouble(totalPax) * Convert.ToDouble(Amount) * Convert.ToDouble(ExchangeRate);
            ViewBag.TotalAmountSDG = TotalAmountSDG;
            double KartoumPart = TotalAmountSDG * 0.6;
            double RightPart = TotalAmountSDG * 0.4;

            double TaxSDG = TotalAmountSDG * 0.05;

            ViewBag.KartoumPart = KartoumPart;
            ViewBag.RightPart = RightPart;
            ViewBag.TaxSDG = TaxSDG;
            return View();
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


        
        /// /////////////////////////// Ny///////////////////////////////////////////////////////////////////////
        /// ------------------------------------------------------------------------------------------------------
        
     

        public ActionResult PrintDataTotalInterNYDetails(DateTime DateFrom)
        {
            var id = User.Identity.GetUserId();
            RoleController role = new RoleController();
            int GetCompanyId = role.GetCompanyId(id);
            var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);

            double Amount = Convert.ToDouble(db.NYFees.SingleOrDefault(x => x.Id == 1).AmountInDollar);
             

            ViewBag.Amount = Amount +"$";




            string txtDate;
            string UserName = CommonUsed.GetUserName(id);
            string ManagerName = CompanyInfo.RevenuesManager;
            ViewBag.ManagerName = ManagerName;

            string SupervisorName = CompanyInfo.InterNYSupervisor;
            ViewBag.SupervisorName = SupervisorName;

            if (DateFrom != null)
            {
                ExchangeRate = CommonUsed.getExchangeRateByLandingDate(DateFrom.Month, DateFrom.Year);

                ViewBag.ExchangeRate = ExchangeRate;
            }
            else
            {
                //ExchangeRate = CommonUsed.getExchangeRateByLandingDate(currentDate.Month, currentDate.Year);
                return RedirectToAction("Error");
            }

            List<spTotalCompaniesNYForInter_Result> data = getTotalInterNYDetails(DateFrom);

            int N = 0;
            var ReportList = new List<PrintTotalCompaniesInterCUPPSviewModel>();
            foreach (var item in data)
            {
                PrintTotalCompaniesInterCUPPSviewModel C = new PrintTotalCompaniesInterCUPPSviewModel();
                N = N + 1;
                C.N = N;
                C.Name = item.Name;
                C.Pax = Convert.ToInt32(item.Pax);

                C.TotalAmountDollar = (Convert.ToDouble(item.Pax) * Convert.ToDouble(Amount));
                C.TotalAmountSDG = (Convert.ToDouble(item.Pax) * Convert.ToDouble(Amount) * Convert.ToDouble(ExchangeRate));


                //  C.TaxDollar = (Convert.ToDouble(item.TaxDollar) * Convert.ToDouble(ExchangeRate) * 0.05);
                //C.TaxSDG = (C.TotalAmountSDG * 0.05);



                //  C.GrandTotalDollar = Convert.ToInt32(item.GrandTotalDollar) * Convert.ToDouble(ExchangeRate);
                //C.GrandTotalSDG = Convert.ToDouble(C.TotalAmountSDG) + Convert.ToDouble(C.TaxSDG);
                ReportList.Add(C);
            }
            /*************************************************************************************/
            txtDate = DateFrom.Year + " - " + DateFrom.ToString("MMMM");
            ViewBag.txtDate = txtDate;




            double SumPax = Convert.ToDouble(ReportList.Sum(x => x.Pax));
            ViewBag.SumPax = SumPax;

            double SumTotalAmountSDG = Convert.ToDouble(ReportList.Sum(x => x.TotalAmountSDG));
            ViewBag.SumTotalAmountSDG = SumTotalAmountSDG;

            double SumTotalAmountDollar = Convert.ToDouble(ReportList.Sum(x => x.TotalAmountDollar));
            ViewBag.SumTotalAmountDollar = SumTotalAmountDollar;




            return View(ReportList);
        }



        public ActionResult PrintDataJournalNYInter(DateTime DateFrom)
        {
            var id = User.Identity.GetUserId();
            RoleController role = new RoleController();
            int GetCompanyId = role.GetCompanyId(id);
            var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);

            decimal Amount = Convert.ToDecimal(db.NYFees.SingleOrDefault(x => x.Id == 1).AmountInDollar);

            string UserName = CommonUsed.GetUserName(id);
            string ManagerName = CompanyInfo.RevenuesManager;
            string SupervisorName = CompanyInfo.InterNYSupervisor;
            string txtDate;

            ViewBag.SupervisorName = SupervisorName;
            ViewBag.ManagerName = ManagerName;

            if (DateFrom != null)
            {
                ExchangeRate = CommonUsed.getExchangeRateByLandingDate(DateFrom.Month, DateFrom.Year);
            }
            else
            {
                return RedirectToAction("Error");
            }

            /***********************************************************************************/
           // ReportDocument rd = new ReportDocument();

           // rd.Load(Path.Combine(Server.MapPath("~/Reports/Revenue/RptJournalNYInter.rpt")));
            //List<vwJournalNYInter> data = db.vwJournalNYInters.Where(t => DbFunctions.TruncateTime(t.Date).Value.Month ==  && DbFunctions.TruncateTime(t.Date).Value.Year == 2022).ToList();
            List<spTotalCompaniesNYForInter_Result> data = getTotalInterNYDetails(DateFrom);

            var ReportList = new List<int>();
            foreach (var item in data)
            {
                ReportList.Add(Convert.ToInt32(item.Pax));
            }

            JournalViewModel journalViewModel = new JournalViewModel();
            int totalPax = ReportList.Sum();
            double AmountSDG = Convert.ToDouble(totalPax * Amount * ExchangeRate);
            ViewBag.AmountSDG = AmountSDG;

            txtDate = DateFrom.Year + " - " + DateFrom.ToString("MMMM");
            ViewBag.txtDate = txtDate;



            return View();
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


        public ActionResult PrintDataTotalDomesticCUPPSDetails(DateTime DateFrom)
        {
            var id = User.Identity.GetUserId();
            RoleController role = new RoleController();
            int GetCompanyId = role.GetCompanyId(id);
            var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);
            string txtDate;

            double Amount = Convert.ToDouble(db.CUPPSServiceFees.SingleOrDefault(x => x.Id == 2).AmountInDollar);
            ViewBag.Amount = Amount;

            string ManagerName = CompanyInfo.RevenuesManager;
            ViewBag.ManagerName = ManagerName;

            string SupervisorName = CompanyInfo.DomeCUUPSSupervisor;
            ViewBag.SupervisorName = SupervisorName;

            string UserName = CommonUsed.GetUserName(id);

            if (DateFrom != null)
            {
                ExchangeRate = CommonUsed.getExchangeRateByLandingDate(DateFrom.Month, DateFrom.Year);
                ViewBag.ExchangeRate = ExchangeRate;
            }
            else
            {
                //ExchangeRate = CommonUsed.getExchangeRateByLandingDate(currentDate.Month, currentDate.Year);
                return RedirectToAction("Error");
            }

            List<spTotalCompaniesCUPPSForDomestic_Result> data = getTotalDomesticCUPPSDetails(DateFrom);
            /***********************************************************************************/
            //ReportDocument rd = new ReportDocument();
            //rd.Load(Path.Combine(Server.MapPath("~/Reports/Revenue/RptTotalCUPPSByCompanyDom.rpt")));

            var ReportList = new List<PrintTotalCompaniesInterCUPPSViewModel>();

            int N = 0;

            foreach (var item in data)
            {
                PrintTotalCompaniesInterCUPPSViewModel C = new PrintTotalCompaniesInterCUPPSViewModel();
                N = N + 1;
                C.N = N;
                C.Name = item.ArName;
                C.Pax = Convert.ToInt32(item.Pax);

                //  C.TotalAmountDollar = Convert.ToDouble(item.TotalAmountDollar);
                C.TotalAmountSDG = (Convert.ToDouble(item.Pax) * Convert.ToDouble(Amount) * Convert.ToDouble(ExchangeRate));


                //  C.TaxDollar = (Convert.ToDouble(item.TaxDollar) * Convert.ToDouble(ExchangeRate) * 0.05);
                C.TaxSDG = (C.TotalAmountSDG * 0.05);



                //  C.GrandTotalDollar = Convert.ToInt32(item.GrandTotalDollar) * Convert.ToDouble(ExchangeRate);
                C.GrandTotalSDG = Convert.ToDouble(C.TotalAmountSDG) + Convert.ToDouble(C.TaxSDG);

                ReportList.Add(C);
            }
            /*************************************************************************************/
            /*************************************************************************************/
            txtDate = DateFrom.Year + " - " + DateFrom.ToString("MMMM");
            ViewBag.txtDate = txtDate;




            double SumPax = Convert.ToDouble(ReportList.Sum(x => x.Pax));
            ViewBag.SumPax = SumPax;

            double SumTotalAmountSDG = Convert.ToDouble(ReportList.Sum(x => x.TotalAmountSDG));
            ViewBag.SumTotalAmountSDG = SumTotalAmountSDG;

            double SumTaxSDG = Convert.ToDouble(ReportList.Sum(x => x.TaxSDG));
            ViewBag.SumTaxSDG = SumTaxSDG;

            double SumGrandTotalSDG = Convert.ToDouble(ReportList.Sum(x => x.GrandTotalSDG));
            ViewBag.SumGrandTotalSDG = SumGrandTotalSDG;



            double RightPart = SumTotalAmountSDG * 0.4;

            double KhartoumPart = SumTotalAmountSDG * 0.6;

            ViewBag.RightPart = RightPart;
            ViewBag.KhartoumPart = KhartoumPart;
            return View(ReportList);
        }


        public ActionResult PrintDataJournalCUPPSDome(DateTime DateFrom)
        {
            var id = User.Identity.GetUserId();
            RoleController role = new RoleController();
            int GetCompanyId = role.GetCompanyId(id);
            var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);

            decimal Amount = Convert.ToDecimal(db.CUPPSServiceFees.SingleOrDefault(x => x.Id != 1).AmountInDollar);

            string UserName = CommonUsed.GetUserName(id);
            string ManagerName = CompanyInfo.RevenuesManager;
            ViewBag.ManagerName = ManagerName;


            string SupervisorName = CompanyInfo.DomeCUUPSSupervisor;
            ViewBag.SupervisorName = SupervisorName;
            string txtDate;




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

            PrintJournalCuppsDetailsViewModel journalViewModel = new PrintJournalCuppsDetailsViewModel();
            int totalPax = ReportList.Sum();
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            txtDate = DateFrom.Year + " - " + DateFrom.ToString("MMMM");
            ViewBag.txtDate = txtDate;



            double TotalAmountSDG = Convert.ToDouble(totalPax) * Convert.ToDouble(Amount) * Convert.ToDouble(ExchangeRate);            
            double KartoumPart = TotalAmountSDG * 0.6;
            double RightPart = TotalAmountSDG * 0.4;

            double TaxSDG = TotalAmountSDG * 0.05;

            TotalAmountSDG += TaxSDG;
            ViewBag.TotalAmountSDG = TotalAmountSDG;
            ViewBag.KartoumPart = KartoumPart;
            ViewBag.RightPart = RightPart;
            ViewBag.TaxSDG = TaxSDG;


            return View();
        }




        //----------------------------------------------------------------------------------------------------------------------

        //========================================================================================================================
        public ActionResult PrintDataTotalDomesticNYDetails(DateTime DateFrom)
        {
            var id = User.Identity.GetUserId();
            RoleController role = new RoleController();
            int GetCompanyId = role.GetCompanyId(id);
            var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);

            double Amount = Convert.ToDouble(db.NYFees.SingleOrDefault(x => x.Id != 1).AmountInSDG);
            ViewBag.Amount = Amount +"ج";

            string UserName = CommonUsed.GetUserName(id);
            string txtDate;

            string ManagerName = CompanyInfo.RevenuesManager;
            ViewBag.ManagerName = ManagerName;
            string SupervisorName = CompanyInfo.DomeNYSupervisor;
            ViewBag.SupervisorName = SupervisorName;

            if (DateFrom != null)
            {
                ExchangeRate = CommonUsed.getExchangeRateByLandingDate(DateFrom.Month, DateFrom.Year);
                ViewBag.ExchangeRate = ExchangeRate;
            }
            else
            {
                //ExchangeRate = CommonUsed.getExchangeRateByLandingDate(currentDate.Month, currentDate.Year);
                return RedirectToAction("Error");
            }

            List<spTotalCompaniesNYForDomestic_Result> data = getTotalDomesticNYDetails(DateFrom);
            /***********************************************************************************/
            int N = 0;

            var ReportList = new List<PrintTotalCompaniesInterCUPPS>();
            foreach (var item in data)
            {
                PrintTotalCompaniesInterCUPPS C = new PrintTotalCompaniesInterCUPPS();
                N = N + 1;
                C.N = N;
                C.Name = item.Name;
                C.Pax = Convert.ToInt32(item.Pax);

                C.TotalAmountDollar = ((Convert.ToDouble(item.Pax) * Convert.ToDouble(Amount))/ Convert.ToDouble(ExchangeRate));
                C.TotalAmountSDG = (Convert.ToDouble(item.Pax) * Convert.ToDouble(Amount));


                //  C.TaxDollar = (Convert.ToDouble(item.TaxDollar) * Convert.ToDouble(ExchangeRate) * 0.05);
                //C.TaxSDG = (C.TotalAmountSDG * 0.05);



                //  C.GrandTotalDollar = Convert.ToInt32(item.GrandTotalDollar) * Convert.ToDouble(ExchangeRate);
                //C.GrandTotalSDG = Convert.ToDouble(C.TotalAmountSDG) + Convert.ToDouble(C.TaxSDG);
                ReportList.Add(C);
            }
            /*************************************************************************************/
            txtDate = DateFrom.Year + " - " + DateFrom.ToString("MMMM");
            ViewBag.txtDate = txtDate;




            double SumPax = Convert.ToDouble(ReportList.Sum(x => x.Pax));
            ViewBag.SumPax = SumPax;

            double SumTotalAmountSDG = Convert.ToDouble(ReportList.Sum(x => x.TotalAmountSDG));
            ViewBag.SumTotalAmountSDG = SumTotalAmountSDG;

            double SumTotalAmountDollar = Convert.ToDouble(ReportList.Sum(x => x.TotalAmountDollar));
            ViewBag.SumTotalAmountDollar = SumTotalAmountDollar;

            return View(ReportList);
        }




        public ActionResult PrintDataJournalNYDome(DateTime DateFrom)
        {
            var id = User.Identity.GetUserId();
            RoleController role = new RoleController();
            int GetCompanyId = role.GetCompanyId(id);
            var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);

            decimal Amount = Convert.ToDecimal(db.NYFees.SingleOrDefault(x => x.Id != 1).AmountInSDG);

            string UserName = CommonUsed.GetUserName(id);
            string ManagerName = CompanyInfo.RevenuesManager;
            ViewBag.ManagerName = ManagerName;
            string SupervisorName = CompanyInfo.DomeNYSupervisor;
            ViewBag.SupervisorName = SupervisorName;
            string txtDate;

            if (DateFrom != null)
            {
                ExchangeRate = CommonUsed.getExchangeRateByLandingDate(DateFrom.Month, DateFrom.Year);
            }
            else
            {
                return RedirectToAction("Error");
            }

            /***********************************************************************************/
           
            List<spTotalCompaniesNYForDomestic_Result> data = getTotalDomesticNYDetails(DateFrom);

            var ReportList = new List<int>();
            foreach (var item in data)
            {
                ReportList.Add(Convert.ToInt32(item.Pax));
            }

            int totalPax = ReportList.Sum();
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            
            double AmountSDG = Convert.ToDouble(totalPax * Amount);
            ViewBag.AmountSDG = AmountSDG;

            txtDate = DateFrom.Year + " - " + DateFrom.ToString("MMMM");
            ViewBag.txtDate = txtDate;

            return View();
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














      //******************************************CarDetails**************************************************************
   



        public ActionResult PrintDataTotalServiceCarDetails(DateTime DateFrom, DateTime DateTo)
        {
            var id = User.Identity.GetUserId();
            RoleController role = new RoleController();
            int GetCompanyId = role.GetCompanyId(id);
            var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);

            decimal FinalAmountForSudanAirWays = 0;
            string SearchDateFrom;
            string SearchDateTo;
            string txtDate;
             double ExchangeRate;
            string UserName = CommonUsed.GetUserName(id);
            string ManagerName = CompanyInfo.RevenuesManager;
            ViewBag.ManagerName = ManagerName;

            string SupervisorName = CompanyInfo.ServiceCarSupervisor;
            ViewBag.SupervisorName = SupervisorName;

            if (DateFrom != null && DateTo != null) 
            {
                ExchangeRate = Convert.ToDouble(CommonUsed.getExchangeRateByLandingDate(DateFrom.Month, DateFrom.Year));
                ViewBag.ExchangeRate = ExchangeRate;

                FinalAmountForSudanAirWays = cal.FinalAmountForSudanAirWays(DateFrom);


                SearchDateFrom = CommonUsed.CheckDate(DateFrom.ToString()).ToString();
                SearchDateTo = CommonUsed.CheckDate(DateTo.ToString()).ToString();

                ViewBag.DateTo = SearchDateTo;
                ViewBag.DateFrom = SearchDateFrom;






            }
            else
            {
                //ExchangeRate = CommonUsed.getExchangeRateByLandingDate(currentDate.Month, currentDate.Year);
                return RedirectToAction("Error");
            }

            List<spTotalCompaniesServiceCarFees_Result> data = getTotalServiceCarDetails(DateFrom, DateTo);
            /***********************************************************************************/
            int N = 0;

            var ReportList = new List<PrintServiceCarViewModel>();
            foreach (var item in data)
            {
                PrintServiceCarViewModel C = new PrintServiceCarViewModel();
                N = N + 1;
                C.N = N;
                C.Name = item.ArName;
                C.TotalACarCount = Convert.ToInt32(item.TotalACarCount);
                C.TotalFCarCount = Convert.ToInt32(item.TotalFCarCount);
                C.AmbolanceFees = Convert.ToDouble(item.AmbolanceFees);
                C.FireFightingFees = Convert.ToDouble(item.FireFightingFees);
                C.AmbolanceAndFireFees = Convert.ToDouble(C.FireFightingFees+C.AmbolanceFees);

                C.AmbolanceAndFireFeesSDG = Convert.ToDouble(C.FireFightingFees + C.AmbolanceFees ) * Convert.ToDouble(ExchangeRate);

                ReportList.Add(C);
            }
            /*************************************************************************************/


            txtDate = DateFrom.Year + " - " + DateFrom.ToString("MMMM");
            ViewBag.txtDate = txtDate;




            double SumTotalACarCount = Convert.ToDouble(ReportList.Sum(x => x.TotalACarCount));
            ViewBag.SumTotalACarCount = SumTotalACarCount;

            double SumTotalFCarCount = Convert.ToDouble(ReportList.Sum(x => x.TotalFCarCount));
            ViewBag.SumTotalFCarCount = SumTotalFCarCount;

            double SumAmbolanceAndFireFees = Convert.ToDouble(ReportList.Sum(x => x.AmbolanceAndFireFees));
            ViewBag.SumAmbolanceAndFireFees = SumAmbolanceAndFireFees;

            double SumAmbolanceAndFireFeesSDG = Convert.ToDouble(ReportList.Sum(x => x.AmbolanceAndFireFeesSDG));
            ViewBag.SumAmbolanceAndFireFeesSDG = SumAmbolanceAndFireFeesSDG;


            return View(ReportList);
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

        public ActionResult PrintDataTotalPushbackDetails(DateTime DateFrom, decimal LastRequestedAmountSDG)
        {
            var id = User.Identity.GetUserId();
            RoleController role = new RoleController();
            int GetCompanyId = role.GetCompanyId(id);
            var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);

            decimal FinalAmountForSudanAirWays = 0;

            string UserName = CommonUsed.GetUserName(id);
            string ManagerName = CompanyInfo.RevenuesManager;
            ViewBag.ManagerName = ManagerName;

            string SupervisorName = CompanyInfo.PushBackSupervisor;
            ViewBag.SupervisorName = SupervisorName;
            double TotalRequested;

            string txtDate;

            if (DateFrom != null)
            {
                ExchangeRate = CommonUsed.getExchangeRateByLandingDate(DateFrom.Month, DateFrom.Year);



                FinalAmountForSudanAirWays = cal.FinalAmountForSudanAirWays(DateFrom);
                ViewBag.FinalAmountForSudanAirWays = FinalAmountForSudanAirWays;
                ViewBag.ExchangeRate = ExchangeRate;
                ViewBag.LastRequestedAmountSDG = LastRequestedAmountSDG;

                TotalRequested = Convert.ToDouble(LastRequestedAmountSDG + FinalAmountForSudanAirWays);
                ViewBag.TotalRequested = TotalRequested;

            }
            else
            {
                //ExchangeRate = CommonUsed.getExchangeRateByLandingDate(currentDate.Month, currentDate.Year);
                return RedirectToAction("Error");
            }

            List<spTotalCompaniesPushbackFees_Result> data = getTotalPushbackDetails(DateFrom);
            /***********************************************************************************/
            int N = 0;

            var ReportList = new List<PrintTotalCompaniesPushPackModel>();


            foreach (var item in data)
            {
                PrintTotalCompaniesPushPackModel C = new PrintTotalCompaniesPushPackModel();
                N = N + 1;
                C.N = N;
                C.Name = item.ArName;
                C.TotalAmountDollar = Convert.ToDouble(item.PushBackFees);
                C.TotalAmountSDG = Convert.ToDouble(item.PushBackFees) * Convert.ToDouble(ExchangeRate);
                C.TaxSDG = C.TotalAmountSDG * 0.05;
                C.GrandTotalSDG = C.TotalAmountSDG + C.TaxSDG;

                ReportList.Add(C);
            }
            /*************************************************************************************/

            /*************************************************************************************/
            txtDate = DateFrom.Year + " - " + DateFrom.ToString("MMMM");
            ViewBag.txtDate = txtDate;




           

            double SumTotalAmountSDG = Convert.ToDouble(ReportList.Sum(x => x.TotalAmountSDG));
            ViewBag.SumTotalAmountSDG = SumTotalAmountSDG;

            double SumTaxSDG = Convert.ToDouble(ReportList.Sum(x => x.TaxSDG));
            ViewBag.SumTaxSDG = SumTaxSDG;

            double SumGrandTotalSDG = Convert.ToDouble(ReportList.Sum(x => x.GrandTotalSDG));
            ViewBag.SumGrandTotalSDG = SumGrandTotalSDG;



            double SudanAirPart = SumTotalAmountSDG * 0.6;
            double SudanAirPartDis = SudanAirPart * 0.05;


            double KhartoumPart = SumTotalAmountSDG * 0.4;
            double KhartoumPartDis = KhartoumPart * 0.05;



            ViewBag.SudanAirPart = SudanAirPart;
            ViewBag.KhartoumPart = KhartoumPart;

            ViewBag.SudanAirPartDis = SudanAirPartDis;
            ViewBag.KhartoumPartDis = KhartoumPartDis;

            double LastTotalRequested = TotalRequested + SudanAirPartDis;

            ViewBag.LastTotalRequested = LastTotalRequested;


            double NetTotalRequest = SudanAirPart - LastTotalRequested;
            ViewBag.NetTotalRequest = NetTotalRequest;















            return View(ReportList);
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






        public ActionResult CuppsNoDataNotFound() {

            return View();
        }


        /// <summary>
        /// /
        /// </summary>
         ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
         //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
         //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
         ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ActionResult SearchDataTotalAllFees(int? page, int? Get, int? Search, int? Count, DateTime? DateFrom)
        {
            List<spAllFeesRep_Result> Revenue;
            Revenue = getTotalAllFees(DateTime.Now);
            ViewBag.DateFrom = DateFrom;

            if (Get > 0)
            {
                if (DateFrom != null)
                {
                    Revenue = getTotalAllFees(Convert.ToDateTime(DateFrom));
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
                return PartialView("_GetTotalAllFees", Revenue.ToPagedList(PageNumber, pageSize));
            }
        }

        private List<spAllFeesRep_Result> getTotalAllFees(DateTime DateFrom)
        {
            int Year = DateFrom.Year;
            int Month = DateFrom.Month;
            List<spAllFeesRep_Result> data = db.spAllFeesRep(Month, Year).ToList();

            if (data.Count() <= 0)
            {
                return null;
            }

            return data;
        }

        public ActionResult PrintDataTotalAllFees(DateTime DateFrom)
        {
            var id = User.Identity.GetUserId();
            RoleController role = new RoleController();
            int GetCompanyId = role.GetCompanyId(id);
            var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);

         ///   decimal FinalAmountForSudanAirWays = 0;

            string UserName = CommonUsed.GetUserName(id);
            string ManagerName = CompanyInfo.RevenuesManager;
            ViewBag.ManagerName = ManagerName;

            string SupervisorName = CompanyInfo.PushBackSupervisor;
            ViewBag.SupervisorName = SupervisorName;
          //  double TotalRequested;

            string txtDate;

            if (DateFrom != null)
            {
                ExchangeRate = CommonUsed.getExchangeRateByLandingDate(DateFrom.Month, DateFrom.Year);



              //  FinalAmountForSudanAirWays = cal.FinalAmountForSudanAirWays(DateFrom);
              ///  ViewBag.FinalAmountForSudanAirWays = FinalAmountForSudanAirWays;
               // ViewBag.ExchangeRate = ExchangeRate;
              ///  ViewBag.LastRequestedAmountSDG = LastRequestedAmountSDG;

              //  TotalRequested = Convert.ToDouble(LastRequestedAmountSDG + FinalAmountForSudanAirWays);
              //  ViewBag.TotalRequested = TotalRequested;

            }
            else
            {
                //ExchangeRate = CommonUsed.getExchangeRateByLandingDate(currentDate.Month, currentDate.Year);
                return RedirectToAction("Error");
            }

            List<spAllFeesRep_Result> data = getTotalAllFees(DateFrom);
            /***********************************************************************************/
            int N = 0;

            var ReportList = new List<PrintAllFeesViewModel>();


            foreach (var item in data)
            {
                PrintAllFeesViewModel C = new PrintAllFeesViewModel();
                N = N + 1;
                C.N = N;
                C.Name = item.Name;

                C.LandingFees = Convert.ToDouble(item.LandingFees) * Convert.ToDouble(ExchangeRate);  
                C.ParkingFees = Convert.ToDouble(item.ParkingFees) * Convert.ToDouble(ExchangeRate); 
                C.NightOperationFees = Convert.ToDouble(item.NightOperationFees) * Convert.ToDouble(ExchangeRate);
                C.NavigationFees = Convert.ToDouble(item.NavigationFees) * Convert.ToDouble(ExchangeRate);

                C.SecurityFees = Convert.ToDouble(item.SecurityFees) * Convert.ToDouble(ExchangeRate);
                C.FreightFees = Convert.ToDouble(item.FreightFees) * Convert.ToDouble(ExchangeRate);
                C.FirstClassFees = Convert.ToDouble(item.FirstClassFees) * Convert.ToDouble(ExchangeRate);
                //  C.FireFightingFees = Convert.ToDouble(item.FireFightingFees);
                C.ServiceCars = Convert.ToDouble(item.AmbolanceFees) + Convert.ToDouble(item.FireFightingFees)
                    * Convert.ToDouble(ExchangeRate);


                C.CuppsFees = (Convert.ToDouble(item.InterCUPPSFees) + Convert.ToDouble(item.DomeCUPPSFees)) * Convert.ToDouble(ExchangeRate);
                C.NyFees = (Convert.ToDouble(item.InterNYFees) * Convert.ToDouble(ExchangeRate)) + Convert.ToDouble(item.DomeNYFees);


                C.PushBackFees = Convert.ToDouble(item.PushBackFees) * Convert.ToDouble(ExchangeRate);
                C.MailFees = (Convert.ToDouble(item.ExpMailFees) + Convert.ToDouble(item.ImpMailFees)) * Convert.ToDouble(ExchangeRate);

                C.NoiseFees = Convert.ToDouble(item.NoiseFees) * Convert.ToDouble(ExchangeRate);
                C.Total = (C.LandingFees + C.ParkingFees + C.NightOperationFees + C.NavigationFees + C.SecurityFees
                    + C.FreightFees + C.FirstClassFees + C.ServiceCars + C.CuppsFees + C.NyFees + C.PushBackFees + C.MailFees + C.NoiseFees);

                C.TotalWithoutNy = (C.LandingFees + C.ParkingFees + C.NightOperationFees + C.NavigationFees + C.SecurityFees
                 + C.FreightFees + C.FirstClassFees + C.ServiceCars + C.CuppsFees + C.PushBackFees + C.MailFees + C.NoiseFees);

                C.Tax = C.TotalWithoutNy * 0.05;
                C.GrandTotal = C.Total + C.Tax;

                ReportList.Add(C);


            }
            /*************************************************************************************/

            /*************************************************************************************/
            txtDate = DateFrom.Year + " - " + DateFrom.ToString("MMMM");
            ViewBag.txtDate = txtDate;






            return View(ReportList);
        }










        public ActionResult TotalAllFees()
        {
            return View();
        }


    }
    }
