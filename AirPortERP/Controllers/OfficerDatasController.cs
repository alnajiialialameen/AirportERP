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
using PagedList;
using AirPortERP.Extenstion;
using Microsoft.AspNet.Identity;
using System.IO;

namespace AirPortERP.Controllers
{
    
    public class OfficerDatasController : Controller
    {

        private Entities db = new Entities();
        private CommonUsed CommonUsed = new CommonUsed();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult OfficerDataAll()
        {
            return View();
        }
        public ActionResult GetOfficerDataAll()
        {

            IQueryable<OfficerData> OfficerData = db.OfficerDatas;
            var OfficerDataList = new List<OfficerDataViewModels>();
            foreach (var item in OfficerData)
            {
                OfficerDataList.Add(new OfficerDataViewModels
                {
                    Id = item.Id,
                    AirLineName = item?.TowerData.AirLine.Name,
                    FlightNo = item?.TowerData.AirLinesFlightTrip.FlightNo,
                    AircraftRegName = item?.TowerData.AircraftRegistration.Registration,
                    Type = item.TowerData.AircraftRegistration.AircraftType.Type.ToString(),
                    AirportIdFrom = CommonUsed.CheckAirportCode(Convert.ToInt32(item?.TowerData.AirportIdFrom)),
                    AirportIdTo = CommonUsed.CheckAirportCode(Convert.ToInt32(item?.TowerData.AirportIdTo)),
                    Date = Convert.ToDateTime(item.TowerData.Date).ToString("MM/dd/yyyy"),
                    ATA = item.TowerData.ATA.ToString(),
                    ATD = item.TowerData.ATD.ToString(),
                    LandingDate = CommonUsed.CheckDate(item.TowerData.LandingDate.ToString()),

                    CrewNO = item.CrewNO,
                    HandlingAgentName = item.HandlingAgentsCompany.NameEn,
                    IsCurrentAccount = CommonUsed.CheckYesNo(item.IsCurrentAccount),
                    HasNightStop = CommonUsed.CheckYesNo(item.HasNightStop),
                    AireLineAgentName = item.AirLineAgent.NameAn,



                    //TakeOffDate = CheckDate(item.TakeOffDate.ToString()),

                    //POB = item.POB,
                    //QBD = item.QBD,
                    //LandingPermission = item.LandingPermission,

                    //CreatedBy = GetUserName(item.CreatedBy),
                    //CreationDate = item.CreationDate.ToString(),
                    //UpdatedBy = GetUserName(item.UpdatedBy),
                    //UpdatingDate = item.UpdatingDate.ToString(),

                    //LandingDate = CheckDate(item.LandingDate.ToString()),
                    //TakeOffDate = CheckDate(item.TakeOffDate.ToString()),
                });
            }

            return Json(new { data = OfficerDataList.ToList() }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetAllTowerDatas()
        {

            IQueryable<TowerData> TowerData = db.TowerDatas.Where(x=>x.Status==1);
            var TowerDatasList = new List<TowerDataViewModels>();
            foreach (var item in TowerData)
            {
                TowerDatasList.Add(new TowerDataViewModels
                {
                    Id = item.Id,
                    AirLineName = item?.AirLine.Name,
                    FlightNo = item?.AirLinesFlightTrip.FlightNo,
                    AircraftRegName = item?.AircraftRegistration.Registration,
                    Type = item?.AircraftRegistration.AircraftType.Type,
                    AirportIdFrom = CommonUsed.CheckAirportCode(Convert.ToInt32(item?.AirportIdFrom)),
                    AirportIdTo = CommonUsed.CheckAirportCode(Convert.ToInt32(item?.AirportIdTo)),
                    Date = Convert.ToDateTime(item.Date).ToString("MM/dd/yyyy"),
                    ATA = item.ATA.ToString(),
                    ATD = item.ATD.ToString(),
                    LandingDate =CommonUsed.CheckDate(item.LandingDate.ToString()),
                    //TakeOffDate = CheckDate(item.TakeOffDate.ToString()),

                    //POB = item.POB,
                    //QBD = item.QBD,
                    //LandingPermission = item.LandingPermission,

                    //CreatedBy = GetUserName(item.CreatedBy),
                    //CreationDate = item.CreationDate.ToString(),
                    //UpdatedBy = GetUserName(item.UpdatedBy),
                    //UpdatingDate = item.UpdatingDate.ToString(),

                    //LandingDate = CheckDate(item.LandingDate.ToString()),
                    //TakeOffDate = CheckDate(item.TakeOffDate.ToString()),
                });
            }

            return Json(new { data = TowerDatasList.ToList() }, JsonRequestBehavior.AllowGet);
        }




        public ActionResult GetOfficerDataParial(int? TowerDataId)
        {
            var OfficerDataViewModelsOnly = new OfficerDataViewModelsOnly();
            OfficerData OfficerData = db.OfficerDatas.Where(x=>x.TowerDataId== TowerDataId).FirstOrDefault();

            if (OfficerData != null)
            {

                 OfficerDataViewModelsOnly = new OfficerDataViewModelsOnly
                {

                    Id = OfficerData.Id,
                    TowerDataId = OfficerData.TowerDataId,
                    CrewNO = OfficerData.CrewNO,
                    HasNightStop = OfficerData.HasNightStop,
                    HandlingAgentId = OfficerData.HandlingAgentId,
                    AireLineAgentId = OfficerData.AireLineAgentId,
                    IsCurrentAccount = OfficerData.IsCurrentAccount,

                 };


            }

            OfficerDataViewModelsOnly.TowerDataId = TowerDataId;
            ViewBag.AireLineAgentId = new SelectList(db.AirLineAgents, "Id", "NameAr", OfficerDataViewModelsOnly.AireLineAgentId);
            //ViewBag.CompanyInfoId = new SelectList(db.CompanyInfoes, "Id", "NameAr", OfficerDataViewModelsOnly.CompanyInfoId);
            ViewBag.HandlingAgentId = new SelectList(db.HandlingAgentsCompanies, "Id", "NameAr", OfficerDataViewModelsOnly.HandlingAgentId);
            //ViewBag.ServiceCarReqId = new SelectList(db.ServiceCarCompanies, "Id", "NameAr", OfficerDataViewModelsOnly.ServiceCarReqId);
            ViewBag.TowerDataId = new SelectList(db.TowerDatas, "Id", "LandingPermission", OfficerDataViewModelsOnly.TowerDataId);

            return PartialView("_AddEditOfficerData", OfficerDataViewModelsOnly);

        }

        public ActionResult GetOfficerDataParialOfficer(int? OfficerId)
        {
            var OfficerDataViewModelsOnly = new OfficerDataViewModelsOnly();
            OfficerData OfficerData = db.OfficerDatas.Where(x =>x.Id == OfficerId).FirstOrDefault();



            if (OfficerData != null)
            {

                OfficerDataViewModelsOnly = new OfficerDataViewModelsOnly
                {

                    Id = OfficerData.Id,
                    TowerDataId = OfficerData.TowerDataId,
                    CrewNO = OfficerData.CrewNO,
                    HasNightStop = OfficerData.HasNightStop,
                    HandlingAgentId = OfficerData.HandlingAgentId,
                    AireLineAgentId = OfficerData.AireLineAgentId,
                    IsCurrentAccount = OfficerData.IsCurrentAccount,
                   

                };


            }
           
            ViewBag.AireLineAgentId = new SelectList(db.AirLineAgents, "Id", "NameAr", OfficerDataViewModelsOnly.AireLineAgentId);
            //ViewBag.CompanyInfoId = new SelectList(db.CompanyInfoes, "Id", "NameAr", OfficerDataViewModelsOnly.CompanyInfoId);
            ViewBag.HandlingAgentId = new SelectList(db.HandlingAgentsCompanies, "Id", "NameAr", OfficerDataViewModelsOnly.HandlingAgentId);
            //ViewBag.ServiceCarReqId = new SelectList(db.ServiceCarCompanies, "Id", "NameAr", OfficerDataViewModelsOnly.ServiceCarReqId);
            ViewBag.TowerDataId = new SelectList(db.TowerDatas, "Id", "LandingPermission", OfficerDataViewModelsOnly.TowerDataId);

            return PartialView("_AddEditOfficerData", OfficerDataViewModelsOnly);

        }


        public ActionResult CreateOrUpdateOfficerData(OfficerDataViewModelsOnly model)
        {
            if (ModelState.IsValid)
            {
                var id = User.Identity.GetUserId();
                if (model.Id > 0)
                {

                    var UpdateOfficerData = new OfficerData
                    {
                        Id = model.Id,
                        TowerDataId = model.TowerDataId,
                        CrewNO = model.CrewNO,
                        HasNightStop = model.HasNightStop,
                        HandlingAgentId = model.HandlingAgentId,
                        AireLineAgentId = model.AireLineAgentId,
                        UpdatedBy = id,
                        UpdatingDate = System.DateTime.Now,
                        CreatedBy = model.OfficerDataCreatedBy,
                        CreationDate = model.OfficerDataCreationDate,

                         ServiceCarReqId = model.ServiceCarReqId,
                        IsCurrentAccount = model.IsCurrentAccount,
                    };


                    db.TowerDatas.Where(x => x.Id == model.TowerDataId).Single().Status = 2;

                    db.Entry(UpdateOfficerData).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Update" }, JsonRequestBehavior.AllowGet);
                }
                else
                {

                    var CreateOfficerData = new OfficerData
                    {
                        Id = model.Id,
                        TowerDataId = model.TowerDataId,
                        CrewNO = model.CrewNO,
                        HasNightStop = model.HasNightStop,
                        HandlingAgentId = model.HandlingAgentId,
                        AireLineAgentId = model.AireLineAgentId,
                        CreatedBy = id,
                        CreationDate = System.DateTime.Now,

                        ServiceCarReqId = 1,
                        IsCurrentAccount = model.IsCurrentAccount,


                    };



                    db.TowerDatas.Where(x => x.Id == model.TowerDataId).Single().Status = 2;
                    

                    db.OfficerDatas.Add(CreateOfficerData);
                    db.SaveChanges();
                    return Json(new { success = true, message = "Add" }, JsonRequestBehavior.AllowGet);
                }

            }
            return Json(new { success = false, message = "error" }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult OfficerDataReport()
        {
            return View();
        }

        public ActionResult GetData(int? page, int? Get, int? Search, int? Count, int? AirLineId
            ,int? FlightNo,int? AirPortIdFrom,int? AirPortIdTo ,DateTime? DateFrom, DateTime? DateTo
            ,int? HandlingAgentId,int? RegId)
        {
            IQueryable<OfficerData> OfficerData;
            OfficerData = db.OfficerDatas.OrderBy(x => x.Id);

             ViewBag.AirLineId = AirLineId;
             ViewBag.FlightNo = FlightNo;
             ViewBag.AirPortIdFrom = AirPortIdFrom;
             ViewBag.AirPortIdTo = AirPortIdTo;
             ViewBag.HandlingAgentId = HandlingAgentId;

            ViewBag.DateFrom = DateFrom;
            ViewBag.DateTo = DateTo;
            ViewBag.RegId = RegId;

            if (Get > 0)
            {

                if (RegId > 0)
                {
                    OfficerData = OfficerData.Where(x => x.TowerData.AircraftRegId == RegId);
                }

                if (AirPortIdFrom > 0)
                {
                    OfficerData = OfficerData.Where(x => x.TowerData.AirportIdFrom == AirPortIdFrom);
                }
                if (AirPortIdTo > 0)
                {
                    OfficerData = OfficerData.Where(x => x.TowerData.AirportIdTo == AirPortIdTo);
                }

                //if (!string.IsNullOrEmpty(Name))
                //{

                //    ExecuteCourses = ExecuteCourses.Where(x => x.Course.Name.Contains(Name)).OrderBy(x => x.Id);
                //}
                if (AirLineId > 0)
                {
                    OfficerData = OfficerData.Where(x => x.TowerData.AirLineId == AirLineId);
                }
                if (FlightNo > 0)
                {
                    OfficerData = OfficerData.Where(x => x.TowerData.FlightNo == FlightNo);
                }

                if (HandlingAgentId > 0)
                {
                    OfficerData = OfficerData.Where(x => x.HandlingAgentId == HandlingAgentId);
                }
                //if (IsInPlan > 0)
                //{
                //    bool t;
                //    if (IsInPlan == 1) t = true; else t = false;
                //    ExecuteCourses = ExecuteCourses.Where(x => x.Course.IsInPlan == t);
                //}
                if (DateFrom != null && DateTo != null)
                {

                    OfficerData = OfficerData.Where(t => DbFunctions.TruncateTime(t.TowerData.Date) >= DbFunctions.TruncateTime(DateFrom) && DbFunctions.TruncateTime(t.TowerData.Date) <= DbFunctions.TruncateTime(DateTo));
                }


                //////////////////////////////////////////////////////
                if (Search == 1)
                {



                    if (OfficerData != null)
                    {

                        int ItemCount = 0;
                        //List<decimal> Amountlist = new List<decimal>();
                        //List<decimal> Allowancelist = new List<decimal>();


                        ItemCount = OfficerData.Count();
                        ViewBag.ItemCount = ItemCount;

                        //foreach (ExecuteCourse c in ExecuteCourses)
                        //{
                        //    Allowancelist.Add(Convert.ToDecimal(c.TotalAllowance));
                        //    Amountlist.Add(Convert.ToDecimal(c.Amount));
                        //}

                        //ViewBag.Amount = Amountlist.Sum();
                        //ViewBag.Allowance = Allowancelist.Sum();

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
                OfficerData = null;
                ViewBag.ItemCount = 0;
            }

            int pageSize = 5;
            int PageNumber = (page ?? 1);




            return PartialView("_GetData", OfficerData.ToPagedList(PageNumber, pageSize));
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OfficerData OfficerData = db.OfficerDatas.Find(id);
            if (OfficerData == null)
            {
                return HttpNotFound();
            }


            var item = OfficerData;




            var OfficerDataViewModelList = new OfficerDataViewModels
            {

                Id = item.Id,

                //year = "العام التدريبي لسنه" + " - " + Convert.ToDateTime(item.Time.Year).Year + "م",

                AirLineName = item.TowerData.AirLine.Name,
                FlightNo = item.TowerData.AirLinesFlightTrip.FlightNo,
                AircraftRegName = item.TowerData.AircraftRegistration.Registration,
                AirportIdFrom = item.TowerData.AirPort1.Code,
                AirportIdTo = item.TowerData.AirPort.Code,
                Date = Convert.ToDateTime(item.Date).ToString("MM/dd/yyyy"),
                ATA = item.TowerData.ATA.ToString(),
                ATD = item.TowerData.ATD.ToString(),
                POB = item.TowerData.POB,
                QBD = item.TowerData.QBD,
                LandingPermission = item.TowerData.LandingPermission,
                LandingDate = CommonUsed.CheckDate(item.TowerData.LandingDate.ToString()),
                TakeOffDate = CommonUsed.CheckDate(item.TowerData.TakeOffDate.ToString()),

                CrewNO = item.CrewNO,
                 HasNightStop = CommonUsed.CheckYesNo( item.HasNightStop),
                HandlingAgentName = item.HandlingAgentsCompany.NameEn,
                TRNYCUPPS = item.TRNYCUPPS,
                TRNY = item.TRNY,
                TRNone = item.TRNone,
                EMB =item.EMB,
                EmbInft = item.EmbInft,
                 //= item.Pax,
               // DisEmbInft = item.DisEmbInft,

               OfficerDataCreatedBy = CommonUsed.GetUserName(item.CreatedBy),
               OfficerDataCreationDate = CommonUsed.CheckDate( item.CreationDate.ToString()),
               OfficerDataUpdatedBy = CommonUsed.GetUserName(item.UpdatedBy),
               OfficerDataUpdatingDate = CommonUsed.CheckDate(item.UpdatingDate.ToString()),
            };

            return View(OfficerDataViewModelList);
        }



   






      





















































































        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    OfficerData officerData = db.OfficerDatas.Find(id);
        //    if (officerData == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(officerData);
        //}

        // GET: OfficerDatas/Create
        public ActionResult Create()
        {
            ViewBag.AireLineAgentId = new SelectList(db.AirLineAgents, "Id", "NameAr");
            ViewBag.CompanyInfoId = new SelectList(db.CompanyInfoes, "Id", "NameAr");
            ViewBag.HandlingAgentId = new SelectList(db.HandlingAgentsCompanies, "Id", "NameAr");
            ViewBag.ServiceCarReqId = new SelectList(db.ServiceCarCompanies, "Id", "NameAr");
            ViewBag.TowerDataId = new SelectList(db.TowerDatas, "Id", "LandingPermission");
            return View();
        }

        // POST: OfficerDatas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,TowerDataId,CrewNO,HasNightStop,HandlingAgentId,TRNYCUPPS,TRNY,TRNone,EMB,EmbInft,DISEMB,DisEmbInft,Date,FrightLoadingExp,FrightLoadingImp,NormalMailLoadingExp,RapidMailLoadingExp,NormalMailLoadingImp,RapidMailLoadingImp,AmbulanceCarCount,FireFightingCarCount,ServiceCarReqId,IsCurrentAccount,AireLineAgentId,CompanyInfoId,CreatedBy,CreationDate,UpdatedBy,UpdatingDate")] OfficerData officerData)
        {
            if (ModelState.IsValid)
            {
                db.OfficerDatas.Add(officerData);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AireLineAgentId = new SelectList(db.AirLineAgents, "Id", "NameAr", officerData.AireLineAgentId);
            ViewBag.CompanyInfoId = new SelectList(db.CompanyInfoes, "Id", "NameAr", officerData.CompanyInfoId);
            ViewBag.HandlingAgentId = new SelectList(db.HandlingAgentsCompanies, "Id", "NameAr", officerData.HandlingAgentId);
            ViewBag.ServiceCarReqId = new SelectList(db.ServiceCarCompanies, "Id", "NameAr", officerData.ServiceCarReqId);
            ViewBag.TowerDataId = new SelectList(db.TowerDatas, "Id", "LandingPermission", officerData.TowerDataId);
            return View(officerData);
        }

        // GET: OfficerDatas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OfficerData officerData = db.OfficerDatas.Find(id);
            if (officerData == null)
            {
                return HttpNotFound();
            }
            ViewBag.AireLineAgentId = new SelectList(db.AirLineAgents, "Id", "NameAr", officerData.AireLineAgentId);
            ViewBag.CompanyInfoId = new SelectList(db.CompanyInfoes, "Id", "NameAr", officerData.CompanyInfoId);
            ViewBag.HandlingAgentId = new SelectList(db.HandlingAgentsCompanies, "Id", "NameAr", officerData.HandlingAgentId);
            ViewBag.ServiceCarReqId = new SelectList(db.ServiceCarCompanies, "Id", "NameAr", officerData.ServiceCarReqId);
            ViewBag.TowerDataId = new SelectList(db.TowerDatas, "Id", "LandingPermission", officerData.TowerDataId);
            return View(officerData);
        }

        // POST: OfficerDatas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,TowerDataId,CrewNO,HasNightStop,HandlingAgentId,TRNYCUPPS,TRNY,TRNone,EMB,EmbInft,DISEMB,DisEmbInft,Date,FrightLoadingExp,FrightLoadingImp,NormalMailLoadingExp,RapidMailLoadingExp,NormalMailLoadingImp,RapidMailLoadingImp,AmbulanceCarCount,FireFightingCarCount,ServiceCarReqId,IsCurrentAccount,AireLineAgentId,CompanyInfoId,CreatedBy,CreationDate,UpdatedBy,UpdatingDate")] OfficerData officerData)
        {
            if (ModelState.IsValid)
            {
                db.Entry(officerData).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AireLineAgentId = new SelectList(db.AirLineAgents, "Id", "NameAr", officerData.AireLineAgentId);
            ViewBag.CompanyInfoId = new SelectList(db.CompanyInfoes, "Id", "NameAr", officerData.CompanyInfoId);
            ViewBag.HandlingAgentId = new SelectList(db.HandlingAgentsCompanies, "Id", "NameAr", officerData.HandlingAgentId);
            ViewBag.ServiceCarReqId = new SelectList(db.ServiceCarCompanies, "Id", "NameAr", officerData.ServiceCarReqId);
            ViewBag.TowerDataId = new SelectList(db.TowerDatas, "Id", "LandingPermission", officerData.TowerDataId);
            return View(officerData);
        }

        // GET: OfficerDatas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OfficerData OfficerData = db.OfficerDatas.Find(id);
            if (OfficerData == null)
            {
              return Json(new { success = true, message = "AlreadyDeleted" }, JsonRequestBehavior.AllowGet);
            }

            db.TowerDatas.Where(x => x.Id == OfficerData.TowerDataId).Single().Status = 1;

            //if (db.OfficerDatas.Any(x => x.Id == id))
            //{
            //    return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
            //}

            var OfficerDataDelete = db.OfficerDatas.Find(id);
            db.OfficerDatas.Remove(OfficerDataDelete);
            db.SaveChanges();
            return Json(new { success = true, message = "Delete" }, JsonRequestBehavior.AllowGet);
        }


        // POST: OfficerDatas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OfficerData officerData = db.OfficerDatas.Find(id);
            db.OfficerDatas.Remove(officerData);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
