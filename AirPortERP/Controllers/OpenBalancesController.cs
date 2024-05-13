using AirPortERP.Models;
using AirPortERP.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AirPortERP.Extenstion;
using PagedList;
using System.Data.Entity;

namespace AirPortERP.Controllers
{
    public class OpenBalancesController : Controller
    {
        ClsAccount cls = new ClsAccount();
        Entities db = new Entities();
        // GET: OpenBalances
        public ActionResult Index()
        {
            return View();
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
                //r.AccId = Convert.ToInt32(item.AccId);
                //if (item.AccId > 0)
                //{
                //    r.AccName = item?.AccountTree.AccName;
                //}
                r.Date = cls.GetDateddMMyyyy(item.Date.ToString());
                r.Note = item?.Note;

                r.Debit = Convert.ToDecimal(item?.Debit).ToString("N");
                r.Credit = Convert.ToDecimal(item?.Credit).ToString("N");

                //});
                OpenAccList.Add(r);
            }

            return Json(new { data = OpenAccList.ToList() }, JsonRequestBehavior.AllowGet);
        }



        public ActionResult GetOpenAccParial(int? id)
        {

            OpenAccBalance OpenAccBalance = new OpenAccBalance();
            OpenAddBalanceViewModel model = new OpenAddBalanceViewModel();

           // ViewBag.AccId = new SelectList(db.AccountTrees, "Id", "AccName", model.AccId);
            if (id > 0)
            {

                OpenAccBalance = db.OpenAccBalances.Find(id);
              

             //   model.AccId = Convert.ToInt32(OpenAccBalance.AccId);
                model.Id = Convert.ToInt32(OpenAccBalance.Id);

                model.Debit = Convert.ToDecimal(OpenAccBalance.Debit);
                model.Credit = Convert.ToDecimal(OpenAccBalance.Credit);

                model.CurrencyId = Convert.ToInt32(OpenAccBalance.CurrencyID);

                model.Date = Convert.ToDateTime(OpenAccBalance.Date);
              //  model.Note = Convert.ToDecimal(OpenAccBalance.Credit);
            }
            ViewBag.AccId = new SelectList(db.AccountTrees.Where(x=>x.AccType==2), "Id", "AccName", model.AccId);

            return PartialView("_AddEditOpenAcc", model);

        }


        public ActionResult CreateOrUpdateBalance(OpenAddBalanceViewModel model)
        {
            if (ModelState.IsValid)
            {

                OpenAccBalance Balance = new OpenAccBalance();
              //  Balance.AccId = model.AccId;
                Balance.Id = model.Id;

                Balance.Credit = model.Credit;
                Balance.Debit = model.Debit;


                if (model.Id > 0)
                {
                    //if (db.OpenAccBalances.Any(x => x.AccId == model.AccId && x.Id != model.Id))
                    //{
                    //    return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
                    //}




                    Balance.Note = cls.GetAccountName(model.AccId) + "حساب افتتاحي ل";
                    db.Entry(Balance).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Update" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //if (db.OpenAccBalances.Any(x => x.AccId == model.AccId))
                    //{
                    //    return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
                    //}
                    Balance.Note = cls.GetAccountName(model.AccId) + "حساب افتتاحي ل";
                    Balance.Date = DateTime.Now;
                    db.OpenAccBalances.Add(Balance);
                    db.SaveChanges();
                    return Json(new { success = true, message = "Add" }, JsonRequestBehavior.AllowGet);
                }

            }
            return Json(new { success = false, message = "error" }, JsonRequestBehavior.AllowGet);
        }


       


    


























    }
}