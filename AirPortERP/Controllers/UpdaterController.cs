using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AirPortERP.Models;
using AirPortERP.Extenstion;
using System.Data.Entity;
using System.Globalization;

namespace AirPortERP.Controllers
{
    public class UpdaterController : Controller
    {
        Calculation Calculation = new Calculation();
        CommonUsed CommonUsed = new CommonUsed();
        Entities db = new Entities();

        public ActionResult UpdateClaims(int companyInfoId)
        {
            var res = CreateClaim(companyInfoId);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public bool CreateClaim(int companyInfoId)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var claims = db.FinancialClaim1.Where(q=> q.CompanyInfoId == companyInfoId).ToList();
                    //var c = db.FinancialClaim1.Find(1140);

                    foreach (var c in claims)
                    {
                        DateTime dateFrom = c.DateFrom.Value.Date;
                        DateTime dateTo = c.DateTo.Value.Date;

                        var airline = db.AirLines.FirstOrDefault(x => x.AccId == c.AccIdDebit);

                        if (airline == null)
                            return false;

                        decimal? exchangeRate = c.ExchangeRateAgainstPrimaryCurrency;

                        var claim = ClaimCalculation(airline.Id, c.DateFrom, c.DateTo, exchangeRate, airline.Name, companyInfoId);

                        if (claim.GrandTotalDollar <= 0)
                            continue;

                        var RevenuItems = db.AccountTrees.Where(q => q.AccParent == 56)
                                                                                    .Select(q => new { q.Id, q.AccName })
                                                                                    .ToList();

                        var revenueDictionary = RevenuItems
                            .ToDictionary(
                                x => x.Id,
                                x => x.AccName.Replace(" ", "").ToLower()
                            );

                        var newFinancialClaim = new FinancialClaim()
                        {
                            AccId = c.AccIdDebit,
                            ClaimDate = c.CreationDate,
                            DateFrom = c.DateFrom,
                            DateTo = c.DateTo,
                            FinancialClaimRefId = c.Id,
                            CurrencyId = c.CurrencyId,
                            CompanyInfoId = c.CompanyInfoId,
                            ExchangeRate = c.ExchangeRateAgainstPrimaryCurrency,
                            IsConfirm = true,
                            OperationTypeId = 3,
                            Statement = c.Statement,
                            TowerDataId = c.TowerDataId,
                            TotalAmount = claim.GrandTotalDollar,
                            TotalAmountSDG = claim.GrandTotalSDG,
                            CreationDate = c.CreationDate,
                            CreatedBy = c.CreatedBy
                        };

                        // إنشاء التفاصيل
                        var details = new List<FinancialClaimDetail>()
                                            {
                                                new FinancialClaimDetail { FinancialClaim = newFinancialClaim, Statement="Landing Fees",Amount=claim.LandingFees,AmountSDG=claim.LandingFeesSDG,Code=1 },
                                                new FinancialClaimDetail { FinancialClaim = newFinancialClaim, Statement="Parking Fees",Amount=claim.ParkingFees,AmountSDG=claim.ParkingFeesSDG,Code=2 },
                                                new FinancialClaimDetail { FinancialClaim = newFinancialClaim, Statement="NightOperation Fees",Amount=claim.NightOperationFees,AmountSDG=claim.NightOperationFeesSDG,Code=3 },
                                                new FinancialClaimDetail { FinancialClaim = newFinancialClaim, Statement="Navigation Fees",Amount=claim.NavigationFees,AmountSDG=claim.NavigationFeesSDG,Code=4 },
                                                new FinancialClaimDetail { FinancialClaim = newFinancialClaim, Statement="Security Fees",Amount=claim.SecurityFees,AmountSDG=claim.SecurityFeesSDG,Code=5 },
                                                new FinancialClaimDetail { FinancialClaim = newFinancialClaim, Statement="InterCUPPS Fees",Amount=claim.InterCUPPSFees,AmountSDG=claim.InterCUPPSFeesSDG,Code=6 },
                                                new FinancialClaimDetail { FinancialClaim = newFinancialClaim, Statement="DomeCUPPS Fees",Amount=claim.DomeCUPPSFees,AmountSDG=claim.DomeCUPPSFeesSDG,Code=7 },
                                                new FinancialClaimDetail { FinancialClaim = newFinancialClaim, Statement="InterNY Fees",Amount=claim.InterNYFees,AmountSDG=claim.InterNYFeesSDG,Code=8},
                                                new FinancialClaimDetail { FinancialClaim = newFinancialClaim, Statement="DomeNY Fees",Amount=claim.DomeNYFees,AmountSDG=claim.DomeNYFeesSDG,Code=9 },
                                                new FinancialClaimDetail { FinancialClaim = newFinancialClaim, Statement="PushBack Fees",Amount=claim.PushBackFees,AmountSDG=claim.PushBackFeesSDG,Code=10 },
                                                new FinancialClaimDetail { FinancialClaim = newFinancialClaim, Statement="Service Cars",Amount=claim.ServiceCars,AmountSDG=claim.ServiceCarsSDG,Code=11 },
                                                new FinancialClaimDetail { FinancialClaim = newFinancialClaim, Statement="Noise Fees",Amount=claim.NoiseFees,AmountSDG=claim.NoiseFeesSDG,Code=12 },
                                                new FinancialClaimDetail { FinancialClaim = newFinancialClaim, Statement="ExpMail Fees",Amount=claim.ExpMailFees,AmountSDG=claim.ExpMailFeesSDG,Code=13 },
                                                new FinancialClaimDetail { FinancialClaim = newFinancialClaim, Statement="ImpMail Fees",Amount=claim.ImpMailFees,AmountSDG=claim.ImpMailFeesSDG,Code=14 },
                                                new FinancialClaimDetail { FinancialClaim = newFinancialClaim, Statement="Freight Fees",Amount=claim.FreightFees,AmountSDG=claim.FreightFeesSDG,Code=15 },
                                                new FinancialClaimDetail { FinancialClaim = newFinancialClaim, Statement="FirstClass Fees",Amount=claim.FirstClassFees,AmountSDG=claim.FirstClassFeesSDG,Code= 16 },
                                                new FinancialClaimDetail { FinancialClaim = newFinancialClaim, Statement="Revenue Stamp(5%)",Amount=claim.TaxDollar,AmountSDG=claim.TaxSDG,Code=17 }
                                            };

                        foreach (var detail in details)
                        {
                            var itemName = detail.Statement.Replace(" ", "").ToLower();

                            if (revenueDictionary.Any(q=> q.Value == itemName))
                            {
                                detail.AccId = revenueDictionary.First(q=> q.Value == itemName).Key;
                            }
                        }

                        db.FinancialClaims.Add(newFinancialClaim);
                        db.FinancialClaimDetails.AddRange(details);

                        // تحديث الإيرادات
                        var revenues = db.Revenues
                            .Where(x =>
                                x.OfficerData.TowerData.AirLineId == airline.Id &&
                                DbFunctions.TruncateTime(x.OfficerData.TowerData.Date)
                                >= DbFunctions.TruncateTime(c.DateFrom) &&
                                DbFunctions.TruncateTime(x.OfficerData.TowerData.Date)
                                <= DbFunctions.TruncateTime(c.DateTo))
                            .ToList();

                        foreach (var r in revenues)
                        {
                            r.ExchangeRate = exchangeRate;
                        }
                    }

                    db.SaveChanges();

                    transaction.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    throw new Exception(
                        ex.InnerException?.Message ??
                        ex.Message
                    );
                }
            }
        }

        public ClaimCalculateDomain ClaimCalculation(int? AirLineId, DateTime? DateFrom, DateTime? DateTo, decimal? myExchangeRate,  string airlineName, int companyInfoId)
        {
            int month;
            int year;

            DateTime refDate = DateTo ?? DateFrom.Value;

            month = refDate.Month;
            year = refDate.Year;

            var monthName = DateFrom.Value.ToString(
                "MMMM",
                new CultureInfo("ar-EG"));

            ClaimCalculateDomain claim = new ClaimCalculateDomain();

            claim.Statement =
                $"مطالبة {airlineName} لشهر {monthName} {year}م";

            DateTime from = DateFrom.Value.Date;
            DateTime to = DateTo.Value.Date;

            var revenue = this.db.Revenues
                .Where(x =>
                    x.OfficerData.TowerData.CompanyInfoId == companyInfoId &&
                    x.OfficerData.TowerData.AirLineId == AirLineId
                    &&
                    DbFunctions.TruncateTime(x.OfficerData.TowerData.Date) >= from
                    &&
                    DbFunctions.TruncateTime(x.OfficerData.TowerData.Date) <= to
                )
                .ToList();

            claim.LandingFees = Convert.ToDecimal(revenue.Sum(x => x.LandingFees));
            claim.LandingFeesSDG = claim.LandingFees * myExchangeRate;

            claim.ParkingFees = Convert.ToDecimal(revenue.Sum(x => x.ParkingFees));
            claim.ParkingFeesSDG = claim.ParkingFees * myExchangeRate;

            claim.NightOperationFees = Convert.ToDecimal(revenue.Sum(x => x.NightOperationFees));
            claim.NightOperationFeesSDG = claim.NightOperationFees * myExchangeRate;

            claim.NavigationFees = Convert.ToDecimal(revenue.Sum(x => x.NavigationFees));
            claim.NavigationFeesSDG = claim.NavigationFees * myExchangeRate;

            claim.SecurityFees = Convert.ToDecimal(revenue.Sum(x => x.SecurityFees));
            claim.SecurityFeesSDG = claim.SecurityFees * myExchangeRate;

            claim.FirstClassFees = Convert.ToDecimal(revenue.Sum(x => x.FirstClassFees));
            claim.FirstClassFeesSDG = claim.FirstClassFees * myExchangeRate;

            claim.InterCUPPSFees = Convert.ToDecimal(revenue.Sum(x => x.InterCUPPSFees));
            claim.InterCUPPSFeesSDG = claim.InterCUPPSFees * myExchangeRate;

            claim.DomeCUPPSFees = Convert.ToDecimal(revenue.Sum(x => x.DomeCUPPSFees)); 
            claim.DomeCUPPSFeesSDG = claim.DomeCUPPSFees * myExchangeRate;

            claim.InterNYFees = Convert.ToDecimal(revenue.Sum(x => x.InterNYFees));
            claim.InterNYFeesSDG = claim.InterNYFees * myExchangeRate;

            claim.DomeNYFees = Convert.ToDecimal(revenue.Sum(x => x.DomeNYFees));
            claim.DomeNYFeesSDG = claim.DomeNYFees * myExchangeRate;

            claim.PushBackFees = Convert.ToDecimal(revenue.Sum(x => x.PushBackFees));
            claim.PushBackFeesSDG = claim.PushBackFees * myExchangeRate;

            claim.ServiceCars =
                Convert.ToDecimal(revenue.Sum(x => x.AmbolanceFees)) +
                Convert.ToDecimal(revenue.Sum(x => x.FireFightingFees));

            claim.ServiceCarsSDG =
                claim.ServiceCars * myExchangeRate;

            claim.NoiseFees =
                Convert.ToDecimal(revenue.Sum(x => x.NoiseFees));

            claim.NoiseFeesSDG =
                claim.NoiseFees * myExchangeRate;

            claim.ExpMailFees =
                Convert.ToDecimal(revenue.Sum(x => x.ExpMailFees));

            claim.ExpMailFeesSDG =
                claim.ExpMailFees * myExchangeRate;

            claim.ImpMailFees =
                Convert.ToDecimal(revenue.Sum(x => x.ImpMailFees));

            claim.ImpMailFeesSDG =
                claim.ImpMailFees * myExchangeRate;

            claim.FreightFees =
                Convert.ToDecimal(revenue.Sum(x => x.FreightFees));

            claim.FreightFeesSDG =
                claim.FreightFees * myExchangeRate;

            claim.SubTotalDollar =
                claim.LandingFees +
                claim.ParkingFees +
                claim.NightOperationFees +
                claim.NavigationFees +
                claim.SecurityFees +
                claim.FirstClassFees +
                claim.FreightFees +
                claim.ServiceCars +
                claim.InterCUPPSFees +
                claim.DomeCUPPSFees +
                claim.InterNYFees +
                claim.DomeNYFees +
                claim.PushBackFees +
                claim.ExpMailFees +
                claim.ImpMailFees +
                claim.NoiseFees;

            decimal? SubTotalDollarwithoutNY =
                claim.LandingFees +
                claim.ParkingFees +
                claim.NightOperationFees +
                claim.NavigationFees +
                claim.SecurityFees +
                claim.FirstClassFees +
                claim.FreightFees +
                claim.ServiceCars +
                claim.InterCUPPSFees +
                claim.DomeCUPPSFees +
                claim.PushBackFees +
                claim.ExpMailFees +
                claim.ImpMailFees +
                claim.NoiseFees;

            claim.TaxDollar =
                SubTotalDollarwithoutNY * 0.05m;

            claim.TaxSDG =
                claim.TaxDollar * myExchangeRate;

            claim.GrandTotalDollar =
                claim.SubTotalDollar +
                claim.TaxDollar;

            claim.GrandTotalSDG =
                claim.GrandTotalDollar *
                myExchangeRate;

            return claim;
        }

        public class ClaimCalculateDomain 
        {
            // ================== Airline ==================
            public string Name { get; set; }
            public int? currencyType { get; set; }
            // ================== Fees (Dollar) ==================
            public decimal LandingFees { get; set; }
            public decimal? LandingFeesSDG { get; set; }
            public decimal? ParkingFees { get; set; }
            public decimal? ParkingFeesSDG { get; set; }
            public decimal? NightOperationFees { get; set; }
            public decimal? NightOperationFeesSDG { get; set; }
            public decimal? NavigationFees { get; set; }
            public decimal? NavigationFeesSDG { get; set; }

            public decimal? SecurityFees { get; set; }
            public decimal? SecurityFeesSDG { get; set; }
            public decimal? FirstClassFees { get; set; }
            public decimal? FirstClassFeesSDG { get; set; }

            public decimal? InterCUPPSFees { get; set; }
            public decimal? InterCUPPSFeesSDG { get; set; }

            public decimal? DomeCUPPSFees { get; set; }
            public decimal? DomeCUPPSFeesSDG { get; set; }


            public decimal? InterNYFees { get; set; }
            public decimal? InterNYFeesSDG { get; set; }
            public decimal? DomeNYFees { get; set; }
            public decimal? DomeNYFeesSDG { get; set; }

            public decimal? PushBackFees { get; set; }
            public decimal? PushBackFeesSDG { get; set; }

            public decimal? ServiceCars { get; set; }
            public decimal? ServiceCarsSDG { get; set; }


            public decimal? NoiseFees { get; set; }
            public decimal? NoiseFeesSDG { get; set; }

            public decimal? ExpMailFees { get; set; }
            public decimal? ExpMailFeesSDG { get; set; }
            public decimal? ImpMailFees { get; set; }
            public decimal? ImpMailFeesSDG { get; set; }
            public decimal? FreightFees { get; set; }
            public decimal? FreightFeesSDG { get; set; }

            // ================== Totals ==================
            public decimal? SubTotalDollar { get; set; }
            public decimal? TaxDollar { get; set; }
            public decimal? GrandTotalDollar { get; set; }
            public decimal? GrandTotalSDG { get; set; }

            // ================== Extra Fees ==================
            public decimal? FireFightingFees { get; set; }
            public decimal? AmbolanceFees { get; set; }
            public decimal? OpenAireportOvertimeFees { get; set; }

            // ================== Invoice Info ==================
            //public string? TxtInvoiceNo { get; set; }
            public string HeaderDate { get; set; }
            public string DateFrom { get; set; }
            public string DateTo { get; set; }
            public DateTime DateNow { get; set; } = DateTime.Now;

            public decimal? ExchangeRate { get; set; }

            // ================== Text Values ==================


            // ================== SDG ==================
            public decimal? TaxSDG { get; set; }

            // ================== Statement ==================
            public string Statement { get; set; }
            public int? CurrencyTypeId { get; set; }

            public decimal? SubTotalSDG { get; set; }

            public string txtInvoiceNo { get; set; }
            public string GrandTotalDollartext { get; set; }
            public string  GrandTotalSDGtext { get; set; }
        }

        public class CreateOrUpdateFinancialClaimModel
        {
            public int Id { get; set; }
            public int? AccId { get; set; }
            public decimal? ExchangeRate { get; set; }
            public DateTime? DateFrom { get; set; }
            public DateTime? DateTo { get; set; }
        }
    }
}