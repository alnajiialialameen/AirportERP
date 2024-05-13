using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AirPortERP.Models;
namespace AirPortERP.Extenstion
{
    public class RptExtention
    {
        Entities db = new Entities();
        CommonUsed CommonUsed = new CommonUsed();
        public int NyAndCuppCount(string id)
        {
         
            int Count = 0;
            bool HasMouth = CommonUsed.HasMouth(id);

            if (HasMouth)
            {
                int Mouth = CommonUsed.GetMouth(id);
                int Year = CommonUsed.GetYear(id);

                Count = db.Revenues.Where(x => x.DomeNYFees == null &&
                x.InterNYFees == null && x.DomeCUPPSFees == null && x.InterNYFees == null
                 &&
                x.OfficerData.TowerData.Date.Value.Month == Mouth &&
                x.OfficerData.TowerData.Date.Value.Year == Year).Count();
            }
            else {
                Count = db.Revenues.Where(x => x.DomeNYFees == null &&
               x.InterNYFees == null && x.DomeCUPPSFees == null && x.InterNYFees == null).Count();
            }


            return Count;
        }

        public int FirstClassCount(string id)
        {
            int Count = 0;
            bool HasMouth = CommonUsed.HasMouth(id);

            if (HasMouth)
            {
                int Mouth = CommonUsed.GetMouth(id);
                int Year = CommonUsed.GetYear(id);
                Count = db.Revenues.Where(x => x.FirstClassFees == null && 
                x.OfficerData.TowerData.Date.Value.Month == Mouth && 
                x.OfficerData.TowerData.Date.Value.Year == Year).Count();
            }
            else {

                Count = db.Revenues.Where(x => x.FirstClassFees == null).Count();
            }



            return Count;
        }

        public int MailAndFrieghtCount(string id)
        {

            int Count = 0;
            bool HasMouth = CommonUsed.HasMouth(id);

            if (HasMouth)
            {
                int Mouth = CommonUsed.GetMouth(id);
                int Year = CommonUsed.GetYear(id);
                Count = db.Revenues.Where(x => x.ExpMailFees == null && x.ImpMailFees == null
                 && x.FreightFees == null
                 &&
                x.OfficerData.TowerData.Date.Value.Month == Mouth &&
                x.OfficerData.TowerData.Date.Value.Year == Year).Count();
            }
            else {

                Count = db.Revenues.Where(x => x.ExpMailFees == null && x.ImpMailFees == null
                && x.FreightFees == null).Count();

            }
             

            return Count;
        }

        public int PushPackCount(string id)
        {

            int Count = 0;
            bool HasMouth = CommonUsed.HasMouth(id);

            if (HasMouth)
            {
                int Mouth = CommonUsed.GetMouth(id);
                int Year = CommonUsed.GetYear(id);
                Count = db.Revenues.Where(x => x.PushBackFees == null
                 &&
                x.OfficerData.TowerData.Date.Value.Month == Mouth &&
                x.OfficerData.TowerData.Date.Value.Year == Year).Count();
            }
            else
            {
                Count = db.Revenues.Where(x => x.PushBackFees == null).Count();
            }
            return Count;
        }
        public int CarServiceCount(string id)
        {

            int Count = 0;
            bool HasMouth = CommonUsed.HasMouth(id);

            if (HasMouth)
            {
                int Mouth = CommonUsed.GetMouth(id);
                int Year = CommonUsed.GetYear(id);
                Count = db.Revenues.Where(x => x.AmbolanceFees == null && x.FireFightingFees == null
                &&
                x.OfficerData.TowerData.Date.Value.Month == Mouth &&
                x.OfficerData.TowerData.Date.Value.Year == Year).Count();
            }
            else {
                Count = db.Revenues.Where(x => x.AmbolanceFees == null && x.FireFightingFees == null).Count();
            }
            return Count;
        }




      




        //public decimal FireFightingFeesSum()
        //{
        //    List<decimal> List = new List<decimal>();
        //    decimal Total = 0;
        //    foreach (Revenue r in db.Revenues)
        //    {
        //        List.Add(Convert.ToDecimal(r.FireFightingFees));
        //    }
        //    Total = List.Sum();
        //    db.SaveChanges();
        //    return Total;
        //}


        //public decimal AmbolanceFeesSum()
        //{
        //    List<decimal> List = new List<decimal>();
        //    decimal Total = 0;
        //    foreach (Revenue r in db.Revenues)
        //    {
        //        List.Add(Convert.ToDecimal(r.AmbolanceFees));
        //    }
        //    Total = List.Sum();
        //    db.SaveChanges();
        //    return Total;
        //}


        

        //public decimal InterCUPPSFeesSum()
        //{
        //    List<decimal> List = new List<decimal>();
        //    decimal Total = 0;
        //    foreach (Revenue r in db.Revenues)
        //    {
        //        List.Add(Convert.ToDecimal(r.InterCUPPSFees));
        //    }
        //    Total = List.Sum();
        //    db.SaveChanges();
        //    return Total;
        //}
        //public decimal DomeCUPPSFeesSum()
        //{
        //    List<decimal> List = new List<decimal>();
        //    decimal Total = 0;
        //    foreach (Revenue r in db.Revenues)
        //    {
        //        List.Add(Convert.ToDecimal(r.DomeCUPPSFees));
        //    }
        //    Total = List.Sum();
        //    db.SaveChanges();
        //    return Total;
        //}

     


        public decimal InterNYFeesSum()
        {
            List<decimal> List = new List<decimal>();
            decimal Total = 0;
            foreach (Revenue r in db.Revenues)
            {
                List.Add(Convert.ToDecimal(r.InterNYFees));
            }
            Total = List.Sum();
            db.SaveChanges();
            return Total;
        }
        public decimal DomeNYFeesSum()
        {
            List<decimal> List = new List<decimal>();
            decimal Total = 0;
            foreach (Revenue r in db.Revenues)
            {
                List.Add(Convert.ToDecimal(r.DomeNYFees));
            }
            Total = List.Sum();
            db.SaveChanges();
            return Total;
        }




        public decimal PushBackFeesSum()
        {
            List<decimal> List = new List<decimal>();
            decimal Total = 0;
            foreach (Revenue r in db.Revenues)
            {
                List.Add(Convert.ToDecimal(r.PushBackFees));
            }
            Total = List.Sum();
            db.SaveChanges();
            return Total;
        }
        public decimal ExpMailFeesSum()
        {
            List<decimal> List = new List<decimal>();
            decimal Total = 0;
            foreach (Revenue r in db.Revenues)
            {
                List.Add(Convert.ToDecimal(r.ExpMailFees));
            }
            Total = List.Sum();
            db.SaveChanges();
            return Total;
        }

        public decimal ImpMailFeesSum()
        {
            List<decimal> List = new List<decimal>();
            decimal Total = 0;
            foreach (Revenue r in db.Revenues)
            {
                List.Add(Convert.ToDecimal(r.ImpMailFees));
            }
            Total = List.Sum();
            db.SaveChanges();
            return Total;
        }


        public decimal NoiseFeesSum()
        {
            List<decimal> List = new List<decimal>();
            decimal Total = 0;
            foreach (Revenue r in db.Revenues)
            {
                List.Add(Convert.ToDecimal(r.NoiseFees));
            }
            Total = List.Sum();
            db.SaveChanges();
            return Total;
        }


        public decimal OpenAireportOvertimeFeesSum()
        {
            List<decimal> List = new List<decimal>();
            decimal Total = 0;
            foreach (Revenue r in db.Revenues)
            {
                List.Add(Convert.ToDecimal(r.OpenAireportOvertimeFees));
            }
            Total = List.Sum();
            db.SaveChanges();
            return Total;
        }



        public decimal ServiceCarsSum()
        {
            List<decimal> List = new List<decimal>();
            decimal Total = 0;
            foreach (Revenue r in db.Revenues)
            {
                List.Add(Convert.ToDecimal(r.FireFightingFees)+ Convert.ToDecimal(r.AmbolanceFees));
            }
            Total = List.Sum();
            db.SaveChanges();
            return Total;
        }




    }
}