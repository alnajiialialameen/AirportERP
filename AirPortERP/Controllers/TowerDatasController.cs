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
    public class TowerDatasController : Controller
    {
        // Calculation Calculation = new Calculation();

        DateTime now = System.DateTime.Now;
        private CommonUsed CommonUsed = new CommonUsed();
        private Entities db = new Entities();
        RoleController Role = new RoleController();
        private Calculation cal = new Calculation();
        int WorkMouth;
        int WorkYear;
        public TowerDatasController()
        {

        }

        [Authorize(Roles = "Landing-Admin,Revenues,Super-Admin, Account, Account-Admin")]
        public ActionResult DailyMovement()
        {
            return View();
        }

        /// <summary>
        /// ////////////////////////////////////////////////Start All//////////////////////////////////////////////////////////////////////////
        ///   /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///   /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

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
            var companyId = CommonUsed.getCompanyId(id);
            var companyInfo = Role.GetCompanyId(id);
            bool IsSingle = CommonUsed.getIsSingleAccounting(companyId);

            WorkMouth = CommonUsed.GetMouth(id);
            WorkYear = CommonUsed.GetYear(id);

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
                TowerDatasList.Add(tdm);
            }


            return Json(new { data = TowerDatasList.ToList() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAircraftAllParial(int? id)
        {
            TowerData towerData = new TowerData();

            if (id > 0)
            {
                towerData = db.TowerDatas.Find(id);
            }
            ViewBag.AircraftRegId = new SelectList(db.AircraftRegistrations, "Id", "Registration", towerData.AircraftRegId);
            ViewBag.AirLineId = new SelectList(db.AirLines, "Id", "Code", towerData.AirLineId);
            ViewBag.FlightNo = new SelectList(db.AirLinesFlightTrips, "Id", "FlightNo", towerData.FlightNo);
            ViewBag.AirportIdTo = new SelectList(db.AirPorts, "Id", "Code", towerData.AirportIdTo);
            ViewBag.AirportIdFrom = new SelectList(db.AirPorts, "Id", "Code", towerData.AirportIdFrom);
            ViewBag.CompanyInfoId = new SelectList(db.CompanyInfoes, "Id", "NameAr", towerData.CompanyInfoId);
            ViewBag.TripTypeId = new SelectList(db.TripTypes, "Id", "Name", towerData.TripTypeId);
            if (towerData.ATA == null || towerData.LandingDate == null)
            {
                //return PartialView("_AddEditTowerDataAll", towerData);

                return PartialView("_AddEditTowerDataTakeOff", towerData);
            }
            else
            {
                return PartialView("_AddEditTowerDataAll", towerData);
            }
        }

        // دي دالة تعديل فقط ما فيها اضافة لقيتها كده
        public ActionResult CreateOrUpdateAll(TowerData model)
        {
            if (ModelState.IsValid)
            {
                var id = User.Identity.GetUserId();
                var userRole = Role.GetRole(id);
                if (model.Id > 0)
                {

                    //if ((model.TakeOffDate == null && model.ATD != null) || (model.TakeOffDate != null && model.ATD == null))
                    //{
                    //    return Json(new { success = true, message = "Repeat" }, JsonRequestBehavior.AllowGet);
                    //}

                    //if (model.TakeOffDate != null)
                    //{
                    //    if (Convert.ToDateTime(model.TakeOffDate).Date > Convert.ToDateTime(model.LandingDate).Date)
                    //    {
                    //        return Json(new { success = true, message = "InvalidDate" }, JsonRequestBehavior.AllowGet);
                    //    }
                    //}

                    if (db.TowerDatas.Any(x => x.AirLineId == model.AirLineId && x.ATA == model.ATA && x.ATD == model.ATD && x.Date == model.Date && x.Id != model.Id))
                    {
                        return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
                    }

                    if (model.TakeOffDate == null && model.ATD == null)
                    {
                        model.Status = 1;
                    }




                    model.UpdatedBy = id;
                    model.UpdatingDate = System.DateTime.Now;

                    //if (model.ATA == null)
                    //{
                    //    model.LandingDate = System.DateTime.Now;
                    //}
                    //else
                    //{
                    //    model.LandingDate = null;
                    //}

                    //if (model.ATD == null)
                    //{
                    //    model.TakeOffDate = System.DateTime.Now;
                    //}


                    ///////////////////////////******* Start Check FlightType***/////////////////////////////////////
                    if (model.AirportIdFrom != null)
                    {
                        AirPort Afrom = db.AirPorts.Single(x => x.Id == model.AirportIdFrom);

                        if (Afrom.CountryId == 1)
                        {
                            // Domistic
                            model.FlightTypeL = 2;
                        }
                        else
                        {    // InterNational
                            model.FlightTypeL = 1;
                        }
                    }
                    ///////////////////////////******* Start Check FlightType***/////////////////////////////////////
                    if (model.AirportIdTo != null)
                    {
                        AirPort ATo = db.AirPorts.Single(x => x.Id == model.AirportIdTo);
                        if (ATo.CountryId == 1)
                        {
                            // Domistic
                            model.FlightTypeD = 2;
                        }
                        else
                        {    // InterNational
                            model.FlightTypeD = 1;
                        }
                    }

                    //********************************End Check Status***********************************
                    if (model.ATA == null && model.LandingDate == null)
                    {
                        model.AirportIdFrom = null;
                    }



                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Update" }, JsonRequestBehavior.AllowGet);
                }


            }
            return Json(new { success = false, message = "error" }, JsonRequestBehavior.AllowGet);
        }
        /// ////////////////////////////////////////////////End All//////////////////////////////////////////////////////////////////////////

        /// /////////TTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTT//////////////////////////Start Take Off////TTTTTTTTTTTTTTTTTT////////////////////////////////////////////////////
        /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //*************************************************************************************************************************

        [Authorize(Roles = "Landing-Admin,Revenues,Super-Admin, Account, Account-Admin")]
        public ActionResult TakeOff()
        {
            return View();
        }

        public ActionResult GetAllTowerDatasTakeOff()
        {
            var id = User.Identity.GetUserId();
            var companyId = CommonUsed.getCompanyId(id);
            bool IsSingle = CommonUsed.getIsSingleAccounting(companyId);
            IQueryable<Revenue> Revenue = db.Revenues.Where(x => DbFunctions.TruncateTime(x.OfficerData.TowerData.Date.Value).ToString() ==
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
                TowerDatasList.Add(tdm);
            }

            return Json(new { data = TowerDatasList.ToList() }, JsonRequestBehavior.AllowGet);
        }
        //public ActionResult GetAircraftTakeOffParial(int? id)
        //{
        //    TowerData towerData = new TowerData();

        //    if (id > 0)
        //    {
        //        towerData = db.TowerDatas.Find(id);
        //    }

        //    ViewBag.AircraftRegId = new SelectList(db.AircraftRegistrations, "Id", "Registration", towerData.AircraftRegId);
        //    ViewBag.AirLineId = new SelectList(db.AirLines, "Id", "Code", towerData.AirLineId);
        //    ViewBag.FlightNo = new SelectList(db.AirLinesFlightTrips, "Id", "FlightNo", towerData.FlightNo);
        //    ViewBag.AirportIdTo = new SelectList(db.AirPorts, "Id", "Code", towerData.AirportIdTo);
        //    ViewBag.AirportIdFrom = new SelectList(db.AirPorts, "Id", "Code", towerData.AirportIdFrom);
        //    ViewBag.CompanyInfoId = new SelectList(db.CompanyInfoes, "Id", "NameAr", towerData.CompanyInfoId);
        //    ViewBag.TripTypeId = new SelectList(db.TripTypes, "Id", "Name", towerData.TripTypeId);

        //    ViewBag.MaxTakoffWieght = towerData.AircraftRegistration.MaxTakoffWieght;
        //   /////return PartialView("_AddEditTowerDataAll", towerData);

        //    return PartialView("_AddEditTowerDataTakeOff", towerData);
        //}






        public ActionResult GetAircraftTakeOffParial(int? id)
        {
            TowerDataDto model = new TowerDataDto();

            if (id.HasValue && id > 0)
            {
                // جلب الـ Entity من قاعدة البيانات
                TowerData towerData = db.TowerDatas.Find(id);

                if (towerData != null)
                {
                    // تحويل Entity إلى DTO
                    model.Id = towerData.Id;
                    model.AirLineId = towerData.AirLineId;
                    model.FlightNo = towerData.FlightNo;
                    model.AircraftRegId = towerData.AircraftRegId;
                    model.AirportIdFrom = towerData.AirportIdFrom;
                    model.AirportIdTo = towerData.AirportIdTo;
                    model.FlightTypeD = towerData.FlightTypeD;
                    model.FlightTypeL = towerData.FlightTypeL;
                    model.Status = towerData.Status;
                    model.TripTypeId = towerData.TripTypeId;
                    model.CompanyInfoId = towerData.CompanyInfoId;
                    model.Note = towerData.Note;

                    // تحويل التواريخ إلى string بصيغة dd/MM/yyyy
                    model.Date = towerData.Date.HasValue ? towerData.Date.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : null;
                    model.LandingDate = towerData.LandingDate.HasValue ? towerData.LandingDate.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : null;
                    model.TakeOffDate = towerData.TakeOffDate.HasValue ? towerData.TakeOffDate.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : null;

                    // TimeSpan مباشرة
                    model.ATA = towerData.ATA;
                    model.ATD = towerData.ATD;

                    // Created/Updated info
                    model.CreatedBy = towerData.CreatedBy;
                    model.CreationDate = towerData.CreationDate;
                    model.UpdatedBy = towerData.UpdatedBy;
                    model.UpdatingDate = towerData.UpdatingDate;

                    // تمرير MaxTakeoffWeight من العلاقة مع AircraftRegistration
                    ViewBag.MaxTakoffWieght = towerData.AircraftRegistration?.MaxTakoffWieght ?? 0;
                }
            }

            // تجهيز SelectLists باستخدام الـ DTO
            ViewBag.AircraftRegId = new SelectList(db.AircraftRegistrations, "Id", "Registration", model.AircraftRegId);
            ViewBag.AirLineId = new SelectList(db.AirLines, "Id", "Code", model.AirLineId);
            ViewBag.FlightNo = new SelectList(db.AirLinesFlightTrips, "Id", "FlightNo", model.FlightNo);
            ViewBag.AirportIdTo = new SelectList(db.AirPorts, "Id", "Code", model.AirportIdTo);
            ViewBag.AirportIdFrom = new SelectList(db.AirPorts, "Id", "Code", model.AirportIdFrom);
            ViewBag.CompanyInfoId = new SelectList(db.CompanyInfoes, "Id", "NameAr", model.CompanyInfoId);
            ViewBag.TripTypeId = new SelectList(db.TripTypes, "Id", "Name", model.TripTypeId);

            return PartialView("_AddEditTowerDataTakeOff", model);
        }



        public ActionResult GetAircraftTakeOffParialU(int? id)
        {
            TowerDataDtoU model = new TowerDataDtoU();

            if (id.HasValue && id > 0)
            {
                // جلب الـ Entity من قاعدة البيانات
                TowerData towerData = db.TowerDatas.Find(id);

                if (towerData != null)
                {
                    // تحويل Entity إلى DTO
                    model.IdU = towerData.Id;
                    model.AirLineIdU = towerData.AirLineId;
                    model.FlightNoU = towerData.FlightNo;
                    model.AircraftRegIdU = towerData.AircraftRegId;
                    model.AirportIdFromU = towerData.AirportIdFrom;
                    model.AirportIdToU = towerData.AirportIdTo;
                    model.FlightTypeDU = towerData.FlightTypeD;
                    model.FlightTypeLU = towerData.FlightTypeL;
                    model.StatusU = towerData.Status;
                    model.TripTypeIdU = towerData.TripTypeId;
                    model.CompanyInfoIdU = towerData.CompanyInfoId;
                    model.NoteU = towerData.Note;

                    // تحويل التواريخ إلى string بصيغة dd/MM/yyyy
                    model.DateU = towerData.Date.HasValue ? towerData.Date.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : null;
                    model.LandingDateU = towerData.LandingDate.HasValue ? towerData.LandingDate.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : null;
                    model.TakeOffDateU = towerData.TakeOffDate.HasValue ? towerData.TakeOffDate.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : null;

                    // TimeSpan مباشرة
                    model.ATAU = towerData.ATA;
                    model.ATDU = towerData.ATD;

                    // Created/Updated info
                    model.CreatedByU = towerData.CreatedBy;
                    model.CreationDateU = towerData.CreationDate;
                    model.UpdatedByU = towerData.UpdatedBy;
                    model.UpdatingDateU = towerData.UpdatingDate;

                    // تمرير MaxTakeoffWeight من العلاقة مع AircraftRegistration
                    ViewBag.MaxTakoffWieght = towerData.AircraftRegistration?.MaxTakoffWieght ?? 0;
                }
            }

            // تجهيز SelectLists باستخدام الـ DTO
            ViewBag.AircraftRegIdU = new SelectList(db.AircraftRegistrations, "Id", "Registration", model.AircraftRegIdU);
            ViewBag.AirLineIdU = new SelectList(db.AirLines, "Id", "Code", model.AirLineIdU);
            ViewBag.FlightNoU = new SelectList(db.AirLinesFlightTrips, "Id", "FlightNo", model.FlightNoU);
            ViewBag.AirportIdToU = new SelectList(db.AirPorts, "Id", "Code", model.AirportIdToU);
            ViewBag.AirportIdFromU = new SelectList(db.AirPorts, "Id", "Code", model.AirportIdFromU);
            ViewBag.CompanyInfoIdU = new SelectList(db.CompanyInfoes, "Id", "NameAr", model.CompanyInfoIdU);
            ViewBag.TripTypeIdU = new SelectList(db.TripTypes, "Id", "Name", model.TripTypeIdU);

            return PartialView("_AddEditTowerDataTakeOff", model);
        }












        //public ActionResult GetAircraftTakeOffParialM(int? id)
        //{

        //    TowerData towerData = new TowerData();

        //    if (id > 0)
        //    {
        //        towerData = db.TowerDatas.Find(id);
        //    }

        //    ViewBag.AircraftRegId = new SelectList(db.AircraftRegistrations, "Id", "Registration", towerData.AircraftRegId);
        //    ViewBag.AirLineId = new SelectList(db.AirLines, "Id", "Code", towerData.AirLineId);
        //    ViewBag.FlightNo = new SelectList(db.AirLinesFlightTrips, "Id", "FlightNo", towerData.FlightNo);
        //    ViewBag.AirportIdTo = new SelectList(db.AirPorts, "Id", "Code", towerData.AirportIdTo);
        //    ViewBag.AirportIdFrom = new SelectList(db.AirPorts, "Id", "Code", towerData.AirportIdFrom);
        //    ViewBag.CompanyInfoId = new SelectList(db.CompanyInfoes, "Id", "NameAr", towerData.CompanyInfoId);
        //    ViewBag.TripTypeId = new SelectList(db.TripTypes, "Id", "Name", towerData.TripTypeId);

        //    //return PartialView("_AddEditTowerDataAll", towerData);

        //    return PartialView("_AddEditTowerDataTakeOffM", towerData);
        //}




        //public ActionResult CreateOrUpdateTakeOff(TowerData model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var id = User.Identity.GetUserId();
        //        // اختبر التاريخ بين في المطالبات المالية
        //        bool isHasClaim = CommonUsed.IsEntryDateinRange(Convert.ToDateTime(model.Date), Role.GetRole(id), Convert.ToInt32(model.AirLineId));
        //        if (isHasClaim)
        //        {
        //            return Json(new { success = true, message = "isStop" }, JsonRequestBehavior.AllowGet);
        //        }
        //        //using (var transaction = db.Database.BeginTransaction())
        //        //{
        //        //    try
        //        //    {
        //        ///----------------------------------------------------------------
        //        /// 
        //        int TransactionId = Convert.ToInt32(db.SpLastTransactionid().FirstOrDefault());
        //        //    TransactionLog TransactionLog = new TransactionLog();
        //        //   TransactionLog.Id = TransactionLogId;
        //        //  db.TransactionLogs.Add(TransactionLog);
        //        //  db.SaveChanges();
        //        //  int TransactionId = db.TransactionLogs.ToList().OrderBy(x => x.Id).Last().Id;
        //        TransactionLog MyTransactionLog = new TransactionLog();

        //        int TowerId = Convert.ToInt32(db.SpLastTowerid().FirstOrDefault());



        //        ///---------------------------------------------------------------------



        //        if (model.Id > 0)
        //        {
        //            model.UpdatedBy = id;
        //            model.UpdatingDate = System.DateTime.Now;
        //            if (db.TowerDatas.Any(x => x.AirLineId == model.AirLineId && x.ATA == model.ATA && x.ATD == model.ATD && x.Date == model.Date && x.Id != model.Id && x.FlightNo == model.FlightNo))
        //            {
        //                return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
        //            }

        //            /////////////////////////////******* Start Check FlightType****//////////////////////////////////////
        //            //AirPort Afrom = db.AirPorts.Single(x => x.Id == model.AirportIdFrom);
        //            AirPort ATo = db.AirPorts.Single(x => x.Id == model.AirportIdTo);
        //            if (ATo.CountryId == 1)
        //            {
        //                // Domistic
        //                model.FlightTypeD = 2;
        //            }
        //            else
        //            {    // InterNational
        //                model.FlightTypeD = 1;
        //            }

        //            AirPort AFrom = db.AirPorts.Single(x => x.Id == model.AirportIdFrom);
        //            if (AFrom.CountryId == 1)
        //            {
        //                // Domistic
        //                model.FlightTypeL = 2;
        //            }
        //            else
        //            {    // InterNational
        //                model.FlightTypeL = 1;
        //            }



        //            db.Entry(model).State = System.Data.Entity.EntityState.Modified;
        //            //   cal.LandingFees(model.Id);
        //            db.SaveChanges();




        //            bool res = calculateFeesUpdate(model);
        //            bool resNyAndCupps = ChangeNyAndCupps(model);


        //            if (!res)
        //            {
        //                return Json(new { success = true, message = "CalculationFeesError" }, JsonRequestBehavior.AllowGet);
        //            }
        //            return Json(new { success = true, message = "Update" }, JsonRequestBehavior.AllowGet);
        //        }
        //        else
        //        {


        //            if (db.TowerDatas.Any(x => x.AirLineId == model.AirLineId && x.ATA == model.ATA && x.ATD == model.ATD && x.Date == model.Date && x.FlightNo == model.FlightNo))

        //            {
        //                //  return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
        //                return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);

        //            }

        //            model.CreatedBy = id;
        //            model.CreationDate = System.DateTime.Now;

        //            /////////////////////////////******* Start Check FlightType******////////////////////////////////////////
        //            //AirPort Afrom = db.AirPorts.Single(x => x.Id == model.AirportIdFrom);
        //            AirPort ATo = db.AirPorts.Single(x => x.Id == model.AirportIdTo);
        //            if (ATo.CountryId == 1)
        //            {
        //                // Domistic
        //                model.FlightTypeD = 2;
        //            }
        //            else
        //            {    // InterNational
        //                model.FlightTypeD = 1;
        //            }

        //            AirPort AFrom = db.AirPorts.Single(x => x.Id == model.AirportIdFrom);
        //            if (AFrom.CountryId == 1)
        //            {
        //                // Domistic
        //                model.FlightTypeL = 2;
        //            }
        //            else
        //            {    // InterNational
        //                model.FlightTypeL = 1;
        //            }

        //            //****************************Start Check Status*****************************************

        //            model.Status = 3;


        //            // ******************************End Check Status***********************************


        //            model.Id = TowerId;
        //            db.TowerDatas.Add(model);


        //            MyTransactionLog.TowerDataId = TowerId;
        //            MyTransactionLog.Id = TransactionId;
        //            MyTransactionLog.IsComplate = false;
        //            db.TransactionLogs.Add(MyTransactionLog);
        //            db.SaveChanges();



        //            bool res = calculateFees(model, MyTransactionLog, TowerId);
        //            //



        //            if (!res)
        //            {
        //                return Json(new { success = true, message = "CalculationFeesError" }, JsonRequestBehavior.AllowGet);
        //            }
        //            return Json(new { success = true, message = "Add" }, JsonRequestBehavior.AllowGet);



        //        }

        //    }
        //    return Json(new { success = false, message = "error" }, JsonRequestBehavior.AllowGet);
        //}




        public ActionResult CreateOrUpdateTakeOff(TowerDataDto model)
        {    ///get companyInfoId
            var id = User.Identity.GetUserId();
            int?companyInfoId = db.WorkOns.Where(x => x.UserId == id).FirstOrDefault().CompanyInfoId;
            if (!companyInfoId.HasValue || companyInfoId <= 0) { return Json(new { success = true, message = "CompanyInfoIdNotFound" }, JsonRequestBehavior.AllowGet); }

            TowerData towerData = new TowerData();
            // تعيين القيم الأساسية
            towerData.Id = model.Id;
            towerData.AirLineId = model.AirLineId;
            towerData.FlightNo = model.FlightNo;
            towerData.AircraftRegId = model.AircraftRegId;
            towerData.AirportIdFrom = model.AirportIdFrom;
            towerData.AirportIdTo = model.AirportIdTo;
            towerData.FlightTypeD = model.FlightTypeD;
            towerData.FlightTypeL = model.FlightTypeL;
            towerData.Status = model.Status;
            towerData.TripTypeId = model.TripTypeId;
            towerData.CompanyInfoId = model.CompanyInfoId;
            towerData.Note = model.Note;

            // تحويل التاريخ من string إلى DateTime
            towerData.LandingDate = CommonUsed.ConvertToDbDate(model.LandingDate);
            towerData.Date = CommonUsed.ConvertToDbDate(model.Date);
            towerData.TakeOffDate = CommonUsed.ConvertToDbDate(model.TakeOffDate);

            // TimeSpan
            towerData.ATA = model.ATA;
            towerData.ATD = model.ATD;

            // خصائص الإنشاء والتعديل
            towerData.CreatedBy = model.CreatedBy;
            towerData.CreationDate = model.CreationDate;
            towerData.UpdatedBy = model.UpdatedBy;
            towerData.UpdatingDate = model.UpdatingDate;


            if (ModelState.IsValid)
            {
              
                // اختبر التاريخ بين في المطالبات المالية
                //bool isHasClaim = CommonUsed.IsEntryDateinRange1(towerData.Id, Role.GetRole(id));
                //if (isHasClaim)
                //{
                //    return Json(new { success = true, message = "isStop" }, JsonRequestBehavior.AllowGet);
                //}

                int TransactionId = Convert.ToInt32(db.SpLastTransactionid().FirstOrDefault());

                TransactionLog MyTransactionLog = new TransactionLog();

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




                    bool res = calculateFeesUpdate(towerData);
                    bool resNyAndCupps = ChangeNyAndCupps(towerData);


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


                    MyTransactionLog.TowerDataId = TowerId;
                    MyTransactionLog.Id = TransactionId;
                    MyTransactionLog.IsComplate = false;
                    db.TransactionLogs.Add(MyTransactionLog);
                    db.SaveChanges();



                    bool res = calculateFees(towerData, MyTransactionLog, TowerId);
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


        public ActionResult CreateOrUpdateTakeOffU(TowerDataDtoU model)
        {
            ///get companyInfoId
            var id = User.Identity.GetUserId();
            int? companyInfoId = db.WorkOns.Where(x => x.UserId == id).FirstOrDefault().CompanyInfoId;
            if (!companyInfoId.HasValue || companyInfoId <= 0) { return Json(new { success = true, message = "CompanyInfoIdNotFound" }, JsonRequestBehavior.AllowGet); }




            TowerData towerData = new TowerData();
            // تعيين القيم الأساسية
            towerData.Id = model.IdU;
            towerData.AirLineId = model.AirLineIdU;
            towerData.FlightNo = model.FlightNoU;
            towerData.AircraftRegId = model.AircraftRegIdU;
            towerData.AirportIdFrom = model.AirportIdFromU;
            towerData.AirportIdTo = model.AirportIdToU;
            towerData.FlightTypeD = model.FlightTypeDU;
            towerData.FlightTypeL = model.FlightTypeLU;
            towerData.Status = model.StatusU;
            towerData.TripTypeId = model.TripTypeIdU;
            towerData.CompanyInfoId = model.CompanyInfoIdU;
            towerData.Note = model.NoteU;

            // تحويل التاريخ من string إلى DateTime
            towerData.LandingDate = CommonUsed.ConvertToDbDate(model.LandingDateU);
            towerData.Date = CommonUsed.ConvertToDbDate(model.DateU);
            towerData.TakeOffDate = CommonUsed.ConvertToDbDate(model.TakeOffDateU);

            // TimeSpan
            towerData.ATA = model.ATAU;
            towerData.ATD = model.ATDU;

            // خصائص الإنشاء والتعديل
            towerData.CreatedBy = model.CreatedByU;
            towerData.CreationDate = model.CreationDateU;
            towerData.UpdatedBy = model.UpdatedByU;
            towerData.UpdatingDate = model.UpdatingDateU;


            if (ModelState.IsValid)
            {
                
                // اختبر التاريخ بين في المطالبات المالية
                //bool isHasClaim = CommonUsed.IsEntryDateinRange1(towerData.Id, Role.GetRole(id));
                //if (isHasClaim)
                //{
                //    return Json(new { success = true, message = "isStop" }, JsonRequestBehavior.AllowGet);
                //}

                int TransactionId = Convert.ToInt32(db.SpLastTransactionid().FirstOrDefault());

                TransactionLog MyTransactionLog = new TransactionLog();

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




                    bool res = calculateFeesUpdate(towerData);
                    bool resNyAndCupps = ChangeNyAndCupps(towerData);


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


                    MyTransactionLog.TowerDataId = TowerId;
                    MyTransactionLog.Id = TransactionId;
                    MyTransactionLog.IsComplate = false;
                    db.TransactionLogs.Add(MyTransactionLog);
                    db.SaveChanges();



                    bool res = calculateFees(towerData, MyTransactionLog, TowerId);
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










        public ActionResult GetAircraftTakeOffParialM(int? id)
        {
            TowerDataDto model = new TowerDataDto();

            if (id.HasValue && id > 0)
            {
                // جلب Entity من قاعدة البيانات
                TowerData towerData = db.TowerDatas.Find(id);

                if (towerData != null)
                {
                    // تحويل Entity إلى DTO
                    model.Id = towerData.Id;
                    model.AirLineId = towerData.AirLineId;
                    model.FlightNo = towerData.FlightNo;
                    model.AircraftRegId = towerData.AircraftRegId;
                    model.AirportIdFrom = towerData.AirportIdFrom;
                    model.AirportIdTo = towerData.AirportIdTo;
                    model.FlightTypeD = towerData.FlightTypeD;
                    model.FlightTypeL = towerData.FlightTypeL;
                    model.Status = towerData.Status;
                    model.TripTypeId = towerData.TripTypeId;
                    model.CompanyInfoId = towerData.CompanyInfoId;
                    model.Note = towerData.Note;

                    // تحويل التاريخ إلى string بصيغة dd/MM/yyyy
                    model.Date = CommonUsed.ToDisplayDate(towerData.Date);

                    // تحويل LandingDate و TakeOffDate إلى string
                    model.LandingDate = towerData.LandingDate.HasValue ? towerData.LandingDate.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : null;
                    model.TakeOffDate = towerData.TakeOffDate.HasValue ? towerData.TakeOffDate.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : null;

                    // TimeSpan مباشرة
                    model.ATA = towerData.ATA;
                    model.ATD = towerData.ATD;

                    // Created/Updated info
                    model.CreatedBy = towerData.CreatedBy;
                    model.CreationDate = towerData.CreationDate;
                    model.UpdatedBy = towerData.UpdatedBy;
                    model.UpdatingDate = towerData.UpdatingDate;
                }
            }

            // تجهيز SelectLists
            ViewBag.AircraftRegId = new SelectList(db.AircraftRegistrations, "Id", "Registration", model.AircraftRegId);
            ViewBag.AirLineId = new SelectList(db.AirLines, "Id", "Code", model.AirLineId);
            ViewBag.FlightNo = new SelectList(db.AirLinesFlightTrips, "Id", "FlightNo", model.FlightNo);
            ViewBag.AirportIdTo = new SelectList(db.AirPorts, "Id", "Code", model.AirportIdTo);
            ViewBag.AirportIdFrom = new SelectList(db.AirPorts, "Id", "Code", model.AirportIdFrom);
            ViewBag.CompanyInfoId = new SelectList(db.CompanyInfoes, "Id", "NameAr", model.CompanyInfoId);
            ViewBag.TripTypeId = new SelectList(db.TripTypes, "Id", "Name", model.TripTypeId);

            return PartialView("_AddEditTowerDataTakeOffM", model);
        }









        /// ///////////////////////////////////////////////////End Take Off///////////////////////////////////////////////////////////////
        /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// ////////////AAAAAAAAAAAAAAAAAAAAAAA//////Start Arrived//////////AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA/////////////////////////////////////////////////////
        /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ActionResult Arrived()
        {
            return View();
        }

        public ActionResult GetTowerDataArrivedParial(int? id)
        {
            TowerData towerData = new TowerData();

            if (id > 0)
            {
                towerData = db.TowerDatas.Find(id);
            }

            ViewBag.AircraftRegId = new SelectList(db.AircraftRegistrations, "Id", "Registration", towerData.AircraftRegId);
            ViewBag.AirLineId = new SelectList(db.AirLines, "Id", "Code", towerData.AirLineId);
            ViewBag.FlightNo = new SelectList(db.AirLinesFlightTrips, "Id", "FlightNo", towerData.FlightNo);
            ViewBag.AirportIdTo = new SelectList(db.AirPorts, "Id", "Code", towerData.AirportIdTo);
            ViewBag.AirportIdFrom = new SelectList(db.AirPorts, "Id", "Code", towerData.AirportIdFrom);
            ViewBag.CompanyInfoId = new SelectList(db.CompanyInfoes, "Id", "NameAr", towerData.CompanyInfoId);
            ViewBag.TripTypeId = new SelectList(db.TripTypes, "Id", "Name", towerData.TripTypeId);

            return PartialView("_AddEditTowerDataArrived", towerData);

        }

        public ActionResult GetAllAircraftNotArrived()
        {

            IQueryable<TowerData> TowerDatas = db.TowerDatas.Where(x => x.ATA == null && x.LandingDate == null);


            var TowerDatasList = new List<TowerDataViewModels>();
            foreach (var item in TowerDatas)
            {
                TowerDatasList.Add(new TowerDataViewModels
                {
                    Id = item.Id,
                    AirLineCode = item?.AirLine.Code,
                    FlightNo = item?.AirLinesFlightTrip.FlightNo,
                    AircraftRegName = item?.AircraftRegistration.Registration,
                    Type = item?.AircraftRegistration.AircraftType.Type,
                    AirportIdFrom = CommonUsed.CheckAirportCode(Convert.ToInt32(item?.AirportIdFrom)),
                    AirportIdTo = CommonUsed.CheckAirportCode(Convert.ToInt32(item?.AirportIdTo)),
                    Date = Convert.ToDateTime(item.Date).ToString("MM/dd/yyyy"),
                    ATA = item.ATA.ToString(),
                    ATD = item.ATD.ToString(),
                    //POB = item.POB,
                    //QBD = item.QBD,
                    LandingPermission = item.LandingPermission,

                    CreatedBy = CommonUsed.GetUserName(item.CreatedBy),
                    CreationDate = item.CreationDate.ToString(),
                    UpdatedBy = CommonUsed.GetUserName(item.UpdatedBy),
                    UpdatingDate = item.UpdatingDate.ToString(),

                    LandingDate = CommonUsed.ToDisplayDate(item.LandingDate),
                    TakeOffDate = CommonUsed.ToDisplayDate(item.TakeOffDate),
                });
            }

            return Json(new { data = TowerDatasList.ToList() }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateOrUpdateArrived(TowerData model)
        {
            if (ModelState.IsValid)
            {
                var id = User.Identity.GetUserId();

                if (model.Id > 0)
                {

                    if ((model.TakeOffDate == null && model.ATD != null) || (model.TakeOffDate != null && model.ATD == null))
                    {
                        return Json(new { success = true, message = "Repeat" }, JsonRequestBehavior.AllowGet);
                    }

                    if (model.TakeOffDate != null)
                    {
                        if (Convert.ToDateTime(model.LandingDate).Date > Convert.ToDateTime(model.TakeOffDate).Date)
                        {
                            return Json(new { success = true, message = "InvalidDate" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    if (model.TakeOffDate == null && model.ATD == null)
                    {
                        model.Status = 1;
                    }

                    model.UpdatedBy = id;
                    model.UpdatingDate = System.DateTime.Now;

                    /////////////////////////////******* Start Check FlightType*******/////////////////////////////////////////
                    AirPort Afrom = db.AirPorts.Single(x => x.Id == model.AirportIdFrom);

                    if (Afrom.CountryId == 1)
                    {
                        // Domistic
                        model.FlightTypeL = 2;
                    }
                    else
                    {    // InterNational
                        model.FlightTypeL = 1;
                    }

                    ////////******************* End Check FlightType/////*****************************//
                    //******************************Start Check Status*****************************************
                    AirPort ATo = db.AirPorts.Single(x => x.Id == model.AirportIdTo);
                    if (ATo.CountryId == 1)
                    {
                        // Domistic
                        model.FlightTypeD = 2;
                    }
                    else
                    {    // InterNational
                        model.FlightTypeD = 1;
                    }
                    //********************************End Check Status***********************************




                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Update" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    model.Date = System.DateTime.Now;
                    model.CreatedBy = id;
                    model.CreationDate = System.DateTime.Now;

                    /////////////////////////////Check if Ata Null Takeoff Null/////////////////////////////////////////
                    if (model.ATA != null)
                    {
                        model.LandingDate = System.DateTime.Now;
                    }
                    else
                    {
                        model.LandingDate = null;
                    }
                    if (model.ATD != null)
                    {
                        model.TakeOffDate = System.DateTime.Now;
                    }
                    else
                    {
                        model.TakeOffDate = null;
                    }
                    /////////////////////////////Check if Ata Null Takeoff Null/////////////////////////////////////////

                    /////////////////////////////******* Start Check FlightType*******/////////////////////////////////////////
                    AirPort Afrom = db.AirPorts.Single(x => x.Id == model.AirportIdFrom);
                    AirPort ATo = db.AirPorts.Single(x => x.Id == model.AirportIdTo);
                    if (Afrom.CountryId == 1 && ATo.CountryId == 1)
                    {
                        // Domistic
                        model.FlightTypeL = 2;
                    }
                    else
                    {    // InterNational
                        model.FlightTypeL = 1;
                    }

                    ////////******************* End Check FlightType/////*****************************//

                    //******************************Start Check Status*****************************************

                    model.Status = 3;


                    //********************************End Check Status***********************************



                    db.TowerDatas.Add(model);
                    db.SaveChanges();
                    return Json(new { success = true, message = "Add" }, JsonRequestBehavior.AllowGet);
                }

            }
            return Json(new { success = false, message = "error" }, JsonRequestBehavior.AllowGet);
        }
        /// ///////////////////////////////////////////////////End Arrived///////////////////////////////////////////////////////////////
        /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////       

        private bool calculateFees(TowerData model, TransactionLog TransactionLog, int TowerId)
        {

            bool res = true;
            // Create New OFFicer Data
            //  TowerData td = db.TowerDatas.ToList().OrderBy(x => x.Id).Last();

            TowerData td = db.TowerDatas.Find(TowerId);
            OfficerData off = new OfficerData();
            off.TowerDataId = td.Id;
            off.IsCurrentAccount = true;

            //--------------------------------
            //  int OfficerId = (db.OfficerDatas.ToList().OrderBy(x => x.Id).Last().Id) + 1;
            int OfficerId = Convert.ToInt32(db.SpLastOfficerid().FirstOrDefault());
            off.Id = OfficerId;
            TransactionLog.OfficerDataId = OfficerId;
            //---------------------------------------------

            db.OfficerDatas.Add(off);
            db.Entry(TransactionLog).State = EntityState.Modified;
            db.SaveChanges();
            // End Create New OFFicer Data

            OfficerData off1 = db.OfficerDatas.FirstOrDefault(x => x.TowerDataId == td.Id);

            decimal wt = Convert.ToDecimal(db.AircraftRegistrations.Single(x => x.Id == model.AircraftRegId).MaxTakoffWieght);
            decimal val = cal.LandingFees(wt, Convert.ToInt32(model.FlightTypeL));

            if (db.Revenues.Any(x => x.OfficerDataId == off1.Id))
            {
                //Update
                Revenue rev = db.Revenues.FirstOrDefault(x => x.OfficerDataId == off1.Id);
                rev.LandingFees = val;
                rev.NightOperationFees = cal.NightOperationFees(model.ATA.Value.Hours, model.ATD.Value.Hours, val);
                rev.OpenAireportOvertimeFees = cal.OpenAireportOvertimeFees(model.ATA.Value.Hours, val);
                rev.NavigationFees = cal.NavigationFees(val);
                //rev.ParkingFees = cal.ParkingFees(off1.Id);
                var AirLine = db.AirLines.FirstOrDefault(x => x.Id == model.AirLineId);
                rev.ParkingFees = cal.ParkingFees(off1.Id, Convert.ToDateTime(model.LandingDate), Convert.ToDateTime(model.TakeOffDate), model.ATA.Value, model.ATD.Value, Convert.ToBoolean(AirLine.IsLocalCompany));
                rev.NoiseFees = cal.NoiseFees(off1.Id);
                rev.SecurityFees = cal.SecurityFees(model);
                if (ModelState.IsValid)
                {
                    db.Entry(rev).State = EntityState.Modified;
                    db.SaveChanges();
                    res = true;
                }
            }
            else
            {
                int RevenuesId = Convert.ToInt32(db.SpLastRevenceid().FirstOrDefault());
                //Insert new one
                Revenue rev = new Revenue();
                rev.Id = RevenuesId;


                rev.OfficerDataId = off1.Id;
                rev.LandingFees = val;
                rev.NightOperationFees = cal.NightOperationFees(model.ATA.Value.Hours, model.ATD.Value.Hours, val);
                rev.OpenAireportOvertimeFees = cal.OpenAireportOvertimeFees(model.ATA.Value.Hours, val);
                rev.NavigationFees = cal.NavigationFees(val);
                //rev.ParkingFees = cal.ParkingFees(off1.Id);
                var AirLine = db.AirLines.FirstOrDefault(x => x.Id == model.AirLineId);
                rev.ParkingFees = cal.ParkingFees(off1.Id, Convert.ToDateTime(model.LandingDate), Convert.ToDateTime(model.TakeOffDate), model.ATA.Value, model.ATD.Value, Convert.ToBoolean(AirLine.IsLocalCompany));
                rev.NoiseFees = cal.NoiseFees(wt);

                rev.SecurityFees = cal.SecurityFees(wt, Convert.ToInt32(model.FlightTypeL), Convert.ToBoolean(AirLine.IsLocalCompany));

                //--------------------------------


                TransactionLog.RevenueDataId = RevenuesId;
                TransactionLog.IsComplate = true;
                db.Entry(TransactionLog).State = System.Data.Entity.EntityState.Modified;
                //---------------------------------------------

                db.Revenues.Add(rev);

                db.SaveChanges();

                res = true;
            }

            return res;
        }

        private bool calculateFeesUpdate(TowerData model)
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
                rev.ParkingFees = cal.ParkingFees(OfficerData.Id, Convert.ToDateTime(model.LandingDate), Convert.ToDateTime(model.TakeOffDate), model.ATA.Value, model.ATD.Value, Convert.ToBoolean(AirLine.IsLocalCompany));
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

        public ActionResult AircraftNotTakeOff()
        {
            return View();
        }

        [Authorize(Roles = "Landing-Admin,Revenues,Super-Admin, Account, Account-Admin")]
        public ActionResult TowerDataReport()
        {
            return View();
        }

        public ActionResult GetData(int? page, int? Get, int? Search, int? Count, int? AirLineId
            , int? FlightNo, int? AirPortIdFrom, int? AirPortIdTo, DateTime? DateFrom, DateTime? DateTo
           , int? RegId, int? FlightTypeId)
        {
            IQueryable<TowerData> TowerData;
            TowerData = db.TowerDatas.OrderBy(x => x.Id);

            ViewBag.AirLineId = AirLineId;
            ViewBag.FlightNo = FlightNo;
            ViewBag.AirPortIdFrom = AirPortIdFrom;
            ViewBag.AirPortIdTo = AirPortIdTo;

            ViewBag.RegId = RegId;

            ViewBag.DateFrom = DateFrom;
            ViewBag.DateTo = DateTo;

            ViewBag.FlightTypeId = FlightTypeId;



            if (Get > 0)
            {

                if (FlightTypeId > 0)
                {
                    if (FlightTypeId == 1) //International
                        TowerData = TowerData.Where(x => x.FlightTypeD == FlightTypeId || x.FlightTypeL == FlightTypeId);
                    else // Domestic
                        TowerData = TowerData.Where(x => x.FlightTypeD == FlightTypeId && x.FlightTypeL == FlightTypeId);
                }



                if (AirPortIdFrom > 0)
                {
                    TowerData = TowerData.Where(x => x.AirportIdFrom == AirPortIdFrom);
                }

                if (RegId > 0)
                {
                    TowerData = TowerData.Where(x => x.AircraftRegId == RegId);
                }

                if (AirPortIdTo > 0)
                {
                    TowerData = TowerData.Where(x => x.AirportIdTo == AirPortIdTo);
                }

                //if (!string.IsNullOrEmpty(Name))
                //{

                //    ExecuteCourses = ExecuteCourses.Where(x => x.Course.Name.Contains(Name)).OrderBy(x => x.Id);
                //}
                if (AirLineId > 0)
                {
                    TowerData = TowerData.Where(x => x.AirLineId == AirLineId);
                }
                if (FlightNo > 0)
                {
                    TowerData = TowerData.Where(x => x.AirLinesFlightTrip.Id == FlightNo);
                }
                //if (IsInPlan > 0)
                //{
                //    bool t;
                //    if (IsInPlan == 1) t = true; else t = false;
                //    ExecuteCourses = ExecuteCourses.Where(x => x.Course.IsInPlan == t);
                //}
                if (DateFrom != null && DateTo != null)
                {

                    TowerData = TowerData.Where(t => DbFunctions.TruncateTime(t.Date) >= DbFunctions.TruncateTime(DateFrom) && DbFunctions.TruncateTime(t.Date) <= DbFunctions.TruncateTime(DateTo));
                }


                //////////////////////////////////////////////////////
                if (Search == 1)
                {



                    if (TowerData != null)
                    {

                        int ItemCount = 0;
                        //List<decimal> Amountlist = new List<decimal>();
                        //List<decimal> Allowancelist = new List<decimal>();


                        ItemCount = TowerData.Count();
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
                TowerData = null;
                ViewBag.ItemCount = 0;
            }

            int pageSize = 20;
            int PageNumber = (page ?? 1);




            return PartialView("_GetData", TowerData.ToPagedList(PageNumber, pageSize));
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OfficerData OfficerData = db.OfficerDatas.Where(x => x.TowerDataId == id).FirstOrDefault();
            if (OfficerData == null)
            {
                return HttpNotFound();
            }


            var item = db.TowerDatas.Single(x => x.Id == id);

            TowerDataViewModels tdm = new TowerDataViewModels();

            tdm.Id = item.Id;
            tdm.AirLineCode = item?.AirLine.Code;
            tdm.AirLineName = item?.AirLine.Name;
            tdm.FlightNo = item?.AirLinesFlightTrip.FlightNo;
            tdm.AircraftRegName = item?.AircraftRegistration.Registration;
            tdm.Type = item?.AircraftRegistration.AircraftType.Type;

            tdm.AirportIdFrom = CommonUsed.CheckAirportCode(Convert.ToInt32(item?.AirportIdFrom));
            tdm.AirportIdTo = CommonUsed.CheckAirportCode(Convert.ToInt32(item?.AirportIdTo));
            tdm.Date = Convert.ToDateTime(item.Date).ToString("MM/dd/yyyy");
            tdm.ATA = item.ATA.ToString();
            tdm.ATD = item.ATD.ToString();
            //tdm.POB = item.POB;
            //tdm.QBD = item.QBD;
            tdm.LandingPermission = item.LandingPermission;
            tdm.MaxTaKeOffWeight = item.AircraftRegistration.MaxTakoffWieght.ToString();
            tdm.CountryFrom = CommonUsed.CheckCountryCode(Convert.ToInt32(item.AirportIdFrom));
            tdm.CountryTo = CommonUsed.CheckCountryCode(Convert.ToInt32(item.AirportIdTo));

            tdm.CreatedBy = CommonUsed.GetUserName(item.CreatedBy);
            tdm.CreationDate = item.CreationDate.ToString();
            tdm.UpdatedBy = CommonUsed.GetUserName(item.UpdatedBy);
            tdm.UpdatingDate = item.UpdatingDate.ToString();

            tdm.LandingDate = CommonUsed.ToDisplayDate(item.LandingDate);
            tdm.TakeOffDate = CommonUsed.ToDisplayDate(item.TakeOffDate);
            tdm.FlightType = CommonUsed.GetFlightTypeName(item.FlightTypeL, item.FlightTypeD);

            tdm.AirportFromName = item.AirPort1.NameEn;
            tdm.AirportToName = item.AirPort.NameEn;
            tdm.AirportIdTo = CommonUsed.CheckAirportCode(Convert.ToInt32(item?.AirportIdTo));

            Revenue Revenue = db.Revenues.Where(x => x.OfficerDataId == OfficerData.Id).FirstOrDefault();
            if (Revenue != null)
            {

                tdm.LandingFees = Revenue.LandingFees.ToString();
                tdm.ParkingFees = Revenue.ParkingFees.ToString();
                tdm.NightOperationFees = Revenue.NightOperationFees.ToString();

                tdm.NavigationFees = Revenue.NavigationFees.ToString();
                tdm.NoiseFees = Revenue.NoiseFees.ToString();
                tdm.SecurityFees = Revenue.SecurityFees.ToString();

            }
















            return View(tdm);
        }

        public ActionResult GetFlightNo(int id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<AirLinesFlightTrip> lstAirLinesFlightTrip = new List<AirLinesFlightTrip>();
            lstAirLinesFlightTrip = db.AirLinesFlightTrips.Where(x => x.AirLineId == id).ToList();

            lstAirLinesFlightTrip.Insert(0, new AirLinesFlightTrip() { FlightNo = "--Select FlightNo--", Id = 0 });
            return Json(lstAirLinesFlightTrip, JsonRequestBehavior.AllowGet);
        }

     

        /// <summary>
        /// //////////////////////////////////////////////////////////////////////////////////////////////
        /// 
        /// </summary>
        /// <returns></returns>

        public ActionResult GetDaily(int? page, int? Get, int? Search, int? Count
    , DateTime? DateDaily)
        {
            IQueryable<TowerData> TowerData;
            TowerData = db.TowerDatas.OrderBy(x => x.Id);




            ViewBag.DateDaily = DateDaily;



            if (Get > 0)
            {



                if (DateDaily != null)
                {

                    TowerData = TowerData.Where(t => DbFunctions.TruncateTime(t.Date) == DbFunctions.TruncateTime(DateDaily));
                }
                //////////////////////////////////////////////////////////////////////////////////////////////////////////


                //////////////////////////////////////////////////////////////////////////////////////////////////
                if (Search == 1)
                {

                    if (TowerData != null)
                    {

                        int ItemCount = 0;
                        //List<decimal> Amountlist = new List<decimal>();
                        //List<decimal> Allowancelist = new List<decimal>();


                        ItemCount = TowerData.Count();
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
                TowerData = null;
                ViewBag.ItemCount = 0;
            }

            int pageSize = 20;
            int PageNumber = (page ?? 1);




            return PartialView("_GetDaily", TowerData.ToPagedList(PageNumber, pageSize));
        }


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

        public ActionResult Create()
        {
            ViewBag.AircraftRegId = new SelectList(db.AircraftRegistrations, "Id", "Registration");
            ViewBag.AirLineId = new SelectList(db.AirLines, "Id", "Name");
            ViewBag.FlightNo = new SelectList(db.AirLinesFlightTrips, "Id", "FlightNo");
            ViewBag.AirportIdTo = new SelectList(db.AirPorts, "Id", "NameAr");
            ViewBag.AirportIdFrom = new SelectList(db.AirPorts, "Id", "NameAr");
            ViewBag.CompanyInfoId = new SelectList(db.CompanyInfoes, "Id", "NameAr");
            return View();
        }

        // POST: TowerDatas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AirLineId,FlightNo,AircraftRegId,AirportIdFrom,AirportIdTo,Date,ATA,ATD,POB,QBD,TripType,LandingPermission,CompanyInfoId,CreatedBy,CreationDate,UpdatedBy,UpdatingDate")] TowerData towerData)
        {
            if (ModelState.IsValid)
            {
                db.TowerDatas.Add(towerData);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AircraftRegId = new SelectList(db.AircraftRegistrations, "Id", "Registration", towerData.AircraftRegId);
            ViewBag.AirLineId = new SelectList(db.AirLines, "Id", "Name", towerData.AirLineId);
            ViewBag.FlightNo = new SelectList(db.AirLinesFlightTrips, "Id", "FlightNo", towerData.FlightNo);
            ViewBag.AirportIdTo = new SelectList(db.AirPorts, "Id", "NameAr", towerData.AirportIdTo);
            ViewBag.AirportIdFrom = new SelectList(db.AirPorts, "Id", "NameAr", towerData.AirportIdFrom);
            ViewBag.CompanyInfoId = new SelectList(db.CompanyInfoes, "Id", "NameAr", towerData.CompanyInfoId);
            return View(towerData);
        }

        // GET: TowerDatas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TowerData towerData = db.TowerDatas.Find(id);
            if (towerData == null)
            {
                return HttpNotFound();
            }
            ViewBag.AircraftRegId = new SelectList(db.AircraftRegistrations, "Id", "Registration", towerData.AircraftRegId);
            ViewBag.AirLineId = new SelectList(db.AirLines, "Id", "Name", towerData.AirLineId);
            ViewBag.FlightNo = new SelectList(db.AirLinesFlightTrips, "Id", "FlightNo", towerData.FlightNo);
            ViewBag.AirportIdTo = new SelectList(db.AirPorts, "Id", "NameAr", towerData.AirportIdTo);
            ViewBag.AirportIdFrom = new SelectList(db.AirPorts, "Id", "NameAr", towerData.AirportIdFrom);
            ViewBag.CompanyInfoId = new SelectList(db.CompanyInfoes, "Id", "NameAr", towerData.CompanyInfoId);
            return View(towerData);
        }

        // POST: TowerDatas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AirLineId,FlightNo,AircraftRegId,AirportIdFrom,AirportIdTo,Date,ATA,ATD,POB,QBD,TripType,LandingPermission,CompanyInfoId,CreatedBy,CreationDate,UpdatedBy,UpdatingDate")] TowerData towerData)
        {
            if (ModelState.IsValid)
            {
                db.Entry(towerData).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AircraftRegId = new SelectList(db.AircraftRegistrations, "Id", "Registration", towerData.AircraftRegId);
            ViewBag.AirLineId = new SelectList(db.AirLines, "Id", "Name", towerData.AirLineId);
            ViewBag.FlightNo = new SelectList(db.AirLinesFlightTrips, "Id", "FlightNo", towerData.FlightNo);
            ViewBag.AirportIdTo = new SelectList(db.AirPorts, "Id", "NameAr", towerData.AirportIdTo);
            ViewBag.AirportIdFrom = new SelectList(db.AirPorts, "Id", "NameAr", towerData.AirportIdFrom);
            ViewBag.CompanyInfoId = new SelectList(db.CompanyInfoes, "Id", "NameAr", towerData.CompanyInfoId);
            return View(towerData);
        }

        // GET: TowerDatas/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    TowerData towerData = db.TowerDatas.Find(id);
        //    if (towerData == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(towerData);
        //}

        // POST: TowerDatas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TowerData towerData = db.TowerDatas.Find(id);
            db.TowerDatas.Remove(towerData);
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


        public ActionResult GetAircraftTakeOffParialEx(int? id)
        {
            Revenue Revenue = db.Revenues.Where(x => x.Id == id).FirstOrDefault();
            OfficerData OfficerData = db.OfficerDatas.Where(x => x.Id == Revenue.OfficerDataId).FirstOrDefault();


            TowerData towerData = new TowerData();

            if (id > 0)
            {
                towerData = db.TowerDatas.Where(x => x.Id == OfficerData.TowerDataId).FirstOrDefault();
            }

            ViewBag.AircraftRegId = new SelectList(db.AircraftRegistrations, "Id", "Registration", towerData.AircraftRegId);
            ViewBag.AirLineId = new SelectList(db.AirLines, "Id", "Code", towerData.AirLineId);
            ViewBag.FlightNo = new SelectList(db.AirLinesFlightTrips, "Id", "FlightNo", towerData.FlightNo);
            ViewBag.AirportIdTo = new SelectList(db.AirPorts, "Id", "Code", towerData.AirportIdTo);
            ViewBag.AirportIdFrom = new SelectList(db.AirPorts, "Id", "Code", towerData.AirportIdFrom);
            ViewBag.CompanyInfoId = new SelectList(db.CompanyInfoes, "Id", "NameAr", towerData.CompanyInfoId);
            ViewBag.TripTypeId = new SelectList(db.TripTypes, "Id", "Name", towerData.TripTypeId);

            //return PartialView("_AddEditTowerDataAll", towerData);

            return PartialView("_AddEditTowerDataTakeOff", towerData);
        }

        public ActionResult CreateOrUpdateTakeOffEx(TowerData model)
        {
            Revenue Revenue = db.Revenues.Where(x => x.Id == model.Id).FirstOrDefault();
            OfficerData OfficerData = db.OfficerDatas.Where(x => x.Id == Revenue.OfficerDataId).FirstOrDefault();

            /// TowerData TowerData = db.TowerDatas.Where(x=>x.)

            if (ModelState.IsValid)
            {
                var id = User.Identity.GetUserId();
                if (OfficerData.TowerDataId > 0)
                {
                    model.UpdatedBy = id;
                    model.UpdatingDate = System.DateTime.Now;
                    if (db.TowerDatas.Any(x => x.AirLineId == model.AirLineId && x.ATA == model.ATA && x.ATD == model.ATD && x.Date == model.Date && x.Id != OfficerData.TowerDataId && x.FlightNo == model.FlightNo))
                    {
                        return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
                    }

                    /////////////////////////////******* Start Check FlightType****//////////////////////////////////////
                    //AirPort Afrom = db.AirPorts.Single(x => x.Id == model.AirportIdFrom);
                    AirPort ATo = db.AirPorts.Single(x => x.Id == model.AirportIdTo);
                    if (ATo.CountryId == 1)
                    {
                        // Domistic
                        model.FlightTypeD = 2;
                    }
                    else
                    {    // InterNational
                        model.FlightTypeD = 1;
                    }

                    AirPort AFrom = db.AirPorts.Single(x => x.Id == model.AirportIdFrom);
                    if (AFrom.CountryId == 1)
                    {
                        // Domistic
                        model.FlightTypeL = 2;
                    }
                    else
                    {    // InterNational
                        model.FlightTypeL = 1;
                    }



                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    //   cal.LandingFees(model.Id);
                    db.SaveChanges();


                    bool res = calculateFeesUpdate(model);

                    if (!res)
                    {
                        return Json(new { success = true, message = "CalculationFeesError" }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { success = true, message = "Update" }, JsonRequestBehavior.AllowGet);
                }
                else
                {


                    if (db.TowerDatas.Any(x => x.AirLineId == model.AirLineId && x.ATA == model.ATA && x.ATD == model.ATD && x.Date == model.Date && x.FlightNo == model.FlightNo))

                    {
                        //  return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);
                        return Json(new { success = true, message = "Used" }, JsonRequestBehavior.AllowGet);

                    }

                    model.CreatedBy = id;
                    model.CreationDate = System.DateTime.Now;

                    /////////////////////////////******* Start Check FlightType******////////////////////////////////////////
                    //AirPort Afrom = db.AirPorts.Single(x => x.Id == model.AirportIdFrom);
                    AirPort ATo = db.AirPorts.Single(x => x.Id == model.AirportIdTo);
                    if (ATo.CountryId == 1)
                    {
                        // Domistic
                        model.FlightTypeD = 2;
                    }
                    else
                    {    // InterNational
                        model.FlightTypeD = 1;
                    }

                    AirPort AFrom = db.AirPorts.Single(x => x.Id == model.AirportIdFrom);
                    if (AFrom.CountryId == 1)
                    {
                        // Domistic
                        model.FlightTypeL = 2;
                    }
                    else
                    {    // InterNational
                        model.FlightTypeL = 1;
                    }

                    //****************************Start Check Status*****************************************

                    model.Status = 3;


                    // ******************************End Check Status***********************************



                    db.TowerDatas.Add(model);
                    db.SaveChanges();

                    //   bool res = calculateFees(model);

                    //if (!res)
                    //{
                    //    return Json(new { success = true, message = "CalculationFeesError" }, JsonRequestBehavior.AllowGet);
                    //}
                    return Json(new { success = true, message = "Add" }, JsonRequestBehavior.AllowGet);
                }

            }
            return Json(new { success = false, message = "error" }, JsonRequestBehavior.AllowGet);
        }

    }
}