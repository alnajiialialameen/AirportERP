using AirPortERP.Extenstion;
using AirPortERP.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AirPortERP.Models;
namespace AirPortERP.Controllers
{
    public class RevenueAdditionController : Controller
    {
        Entities db = new Entities();
        CommonUsed CommonUsed = new CommonUsed();
        // GET: RevenueAddition
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Revenue Revenue = db.Revenues.Where(x => x.Id == id).FirstOrDefault();
            if (Revenue == null)
            {
                return HttpNotFound();
            }


            var item = db.Revenues.Single(x => x.Id == id);

            RevenueDataViewModels tdm = new RevenueDataViewModels();

            tdm.Id = item.Id;
            tdm.AirLineCode = item?.OfficerData.TowerData.AirLine.Code;
            tdm.AirLineName = item?.OfficerData.TowerData.AirLine.Name;
            tdm.FlightNo = item?.OfficerData.TowerData.AirLinesFlightTrip.FlightNo;
            tdm.AircraftRegName = item?.OfficerData.TowerData.AircraftRegistration.Registration;
            tdm.Type = item?.OfficerData.TowerData.AircraftRegistration.AircraftType.Type;

            tdm.AirportIdFrom = CommonUsed.CheckAirportCode(Convert.ToInt32(item?.OfficerData.TowerData.AirportIdFrom));
            tdm.AirportIdTo = CommonUsed.CheckAirportCode(Convert.ToInt32(item?.OfficerData.TowerData.AirportIdTo));
            tdm.Date = Convert.ToDateTime(item.OfficerData.TowerData.Date).ToString("MM/dd/yyyy");
            tdm.ATA = item.OfficerData.TowerData.ATA.ToString();
            tdm.ATD = item.OfficerData.TowerData.ATD.ToString();
            tdm.POB = item.OfficerData.TowerData.POB;
            tdm.QBD = item.OfficerData.TowerData.QBD;
            tdm.LandingPermission = item.OfficerData.TowerData.LandingPermission;

            tdm.CountryFrom = CommonUsed.CheckCountryCode(Convert.ToInt32(item.OfficerData.TowerData.AirportIdFrom));
            tdm.CountryTo = CommonUsed.CheckCountryCode(Convert.ToInt32(item.OfficerData.TowerData.AirportIdTo));

            tdm.CreatedBy = CommonUsed.GetUserName(item.CreatedBy);
            tdm.CreationDate = item.CreationDate.ToString();
            tdm.UpdatedBy = CommonUsed.GetUserName(item.UpdatedBy);
            tdm.UpdatingDate = item.UpdatingDate.ToString();

            tdm.LandingDate = CommonUsed.CheckDate(item.OfficerData.TowerData.LandingDate.ToString());
            tdm.TakeOffDate = CommonUsed.CheckDate(item.OfficerData.TowerData.TakeOffDate.ToString());
            tdm.FlightType = CommonUsed.GetFlightTypeName(item.OfficerData.TowerData.FlightTypeL, item.OfficerData.TowerData.FlightTypeD);

          
           
              //landing
                tdm.LandingFees =item.LandingFees.ToString();
                tdm.ParkingFees = item.ParkingFees.ToString();
                tdm.NightOperationFees = item.NightOperationFees.ToString();

                tdm.NavigationFees = item.NavigationFees.ToString();
                tdm.NoiseFees = item.NoiseFees.ToString();
                tdm.SecurityFees = item.SecurityFees.ToString();


            //CarService
            tdm.AmbolanceFees =Convert.ToDecimal(item.AmbolanceFees);
            tdm.FireFightingFees = Convert.ToDecimal(item.FireFightingFees);

            tdm.AmbolanceCount =Convert.ToInt32( item.OfficerData.AmbulanceCarCount);
            tdm.FireFightingCount = Convert.ToInt32(item.OfficerData.FireFightingCarCount);



            //CarService
            tdm.NightOperationFees = item.NightOperationFees.ToString();

            tdm.NavigationFees = item.NavigationFees.ToString();
            tdm.NoiseFees = item.NoiseFees.ToString();
            tdm.SecurityFees = item.SecurityFees.ToString();

            //NY&CUPPS
            tdm.InterCUPPSFees = Convert.ToDecimal(item.InterCUPPSFees);
            tdm.DomeCUPPSFees = Convert.ToDecimal(item.DomeCUPPSFees);
            tdm.InterNYFees = Convert.ToDecimal(item.InterNYFees);
            tdm.DomeNYFees = Convert.ToDecimal(item.DomeNYFees);
             tdm.Pax = Convert.ToInt32(item.OfficerData.Pax);
            // tdm.InterCUPPSCount = Convert.ToInt32(item.OfficerData.DISEMB+(item.OfficerData.DisEmbInft)+(item.OfficerData.EMB)+(item.OfficerData.TRNYCUPPS));
            /// tdm.DomeCUPPSCount = Convert.ToInt32(item.OfficerData.DISEMB + (item.OfficerData.DisEmbInft) + (item.OfficerData.EMB) + (item.OfficerData.TRNYCUPPS));
            //  tdm.InterNYCount = Convert.ToInt32(item.OfficerData.DISEMB + (item.OfficerData.DisEmbInft) + (item.OfficerData.EMB) + (item.OfficerData.TRNY));
            //  tdm.DomeNYCount = Convert.ToInt32(item.OfficerData.DISEMB + (item.OfficerData.DisEmbInft) + (item.OfficerData.EMB) + (item.OfficerData.TRNY));
            //FirstClass
            tdm.FirstClassFees = Convert.ToDecimal(item.FirstClassFees);
            tdm.FirstClassCount = Convert.ToInt32(item.OfficerData.FirstClassCount);

            //MailAndFreight
            //////////////////////////Freight////////////////////////////////////
            tdm.FreightFees = Convert.ToDecimal(item.FreightFees);
            tdm.FreightImpCount = Convert.ToInt32(item.OfficerData.FreightLoadingImp);
            tdm.FreightExpCount = Convert.ToInt32(item.OfficerData.FreightLoadingExp);
            ///////////ImpMailFees///////////////////////
            tdm.ImpMailFees = Convert.ToDecimal(item.ImpMailFees);
            tdm.NormalImpMailCount = Convert.ToInt32(item.OfficerData.NormalMailLoadingImp);
            tdm.RapidMailImpCount = Convert.ToInt32(item.OfficerData.RapidMailLoadingImp);
            ///////////////////////////////////////////////////////////////////////
            ////////////////////ExpMailFees////////////////////////////////////
            tdm.ExpMailFees = Convert.ToDecimal(item.ExpMailFees);
            tdm.NormalMailExpCount = Convert.ToInt32(item.OfficerData.NormalMailLoadingExp);
            tdm.RapidMailExpCount = Convert.ToInt32(item.OfficerData.RapidMailLoadingExp);
            ////////////////////////////////////////////////////////////////////////////////

            //PushBackFees
            tdm.PushBackFees = Convert.ToDecimal(item.PushBackFees);
            tdm.PushBackCount = Convert.ToInt32(item.OfficerData.HasPushPack);

            return View(tdm);
        }



        public ActionResult DeleteNyAndCupps(int? id)
        {
            Revenue Revenue = db.Revenues.Find(id);
            if (Revenue == null) {
                return HttpNotFound();
            }

                OfficerData OfficerData = db.OfficerDatas.Where(x => x.Id == Revenue.OfficerDataId).FirstOrDefault();
            if (OfficerData == null)
            {
                return HttpNotFound();
            }
            //   TowerData TowerData = db.TowerDatas.Where(x => x.Id == OfficerData.TowerDataId).FirstOrDefault();

                    OfficerData.Pax = null;
                    OfficerData.PaxInter = null;
            //  OfficerData.DisEmbInft = null;
                   OfficerData.TRNY = null;
                    OfficerData.TRNYCUPPS = null;

                    Revenue.DomeCUPPSFees = null;
                    Revenue.DomeNYFees = null;
                    Revenue.InterCUPPSFees = null;
                    Revenue.InterNYFees = null;
                   /// Revenue.InterNYFees = null;



            db.Entry(OfficerData).State = System.Data.Entity.EntityState.Modified;
            db.Entry(Revenue).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return Json(new { success = true, message = "Delete" }, JsonRequestBehavior.AllowGet);

        }


        public ActionResult DeleteFirstClass(int? id)
        {
            Revenue Revenue = db.Revenues.Find(id);
            if (Revenue == null)
            {
                return HttpNotFound();
            }

            OfficerData OfficerData = db.OfficerDatas.Where(x => x.Id == Revenue.OfficerDataId).FirstOrDefault();
            if (OfficerData == null)
            {
                return HttpNotFound();
            }
          

            OfficerData.FirstClassCount = null;  
            Revenue.FirstClassFees = null;

            db.Entry(OfficerData).State = System.Data.Entity.EntityState.Modified;
            db.Entry(Revenue).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return Json(new { success = true, message = "Delete" }, JsonRequestBehavior.AllowGet);

        }



        public ActionResult DeleteMailAndFrieght(int? id)
        {
            Revenue Revenue = db.Revenues.Find(id);
            if (Revenue == null)
            {
                return HttpNotFound();
            }

            OfficerData OfficerData = db.OfficerDatas.Where(x => x.Id == Revenue.OfficerDataId).FirstOrDefault();
            if (OfficerData == null)
            {
                return HttpNotFound();
            }

            OfficerData.NormalMailLoadingExp = null;
            OfficerData.RapidMailLoadingExp = null;

            OfficerData.NormalMailLoadingImp = null;
            OfficerData.RapidMailLoadingImp = null;

            OfficerData.FreightLoadingExp = null;
            OfficerData.FreightLoadingImp = null;

            Revenue.FreightFees = null;
            Revenue.ExpMailFees = null;
            Revenue.ImpMailFees = null;

            db.Entry(OfficerData).State = System.Data.Entity.EntityState.Modified;
            db.Entry(Revenue).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return Json(new { success = true, message = "Delete" }, JsonRequestBehavior.AllowGet);

        }



        public ActionResult DeletePushPack(int? id)
        {
            Revenue Revenue = db.Revenues.Find(id);
            if (Revenue == null)
            {
                return HttpNotFound();
            }

            OfficerData OfficerData = db.OfficerDatas.Where(x => x.Id == Revenue.OfficerDataId).FirstOrDefault();
            if (OfficerData == null)
            {
                return HttpNotFound();
            }


            OfficerData.HasPushPack = null;
            Revenue.PushBackFees = null;

            db.Entry(OfficerData).State = System.Data.Entity.EntityState.Modified;
            db.Entry(Revenue).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return Json(new { success = true, message = "Delete" }, JsonRequestBehavior.AllowGet);

        }



        public ActionResult DeleteCarService(int? id)
        {
            Revenue Revenue = db.Revenues.Find(id);
            if (Revenue == null)
            {
                return HttpNotFound();
            }

            OfficerData OfficerData = db.OfficerDatas.Where(x => x.Id == Revenue.OfficerDataId).FirstOrDefault();
            if (OfficerData == null)
            {
                return HttpNotFound();
            }

            OfficerData.FireFightingCarCount = null;
            OfficerData.AmbulanceCarCount = null;

            Revenue.AmbolanceFees = null;
            Revenue.FireFightingFees = null;

            db.Entry(OfficerData).State = System.Data.Entity.EntityState.Modified;
            db.Entry(Revenue).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return Json(new { success = true, message = "Delete" }, JsonRequestBehavior.AllowGet);

        }




















    }
}