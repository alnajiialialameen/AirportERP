using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AirPortERP.ViewModels
{
    public class NyAndCuppsViewModelDto
    {
        public int Id { get; set; }
        public int TowerId { get; set; }
        public string AirlineName { get; set; }
        public string Date { get; set; }

        public string FlightType { get; set; }

        public string AirPortTo { get; set; }
        public string AirPortFrom { get; set; }

        public string FlightNo { get; set; }

        public string RegNo { get; set; } 
        public string TakeOffDate { get; set; }

        public string LandingDate { get; set; }

      
        public int OfficerDataId { get; set; }
        public int DisEmbInft { get; set; }

        public int Pax { get; set; }
        public string Pax2 { get; set; }
        public int TRNYCUPPS { get; set; }
        public int TRNY { get; set; }

        public int DISEMB { get; set; }

        public string InterNYFees { get; set; }
        public string DomeNYFees { get; set; }
        public string InterCUPPSFees { get; set; }
        public string DomeCUPPSFees { get; set; }

        public string MaxTakoffWieght { get; set; }
        public string Size { get; set; }

        public string Type { get; set; }
        public string ATA { get; set; }
        public string ATD { get; set; }
        public string AircraftRegName { get; set; }
        public bool IsStop { get; set; }
        public string AirportName { get; set; }
        
    }

    public class FirstClassViewModelDto
    {
        public int Id { get; set; }

        public string AirlineName { get; set; }
        public string Date { get; set; }
        public string TripType { get; set; }

        public int TowerId { get; set; }
        
        public string AirPortTo { get; set; }
        public string AirPortFrom { get; set; }

        public string FlightNo { get; set; }

        public string RegNo { get; set; }
        public string TakeOffDate { get; set; }

        public string LandingDate { get; set; }


        public int OfficerDataId { get; set; }
        public string FirstClassFees { get; set; }
        public int FirstClassCount { get; set; }

        public string MaxTakoffWieght { get; set; }
        public string Size { get; set; }

        public string Type { get; set; }
        public string ATA { get; set; }
        public string ATD { get; set; }
        public string AircraftRegName { get; set; }
        public bool IsStop { get; set; }

        public string AirportName { get; set; }
    }


    public class CarServiceViewModelDto
    {
        public int Id { get; set; }

        public string AirlineName { get; set; }
        public string Date { get; set; }
        public string TripType { get; set; }
        public int TowerId { get; set; }
        
        public string AirPortTo { get; set; }
        public string AirPortFrom { get; set; }

        public string FlightNo { get; set; }

        public string RegNo { get; set; }
        public string TakeOffDate { get; set; }

        public string LandingDate { get; set; }


        public int OfficerDataId { get; set; }
        public int AmbulanceCarCount { get; set; }
        public int FireFightingCarCount { get; set; }

        public decimal AmbulanceCarFees { get; set; }
        public decimal FireFightingCarFees { get; set; }


        public string MaxTakoffWieght { get; set; }
        public string Size { get; set; }

        public string Type { get; set; }
        public string ATA { get; set; }
        public string ATD { get; set; }
        public string AircraftRegName { get; set; }
        public bool IsStop { get; set; }
        public string AirportName { get; set; }
        
    }



    public class MailAndFreightViewModelDto
    {
        public int Id { get; set; }
        public int TowerId { get; set; }
        public string AirlineName { get; set; }
        public string TripType { get; set; }
        public string Date { get; set; }

        public string AirPortTo { get; set; }
        public string AirPortFrom { get; set; }

        public string FlightNo { get; set; }

        public string RegNo { get; set; }
        public string TakeOffDate { get; set; }

        public string LandingDate { get; set; }


        public int OfficerDataId { get; set; }
        public decimal ExpMailFees { get; set; }

        public decimal ImpMailFees { get; set; }
       

        public int FreightLoadingExp { get; set; }
        public int FreightLoadingImp { get; set; }

        public decimal NormalMailLoadingExp { get; set; }
        public decimal RapidMailLoadingExp { get; set; }

        public decimal NormalMailLoadingImp { get; set; }
        public decimal RapidMailLoadingImp { get; set; }

        public decimal FreightFees { get; set; }


        public string MaxTakoffWieght { get; set; }
        public string Size { get; set; }

        public string Type { get; set; }
        public string ATA { get; set; }
        public string ATD { get; set; }
        public string AircraftRegName { get; set; }
        public bool IsStop { get; set; }

        public string AirportName { get; set; }
    }

    public class PushPackViewModelDto
    {
        public int Id { get; set; }
        public int TowerId { get; set; }
        public string AirlineName { get; set; }
        public string Date { get; set; }

        public string FlightType { get; set; }

        public string AirPortTo { get; set; }
        public string AirPortFrom { get; set; }

        public string FlightNo { get; set; }

        public string RegNo { get; set; }
        public string TakeOffDate { get; set; }

        public string LandingDate { get; set; }


        public int OfficerDataId { get; set; }
        public string hasPushPack { get; set; }

        public decimal PushPackFees { get; set; }


        public string MaxTakoffWieght { get; set; }
        public string Size { get; set; }

        public string Type { get; set; }
        public string ATA { get; set; }
        public string ATD { get; set; }
        public string AircraftRegName { get; set; }
        public bool IsStop { get; set; }

        public string AirportName { get; set; }
        

    }



}