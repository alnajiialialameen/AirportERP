using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AirPortERP.Models;
using AirPortERP.Extenstion;
namespace AirPortERP.Controllers
{
     
    public class UpdaterController : Controller
    {
        Calculation Calculation = new Calculation();
        Entities db = new Entities();
        // GET: Updater
        public ActionResult UpdateInterNYFees()
        {
            return View();
        }
        
        public ActionResult UpdateInterNYFeesLocalCompany()
        {

            List<OfficerData> OfficerDataLocalCompany = db.OfficerDatas.Where(x => x.TowerData.AirLine.IsLocalCompany == true).ToList();



            foreach (OfficerData off in OfficerDataLocalCompany) {

                decimal Value = Calculation.InterNYFees(Convert.ToInt32(off.Pax), true);


                Revenue Revenue = db.Revenues.Where(x => x.OfficerDataId == off.Id).FirstOrDefault();
                Revenue.InterNYFees = Value;


                db.Entry(Revenue).State = System.Data.Entity.EntityState.Modified;
              
              

            }

            db.SaveChanges();


           
                return Json(new { success = true, message = "Success" }, JsonRequestBehavior.AllowGet);
          
              //  return Json(new { success = true, message = "Faild" }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult UpdateSecurityFees()
        {

            return View();
        }

        public ActionResult UpdateSecurityFeesLocalCompany()
        {

            List<OfficerData> OfficerDataLocalCompany = db.OfficerDatas.Where(x => x.TowerData.AirLine.IsLocalCompany == true).ToList();

            foreach (OfficerData off in OfficerDataLocalCompany)
            {

                decimal Value = Calculation.SecurityFees(Convert.ToDecimal(off.TowerData.AircraftRegistration.MaxTakoffWieght), Convert.ToInt32(off.TowerData.FlightTypeL), true);


                Revenue Revenue = db.Revenues.Where(x => x.OfficerDataId == off.Id).FirstOrDefault();
                Revenue.SecurityFees = Value;


                db.Entry(Revenue).State = System.Data.Entity.EntityState.Modified;



            }

            db.SaveChanges();


            return Json(new { success = true, message = "Success" }, JsonRequestBehavior.AllowGet);
        }



        public ActionResult Trible()
        {

            List<OfficerData> OfficerDataLocalCompany = db.OfficerDatas.Where(x => x.TowerData.AirLine.IsLocalCompany == true
            && x.TowerData.FlightTypeD == 2 && (x.Pax != null && x.TRNYCUPPS != null)).ToList();



            foreach (OfficerData off in OfficerDataLocalCompany)
            {

                decimal Value = Calculation.InterNYFees(Convert.ToInt32(off.Pax), true);
                decimal ValueC = Calculation.InterCUPPSFees(Convert.ToInt32(off.Pax), Convert.ToInt32(off.TRNYCUPPS), false);


                Revenue Revenue = db.Revenues.Where(x => x.OfficerDataId == off.Id).FirstOrDefault();
                Revenue.DomeNYFees = Value;
                Revenue.InterCUPPSFees = null;

                db.Entry(Revenue).State = System.Data.Entity.EntityState.Modified;



            }

            db.SaveChanges();



            return Json(new { success = true, message = "Success" }, JsonRequestBehavior.AllowGet);

            //  return Json(new { success = true, message = "Faild" }, JsonRequestBehavior.AllowGet);
        }


    }
}