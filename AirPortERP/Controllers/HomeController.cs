using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AirPortERP.ViewModels;
using AirPortERP.Models;
using System.Data.Entity;
using AirPortERP.Extenstion;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using System.IO;
using System.Threading;

namespace AirPortERP.Controllers
{
   
    public class HomeController : Controller
    {
        private ApplicationUserManager _userManager;
        DescriptionReportModel DescriptionReport = new DescriptionReportModel();
        DateTime now = System.DateTime.Now;
        Entities db = new Entities();
        CommonUsed c = new CommonUsed();
         
        public ApplicationUserManager UserManager
        {
            
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }



        public ActionResult Index()
        {

            db.TransactionLogDelete();
            CommonUsed CommonUsed = new CommonUsed();
            CommonUsed.TransactionRolleBack();

            // international && domestic In Month
            DescriptionReport.TotalInternationalMonthCount = db.TowerDatas.Where(x => x.FlightTypeD == 1 || x.FlightTypeL == 1 && x.Date.Value.Year == now.Year 
            && x.Date.Value.Month==now.Month).Count();
            DescriptionReport.TotalDomesticMonthCount = db.TowerDatas.Where(x => x.FlightTypeD == 2 && x.FlightTypeL == 2 && x.Date.Value.Year == now.Year
            && x.Date.Value.Year == now.Year && x.Date.Value.Month == now.Month).Count();
            DescriptionReport.InternationalDomesticMonthTitle = "International And Domestic Flight in " + " - " + now.ToString("MM/yyyy");   //ToString("MM/yyyy")







            // international && domestic
            DescriptionReport.TotalInternationalCount = db.TowerDatas.Where(x => x.FlightTypeD == 1 || x.FlightTypeL == 1 && x.Date.Value.Year == now.Year).Count();
            DescriptionReport.TotalDomesticCount = db.TowerDatas.Where(x => x.FlightTypeD == 2 && x.FlightTypeL == 2 && x.Date.Value.Year == now.Year).Count();
            DescriptionReport.InternationalDomesticTitle = "International Domestic Flight in " + " - " + now.ToString("yyyy");   //ToString("MM/yyyy")


            DescriptionReport.TotalForeign = db.TowerDatas.Where(x => x.AirLine.IsLocalCompany==false).Count();
            DescriptionReport.Totallocal = db.TowerDatas.Where(x => x.AirLine.IsLocalCompany == true).Count();
            DescriptionReport.TotallocalDomstic = db.TowerDatas.Where(x => x.AirLine.IsLocalCompany == true &&
            x.FlightTypeL==2 && x.FlightTypeD==2).Count();

            DescriptionReport.TotallocalInter = db.TowerDatas.Where(x => x.AirLine.IsLocalCompany == true &&
            x.FlightTypeL == 1 || x.FlightTypeD == 1).Count();


            DescriptionReport.TripTypeTitle = "Trip Type Flight in " + " - " + now.ToString("MM/yyyy");



            List<vwTopMaxAirLine> mList = db.vwTopMaxAirLines.OrderByDescending(x => x.Count).Take(5).ToList();

            if (mList.Count >= 5)
            {
                DescriptionReport.FiveTakeOffStatus = true;
                DescriptionReport.FiveTakeOffCount1 = Convert.ToInt32(mList[0].Count);
                DescriptionReport.FiveTakeOffName1 = mList[0].Name.ToString();

                DescriptionReport.FiveTakeOffCount2 = Convert.ToInt32(mList[1].Count);
                DescriptionReport.FiveTakeOffName2 = mList[1].Name.ToString();

                DescriptionReport.FiveTakeOffCount3 = Convert.ToInt32(mList[2].Count);
                DescriptionReport.FiveTakeOffName3 = mList[2].Name.ToString();

                DescriptionReport.FiveTakeOffCount4 = Convert.ToInt32(mList[3].Count);
                DescriptionReport.FiveTakeOffName4 = mList[3].Name.ToString();

                DescriptionReport.FiveTakeOffCount5= Convert.ToInt32(mList[4].Count);
                DescriptionReport.FiveTakeOffName5 = mList[4].Name.ToString();
               
                DescriptionReport.FiveTakeOffTitle = "Top Flights Airlines" + " - " + now.ToString("MM/yyyy");
            }
            else {
                DescriptionReport.FiveTakeOffStatus = false;
            }


            //بعد الدوام     
            DescriptionReport.InOverTimeCount = db.TowerDatas.Where(x => x.ATA.Value.Hours >= 16 || x.ATA.Value.Hours <= 4).Count();

            //أثناء الدوام
            DescriptionReport.InDuityCount = db.TowerDatas.Where(x => x.ATA.Value.Hours > 4 && x.ATA.Value.Hours < 16).Count();

            DescriptionReport.FiveTakeOffTitle = "Top Flights Airlines" + " - " + now.ToString("MM/yyyy");





            DescriptionReport.TotalCountOfAirline = db.AirLines.Count();
            DescriptionReport.TotalAirLinesFlightTrip = db.AirLinesFlightTrips.Count();
            DescriptionReport.TotalAirPort = db.AirPorts.Count();
            DescriptionReport.TotalCountries = db.Countries.Count();


            // Day && night
            DescriptionReport.TotalFlightInDay =
           db.TowerDatas.Where(x => DbFunctions.TruncateTime(x.Date.Value).ToString() == 
           DbFunctions.TruncateTime(now.Date).ToString()).Count();

            DescriptionReport.TotalFlightInMonth = db.TowerDatas.Where(x => x.Date.Value.Month 
            == now.Month && x.Date.Value.Year ==now.Year).Count();

          DescriptionReport.TotalFlightInYear = db.TowerDatas.Where(x => x.Date.Value.Year == now.Year).Count();

            DescriptionReport.HasNightStop = db.OfficerDatas.Where(x => x.HasNightStop == true && x.TowerData.Date.Value.Month == now.Month).Count();
            DescriptionReport.NotHasNightStop = db.OfficerDatas.Where(x => x.HasNightStop == false && x.TowerData.Date.Value.Month == now.Month).Count();
            DescriptionReport.NightStopTitle = "Flight Has Night Stop in " + now.Month + "/" + now.Year;


            return View(DescriptionReport);
        }

        //public string GetRole()
        //{
        //    var id = User.Identity.GetUserId();
        //    ApplicationUser user = UserManager.FindById(id);
        //    string rolename = UserManager.GetRoles(user.Id).FirstOrDefault();

        //    //Session["Role"] = rolename;
        //    ViewBag.Role = rolename;

        //    return rolename;
        //}

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        [HttpGet]
        public ActionResult Backup()
        {
            return View();
        }

        //[HttpPost]
        //public ActionResult BackUp()
        //{
        //    BackUp1();

        //    //dto.MessageBody = "تمت العملية بنجاح";
        //    //dto.MessageTitle = "نجاح العملية";
        //    //dto.MessageType = "Success";
        //    //return Json(new , JsonRequestBehavior.AllowGet);

        //    return Json(new { success = true, message = "Success" }, JsonRequestBehavior.AllowGet);
        //}


        public ActionResult BackUp()
        {
            Thread.Sleep(300);

            int res = BackUp1();

            if (res > 0)
                return Json(new { success = true, message = "Success" }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { success = true, message = "Faild" }, JsonRequestBehavior.AllowGet);
            
        }

        public int BackUp1()
        {
            Entities entity = new Entities();
            string dataTime = DateTime.Now.ToString("yyyy-MM");
            int Day = DateTime.Now.Day;
            string directory = HttpContext.Server.MapPath("~/") + "Backup/" + dataTime + "/";
            string fileName = directory + "/back  Day  "+ Day + " at " + DateTime.Now.ToString(" HH-mm") + ".bak";

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            // Here the procedure is called and executes successfully
            try {
                entity.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, "EXEC [dbo].[spBackUp] @path = N'" + fileName + "'");
                return 1;
            }
            catch(Exception){
                return -1;
            }
           
        }


        public ActionResult Gett()
        {
            List<GettVM> data1 = new List<GettVM>();
            try
            {
                var data = db.AccountOperations.Where(d => d.AccountTree.AccGroupId == 1).Select(x =>
                      new GettVM()
                      {
                          Id = x.AccId,
                          AirLineName = x.AccountTree.AirLines.FirstOrDefault().Name,
                          Debit = x.OperationDebit,
                          Credit = x.OperationCredit,
                          CurrencyValue = x.CurrencyValue,
                          Balance = x.OperationDebit - x.OperationCredit,
                      }
                ).ToList();

                foreach (var item in data)
                {
                    if (!data1.Any(x => x.Id == item.Id))
                    {
                        data1.Add(item);
                    }
                    else
                    {
                        var obj = data1.FirstOrDefault(x => x.Id == item.Id);
                        obj.Credit = obj.Credit + item.Credit;
                        obj.Debit = obj.Debit + item.Debit;
                        obj.Balance = obj.Balance + item.Balance;
                    }
                }

                ViewBag.SumOfDebit = data1.Sum(x => x.Debit);
                ViewBag.SumOfCredit = data1.Sum(x => x.Credit);
                ViewBag.SumOfBalance = data1.Sum(x => x.Balance);
                ViewBag.Count = data1.Count();


                //ToWord toWord1 = new ToWord(Convert.ToDecimal(DollarRound), currencies[Convert.ToInt32(1)]);

                //F.GrandTotalDollarTxt = toWord1.ConvertToArabic().ToString();


                return View(data1);
            }
            catch (Exception e)
            {
                return Json(e.Message.ToString(), JsonRequestBehavior.AllowGet);
            }
        }

    }
}