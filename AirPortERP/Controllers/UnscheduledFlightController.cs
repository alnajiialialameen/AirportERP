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
using System.ComponentModel.Design;
using System.Drawing;

namespace AirPortERP.Controllers
{
    public class UnscheduledFlightController : Controller
    {
        // Calculation Calculation = new Calculation();

        DateTime now = System.DateTime.Now;
        private CommonUsed commonUsed = new CommonUsed();
        private Entities db = new Entities();
        RoleController Role = new RoleController();
        private Calculation cal = new Calculation();
        int WorkMouth;
        int WorkYear;
        public UnscheduledFlightController()
        {

        }



        public ActionResult Unscheduled()
        {
            return View();
        }




  

        public ActionResult GetUnscheduled()
        {
            var id = User.Identity.GetUserId();
            var companyId = commonUsed.getCompanyId(id);
            bool IsSingle = commonUsed.getIsSingleAccounting(companyId);
            IQueryable<Revenue> Revenue = db.Revenues.Where(x => DbFunctions.TruncateTime(x.OfficerData.TowerData.Date.Value).ToString() ==
            DbFunctions.TruncateTime(now.Date).ToString() && x.OfficerData.TowerData.CompanyInfoId == companyId);

            var TowerDatasList = new List<TowerDataViewModels>();

            foreach (var item in Revenue)
            {
                TowerDataViewModels tdm = new TowerDataViewModels();

                tdm.Id = item.Id;
                tdm.AirLineCode = item?.OfficerData.TowerData.AirLine.Code;
                tdm.AirLineName = item?.OfficerData.TowerData.AirLine.Name;
                //tdm.TripType = CommonUsed.GetFlightTypeName(item.OfficerData.TowerData.FlightTypeL, item.OfficerData.TowerData.FlightTypeD);
                tdm.TripType = commonUsed.GetFlightTypeNameLanding(item.OfficerData.TowerData.FlightTypeL);
                tdm.FlightNo = item?.OfficerData.TowerData.AirLinesFlightTrip.FlightNo;
                tdm.AircraftRegName = item?.OfficerData.TowerData.AircraftRegistration.Registration;
                tdm.Type = item?.OfficerData.TowerData.AircraftRegistration.AircraftType.Type;
                tdm.MaxTaKeOffWeight = item?.OfficerData.TowerData.AircraftRegistration.MaxTakoffWieght.ToString();

                //from --------------------------------------------------------------------------------------
                tdm.AirportIdFrom = commonUsed.CheckAirportCode(Convert.ToInt32(item?.OfficerData.TowerData.AirportIdFrom));
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
                tdm.AirportIdTo = commonUsed.CheckAirportCode(Convert.ToInt32(item?.OfficerData.TowerData.AirportIdTo));
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


                tdm.CreatedBy = commonUsed.GetUserName(item.OfficerData.TowerData.CreatedBy);
                tdm.CreationDate = item.OfficerData.TowerData.CreationDate.ToString();
                tdm.UpdatedBy = commonUsed.GetUserName(item.OfficerData.TowerData.UpdatedBy);
                tdm.UpdatingDate = item.OfficerData.TowerData.ToString();

                tdm.LandingDate = CommonUsed.ToDisplayDate(item.OfficerData.TowerData.LandingDate);
                tdm.TakeOffDate = CommonUsed.ToDisplayDate(item.OfficerData.TowerData.TakeOffDate);

                ///////
                tdm.Size = item?.OfficerData.TowerData.AircraftRegistration.AircraftType.AircraftSize.Size.ToString();
                tdm.LandingFees = item.LandingFees?.ToString("N0");

                tdm.ParkingFees = item.ParkingFees?.ToString("N0");

                tdm.NightOperationFees = item.NightOperationFees?.ToString("N0");
                tdm.NavigationFees = item.NavigationFees?.ToString("N0");
                tdm.NoiseFees = item.NoiseFees?.ToString("N0");
                tdm.SecurityFees = item.SecurityFees?.ToString("N0");
                if (IsSingle)
                {
                    tdm.IsStop = commonUsed.IsEntryDateinRange2(item.Id, Role.GetRole(id));  // check date in range in claim table
                }
                else
                {
                    tdm.IsStop = commonUsed.IsEntryDateinRange(Convert.ToDateTime(item.OfficerData.TowerData.Date), Role.GetRole(id),
                  Convert.ToInt32(item.OfficerData.TowerData.AirLineId), Convert.ToInt32(companyId));  // check date in range in claim table
                }
                tdm.AirportName = commonUsed.getCompanyName(item.OfficerData.TowerData.CompanyInfoId);
                //-----------------------
                tdm.InterNYFees = item.InterNYFees?.ToString("N0");
                tdm.DomeNYFees = item.DomeNYFees?.ToString("N0");
              
                tdm.InterCUPPSFees = item.InterCUPPSFees?.ToString("N0");
                tdm.DomeCUPPSFees = item.DomeCUPPSFees?.ToString("N0");
                
                tdm.ExpMailFees = item.ExpMailFees?.ToString("N0");
                tdm.ImpMailFees = item.ImpMailFees?.ToString("N0");

                tdm.FreightFees = item.FreightFees?.ToString("N0");

                tdm.ServiceCar = ((item?.AmbolanceFees ?? 0) + (item?.FireFightingFees ?? 0)).ToString("N0");
                tdm.FreightFees = item.FreightFees?.ToString("N0");
                tdm.PushBackFees = item.PushBackFees?.ToString("N0");
                tdm.FuelFees = Convert.ToDecimal(item.FuelFees);


                TowerDatasList.Add(tdm);
            }

            return Json(new { data = TowerDatasList.ToList() }, JsonRequestBehavior.AllowGet);
        }



        //public ActionResult CreateUnscheduled(AirArriveModel model)
        //{
        //    var id = User.Identity.GetUserId();
        //    var companyId = CommonUsed.getCompanyId(id);

        //    if (ModelState.IsValid)
        //    {
        //        TowerData TowerData = new TowerData();

        //        TowerData.Id = model.IdAr;
        //        TowerData.AirLineId = Convert.ToInt32(model.AirLineIdAr);
        //        TowerData.FlightNo = Convert.ToInt32(model.FlightNoAr);
        //        TowerData.AircraftRegId = Convert.ToInt32(model.AircraftRegIdAr);
        //        TowerData.AirportIdFrom = model.AirportIdFromAr;
        //        TowerData.AirportIdTo = model.AirportIdToAr;
        //        //TowerData.LandingDate = Convert.ToDateTime(model.LandingDateAr);
        //        TowerData.LandingDate = DateTime.ParseExact(model.LandingDateAr, "yyyy-MM-dd", CultureInfo.InvariantCulture);

        //        //TowerData.Date = Convert.ToDateTime(model.DateAr);
        //        TowerData.Date = DateTime.ParseExact(model.DateAr, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        //        ///TowerData.TakeOffDate = CommonUsed.GetDateD(model.TakeOffDateAr);

        //        //TowerData.CreationDate = DateTime.ParseExact(model.CreationDateAr, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        //        TowerData.UpdatingDate = model.UpdatingDateAr;

        //        TowerData.CreatedBy = model.CreatedByAr;
        //        TowerData.UpdatedBy = model.UpdatedByAr;

        //        TowerData.ATA = model.ATAAr;
        //        TowerData.ATD = model.ATDAr;

        //        TowerData.Note = model.NoteAr;
        //        TowerData.TripTypeId = model.TripTypeIdAr;

        //        TowerData.POB = model.POBAr;
        //        TowerData.QBD = model.QBDAr;
        //        TowerData.LandingPermission = model.LandingPermissionAr;

        //        int TowerId = Convert.ToInt32(db.SpLastTowerid().FirstOrDefault());





        //        if (db.TowerDatas.Any(x => x.AirLineId == model.AirLineIdAr && x.ATA == model.ATAAr && x.ATD == model.ATDAr && x.Date == TowerData.Date && x.FlightNo == model.FlightNoAr))

        //        {
        //            //  return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
        //            ///   return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
        //            return Json(new { Message = "هذا الرحلة  موجود  مسبقا", Title = "عملية الاضافة", Status = "error" });


        //        }

        //        TowerData.CreatedBy = id;
        //        TowerData.CreationDate = System.DateTime.Now;

        //        /////////////////////////////******* Start Check FlightType******////////////////////////////////////////
        //        // From Not Found When Aircraft Departure
        //        if (model.AirportIdToAr > 0)
        //        {
        //            AirPort ATo = db.AirPorts.Single(x => x.Id == model.AirportIdToAr);
        //            if (ATo.CountryId == 1)
        //            {
        //                // Domistic
        //                TowerData.FlightTypeD = 2;
        //            }
        //            else
        //            {    // InterNational
        //                TowerData.FlightTypeD = 1;
        //            }
        //        }
        //        // From Not Found When Aircraft Take Off

        //        if (model.AirportIdFromAr > 0)
        //        {
        //            AirPort AFrom = db.AirPorts.Single(x => x.Id == model.AirportIdFromAr);
        //            if (model.AirportIdFromAr > 0)
        //            {
        //                if (AFrom.CountryId == 1)
        //                {
        //                    // Domistic
        //                    TowerData.FlightTypeL = 2;
        //                }
        //                else
        //                {    // InterNational
        //                    TowerData.FlightTypeL = 1;
        //                }
        //            }
        //        }
        //        //****************************Start Check Status*****************************************

        //        TowerData.Status = 2;


        //        // ******************************End Check Status***********************************

        //        TowerData.CompanyInfoId = companyId;
        //        TowerData.Id = TowerId;
        //        db.TowerDatas.Add(TowerData);


        //        db.SaveChanges();


        //        return Json(new { Message = "تمت عملية الإضافة  بنجاح", Title = "نجاح", Status = "success" });

        //    }
        //    return Json(new { success = false, message = "error" }, JsonRequestBehavior.AllowGet);
        //}


        public ActionResult CreateUnscheduled1(UnScheduledModel model)
        {    ///get companyInfoId
            var id = User.Identity.GetUserId();
            int? companyInfoId = db.WorkOns.Where(x => x.UserId == id).FirstOrDefault().CompanyInfoId;
            if (!companyInfoId.HasValue || companyInfoId <= 0) { return Json(new { success = true, message = "CompanyInfoIdNotFound" }, JsonRequestBehavior.AllowGet); }

            TowerData towerData = new TowerData();
            // تعيين القيم الأساسية
            towerData.Id = model.IdAr;
            towerData.AirLineId = model.AirLineIdAr;
            towerData.FlightNo = model.FlightNoAr;
            towerData.AircraftRegId = model.AircraftRegIdAr;
            towerData.AirportIdFrom = model.AirportIdFromAr;
            towerData.AirportIdTo = model.AirportIdToAr;
            towerData.FlightTypeD = model.FlightTypeD;
            towerData.FlightTypeL = model.FlightTypeL;
            towerData.Status = model.StatusAr;
            towerData.TripTypeId = model.TripTypeIdAr;
            towerData.CompanyInfoId = model.CompanyInfoId;
            towerData.Note = model.NoteAr;

            // تحويل التاريخ من string إلى DateTime
            //towerData.LandingDate = CommonUsed.ConvertToDbDate(model.LandingDateAr);
            towerData.LandingDate = DateTime.ParseExact(model.DateAr, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            towerData.Date = DateTime.ParseExact(model.DateAr, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            towerData.TakeOffDate = DateTime.ParseExact(model.TakeOffDateAr, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            // TimeSpan
            towerData.ATA = model.ATAAr;
            towerData.ATD = model.ATDAr;

            // خصائص الإنشاء والتعديل
            towerData.CreatedBy = model.CreatedByAr;
            //towerData.CreationDate = model.CreationDateAr;
            towerData.UpdatedBy = model.UpdatedByAr;
            //towerData.UpdatingDate = model.UpdatingDate;


            if (ModelState.IsValid)
            {   
                int TowerId = Convert.ToInt32(db.SpLastTowerid().FirstOrDefault());
                if (towerData.Id > 0)
                {
                    towerData.UpdatedBy = id;
                    towerData.UpdatingDate = System.DateTime.Now;
                    if (db.TowerDatas.Any(x => x.AirLineId == towerData.AirLineId && x.ATA == towerData.ATA && x.ATD == towerData.ATD && x.Date == towerData.Date && x.Id != towerData.Id && x.FlightNo == towerData.FlightNo))
                    {
                        return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
                    }

                    /////////////////////////////******* Start Check FlightType****//////////////////////////////////////
                    //AirPort Afrom = db.AirPorts.Single(x => x.Id == model.AirportIdFrom);
                    AirPort ATo = db.AirPorts.Single(x => x.Id == towerData.AirportIdTo);
                    if (ATo.CountryId == 1)
                    {
                        // Domistic
                        towerData.FlightTypeD = 2;
                    }
                    else
                    {    // InterNational
                        towerData.FlightTypeD = 1;
                    }

                    AirPort AFrom = db.AirPorts.Single(x => x.Id == towerData.AirportIdFrom);
                    if (AFrom.CountryId == 1)
                    {
                        // Domistic
                        towerData.FlightTypeL = 2;
                    }
                    else
                    {    // InterNational
                        towerData.FlightTypeL = 1;
                    }


                    towerData.CompanyInfoId = companyInfoId;
                    db.Entry(towerData).State = System.Data.Entity.EntityState.Modified;
                    //   cal.LandingFees(model.Id);
                    db.SaveChanges();




                    bool res = calculateFeesUpdate(towerData, model);
                    //bool resNyAndCupps = ChangeNyAndCupps(towerData);


                    if (!res)
                    {
                        return Json(new { success = true, message = "CalculationFeesError" }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { success = true, message = "Update" }, JsonRequestBehavior.AllowGet);
                }
                else
                {


                    if (db.TowerDatas.Any(x => x.AirLineId == towerData.AirLineId && x.ATA == towerData.ATA && x.ATD == towerData.ATD && x.Date == towerData.Date && x.FlightNo == towerData.FlightNo))

                    {
                        //  return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
                        return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);

                    }

                    towerData.CreatedBy = id;
                    towerData.CreationDate = System.DateTime.Now;

                    /////////////////////////////******* Start Check FlightType******////////////////////////////////////////
                    //AirPort Afrom = db.AirPorts.Single(x => x.Id == model.AirportIdFrom);
                    AirPort ATo = db.AirPorts.Single(x => x.Id == towerData.AirportIdTo);
                    if (ATo.CountryId == 1)
                    {
                        // Domistic
                        towerData.FlightTypeD = 2;
                    }
                    else
                    {    // InterNational
                        towerData.FlightTypeD = 1;
                    }

                    AirPort AFrom = db.AirPorts.Single(x => x.Id == towerData.AirportIdFrom);
                    if (AFrom.CountryId == 1)
                    {
                        // Domistic
                        towerData.FlightTypeL = 2;
                    }
                    else
                    {    // InterNational
                        towerData.FlightTypeL = 1;
                    }

                    //****************************Start Check Status*****************************************

                    towerData.Status = 3;


                    // ******************************End Check Status***********************************

                    towerData.CompanyInfoId = companyInfoId;
                    towerData.Id = TowerId;
                    db.TowerDatas.Add(towerData);
                    db.SaveChanges();



                    bool res = calculateFees(towerData, TowerId, model, Convert.ToInt32(companyInfoId));
                    //



                    if (!res)
                    {
                        return Json(new { success = true, message = "CalculationFeesError" }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { success = true, message = "Add" }, JsonRequestBehavior.AllowGet);



                }

            }
            return Json(new { success = false, message = "error" }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult CreateUnscheduled(UnScheduledModel model)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var id = User.Identity.GetUserId();

                    int? companyInfoId = db.WorkOns
                        .Where(x => x.UserId == id)
                        .Select(x => x.CompanyInfoId)
                        .FirstOrDefault();

                    if (!companyInfoId.HasValue || companyInfoId <= 0)
                    {
                        return Json(new { Message = "CompanyInfoId Not Found", Title = "عملية الاضافة", Status = "error" });

                    }

                    if (!ModelState.IsValid)
                    {
                        return Json(new { Message = "Model State Error", Title = "عملية الاضافة", Status = "error" });


                    }

                    // ===========================
                    // Date Parse
                    // ===========================

                    DateTime date =
                        DateTime.ParseExact(
                            model.DateAr,
                            "yyyy-MM-dd",
                            CultureInfo.InvariantCulture);

                    DateTime takeOffDate =
                        DateTime.ParseExact(
                            model.TakeOffDateAr,
                            "yyyy-MM-dd",
                            CultureInfo.InvariantCulture);

                    // ===========================
                    // Create TowerData
                    // ===========================

                    TowerData towerData = new TowerData();

                    towerData.Id = model.IdAr;
                    towerData.AirLineId = model.AirLineIdAr;
                    towerData.FlightNo = model.FlightNoAr;
                    towerData.AircraftRegId = model.AircraftRegIdAr;

                    towerData.AirportIdFrom = model.AirportIdFromAr;
                    towerData.AirportIdTo = model.AirportIdToAr;

                    towerData.TripTypeId = model.TripTypeIdAr;

                    towerData.Note = model.NoteAr;

                    towerData.Date = date;
                    towerData.LandingDate = date;
                    towerData.TakeOffDate = takeOffDate;

                    towerData.ATA = model.ATAAr;
                    towerData.ATD = model.ATDAr;

                    towerData.CompanyInfoId = companyInfoId;

                    // ===========================
                    // Duplicate Check
                    // ===========================

                    bool exists = db.TowerDatas.Any(x =>
                        x.AirLineId == towerData.AirLineId &&
                        x.ATA == towerData.ATA &&
                        x.ATD == towerData.ATD &&
                        x.Date == towerData.Date &&
                        x.FlightNo == towerData.FlightNo &&
                        x.Id != towerData.Id);

                    if (exists)
                    {
                        return Json(new { Message = "هذا الرحلة  موجود  مسبقا", Title = "عملية الاضافة", Status = "error" });

                    }

                    // ===========================
                    // Flight Type
                    // ===========================

                    AirPort airportTo = db.AirPorts
                        .Single(x => x.Id == towerData.AirportIdTo);

                    towerData.FlightTypeD =
                        airportTo.CountryId == 1 ? 2 : 1;

                    AirPort airportFrom = db.AirPorts
                        .Single(x => x.Id == towerData.AirportIdFrom);

                    towerData.FlightTypeL =
                        airportFrom.CountryId == 1 ? 2 : 1;

                    // ===========================
                    // Add OR Update
                    // ===========================

                    int towerId = towerData.Id;

                    if (towerData.Id > 0)
                    {
                        towerData.UpdatedBy = id;
                        towerData.UpdatingDate = DateTime.Now;

                        db.Entry(towerData).State =
                        System.Data.Entity.EntityState.Modified;
                    }
                    else
                    {
                        towerId = Convert.ToInt32(
                            db.SpLastTowerid().FirstOrDefault());

                        towerData.Id = towerId;

                        towerData.CreatedBy = id;
                        towerData.CreationDate = DateTime.Now;

                        towerData.Status = 3;

                        db.TowerDatas.Add(towerData);
                    }

                    // ===========================
                    // Save TowerData
                    // ===========================

                    db.SaveChanges();

                    // ===========================
                    // Fees
                    // ===========================
                    bool feesResult;
                    if (model.IdAr > 0)
                    {
                        feesResult = calculateFeesUpdate(towerData, model);

                    }
                    else
                    {
                        feesResult = calculateFees(towerData, towerId, model, Convert.ToInt32(companyInfoId));

                    }
                    if (!feesResult)
                    {
                        transaction.Rollback();

                        return Json(new { Message = "هذا خطا في عملية الحساب", Title = "عملية الاضافة", Status = "error" });

                    }

                    // ===========================
                    // Final Save
                    // ===========================

                    db.SaveChanges();

                    transaction.Commit();

                    if (towerData.Id > 0)
                    {
                        return Json(new { Message = "تمت عملية التعديل  بنجاح", Title = "نجاح", Status = "success" });
                    }
                    else
                    {
                        return Json(new { Message = "تمت عملية  الاضافة بنجاح", Title = "نجاح", Status = "success" });
                    }

                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    return Json(new { Message = ex.Message, Title = "عملية الاضافة", Status = "error" });

                   
                }
            }
        }


        public ActionResult GetDataForUpdate(int id)
        {
            Revenue revenue = db.Revenues.Find(id);

            AirArriveModel model = new AirArriveModel();
            model.IdAr = revenue.OfficerData.TowerData.Id;
            model.AirLineIdAr = Convert.ToInt32(revenue.OfficerData.TowerData.AirLineId);
            model.FlightNoAr = Convert.ToInt32(revenue.OfficerData.TowerData.FlightNo);

            model.AircraftRegIdAr = Convert.ToInt32(revenue.OfficerData.TowerData.AircraftRegId);
            model.AirportIdFromAr = revenue.OfficerData.TowerData.AirportIdFrom;
            model.AirportIdToAr = revenue.OfficerData.TowerData.AirportIdTo;
            

            model.ATAAr = revenue.OfficerData.TowerData.ATA;
            model.ATDAr = revenue.OfficerData.TowerData.ATD;
            model.ATAArShow = ConvertToHHmm(revenue.OfficerData.TowerData.ATA.ToString());
            model.ATDArShow = ConvertToHHmm(revenue.OfficerData.TowerData.ATD.ToString());



            model.TripTypeIdAr = Convert.ToInt32(revenue.OfficerData.TowerData.TripTypeId);
            model.DateAr = Convert.ToDateTime(revenue.OfficerData.TowerData.Date).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            model.LandingDateAr = Convert.ToDateTime(revenue.OfficerData.TowerData.LandingDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            model.TakeOffDateAr = Convert.ToDateTime(revenue.OfficerData.TowerData.TakeOffDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);


            model.POBAr = revenue.OfficerData.TowerData.POB;
            model.QBDAr = revenue.OfficerData.TowerData.QBD;
            model.NoteAr = revenue.OfficerData.TowerData.Note;

            model.LandingPermissionAr = revenue.OfficerData.TowerData.LandingPermission;
            model.CreatedByAr = revenue.OfficerData.TowerData.CreatedBy;
            model.CreationDateAr = revenue.OfficerData.TowerData.CreationDate.ToString();


            model.POBAr = revenue.OfficerData.TowerData.POB;
            model.QBDAr = revenue.OfficerData.TowerData.QBD;
            model.NoteAr = revenue.OfficerData.TowerData.Note;


            model.Pax = revenue.OfficerData.Pax;
            model.TRNYCUPPS = revenue.OfficerData.TRNYCUPPS;
          
            model.NormalMailLoadingExp = revenue.OfficerData.NormalMailLoadingExp;
            model.NormalMailLoadingImp = revenue.OfficerData.NormalMailLoadingImp;

            model.RapidMailLoadingExp = revenue.OfficerData.RapidMailLoadingExp;
            model.RapidMailLoadingImp = revenue.OfficerData.RapidMailLoadingImp;

            model.FreightLoadingExp = revenue.OfficerData.FreightLoadingExp;
            model.FreightLoadingImp = revenue.OfficerData.FreightLoadingImp;

            model.AmbulanceCarCount = revenue.OfficerData.AmbulanceCarCount;
            model.FireFightingCarCount = revenue.OfficerData.FireFightingCarCount;

            model.HasPushPack = revenue.OfficerData.HasPushPack;
            model.FuelLiter = revenue.OfficerData.FuelLiter;

            return Json(model, JsonRequestBehavior.AllowGet);

        }




        private bool calculateFees1(TowerData model, int towerId, UnScheduledModel unScheduled, int companyId)
        {
            try
            {
                if (!model.ATA.HasValue || !model.ATD.HasValue)
                    return false;

                var aircraft = db.AircraftRegistrations
                    .Include(x => x.AircraftType)
                    .Include(x => x.AircraftType.AircraftSize)
                    .AsNoTracking()
                    .FirstOrDefault(x => x.Id == model.AircraftRegId);

                if (aircraft == null)
                    return false;

                var airline = db.AirLines
                    .AsNoTracking()
                    .FirstOrDefault(x => x.Id == model.AirLineId);

                if (airline == null)
                    return false;

                int officerId =
                    Convert.ToInt32(
                        db.SpLastOfficerid().FirstOrDefault());

                OfficerData officer = new OfficerData
                {
                    Id = officerId,
                    TowerDataId = towerId,
                    IsCurrentAccount = true,

                    AmbulanceCarCount =
                        unScheduled.AmbulanceCarCount,

                    FireFightingCarCount =
                        unScheduled.FireFightingCarCount,

                    NormalMailLoadingExp =
                        unScheduled.NormalMailLoadingExp,

                    RapidMailLoadingExp =
                        unScheduled.RapidMailLoadingExp,

                    NormalMailLoadingImp =
                        unScheduled.NormalMailLoadingImp,

                    RapidMailLoadingImp =
                        unScheduled.RapidMailLoadingImp,

                    FreightLoadingExp =
                        unScheduled.FreightLoadingExp,

                    FreightLoadingImp =
                        unScheduled.FreightLoadingImp,

                    HasPushPack =
                        unScheduled.hasPushPack
                };

                decimal wt =
                    Convert.ToDecimal(
                        aircraft.MaxTakoffWieght);

                decimal landingFees =
                    cal.LandingFees(
                        wt,
                        Convert.ToInt32(model.FlightTypeL));

                Revenue revenue = new Revenue();

                revenue.Id =
                    Convert.ToInt32(
                        db.SpLastRevenceid().FirstOrDefault());

                revenue.OfficerDataId = officerId;

                // Landing
                revenue.LandingFees = landingFees;

                revenue.NightOperationFees =
                    cal.NightOperationFees(
                        model.ATA.Value.Hours,
                        model.ATD.Value.Hours,
                        landingFees);

                revenue.OpenAireportOvertimeFees =
                    cal.OpenAireportOvertimeFees(
                        model.ATA.Value.Hours,
                        landingFees);

                revenue.NavigationFees =
                    cal.NavigationFees(landingFees);

                revenue.ParkingFees =
                    cal.ParkingFeestowerId(
                        towerId,
                        Convert.ToDateTime(model.LandingDate),
                        Convert.ToDateTime(model.TakeOffDate),
                        model.ATA.Value,
                        model.ATD.Value,
                        Convert.ToBoolean(airline.IsLocalCompany));

                revenue.NoiseFees =
                    cal.NoiseFees(wt);

                revenue.SecurityFees =
                    cal.SecurityFees(
                        wt,
                        Convert.ToInt32(model.FlightTypeL),
                        Convert.ToBoolean(airline.IsLocalCompany));

                // NY + CUPPS
                if (model.FlightTypeD == 2)
                {
                    revenue.DomeNYFees =
                        cal.DomeNYFees(
                            Convert.ToInt32(unScheduled.Pax),
                            airline.IsLocalCompany);

                    revenue.DomeCUPPSFees =
                        cal.DomeCUPPSFees(
                            Convert.ToInt32(unScheduled.Pax),
                            Convert.ToInt32(unScheduled.TRNYCUPPS),
                           Convert.ToBoolean(unScheduled.IsNotUsedCUPPS));

                    revenue.InterCUPPSFees = 0;
                    revenue.InterNYFees = 0;
                }
                else
                {
                    revenue.InterCUPPSFees =
                        cal.InterCUPPSFees(
                           Convert.ToInt32(unScheduled.Pax),
                           Convert.ToInt32(unScheduled.TRNYCUPPS),
                           Convert.ToBoolean( unScheduled.IsNotUsedCUPPS));

                    revenue.InterNYFees =
                        cal.InterNYFees(
                            Convert.ToInt32(unScheduled.Pax),
                            airline.IsLocalCompany,
                            companyId);

                    revenue.DomeCUPPSFees = 0;
                    revenue.DomeNYFees = 0;
                }

                // Service Car
                revenue.AmbolanceFees =
                    cal.AmbulanceFees(
                        Convert.ToInt32(unScheduled.AmbulanceCarCount));

                revenue.FireFightingFees =
                    cal.FireFightingFees(
                        Convert.ToInt32(unScheduled.FireFightingCarCount));

                // Mail
                revenue.ExpMailFees =
                    cal.ExpMailFees(
                        Convert.ToDecimal(unScheduled.NormalMailLoadingExp),
                        Convert.ToDecimal(unScheduled.RapidMailLoadingExp));

                revenue.ImpMailFees =
                    cal.ImpMailFees(
                        Convert.ToDecimal(unScheduled.NormalMailLoadingImp),
                        Convert.ToDecimal(unScheduled.RapidMailLoadingImp));

                // Freight
                revenue.FreightFees =
                    cal.FreightFees(
                        Convert.ToInt32(unScheduled.FreightLoadingExp),
                        Convert.ToInt32(unScheduled.FreightLoadingImp));

                // PushBack
                if (Convert.ToBoolean(unScheduled.hasPushPack))
                {
                    string size =
                        aircraft.AircraftType
                        .AircraftSize
                        .Size;

                    revenue.PushBackFees =
                        cal.PushBackFees(size);
                }
                else
                {
                    revenue.PushBackFees = 0;
                }

                db.OfficerDatas.Add(officer);
                db.Revenues.Add(revenue);

                return true;
            }
            catch
            {
                return false;
            }
        }


        private bool calculateFees(TowerData model, int towerId, UnScheduledModel unScheduled, int companyId)
        {
            try
            {
                if (!model.ATA.HasValue || !model.ATD.HasValue)
                    return false;

                var aircraft = db.AircraftRegistrations
                    .AsNoTracking()
                    .FirstOrDefault(x => x.Id == model.AircraftRegId);

                if (aircraft == null)
                    return false;

                var airline = db.AirLines
                    .AsNoTracking()
                    .FirstOrDefault(x => x.Id == model.AirLineId);

                if (airline == null)
                    return false;

                int officerId = Convert.ToInt32(db.SpLastOfficerid().FirstOrDefault());

                var officer = new OfficerData
                {
                    Id = officerId,
                    TowerDataId = towerId,
                    IsCurrentAccount = true
                };

                decimal wt = Convert.ToDecimal(aircraft.MaxTakoffWieght);

                decimal landingFees =
                    cal.LandingFees(wt, Convert.ToInt32(model.FlightTypeL));

                var revenue = new Revenue();

                ///---------------------------Landing--------------------------------------------------------

                revenue.Id = Convert.ToInt32(db.SpLastRevenceid().FirstOrDefault());
                revenue.OfficerDataId = officerId;
                revenue.LandingFees = landingFees;
                revenue.NightOperationFees = cal.NightOperationFees(model.ATA.Value.Hours, model.ATD.Value.Hours, landingFees);
                revenue.OpenAireportOvertimeFees = cal.OpenAireportOvertimeFees(model.ATA.Value.Hours, landingFees);
                revenue.NavigationFees = cal.NavigationFees(landingFees);
                revenue.ParkingFees = cal.ParkingFeesmodel(model, Convert.ToDateTime(model.LandingDate), Convert.ToDateTime(model.TakeOffDate), model.ATA.Value, model.ATD.Value, Convert.ToBoolean(airline.IsLocalCompany));
                revenue.NoiseFees = cal.NoiseFees(wt);
                revenue.SecurityFees = cal.SecurityFees(wt, Convert.ToInt32(model.FlightTypeL), Convert.ToBoolean(airline.IsLocalCompany));

                ///---------------------------Cupps and Ny--------------------------------------------------------
                if (model.FlightTypeD == 2)
                {
                    revenue.DomeNYFees = cal.DomeNYFees(Convert.ToInt32(unScheduled.Pax), airline.IsLocalCompany);
                    revenue.DomeCUPPSFees = cal.DomeCUPPSFees(Convert.ToInt32(unScheduled.Pax), Convert.ToInt32(unScheduled.TRNYCUPPS), Convert.ToBoolean(unScheduled.IsNotUsedCUPPS));

                    revenue.InterCUPPSFees = 0; revenue.InterNYFees = 0;
                }
                else
                {
                    revenue.InterCUPPSFees = cal.InterCUPPSFees(Convert.ToInt32(unScheduled.Pax), Convert.ToInt32(unScheduled.TRNYCUPPS), Convert.ToBoolean(unScheduled.IsNotUsedCUPPS));
                    revenue.InterNYFees = cal.InterNYFees(Convert.ToInt32(unScheduled.Pax), airline.IsLocalCompany, companyId);

                    revenue.DomeCUPPSFees = 0; revenue.DomeNYFees = 0;
                }
                officer.Pax = unScheduled.Pax;
                officer.TRNYCUPPS = unScheduled.TRNYCUPPS;
                ///---------------------------friend and Mail--------------------------------------------------------


                revenue.AmbolanceFees = cal.AmbulanceFees(Convert.ToInt32(unScheduled.AmbulanceCarCount));
                revenue.FireFightingFees = cal.FireFightingFees(Convert.ToInt32(unScheduled.FireFightingCarCount));
                officer.AmbulanceCarCount = unScheduled.AmbulanceCarCount;
                officer.FireFightingCarCount = unScheduled.FireFightingCarCount;

                revenue.ExpMailFees = cal.ExpMailFees(Convert.ToDecimal(unScheduled.NormalMailLoadingExp), Convert.ToDecimal(unScheduled.RapidMailLoadingExp));
                revenue.ImpMailFees = cal.ImpMailFees(Convert.ToDecimal(unScheduled.NormalMailLoadingImp), Convert.ToDecimal(unScheduled.RapidMailLoadingImp));
                revenue.FreightFees = cal.FreightFees(Convert.ToDecimal(unScheduled.FreightLoadingExp), Convert.ToDecimal(unScheduled.FreightLoadingImp));

                officer.NormalMailLoadingExp = unScheduled.NormalMailLoadingExp;
                officer.RapidMailLoadingExp = unScheduled.RapidMailLoadingExp;

                officer.NormalMailLoadingImp = unScheduled.NormalMailLoadingImp;
                officer.RapidMailLoadingImp = unScheduled.RapidMailLoadingImp;

                officer.FreightLoadingExp = unScheduled.FreightLoadingExp;
                officer.FreightLoadingImp = unScheduled.FreightLoadingImp;
                if (Convert.ToBoolean( unScheduled.hasPushPack))
                {

                    officer.HasPushPack = true;
                    string size = aircraft.AircraftType.AircraftSize.Size;
                    revenue.PushBackFees = cal.PushBackFees(size);

                }
                else
                {
                    officer.HasPushPack = false;
                    revenue.PushBackFees = 0;
                }

                ///fuel
                officer.FuelLiter = unScheduled.FuelLiter;
                revenue.FuelFees = Calculation.FuelPerLiterFees(Convert.ToDecimal(unScheduled.FuelLiter));

                officer.IsCurrentAccount = true;
                db.OfficerDatas.Add(officer);
                db.Revenues.Add(revenue);

                db.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }


        static string ConvertToHHmm(string time)
        {
            if (time != "")
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




        private bool calculateFeesUpdate1(TowerData model)
        {
            bool res = false;

            OfficerData OfficerData = db.OfficerDatas.FirstOrDefault(x => x.TowerDataId == model.Id);

            decimal wt = Convert.ToDecimal(db.AircraftRegistrations.Single(x => x.Id == model.AircraftRegId).MaxTakoffWieght);
            decimal val = cal.LandingFees(wt, Convert.ToInt32(model.FlightTypeL));

            if (db.Revenues.Any(x => x.OfficerDataId == OfficerData.Id))
            {
                //Update
                Revenue rev = db.Revenues.FirstOrDefault(x => x.OfficerDataId == OfficerData.Id);
                rev.LandingFees = val;
                rev.NightOperationFees = cal.NightOperationFees(model.ATA.Value.Hours, model.ATD.Value.Hours, val);
                rev.OpenAireportOvertimeFees = cal.OpenAireportOvertimeFees(model.ATA.Value.Hours, val);
                rev.NavigationFees = cal.NavigationFees(val);
                var AirLine = db.AirLines.FirstOrDefault(x => x.Id == model.AirLineId);
                rev.ParkingFees = cal.ParkingFeesmodel(model, Convert.ToDateTime(model.LandingDate), Convert.ToDateTime(model.TakeOffDate), model.ATA.Value, model.ATD.Value, Convert.ToBoolean(AirLine.IsLocalCompany));
                rev.NoiseFees = cal.NoiseFees(wt);
                rev.SecurityFees = cal.SecurityFees(wt, Convert.ToInt32(model.FlightTypeL), Convert.ToBoolean(AirLine.IsLocalCompany));
                if (ModelState.IsValid)
                {
                    db.Entry(rev).State = EntityState.Modified;
                    db.SaveChanges();
                    res = true;
                }
            }
            else
            {
                //Insert new one
                Revenue rev = new Revenue();
                rev.LandingFees = val;
                rev.NightOperationFees = cal.NightOperationFees(model.ATA.Value.Hours, model.ATD.Value.Hours, val);
                rev.OpenAireportOvertimeFees = cal.OpenAireportOvertimeFees(model.ATA.Value.Hours, val);
                rev.NavigationFees = cal.NavigationFees(val);
                var AirLine = db.AirLines.FirstOrDefault(x => x.Id == model.AirLineId);
                rev.ParkingFees = cal.ParkingFees(OfficerData.Id, Convert.ToDateTime(model.LandingDate), Convert.ToDateTime(model.TakeOffDate), model.ATA.Value, model.ATD.Value, Convert.ToBoolean(AirLine.IsLocalCompany));
                rev.NoiseFees = cal.NoiseFees(wt);

                rev.SecurityFees = cal.SecurityFees(wt, Convert.ToInt32(model.FlightTypeL), Convert.ToBoolean(AirLine.IsLocalCompany));

                db.Revenues.Add(rev);
                db.SaveChanges();

                res = true;
            }

            return res;
        }




        private bool calculateFeesUpdate(TowerData towerData, UnScheduledModel model)
        {
            bool res = false;

            OfficerData officerData = db.OfficerDatas
                .FirstOrDefault(x => x.TowerDataId == model.IdAr);

            if (officerData == null)
                return false;

            decimal wt = Convert.ToDecimal(
                db.AircraftRegistrations
                .Include(x => x.AircraftType.AircraftSize)
                .Single(x => x.Id == model.AircraftRegIdAr)
                .MaxTakoffWieght);

            decimal val = cal.LandingFees(wt, Convert.ToInt32(model.FlightTypeL));

            var AirLine = db.AirLines
                .FirstOrDefault(x => x.Id == model.AirLineIdAr);

            if (db.Revenues.Any(x => x.OfficerDataId == officerData.Id))
            {
                // Update
                Revenue rev = db.Revenues
                    .FirstOrDefault(x => x.OfficerDataId == officerData.Id);

                rev.LandingFees = val;

                rev.NightOperationFees = cal.NightOperationFees(model.ATAAr.Value.Hours, model.ATDAr.Value.Hours, val);

                rev.OpenAireportOvertimeFees = cal.OpenAireportOvertimeFees(model.ATAAr.Value.Hours, val);

                rev.NavigationFees = cal.NavigationFees(val);

                rev.ParkingFees = cal.ParkingFeesmodel(towerData, Convert.ToDateTime(model.LandingDateAr), Convert.ToDateTime(model.TakeOffDateAr), model.ATAAr.Value, model.ATDAr.Value, Convert.ToBoolean(AirLine.IsLocalCompany));

                rev.NoiseFees = cal.NoiseFees(wt);

                rev.SecurityFees = cal.SecurityFees(wt, Convert.ToInt32(model.FlightTypeL), Convert.ToBoolean(AirLine.IsLocalCompany));

                ///---------------------------CUPPS and NY--------------------------------------------------------

                if (model.FlightTypeD == 2)
                {
                    rev.DomeNYFees = cal.DomeNYFees(Convert.ToInt32(model.Pax), AirLine.IsLocalCompany);

                    rev.DomeCUPPSFees = cal.DomeCUPPSFees(Convert.ToInt32(model.Pax), Convert.ToInt32(model.TRNYCUPPS), Convert.ToBoolean(model.IsNotUsedCUPPS));

                    rev.InterCUPPSFees = 0;
                    rev.InterNYFees = 0;
                }
                else
                {
                    rev.InterCUPPSFees = cal.InterCUPPSFees(Convert.ToInt32(model.Pax), Convert.ToInt32(model.TRNYCUPPS), Convert.ToBoolean(model.IsNotUsedCUPPS));

                    rev.InterNYFees = cal.InterNYFees(Convert.ToInt32(model.Pax), AirLine.IsLocalCompany, model.CompanyInfoId);

                    rev.DomeCUPPSFees = 0;
                    rev.DomeNYFees = 0;
                }
                officerData.Pax = model.Pax;
                officerData.TRNYCUPPS = model.TRNYCUPPS;
              

                rev.AmbolanceFees = cal.AmbulanceFees(Convert.ToInt32(model.AmbulanceCarCount));
                rev.FireFightingFees = cal.FireFightingFees(Convert.ToInt32(model.FireFightingCarCount));
              
                officerData.AmbulanceCarCount = model.AmbulanceCarCount;
                officerData.FireFightingCarCount = model.FireFightingCarCount;

                ///---------------------------friend and Mail--------------------------------------------------------
                rev.ExpMailFees = cal.ExpMailFees(Convert.ToDecimal(model.NormalMailLoadingExp), Convert.ToDecimal(model.RapidMailLoadingExp));

                rev.ImpMailFees = cal.ImpMailFees( Convert.ToDecimal(model.NormalMailLoadingImp),  Convert.ToDecimal(model.RapidMailLoadingImp));

                rev.FreightFees = cal.FreightFees( Convert.ToDecimal(model.FreightLoadingExp), Convert.ToDecimal(model.FreightLoadingImp));

                officerData.NormalMailLoadingExp = model.NormalMailLoadingExp;
                officerData.NormalMailLoadingImp = model.NormalMailLoadingImp;
              
                officerData.RapidMailLoadingExp = model.RapidMailLoadingExp;
                officerData.RapidMailLoadingImp = model.RapidMailLoadingImp;

                officerData.FreightLoadingExp = model.FreightLoadingExp;
                officerData.FreightLoadingImp = model.FreightLoadingImp;
                ///---------------------------Push Back--------------------------------------------------------

                if (Convert.ToBoolean(model.hasPushPack))
                {
                    string size = db.AircraftRegistrations
                        .Include(x => x.AircraftType.AircraftSize)
                        .FirstOrDefault(x => x.Id == model.AircraftRegIdAr)
                        .AircraftType.AircraftSize.Size;

                    rev.PushBackFees = cal.PushBackFees(size);
                    officerData.HasPushPack = true;
                }
                else
                {
                    rev.PushBackFees = 0;
                    officerData.HasPushPack = false;
                }


                /// Fuel


                officerData.FuelLiter = model.FuelLiter;
                rev.FuelFees = Calculation.FuelPerLiterFees(Convert.ToDecimal(model.FuelLiter));

                officerData.IsCurrentAccount = true;

                if (ModelState.IsValid)
                {
                    db.Entry(rev).State = EntityState.Modified;
                    db.Entry(officerData).State = EntityState.Modified;
                    db.SaveChanges();

                    res = true;
                }
            }
            else
            {
                res = false;
            }

            return res;
        }




       



        public ActionResult PrintDataFinalInvoiceUnscheduled(int? Id)
        {
            if (Id == null)
                return RedirectToAction("Error");

            var id = User.Identity.GetUserId();
            Revenue revenue = db.Revenues.Find(Id);
          


           

            int AirLineId = Convert.ToInt32(revenue.OfficerData.TowerData.AirLineId);
            int CurrencyType = Convert.ToInt32(revenue.OfficerData.TowerData.AirLine.CurrencyType);

            int Month = revenue.OfficerData.TowerData.Date.Value.Month;
            int Year = revenue.OfficerData.TowerData.Date.Value.Year;

            decimal ExchangeRate;

            // ===== تحديد سعر الصرف =====
            if (CurrencyType == 1)
            {
                ExchangeRate = commonUsed.getExchangeRateByLandingDate(Month, Year);
            }
            else
            {
                ExchangeRate = 0;
            }

            // ===== جلب Revenue الخاص بهذه الرحلة فقط =====
            

            if (revenue == null)
                return RedirectToAction("Error");

            PrintFinalInvoiceViewModel F = new PrintFinalInvoiceViewModel();

            var airline =revenue.OfficerData.TowerData.AirLine;

            F.Name = airline.Code + "/ " + airline.Name;

            // ===== الحسابات =====

            F.LandingFees = Convert.ToDouble(revenue.LandingFees ?? 0);
            F.ParkingFees = Convert.ToDouble(revenue.ParkingFees ?? 0);
            F.NightOperationFees = Convert.ToDouble(revenue.NightOperationFees ?? 0);
            F.NavigationFees = Convert.ToDouble(revenue.NavigationFees ?? 0);
            F.SecurityFees = Convert.ToDouble(revenue.SecurityFees ?? 0);
            F.FirstClassFees = Convert.ToDouble(revenue.FirstClassFees ?? 0);

            F.InterCUPPSFees = Convert.ToDouble(revenue.InterCUPPSFees ?? 0);
            F.DomeCUPPSFees = Convert.ToDouble(revenue.DomeCUPPSFees ?? 0);
            F.InterNYFees = Convert.ToDouble(revenue.InterNYFees ?? 0);
            F.DomeNYFees = Convert.ToDouble(revenue.DomeNYFees ?? 0);

            F.PushBackFees = Convert.ToDouble(revenue.PushBackFees ?? 0);

            F.ServiceCars =
                Convert.ToDouble(revenue.AmbolanceFees ?? 0) +
                Convert.ToDouble(revenue.FireFightingFees ?? 0);

            F.NoiseFees = Convert.ToDouble(revenue.NoiseFees ?? 0);
            F.ExpMailFees = Convert.ToDouble(revenue.ExpMailFees ?? 0);
            F.ImpMailFees = Convert.ToDouble(revenue.ImpMailFees ?? 0);
            F.FreightFees = Convert.ToDouble(revenue.FreightFees ?? 0);

            // ===== SubTotal =====

            F.SubTotalDollar =
                F.LandingFees + F.ParkingFees + F.NightOperationFees + F.NavigationFees +
                F.SecurityFees + F.FirstClassFees + F.FreightFees + F.ServiceCars +
                F.InterCUPPSFees + F.DomeCUPPSFees + F.InterNYFees + F.DomeNYFees +
                F.PushBackFees + F.ExpMailFees + F.ImpMailFees + F.NoiseFees;

            F.SubTotalSDG = F.SubTotalDollar * Convert.ToDouble(ExchangeRate);

            // ===== الضريبة =====

            F.TaxDollar = (
                F.LandingFees + F.ParkingFees + F.NightOperationFees +
                F.NavigationFees + F.SecurityFees + F.FirstClassFees +
                F.FreightFees + F.ServiceCars + F.InterCUPPSFees +
                F.DomeCUPPSFees + F.PushBackFees +
                F.ExpMailFees + F.ImpMailFees + F.NoiseFees
            ) * 0.05;

            F.TaxSDG = F.TaxDollar * Convert.ToDouble(ExchangeRate);

            // ===== الإجمالي =====

            F.GrandTotalDollar = F.SubTotalDollar + F.TaxDollar;
            F.GrandTotalSDG = F.SubTotalSDG + F.TaxSDG;

            // ===== رقم الفاتورة =====

            int InvoiceNo;
            FinalInvoiceNumber finalInvoiceNumber;

            if (db.FinalInvoiceNumbers.Any(x => x.Month == Month && x.Year == Year))
            {
                if (db.FinalInvoiceNumbers.Any(x => x.AirLineId == AirLineId && x.Month == Month && x.Year == Year))
                {
                    InvoiceNo = db.FinalInvoiceNumbers
                        .First(x => x.AirLineId == AirLineId && x.Month == Month && x.Year == Year)
                        .InvoiceNo.Value;
                }
                else
                {
                    InvoiceNo = db.FinalInvoiceNumbers
                        .Where(x => x.Month == Month && x.Year == Year)
                        .Max(n => n.InvoiceNo).Value + 1;

                    finalInvoiceNumber = new FinalInvoiceNumber()
                    {
                        Month = Month,
                        Year = Year,
                        InvoiceNo = InvoiceNo,
                        AirLineId = AirLineId,
                        IsFinal = false,
                        CreatedBy = id,
                        CreationDate = DateTime.UtcNow.ToShortDateString()
                    };

                    db.FinalInvoiceNumbers.Add(finalInvoiceNumber);
                    db.SaveChanges();
                }
            }
            else
            {
                finalInvoiceNumber = new FinalInvoiceNumber()
                {
                    Month = Month,
                    Year = Year,
                    InvoiceNo = 1,
                    AirLineId = AirLineId,
                    IsFinal = false,
                    CreatedBy = id,
                    CreationDate = DateTime.UtcNow.ToShortDateString()
                };

                db.FinalInvoiceNumbers.Add(finalInvoiceNumber);
                db.SaveChanges();

                InvoiceNo = 1;
            }

            F.txtInvoiceNo = "#" + Month.ToString("00") + Year.ToString() + "00" + InvoiceNo.ToString();

            // ===== Header =====

            F.HeaderDate =revenue.OfficerData.TowerData.Date.Value.ToString("MMM-yyyy", CultureInfo.InvariantCulture);


            F.ExchangeRate = Convert.ToDouble(ExchangeRate);
            F.DateNow = DateTime.Now.ToString();
            F.DateFrom = CommonUsed.ToDisplayDate(revenue.OfficerData.TowerData.Date);
            F.DateTo = CommonUsed.ToDisplayDate(revenue.OfficerData.TowerData.Date);

            if (CurrencyType == 1)
            {
                double SDGRounded = Math.Round(F.GrandTotalSDG, 2);
                F.NumberWord = CurrencyFormat.ConvertToWords(SDGRounded.ToString(), "SDG");

                ViewBag.Header = "Final-" + airline.Name + "-" + DateTime.Now.ToShortDateString();
                return View(F);
            }
            else
            {
                double DollarRounded = Math.Round(F.GrandTotalDollar, 2);
                F.NumberWord = CurrencyFormat.ConvertToWords(DollarRounded.ToString(), "Dollar");

                ViewBag.Header = "Final-" + airline.Name + "-" + DateTime.Now.ToShortDateString();

                return View("PrintDataFinalInvoiceNotSDG", F);
            }
        }

        public ActionResult PrintInoviceDetailUnscheduled(int? Id)
        {
            if (Id == null)
                return RedirectToAction("Error");

            var id = User.Identity.GetUserId();
            string UserName = commonUsed.GetUserName(id);

            // جلب بيانات الرحلة
         
         


            // جلب revenue للرحلة فقط
            Revenue item = db.Revenues
                .Find(Id);

            if (item == null)
                return RedirectToAction("Error");


            // =================== تحويل إلى ViewModel ===================
            //ExchangeRate = Convert.ToDecimal(financialClaim.ExchangeRateAgainstPrimaryCurrency);

            PrintInoviceDetailViewModel L = new PrintInoviceDetailViewModel();
            //L.ExchangeRateAgainstPrimaryCurrency = ExchangeRate;
            L.Id = item.Id;
            L.N = 1;
            L.FlightNo = item?.OfficerData.TowerData.AirLinesFlightTrip.FlightNo;
            L.MaxTakoffWieght = item?.OfficerData.TowerData.AircraftRegistration.MaxTakoffWieght.ToString();
            L.LandingDate = CommonUsed.ToDisplayDate(item?.OfficerData.TowerData.LandingDate);
            L.Date = CommonUsed.ToDisplayDate(item?.OfficerData.TowerData.Date);

            L.Wt = Convert.ToDouble(item.OfficerData.TowerData.AircraftRegistration.MaxTakoffWieght).ToString();
            L.Type = item.OfficerData.TowerData.AircraftRegistration.AircraftType.Type.ToString();
            L.RegNo = item.OfficerData.TowerData.AircraftRegistration.Registration.ToString();

            L.LandingFees = Convert.ToDecimal(item?.LandingFees);
            L.ParkingFees = Convert.ToDecimal(item?.ParkingFees);
            L.NightOperationFees = Convert.ToDecimal(item?.NightOperationFees);
            L.NavigationFees = Convert.ToDecimal(item?.NavigationFees);
            L.SecurityFees = Convert.ToDecimal(item?.SecurityFees);
            L.FreightFees = Convert.ToDecimal(item?.FreightFees);
            L.NoiseFees = Convert.ToDecimal(item?.NoiseFees);
            L.ImpMailFees = Convert.ToDecimal(item?.ImpMailFees);
            L.ExpMailFees = Convert.ToDecimal(item?.ExpMailFees);

            L.TotalLanding =
                L.ParkingFees +
                L.LandingFees +
                L.NightOperationFees +
                L.NavigationFees +
                L.FreightFees +
                L.SecurityFees +
                L.NoiseFees +
                L.ExpMailFees +
                L.ImpMailFees;


            double LandingTax = Convert.ToDouble(L.TotalLanding) * 0.05;
            ViewBag.LandingTax = LandingTax;
            ViewBag.LandingGrandTotal = LandingTax + Convert.ToDouble(L.TotalLanding);



            ///-----------------------------------------------------------------------------------------------------
            L.Pax = Convert.ToInt32(item?.OfficerData.Pax);
            L.TR = Convert.ToInt32(item?.OfficerData.TRNYCUPPS);


            L.AirPortTo = item?.OfficerData.TowerData.AirPort.Code;
            //CInter.TakeOffDate = CommonUsed.CheckDate(item?.OfficerData.TowerData.TakeOffDate.ToString());
            L.TakeOffDate = CommonUsed.ToDisplayDate(item?.OfficerData.TowerData.LandingDate);//تم تعديلها نسبة لان بعض الرحلات تكون بايتة وتظهر بتاريخ الشهر الجديد ودي مشكلة
            L.FlightNo = item?.OfficerData.TowerData.AirLinesFlightTrip.FlightNo;

            if (item?.OfficerData.TowerData.FlightTypeD == 1) // International cupps
            {
                L.CuppsFees = Convert.ToDecimal(item?.InterCUPPSFees);
                if (L.CuppsFees == 0)
                {
                    L.CFees = 0;
                }
                else
                {
                    L.CFees = (L.CuppsFees / (L.Pax + L.TR));
                }

                //L.CUPPSSDG = L.CuppsFees * ExchangeRate;
                //----------------------------------------------------------------------------------------------------------------
            }
            else
            {
                L.CuppsFees = Convert.ToDecimal(item?.DomeCUPPSFees);
                if (L.CuppsFees == 0)
                {
                    L.CFees = 0;
                }
                else
                {
                    L.CFees = (L.CuppsFees / (L.Pax + L.TR));
                }

                //L.CUPPSSDG = L.CuppsFees * ExchangeRate;
            }


            


            double CuppsTaxDollar = Convert.ToDouble(L.CuppsFees) * 0.05;
            ViewBag.CuppsTaxDollar = CuppsTaxDollar;

            //double TaxSDG = Convert.ToDouble(L.CUPPSSDG) * 0.05;
            //ViewBag.TaxSDG = TaxSDG;


            double CuppsGrandTotalDollar = Convert.ToDouble(L.CuppsFees) + CuppsTaxDollar;
            ViewBag.CuppsGrandTotalDollar = CuppsGrandTotalDollar;

            //double GrandTotalSDG = Convert.ToDouble(L.CUPPSSDG) + TaxSDG;
            //ViewBag.GrandTotalSDG = GrandTotalSDG;

            //--------------------------------------------------------

            if (item?.OfficerData.TowerData.FlightTypeD == 1) // International cupps
            {
                L.Pax = Convert.ToInt32(item?.OfficerData.Pax);


                L.AirPortTo = item?.OfficerData.TowerData.AirPort.Code;
                //N.TakeOffDate = CommonUsed.CheckDate(item?.OfficerData.TowerData.TakeOffDate.ToString());
                L.TakeOffDate = CommonUsed.ToDisplayDate(item?.OfficerData.TowerData.LandingDate);//تم تعديلها نسبة لان بعض الرحلات تكون بايتة وتظهر بتاريخ الشهر الجديد ودي مشكلة
                L.FlightNo = item?.OfficerData.TowerData.AirLinesFlightTrip.FlightNo;
                L.NyFees = Convert.ToDecimal(item?.InterNYFees);
                if (L.NyFees == 0)
                {
                    L.NFees = 0;
                }
                else
                {
                    L.NFees = (L.NyFees / L.Pax);
                }

                //L.NyAmountSDG = L.NyFees * ExchangeRate;

            }
            else
            {
                L.Pax = Convert.ToInt32(item?.OfficerData.Pax);
                L.AirPortTo = item?.OfficerData.TowerData.AirPort.Code;

                L.TakeOffDate = CommonUsed.ToDisplayDate(item?.OfficerData.TowerData.LandingDate);//تم تعديلها نسبة لان بعض الرحلات تكون بايتة وتظهر بتاريخ الشهر الجديد ودي مشكلة
                L.FlightNo = item?.OfficerData.TowerData.AirLinesFlightTrip.FlightNo;
                L.NyFees = Convert.ToDecimal(item?.DomeNYFees);
                if (L.NyFees == 0)
                {
                    L.NFees = 0;
                }
                else
                {
                    L.NFees = (L.NyFees / L.Pax);
                }

                //L.NyAmountSDG = L.NyFees * ExchangeRate;
            }




            double NyFees = Convert.ToDouble(L.NyFees);
            ViewBag.NyFees = NyFees;
            //double NYFeesSDG = NyFees * Convert.ToDouble(ExchangeRate);
            //ViewBag.NYFeesSDG = NYFeesSDG;



            ///----------------------------------Start Service Car-------------------------------------------------------





            L.AmbulanceNo = Convert.ToInt32(item?.OfficerData.AmbulanceCarCount);
            L.FeesAmbulanceAmount = Convert.ToDecimal(item?.AmbolanceFees);

            if (L.FeesAmbulanceAmount == 0)
            {
                L.FeesAmbulanceAmount = 0;
            }
            else
            {
                L.FeesAmbulance = Convert.ToDecimal(L.FeesAmbulanceAmount / L.AmbulanceNo);
            }
            L.FireNo = Convert.ToInt32(item?.OfficerData.FireFightingCarCount);
            L.FeesFireAmount = Convert.ToDecimal(item?.FireFightingFees);
            if (L.FeesFireAmount == 0)
            {
                L.FeesFire = 0;
            }
            else
            {
                L.FeesFire = Convert.ToDecimal(L.FeesFireAmount / L.FireNo);
            }
            //L.AmbulanceSDG = L.FeesAmbulanceAmount * ExchangeRate;
            //L.FireSDG = L.FeesFireAmount * ExchangeRate;
            L.ServiceCar = Convert.ToDecimal(L.FeesAmbulanceAmount + L.FeesFireAmount);
            L.ServiceCarSDG = Convert.ToDecimal(L.AmbulanceSDG + L.FireSDG);

            double ServiceCarTaxDollar = Convert.ToDouble(L.ServiceCar) * 0.05;
            ViewBag.ServiceCarTaxDollar = ServiceCarTaxDollar;

            double ServiceCarTaxSDG = Convert.ToDouble(L.ServiceCarSDG) * 0.05;
            ViewBag.ServiceCarTaxSDG = ServiceCarTaxSDG;


            double ServiceCarGrandTotalDollar = Convert.ToDouble(L.ServiceCar) + ServiceCarTaxDollar;
            ViewBag.ServiceCarGrandTotalDollar = ServiceCarGrandTotalDollar;

            double ServiceCarGrandTotalSDG = Convert.ToDouble(L.ServiceCarSDG) + ServiceCarTaxSDG;
            ViewBag.ServiceCarGrandTotalSDG = ServiceCarGrandTotalSDG;




            if (L.FeesFireAmount > 0 || L.FeesAmbulanceAmount > 0)
            {
                L.HasCarService = true;
            }
            else { L.HasCarService = false; }
            //---------------------------------------End Service Car-------------------------------------------------




            ///------------- Start Pushpack
            L.HasPushPack = Convert.ToBoolean(item.OfficerData.HasPushPack);


            L.AirPortTo = item?.OfficerData.TowerData.AirPort.Code;
            L.PlaneSize = item?.OfficerData.TowerData.AircraftRegistration.AircraftType.AircraftSize.Size;
            L.Wt = item?.OfficerData.TowerData.AircraftRegistration.MaxTakoffWieght.ToString();
            L.PushPackFees = Convert.ToDecimal(item?.PushBackFees);

            //L.PushPackAmountSDG = ExchangeRate * L.PushPackFees;


            double PushPackTaxDollar = Convert.ToDouble(L.PushPackFees) * 0.05;
            ViewBag.PushPackTaxDollar = PushPackTaxDollar;

            double PushPackTaxSDG = Convert.ToDouble(L.PushPackAmountSDG) * 0.05;
            ViewBag.PushPackTaxSDG = PushPackTaxSDG;

            // Grand Total
            double PushPackGrandTotalDollar = Convert.ToDouble(L.PushPackFees) + PushPackTaxDollar;
            ViewBag.PushPackGrandTotalDollar = PushPackGrandTotalDollar;

            double PushPackGrandTotalSDG = Convert.ToDouble(L.PushPackAmountSDG) + PushPackTaxSDG;
            ViewBag.PushPackGrandTotalSDG = PushPackGrandTotalSDG;

            //----------------------------------------------------------------------------------


            L.TakeOffDate = CommonUsed.ToDisplayDate(item?.OfficerData.TowerData.TakeOffDate);

            // L.MaxTakoffWieght = item?.OfficerData.TowerData.AircraftRegistration.AircraftType.Type.ToString();

            L.FirstClassCount = Convert.ToInt32(item?.OfficerData.FirstClassCount);

            L.FirstClassFees = Convert.ToDecimal(item?.FirstClassFees);

            //L.FirstClassFeesSDG = Convert.ToDecimal(item?.FirstClassFees) * Convert.ToDecimal(ExchangeRate);
            if (L.FirstClassFees == 0)
            {
                L.FFees = 0;
            }
            else
            {
                L.FFees = (L.FirstClassFees / L.FirstClassCount);

            }



            /*************************************************************************************/

            double FirstClassTaxDollar = Convert.ToDouble(L.FirstClassFees) * 0.05;
            ViewBag.FirstClassTaxDollar = FirstClassTaxDollar;

            double FirstClassTaxSDG = Convert.ToDouble(L.FirstClassFeesSDG) * 0.05;
            ViewBag.FirstClassTaxSDG = FirstClassTaxSDG;

            // Grand Total
            double FirstClassGrandTotalDollar = Convert.ToDouble(L.FirstClassFees) + FirstClassTaxDollar;
            ViewBag.FirstClassGrandTotalDollar = FirstClassGrandTotalDollar;

            double FirstClassGrandTotalSDG = Convert.ToDouble(L.FirstClassFeesSDG) + FirstClassTaxSDG;
            ViewBag.FirstClassGrandTotalSDG = FirstClassGrandTotalSDG;


            if (L.FirstClassFees > 0)
            {
                L.HasFirstClass = true;
            }
            else { L.HasFirstClass = false; }







            if (item.OfficerData.TowerData.AirLineId > 0)
            {
                AirLine airline = db.AirLines.Find(item.OfficerData.TowerData.AirLineId);
                ViewBag.Name = airline.Name + "/" + airline.Code;
            }

            ViewBag.Header = "Landing-" + ViewBag.Name + "-" + DateTime.Now.ToShortDateString();


            ///------------- Start Fuel
            L.FuelLiter = Convert.ToDecimal(item.OfficerData.FuelLiter);
            L.FuelFees = Convert.ToDecimal(item?.FuelFees);

            double FuelTaxDollar = Convert.ToDouble(L.FuelFees) * 0.05;
            ViewBag.FuelTaxDollar = FuelTaxDollar;


            // Grand Total
            double FuelFeesGrandTotalDollar = Convert.ToDouble(L.FuelFees) + FuelTaxDollar;
            ViewBag.FuelFeesGrandTotalDollar = FuelFeesGrandTotalDollar;

            //----------------------------------------------------------------------------------


            decimal SumWithout =
              L.TotalLanding + L.ServiceCar + L.FirstClassFees + L.CuppsFees + L.PushPackFees + L.FuelFees;
              L.TotalRevenueStampInvoice = SumWithout * 0.05m;

            L.TotalInvoice = L.TotalLanding + L.ServiceCar + L.FirstClassFees + L.CuppsFees + L.NyFees + L.PushPackFees + L.FuelFees;
            L.GrandTotalInvoice = L.TotalInvoice + L.TotalRevenueStampInvoice;


          





            return View(L);

        }










        [Authorize(Roles = "Landing-Admin,Revenues,Super-Admin, Account, Account-Admin")]
        public ActionResult Index()
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

        public ActionResult GetAllTowerDatas()
        {
            IQueryable<Revenue> Revenue;
            var id = User.Identity.GetUserId();
            var companyId = commonUsed.getCompanyId(id);
            var companyInfo = Role.GetCompanyId(id);
            bool IsSingle = commonUsed.getIsSingleAccounting(companyId);

            WorkMouth = commonUsed.GetMouth(id);
            WorkYear = commonUsed.GetYear(id);

            Revenue = db.Revenues.OrderBy(x => x.Id).Where(x => x.OfficerData.TowerData.Date.Value.Month == WorkMouth
            && x.OfficerData.TowerData.Date.Value.Year == WorkYear && x.OfficerData.TowerData.CompanyInfoId == companyId);

            var TowerDatasList = new List<TowerDataViewModels>();

            // var TowerDatasList = new List<TowerDataViewModels>();
            foreach (var item in Revenue)
            {
                TowerDataViewModels tdm = new TowerDataViewModels();

                tdm.Id = item.OfficerData.TowerData.Id;
                tdm.AirLineCode = item?.OfficerData.TowerData.AirLine.Code;
                tdm.AirLineName = item?.OfficerData.TowerData.AirLine.Name;
                tdm.TripType = commonUsed.GetFlightTypeNameLanding(item.OfficerData.TowerData.FlightTypeL);
                tdm.FlightNo = item?.OfficerData.TowerData.AirLinesFlightTrip.FlightNo;
                tdm.AircraftRegName = item?.OfficerData.TowerData.AircraftRegistration.Registration;
                tdm.Type = item?.OfficerData.TowerData.AircraftRegistration.AircraftType.Type;
                tdm.MaxTaKeOffWeight = item?.OfficerData.TowerData.AircraftRegistration.MaxTakoffWieght.ToString();

                //from --------------------------------------------------------------------------------------
                tdm.AirportIdFrom = commonUsed.CheckAirportCode(Convert.ToInt32(item?.OfficerData.TowerData.AirportIdFrom));
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
                tdm.AirportIdTo = commonUsed.CheckAirportCode(Convert.ToInt32(item?.OfficerData.TowerData.AirportIdTo));
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


                tdm.CreatedBy = commonUsed.GetUserName(item.OfficerData.TowerData.CreatedBy);
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
                    tdm.IsStop = commonUsed.IsEntryDateinRange2(item.Id, Role.GetRole(id));  // check date in range in claim table
                }
                else
                {
                    tdm.IsStop = commonUsed.IsEntryDateinRange(Convert.ToDateTime(item.OfficerData.TowerData.Date), Role.GetRole(id),
                  Convert.ToInt32(item.OfficerData.TowerData.AirLineId), Convert.ToInt32(companyId));  // check date in range in claim table
                }
                tdm.AirportName = commonUsed.getCompanyName(item.OfficerData.TowerData.CompanyInfoId);
                TowerDatasList.Add(tdm);
            }


            return Json(new { data = TowerDatasList.ToList() }, JsonRequestBehavior.AllowGet);
        }





        public JsonResult GetAccounts()
        {
            var data = db.AccountTrees
                         .Where(x => x.Id == 6)
                         .Select(x => new
                         {
                             Id = x.Id,
                             Name = x.AccName
                         }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }



        public ActionResult GetAirline()
        {

            var data = db.AirLines.Select(p => new
            {
                Id = p.Id,
                Name = p.Name,

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


        public ActionResult GetAircraftType()
        {

            var data = db.AircraftTypes.Select(p => new
            {
                Id = p.Id,
                Name = p.Type,

            });

            return Json(data, JsonRequestBehavior.AllowGet);

        }


        public ActionResult GetCountry()
        {

            var data = db.Countries.Select(p => new
            {
                Id = p.Id,
                Name = p.NameEn,

            });

            return Json(data, JsonRequestBehavior.AllowGet);

        }




        public ActionResult GetFlightNo(int id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<AirLinesFlightTrip> lstAirLinesFlightTrip = new List<AirLinesFlightTrip>();
            lstAirLinesFlightTrip = db.AirLinesFlightTrips.Where(x => x.AirLineId == id).ToList();

            lstAirLinesFlightTrip.Insert(0, new AirLinesFlightTrip() { FlightNo = "--Select FlightNo--", Id = 0 });
            return Json(lstAirLinesFlightTrip, JsonRequestBehavior.AllowGet);
        }

     

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Revenue revenue = db.Revenues.Find(id);
            if (revenue == null)
            {
                return HttpNotFound();
            }

            OfficerData officerData = db.OfficerDatas.Where(x => x.Id == revenue.OfficerDataId).FirstOrDefault();
            TowerData towerData = db.TowerDatas.Where(x => x.Id == officerData.TowerDataId).FirstOrDefault();

          

            if (revenue != null)
            {
                var RevenueDelete = db.Revenues.Find(revenue.Id);
                db.Revenues.Remove(RevenueDelete);
            }

            if (officerData != null)
            {
                var OfficerDataDelete = db.OfficerDatas.Find(officerData.Id);
                db.OfficerDatas.Remove(OfficerDataDelete);
            }

            if (towerData != null)
            {
                var TowerDataDelete = db.TowerDatas.Find(officerData.Id);
                db.TowerDatas.Remove(TowerDataDelete);
            }

            db.SaveChanges();
            return Json(new { success = true, message = "Delete" }, JsonRequestBehavior.AllowGet);
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