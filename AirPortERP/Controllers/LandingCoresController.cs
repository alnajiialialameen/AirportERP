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
    public class LandingCoresController : Controller
    {
        CommonUsed CommonUsed = new CommonUsed();
        private Entities db = new Entities();

        // GET: LandingCores

        public ActionResult LandingCore()
        {
            return View();
        }


        public ActionResult GetAllLandingCore()
        {
            var id = User.Identity.GetUserId();
           // var companyInfo = Role.GetCompanyId(id);
            IQueryable<LandingCore> Revenue = db.LandingCores.Where(x =>
             x.FlightDate.Value.Month == 9 || x.FlightDate.Value.Month == 8 || x.FlightDate.Value.Month == 7);


            //  List<LandingCore> Revenue = db.LandingCores.Take(12100).ToList();


            //List<LandingCore> Revenue = db.LandingCores.OrderBy(x => x.Id).ToList().Where(x => x.FlightNo != "-"

            //&& x.FlightNo!=null && x.CompanyCode!=null && x.RegNo!= null &&  x.FlightType != null && x.PlaneSizeID != null

            //&&  x.FlightDate.Value.Month == 9 ||  x.FlightDate.Value.Month == 8 || x.FlightDate.Value.Month == 7
            //&& x.FlightDate.Value.Month == 6).ToList();



            var TowerDatasList = new List<LandingCoreViewModel>();

            // var TowerDatasList = new List<TowerDataViewModels>();
            foreach (var item in Revenue)
            {
                LandingCoreViewModel tdm = new LandingCoreViewModel();

                tdm.Id = item.Id;

                tdm.CompanyCode = item?.CompanyCode;
                
                tdm.FlightNo = item?.FlightNo;
                tdm.RegNo = item?.RegNo;
                tdm.PlaneSizeID = item?.PlaneSizeID;


                tdm.Wt = item?.Wt.ToString();

                tdm.FlightFrom = item?.FlightFrom.ToString();
                tdm.FlightTo = item?.FlightTo.ToString();

                tdm.FlightType = item?.FlightType.ToString();
                tdm.FlightDate = CommonUsed.CheckDate(item.FlightDate.ToString());

                //tdm.TripType = CommonUsed.GetFlightTypeName(item.OfficerData.TowerData.FlightTypeL, item.OfficerData.TowerData.FlightTypeD);
                //tdm.FlightNo = item?.OfficerData.TowerData.AirLinesFlightTrip.FlightNo;
                //tdm.AircraftRegName = item?.OfficerData.TowerData.AircraftRegistration.Registration;
                //tdm.Type = item?.OfficerData.TowerData.AircraftRegistration.AircraftType.Type;
                //tdm.MaxTaKeOffWeight = item?.OfficerData.TowerData.AircraftRegistration.MaxTakoffWieght.ToString();

                //tdm.AirportIdFrom = CommonUsed.CheckAirportCode(Convert.ToInt32(item?.OfficerData.TowerData.AirportIdFrom));
                //tdm.AirportIdTo = CommonUsed.CheckAirportCode(Convert.ToInt32(item?.OfficerData.TowerData.AirportIdTo));


                //tdm.Date = Convert.ToDateTime(item.OfficerData.TowerData.Date).ToString("MM/dd/yyyy");
                //tdm.ATA = item.OfficerData.TowerData.ATA.ToString();
                //tdm.ATD = item.OfficerData.TowerData.ATD.ToString();
                ////tdm.POB = item.POB;
                ////tdm.QBD = item.QBD;
                //tdm.LandingPermission = item.OfficerData.TowerData.LandingPermission;


                //tdm.CreatedBy = CommonUsed.GetUserName(item.OfficerData.TowerData.CreatedBy);
                //tdm.CreationDate = item.OfficerData.TowerData.CreationDate.ToString();
                //tdm.UpdatedBy = CommonUsed.GetUserName(item.OfficerData.TowerData.UpdatedBy);
                //tdm.UpdatingDate = item.OfficerData.TowerData.ToString();

                //tdm.LandingDate = CommonUsed.CheckDate(item.OfficerData.TowerData.LandingDate.ToString());
                //tdm.TakeOffDate = CommonUsed.CheckDate(item.OfficerData.TowerData.TakeOffDate.ToString());

                /////////
                //tdm.Size = item?.OfficerData.TowerData.AircraftRegistration.AircraftType.AircraftSize.Size.ToString();
                //tdm.LandingFees = item.LandingFees.ToString();

                //tdm.ParkingFees = item.ParkingFees.ToString();
                //tdm.NightOperationFees = item.NightOperationFees.ToString();
                //tdm.NavigationFees = item.NavigationFees.ToString();
                //tdm.NoiseFees = item.NoiseFees.ToString();
                //tdm.SecurityFees = item.SecurityFees.ToString();

                TowerDatasList.Add(tdm);
            }


            return Json(new { data = TowerDatasList.ToList() }, JsonRequestBehavior.AllowGet);
        }

















        public ActionResult Index()
        {
            return View(db.LandingCores.ToList());
        }

        // GET: LandingCores/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LandingCore landingCore = db.LandingCores.Find(id);
            if (landingCore == null)
            {
                return HttpNotFound();
            }
            return View(landingCore);
        }

        // GET: LandingCores/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LandingCores/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FlightNo,FlightDate,CompanyCode,FlightType,RegNo,PlaneSizeID,Wt,ATA,ATA2,ATD,ATD2,FlightFrom,FlightTo,Freight_Out,Freight_IN,Mail_IN,Mail_Out,ExpMainIN,ExpMailOut,Cargo,Posted,NightSurgarche,TrainingOp,UpdateTime,UserID,DolarRate,UnitsParking,Landing_Fees,Parking_Fees,NightSurgarcge_Fees,UserCharge_Fees,Security_Fees,Freight_Fees,FirstClass_Fees,PushBack_Fees,ExpressMail_Fees,Mail_Fees,HandlingCode,NoUserCharge,PushBack,NoiceFees,Scheduled")] LandingCore landingCore)
        {
            if (ModelState.IsValid)
            {
                db.LandingCores.Add(landingCore);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(landingCore);
        }

        // GET: LandingCores/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LandingCore landingCore = db.LandingCores.Find(id);
            if (landingCore == null)
            {
                return HttpNotFound();
            }
            return View(landingCore);
        }

        // POST: LandingCores/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FlightNo,FlightDate,CompanyCode,FlightType,RegNo,PlaneSizeID,Wt,ATA,ATA2,ATD,ATD2,FlightFrom,FlightTo,Freight_Out,Freight_IN,Mail_IN,Mail_Out,ExpMainIN,ExpMailOut,Cargo,Posted,NightSurgarche,TrainingOp,UpdateTime,UserID,DolarRate,UnitsParking,Landing_Fees,Parking_Fees,NightSurgarcge_Fees,UserCharge_Fees,Security_Fees,Freight_Fees,FirstClass_Fees,PushBack_Fees,ExpressMail_Fees,Mail_Fees,HandlingCode,NoUserCharge,PushBack,NoiceFees,Scheduled")] LandingCore landingCore)
        {
            if (ModelState.IsValid)
            {
                db.Entry(landingCore).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(landingCore);
        }

        // GET: LandingCores/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LandingCore landingCore = db.LandingCores.Find(id);
            if (landingCore == null)
            {
                return HttpNotFound();
            }
            return View(landingCore);
        }

        // POST: LandingCores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LandingCore landingCore = db.LandingCores.Find(id);
            db.LandingCores.Remove(landingCore);
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
