using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AirPortERP.ViewModels
{
    public class AirPortsViewMosels
    {
        public int Id { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string Code { get; set; }
        public string CountryName { get; set; }



    }





    public class GroundHandlingViewModels
    {

        public int Id { get; set; }

        public int TowerDataId { get; set; }
        public int GroundHandlingCompId { get; set; }
        public string HandlingCompName { get; set; }

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
        public string GroundHandlingFees { get; set; }
    }

    public class HandlingAgentsCompaniesViewModel
    {

        public int Id { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string IsActive { get; set; }
        public Nullable<decimal> AccId { get; set; }


    }


    public class PrintTripInfoViewModel
    {
        /*----------------------Main----------------------------*/
        public int Id { get; set; }
        public string Airline { get; set; }
        public string FlightDate { get; set; }
        public string LandingDate { get; set; }
        public string ATA { get; set; }
        public string ATD { get; set; }
        public string FlightNo { get; set; }
        public string MaxTakoffWieght { get; set; }
        public string RegNo { get; set; }
        public string Type { get; set; }
        public string From { get; set; }
        public string To { get; set; }

        /*----------------------Pasingers----------------------------*/
        public string DomesticPax { get; set; }
        public string InterPax { get; set; }
        public string TRNPax { get; set; }

        /*-----------------------Services---------------------------*/
        public string AmbulanceCarCount { get; set; }
        public string FireTruckCount { get; set; }
        public string MailExp { get; set; }
        public string MailImp { get; set; }
        public string FrieghtExp { get; set; }
        public string FrieghtImp { get; set; }
        public string GroundHandlingComp { get; set; }
        public string FuelComp { get; set; }
        public string FuelLiters { get; set; }
    }



    public class AirlineViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string ArName { get; set; }
        public string Address { get; set; }

        public int AccId { get; set; }

        public bool IsLocalCompany { get; set; }
        public Nullable<int> CurrencyType { get; set; }


    }
    


    public class FlightNoCoreViewModels
    {

        public int id { get; set; }
        public string FlightNo { get; set; }
        public string CompanyCode { get; set; }
        public string FlightTo { get; set; }


    }


    public class RegisterationCoreViewModels
    {

        public int Id { get; set; }
        public string CompanyCode { get; set; }
        public string Reg { get; set; }
        public string Type { get; set; }
        public string MaxTakeOff { get; set; }
        public string PlaneSize { get; set; }


        public string CreatedBy { get; set; }
        public string CreationDate { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatingDate { get; set; }


    }
    public class PrintFormatsViewModels
    {
        public int Id { get; set; }
        public string Type { get; set; }
       



    }
    
    public class AircraftTypeModel
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public decimal MaxTakoffWieght { get; set; }
        public string SizeName { get; set; }
       



    }

    public class AircraftRegistrationModels
    {
        public int Id { get; set; }
        public string Registration { get; set; }
        public string AircraftTypeName { get; set; }
        public string MaxTakoffWieght { get; set; }

        public string Size { get; set; }


    }







    public class AirLinesViewMosels
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public Nullable<decimal> AccNo { get; set; }
        public string Islocal { get; set; }


    }

    public class AirLinesFlightTripsViewMosels
    {

        public int Id { get; set; }
        public string FlightNo { get; set; }
        public string AirPortName { get; set; }
        public string AirPortCode { get; set; }
        public string Country { get; set; }
        public string AirLineName { get; set; }
        public string IsActive { get; set; }


    }

    public class UsersViewModel
    {

        public string Id { get; set; }
        public string UserName { get; set; }
        public string RoleCurrent { get; set; }
        public string Email { get; set; }

        public string RoleNew { get; set; }

        public string RoleId { get; set; }

    }



    public class ResetPasswordViewModel
    {

        public string Id { get; set; }
        public string UserName { get; set; }
        public string RoleCurrent { get; set; }
        public string Email { get; set; }

        public string RoleNew { get; set; }

        public string RoleId { get; set; }

        public string Password { get; set; }
        
    }














    public class CarServiceViewModels
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string AmountInDollar { get; set; }
        public string CreatedBy { get; set; }
        public string CreationDate { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatingDate { get; set; }


    }

    public partial class CUPPSServiceFeeViewModels
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string AmountInDollar { get; set; }
        public string CreatedBy { get; set; }
        public string CreationDate { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatingDate { get; set; }
    }



    public partial class FreightFeeViewModels
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Nullable<decimal> AmountInDollar { get; set; }
        public string CreatedBy { get; set; }
        public string CreationDate { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatingDate { get; set; }
    }



    public partial class LandingServiceFeeViewModels
    {
        public int Id { get; set; }
        public Nullable<decimal> WieghtFrom { get; set; }
        public Nullable<decimal> WieghtTo { get; set; }
        public Nullable<decimal> AmountInDollar { get; set; }
        public bool CheckAppendWieght { get; set; }
        public string CreatedBy { get; set; }
        public string CreationData { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatingDate { get; set; }
    }




    public partial class MailServiceFeeViewModels
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string AmountInDollar { get; set; }
        public string CreatedBy { get; set; }
        public string CreationDate { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatingDate { get; set; }
    }



    public partial class NoiseFeeViewModels
    {
        public int Id { get; set; }
        public Nullable<decimal> WieghtFrom { get; set; }
        public Nullable<decimal> WieghtTo { get; set; }
        public string AmountInDollar { get; set; }
        public string CreatedBy { get; set; }
        public string CreationDate { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatingDate { get; set; }
    }




    public partial class NYFeesViewModels
    {
        public int Id { get; set; }
        public Nullable<int> FlightType { get; set; }
        public Nullable<decimal> AmountInDollar { get; set; }        
        public string FlightTypeName { get; set; }
        public string CreatedBy { get; set; }
        public string CreationDate { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatingDate { get; set; }
    }



    public partial class ParkingServiceFeesViewModels
    {
        public int Id { get; set; }
        public Nullable<decimal> WieghtFrom { get; set; }
        public Nullable<decimal> WieghtTo { get; set; }
        public Nullable<decimal> AmountInDollar { get; set; }
        public bool CheckAppendWieght { get; set; }
        public string CreatedBy { get; set; }
        public string CreationDate { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatingDate { get; set; }
    }



    public partial class PushBackFeesViewModels
    {
        public int Id { get; set; }
        public string AireCraftSize { get; set; }
        public Nullable<decimal> AmountInDollar { get; set; }
        public string CreatedBy { get; set; }
        public string CreationDate { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatingDate { get; set; }
    }


    public partial class SecurityFeesViewModels
    {
        public int Id { get; set; }
        public Nullable<decimal> WieghtFrom { get; set; }
        public Nullable<decimal> WieghtTo { get; set; }
        public Nullable<int> FlightType { get; set; }
        public Nullable<decimal> AmountInDollar { get; set; }
        public bool CheckAppendWieght { get; set; }
        public string CreatedBy { get; set; }
        public string CreationDate { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatingDate { get; set; }
    }



    public partial class StayInFeesViewModels
    {
        public int Id { get; set; }
        public Nullable<decimal> WieghtFrom { get; set; }
        public Nullable<decimal> WieghtTo { get; set; }
        public Nullable<decimal> AmountInDollar { get; set; }
        public bool CheckAppendWieght { get; set; }
        public string CreatedBy { get; set; }
        public string CreationDate { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatingDate { get; set; }
    }


    public class AirLineAgentsViewMosels
    {
        public int Id { get; set; }
        public string NameAr { get; set; }
        public string NameAn { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string AirLineName { get; set; }
        public string IsActive { get; set; }
    }



    public class HandlingAgentsCompaniesViewModels
    {

        public int Id { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string Address { get; set; }
        public string IsActive { get; set; }


    }

    public class CompanyInfoViewModels
    {
     

        public int Id { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string Code { get; set; }
        public string Address { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string IsActive { get; set; }
        public string Image { get; set; }

        public bool IsActiveCom { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreationDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatingDate { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }


    }




    public partial class CurrencieViewModels
    {
        public int Id { get; set; }
        public string CurrencyName { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string MajorUnit { get; set; }
        public string MinorUnit { get; set; }
        public Nullable<decimal> ExchangeRateAgainstPrimaryCurrency { get; set; }
        public string Notes { get; set; }
        public string CreatedBy { get; set; }
        public string CreationDate { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatingDate { get; set; }
    }




    public class WorkOnViewModel
    {

        public int Id { get; set; }
        public string MouthWorkOn { get; set; }
        public int YearWorkOn { get; set; }
        public string UserName { get; set; }


    }


     public class CurrencyDetailsViewModel
    {

        public int Id { get; set; }
        public string CurrencyName { get; set; }
        public string ExchangeRate { get; set; }
        public string Month { get; set; }
        public int Year { get; set; }

        public bool IsActive { get; set; }
        

    }
}