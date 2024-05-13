using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AirPortERP.Models;

namespace AirPortERP.Extenstion
{
    public class Calculation
    {
        private static Entities db = new Entities();
        private decimal FinalAmountInDollar = 0;
        private CommonUsed cu = new CommonUsed();

        /// <summary>
        /// دالة حساب رسوم الهبوط حسب اللائحة
        /// الفقرة رقم 1 و2 
        /// الهبوط للرحلات العالمية والمحلية
        /// </summary>
        /// <param name="towerDataId"></param>
        /// <returns></returns>

        /// naji Landing Fees
        //public decimal LandingFees(int OfficerDataId,int FlightTypeD,int FlightTypeL)
        //{
        //    int towerDataId = Convert.ToInt32(db.OfficerDatas.FirstOrDefault(x => x.Id == OfficerDataId).TowerDataId);
        //    TowerData td = db.TowerDatas.FirstOrDefault(x => x.Id == towerDataId);
        //    AircraftRegistration rg = db.AircraftRegistrations.FirstOrDefault(x => x.Id == td.AircraftRegId);
        //    decimal maxTakoffWieght = Convert.ToDecimal(db.AircraftTypes.FirstOrDefault(x => x.Id == rg.Id).MaxTakoffWieght);

        //    decimal LastWieght =Convert.ToDecimal( db.LandingServiceFees.FirstOrDefault(x => x.CheckAppendWieght == true).WieghtFrom);

        //    if (maxTakoffWieght <= LastWieght) // When Airecraft Max takeoff Wieght is greater then Max Wieght in LandingServiceFees Setting Table
        //    {
        //        decimal AmountInDollar = Convert.ToDecimal(db.LandingServiceFees.FirstOrDefault(x => x.WieghtFrom <= maxTakoffWieght && x.WieghtTo > maxTakoffWieght).AmountInDollar);

        //        if (FlightTypeL == 1 || FlightTypeD==1) //International
        //        {
        //            FinalAmountInDollar = AmountInDollar;
        //        }
        //        else //Domestic
        //        {
        //            if (AmountInDollar % 2 == 0)
        //                FinalAmountInDollar = (AmountInDollar) / 2;
        //            else
        //                FinalAmountInDollar = (AmountInDollar / 2) + 1;
        //        }
        //    }
        //    else  // When Airecraft Max takeoff Wieght is less then Max Wieght in LandingServiceFees Setting Table
        //    {
        //        decimal BeforLastAmount = Convert.ToDecimal(db.LandingServiceFees.FirstOrDefault(x => LastWieght > x.WieghtFrom && LastWieght <= x.WieghtTo).AmountInDollar);
        //        decimal OverWieght = maxTakoffWieght - LastWieght;
        //        decimal LastAmount = Convert.ToDecimal(db.LandingServiceFees.SingleOrDefault(x => x.CheckAppendWieght == true).AmountInDollar);
        //        decimal TotalAmount = OverWieght * LastAmount;
        //        decimal IncreaseAmount = TotalAmount + BeforLastAmount;

        //        if (FlightTypeL == 1 || FlightTypeD==1) //International
        //        {
        //            FinalAmountInDollar = IncreaseAmount;
        //        }
        //        else //Domestic
        //        {
        //            if (IncreaseAmount % 2 == 0)
        //                FinalAmountInDollar = IncreaseAmount / 2;
        //            else
        //                FinalAmountInDollar = (IncreaseAmount / 2) + 1;
        //        }
        //    }

        //    return FinalAmountInDollar;
        //}

        public decimal LandingFees(decimal maxTakoffWieght, int FlightTypeL)
        {
            // int towerDataId = model.Id;
            /// TowerData td = db.TowerDatas.FirstOrDefault(x => x.Id == towerDataId);
            ///  AircraftRegistration rg = db.AircraftRegistrations.FirstOrDefault(x => x.Id == model.AircraftRegId);
            //  decimal maxTakoffWieght = Convert.ToDecimal(db.AircraftRegistrations.FirstOrDefault(x => x.Id == model.AircraftRegId).MaxTakoffWieght);

            decimal LastWieght = Convert.ToDecimal(db.LandingServiceFees.FirstOrDefault(x => x.CheckAppendWieght == true).WieghtFrom);

            if (maxTakoffWieght <= LastWieght) // When Airecraft Max takeoff Wieght is greater then Max Wieght in LandingServiceFees Setting Table
            {
                decimal AmountInDollar = Convert.ToDecimal(db.LandingServiceFees.FirstOrDefault(x => x.WieghtFrom <= maxTakoffWieght && x.WieghtTo > maxTakoffWieght).AmountInDollar);

                if (FlightTypeL == 1 /*|| model.FlightTypeD == 1*/) //International
                {
                    FinalAmountInDollar = AmountInDollar;
                }
                else //Domestic
                {
                    if (AmountInDollar % 2 == 0)
                        FinalAmountInDollar = (AmountInDollar) / 2;
                    else
                        FinalAmountInDollar = (AmountInDollar / 2) + 1;
                }
            }
            else  // When Airecraft Max takeoff Wieght is less then Max Wieght in LandingServiceFees Setting Table
            {
                decimal BeforLastAmount = Convert.ToDecimal(db.LandingServiceFees.FirstOrDefault(x => LastWieght > x.WieghtFrom && LastWieght <= x.WieghtTo).AmountInDollar);
                decimal OverWieght = maxTakoffWieght - LastWieght;
                decimal LastAmount = Convert.ToDecimal(db.LandingServiceFees.SingleOrDefault(x => x.CheckAppendWieght == true).AmountInDollar);
                decimal TotalAmount = OverWieght * LastAmount;
                decimal IncreaseAmount = TotalAmount + BeforLastAmount;

                if (FlightTypeL == 1 /*|| model.FlightTypeD == 1*/) //International
                {
                    FinalAmountInDollar = IncreaseAmount;
                }
                else //Domestic
                {
                    if (IncreaseAmount % 2 == 0)
                        FinalAmountInDollar = IncreaseAmount / 2;
                    else
                        FinalAmountInDollar = (IncreaseAmount / 2) + 1;
                }
            }

            return FinalAmountInDollar;
        }

















        public decimal LandingFees(int OfficerDataId)
        {
            int towerDataId = Convert.ToInt32(db.OfficerDatas.FirstOrDefault(x => x.Id == OfficerDataId).TowerDataId);
            TowerData td = db.TowerDatas.FirstOrDefault(x => x.Id == towerDataId);
            AircraftRegistration rg = db.AircraftRegistrations.FirstOrDefault(x => x.Id == td.AircraftRegId);
            decimal maxTakoffWieght = Convert.ToDecimal(db.AircraftRegistrations.FirstOrDefault(x => x.Id == rg.Id).MaxTakoffWieght);

            decimal LastWieght = Convert.ToDecimal(db.LandingServiceFees.FirstOrDefault(x => x.CheckAppendWieght == true).WieghtFrom);

            if (maxTakoffWieght <= LastWieght) // When Airecraft Max takeoff Wieght is greater then Max Wieght in LandingServiceFees Setting Table
            {
                decimal AmountInDollar = Convert.ToDecimal(db.LandingServiceFees.FirstOrDefault(x => x.WieghtFrom <= maxTakoffWieght && x.WieghtTo > maxTakoffWieght).AmountInDollar);

                if (td.FlightTypeL == 1 || td.FlightTypeD == 1) //International
                {
                    FinalAmountInDollar = AmountInDollar;
                }
                else //Domestic
                {
                    if (AmountInDollar % 2 == 0)
                        FinalAmountInDollar = (AmountInDollar) / 2;
                    else
                        FinalAmountInDollar = (AmountInDollar / 2) + 1;
                }
            }
            else  // When Airecraft Max takeoff Wieght is less then Max Wieght in LandingServiceFees Setting Table
            {
                decimal BeforLastAmount = Convert.ToDecimal(db.LandingServiceFees.FirstOrDefault(x => LastWieght > x.WieghtFrom && LastWieght <= x.WieghtTo).AmountInDollar);
                decimal OverWieght = maxTakoffWieght - LastWieght;
                decimal LastAmount = Convert.ToDecimal(db.LandingServiceFees.SingleOrDefault(x => x.CheckAppendWieght == true).AmountInDollar);
                decimal TotalAmount = OverWieght * LastAmount;
                decimal IncreaseAmount = TotalAmount + BeforLastAmount;

                if (td.FlightTypeL == 1 || td.FlightTypeD == 1) //International
                {
                    FinalAmountInDollar = IncreaseAmount;
                }
                else //Domestic
                {
                    if (IncreaseAmount % 2 == 0)
                        FinalAmountInDollar = IncreaseAmount / 2;
                    else
                        FinalAmountInDollar = (IncreaseAmount / 2) + 1;
                }
            }

            return FinalAmountInDollar;
        }

        public decimal ParkingFeesOLD(int OfficerDataId, DateTime LandingDate, DateTime TakeOffDate, TimeSpan ATA, TimeSpan ATD, bool IsLocalCompany)
        {
            int towerDataId = Convert.ToInt32(db.OfficerDatas.FirstOrDefault(x => x.Id == OfficerDataId).TowerDataId);
            TowerData td = db.TowerDatas.FirstOrDefault(x => x.Id == towerDataId);

            //bool IsLocalCompany = td.AirLine.IsLocalCompany; 
            if (!IsLocalCompany)// نختبر هل شركة الطيران مركزها السودان ام لا مثلا تاركو وبدر
            {
                AircraftRegistration rg = db.AircraftRegistrations.FirstOrDefault(x => x.Id == td.AircraftRegId);
                decimal maxTakoffWieght = Convert.ToDecimal(db.AircraftRegistrations.FirstOrDefault(x => x.Id == rg.Id).MaxTakoffWieght);

                decimal AmountInDollar = Convert.ToDecimal(db.ParkingServiceFees.FirstOrDefault(x => maxTakoffWieght >= x.WieghtFrom && maxTakoffWieght < x.WieghtTo).AmountInDollar);

                //DateTime LandingDate = Convert.ToDateTime(td.LandingDate);
                //DateTime TakeOffDate = Convert.ToDateTime(td.TakeOffDate);

                //TimeSpan ATA = TimeSpan.Parse(td.ATA.ToString()); // الزمن الفعلي للوصول
                //TimeSpan ATD = TimeSpan.Parse(td.ATD.ToString()); // الزمن الفعلي للمغادرة
                int ATAHours = ATA.Hours; // جلب الساعة عند الوصول                    
                int ATDHours = ATD.Hours; // جلب الساعة عند المغادرة
                int ATDMinutes = ATD.Minutes; // جلب الدقائق عند المغادرة
                int ATAMinutes = ATA.Minutes; // جلب الدقائق عند الوصول

                int FinalTotalHours = 0;
                int TotalHours = 0;
                int TotalMinutes = 0;

                // When LandingDate = TakeOffDate (Same Date) في حالة انه الهبوط والاقلاع كان في نفس اليوم
                if ((LandingDate.Year == TakeOffDate.Year) && (LandingDate.Month == TakeOffDate.Month) && (LandingDate.Day == TakeOffDate.Day))
                {
                    //حسب اللائحة أي دقيقة زيادة تحسب ساعة كاملة                    
                    if (ATAMinutes > 0)
                    {
                        TotalMinutes = TotalMinutes + (60 - ATAMinutes);
                    }
                    //حسب اللائحة أي دقيقة زيادة تحسب ساعة كاملة                    
                    if (ATDMinutes > 0)
                    {
                        TotalMinutes = TotalMinutes + (60 - ATDMinutes);
                    }
                    // هنا عملنا احتياط انه النظام يعمل بي نظام ال 2 ساعة وليس بي نظام ال 24 ساعة
                    if (ATDHours != ATAHours)
                    {
                        TotalHours = Math.Abs(ATAHours - ATDHours - 1); // حساب فرق الدقائق بين الزمنين  بالساعات
                    }
                    else // هنا 8 ص   و  8 م مثلا
                    {
                        TotalHours = 12;
                    }

                    //نختبر هل عدد الساعات الفعلية للوقوف اكبر من ساعتين ام لا
                    if (TotalHours <= 2 && TotalMinutes <= 120)
                    {
                        //في حالة انه عدد الساعات الفعلي للوقوف اقل من او يساوي ساعتين هنا حسب اللائحة مافي حسابات او مافي رسوم وقوف
                        FinalAmountInDollar = 0;
                    }
                    else
                    {
                        //في حالة انه عدد الساعات الفعلي للوقوف اقل من او يساوي ساعتين هنا حسب اللائحة مافي حسابات او مافي رسوم وقوف               
                        if (TotalHours <= 6)
                        {
                            //يعني عليهو حساب مرة واحدة
                            FinalTotalHours = 1;
                        }
                        else
                        {
                            // عليهو حساب اكتر من مرة
                            if (TotalHours % 6 == 0)
                            {
                                FinalTotalHours = TotalHours / 6;
                            }
                            else
                            {
                                FinalTotalHours = (TotalHours / 6) + 1;
                            }
                        }
                        // القيمة الفعلية النهائية لرسوم الوقوف
                        FinalAmountInDollar = Convert.ToInt32(AmountInDollar * FinalTotalHours);
                    }
                }
                // When LandingDate != TakeOffDate (Different Date's)  اذا كان الهبوط و الاقلاع في تاريخين مختلفين
                else if ((TakeOffDate.Month != LandingDate.Month) || (TakeOffDate.Day != LandingDate.Day) || (TakeOffDate.Year != LandingDate.Year))
                {
                    // نحسب فرق الشهور بين تاريخ الوصول وتاريخ الاقلاع                    
                    int TotalMonths = TakeOffDate.Month - LandingDate.Month;

                    //في حالة الفرق في عدد الشهور = 1 نحسب عدد الايام في كل شهر
                    int TotalDays = 0;
                    if (TotalMonths == 1)// في حالة انه الهبوط في شهر والاقلاع في الشهر الذي يليه 
                    {
                        TotalDays = (DateTime.DaysInMonth(LandingDate.Year, LandingDate.Month) - LandingDate.Day) + TakeOffDate.Day - 2;
                    }
                    //في حالة أنو الفرق في عدد الشهور أكبر من 1 نحسب عدد الايام في كل شهر
                    else if (TotalMonths > 1)
                    {
                        /*
                            هنا نحسب عدد الايام في شهر الهبوط + عدد الايام في شهر الاقلاع + عدد الايام في الشهور بين التاريخين التي تم فيها الوقوف (إن وجدت) ههههههههه
                        */
                        TotalDays = (DateTime.DaysInMonth(LandingDate.Year, LandingDate.Month) - LandingDate.Day) + TakeOffDate.Day - 2; //عدد أيام في شهر الهبوط + عدد الايام في شهر الاقلاع

                        // الحلقة دي لحساب عدد الايام في الشهور التي تم فيها الوقوف 
                        for (int i = 1; i < TotalMonths; i++)
                        {
                            TotalDays = TotalDays + DateTime.DaysInMonth(LandingDate.Year, LandingDate.Month + i);
                        }
                    }
                    else // في حالة انه الوقوف كان في نفس الشهر بس لكن لاكثر من يوم
                    {
                        TotalDays = (LandingDate.Day - TakeOffDate.Day) - 2;
                    }
                    /*
                        قد يكون فرق الايام 0 أو أكبر من 0 أوأقل من 0

                        يكون أقل من صفر في حالة إنه فرق الهبوط من الاقلاع أقل من 2 يوم
                        يكون صفر في حالة إنه فرق الهبوط من الاقلاع = 2 يوم                        
                        يكون أكبر من صفر في حالة إنه فرق الهبوط من الاقلاع أكبر من 2 يوم
                    */
                    if (TotalDays < 0) // في حالة إنه فرق الهبوط من الاقلاع أقل من 2 يوم
                    {
                        /******** NEW *********/
                        //حسب اللائحة أي دقيقة زيادة تحسب ساعة كاملة                    
                        if (ATDMinutes > 0)
                        {
                            TotalMinutes = TotalMinutes + (60 - ATDMinutes);
                        }
                        //حسب اللائحة أي دقيقة زيادة تحسب ساعة كاملة                    
                        if (ATAMinutes > 0)
                        {
                            TotalMinutes = TotalMinutes + (60 - ATAMinutes);
                        }
                        // هنا عملنا احتياط انه النظام يعمل بي نظام ال 2 ساعة وليس بي نظام ال 24 ساعة
                        if (ATDHours != ATAHours)
                        {
                            TotalHours = Math.Abs(ATAHours - ATDHours + 1); // حساب فرق الدقائق بين الزمنين  بالساعات
                        }
                        else // هنا 8 ص   و  8 م مثلا
                        {
                            TotalHours = 12;
                        }
                        /******** END NEW *********/
                        TotalHours = Math.Abs(24 - ATAHours + 1 + ATDHours);
                    }
                    else if (TotalDays == 0) // في حالة إنه فرق الهبوط من الاقلاع = 2 يوم
                    {
                        /******** NEW *********/
                        //حسب اللائحة أي دقيقة زيادة تحسب ساعة كاملة                    
                        if (ATDMinutes > 0)
                        {
                            TotalMinutes = TotalMinutes + (60 - ATDMinutes);
                        }
                        //حسب اللائحة أي دقيقة زيادة تحسب ساعة كاملة                    
                        if (ATAMinutes > 0)
                        {
                            TotalMinutes = TotalMinutes + (60 - ATAMinutes);
                        }
                        // هنا عملنا احتياط انه النظام يعمل بي نظام ال 2 ساعة وليس بي نظام ال 24 ساعة
                        if (ATDHours != ATAHours)
                        {
                            TotalHours = Math.Abs(ATAHours - ATDHours); // حساب فرق الدقائق بين الزمنين  بالساعات
                        }
                        else // هنا 8 ص   و  8 م مثلا
                        {
                            TotalHours = 12;
                        }
                        /********END NEW *********/

                        TotalHours = 24 + Math.Abs(24 - ATAHours + ATDHours);
                    }
                    else // في حالة إنه فرق الهبوط من الاقلاع أكبر من 2 يوم
                    {
                        TotalHours = (TotalDays * 24) + Math.Abs(24 - ATAHours + ATDHours);
                    }

                    /* //حسب اللائحة أي دقيقة زيادة في زمن الاقلاع تحسب ساعة كاملة                    
                     if (ATDMinutes > 0)
                     {
                         TotalHours = TotalHours + 1;
                     }*/





















                    //نختبر هل عدد الساعات الفعلية للوقوف اكبر من ساعتين ام لا
                    if (TotalHours <= 2 && TotalMinutes <= 120)
                    {
                        //في حالة انه عدد الساعات الفعلي للوقوف اقل من او يساوي ساعتين هنا حسب اللائحة مافي حسابات او مافي رسوم وقوف
                        FinalAmountInDollar = 0;
                    }
                    else
                    {
                        //في حالة انه عدد الساعات الفعلي للوقوف اقل من او يساوي ساعتين هنا حسب اللائحة مافي حسابات او مافي رسوم وقوف               
                        if (TotalHours <= 6)
                        {
                            //يعني عليهو حساب مرة واحدة
                            FinalTotalHours = 1;
                        }
                        else
                        {
                            // عليهو حساب اكتر من مرة
                            if (TotalHours % 6 == 0)
                            {
                                FinalTotalHours = TotalHours / 6;
                            }
                            else
                            {
                                FinalTotalHours = (TotalHours / 6) + 1;
                            }
                        }
                    }
                    // القيمة الفعلية النهائية لرسوم الوقوف
                    FinalAmountInDollar = AmountInDollar * FinalTotalHours;
                }
            }
            else //عندما تكون الشركية محلية وليس عليها رسوم وقوف
            {
                FinalAmountInDollar = 0;
            }

            return FinalAmountInDollar;
        }

        public decimal ParkingFees1(int OfficerDataId, DateTime LandingDate, DateTime TakeOffDate, TimeSpan ATA, TimeSpan ATD, bool IsLocalCompany)
        {
            int towerDataId = Convert.ToInt32(db.OfficerDatas.FirstOrDefault(x => x.Id == OfficerDataId).TowerDataId);
            TowerData td = db.TowerDatas.FirstOrDefault(x => x.Id == towerDataId);
            //TakeOffDate = new DateTime(2022, 12, 03);
            //LandingDate = new DateTime(2022, 11, 29);
            //bool IsLocalCompany = td.AirLine.IsLocalCompany; 
            if (!IsLocalCompany)// نختبر هل شركة الطيران مركزها السودان ام لا مثلا تاركو وبدر
            {
                AircraftRegistration rg = db.AircraftRegistrations.FirstOrDefault(x => x.Id == td.AircraftRegId);
                decimal maxTakoffWieght = Convert.ToDecimal(db.AircraftRegistrations.FirstOrDefault(x => x.Id == rg.Id).MaxTakoffWieght);

                decimal AmountInDollar = Convert.ToDecimal(db.ParkingServiceFees.FirstOrDefault(x => maxTakoffWieght >= x.WieghtFrom && maxTakoffWieght < x.WieghtTo).AmountInDollar);

                //DateTime LandingDate = Convert.ToDateTime(td.LandingDate);
                //DateTime TakeOffDate = Convert.ToDateTime(td.TakeOffDate);

                //TimeSpan ATA = TimeSpan.Parse(td.ATA.ToString()); // الزمن الفعلي للوصول
                //TimeSpan ATD = TimeSpan.Parse(td.ATD.ToString()); // الزمن الفعلي للمغادرة
                int ATAHours = ATA.Hours; // جلب الساعة عند الوصول                    
                int ATDHours = ATD.Hours; // جلب الساعة عند المغادرة
                int ATDMinutes = ATD.Minutes; // جلب الدقائق عند المغادرة
                int ATAMinutes = ATA.Minutes; // جلب الدقائق عند الوصول

                int FinalTotalHours = 0;
                int TotalHours = 0;
                int TotalMinutes = 0;
                int TotalDays = 0;

                // When LandingDate = TakeOffDate (Same Date) في حالة انه الهبوط والاقلاع كان في نفس اليوم
                if ((LandingDate.Year == TakeOffDate.Year) && (LandingDate.Month == TakeOffDate.Month) && (LandingDate.Day == TakeOffDate.Day))
                {
                    int t = GETFIRST2HOURSE(ATAHours, ATDHours, ATAMinutes, ATDMinutes, true);

                    //نختبر هل عدد الساعات الفعلية للوقوف اكبر من ساعتين ام لا
                    if (t <= 120)
                    {
                        //في حالة انه عدد الساعات الفعلي للوقوف اقل من او يساوي ساعتين هنا حسب اللائحة مافي حسابات او مافي رسوم وقوف
                        FinalAmountInDollar = 0;
                    }
                    else
                    {
                        //نطرح 120 عشان نطلع ال 2 ساعة المجتنية من الحسابات
                        t = t - 120;
                        // عليهو حساب اكتر من مرة
                        if (t % 60 == 0)
                        {
                            TotalHours = t / 60;
                        }
                        else
                        {
                            TotalHours = (t / 60) + 1;
                        }

                        //في حالة انه عدد الساعات الفعلي للوقوف اقل من او يساوي ساعتين هنا حسب اللائحة مافي حسابات او مافي رسوم وقوف               
                        if (TotalHours <= 6)
                        {
                            //يعني عليهو حساب مرة واحدة
                            FinalTotalHours = 1;
                        }
                        else
                        {
                            // عليهو حساب اكتر من مرة
                            if (TotalHours % 6 == 0)
                            {
                                FinalTotalHours = TotalHours / 6;
                            }
                            else
                            {
                                FinalTotalHours = (TotalHours / 6) + 1;
                            }
                        }
                        // القيمة الفعلية النهائية لرسوم الوقوف
                        FinalAmountInDollar = Convert.ToInt32(AmountInDollar * FinalTotalHours);
                    }
                }
                // When LandingDate != TakeOffDate (Different Date's)  اذا كان الهبوط و الاقلاع في تاريخين مختلفين
                else if ((TakeOffDate.Month != LandingDate.Month) || (TakeOffDate.Day != LandingDate.Day) || (TakeOffDate.Year != LandingDate.Year))
                {
                    int TotalMonths = 0;
                    /************** moh **********/
                    /*if (TakeOffDate.Year != LandingDate.Year)
                    {
                        if ((LandingDate.Year - TakeOffDate.Year) == 1)
                        {
                            TotalMonths = (12 - LandingDate.Month + TakeOffDate.Month) - 1;
                            //TotalDays = TotalMonths * 30;
                        }
                        else if ((LandingDate.Year - TakeOffDate.Year) > 1)
                        {
                            int TotalYears = TakeOffDate.Year - LandingDate.Year -1;
                            TotalMonths = TotalYears * 12;
                            //TotalDays = TotalMonths * 30;
                        }
                    }
                    else
                    {
                        /********************* end moh***********************
                        // نحسب فرق الشهور بين تاريخ الوصول وتاريخ الاقلاع                    
                        TotalMonths = TakeOffDate.Month - LandingDate.Month;
                    }*/
                    // نحسب فرق الشهور بين تاريخ الوصول وتاريخ الاقلاع                    
                    TotalMonths = TakeOffDate.Month - LandingDate.Month;
                    int t = GETFIRST2HOURSE(ATAHours, ATDHours, ATAMinutes, ATDMinutes, false);
                    //في حالة الفرق في عدد الشهور = 1 نحسب عدد الايام في كل شهر
                    //int TotalDays = 0;
                    if (TotalMonths == 1)// في حالة انه الهبوط في شهر والاقلاع في الشهر الذي يليه 
                    {
                        //int t = GETFIRST2HOURSE(ATAHours,ATDHours,ATAMinutes,ATDMinutes, false);                        
                        TotalDays = (DateTime.DaysInMonth(LandingDate.Year, LandingDate.Month) - LandingDate.Day) + TakeOffDate.Day - 1;
                    }
                    //في حالة أنو الفرق في عدد الشهور أكبر من 1 نحسب عدد الايام في كل شهر
                    else if (TotalMonths > 1)
                    {
                        /*
                            هنا نحسب عدد الايام في شهر الهبوط + عدد الايام في شهر الاقلاع + عدد الايام في الشهور بين التاريخين التي تم فيها الوقوف (إن وجدت) ههههههههه
                        */
                        TotalDays = (DateTime.DaysInMonth(LandingDate.Year, LandingDate.Month) - LandingDate.Day) + TakeOffDate.Day - 1; //عدد أيام في شهر الهبوط + عدد الايام في شهر الاقلاع

                        // الحلقة دي لحساب عدد الايام في الشهور التي تم فيها الوقوف 
                        for (int i = 1; i < TotalMonths; i++)
                        {
                            TotalDays = TotalDays + DateTime.DaysInMonth(LandingDate.Year, LandingDate.Month + i);
                        }
                    }
                    else // في حالة انه الوقوف كان في نفس الشهر بس لكن لاكثر من يوم
                    {
                        TotalDays = (TakeOffDate.Day - LandingDate.Day) - 1;
                    }
                    /*
                        قد يكون فرق الايام 0 أو أكبر من 0 أوأقل من 0

                        (يكون أقل من صفر في حالة إنه فرق الهبوط من الاقلاع أقل من 2 يوم(اليوم واليوم ال بعده
                        يكون صفر في حالة إنه فرق الهبوط من الاقلاع = 2 يوم                        
                        يكون أكبر من صفر في حالة إنه فرق الهبوط من الاقلاع أكبر من 2 يوم
                    */




                    /////////////////////////////////////////////////////////////////////////////////////////////////////////
                    if (TotalDays < 0) // في حالة إنه فرق الهبوط من الاقلاع أقل من 2 يوم
                    {
                        TotalMinutes = GETFIRST2HOURSE(ATAHours, ATDHours, ATAMinutes, ATDMinutes, false);
                    }
                    /* 
                    في حالة إنه فرق الهبوط من الاقلاع = 2 يوم
                    مثلا الهبوط يوم 02/02/2022
                    الاقلاع يوم 04/02/2022
                    */

                    else if (TotalDays == 0)
                    {
                        TotalHours = 24 + Math.Abs(24 - ATAHours + ATDHours);
                        TotalMinutes = TotalHours * 60;

                    }
                    else // في حالة إنه فرق الهبوط من الاقلاع أكبر من 2 يوم
                    {
                        TotalMinutes = GETFIRST2HOURSE(ATAHours, ATDHours, ATAMinutes, ATDMinutes, false);
                        TotalHours = (TotalDays * 24);
                        TotalMinutes = TotalMinutes + TotalHours * 60;
                    }
                    //نختبر هل عدد الساعات الفعلية للوقوف اكبر من ساعتين ام لا
                    if (TotalMinutes <= 120)
                    {
                        //في حالة انه عدد الساعات الفعلي للوقوف اقل من او يساوي ساعتين هنا حسب اللائحة مافي حسابات او مافي رسوم وقوف
                        FinalAmountInDollar = 0;
                    }
                    else
                    {
                        TotalMinutes = TotalMinutes - 120;
                        if (TotalMinutes % 60 == 0)
                        {
                            TotalHours = TotalMinutes / 60;
                        }
                        else
                        {
                            TotalHours = (TotalMinutes / 60) + 1;
                        }
                        //في حالة انه عدد الساعات الفعلي للوقوف اقل من او يساوي ساعتين هنا حسب اللائحة مافي حسابات او مافي رسوم وقوف               
                        if (TotalHours <= 6)
                        {
                            //يعني عليهو حساب مرة واحدة
                            FinalTotalHours = 1;
                        }
                        else
                        {
                            // عليهو حساب اكتر من مرة
                            if (TotalHours % 6 == 0)
                            {
                                FinalTotalHours = TotalHours / 6;
                            }
                            else
                            {
                                FinalTotalHours = (TotalHours / 6) + 1;
                            }
                        }
                    }
                    // القيمة الفعلية النهائية لرسوم الوقوف
                    FinalAmountInDollar = AmountInDollar * FinalTotalHours;
                }
            }
            else //عندما تكون الشركية محلية وليس عليها رسوم وقوف
            {
                FinalAmountInDollar = 0;
            }

            return FinalAmountInDollar;
        }
        public decimal ParkingFeesNew(int OfficerDataId, DateTime LandingDate, DateTime TakeOffDate, TimeSpan ATA, TimeSpan ATD, bool IsLocalCompany)
        {
            int towerDataId = Convert.ToInt32(db.OfficerDatas.FirstOrDefault(x => x.Id == OfficerDataId).TowerDataId);
            TowerData td = db.TowerDatas.FirstOrDefault(x => x.Id == towerDataId);

            //bool IsLocalCompany = td.AirLine.IsLocalCompany; 
            if (!IsLocalCompany)// نختبر هل شركة الطيران مركزها السودان ام لا مثلا تاركو وبدر
            {
                AircraftRegistration rg = db.AircraftRegistrations.FirstOrDefault(x => x.Id == td.AircraftRegId);
                decimal maxTakoffWieght = Convert.ToDecimal(db.AircraftRegistrations.FirstOrDefault(x => x.Id == rg.Id).MaxTakoffWieght);

                decimal AmountInDollar = Convert.ToDecimal(db.ParkingServiceFees.FirstOrDefault(x => maxTakoffWieght >= x.WieghtFrom && maxTakoffWieght < x.WieghtTo).AmountInDollar);

                //DateTime LandingDate = Convert.ToDateTime(td.LandingDate);
                //DateTime TakeOffDate = Convert.ToDateTime(td.TakeOffDate);

                //TimeSpan ATA = TimeSpan.Parse(td.ATA.ToString()); // الزمن الفعلي للوصول
                //TimeSpan ATD = TimeSpan.Parse(td.ATD.ToString()); // الزمن الفعلي للمغادرة
                int ATAHours = ATA.Hours; // جلب الساعة عند الوصول                    
                int ATDHours = ATD.Hours; // جلب الساعة عند المغادرة
                int ATDMinutes = ATD.Minutes; // جلب الدقائق عند المغادرة
                int ATAMinutes = ATA.Minutes; // جلب الدقائق عند الوصول

                int FinalTotalHours = 0;
                int TotalHours = 0;
                int TotalMinutes = 0;

                // When LandingDate = TakeOffDate (Same Date) في حالة انه الهبوط والاقلاع كان في نفس اليوم
                if ((LandingDate.Year == TakeOffDate.Year) && (LandingDate.Month == TakeOffDate.Month) && (LandingDate.Day == TakeOffDate.Day))
                {
                    //حسب اللائحة أي دقيقة زيادة تحسب ساعة كاملة                    
                    if (ATAMinutes > 0)
                    {
                        TotalMinutes = TotalMinutes + (60 - ATAMinutes);
                    }
                    //حسب اللائحة أي دقيقة زيادة تحسب ساعة كاملة                    
                    if (ATDMinutes > 0)
                    {
                        TotalMinutes = TotalMinutes + (60 - ATDMinutes);
                    }
                    // هنا عملنا احتياط انه النظام يعمل بي نظام ال 2 ساعة وليس بي نظام ال 24 ساعة
                    if (ATDHours != ATAHours)
                    {
                        TotalHours = Math.Abs(ATAHours - ATDHours - 1); // حساب فرق الدقائق بين الزمنين  بالساعات
                    }
                    else // هنا 8 ص   و  8 م مثلا
                    {
                        TotalHours = 12;
                    }

                    //نختبر هل عدد الساعات الفعلية للوقوف اكبر من ساعتين ام لا
                    if (TotalHours <= 2 && TotalMinutes <= 120)
                    {
                        //في حالة انه عدد الساعات الفعلي للوقوف اقل من او يساوي ساعتين هنا حسب اللائحة مافي حسابات او مافي رسوم وقوف
                        FinalAmountInDollar = 0;
                    }
                    else
                    {
                        //في حالة انه عدد الساعات الفعلي للوقوف اقل من او يساوي ساعتين هنا حسب اللائحة مافي حسابات او مافي رسوم وقوف               
                        if (TotalHours <= 6)
                        {
                            //يعني عليهو حساب مرة واحدة
                            FinalTotalHours = 1;
                        }
                        else
                        {
                            // عليهو حساب اكتر من مرة
                            if (TotalHours % 6 == 0)
                            {
                                FinalTotalHours = TotalHours / 6;
                            }
                            else
                            {
                                FinalTotalHours = (TotalHours / 6) + 1;
                            }
                        }
                        // القيمة الفعلية النهائية لرسوم الوقوف
                        FinalAmountInDollar = Convert.ToInt32(AmountInDollar * FinalTotalHours);
                    }
                }
                // When LandingDate != TakeOffDate (Different Date's)  اذا كان الهبوط و الاقلاع في تاريخين مختلفين
                else if ((TakeOffDate.Month != LandingDate.Month) || (TakeOffDate.Day != LandingDate.Day) || (TakeOffDate.Year != LandingDate.Year))
                {
                    // نحسب فرق الشهور بين تاريخ الوصول وتاريخ الاقلاع                    
                    int TotalMonths = TakeOffDate.Month - LandingDate.Month;

                    //في حالة الفرق في عدد الشهور = 1 نحسب عدد الايام في كل شهر
                    int TotalDays = 0;
                    if (TotalMonths == 1)// في حالة انه الهبوط في شهر والاقلاع في الشهر الذي يليه 
                    {
                        TotalDays = (DateTime.DaysInMonth(LandingDate.Year, LandingDate.Month) - LandingDate.Day) + TakeOffDate.Day - 2;
                    }
                    //في حالة أنو الفرق في عدد الشهور أكبر من 1 نحسب عدد الايام في كل شهر
                    else if (TotalMonths > 1)
                    {
                        /*
                            هنا نحسب عدد الايام في شهر الهبوط + عدد الايام في شهر الاقلاع + عدد الايام في الشهور بين التاريخين التي تم فيها الوقوف (إن وجدت) ههههههههه
                        */
                        TotalDays = (DateTime.DaysInMonth(LandingDate.Year, LandingDate.Month) - LandingDate.Day) + TakeOffDate.Day - 2; //عدد أيام في شهر الهبوط + عدد الايام في شهر الاقلاع

                        // الحلقة دي لحساب عدد الايام في الشهور التي تم فيها الوقوف 
                        for (int i = 1; i < TotalMonths; i++)
                        {
                            TotalDays = TotalDays + DateTime.DaysInMonth(LandingDate.Year, LandingDate.Month + i);
                        }
                    }
                    else // في حالة انه الوقوف كان في نفس الشهر بس لكن لاكثر من يوم
                    {
                        TotalDays = (LandingDate.Day - TakeOffDate.Day) - 2;
                    }
                    /*
                        قد يكون فرق الايام 0 أو أكبر من 0 أوأقل من 0

                        يكون أقل من صفر في حالة إنه فرق الهبوط من الاقلاع أقل من 2 يوم
                        يكون صفر في حالة إنه فرق الهبوط من الاقلاع = 2 يوم                        
                        يكون أكبر من صفر في حالة إنه فرق الهبوط من الاقلاع أكبر من 2 يوم
                    */
                    if (TotalDays < 0) // في حالة إنه فرق الهبوط من الاقلاع أقل من 2 يوم
                    {
                        /******** NEW *********/
                        //حسب اللائحة أي دقيقة زيادة تحسب ساعة كاملة                    
                        if (ATDMinutes > 0)
                        {
                            TotalMinutes = TotalMinutes + (60 - ATDMinutes);
                        }
                        //حسب اللائحة أي دقيقة زيادة تحسب ساعة كاملة                    
                        if (ATAMinutes > 0)
                        {
                            TotalMinutes = TotalMinutes + (60 - ATAMinutes);
                        }
                        // هنا عملنا احتياط انه النظام يعمل بي نظام ال 2 ساعة وليس بي نظام ال 24 ساعة
                        if (ATDHours != ATAHours)
                        {
                            TotalHours = Math.Abs(ATAHours - ATDHours + 1); // حساب فرق الدقائق بين الزمنين  بالساعات
                        }
                        else // هنا 8 ص   و  8 م مثلا
                        {
                            TotalHours = 12;
                        }
                        /******** END NEW *********/
                        TotalHours = Math.Abs(24 - ATAHours + 1 + ATDHours);
                    }
                    else if (TotalDays == 0) // في حالة إنه فرق الهبوط من الاقلاع = 2 يوم
                    {
                        /******** NEW *********/
                        //حسب اللائحة أي دقيقة زيادة تحسب ساعة كاملة                    
                        if (ATDMinutes > 0)
                        {
                            TotalMinutes = TotalMinutes + (60 - ATDMinutes);
                        }
                        //حسب اللائحة أي دقيقة زيادة تحسب ساعة كاملة                    
                        if (ATAMinutes > 0)
                        {
                            TotalMinutes = TotalMinutes + (60 - ATAMinutes);
                        }
                        // هنا عملنا احتياط انه النظام يعمل بي نظام ال 2 ساعة وليس بي نظام ال 24 ساعة
                        if (ATDHours != ATAHours)
                        {
                            TotalHours = Math.Abs(ATAHours - ATDHours); // حساب فرق الدقائق بين الزمنين  بالساعات
                        }
                        else // هنا 8 ص   و  8 م مثلا
                        {
                            TotalHours = 12;
                        }
                        /********END NEW *********/

                        TotalHours = 24 + Math.Abs(24 - ATAHours + ATDHours);
                    }
                    else // في حالة إنه فرق الهبوط من الاقلاع أكبر من 2 يوم
                    {
                        TotalHours = (TotalDays * 24) + Math.Abs(24 - ATAHours + ATDHours);
                    }

                    /* //حسب اللائحة أي دقيقة زيادة في زمن الاقلاع تحسب ساعة كاملة                    
                     if (ATDMinutes > 0)
                     {
                         TotalHours = TotalHours + 1;
                     }*/





















                    //نختبر هل عدد الساعات الفعلية للوقوف اكبر من ساعتين ام لا
                    if (TotalHours <= 2 && TotalMinutes <= 120)
                    {
                        //في حالة انه عدد الساعات الفعلي للوقوف اقل من او يساوي ساعتين هنا حسب اللائحة مافي حسابات او مافي رسوم وقوف
                        FinalAmountInDollar = 0;
                    }
                    else
                    {
                        //في حالة انه عدد الساعات الفعلي للوقوف اقل من او يساوي ساعتين هنا حسب اللائحة مافي حسابات او مافي رسوم وقوف               
                        if (TotalHours <= 6)
                        {
                            //يعني عليهو حساب مرة واحدة
                            FinalTotalHours = 1;
                        }
                        else
                        {
                            // عليهو حساب اكتر من مرة
                            if (TotalHours % 6 == 0)
                            {
                                FinalTotalHours = TotalHours / 6;
                            }
                            else
                            {
                                FinalTotalHours = (TotalHours / 6) + 1;
                            }
                        }
                    }
                    // القيمة الفعلية النهائية لرسوم الوقوف
                    FinalAmountInDollar = AmountInDollar * FinalTotalHours;
                }
            }
            else //عندما تكون الشركية محلية وليس عليها رسوم وقوف
            {
                FinalAmountInDollar = 0;
            }

            return FinalAmountInDollar;
        }
        public decimal ParkingFees(int OfficerDataId, DateTime LandingDate, DateTime TakeOffDate, TimeSpan ATA, TimeSpan ATD, bool IsLocalCompany)
        {
            int ATAh = ATA.Hours;
            int ATDh = ATD.Hours;
            int ATAm = ATA.Minutes;
            int ATDm = ATD.Minutes;
            DateTime dL = new DateTime(LandingDate.Year, LandingDate.Month, LandingDate.Day, ATAh, ATAm, 0);
            DateTime dT = new DateTime(TakeOffDate.Year, TakeOffDate.Month, TakeOffDate.Day, ATDh, ATDm, 0);

            TimeSpan ts = dT - dL;

            int towerDataId = Convert.ToInt32(db.OfficerDatas.FirstOrDefault(x => x.Id == OfficerDataId).TowerDataId);
            TowerData td = db.TowerDatas.FirstOrDefault(x => x.Id == towerDataId);

            //bool IsLocalCompany = td.AirLine.IsLocalCompany; 
            if (!IsLocalCompany)// نختبر هل شركة الطيران مركزها السودان ام لا مثلا تاركو وبدر
            {
                AircraftRegistration rg = db.AircraftRegistrations.FirstOrDefault(x => x.Id == td.AircraftRegId);
                decimal maxTakoffWieght = Convert.ToDecimal(db.AircraftRegistrations.FirstOrDefault(x => x.Id == rg.Id).MaxTakoffWieght);

                decimal AmountInDollar = Convert.ToDecimal(db.ParkingServiceFees.FirstOrDefault(x => maxTakoffWieght >= x.WieghtFrom && maxTakoffWieght < x.WieghtTo).AmountInDollar);

                if (ts.TotalMinutes > 120)
                {
                    int TotalHourse = 0;
                    int FinalHourse = 0;
                    if (((ts.TotalMinutes) - 120) % 60 == 0)
                    {
                        TotalHourse = Convert.ToInt32(((ts.TotalMinutes) - 120) / 60);
                    }
                    else
                    {
                        TotalHourse = Convert.ToInt32(((ts.TotalMinutes) - 120) / 60) + 1;
                    }

                    if (TotalHourse % 6 == 0)
                    {
                        FinalHourse = TotalHourse / 6;
                    }
                    else
                    {
                        FinalHourse = (TotalHourse / 6) + 1;
                    }
                    FinalAmountInDollar = AmountInDollar * FinalHourse;
                }
                else
                {
                    FinalAmountInDollar = 0;
                }
            }
            else
            {
                FinalAmountInDollar = 0;
            }
            return FinalAmountInDollar;
        }
        private int GETFIRST2HOURSE(int ATAHoures, int ATDHoures, int ATAMin, int ATDMin, bool inSameDay)
        {
            int TotalHourse = 0;
            int TotalMinuts = 0;
            if (inSameDay)
            {
                if (ATAMin == 0 && ATDMin == 0)
                {
                    //Done
                    //TotalHourse = ATAHoures < ATDHoures ? (ATDHoures - ATAHoures) : (ATAHoures - ATDHoures);
                    TotalHourse = ATDHoures - ATAHoures;
                    TotalMinuts = TotalHourse * 60;
                }
                else
                {
                    TotalHourse = (ATDHoures - ATAHoures);
                    //TotalHourse = ATAHoures > ATDHoures ? (ATAHoures - ATDHoures) - 1 : (ATDHoures - ATAHoures) - 1;                    
                    if (TotalHourse == 0)
                    {
                        TotalMinuts = 60 - ATAMin + ATDMin;
                    }
                    else
                    {
                        TotalMinuts = (TotalHourse - 1) * 60;
                        TotalMinuts = TotalMinuts + ((60 - ATAMin) + ATDMin);
                    }
                }
            }
            else
            { //In different days
                /*
                    هنا جبنا فقط عدد الدقائق المتبقية في يوم الهبوط وعدد الدقائق في ويوم الاقلاع
                */
                TotalHourse = (24 - ATAHoures + ATDHoures) - 1;
                if (ATAMin == 0 && ATDMin == 0)
                {
                    TotalMinuts = TotalHourse * 60;
                }
                else
                {
                    TotalMinuts = TotalHourse * 60;
                    TotalMinuts = TotalMinuts + (60 - ATAMin + ATDMin);
                }
            }
            return TotalMinuts;
        }
        private int GETFIRST2HOURSE(int ATAHoures, int ATDHoures, int ATAMin, int ATDMin)
        {
            int TotalHourse = ATAHoures - ATDHoures;
            int TotalMinuts = ATAMin - ATDMin;
            if (ATAMin == 0 && ATDMin == 0)
            {
                TotalHourse = ATAHoures > ATDHoures ? (ATAHoures - ATDHoures) - 1 : (ATDHoures - ATAHoures) - 1;

                TotalMinuts = TotalHourse * 60;
            }
            else
            {
                TotalHourse = ATAHoures > ATDHoures ? (ATAHoures - ATDHoures) - 1 : (ATDHoures - ATAHoures) - 1;
                if (TotalHourse == 1)
                {
                    TotalMinuts = 60 - ATAMin + ATDMin;
                }
                else
                {
                    TotalMinuts = TotalHourse * 60;
                    TotalMinuts = TotalMinuts + ((60 - ATAMin) + ATDMin);
                }

            }
            return 0;
        }
        public void calc(int OfficerDataId)
        {
            int towerDataId = Convert.ToInt32(db.OfficerDatas.FirstOrDefault(x => x.Id == OfficerDataId).TowerDataId);
            decimal LandingFee = LandingFees(towerDataId);
            decimal OpenAireportOvertimeFees = LandingFee * 75 / 100;
            decimal NavigationFees = LandingFee * 25 / 100;
            decimal NightOperationFees = LandingFee * 15 / 100;
        }
        public decimal OpenAireportOvertimeFees(int ATAHours, decimal LandingFees)
        {
            GeneralService gs = db.GeneralServices.FirstOrDefault(x => x.Id == 1);
            int Type = Convert.ToInt32(gs.Type);
            decimal Value = 0;
            if (Type == 2)//means It's a value Not Percent
            { Value = Convert.ToDecimal(gs.Value); }
            else //means It's a Percent Not value
            { Value = Convert.ToDecimal(gs.Value) / 100; }

            if (ATAHours >= 16 || ATAHours < 4)
            {
                FinalAmountInDollar = LandingFees * Value;
            }
            else
            {
                FinalAmountInDollar = 0;
            }

            return FinalAmountInDollar;
        }
        public decimal NightOperationFees(int ATAHours, int ATDHours, decimal LandingFees)
        {
            GeneralService gs = db.GeneralServices.FirstOrDefault(x => x.Id == 3);
            int Type = Convert.ToInt32(gs.Type);
            decimal Value = 0;
            if (Type == 2)//means It's a value Not Percent
            { Value = Convert.ToDecimal(gs.Value); }
            else //means It's a Percent Not value
            { Value = Convert.ToDecimal(gs.Value) / 100; }

            //   if ((ATAHours >= 16 || ATAHours < 4) && (ATDHours >= 16 || ATDHours < 4))
            //{
            //    FinalAmountInDollar = (LandingFees * Value);
            //}
            if (ATAHours >= 16 || ATAHours < 4)
            {
                FinalAmountInDollar = LandingFees * Value;
            }
            else if (ATDHours >= 16 || ATDHours < 4)
            {
                FinalAmountInDollar = LandingFees * Value;
            }
            else
            {
                FinalAmountInDollar = 0;
            }

            return FinalAmountInDollar;
        }
        public decimal NavigationFees(decimal LandingFees)
        {
            GeneralService gs = db.GeneralServices.FirstOrDefault(x => x.Id == 2);
            int Type = Convert.ToInt32(gs.Type);
            decimal Value = 0;
            if (Type == 2)//means It's a value Not Percent
            { Value = Convert.ToDecimal(gs.Value); }
            else //means It's a Percent Not value
            { Value = Convert.ToDecimal(gs.Value) / 100; }

            FinalAmountInDollar = LandingFees * Value;
            return FinalAmountInDollar;
        }
        public decimal StayInFees(int Id)
        {
            AireCraftStyIn td = db.AireCraftStyIns.FirstOrDefault(x => x.Id == Id);
            int AireCraftRegId = Convert.ToInt32(td.AireCraftRegId);

            AircraftRegistration rg = db.AircraftRegistrations.FirstOrDefault(x => x.Id == AireCraftRegId);
            decimal maxTakoffWieght = Convert.ToDecimal(db.AircraftRegistrations.FirstOrDefault(x => x.Id == rg.Id).MaxTakoffWieght);

            decimal AmountInDollar = Convert.ToDecimal(db.StayInFees.FirstOrDefault(x => maxTakoffWieght >= x.WieghtFrom && maxTakoffWieght < x.WieghtTo).AmountInDollar);

            DateTime StartDate = Convert.ToDateTime(td.StartDate);
            DateTime EndDate = Convert.ToDateTime(td.EndDate);
            int totalDays = 0;
            if (td.EndDate == null)
            {
                totalDays = Convert.ToInt32((StartDate - DateTime.UtcNow).Days);
            }
            else
            {
                totalDays = Convert.ToInt32((StartDate - EndDate).Days);
            }

            int TotalHours = totalDays * 24;
            int FinalTotalHours = 0;

            if (TotalHours % 6 == 0)
            {
                FinalTotalHours = TotalHours / 6;
            }
            else
            {
                FinalTotalHours = (TotalHours / 6) + 1;
            }

            FinalAmountInDollar = AmountInDollar * FinalTotalHours;
            return FinalAmountInDollar;
        }
        public decimal PushBackFees(string Size)
        {
            //  int towerDataId = Convert.ToInt32(db.OfficerDatas.FirstOrDefault(x => x.Id == OfficerDataId).TowerDataId);
            ///  TowerData t = db.TowerDatas.FirstOrDefault(x => x.Id == towerDataId);

            //  String Size = db.AircraftTypes.FirstOrDefault(x => x.Id == t.AircraftRegistration.AircraftTypeId).AircraftSize.Size.ToString();
            FinalAmountInDollar = Convert.ToDecimal(db.PushBackFees.FirstOrDefault(x => x.AireCraftSize == Size).AmountInDollar);
            return FinalAmountInDollar;
        }

        public decimal DomeCUPPSFees(int Pax, int TRNYCUPPS, bool IsNotUsedCUPPS)
        {
            // OfficerData off = db.OfficerDatas.FirstOrDefault(x => x.Id == OfficerDataId);
            //if(off.TowerData.FlightTypeD != 1)
            //{
            //int TRNYCUPPS  = Convert.ToInt32(off.TRNYCUPPS);
            //int DISEMB     = Convert.ToInt32(off.DISEMB);
            //int DisEmbInft = Convert.ToInt32(off.DisEmbInft);
            decimal AmountInDollar = 0;
            // للراكب المحلي المغادر  
            if (!IsNotUsedCUPPS)
            {
                AmountInDollar = Convert.ToDecimal(db.CUPPSServiceFees.FirstOrDefault(x => x.Id != 1).AmountInDollar);
            }
            else
            {
                AmountInDollar = 0;
            }

            decimal TotalAmount = (Pax + TRNYCUPPS) * AmountInDollar;
            FinalAmountInDollar = TotalAmount;
            //}
            //else
            //{
            //    FinalAmountInDollar = 0; 
            //}
            return FinalAmountInDollar;
        }

        public decimal InterCUPPSFees(int Pax, int TRNYCUPPS, bool IsNotUsedCUPPS)
        {
            //  OfficerData off = db.OfficerDatas.FirstOrDefault(x => x.Id == OfficerDataId);
            //if (off.TowerData.FlightTypeD == 1 || off.TowerData.FlightTypeL == 1)
            //{
            //int TRNYCUPPS = Convert.ToInt32(off.TRNYCUPPS);
            //int DISEMB = Convert.ToInt32(off.DISEMB);
            //int DisEmbInft = Convert.ToInt32(off.DisEmbInft);
            decimal AmountInDollar = 0;
            //للراكب الدولي المغادر1
            if (!IsNotUsedCUPPS)
            {
                AmountInDollar = Convert.ToDecimal(db.CUPPSServiceFees.FirstOrDefault(x => x.Id == 1).AmountInDollar);
            }
            else
            {
                AmountInDollar = 0;
            }

            decimal TotalAmount = (Pax + TRNYCUPPS) * AmountInDollar;
            FinalAmountInDollar = TotalAmount;
            //}
            //else
            //{
            //    FinalAmountInDollar = 0;
            //}
            return FinalAmountInDollar;
        }

        public decimal InterNYFees(int Pax, bool IsLocalCompany)
        {
            decimal AmountInDollar = Convert.ToDecimal(db.NYFees.FirstOrDefault(x => x.Id == 1).AmountInDollar);
            FinalAmountInDollar = (Pax/* + TRNYCUPPS + TRNY*/) * AmountInDollar;

            if (IsLocalCompany)
            {
                // When Airecraft Max takeoff Wieght is greater then Max Wieght in SecurityFees Setting Table                        
                GeneralService gs = db.GeneralServices.FirstOrDefault(x => x.Id == 6);
                int Type = Convert.ToInt32(gs.Type);
                decimal Value = 0;
                if (Type == 2)//means It's a value Not Percent
                {
                    Value = Convert.ToDecimal(gs.Value);
                    decimal DisValue = AmountInDollar - Value;
                    FinalAmountInDollar = Pax * DisValue;
                }
                else //means It's a Percent Not value
                {
                    Value = Convert.ToDecimal(gs.Value) / 100;
                    Value = FinalAmountInDollar * Value;
                    FinalAmountInDollar = FinalAmountInDollar - Value;
                }
            }
            else
            {
                // When Airecraft Max takeoff Wieght is greater then Max Wieght in SecurityFees Setting Table        
                FinalAmountInDollar = (Pax/* + TRNYCUPPS + TRNY*/) * AmountInDollar;
            }

            return FinalAmountInDollar;
        }

        public decimal DomeNYFees(int Pax, bool IsLocalCompany)
        {
            decimal AmountInDollar = Convert.ToDecimal(db.NYFees.FirstOrDefault(x => x.Id != 1).AmountInSDG);
            FinalAmountInDollar = (Pax/* + TRNYCUPPS + TRNY*/) * AmountInDollar;

            if (IsLocalCompany)
            {
                // When Airecraft Max takeoff Wieght is greater then Max Wieght in SecurityFees Setting Table                        
                GeneralService gs = db.GeneralServices.FirstOrDefault(x => x.Id == 6);
                int Type = Convert.ToInt32(gs.Type);
                decimal Value = 0;
                if (Type == 2)//means It's a value Not Percent
                {
                    //Value = Convert.ToDecimal(gs.Value);
                    //FinalAmountInDollar = FinalAmountInDollar - Value;
                }
                else //means It's a Percent Not value
                {
                    Value = Convert.ToDecimal(gs.Value) / 100;
                    Value = FinalAmountInDollar * Value;
                    FinalAmountInDollar = FinalAmountInDollar - Value;
                }
            }
            else
            {
                // When Airecraft Max takeoff Wieght is greater then Max Wieght in SecurityFees Setting Table        
                FinalAmountInDollar = (Pax/* + TRNYCUPPS + TRNY*/) * AmountInDollar;
            }

            return FinalAmountInDollar;
        }

        public decimal AmbulanceFees(int AmbulanceCarCount)
        {
            //  OfficerData off = db.OfficerDatas.Single(x => x.Id == OfficerDataId);
            //int AmbulanceCarCount = Convert.ToInt32(off.AmbulanceCarCount);
            decimal AmountInDollar = 0;
            AmountInDollar = Convert.ToDecimal(db.CarServiceFees.FirstOrDefault(x => x.Id == 1).AmountInDollar);
            FinalAmountInDollar = (AmbulanceCarCount * AmountInDollar);
            return FinalAmountInDollar;
        }

        public decimal FireFightingFees(int FireFightingCarCount)
        {
            //OfficerData off = db.OfficerDatas.FirstOrDefault(x => x.Id == OfficerDataId);
            //int FireFightingCarCount = Convert.ToInt32(off.FireFightingCarCount);            
            decimal AmountInDollar = 0;
            AmountInDollar = Convert.ToDecimal(db.CarServiceFees.FirstOrDefault(x => x.Id != 1).AmountInDollar);
            FinalAmountInDollar = (FireFightingCarCount * AmountInDollar);
            return FinalAmountInDollar;

        }

        public decimal FirstClassFees(int FirstClassCount)
        {
            // OfficerData off = db.OfficerDatas.Where(x => x.Id == OfficerDataId).OrderBy(o=>o.Id).First();
            //int FirstClassCount = Convert.ToInt32(off.FirstClassCount);
            GeneralService gs = db.GeneralServices.FirstOrDefault(x => x.Id == 4);
            int Type = Convert.ToInt32(gs.Type);
            decimal Value = 0;
            if (Type == 2)//means It's a value Not Percent
            { Value = Convert.ToDecimal(gs.Value); }
            else
            { Value = Convert.ToDecimal(gs.Value) / 100; }

            FinalAmountInDollar = (FirstClassCount * Value);
            return FinalAmountInDollar;
        }

        public decimal FreightFees(decimal FrightLoadingExp, decimal FrightLoadingImp)
        {
            /*
                يوجد نوعين للشحن:
                صادر و وارد 
                الصادر مفترض يكون رقم 1 في الجدول
                الوارد مفترض يكون رقم 2 في الجدول
            */
            // int towerDataId = Convert.ToInt32(db.OfficerDatas.FirstOrDefault(x => x.Id == OfficerDataId).TowerDataId);
            // OfficerData off = db.OfficerDatas.FirstOrDefault(x => x.TowerDataId == towerDataId);
            //int FrightLoadingExp = Convert.ToInt32(off.FreightLoadingExp);
            //int FrightLoadingImp = Convert.ToInt32(off.FreightLoadingImp);

            decimal NormalMailLoadingExpAmount = Convert.ToDecimal(db.FreightFees.FirstOrDefault(x => x.Id == 1).AmountInDollar);//الشحن الصادر
            decimal NormalMailLoadingImpAmount = Convert.ToDecimal(db.FreightFees.FirstOrDefault(x => x.Id == 2).AmountInDollar);//الشحن الوارد

            FinalAmountInDollar = (FrightLoadingExp * NormalMailLoadingExpAmount) + (FrightLoadingImp * NormalMailLoadingImpAmount);
            return FinalAmountInDollar;

        }

        public decimal ExpMailFees(decimal NormalMailLoadingExpAmount, decimal RapidMailLoadingExpAmount)
        {
            /*
                يوجد 4 أنواع للبريد:
                صادر سريع / صادر عادي / وارد سريع / وارد عادي 
                الصادر العادي مفترض يكون رقم 1 في الجدول
                الوارد العادي مفترض يكون رقم 2 في الجدول
                الصادر السريع مفترض يكون رقم 3 في الجدول
                الوارد السريع مفترض يكون رقم 4 في الجدول
            */
            //int towerDataId = Convert.ToInt32(db.OfficerDatas.FirstOrDefault(x => x.Id == OfficerDataId).TowerDataId);
            //OfficerData off = db.OfficerDatas.FirstOrDefault(x => x.TowerDataId == towerDataId);
            //int NormalMailLoadingExp = Convert.ToInt32(off.NormalMailLoadingExp);            
            //int RapidMailLoadingExp = Convert.ToInt32(off.RapidMailLoadingExp);

            decimal NormalMailLoadingExpAmountfees = Convert.ToDecimal(db.MailServiceFees.FirstOrDefault(x => x.Id == 1).AmountInDollar);//صادر عادي            
            decimal RapidMailLoadingExpAmountfees = Convert.ToDecimal(db.MailServiceFees.FirstOrDefault(x => x.Id == 3).AmountInDollar);//صادر سريع

            FinalAmountInDollar = (NormalMailLoadingExpAmountfees * NormalMailLoadingExpAmount) + (RapidMailLoadingExpAmountfees * RapidMailLoadingExpAmount);
            return FinalAmountInDollar;
        }

        public decimal ImpMailFees(decimal NormalMailLoadingImp, decimal RapidMailLoadingImp)
        {
            /*
            يوجد 4 أنواع للبريد:
            صادر سريع / صادر عادي / وارد سريع / وارد عادي 
            الصادر العادي مفترض يكون رقم 1 في الجدول
            الوارد العادي مفترض يكون رقم 2 في الجدول
            الصادر السريع مفترض يكون رقم 3 في الجدول
            الوارد السريع مفترض يكون رقم 4 في الجدول
            */
            //int towerDataId = Convert.ToInt32(db.OfficerDatas.FirstOrDefault(x => x.Id == OfficerDataId).TowerDataId);
            //OfficerData off = db.OfficerDatas.FirstOrDefault(x => x.TowerDataId == towerDataId);
            //int NormalMailLoadingImp = Convert.ToInt32(off.NormalMailLoadingImp);
            //int RapidMailLoadingImp = Convert.ToInt32(off.RapidMailLoadingImp);

            decimal NormalMailLoadingImpAmount = Convert.ToDecimal(db.MailServiceFees.FirstOrDefault(x => x.Id == 2).AmountInDollar);//وارد عادي            
            decimal RapidMailLoadingImpAmount = Convert.ToDecimal(db.MailServiceFees.FirstOrDefault(x => x.Id == 4).AmountInDollar);//وارد سريع

            FinalAmountInDollar = (NormalMailLoadingImp * NormalMailLoadingImpAmount) + (RapidMailLoadingImp * RapidMailLoadingImpAmount);
            return FinalAmountInDollar;
        }

        public decimal NoiseFees(decimal maxTakoffWieght)
        {
            // int towerDataId = Convert.ToInt32(db.OfficerDatas.FirstOrDefault(x => x.Id == OfficerDataId).TowerDataId);
            //  TowerData td = db.TowerDatas.FirstOrDefault(x => x.Id == towerDataId);
            //  AircraftRegistration rg = db.AircraftRegistrations.FirstOrDefault(x => x.Id == td.AircraftRegId);
            /// decimal maxTakoffWieght = Convert.ToDecimal(db.AircraftRegistrations.FirstOrDefault(x => x.Id == rg.Id).MaxTakoffWieght);

            // When Airecraft Max takeoff Wieght is greater then Max Wieght in NoiseFees Setting Table
            decimal AmountInDollar = Convert.ToDecimal(db.NoiseFees.FirstOrDefault(x => x.WieghtFrom <= maxTakoffWieght && x.WieghtTo > maxTakoffWieght).AmountInDollar);
            FinalAmountInDollar = AmountInDollar;

            return FinalAmountInDollar;
        }
        public decimal NoiseFees(TowerData model)
        {
            //  int towerDataId = Convert.ToInt32(db.OfficerDatas.FirstOrDefault(x => x.Id == OfficerDataId).TowerDataId);
            //  TowerData td = db.TowerDatas.FirstOrDefault(x => x.Id == towerDataId);
            AircraftRegistration rg = db.AircraftRegistrations.FirstOrDefault(x => x.Id == model.AircraftRegId);
            decimal maxTakoffWieght = Convert.ToDecimal(db.AircraftRegistrations.FirstOrDefault(x => x.Id == rg.Id).MaxTakoffWieght);

            // When Airecraft Max takeoff Wieght is greater then Max Wieght in NoiseFees Setting Table
            decimal AmountInDollar = Convert.ToDecimal(db.NoiseFees.FirstOrDefault(x => x.WieghtFrom <= maxTakoffWieght && x.WieghtTo > maxTakoffWieght).AmountInDollar);
            FinalAmountInDollar = AmountInDollar;

            return FinalAmountInDollar;
        }
        public decimal SecurityFees(decimal maxTakoffWieght, int FlightTypeL, bool IsLocalCompany)
        {
           
                decimal FinalAmountInDollar;
                int FlightType = cu.GetFlightType(FlightTypeL /*, td.FlightTypeD*/);
                // When Airecraft Max takeoff Wieght is greater then Max Wieght in SecurityFees Setting Table
                decimal AmountInDollar = Convert.ToDecimal(db.SecurityFees.FirstOrDefault(x => x.WieghtFrom <= maxTakoffWieght && x.WieghtTo > maxTakoffWieght && x.FlightType == FlightType).AmountInDollar);
                FinalAmountInDollar = AmountInDollar;

             if (IsLocalCompany)
             {
                //if (FlightType == 1)
                //{
                    GeneralService gs = db.GeneralServices.FirstOrDefault(x => x.Id == 5);
                    int Type = Convert.ToInt32(gs.Type);
                    decimal Value = 0;
                    if (Type == 2)//means It's a value Not Percent
                    {
                        Value = Convert.ToDecimal(gs.Value);
                        FinalAmountInDollar = FinalAmountInDollar - Value;
                    }
                    else //means It's a Percent Not value
                    {
                        Value = Convert.ToDecimal(gs.Value) / 100;
                        Value = FinalAmountInDollar * Value;
                        FinalAmountInDollar = FinalAmountInDollar - Value;
                    }
                //}
            }

            return FinalAmountInDollar;
        }

        public decimal SecurityFees(TowerData model)
        {
            int towerDataId = model.Id;
            //  TowerData td = db.TowerDatas.FirstOrDefault(x => x.Id == towerDataId);
            AircraftRegistration rg = db.AircraftRegistrations.FirstOrDefault(x => x.Id == model.AircraftRegId);
            decimal maxTakoffWieght = Convert.ToDecimal(db.AircraftRegistrations.FirstOrDefault(x => x.Id == rg.Id).MaxTakoffWieght);

            int FlightType = cu.GetFlightType(model.FlightTypeL/*, model.FlightTypeD*/);
            if (Convert.ToBoolean(model.AirLine.IsLocalCompany))
            {
                // When Airecraft Max takeoff Wieght is greater then Max Wieght in SecurityFees Setting Table        
                FinalAmountInDollar = Convert.ToDecimal(db.SecurityFees.FirstOrDefault(x => x.WieghtFrom <= maxTakoffWieght && x.WieghtTo > maxTakoffWieght && x.FlightType == FlightType).AmountInDollar);
                GeneralService gs = db.GeneralServices.FirstOrDefault(x => x.Id == 5);
                int Type = Convert.ToInt32(gs.Type);
                decimal Value = 0;
                if (Type == 2)//means It's a value Not Percent
                {
                    Value = Convert.ToDecimal(gs.Value);
                    FinalAmountInDollar = FinalAmountInDollar - Value;
                }
                else //means It's a Percent Not value
                {
                    Value = Convert.ToDecimal(gs.Value) / 100;
                    Value = FinalAmountInDollar * Value;
                    FinalAmountInDollar = FinalAmountInDollar - Value;
                }
            }
            else
            {
                // When Airecraft Max takeoff Wieght is greater then Max Wieght in SecurityFees Setting Table        
                FinalAmountInDollar = Convert.ToDecimal(db.SecurityFees.FirstOrDefault(x => x.WieghtFrom <= maxTakoffWieght && x.WieghtTo > maxTakoffWieght && x.FlightType == FlightType).AmountInDollar);
            }

            return FinalAmountInDollar;
        }


        //public decimal ParkingFeesNaji(int OfficerDataId, DateTime LandingDate, DateTime TakeOffDate, TimeSpan ATA, TimeSpan ATD, bool IsLocalCompany)
        //{
        //      decimal FinalAmountInDollar;

        //    DateTime FullLandingDate = Convert.ToDateTime(LandingDate).AddHours(ATA.Hours).AddMinutes(ATA.Minutes);
        //    DateTime FullTakeOffDate = Convert.ToDateTime(TakeOffDate).AddHours(ATD.Hours).AddMinutes(ATD.Minutes);


        //    return FinalAmountInDollar;
        //    }


        public decimal FinalAmountForSudanAirWays(DateTime DateFrom)
        {
            IQueryable<Revenue> Revenue;
            //Revenue = db.Revenues.OrderBy(x => x.OfficerData.TowerData.Date);

            Revenue = db.Revenues.Where(x => x.OfficerData.TowerData.AirLineId == 176);
            decimal total;
            if (DateFrom != null)
            {
                decimal ExchangeRate = cu.getExchangeRateByLandingDate(DateFrom.Month, DateFrom.Year);
                Revenue = Revenue.Where(t => t.OfficerData.TowerData.Date.Value.Month == DateFrom.Month && t.OfficerData.TowerData.Date.Value.Year == DateFrom.Year);

                total = Convert.ToDecimal(Revenue.Sum(x => x.LandingFees)) + Convert.ToDecimal(Revenue.Sum(x => x.ParkingFees)) + Convert.ToDecimal(Revenue.Sum(x => x.NoiseFees)) +
                            Convert.ToDecimal(Revenue.Sum(x => x.NightOperationFees)) + Convert.ToDecimal(Revenue.Sum(x => x.NavigationFees)) +
                            Convert.ToDecimal(Revenue.Sum(x => x.SecurityFees)) + Convert.ToDecimal(Revenue.Sum(x => x.FreightFees)) +
                            Convert.ToDecimal(Revenue.Sum(x => x.ExpMailFees)) + Convert.ToDecimal(Revenue.Sum(x => x.ImpMailFees)) +
                            Convert.ToDecimal(Revenue.Sum(x => x.InterCUPPSFees)) + Convert.ToDecimal(Revenue.Sum(x => x.DomeCUPPSFees)) +
                            Convert.ToDecimal(Revenue.Sum(x => x.InterNYFees)) +
                            Convert.ToDecimal(Revenue.Sum(x => x.FirstClassFees)) + Convert.ToDecimal(Revenue.Sum(x => x.PushBackFees)) +
                            Convert.ToDecimal(Revenue.Sum(x => x.AmbolanceFees)) + Convert.ToDecimal(Revenue.Sum(x => x.FireFightingFees));

                decimal tax = (Convert.ToDecimal(Revenue.Sum(x => x.LandingFees)) + Convert.ToDecimal(Revenue.Sum(x => x.ParkingFees)) + Convert.ToDecimal(Revenue.Sum(x => x.NoiseFees)) +
                            Convert.ToDecimal(Revenue.Sum(x => x.NightOperationFees)) + Convert.ToDecimal(Revenue.Sum(x => x.NavigationFees)) +
                            Convert.ToDecimal(Revenue.Sum(x => x.SecurityFees)) + Convert.ToDecimal(Revenue.Sum(x => x.FreightFees)) +
                            Convert.ToDecimal(Revenue.Sum(x => x.ExpMailFees)) + Convert.ToDecimal(Revenue.Sum(x => x.ImpMailFees)) +
                            Convert.ToDecimal(Revenue.Sum(x => x.InterCUPPSFees)) + Convert.ToDecimal(Revenue.Sum(x => x.DomeCUPPSFees)) +
                            Convert.ToDecimal(Revenue.Sum(x => x.FirstClassFees)) + Convert.ToDecimal(Revenue.Sum(x => x.PushBackFees)) +
                            Convert.ToDecimal(Revenue.Sum(x => x.AmbolanceFees)) + Convert.ToDecimal(Revenue.Sum(x => x.FireFightingFees))) * 5 / 100;

                total = ((total + tax) * ExchangeRate) + Convert.ToDecimal(Revenue.Sum(x => x.DomeNYFees));
            }
            else
            {
                total = 0;
            }

            return total;
        }


        public static decimal GroundHandlingFees(int TowerDataId, int OfficerDataId)
        {
            decimal myFinalAmountInDollar = 0;
            try
            {
                if (TowerDataId > 0)
                {
                    TowerData td = db.TowerDatas.FirstOrDefault(x => x.Id == TowerDataId);
                    AircraftRegistration rg = db.AircraftRegistrations.FirstOrDefault(x => x.Id == td.AircraftRegId);
                    decimal maxTakoffWieght = Convert.ToDecimal(db.AircraftRegistrations.FirstOrDefault(x => x.Id == rg.Id).MaxTakoffWieght);

                    //decimal LastWieght = Convert.ToDecimal(db.GroundHandlingtFees.FirstOrDefault(x => x.CheckAppendWieght == true).WieghtFrom);

                    //if (maxTakoffWieght <= LastWieght) // When Airecraft Max takeoff Wieght is greater then Max Wieght in LandingServiceFees Setting Table
                    //{
                    decimal AmountInDollar = Convert.ToDecimal(db.GroundHandlingFees.FirstOrDefault(x => x.WieghtFrom <= maxTakoffWieght && x.WieghtTo > maxTakoffWieght).AmountInDollar);
                    myFinalAmountInDollar = AmountInDollar;
                    //}
                    //else  // When Airecraft Max takeoff Wieght is less then Max Wieght in LandingServiceFees Setting Table
                    //{
                    //    decimal BeforLastAmount = Convert.ToDecimal(db.GroundHandlingtFees.FirstOrDefault(x => LastWieght > x.WieghtFrom && LastWieght <= x.WieghtTo).AmountInDollar);
                    //    decimal OverWieght = maxTakoffWieght - LastWieght;
                    //    decimal LastAmount = Convert.ToDecimal(db.GroundHandlingtFees.SingleOrDefault(x => x.CheckAppendWieght == true).AmountInDollar);
                    //    decimal TotalAmount = OverWieght * LastAmount;
                    //    decimal IncreaseAmount = TotalAmount + BeforLastAmount;

                    //    myFinalAmountInDollar = IncreaseAmount;
                    //}

                    return myFinalAmountInDollar;
                }

                return 0;
            }
            catch (Exception e)
            {
                return -1;
            }

        }


        public static decimal FuelPerLiterFees(decimal FuelInLiter)
        {
            decimal myFinalAmountInDollar = 0;
            try
            {
                if (FuelInLiter > 0)
                {
                    var genSer = db.GeneralServices.Find(7); //The FuelFees Id must be = 7 in GeneralService Table
                    if (genSer.Type == 3)// Count Means :: Count * AmounForSingleValue
                    {
                        myFinalAmountInDollar = FuelInLiter * Convert.ToDecimal(genSer.Value);

                    }
                    else if (genSer.Type == 2) // Means Percent
                    {
                        myFinalAmountInDollar = FuelInLiter * Convert.ToDecimal(genSer.Value) / 100;
                    }
                    else //genSer.Type == 1  Means Static value
                    {
                        myFinalAmountInDollar = Convert.ToDecimal(genSer.Value);
                    }

                    return myFinalAmountInDollar;
                }

                return 0;
            }
            catch (Exception e)
            {
                return -1;
            }

        }

    }
}







