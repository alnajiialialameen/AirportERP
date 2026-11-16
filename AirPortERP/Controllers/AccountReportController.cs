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
                          Id = x.AccId,
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
                          Id = x.AccId,
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

        public ActionResult PrintClaimConstraintDaily(int? id)
        {
            FinancialClaim1 financialClaim = db.FinancialClaim1.Where(x => x.TowerDataId == id).FirstOrDefault();
            IQueryable<AccountOperation> AccountOperation = db.AccountOperations.Where(x => x.OperationID == financialClaim.Id && x.OperationTypeId == 3);

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

            FinancialClaim1 financialClaim1 = db.FinancialClaim1.Find(id);
            int? operationtypeId = financialClaim1.OperationTypeId;

            IQueryable<AccountOperation> AccountOperation = db.AccountOperations.Where(x => x.OperationID == id && x.OperationTypeId == operationtypeId);

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

            var id = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(id);
            IQueryable<SandDetails1> Sand;
            Sand = db.SandDetails1.Where(x => x.SandMain1.OperationTypeId == 2 && x.SandMain1.CompanyInfoId == companyId).OrderBy(x => x.Id);
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
                    Sand = Sand.Where(t => DbFunctions.TruncateTime(t.SandMain1.SandDate) >= DbFunctions.TruncateTime(DateFrom)
                    && DbFunctions.TruncateTime(t.SandMain1.SandDate) <= DbFunctions.TruncateTime(DateTo));
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
            var id = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(id);
            IQueryable<SandDetails1> Sand = db.SandDetails1.Where(x => x.SandMain1.OperationTypeId == 2 && x.SandMain1.CompanyInfoId == companyId);
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
                //Year = db.YearWorkOns.Find(YearId).Year.ToString();
                Year = YearId.ToString();
            }
            ViewBag.Year = Year;
            if (MonthId > 0)
            {

                Sand = Sand.OrderBy(x => x.Id).Where(x => x.MonthId == MonthId);
                //Mouth  = db.MouthWorkOns.Find(MonthId).Mouth.ToString();
                Year = MonthId.ToString();
            }
            ViewBag.Mouth = Mouth;


            if (DateFrom != null && DateTo != null)
            {
                ///SearchDateFrom = CommonUsed.CheckDate(DateFrom.ToString());
                ///SearchDateTo = CommonUsed.CheckDate(DateTo.ToString());
                Sand = Sand.Where(t => DbFunctions.TruncateTime(t.SandMain1.SandDate) >= DbFunctions.TruncateTime(DateFrom)
                && DbFunctions.TruncateTime(t.SandMain1.SandDate) <= DbFunctions.TruncateTime(DateTo));

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
              
                L.Date = Convert.ToDateTime(item.SandMain1.SandDate).ToString("dd/MM/yyyy");
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

           
            IQueryable<SandDetails1> Sand;
            Sand = db.SandDetails1.Where(x => x.SandMain1.OperationTypeId == 1).OrderBy(x => x.Id);
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
                    Sand = Sand.Where(t => DbFunctions.TruncateTime(t.SandMain1.SandDate) >= DbFunctions.TruncateTime(DateFrom)
                    && DbFunctions.TruncateTime(t.SandMain1.SandDate) <= DbFunctions.TruncateTime(DateTo));
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

            IQueryable<SandDetails1> Sand = db.SandDetails1.Where(x => x.SandMain1.OperationTypeId == 1);
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


            //if (YearId > 0)
            //{

            //    Sand = Sand.OrderBy(x => x.Id).Where(x => x.YearId == YearId);
            //    Year = db.YearWorkOns.Find(YearId).Year.ToString();
            //}
            //ViewBag.Year = Year;
            //if (MonthId > 0)
            //{

            //    Sand = Sand.OrderBy(x => x.Id).Where(x => x.MonthId == MonthId);
            //    Mouth = db.MouthWorkOns.Find(MonthId).Mouth.ToString();
            //}
            ViewBag.Mouth = Mouth;


            if (DateFrom != null && DateTo != null)
            {
                ///SearchDateFrom = CommonUsed.CheckDate(DateFrom.ToString());
                ///SearchDateTo = CommonUsed.CheckDate(DateTo.ToString());
                Sand = Sand.Where(t => DbFunctions.TruncateTime(t.SandMain1.SandDate) >= DbFunctions.TruncateTime(DateFrom)
                && DbFunctions.TruncateTime(t.SandMain1.SandDate) <= DbFunctions.TruncateTime(DateTo));

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

                L.Date = Convert.ToDateTime(item.SandMain1.SandDate).ToString("dd/MM/yyyy");
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

        
        //هنا مفترض نعتمد علي الفاتورة القديمة 
        // عملنا جلب للبيانات ورسلناها لطابعة الفاتورة القديمة لانه مفصلة اكتر
        // وعملنا كده برضو عشان نمنع تكرار الشغل يعني عملنتا طباعة فاتورة ما نعمل تاني طباعة فاتورة-- نناد\ي الدالة القديمة بس
        public ActionResult PrintDataNYGraf(int? id)
        {
            FinancialClaim1 Claim = db.FinancialClaim1.Find(id);
            AirLine AirLine = db.AirLines.Where(x => x.AccId == Claim.AccIdDebit).FirstOrDefault();

            int AirLineId = AirLine.Id;
            DateTime DateFrom = Convert.ToDateTime(Claim.DateFrom);
            DateTime DateTo = Convert.ToDateTime(Claim.DateTo);
            decimal ExchangeRate = Convert.ToDecimal(Claim.ExchangeRateAgainstPrimaryCurrency);

            return RedirectToAction("PrintDataNYGraf", "AdvencePrintBasic", new { AirLineId = AirLineId, DateFrom = DateFrom, DateTo = DateTo, ExcRate = ExchangeRate });
        }

        public ActionResult PrintDataGrafDollar()
        {

            return View();
        }

        //هنا مفترض نعتمد علي الفاتورة القديمة 
        // عملنا جلب للبيانات ورسلناها لطابعة الفاتورة القديمة لانه مفصلة اكتر
        // وعملنا كده برضو عشان نمنع تكرار الشغل يعني عملنتا طباعة فاتورة ما نعمل تاني طباعة فاتورة-- نناد\ي الدالة القديمة بس
        public ActionResult PrintDataGraf(int? id)
        {
            FinancialClaim1 Claim = db.FinancialClaim1.Find(id);
            AirLine AirLine = db.AirLines.Where(x => x.AccId == Claim.AccIdDebit).FirstOrDefault();

            int AirLineId = AirLine.Id;
            DateTime DateFrom = Convert.ToDateTime(Claim.DateFrom);
            DateTime DateTo = Convert.ToDateTime(Claim.DateTo);
            decimal ExchangeRate = Convert.ToDecimal(Claim.ExchangeRateAgainstPrimaryCurrency);

            return RedirectToAction("PrintDataGraf", "AdvencePrintBasic", new { AirLineId = AirLineId, DateFrom = DateFrom, DateTo = DateTo, ExcRate = ExchangeRate });


        }
            public ActionResult PrintFreightGraf(int? id)
        {
            FinancialClaim1 Claim = db.FinancialClaim1.Find(id);
            AirLine AirLine = db.AirLines.Where(x => x.AccId == Claim.AccIdDebit).FirstOrDefault();

            int AirLineId = AirLine.Id;
            DateTime DateFrom = Convert.ToDateTime(Claim.DateFrom);
            DateTime DateTo = Convert.ToDateTime(Claim.DateTo);
            decimal ExchangeRate = Convert.ToDecimal(Claim.ExchangeRateAgainstPrimaryCurrency);

            return RedirectToAction("PrintDataFreightGraf", "AdvencePrintBasic", new { AirLineId = AirLineId, DateFrom = DateFrom, DateTo = DateTo, ExcRate = ExchangeRate });



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

        public ActionResult PrintDataGrafSingle(int? id)
        {
            FinancialClaim1 Claim = db.FinancialClaim1.Where(x => x.TowerDataId == id).FirstOrDefault();
            AirLine AirLine = db.AirLines.Where(x => x.AccId == Claim.AccIdDebit).FirstOrDefault();

           
           
            decimal ExchangeRate = Convert.ToDecimal(Claim.ExchangeRateAgainstPrimaryCurrency);

            return RedirectToAction("PrintDataGrafSingle", "AdvencePrintBasic", new { TowerDataId = id, ExcRate = ExchangeRate });





          
        }

        //هنا مفترض نعتمد علي الفاتورة القديمة 
        // عملنا جلب للبيانات ورسلناها لطابعة الفاتورة القديمة لانه مفصلة اكتر
        // وعملنا كده برضو عشان نمنع تكرار الشغل يعني عملنتا طباعة فاتورة ما نعمل تاني طباعة فاتورة-- نناد\ي الدالة القديمة بس
        public ActionResult PrintDataFinalInvoice(int? id)
        {
            FinancialClaim1 Claim = db.FinancialClaim1.Find(id);
            AirLine AirLine = db.AirLines.Where(x => x.AccId == Claim.AccIdDebit).FirstOrDefault();
            
            int AirLineId = AirLine.Id;
            DateTime DateFrom = Convert.ToDateTime(Claim.DateFrom);
            DateTime DateTo = Convert.ToDateTime(Claim.DateTo);
            decimal ExchangeRate = Convert.ToDecimal(Claim.ExchangeRateAgainstPrimaryCurrency);

            return RedirectToAction("PrintDataFinalInvoice", "AdvencePrintBasic", new { AirLineId = AirLineId, DateFrom  = DateFrom , DateTo = DateTo, ExcRate = ExchangeRate });
        }

        public ActionResult PrintDataFinalInvoiceDaily(int? id)
        {

            FinancialClaim1 Claim = db.FinancialClaim1.Where(x => x.TowerDataId == id).FirstOrDefault();
            AirLine AirLine = db.AirLines.Where(x => x.AccId == Claim.AccIdDebit).FirstOrDefault();

            int AirLineId = AirLine.Id;
            DateTime DateFrom = Convert.ToDateTime(Claim.DateFrom);
            DateTime DateTo = Convert.ToDateTime(Claim.DateTo);
            decimal ExchangeRate = Convert.ToDecimal(Claim.ExchangeRateAgainstPrimaryCurrency);

            return RedirectToAction("PrintDataFinalInvoiceDaily", "AdvencePrintBasic", new { TowerDataId =id, ExcRate = ExchangeRate });
        }

        ///===================================================Start Claim===============================================================================================

        public ActionResult Claim()
        {
            return View();
        }
        
        public ActionResult GetDataClaim(int? page,
      int? Get, int? Search, DateTime? DateFrom, DateTime? DateTo, int? AccId,int? MouthId,int? YearId)
        {
            var id = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(id);
            IQueryable<FinancialClaim1> Claim;
            Claim = db.FinancialClaim1.OrderBy(x => x.Id).Where(x => x.CompanyInfoId == companyId);
            ViewBag.AccId = AccId;

            ViewBag.DateFrom = DateFrom;
            ViewBag.DateTo = DateTo;

            ViewBag.MouthId = MouthId;
            ViewBag.YearId = YearId;

            int Mouth = Convert.ToInt32(MouthId);
            int Year;
            if (YearId > 0) {
                /* Year = Convert.ToInt32(db.YearWorkOns.Find(YearId).Year)*/
                Year = Convert.ToInt32(YearId);
                ;
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
            var id = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(id);
            IQueryable<FinancialClaim1> Claim = db.FinancialClaim1.Where(x => x.CompanyInfoId == companyId);
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
                //Year = db.YearWorkOns.Find(YearId).Year.ToString();
                Year = YearId.ToString();
                YearNum = Convert.ToInt32(Year);
            }
            if (MouthId > 0)
            {
                //Mouth = db.MouthWorkOns.Find(MouthId).Mouth.ToString();
                Year = MouthId.ToString();

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
                L.MouthName = CommonUsed.GetMonthName(Convert.ToInt32(item.Month));
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

        public ActionResult Statement()
        {
            return View();
        }

        public ActionResult GetDataStatement(int? page,
     int? Get, int? Search, DateTime? DateFrom, DateTime? DateTo, int? AccId)
        {
            var id = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(id);
            IQueryable<AccountOperation> Operation;
            Operation = db.AccountOperations.OrderBy(x => x.Id).Where(x => x.CompanyInfoId == companyId);
            ViewBag.AccId = AccId;



            DateFrom = ViewBag.DateFrom;
            DateTo = ViewBag.DateTo;

            if (Get > 0)
            {
                if (AccId > 0)
                {

                    Operation = Operation.OrderBy(x => x.Id).Where(x => x.AccId == AccId);
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

        public ActionResult PrintStatement(DateTime? DateFrom, DateTime? DateTo, int? AccId)
        {
            var id = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(id);
            IQueryable<AccountOperation> Operation = db.AccountOperations.Where(x => x.CompanyInfoId == companyId);
            string AccName = "كل الحسابات";

            if (AccId > 0)
            {
                Operation = Operation.OrderBy(x => x.Id).Where(x => x.AccId == AccId);
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

                L.Date = CommonUsed.ToDisplayDate(item.OperationDate);
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

        //public ActionResult PrintDataLanding(int? towerDataId)
        //{
        //    //TowerData towerData = db.TowerDatas.Find(towerDataId);
        //    OfficerData officerData = db.OfficerDatas.Where(x => x.TowerDataId == towerDataId).FirstOrDefault();
        //    Revenue revenue = db.Revenues.Where(x => x.OfficerDataId == officerData.Id).FirstOrDefault();


        //    var id = User.Identity.GetUserId();
        //    RoleController role = new RoleController();
        //    //int GetCompanyId = role.GetCompanyId(id);
        //    //var CompanyInfo = db.CompanyInfoes.Single(x => x.Id == GetCompanyId);
        //    string TitleDiscount = "";
        //    string UserName = CommonUsed.GetUserName(id);
        //    bool IsLocalCompany = false;

        //    string Name = "غير محدد";
        //    string SearchDateFrom = "غير محدد";
        //    string SearchDateTo = "غير محدد";
        //    string MessageDiscount = "";
        //    string Discount = "";

        //    //   ReportDocument rd = new ReportDocument();

        //    IQueryable<Revenue> Revenue;
        //    Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date);


        //            Revenue = Revenue.Where(x => x.OfficerData.TowerData.FlightTypeL == 1);




        //    if (towerData.AirLineId> 0)
        //    {
        //        Revenue = Revenue.Where(x => x.OfficerData.TowerData.AirLineId == towerData.AirLineId);
        //        Name = db.AirLines.Where(x => x.Id == towerData.AirLineId).Single().Name;

        //        IsLocalCompany = Convert.ToBoolean(db.AirLines.Where(x => x.Id == towerData.AirLineId).Single().IsLocalCompany);

        //        if (IsLocalCompany)
        //        {
        //            GeneralService gs = db.GeneralServices.FirstOrDefault(x => x.Id == 5);
        //            if (gs.Value > 0)
        //            {
        //                int Type = Convert.ToInt32(gs.Type);
        //                decimal Value = 0;
        //                if (Type == 2)//means It's a value Not Percent
        //                {
        //                    Value = Convert.ToDecimal(gs.Value);
        //                    MessageDiscount = "*Discount   " + Convert.ToDouble(Value) + "$ from Security Fees";
        //                    Discount = "(- " + Convert.ToDouble(Value) + "$)";
        //                    TitleDiscount = "Discount-" + Convert.ToDouble(Value) + "$";

        //                }
        //                else //means It's a Percent Not value
        //                {
        //                    Value = Convert.ToDecimal(gs.Value);
        //                    MessageDiscount = " *Discount " + Convert.ToDouble(Value) + "% from Security Fees";
        //                    Discount = "(- " + Convert.ToDouble(Value) + "%)";
        //                    TitleDiscount = "Discount-" + Convert.ToDouble(Value) + "%";

        //                }
        //            }
        //        }
        //    }

        //    ViewBag.TitleDiscount = TitleDiscount;

        //    if (Revenue.Count() <= 0)
        //    {
        //        return RedirectToAction("Error");
        //    }
        //    ///***********************************************************************************
        //    var RevenueList = new List<PrintLandingViewModel>();
        //    int N = 0;
        //    foreach (var item in Revenue)
        //    {
        //        PrintLandingViewModel L = new PrintLandingViewModel();
        //        L.Id = item.Id;
        //        N = N + 1;
        //        L.N = N;
        //        // L.Name = item?.OfficerData.TowerData.AirLine.Code + "/ " + item?.OfficerData.TowerData.AirLine.Name;
        //        L.FlightNo = item?.OfficerData.TowerData.AirLinesFlightTrip.FlightNo;
        //        L.MaxTakoffWieght = item?.OfficerData.TowerData.AircraftRegistration.MaxTakoffWieght.ToString();
        //        L.LandingDate = CommonUsed.CheckDate(item?.OfficerData.TowerData.LandingDate.ToString());
        //        L.Wt = Convert.ToDouble(item.OfficerData.TowerData.AircraftRegistration.MaxTakoffWieght).ToString();
        //        L.Type = item.OfficerData.TowerData.AircraftRegistration.AircraftType.Type.ToString();
        //        L.RegNo = item.OfficerData.TowerData.AircraftRegistration.Registration.ToString();
        //        L.LandingFees = Convert.ToDecimal(item?.LandingFees);
        //        L.ParkingFees = Convert.ToDecimal(item?.ParkingFees);
        //        L.NightOperationFees = Convert.ToDecimal(item?.NightOperationFees);
        //        L.NavigationFees = Convert.ToDecimal(item?.NavigationFees);
        //        L.SecurityFees = Convert.ToDecimal(item?.SecurityFees);
        //        L.FreightFees = Convert.ToDecimal(item?.FreightFees);
        //        L.NoiseFees = Convert.ToDecimal(item?.NoiseFees);
        //        L.ImpMailFees = Convert.ToDecimal(item?.ImpMailFees);
        //        L.ExpMailFees = Convert.ToDecimal(item?.ExpMailFees);
        //        L.Total = L.ParkingFees + L.LandingFees + L.NightOperationFees + L.NavigationFees + L.FreightFees + L.SecurityFees
        //            + L.NoiseFees + L.ExpMailFees + L.ImpMailFees;

        //        RevenueList.Add(L);
        //    }



        //    ViewBag.DateFrom = SearchDateFrom.ToString();
        //    ViewBag.DateTo = SearchDateTo.ToString();

        //    double SumLandingFees = Convert.ToDouble(RevenueList.Sum(x => x.LandingFees));
        //    ViewBag.SumLandingFees = SumLandingFees;

        //    double SumNightOperationFees = Convert.ToDouble(RevenueList.Sum(x => x.NightOperationFees));
        //    ViewBag.SumNightOperationFees = SumNightOperationFees;

        //    double SumNavigationFees = Convert.ToDouble(RevenueList.Sum(x => x.NavigationFees));
        //    ViewBag.SumNavigationFees = SumNavigationFees;

        //    double SumSecurityFees = Convert.ToDouble(RevenueList.Sum(x => x.SecurityFees));
        //    ViewBag.SumSecurityFees = SumSecurityFees;

        //    double SumParkingFees = Convert.ToDouble(RevenueList.Sum(x => x.ParkingFees));
        //    ViewBag.SumParkingFees = SumParkingFees;

        //    double SumFreightFees = Convert.ToDouble(RevenueList.Sum(x => x.FreightFees));
        //    ViewBag.SumFreightFees = SumFreightFees;

        //    double SumImpMailFees = Convert.ToDouble(RevenueList.Sum(x => x.ImpMailFees));
        //    ViewBag.SumImpMailFees = SumImpMailFees;

        //    double SumExpMailFees = Convert.ToDouble(RevenueList.Sum(x => x.ExpMailFees));
        //    ViewBag.SumExpMailFees = SumExpMailFees;

        //    double SumNoiseFees = Convert.ToDouble(RevenueList.Sum(x => x.NoiseFees));
        //    ViewBag.SumNoiseFees = SumNoiseFees;


        //    double SumTotal = Convert.ToDouble(RevenueList.Sum(x => x.Total));
        //    ViewBag.SumTotal = SumTotal;

        //    ///////////////////////////////////////////
        //    double Tax = SumTotal * 0.05;
        //    ViewBag.Tax = Tax;

        //    double GrandTotal = Tax + SumTotal;
        //    ViewBag.GrandTotal = GrandTotal;




        //    if (towerData.AirLineId > 0)
        //    {
        //        AirLine airline = db.AirLines.Find(towerData.AirLineId);
        //        ViewBag.Name = airline.Name + "/" + airline.Code;
        //    }
        //    //************************************************************************************/


        //        ViewBag.Header = "Landing-" + Name + "-" + DateTime.Now.ToShortDateString();
        //        return View(RevenueList);





        //}

        public ActionResult PrintInoviceDetail(int? towerDataId)
       {
            if (towerDataId == null)
                return RedirectToAction("Error");

            var id = User.Identity.GetUserId();
            string UserName = CommonUsed.GetUserName(id);

            FinancialClaim1 financialClaim = db.FinancialClaim1.Where(x => x.TowerDataId == towerDataId).FirstOrDefault();
            // جلب بيانات الرحلة
            TowerData towerData = db.TowerDatas.Find(towerDataId);
            if (towerData == null)
                return RedirectToAction("Error");

            // جلب officer
            OfficerData officerData = db.OfficerDatas
                .FirstOrDefault(x => x.TowerDataId == towerDataId);

            if (officerData == null)
                return RedirectToAction("Error");

            // جلب revenue للرحلة فقط
            Revenue item = db.Revenues
                .FirstOrDefault(x => x.OfficerDataId == officerData.Id);

            if (item == null)
                return RedirectToAction("Error");


            // =================== تحويل إلى ViewModel ===================
            ExchangeRate = Convert.ToDecimal(financialClaim.ExchangeRateAgainstPrimaryCurrency);
          
            PrintInoviceDetailViewModel L = new PrintInoviceDetailViewModel();
            L.ExchangeRateAgainstPrimaryCurrency = ExchangeRate;
            L.Id = item.Id;
            L.N = 1;
            L.FlightNo = item?.OfficerData.TowerData.AirLinesFlightTrip.FlightNo;
            L.MaxTakoffWieght = item?.OfficerData.TowerData.AircraftRegistration.MaxTakoffWieght.ToString();
            L.LandingDate = CommonUsed.ToDisplayDate(item?.OfficerData.TowerData.LandingDate);
            L.Date = CommonUsed.ToDisplayDate(item?.OfficerData.TowerData.Date);

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

            L.TotalLanding =
                L.ParkingFees +
                L.LandingFees +
                L.NightOperationFees +
                L.NavigationFees +
                L.FreightFees +
                L.SecurityFees +
                L.NoiseFees +
                L.ExpMailFees +
                L.ImpMailFees;

          
            double Tax = Convert.ToDouble(L.TotalLanding) * 0.05;
            ViewBag.Tax = Tax;
            ViewBag.GrandTotal = Tax + Convert.ToDouble(L.TotalLanding);



            ///-----------------------------------------------------------------------------------------------------
            L.Pax = Convert.ToInt32(item?.OfficerData.Pax);
            L.TR = Convert.ToInt32(item?.OfficerData.TRNYCUPPS);


            L.AirPortTo = item?.OfficerData.TowerData.AirPort.Code;
            //CInter.TakeOffDate = CommonUsed.CheckDate(item?.OfficerData.TowerData.TakeOffDate.ToString());
            L.TakeOffDate = CommonUsed.ToDisplayDate(item?.OfficerData.TowerData.LandingDate);//تم تعديلها نسبة لان بعض الرحلات تكون بايتة وتظهر بتاريخ الشهر الجديد ودي مشكلة
            L.FlightNo = item?.OfficerData.TowerData.AirLinesFlightTrip.FlightNo;

            if (item?.OfficerData.TowerData.FlightTypeD == 1) // International cupps
            {
                L.CuppsFees = Convert.ToDecimal(item?.InterCUPPSFees);
                if (L.CuppsFees == 0)
                {
                    L.CFees = 0;
                }
                else
                {
                    L.CFees = (L.CuppsFees / (L.Pax + L.TR));
                }

                L.CUPPSSDG = L.CuppsFees * ExchangeRate;
                //----------------------------------------------------------------------------------------------------------------
            }
            else {
                L.CuppsFees = Convert.ToDecimal(item?.DomeCUPPSFees);
                if (L.CuppsFees == 0)
                {
                    L.CFees = 0;
                }
                else
                {
                    L.CFees = (L.CuppsFees / (L.Pax + L.TR));
                }

                L.CUPPSSDG = L.CuppsFees * ExchangeRate;
            }

           


            double TaxDollar = Convert.ToDouble(L.CuppsFees) * 0.05;
            ViewBag.TaxDollar = TaxDollar;

            double TaxSDG = Convert.ToDouble(L.CUPPSSDG) * 0.05;
            ViewBag.TaxSDG = TaxSDG;


            double GrandTotalDollar = Convert.ToDouble(L.CuppsFees) + TaxDollar;
            ViewBag.GrandTotalDollar = GrandTotalDollar;

            double GrandTotalSDG = Convert.ToDouble(L.CUPPSSDG) + TaxSDG;
            ViewBag.GrandTotalSDG = GrandTotalSDG;

            //--------------------------------------------------------

            if (item?.OfficerData.TowerData.FlightTypeD == 1) // International cupps
            {
                L.Pax = Convert.ToInt32(item?.OfficerData.Pax);


                L.AirPortTo = item?.OfficerData.TowerData.AirPort.Code;
                //N.TakeOffDate = CommonUsed.CheckDate(item?.OfficerData.TowerData.TakeOffDate.ToString());
                L.TakeOffDate = CommonUsed.ToDisplayDate(item?.OfficerData.TowerData.LandingDate);//تم تعديلها نسبة لان بعض الرحلات تكون بايتة وتظهر بتاريخ الشهر الجديد ودي مشكلة
                L.FlightNo = item?.OfficerData.TowerData.AirLinesFlightTrip.FlightNo;
                L.NyFees = Convert.ToDecimal(item?.InterNYFees);
                if (L.NyFees == 0)
                {
                    L.NFees = 0;
                }
                else
                {
                    L.NFees = (L.NyFees / L.Pax);
                }

                L.NyAmountSDG = L.NyFees * ExchangeRate;

            }
            else 
            {
                L.Pax = Convert.ToInt32(item?.OfficerData.Pax);
                L.AirPortTo = item?.OfficerData.TowerData.AirPort.Code;
              
                L.TakeOffDate = CommonUsed.ToDisplayDate(item?.OfficerData.TowerData.LandingDate);//تم تعديلها نسبة لان بعض الرحلات تكون بايتة وتظهر بتاريخ الشهر الجديد ودي مشكلة
                L.FlightNo = item?.OfficerData.TowerData.AirLinesFlightTrip.FlightNo;
                L.NyFees = Convert.ToDecimal(item?.DomeNYFees);
                if (L.NyFees == 0)
                {
                    L.NFees = 0;
                }
                else
                {
                    L.NFees = (L.NyFees / L.Pax);
                }

                L.NyAmountSDG = L.NyFees * ExchangeRate;
            }


          

            double NyFees = Convert.ToDouble(L.NyFees);
            ViewBag.NyFees = NyFees;
            double NYFeesSDG = NyFees * Convert.ToDouble(ExchangeRate);
            ViewBag.NYFeesSDG = NYFeesSDG;



            ///----------------------------------Start Service Car-------------------------------------------------------





            L.AmbulanceNo = Convert.ToInt32(item?.OfficerData.AmbulanceCarCount);
            L.FeesAmbulanceAmount = Convert.ToDecimal(item?.AmbolanceFees);

            if (L.FeesAmbulanceAmount == 0)
            {
                L.FeesAmbulanceAmount = 0;
            }
            else
            {
                L.FeesAmbulance = Convert.ToDecimal(L.FeesAmbulanceAmount / L.AmbulanceNo);
            }
            L.FireNo = Convert.ToInt32(item?.OfficerData.FireFightingCarCount);
            L.FeesFireAmount = Convert.ToDecimal(item?.FireFightingFees);
            if (L.FeesFireAmount == 0)
            {
                L.FeesFire = 0;
            }
            else
            {
                L.FeesFire = Convert.ToDecimal(L.FeesFireAmount / L.FireNo);
            }
            L.AmbulanceSDG = L.FeesAmbulanceAmount * ExchangeRate;
            L.FireSDG = L.FeesFireAmount * ExchangeRate;
            L.ServiceCar = Convert.ToDecimal(L.FeesAmbulanceAmount + L.FeesFireAmount);
            L.ServiceCarSDG = Convert.ToDecimal(L.AmbulanceSDG + L.FireSDG);

            double ServiceCarTaxDollar = Convert.ToDouble(L.ServiceCar) * 0.05;
            ViewBag.ServiceCarTaxDollar = ServiceCarTaxDollar;

            double ServiceCarTaxSDG = Convert.ToDouble(L.ServiceCarSDG) * 0.05;
            ViewBag.ServiceCarTaxSDG = ServiceCarTaxSDG;


            double ServiceCarGrandTotalDollar = Convert.ToDouble(L.ServiceCar) + ServiceCarTaxDollar;
            ViewBag.ServiceCarGrandTotalDollar = ServiceCarGrandTotalDollar;

            double ServiceCarGrandTotalSDG = Convert.ToDouble(L.ServiceCarSDG) + ServiceCarTaxSDG;
            ViewBag.ServiceCarGrandTotalSDG = ServiceCarGrandTotalSDG;




            if (L.FeesFireAmount > 0 && L.FeesAmbulanceAmount > 0)
            {
                L.HasCarService = true;
            }
            else { L.HasCarService = false; }
            //---------------------------------------End Service Car-------------------------------------------------




            ///------------- Start Pushpack
            L.HasPushPack = Convert.ToBoolean(item.OfficerData.HasPushPack);
            

            L.AirPortTo = item?.OfficerData.TowerData.AirPort.Code;      
            L.PlaneSize = item?.OfficerData.TowerData.AircraftRegistration.AircraftType.AircraftSize.Size;
            L.Wt = item?.OfficerData.TowerData.AircraftRegistration.MaxTakoffWieght.ToString();
            L.PushPackFees = Convert.ToDecimal(item?.PushBackFees);

            L.PushPackAmountSDG = ExchangeRate * L.PushPackFees;
          

            double PushPackTaxDollar = Convert.ToDouble(L.PushPackFees) * 0.05;
            ViewBag.PushPackTaxDollar = PushPackTaxDollar;

            double PushPackTaxSDG = Convert.ToDouble(L.PushPackAmountSDG) * 0.05;
            ViewBag.PushPackTaxSDG = PushPackTaxSDG;

            // Grand Total
            double PushPackGrandTotalDollar = Convert.ToDouble(L.PushPackFees) + PushPackTaxDollar;
            ViewBag.PushPackGrandTotalDollar = PushPackGrandTotalDollar;

            double PushPackGrandTotalSDG = Convert.ToDouble(L.PushPackAmountSDG) + PushPackTaxSDG;
            ViewBag.PushPackGrandTotalSDG = PushPackGrandTotalSDG;

            //----------------------------------------------------------------------------------

          
            L.TakeOffDate = CommonUsed.ToDisplayDate(item?.OfficerData.TowerData.TakeOffDate);

            // L.MaxTakoffWieght = item?.OfficerData.TowerData.AircraftRegistration.AircraftType.Type.ToString();

            L.FirstClassCount = Convert.ToInt32(item?.OfficerData.FirstClassCount);

            L.FirstClassFees = Convert.ToDecimal(item?.FirstClassFees);

            L.FirstClassFeesSDG = Convert.ToDecimal(item?.FirstClassFees) * Convert.ToDecimal(ExchangeRate);
            if (L.FirstClassFees == 0)
            {
                L.FFees = 0;
            }
            else
            {
                L.FFees = (L.FirstClassFees / L.FirstClassCount);

            }



            /*************************************************************************************/          

            double FirstClassTaxDollar = Convert.ToDouble(L.FirstClassFees) * 0.05;
            ViewBag.FirstClassTaxDollar = FirstClassTaxDollar;

            double FirstClassTaxSDG = Convert.ToDouble(L.FirstClassFeesSDG) * 0.05;
            ViewBag.FirstClassTaxSDG = FirstClassTaxSDG;

            // Grand Total
            double FirstClassGrandTotalDollar = Convert.ToDouble(L.FirstClassFees) + FirstClassTaxDollar;
            ViewBag.FirstClassGrandTotalDollar = FirstClassGrandTotalDollar;

            double FirstClassGrandTotalSDG = Convert.ToDouble(L.FirstClassFeesSDG) + FirstClassTaxSDG;
            ViewBag.FirstClassGrandTotalSDG = FirstClassGrandTotalSDG;


            if (L.FirstClassFees > 0 )
            {
                L.HasFirstClass = true;
            }
            else { L.HasFirstClass = false; }







            if (towerData.AirLineId > 0)
            {
                AirLine airline = db.AirLines.Find(towerData.AirLineId);
                ViewBag.Name = airline.Name + "/" + airline.Code;
            }

            ViewBag.Header = "Landing-" + ViewBag.Name + "-" + DateTime.Now.ToShortDateString();

            return View(L);

        }

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
            //Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.TakeOffDate).Where(x => x.OfficerData.Pax > 0 | x.OfficerData.Pax != null | x.OfficerData.TRNYCUPPS != null);
            Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.TakeOffDate).Where(x => x.OfficerData.Pax > 0 | x.OfficerData.TRNYCUPPS > 0);

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
                 (DbFunctions.TruncateTime(x.DateFrom) <= DateTo & DbFunctions.TruncateTime(x.DateTo) >= DateTo)))
            {
                var obj = db.FinancialClaim1.Where(x => x.AccIdDebit == AirLineAccId && (DbFunctions.TruncateTime(x.DateFrom) <= DateFrom & DbFunctions.TruncateTime(x.DateTo) >= DateFrom) |
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
            if (db.FinancialClaim1.Any(x => x.AccIdDebit == AirLineId && (DbFunctions.TruncateTime(x.DateFrom) <= DateFrom & DbFunctions.TruncateTime(x.DateTo) >= DateFrom) |
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




            double GrandATotalDollar = SumFeesAmbulanceAmount + TaxADollar;
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






            ViewBag.Header = "PushPack-" + Name + "-" + DateTime.Now.ToShortDateString();
            return View(RevenueList);
        }

    }
}