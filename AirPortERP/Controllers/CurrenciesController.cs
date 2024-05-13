using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AirPortERP.Models;
using AirPortERP.ViewModels;
using AirPortERP.Extenstion;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;

namespace AirPortERP.Controllers
{
    public class CurrenciesController : Controller
    {
        private Entities db = new Entities();
        private CommonUsed CommonUsed = new CommonUsed();
        // GET: Currencies
        public ActionResult Index()
        {
            return View(db.Currencies.ToList());
        }

        public ActionResult GetAllCurrencies()
        {

            IQueryable<Currency> Currencies = db.Currencies;


            var CurrenciesList = new List<CurrencieViewModels>();
            foreach (var item in Currencies)
            {
                CurrenciesList.Add(new CurrencieViewModels
                {
                    Id = item.Id,
                    CurrencyName = item.CurrencyName,
                    IsActive = item.IsActive,
                    MajorUnit = item.MajorUnit,
                    MinorUnit = item.MinorUnit,
                    ExchangeRateAgainstPrimaryCurrency = item.ExchangeRateAgainstPrimaryCurrency,
                    Notes = item.Notes,
                    CreatedBy = CommonUsed.GetUserName(item.CreatedBy),
                    CreationDate = CommonUsed.CheckDate(item.CreationDate.ToString()),
                    UpdatedBy = CommonUsed.GetUserName(item.UpdatedBy),
                    UpdatingDate = CommonUsed.CheckDate(item.UpdatingDate.ToString()),


                });

            }


            return Json(new { data = CurrenciesList }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult GetCurrenciesParial(int? id)
        {

            Currency Currencies = new Currency();
            if (id > 0)
            {

                Currencies = db.Currencies.Find(id);

            }


            return PartialView("_AddEditCurrencies", Currencies);

        }

        public ActionResult CreateOrUpdateCurrencies(Currency model)
        {
            if (ModelState.IsValid)
            {

                if (model.Id > 0)
                {


                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Update" }, JsonRequestBehavior.AllowGet);
                }
                else
                {

                    db.Currencies.Add(model);
                    db.SaveChanges();
                    return Json(new { success = true, message = "Add" }, JsonRequestBehavior.AllowGet);
                }

            }
            return Json(new { success = false, message = "error" }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Currency Currencies = db.Currencies.Find(id);
            if (Currencies == null)
            {
                return HttpNotFound();
            }

            //if (db.AirPorts.Any(x => x.Id == Currencies.Id))
            //{
            //    return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
            //}

            var CurrenciesDelete = db.Currencies.Find(id);
            db.Currencies.Remove(CurrenciesDelete);
            db.SaveChanges();
            return Json(new { success = true, message = "Delete" }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult PrintData()
        {
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports/BasicData/RptCurrencies.rpt")));
            IQueryable<Currency> Currencies;
            Currencies = db.Currencies;
            rd.SetDataSource(Currencies.Select(p => new
            {
                Id = p.Id,
                CurrencyName = p.CurrencyName,
                IsActive = p.IsActive,
                MajorUnit = p.MajorUnit,
                MinorUnit = p.MinorUnit,
                ExchangeRateAgainstPrimaryCurrency = p.ExchangeRateAgainstPrimaryCurrency,
                Notes = p.Notes,
                CreatedBy = p.CreatedBy,
                CreationDate = p.CreationDate,
                UpdatedBy = p.UpdatedBy,
                UpdatingDate = p.UpdatingDate,
            }).ToList());



            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Stream Stream = rd.ExportToStream
                //WordForWindows
                (CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            Stream.Seek(0, SeekOrigin.Begin);//application/doc
            return File(Stream, "application/pdf", "Currencies.pdf");
        }


        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Currency Currencies = db.Currencies.Find(id);
            if (Currencies == null)
            {
                return HttpNotFound();
            }


            var item = db.Currencies.Single(x => x.Id == id);





            var CurrenciesViewModelList = new CurrencieViewModels
            {


                Id = item.Id,
                CurrencyName = item.CurrencyName,
                IsActive = item.IsActive,
                MajorUnit = item.MajorUnit,
                MinorUnit = item.MinorUnit,
                ExchangeRateAgainstPrimaryCurrency = item.ExchangeRateAgainstPrimaryCurrency,
                Notes = item.Notes,
                CreatedBy = CommonUsed.GetUserName(item.CreatedBy),
                CreationDate = CommonUsed.CheckDate(item.CreationDate.ToString()),
                UpdatedBy = CommonUsed.GetUserName(item.UpdatedBy),
                UpdatingDate = CommonUsed.CheckDate(item.UpdatingDate.ToString()),

            };

            return View(CurrenciesViewModelList);
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
