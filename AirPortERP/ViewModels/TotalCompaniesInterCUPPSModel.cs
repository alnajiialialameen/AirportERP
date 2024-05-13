using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AirPortERP.ViewModels
{
    public class TotalCompaniesInterCUPPSModel
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public String Code { get; set; }    
        public decimal ExchRate { get; set; }
        public int Pax { get; set; }
        public decimal TotalAmountDollar { get; set; }
        public decimal TaxDollar { get; set; }
        public decimal GrandTotalDollar { get; set; }
        public decimal Amount { get; set; }
        public decimal KHPercent { get; set; }
        public decimal RightPercent { get; set; }
    }

    public class JournalViewModel
    {
        public decimal TotalAmountDollar { get; set; }
        public string Name { get; set; }
    }

    public class ServiceCarViewModel
    {
        public int Id { get; set; }
        public decimal AmbolanceFees { get; set; }
        public decimal FireFightingFees { get; set; }
        public int TotalACarCount { get; set; }
        public int TotalFCarCount { get; set; }        
        public string Name { get; set; }
        public string ArName { get; set; }
        public string Code { get; set; }
    }
}