using System;
using System.Linq;
using Starcounter;
using BDB;

namespace scExpert
{
    class Program
    {
        static void Main()
        {
            Handle.GET("/bodved/LoadPP", () =>
            {
                BDB.H.LoadPP();
                return "OK";
            });
            Handle.GET("/bodved/LoadCC", () =>
            {
                BDB.H.LoadCC();
                return "OK";
            });
            Handle.GET("/bodved/RestoreCC/{?}", (string ccID) =>
            {
                BDB.H.RestoreCC(ccID);
                return "OK";
            });


            Handle.GET("/bodved/init", () =>
            {
                //popTable();
                BDB.Tanimlar.popTable();
                return "Init: OK";
            });

            Handle.GET("/scExpert/delCETP382", () =>
            {
                Db.Transact(() =>
                {
                    Db.SQL("DELETE FROM CETP where CET.ObjectNo = ?", 382);
                });

                return "OK";
            });

            Handle.GET("/bodved/deneme2", () =>
            {
                return BDB.Tanimlar.deneme2();
            });
            Handle.GET("/bodved/deneme3", () =>
            {
                return BDB.Tanimlar.deneme3();
            });
            Handle.GET("/bodved/deneme4", () =>
            {
                return BDB.Tanimlar.deneme4();
            });


        }
    }
}