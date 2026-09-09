using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AirPortERP.Models;
using AirPortERP.ViewModels;
using System.Data.Entity;
using System.Globalization;
using System.ComponentModel.Design;
using Microsoft.AspNet.Identity;
namespace AirPortERP.Extenstion
{
    public class CommonUsed
    {

        Entities db = new Entities();

        // دي من خلالها بختبر هل المستخدم ده متاح ليهو يعدل بعد استخراج المطالبة ولا لا حسب دوره
        private string[] AllowedRolesList = new string[] {"Account", "Account-Admin"};

        //public string CheckDate(string d)
        //{
        //    string value = "";
        //    if (d == null || d == "" || string.IsNullOrEmpty(d))
        //    {
        //        value = null;
        //        return value;
        //    }
        //    else
        //    {
        //        value = Convert.ToDateTime(d).ToString("MM/dd/yyyy");
        //        return value;
        //    }
        //}
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
            int? Mouth;
            if (Id == null)
            {
                return 0;
            }
            else
            {


                Mouth = db.WorkOns.Where(x => x.UserId == Id).FirstOrDefault().MonthWorkOn;
                return Convert.ToInt32(Mouth);
            }
        }
        public string GetMouthText(string Id)
        {
            int? Mouth;
            if (Id == null)
            {
                return "";
            }
            else
            {


                Mouth = db.WorkOns.Where(x => x.UserId == Id).FirstOrDefault().MonthWorkOn;
                
                return GetMonthName(Convert.ToInt32(Mouth));
            }
        }



        //public string GetMouthbyId(int Id)
        //{
        //    string Mouth;
        //    if (Id == 0)
        //    {
        //        return "";
        //    }
        //    else
        //    {


        //        Mouth = db.MouthWorkOns.Single(x => x.Id == Id).Mouth;
        //        return Mouth;
        //    }
        //}









        public int GetYear(string Id)
        {
            int Year;
            if (Id == null)
            {
                return 0;
            }
            else
            {


                Year = Convert.ToInt32(db.WorkOns.Single(x => x.UserId == Id).YearWorkOn);
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
            //decimal ExchangeRate = Convert.ToDecimal(db.CurrencyDetails.SingleOrDefault(x => x.CurrencyId == 1 && x.MouthWorkOn.Id == Month && x.YearWorkOn.Year == Year).ExchangeRateAgainstPrimaryCurrency);
            decimal? ExchangeRate = db.CurrencyDetails.Where(x => x.CurrencyId == 1
               && x.MonthWorkOn == Month
               && x.YearWorkOn == Year)
      .OrderByDescending(x => x.Id)   // أو أي عمود يمثل الترتيب الصحيح
      .Select(x => x.ExchangeRateAgainstPrimaryCurrency)
      .FirstOrDefault();

            return Convert.ToDecimal(ExchangeRate);
        }
        public decimal getExchangeRateByLandingDateForSingle(DateTime date)
        {

            var currency = db.CurrencyDetails
     .SingleOrDefault(x =>
         x.CurrencyId == 1 &&
         DbFunctions.TruncateTime(x.Date) == DbFunctions.TruncateTime(date));


            if (currency == null)
                return 0; // أو أي قيمة افتراضية تريدها

            return currency.ExchangeRateAgainstPrimaryCurrency ?? 0;
        }

        public decimal getExchangeRateByTakeoffDate(int Month, int Year)
        {
            //decimal ExchangeRate = Convert.ToDecimal(db.CurrencyDetails.Where(x => x.CurrencyId == 1 && x.Date <= TakeoffDate).OrderBy(o => o.Date).Last().ExchangeRateAgainstPrimaryCurrency);
            decimal ExchangeRate = Convert.ToDecimal(db.CurrencyDetails.SingleOrDefault(x => x.CurrencyId == 1 && x.MonthWorkOn == Month && x.YearWorkOn == Year).ExchangeRateAgainstPrimaryCurrency);
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


        public bool IsEntryDateinRange(DateTime entryDate, string Role, int AirlineId,int companyId)
        {
       
            AirLine airline = db.AirLines.Find(AirlineId);

            // if (Role != "Super-Admin3")
            if (!AllowedRolesList.Contains(Role))
            {
                // Query the database to check if the EntryDate is within any DateFrom and DateTo range
                //  return db.FinancialClaims.Any(x =>x.DateFrom.HasValue && x.DateTo.HasValue && entryDate >= x.DateFrom && entryDate <= x.DateTo);
                ///  Revenue = Revenue.Where(t => DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) >= DbFunctions.TruncateTime(DateFrom) && DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) <= DbFunctions.TruncateTime(DateTo));

                //  return db.FinancialClaims.Any(x => x.AccIdDebit == airline.AccId && (DbFunctions.TruncateTime(entryDate) >= DbFunctions.TruncateTime(x.DateFrom) && DbFunctions.TruncateTime(entryDate) <= DbFunctions.TruncateTime(x.DateTo)));
                return db.FinancialClaim1.Any(x => x.AccIdDebit == airline.AccId && x.DateTo.HasValue && entryDate >= x.DateFrom && entryDate <= x.DateTo && x.CompanyInfoId == companyId);
            }
            else
            {
                return false;
            }
        }






        public static DateTime ConvertToDbDate(string dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString))
                throw new ArgumentNullException(nameof(dateString), "Date is required");

            if (DateTime.TryParseExact(
                dateString.Trim(),
                "dd/MM/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime date))
            {
                return date;
            }

            throw new FormatException("Invalid date format. Expected dd/MM/yyyy");
        }



       
            /// <summary>
            /// تحويل تاريخ من قاعدة البيانات إلى صيغة dd/MM/yyyy ميلادية
            /// </summary>
            /// <param name="date">DateTime? من قاعدة البيانات</param>
            /// <returns>سلسلة التاريخ بصيغة dd/MM/yyyy أو null إذا كان التاريخ فارغ</returns>
            public static string ToDisplayDate(DateTime? date)
            {
                if (!date.HasValue)
                    return null;

                return date.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
        public static string ToDisplayDate1(DateTime? date)
        {
            if (!date.HasValue)
                return null;

            var culture = new CultureInfo("en-US");
            culture.DateTimeFormat.Calendar = new GregorianCalendar();

            return date.Value.ToString("dd/MM/yyyy", culture);
        }

        public static string CheckDate(string date)
        {
            DateTime? myDate = Convert.ToDateTime(date);
            if (!myDate.HasValue)
                return null;

            return myDate.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        }

        public bool IsEntryDateinRange2(int? TowerDataId, string Role)
        {
            //AirLine airline = db.AirLines.Find(AirlineId);

            // if (Role != "Super-Admin3")
            if (!AllowedRolesList.Contains(Role))
            {
                // Query the database to check if the EntryDate is within any DateFrom and DateTo range
                //  return db.FinancialClaims.Any(x =>x.DateFrom.HasValue && x.DateTo.HasValue && entryDate >= x.DateFrom && entryDate <= x.DateTo);
                ///  Revenue = Revenue.Where(t => DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) >= DbFunctions.TruncateTime(DateFrom) && DbFunctions.TruncateTime(t.OfficerData.TowerData.Date) <= DbFunctions.TruncateTime(DateTo));

                //  return db.FinancialClaims.Any(x => x.AccIdDebit == airline.AccId && (DbFunctions.TruncateTime(entryDate) >= DbFunctions.TruncateTime(x.DateFrom) && DbFunctions.TruncateTime(entryDate) <= DbFunctions.TruncateTime(x.DateTo)));
                return db.FinancialClaim1.Any(x => x.TowerDataId == TowerDataId);
            }
            else
            {
                return false;
            }
        }


        public int? getCompanyId(string id)
        {          
            return db.WorkOns.Where(x => x.UserId == id).FirstOrDefault().CompanyInfoId;
        }



        public string getCompanyName(int? CompanyInfoId)
        {
            if (CompanyInfoId > 0)
            {
                return db.CompanyInfoes.Where(x => x.Id == CompanyInfoId).FirstOrDefault().NameAr;
            }
            else { return ""; }
        }

        public bool getIsSingleAccounting(int? CompanyInfoId)
        {
            bool IsSingleAccounting = false;
            if (CompanyInfoId >= 0)
            {
                IsSingleAccounting = Convert.ToBoolean(db.CompanyInfoes.Where(x => x.Id == CompanyInfoId).FirstOrDefault().IsSingleAccounting);
                return IsSingleAccounting;
            }
            else { return IsSingleAccounting; }

        }







        //////-----------------------------------------------

        public string GetMonthName(int month)
        {
            string[] months = {
        "يناير","فبراير","مارس","أبريل","مايو","يونيو",
        "يوليو","أغسطس","سبتمبر","أكتوبر","نوفمبر","ديسمبر"
    };

            return months[month - 1];
        }

        public List<YearDto> GetYears()
        {
            int currentYear = DateTime.Now.Year;

            return Enumerable.Range(currentYear - 5, 6)
                .OrderByDescending(y => y)
                .Select(y => new YearDto
                {
                    Id = y,
                    Year = y
                })
                .ToList();
        }

        public List<MonthDto> GetMonths()
        {
            return Enumerable.Range(1, 12)
                .Select(m => new MonthDto
                {
                    Id = m,
                    Name = GetMonthName(m)
                })
                .ToList();
        }








    }
}