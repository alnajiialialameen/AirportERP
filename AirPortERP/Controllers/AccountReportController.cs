using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AirPortERP.ViewModels;
using AirPortERP.Models;
using System.Data.Entity;
using PagedList;
using AirPortERP.Extenstion;
using Microsoft.AspNet.Identity;
using NumberToWord;
using System.Globalization;

namespace AirPortERP.Controllers
{
    public class AccountReportController : Controller
    {
        List<CurrencyInfo> currencies = new List<CurrencyInfo>();
        private CommonUsed CommonUsed = new CommonUsed();
        Entities db = new Entities();
        ClsAccount ClsAccount = new ClsAccount();
        private int Format, Month, Year;
        static decimal ExchangeRate;
        // GET: AccountReport




        public AccountReportController()
        {

            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Sudan));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Dollar));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Syria));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.UAE));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.SaudiArabia));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Tunisia));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Gold));


        }




        public ActionResult ComapniesPaymentStatus()
        {
            List<GettVM> data1 = new List<GettVM>();
            try
            {
                var data = db.AccountOperations.Where(d => d.AccountTree.AccGroupId == 1 & d.CurrencyId == 2).Select(x =>
                      new GettVM()
                      {
                          Id = x.AccountNumber,
                          AirLineName = x.AccountTree.AirLines.FirstOrDefault().Name,
                          Debit = x.OperationDebit,
                          Credit = x.OperationCredit,
                          CurrencyValue = x.CurrencyValue,
                          Balance = x.OperationDebit - x.OperationCredit,
                      }
                ).ToList();

                foreach (var item in data)
                {
                    if (!data1.Any(x => x.Id == item.Id))
                    {
                        data1.Add(item);
                    }
                    else
                    {
                        var obj = data1.FirstOrDefault(x => x.Id == item.Id);
                        obj.Credit = obj.Credit + item.Credit;
                        obj.Debit = obj.Debit + item.Debit;
                        obj.Balance = obj.Balance + item.Balance;
                    }
                }

                ViewBag.SumOfDebit = data1.Sum(x => x.Debit);
                ViewBag.SumOfCredit = data1.Sum(x => x.Credit);
                ViewBag.SumOfBalance = data1.Sum(x => x.Balance);
                ViewBag.Count = data1.Count();

                decimal? balance = data1.Sum(x => x.Balance);

                //ToWord toWord1 = new ToWord(Convert.ToDecimal(balance), currencies[Convert.ToInt32(0)]);
                //ViewBag.GrandTotalDollarTxt = toWord1.ConvertToArabic().ToString();


                return View(data1);
            }
            catch (Exception e)
            {
                return Json(e.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult ComapniesPaymentStatusDollar()
        {
            List<GettVM> data1 = new List<GettVM>();
            try
            {
                var data = db.AccountOperations.Where(d => d.AccountTree.AccGroupId == 1 & d.CurrencyId == 1).Select(x =>
                      new GettVM()
                      {
                          Id = x.AccountNumber,
                          AirLineName = x.AccountTree.AirLines.FirstOrDefault().Name,
                          Debit = x.OperationDebit,
                          Credit = x.OperationCredit,
                          CurrencyValue = x.CurrencyValue,
                          Balance = x.OperationDebit - x.OperationCredit,
                      }
                ).ToList();

                foreach (var item in data)
                {
                    if (!data1.Any(x => x.Id == item.Id))
                    {
                        data1.Add(item);
                    }
                    else
                    {
                        var obj = data1.FirstOrDefault(x => x.Id == item.Id);
                        obj.Credit = obj.Credit + item.Credit;
                        obj.Debit = obj.Debit + item.Debit;
                        obj.Balance = obj.Balance + item.Balance;
                    }
                }

                ViewBag.SumOfDebit = data1.Sum(x => x.Debit);
                ViewBag.SumOfCredit = data1.Sum(x => x.Credit);
                ViewBag.SumOfBalance = data1.Sum(x => x.Balance);
                ViewBag.Count = data1.Count();

                decimal? balance = data1.Sum(x => x.Balance);

                return View(data1);
            }
            catch (Exception e)
            {
                return Json(e.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }




        public ActionResult PrintSandConstraint(int? id)
        {

            IQueryable<AccountOperation> AccountOperation = db.AccountOperations.Where(x => x.OperationID == id && x.OperationTypeId == 2);

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            ///***********************************************************************************
            var OperationList = new List<OperationViewModel>();
            int N = 0;
            foreach (var item in AccountOperation)
            {
                OperationViewModel L = new OperationViewModel();
                L.Id = item.Id;
                N = N + 1;
                L.N = N;

                L.OperationTypeName = item?.OperationType.Name;

                L.AccountName = item?.AccountTree.AccName;
                L.AccountNumber = Convert.ToDecimal(item?.AccountTree.AccNumber);

                L.Debit = Convert.ToDecimal(item?.OperationDebit);
                L.Credit = Convert.ToDecimal(item?.OperationCredit);

                L.OperationTypeName = item?.OperationType.Name;
                L.CurrencyName = item?.Currency.CurrencyName;

                L.Statement = item?.OperationStatement;
                OperationList.Add(L);
            }





            return View(OperationList);


        }

        public ActionResult PrintSandConstraintSarf(int? id)
        {

            IQueryable<AccountOperation> AccountOperation = db.AccountOperations.Where(x => x.OperationID == id && x.OperationTypeId == 1);

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            ///***********************************************************************************
            var OperationList = new List<OperationViewModel>();
            int N = 0;
            foreach (var item in AccountOperation)
            {
                OperationViewModel L = new OperationViewModel();
                L.Id = item.Id;
                N = N + 1;
                L.N = N;

                L.OperationTypeName = item?.OperationType.Name;

                L.AccountName = item?.AccountTree.AccName;
                L.AccountNumber = Convert.ToDecimal(item?.AccountTree.AccNumber);

                L.Debit = Convert.ToDecimal(item?.OperationDebit);
                L.Credit = Convert.ToDecimal(item?.OperationCredit);

                L.OperationTypeName = item?.OperationType.Name;
                L.CurrencyName = item?.Currency.CurrencyName;

                L.Statement = item?.OperationStatement;
                OperationList.Add(L);
            }





            return View(OperationList);


        }













        public ActionResult PrintClaimConstraint(int? id)
        {

            IQueryable<AccountOperation> AccountOperation = db.AccountOperations.Where(x => x.OperationID == id && x.OperationTypeId == 3);

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            ///***********************************************************************************
            var OperationList = new List<OperationViewModel>();
            int N = 0;
            foreach (var item in AccountOperation)
            {
                OperationViewModel L = new OperationViewModel();
                L.Id = item.Id;
                N = N + 1;
                L.N = N;

                L.OperationTypeName = item?.OperationType.Name;

                L.AccountName = item?.AccountTree.AccName;
                L.AccountNumber = Convert.ToDecimal(item?.AccountTree.AccNumber);

                L.Debit = Convert.ToDecimal(item?.OperationDebit);
                L.Credit = Convert.ToDecimal(item?.OperationCredit);

                L.OperationTypeName = item?.OperationType.Name;
                L.CurrencyName = item?.Currency.CurrencyName;

                L.Statement = item?.OperationStatement;
                OperationList.Add(L);
            }





            return View(OperationList);


        }

        public ActionResult PrintOpenConstraint(int? id)
        {

            IQueryable<AccountOperation> AccountOperation = db.AccountOperations.Where(x => x.OperationID == id && x.OperationTypeId == 4);

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            ///***********************************************************************************
            var OperationList = new List<OperationViewModel>();
            int N = 0;
            foreach (var item in AccountOperation)
            {
                OperationViewModel L = new OperationViewModel();
                L.Id = item.Id;
                N = N + 1;
                L.N = N;

                L.OperationTypeName = item?.OperationType.Name;

                L.AccountName = item?.AccountTree.AccName;
                L.AccountNumber = Convert.ToDecimal(item?.AccountTree.AccNumber);

                L.Debit = Convert.ToDecimal(item?.OperationDebit);
                L.Credit = Convert.ToDecimal(item?.OperationCredit);

                L.OperationTypeName = item?.OperationType.Name;
                L.CurrencyName = item?.Currency.CurrencyName;

                L.Statement = item?.OperationStatement;
                OperationList.Add(L);
            }
            
            return View(OperationList);
        }

        ///------------------------------Sand Report------------------------------------------------------------------------------------------------


        public ActionResult Sand()
        {
            return View();
        }

        public ActionResult GetDataSand(int? page,
        int? Get, int? Search, DateTime? DateFrom, DateTime? DateTo, int? AccId, int? YearId, int? MonthId)
        
        {

          
            IQueryable<SandDetail> Sand;
            Sand = db.SandDetails.Where(x => x.SandMain.OperationTypeId == 2).OrderBy(x => x.Id);
            ViewBag.AccId = AccId;

            ViewBag.DateFrom = DateFrom;
            ViewBag.DateTo = DateTo;


            ViewBag.YearId = YearId;
            ViewBag.MonthId = MonthId;

            if (Get > 0)
            {
                if (AccId > 0)
                {

                    Sand = Sand.OrderBy(x => x.Id).Where(x => x.AccId == AccId);
                }


                if (YearId > 0)
                {

                    Sand = Sand.OrderBy(x => x.Id).Where(x => x.YearId == YearId);
                }

                if (MonthId > 0)
                {

                    Sand = Sand.OrderBy(x => x.Id).Where(x => x.MonthId == MonthId);
                }


                if (DateFrom != null && DateTo != null)
                {
                    ///SearchDateFrom = CommonUsed.CheckDate(DateFrom.ToString());
                    ///SearchDateTo = CommonUsed.CheckDate(DateTo.ToString());
                    Sand = Sand.Where(t => DbFunctions.TruncateTime(t.SandMain.SandDate) >= DbFunctions.TruncateTime(DateFrom)
                    && DbFunctions.TruncateTime(t.SandMain.SandDate) <= DbFunctions.TruncateTime(DateTo));
                }

                //////////////////////////////////////////////////////
                if (Search == 1)
                {

                    if (Sand != null)
                    {

                        int ItemCount = 0;
                        ItemCount = Sand.Count();
                        ViewBag.ItemCount = ItemCount;

                    }
                }
                else
                {


                    ViewBag.ItemCount = 0;
                }

                //////////////////////////////////////////////////////////////////////

            }
            else
            {
                Sand = null;
                ViewBag.ItemCount = 0;
            }

            int pageSize = 5;
            int PageNumber = (page ?? 1);

         
            return PartialView("_GetDataSand", Sand.ToPagedList(PageNumber, pageSize));
        }



        public ActionResult PrintSand(DateTime? DateFrom, DateTime? DateTo, int? AccId, int? YearId, int? MonthId) 
        {

            IQueryable<SandDetail> Sand = db.SandDetails.Where(x => x.SandMain.OperationTypeId == 2);
            string DateToR = "غير محدد";
            string DateFromR = "غير محدد";
            string AccName = "كل الحسابات";


            string Year ="";
            string Mouth ="";

            decimal SumCreditDoller;
            decimal SumCreditSDN;

            if (AccId > 0)
            {

                Sand = Sand.OrderBy(x => x.Id).Where(x => x.AccId == AccId);
                AccName = db.AccountTrees.Where(x => x.Id == AccId).FirstOrDefault().AccName;
            }
            ViewBag.Airline = AccName;


            if (YearId > 0)
            {

                Sand = Sand.OrderBy(x => x.Id).Where(x => x.YearId == YearId);
                Year = db.YearWorkOns.Find(YearId).Year.ToString();
            }
            ViewBag.Year = Year;
            if (MonthId > 0)
            {

                Sand = Sand.OrderBy(x => x.Id).Where(x => x.MonthId == MonthId);
                Mouth  = db.MouthWorkOns.Find(MonthId).Mouth.ToString();
            }
            ViewBag.Mouth = Mouth;


            if (DateFrom != null && DateTo != null)
            {
                ///SearchDateFrom = CommonUsed.CheckDate(DateFrom.ToString());
                ///SearchDateTo = CommonUsed.CheckDate(DateTo.ToString());
                Sand = Sand.Where(t => DbFunctions.TruncateTime(t.SandMain.SandDate) >= DbFunctions.TruncateTime(DateFrom)
                && DbFunctions.TruncateTime(t.SandMain.SandDate) <= DbFunctions.TruncateTime(DateTo));

                DateToR = Convert.ToDateTime(DateTo).ToString("dd/MM/yyyy");
                DateFromR = Convert.ToDateTime(DateFrom).ToString("dd/MM/yyyy");
            }


            ViewBag.DateToR = DateToR;
            ViewBag.DateFromR = DateFromR;

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            ///***********************************************************************************
            var OperationList = new List<PrintSandViewModel>();
            int N = 0;
            foreach (var item in Sand)
            {
                PrintSandViewModel L = new PrintSandViewModel();
                L.Id = item.Id;
                N = N + 1;
                L.N = N;

              

                L.AccountName = item?.AccountTree.AccName;
                L.Credit = Convert.ToDecimal(item?.Amount);

                L.CurrencyName = item?.Currency.CurrencyName;
                L.CurrencyId = Convert.ToInt32(item?.CurrencyId);

                L.Statement = item?.Statement;
              
                L.Date = Convert.ToDateTime(item.SandMain.SandDate).ToString("dd/MM/yyyy");
                OperationList.Add(L);
            }

            if (OperationList.Count <= 0)
            {
                ErrorViewModel Error = new ErrorViewModel();
                Error.MassegeHeader = "عفوا";
                Error.MassegeContent = "لا يوجد اي بيانات لعرضها ";

                return RedirectToAction("ErrorMassege", Error);
            }

            SumCreditDoller = OperationList.Where(x => x.CurrencyId == 1).Sum(x => x.Credit);
            ViewBag.SumCreditDoller = SumCreditDoller;

            SumCreditSDN = OperationList.Where(x => x.CurrencyId == 2).Sum(x => x.Credit);
            ViewBag.SumCreditSDN = SumCreditSDN;

            return View(OperationList);


        }

        // ***************************************************************************************************
        public ActionResult SandSarf()
        {
            return View();
        }

        public ActionResult GetDataSandSarf(int? page,
        int? Get, int? Search, DateTime? DateFrom, DateTime? DateTo, int? AccId, int? YearId, int? MonthId)

        {

           
            IQueryable<SandDetail> Sand;
            Sand = db.SandDetails.Where(x => x.SandMain.OperationTypeId == 1).OrderBy(x => x.Id);
            ViewBag.AccId = AccId;

            ViewBag.DateFrom = DateFrom;
            ViewBag.DateTo = DateTo;


            ViewBag.YearId = YearId;
            ViewBag.MonthId = MonthId;

            if (Get > 0)
            {
                if (AccId > 0)
                {

                    Sand = Sand.OrderBy(x => x.Id).Where(x => x.AccId == AccId);
                }


                if (YearId > 0)
                {

                    Sand = Sand.OrderBy(x => x.Id).Where(x => x.YearId == YearId);
                }

                if (MonthId > 0)
                {

                    Sand = Sand.OrderBy(x => x.Id).Where(x => x.MonthId == MonthId);
                }


                if (DateFrom != null && DateTo != null)
                {
                    ///SearchDateFrom = CommonUsed.CheckDate(DateFrom.ToString());
                    ///SearchDateTo = CommonUsed.CheckDate(DateTo.ToString());
                    Sand = Sand.Where(t => DbFunctions.TruncateTime(t.SandMain.SandDate) >= DbFunctions.TruncateTime(DateFrom)
                    && DbFunctions.TruncateTime(t.SandMain.SandDate) <= DbFunctions.TruncateTime(DateTo));
                }

                //////////////////////////////////////////////////////
                if (Search == 1)
                {

                    if (Sand != null)
                    {

                        int ItemCount = 0;
                        ItemCount = Sand.Count();
                        ViewBag.ItemCount = ItemCount;

                    }
                }
                else
                {


                    ViewBag.ItemCount = 0;
                }

                //////////////////////////////////////////////////////////////////////

            }
            else
            {
                Sand = null;
                ViewBag.ItemCount = 0;
            }

            int pageSize = 5;
            int PageNumber = (page ?? 1);


            return PartialView("_GetDataSandSarf", Sand.ToPagedList(PageNumber, pageSize));
        }

        public ActionResult PrintSandSarf(DateTime? DateFrom, DateTime? DateTo, int? AccId, int? YearId, int? MonthId)
        {

            IQueryable<SandDetail> Sand = db.SandDetails.Where(x => x.SandMain.OperationTypeId == 1);
            string DateToR = "غير محدد";
            string DateFromR = "غير محدد";
            string AccName = "كل الحسابات";


            string Year = "";
            string Mouth = "";

            decimal SumCreditDoller;
            decimal SumCreditSDN;

            if (AccId > 0)
            {

                Sand = Sand.OrderBy(x => x.Id).Where(x => x.AccId == AccId);
                AccName = db.AccountTrees.Where(x => x.Id == AccId).FirstOrDefault().AccName;
            }
            ViewBag.Airline = AccName;


            if (YearId > 0)
            {

                Sand = Sand.OrderBy(x => x.Id).Where(x => x.YearId == YearId);
                Year = db.YearWorkOns.Find(YearId).Year.ToString();
            }
            ViewBag.Year = Year;
            if (MonthId > 0)
            {

                Sand = Sand.OrderBy(x => x.Id).Where(x => x.MonthId == MonthId);
                Mouth = db.MouthWorkOns.Find(MonthId).Mouth.ToString();
            }
            ViewBag.Mouth = Mouth;


            if (DateFrom != null && DateTo != null)
            {
                ///SearchDateFrom = CommonUsed.CheckDate(DateFrom.ToString());
                ///SearchDateTo = CommonUsed.CheckDate(DateTo.ToString());
                Sand = Sand.Where(t => DbFunctions.TruncateTime(t.SandMain.SandDate) >= DbFunctions.TruncateTime(DateFrom)
                && DbFunctions.TruncateTime(t.SandMain.SandDate) <= DbFunctions.TruncateTime(DateTo));

                DateToR = Convert.ToDateTime(DateTo).ToString("dd/MM/yyyy");
                DateFromR = Convert.ToDateTime(DateFrom).ToString("dd/MM/yyyy");
            }


            ViewBag.DateToR = DateToR;
            ViewBag.DateFromR = DateFromR;

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            ///***********************************************************************************
            var OperationList = new List<PrintSandViewModel>();
            int N = 0;
            foreach (var item in Sand)
            {
                PrintSandViewModel L = new PrintSandViewModel();
                L.Id = item.Id;
                N = N + 1;
                L.N = N;



                L.AccountName = item?.AccountTree.AccName;
                L.Credit = Convert.ToDecimal(item?.Amount);

                L.CurrencyName = item?.Currency.CurrencyName;
                L.CurrencyId = Convert.ToInt32(item?.CurrencyId);

                L.Statement = item?.Statement;

                L.Date = Convert.ToDateTime(item.SandMain.SandDate).ToString("dd/MM/yyyy");
                OperationList.Add(L);
            }

            if (OperationList.Count <= 0)
            {
                ErrorViewModel Error = new ErrorViewModel();
                Error.MassegeHeader = "عفوا";
                Error.MassegeContent = "لا يوجد اي بيانات لعرضها ";

                return RedirectToAction("ErrorMassege", Error);
            }

            SumCreditDoller = OperationList.Where(x => x.CurrencyId == 1).Sum(x => x.Credit);
            ViewBag.SumCreditDoller = SumCreditDoller;

            SumCreditSDN = OperationList.Where(x => x.CurrencyId == 2).Sum(x => x.Credit);
            ViewBag.SumCreditSDN = SumCreditSDN;

            return View(OperationList);


        }


        //*******************************************************************************
        public ActionResult PrintDataGrafDollar()
        {

            return View();
        }


        //هنا مفترض نعتمد علي الفاتورة القديمة 
        // عملنا جلب للبيانات ورسلناها لطابعة الفاتورة القديمة لانه مفصلة اكتر
        // وعملنا كده برضو عشان نمنع تكرار الشغل يعني عملنتا طباعة فاتورة ما نعمل تاني طباعة فاتورة-- نناد\ي الدالة القديمة بس
        public ActionResult PrintDataGraf(int? id)
        {
            FinancialClaim Claim = db.FinancialClaims.Find(id);
            AirLine AirLine = db.AirLines.Where(x => x.AccId == Claim.AccIdDebit).FirstOrDefault();

            int AirLineId = AirLine.Id;
            DateTime DateFrom = Convert.ToDateTime(Claim.DateFrom);
            DateTime DateTo = Convert.ToDateTime(Claim.DateTo);
            decimal ExchangeRate = Convert.ToDecimal(Claim.ExchangeRateAgainstPrimaryCurrency);

            return RedirectToAction("PrintDataGraf", "AdvencePrintBasic", new { AirLineId = AirLineId, DateFrom = DateFrom, DateTo = DateTo, ExcRate = ExchangeRate });





            //  FinancialClaim Claim = db.FinancialClaims.Find(id);
            //  AirLine AirLine = db.AirLines.Where(x => x.AccId == Claim.AccIdDebit).FirstOrDefault();


            //  int AirLineId = AirLine.Id;
            //  DateTime DateFrom = Convert.ToDateTime(Claim.DateFrom);
            //  DateTime DateTo = Convert.ToDateTime(Claim.DateTo);


            //  var userid = User.Identity.GetUserId();
            //  RoleController role = new RoleController();




            //  int GetCompanyId = role.GetCompanyId(userid);
            //  var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);

            //  string RevenuesManager = CompanyInfo.RevenuesManager;
            //  ViewBag.RevenuesManager = RevenuesManager;

            //  string DateNow = System.DateTime.Now.ToString("yyyy/MM/dd") + "م";
            //  ViewBag.DateNow = DateNow;

            //  string Serial = Guid.NewGuid().ToString();

            //  string UserName = CommonUsed.GetUserName(userid);

            //  if (DateTo != null)
            //  {
            //      Month = Convert.ToDateTime(DateTo).Month;
            //      Year = Convert.ToDateTime(DateTo).Year;
            //  }
            //  else if (DateFrom != null)
            //  {
            //      Month = Convert.ToDateTime(DateFrom).Month;
            //      Year = Convert.ToDateTime(DateFrom).Year;
            //  }
            //  else
            //  {
            //      //Month = Convert.ToDateTime(currentDate).Month;
            //      //Year = Convert.ToDateTime(currentDate).Year;
            //      return RedirectToAction("Error");
            //  }
            //  ExchangeRate = CommonUsed.getExchangeRateByLandingDate(Month, Year);
            //  string DemandMouth = db.MouthWorkOns.Where(x => x.Id == Month).FirstOrDefault().Mouth;
            //  //  string DemandYear = db.WorkOns.Where(x => x.UserId == id).FirstOrDefault().YearWorkOn.Year.ToString();




            //  string DemandDate = "مطالبة شهر " + DemandMouth + " " + Year + "م";
            //  ViewBag.DemandDate = DemandDate;

            //  string ArName = "غير محدد";
            //  string Name = "غير محدد";

            //  string SearchDateFrom = "غير محدد";
            //  string SearchDateTo = "غير محدد";

            //  // ReportDocument rd = new ReportDocument();


            //  IQueryable<Revenue> Revenue;
            //  Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date);


            //  if (AirLineId > 0)
            //  {
            //      Revenue = Revenue.Where(x => x.OfficerData.TowerData.AirLineId == AirLineId);
            //      ArName = db.AirLines.Where(x => x.Id == AirLineId).Single().ArName;

            //      Name = db.AirLines.Where(x => x.Id == AirLineId).Single().Name;
            //  }
            //  ViewBag.ArName = ArName;
            //  if (DateFrom != null && DateTo != null)
            //  {
            //      Revenue = Revenue.Where(t => DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) >= DbFunctions.TruncateTime(DateFrom) && DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) <= DbFunctions.TruncateTime(DateTo));
            //      SearchDateFrom = CommonUsed.CheckDate(DateFrom.ToString()).ToString();
            //      SearchDateTo = CommonUsed.CheckDate(DateTo.ToString()).ToString();
            //  }
            //  //if (Revenue.Count() <= 0)
            //  //{
            //  //    return RedirectToAction("Error");
            //  //}
            //  ///***********************************************************************************
            ////  var RevenueList = new List<PrintFinalInvoiceViewModel>();

            //  PrintFinalInvoiceViewModel F = new PrintFinalInvoiceViewModel();

            //  F.Name = db.AirLines.Where(x => x.Id == AirLineId).Single().Code + "/ " + db.AirLines.Where(x => x.Id == AirLineId).Single().Name;
            //  F.LandingFees = Convert.ToDouble(Revenue.Sum(x => x.LandingFees));
            //  F.ParkingFees = Convert.ToDouble(Revenue.Sum(x => x.ParkingFees));
            //  F.NightOperationFees = Convert.ToDouble(Revenue.Sum(x => x.NightOperationFees));
            //  F.NavigationFees = Convert.ToDouble(Revenue.Sum(x => x.NavigationFees));

            //  F.SecurityFees = Convert.ToDouble(Revenue.Sum(x => x.SecurityFees));
            //  F.FirstClassFees = Convert.ToDouble(Revenue.Sum(x => x.FirstClassFees));

            //  F.InterCUPPSFees = Convert.ToDouble(Revenue.Sum(x => x.InterCUPPSFees));
            //  F.DomeCUPPSFees = Convert.ToDouble(Revenue.Sum(x => x.DomeCUPPSFees));

            //  F.InterNYFees = Convert.ToDouble(Revenue.Sum(x => x.InterNYFees));
            //  F.DomeNYFees = Convert.ToDouble(Revenue.Sum(x => x.DomeNYFees) / ExchangeRate);

            //  F.PushBackFees = Convert.ToDouble(Revenue.Sum(x => x.PushBackFees));
            //  F.ServiceCars = Convert.ToDouble(Revenue.Sum(x => x.AmbolanceFees))
            //      + Convert.ToDouble(Revenue.Sum(x => x.FireFightingFees));

            //  F.NoiseFees = Convert.ToDouble(Revenue.Sum(x => x.NoiseFees));
            //  F.ExpMailFees = Convert.ToDouble(Revenue.Sum(x => x.ExpMailFees));
            //  F.ImpMailFees = Convert.ToDouble(Revenue.Sum(x => x.ImpMailFees));
            //  F.FreightFees = Convert.ToDouble(Revenue.Sum(x => x.FreightFees));

            //  F.SubTotalDollar = F.LandingFees + F.ParkingFees + F.NightOperationFees + F.NavigationFees + F.SecurityFees +
            //      F.FirstClassFees + F.FreightFees + F.ServiceCars + F.InterCUPPSFees + F.DomeCUPPSFees + F.InterNYFees +
            //      F.DomeNYFees + F.PushBackFees + F.ExpMailFees + F.ImpMailFees // + F.OpenAireportOvertimeFees
            //  + F.NoiseFees;


            //  F.TaxDollar = ((F.LandingFees + F.ParkingFees + F.NightOperationFees + F.NavigationFees + F.SecurityFees +
            //  F.FirstClassFees + F.FreightFees + F.ServiceCars + F.InterCUPPSFees + F.DomeCUPPSFees +
            //  F.PushBackFees + F.ExpMailFees + F.ImpMailFees // + F.OpenAireportOvertimeFees
            //  + F.NoiseFees) * 0.05);





            //  F.GrandTotalDollar = (F.SubTotalDollar + F.TaxDollar);
            //  F.GrandTotalSDG = (F.GrandTotalDollar) * Convert.ToDouble(ExchangeRate);




            //  ///-------------------------------------SDG------------------------------------------------------------
            //  double SDGRound = Convert.ToDouble(Math.Round(F.GrandTotalSDG, 2));

            //  ToWord toWord = new ToWord(Convert.ToDecimal(SDGRound), currencies[Convert.ToInt32(0)]);

            //  F.GrandTotalSDGTxt = toWord.ConvertToArabic().ToString();

            //  ///--------------------------------------Dollar-----------------------------------------------------------

            //  double DollarRound = Convert.ToDouble(Math.Round(F.GrandTotalDollar, 2));
            //  ToWord toWord1 = new ToWord(Convert.ToDecimal(DollarRound), currencies[Convert.ToInt32(1)]);


            //  F.GrandTotalDollarTxt = toWord1.ConvertToArabic().ToString();


            //  ////*************************************************************************************
            //  ViewBag.HeaderDate = Convert.ToDateTime(DateFrom).ToString("MMM-yyyy");

            //  int CurrencyType = Convert.ToInt32(db.AirLines.Where(x => x.Id == AirLineId).Single().CurrencyType);

            //  if (CurrencyType == 1)
            //  {
            //      ViewBag.Header = "Graf-" + Name + "-" + DateTime.Now.ToShortDateString();
            //      return View(F);
            //  }
            //  else
            //  {
            //      ViewBag.Header = "Graf-" + Name + "-" + DateTime.Now.ToShortDateString();
            //      return View("PrintDataGrafDollar",F);
            //  }
        }

        //هنا مفترض نعتمد علي الفاتورة القديمة 
        // عملنا جلب للبيانات ورسلناها لطابعة الفاتورة القديمة لانه مفصلة اكتر
        // وعملنا كده برضو عشان نمنع تكرار الشغل يعني عملنتا طباعة فاتورة ما نعمل تاني طباعة فاتورة-- نناد\ي الدالة القديمة بس
        public ActionResult PrintDataFinalInvoice(int? id)
        {
            FinancialClaim Claim = db.FinancialClaims.Find(id);
            AirLine AirLine = db.AirLines.Where(x => x.AccId == Claim.AccIdDebit).FirstOrDefault();
            
            int AirLineId = AirLine.Id;
            DateTime DateFrom = Convert.ToDateTime(Claim.DateFrom);
            DateTime DateTo = Convert.ToDateTime(Claim.DateTo);
            decimal ExchangeRate = Convert.ToDecimal(Claim.ExchangeRateAgainstPrimaryCurrency);

            return RedirectToAction("PrintDataFinalInvoice", "AdvencePrintBasic", new { AirLineId = AirLineId, DateFrom  = DateFrom , DateTo = DateTo, ExcRate = ExchangeRate });
        }
        ///===================================================Start Claim===============================================================================================
        
        public ActionResult Claim()
        {
            return View();
        }
        
        public ActionResult GetDataClaim(int? page,
      int? Get, int? Search, DateTime? DateFrom, DateTime? DateTo, int? AccId,int? MouthId,int? YearId)
        {
            IQueryable<FinancialClaim> Claim;
            Claim = db.FinancialClaims.OrderBy(x => x.Id);
            ViewBag.AccId = AccId;

            ViewBag.DateFrom = DateFrom;
            ViewBag.DateTo = DateTo;

            ViewBag.MouthId = MouthId;
            ViewBag.YearId = YearId;

            int Mouth = Convert.ToInt32(MouthId);
            int Year;
            if (YearId > 0) {
               Year = Convert.ToInt32(db.YearWorkOns.Find(YearId).Year);
                } else { Year = 0; }
            
            if (Get > 0)
            {
                if (AccId > 0)
                {

                    Claim = Claim.OrderBy(x => x.Id).Where(x => x.AccIdDebit == AccId);
                }


                if (DateFrom != null && DateTo != null)
                {
                    ///SearchDateFrom = CommonUsed.CheckDate(DateFrom.ToString());
                    ///SearchDateTo = CommonUsed.CheckDate(DateTo.ToString());
                    Claim = Claim.Where(t => DbFunctions.TruncateTime(t.Date) >= DbFunctions.TruncateTime(DateFrom)
                    && DbFunctions.TruncateTime(t.Date) <= DbFunctions.TruncateTime(DateTo));
                }

                if (MouthId > 0)
                {
                    Claim = Claim.OrderBy(x => x.Id).Where(x => x.Month == Mouth);
                }
                if (YearId > 0)
                {
                    Claim = Claim.OrderBy(x => x.Id).Where(x => x.Year == Year);
                }
                //////////////////////////////////////////////////////
                if (Search == 1)
                {

                    if (Claim != null)
                    {

                        int ItemCount = 0;
                        ItemCount = Claim.Count();
                        ViewBag.ItemCount = ItemCount;

                    }
                }
                else
                {


                    ViewBag.ItemCount = 0;
                }

                //////////////////////////////////////////////////////////////////////

            }
            else
            {
                Claim = null;
                ViewBag.ItemCount = 0;
            }

            int pageSize = 5;
            int PageNumber = (page ?? 1);


            return PartialView("_GetDataClaim", Claim.ToPagedList(PageNumber, pageSize));
        }

        //---------------------------------------------------------------------------------------------------------------------------------------------------
        
        public ActionResult PrintClaim(DateTime? DateFrom, DateTime? DateTo, int? AccId, int? MouthId, int? YearId)
        {

            IQueryable<FinancialClaim> Claim = db.FinancialClaims;
            string DateToR = "غير محدد";
            string DateFromR = "غير محدد"; 
            string AccName = "كل الحسابات"; 
            string Mouth ="";
            string Year = "";
            int YearNum = 0;
            decimal SumDebitDoller;
            decimal SumDebitSDN;
            if (YearId > 0)
            {
                Year = db.YearWorkOns.Find(YearId).Year.ToString();
                YearNum = Convert.ToInt32(Year);
            }
            if (MouthId > 0)
            {
                Mouth = db.MouthWorkOns.Find(MouthId).Mouth.ToString();
               /// YearNum = Convert.ToInt32(Year);
            }




            if (AccId > 0)
            {

                Claim = Claim.OrderBy(x => x.Id).Where(x => x.AccIdDebit == AccId);
                AccName = db.AccountTrees.Where(x => x.Id == AccId).FirstOrDefault().AccName;             
            }
            ViewBag.Airline = AccName;

            if (DateFrom != null && DateTo != null)
            {
                ///SearchDateFrom = CommonUsed.CheckDate(DateFrom.ToString());
                ///SearchDateTo = CommonUsed.CheckDate(DateTo.ToString());
                Claim = Claim.Where(t => DbFunctions.TruncateTime(t.Date) >= DbFunctions.TruncateTime(DateFrom)
                && DbFunctions.TruncateTime(t.Date) <= DbFunctions.TruncateTime(DateTo));

                DateToR = Convert.ToDateTime(DateTo).ToString("dd/MM/yyyy");
                DateFromR = Convert.ToDateTime(DateFrom).ToString("dd/MM/yyyy");

              
            }
            ViewBag.DateToR = DateToR;
            ViewBag.DateFromR = DateFromR;

            if (MouthId > 0)
            {
                Claim = Claim.OrderBy(x => x.Id).Where(x => x.Month == MouthId);
               
            }
            ViewBag.Mouth = Mouth;
            if (YearId > 0)
            {
                Claim = Claim.OrderBy(x => x.Id).Where(x => x.Year == YearNum);

            }
            ViewBag.Year = Year;
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (Claim.ToList().Count <= 0)
            {
                ErrorViewModel Error = new ErrorViewModel();
                Error.MassegeHeader = "عفوا";
                Error.MassegeContent = "لا يوجد اي بيانات لعرضها ";

                return RedirectToAction("ErrorMassege", Error);
            }


            ///***********************************************************************************
            var OperationList = new List<PrintClaimViewModel>();
            int N = 0;
            foreach (var item in Claim)
            {
                PrintClaimViewModel L = new PrintClaimViewModel();
                L.Id = item.Id;
                N = N + 1;
                L.N = N;



                L.AccountName = item?.AccountTree.AccName;
                L.Debit = Convert.ToDecimal(item?.Amount);

                L.CurrencyName = item?.Currency.CurrencyName;
                L.CurrencyId = Convert.ToInt32(item.CurrencyId);

                L.Statement = item?.Statement;

                L.Date = Convert.ToDateTime(item.Date).ToString("dd/MM/yyyy");
                L.MouthName = CommonUsed.GetMouthbyId(Convert.ToInt32(item.Month));
                L.Year = Convert.ToInt32(item.Year);
                OperationList.Add(L);
            }

            SumDebitDoller = OperationList.Where(x=>x.CurrencyId==1).Sum(x => x.Debit);
            ViewBag.SumDebitDoller = SumDebitDoller;

            SumDebitSDN = OperationList.Where(x => x.CurrencyId == 2).Sum(x => x.Debit);
            ViewBag.SumDebitSDN = SumDebitSDN;
            return View(OperationList);


        }
        
        //========================================================End Claim=====================================================================================


        //*****************************************************strart Open***********************************************************

        public ActionResult Open()
        {
            return View();
        }
        
        public ActionResult GetDataOpen(int? page,
      int? Get, int? Search, DateTime? DateFrom, DateTime? DateTo, int? AccId)
        {
            IQueryable<OpenAccBalance> Open;
            Open = db.OpenAccBalances.OrderBy(x => x.Id);
            ViewBag.AccId = AccId;

            ViewBag.DateFrom = DateFrom;
            ViewBag.DateTo = DateTo;

          

            

            if (Get > 0)
            {
                if (AccId > 0)
                {

                    Open = Open.OrderBy(x => x.Id).Where(x => x.AccId == AccId);
                }


                if (DateFrom != null && DateTo != null)
                {
                    ///SearchDateFrom = CommonUsed.CheckDate(DateFrom.ToString());
                    ///SearchDateTo = CommonUsed.CheckDate(DateTo.ToString());
                    Open = Open.Where(t => DbFunctions.TruncateTime(t.Date) >= DbFunctions.TruncateTime(DateFrom)
                    && DbFunctions.TruncateTime(t.Date) <= DbFunctions.TruncateTime(DateTo));
                }

              
                //////////////////////////////////////////////////////
                if (Search == 1)
                {

                    if (Open != null)
                    {

                        int ItemCount = 0;
                        ItemCount = Open.Count();
                        ViewBag.ItemCount = ItemCount;

                    }
                }
                else
                {


                    ViewBag.ItemCount = 0;
                }

                //////////////////////////////////////////////////////////////////////

            }
            else
            {
                Open = null;
                ViewBag.ItemCount = 0;
            }

            int pageSize = 5;
            int PageNumber = (page ?? 1);


            return PartialView("_GetDataOpen", Open.ToPagedList(PageNumber, pageSize));
        }

        //---------------------------------------------------------------------------------------------------------------------------------------------------


        public ActionResult PrintOpen(DateTime? DateFrom, DateTime? DateTo, int? AccId)
        {

            IQueryable<OpenAccBalance> Open = db.OpenAccBalances;
            string DateToR = "غير محدد";
            string DateFromR = "غير محدد";
            string AccName = "كل الحسابات";
            
           
            decimal SumDebitDoller;
            decimal SumDebitSDN;

            decimal SumCreditDoller;
            decimal SumCreditSDN;





            if (AccId > 0)
            {

                Open = Open.OrderBy(x => x.Id).Where(x => x.AccId == AccId);
                AccName = db.AccountTrees.Where(x => x.Id == AccId).FirstOrDefault().AccName;
            }
            ViewBag.Airline = AccName;

            if (DateFrom != null && DateTo != null)
            {
                ///SearchDateFrom = CommonUsed.CheckDate(DateFrom.ToString());
                ///SearchDateTo = CommonUsed.CheckDate(DateTo.ToString());
                Open = Open.Where(t => DbFunctions.TruncateTime(t.Date) >= DbFunctions.TruncateTime(DateFrom)
                && DbFunctions.TruncateTime(t.Date) <= DbFunctions.TruncateTime(DateTo));

                DateToR = Convert.ToDateTime(DateTo).ToString("dd/MM/yyyy");
                DateFromR = Convert.ToDateTime(DateFrom).ToString("dd/MM/yyyy");


            }
            ViewBag.DateToR = DateToR;
            ViewBag.DateFromR = DateFromR;

           
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            ///***********************************************************************************
            var OperationList = new List<PrintOpenViewModel>();
            int N = 0;
            foreach (var item in Open)
            {
                PrintOpenViewModel L = new PrintOpenViewModel();
                L.Id = item.Id;
                N = N + 1;
                L.N = N;

                L.AccountName = item?.AccountTree.AccName;
                L.Debit = Convert.ToDecimal(item?.Debit);
                L.Credit = Convert.ToDecimal(item?.Credit);
                L.CurrencyId = Convert.ToInt32(item.CurrencyID);
                
                L.Date = Convert.ToDateTime(item.Date).ToString("dd/MM/yyyy");
              
                OperationList.Add(L);
            }

            if (OperationList.Count <= 0)
            {
                ErrorViewModel Error = new ErrorViewModel();
                Error.MassegeHeader = "عفوا";
                Error.MassegeContent = "لا يوجد اي بيانات لعرضها ";

                return RedirectToAction("ErrorMassege", Error);
            }


            SumDebitDoller = OperationList.Where(x => x.CurrencyId == 1).Sum(x => x.Debit);
            ViewBag.SumDebitDoller = SumDebitDoller;

            SumDebitSDN = OperationList.Where(x => x.CurrencyId == 2).Sum(x => x.Debit);
            ViewBag.SumDebitSDN = SumDebitSDN;

            SumCreditDoller = OperationList.Where(x => x.CurrencyId == 1).Sum(x => x.Credit);
            ViewBag.SumCreditDoller = SumCreditDoller;

            SumCreditSDN = OperationList.Where(x => x.CurrencyId == 2).Sum(x => x.Credit);
            ViewBag.SumCreditSDN = SumCreditSDN;



            return View(OperationList);


        }








        ////**********************************************End Open**************************************************

















        ///  account statement

        public ActionResult Statement()
        {
            return View();
        }

      

        public ActionResult GetDataStatement(int? page,
     int? Get, int? Search, DateTime? DateFrom, DateTime? DateTo, int? AccId)
        {
            IQueryable<AccountOperation> Operation;
            Operation = db.AccountOperations.OrderBy(x => x.Id);
            ViewBag.AccId = AccId;



            DateFrom = ViewBag.DateFrom;
            DateTo = ViewBag.DateTo;

            if (Get > 0)
            {
                if (AccId > 0)
                {

                    Operation = Operation.OrderBy(x => x.Id).Where(x => x.AccountNumber == AccId);
                }


                if (DateFrom != null && DateTo != null)
                {
                    ///SearchDateFrom = CommonUsed.CheckDate(DateFrom.ToString());
                    ///SearchDateTo = CommonUsed.CheckDate(DateTo.ToString());
                    Operation = Operation.Where(t => DbFunctions.TruncateTime(t.OperationDate) >= DbFunctions.TruncateTime(DateFrom)
                    && DbFunctions.TruncateTime(t.OperationDate) <= DbFunctions.TruncateTime(DateTo));
                }

                //////////////////////////////////////////////////////
                if (Search == 1)
                {

                    if (Operation != null)
                    {

                        int ItemCount = 0;
                        ItemCount = Operation.Count();
                        ViewBag.ItemCount = ItemCount;

                    }
                }
                else
                {


                    ViewBag.ItemCount = 0;
                }

                //////////////////////////////////////////////////////////////////////

            }
            else
            {
                Operation = null;
                ViewBag.ItemCount = 0;
            }

            int pageSize = 20;
            int PageNumber = (page ?? 1);


            return PartialView("_GetDataStatement", Operation.ToPagedList(PageNumber, pageSize));
        }

        //---------------------------------------------------------------------------------------------------------------------------------------------------


        public ActionResult PrintStatement(DateTime? DateFrom, DateTime? DateTo, int? AccId)
        {
            IQueryable<AccountOperation> Operation = db.AccountOperations;
            string AccName = "كل الحسابات";

            if (AccId > 0)
            {
                Operation = Operation.OrderBy(x => x.Id).Where(x => x.AccountNumber == AccId);
                AccName = db.AccountTrees.Find(AccId).AccName;  
            }
            ViewBag.AccName = AccName;

            if (DateFrom != null && DateTo != null)
            {
                ///SearchDateFrom = CommonUsed.CheckDate(DateFrom.ToString());
                ///SearchDateTo = CommonUsed.CheckDate(DateTo.ToString());
                //Operation = Operation.Where(t => DbFunctions.TruncateTime(t.OperationDate) >= DbFunctions.TruncateTime(DateFrom)
                //&& DbFunctions.TruncateTime(t.OperationDate) <= DbFunctions.TruncateTime(DateTo));

                //Operation = Operation.Where(t => DbFunctions.TruncateTime(t.OperationDate).Value.Month == DbFunctions.TruncateTime(DateFrom).Value.Month
                //&& DbFunctions.TruncateTime(t.OperationDate).Value.Month == DbFunctions.TruncateTime(DateTo).Value.Month);

                Operation = Operation.Where(t => DbFunctions.TruncateTime(t.OperationDate) >= DbFunctions.TruncateTime(DateFrom)
                && DbFunctions.TruncateTime(t.OperationDate) <= DbFunctions.TruncateTime(DateTo));

                string DateFromR = Convert.ToDateTime(DateFrom).ToString("yyyy/MM/dd", new CultureInfo("ar-AE"));
                ViewBag.DateFromR = DateFromR;


           

                string DateToR = Convert.ToDateTime(DateTo).ToString("yyyy/MM/dd", new CultureInfo("ar-AE"));
                ViewBag.DateToR = DateToR;
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            ///***********************************************************************************



            var OperationList = new List<PrintOperationViewModel>();
            int N = 0;
            foreach (var item in Operation)
            {
                PrintOperationViewModel L = new PrintOperationViewModel();
                L.Id = item.Id;
                N = N + 1;
                L.N = N;



                L.AccountName = item?.AccountTree.AccName;
                L.Debit = Convert.ToDecimal(item?.OperationDebit);
                L.Credit = Convert.ToDecimal(item?.OperationCredit);
                L.CurrencyName = item?.Currency.CurrencyName;
                L.CurrencyId = Convert.ToInt32(item?.CurrencyId);

                L.Statement = item?.OperationStatement;

                L.Date = Convert.ToDateTime(item.OperationDate).ToString("dd/MM/yyyy");
                OperationList.Add(L);
            }

            if (OperationList.Count <= 0)
            {
                ErrorViewModel Error = new ErrorViewModel();
                Error.MassegeHeader = "عفوا";
                Error.MassegeContent = "لا يوجد اي بيانات لعرضها ";

                return RedirectToAction("ErrorMassege", Error);
            }


            decimal TotalDebitSDG = OperationList.Where(x => x.CurrencyId == 2).Sum(x => x.Debit);
            ViewBag.TotalDebitSDG = TotalDebitSDG;

            decimal TotalCreditSDG = OperationList.Where(x => x.CurrencyId == 2).Sum(x => x.Credit);
            ViewBag.TotalCreditSDG = TotalCreditSDG;

            decimal TotalDebitDollar = OperationList.Where(x => x.CurrencyId == 1).Sum(x => x.Debit);
            ViewBag.TotalDebitDollar = TotalDebitDollar;

            decimal TotalCreditDollar = OperationList.Where(x => x.CurrencyId == 1).Sum(x => x.Credit);
            ViewBag.TotalCreditDollar = TotalCreditDollar;

            decimal DiffSDG = TotalDebitSDG - TotalCreditSDG;
            ViewBag.DiffSDG = DiffSDG;

            decimal DiffDollar = TotalDebitDollar - TotalCreditDollar;
            ViewBag.DiffDollar = DiffDollar;

            return View(OperationList);


        }




        public ActionResult ErrorMassege(ErrorViewModel error)
        {
            return View(error);
        }

















    }
}