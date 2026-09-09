using AirPortERP.Extenstion;
using AirPortERP.Models;
using AirPortERP.ViewModels;
using Microsoft.AspNet.Identity;
using NumberToWord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AirPortERP.Controllers
{
    public class AccountOpenController : Controller
    {
        private int Format, Month, Year;
        static decimal ExchangeRate;
        private CommonUsed CommonUsed = new CommonUsed();
        private RoleController Role = new RoleController();
        ClsAccount cls = new ClsAccount();
        Entities db = new Entities();
        List<CurrencyInfo> currencies = new List<CurrencyInfo>();
        // GET: AccountOpen


        public AccountOpenController()
        {
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Sudan));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Dollar));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Syria));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.UAE));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.SaudiArabia));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Tunisia));
            currencies.Add(new CurrencyInfo(CurrencyInfo.Currencies.Gold));
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ConfirmOpenTransaction()
        {
            return View();
        }
        
        public ActionResult OpenParial(int? id)
        {

            OpenAccBalance OpenAccBalance = new OpenAccBalance();
            OpenAddBalanceViewModel model = new OpenAddBalanceViewModel();

            // ViewBag.AccId = new SelectList(db.AccountTrees, "Id", "AccName", model.AccId);
            if (id > 0)
            {

                OpenAccBalance = db.OpenAccBalances.Find(id);

                model.AccId = Convert.ToInt32(OpenAccBalance.AccId);
                model.Debit = OpenAccBalance.Debit;
                model.Credit = OpenAccBalance.Credit;

                //if (model.Debit > 0)
                //{
                //    model.AccId = Convert.ToInt32(OpenAccBalance.AccDebitId);
                //}
                //else { model.AccId = Convert.ToInt32(OpenAccBalance.AccCreditId); }
          
                model.Id = Convert.ToInt32(OpenAccBalance.Id);
                model.CurrencyId = Convert.ToInt32(OpenAccBalance.CurrencyID);
                model.Date = Convert.ToDateTime(OpenAccBalance.Date);        

            }


            ViewBag.CurrencyId = new SelectList(db.Currencies, "Id", "CurrencyName", model.CurrencyId);


            List<AccountTree> Acclist = new List<AccountTree>();
            Acclist = db.AccountTrees.Where(x => x.AccGroupId == 1).ToList();
            Acclist.Insert(0, new AccountTree() { AccName = "--Select Account--", Id = 0 });
            ViewBag.AccId = new SelectList(Acclist, "Id", "AccName", model.AccId);
            return PartialView("_AddEditOpenAcc", model);

        }
        
        public ActionResult GetAllOpenAcc()
        {

            IQueryable<OpenAccBalance> Balances = db.OpenAccBalances;
            var OpenAccList = new List<BalanceViewModel>();
            foreach (var item in Balances)
            {
                //AirPortsList.Add(new AirPortsViewMosels
                //{
                BalanceViewModel r = new BalanceViewModel();
                r.Id = item.Id;
             
                r.Date = cls.GetDateddMMyyyy(item.Date.ToString());
                r.Note = item?.Note;
                r.CurrencyName = db.Currencies.Find(item.CurrencyID).CurrencyName;

                r.Debit = Convert.ToDecimal(item?.Debit).ToString("N");
                r.Credit = Convert.ToDecimal(item?.Credit).ToString("N");
                r.AccId = Convert.ToInt32(item.AccId);
                if (r.AccId > 0)
                {
                    r.AccName = item?.AccountTree.AccName;
                    r.AccountNumber = cls.GetAccountNumber(Convert.ToDecimal(r.AccId));
                }

                //});
                OpenAccList.Add(r);
            }

            return Json(new { data = OpenAccList.ToList() }, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult GetAllOpenAccTransfer()
        {
            var id = User.Identity.GetUserId();
            IQueryable<OpenAccBalance> Balances = db.OpenAccBalances;
            var OpenAccList = new List<BalanceViewModel>();
            foreach (var item in Balances)
            {
                //AirPortsList.Add(new AirPortsViewMosels
                //{
                BalanceViewModel r = new BalanceViewModel();
                r.Id = item.Id;

                r.Date = cls.GetDateddMMyyyy(item.Date.ToString());
                r.Note = item?.Note;
                r.CurrencyName = db.Currencies.Find(item.CurrencyID).CurrencyName;

                r.Debit = Convert.ToDecimal(item?.Debit).ToString("N");
                r.Credit = Convert.ToDecimal(item?.Credit).ToString("N");
                r.AccId = Convert.ToInt32(item.AccId);
                if (r.AccId > 0)
                {
                    r.AccName = item?.AccountTree.AccName;
                    r.AccountNumber = cls.GetAccountNumber(Convert.ToDecimal(r.AccId));
                }

                r.Role = Role.GetRole(id);
                //});
                OpenAccList.Add(r);
            }

            return Json(new { data = OpenAccList.ToList() }, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult CreateOrUpdateBalance(OpenAddBalanceViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Credit > 0 && model.Debit > 0) 
                {
                    return Json(new { success = true, message = "AmountError" }, JsonRequestBehavior.AllowGet);
                }
                if (Convert.ToDecimal(model.Credit) == 0 && Convert.ToDecimal(model.Debit) == 0) 
                {
                    return Json(new { success = true, message = "AmountError" }, JsonRequestBehavior.AllowGet);
                }

                OpenAccBalance Balance = new OpenAccBalance();
                //  Balance.AccId = model.AccId;
              
                if (model.Id > 0)
                {
                    if (db.OpenAccBalances.Any(x => x.AccId == model.AccId && x.Id != model.Id))
                    {
                        return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
                    }

                    Balance = db.OpenAccBalances.Find(model.Id);

                    Balance.Credit = model.Credit;
                    Balance.Debit = model.Debit;
                    Balance.AccId = model.AccId;
                    Balance.CurrencyID = model.CurrencyId;
                    
                    Balance.Date = model.Date;
                    bool IsOperation = db.AccountOperations.Any(x => x.OperationID == model.Id && x.OperationTypeId == 4);
                    if (IsOperation)
                    {
                        //delete old operation
                        List<AccountOperation> Operation = db.AccountOperations.Where(x => x.OperationTypeId == 4 && x.OperationID == model.Id).ToList();
                        
                        foreach (var item in Operation)
                        {
                            db.AccountOperations.Remove(item);
                        }

                        //Add Opertion  operation
                        //----------------------------------------------------------------------------------------------------------------------------------------
                        ///----------------------------------عمل قيد مزدوج------------------------------------------
                        AccountOperation OperationDebit = new AccountOperation();
                        AccountOperation OperationCredit = new AccountOperation();

                        if (Convert.ToDecimal(model.Debit) > 0)
                        {
                            int Id = Convert.ToInt32(db.SpLastAccountOperationId().FirstOrDefault());
                            OperationDebit.Id = Id;
                            OperationDebit.AccId = model.AccId;
                            OperationDebit.OperationDebit = Convert.ToDecimal(model.Debit);
                            OperationDebit.OperationCredit = 0;
                            OperationDebit.OperationDate = Balance.Date;
                            OperationDebit.OperationID = model.Id;
                            OperationDebit.CurrencyId = model.CurrencyId;
                            OperationDebit.CurrencyValue = Convert.ToDouble(ExchangeRate);
                            OperationDebit.OperationTypeId = 4;
                            ///----------------------------------------------------------------------------
                            int Id2 = Convert.ToInt32(db.SpLastAccountOperationId().FirstOrDefault() + 1);
                            OperationCredit.Id = Id2;
                            OperationCredit.AccId = 7;  /// المدنيون
                            OperationCredit.OperationDebit = 0;
                            OperationCredit.OperationCredit = Convert.ToDecimal(model.Debit);
                            OperationCredit.OperationDate = Balance.Date;
                            OperationCredit.OperationID = model.Id;
                            OperationCredit.CurrencyId = model.CurrencyId;
                            OperationCredit.CurrencyValue = Convert.ToDouble(ExchangeRate);
                            OperationCredit.OperationTypeId = 4;
                        }
                        else
                        {
                            int Id = Convert.ToInt32(db.SpLastAccountOperationId().FirstOrDefault());
                            OperationDebit.Id = Id;
                            OperationDebit.AccId = model.AccId;
                            OperationDebit.OperationDebit = 0;
                            OperationDebit.OperationCredit = Convert.ToDecimal(model.Credit);
                            OperationDebit.OperationDate = Balance.Date;
                            OperationDebit.OperationID = model.Id;
                            OperationDebit.CurrencyId = model.CurrencyId;
                            OperationDebit.CurrencyValue = Convert.ToDouble(ExchangeRate);
                            OperationDebit.OperationTypeId = 4;
                            ///----------------------------------------------------------------------------
                            int Id2 = Convert.ToInt32(db.SpLastAccountOperationId().FirstOrDefault() + 1);
                            OperationCredit.Id = Id2;
                            OperationCredit.AccId = 7;  // الدائنون16
                            OperationCredit.OperationDebit = Convert.ToDecimal(model.Credit);
                            OperationCredit.OperationCredit = 0;
                            OperationCredit.OperationDate = model.Date;
                            OperationCredit.OperationID = model.Id;
                            OperationCredit.CurrencyId = model.CurrencyId;
                            OperationCredit.CurrencyValue = Convert.ToDouble(ExchangeRate);
                            OperationCredit.OperationTypeId = 4;

                        }
                        db.AccountOperations.Add(OperationDebit);
                        db.AccountOperations.Add(OperationCredit);

                        //------------------------------------------------------------------------------------------------------------------------------------------
                    }

                    db.Entry(Balance).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Update" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (db.OpenAccBalances.Any(x => x.AccId == model.AccId))
                    {
                        return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
                    }
                    
                    Balance.Credit = model.Credit;
                    Balance.Debit = model.Debit;
                    Balance.CurrencyID = model.CurrencyId;
                    Balance.AccId = model.AccId;
                    Balance.Date = DateTime.Now;
                    db.OpenAccBalances.Add(Balance);

                    db.SaveChanges();

                    return Json(new { success = true, message = "Add" }, JsonRequestBehavior.AllowGet);
                }

            }
            return Json(new { success = false, message = "error" }, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult GetOpenToConfirm(int id)
        {
            string Message = "";
            string MoneyText = "";
            OpenAccBalance OpenAcc = db.OpenAccBalances.Find(id);
            if (OpenAcc.Debit > 0)

            {
                Message = OpenAcc.Debit + "مدينة بمبلغ  " + OpenAcc.AccountTree.AccName;
                MoneyText = GetAmountTextString(Convert.ToInt32(OpenAcc.CurrencyID), Convert.ToDecimal(OpenAcc.Debit));
            }
            else
            {
                Message = OpenAcc.Credit + "دائنة بمبلغ  " + OpenAcc.AccountTree.AccName;
                MoneyText = GetAmountTextString(Convert.ToInt32(OpenAcc.CurrencyID), Convert.ToDecimal(OpenAcc.Credit));
            }
           

           // double value = Convert.ToDouble(Claim.Amount);
            

            return Json(new { success = true, message = Message, moneytext = MoneyText }, JsonRequestBehavior.AllowGet);


            ///--------------------------------------Dollar-----------------------------------------------------------
        }

        public ActionResult AddToConfrim(int id)
        {
            // string MoneyText = "";
            OpenAccBalance Open = db.OpenAccBalances.Find(id);


            ///----------------------------------عمل قيد مزدوج------------------------------------------
            AccountOperation OperationDebit = new AccountOperation();
            AccountOperation OperationCredit = new AccountOperation();
            if (Convert.ToDecimal(Open.Debit) > 0)
            {
                int Id = Convert.ToInt32(db.SpLastAccountOperationId().FirstOrDefault());
                OperationDebit.Id = Id;
                OperationDebit.AccId = Open.AccId;
                OperationDebit.OperationDebit = Convert.ToDecimal(Open.Debit);
                OperationDebit.OperationCredit = 0;
                OperationDebit.OperationDate = Open.Date;
                OperationDebit.OperationID = Open.Id;
                OperationDebit.CurrencyId = Open.CurrencyID;
                OperationDebit.OperationTypeId = 4;
                ///----------------------------------------------------------------------------
                int Id2 = Convert.ToInt32(db.SpLastAccountOperationId().FirstOrDefault() + 1);
                OperationCredit.Id = Id2;
                OperationCredit.AccId = 7;  /// المدنيون
                OperationCredit.OperationDebit = 0;
                OperationCredit.OperationCredit = Convert.ToDecimal(Open.Debit);
                OperationCredit.OperationDate = Open.Date;
                OperationCredit.OperationID = Open.Id;
                OperationCredit.CurrencyId = Open.CurrencyID;
                OperationCredit.OperationTypeId = 4;
            }
            else
            {
                int Id = Convert.ToInt32(db.SpLastAccountOperationId().FirstOrDefault());
                OperationDebit.Id = Id;
                OperationDebit.AccId = Open.AccId;
                OperationDebit.OperationDebit = 0;
                OperationDebit.OperationCredit = Convert.ToDecimal(Open.Credit);
                OperationDebit.OperationDate = Open.Date;
                OperationDebit.OperationID = Open.Id;
                OperationDebit.CurrencyId = Open.CurrencyID;
                OperationDebit.OperationTypeId = 4;
                ///----------------------------------------------------------------------------
                int Id2 = Convert.ToInt32(db.SpLastAccountOperationId().FirstOrDefault() + 1);
                OperationCredit.Id = Id2;
                OperationCredit.AccId = 7;  //16 الدائنون
                OperationCredit.OperationDebit = Convert.ToDecimal(Open.Credit);
                OperationCredit.OperationCredit = 0;
                OperationCredit.OperationDate = Open.Date;
                OperationCredit.OperationID = Open.Id;
                OperationCredit.CurrencyId = Open.CurrencyID;
                OperationCredit.OperationTypeId = 4;

            }

            db.AccountOperations.Add(OperationDebit);
            db.AccountOperations.Add(OperationCredit);
            db.SaveChanges();

            return Json(new { success = true, message = "Add" }, JsonRequestBehavior.AllowGet);


            ///--------------------------------------Dollar-----------------------------------------------------------
        }
        
        public string GetAmountTextString(int CurrencyId, decimal Statement)
        {
            string MoneyText = "";
            if (CurrencyId == 2)
            {
                double SDGRound = Convert.ToDouble(Math.Round(Statement, 2));

                ToWord toWord = new ToWord(Convert.ToDecimal(SDGRound), currencies[Convert.ToInt32(0)]);

                MoneyText = toWord.ConvertToArabic().ToString();

                return MoneyText;


                ///--------------------------------------Dollar-----------------------------------------------------------
            }
            else
            {
                double DollarRound = Convert.ToDouble(Math.Round(Statement, 2));
                ToWord toWord1 = new ToWord(Convert.ToDecimal(DollarRound), currencies[Convert.ToInt32(1)]);
                MoneyText = toWord1.ConvertToArabic().ToString();


                return MoneyText;
            }
        }
        
        public ActionResult Delete(int id)
        {
            OpenAccBalance OpenAcc = db.OpenAccBalances.Find(id);

            if (db.AccountOperations.Any(x => x.OperationTypeId == 4 && x.OperationID == OpenAcc.Id))
            {
                List<AccountOperation> Operation = db.AccountOperations.Where(x => x.OperationTypeId == 4 && x.OperationID == OpenAcc.Id).ToList();


                foreach (var item in Operation)
                {
                    db.AccountOperations.Remove(item);
                }
            }

            db.OpenAccBalances.Remove(OpenAcc);
            db.SaveChanges();


            return Json(new { success = true, message = "Delete" }, JsonRequestBehavior.AllowGet);



        }
        
        public ActionResult Transfer(int id)
        {
            OpenAccBalance OpenAcc = db.OpenAccBalances.Find(id);


            db.Entry(OpenAcc).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return Json(new { success = true, message = "Add" }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult ReTransfer(int id)
        {
            OpenAccBalance OpenAcc = db.OpenAccBalances.Find(id);


            db.Entry(OpenAcc).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return Json(new { success = true, message = "Add" }, JsonRequestBehavior.AllowGet);

        }
        
    }
}