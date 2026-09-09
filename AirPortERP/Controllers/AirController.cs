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
using System.IO;
using Microsoft.AspNet.Identity;
using PagedList;
using AirPortERP.Extenstion;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading;
using System.Globalization;

namespace AirPortERP.Controllers
{
    public class AirController : Controller
    {

        DateTime now = System.DateTime.Now;
        private CommonUsed CommonUsed = new CommonUsed();
        private Entities db = new Entities();
        RoleController Role = new RoleController();
        private Calculation cal = new Calculation();
        IQueryable<PrintDailyViewModel> DailyList;
        int WorkMouth;
        int WorkYear;
        // GET: Air

        #region How Data And Takeoff And Arrived 
        public ActionResult AirMove()
        {
            return View();
        }
        public ActionResult AirCompleted()
        {
            return View();
        }

        public ActionResult GetAirline()
        {
            var data = db.AirLines.Select(p => new
            {
                Id = p.Id,
                Name = p.Code,
            });

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetFlightNo()
        {
            var data = db.AirLinesFlightTrips.Select(p => new
            {
                Id = p.Id,
                FlightNo = p.FlightNo,
            });

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetReg()
        {
            var data = db.AircraftRegistrations.Select(p => new
            {
                Id = p.Id,
                Registration = p.Registration,
            });

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAirport()
        {
            var data = db.AirPorts.Select(p => new
            {
                Id = p.Id,
                Code = p.Code,
            });

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTripType()
        {
            var data = db.TripTypes.Select(p => new
            {
                Id = p.Id,
                Name = p.Name,
            });

            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public ActionResult CreateArrive(AirArriveModel model)
        {
            var id = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(id);

            if (ModelState.IsValid)
            {
                TowerData TowerData = new TowerData();

                TowerData.Id = model.IdAr;
                TowerData.AirLineId = Convert.ToInt32(model.AirLineIdAr);
                TowerData.FlightNo = Convert.ToInt32(model.FlightNoAr);
                TowerData.AircraftRegId = Convert.ToInt32(model.AircraftRegIdAr);
                TowerData.AirportIdFrom = model.AirportIdFromAr;
                TowerData.AirportIdTo = model.AirportIdToAr;
                //TowerData.LandingDate = Convert.ToDateTime(model.LandingDateAr);
                TowerData.LandingDate = DateTime.ParseExact(model.LandingDateAr,"yyyy-MM-dd", CultureInfo.InvariantCulture);

                //TowerData.Date = Convert.ToDateTime(model.DateAr);
                TowerData.Date = DateTime.ParseExact(model.DateAr, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                ///TowerData.TakeOffDate = CommonUsed.GetDateD(model.TakeOffDateAr);

                //TowerData.CreationDate = DateTime.ParseExact(model.CreationDateAr, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                TowerData.UpdatingDate = model.UpdatingDateAr;

                TowerData.CreatedBy = model.CreatedByAr;
                TowerData.UpdatedBy = model.UpdatedByAr;

                TowerData.ATA = model.ATAAr;
                TowerData.ATD = model.ATDAr;
               
                TowerData.Note = model.NoteAr;
                TowerData.TripTypeId = model.TripTypeIdAr;

                TowerData.POB = model.POBAr;
                TowerData.QBD = model.QBDAr;
                TowerData.LandingPermission = model.LandingPermissionAr;

                int TowerId = Convert.ToInt32(db.SpLastTowerid().FirstOrDefault());

              
               


                    if (db.TowerDatas.Any(x => x.AirLineId == model.AirLineIdAr && x.ATA == model.ATAAr && x.ATD == model.ATDAr && x.Date == TowerData.Date && x.FlightNo == model.FlightNoAr))

                    {
                        //  return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
                     ///   return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
                    return Json(new { Message = "هذا الرحلة  موجود  مسبقا", Title = "عملية الاضافة", Status = "error" });


                }

                TowerData.CreatedBy = id;
                    TowerData.CreationDate = System.DateTime.Now;

                    /////////////////////////////******* Start Check FlightType******////////////////////////////////////////
                    // From Not Found When Aircraft Departure
                    if (model.AirportIdToAr > 0)
                    {
                        AirPort ATo = db.AirPorts.Single(x => x.Id == model.AirportIdToAr);
                        if (ATo.CountryId == 1)
                        {
                            // Domistic
                            TowerData.FlightTypeD = 2;
                        }
                        else
                        {    // InterNational
                            TowerData.FlightTypeD = 1;
                        }
                    }
                    // From Not Found When Aircraft Take Off

                    if (model.AirportIdFromAr > 0)
                    {
                        AirPort AFrom = db.AirPorts.Single(x => x.Id == model.AirportIdFromAr);
                        if (model.AirportIdFromAr > 0)
                        {
                            if (AFrom.CountryId == 1)
                            {
                                // Domistic
                                TowerData.FlightTypeL = 2;
                            }
                            else
                            {    // InterNational
                                TowerData.FlightTypeL = 1;
                            }
                        }
                    }
                //****************************Start Check Status*****************************************

                TowerData.Status = 2;


                // ******************************End Check Status***********************************

                   TowerData.CompanyInfoId = companyId;
                    TowerData.Id = TowerId;
                    db.TowerDatas.Add(TowerData);


                db.SaveChanges();


                return Json(new { Message = "تمت عملية الإضافة  بنجاح", Title = "نجاح", Status = "success" });

            }
            return Json(new { success = false, message = "error" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDataForUpdateAr(int id)
        {
            TowerData TowerData = db.TowerDatas.Find(id);

            AirArriveModel model = new AirArriveModel();
            model.IdAr = TowerData.Id;
            model.AirLineIdAr = Convert.ToInt32(TowerData.AirLineId);
            model.FlightNoAr = Convert.ToInt32(TowerData.FlightNo);

            model.AircraftRegIdAr = Convert.ToInt32(TowerData.AircraftRegId);
            model.AirportIdFromAr = TowerData.AirportIdFrom;

            model.ATAAr = TowerData.ATA;
            model.ATAArShow = ConvertToHHmm(TowerData.ATA.ToString());
            


            model.TripTypeIdAr = Convert.ToInt32(TowerData.TripTypeId);
            model.DateAr = Convert.ToDateTime(TowerData.Date).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            model.LandingDateAr = Convert.ToDateTime(TowerData.LandingDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);



            model.POBAr = TowerData.POB;
            model.QBDAr = TowerData.QBD;
            model.NoteAr = TowerData.Note;

            model.LandingPermissionAr = TowerData.LandingPermission;
            model.CreatedByAr = TowerData.CreatedBy;
            model.CreationDateAr = TowerData.CreationDate.ToString();
            return Json(model, JsonRequestBehavior.AllowGet);

        }

        public ActionResult UpdateArrive(AirArriveModel model)
        {
            var id = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(id);
            if (ModelState.IsValid)
            {
                TowerData TowerData = new TowerData();

                TowerData.Id = model.IdAr;
                TowerData.AirLineId = Convert.ToInt32(model.AirLineIdAr);
                TowerData.FlightNo = Convert.ToInt32(model.FlightNoAr);
                TowerData.AircraftRegId = Convert.ToInt32(model.AircraftRegIdAr);
                TowerData.AirportIdFrom = model.AirportIdFromAr;
                TowerData.AirportIdTo = model.AirportIdToAr;
                //TowerData.LandingDate = Convert.ToDateTime(model.LandingDateAr);
                TowerData.LandingDate = DateTime.ParseExact(model.LandingDateAr,"yyyy-MM-dd", CultureInfo.InvariantCulture);


               
                //TowerData.Date = Convert.ToDateTime(model.DateAr);
                TowerData.Date = DateTime.ParseExact(model.DateAr, "yyyy-MM-dd", CultureInfo.InvariantCulture);



                TowerData.CreationDate = Convert.ToDateTime(model.CreationDateAr);
                TowerData.UpdatingDate = model.UpdatingDateAr;

                TowerData.CreatedBy = model.CreatedByAr;
                TowerData.UpdatedBy = model.UpdatedByAr;

                TowerData.ATA = model.ATAAr;
                //TowerData.ATD = model.ATDAr;

                TowerData.Note = model.NoteAr;
                TowerData.TripTypeId = model.TripTypeIdAr;

                TowerData.POB = model.POBAr;
                TowerData.QBD = model.QBDAr;
                TowerData.LandingPermission = model.LandingPermissionAr;
              
                if (db.TowerDatas.Any(x => x.AirLineId == model.AirLineIdAr && x.ATA == model.ATAAr && x.Date == TowerData.Date && x.FlightNo == model.FlightNoAr && x.Id != model.IdAr)) 
                {              
                    return Json(new { Message = "هذا الرحلة  موجود  مسبقا", Title = "عملية الاضافة", Status = "error" });
                }

                TowerData.UpdatedBy = id;
                TowerData.UpdatingDate = System.DateTime.Now;

                /////////////////////////////******* Start Check FlightType******////////////////////////////////////////
                // From Not Found When Aircraft Departure
                if (model.AirportIdToAr > 0)
                {
                    AirPort ATo = db.AirPorts.Single(x => x.Id == model.AirportIdToAr);
                    if (ATo.CountryId == 1)
                    {
                        // Domistic
                        TowerData.FlightTypeD = 2;
                    }
                    else
                    {    // InterNational
                        TowerData.FlightTypeD = 1;
                    }
                }
                // From Not Found When Aircraft Take Off

                if (model.AirportIdFromAr > 0)
                {
                    AirPort AFrom = db.AirPorts.Single(x => x.Id == model.AirportIdFromAr);
                    if (model.AirportIdFromAr > 0)
                    {
                        if (AFrom.CountryId == 1)
                        {
                            // Domistic
                            TowerData.FlightTypeL = 2;
                        }
                        else
                        {    // InterNational
                            TowerData.FlightTypeL = 1;
                        }
                    }
                }
         
                TowerData.Status = 2;
                TowerData.CompanyInfoId = companyId;
                db.Entry(TowerData).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new { Message = "تمت عملية التعديل  بنجاح", Title = "نجاح", Status = "success" });

            }
            return Json(new { success = false, message = "error" }, JsonRequestBehavior.AllowGet);
        }



        public ActionResult GetDataForUpdateMkAr(int id)
        {
            TowerData TowerData = db.TowerDatas.Find(id);

            AirMakeArrivedModel model = new AirMakeArrivedModel();
            model.IdMArrived = TowerData.Id;
            model.AirLineIdMArrived = Convert.ToInt32(TowerData.AirLineId);
            model.FlightNoMArrived = Convert.ToInt32(TowerData.FlightNo);

            model.AircraftRegIdMArrived = Convert.ToInt32(TowerData.AircraftRegId);
            model.AirportIdToMArrived = TowerData.AirportIdTo;
            model.AirportIdFromMArrived = TowerData.AirportIdFrom;


            model.ATDMArrived = TowerData.ATD;
            model.ATDMArrivedShow = ConvertToHHmm(TowerData.ATD.ToString());

            model.ATAMArrived = TowerData.ATA;
            model.ATAMArrivedShow = ConvertToHHmm(TowerData.ATA.ToString());

            model.TripTypeIdMArrived = Convert.ToInt32(TowerData.TripTypeId);
            model.DateMArrived = Convert.ToDateTime(TowerData.Date).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            //model.DateMTakeOff = Convert.ToDateTime(TowerData.Date).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);



            model.TakeOffDateMArrived = Convert.ToDateTime(TowerData.TakeOffDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            //model.LandingDateMArrived = Convert.ToDateTime(TowerData.LandingDate).ToString("yyyy-MM-dd");


            model.POBMArrived = TowerData.POB;
            model.QBDMArrived = TowerData.QBD;
            model.NoteMArrived = TowerData.Note;

            model.LandingPermissionMArrived = TowerData.LandingPermission;
            model.CreatedByMArrived = TowerData.CreatedBy;
            //model.CreationDateMArrived = Convert.ToDateTime(TowerData.CreationDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            model.CreationDateMArrived = Convert.ToDateTime(TowerData.CreationDate).ToString();


            return Json(model, JsonRequestBehavior.AllowGet);

        }


        public ActionResult GetDataForUpdateMkDe(int id)
        {
            TowerData TowerData = db.TowerDatas.Find(id);

            AirMakeTakeOffModel model = new AirMakeTakeOffModel();
            model.IdMTakeOff = TowerData.Id;
            model.AirLineIdMTakeOff = Convert.ToInt32(TowerData.AirLineId);
            model.FlightNoMTakeOff = Convert.ToInt32(TowerData.FlightNo);

            model.AircraftRegIdMTakeOff = Convert.ToInt32(TowerData.AircraftRegId);
          ///  model.AirportIdToMTakeOff = TowerData.AirportIdTo;
            model.AirportIdFromMTakeOff = TowerData.AirportIdFrom;


          //  model.ATDMTakeOff = TowerData.ATD;
            model.ATDMTakeOffShow = ConvertToHHmm(TowerData.ATD.ToString());

            model.ATAMTakeOff = TowerData.ATA;
            model.ATAMTakeOffShow = ConvertToHHmm(TowerData.ATA.ToString());

            model.TripTypeIdMTakeOff = Convert.ToInt32(TowerData.TripTypeId);
            model.DateMTakeOff = Convert.ToDateTime(TowerData.Date).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
          ///  model.TakeOffDateMTakeOff = Convert.ToDateTime(TowerData.TakeOffDate).ToString("yyyy-MM-dd");
            model.LandingDateMTakeOff = Convert.ToDateTime(TowerData.LandingDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);


            model.POBMTakeOff = TowerData.POB;
            model.QBDMTakeOff = TowerData.QBD;
            model.NoteMTakeOff = TowerData.Note;

            model.LandingPermissionMTakeOff = TowerData.LandingPermission;
            model.CreatedByMTakeOff = TowerData.CreatedBy;
            model.CreationDateMTakeOff = Convert.ToDateTime(TowerData.CreationDate).ToString();
            return Json(model, JsonRequestBehavior.AllowGet);

        }



        public ActionResult CreateOrUpdateMakeTakeOff(AirMakeTakeOffModel model)
        {
            var id = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(id);
            if (ModelState.IsValid)
            {
                TowerData TowerData = new TowerData();

                TowerData.Id = model.IdMTakeOff;
                TowerData.AirLineId = Convert.ToInt32(model.AirLineIdMTakeOff);
                TowerData.FlightNo = Convert.ToInt32(model.FlightNoMTakeOff);
                TowerData.AircraftRegId = Convert.ToInt32(model.AircraftRegIdMTakeOff);
                TowerData.AirportIdFrom = model.AirportIdFromMTakeOff;
                TowerData.AirportIdTo = model.AirportIdToMTakeOff;

                //TowerData.LandingDate = Convert.ToDateTime(model.LandingDateMTakeOff);
                //TowerData.LandingDate = DateTime.ParseExact(model.LandingDateMTakeOff, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                TowerData.LandingDate = DateTime.ParseExact(model.LandingDateMTakeOff, "yyyy-MM-dd", CultureInfo.InvariantCulture);



                TowerData.Date = DateTime.ParseExact(model.DateMTakeOff, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                TowerData.TakeOffDate = DateTime.ParseExact(model.TakeOffDateMTakeOff, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                TowerData.CreationDate = Convert.ToDateTime(model.CreationDateMTakeOff);
                TowerData.UpdatingDate = model.UpdatingDateMTakeOff;

                TowerData.CreatedBy = model.CreatedByMTakeOff;
                TowerData.UpdatedBy = model.UpdatedByMTakeOff;

                TowerData.ATA = model.ATAMTakeOff;
                TowerData.ATD = model.ATDMTakeOff;
                TowerData.Status = model.StatusMTakeOff;
                TowerData.Note = model.NoteMTakeOff;
                TowerData.TripTypeId = model.TripTypeIdMTakeOff;

                TowerData.POB = model.POBMTakeOff;
                TowerData.QBD = model.QBDMTakeOff;
                TowerData.LandingPermission = model.LandingPermissionMTakeOff;

              
                if (model.IdMTakeOff > 0)
                {
                    //model.UpdatedBy = id;
                    //model.UpdatingDate = System.DateTime.Now;
                    if (db.TowerDatas.Any(x => x.AirLineId == model.AirLineIdMTakeOff && x.ATA == model.ATAMTakeOff && x.ATD == model.ATDMTakeOff && x.Date == TowerData.Date && x.Id != model.IdMTakeOff && x.FlightNo == model.FlightNoMTakeOff))
                    {
                        return Json(new { Message = "تمت اضافة الرحلة مسبقا", Title = "فشل", Status = "error" });
                    }

                    /////////////////////////////******* Start Check FlightType****//////////////////////////////////////
                    //AirPort Afrom = db.AirPorts.Single(x => x.Id == model.AirportIdFrom);
                    AirPort ATo = db.AirPorts.Single(x => x.Id == model.AirportIdToMTakeOff);
                    if (ATo.CountryId == 1)
                    {
                        // Domistic
                        TowerData.FlightTypeD = 2;
                    }
                    else
                    {    // InterNational
                        TowerData.FlightTypeD = 1;
                    }

                    AirPort AFrom = db.AirPorts.Single(x => x.Id == model.AirportIdFromMTakeOff);
                    if (AFrom.CountryId == 1)
                    {
                        // Domistic
                        TowerData.FlightTypeL = 2;
                    }
                    else
                    {    // InterNational
                        TowerData.FlightTypeL = 1;
                    }



                    TowerData.Status = 3;
                    TowerData.UpdatedBy = id;
                    TowerData.UpdatingDate = DateTime.Now;
                    TowerData.CompanyInfoId = companyId;
                    db.Entry(TowerData).State = System.Data.Entity.EntityState.Modified;

                    bool IsRev = db.OfficerDatas.Any(x => x.TowerDataId == model.IdMTakeOff);


                    bool res;
                    if (IsRev)
                    {
                        res = calculateFeesUpdate(TowerData);

                    }
                    else
                    {
                        res = calculateFees(TowerData, model.IdMTakeOff);
                    }



                    //bool resNyAndCupps = ChangeNyAndCupps(TowerData);


                    if (!res)
                    {
                        return Json(new { success = true, message = "CalculationFeesError" }, JsonRequestBehavior.AllowGet);
                    }


                    return Json(new { Message = "تمت عملية الإضافة  بنجاح", Title = "نجاح", Status = "success" });
                }



            }
            return Json(new { success = false, message = "error" }, JsonRequestBehavior.AllowGet);
        }













        public ActionResult CreateOrUpdateMakeArrived(AirMakeArrivedModel model)
        {
            var id = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(id);
            if (ModelState.IsValid)
            {
                TowerData TowerData = new TowerData();

                TowerData.Id = model.IdMArrived;
                TowerData.AirLineId = Convert.ToInt32(model.AirLineIdMArrived);
                TowerData.FlightNo = Convert.ToInt32(model.FlightNoMArrived);
                TowerData.AircraftRegId = Convert.ToInt32(model.AircraftRegIdMArrived);
                TowerData.AirportIdFrom = model.AirportIdFromMArrived;
                TowerData.AirportIdTo = model.AirportIdToMArrived;

               
                TowerData.LandingDate = DateTime.ParseExact(model.LandingDateMArrived, "yyyy-MM-dd", CultureInfo.InvariantCulture);



                TowerData.Date = DateTime.ParseExact(model.DateMArrived,"yyyy-MM-dd", CultureInfo.InvariantCulture);
                TowerData.TakeOffDate = DateTime.ParseExact(model.TakeOffDateMArrived, "yyyy-MM-dd", CultureInfo.InvariantCulture);




                TowerData.CreationDate = Convert.ToDateTime(model.CreationDateMArrived);
                //TowerData.CreationDate = model.CreationDateMArrived;
                //TowerData.CreationDate = DateTime.ParseExact(model.CreationDateMArrived, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                TowerData.UpdatingDate = model.UpdatingDateMArrived;

                TowerData.CreatedBy = model.CreatedByMArrived;
                TowerData.UpdatedBy = model.UpdatedByMArrived;

                TowerData.ATA = model.ATAMArrived;
                TowerData.ATD = model.ATDMArrived;
                TowerData.Status = model.StatusMArrived;
                TowerData.Note = model.NoteMArrived;
                TowerData.TripTypeId = model.TripTypeIdMArrived;

                TowerData.POB = model.POBMArrived;
                TowerData.QBD = model.QBDMArrived;
                TowerData.LandingPermission = model.LandingPermissionMArrived;

              
                if (model.IdMArrived > 0)
                {
                    //model.UpdatedBy = id;
                    //model.UpdatingDate = System.DateTime.Now;
                    if (db.TowerDatas.Any(x => x.AirLineId == model.AirLineIdMArrived && x.ATA == model.ATAMArrived && x.ATD == model.ATDMArrived && x.Date == TowerData.Date && x.Id != model.IdMArrived && x.FlightNo == model.FlightNoMArrived))
                    {
                        return Json(new { Message = "تمت اضافة الرحلة  مسبقا", Title = "فشل", Status = "error" });
                    }

                    /////////////////////////////******* Start Check FlightType****//////////////////////////////////////
                    //AirPort Afrom = db.AirPorts.Single(x => x.Id == model.AirportIdFrom);
                    AirPort ATo = db.AirPorts.Single(x => x.Id == model.AirportIdToMArrived);
                    if (ATo.CountryId == 1)
                    {
                        // Domistic
                        TowerData.FlightTypeD = 2;
                    }
                    else
                    {    // InterNational
                        TowerData.FlightTypeD = 1;
                    }

                    AirPort AFrom = db.AirPorts.Single(x => x.Id == model.AirportIdFromMArrived);
                    if (AFrom.CountryId == 1)
                    {
                        // Domistic
                        TowerData.FlightTypeL = 2;
                    }
                    else
                    {    // InterNational
                        TowerData.FlightTypeL = 1;
                    }



                    TowerData.Status = 3;
                    TowerData.UpdatedBy = id;
                    TowerData.UpdatingDate = DateTime.Now;
                    TowerData.CompanyInfoId = companyId;
                    db.Entry(TowerData).State = System.Data.Entity.EntityState.Modified;

                    bool IsRev = db.OfficerDatas.Any(x => x.TowerDataId == model.IdMArrived);


                    bool res;
                    if (IsRev)
                    {
                        res = calculateFeesUpdate(TowerData);

                    }
                    else
                    {
                        res = calculateFees(TowerData, model.IdMArrived);
                    }



                    //bool resNyAndCupps = ChangeNyAndCupps(TowerData);


                    if (!res)
                    {
                        return Json(new { success = true, message = "CalculationFeesError" }, JsonRequestBehavior.AllowGet);
                    }


                    return Json(new { Message = "تمت عملية الإضافة  بنجاح", Title = "نجاح", Status = "success" });
                }



            }
            return Json(new { success = false, message = "error" }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetDataForUpdateCo(int id)
        {
            TowerData TowerData = db.TowerDatas.Find(id);

            AirCompletedModel model = new AirCompletedModel();
            model.IdCompleted = TowerData.Id;
            model.AirLineIdCompleted = Convert.ToInt32(TowerData.AirLineId);
            model.FlightNoCompleted = Convert.ToInt32(TowerData.FlightNo);

            model.AircraftRegIdCompleted = Convert.ToInt32(TowerData.AircraftRegId);
            model.AirportIdToCompleted = TowerData.AirportIdTo;
            model.AirportIdFromCompleted = TowerData.AirportIdFrom;


            model.ATDCompleted = TowerData.ATD;
            model.ATDCompletedShow = ConvertToHHmm(TowerData.ATD.ToString());

            model.ATACompleted = TowerData.ATA;
            model.ATACompletedShow = ConvertToHHmm(TowerData.ATA.ToString());

            model.TripTypeIdCompleted = Convert.ToInt32(TowerData.TripTypeId);
        
            model.DateCompleted = Convert.ToDateTime(TowerData.Date).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            model.TakeOffDateCompleted = Convert.ToDateTime(TowerData.TakeOffDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            model.LandingDateCompleted = Convert.ToDateTime(TowerData.LandingDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);


            model.POBCompleted = TowerData.POB;
            model.QBDCompleted = TowerData.QBD;
            model.NoteCompleted = TowerData.Note;

            model.LandingPermissionCompleted = TowerData.LandingPermission;
            model.CreatedByCompleted = TowerData.CreatedBy;
            model.CreationDateCompleted = Convert.ToDateTime(TowerData.CreationDate).ToString();
            return Json(model, JsonRequestBehavior.AllowGet);

        }


        public ActionResult CreateOrUpdateCompleted(AirCompletedModel model)
        {
            var id = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(id);
            if (ModelState.IsValid)
            {
                TowerData TowerData = new TowerData();

                TowerData.Id = model.IdCompleted;
                TowerData.AirLineId = Convert.ToInt32(model.AirLineIdCompleted);
                TowerData.FlightNo = Convert.ToInt32(model.FlightNoCompleted);
                TowerData.AircraftRegId = Convert.ToInt32(model.AircraftRegIdCompleted);
                TowerData.AirportIdFrom = model.AirportIdFromCompleted;
                TowerData.AirportIdTo = model.AirportIdToCompleted;

                //TowerData.LandingDate = CommonUsed.GetDateD(model.LandingDateCompleted);
                TowerData.LandingDate = DateTime.ParseExact(model.LandingDateCompleted, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                TowerData.Date = DateTime.ParseExact(model.DateCompleted, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                //TowerData.Date = CommonUsed.GetDateD(model.DateCompleted);
                //TowerData.TakeOffDate = CommonUsed.GetDateD(model.TakeOffDateCompleted);
                TowerData.TakeOffDate = DateTime.ParseExact(model.TakeOffDateCompleted, "yyyy-MM-dd", CultureInfo.InvariantCulture);


                TowerData.CreationDate = Convert.ToDateTime(model.CreationDateCompleted);
                TowerData.UpdatingDate = model.UpdatingDateCompleted;

                TowerData.CreatedBy = model.CreatedByCompleted;
                TowerData.UpdatedBy = model.UpdatedByCompleted;

                TowerData.ATA = model.ATACompleted;
                TowerData.ATD = model.ATDCompleted;
                TowerData.Status = model.StatusCompleted;
                TowerData.Note = model.NoteCompleted;
                TowerData.TripTypeId = model.TripTypeIdCompleted;


                TowerData.POB = model.POBCompleted;
                TowerData.QBD = model.QBDCompleted;
                TowerData.LandingPermission = model.LandingPermissionCompleted;




              
                if (model.IdCompleted > 0)
                {
                    model.UpdatedByCompleted = id;
                    model.UpdatingDateCompleted = System.DateTime.Now;
                    if (db.TowerDatas.Any(x => x.AirLineId == model.AirLineIdCompleted && x.ATA == model.ATACompleted && x.ATD == model.ATDCompleted && x.Date == TowerData.Date && x.Id != model.IdCompleted && x.FlightNo == model.FlightNoCompleted))
                    {
                        return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
                    }

                    /////////////////////////////******* Start Check FlightType****//////////////////////////////////////
                    //AirPort Afrom = db.AirPorts.Single(x => x.Id == model.AirportIdFrom);
                    AirPort ATo = db.AirPorts.Single(x => x.Id == model.AirportIdToCompleted);
                    if (ATo.CountryId == 1)
                    {
                        // Domistic
                        TowerData.FlightTypeD = 2;
                    }
                    else
                    {    // InterNational
                        TowerData.FlightTypeD = 1;
                    }

                    AirPort AFrom = db.AirPorts.Single(x => x.Id == model.AirportIdFromCompleted);
                    if (AFrom.CountryId == 1)
                    {
                        // Domistic
                        TowerData.FlightTypeL = 2;
                    }
                    else
                    {    // InterNational
                        TowerData.FlightTypeL = 1;
                    }



                    TowerData.Status = 3;
                    TowerData.UpdatedBy = id;
                    TowerData.UpdatingDate = DateTime.Now;
                    TowerData.CompanyInfoId = companyId;
                    db.Entry(TowerData).State = System.Data.Entity.EntityState.Modified;

                    //OfficerData officerData = db.OfficerDatas.Where(x => x.TowerDataId == model.IdCompleted).FirstOrDefault();
                    bool IsRev = db.OfficerDatas.Any(x => x.TowerDataId == model.IdCompleted);


                    bool res;
                    if (IsRev)
                    {
                        res = calculateFeesUpdate(TowerData);

                    }
                    else
                    {
                        res = calculateFees(TowerData, model.IdCompleted);
                    }



                    //bool resNyAndCupps = ChangeNyAndCupps(TowerData);


                    if (!res)
                    {
                        return Json(new { success = true, message = "CalculationFeesError" }, JsonRequestBehavior.AllowGet);
                    }


                    return Json(new { Message = "تمت عملية التعديل  بنجاح", Title = "نجاح", Status = "success" });
                }



            }
            return Json(new { success = false, message = "error" }, JsonRequestBehavior.AllowGet);
        }


























        public ActionResult CreateOrUpdateTakeOff(AirTakeOffModel model)
        {
            var id = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(id);

            if (ModelState.IsValid)
            {
                TowerData TowerData = new TowerData();

                TowerData.Id = model.IdDe;
                TowerData.AirLineId = Convert.ToInt32(model.AirLineIdDe);
                TowerData.FlightNo = Convert.ToInt32(model.FlightNoDe);
                TowerData.AircraftRegId = Convert.ToInt32(model.AircraftRegIdDe);
                TowerData.AirportIdFrom = model.AirportIdFromDe;
                TowerData.AirportIdTo = model.AirportIdToDe;

                TowerData.LandingDate = null;
                TowerData.Date = DateTime.ParseExact(model.DateDe, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                //TowerData.Date = Convert.ToDateTime(model.DateDe);
                //TowerData.TakeOffDate = Convert.ToDateTime(model.TakeOffDateDe);
                TowerData.TakeOffDate = DateTime.ParseExact(model.TakeOffDateDe, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                TowerData.CreationDate = Convert.ToDateTime(model.CreationDateDe);
                TowerData.UpdatingDate = model.UpdatingDateDe;

                TowerData.CreatedBy = model.CreatedByDe;
                TowerData.UpdatedBy = model.UpdatedByDe;

                TowerData.ATA = model.ATADe;
                TowerData.ATD = model.ATDDe;
                TowerData.Note = model.NoteDe;
                TowerData.TripTypeId = model.TripTypeIdDe;

                TowerData.POB = model.POBDe;
                TowerData.QBD = model.QBDDe;
                TowerData.LandingPermission = model.LandingPermissionDe;

                int TowerId = Convert.ToInt32(db.SpLastTowerid().FirstOrDefault());



                ///---------------------------------------------------------------------


                if (model.IdDe > 0)
                {
                    TowerData.UpdatedBy = id;
                    TowerData.UpdatingDate = System.DateTime.Now;
                    if (db.TowerDatas.Any(x => x.AirLineId == model.AirLineIdDe && x.ATA == model.ATADe && x.ATD == model.ATDDe && x.Date == TowerData.Date && x.Id != model.IdDe && x.FlightNo == model.FlightNoDe))
                    {
                        return Json(new { Message = "هذا الرحلة  موجود  مسبقا", Title = "عملية الاضافة", Status = "error" });
                    }

                    /////////////////////////////******* Start Check FlightType****//////////////////////////////////////
                    // From Not Found When Aircraft Departure 
                    if (model.AirportIdToDe > 0)
                    {
                        AirPort ATo = db.AirPorts.Single(x => x.Id == model.AirportIdToDe);
                        if (ATo.CountryId == 1)
                        {
                            // Domistic
                            TowerData.FlightTypeD = 2;
                        }
                        else
                        {    // InterNational
                            TowerData.FlightTypeD = 1;
                        }
                    }
                    //// From Not Found When Aircraft Take Off
                    if (model.AirportIdFromDe > 0)
                    {
                        AirPort AFrom = db.AirPorts.Single(x => x.Id == model.AirportIdFromDe);
                        if (AFrom.CountryId == 1)
                        {
                            // Domistic
                            TowerData.FlightTypeL = 2;
                        }
                        else
                        {    // InterNational
                            TowerData.FlightTypeL = 1;
                        }
                    }

                    TowerData.Status = model.Status;
                    db.Entry(TowerData).State = System.Data.Entity.EntityState.Modified;
                    TowerData.CompanyInfoId = companyId;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Update" }, JsonRequestBehavior.AllowGet);
                }
                else
                {


                    if (db.TowerDatas.Any(x => x.AirLineId == model.AirLineIdDe && x.ATA == model.ATADe && x.ATD == model.ATDDe && x.Date == TowerData.Date && x.FlightNo == model.FlightNoDe))

                    {
                        //  return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
                        return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);

                    }

                    TowerData.CreatedBy = id;
                    TowerData.CreationDate = System.DateTime.Now;

                    /////////////////////////////******* Start Check FlightType******////////////////////////////////////////
                    // From Not Found When Aircraft Departure
                    if (model.AirportIdToDe > 0)
                    {
                        AirPort ATo = db.AirPorts.Single(x => x.Id == model.AirportIdToDe);
                        if (ATo.CountryId == 1)
                        {
                            // Domistic
                            TowerData.FlightTypeD = 2;
                        }
                        else
                        {    // InterNational
                            TowerData.FlightTypeD = 1;
                        }
                    }
                    // From Not Found When Aircraft Take Off

                    if (model.AirportIdFromDe > 0)
                    {
                        AirPort AFrom = db.AirPorts.Single(x => x.Id == model.AirportIdFromDe);
                        if (model.AirportIdFromDe > 0)
                        {
                            if (AFrom.CountryId == 1)
                            {
                                // Domistic
                                TowerData.FlightTypeL = 2;
                            }
                            else
                            {    // InterNational
                                TowerData.FlightTypeL = 1;
                            }
                        }
                    }
                    //****************************Start Check Status*****************************************

                    TowerData.Status = 1;


                    // ******************************End Check Status***********************************


                    TowerData.Id = TowerId;
                    TowerData.CompanyInfoId = companyId;
                    db.TowerDatas.Add(TowerData);
                    db.SaveChanges();

                    return Json(new { Message = "تمت عملية الإضافة  بنجاح", Title = "نجاح", Status = "success" });

                }

            }
            return Json(new { success = false, message = "error" }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetDataForUpdateDe(int id)
        {
            TowerData TowerData = db.TowerDatas.Find(id);

            AirTakeOffModel model = new AirTakeOffModel();
            model.IdDe = TowerData.Id;
            model.AirLineIdDe = Convert.ToInt32(TowerData.AirLineId);
            model.FlightNoDe = Convert.ToInt32(TowerData.FlightNo);

            model.AircraftRegIdDe = Convert.ToInt32(TowerData.AircraftRegId);
            model.AirportIdToDe = TowerData.AirportIdTo;

            model.ATDDe = TowerData.ATD;
            model.ATDDeShow = ConvertToHHmm(TowerData.ATD.ToString());



            model.TripTypeIdDe = Convert.ToInt32(TowerData.TripTypeId);
            model.DateDe = Convert.ToDateTime(TowerData.Date).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            model.TakeOffDateDe = Convert.ToDateTime(TowerData.TakeOffDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);


          



            model.POBDe = TowerData.POB;
            model.QBDDe = TowerData.QBD;
            model.NoteDe = TowerData.Note;

            model.LandingPermissionDe = TowerData.LandingPermission;
            model.CreatedByDe = TowerData.CreatedBy;
            model.CreationDateDe = Convert.ToDateTime(TowerData.CreationDate).ToString();
            return Json(model, JsonRequestBehavior.AllowGet);

        }


        public ActionResult UpdateTakeOff(AirTakeOffModel model)
        {
            var id = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(id);
            if (ModelState.IsValid)
            {
                TowerData TowerData = new TowerData();

                TowerData.Id = model.IdDe;
                TowerData.AirLineId = Convert.ToInt32(model.AirLineIdDe);
                TowerData.FlightNo = Convert.ToInt32(model.FlightNoDe);
                TowerData.AircraftRegId = Convert.ToInt32(model.AircraftRegIdDe);
                TowerData.AirportIdFrom = model.AirportIdFromDe;
                TowerData.AirportIdTo = model.AirportIdToDe;

                TowerData.Date = DateTime.ParseExact(model.DateDe, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                TowerData.TakeOffDate = DateTime.ParseExact(model.TakeOffDateDe, "yyyy-MM-dd", CultureInfo.InvariantCulture);



                TowerData.CreationDate = Convert.ToDateTime(model.CreationDateDe);
                TowerData.UpdatingDate = model.UpdatingDateDe;

                TowerData.CreatedBy = model.CreatedByDe;
                TowerData.UpdatedBy = model.UpdatedByDe;

                TowerData.ATA = model.ATADe;
                TowerData.ATD = model.ATDDe;
                TowerData.Note = model.NoteDe;
                TowerData.TripTypeId = model.TripTypeIdDe;

                TowerData.POB = model.POBDe;
                TowerData.QBD = model.QBDDe;
                TowerData.LandingPermission = model.LandingPermissionDe;
                TowerData.CreatedBy = model.CreatedByDe;

                if (model.IdDe > 0)
                {
                    TowerData.UpdatedBy = id;
                    TowerData.UpdatingDate = System.DateTime.Now;
                    if (db.TowerDatas.Any(x => x.AirLineId == model.AirLineIdDe && x.ATD == model.ATDDe && x.Date == TowerData.Date && x.Id != model.IdDe && x.FlightNo == model.FlightNoDe))
                    {
                        return Json(new { Message = "هذا الرحلة  موجود  مسبقا", Title = "عملية الاضافة", Status = "error" });
                    }

                    /////////////////////////////******* Start Check FlightType****//////////////////////////////////////
                    // From Not Found When Aircraft Departure 
                    if (model.AirportIdToDe > 0)
                    {
                        AirPort ATo = db.AirPorts.Single(x => x.Id == model.AirportIdToDe);
                        if (ATo.CountryId == 1)
                        {
                            // Domistic
                            TowerData.FlightTypeD = 2;
                        }
                        else
                        {    // InterNational
                            TowerData.FlightTypeD = 1;
                        }
                    }
                    //// From Not Found When Aircraft Take Off
                    if (model.AirportIdFromDe > 0)
                    {
                        AirPort AFrom = db.AirPorts.Single(x => x.Id == model.AirportIdFromDe);
                        if (AFrom.CountryId == 1)
                        {
                            // Domistic
                            TowerData.FlightTypeL = 2;
                        }
                        else
                        {    // InterNational
                            TowerData.FlightTypeL = 1;
                        }
                    }

                    TowerData.Status = 1;
                    TowerData.CompanyInfoId = companyId;
                    db.Entry(TowerData).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { Message = "تمت عملية التعديل  بنجاح", Title = "نجاح", Status = "success" });
                }

            }
            return Json(new { success = false, message = "error" }, JsonRequestBehavior.AllowGet);
        }








        [HttpPost]
        public ActionResult YourAction(string ATAAr)
        {
            if (ATAAr!=null)
            {
                // Process the ATA value as TimeSpan
               // TimeSpan ataTime = ATAAr.Value;

                return Json("Time received: " + ATAAr);
            }

            return Json("Error: No valid time received.");
        }




















        public ActionResult GetAirTakeOff()
        {
            var id = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(id);
            bool IsSingle = CommonUsed.getIsSingleAccounting(companyId);
            //IQueryable<TowerData> TowerData = db.TowerDatas.Where(x => DbFunctions.TruncateTime(x.CreationDate.Value).ToString() ==
            //DbFunctions.TruncateTime(now.Date).ToString());

            IQueryable<TowerData> TowerData = db.TowerDatas;

            WorkMouth = CommonUsed.GetMouth(id);
            WorkYear = CommonUsed.GetYear(id);

            TowerData = TowerData.Where(x => DbFunctions.TruncateTime(x.CreationDate.Value).ToString() ==
            DbFunctions.TruncateTime(now.Date).ToString() || DbFunctions.TruncateTime(x.TakeOffDate.Value).ToString() ==
            DbFunctions.TruncateTime(now.Date).ToString() || DbFunctions.TruncateTime(x.LandingDate.Value).ToString() ==
            DbFunctions.TruncateTime(now.Date).ToString() || x.Status == 2 || x.Status == 1);




            var TowerDatasList = new List<TowerDataViewModels>();

            foreach (var item in TowerData)
            {
                TowerDataViewModels tdm = new TowerDataViewModels();

                tdm.Id = item.Id;
                tdm.AirLineCode = item?.AirLine.Code;
                tdm.AirLineName = item?.AirLine.Name;
                //tdm.TripType = CommonUsed.GetFlightTypeName(item.OfficerData.TowerData.FlightTypeL, item.OfficerData.TowerData.FlightTypeD);
                tdm.TripType = CommonUsed.GetFlightTypeNameLanding(item.FlightTypeL);
                tdm.FlightNo = item?.AirLinesFlightTrip.FlightNo;
                tdm.AircraftRegName = item?.AircraftRegistration.Registration;
                tdm.Type = item?.AircraftRegistration.AircraftType.Type;
                tdm.MaxTaKeOffWeight = item?.AircraftRegistration.MaxTakoffWieght.ToString();
                tdm.Status = Convert.ToInt16(item.Status);
                tdm.StatusName = GetStatus(Convert.ToInt16(item.Status));
                //from --------------------------------------------------------------------------------------
                tdm.AirportIdFrom = CommonUsed.CheckAirportCode(Convert.ToInt32(item?.AirportIdFrom));

                if (item.AirportIdFrom > 0)
                {
                    tdm.FromArabicName = item.AirPort1.NameAr;
                }
                else
                {
                    tdm.FromArabicName = "لم تهبط";
                }
                //------------------------------------------------------------------------------
                if (item.AirportIdFrom > 0)
                {
                    if (Convert.ToInt32(item.AirPort1.CountryId) > 0)
                    {
                        tdm.FromArabicCountry = item.AirPort1.Country.NameAr;
                    }
                    else
                    {
                        tdm.FromArabicCountry = "غير محدد";
                    }
                }
                else { tdm.FromArabicCountry = "لم تهبط"; }

                //------------------------------------------------------------------------------
                //To --------------------------------------------------------------------------------------
                tdm.AirportIdTo = CommonUsed.CheckAirportCode(Convert.ToInt32(item?.AirportIdTo));
                if (item.AirportIdTo > 0)
                {
                    tdm.ToArabicName = item.AirPort.NameAr;
                }
                else
                {
                    tdm.ToArabicName = "لم تقلع";
                }
                if (item.AirportIdTo > 0)
                {
                    if (Convert.ToInt32(item.AirPort.CountryId) > 0)
                    {
                        tdm.ToArabicCountry = item.AirPort.Country.NameAr;
                    }
                    else
                    {
                        tdm.ToArabicCountry = "غير محدد";
                    }
                }
                else { tdm.FromArabicCountry = "لم تقلع"; }
                // tdm.ToArabicCountry = item.OfficerData.TowerData.AirPort.Country.NameAr;


                tdm.Date = CommonUsed.ToDisplayDate(item.Date);
                tdm.ATA = item.ATA.ToString();
                tdm.ATD = item.ATD.ToString();
                //tdm.POB = item.POB;
                //tdm.QBD = item.QBD;
                /// tdm.LandingPermission = item.LandingPermission;


                tdm.CreatedBy = CommonUsed.GetUserName(item.CreatedBy);
                tdm.CreationDate = item.CreationDate.ToString();
                tdm.UpdatedBy = CommonUsed.GetUserName(item.UpdatedBy);
                tdm.UpdatingDate = item.ToString();

                tdm.LandingDate = CommonUsed.ToDisplayDate(item.LandingDate);
                tdm.TakeOffDate = CommonUsed.ToDisplayDate(item.TakeOffDate);
                if (IsSingle) 
                {
                    tdm.IsStop = CommonUsed.IsEntryDateinRange2(item.Id, Role.GetRole(id));  // check date in range in claim table
                } else
                {
                    tdm.IsStop = CommonUsed.IsEntryDateinRange(Convert.ToDateTime(item.Date), Role.GetRole(id),
                  Convert.ToInt32(item.AirLineId), Convert.ToInt32(companyId));  // check date in range in claim table
                }
                
              
                tdm.AirportName = CommonUsed.getCompanyName(item.CompanyInfoId);


                ///////
                tdm.Size = item?.AircraftRegistration.AircraftType.AircraftSize.Size.ToString();
                RevenueDataViewModel r = new RevenueDataViewModel();
                r = GetRevenueData(tdm.Id);
                if (r.LandingFees != null)
                {
                    tdm.LandingFees = r.LandingFees.ToString();

                    tdm.ParkingFees = r.ParkingFees.ToString();
                    tdm.NightOperationFees = r.NightOperationFees.ToString();
                    tdm.NavigationFees = r.NavigationFees.ToString();
                    tdm.NoiseFees = r.NoiseFees.ToString();
                    tdm.SecurityFees = r.SecurityFees.ToString();
                }

                TowerDatasList.Add(tdm);
            }

            return Json(new { data = TowerDatasList.ToList() }, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult GetCompleted()
        //{
        //    var id = User.Identity.GetUserId();
        //    WorkMouth = CommonUsed.GetMouth(id);
        //    WorkYear = CommonUsed.GetYear(id);

        //    var companyInfo = Role.GetCompanyId(id);
        //    IQueryable<TowerData> TowerData = db.TowerDatas.OrderBy(x => x.Id).Where(x => x.Date.Value.Month == WorkMouth
        //     && x.Date.Value.Year == WorkYear && x.Status == 3);

        //    var TowerDatasList = new List<AirTakeOffViewModel>();

        //    foreach (var item in TowerData)
        //    {
        //        AirTakeOffViewModel tdm = new AirTakeOffViewModel();

        //        tdm.Id = item.Id;
        //        tdm.AirLineCode = item?.AirLine.Code;
        //        tdm.AirLineName = item?.AirLine.Name;
        //        //tdm.TripType = CommonUsed.GetFlightTypeName(item.OfficerData.TowerData.FlightTypeL, item.OfficerData.TowerData.FlightTypeD);
        //        tdm.TripType = CommonUsed.GetFlightTypeNameLanding(item.FlightTypeL);
        //        tdm.FlightNo = item?.AirLinesFlightTrip.FlightNo;
        //        tdm.AircraftRegName = item?.AircraftRegistration.Registration;
        //        tdm.Type = item?.AircraftRegistration.AircraftType.Type;
        //        tdm.MaxTaKeOffWeight = item?.AircraftRegistration.MaxTakoffWieght.ToString();
        //        tdm.Status = Convert.ToInt16(item.Status);
        //        tdm.StatusName = GetStatus(Convert.ToInt16(item.Status));
        //        //from --------------------------------------------------------------------------------------
        //        tdm.AirportIdFrom = CommonUsed.CheckAirportCode(Convert.ToInt32(item?.AirportIdFrom));

        //        if (item.AirportIdFrom > 0)
        //        {
        //            tdm.FromArabicName = item.AirPort1.NameAr;
        //        }
        //        else
        //        {
        //            tdm.FromArabicName = "لم تهبط";
        //        }
        //        //------------------------------------------------------------------------------
        //        if (item.AirportIdFrom > 0)
        //        {
        //            if (Convert.ToInt32(item.AirPort1.CountryId) > 0)
        //            {
        //                tdm.FromArabicCountry = item.AirPort1.Country.NameAr;
        //            }
        //            else
        //            {
        //                tdm.FromArabicCountry = "غير محدد";
        //            }
        //        }
        //        else { tdm.FromArabicCountry = "لم تهبط"; }

        //        //------------------------------------------------------------------------------
        //        //To --------------------------------------------------------------------------------------
        //        tdm.AirportIdTo = CommonUsed.CheckAirportCode(Convert.ToInt32(item?.AirportIdTo));
        //        if (item.AirportIdTo > 0)
        //        {
        //            tdm.ToArabicName = item.AirPort.NameAr;
        //        }
        //        else
        //        {
        //            tdm.ToArabicName = "لم تقلع";
        //        }
        //        if (item.AirportIdTo > 0)
        //        {
        //            if (Convert.ToInt32(item.AirPort.CountryId) > 0)
        //            {
        //                tdm.ToArabicCountry = item.AirPort.Country.NameAr;
        //            }
        //            else
        //            {
        //                tdm.ToArabicCountry = "غير محدد";
        //            }
        //        }
        //        else { tdm.FromArabicCountry = "لم تقلع"; }
        //        // tdm.ToArabicCountry = item.OfficerData.TowerData.AirPort.Country.NameAr;


        //        tdm.Date = Convert.ToDateTime(item.Date).ToString("MM/dd/yyyy");
        //        tdm.ATA = item.ATA.ToString();
        //        tdm.ATD = item.ATD.ToString();
        //        //tdm.POB = item.POB;
        //        //tdm.QBD = item.QBD;
        //        tdm.LandingPermission = item.LandingPermission;


        //        tdm.CreatedBy = CommonUsed.GetUserName(item.CreatedBy);
        //        tdm.CreationDate = item.CreationDate.ToString();
        //        tdm.UpdatedBy = CommonUsed.GetUserName(item.UpdatedBy);
        //        tdm.UpdatingDate = item.ToString();

        //        tdm.LandingDate = CommonUsed.CheckDate(item.LandingDate.ToString());
        //        tdm.TakeOffDate = CommonUsed.CheckDate(item.TakeOffDate.ToString());

        //        ///////
        //        tdm.Size = item?.AircraftRegistration.AircraftType.AircraftSize.Size.ToString();
        //        RevenueDataViewModel r = new RevenueDataViewModel();
        //        r = GetRevenueData(tdm.Id);
        //        if (r.LandingFees != null)
        //        {
        //            tdm.LandingFees = r.LandingFees.ToString();

        //            tdm.ParkingFees = r.ParkingFees.ToString();
        //            tdm.NightOperationFees = r.NightOperationFees.ToString();
        //            tdm.NavigationFees = r.NavigationFees.ToString();
        //            tdm.NoiseFees = r.NoiseFees.ToString();
        //            tdm.SecurityFees = r.SecurityFees.ToString();
        //        }
        //        TowerDatasList.Add(tdm);
        //    }

        //    return Json(new { data = TowerDatasList.ToList() }, JsonRequestBehavior.AllowGet);
        //}



        //public ActionResult GetCompletedFast()
        //{
        //    IQueryable<Revenue> Revenue;
        //    var id = User.Identity.GetUserId();

        //    var companyInfo = Role.GetCompanyId(id);

        //    WorkMouth = CommonUsed.GetMouth(id);
        //    WorkYear = CommonUsed.GetYear(id);

        //    Revenue = db.Revenues.OrderBy(x => x.Id).Where(x => x.OfficerData.TowerData.Date.Value.Month == WorkMouth
        //    && x.OfficerData.TowerData.Date.Value.Year == WorkYear);

        //    var TowerDatasList = new List<TowerDataViewFastModel>();

        //    foreach (var item in Revenue)
        //    {
        //        TowerDataViewFastModel tdm = new TowerDataViewFastModel();

        //        tdm.Id = item.OfficerData.TowerData.Id;
        //        tdm.AirLineCode = item?.OfficerData.TowerData.AirLine.Code;
        //        tdm.AirLineName = item?.OfficerData.TowerData.AirLine.Name;
        //        //tdm.TripType = CommonUsed.GetFlightTypeName(item.OfficerData.TowerData.FlightTypeL, item.OfficerData.TowerData.FlightTypeD);
        //        tdm.TripType = CommonUsed.GetFlightTypeNameLanding(item.OfficerData.TowerData.FlightTypeL);
        //        tdm.FlightNo = item?.OfficerData.TowerData.AirLinesFlightTrip.FlightNo;
        //        tdm.AircraftRegName = item?.OfficerData.TowerData.AircraftRegistration.Registration;
        //        tdm.Type = item?.OfficerData.TowerData.AircraftRegistration.AircraftType.Type;
        //        tdm.MaxTaKeOffWeight = item?.OfficerData.TowerData.AircraftRegistration.MaxTakoffWieght.ToString();

        //        //from --------------------------------------------------------------------------------------
        //        tdm.AirportIdFrom = CommonUsed.CheckAirportCode(Convert.ToInt32(item?.OfficerData.TowerData.AirportIdFrom));
        //        tdm.FromArabicName = item.OfficerData.TowerData.AirPort1.NameAr;

        //        if (Convert.ToInt32(item.OfficerData.TowerData.AirPort1.CountryId) > 0)
        //        {
        //            tdm.FromArabicCountry = item.OfficerData.TowerData.AirPort1.Country.NameAr;
        //        }
        //        else
        //        {
        //            tdm.FromArabicCountry = "غير محدد";
        //        }

        //        //To --------------------------------------------------------------------------------------
        //        tdm.AirportIdTo = CommonUsed.CheckAirportCode(Convert.ToInt32(item?.OfficerData.TowerData.AirportIdTo));
        //        tdm.ToArabicName = item.OfficerData.TowerData.AirPort.NameAr;

        //        if (Convert.ToInt32(item.OfficerData.TowerData.AirPort.CountryId) > 0)
        //        {
        //            tdm.ToArabicCountry = item.OfficerData.TowerData.AirPort.Country.NameAr;
        //        }
        //        else
        //        {
        //            tdm.ToArabicCountry = "غير محدد";
        //        }

        //        // tdm.ToArabicCountry = item.OfficerData.TowerData.AirPort.Country.NameAr;


        //        tdm.Date = Convert.ToDateTime(item.OfficerData.TowerData.Date).ToString("MM/dd/yyyy");
        //        tdm.ATA = item.OfficerData.TowerData.ATA.ToString();
        //        tdm.ATD = item.OfficerData.TowerData.ATD.ToString();
        //        //tdm.POB = item.POB;
        //        //tdm.QBD = item.QBD;
        //        tdm.LandingPermission = item.OfficerData.TowerData.LandingPermission;


        //        tdm.CreatedBy = CommonUsed.GetUserName(item.OfficerData.TowerData.CreatedBy);
        //        tdm.CreationDate = item.OfficerData.TowerData.CreationDate.ToString();
        //        tdm.UpdatedBy = CommonUsed.GetUserName(item.OfficerData.TowerData.UpdatedBy);
        //        tdm.UpdatingDate = item.OfficerData.TowerData.ToString();

        //        tdm.LandingDate = CommonUsed.CheckDate(item.OfficerData.TowerData.LandingDate.ToString());
        //        tdm.TakeOffDate = CommonUsed.CheckDate(item.OfficerData.TowerData.TakeOffDate.ToString());

        //        ///////
        //        tdm.Size = item?.OfficerData.TowerData.AircraftRegistration.AircraftType.AircraftSize.Size.ToString();
        //        tdm.LandingFees = item.LandingFees.ToString();

        //        tdm.ParkingFees = item.ParkingFees.ToString();
        //        tdm.NightOperationFees = item.NightOperationFees.ToString();
        //        tdm.NavigationFees = item.NavigationFees.ToString();
        //        tdm.NoiseFees = item.NoiseFees.ToString();
        //        tdm.SecurityFees = item.SecurityFees.ToString();
        //        tdm.Status = Convert.ToInt32(item.OfficerData.TowerData.Status);
        //        tdm.StatusName = GetStatus(Convert.ToInt32(item.OfficerData.TowerData.Status));
        //        TowerDatasList.Add(tdm);
        //    }

        //    return Json(new { data = TowerDatasList.ToList() }, JsonRequestBehavior.AllowGet);
        //}


        public ActionResult GetAllTowerDatasTakeOff()
        {
            var id = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(id);
            bool IsSingle = CommonUsed.getIsSingleAccounting(companyId);
            IQueryable<Revenue> Revenue = db.Revenues.Where(x => DbFunctions.TruncateTime(x.OfficerData.TowerData.CreationDate.Value).ToString() ==
            DbFunctions.TruncateTime(now.Date).ToString() && x.OfficerData.TowerData.CompanyInfoId == companyId);


            var TowerDatasList = new List<TowerDataViewModels>();

            foreach (var item in Revenue)
            {
                TowerDataViewModels tdm = new TowerDataViewModels();

                tdm.Id = item.OfficerData.TowerData.Id;
                tdm.AirLineCode = item?.OfficerData.TowerData.AirLine.Code;
                tdm.AirLineName = item?.OfficerData.TowerData.AirLine.Name;
                //tdm.TripType = CommonUsed.GetFlightTypeName(item.OfficerData.TowerData.FlightTypeL, item.OfficerData.TowerData.FlightTypeD);
                tdm.TripType = CommonUsed.GetFlightTypeNameLanding(item.OfficerData.TowerData.FlightTypeL);
                tdm.FlightNo = item?.OfficerData.TowerData.AirLinesFlightTrip.FlightNo;
                tdm.AircraftRegName = item?.OfficerData.TowerData.AircraftRegistration.Registration;
                tdm.Type = item?.OfficerData.TowerData.AircraftRegistration.AircraftType.Type;
                tdm.MaxTaKeOffWeight = item?.OfficerData.TowerData.AircraftRegistration.MaxTakoffWieght.ToString();

                //from --------------------------------------------------------------------------------------
                tdm.AirportIdFrom = CommonUsed.CheckAirportCode(Convert.ToInt32(item?.OfficerData.TowerData.AirportIdFrom));
                tdm.FromArabicName = item.OfficerData.TowerData.AirPort1.NameAr;

                if (Convert.ToInt32(item.OfficerData.TowerData.AirPort1.CountryId) > 0)
                {
                    tdm.FromArabicCountry = item.OfficerData.TowerData.AirPort1.Country.NameAr;
                }
                else
                {
                    tdm.FromArabicCountry = "غير محدد";
                }

                //To --------------------------------------------------------------------------------------
                tdm.AirportIdTo = CommonUsed.CheckAirportCode(Convert.ToInt32(item?.OfficerData.TowerData.AirportIdTo));
                tdm.ToArabicName = item.OfficerData.TowerData.AirPort.NameAr;

                if (Convert.ToInt32(item.OfficerData.TowerData.AirPort.CountryId) > 0)
                {
                    tdm.ToArabicCountry = item.OfficerData.TowerData.AirPort.Country.NameAr;
                }
                else
                {
                    tdm.ToArabicCountry = "غير محدد";
                }

                // tdm.ToArabicCountry = item.OfficerData.TowerData.AirPort.Country.NameAr;


                tdm.Date = CommonUsed.ToDisplayDate(item.OfficerData.TowerData.Date);
                tdm.ATA = item.OfficerData.TowerData.ATA.ToString();
                tdm.ATD = item.OfficerData.TowerData.ATD.ToString();
                //tdm.POB = item.POB;
                //tdm.QBD = item.QBD;
                tdm.LandingPermission = item.OfficerData.TowerData.LandingPermission;


                tdm.CreatedBy = CommonUsed.GetUserName(item.OfficerData.TowerData.CreatedBy);
                tdm.CreationDate = item.OfficerData.TowerData.CreationDate.ToString();
                tdm.UpdatedBy = CommonUsed.GetUserName(item.OfficerData.TowerData.UpdatedBy);
                tdm.UpdatingDate = item.OfficerData.TowerData.ToString();

                tdm.LandingDate = CommonUsed.ToDisplayDate(item.OfficerData.TowerData.LandingDate);
                tdm.TakeOffDate = CommonUsed.ToDisplayDate(item.OfficerData.TowerData.TakeOffDate);

                ///////
                tdm.Size = item?.OfficerData.TowerData.AircraftRegistration.AircraftType.AircraftSize.Size.ToString();
                tdm.LandingFees = item.LandingFees.ToString();

                tdm.ParkingFees = item.ParkingFees.ToString();
                tdm.NightOperationFees = item.NightOperationFees.ToString();
                tdm.NavigationFees = item.NavigationFees.ToString();
                tdm.NoiseFees = item.NoiseFees.ToString();
                tdm.SecurityFees = item.SecurityFees.ToString();
                if (IsSingle)
                {
                    tdm.IsStop = CommonUsed.IsEntryDateinRange2(item.Id, Role.GetRole(id));  // check date in range in claim table
                }
                else
                {
                    tdm.IsStop = CommonUsed.IsEntryDateinRange(Convert.ToDateTime(item.OfficerData.TowerData.Date), Role.GetRole(id),
                  Convert.ToInt32(item.OfficerData.TowerData.AirLineId), Convert.ToInt32(companyId));  // check date in range in claim table
                }
                tdm.AirportName = CommonUsed.getCompanyName(item.OfficerData.TowerData.CompanyInfoId);
                tdm.StatusName = GetStatus(Convert.ToInt16(item.OfficerData.TowerData.Status));


                TowerDatasList.Add(tdm);
            }

            return Json(new { data = TowerDatasList.ToList() }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetAllTowerDatas()
        {
            IQueryable<Revenue> Revenue;
            var id = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(id);
            bool IsSingle = CommonUsed.getIsSingleAccounting(companyId);
          

            WorkMouth = CommonUsed.GetMouth(id);
            WorkYear = CommonUsed.GetYear(id);

            Revenue = db.Revenues.OrderBy(x => x.Id).Where(x => x.OfficerData.TowerData.Date.Value.Month == WorkMouth
            && x.OfficerData.TowerData.Date.Value.Year == WorkYear && x.OfficerData.TowerData.CompanyInfoId == companyId
            && x.OfficerData.TowerData.Status == 3);

            var TowerDatasList = new List<TowerDataViewModels>();

            // var TowerDatasList = new List<TowerDataViewModels>();
            foreach (var item in Revenue)
            {
                TowerDataViewModels tdm = new TowerDataViewModels();

                tdm.Id = item.OfficerData.TowerData.Id;
                tdm.AirLineCode = item?.OfficerData.TowerData.AirLine.Code;
                tdm.AirLineName = item?.OfficerData.TowerData.AirLine.Name;
                tdm.TripType = CommonUsed.GetFlightTypeNameLanding(item.OfficerData.TowerData.FlightTypeL);
                tdm.FlightNo = item?.OfficerData.TowerData.AirLinesFlightTrip.FlightNo;
                tdm.AircraftRegName = item?.OfficerData.TowerData.AircraftRegistration.Registration;
                tdm.Type = item?.OfficerData.TowerData.AircraftRegistration.AircraftType.Type;
                tdm.MaxTaKeOffWeight = item?.OfficerData.TowerData.AircraftRegistration.MaxTakoffWieght.ToString();

                //from --------------------------------------------------------------------------------------
                tdm.AirportIdFrom = CommonUsed.CheckAirportCode(Convert.ToInt32(item?.OfficerData.TowerData.AirportIdFrom));
                tdm.FromArabicName = item.OfficerData.TowerData.AirPort1.NameAr;

                if (Convert.ToInt32(item.OfficerData.TowerData.AirPort1.CountryId) > 0)
                {
                    tdm.FromArabicCountry = item.OfficerData.TowerData.AirPort1.Country.NameAr;
                }
                else
                {
                    tdm.FromArabicCountry = "غير محدد";
                }

                //To --------------------------------------------------------------------------------------
                tdm.AirportIdTo = CommonUsed.CheckAirportCode(Convert.ToInt32(item?.OfficerData.TowerData.AirportIdTo));
                tdm.ToArabicName = item.OfficerData.TowerData.AirPort.NameAr;

                if (Convert.ToInt32(item.OfficerData.TowerData.AirPort.CountryId) > 0)
                {
                    tdm.ToArabicCountry = item.OfficerData.TowerData.AirPort.Country.NameAr;
                }
                else
                {
                    tdm.ToArabicCountry = "غير محدد";
                }

                // tdm.ToArabicCountry = item.OfficerData.TowerData.AirPort.Country.NameAr;


                tdm.Date = CommonUsed.ToDisplayDate(item.OfficerData.TowerData.Date);
                tdm.ATA = item.OfficerData.TowerData.ATA.ToString();
                tdm.ATD = item.OfficerData.TowerData.ATD.ToString();
                //tdm.POB = item.POB;
                //tdm.QBD = item.QBD;
                //  tdm.LandingPermission = item.OfficerData.TowerData.LandingPermission;


                tdm.CreatedBy = CommonUsed.GetUserName(item.OfficerData.TowerData.CreatedBy);
                //tdm.CreationDate = item.OfficerData.TowerData.CreationDate.ToString();
                //  tdm.UpdatedBy = CommonUsed.GetUserName(item.OfficerData.TowerData.UpdatedBy);
                //  tdm.UpdatingDate = item.OfficerData.TowerData.ToString();

                tdm.LandingDate = CommonUsed.ToDisplayDate(item.OfficerData.TowerData.LandingDate);
                tdm.TakeOffDate = CommonUsed.ToDisplayDate(item.OfficerData.TowerData.TakeOffDate);

                ///////
                tdm.Size = item?.OfficerData.TowerData.AircraftRegistration.AircraftType.AircraftSize.Size.ToString();
                tdm.LandingFees = item.LandingFees.ToString();

                tdm.ParkingFees = item.ParkingFees.ToString();
                tdm.NightOperationFees = item.NightOperationFees.ToString();
                tdm.NavigationFees = item.NavigationFees.ToString();
                tdm.NoiseFees = item.NoiseFees.ToString();
                tdm.SecurityFees = item.SecurityFees.ToString();
                if (IsSingle)
                {
                    tdm.IsStop = CommonUsed.IsEntryDateinRange2(item.Id, Role.GetRole(id));  // check date in range in claim table
                }
                else
                {
                    tdm.IsStop = CommonUsed.IsEntryDateinRange(Convert.ToDateTime(item.OfficerData.TowerData.Date), Role.GetRole(id),
                  Convert.ToInt32(item.OfficerData.TowerData.AirLineId), Convert.ToInt32(companyId));  // check date in range in claim table
                }
                tdm.AirportName = CommonUsed.getCompanyName(item.OfficerData.TowerData.CompanyInfoId);
                tdm.Status = Convert.ToInt32(item.OfficerData.TowerData.Status);
                tdm.StatusName = GetStatus(Convert.ToInt32(item.OfficerData.TowerData.Status));

                TowerDatasList.Add(tdm);
            }


            return Json(new { data = TowerDatasList.ToList() }, JsonRequestBehavior.AllowGet);
        }



















        //public ActionResult GetAirTakeOffParial(int? id)
        //{

        //    TowerData towerData = new TowerData();
        //    AirTakeOffModel Air = new AirTakeOffModel();
        //    if (id > 0)
        //    {

        //        towerData = db.TowerDatas.Find(id);

        //        Air.Id = towerData.Id;
        //        Air.AirLineId = Convert.ToInt32(towerData.AirLineId);
        //        Air.FlightNo = Convert.ToInt32(towerData.FlightNo);
        //        Air.AircraftRegId = Convert.ToInt32(towerData.AircraftRegId);
        //        Air.AirportIdFrom = towerData.AirportIdFrom;
        //        Air.AirportIdTo = towerData.AirportIdTo;

        //        Air.LandingDate = CommonUsed.ConvertToDdMMyyyy(towerData.LandingDate.ToString());
        //        Air.Date = CommonUsed.ConvertToDdMMyyyy(towerData.Date.ToString());
        //        Air.TakeOffDate = CommonUsed.ConvertToDdMMyyyy(towerData.TakeOffDate.ToString());

        //        Air.CreationDate = towerData.CreationDate;
        //        Air.UpdatingDate = towerData.UpdatingDate;

        //        Air.CreatedBy = towerData.CreatedBy;
        //        Air.UpdatedBy = towerData.UpdatedBy;

        //        Air.ATA = towerData.ATA;
        //        Air.ATD = towerData.ATD;
        //        Air.Status = 1;
        //        Air.Note = towerData.Note;
        //        Air.POB = towerData.POB;
        //        Air.QBD = towerData.QBD;
        //        Air.LandingPermission = towerData.LandingPermission;
        //    }

        //    ViewBag.AircraftRegId = new SelectList(db.AircraftRegistrations, "Id", "Registration", towerData.AircraftRegId);
        //    ViewBag.AirLineId = new SelectList(db.AirLines, "Id", "Code", towerData.AirLineId);
        //    ViewBag.FlightNo = new SelectList(db.AirLinesFlightTrips, "Id", "FlightNo", towerData.FlightNo);
        //    ViewBag.AirportIdTo = new SelectList(db.AirPorts, "Id", "Code", towerData.AirportIdTo);
        //    ViewBag.AirportIdFrom = new SelectList(db.AirPorts, "Id", "Code", towerData.AirportIdFrom);
        //    ViewBag.CompanyInfoId = new SelectList(db.CompanyInfoes, "Id", "NameAr", towerData.CompanyInfoId);
        //    ViewBag.TripTypeId = new SelectList(db.TripTypes, "Id", "Name", towerData.TripTypeId);

        //    Air.Status = 1;

        //    return PartialView("_AirTakeOff", Air);
        //}



















       












       




























        #endregion

        #region Completed
       

        #endregion




        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TowerData TowerData = db.TowerDatas.Find(id);
            if (TowerData == null)
            {
                return HttpNotFound();
            }

            OfficerData OfficerData = db.OfficerDatas.Where(x => x.TowerDataId == TowerData.Id).FirstOrDefault();
            Revenue Revenue = db.Revenues.Where(x => x.OfficerData.Id == OfficerData.Id).FirstOrDefault();

            if (OfficerData.Pax > 0 || OfficerData.TRNYCUPPS > 0)
            {
                return Json(new { success = true, message = "CuppsAndNyFound" }, JsonRequestBehavior.AllowGet);
            }

            if (Revenue.FreightFees > 0 || Revenue.ImpMailFees > 0 || Revenue.ExpMailFees > 0)
            {
                return Json(new { success = true, message = "MailAndFreightFound" }, JsonRequestBehavior.AllowGet);
            }

            if (Revenue.AmbolanceFees > 0 || Revenue.FireFightingFees > 0)
            {
                return Json(new { success = true, message = "CarServiceFound" }, JsonRequestBehavior.AllowGet);
            }

            if (Revenue.PushBackFees > 0)
            {
                return Json(new { success = true, message = "PushPackFound" }, JsonRequestBehavior.AllowGet);
            }

            if (Revenue != null)
            {
                var RevenueDelete = db.Revenues.Find(Revenue.Id);
                db.Revenues.Remove(RevenueDelete);
            }

            if (OfficerData != null)
            {
                var OfficerDataDelete = db.OfficerDatas.Find(OfficerData.Id);
                db.OfficerDatas.Remove(OfficerDataDelete);
            }

            if (TowerData != null)
            {
                var TowerDataDelete = db.TowerDatas.Find(id);
                db.TowerDatas.Remove(TowerDataDelete);
            }






            //if (db.OfficerDatas.Any(x => x.TowerDataId == id))
            //{
            //    return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
            //}





            db.SaveChanges();
            return Json(new { success = true, message = "Delete" }, JsonRequestBehavior.AllowGet);
        }




       



        










        public ActionResult DeleteFromTower(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TowerData TowerData = db.TowerDatas.Find(id);
            if (TowerData == null)
            {
                return HttpNotFound();
            }


            
           

            var TowerDataDelete = db.TowerDatas.Find(id);
            db.TowerDatas.Remove(TowerDataDelete);
            db.SaveChanges();
            return Json(new { success = true, message = "Delete" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteCompleted(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TowerData TowerData = db.TowerDatas.Find(id);
            if (TowerData == null)
            {
                return HttpNotFound();
            }
            bool IsHasOfficer = db.OfficerDatas.Any(x => x.TowerDataId == TowerData.Id);
            if (!IsHasOfficer)
            {
                var TowerDataDelete = db.TowerDatas.Find(id);
                db.TowerDatas.Remove(TowerDataDelete);
                db.SaveChanges();
                return Json(new { success = true, message = "Delete" }, JsonRequestBehavior.AllowGet);
            }

            OfficerData OfficerData = db.OfficerDatas.Where(x => x.TowerDataId == TowerData.Id).FirstOrDefault();
            bool IsHasRevenue = db.Revenues.Any(x => x.OfficerData.Id == OfficerData.Id);
            if (!IsHasRevenue)
            {
                var TowerDataDelete = db.TowerDatas.Find(id);

                var OfficerDataDelete = db.OfficerDatas.Where(x => x.TowerDataId == TowerDataDelete.Id).FirstOrDefault();

                db.OfficerDatas.Remove(OfficerDataDelete);
                db.TowerDatas.Remove(TowerDataDelete);
                db.SaveChanges();
                return Json(new { success = true, message = "Delete" }, JsonRequestBehavior.AllowGet);
            }
            Revenue Revenue = db.Revenues.Where(x => x.OfficerData.Id == OfficerData.Id).FirstOrDefault();

            if (OfficerData.Pax > 0 || OfficerData.TRNYCUPPS > 0)
            {
                return Json(new { success = true, message = "CuppsAndNyFound" }, JsonRequestBehavior.AllowGet);
            }

            if (Revenue.FreightFees > 0 || Revenue.ImpMailFees > 0 || Revenue.ExpMailFees > 0)
            {
                return Json(new { success = true, message = "MailAndFreightFound" }, JsonRequestBehavior.AllowGet);
            }

            if (Revenue.AmbolanceFees > 0 || Revenue.FireFightingFees > 0)
            {
                return Json(new { success = true, message = "CarServiceFound" }, JsonRequestBehavior.AllowGet);
            }

            if (Revenue.PushBackFees > 0)
            {
                return Json(new { success = true, message = "PushPackFound" }, JsonRequestBehavior.AllowGet);
            }

            if (Revenue != null)
            {
                var RevenueDelete = db.Revenues.Find(Revenue.Id);
                db.Revenues.Remove(RevenueDelete);
            }

            if (OfficerData != null)
            {
                var OfficerDataDelete = db.OfficerDatas.Find(OfficerData.Id);
                db.OfficerDatas.Remove(OfficerDataDelete);
            }

            if (TowerData != null)
            {
                var TowerDataDelete = db.TowerDatas.Find(id);
                db.TowerDatas.Remove(TowerDataDelete);
            }


            //if (db.OfficerDatas.Any(x => x.TowerDataId == id))
            //{
            //    return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
            //}
            try {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                string d = ex.Message;
            }
           
            return Json(new { success = true, message = "Delete" }, JsonRequestBehavior.AllowGet);
        }













        #region Calcutate And Classes
        public string GetStatus(int Status)
        {
            string StatusName = "";
            if (Status == 1)
            {
                StatusName = "Departure";

            }
            if (Status == 2)
            {
                StatusName = "Arrived";

            }
            if (Status == 3)
            {
                StatusName = "Completed";

            }
            return StatusName;
        }


        private bool calculateFeesUpdate(TowerData model)
        {
            bool res = false;

            OfficerData OfficerData = db.OfficerDatas.FirstOrDefault(x => x.TowerDataId == model.Id);

            decimal wt = Convert.ToDecimal(db.AircraftRegistrations.Single(x => x.Id == model.AircraftRegId).MaxTakoffWieght);
            decimal val = cal.LandingFees(wt, Convert.ToInt32(model.FlightTypeL));

           
                //Update
                Revenue rev = db.Revenues.FirstOrDefault(x => x.OfficerDataId == OfficerData.Id);
                rev.LandingFees = val;
                rev.NightOperationFees = cal.NightOperationFees(model.ATA.Value.Hours, model.ATD.Value.Hours, val);
                rev.OpenAireportOvertimeFees = cal.OpenAireportOvertimeFees(model.ATA.Value.Hours, val);
                rev.NavigationFees = cal.NavigationFees(val);
                var AirLine = db.AirLines.FirstOrDefault(x => x.Id == model.AirLineId);
                rev.ParkingFees = cal.ParkingFeestowerId(model.Id, Convert.ToDateTime(model.LandingDate), Convert.ToDateTime(model.TakeOffDate), model.ATA.Value, model.ATD.Value, Convert.ToBoolean(AirLine.IsLocalCompany));
                rev.NoiseFees = cal.NoiseFees(wt);
                rev.SecurityFees = cal.SecurityFees(wt, Convert.ToInt32(model.FlightTypeL), Convert.ToBoolean(AirLine.IsLocalCompany));
                if (ModelState.IsValid)
                {
                    db.Entry(rev).State = EntityState.Modified;
                    db.SaveChanges();
                    res = true;
                }
           

            return res;
        }


        private bool ChangeNyAndCupps(TowerData model)
        {
            var id = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(id);
            if (model.Id > 0)
            {

                OfficerData OfficerData = db.OfficerDatas.Where(x => x.TowerDataId == model.Id).FirstOrDefault();
                if (OfficerData != null && (OfficerData.Pax > 0 || OfficerData.TRNYCUPPS > 0))
                {
                    Revenue Revenue = db.Revenues.Where(x => x.OfficerDataId == OfficerData.Id).FirstOrDefault();

                    if (Revenue != null)
                    {


                        /// update
                        bool IsLocalCompany = Convert.ToBoolean(model.AirLine.IsLocalCompany);

                        if (model.FlightTypeD == 2/* && TowerData.FlightTypeL == 2*/)
                        {
                            //Domestic Cupps
                            decimal DomeNYFees = cal.DomeNYFees(Convert.ToInt32(OfficerData.Pax), IsLocalCompany);

                            db.Revenues.SingleOrDefault(x => x.Id == Revenue.Id).DomeNYFees = DomeNYFees;

                            decimal DomeCUPPSFees = cal.DomeCUPPSFees(Convert.ToInt32(OfficerData.Pax), Convert.ToInt32(OfficerData.TRNYCUPPS), Convert.ToBoolean(OfficerData.IsNotUsedCUPPS));

                            db.Revenues.SingleOrDefault(x => x.Id == Revenue.Id).DomeCUPPSFees = DomeCUPPSFees;
                            /// Zeros Inter
                            db.Revenues.SingleOrDefault(x => x.Id == Revenue.Id).InterCUPPSFees = 0;
                            db.Revenues.SingleOrDefault(x => x.Id == Revenue.Id).InterNYFees = 0;
                        }
                        else
                        {
                            //international Cupps

                            decimal InterCUPPSFees = cal.InterCUPPSFees(Convert.ToInt32(OfficerData.Pax), Convert.ToInt32(OfficerData.TRNYCUPPS), Convert.ToBoolean(OfficerData.IsNotUsedCUPPS));

                            db.Revenues.SingleOrDefault(x => x.Id == Revenue.Id).InterCUPPSFees = InterCUPPSFees;


                            decimal InterNYFees = cal.InterNYFees(Convert.ToInt32(OfficerData.Pax), IsLocalCompany, companyId);

                            db.Revenues.SingleOrDefault(x => x.Id == Revenue.Id).InterNYFees = InterNYFees;
                            /// Zeros Inter
                            db.Revenues.SingleOrDefault(x => x.Id == Revenue.Id).DomeCUPPSFees = 0;
                            db.Revenues.SingleOrDefault(x => x.Id == Revenue.Id).DomeNYFees = 0;
                        }

                        db.Entry(OfficerData).State = System.Data.Entity.EntityState.Modified;
                        //   cal.LandingFees(model.Id);
                        db.SaveChanges();
                        return true;







                    }
                    return false;

                }
                return false;
            }
            return false;
        }


        private bool calculateFees(TowerData model, int TowerId)
        {

            bool res = true;
           
            TowerData td = db.TowerDatas.Find(TowerId);
            OfficerData off = new OfficerData();
            off.TowerDataId = td.Id;
            off.IsCurrentAccount = true;
           
            int OfficerId = Convert.ToInt32(db.SpLastOfficerid().FirstOrDefault());
            off.Id = OfficerId;

            decimal wt = Convert.ToDecimal(db.AircraftRegistrations.Single(x => x.Id == model.AircraftRegId).MaxTakoffWieght);
            decimal val = cal.LandingFees(wt, Convert.ToInt32(model.FlightTypeL));


            int RevenuesId = Convert.ToInt32(db.SpLastRevenceid().FirstOrDefault());
                //Insert new one
                Revenue rev = new Revenue();
                rev.Id = RevenuesId;


                rev.OfficerDataId = OfficerId;
                rev.LandingFees = val;
                rev.NightOperationFees = cal.NightOperationFees(model.ATA.Value.Hours, model.ATD.Value.Hours, val);
                rev.OpenAireportOvertimeFees = cal.OpenAireportOvertimeFees(model.ATA.Value.Hours, val);
                rev.NavigationFees = cal.NavigationFees(val);
                //rev.ParkingFees = cal.ParkingFees(off1.Id);
                var AirLine = db.AirLines.FirstOrDefault(x => x.Id == model.AirLineId);
                rev.ParkingFees = cal.ParkingFeestowerId(TowerId, Convert.ToDateTime(model.LandingDate), Convert.ToDateTime(model.TakeOffDate), model.ATA.Value, model.ATD.Value, Convert.ToBoolean(AirLine.IsLocalCompany));
                rev.NoiseFees = cal.NoiseFees(wt);

                rev.SecurityFees = cal.SecurityFees(wt, Convert.ToInt32(model.FlightTypeL), Convert.ToBoolean(AirLine.IsLocalCompany));


            //---------------------------------------------
            db.OfficerDatas.Add(off);
            db.Revenues.Add(rev);

            db.SaveChanges();

                res = true;
            

            return res;
        }

        public RevenueDataViewModel GetRevenueData(int Id)
        {
            RevenueDataViewModel model = new RevenueDataViewModel();
            TowerData TowerData = db.TowerDatas.Find(Id);
            if (TowerData != null)
            {
                OfficerData off = db.OfficerDatas.Where(x => x.TowerDataId == TowerData.Id).FirstOrDefault();
                if (off != null)
                {
                    Revenue Revenue = db.Revenues.Where(x => x.OfficerDataId == off.Id).FirstOrDefault();
                    if (Revenue != null)
                    {
                        model.LandingFees = Revenue.LandingFees.ToString();
                        model.ParkingFees = Revenue.ParkingFees.ToString();
                        model.NavigationFees = Revenue.NavigationFees.ToString();
                        model.NoiseFees = Revenue.NoiseFees.ToString();
                        model.NightOperationFees = Revenue.NightOperationFees.ToString();
                        model.SecurityFees = Revenue.SecurityFees.ToString();
                        return model;
                    }
                    return model;
                }
                return model;
            }
            return model;
        }








        #endregion

        #region Report




        public ActionResult DailyMovement()
        {
            return View();
        }








        public ActionResult GetDaily(int? page, int? Get, int? Search, int? Count
         , DateTime? DateDaily)
        {
           
            // TowerData = db.TowerDatas.OrderBy(x => x.Id);

            ViewBag.DateDaily = DateDaily;
            var TowerDataList = new List<PrintDailyViewModel>();


            if (Get > 0)
            {


                int N = 0;
                IQueryable<TowerData> TowerDataCom = db.TowerDatas.OrderBy(x => x.Id);
                TowerDataCom = TowerDataCom.Where(t => DbFunctions.TruncateTime(t.Date) == DbFunctions.TruncateTime(DateDaily));

                ViewBag.Date = Convert.ToDateTime(DateDaily).ToString("MM/dd/yyyy");

                ///***********************************************************************************


                foreach (var item in TowerDataCom)
                {
                    PrintDailyViewModel L = new PrintDailyViewModel();
                    L.Id = item.Id;
                    N = N + 1;
                    L.N = N;
                    L.Call = item?.AirLine.Code;
                    L.FlightNo = item?.AirLinesFlightTrip.FlightNo;
                    L.Reg = item?.AircraftRegistration.Registration;
                    L.Operation = item?.AirLine.Name;
                    L.Type = item?.AircraftRegistration.AircraftType.Type;
                    if (item.AirportIdFrom > 0)
                    {
                        L.From = item.AirPort1.Code;
                    }
                    if (item.AirportIdTo > 0)
                    {
                        L.To = item.AirPort.Code;
                    }

                    L.ATA = item.ATA.ToString();
                    L.ATD = item.ATD.ToString();
                    L.POB = item.POB.ToString();
                    L.QBD = item.QBD.ToString();
                    L.LandingPermission = item.LandingPermission;
                    if (Convert.ToDateTime(item.Date).Date != Convert.ToDateTime(item.TakeOffDate).Date)
                    {
                        L.ATD = "";
                        L.To = "";
                    }
                    if (Convert.ToDateTime(item.Date).Date != Convert.ToDateTime(item.LandingDate).Date)
                    {
                        L.ATA = "";
                        L.From = "";
                    }
                    TowerDataList.Add(L);
                }
                IQueryable<TowerData> TowerDataTakeOff = db.TowerDatas.OrderBy(x => x.Id).Where(t => DbFunctions.TruncateTime(t.TakeOffDate) == DbFunctions.TruncateTime(DateDaily));
                TowerDataTakeOff = TowerDataTakeOff.Where(t => DbFunctions.TruncateTime(t.Date) != DbFunctions.TruncateTime(DateDaily));

                foreach (var item in TowerDataTakeOff)
                {
                    PrintDailyViewModel L = new PrintDailyViewModel();
                    L.Id = item.Id;
                    N = N + 1;
                    L.N = N;
                    L.Call = item?.AirLine.Code;
                    L.FlightNo = item?.AirLinesFlightTrip.FlightNo;
                    L.Reg = item?.AircraftRegistration.Registration;
                    L.Operation = item?.AirLine.Name;
                    L.Type = item?.AircraftRegistration.AircraftType.Type;
                    if (item.AirportIdFrom > 0)
                        //{
                        //    L.From = item.AirPort1.Code;
                        //}
                        if (item.AirportIdTo > 0)
                        {
                            L.To = item.AirPort.Code;
                        }

                    // L.ATA = item.ATA.ToString();
                    L.ATD = item.ATD.ToString();
                    L.POB = item.POB.ToString();
                    L.QBD = item.QBD.ToString();
                    L.LandingPermission = item.LandingPermission;

                    L.ATA = "";
                    L.From = "";

                    TowerDataList.Add(L);
                }
                IQueryable<TowerData> TowerDataLandingDate = db.TowerDatas.OrderBy(x => x.Id).Where(t => DbFunctions.TruncateTime(t.LandingDate) == DbFunctions.TruncateTime(DateDaily));
                TowerDataLandingDate = TowerDataLandingDate.Where(t => DbFunctions.TruncateTime(t.Date) != DbFunctions.TruncateTime(DateDaily));

                foreach (var item in TowerDataLandingDate)
                {
                    PrintDailyViewModel L = new PrintDailyViewModel();
                    L.Id = item.Id;
                    N = N + 1;
                    L.N = N;
                    L.Call = item?.AirLine.Code;
                    L.FlightNo = item?.AirLinesFlightTrip.FlightNo;
                    L.Reg = item?.AircraftRegistration.Registration;
                    L.Operation = item?.AirLine.Name;
                    L.Type = item?.AircraftRegistration.AircraftType.Type;
                    if (item.AirportIdFrom > 0)
                    {
                        L.From = item.AirPort1.Code;
                    }
                    //if (item.AirportIdTo > 0)
                    //    {
                    //        L.To = item.AirPort.Code;
                    //    }

                    L.ATA = item.ATA.ToString();
                    //L.ATD = item.ATD.ToString();
                    L.POB = item.POB.ToString();
                    L.QBD = item.QBD.ToString();
                    L.LandingPermission = item.LandingPermission;

                    L.ATD = "";
                    L.To = "";

                    TowerDataList.Add(L);
                }




                //////////////////////////////////////////////////////////////////////////////////////////////////
                if (Search == 1)
                {

                    if (TowerDataList != null)
                    {

                        int ItemCount = 0;
                        //List<decimal> Amountlist = new List<decimal>();
                        //List<decimal> Allowancelist = new List<decimal>();


                        ItemCount = TowerDataList.Count();
                        ViewBag.ItemCount = ItemCount;

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
                TowerDataList = null;
                ViewBag.ItemCount = 0;
            }

            int pageSize = 20;
            int PageNumber = (page ?? 1);


            if (TowerDataList == null)
            {
                
                return PartialView("_GetDaily", DailyList.ToPagedList(PageNumber, pageSize));
            }
            else
            {

                return PartialView("_GetDaily", TowerDataList.ToPagedList(PageNumber, pageSize));
            }
        }


        public ActionResult PrintDataDaily(DateTime? DateDaily)
        {
            var id = User.Identity.GetUserId();
           // RoleController role = new RoleController();
          
           
            string UserName = CommonUsed.GetUserName(id);
            int N = 0;
            IQueryable<TowerData> TowerDataCom = db.TowerDatas.OrderBy(x => x.Id);
            TowerDataCom = TowerDataCom.Where(t => DbFunctions.TruncateTime(t.Date) == DbFunctions.TruncateTime(DateDaily));
          
            ViewBag.Date = Convert.ToDateTime(DateDaily).ToString("MM/dd/yyyy");

            ///***********************************************************************************
            var TowerDataList = new List<PrintDailyViewModel>();
          
            foreach (var item in TowerDataCom)
            {
                PrintDailyViewModel L = new PrintDailyViewModel();
                L.Id = item.Id;
                N = N + 1;
                L.N = N;
                L.Call = item?.AirLine.Code;
                L.FlightNo = item?.AirLinesFlightTrip.FlightNo;
                L.Reg = item?.AircraftRegistration.Registration;
                L.Operation = item?.AirLine.Name;
                L.Type = item?.AircraftRegistration.AircraftType.Type;
                if (item.AirportIdFrom > 0)
                {
                    L.From = item.AirPort1.Code;
                }
                if (item.AirportIdTo > 0)
                {
                    L.To = item.AirPort.Code;
                }

                L.ATA = item.ATA.ToString();
                L.ATD = item.ATD.ToString();
                L.POB = item.POB.ToString();
                L.QBD = item.QBD.ToString();
                L.LandingPermission = item.LandingPermission;
                if (Convert.ToDateTime(item.Date).Date != Convert.ToDateTime(item.TakeOffDate).Date) 
                {
                    L.ATD = "";
                    L.To = "";
                }
                if (Convert.ToDateTime(item.Date).Date != Convert.ToDateTime(item.LandingDate).Date)
                {
                    L.ATA = "";
                    L.From = "";
                }
                TowerDataList.Add(L);
            }
            IQueryable<TowerData> TowerDataTakeOff = db.TowerDatas.OrderBy(x => x.Id).Where(t => DbFunctions.TruncateTime(t.TakeOffDate) == DbFunctions.TruncateTime(DateDaily));
            TowerDataTakeOff = TowerDataTakeOff.Where(t => DbFunctions.TruncateTime(t.Date) != DbFunctions.TruncateTime(DateDaily));

            foreach (var item in TowerDataTakeOff)
            {
                PrintDailyViewModel L = new PrintDailyViewModel();
                L.Id = item.Id;
                N = N + 1;
                L.N = N;
                L.Call = item?.AirLine.Code;
                L.FlightNo = item?.AirLinesFlightTrip.FlightNo;
                L.Reg = item?.AircraftRegistration.Registration;
                L.Operation = item?.AirLine.Name;
                L.Type = item?.AircraftRegistration.AircraftType.Type;
                if (item.AirportIdFrom > 0)
                //{
                //    L.From = item.AirPort1.Code;
                //}
                if (item.AirportIdTo > 0)
                {
                    L.To = item.AirPort.Code;
                }

               // L.ATA = item.ATA.ToString();
                L.ATD = item.ATD.ToString();
                L.POB = item.POB.ToString();
                L.QBD = item.QBD.ToString();
                L.LandingPermission = item.LandingPermission;
               
                    L.ATA = "";
                    L.From = "";
                
                TowerDataList.Add(L);
            }
            IQueryable<TowerData> TowerDataLandingDate = db.TowerDatas.OrderBy(x => x.Id).Where(t => DbFunctions.TruncateTime(t.LandingDate) == DbFunctions.TruncateTime(DateDaily));
            TowerDataLandingDate = TowerDataLandingDate.Where(t => DbFunctions.TruncateTime(t.Date) != DbFunctions.TruncateTime(DateDaily));

            foreach (var item in TowerDataLandingDate)
            {
                PrintDailyViewModel L = new PrintDailyViewModel();
                L.Id = item.Id;
                N = N + 1;
                L.N = N;
                L.Call = item?.AirLine.Code;
                L.FlightNo = item?.AirLinesFlightTrip.FlightNo;
                L.Reg = item?.AircraftRegistration.Registration;
                L.Operation = item?.AirLine.Name;
                L.Type = item?.AircraftRegistration.AircraftType.Type;
                if (item.AirportIdFrom > 0)
                {
                    L.From = item.AirPort1.Code;
                }
                //if (item.AirportIdTo > 0)
                //    {
                //        L.To = item.AirPort.Code;
                //    }

                L.ATA = item.ATA.ToString();
                //L.ATD = item.ATD.ToString();
                L.POB = item.POB.ToString();
                L.QBD = item.QBD.ToString();
                L.LandingPermission = item.LandingPermission;

                L.ATD = "";
                L.To = "";

                TowerDataList.Add(L);
            }












            return View(TowerDataList);

        }




        #endregion


        static string ConvertToHHmm(string time)
        {
            if (time !="")
            {
                // Split the time string and take the first two parts (HH:mm)
                var parts = time.Split(':');
                if (parts.Length >= 2)
                {
                    return $"{parts[0]}:{parts[1]}"; // HH:mm format
                }
                else { return ""; }

            }
            else { return ""; }
            
        }




        #region Report All 




        public ActionResult DailyMovementAll()
        {
            return View();
        }








        public ActionResult GetDailyAll(int? page, int? Get, int? Search, int? Count
         , DateTime? DateDaily)
        {
            var id = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(id);

            // TowerData = db.TowerDatas.OrderBy(x => x.Id);

            ViewBag.DateDaily = DateDaily;
            var TowerDataList = new List<PrintDailyViewModel>();


            if (Get > 0)
            {


                int N = 0;
                IQueryable<TowerData> TowerDataCom = db.TowerDatas.Where(q=> q.CompanyInfoId == companyId).OrderBy(x => x.Id);
                TowerDataCom = TowerDataCom.Where(t => DbFunctions.TruncateTime(t.Date) == DbFunctions.TruncateTime(DateDaily));

                ViewBag.Date = Convert.ToDateTime(DateDaily).ToString("MM/dd/yyyy");

                ///***********************************************************************************


                foreach (var item in TowerDataCom)
                {
                    PrintDailyViewModel L = new PrintDailyViewModel();
                    L.Id = item.Id;
                    N = N + 1;
                    L.N = N;
                    L.Call = item?.AirLine.Code;
                    L.FlightNo = item?.AirLinesFlightTrip.FlightNo;
                    L.Reg = item?.AircraftRegistration.Registration;
                    L.Operation = item?.AirLine.Name;
                    L.Type = item?.AircraftRegistration.AircraftType.Type;
                    if (item.AirportIdFrom > 0)
                    {
                        L.From = item.AirPort1.Code;
                    }
                    if (item.AirportIdTo > 0)
                    {
                        L.To = item.AirPort.Code;
                    }

                    L.ATA = item.ATA.ToString();
                    L.ATD = item.ATD.ToString();
                    L.POB = item.POB.ToString();
                    L.QBD = item.QBD.ToString();
                    L.LandingPermission = item.LandingPermission;
                    if (Convert.ToDateTime(item.Date).Date != Convert.ToDateTime(item.TakeOffDate).Date)
                    {
                        L.ATD = "";
                        L.To = "";
                    }
                    if (Convert.ToDateTime(item.Date).Date != Convert.ToDateTime(item.LandingDate).Date)
                    {
                        L.ATA = "";
                        L.From = "";
                    }
                    TowerDataList.Add(L);
                }
                IQueryable<TowerData> TowerDataTakeOff = db.TowerDatas.OrderBy(x => x.Id).Where(t => DbFunctions.TruncateTime(t.TakeOffDate) == DbFunctions.TruncateTime(DateDaily) && t.CompanyInfoId == companyId);
                TowerDataTakeOff = TowerDataTakeOff.Where(t => DbFunctions.TruncateTime(t.Date) != DbFunctions.TruncateTime(DateDaily));

                foreach (var item in TowerDataTakeOff)
                {
                    PrintDailyViewModel L = new PrintDailyViewModel();
                    L.Id = item.Id;
                    N = N + 1;
                    L.N = N;
                    L.Call = item?.AirLine.Code;
                    L.FlightNo = item?.AirLinesFlightTrip.FlightNo;
                    L.Reg = item?.AircraftRegistration.Registration;
                    L.Operation = item?.AirLine.Name;
                    L.Type = item?.AircraftRegistration.AircraftType.Type;
                    if (item.AirportIdFrom > 0)
                        //{
                        //    L.From = item.AirPort1.Code;
                        //}
                        if (item.AirportIdTo > 0)
                        {
                            L.To = item.AirPort.Code;
                        }

                    // L.ATA = item.ATA.ToString();
                    L.ATD = item.ATD.ToString();
                    L.POB = item.POB.ToString();
                    L.QBD = item.QBD.ToString();
                    L.LandingPermission = item.LandingPermission;

                    L.ATA = "";
                    L.From = "";

                    TowerDataList.Add(L);
                }
                IQueryable<TowerData> TowerDataLandingDate = db.TowerDatas.OrderBy(x => x.Id).Where(t => DbFunctions.TruncateTime(t.LandingDate) == DbFunctions.TruncateTime(DateDaily));
                TowerDataLandingDate = TowerDataLandingDate.Where(t => DbFunctions.TruncateTime(t.Date) != DbFunctions.TruncateTime(DateDaily) && t.CompanyInfoId == companyId);

                foreach (var item in TowerDataLandingDate)
                {
                    PrintDailyViewModel L = new PrintDailyViewModel();
                    L.Id = item.Id;
                    N = N + 1;
                    L.N = N;
                    L.Call = item?.AirLine.Code;
                    L.FlightNo = item?.AirLinesFlightTrip.FlightNo;
                    L.Reg = item?.AircraftRegistration.Registration;
                    L.Operation = item?.AirLine.Name;
                    L.Type = item?.AircraftRegistration.AircraftType.Type;
                    if (item.AirportIdFrom > 0)
                    {
                        L.From = item.AirPort1.Code;
                    }
                    //if (item.AirportIdTo > 0)
                    //    {
                    //        L.To = item.AirPort.Code;
                    //    }

                    L.ATA = item.ATA?.ToString(@"hh\:mm");
                    //L.ATD = item.ATD.ToString();
                    L.POB = item.POB.ToString();
                    L.QBD = item.QBD.ToString();
                    L.LandingPermission = item.LandingPermission;

                    L.ATD = "";
                    L.To = "";

                    TowerDataList.Add(L);
                }




                //////////////////////////////////////////////////////////////////////////////////////////////////
                if (Search == 1)
                {

                    if (TowerDataList != null)
                    {

                        int ItemCount = 0;
                        //List<decimal> Amountlist = new List<decimal>();
                        //List<decimal> Allowancelist = new List<decimal>();


                        ItemCount = TowerDataList.Count();
                        ViewBag.ItemCount = ItemCount;

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
                TowerDataList = null;
                ViewBag.ItemCount = 0;
            }

            int pageSize = 20;
            int PageNumber = (page ?? 1);


            if (TowerDataList == null)
            {

                return PartialView("_GetDaily", DailyList.ToPagedList(PageNumber, pageSize));
            }
            else
            {

                return PartialView("_GetDaily", TowerDataList.ToPagedList(PageNumber, pageSize));
            }
        }


        public ActionResult PrintDataDailyAll(DateTime? DateDaily)
        {
            var id = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(id);
            // RoleController role = new RoleController();


            string UserName = CommonUsed.GetUserName(id);
            int N = 0;
            IQueryable<TowerData> TowerDataCom = db.TowerDatas.OrderBy(x => x.Id);
            TowerDataCom = TowerDataCom.Where(t => DbFunctions.TruncateTime(t.Date) == DbFunctions.TruncateTime(DateDaily) && t.CompanyInfoId
            == companyId);

            ViewBag.Date = Convert.ToDateTime(DateDaily).ToString("MM/dd/yyyy");

            ///***********************************************************************************
            var TowerDataList = new List<PrintDailyViewModel>();

            foreach (var item in TowerDataCom)
            {
                PrintDailyViewModel L = new PrintDailyViewModel();
                L.Id = item.Id;
                N = N + 1;
                L.N = N;
                L.Call = item?.AirLine.Code;
                L.FlightNo = item?.AirLinesFlightTrip.FlightNo;
                L.Reg = item?.AircraftRegistration.Registration;
                L.Operation = item?.AirLine.Name;
                L.Type = item?.AircraftRegistration.AircraftType.Type;
                if (item.AirportIdFrom > 0)
                {
                    L.From = item.AirPort1.Code;
                }
                if (item.AirportIdTo > 0)
                {
                    L.To = item.AirPort.Code;
                }

                L.ATA = item.ATA?.ToString(@"hh\:mm");
                L.ATD = item.ATD?.ToString(@"hh\:mm");
                L.POB = item.POB.ToString();
                L.QBD = item.QBD.ToString();
                L.LandingPermission = item.LandingPermission;
                if (Convert.ToDateTime(item.Date).Date != Convert.ToDateTime(item.TakeOffDate).Date)
                {
                    L.ATD = "";
                    L.To = "";
                }
                if (Convert.ToDateTime(item.Date).Date != Convert.ToDateTime(item.LandingDate).Date)
                {
                    L.ATA = "";
                    L.From = "";
                }

                ///--اضافة الببانات الجديدة
                L.Pax = item.OfficerDatas.FirstOrDefault(x => x.TowerDataId == item.Id)?.Pax + item.OfficerDatas.FirstOrDefault(x => x.TowerDataId == item.Id)?.PaxInter;
                //L.tr = item.OfficerDatas.FirstOrDefault(x => x.TowerDataId == item.Id)?.TRNY;
                L.AmbulanceCarCount = item.OfficerDatas.FirstOrDefault(x => x.TowerDataId == item.Id)?.AmbulanceCarCount;
                L.FireFightingCarCount = item.OfficerDatas.FirstOrDefault(x => x.TowerDataId == item.Id)?.FireFightingCarCount;
                L.FirstClassCount = item.OfficerDatas.FirstOrDefault(x => x.TowerDataId == item.Id)?.FirstClassCount;
                L.HasPushPack = item.OfficerDatas.FirstOrDefault(x => x.TowerDataId == item.Id)?.HasPushPack;
               
                L.FreightImp = item.OfficerDatas.FirstOrDefault(x => x.TowerDataId == item.Id)?.FreightLoadingImp;
                L.FreightExp = item.OfficerDatas.FirstOrDefault(x => x.TowerDataId == item.Id)?.FreightLoadingExp;

                L.NormalMailImp = item.OfficerDatas.FirstOrDefault(x => x.TowerDataId == item.Id)?.NormalMailLoadingImp;
                L.NormalMailExp = item.OfficerDatas.FirstOrDefault(x => x.TowerDataId == item.Id)?.NormalMailLoadingExp;

                L.RapidMailImp = item.OfficerDatas.FirstOrDefault(x => x.TowerDataId == item.Id)?.RapidMailLoadingImp;
                L.RapidMailExp = item.OfficerDatas.FirstOrDefault(x => x.TowerDataId == item.Id)?.RapidMailLoadingExp;

                TowerDataList.Add(L);
            }
            IQueryable<TowerData> TowerDataTakeOff = db.TowerDatas.OrderBy(x => x.Id).Where(t => DbFunctions.TruncateTime(t.TakeOffDate) == DbFunctions.TruncateTime(DateDaily)
                                                            && t.CompanyInfoId == companyId);
            TowerDataTakeOff = TowerDataTakeOff.Where(t => DbFunctions.TruncateTime(t.Date) != DbFunctions.TruncateTime(DateDaily));

            foreach (var item in TowerDataTakeOff)
            {
                PrintDailyViewModel L = new PrintDailyViewModel();
                L.Id = item.Id;
                N = N + 1;
                L.N = N;
                L.Call = item?.AirLine.Code;
                L.FlightNo = item?.AirLinesFlightTrip.FlightNo;
                L.Reg = item?.AircraftRegistration.Registration;
                L.Operation = item?.AirLine.Name;
                L.Type = item?.AircraftRegistration.AircraftType.Type;
                if (item.AirportIdFrom > 0)
                    //{
                    //    L.From = item.AirPort1.Code;
                    //}
                    if (item.AirportIdTo > 0)
                    {
                        L.To = item.AirPort.Code;
                    }

                // L.ATA = item.ATA.ToString();
             
              
                L.ATD = item.ATD?.ToString(@"hh\:mm");
                L.POB = item.POB.ToString();
                L.QBD = item.QBD.ToString();
                L.LandingPermission = item.LandingPermission;

                L.ATA = "";
                L.From = "";


                ///--اضافة الببانات الجديدة
                L.Pax = item.OfficerDatas.FirstOrDefault(x => x.TowerDataId == item.Id)?.Pax + item.OfficerDatas.FirstOrDefault(x => x.TowerDataId == item.Id)?.PaxInter;
                //L.tr = item.OfficerDatas.FirstOrDefault(x => x.TowerDataId == item.Id)?.TRNY;
                L.AmbulanceCarCount = item.OfficerDatas.FirstOrDefault(x => x.TowerDataId == item.Id)?.AmbulanceCarCount;
                L.FireFightingCarCount = item.OfficerDatas.FirstOrDefault(x => x.TowerDataId == item.Id)?.FireFightingCarCount;
                L.FirstClassCount = item.OfficerDatas.FirstOrDefault(x => x.TowerDataId == item.Id)?.FirstClassCount;
                L.HasPushPack = item.OfficerDatas.FirstOrDefault(x => x.TowerDataId == item.Id)?.HasPushPack;

                L.FreightImp = item.OfficerDatas.FirstOrDefault(x => x.TowerDataId == item.Id)?.FreightLoadingImp;
                L.FreightExp = item.OfficerDatas.FirstOrDefault(x => x.TowerDataId == item.Id)?.FreightLoadingExp;

                L.NormalMailImp = item.OfficerDatas.FirstOrDefault(x => x.TowerDataId == item.Id)?.NormalMailLoadingImp;
                L.NormalMailExp = item.OfficerDatas.FirstOrDefault(x => x.TowerDataId == item.Id)?.NormalMailLoadingExp;

                L.RapidMailImp = item.OfficerDatas.FirstOrDefault(x => x.TowerDataId == item.Id)?.RapidMailLoadingImp;
                L.RapidMailExp = item.OfficerDatas.FirstOrDefault(x => x.TowerDataId == item.Id)?.RapidMailLoadingExp;
                TowerDataList.Add(L);
            }
            IQueryable<TowerData> TowerDataLandingDate = db.TowerDatas.OrderBy(x => x.Id).Where(t => DbFunctions.TruncateTime(t.LandingDate) == DbFunctions.TruncateTime(DateDaily)
                                                                                && t.CompanyInfoId == companyId);
            TowerDataLandingDate = TowerDataLandingDate.Where(t => DbFunctions.TruncateTime(t.Date) != DbFunctions.TruncateTime(DateDaily));

            foreach (var item in TowerDataLandingDate)
            {
                PrintDailyViewModel L = new PrintDailyViewModel();
                L.Id = item.Id;
                N = N + 1;
                L.N = N;
                L.Call = item?.AirLine.Code;
                L.FlightNo = item?.AirLinesFlightTrip.FlightNo;
                L.Reg = item?.AircraftRegistration.Registration;
                L.Operation = item?.AirLine.Name;
                L.Type = item?.AircraftRegistration.AircraftType.Type;
                if (item.AirportIdFrom > 0)
                {
                    L.From = item.AirPort1.Code;
                }
                //if (item.AirportIdTo > 0)
                //    {
                //        L.To = item.AirPort.Code;
                //    }

                L.ATA = item.ATA?.ToString(@"hh\:mm");
                //L.ATD = item.ATD.ToString();
                L.POB = item.POB.ToString();
                L.QBD = item.QBD.ToString();
                L.LandingPermission = item.LandingPermission;

                L.ATD = "";
                L.To = "";

                ///--اضافة الببانات الجديدة
                L.Pax = item.OfficerDatas.FirstOrDefault(x => x.TowerDataId == item.Id)?.Pax + item.OfficerDatas.FirstOrDefault(x => x.TowerDataId == item.Id)?.PaxInter;
                //L.tr = item.OfficerDatas.FirstOrDefault(x => x.TowerDataId == item.Id)?.TRNY;
                L.AmbulanceCarCount = item.OfficerDatas.FirstOrDefault(x => x.TowerDataId == item.Id)?.AmbulanceCarCount;
                L.FireFightingCarCount = item.OfficerDatas.FirstOrDefault(x => x.TowerDataId == item.Id)?.FireFightingCarCount;
                L.FirstClassCount = item.OfficerDatas.FirstOrDefault(x => x.TowerDataId == item.Id)?.FirstClassCount;
                L.HasPushPack = item.OfficerDatas.FirstOrDefault(x => x.TowerDataId == item.Id)?.HasPushPack;

                L.FreightImp = item.OfficerDatas.FirstOrDefault(x => x.TowerDataId == item.Id)?.FreightLoadingImp;
                L.FreightExp = item.OfficerDatas.FirstOrDefault(x => x.TowerDataId == item.Id)?.FreightLoadingExp;

                L.NormalMailImp = item.OfficerDatas.FirstOrDefault(x => x.TowerDataId == item.Id)?.NormalMailLoadingImp;
                L.NormalMailExp = item.OfficerDatas.FirstOrDefault(x => x.TowerDataId == item.Id)?.NormalMailLoadingExp;

                L.RapidMailImp = item.OfficerDatas.FirstOrDefault(x => x.TowerDataId == item.Id)?.RapidMailLoadingImp;
                L.RapidMailExp = item.OfficerDatas.FirstOrDefault(x => x.TowerDataId == item.Id)?.RapidMailLoadingExp;

                TowerDataList.Add(L);
            }












            return View(TowerDataList);

        }




        #endregion






    }
}