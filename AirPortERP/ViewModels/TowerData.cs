using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AirPortERP.ViewModels
{
    public class TowerDataViewModels
    {

        public int Id { get; set; }

        public string AirLineName { get; set; }
        public string AirLineCode { get; set; }
      //  public string AirLineArabicName { get; set; }


        public string FlightNo { get; set; }
        public string AircraftRegName { get; set; }
        public string Type { get; set; }
        public string AirportIdFrom { get; set; }

        public string FromArabicName { get; set; }
        public string FromArabicCountry { get; set; }
        public string AirportIdTo { get; set; }
        public string ToArabicName { get; set; }
        public string ToArabicCountry { get; set; }








        public string CountryFrom { get; set; }
        public string CountryTo { get; set; }

        public string Date { get; set; }
        //Nullable<System.TimeSpan>
        public string ATA { get; set; }
        public string ATD { get; set; }
        //public Nullable<int> POB { get; set; }
        //public Nullable<int> QBD { get; set; }
        public string TripType { get; set; }
        public string LandingPermission { get; set; }
        public Nullable<int> CompanyInfoId { get; set; }
        public string CreatedBy { get; set; }
        public string CreationDate { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatingDate { get; set; }

        public string LandingDate { get; set; }
        public string TakeOffDate { get; set; }

        public string FlightType { get; set; }


        public string AirportFromName { get; set; }
        public string AirportToName { get; set; }
        public string MaxTaKeOffWeight { get; set; }

        public string Size { get; set; }


        public string LandingFees { get; set; }
        public string ParkingFees { get; set; }
        public string NightOperationFees { get; set; }
        public string NavigationFees { get; set; }
        public string SecurityFees { get; set; }
        public string NoiseFees { get; set; }





    }


    public class LandingCoreViewModel
    {

        public int Id { get; set; }


        
        public string CompanyCode { get; set; }


        public string FlightNo { get; set; }

        public string RegNo { get; set; }
           public string PlaneSizeID { get; set; }
        public string Wt { get; set; }



        public string FlightFrom { get; set; }
        public string FlightTo { get; set; }



        public string FlightType { get; set; }

        public string FlightDate { get; set; }


        //public string AirportIdTo { get; set; }

        //public string CountryFrom { get; set; }
        //public string CountryTo { get; set; }

        //public string Date { get; set; }
        ////Nullable<System.TimeSpan>
        //public string ATA { get; set; }
        //public string ATD { get; set; }
        ////public Nullable<int> POB { get; set; }
        ////public Nullable<int> QBD { get; set; }
        //public string TripType { get; set; }
        //public string LandingPermission { get; set; }
        //public Nullable<int> CompanyInfoId { get; set; }
        //public string CreatedBy { get; set; }
        //public string CreationDate { get; set; }
        //public string UpdatedBy { get; set; }
        //public string UpdatingDate { get; set; }

        //public string LandingDate { get; set; }
        //public string TakeOffDate { get; set; }

        //public string FlightType { get; set; }


        //public string AirportFromName { get; set; }
        //public string AirportToName { get; set; }
        //public string MaxTaKeOffWeight { get; set; }

        //public string Size { get; set; }


        //public string LandingFees { get; set; }
        //public string ParkingFees { get; set; }
        //public string NightOperationFees { get; set; }
        //public string NavigationFees { get; set; }
        //public string SecurityFees { get; set; }
        //public string NoiseFees { get; set; }





    }

    public class TowerDataViewModelsDaily
    {

        public int Id { get; set; }
        public string AirLineName { get; set; }

        public string AirLineCode { get; set; }
        // public string CallSingAndFlightNo { get; set; }
        public string FlightNo { get; set; }
        public string AircraftRegName { get; set; }
        public string AircraftType { get; set; }
        public string AirportIdFrom { get; set; }
        public string AirportIdTo { get; set; }

        public string CountryFrom { get; set; }
        public string CountryTo { get; set; }

        public string Date { get; set; }
        //Nullable<System.TimeSpan>
        public string ATA { get; set; }
        public string ATD { get; set; }
        public Nullable<int> POB { get; set; }
        public Nullable<int> QBD { get; set; }
        public string TripType { get; set; }
        public string LandingPermission { get; set; }
        public Nullable<int> CompanyInfoId { get; set; }
        public string CreatedBy { get; set; }
        public string CreationDate { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatingDate { get; set; }

        public string LandingDate { get; set; }
        public string TakeOffDate { get; set; }

        public string FlightType { get; set; }

        public string Note { get; set; }






    }


    public class RevenueDataViewModels
    {

        public int Id { get; set; }

        public string AirLineName { get; set; }
        public string AirLineCode { get; set; }
        // public string CallSingAndFlightNo { get; set; }


        public string FlightNo { get; set; }
        public string AircraftRegName { get; set; }
        public string Type { get; set; }
        public string AirportIdFrom { get; set; }
        public string AirportIdTo { get; set; }

        public string CountryFrom { get; set; }
        public string CountryTo { get; set; }

        public string Date { get; set; }
        //Nullable<System.TimeSpan>
        public string ATA { get; set; }
        public string ATD { get; set; }
        public Nullable<int> POB { get; set; }
        public Nullable<int> QBD { get; set; }
        public string TripType { get; set; }
        public string LandingPermission { get; set; }
        public Nullable<int> CompanyInfoId { get; set; }
        public string CreatedBy { get; set; }
        public string CreationDate { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatingDate { get; set; }

        public string LandingDate { get; set; }
        public string TakeOffDate { get; set; }

        public string FlightType { get; set; }

        //landing
        public string LandingFees { get; set; }
        public string ParkingFees { get; set; }
        public string NightOperationFees { get; set; }
        public string NavigationFees { get; set; }
        public string SecurityFees { get; set; }
        public string NoiseFees { get; set; }
        //mail And Freight
        public decimal FreightFees { get; set; }
        public decimal ImpMailFees { get; set; }
        public decimal ExpMailFees { get; set; }

        public int FreightImpCount { get; set; }
        public int FreightExpCount { get; set; }
        public int NormalImpMailCount { get; set; }
        public int NormalMailExpCount { get; set; }
        public int RapidMailImpCount { get; set; }
        public int RapidMailExpCount { get; set; }


        //Car Service
        public decimal FireFightingFees { get; set; }
        public decimal AmbolanceFees { get; set; }
        public int FireFightingCount { get; set; }
        public int AmbolanceCount { get; set; }





        //FirstClass
        public decimal FirstClassFees { get; set; }
        public int FirstClassCount { get; set; }


        //CUPPS And Ny
        public decimal InterCUPPSFees { get; set; }
        public decimal DomeCUPPSFees { get; set; }
        public decimal InterNYFees { get; set; }
        public decimal DomeNYFees { get; set; }

        public int Pax { get; set; }
        public int InterCUPPSCount { get; set; }
        public int DomeCUPPSCount { get; set; }
        public int InterNYCount { get; set; }
        public int DomeNYCount { get; set; }

        //PushBack
        public decimal PushBackFees { get; set; }
        public int PushBackCount { get; set; }


    }














}