using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AirPortERP.ViewModels
{
    public class TreeViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ParentName { get; set; }
        public string GroupName { get; set; }
        public string AccTypeName { get; set; }
        public decimal AccNumber { get; set; }

        public string ParentNameList { get; set; }
        
    }


    public class GettVM
    {
        public int? Id { get; set; }
        public string AirLineName { get; set; }
        public decimal? Debit { get; set; }
        public decimal? Credit { get; set; }
        public double? CurrencyValue { get; set; }
        public decimal? Balance { get; set; }
    }


    public class TreeAddViewModel
    {
        public int Id { get; set; }
        public string AccName { get; set; }
        public decimal AccParent { get; set; }
        public decimal AccNumber { get; set; }
        public int AccType { get; set; }
        public int AccLevel { get; set; }
        public int AccGroupId { get; set; }

    }



    public class TreeUpdateViewModel
    {
        public int Id { get; set; }
        public string AccName { get; set; }
        public decimal AccParent { get; set; }

        public string AccParentName { get; set; }
        public decimal AccNumber { get; set; }
        public int AccType { get; set; }
        public int AccLevel { get; set; }
        public int AccGroupId { get; set; }
       // public int OldAccGroupId { get; set; }

    }


    public class BankViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
      
        public decimal BankAccountNumber { get; set; }

    }

    public class FundViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public decimal FundAccountNumber { get; set; }

    }


    public class ClaimAddViewModel
    {
        public int Id { get; set; }
        public string DateFrom { get; set; }

        public string DateTo { get; set; }
        public int AccId { get; set; }
        public decimal ExchangeRate { get; set; }

    }
    public class ClaimAddViewModel2
    {
        public int Id { get; set; }

        public int TowerDataId { get; set; }

        public int AccId { get; set; }
    }

    public class BankAddViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AccId { get; set; }

        public int AccParentId { get; set; }
    }

    public class FundAddViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AccId { get; set; }

        public int AccParentId { get; set; }
    }

    public class BalanceViewModel
    {
        public int Id { get; set; }

        public int AccId { get; set; }
        public string AccName { get; set; }
        public string Date { get; set; }

        public string Note { get; set; }

        public string CurrencyName { get; set; }

        public string Debit { get; set; }
        public string Credit { get; set; }
        public string Statement { get; set; }

        public decimal AccountNumber { get; set; }
        public bool IsConfirm { get; set; }

        public string Role { get; set; }

    }



    public class OpenAddBalanceViewModel
    {
        public int Id { get; set; }
        public int AccId { get; set; }
        public DateTime Date { get; set; }

        public string Note { get; set; }

        public string CurrencyName { get; set; }

        public Nullable<decimal> Debit { get; set; }
        public Nullable<decimal> Credit { get; set; }

        public int CurrencyId { get; set; }
    }

    public class OpenViewModel
    {
        public int Id { get; set; }
        public string AccName { get; set; }
      //  public DateTime Date { get; set; }

        public string Note { get; set; }

        public string Date { get; set; }
        public string CurrencyName { get; set; }

        public decimal Debit { get; set; }
        public decimal Credit { get; set; }

      
    }
    public class SadSarfViewModel
    {
        public int Id { get; set; }

        public decimal TreeAccountNumber { get; set; }
        public int AccId { get; set; }
        public string AccName { get; set; }
        public string Date { get; set; }

        public string Statement { get; set; }

        public string CurrencyName { get; set; }

        public bool IsConfirm { get; set; }

        public string Debit { get; set; }
        public string TotalAmountCredit { get; set; }


        public string Role { get; set; }
        public string TotalAmountText { get; set; }



        public string Year { get; set; }
        public string Month { get; set; }




    }
    public class SadViewModel
    {
        public int Id { get; set; }

        public decimal TreeAccountNumber { get; set; }
        public int AccId { get; set; }
        public string AccName { get; set; }
        public string Date { get; set; }

        public string Statement { get; set; }

        public string CurrencyName { get; set; }

        public bool IsConfirm { get; set; }

        public string Debit { get; set; }
        public string TotalAmountCredit { get; set; }


        public string Role { get; set; }
        public string TotalAmountText { get; set; }



        public string Year { get; set; }
        public string Month { get; set; }




    }
    public class SadViewModelSarf
    {
        public int Id { get; set; }

        public decimal TreeAccountNumber { get; set; }
        public int AccId { get; set; }
        public string AccName { get; set; }
        public string Date { get; set; }

        public string Statement { get; set; }

        public string CurrencyName { get; set; }

        public bool IsConfirm { get; set; }

        public string Debit { get; set; }
        public string TotalAmountCredit { get; set; }


        public string Role { get; set; }
        public string TotalAmountText{ get; set; }



        public string Year { get; set; }
        public string Month { get; set; }




    }

    public class ClaimViewModel
    {
        public int Id { get; set; }

        public int AccId { get; set; }
        public int AirLineId { get; set; }

        public decimal TreeAccountNumber { get; set; }
        public string AccName { get; set; }
        public string Date { get; set; }

        public string Statement { get; set; }

        public string CurrencyName { get; set; }

        public bool IsConfirm { get; set; }

        public string Debit { get; set; }
        public string TotalAmountCredit { get; set; }
        public string ExchangeRate { get; set; }
        
        public int Month { get; set; }
        public int Year { get; set; }

        public string TotalAmountText { get; set; }

        public string DateFrom { get; set; }
        public string DateTo { get; set; }

        public string Role { get; set; }
    }
    public class SandAddViewModel
    {
        public int Id { get; set; }

        public int MainId { get; set; }
        public int AccIdDetail { get; set; }
        public string Date { get; set; }

        public string Statement { get; set; }

        public string MoneyText { get; set; }


        public string ShakNumber { get; set; }

        public bool ISCash { get; set; }

        public bool WriteAutomatic { get; set; }

        public bool ISConfirmAmount { get; set; }

        public int CurrencyId { get; set; }


        public Nullable<int> YearId { get; set; }
        public Nullable<int> MonthId { get; set; }

        public decimal Debit { get; set; }
        public Nullable<decimal> TotalAmountCredit { get; set; }
        public int AccIdMain { get; set; }

    }
    public class SandAddViewModelSarf
    {
        public int Id { get; set; }

        public int MainId { get; set; }
        public int AccIdDetail { get; set; }
        public DateTime Date { get; set; }

        public string Statement { get; set; }

        public string MoneyText { get; set; }


        //public string ShakNumber { get; set; }

       // public bool ISCash { get; set; }

      //  public bool WriteAutomatic { get; set; }

      //  public bool ISConfirmAmount { get; set; }

        public int CurrencyId { get; set; }


        public Nullable<int> YearId { get; set; }
        public Nullable< int> MonthId { get; set; }

        public decimal Debit { get; set; }
        public Nullable< decimal> TotalAmountCredit { get; set; }
        public int AccIdMain { get; set; }

    }


    public class ErrorViewModel
    {

        public string MassegeHeader { get; set; }
        public string MassegeContent { get; set; }
    }
    public class PrintSandViewModel
    {
        public int Id { get; set; }
        public int N { get; set; }
        public string AccountName { get; set; }
      
        public string Date { get; set; }

        public string Statement { get; set; }
        public decimal Credit { get; set; }

        
        public string CurrencyName { get; set; }


        public string ShakNumber { get; set; }

        public bool ISCash { get; set; }



        public int CurrencyId { get; set; }

       // public decimal Credit { get; set; }
        public decimal TotalAmountCredit { get; set; }




        public int AccIdMain { get; set; }










    }


    public class PrintClaimViewModel
    {
        public int Id { get; set; }
        public int N { get; set; }
        public string AccountName { get; set; }

        public string Date { get; set; }

        public int Year { get; set; }
        public int Mouth { get; set; }
        public string MouthName { get; set; }
        //public string Airline { get; set; }
        //public string DateFrom { get; set; }
        //public string DateTo { get; set; }
        public string Statement { get; set; }
        public decimal Debit { get; set; }


        public string CurrencyName { get; set; }


        public string ShakNumber { get; set; }

        public bool ISCash { get; set; }



        public int CurrencyId { get; set; }

        // public decimal Credit { get; set; }
        public decimal TotalAmountCredit { get; set; }




        public int AccIdMain { get; set; }










    }

    public class PrintOpenViewModel
    {
        public int Id { get; set; }
        public int N { get; set; }
        public string AccountName { get; set; }

        public string Date { get; set; } 
        public string Statement { get; set; }
        public decimal Debit { get; set; }

        public decimal Credit { get; set; }
        public string CurrencyName { get; set; }

        public int CurrencyId { get; set; }

        // public decimal Credit { get; set; }
        public decimal TotalAmountCredit { get; set; }

        public int AccIdMain { get; set; }


    }









    public class PrintOperationViewModel
    {
        public int Id { get; set; }
        public int N { get; set; }
        public string AccountName { get; set; }

        public string Date { get; set; }

        public string Statement { get; set; }
        public decimal Debit { get; set; }

        public decimal Credit { get; set; }
        public string CurrencyName { get; set; }


        public string ShakNumber { get; set; }

        public bool ISCash { get; set; }



        public int CurrencyId { get; set; }

        // public decimal Credit { get; set; }
        public decimal TotalAmountCredit { get; set; }




        public int AccIdMain { get; set; }










    }







    public class DeleteReasonAddViewModel
    {
        public int Id { get; set; }

      

        public string DeleteReason { get; set; }


    }

    public class YearViewModel
    {
        public int Id { get; set; }
        public string Year { get; set; }


    }




    public class OperationViewModel
    {
        public int Id { get; set; }

        public int N { get; set; }
        public string OperationTypeName { get; set; }

        public string AccountName { get; set; }
        public decimal AccountNumber { get; set; }

        public decimal Debit { get; set; }

        public decimal Credit { get; set; }

        public string CurrencyName { get; set; }
        public string Statement { get; set; }
        
    }



    public class FinancialClaimViewModel
    {
        public int Id { get; set; }

        public int N { get; set; }
        public double MonthingClaim { get; set; }

     

        public int CurrencyId { get; set; }
        public string Statement { get; set; }

    }

    public class AccountGroupViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

    }









}