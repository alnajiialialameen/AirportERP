using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AirPortERP.Models;
using AirPortERP.Extenstion;
using AirPortERP.ViewModels;
using Microsoft.AspNet.Identity;

namespace AirPortERP.Controllers
{
    public class RegisterationCoresController : Controller
    {
        private Entities db = new Entities();

        private CommonUsed CommonUsed = new CommonUsed();

        // GET: RegisterationCores
        public ActionResult Index()
        {
            return View(db.RegisterationCores.ToList());
        }





        public ActionResult GetAllRegisterationCores()
        {
            IQueryable<RegisterationCore> RegisterationCore = db.RegisterationCores;


            var RegisterationCoreList = new List<RegisterationCoreViewModels>();
            foreach (var item in RegisterationCore)
            {
                RegisterationCoreList.Add(new RegisterationCoreViewModels
                {
                    Id = item.Id,
                    CompanyCode = item.CompanyCode,
                    Reg = item.Reg.ToString(),
                    Type = item.Type.ToString(),
                    MaxTakeOff = item.MaxTakeOff.ToString(),
                    PlaneSize = item.PlaneSize.ToString(),
                    CreatedBy = CommonUsed.GetUserName(item.CreatedBy),
                    CreationDate = CommonUsed.CheckDate(item.CreationDate.ToString()),
                    UpdatedBy = CommonUsed.GetUserName(item.UpdatedBy),
                    UpdatingDate = CommonUsed.CheckDate(item.UpdatingDate.ToString()),


                });

            }
            return Json(new { data = RegisterationCoreList }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult GetRegisterationCoresParial(int? id)
        {

            RegisterationCore RegisterationCores = new RegisterationCore();
            if (id > 0)
            {


                RegisterationCores = db.RegisterationCores.Find(id);

            }


            return PartialView("_AddEditRegisterationCores", RegisterationCores);

        }

        public ActionResult CreateOrUpdateRegisterationCores(RegisterationCore model)
        {
            if (ModelState.IsValid)
            {
                var UserId = User.Identity.GetUserId();
                if (model.Id > 0)
                {

                    model.UpdatedBy = UserId;
                    model.UpdatingDate = System.DateTime.Now;
                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Update" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    model.CreatedBy = UserId;
                    model.CreationDate = System.DateTime.Now;
                    db.RegisterationCores.Add(model);
                    db.SaveChanges();
                    return Json(new { success = true, message = "Add" }, JsonRequestBehavior.AllowGet);
                }

            }
            return Json(new { success = false, message = "error" }, JsonRequestBehavior.AllowGet);
        }

        // GET: RegisterationCores/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RegisterationCore registerationCore = db.RegisterationCores.Find(id);
            if (registerationCore == null)
            {
                return HttpNotFound();
            }
            return View(registerationCore);
        }

        // GET: RegisterationCores/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RegisterationCores/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CompanyCode,Reg,Type,MaxTakeOff,PlaneSize")] RegisterationCore registerationCore)
        {
            if (ModelState.IsValid)
            {
                db.RegisterationCores.Add(registerationCore);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(registerationCore);
        }

        // GET: RegisterationCores/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RegisterationCore registerationCore = db.RegisterationCores.Find(id);
            if (registerationCore == null)
            {
                return HttpNotFound();
            }
            return View(registerationCore);
        }

        // POST: RegisterationCores/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CompanyCode,Reg,Type,MaxTakeOff,PlaneSize")] RegisterationCore registerationCore)
        {
            if (ModelState.IsValid)
            {
                db.Entry(registerationCore).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(registerationCore);
        }

        // GET: RegisterationCores/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RegisterationCore registerationCore = db.RegisterationCores.Find(id);
            if (registerationCore == null)
            {
                return HttpNotFound();
            }
            return View(registerationCore);
        }

        // POST: RegisterationCores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RegisterationCore registerationCore = db.RegisterationCores.Find(id);
            db.RegisterationCores.Remove(registerationCore);
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
