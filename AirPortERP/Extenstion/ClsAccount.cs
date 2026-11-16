using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AirPortERP.Models;
namespace AirPortERP.Extenstion
{
    public class ClsAccount
    {
        Entities db = new Entities();
        public string ParentNameList = "";
        public Decimal AccId;
        public string GetAccParentName(decimal? AccParent)
        {
            string AccParentName = "";
            AccountTree Tree = db.AccountTrees.Where(x => x.Id == AccParent).FirstOrDefault();
            if (Tree != null)
            {
               AccParentName = Tree.AccName;
            }
            return AccParentName;

        }


        public int GetAccParentLevel(decimal? AccParent)
        {
            int Level = 0;
           

            AccountTree Tree = db.AccountTrees.Where(x => x.Id == AccParent).FirstOrDefault();
            if (Tree != null)
            {
                Level = Convert.ToInt32(Tree.AccLevel);
            }
            return Level;

        }



        public int GetAccNumberForParent(decimal? AccParent)
        {
            int AccNumber = 0;
            AccountTree Tree = db.AccountTrees.Where(x => x.Id == AccParent).FirstOrDefault();
            if (Tree != null)
            {
                AccNumber = Convert.ToInt32(Tree.AccNumber);
            }
            return AccNumber;

        }


        public string GetParentNameList(decimal? AccParentId)
        {
            ParentNameList = "";
            if (AccParentId != 0 || AccParentId == null)
            {


                AccountTree tree = db.AccountTrees.Where(x => x.AccNumber == AccParentId).FirstOrDefault();

                if (tree != null)
                {
                    int TreeLevel = Convert.ToInt32(tree.AccLevel);
                    AccId = Convert.ToDecimal(tree.AccNumber);
                    for (int i = 0; i < TreeLevel; i++)
                    {
                        //  AccId = tree.Id;
                        AccountTree t = db.AccountTrees.Where(x => x.AccNumber == AccId).FirstOrDefault();
                        ParentNameList += " <img src='/Temp/Image/Arrow.png' height='30' /> " + t.AccName.ToString();

                        AccId = Convert.ToDecimal(t.AccParent);

                        //  AccountTree P = db.AccountTrees.Find(t.Id);

                    }





                }
                else
                {
                    return ParentNameList;
                }


            }
            return ParentNameList;
        }

        ///----------------------------------------------------------------------------------------------------

        public decimal GetAccountNumber(decimal? BankId)
        {

            decimal AccountNumber = 0;
            AccountTree Tree = db.AccountTrees.Find(BankId);
            if (Tree != null)
            {
                AccountNumber = Convert.ToDecimal(Tree.AccNumber);
            }

            return AccountNumber;
        }
        ////------------------------------------------------------------------------------------------------------



        public string GetAccountName(decimal? AccId) 
        {

            string AccountName;
            AccountTree Tree = db.AccountTrees.Find(AccId);
            AccountName = Tree.AccName;

            return AccountName;
        }
        public string GetAccountNamePlus(decimal? AccId)
        {

            string AccountName;
            AccountTree Tree = db.AccountTrees.Find(AccId);
            if (Tree.AccGroupId == 1)
            {

                AccountName = db.AirLines.Where(x => x.AccId == AccId).FirstOrDefault().ArName;
               
            }
            else
            {
                AccountName = Tree.AccName;
            }   

            return AccountName;
        }
        public string GetDateddMMyyyy(string Date)
        {

            string HireDate;
            if (Date == null || Date == "")
            {

                HireDate = "غير معروف";
            }
            else
            {

                // HireDate = Convert.ToDateTime(Date).ToString("MM/dd/yyyy");
                HireDate = Convert.ToDateTime(Date).ToString("dd/MM/yyyy");
            }

            return HireDate;
        }




        public DateTime GetDateD(String D)
        {
            return DateTime.ParseExact(D, "dd/MM/yyyy", null);
        }



        public String ConvertToddMMyyyy(String S)
        {

            ///DateTime Test= DateTime.ParseExact(S, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            string Test = Convert.ToDateTime(S).ToString("MM/dd/yyyy");
            var d1 = Test.ToString().Substring(0, 10);
            var m1 = d1.Substring(0, 2);
            var days = d1.Substring(3, 2);
            var y = d1.Substring(6, 4);
            var FullDate = days + "/" + m1 + "/" + y;


            //int Year = Convert.ToInt32(S.Split('/')[2]);
            //int Month = Convert.ToInt32(S.Split('/')[1]);
            //int Day = Convert.ToInt32(S.Split('/')[0]);
            //DateTime dt = new DateTime(Year, Month, Day);



            return FullDate;
        }



    }
}