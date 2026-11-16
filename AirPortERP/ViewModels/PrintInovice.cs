using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AirPortERP.ViewModels
{
    public class PrintFinalInvoiceViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string txtInvoiceNo { get; set; }

        public string HeaderDate { get; set; }

        public string DateFrom { get; set; }
        public string DateTo { get; set; }

        public double ExchangeRate { get; set; }

        public string DateNow { get; set; }


        public double LandingFees { get; set; }
        public double ParkingFees { get; set; }
        public double NightOperationFees { get; set; }

        public double NavigationFees { get; set; }

        public double SecurityFees { get; set; }

        public double FreightFees { get; set; }
        public double FuelFees { get; set; }
        
        public double FirstClassFees { get; set; }

        public double FireFightingFees { get; set; }
        public double AmbolanceFees { get; set; }
        public double InterCUPPSFees { get; set; }
        public double DomeCUPPSFees { get; set; }
        public double InterNYFees { get; set; }

        public double DomeNYFees { get; set; }
        public double PushBackFees { get; set; }
        public double ExpMailFees { get; set; }
        public double ImpMailFees { get; set; }
        public double ServiceCars { get; set; }


        public double NoiseFees { get; set; }
        public double SubTotalDollar { get; set; }

        public double SubTotalSDG { get; set; }
        public double GrandTotalDollar { get; set; }

        public double GrandTotalSDG { get; set; }

        public string GrandTotalSDGTxt { get; set; }
        public string GrandTotalDollarTxt { get; set; }
        
        public string NumberWord { get; set; }
      

        public double TaxDollar { get; set; }
        public double TaxSDG { get; set; }

     
        





        public double OpenAireportOvertimeFees { get; set; }
    }


    public class PrintFuelFinalInvoiceViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string txtInvoiceNo { get; set; }

        public string HeaderDate { get; set; }

        public string DateFrom { get; set; }
        public string DateTo { get; set; }

        public double ExchangeRate { get; set; }

        public string DateNow { get; set; }

        public double FuelFees { get; set; }
        public double FreightFees { get; set; }

        
        public double SubTotalDollar { get; set; }

        public double SubTotalSDG { get; set; }
        public double GrandTotalDollar { get; set; }

        public double GrandTotalSDG { get; set; }

        public string GrandTotalSDGTxt { get; set; }
        public string GrandTotalDollarTxt { get; set; }

        public string NumberWord { get; set; }


        public double TaxDollar { get; set; }
        public double TaxSDG { get; set; }








        public double OpenAireportOvertimeFees { get; set; }
    }


    public class PrintLandingViewModel
    {
        public int Id { get; set; }
        public int N { get; set; }
        public string Name { get; set; }
        public decimal LandingFees { get; set; }
       
        public decimal ParkingFees { get; set; }
       

        public string FlightNo { get; set; }
        public decimal NightOperationFees { get; set; }
       
        public decimal NavigationFees { get; set; }
      

        public decimal SecurityFees { get; set; }
       

        public decimal FreightFees { get; set; }
      
        public string MaxTakoffWieght { get; set; }

        public string RegNo { get; set; }
        public string Wt { get; set; }
        public string Type { get; set; }

        public string LandingDate { get; set; }

        public decimal Total { get; set; }
        public decimal NoiseFees { get; set; }

        public decimal ExpMailFees { get; set; }
     

        public decimal ImpMailFees { get; set; }
       

    }

    public class PrintInoviceDetailViewModel
    {
        public int Id { get; set; }
        public int N { get; set; }
        public string Name { get; set; }
        public decimal LandingFees { get; set; }
        public decimal ExchangeRateAgainstPrimaryCurrency { get; set; }

        
        public decimal ParkingFees { get; set; }


        public string FlightNo { get; set; }
        public string AirPortTo { get; set; }
     
        public decimal NightOperationFees { get; set; }

        public decimal NavigationFees { get; set; }


        public decimal SecurityFees { get; set; }


        public decimal FreightFees { get; set; }

        public string MaxTakoffWieght { get; set; }

        public string RegNo { get; set; }
        public string Wt { get; set; }
        public string Type { get; set; }

        public string LandingDate { get; set; }
        public string Date { get; set; }

        public decimal TotalLanding { get; set; }
        public decimal TotalInvoice { get; set; }
        public decimal TotalRevenueStampInvoice { get; set; }

        public decimal GrandTotalInvoice { get; set; }
        public decimal NoiseFees { get; set; }

        public decimal ExpMailFees { get; set; }


        public decimal ImpMailFees { get; set; }

        ///---------------------------------------
        public decimal CUPPSSDG { get; set; }

        public int Pax { get; set; }

        public int TR { get; set; }

        public decimal CFees { get; set; }
        public decimal DomeFees { get; set; }

        public string TakeOffDate { get; set; }

        public decimal CuppsFees { get; set; }

        public decimal DomeCuppsFees { get; set; }


        //------------Car------------------
        public int AmbulanceNo { get; set; }
        public decimal FeesAmbulance { get; set; }
        public decimal FeesAmbulanceAmount { get; set; }

        public int FireNo { get; set; }
        public decimal FeesFire { get; set; }

        public decimal ServiceCarSDG { get; set; }
        public decimal ServiceCar { get; set; }
        
        public decimal FeesFireAmount { get; set; }


        public decimal AmbulanceSDG { get; set; }
        public decimal FireSDG { get; set; }


        //---------pushpack
        public decimal PushPackFees { get; set; }
        public decimal PFees { get; set; }
        public string PlaneSize { get; set; }
        public decimal PushPackAmountSDG { get; set; }
        ///---------------First Class
        public int FirstClassCount { get; set; }
        public decimal FirstClassFees { get; set; }

        public decimal FirstClassFeesSDG { get; set; }

        public decimal FFees { get; set; }
        ///------------Ny
        public decimal NyFees { get; set; }

        public decimal NyAmountSDG { get; set; }
        public decimal NyAmountDollar { get; set; }
        public decimal NFees { get; set; }

        public bool HasPushPack { get; set; }
        public bool HasCarService { get; set; }

        public bool HasNy { get; set; }
        public bool HasCupps { get; set; }

        public bool HasFirstClass { get; set; }

        //---------pushpack
        public decimal FuelFees { get; set; }
        public decimal FuelLiter { get; set; }
    

    }

    public class PrintCarServiceViewModel
    {
        public int Id { get; set; }
        public int N { get; set; }
        public string LandingDate { get; set; }
        public string FlightNo { get; set; }


        public string Name { get; set; }

        public int AmbulanceNo { get; set; }
        public decimal FeesAmbulance { get; set; }
        public decimal FeesAmbulanceAmount { get; set; }

        public int FireNo { get; set; }
        public decimal FeesFire { get; set; }
        public decimal FeesFireAmount { get; set; }


        public decimal AmbulanceSDG { get; set; }
        public decimal FireSDG { get; set; }



    }


    public class PrintPushPackViewModel
    {
        public int Id { get; set; }

        public int N { get; set; }

        public string Name { get; set; }

        public string AirPortTo { get; set; }

        public string FlightNo { get; set; }

        //public int Pax { get; set; }

        //  public int TR { get; set; }

        public decimal PushPackFees { get; set; }
        public decimal Fees { get; set; }

        public string LandingDate { get; set; }

        public string RegNo { get; set; }

        public string Type { get; set; }

        public string PlaneSize { get; set; }

        public string Wt { get; set; }

        public decimal AmountSDG { get; set; }


    }


    public class PrintFirstClassViewModel
    {
        public int Id { get; set; }
        public int N { get; set; }
        public string Name { get; set; }
        public string FlightNo { get; set; }


        public int FirstClassCount { get; set; }
        public decimal FirstClassFees { get; set; }

        public decimal FirstClassFeesSDG { get; set; }

        public decimal Fees { get; set; }

        public string TakeOffDate { get; set; }




    }


    public class PrintNyViewModel
    {
        public int Id { get; set; }
        public int N { get; set; }
        public string Name { get; set; }

        public string AirPortTo { get; set; }

        public string FlightNo { get; set; }

        public int Pax { get; set; }
        public int PaxInter { get; set; }
        public string Pax2 { get; set; }
        public decimal InterFees { get; set; }
        public decimal DomeFees { get; set; }
        public string TakeOffDate { get; set; }

        public decimal AmountSDG { get; set; }
        public decimal AmountDollar { get; set; }
        public decimal InterNYFees { get; set; }

        public decimal DomeNYFeesSDG { get; set; }


    }

    public class PrintCuppsViewModel
    {
        public int Id { get; set; }
        public int N { get; set; }
        public string Name { get; set; }
        public string Registration { get; set; }

        public string AirPortTo { get; set; }

        public string FlightNo { get; set; }

        public decimal CUPPSSDG { get; set; }

        public int Pax { get; set; }
        public int PaxInter { get; set; }
        public string Pax2 { get; set; }
        public int TR { get; set; }

        public decimal InterFees { get; set; }
        public decimal DomeFees { get; set; }

        public string TakeOffDate { get; set; }

        public decimal InterCuppsFees { get; set; }

        public decimal DomeCuppsFees { get; set; }

        public decimal? ExchangeRate { get; set; }
    }

    public class PrintCuppsNoDataViewModel
    {
        public int Id { get; set; }
        public int N { get; set; }
        public string Name { get; set; }
        public string Registration { get; set; }

        public string AirPortTo { get; set; }

        public string FlightNo { get; set; }

        public decimal CUPPSSDG { get; set; }

        public int Pax { get; set; }

        public int TR { get; set; }

        public decimal InterFees { get; set; }
        public decimal DomeFees { get; set; }

        public string TakeOffDate { get; set; }

        public decimal InterCuppsFees { get; set; }

        public decimal DomeCuppsFees { get; set; }


    }


    public class PrintMailViewModel
    {
        public int Id { get; set; }
        public int N { get; set; }
        public string LandingDate { get; set; }
        public string FlightNo { get; set; }


        public string Name { get; set; }

        public double FreightLoadingExp { get; set; }
        public double FreightLoadingImp { get; set; }

        public double NormalMailLoadingExp { get; set; }
        public double RapidMailLoadingExp { get; set; }


        public double NormalMailLoadingImp { get; set; }
        public double RapidMailLoadingImp { get; set; }

        public decimal ExpMailFees { get; set; }

        public decimal ImpMailFees { get; set; }


        public decimal FreightFees { get; set; }


    }



    public class PrintTotalCompaniesInterCUPPSModel
    {
        public int Id { get; set; }

        public int N { get; set; }
        public String Name { get; set; }
        public String Code { get; set; }
        public decimal ExchRate { get; set; }
        public int Pax { get; set; }
        public double TotalAmountDollar { get; set; }

        public double TotalAmountSDG { get; set; }
        public double TaxDollar { get; set; }
        public double TaxSDG { get; set; }
        public double GrandTotalDollar { get; set; }
        public double GrandTotalSDG { get; set; }
        public decimal Amount { get; set; }
        public decimal KHPercent { get; set; }
        public decimal RightPercent { get; set; }
    }
    public class PrintJournalViewModel
    {
        public decimal TotalAmountDollar { get; set; }
        public string Name { get; set; }
    }


    public class PrintTotalCompaniesInterCUPPSviewModel
    {
        public int Id { get; set; }

        public int N { get; set; }
        public string Name { get; set; }
      
        public decimal ExchRate { get; set; }
        public int Pax { get; set; }
        public double TotalAmountDollar { get; set; }
        //public decimal TaxDollar { get; set; }
        public decimal GrandTotalDollar { get; set; }
        public decimal Amount { get; set; }
        //public double GrandTotalSDG { get; set; }
        public double TotalAmountSDG { get; set; }
        //public double TaxSDG { get; set; }
    }


    public class PrintTotalCompaniesInterCUPPSViewModel
    {
        public int N { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
      
        public decimal ExchangeRate { get; set; }

        public int Pax { get; set; }


        public double TotalAmountDollar { get; set; }
        public double TaxDollar { get; set; }
        public double GrandTotalDollar { get; set; }

        public double TotalAmountSDG { get; set; }
        public double TaxSDG { get; set; }
        public double GrandTotalSDG { get; set; }







        public decimal Amount { get; set; }
        public decimal KHPercent { get; set; }
        public decimal RightPercent { get; set; }
    }

    public class PrintJournalCuppsDetailsViewModel
    {
        public decimal TotalAmountDollar { get; set; }
        public string Name { get; set; }
    }

    public class PrintTotalCompaniesInterCUPPS
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int N { get; set; }
        public decimal ExchRate { get; set; }
        public int Pax { get; set; }
        public double TotalAmountDollar { get; set; }
        public double TotalAmountSDG { get; set; }
        
        public double TaxDollar { get; set; }
        public double GrandTotalDollar { get; set; }
        public double Amount { get; set; }
        public double KHPercent { get; set; }
        public double RightPercent { get; set; }
    }


    public class PrintServiceCarViewModel
    {
        public int Id { get; set; }
        public int N { get; set; }
        public double AmbolanceFees { get; set; }
        public double FireFightingFees { get; set; }

        public double AmbolanceAndFireFees { get; set; }

        public double AmbolanceAndFireFeesSDG { get; set; }


        public int TotalACarCount { get; set; }
        public int TotalFCarCount { get; set; }
        public string Name { get; set; }
        public string ArName { get; set; }
        public string Code { get; set; }
    }

    public class PrintTotalCompaniesPushPackModel
    {
        public int Id { get; set; }
        public int N { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public decimal ExchRate { get; set; }
        public int Pax { get; set; }
        public double TotalAmountDollar { get; set; }
        public double TotalAmountSDG { get; set; }

        public double TaxDollar { get; set; }
        public double GrandTotalDollar { get; set; }
        public double GrandTotalSDG { get; set; }
        public double TaxSDG { get; set; }
        public decimal KHPercent { get; set; }
        public decimal RightPercent { get; set; }
    }


    public class PrintAllFeesViewModel
    {
        public int Id { get; set; }

        public int N { get; set; }
        public string Name { get; set; }

        public string txtInvoiceNo { get; set; }

        public string HeaderDate { get; set; }

        public string DateFrom { get; set; }
        public string DateTo { get; set; }

        public double ExchangeRate { get; set; }

        public string DateNow { get; set; }


        public double LandingFees { get; set; }
        public double ParkingFees { get; set; }
        public double NightOperationFees { get; set; }

        public double NavigationFees { get; set; }

        public double SecurityFees { get; set; }

        public double FreightFees { get; set; }

        public double FirstClassFees { get; set; }

        public double FireFightingFees { get; set; }
        //public double CarService { get; set; }
        public double CuppsFees { get; set; }
      //  public double DomeCUPPSFees { get; set; }
        public double NyFees { get; set; }

       // public double DomeNYFees { get; set; }
        public double PushBackFees { get; set; }
       // public double ExpMailFees { get; set; }
        public double MailFees { get; set; }
        public double ServiceCars { get; set; }


        public double NoiseFees { get; set; }



        public double TotalWithoutNy { get; set; }
        public double Total { get; set; }

        public double GrandTotal { get; set; }

       


        public double Tax { get; set; }
     








        public double OpenAireportOvertimeFees { get; set; }




    }




    public class PrintGroundHandlingViewModel
    {
        public int Id { get; set; }
        public int N { get; set; }
        public string Name { get; set; }
        public string FlightNo { get; set; }

        public string AirCraftReg { get; set; }
        public string AirCraftType { get; set; }
        public string AirCraftWieght { get; set; }

        public decimal FuelLiter { get; set; }
        
        public string AirlineCode { get; set; }
        public decimal GroundHandlingFees { get; set; }

        public decimal GroundHandlingFeeSDG { get; set; }

        public decimal Fees { get; set; }

        public string TakeOffDate { get; set; }
    }

    public class RepoObj
    {
        public int TripCount { get; set; }
        public int Month { get; set; }
        public int TotalPax { get; set; }
        public decimal? TotalFreightIn { get; set; }
        public decimal? TotalFreightOut { get; set; }
    }
}