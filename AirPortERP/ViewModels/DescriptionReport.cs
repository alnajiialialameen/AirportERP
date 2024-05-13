using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AirPortERP.ViewModels
{
    public class DescriptionReportModel
    {

        public string InternationalDomesticMonthTitle { get; set; }
        public int TotalInternationalMonthCount { get; set; }
        public int TotalDomesticMonthCount { get; set; }
       
        /// <summary>
        /// //////////////////////////////////////////////////////////
        /// </summary>
        public string InternationalDomesticTitle { get; set; }
        public int TotalInternationalCount { get; set; }
        public int TotalDomesticCount { get; set; }

        // /////////////////////////////////////////////

        public string TripTypeTitle { get; set; }

     //   public int TotalmilitaryCount { get; set; }

        public int Totallocal { get; set; }
        public int TotallocalDomstic { get; set; }
        public int TotalForeign { get; set; }
        public int TotallocalInter { get; set; }

        // /////////////////////////////////////////////

        public string FiveTakeOffTitle { get; set; }

        public bool FiveTakeOffStatus { get; set; }

        public string FiveTakeOffName1 { get; set; }
        public int FiveTakeOffCount1 { get; set; }


        public string FiveTakeOffName2 { get; set; }
        public int FiveTakeOffCount2 { get; set; }

        public string FiveTakeOffName3 { get; set; }
        public int FiveTakeOffCount3 { get; set; }

        public string FiveTakeOffName4 { get; set; }
        public int FiveTakeOffCount4 { get; set; }

        public string FiveTakeOffName5 { get; set; }
        public int FiveTakeOffCount5 { get; set; }

      //  public string TargetsExecuteTitle { get; set; }

        /////////////////////////////////////////////



        public string NightDayTitle { get; set; }

        public int InOverTimeCount { get; set; }

        public int InDuityCount { get; set; }




        public int TotalCountOfAirline { get; set; }
        public int TotalAirLinesFlightTrip { get; set; }

        public int TotalAirPort { get; set; }

        public int TotalCountries { get; set; }

        public int TotalFlightInDay { get; set; }

        public int TotalFlightInMonth { get; set; }

        public int TotalFlightInYear { get; set; }


        public int HasNightStop { get; set; }

        public int NotHasNightStop { get; set; }

        public string NightStopTitle { get; set; }



    }
}