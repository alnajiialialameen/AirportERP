using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AirPortERP.ViewModels
{
    public class NyAndCuppsFeesViewModelDTO
    {
        public int Id { get; set; }
        public int OfficerDataId { get; set; }
        //public int DisEmbInft { get; set; }
        public int TRNYCUPPS { get; set; }
        //public int TRNY { get; set; }
        public bool IsNotUsedCUPPS { get; set; }
        public int Pax { get; set; }
        //public int DISEMB { get; set; }
    }
    //public class CuppsFeesViewModelDTO
    //{
    //    public int Id { get; set; }
    //    public int OfficerDataId { get; set; }
    //    public int DisEmbInft { get; set; }
    //    public int TRNYCUPPS { get; set; }
    

    //    public int DISEMB { get; set; }
    //}

    public class FirstClassFeesViewModelDTO
    {
        public int Id { get; set; }
        public int OfficerDataId { get; set; }
     

        public int FirstClassCount { get; set; }
    }

    public class CarServiceFeesViewModelDTO
    {
        public int Id { get; set; }
        public int OfficerDataId { get; set; }


        public int AmbulanceCarCount { get; set; }
        public int FireFightingCarCount { get; set; }
    }

    public class FreightFeesViewModelDTO
    {
        public int Id { get; set; }
        public int OfficerDataId { get; set; }


        public int FrightLoadingExp { get; set; }
        public int FrightLoadingImp { get; set; }
    }
    public class MailFeesViewModelDTO
    {
        public int Id { get; set; }
        public int OfficerDataId { get; set; }


        public int NormalMailLoadingExpAmount { get; set; }
        public int RapidMailLoadingExpAmount { get; set; }


        public int NormalMailLoadingImpAmount { get; set; }
        public int RapidMailLoadingImpAmount { get; set; }
    }


    public class PushPackFeesViewModelDTO
    {
        public int Id { get; set; }
        public int OfficerDataId { get; set; }

        public bool hasPushPack { get; set; }
       

        public decimal PushPackFees { get; set; }
    }


    public class MailAndFreightFeesViewModelDTO
    {
        public int Id { get; set; }
        public int OfficerDataId { get; set; }

        public int RevenueId { get; set; }
        

        public decimal NormalMailLoadingExpAmount { get; set; }
        public decimal RapidMailLoadingExpAmount { get; set; }


        public decimal NormalMailLoadingImpAmount { get; set; }
        public decimal RapidMailLoadingImpAmount { get; set; }

        public int FrightLoadingExp { get; set; }
        public int FrightLoadingImp { get; set; }
    }



}






