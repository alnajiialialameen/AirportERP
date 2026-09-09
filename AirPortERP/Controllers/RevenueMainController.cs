using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AirPortERP.Models;
using AirPortERP.ViewModels;
using AirPortERP.Extenstion;
using Microsoft.AspNet.Identity;
using System.Data.Entity;

namespace AirPortERP.Controllers
{
    public class RevenueMainController : Controller
    {
        RoleController Role = new RoleController();
        Calculation Cal = new Calculation();
        Entities db = new Entities();
        CommonUsed CommonUsed = new CommonUsed();
        int WorkMouth;
        int WorkYear;
        // GET: RevenueMain


        [Authorize(Roles = "Cupps,Ny,Revenues,Super-Admin,Cupps&&Car,Ny&&Car, Account, Account-Admin")]
        public ActionResult NyAndCuppMain()
        {
            var id = User.Identity.GetUserId();

            bool HasMouth = CommonUsed.HasMouth(id);
            if (HasMouth)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "WorkOns");
            }
        }
        public ActionResult GetNyAndCupps()
        {
            var id = User.Identity.GetUserId();
            WorkMouth = CommonUsed.GetMouth(id);
            WorkYear = CommonUsed.GetYear(id);
            var companyId = CommonUsed.getCompanyId(id);
            bool IsSingle = CommonUsed.getIsSingleAccounting(companyId);


            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.Where(x => x.DomeNYFees == null && x.InterNYFees == null && x.DomeCUPPSFees == null && x.InterNYFees == null
             && x.OfficerData.TowerData.Date.Value.Month == WorkMouth && x.OfficerData.TowerData.Date.Value.Year == WorkYear
             && x.OfficerData.TowerData.CompanyInfoId == companyId);
            var RevenueList = new List<NyAndCuppsViewModelDto>();
            foreach (var item in Revenue)
            {
                NyAndCuppsViewModelDto N = new NyAndCuppsViewModelDto();

                N.Id = item.Id;
                N.TowerId = item.OfficerData.TowerData.Id;
                N.AirlineName = item?.OfficerData.TowerData.AirLine.Name;
                N.FlightNo = item?.OfficerData.TowerData.AirLinesFlightTrip.FlightNo;
                N.RegNo = item?.OfficerData.TowerData.AircraftRegistration.Registration;


                N.AirPortFrom = item?.OfficerData.TowerData.AirPort1.Code;
                N.AirPortTo = item?.OfficerData.TowerData.AirPort.Code;
                N.Date = CommonUsed.CheckDate(item?.OfficerData.TowerData.Date.ToString());
                N.TakeOffDate = CommonUsed.CheckDate(item?.OfficerData.TowerData.TakeOffDate.ToString());
                N.LandingDate = CommonUsed.CheckDate(item?.OfficerData.TowerData.LandingDate.ToString());

                //N.FlightType = CommonUsed.GetFlightTypeName(item?.OfficerData.TowerData.FlightTypeL, item?.OfficerData.TowerData.FlightTypeD);
                N.FlightType = CommonUsed.GetFlightTypeNameForCUPPSNY(item?.OfficerData.TowerData.FlightTypeD);

                N.Pax = Convert.ToInt32(item?.OfficerData.Pax);

                //  N.DisEmbInft = Convert.ToInt32(item?.OfficerData.DisEmbInft);

                //   N.TRNY = Convert.ToInt32(item?.OfficerData.TRNY);

                N.TRNYCUPPS = Convert.ToInt32(item?.OfficerData.TRNYCUPPS);
                // N.TRNYCUPPS = Convert.ToInt32(item?.OfficerData.TRNYCUPPS);


                N.InterNYFees = item?.InterNYFees.ToString();

                N.DomeNYFees = item?.DomeNYFees.ToString();

                N.InterCUPPSFees = item?.InterCUPPSFees.ToString();
                N.DomeCUPPSFees = item?.DomeCUPPSFees.ToString(); ;

                N.Size = item?.OfficerData.TowerData.AircraftRegistration.AircraftType.AircraftSize.Size.ToString();
                N.MaxTakoffWieght = item?.OfficerData.TowerData.AircraftRegistration.MaxTakoffWieght.ToString();
                N.Type = item?.OfficerData.TowerData.AircraftRegistration.AircraftType.Type.ToString();

                N.ATA = item?.OfficerData.TowerData.ATA.ToString();
                N.ATD = item?.OfficerData.TowerData.ATD.ToString();
                N.AircraftRegName = item?.OfficerData.TowerData.AircraftRegistration.Registration.ToString();
                if (IsSingle)
                {
                    N.IsStop = CommonUsed.IsEntryDateinRange2(item.Id, Role.GetRole(id));  // check date in range in claim table
                }
                else
                {
                    N.IsStop = CommonUsed.IsEntryDateinRange(Convert.ToDateTime(item.OfficerData.TowerData.Date), Role.GetRole(id),
                  Convert.ToInt32(item.OfficerData.TowerData.AirLineId), Convert.ToInt32(companyId));  // check date in range in claim table
                }
                N.AirportName = CommonUsed.getCompanyName(item.OfficerData.TowerData.CompanyInfoId);

                RevenueList.Add(N);
            }
            
            return Json(new { data = RevenueList.ToList() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AllGetNyAndCupps()
        {
            var id = User.Identity.GetUserId();
            WorkMouth = CommonUsed.GetMouth(id);
            WorkYear = CommonUsed.GetYear(id);
           
            var companyId = CommonUsed.getCompanyId(id);
            bool IsSingle = CommonUsed.getIsSingleAccounting(companyId);
            IQueryable<Revenue> Revenue;
            //Revenue = db.Revenues.Where(x => (x.DomeNYFees != null || x.InterNYFees != null || x.DomeCUPPSFees != null || x.InterCUPPSFees != null)
            //                        && x.OfficerData.TowerData.Date.Value.Month == WorkMouth && x.OfficerData.TowerData.Date.Value.Year == WorkYear 
            //                        && x.OfficerData.TowerData.CompanyInfoId == companyId);

          

            Revenue = db.Revenues.Where(x =>
                (x.DomeNYFees != null ||
                 x.InterNYFees != null ||
                 x.DomeCUPPSFees != null ||
                 x.InterCUPPSFees != null)

                && x.OfficerData.TowerData.Date.HasValue
                && DbFunctions.TruncateTime(x.OfficerData.TowerData.Date).Value.Month == WorkMouth
                && DbFunctions.TruncateTime(x.OfficerData.TowerData.Date).Value.Year == WorkYear
                && x.OfficerData.TowerData.CompanyInfoId == companyId
            );


            var RevenueList = new List<NyAndCuppsViewModelDto>();
            foreach (var item in Revenue)
            {
                NyAndCuppsViewModelDto N = new NyAndCuppsViewModelDto();

                N.Id = item.Id;
                N.TowerId = item.OfficerData.TowerData.Id;
                N.AirlineName = item?.OfficerData.TowerData.AirLine.Name;
                N.FlightNo = item?.OfficerData.TowerData.AirLinesFlightTrip.FlightNo;
                N.RegNo = item?.OfficerData.TowerData.AircraftRegistration.Registration;


                N.AirPortFrom = item?.OfficerData.TowerData.AirPort1.Code;
                N.AirPortTo = item?.OfficerData.TowerData.AirPort.Code;

                // N.FlightType = CommonUsed.GetFlightTypeName(item?.OfficerData.TowerData.FlightTypeL, item?.OfficerData.TowerData.FlightTypeD);
                N.FlightType = CommonUsed.GetFlightTypeNameForCUPPSNY(item?.OfficerData.TowerData.FlightTypeD);
                N.Date = CommonUsed.CheckDate(item?.OfficerData.TowerData.Date.ToString());
                N.TakeOffDate = CommonUsed.CheckDate(item?.OfficerData.TowerData.TakeOffDate.ToString());
                N.LandingDate = CommonUsed.CheckDate(item?.OfficerData.TowerData.LandingDate.ToString());

                if (item.OfficerData.PaxInter > 0)
                {
                    N.Pax2 = Convert.ToInt32(item?.OfficerData.Pax).ToString() + "+" + Convert.ToInt32(item?.OfficerData.PaxInter).ToString();
                }
                else { N.Pax2 = (Convert.ToInt32(item?.OfficerData.Pax) + Convert.ToInt32(item?.OfficerData.PaxInter)).ToString(); }
               

                //N.Pax = Convert.ToInt32(item?.OfficerData.Pax) + Convert.ToInt32(item?.OfficerData.PaxInter);


                // N.DisEmbInft = Convert.ToInt32(item?.OfficerData.DisEmbInft);

                N.TRNY = Convert.ToInt32(item?.OfficerData.TRNY);

                N.TRNYCUPPS = Convert.ToInt32(item?.OfficerData.TRNYCUPPS);
                // N.TRNYCUPPS = Convert.ToInt32(item?.OfficerData.TRNYCUPPS);


                N.InterNYFees = item?.InterNYFees.ToString();

                N.DomeNYFees = item?.DomeNYFees.ToString();

                N.InterCUPPSFees = item?.InterCUPPSFees.ToString();
                N.DomeCUPPSFees = item?.DomeCUPPSFees.ToString(); ;



                N.Size = item?.OfficerData.TowerData.AircraftRegistration.AircraftType.AircraftSize.Size.ToString();
                N.MaxTakoffWieght = item?.OfficerData.TowerData.AircraftRegistration.MaxTakoffWieght.ToString();
                N.Type = item?.OfficerData.TowerData.AircraftRegistration.AircraftType.Type.ToString();

                N.ATA = item?.OfficerData.TowerData.ATA.ToString();
                N.ATD = item?.OfficerData.TowerData.ATD.ToString();
                N.AircraftRegName = item?.OfficerData.TowerData.AircraftRegistration.Registration.ToString();
                if (IsSingle)
                {
                    N.IsStop = CommonUsed.IsEntryDateinRange2(item.Id, Role.GetRole(id));  // check date in range in claim table
                }
                else
                {
                    N.IsStop = CommonUsed.IsEntryDateinRange(Convert.ToDateTime(item.OfficerData.TowerData.Date), Role.GetRole(id),
                  Convert.ToInt32(item.OfficerData.TowerData.AirLineId), Convert.ToInt32(companyId));  // check date in range in claim table
                }
                N.AirportName = CommonUsed.getCompanyName(item.OfficerData.TowerData.CompanyInfoId);




                RevenueList.Add(N);

            }










            return Json(new { data = RevenueList.OrderBy(x => x.Id).ToList() }, JsonRequestBehavior.AllowGet);
        }



        [Authorize(Roles = "Cupps,Ny,Revenues,Super-Admin,Cupps&&Car,Ny&&Car, Account, Account-Admin")]
        public ActionResult AllNyAndCuppMain()
        {
            var id = User.Identity.GetUserId();

            if (db.WorkOns.Any(x => x.UserId == id))
            {
                WorkMouth = Convert.ToInt32(db.WorkOns.Where(x => x.UserId == id).Single().MonthWorkOn);
            }
            if (WorkMouth > 0)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "WorkOns");
            }
        }


        public ActionResult GetNyAndCuppsfeesParial(int? id)
        {
            Revenue Revenue = new Revenue();
            NyAndCuppsFeesViewModelDTO NyFeesViewModelDTO = new NyAndCuppsFeesViewModelDTO();
            if (id > 0)
            {
                Revenue = db.Revenues.Find(id);
            }
            if (Revenue != null)
            {

                OfficerData OfficerData = db.OfficerDatas.Where(x => x.Id == Revenue.OfficerDataId).FirstOrDefault();


                NyFeesViewModelDTO.IsNotUsedCUPPS = Convert.ToBoolean(OfficerData.IsNotUsedCUPPS);
                NyFeesViewModelDTO.FlightTypeD = Convert.ToInt32(OfficerData.TowerData.FlightTypeD);
                NyFeesViewModelDTO.Pax = Convert.ToInt32(OfficerData.Pax);
                NyFeesViewModelDTO.PaxInter = Convert.ToInt32(OfficerData.PaxInter);

                NyFeesViewModelDTO.TRNYCUPPS = Convert.ToInt32(OfficerData.TRNYCUPPS);
                NyFeesViewModelDTO.Id = Convert.ToInt32(id);
                NyFeesViewModelDTO.OfficerDataId = OfficerData.Id;


            }


            return PartialView("_AddEditNyFees", NyFeesViewModelDTO);
        }


        public ActionResult UpdateNyAndCuppsfees(NyAndCuppsFeesViewModelDTO model)
        {
            var id = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(id);
            if (ModelState.IsValid)
            {
              
                bool IsLocalCompany = Convert.ToBoolean(db.OfficerDatas.FirstOrDefault(x => x.Id == model.OfficerDataId).TowerData.AirLine.IsLocalCompany);
                if (model.Id > 0)
                {

                    OfficerData OfficerData = db.OfficerDatas.Where(x => x.Id == model.OfficerDataId).FirstOrDefault();
                    ///  db.Revenues.Where(x => x.Id == model.Id).FirstOrDefault();
                    //Revenue Revenue = new Revenue();

                    OfficerData.Pax = model.Pax;
                    //  OfficerData.DisEmbInft = model.DisEmbInft;
                    OfficerData.TRNYCUPPS = model.TRNYCUPPS;
                    OfficerData.IsNotUsedCUPPS = model.IsNotUsedCUPPS;
                    OfficerData.PaxInter = model.PaxInter;
                    Revenue RevObject = db.Revenues.FirstOrDefault(x => x.Id == model.Id);

                    TowerData TowerData = db.TowerDatas.Where(x => x.Id == OfficerData.TowerDataId).FirstOrDefault();
                    if (TowerData.FlightTypeD == 2/* && TowerData.FlightTypeL == 2*/)
                    {
                        //Domestic Cupps
                        decimal DomeNYFees = Cal.DomeNYFees(Convert.ToInt32(OfficerData.Pax), IsLocalCompany);

                        RevObject.DomeNYFees = DomeNYFees;

                        decimal DomeCUPPSFees = Cal.DomeCUPPSFees(Convert.ToInt32(OfficerData.Pax), Convert.ToInt32(OfficerData.TRNYCUPPS), model.IsNotUsedCUPPS);

                        RevObject.DomeCUPPSFees = DomeCUPPSFees;
                        if (model.PaxInter > 0)
                        {
                            decimal InterCUPPSFees = Cal.InterCUPPSFees(Convert.ToInt32(model.PaxInter), 0, model.IsNotUsedCUPPS);
                            decimal InterNYFees = Cal.InterNYFees(Convert.ToInt32(model.PaxInter), IsLocalCompany, companyId);
                            RevObject.InterCUPPSFees = InterCUPPSFees;
                            RevObject.InterNYFees = InterNYFees;

                        }
                        else
                        {
                            RevObject.InterCUPPSFees = 0;
                            RevObject.InterNYFees = 0;
                        }


                        /// Zeros Inter
                       
                    }
                    else
                    {
                        //international Cupps

                        decimal InterCUPPSFees = Cal.InterCUPPSFees(Convert.ToInt32(OfficerData.Pax), Convert.ToInt32(OfficerData.TRNYCUPPS), model.IsNotUsedCUPPS);

                        RevObject.InterCUPPSFees = InterCUPPSFees;


                        decimal InterNYFees = Cal.InterNYFees(Convert.ToInt32(OfficerData.Pax), IsLocalCompany, companyId);

                        RevObject.InterNYFees = InterNYFees;
                        /// Zeros Inter
                        RevObject.DomeCUPPSFees = 0;
                        RevObject.DomeNYFees = 0;
                    }


                    db.Entry(RevObject).State = EntityState.Modified;
                    db.Entry(OfficerData).State = EntityState.Modified;
                    //   cal.LandingFees(model.Id);
                    db.SaveChanges();



                    return Json(new { success = true, message = "Update" }, JsonRequestBehavior.AllowGet);
                }
                else
                {


                    //db.TowerDatas.Add(model);
                    //db.SaveChanges();


                    return Json(new { success = true, message = "Add" }, JsonRequestBehavior.AllowGet);
                }

            }
            return Json(new { success = false, message = "error" }, JsonRequestBehavior.AllowGet);
        }





        ///////////////////////////////////////////////////////////////////////////////////////////////////

        [Authorize(Roles = "FirstClass,Revenues,Super-Admin, Account, Account-Admin, Account, Account-Admin")]
        public ActionResult FirstClassMain()
        {
            var id = User.Identity.GetUserId();

            if (db.WorkOns.Any(x => x.UserId == id))
            {
                WorkMouth = Convert.ToInt32(db.WorkOns.Where(x => x.UserId == id).Single().MonthWorkOn);
            }
            if (WorkMouth > 0)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "WorkOns");
            }
        }

        [Authorize(Roles = "FirstClass,Revenues,Super-Admin, Account, Account-Admin, Account, Account-Admin")]
        public ActionResult AllFirstClassMain()
        {
            var id = User.Identity.GetUserId();

            if (db.WorkOns.Any(x => x.UserId == id))
            {
                WorkMouth = Convert.ToInt32(db.WorkOns.Where(x => x.UserId == id).Single().MonthWorkOn);
            }
            if (WorkMouth > 0)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "WorkOns");
            }
        }
        public ActionResult GetFirstClass()


        {
            var id = User.Identity.GetUserId();
            WorkMouth = CommonUsed.GetMouth(id);
            WorkYear = CommonUsed.GetYear(id);
            var companyId = CommonUsed.getCompanyId(id);
            bool IsSingle = CommonUsed.getIsSingleAccounting(companyId);

            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.Where(x => x.FirstClassFees == null
             && x.OfficerData.TowerData.Date.Value.Month == WorkMouth && x.OfficerData.TowerData.Date.Value.Year == WorkYear
             && x.OfficerData.TowerData.CompanyInfoId == companyId);
            //Revenue = db.Revenues;
            var RevenueList = new List<FirstClassViewModelDto>();
            foreach (var item in Revenue)
            {
                FirstClassViewModelDto N = new FirstClassViewModelDto();

                N.Id = item.Id;
                N.TowerId = item.OfficerData.TowerData.Id;
                N.AirlineName = item?.OfficerData.TowerData.AirLine.Name;
                N.FlightNo = item?.OfficerData.TowerData.AirLinesFlightTrip.FlightNo;
                N.RegNo = item?.OfficerData.TowerData.AircraftRegistration.Registration;


                N.AirPortFrom = item?.OfficerData.TowerData.AirPort1.Code;
                N.AirPortTo = item?.OfficerData.TowerData.AirPort.Code;

                N.TakeOffDate = CommonUsed.CheckDate(item?.OfficerData.TowerData.TakeOffDate.ToString());
                N.LandingDate = CommonUsed.CheckDate(item?.OfficerData.TowerData.LandingDate.ToString());

                N.FirstClassCount = Convert.ToInt32(item?.OfficerData.FirstClassCount);
                N.TripType = CommonUsed.GetFlightTypeName(item.OfficerData.TowerData.FlightTypeL, item.OfficerData.TowerData.FlightTypeD);
                N.Date = CommonUsed.CheckDate(item?.OfficerData.TowerData.Date.ToString());

                N.FirstClassFees = item?.FirstClassFees.ToString();

                N.Size = item?.OfficerData.TowerData.AircraftRegistration.AircraftType.AircraftSize.Size.ToString();
                N.MaxTakoffWieght = item?.OfficerData.TowerData.AircraftRegistration.MaxTakoffWieght.ToString();
                N.Type = item?.OfficerData.TowerData.AircraftRegistration.AircraftType.Type.ToString();

                N.ATA = item?.OfficerData.TowerData.ATA.ToString();
                N.ATD = item?.OfficerData.TowerData.ATD.ToString();
                N.AircraftRegName = item?.OfficerData.TowerData.AircraftRegistration.Registration.ToString();
                if (IsSingle)
                {
                    N.IsStop = CommonUsed.IsEntryDateinRange2(item.Id, Role.GetRole(id));  // check date in range in claim table
                }
                else
                {
                    N.IsStop = CommonUsed.IsEntryDateinRange(Convert.ToDateTime(item.OfficerData.TowerData.Date), Role.GetRole(id),
                  Convert.ToInt32(item.OfficerData.TowerData.AirLineId), Convert.ToInt32(companyId));  // check date in range in claim table
                }
                N.AirportName = CommonUsed.getCompanyName(item.OfficerData.TowerData.CompanyInfoId);



                RevenueList.Add(N);
            }

            return Json(new { data = RevenueList.ToList() }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AllGetFirstClass()
        {
            var id = User.Identity.GetUserId();
            WorkMouth = CommonUsed.GetMouth(id);
            WorkYear = CommonUsed.GetYear(id);
            var companyId = CommonUsed.getCompanyId(id);
            bool IsSingle = CommonUsed.getIsSingleAccounting(companyId);

            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.Where(x => x.FirstClassFees != null
            && x.OfficerData.TowerData.Date.Value.Month == WorkMouth && x.OfficerData.TowerData.Date.Value.Year == WorkYear
            && x.OfficerData.TowerData.CompanyInfoId == companyId);

            // Revenue = db.Revenues;
            var RevenueList = new List<FirstClassViewModelDto>();
            foreach (var item in Revenue)
            {
                FirstClassViewModelDto N = new FirstClassViewModelDto();

                N.Id = item.Id;
                N.TowerId = item.OfficerData.TowerData.Id;
                N.AirlineName = item?.OfficerData.TowerData.AirLine.Name;
                N.FlightNo = item?.OfficerData.TowerData.AirLinesFlightTrip.FlightNo;
                N.RegNo = item?.OfficerData.TowerData.AircraftRegistration.Registration;

                N.AirPortFrom = item?.OfficerData.TowerData.AirPort1.Code;
                N.AirPortTo = item?.OfficerData.TowerData.AirPort.Code;

                N.TakeOffDate = CommonUsed.CheckDate(item?.OfficerData.TowerData.TakeOffDate.ToString());
                N.LandingDate = CommonUsed.CheckDate(item?.OfficerData.TowerData.LandingDate.ToString());

                N.FirstClassCount = Convert.ToInt32(item?.OfficerData.FirstClassCount);

                N.Date = CommonUsed.CheckDate(item?.OfficerData.TowerData.Date.ToString());
                N.TripType = CommonUsed.GetFlightTypeName(item.OfficerData.TowerData.FlightTypeL, item.OfficerData.TowerData.FlightTypeD);
                N.FirstClassFees = item?.FirstClassFees.ToString();

                N.Size = item?.OfficerData.TowerData.AircraftRegistration.AircraftType.AircraftSize.Size.ToString();
                N.MaxTakoffWieght = item?.OfficerData.TowerData.AircraftRegistration.MaxTakoffWieght.ToString();
                N.Type = item?.OfficerData.TowerData.AircraftRegistration.AircraftType.Type.ToString();

                N.ATA = item?.OfficerData.TowerData.ATA.ToString();
                N.ATD = item?.OfficerData.TowerData.ATD.ToString();
                N.AircraftRegName = item?.OfficerData.TowerData.AircraftRegistration.Registration.ToString();
                if (IsSingle)
                {
                    N.IsStop = CommonUsed.IsEntryDateinRange2(item.Id, Role.GetRole(id));  // check date in range in claim table
                }
                else
                {
                    N.IsStop = CommonUsed.IsEntryDateinRange(Convert.ToDateTime(item.OfficerData.TowerData.Date), Role.GetRole(id),
                  Convert.ToInt32(item.OfficerData.TowerData.AirLineId), Convert.ToInt32(companyId));  // check date in range in claim table
                }
                N.AirportName = CommonUsed.getCompanyName(item.OfficerData.TowerData.CompanyInfoId);

                RevenueList.Add(N);
            }
            return Json(new { data = RevenueList.ToList() }, JsonRequestBehavior.AllowGet);
        }




        public ActionResult GetFirstClassParial(int? id)
        {
            Revenue Revenue = new Revenue();
            FirstClassFeesViewModelDTO FirstClassFeesViewModelDTO = new FirstClassFeesViewModelDTO();
            if (id > 0)
            {
                Revenue = db.Revenues.Find(id);
            }
            if (Revenue != null)
            {
                //if (Revenue.InterNYFees != null || Revenue.DomeNYFees != null)
                //{
                OfficerData OfficerData = db.OfficerDatas.Where(x => x.Id == Revenue.OfficerDataId).FirstOrDefault();




                FirstClassFeesViewModelDTO.FirstClassCount = Convert.ToInt32(OfficerData.FirstClassCount);

                FirstClassFeesViewModelDTO.Id = OfficerData.Id;
                FirstClassFeesViewModelDTO.OfficerDataId = OfficerData.Id;

                //}
            }


            return PartialView("_AddEditFirstClassFees", FirstClassFeesViewModelDTO);
        }


        public ActionResult UpdateFirstClassfees(FirstClassFeesViewModelDTO model)
        {
            if (ModelState.IsValid)
            {
                var id = User.Identity.GetUserId();

                if (model.Id > 0)
                {

                    OfficerData OfficerData = db.OfficerDatas.Where(x => x.Id == model.OfficerDataId).FirstOrDefault();
                    db.Revenues.Where(x => x.Id == model.Id).FirstOrDefault();
                    //Revenue Revenue = new Revenue();

                    OfficerData.FirstClassCount = model.FirstClassCount;
                    OfficerData.Id = model.OfficerDataId;
                    // OfficerData.TRNYCUPPS = model.TRNYCUPPS;
                    //  OfficerData.TRNY = model.TRNY;

                    TowerData TowerData = db.TowerDatas.Where(x => x.Id == OfficerData.TowerDataId).FirstOrDefault();

                    decimal value = Cal.FirstClassFees(Convert.ToInt32(OfficerData.FirstClassCount));

                    db.Revenues.SingleOrDefault(x => x.Id == model.Id).FirstClassFees = value;

                    db.Entry(OfficerData).State = System.Data.Entity.EntityState.Modified;
                    //   cal.LandingFees(model.Id);
                    db.SaveChanges();



                    return Json(new { success = true, message = "Update" }, JsonRequestBehavior.AllowGet);
                }
                else
                {


                    //db.TowerDatas.Add(model);
                    //db.SaveChanges();


                    return Json(new { success = true, message = "Add" }, JsonRequestBehavior.AllowGet);
                }

            }
            return Json(new { success = false, message = "error" }, JsonRequestBehavior.AllowGet);
        }




        [Authorize(Roles = "Car,Revenues,Super-Admin,Cupps&&Car,Ny&&Car, Account, Account-Admin")]
        public ActionResult CarSeriveMain()
        {
            var id = User.Identity.GetUserId();

            if (db.WorkOns.Any(x => x.UserId == id))
            {
                WorkMouth = Convert.ToInt32(db.WorkOns.Where(x => x.UserId == id).Single().MonthWorkOn);
            }
            if (WorkMouth > 0)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "WorkOns");
            }
        }


        [Authorize(Roles = "Car,Revenues,Super-Admin,Cupps&&Car,Ny&&Car, Account, Account-Admin")]
        public ActionResult AllCarSeriveMain()
        {
            var id = User.Identity.GetUserId();

            if (db.WorkOns.Any(x => x.UserId == id))
            {
                WorkMouth = Convert.ToInt32(db.WorkOns.Where(x => x.UserId == id).Single().MonthWorkOn);
            }
            if (WorkMouth > 0)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "WorkOns");
            }
        }


        public ActionResult GetCarService()
        {
            var id = User.Identity.GetUserId();
            WorkMouth = CommonUsed.GetMouth(id);
            WorkYear = CommonUsed.GetYear(id);
            var companyId = CommonUsed.getCompanyId(id);
            bool IsSingle = CommonUsed.getIsSingleAccounting(companyId);

            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.Where(x => x.AmbolanceFees == null && x.FireFightingFees == null
            && x.OfficerData.TowerData.Date.Value.Month == WorkMouth && x.OfficerData.TowerData.Date.Value.Year == WorkYear
            && x.OfficerData.TowerData.CompanyInfoId == companyId);
            var RevenueList = new List<CarServiceViewModelDto>();
            foreach (var item in Revenue)
            {
                CarServiceViewModelDto N = new CarServiceViewModelDto();

                N.Id = item.Id;
                N.TowerId = item.OfficerData.TowerData.Id;
                N.AirlineName = item?.OfficerData.TowerData.AirLine.Name;
                N.FlightNo = item?.OfficerData.TowerData.AirLinesFlightTrip.FlightNo;
                N.RegNo = item?.OfficerData.TowerData.AircraftRegistration.Registration;


                N.AirPortFrom = item?.OfficerData.TowerData.AirPort1.Code;
                N.AirPortTo = item?.OfficerData.TowerData.AirPort.Code;

                N.TakeOffDate = CommonUsed.CheckDate(item?.OfficerData.TowerData.TakeOffDate.ToString());
                N.LandingDate = CommonUsed.CheckDate(item?.OfficerData.TowerData.LandingDate.ToString());

                N.TripType = CommonUsed.GetFlightTypeName(item.OfficerData.TowerData.FlightTypeL, item.OfficerData.TowerData.FlightTypeD);
                N.Date = Convert.ToDateTime(item.OfficerData.TowerData.Date).ToString("MM/dd/yyyy");

                N.AmbulanceCarCount = Convert.ToInt32(item?.OfficerData.AmbulanceCarCount);
                N.FireFightingCarCount = Convert.ToInt32(item?.OfficerData.FireFightingCarCount);

                N.AmbulanceCarFees = Convert.ToInt32(item?.AmbolanceFees);
                N.FireFightingCarFees = Convert.ToInt32(item?.FireFightingFees);

                N.Size = item?.OfficerData.TowerData.AircraftRegistration.AircraftType.AircraftSize.Size.ToString();
                N.MaxTakoffWieght = item?.OfficerData.TowerData.AircraftRegistration.MaxTakoffWieght.ToString();
                N.Type = item?.OfficerData.TowerData.AircraftRegistration.AircraftType.Type.ToString();

                N.ATA = item?.OfficerData.TowerData.ATA.ToString();
                N.ATD = item?.OfficerData.TowerData.ATD.ToString();
                N.AircraftRegName = item?.OfficerData.TowerData.AircraftRegistration.Registration.ToString();
                if (IsSingle)
                {
                    N.IsStop = CommonUsed.IsEntryDateinRange2(item.Id, Role.GetRole(id));  // check date in range in claim table
                }
                else
                {
                    N.IsStop = CommonUsed.IsEntryDateinRange(Convert.ToDateTime(item.OfficerData.TowerData.Date), Role.GetRole(id),
                  Convert.ToInt32(item.OfficerData.TowerData.AirLineId), Convert.ToInt32(companyId));  // check date in range in claim table
                }
                N.AirportName = CommonUsed.getCompanyName(item.OfficerData.TowerData.CompanyInfoId);

                RevenueList.Add(N);

            }

            return Json(new { data = RevenueList.ToList() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AllGetCarService()
        {
            var id = User.Identity.GetUserId();
            WorkMouth = CommonUsed.GetMouth(id);
            WorkYear = CommonUsed.GetYear(id);
            var companyId = CommonUsed.getCompanyId(id);
            bool IsSingle = CommonUsed.getIsSingleAccounting(companyId);
            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.Where(x => x.AmbolanceFees != null && x.FireFightingFees != null
            && x.OfficerData.TowerData.Date.Value.Month == WorkMouth && x.OfficerData.TowerData.Date.Value.Year == WorkYear
            && x.OfficerData.TowerData.CompanyInfoId == companyId);

            // Revenue = db.Revenues;
            var RevenueList = new List<CarServiceViewModelDto>();
            foreach (var item in Revenue)
            {
                CarServiceViewModelDto N = new CarServiceViewModelDto();

                N.Id = item.Id;
                N.TowerId = item.OfficerData.TowerData.Id;
                N.AirlineName = item?.OfficerData.TowerData.AirLine.Name;
                N.FlightNo = item?.OfficerData.TowerData.AirLinesFlightTrip.FlightNo;
                N.RegNo = item?.OfficerData.TowerData.AircraftRegistration.Registration;


                N.AirPortFrom = item?.OfficerData.TowerData.AirPort1.Code;
                N.AirPortTo = item?.OfficerData.TowerData.AirPort.Code;

                N.TakeOffDate = CommonUsed.CheckDate(item?.OfficerData.TowerData.TakeOffDate.ToString());
                N.LandingDate = CommonUsed.CheckDate(item?.OfficerData.TowerData.LandingDate.ToString());

                N.TripType = CommonUsed.GetFlightTypeName(item.OfficerData.TowerData.FlightTypeL, item.OfficerData.TowerData.FlightTypeD);
                N.Date = Convert.ToDateTime(item.OfficerData.TowerData.Date).ToString("MM/dd/yyyy");

                N.AmbulanceCarCount = Convert.ToInt32(item?.OfficerData.AmbulanceCarCount);
                N.FireFightingCarCount = Convert.ToInt32(item?.OfficerData.FireFightingCarCount);

                N.AmbulanceCarFees = Convert.ToInt32(item?.AmbolanceFees);
                N.FireFightingCarFees = Convert.ToInt32(item?.FireFightingFees);

                N.Size = item?.OfficerData.TowerData.AircraftRegistration.AircraftType.AircraftSize.Size.ToString();
                N.MaxTakoffWieght = item?.OfficerData.TowerData.AircraftRegistration.MaxTakoffWieght.ToString();
                N.Type = item?.OfficerData.TowerData.AircraftRegistration.AircraftType.Type.ToString();

                N.ATA = item?.OfficerData.TowerData.ATA.ToString();
                N.ATD = item?.OfficerData.TowerData.ATD.ToString();
                N.AircraftRegName = item?.OfficerData.TowerData.AircraftRegistration.Registration.ToString();
                if (IsSingle)
                {
                    N.IsStop = CommonUsed.IsEntryDateinRange2(item.Id, Role.GetRole(id));  // check date in range in claim table
                }
                else
                {
                    N.IsStop = CommonUsed.IsEntryDateinRange(Convert.ToDateTime(item.OfficerData.TowerData.Date), Role.GetRole(id),
                  Convert.ToInt32(item.OfficerData.TowerData.AirLineId), Convert.ToInt32(companyId));  // check date in range in claim table
                }
                N.AirportName = CommonUsed.getCompanyName(item.OfficerData.TowerData.CompanyInfoId);
                RevenueList.Add(N);

            }

            return Json(new { data = RevenueList.ToList() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCarServicefeesParial(int? id)
        {
            Revenue Revenue = new Revenue();
            CarServiceFeesViewModelDTO CarServiceFeesViewModelDTO = new CarServiceFeesViewModelDTO();
            if (id > 0)
            {
                Revenue = db.Revenues.Find(id);
            }
            if (Revenue != null)
            {
                //if (Revenue.InterNYFees != null || Revenue.DomeNYFees != null)
                //{
                OfficerData OfficerData = db.OfficerDatas.Where(x => x.Id == Revenue.OfficerDataId).FirstOrDefault();



                CarServiceFeesViewModelDTO.AmbulanceCarCount = Convert.ToInt32(OfficerData.AmbulanceCarCount);

                CarServiceFeesViewModelDTO.FireFightingCarCount = Convert.ToInt32(OfficerData.FireFightingCarCount);

                CarServiceFeesViewModelDTO.Id = OfficerData.Id;
                CarServiceFeesViewModelDTO.OfficerDataId = OfficerData.Id;

                //}
            }

            return PartialView("_AddEditCarServiceFees", CarServiceFeesViewModelDTO);
        }


        public ActionResult UpdateCarServicefees(CarServiceFeesViewModelDTO model)
        {
            if (ModelState.IsValid)
            {
                var id = User.Identity.GetUserId();

                if (model.Id > 0)
                {

                    OfficerData OfficerData = db.OfficerDatas.Where(x => x.Id == model.OfficerDataId).FirstOrDefault();
                    db.Revenues.Where(x => x.Id == model.Id).FirstOrDefault();
                    //Revenue Revenue = new Revenue();

                    OfficerData.AmbulanceCarCount = model.AmbulanceCarCount;
                    OfficerData.Id = model.OfficerDataId;
                    OfficerData.FireFightingCarCount = model.FireFightingCarCount;
                    //  OfficerData.TRNY = model.TRNY;

                    TowerData TowerData = db.TowerDatas.Where(x => x.Id == OfficerData.TowerDataId).FirstOrDefault();

                    decimal AmbulanceCarvalue = Cal.AmbulanceFees(Convert.ToInt32(OfficerData.AmbulanceCarCount));

                    db.Revenues.SingleOrDefault(x => x.Id == model.Id).AmbolanceFees = AmbulanceCarvalue;

                    decimal FireFightingvalue = Cal.FireFightingFees(Convert.ToInt32(OfficerData.FireFightingCarCount));

                    db.Revenues.SingleOrDefault(x => x.Id == model.Id).FireFightingFees = FireFightingvalue;




                    db.Entry(OfficerData).State = System.Data.Entity.EntityState.Modified;
                    //   cal.LandingFees(model.Id);
                    db.SaveChanges();



                    return Json(new { success = true, message = "Update" }, JsonRequestBehavior.AllowGet);
                }
                else
                {


                    //db.TowerDatas.Add(model);
                    //db.SaveChanges();


                    return Json(new { success = true, message = "Add" }, JsonRequestBehavior.AllowGet);
                }

            }
            return Json(new { success = false, message = "error" }, JsonRequestBehavior.AllowGet);
        }


        /////////////////////////////////////////////////Mail && Frieght///////////////////////////////////////////////////////////////////


        [Authorize(Roles = "Landing-Admin,Revenues,Super-Admin,Mail&&Freight, Account, Account-Admin")]
        public ActionResult MailAndFreightMain()
        {
            var id = User.Identity.GetUserId();

            if (db.WorkOns.Any(x => x.UserId == id))
            {
                WorkMouth = Convert.ToInt32(db.WorkOns.Where(x => x.UserId == id).Single().MonthWorkOn);
            }
            if (WorkMouth > 0)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "WorkOns");
            }
        }

        [Authorize(Roles = "Landing-Admin,Revenues,Super-Admin,Mail&&Freight, Account, Account-Admin")]
        public ActionResult AllMailAndFreightMain()
        {
            var id = User.Identity.GetUserId();

            if (db.WorkOns.Any(x => x.UserId == id))
            {
                WorkMouth = Convert.ToInt32(db.WorkOns.Where(x => x.UserId == id).Single().MonthWorkOn);
            }
            if (WorkMouth > 0)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "WorkOns");
            }
        }



        public ActionResult GetMailAndFreight()
        {
            var id = User.Identity.GetUserId();
            WorkMouth = CommonUsed.GetMouth(id);
            WorkYear = CommonUsed.GetYear(id);
            var companyId = CommonUsed.getCompanyId(id);
            bool IsSingle = CommonUsed.getIsSingleAccounting(companyId);


            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.Where(x => x.FreightFees == null && x.ExpMailFees == null && x.ImpMailFees == null
            && x.OfficerData.TowerData.Date.Value.Month == WorkMouth && x.OfficerData.TowerData.Date.Value.Year == WorkYear
            && x.OfficerData.TowerData.CompanyInfoId == companyId);
            var RevenueList = new List<MailAndFreightViewModelDto>();
            foreach (var item in Revenue)
            {
                MailAndFreightViewModelDto N = new MailAndFreightViewModelDto();

                N.Id = item.Id;
                N.TowerId = item.OfficerData.TowerData.Id;

                N.AirlineName = item?.OfficerData.TowerData.AirLine.Name;
                N.FlightNo = item?.OfficerData.TowerData.AirLinesFlightTrip.FlightNo;
                N.RegNo = item?.OfficerData.TowerData.AircraftRegistration.Registration;

                N.AirPortFrom = item?.OfficerData.TowerData.AirPort1.Code;
                N.AirPortTo = item?.OfficerData.TowerData.AirPort.Code;

                N.TakeOffDate = CommonUsed.CheckDate(item?.OfficerData.TowerData.TakeOffDate.ToString());
                N.LandingDate = CommonUsed.CheckDate(item?.OfficerData.TowerData.LandingDate.ToString());

                N.TripType = CommonUsed.GetFlightTypeName(item.OfficerData.TowerData.FlightTypeL, item.OfficerData.TowerData.FlightTypeD);
                N.Date = Convert.ToDateTime(item.OfficerData.TowerData.Date).ToString("MM/dd/yyyy");

                N.FreightLoadingExp = Convert.ToInt32(item?.OfficerData.FreightLoadingExp);

                N.FreightLoadingImp = Convert.ToInt32(item?.OfficerData.FreightLoadingImp);

                N.NormalMailLoadingExp = Convert.ToDecimal(item?.OfficerData.NormalMailLoadingExp);
                N.NormalMailLoadingImp = Convert.ToDecimal(item?.OfficerData.NormalMailLoadingImp);

                N.RapidMailLoadingExp = Convert.ToDecimal(item?.OfficerData.RapidMailLoadingExp);
                N.RapidMailLoadingImp = Convert.ToDecimal(item?.OfficerData.RapidMailLoadingImp);

                N.FreightFees = Convert.ToInt32(item?.FreightFees);

                N.ExpMailFees = Convert.ToDecimal(item?.ExpMailFees);
                N.ImpMailFees = Convert.ToDecimal(item?.ImpMailFees);

                N.Size = item?.OfficerData.TowerData.AircraftRegistration.AircraftType.AircraftSize.Size.ToString();
                N.MaxTakoffWieght = item?.OfficerData.TowerData.AircraftRegistration.MaxTakoffWieght.ToString();
                N.Type = item?.OfficerData.TowerData.AircraftRegistration.AircraftType.Type.ToString();

                N.ATA = item?.OfficerData.TowerData.ATA.ToString();
                N.ATD = item?.OfficerData.TowerData.ATD.ToString();
                N.AircraftRegName = item?.OfficerData.TowerData.AircraftRegistration.Registration.ToString();
                if (IsSingle)
                {
                    N.IsStop = CommonUsed.IsEntryDateinRange2(item.Id, Role.GetRole(id));  // check date in range in claim table
                }
                else
                {
                    N.IsStop = CommonUsed.IsEntryDateinRange(Convert.ToDateTime(item.OfficerData.TowerData.Date), Role.GetRole(id),
                  Convert.ToInt32(item.OfficerData.TowerData.AirLineId), Convert.ToInt32(companyId));  // check date in range in claim table
                }
                N.AirportName = CommonUsed.getCompanyName(item.OfficerData.TowerData.CompanyInfoId);

                RevenueList.Add(N);

            }

            return Json(new { data = RevenueList.ToList() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AllGetMailAndFreight()
        {

            var id = User.Identity.GetUserId();
            WorkMouth = CommonUsed.GetMouth(id);
            WorkYear = CommonUsed.GetYear(id);
            var companyId = CommonUsed.getCompanyId(id);
            bool IsSingle = CommonUsed.getIsSingleAccounting(companyId);

            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.Where(x => x.FreightFees != null && x.ExpMailFees != null && x.ImpMailFees != null
           && x.OfficerData.TowerData.Date.Value.Month == WorkMouth && x.OfficerData.TowerData.Date.Value.Year == WorkYear
           && x.OfficerData.TowerData.CompanyInfoId == companyId);

            var RevenueList = new List<MailAndFreightViewModelDto>();
            foreach (var item in Revenue)
            {
                MailAndFreightViewModelDto N = new MailAndFreightViewModelDto();

                N.Id = item.Id;
                N.TowerId = item.OfficerData.TowerData.Id;
                N.AirlineName = item?.OfficerData.TowerData.AirLine.Name;

                N.FlightNo = item?.OfficerData.TowerData.AirLinesFlightTrip.FlightNo;
                N.RegNo = item?.OfficerData.TowerData.AircraftRegistration.Registration;

                N.AirPortFrom = item?.OfficerData.TowerData.AirPort1.Code;
                N.AirPortTo = item?.OfficerData.TowerData.AirPort.Code;

                N.TakeOffDate = CommonUsed.CheckDate(item?.OfficerData.TowerData.TakeOffDate.ToString());
                N.LandingDate = CommonUsed.CheckDate(item?.OfficerData.TowerData.LandingDate.ToString());

                N.TripType = CommonUsed.GetFlightTypeName(item.OfficerData.TowerData.FlightTypeL, item.OfficerData.TowerData.FlightTypeD);
                N.Date = Convert.ToDateTime(item.OfficerData.TowerData.Date).ToString("MM/dd/yyyy");

                N.FreightLoadingExp = Convert.ToInt32(item?.OfficerData.FreightLoadingExp);

                N.FreightLoadingImp = Convert.ToInt32(item?.OfficerData.FreightLoadingImp);

                N.NormalMailLoadingExp = Convert.ToDecimal(item?.OfficerData.NormalMailLoadingExp);
                N.NormalMailLoadingImp = Convert.ToDecimal(item?.OfficerData.NormalMailLoadingImp);

                N.RapidMailLoadingExp = Convert.ToDecimal(item?.OfficerData.RapidMailLoadingExp);
                N.RapidMailLoadingImp = Convert.ToDecimal(item?.OfficerData.RapidMailLoadingImp);

                N.FreightFees = Convert.ToInt32(item?.FreightFees);

                N.ExpMailFees = Convert.ToDecimal(item?.ExpMailFees);
                N.ImpMailFees = Convert.ToDecimal(item?.ImpMailFees);


                N.Size = item?.OfficerData.TowerData.AircraftRegistration.AircraftType.AircraftSize.Size.ToString();
                N.MaxTakoffWieght = item?.OfficerData.TowerData.AircraftRegistration.MaxTakoffWieght.ToString();
                N.Type = item?.OfficerData.TowerData.AircraftRegistration.AircraftType.Type.ToString();

                N.ATA = item?.OfficerData.TowerData.ATA.ToString();
                N.ATD = item?.OfficerData.TowerData.ATD.ToString();
                N.AircraftRegName = item?.OfficerData.TowerData.AircraftRegistration.Registration.ToString();
                if (IsSingle)
                {
                    N.IsStop = CommonUsed.IsEntryDateinRange2(item.Id, Role.GetRole(id));  // check date in range in claim table
                }
                else
                {
                    N.IsStop = CommonUsed.IsEntryDateinRange(Convert.ToDateTime(item.OfficerData.TowerData.Date), Role.GetRole(id),
                  Convert.ToInt32(item.OfficerData.TowerData.AirLineId), Convert.ToInt32(companyId));  // check date in range in claim table
                }
                N.AirportName = CommonUsed.getCompanyName(item.OfficerData.TowerData.CompanyInfoId);




                RevenueList.Add(N);

            }

            return Json(new { data = RevenueList.ToList() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMailAndFreightfeesParial(int? id)
        {
            Revenue Revenue = new Revenue();
            MailAndFreightFeesViewModelDTO MailAndFreightFeesViewModelDTO = new MailAndFreightFeesViewModelDTO();
            if (id > 0)
            {
                Revenue = db.Revenues.Find(id);
            }
            if (Revenue != null)
            {
                //if (Revenue.InterNYFees != null || Revenue.DomeNYFees != null)
                //{
                OfficerData OfficerData = db.OfficerDatas.Where(x => x.Id == Revenue.OfficerDataId).FirstOrDefault();

                MailAndFreightFeesViewModelDTO.FrightLoadingExp = Convert.ToInt32(OfficerData.FreightLoadingExp);
                MailAndFreightFeesViewModelDTO.FrightLoadingImp = Convert.ToInt32(OfficerData.FreightLoadingImp);

                MailAndFreightFeesViewModelDTO.NormalMailLoadingExpAmount = Convert.ToDecimal(OfficerData.NormalMailLoadingExp);
                MailAndFreightFeesViewModelDTO.RapidMailLoadingExpAmount = Convert.ToDecimal(OfficerData.RapidMailLoadingExp);

                MailAndFreightFeesViewModelDTO.NormalMailLoadingImpAmount = Convert.ToDecimal(OfficerData.NormalMailLoadingImp);
                MailAndFreightFeesViewModelDTO.RapidMailLoadingImpAmount = Convert.ToDecimal(OfficerData.RapidMailLoadingImp);
                //  CuppsFeesViewModelDTO.TRNY = Convert.ToInt32(OfficerData.TRNY);
                //  CuppsFeesViewModelDTO.TRNYCUPPS = Convert.ToInt32(OfficerData.TRNYCUPPS);
                MailAndFreightFeesViewModelDTO.Id = Convert.ToInt32(id);
                MailAndFreightFeesViewModelDTO.OfficerDataId = OfficerData.Id;
                MailAndFreightFeesViewModelDTO.RevenueId = Convert.ToInt32(id);

                //}
            }

            return PartialView("_AddEditMailAndFreightFees", MailAndFreightFeesViewModelDTO);
        }

        public ActionResult UpdateMailAndFreightfees(MailAndFreightFeesViewModelDTO model)
        {
            if (ModelState.IsValid)
            {
                var id = User.Identity.GetUserId();

                if (model.RevenueId > 0)
                {

                    OfficerData OfficerData = db.OfficerDatas.Where(x => x.Id == model.OfficerDataId).FirstOrDefault();
                    db.Revenues.Where(x => x.Id == model.Id).FirstOrDefault();
                    //Revenue Revenue = new Revenue();
                    OfficerData.Id = model.OfficerDataId;

                    OfficerData.NormalMailLoadingExp = model.NormalMailLoadingExpAmount;
                    OfficerData.RapidMailLoadingExp = model.RapidMailLoadingExpAmount;

                    OfficerData.NormalMailLoadingImp = model.NormalMailLoadingImpAmount;
                    OfficerData.RapidMailLoadingImp = model.RapidMailLoadingImpAmount;

                    OfficerData.FreightLoadingExp = model.FrightLoadingExp;
                    OfficerData.FreightLoadingImp = model.FrightLoadingImp;


                    decimal ExpMailFeesValue = Cal.ExpMailFees(Convert.ToDecimal(OfficerData.NormalMailLoadingExp), Convert.ToDecimal(OfficerData.RapidMailLoadingExp));

                    db.Revenues.SingleOrDefault(x => x.Id == model.RevenueId).ExpMailFees = ExpMailFeesValue;

                    decimal ImpMailFeesValue = Cal.ImpMailFees(Convert.ToDecimal(OfficerData.NormalMailLoadingImp), Convert.ToDecimal(OfficerData.RapidMailLoadingImp));

                    db.Revenues.SingleOrDefault(x => x.Id == model.RevenueId).ImpMailFees = ImpMailFeesValue;

                    decimal FreightFeesValue = Cal.FreightFees(Convert.ToDecimal(OfficerData.FreightLoadingExp), Convert.ToDecimal(OfficerData.FreightLoadingImp));

                    db.Revenues.SingleOrDefault(x => x.Id == model.RevenueId).FreightFees = FreightFeesValue;


                    db.Entry(OfficerData).State = System.Data.Entity.EntityState.Modified;
                    //   cal.LandingFees(model.Id);
                    db.SaveChanges();



                    return Json(new { success = true, message = "Update" }, JsonRequestBehavior.AllowGet);
                }
                else
                {


                    //db.TowerDatas.Add(model);
                    //db.SaveChanges();


                    return Json(new { success = true, message = "Add" }, JsonRequestBehavior.AllowGet);
                }

            }
            return Json(new { success = false, message = "error" }, JsonRequestBehavior.AllowGet);
        }






        ///////////////////////////////////////////////PushPack///////////////////////////////////////////////////////////





        public ActionResult PushPackMain()
        {
            var id = User.Identity.GetUserId();

            if (db.WorkOns.Any(x => x.UserId == id))
            {
                WorkMouth = Convert.ToInt32(db.WorkOns.Where(x => x.UserId == id).Single().MonthWorkOn);
            }
            if (WorkMouth > 0)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "WorkOns");
            }
        }
        public ActionResult AllPushPackMain()
        {
            var id = User.Identity.GetUserId();

            if (db.WorkOns.Any(x => x.UserId == id))
            {
                WorkMouth = Convert.ToInt32(db.WorkOns.Where(x => x.UserId == id).Single().MonthWorkOn);
            }
            if (WorkMouth > 0)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "WorkOns");
            }
        }
        public ActionResult GetPushPack()
        {
            var id = User.Identity.GetUserId();
            WorkMouth = CommonUsed.GetMouth(id);
            WorkYear = CommonUsed.GetYear(id);
            var companyId = CommonUsed.getCompanyId(id);
            bool IsSingle = CommonUsed.getIsSingleAccounting(companyId);

            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.Where(x => x.PushBackFees == null
            && x.OfficerData.TowerData.Date.Value.Month == WorkMouth && x.OfficerData.TowerData.Date.Value.Year == WorkYear
            && x.OfficerData.TowerData.CompanyInfoId == companyId);

            var RevenueList = new List<PushPackViewModelDto>();
            foreach (var item in Revenue)
            {
                PushPackViewModelDto N = new PushPackViewModelDto();

                N.Id = item.Id;
                N.TowerId = item.OfficerData.TowerData.Id;
                N.AirlineName = item?.OfficerData.TowerData.AirLine.Name;
                N.FlightNo = item?.OfficerData.TowerData.AirLinesFlightTrip.FlightNo;
                N.RegNo = item?.OfficerData.TowerData.AircraftRegistration.Registration;


                N.AirPortFrom = item?.OfficerData.TowerData.AirPort1.Code;
                N.AirPortTo = item?.OfficerData.TowerData.AirPort.Code;
                N.Date = CommonUsed.CheckDate(item?.OfficerData.TowerData.Date.ToString());
                //N.TakeOffDate = CommonUsed.CheckDate(item?.OfficerData.TowerData.TakeOffDate.ToString());
                //N.LandingDate = CommonUsed.CheckDate(item?.OfficerData.TowerData.LandingDate.ToString());

                N.FlightType = CommonUsed.GetFlightTypeName(item?.OfficerData.TowerData.FlightTypeL, item?.OfficerData.TowerData.FlightTypeD);

                N.hasPushPack = CommonUsed.CheckYesNo(Convert.ToBoolean(item?.OfficerData.HasPushPack));

                N.PushPackFees = Convert.ToDecimal(item?.PushBackFees);

                N.Size = item?.OfficerData.TowerData.AircraftRegistration.AircraftType.AircraftSize.Size.ToString();
                N.MaxTakoffWieght = item?.OfficerData.TowerData.AircraftRegistration.MaxTakoffWieght.ToString();
                N.Type = item?.OfficerData.TowerData.AircraftRegistration.AircraftType.Type.ToString();

                N.ATA = item?.OfficerData.TowerData.ATA.ToString();
                N.ATD = item?.OfficerData.TowerData.ATD.ToString();
                N.AircraftRegName = item?.OfficerData.TowerData.AircraftRegistration.Registration.ToString();
                if (IsSingle)
                {
                    N.IsStop = CommonUsed.IsEntryDateinRange2(item.Id, Role.GetRole(id));  // check date in range in claim table
                }
                else
                {
                    N.IsStop = CommonUsed.IsEntryDateinRange(Convert.ToDateTime(item.OfficerData.TowerData.Date), Role.GetRole(id),
                  Convert.ToInt32(item.OfficerData.TowerData.AirLineId), Convert.ToInt32(companyId));  // check date in range in claim table
                }
                N.AirportName = CommonUsed.getCompanyName(item.OfficerData.TowerData.CompanyInfoId);




                RevenueList.Add(N);

            }

            return Json(new { data = RevenueList.ToList() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AllGetPushPack()
        {
            var id = User.Identity.GetUserId();
            WorkMouth = CommonUsed.GetMouth(id);
            WorkYear = CommonUsed.GetYear(id);
            var companyId = CommonUsed.getCompanyId(id);
            bool IsSingle = CommonUsed.getIsSingleAccounting(companyId);

            IQueryable<Revenue> Revenue;

            Revenue = db.Revenues.Where(x => x.PushBackFees != null
            && x.OfficerData.TowerData.Date.Value.Month == WorkMouth && x.OfficerData.TowerData.Date.Value.Year == WorkYear
            && x.OfficerData.TowerData.CompanyInfoId == companyId);
          
            var RevenueList = new List<PushPackViewModelDto>();
            foreach (var item in Revenue)
            {
                PushPackViewModelDto N = new PushPackViewModelDto();

                N.Id = item.Id;
                N.TowerId = item.OfficerData.TowerData.Id;
                N.AirlineName = item?.OfficerData.TowerData.AirLine.Name;
                N.FlightNo = item?.OfficerData.TowerData.AirLinesFlightTrip.FlightNo;
                N.RegNo = item?.OfficerData.TowerData.AircraftRegistration.Registration;


                N.AirPortFrom = item?.OfficerData.TowerData.AirPort1.Code;
                N.AirPortTo = item?.OfficerData.TowerData.AirPort.Code;
                N.FlightType = CommonUsed.GetFlightTypeName(item?.OfficerData.TowerData.FlightTypeL, item?.OfficerData.TowerData.FlightTypeD);

                N.Date = CommonUsed.CheckDate(item?.OfficerData.TowerData.Date.ToString());
                N.LandingDate = CommonUsed.CheckDate(item?.OfficerData.TowerData.LandingDate.ToString());
                N.TakeOffDate = CommonUsed.CheckDate(item?.OfficerData.TowerData.TakeOffDate.ToString());

                N.hasPushPack = CommonUsed.CheckYesNo(Convert.ToBoolean(item.OfficerData.HasPushPack));

                N.PushPackFees = Convert.ToDecimal(item?.PushBackFees);


                N.Size = item?.OfficerData.TowerData.AircraftRegistration.AircraftType.AircraftSize.Size.ToString();
                N.MaxTakoffWieght = item?.OfficerData.TowerData.AircraftRegistration.MaxTakoffWieght.ToString();
                N.Type = item?.OfficerData.TowerData.AircraftRegistration.AircraftType.Type.ToString();

                N.ATA = item?.OfficerData.TowerData.ATA.ToString();
                N.ATD = item?.OfficerData.TowerData.ATD.ToString();
                N.AircraftRegName = item?.OfficerData.TowerData.AircraftRegistration.Registration.ToString();
                if (IsSingle)
                {
                    N.IsStop = CommonUsed.IsEntryDateinRange2(item.Id, Role.GetRole(id));  // check date in range in claim table
                }
                else
                {
                    N.IsStop = CommonUsed.IsEntryDateinRange(Convert.ToDateTime(item.OfficerData.TowerData.Date), Role.GetRole(id),
                  Convert.ToInt32(item.OfficerData.TowerData.AirLineId), Convert.ToInt32(companyId));  // check date in range in claim table
                }
                N.AirportName = CommonUsed.getCompanyName(item.OfficerData.TowerData.CompanyInfoId);




                RevenueList.Add(N);

            }

            return Json(new { data = RevenueList.ToList() }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetPushPackfeesParial(int? id)
        {
            Revenue Revenue = new Revenue();
            PushPackFeesViewModelDTO PushPackFeesViewModelDTO = new PushPackFeesViewModelDTO();
            if (id > 0)
            {
                Revenue = db.Revenues.Find(id);
            }
            if (Revenue != null)
            {
                //if (Revenue.InterNYFees != null || Revenue.DomeNYFees != null)
                //{
                OfficerData OfficerData = db.OfficerDatas.Where(x => x.Id == Revenue.OfficerDataId).FirstOrDefault();



                PushPackFeesViewModelDTO.hasPushPack = Convert.ToBoolean(OfficerData.HasPushPack);



                //  CuppsFeesViewModelDTO.TRNY = Convert.ToInt32(OfficerData.TRNY);
                //  CuppsFeesViewModelDTO.TRNYCUPPS = Convert.ToInt32(OfficerData.TRNYCUPPS);
                PushPackFeesViewModelDTO.Id = Convert.ToInt32(id);
                PushPackFeesViewModelDTO.OfficerDataId = OfficerData.Id;

                //}
            }

            return PartialView("_AddEditPushPackFees", PushPackFeesViewModelDTO);
        }


        public ActionResult UpdatePushPackfees(PushPackFeesViewModelDTO model)
        {
            if (ModelState.IsValid)
            {
                var id = User.Identity.GetUserId();

                if (model.Id > 0)
                {

                    OfficerData OfficerData = db.OfficerDatas.Where(x => x.Id == model.OfficerDataId).FirstOrDefault();
                    db.Revenues.Where(x => x.Id == model.Id).FirstOrDefault();
                    //Revenue Revenue = new Revenue();
                    OfficerData.Id = model.OfficerDataId;

                    if (model.hasPushPack)
                    {
                        if (Convert.ToInt32(OfficerData.TowerData.AircraftRegistration.AircraftType.SizeId) <= 0)
                        {
                            return Json(new { success = true, message = "SizeNotFound" }, JsonRequestBehavior.AllowGet);
                        }
                        OfficerData.HasPushPack = true;

                        string size = OfficerData.TowerData.AircraftRegistration.AircraftType.AircraftSize.Size;
                        decimal PushBackValue = Cal.PushBackFees(size);

                        db.Revenues.SingleOrDefault(x => x.Id == model.Id).PushBackFees = PushBackValue;

                    }
                    else
                    {
                        OfficerData.HasPushPack = false;


                        db.Revenues.SingleOrDefault(x => x.Id == model.Id).PushBackFees = 0;
                    }



                    db.Entry(OfficerData).State = System.Data.Entity.EntityState.Modified;
                    //   cal.LandingFees(model.Id);
                    db.SaveChanges();



                    return Json(new { success = true, message = "Update" }, JsonRequestBehavior.AllowGet);
                }
                else
                {


                    //db.TowerDatas.Add(model);
                    //db.SaveChanges();


                    return Json(new { success = true, message = "Add" }, JsonRequestBehavior.AllowGet);
                }

            }
            return Json(new { success = false, message = "error" }, JsonRequestBehavior.AllowGet);
        }
    }
}