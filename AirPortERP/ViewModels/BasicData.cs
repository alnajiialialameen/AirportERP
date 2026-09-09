using AirPortERP.Models;
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
        public string AirPort { get; set; }
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
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string IsActive { get; set; }
        public int? AccId { get; set; }
        

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
        public string IsSingleAccounting { get; set; }
        


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
        public string Day { get; set; }
        public string Month { get; set; }
        public int Year { get; set; }

        public bool IsActive { get; set; }
        

    }





    public class CurrencyDetailDto
    {
        public int Id { get; set; }
        public int? CurrencyId { get; set; }
        public int? MonthWorkOnId { get; set; }
        public int? YearWorkOnId { get; set; }
        public string Date { get; set; }
        public decimal? ExchangeRateAgainstPrimaryCurrency { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreationDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatingDate { get; set; }

       
    }


    public class TowerDataDto
    {
       

        public int Id { get; set; }
        public int? AirLineId { get; set; }
        public int? FlightNo { get; set; }
        public int? AircraftRegId { get; set; }
        public int? AirportIdFrom { get; set; }
        public int? AirportIdTo { get; set; }
        public int? FlightTypeD { get; set; }
        public int? FlightTypeL { get; set; }
        public string LandingDate { get; set; }
        public string TakeOffDate { get; set; }
        public int? Status { get; set; }
        public string Date { get; set; }
        public Nullable<System.TimeSpan> ATA { get; set; }
        public Nullable<System.TimeSpan> ATD { get; set; }
    
        public Nullable<int> TripTypeId { get; set; }
    
        public Nullable<int> CompanyInfoId { get; set; }
        public string Note { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreationDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatingDate { get; set; }

   
    }

    public class TowerDataDtoU
    {


        public int IdU { get; set; }
        public int? AirLineIdU { get; set; }
        public int? FlightNoU { get; set; }
        public int? AircraftRegIdU { get; set; }
        public int? AirportIdFromU { get; set; }
        public int? AirportIdToU { get; set; }
        public int? FlightTypeDU { get; set; }
        public int? FlightTypeLU { get; set; }
        public string LandingDateU { get; set; }
        public string TakeOffDateU { get; set; }
        public int? StatusU { get; set; }
        public string DateU { get; set; }
        public Nullable<System.TimeSpan> ATAU { get; set; }
        public Nullable<System.TimeSpan> ATDU { get; set; }

        public Nullable<int> TripTypeIdU { get; set; }

        public Nullable<int> CompanyInfoIdU { get; set; }
        public string NoteU { get; set; }
        public string CreatedByU { get; set; }
        public Nullable<System.DateTime> CreationDateU { get; set; }
        public string UpdatedByU { get; set; }
        public Nullable<System.DateTime> UpdatingDateU { get; set; }


    }





    public class CompanyInfoModel
    {
        public int Id { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string Code { get; set; }

        public bool IsSingleAccounting { get; set; }

    }



    ///----------------------------Strart Air

    public class AirTakeOffModel
    {
        public int IdDe { get; set; }
        public int AirLineIdDe { get; set; }
        public int FlightNoDe { get; set; }
        public int AircraftRegIdDe { get; set; }
        public Nullable<int> AirportIdFromDe { get; set; }
        public Nullable<int> AirportIdToDe { get; set; }
        public int FlightTypeD { get; set; }
        public int FlightTypeL { get; set; }
        public string LandingDateDe { get; set; }
        public string TakeOffDateDe { get; set; }
        public int Status { get; set; }
        public string DateDe { get; set; }
        public Nullable<System.TimeSpan> ATADe { get; set; }
        public Nullable<System.TimeSpan> ATDDe { get; set; }
        public string ATDDeShow { get; set; }

        public Nullable<int> POBDe { get; set; }
        public Nullable<int> QBDDe { get; set; }
        public int TripTypeIdDe { get; set; }
        public string LandingPermissionDe { get; set; }
        public Nullable<int> CompanyInfoId { get; set; }
        public string NoteDe { get; set; }
        public string CreatedByDe { get; set; }
        public string CreationDateDe { get; set; }
        public string UpdatedByDe { get; set; }
        public Nullable<System.DateTime> UpdatingDateDe { get; set; }
    }


    public class AirArriveModel
    {
        public int IdAr { get; set; }
        public int AirLineIdAr { get; set; }
        public int FlightNoAr { get; set; }
        public int AircraftRegIdAr { get; set; }
        public Nullable<int> AirportIdFromAr { get; set; }
        public Nullable<int> AirportIdToAr { get; set; }
        public int FlightTypeD { get; set; }
        public int FlightTypeL { get; set; }
        public string LandingDateAr { get; set; }
        public string TakeOffDateAr { get; set; }
        public int StatusAr { get; set; }
        public string DateAr { get; set; }
        public Nullable<System.TimeSpan> ATAAr { get; set; }
        public string ATAArShow { get; set; }
        public string ATDArShow { get; set; }
        public Nullable<System.TimeSpan> ATDAr { get; set; }
        public Nullable<int> POBAr { get; set; }
        public Nullable<int> QBDAr { get; set; }
        public int TripTypeIdAr { get; set; }
        public string LandingPermissionAr { get; set; }
        public Nullable<int> CompanyInfoId { get; set; }
        public string NoteAr { get; set; }
        public string CreatedByAr { get; set; }
        public string CreationDateAr { get; set; }
        public string UpdatedByAr { get; set; }
        public Nullable<System.DateTime> UpdatingDateAr { get; set; }


        //-------------------------------------------------

        public Nullable<int> FreightLoadingExp { get; set; }
        public Nullable<int> FreightLoadingImp { get; set; }
        public Nullable<decimal> NormalMailLoadingExp { get; set; }
        public Nullable<decimal> RapidMailLoadingExp { get; set; }
        public Nullable<decimal> NormalMailLoadingImp { get; set; }
        public Nullable<decimal> RapidMailLoadingImp { get; set; }
        public Nullable<int> AmbulanceCarCount { get; set; }
        public Nullable<int> FireFightingCarCount { get; set; }

        public Nullable<int> Pax { get; set; }
        public Nullable<int> TRNYCUPPS { get; set; }

        public Nullable<bool> HasPushPack { get; set; }

        public Nullable<decimal> FuelLiter { get; set; }
      
        
    }



    public class UnScheduledModel
    {
        public int IdAr { get; set; }
        public int AirLineIdAr { get; set; }
        public int FlightNoAr { get; set; }
        public int AircraftRegIdAr { get; set; }
        public Nullable<int> AirportIdFromAr { get; set; }
        public Nullable<int> AirportIdToAr { get; set; }
        public int FlightTypeD { get; set; }
        public int FlightTypeL { get; set; }
        public string LandingDateAr { get; set; }
        public string TakeOffDateAr { get; set; }
        public int StatusAr { get; set; }
        public string DateAr { get; set; }
        public Nullable<System.TimeSpan> ATAAr { get; set; }
        public string ATAArShow { get; set; }

        public Nullable<System.TimeSpan> ATDAr { get; set; }
        public Nullable<int> POBAr { get; set; }
        public Nullable<int> QBDAr { get; set; }
        public int TripTypeIdAr { get; set; }
        public string LandingPermissionAr { get; set; }
        public Nullable<int> CompanyInfoId { get; set; }
        public string NoteAr { get; set; }
        public string CreatedByAr { get; set; }
        public string CreationDateAr { get; set; }
        public string UpdatedByAr { get; set; }
        public Nullable<System.DateTime> UpdatingDateAr { get; set; }

        public int? Pax { get; set; }
        public int? TRNYCUPPS { get; set; }
        public bool? IsNotUsedCUPPS { get; set; }

        public int? AmbulanceCarCount { get; set; }
        public int? FireFightingCarCount { get; set; }

     


        public decimal? NormalMailLoadingExp { get; set; }
        public decimal? RapidMailLoadingExp { get; set; }

        public decimal? NormalMailLoadingImp { get; set; }
        public decimal? RapidMailLoadingImp { get; set; }
        public int? FreightLoadingExp { get; set; }
        public int? FreightLoadingImp { get; set; }
        public decimal? FuelLiter { get; set; }

        
        public bool? hasPushPack { get; set; }
        

    }








    public class AirMakeArrivedModel
    {
        public int IdMArrived { get; set; }
        public int AirLineIdMArrived { get; set; }
        public int FlightNoMArrived { get; set; }
        public int AircraftRegIdMArrived { get; set; }
        public Nullable<int> AirportIdFromMArrived { get; set; }
        public Nullable<int> AirportIdToMArrived { get; set; }
        public int FlightTypeD { get; set; }
        public int FlightTypeL { get; set; }
        public string LandingDateMArrived { get; set; }
        public string TakeOffDateMArrived { get; set; }
        public int StatusMArrived { get; set; }
        public string DateMArrived { get; set; }
        public Nullable<System.TimeSpan> ATAMArrived { get; set; }
        public Nullable<System.TimeSpan> ATDMArrived { get; set; }
        public string ATDMArrivedShow { get; set; }
        public string ATAMArrivedShow { get; set; }

        public Nullable<int> POBMArrived { get; set; }
        public Nullable<int> QBDMArrived { get; set; }
        public int TripTypeIdMArrived { get; set; }
        public string LandingPermissionMArrived { get; set; }
        public Nullable<int> CompanyInfoId { get; set; }
        public string NoteMArrived { get; set; }
        public string CreatedByMArrived { get; set; }
        public string CreationDateMArrived { get; set; }
        public string UpdatedByMArrived { get; set; }
        public Nullable<System.DateTime> UpdatingDateMArrived { get; set; }
    }

    public class AirMakeTakeOffModel
    {
        public int IdMTakeOff { get; set; }
        public int AirLineIdMTakeOff { get; set; }
        public int FlightNoMTakeOff { get; set; }
        public int AircraftRegIdMTakeOff { get; set; }
        public Nullable<int> AirportIdFromMTakeOff { get; set; }
        public Nullable<int> AirportIdToMTakeOff { get; set; }
        public int FlightTypeD { get; set; }
        public int FlightTypeL { get; set; }
        public string LandingDateMTakeOff { get; set; }
        public string TakeOffDateMTakeOff { get; set; }
        public int StatusMTakeOff { get; set; }
        public string DateMTakeOff { get; set; }
        public Nullable<System.TimeSpan> ATAMTakeOff { get; set; }
        public Nullable<System.TimeSpan> ATDMTakeOff { get; set; }
        public Nullable<int> POBMTakeOff { get; set; }
        public string ATAMTakeOffShow { get; set; }
        public string ATDMTakeOffShow { get; set; }

        public Nullable<int> QBDMTakeOff { get; set; }
        public int TripTypeIdMTakeOff { get; set; }
        public string LandingPermissionMTakeOff { get; set; }
        public Nullable<int> CompanyInfoId { get; set; }
        public string NoteMTakeOff { get; set; }
        public string CreatedByMTakeOff { get; set; }
        public string CreationDateMTakeOff { get; set; }
        public string UpdatedByMTakeOff { get; set; }
        public Nullable<System.DateTime> UpdatingDateMTakeOff { get; set; }
    }














    public class AirCompletedModel
    {
        public int IdCompleted { get; set; }
        public int AirLineIdCompleted { get; set; }
        public int FlightNoCompleted { get; set; }
        public int AircraftRegIdCompleted { get; set; }
        public Nullable<int> AirportIdFromCompleted { get; set; }
        public Nullable<int> AirportIdToCompleted { get; set; }
        public int FlightTypeDCompleted { get; set; }
        public int FlightTypeLCompleted { get; set; }
        public string LandingDateCompleted { get; set; }
        public string TakeOffDateCompleted { get; set; }
        public int StatusCompleted { get; set; }
        public string DateCompleted { get; set; }
        public Nullable<System.TimeSpan> ATACompleted { get; set; }
        public string ATACompletedShow { get; set; }

        public Nullable<System.TimeSpan> ATDCompleted { get; set; }
        public string ATDCompletedShow { get; set; }

        public Nullable<int> POBCompleted { get; set; }
        public Nullable<int> QBDCompleted { get; set; }
        public int TripTypeIdCompleted { get; set; }
        public string LandingPermissionCompleted { get; set; }
        public Nullable<int> CompanyInfoId { get; set; }
        public string NoteCompleted { get; set; }
        public string CreatedByCompleted { get; set; }
        public string CreationDateCompleted { get; set; }
        public string UpdatedByCompleted { get; set; }
        public Nullable<System.DateTime> UpdatingDateCompleted { get; set; }
    }









    public class RevenueDataViewModel
    {
        public string LandingFees { get; set; }
        public string ParkingFees { get; set; }
        public string NightOperationFees { get; set; }
        public string NavigationFees { get; set; }
        public string SecurityFees { get; set; }
        public string NoiseFees { get; set; }
    }


    public class PrintDailyViewModel
    {
        public int Id { get; set; }
        public int N { get; set; }
        public string Call { get; set; }
        public string Operation { get; set; }
        public string FlightNo { get; set; }
        public string Reg { get; set; }
        public string Type { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public int FlightTypeD { get; set; }
        public int FlightTypeL { get; set; }
        public Nullable<System.DateTime> LandingDate { get; set; }
        public Nullable<System.DateTime> TakeOffDate { get; set; }
        public int Status { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string ATA { get; set; }
        public string ATD { get; set; }
        public string POB { get; set; }
        public string QBD { get; set; }
        public int? Pax { get; set; }
        public int? Tren { get; set; }
        public int? AmbulanceCarCount { get; set; }
        public int? FireFightingCarCount { get; set; }
        public int? FirstClassCount { get; set; }
        public bool? HasPushPack { get; set; }

        public decimal? FreightImp { get; set; }
        public decimal? FreightExp { get; set; }
        public decimal? NormalMailExp { get; set; }
        public decimal? NormalMailImp { get; set; }

        public decimal? RapidMailImp { get; set; }
        public decimal? RapidMailExp { get; set; }

        public int TripTypeId { get; set; }
        public string LandingPermission { get; set; }
        public Nullable<int> CompanyInfoId { get; set; }
        public string Note { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreationDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatingDate { get; set; }
    }













    ///-------------------------End Air

    public class WorkOnModel
    {
        public int Id { get; set; }

        public int? YearWorkOn { get; set; }
        public int? MouthWorkOn { get; set; }

        public string UserId { get; set; }
        public int? CompanyInfoId { get; set; }

        // 👇 فقط للعرض
        public string MonthYearDisplay { get; set; }
    }


    public class MonthDto
    {
        public int Id { get; set; }      // Value
        public string Name { get; set; } // Text
    }


    public class YearDto
    {
        public int Id { get; set; }
        public int Year { get; set; }
    }
}