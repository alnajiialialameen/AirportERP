using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AirPortERP.ViewModels
{
    public class OfficerDataViewModels
    {
        public int Id { get; set; }
        public string AirLineName { get; set; }
        public string FlightNo { get; set; }
        public string AircraftRegName { get; set; }
        public string AirportIdFrom { get; set; }
        public string AirportIdTo { get; set; }

        public string Type { get; set; }

        public string Date { get; set; }
        //Nullable<System.TimeSpan>
        public string ATA { get; set; }
        public string ATD { get; set; }
        public Nullable<int> POB { get; set; }
        public Nullable<int> QBD { get; set; }
        public string TripType { get; set; }
        public string LandingPermission { get; set; }
        public Nullable<int> CompanyInfoId { get; set; }
        public string TowerDataCreatedBy { get; set; }
        public string TowerDataCreationDate { get; set; }
        public string TowerDataUpdatedBy { get; set; }
        public string TowerDataUpdatingDate { get; set; }

        public string LandingDate { get; set; }
        public string TakeOffDate { get; set; }


        //public int Id { get; set; }
        public Nullable<int> TowerDataId { get; set; }
        public Nullable<int> CrewNO { get; set; }
        public string HasNightStop { get; set; }
        public string HandlingAgentName { get; set; }
        public Nullable<int> TRNYCUPPS { get; set; }
        public Nullable<int> TRNY { get; set; }
        public Nullable<int> TRNone { get; set; }
        public Nullable<int> EMB { get; set; }
        public Nullable<int> EmbInft { get; set; }
        public Nullable<int> DISEMB { get; set; }
        public Nullable<int> DisEmbInft { get; set; }
        //public Nullable<System.DateTime> Date { get; set; }
        public Nullable<int> FrightLoadingExp { get; set; }
        public Nullable<int> FrightLoadingImp { get; set; }
        public Nullable<int> NormalMailLoadingExp { get; set; }
        public Nullable<int> RapidMailLoadingExp { get; set; }
        public Nullable<int> NormalMailLoadingImp { get; set; }
        public Nullable<int> RapidMailLoadingImp { get; set; }
        public Nullable<int> AmbulanceCarCount { get; set; }
        public Nullable<int> FireFightingCarCount { get; set; }
        public Nullable<int> ServiceCarReqId { get; set; }
        public string IsCurrentAccount { get; set; }
        public string AireLineAgentName { get; set; }


        
        //  public Nullable<int> CompanyInfoId { get; set; }
        public string OfficerDataCreatedBy { get; set; }
        public string OfficerDataCreationDate { get; set; }
        public string OfficerDataUpdatedBy { get; set; }
        public string OfficerDataUpdatingDate { get; set; }
    }



    public class OfficerDataViewModelsOnly
    {
        public int Id { get; set; }
       


      
        public Nullable<int> TowerDataId { get; set; }
        public Nullable<int> CrewNO { get; set; }
        public bool HasNightStop { get; set; }
        public Nullable<int> HandlingAgentId { get; set; }
     
        //public Nullable<int> AmbulanceCarCount { get; set; }
        //public Nullable<int> FireFightingCarCount { get; set; }
        public Nullable<int> ServiceCarReqId { get; set; }
       public bool IsCurrentAccount { get; set; }
        public Nullable<int> AireLineAgentId { get; set; }
        //  public Nullable<int> CompanyInfoId { get; set; }
        public string OfficerDataCreatedBy { get; set; }
        public Nullable<DateTime> OfficerDataCreationDate { get; set; }
        public string OfficerDataUpdatedBy { get; set; }
        public Nullable<DateTime> OfficerDataUpdatingDate { get; set; }
    }
}