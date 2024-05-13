using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AirPortERP.ViewModels
{
    public class NyRptViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string AirPortTo { get; set; }

        public string FlightNo { get; set; }

        public int Pax { get; set; }

        public decimal InterFees { get; set; }
        public decimal DomeFees { get; set; }
        public string TakeOffDate { get; set; }

        public decimal AmountSDG { get; set; }
        public decimal AmountDollar { get; set; }
        public decimal InterNYFees { get; set; }

        public decimal DomeNYFees { get; set; }

        
    }



    public class CuppsRptViewModel
    {
        public int Id { get; set; }

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

    public class CuppsRptViewModelD
    {
      

        public int Pax { get; set; }
        public int TR { get; set; }
        public decimal AmountDollar { get; set; }
        public decimal AmountSDG { get; set; }

        public decimal TaxSDG { get; set; }
        public decimal TaxDollar { get; set; }
        public decimal GrandTotalDollar { get; set; }
        public decimal GrandTotalSDG { get; set; }

      

    


    }
    public class CarServiceRptViewModel
    {
        public int Id { get; set; }

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


    public class LandingRptViewModel
    {
        public int Id { get; set; }
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


    public class FinalInvoiceRptViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string txtInvoiceNo { get; set; }

        public string HeaderDate { get; set; }

        public string DateFrom { get; set; }
        public string DateTo { get; set; }

        public decimal ExchangeRate { get; set; }

        


        public decimal LandingFees { get; set; }
        public decimal ParkingFees { get; set; }
        public decimal NightOperationFees { get; set; }

        public decimal NavigationFees { get; set; }

        public decimal SecurityFees { get; set; }

        public decimal FreightFees { get; set; }

        public decimal FirstClassFees { get; set; }

        public decimal FireFightingFees { get; set; }
        public decimal AmbolanceFees { get; set; }
        public decimal InterCUPPSFees { get; set; }
        public decimal DomeCUPPSFees { get; set; }
        public decimal InterNYFees { get; set; }

        public decimal DomeNYFees { get; set; }
        public decimal PushBackFees { get; set; }
        public decimal ExpMailFees { get; set; }
        public decimal ImpMailFees { get; set; }
        public decimal ServiceCars { get; set; }


        public decimal NoiseFees { get; set; }
        public decimal SubTotal { get; set; }
        public decimal GrandTotalDollar { get; set; }

        public decimal GrandTotalSDG { get; set; }
        public string GrandTotalSDGTxt { get; set; }
        public string GrandTotalDollarTxt { get; set; }

        public decimal Tax { get; set; }

        public decimal OpenAireportOvertimeFees { get; set; }
    }

    public class FirstClassRptViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FlightNo { get; set; }


        public int FirstClassCount { get; set; }
        public decimal FirstClassFees { get; set; }

        public decimal FirstClassFeesSDG { get; set; }

        public decimal Fees { get; set; }

        public string TakeOffDate { get; set; }

       


    }

    public class PushPackRptViewModel
    {
        public int Id { get; set; }

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
    public class FinalRptViewModelTax
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal LandingFees { get; set; }
        public decimal ParkingFees { get; set; }
        public decimal NightOperationFees { get; set; }

        public decimal NavigationFees { get; set; }

        public decimal SecurityFees { get; set; }

        public decimal FreightFees { get; set; }

        public decimal FirstClassFees { get; set; }

        public decimal FireFightingFees { get; set; }
        public decimal AmbolanceFees { get; set; }
        public decimal InterCUPPSFees { get; set; }
        public decimal DomeCUPPSFees { get; set; }
        public decimal InterNYFees { get; set; }

        public decimal DomeNYFees { get; set; }
        public decimal PushBackFees { get; set; }
        public decimal ExpMailFees { get; set; }
        public decimal ImpMailFees { get; set; }
        public decimal ServiceCars { get; set; }


        public decimal NoiseFees { get; set; }
        public decimal SubTotal { get; set; }
        public decimal GrandTotal { get; set; }

        public decimal Tax { get; set; }

        public decimal OpenAireportOvertimeFees { get; set; }
    }


    public class MailRptViewModel
    {
        public int Id { get; set; }

        public string LandingDate { get; set; }
        public string FlightNo { get; set; }


        public string Name { get; set; }

        public int FreightLoadingExp { get; set; }
        public int FreightLoadingImp { get; set; }

        public int NormalMailLoadingExp { get; set; }
        public int RapidMailLoadingExp { get; set; }

       
        public int NormalMailLoadingImp { get; set; }
        public int RapidMailLoadingImp { get; set; }

        public decimal ExpMailFees { get; set; }

        public decimal ImpMailFees { get; set; }


        public decimal FreightFees { get; set; }


    }

    public class RptExtenstionInternalCupps
    {
      

        public string Airline { get; set; }
        public int Pax { get; set; }

        public int Tr { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }

        public decimal CuppsFees { get; set; }
       


    }

}