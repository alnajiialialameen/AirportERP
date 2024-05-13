using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AirPortERP.Models;
using AirPortERP.ViewModels;

namespace AirPortERP.Extenstion
{
    public class CommonUsed
    {

        Entities db = new Entities();
        public string CheckDate(string d)
        {
            string value = "";
            if (d == null || d == "" || string.IsNullOrEmpty(d))
            {
                value = null;
                return value;
            }
            else
            {
                value = Convert.ToDateTime(d).ToString("MM/dd/yyyy");
                return value;
            }
        }
        public string GetFlightTypeNameLanding(int? FlightTypeL)
        {
            string value = "Not Detected";

            if (FlightTypeL == 2)
            {
                value = "Domestic";
            }
            else
            {
                value = "International";
            }

            return value;
        }

        public string GetFlightTypeNameForCUPPSNY(int? FlightTypeD)
        {
            string value = "Not Detected";
            if (FlightTypeD == 2)
            {
                value = "Domestic";
            }
            else
            {
                value = "International";
            }

            return value;
        }   
        public string GetFlightTypeName(int? FlightTypeL, int? FlightTypeD)
        {
            string value ="Not Detected";
        
            //if(FlightTypeD == 1 && FlightTypeL == 1)
            //{
            //    value = "International";            
            //}
            //else 
            if (FlightTypeD == 2 && FlightTypeL == 2)
            {
                value = "Domestic";           
            }
            else
            {
                value = "International";
            }
            
            //if(FlightTypeD == 1)
            //{
            //    value = "International";
            //}
            //else if (FlightTypeL == 1)
            //{
            //    value = "International";
            //}
            return value;
        }

        //public int GetFlightType(int? FlightTypeL, int? FlightTypeD)
        //{
        //    int value = 0;

        //    //if(FlightTypeD == 1 && FlightTypeL == 1)
        //    //{
        //    //    value = "International";            
        //    //}
        //    //else 
        //    if (FlightTypeD == 2 && FlightTypeL == 2)
        //    {
        //        value = 2;
        //    }
        //    else
        //    {
        //        value = 1;
        //    }

        //    //if(FlightTypeD == 1)
        //    //{
        //    //    value = "International";
        //    //}
        //    //else if (FlightTypeL == 1)
        //    //{
        //    //    value = "International";
        //    //}
        //    return value;
        //}


        public int GetFlightType(int? FlightTypeL)
        {
            int value = 0;

            //if(FlightTypeD == 1 && FlightTypeL == 1)
            //{
            //    value = "International";            
            //}
            //else 
            if (FlightTypeL == 2)
            {
                value = 2;
            }
            else
            {
                value = 1;
            }

            //if(FlightTypeD == 1)
            //{
            //    value = "International";
            //}
            //else if (FlightTypeL == 1)
            //{
            //    value = "International";
            //}
            return value;
        }

        public string GetFlightTypeForSetting(int? FlightType)
        {
            if (FlightType == 1)
            {
                return "International";
            }
            else
            {
                return "Domestic";
            }            
        }
        public string GetFlightTypeName(int? FlightTypeId)
        {
            string value = "Not Detected";

            if (FlightTypeId == 1)
            {
                value = "International";
            }
            else if (FlightTypeId == 2)
            {
                value = "Domestic";
            }
            return value;
        }

        public string GetUserName(string Id)
        {
            string UserName;
            if (Id == null)
            {
                return "";
            }
            else
            {


                UserName = db.AspNetUsers.Single(x => x.Id == Id).UserName;
                return UserName;
            }
        }

        public string GetUserRole(string Id)
        {
            string Role;
            if (Id == null)
            {
                return "";
            }
            else
            {


                Role = db.AspNetRoles.Single(x => x.Id == Id).Name;
                return Role;
            }
        }

        public string CheckYesNo(bool Value)
        {
            string StatusName;
            if (Value == false)
            {
                StatusName = "No";
            }
            else
            {
                StatusName = "Yes";
            }
            return StatusName;
        }

        public string CheckAirportCode(int AirportId)
        {
            string StatusName = "";
            if(AirportId > 0)
            {
                if (!db.AirPorts.Any(x => x.Id == AirportId))
                {
                    StatusName = "";
                }
                else
                {
                    StatusName = db.AirPorts.SingleOrDefault(x => x.Id == AirportId).Code;
                }
            }
                  
            return StatusName;
        }

        public string CheckCountryCode(int AirportId)
        {
            string StatusName = "";
            if (AirportId > 0)
            {               
                if (!db.AirPorts.Any(x => x.Id == AirportId))
                {
                    StatusName = "";
                }
                else
                {
                    StatusName = db.AirPorts.SingleOrDefault(x => x.Id == AirportId).Country.NameEn;
                }
            }

            return StatusName;
        }

        public string isLocal(bool Value)
        {
            string StatusName;
            if (Value == false)
            {
                StatusName = "Forgien";
            }
            else
            {
                StatusName = "Local";
            }
            return StatusName;
        }

        public string isActive(bool Value)
        {
            string StatusName;
            if (Value == false)
            {
                StatusName = "Not Active";
            }
            else
            {
                StatusName = "Active";
            }
            return StatusName;
        }

        public int GetMouth(string Id)
        {
            int Mouth;
            if (Id == null)
            {
                return 0;
            }
            else
            {


                Mouth = db.WorkOns.Single(x => x.UserId == Id).MouthWorkOn.Id;
                return Mouth;
            }
        }

        public string GetMouthName(string Id)
        {
            string Mouth;
            if (Id == null)
            {
                return "";
            }
            else
            {


                Mouth = db.WorkOns.Single(x => x.UserId == Id).MouthWorkOn.Mouth;
                return Mouth;
            }
        }


        public string GetMouthbyId(int Id)
        {
            string Mouth;
            if (Id == 0)
            {
                return "";
            }
            else
            {


                Mouth = db.MouthWorkOns.Single(x => x.Id == Id).Mouth;
                return Mouth;
            }
        }









        public int GetYear(string Id)
        {
            int Year;
            if (Id == null)
            {
                return 0;
            }
            else
            {


                Year = Convert.ToInt32(db.WorkOns.Single(x => x.UserId == Id).YearWorkOn.Year);
                return Year;
            }
        }

        public bool HasMouth(string Id)
        {
            bool HasMouth;
            if (Id == null)
            {
                return false;
            }
            else
            {


                HasMouth = db.WorkOns.Any(x => x.UserId == Id);
               
                return HasMouth;
            }
        }

        /**************************************** Added By Yousif ****Update By Naji 21-5-2022********************************************************/
        public decimal getExchangeRateByLandingDate(int Month, int Year)
        {
            decimal ExchangeRate = Convert.ToDecimal(db.CurrencyDetails.SingleOrDefault(x => x.CurrencyId == 1 && x.MouthWorkOn.Id == Month && x.YearWorkOn.Year == Year).ExchangeRateAgainstPrimaryCurrency);
            return ExchangeRate;
        }

        public decimal getExchangeRateByTakeoffDate(int Month, int Year)
        {
            //decimal ExchangeRate = Convert.ToDecimal(db.CurrencyDetails.Where(x => x.CurrencyId == 1 && x.Date <= TakeoffDate).OrderBy(o => o.Date).Last().ExchangeRateAgainstPrimaryCurrency);
            decimal ExchangeRate = Convert.ToDecimal(db.CurrencyDetails.SingleOrDefault(x => x.CurrencyId == 1 && x.MouthWorkOn.Id == Month && x.YearWorkOn.Year == Year).ExchangeRateAgainstPrimaryCurrency);
            return ExchangeRate;
        }


        public void TransactionRolleBack()
        {
            TransactionLog TransactionLog = db.TransactionLogs.Where(x => x.IsComplate == false).FirstOrDefault();
            if (TransactionLog!=null)
            {
                if (TransactionLog.OfficerDataId > 0 && TransactionLog.TowerDataId > 0) {

                    OfficerData OfficerData = db.OfficerDatas.Where(x => x.Id == TransactionLog.OfficerDataId).FirstOrDefault();
                    TowerData TowerData = db.TowerDatas.Where(x => x.Id == TransactionLog.TowerDataId).FirstOrDefault();

                    // var AirlineDelete = db.AirLines.Find(id);
                  
                    db.OfficerDatas.Remove(OfficerData);
                    db.TowerDatas.Remove(TowerData);

                    db.TransactionLogs.Remove(TransactionLog);
                    db.SaveChanges();
                }


                if (TransactionLog.OfficerDataId == null && TransactionLog.TowerDataId > 0)
                {

                   // OfficerData OfficerData = db.OfficerDatas.Where(x => x.Id == TransactionLog.OfficerDataId).FirstOrDefault();
                    TowerData TowerData = db.TowerDatas.Where(x => x.Id == TransactionLog.TowerDataId).FirstOrDefault();

                    // var AirlineDelete = db.AirLines.Find(id);
                    // db.OfficerDatas.Remove(OfficerData);
                
                    db.TowerDatas.Remove(TowerData);

                    db.TransactionLogs.Remove(TransactionLog);
                    db.SaveChanges();
                }

                if (TransactionLog.OfficerDataId > 0 && TransactionLog.TowerDataId > 0 && TransactionLog.RevenueDataId > 0) 
                {
                    TransactionLog.IsComplate = true;
                    db.Entry(TransactionLog).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }


            }

        }

    }
}