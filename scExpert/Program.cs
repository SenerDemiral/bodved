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
            Handle.GET("/scExpert/insOtoNotice", () =>
            {
                BDB.H.insOtoNotice();
                return $"OK: insOtoNotice";
            });

            Handle.GET("/scExpert/perfTest", () =>
            {
                return BDB.H.perfTest();
            });

            Handle.GET("/scExpert", (Request req) =>
            {
                var aaa = Db.SQL<CETR>("select p from CETR p where p.PP.ObjectNo = ? and p.CC.Lig = ?", 457, "1").GroupBy((x) => x.PP.Ad);
                var bbb = Db.SQL<CETR>("select p from CETR p where p.PP.ObjectNo = ?", 457).GroupBy((x) => x.CC.Lig);
                var ccc = Db.SQL<CETR>("select p from CETR p where p.PP.ObjectNo = ?", 457).GroupBy((x) => new { x.PP.oNo, x.CC.Lig });
                ulong LigP = 0;
                string Lig = "";
                int LigC = 0;
                foreach (var b in bbb)
                {
                    Lig = b.Key;
                    LigC = b.Count();
                }
                foreach (var c in ccc)
                {
                    LigP = c.Key.oNo;
                    Lig = c.Key.Lig;
                    LigC = c.Count();
                }
                return $"client IP: {req.ClientIpAddress}";
            });

            Handle.GET("/scExpert/UpdPPLigMacSay", () =>
            {
                BDB.H.UpdPPLigMacSay();
                return $"OK: UpdPPLigMacSay";
            });
            Handle.GET("/scExpert/UpdPPLigMacSay2", () =>
            {
                BDB.H.UpdPPLigMacSay2();
                return $"OK: UpdPPLigMacSay2";
            });
            Handle.GET("/scExpert/UpdPPLigMacSay3", () =>
            {
                BDB.H.UpdPPLigMacSay3();
                return $"OK: UpdPPLigMacSay3";
            });

            Handle.GET("/scExpert/refreshPRH2", () =>
            {
                BDB.H.refreshPRH2();
                return "refreshPRH: OK";
            });

            Handle.GET("/scExpert/updCETsumCC/{?}", (string CCoNo) =>
            {
                BDB.H.updCETsumCC(ulong.Parse(CCoNo));
                return $"OK: updCETsumCC({CCoNo})";
            });

            Handle.GET("/scExpert/updCTsumCC/{?}", (string CCoNo) =>
            {
                BDB.H.updCTsumCC(ulong.Parse(CCoNo));
                return $"OK: UpdateCTsumCC({CCoNo})";
            });

            Handle.GET("/scExpert/SavePP", () =>
            {
                BDB.H.SavePP();
                return "OK: SavePP()";
            });
            Handle.GET("/scExpert/SaveCC", () =>
            {
                BDB.H.SaveCC();
                return "OK: SaveCC()";
            });

            Handle.GET("/scExpert/LoadPP", () =>
            {
                BDB.H.LoadPP();
                return "OK: LoadPP()";
            });

            Handle.GET("/scExpert/LoadCC", () =>
            {
                BDB.H.LoadCC();
                return "OK: LoadCC()";
            });

            Handle.GET("/scExpert/RestoreCC/{?}", (string ccID) =>
            {
                BDB.H.RestoreCC(ccID);
                return $"OK: RestoreCC({ccID})";
            });

            Handle.GET("/scExpert/BackupCC/{?}", (ulong CCoNo) =>
            {
                BDB.H.BackupCC(CCoNo);
                return $"OK: BackupCC({CCoNo})";
            });

            Handle.GET("/scExpert/BackupCET/{?}/{?}", (string ccID, string cetID) =>
            {
                BDB.H.BackupCET(ccID, cetID);
                return $"OK: BackupCET({ccID}, {cetID})";
            });

            Handle.GET("/scExpert/DeleteCETrelated/{?}", (string cetOnO) =>
            {
                H.DeleteCETrelated(ulong.Parse(cetOnO));
                return $"OK: DeleteCETrelated({cetOnO}) CEToNo";
            });

            Handle.GET("/bodved/init", () =>
            {
                //popTable();
                BDB.Tanimlar.popTable();
                return "Init: OK";
            });

            Handle.GET("/scExpert/DeleteCETrelated/{?}", (string cetOnO) =>
            {
                H.DeleteCETrelated(ulong.Parse(cetOnO));

                return $"OK: DeleteCETrelated({cetOnO})";
            });

            Handle.GET("/scExpert/CETidnumaraVer/{?}", (string ccID) =>
            {
                var cc = Db.SQL<CC>("select r from CC r where r.ID = ?", ccID).FirstOrDefault();

                Db.Transact(() =>
                {
                    var cets = Db.SQL<CET>("select c from CET c where c.CC = ?", cc);
                    foreach(var cet in cets)
                    {
                        cet.ID = $"1{cet.ID}";
                    }
                });

                return "OK: CETidNumaraVer";
            });


            Handle.GET("/scExpert/deneme2", () =>
            {
                return BDB.Tanimlar.deneme2();
            });
            Handle.GET("/scExpert/deneme3", () =>
            {
                return BDB.Tanimlar.deneme3();
            });
            Handle.GET("/scExpert/deneme4", () =>
            {
                return BDB.Tanimlar.deneme4();
            });



            Handle.GET("/scExpert/refreshPRH", () =>
            {
                BDB.H.refreshPRH();
                return "refreshPRH: OK";
            });

            Handle.GET("/scExpert/updPPsum", () =>
            {
                BDB.H.updPPsum();
                return "updPPsum: OK";
            });

            Handle.GET("/scExpert/IndexCreate", () =>
            {
                if (Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "IdxPP_Ad").FirstOrDefault() == null)
                    Db.SQL("CREATE INDEX IdxPP_Ad      ON PP   (Ad)");
                if (Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "IdxPP_Sra").FirstOrDefault() == null)
                    Db.SQL("CREATE INDEX IdxPP_Sra     ON PP   (Sra)");

                if (Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "IdxPRH_Trh").FirstOrDefault() == null)
                    Db.SQL("CREATE INDEX IdxPRH_Trh    ON PRH  (Trh ASC)");
                if (Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "IdxPRH_PP_Trh").FirstOrDefault() == null)
                    Db.SQL("CREATE INDEX IdxPRH_PP_Trh ON PRH  (PP, Trh DESC)");

                if (Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "IdxCC_Idx").FirstOrDefault() == null)
                    Db.SQL("CREATE INDEX IdxCC_Idx     ON CC   (Idx DESC)");

                if (Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "IdxCT_CC").FirstOrDefault() == null)
                    Db.SQL("CREATE INDEX IdxCT_CC      ON CT   (CC)");

                if (Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "IdxCET_CC_Trh").FirstOrDefault() == null)
                    Db.SQL("CREATE INDEX IdxCET_CC_Trh ON CET  (CC, Trh)");
                if (Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "IdxCET_hCT").FirstOrDefault() == null)
                    Db.SQL("CREATE INDEX IdxCET_hCT    ON CET  (hCT)");
                if (Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "IdxCET_gCT").FirstOrDefault() == null)
                    Db.SQL("CREATE INDEX IdxCET_gCT    ON CET  (gCT)");

                if (Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "IdxCTP_CC").FirstOrDefault() == null)
                    Db.SQL("CREATE INDEX IdxCTP_CC     ON CTP  (CC)");
                if (Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "IdxCTP_CT").FirstOrDefault() == null)
                    Db.SQL("CREATE INDEX IdxCTP_CT     ON CTP  (CT)");

                if (Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "IdxCETP_CET").FirstOrDefault() == null)
                    Db.SQL("CREATE INDEX IdxCETP_CET   ON CETP (CET)");

                if (Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "IdxCETR_CC").FirstOrDefault() == null)
                    Db.SQL("CREATE INDEX IdxCETR_CC    ON CETR (CC)");
                if (Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "IdxCETR_CET").FirstOrDefault() == null)
                    Db.SQL("CREATE INDEX IdxCETR_CET   ON CETR (CET)");

                return "IndexCreate: OK";
            });

            Handle.GET("/scExpert/IndexDrop", () =>
            {
                Db.SQL("DROP INDEX IdxPP_Ad       ON PP");
                Db.SQL("DROP INDEX IdxPP_Sra      ON PP");

                Db.SQL("DROP INDEX IdxPRH_Trh     ON PRH");
                Db.SQL("DROP INDEX IdxPRH_PP_Trh  ON PRH");

                Db.SQL("DROP INDEX IdxCC_Idx      ON CC");

                Db.SQL("DROP INDEX IdxCT_CC       ON CT");

                Db.SQL("DROP INDEX IdxCET_CC_Trh  ON CET");
                Db.SQL("DROP INDEX IdxCET_hCT     ON CET");
                Db.SQL("DROP INDEX IdxCET_gCT     ON CET");

                Db.SQL("DROP INDEX IdxCTP_CC      ON CTP");
                Db.SQL("DROP INDEX IdxCTP_CT      ON CTP");

                Db.SQL("DROP INDEX IdxCETP_CET    ON CETP");

                Db.SQL("DROP INDEX IdxCETR_CC     ON CETR");
                Db.SQL("DROP INDEX IdxCETR_CET    ON CETR");

                return "IndexDrop: OK";
            });


        }
    }
}