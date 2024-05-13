using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AirPortERP.Extenstion;
using AirPortERP.Models;
using AirPortERP.ViewModels;
using System.Data.Entity;
using PagedList;
using Microsoft.AspNet.Identity;
namespace AirPortERP.Controllers
{
    public class RevenueExtenstionController : Controller
    {
        Calculation Cal = new Calculation();
        Entities db = new Entities();
        CommonUsed CommonUsed = new CommonUsed();
        // //////////////////////////////////Start Ny////////////////////////////////////////////////////////////



        [Authorize(Roles = "Cupps,Ny,Revenues,Super-Admin,Cupps&&Car")]
        public ActionResult NyAndCuppsFillter()
        {
            return View();
        }

        public ActionResult GetDataNyAndCupps(int? page, int? Get, int? Search, int? Count, int? AirLineId
        , DateTime? DateFrom, DateTime? DateTo
       , int? FlightTypeId)
        {


            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.OrderBy(x => x.Id).Where(x => x.DomeNYFees > 0 || x.InterNYFees > 0);

            ViewBag.AirLineId = AirLineId;


            ViewBag.DateFrom = DateFrom;
            ViewBag.DateTo = DateTo;

            ViewBag.FlightTypeId = FlightTypeId;

            if (Get > 0)
            {

                if (FlightTypeId > 0)
                {
                    if (FlightTypeId == 2)
                    {
                        Revenue = Revenue.Where(x => x.OfficerData.TowerData.FlightTypeD == 2 && x.OfficerData.TowerData.FlightTypeL == 2);
                    }
                    else
                    {
                        Revenue = Revenue.Where(x => x.OfficerData.TowerData.FlightTypeD == 1 || x.OfficerData.TowerData.FlightTypeL == 1);
                    }

                }

                if (AirLineId > 0)
                {
                    Revenue = Revenue.Where(x => x.OfficerData.TowerData.AirLineId == AirLineId);
                }

                if (DateFrom != null && DateTo != null)
                {

                    Revenue = Revenue.Where(t => DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) >= DbFunctions.TruncateTime(DateFrom) && DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) <= DbFunctions.TruncateTime(DateTo));
                }


                //////////////////////////////////////////////////////
                if (Search == 1)
                {

                    if (Revenue != null)
                    {

                        int ItemCount = 0;
                        //List<decimal> Amountlist = new List<decimal>();
                        //List<decimal> Allowancelist = new List<decimal>();


                        ItemCount = Revenue.Count();
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
                Revenue = null;
                ViewBag.ItemCount = 0;
            }

            int pageSize = 5;
            int PageNumber = (page ?? 1);




            return PartialView("_GetNyAndCuppsFillter", Revenue.ToPagedList(PageNumber, pageSize));
        }

        public ActionResult GetNyAndCuppsfeesParial(int? id)
        {
            Revenue Revenue = new Revenue();
            NyAndCuppsFeesViewModelDTO NyAndCuppsFeesViewModelDTO = new NyAndCuppsFeesViewModelDTO();
            if (id > 0)
            {
                Revenue = db.Revenues.Find(id);
            }
            if (Revenue != null)
            {
                if (Revenue.InterNYFees != null || Revenue.DomeNYFees != null)
                {
                    OfficerData OfficerData = db.OfficerDatas.Where(x => x.Id == Revenue.OfficerDataId).FirstOrDefault();


                    NyAndCuppsFeesViewModelDTO.IsNotUsedCUPPS = Convert.ToBoolean(OfficerData.IsNotUsedCUPPS);
                    NyAndCuppsFeesViewModelDTO.Pax = Convert.ToInt32(OfficerData.Pax);
                    //  NyAndCuppsFeesViewModelDTO.DisEmbInft = Convert.ToInt32(OfficerData.DisEmbInft);
                    //  NyAndCuppsFeesViewModelDTO.TRNY = Convert.ToInt32(OfficerData.TRNY);
                    NyAndCuppsFeesViewModelDTO.TRNYCUPPS = Convert.ToInt32(OfficerData.TRNYCUPPS);
                    NyAndCuppsFeesViewModelDTO.Id = OfficerData.Id;
                    NyAndCuppsFeesViewModelDTO.OfficerDataId = OfficerData.Id;

                }
            }
            //ViewBag.AircraftRegId = new SelectList(db.AircraftRegistrations, "Id", "Registration", towerData.AircraftRegId);
            //ViewBag.AirLineId = new SelectList(db.AirLines, "Id", "Code", towerData.AirLineId);
            //ViewBag.FlightNo = new SelectList(db.AirLinesFlightTrips, "Id", "FlightNo", towerData.FlightNo);
            //ViewBag.AirportIdTo = new SelectList(db.AirPorts, "Id", "Code", towerData.AirportIdTo);
            //ViewBag.AirportIdFrom = new SelectList(db.AirPorts, "Id", "Code", towerData.AirportIdFrom);
            //ViewBag.CompanyInfoId = new SelectList(db.CompanyInfoes, "Id", "NameAr", towerData.CompanyInfoId);
            //ViewBag.TripTypeId = new SelectList(db.TripTypes, "Id", "Name", towerData.TripTypeId);

            //return PartialView("_AddEditTowerDataAll", towerData);

            return PartialView("_AddEditNyAndCuppsFees", NyAndCuppsFeesViewModelDTO);
        }

        public ActionResult UpdateNyAndCuppsfees(NyAndCuppsFeesViewModelDTO model)
        {
            if (ModelState.IsValid)
            {
                bool IsLocalCompany = Convert.ToBoolean(db.OfficerDatas.FirstOrDefault(x => x.Id == model.OfficerDataId).TowerData.AirLine.IsLocalCompany);
                var id = User.Identity.GetUserId();

                if (model.Id > 0)
                {
                    OfficerData OfficerData = db.OfficerDatas.Where(x => x.Id == model.OfficerDataId).FirstOrDefault();
                    db.Revenues.Where(x => x.Id == model.Id).FirstOrDefault();
                    //Revenue Revenue = new Revenue();

                    OfficerData.Pax = model.Pax;

                    //OfficerData.DISEMB = model.DISEMB;
                    // OfficerData.DisEmbInft = model.DisEmbInft;
                    OfficerData.TRNYCUPPS = model.TRNYCUPPS;
                    OfficerData.IsNotUsedCUPPS = model.IsNotUsedCUPPS;

                    TowerData TowerData = db.TowerDatas.Where(x => x.Id == OfficerData.TowerDataId).FirstOrDefault();
                    if (TowerData.FlightTypeD == 2 && TowerData.FlightTypeL == 2)
                    {
                        decimal DomeNYFees = Cal.DomeNYFees(Convert.ToInt32(OfficerData.Pax), IsLocalCompany);

                        db.Revenues.SingleOrDefault(x => x.Id == model.Id).DomeNYFees = DomeNYFees;
                        //DomeCUPPSFees
                        decimal DomeCUPPSFees = Cal.DomeCUPPSFees(Convert.ToInt32(OfficerData.Pax), Convert.ToInt32(OfficerData.TRNYCUPPS), model.IsNotUsedCUPPS);
                        db.Revenues.SingleOrDefault(x => x.Id == model.Id).DomeCUPPSFees = DomeCUPPSFees;
                        //Zeros Inter
                        db.Revenues.SingleOrDefault(x => x.Id == model.Id).InterCUPPSFees = 0;
                        db.Revenues.SingleOrDefault(x => x.Id == model.Id).InterNYFees = 0;
                    }
                    else
                    {

                        decimal InterNYFees = Cal.InterNYFees(Convert.ToInt32(OfficerData.Pax), IsLocalCompany);

                        db.Revenues.SingleOrDefault(x => x.Id == model.Id).InterNYFees = InterNYFees;

                        //

                        decimal InterCUPPSFees = Cal.InterCUPPSFees(Convert.ToInt32(OfficerData.Pax), Convert.ToInt32(OfficerData.TRNYCUPPS), model.IsNotUsedCUPPS);

                        db.Revenues.SingleOrDefault(x => x.Id == model.Id).InterCUPPSFees = InterCUPPSFees;


                        //Zeros Dome
                        db.Revenues.SingleOrDefault(x => x.Id == model.Id).DomeCUPPSFees = 0;
                        db.Revenues.SingleOrDefault(x => x.Id == model.Id).DomeNYFees = 0;
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

        // //////////////////////////////////End Ny////////////////////////////////////////////////////////////




        // //////////////////////////////////Start Ny////////////////////////////////////////////////////////////
        // public ActionResult CuppsFillter()
        // {
        //     return View();
        // }

        // public ActionResult GetDataCupps(int? page, int? Get, int? Search, int? Count, int? AirLineId
        // , DateTime? DateFrom, DateTime? DateTo
        //, int? FlightTypeId)
        // {


        //     IQueryable<Revenue> Revenue;
        //     Revenue = db.Revenues.OrderBy(x => x.Id).Where(x => x.DomeNYFees != null || x.InterNYFees != null);

        //     ViewBag.AirLineId = AirLineId;


        //     ViewBag.DateFrom = DateFrom;
        //     ViewBag.DateTo = DateTo;

        //     ViewBag.FlightTypeId = FlightTypeId;

        //     if (Get > 0)
        //     {

        //         if (FlightTypeId > 0)
        //         {
        //             Revenue = Revenue.Where(x => x.OfficerData.TowerData.FlightTypeD == FlightTypeId);
        //         }

        //         if (AirLineId > 0)
        //         {
        //             Revenue = Revenue.Where(x => x.OfficerData.TowerData.AirLineId == AirLineId);
        //         }

        //         if (DateFrom != null && DateTo != null)
        //         {

        //             Revenue = Revenue.Where(t => DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) >= DbFunctions.TruncateTime(DateFrom) && DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) <= DbFunctions.TruncateTime(DateTo));
        //         }


        //         //////////////////////////////////////////////////////
        //         if (Search == 1)
        //         {

        //             if (Revenue != null)
        //             {

        //                 int ItemCount = 0;
        //                 //List<decimal> Amountlist = new List<decimal>();
        //                 //List<decimal> Allowancelist = new List<decimal>();


        //                 ItemCount = Revenue.Count();
        //                 ViewBag.ItemCount = ItemCount;

        //             }
        //         }
        //         else
        //         {

        //             //ViewBag.Amount = Amount;
        //             //ViewBag.Allowance = Allowance;
        //             ViewBag.ItemCount = Count;
        //         }

        //         //////////////////////////////////////////////////////////////////////

        //     }
        //     else
        //     {
        //         Revenue = null;
        //         ViewBag.ItemCount = 0;
        //     }

        //     int pageSize = 5;
        //     int PageNumber = (page ?? 1);




        //     return PartialView("_GetCuppsFillter", Revenue.ToPagedList(PageNumber, pageSize));
        // }


        //public ActionResult GetCuppsfeesParial(int? id)
        //{
        //    Revenue Revenue = new Revenue();
        //    CuppsFeesViewModelDTO CuppsFeesViewModelDTO = new CuppsFeesViewModelDTO();
        //    if (id > 0)
        //    {
        //        Revenue = db.Revenues.Find(id);
        //    }
        //    if (Revenue != null)
        //    {
        //        if (Revenue.InterNYFees != null || Revenue.DomeNYFees != null)
        //        {
        //            OfficerData OfficerData = db.OfficerDatas.Where(x => x.Id == Revenue.OfficerDataId).FirstOrDefault();



        //            CuppsFeesViewModelDTO.DISEMB = Convert.ToInt32(OfficerData.DISEMB);
        //            CuppsFeesViewModelDTO.DisEmbInft = Convert.ToInt32(OfficerData.DisEmbInft);
        //          //  CuppsFeesViewModelDTO.TRNY = Convert.ToInt32(OfficerData.TRNY);
        //            CuppsFeesViewModelDTO.TRNYCUPPS = Convert.ToInt32(OfficerData.TRNYCUPPS);
        //            CuppsFeesViewModelDTO.Id = OfficerData.Id;
        //            CuppsFeesViewModelDTO.OfficerDataId = OfficerData.Id;

        //        }
        //    }
        //    //ViewBag.AircraftRegId = new SelectList(db.AircraftRegistrations, "Id", "Registration", towerData.AircraftRegId);
        //    //ViewBag.AirLineId = new SelectList(db.AirLines, "Id", "Code", towerData.AirLineId);
        //    //ViewBag.FlightNo = new SelectList(db.AirLinesFlightTrips, "Id", "FlightNo", towerData.FlightNo);
        //    //ViewBag.AirportIdTo = new SelectList(db.AirPorts, "Id", "Code", towerData.AirportIdTo);
        //    //ViewBag.AirportIdFrom = new SelectList(db.AirPorts, "Id", "Code", towerData.AirportIdFrom);
        //    //ViewBag.CompanyInfoId = new SelectList(db.CompanyInfoes, "Id", "NameAr", towerData.CompanyInfoId);
        //    //ViewBag.TripTypeId = new SelectList(db.TripTypes, "Id", "Name", towerData.TripTypeId);

        //    //return PartialView("_AddEditTowerDataAll", towerData);

        //    return PartialView("_AddEditCuppsFees", CuppsFeesViewModelDTO);
        //}


        //public ActionResult UpdateCuppsfees(CuppsFeesViewModelDTO model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var id = User.Identity.GetUserId();

        //        if (model.Id > 0)
        //        {

        //            OfficerData OfficerData = db.OfficerDatas.Where(x => x.Id == model.OfficerDataId).FirstOrDefault();
        //            db.Revenues.Where(x => x.Id == model.Id).FirstOrDefault();
        //            //Revenue Revenue = new Revenue();

        //            OfficerData.DISEMB = model.DISEMB;
        //            OfficerData.DisEmbInft = model.DisEmbInft;
        //            OfficerData.TRNYCUPPS = model.TRNYCUPPS;
        //          //  OfficerData.TRNY = model.TRNY;

        //            TowerData TowerData = db.TowerDatas.Where(x => x.Id == OfficerData.TowerDataId).FirstOrDefault();
        //            if (TowerData.FlightTypeD == 2 && TowerData.FlightTypeL == 2)
        //            {
        //                decimal value = Calculation.DomeCUPPSFees(OfficerData.Id, Convert.ToInt32(OfficerData.DISEMB), Convert.ToInt32(OfficerData.DisEmbInft), Convert.ToInt32(OfficerData.TRNYCUPPS)
        //                      );

        //                db.Revenues.SingleOrDefault(x => x.Id == model.Id).DomeCUPPSFees = value;

        //            }
        //            else
        //            {
        //                decimal value = Calculation.InterCUPPSFees(OfficerData.Id, Convert.ToInt32(OfficerData.DISEMB), Convert.ToInt32(OfficerData.DisEmbInft), Convert.ToInt32(OfficerData.TRNYCUPPS)
        //                                             );

        //                db.Revenues.SingleOrDefault(x => x.Id == model.Id).InterCUPPSFees = value;
        //            }




        //            db.Entry(OfficerData).State = System.Data.Entity.EntityState.Modified;
        //            //   cal.LandingFees(model.Id);
        //            db.SaveChanges();



        //            return Json(new { success = true, message = "Update" }, JsonRequestBehavior.AllowGet);
        //        }
        //        else
        //        {


        //            //db.TowerDatas.Add(model);
        //            //db.SaveChanges();


        //            return Json(new { success = true, message = "Add" }, JsonRequestBehavior.AllowGet);
        //        }

        //    }
        //    return Json(new { success = false, message = "error" }, JsonRequestBehavior.AllowGet);
        //}

        // //////////////////////////////////End Ny////////////////////////////////////////////////////////////




        // //////////////////////////////////Start First Class////////////////////////////////////////////////////////////

        [Authorize(Roles = "FirstClass,Revenues,Super-Admin")]
        public ActionResult FirstClassFillter()
        {
            return View();
        }

        public ActionResult GetDataFirstClass(int? page, int? Get, int? Search, int? Count, int? AirLineId
        , DateTime? DateFrom, DateTime? DateTo
       , int? FlightTypeId)
        {


            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.OrderBy(x => x.Id).Where(x => x.FirstClassFees != null && x.FirstClassFees > 0);

            ViewBag.AirLineId = AirLineId;


            ViewBag.DateFrom = DateFrom;
            ViewBag.DateTo = DateTo;

            ViewBag.FlightTypeId = FlightTypeId;

            if (Get > 0)
            {

                if (FlightTypeId > 0)
                {
                    if (FlightTypeId == 2)
                    {
                        Revenue = Revenue.Where(x => x.OfficerData.TowerData.FlightTypeD == 2 && x.OfficerData.TowerData.FlightTypeL == 2);
                    }
                    else
                    {
                        Revenue = Revenue.Where(x => x.OfficerData.TowerData.FlightTypeD == 1 || x.OfficerData.TowerData.FlightTypeL == 1);
                    }
                }

                if (AirLineId > 0)
                {
                    Revenue = Revenue.Where(x => x.OfficerData.TowerData.AirLineId == AirLineId);
                }

                if (DateFrom != null && DateTo != null)
                {

                    Revenue = Revenue.Where(t => DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) >= DbFunctions.TruncateTime(DateFrom) && DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) <= DbFunctions.TruncateTime(DateTo));
                }


                //////////////////////////////////////////////////////
                if (Search == 1)
                {

                    if (Revenue != null)
                    {

                        int ItemCount = 0;
                        //List<decimal> Amountlist = new List<decimal>();
                        //List<decimal> Allowancelist = new List<decimal>();


                        ItemCount = Revenue.Count();
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
                Revenue = null;
                ViewBag.ItemCount = 0;
            }

            int pageSize = 5;
            int PageNumber = (page ?? 1);




            return PartialView("_GetFirstClassFillter", Revenue.ToPagedList(PageNumber, pageSize));
        }

        public ActionResult GetFirstClassfeesParial(int? id)
        {
            Revenue Revenue = new Revenue();
            FirstClassFeesViewModelDTO FirstClassFeesViewModelDTO = new FirstClassFeesViewModelDTO();
            if (id > 0)
            {
                Revenue = db.Revenues.Find(id);
            }
            if (Revenue != null)
            {
                if (Revenue.InterNYFees != null || Revenue.DomeNYFees != null)
                {
                    OfficerData OfficerData = db.OfficerDatas.Where(x => x.Id == Revenue.OfficerDataId).FirstOrDefault();



                    //  CuppsFeesViewModelDTO.DISEMB = Convert.ToInt32(OfficerData.DISEMB);
                    FirstClassFeesViewModelDTO.FirstClassCount = Convert.ToInt32(OfficerData.FirstClassCount);
                    //  CuppsFeesViewModelDTO.TRNY = Convert.ToInt32(OfficerData.TRNY);
                    //  CuppsFeesViewModelDTO.TRNYCUPPS = Convert.ToInt32(OfficerData.TRNYCUPPS);
                    FirstClassFeesViewModelDTO.Id = OfficerData.Id;
                    FirstClassFeesViewModelDTO.OfficerDataId = OfficerData.Id;

                }
            }
            //ViewBag.AircraftRegId = new SelectList(db.AircraftRegistrations, "Id", "Registration", towerData.AircraftRegId);
            //ViewBag.AirLineId = new SelectList(db.AirLines, "Id", "Code", towerData.AirLineId);
            //ViewBag.FlightNo = new SelectList(db.AirLinesFlightTrips, "Id", "FlightNo", towerData.FlightNo);
            //ViewBag.AirportIdTo = new SelectList(db.AirPorts, "Id", "Code", towerData.AirportIdTo);
            //ViewBag.AirportIdFrom = new SelectList(db.AirPorts, "Id", "Code", towerData.AirportIdFrom);
            //ViewBag.CompanyInfoId = new SelectList(db.CompanyInfoes, "Id", "NameAr", towerData.CompanyInfoId);
            //ViewBag.TripTypeId = new SelectList(db.TripTypes, "Id", "Name", towerData.TripTypeId);

            //return PartialView("_AddEditTowerDataAll", towerData);

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

        // //////////////////////////////////End Ny////////////////////////////////////////////////////////////


        // //////////////////////////////////Start CarService////////////////////////////////////////////////////////////
        [Authorize(Roles = "Car,Cupps&&Car,Revenues,Super-Admin")]
        public ActionResult CarServiceFillter()
        {
            return View();
        }

        public ActionResult GetDataCarService(int? page, int? Get, int? Search, int? Count, int? AirLineId
        , DateTime? DateFrom, DateTime? DateTo
       , int? FlightTypeId)
        {


            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.OrderBy(x => x.Id).Where(x => x.AmbolanceFees >= 0 || x.FireFightingFees > 0);

            ViewBag.AirLineId = AirLineId;


            ViewBag.DateFrom = DateFrom;
            ViewBag.DateTo = DateTo;

            ViewBag.FlightTypeId = FlightTypeId;

            if (Get > 0)
            {

                if (FlightTypeId > 0)
                {
                    if (FlightTypeId == 2)
                    {
                        Revenue = Revenue.Where(x => x.OfficerData.TowerData.FlightTypeD == 2 && x.OfficerData.TowerData.FlightTypeL == 2);
                    }
                    else
                    {
                        Revenue = Revenue.Where(x => x.OfficerData.TowerData.FlightTypeD == 1 || x.OfficerData.TowerData.FlightTypeL == 1);
                    }
                }

                if (AirLineId > 0)
                {
                    Revenue = Revenue.Where(x => x.OfficerData.TowerData.AirLineId == AirLineId);
                }

                if (DateFrom != null && DateTo != null)
                {

                    Revenue = Revenue.Where(t => DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) >= DbFunctions.TruncateTime(DateFrom) && DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) <= DbFunctions.TruncateTime(DateTo));
                }


                //////////////////////////////////////////////////////
                if (Search == 1)
                {

                    if (Revenue != null)
                    {

                        int ItemCount = 0;
                        //List<decimal> Amountlist = new List<decimal>();
                        //List<decimal> Allowancelist = new List<decimal>();


                        ItemCount = Revenue.Count();
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
                Revenue = null;
                ViewBag.ItemCount = 0;
            }

            int pageSize = 5;
            int PageNumber = (page ?? 1);




            return PartialView("_GetCarServiceFillter", Revenue.ToPagedList(PageNumber, pageSize));
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



                CarServiceFeesViewModelDTO.FireFightingCarCount = Convert.ToInt32(OfficerData.FireFightingCarCount);
                CarServiceFeesViewModelDTO.AmbulanceCarCount = Convert.ToInt32(OfficerData.AmbulanceCarCount);
                //  CuppsFeesViewModelDTO.TRNY = Convert.ToInt32(OfficerData.TRNY);
                //  CuppsFeesViewModelDTO.TRNYCUPPS = Convert.ToInt32(OfficerData.TRNYCUPPS);
                CarServiceFeesViewModelDTO.Id = OfficerData.Id;
                CarServiceFeesViewModelDTO.OfficerDataId = OfficerData.Id;


            }
            //ViewBag.AircraftRegId = new SelectList(db.AircraftRegistrations, "Id", "Registration", towerData.AircraftRegId);
            //ViewBag.AirLineId = new SelectList(db.AirLines, "Id", "Code", towerData.AirLineId);
            //ViewBag.FlightNo = new SelectList(db.AirLinesFlightTrips, "Id", "FlightNo", towerData.FlightNo);
            //ViewBag.AirportIdTo = new SelectList(db.AirPorts, "Id", "Code", towerData.AirportIdTo);
            //ViewBag.AirportIdFrom = new SelectList(db.AirPorts, "Id", "Code", towerData.AirportIdFrom);
            //ViewBag.CompanyInfoId = new SelectList(db.CompanyInfoes, "Id", "NameAr", towerData.CompanyInfoId);
            //ViewBag.TripTypeId = new SelectList(db.TripTypes, "Id", "Name", towerData.TripTypeId);

            //return PartialView("_AddEditTowerDataAll", towerData);

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

        // //////////////////////////////////End Ny////////////////////////////////////////////////////////////





        // //////////////////////////////////Start CarService////////////////////////////////////////////////////////////
        // public ActionResult FreightFillter()
        // {
        //     return View();
        // }

        // public ActionResult GetDataFreight(int? page, int? Get, int? Search, int? Count, int? AirLineId
        // , DateTime? DateFrom, DateTime? DateTo
        //, int? FlightTypeId)
        // {


        //     IQueryable<Revenue> Revenue;
        //     Revenue = db.Revenues.OrderBy(x => x.Id).Where(x => x.DomeNYFees != null || x.InterNYFees != null);

        //     ViewBag.AirLineId = AirLineId;


        //     ViewBag.DateFrom = DateFrom;
        //     ViewBag.DateTo = DateTo;

        //     ViewBag.FlightTypeId = FlightTypeId;

        //     if (Get > 0)
        //     {

        //         if (FlightTypeId > 0)
        //         {
        //             Revenue = Revenue.Where(x => x.OfficerData.TowerData.FlightTypeD == FlightTypeId);
        //         }

        //         if (AirLineId > 0)
        //         {
        //             Revenue = Revenue.Where(x => x.OfficerData.TowerData.AirLineId == AirLineId);
        //         }

        //         if (DateFrom != null && DateTo != null)
        //         {

        //             Revenue = Revenue.Where(t => DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) >= DbFunctions.TruncateTime(DateFrom) && DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) <= DbFunctions.TruncateTime(DateTo));
        //         }


        //         //////////////////////////////////////////////////////
        //         if (Search == 1)
        //         {

        //             if (Revenue != null)
        //             {

        //                 int ItemCount = 0;
        //                 //List<decimal> Amountlist = new List<decimal>();
        //                 //List<decimal> Allowancelist = new List<decimal>();


        //                 ItemCount = Revenue.Count();
        //                 ViewBag.ItemCount = ItemCount;

        //             }
        //         }
        //         else
        //         {

        //             //ViewBag.Amount = Amount;
        //             //ViewBag.Allowance = Allowance;
        //             ViewBag.ItemCount = Count;
        //         }

        //         //////////////////////////////////////////////////////////////////////

        //     }
        //     else
        //     {
        //         Revenue = null;
        //         ViewBag.ItemCount = 0;
        //     }

        //     int pageSize = 5;
        //     int PageNumber = (page ?? 1);




        //     return PartialView("_GetFreightFillter", Revenue.ToPagedList(PageNumber, pageSize));
        // }


        // public ActionResult GetFreightfeesParial(int? id)
        // {
        //     Revenue Revenue = new Revenue();
        //     FreightFeesViewModelDTO FreightFeesViewModelDTO = new FreightFeesViewModelDTO();
        //     if (id > 0)
        //     {
        //         Revenue = db.Revenues.Find(id);
        //     }
        //     if (Revenue != null)
        //     {
        //         if (Revenue.InterNYFees != null || Revenue.DomeNYFees != null)
        //         {
        //             OfficerData OfficerData = db.OfficerDatas.Where(x => x.Id == Revenue.OfficerDataId).FirstOrDefault();



        //             FreightFeesViewModelDTO.FrightLoadingExp = Convert.ToInt32(OfficerData.FreightLoadingExp);
        //             FreightFeesViewModelDTO.FrightLoadingImp = Convert.ToInt32(OfficerData.FreightLoadingImp);
        //             //  CuppsFeesViewModelDTO.TRNY = Convert.ToInt32(OfficerData.TRNY);
        //             //  CuppsFeesViewModelDTO.TRNYCUPPS = Convert.ToInt32(OfficerData.TRNYCUPPS);
        //             FreightFeesViewModelDTO.Id = OfficerData.Id;
        //             FreightFeesViewModelDTO.OfficerDataId = OfficerData.Id;

        //         }
        //     }
        //     //ViewBag.AircraftRegId = new SelectList(db.AircraftRegistrations, "Id", "Registration", towerData.AircraftRegId);
        //     //ViewBag.AirLineId = new SelectList(db.AirLines, "Id", "Code", towerData.AirLineId);
        //     //ViewBag.FlightNo = new SelectList(db.AirLinesFlightTrips, "Id", "FlightNo", towerData.FlightNo);
        //     //ViewBag.AirportIdTo = new SelectList(db.AirPorts, "Id", "Code", towerData.AirportIdTo);
        //     //ViewBag.AirportIdFrom = new SelectList(db.AirPorts, "Id", "Code", towerData.AirportIdFrom);
        //     //ViewBag.CompanyInfoId = new SelectList(db.CompanyInfoes, "Id", "NameAr", towerData.CompanyInfoId);
        //     //ViewBag.TripTypeId = new SelectList(db.TripTypes, "Id", "Name", towerData.TripTypeId);

        //     //return PartialView("_AddEditTowerDataAll", towerData);

        //     return PartialView("_AddEditFreightFees", FreightFeesViewModelDTO);
        // }


        // public ActionResult UpdateFreightfees(FreightFeesViewModelDTO model)
        // {
        //     if (ModelState.IsValid)
        //     {
        //         var id = User.Identity.GetUserId();

        //         if (model.Id > 0)
        //         {

        //             OfficerData OfficerData = db.OfficerDatas.Where(x => x.Id == model.OfficerDataId).FirstOrDefault();
        //             db.Revenues.Where(x => x.Id == model.Id).FirstOrDefault();
        //             //Revenue Revenue = new Revenue();

        //             OfficerData.FreightLoadingExp = model.FrightLoadingExp;
        //             OfficerData.Id = model.OfficerDataId;
        //             OfficerData.FreightLoadingImp = model.FrightLoadingImp;
        //             //  OfficerData.TRNY = model.TRNY;

        //             //TowerData TowerData = db.TowerDatas.Where(x => x.Id == OfficerData.TowerDataId).FirstOrDefault();

        //             decimal FreightFeesValue = Calculation.FreightFees(Convert.ToInt32(model.OfficerDataId), Convert.ToInt32(OfficerData.FreightLoadingExp),Convert.ToInt32(OfficerData.FreightLoadingImp));

        //             db.Revenues.SingleOrDefault(x => x.Id == model.Id).FreightFees = FreightFeesValue;

        //             //decimal FireFightingvalue = Calculation.FireFightingFees(Convert.ToInt32(model.OfficerDataId), Convert.ToInt32(OfficerData.AmbulanceCarCount));

        //             //db.Revenues.SingleOrDefault(x => x.Id == model.Id).FireFightingFees = FireFightingvalue;




        //             db.Entry(OfficerData).State = System.Data.Entity.EntityState.Modified;
        //             //   cal.LandingFees(model.Id);
        //             db.SaveChanges();



        //             return Json(new { success = true, message = "Update" }, JsonRequestBehavior.AllowGet);
        //         }
        //         else
        //         {


        //             //db.TowerDatas.Add(model);
        //             //db.SaveChanges();


        //             return Json(new { success = true, message = "Add" }, JsonRequestBehavior.AllowGet);
        //         }

        //     }
        //     return Json(new { success = false, message = "error" }, JsonRequestBehavior.AllowGet);
        // }

        // //////////////////////////////////End Ny////////////////////////////////////////////////////////////






        // //////////////////////////////////Start CarService////////////////////////////////////////////////////////////

        [Authorize(Roles = "Landing-Admin,Revenues,Super-Admin,Mail&&Freight")]
        public ActionResult MailAndFreightFillter()
        {
            return View();
        }

        public ActionResult GetDataMailAndFreight(int? page, int? Get, int? Search, int? Count, int? AirLineId
        , DateTime? DateFrom, DateTime? DateTo
       , int? FlightTypeId)
        {


            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.OrderBy(x => x.Id).Where(x => x.FreightFees > 0 && x.ExpMailFees > 0 && x.ImpMailFees > 0);

            ViewBag.AirLineId = AirLineId;


            ViewBag.DateFrom = DateFrom;
            ViewBag.DateTo = DateTo;

            ViewBag.FlightTypeId = FlightTypeId;

            if (Get > 0)
            {

                if (FlightTypeId > 0)
                {
                    if (FlightTypeId == 2)
                    {
                        Revenue = Revenue.Where(x => x.OfficerData.TowerData.FlightTypeD == 2 && x.OfficerData.TowerData.FlightTypeL == 2);
                    }
                    else
                    {
                        Revenue = Revenue.Where(x => x.OfficerData.TowerData.FlightTypeD == 1 || x.OfficerData.TowerData.FlightTypeL == 1);
                    }
                }

                if (AirLineId > 0)
                {
                    Revenue = Revenue.Where(x => x.OfficerData.TowerData.AirLineId == AirLineId);
                }

                if (DateFrom != null && DateTo != null)
                {

                    Revenue = Revenue.Where(t => DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) >= DbFunctions.TruncateTime(DateFrom) && DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) <= DbFunctions.TruncateTime(DateTo));
                }


                //////////////////////////////////////////////////////
                if (Search == 1)
                {

                    if (Revenue != null)
                    {

                        int ItemCount = 0;
                        //List<decimal> Amountlist = new List<decimal>();
                        //List<decimal> Allowancelist = new List<decimal>();


                        ItemCount = Revenue.Count();
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
                Revenue = null;
                ViewBag.ItemCount = 0;
            }

            int pageSize = 5;
            int PageNumber = (page ?? 1);




            return PartialView("_GetMailAndFreightFillter", Revenue.ToPagedList(PageNumber, pageSize));
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



                MailAndFreightFeesViewModelDTO.NormalMailLoadingExpAmount = Convert.ToInt32(OfficerData.NormalMailLoadingExp);
                MailAndFreightFeesViewModelDTO.RapidMailLoadingExpAmount = Convert.ToInt32(OfficerData.RapidMailLoadingExp);

                MailAndFreightFeesViewModelDTO.NormalMailLoadingImpAmount = Convert.ToInt32(OfficerData.NormalMailLoadingImp);
                MailAndFreightFeesViewModelDTO.RapidMailLoadingImpAmount = Convert.ToInt32(OfficerData.RapidMailLoadingImp);


                MailAndFreightFeesViewModelDTO.FrightLoadingExp = Convert.ToInt32(OfficerData.FreightLoadingExp);
                MailAndFreightFeesViewModelDTO.FrightLoadingImp = Convert.ToInt32(OfficerData.FreightLoadingImp);
                //  CuppsFeesViewModelDTO.TRNY = Convert.ToInt32(OfficerData.TRNY);
                //  CuppsFeesViewModelDTO.TRNYCUPPS = Convert.ToInt32(OfficerData.TRNYCUPPS);
                MailAndFreightFeesViewModelDTO.Id = OfficerData.Id;
                MailAndFreightFeesViewModelDTO.OfficerDataId = OfficerData.Id;

                //}
            }

            return PartialView("_AddEditMailAndfreightFees", MailAndFreightFeesViewModelDTO);
        }

        public ActionResult UpdateMailAndFreightfees(MailAndFreightFeesViewModelDTO model)
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

                    OfficerData.NormalMailLoadingExp = model.NormalMailLoadingExpAmount;
                    OfficerData.RapidMailLoadingExp = model.RapidMailLoadingExpAmount;

                    OfficerData.NormalMailLoadingImp = model.NormalMailLoadingImpAmount;
                    OfficerData.RapidMailLoadingImp = model.RapidMailLoadingImpAmount;

                    OfficerData.FreightLoadingExp = model.FrightLoadingExp;
                    OfficerData.FreightLoadingImp = model.FrightLoadingImp;


                    decimal ExpMailFeesValue = Cal.ExpMailFees(Convert.ToInt32(OfficerData.NormalMailLoadingExp), Convert.ToInt32(OfficerData.RapidMailLoadingExp));

                    db.Revenues.SingleOrDefault(x => x.Id == model.Id).ExpMailFees = ExpMailFeesValue;

                    decimal ImpMailFeesValue = Cal.ExpMailFees(Convert.ToInt32(OfficerData.NormalMailLoadingImp), Convert.ToInt32(OfficerData.RapidMailLoadingImp));

                    db.Revenues.SingleOrDefault(x => x.Id == model.Id).ImpMailFees = ImpMailFeesValue;

                    decimal FreightFeesValue = Cal.FreightFees(Convert.ToInt32(OfficerData.FreightLoadingExp), Convert.ToInt32(OfficerData.FreightLoadingImp));

                    db.Revenues.SingleOrDefault(x => x.Id == model.Id).FreightFees = FreightFeesValue;


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

        // //////////////////////////////////End Ny////////////////////////////////////////////////////////////





        // //////////////////////////////////Start CarService////////////////////////////////////////////////////////////
        [Authorize(Roles = "PushPack,Revenues,Super-Admin")]
        public ActionResult PushPackFillter()
        {
            return View();
        }

        public ActionResult GetDataPushPack(int? page, int? Get, int? Search, int? Count, int? AirLineId
        , DateTime? DateFrom, DateTime? DateTo
       , int? FlightTypeId)
        {


            IQueryable<Revenue> Revenue;
            Revenue = db.Revenues.OrderBy(x => x.Id).Where(x => x.PushBackFees > 0);

            ViewBag.AirLineId = AirLineId;


            ViewBag.DateFrom = DateFrom;
            ViewBag.DateTo = DateTo;

            ViewBag.FlightTypeId = FlightTypeId;

            if (Get > 0)
            {

                if (FlightTypeId > 0)
                {
                    if (FlightTypeId == 2)
                    {
                        Revenue = Revenue.Where(x => x.OfficerData.TowerData.FlightTypeD == 2 && x.OfficerData.TowerData.FlightTypeL == 2);
                    }
                    else
                    {
                        Revenue = Revenue.Where(x => x.OfficerData.TowerData.FlightTypeD == 1 || x.OfficerData.TowerData.FlightTypeL == 1);
                    }
                }

                if (AirLineId > 0)
                {
                    Revenue = Revenue.Where(x => x.OfficerData.TowerData.AirLineId == AirLineId);
                }

                if (DateFrom != null && DateTo != null)
                {

                    Revenue = Revenue.Where(t => DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) >= DbFunctions.TruncateTime(DateFrom) && DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) <= DbFunctions.TruncateTime(DateTo));
                }


                //////////////////////////////////////////////////////
                if (Search == 1)
                {

                    if (Revenue != null)
                    {

                        int ItemCount = 0;
                        //List<decimal> Amountlist = new List<decimal>();
                        //List<decimal> Allowancelist = new List<decimal>();


                        ItemCount = Revenue.Count();
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
                Revenue = null;
                ViewBag.ItemCount = 0;
            }

            int pageSize = 5;
            int PageNumber = (page ?? 1);




            return PartialView("_GetPushPackFillter", Revenue.ToPagedList(PageNumber, pageSize));
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
                PushPackFeesViewModelDTO.Id = OfficerData.Id;
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
                        if (OfficerData.TowerData.AircraftRegistration.AircraftType.AircraftSize.Size == null)
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

        // //////////////////////////////////End Ny////////////////////////////////////////////////////////////












    }
}