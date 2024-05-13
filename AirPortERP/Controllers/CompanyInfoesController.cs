using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AirPortERP.Models;
using System.IO;
using AirPortERP.ViewModels;
using CrystalDecisions.CrystalReports.Engine;
using System.Drawing;

namespace AirPortERP.Controllers
{
    public class CompanyInfoesController : Controller
    {
        private Entities db = new Entities();
        public ActionResult Index()
        {
            return View();
        }   
        public ActionResult GetAllCompanyInfo()
        {

            IQueryable<CompanyInfo> CompanyInfo = db.CompanyInfoes;
            var CompanyInfoList = new List<CompanyInfoViewModels>();
            foreach (var item in CompanyInfo)
            {
                CompanyInfoList.Add(new CompanyInfoViewModels
                {
                    Id = item.Id,
                    NameAr = item?.NameAr,
                    NameEn = item?.NameEn,
                    Code = item?.Code,
                    Address = item?.Address,
                    Longitude = item?.Longitude,
                    Latitude = item?.Latitude,
            //0999777796
                   //Image = item?.Image,
                   IsActive = GetActiveName(Convert.ToBoolean(item.IsActive)),


                });
            }

            return Json(new { data = CompanyInfoList.ToList() }, JsonRequestBehavior.AllowGet);
        }    
        public ActionResult GetCompanyInfoParial(int? id)
        {
            CompanyInfo CompanyInfo = new CompanyInfo();
            if (id > 0)
            {
                CompanyInfo = db.CompanyInfoes.Find(id);
            }
            //ViewBag.AirLineId = new SelectList(db.AirLines, "Id", "Name", AirLineAgent.AirLineId);
            return PartialView("_AddEditCompanyInfo", CompanyInfo);
        }
        public ActionResult CreateOrUpdateCompanyInfo(CompanyInfo model)
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
                  ///  string Image_ID = SaveImageFile(model.ImageFile);

                   // db.CompanyInfoes.Add(model);
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
            CompanyInfo CompanyInfo = db.CompanyInfoes.Find(id);
            if (CompanyInfo == null)
            {
                return HttpNotFound();
            }

            //if (db.AirLinesFlightTrips.Any(x => x.AirPortId == id))
            //{
            //    return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
            //}

            var CompanyInfoDelete = db.CompanyInfoes.Find(id);
            db.CompanyInfoes.Remove(CompanyInfoDelete);
            db.SaveChanges();
            return Json(new { success = true, message = "Delete" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PrintData()
        {
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports/BasicData/RptAirLineAgents.rpt")));
            //IQueryable<AirLineAgent> AirLineAgents;
            //AirLineAgents = db.AirLineAgents;


            IQueryable<AirLineAgent> AirLineAgent = db.AirLineAgents;
            var AirLineAgentsList = new List<AirLineAgentsViewMosels>();
            foreach (var item in AirLineAgent)
            {
                AirLineAgentsList.Add(new AirLineAgentsViewMosels
                {
                    Id = item.Id,
                    NameAr = item?.NameAr,
                    NameAn = item?.NameAn,
                    Phone1 = item?.Phone1,
                    Phone2 = item?.Phone2,
                    Email = item?.Email,
                    Address = item?.Address,
                    AirLineName = item?.AirLine.Name,
                    IsActive = GetActiveName(Convert.ToBoolean(item.IsActive)),


                });
            }









            rd.SetDataSource(AirLineAgentsList.Select(p => new
            {

                NameEn = p.NameAn,
                NameAr = p.NameAr,
                Phone1 = p.Phone1,
                Phone2 = p.Phone2,
                Email = p.Email,
                Address = p.Address,
                AirLineName = p.AirLineName,
                IsActive = p.IsActive,
            }).ToList());



            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Stream Stream = rd.ExportToStream
                //WordForWindows
                (CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            Stream.Seek(0, SeekOrigin.Begin);//application/doc
            return File(Stream, "application/pdf", "AirLineAgents.pdf");
        }
        public string GetActiveName(bool IsActive)
        {
            string StatusName;
            if (IsActive == false)
            {
                StatusName = "Not Active";
            }
            else
            {
                StatusName = "Active";
            }
            return StatusName;
        }
        private string SaveImageFile(HttpPostedFileBase imageFile, string CurrentImageId = "")
        {


            if (imageFile != null)
            {


                var fileExtension = Path.GetExtension(imageFile.FileName);
                var imageGuid = Guid.NewGuid().ToString();

                string imageId = imageGuid + fileExtension;


                //Save File
                string filePath = Server.MapPath($"~/Uploads/Images/{imageId }");
                imageFile.SaveAs(filePath);

                //Delete Old File -Update
                if (!string.IsNullOrEmpty(CurrentImageId))
                {
                    string OldfilePath = Server.MapPath($"~/Uploads/Images/{CurrentImageId }");
                    System.IO.File.Delete(OldfilePath);

                }

                return imageId;
            }
            return CurrentImageId;
        }
        // GET: CompanyInfoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompanyInfo companyInfo = db.CompanyInfoes.Find(id);

            if (companyInfo == null)
            {
                return HttpNotFound();
            }
           
            return View(companyInfo);
        }
        // GET: CompanyInfoes/Create
        public ActionResult Create()
        {
            return View();
        }
        // POST: CompanyInfoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CompanyInfo model, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {


                if (ImageFile != null)
                {
                    model.Image = new byte[ImageFile.ContentLength];
                    ImageFile.InputStream.Read(model.Image, 0, ImageFile.ContentLength);
                }



                db.CompanyInfoes.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
        }
        // GET: CompanyInfoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompanyInfo companyInfo = db.CompanyInfoes.Find(id);
            if (companyInfo == null)
            {
                return HttpNotFound();
            }
            return View(companyInfo);
        }            
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CompanyInfo model, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {


                if (ImageFile != null)
                {
                    model.Image = new byte[ImageFile.ContentLength];
                    ImageFile.InputStream.Read(model.Image, 0, ImageFile.ContentLength);
                }
             



                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }
        // GET: CompanyInfoes/Delete/5
      

        // POST: CompanyInfoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CompanyInfo companyInfo = db.CompanyInfoes.Find(id);
            db.CompanyInfoes.Remove(companyInfo);
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
        public ActionResult show()
        {
            var CompanyInfo = db.CompanyInfoes.ToList();
            return View(CompanyInfo);
        }
    }
}