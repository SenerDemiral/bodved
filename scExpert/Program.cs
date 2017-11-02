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
                return "OK: RestoreCC";
            });

            Handle.GET("/bodved/BackupCC/{?}", (string ccID) =>
            {
                BDB.H.BackupCC(ccID);
                return "OK: BackupCC";
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

            Handle.GET("/bodved/refreshPRH", () =>
            {
                BDB.H.refreshPRH();
                return "refreshPRH: OK";
            });

            Handle.GET("/bodved/updPPsum", () =>
            {
                BDB.H.updPPsum();
                return "updPPsum: OK";
            });

            Handle.GET("/bodved/IndexCreate", () =>
            {
                if (Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "IdxPP_Ad").FirstOrDefault() == null)
                    Db.SQL("CREATE INDEX IdxPP_Ad   ON PP   (Ad ASC)");

                if (Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "IdxPRH_Trh").FirstOrDefault() == null)
                    Db.SQL("CREATE INDEX IdxPRH_Trh ON PRH  (Trh ASC)");

                if (Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "IdxPRH_PP_Trh").FirstOrDefault() == null)
                    Db.SQL("CREATE INDEX IdxPRH_PP_Trh ON PRH  (PP, Trh DESC)");

                if (Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "IdxCT_CC").FirstOrDefault() == null)
                    Db.SQL("CREATE INDEX IdxCT_CC  ON CT (CC)");

                if (Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "IdxCET_CC_Trh").FirstOrDefault() == null)
                    Db.SQL("CREATE INDEX IdxCET_CC_Trh  ON CET (CC, Trh)");

                if (Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "IdxCTP_CT").FirstOrDefault() == null)
                    Db.SQL("CREATE INDEX IdxCTP_CT  ON CTP (CT)");

                if (Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "IdxCETP_CET").FirstOrDefault() == null)
                    Db.SQL("CREATE INDEX IdxCETP_CET  ON CETP (CET)");

                if (Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "IdxCETR_CET").FirstOrDefault() == null)
                    Db.SQL("CREATE INDEX IdxCETR_CET  ON CETR (CET)");

                return "IndexCreate: OK";
            });

            Handle.GET("/bodved/IndexDrop", () =>
            {
                Db.SQL("DROP INDEX IdxPP_Ad           ON PP");
                Db.SQL("DROP INDEX IdxPRH_Trh         ON PRH");
                Db.SQL("DROP INDEX IdxPRH_PP_Trh      ON PRH");
                Db.SQL("DROP INDEX IdxCT_CC           ON CT");
                Db.SQL("DROP INDEX IdxCET_CC_Trh      ON CET");
                Db.SQL("DROP INDEX IdxCTP_CT          ON CTP");
                Db.SQL("DROP INDEX IdxCETP_CET        ON CETP");
                Db.SQL("DROP INDEX IdxCETR_CET        ON CETR");

                return "IndexDrop: OK";
            });


        }
    }
}