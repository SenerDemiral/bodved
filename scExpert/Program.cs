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
            //HB.PKset(); Bir kere yap
            
            Handle.GET("/scExpert/Backup", () =>
            {
                BDB.HB.bCC();
                return $"OK: Backup";
            });

            Handle.GET("/scExpert/Restore", () =>
            {
                BDB.HB.rCC();
                return $"OK: Restore";
            });

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

            Handle.GET("/scExpert/initBazRanks", () =>
            {
                BDB.H.initBazRanks();
                return "OK: initBazRanks";
            });

            Handle.GET("/scExpert/refreshRH", () =>
            {
                BDB.H.RefreshRH();
                return "refreshRH: OK";
            });

            Handle.GET("/scExpert/refreshRH2/{?}", (ulong CCoNo) =>
            {
                BDB.H.RefreshRH2(CCoNo);
                return $"OK: refreshRH({CCoNo})";
            });

            Handle.GET("/scExpert/CompCTtRnkOfCC/{?}", (ulong CCoNo) =>
            {
                BDB.H.CompCTtRnkOfCC(CCoNo);
                return $"OK: CompCTtRnkOfCC({CCoNo})";
            });

            Handle.GET("/scExpert/ReCreateRHofCC/{?}", (ulong CCoNo) =>
            {
                BDB.H.ReCreateRHofCC(CCoNo);
                return $"OK: ReCreateRHofCC({CCoNo})";
            });

            Handle.GET("/scExpert/UpdCETsumCC/{?}", (string CCoNo) =>
            {
                BDB.H.UpdCETsumCC(ulong.Parse(CCoNo));
                return $"OK: updCETsumCC({CCoNo})";
            });

            Handle.GET("/scExpert/UpdCTsumCC/{?}", (string CCoNo) =>
            {
                BDB.H.UpdCTsumCC(ulong.Parse(CCoNo));
                return $"OK: UpdCTsumCC({CCoNo})";
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

            Handle.GET("/scExpert/updPPsum", () =>
            {
                BDB.H.updPPsum();
                return "updPPsum: OK";
            });

            Handle.GET("/scExpert/IndexCreate", () =>
            {
                
                if (Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "IdxPP_Ad").FirstOrDefault() == null)
                    Db.SQL("CREATE INDEX IdxPP_Ad      ON BDB.PP (Ad)");
                if (Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "IdxPP_Rnk").FirstOrDefault() == null)
                    Db.SQL("CREATE INDEX IdxPP_Rnk     ON BDB.PP (Rnk DESC)");
                if (Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "IdxPP_Sra").FirstOrDefault() == null)
                    Db.SQL("CREATE INDEX IdxPP_Sra     ON BDB.PP (Sra)");

                if (Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "IdxPPGR_PPRnkID").FirstOrDefault() == null)
                    Db.SQL("CREATE INDEX IdxPPGR_PPRnkID  ON BDB.PPGR (PP, RnkID)");
                if (Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "IdxPPGR_Rnk").FirstOrDefault() == null)
                    Db.SQL("CREATE INDEX IdxPPGR_Rnk     ON BDB.PPGR (Rnk DESC)");
                if (Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "IdxPPGR_RnkID").FirstOrDefault() == null)
                    Db.SQL("CREATE INDEX IdxPPGR_RnkID   ON BDB.PPGR (RnkID)");

                if (Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "IdxCC_Idx").FirstOrDefault() == null)
                    Db.SQL("CREATE INDEX IdxCC_Idx     ON BDB.CC (Idx DESC)");
                
                if (Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "IdxCT_CC").FirstOrDefault() == null)
                    Db.SQL("CREATE INDEX IdxCT_CC      ON BDB.CT (CC)");

                if (Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "IdxCET_CC_Trh").FirstOrDefault() == null)
                    Db.SQL("CREATE INDEX IdxCET_CC_Trh ON BDB.CET (CC, Trh)");
                if (Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "IdxCET_hCT").FirstOrDefault() == null)
                    Db.SQL("CREATE INDEX IdxCET_hCT    ON BDB.CET (hCT)");
                if (Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "IdxCET_gCT").FirstOrDefault() == null)
                    Db.SQL("CREATE INDEX IdxCET_gCT    ON BDB.CET (gCT)");

                if (Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "IdxCTP_CC").FirstOrDefault() == null)
                    Db.SQL("CREATE INDEX IdxCTP_CC     ON BDB.CTP (CC)");
                if (Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "IdxCTP_CT").FirstOrDefault() == null)
                    Db.SQL("CREATE INDEX IdxCTP_CT     ON BDB.CTP (CT)");

                if (Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "IdxCETP_CET").FirstOrDefault() == null)
                    Db.SQL("CREATE INDEX IdxCETP_CET   ON BDB.CETP (CET)");

                if (Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "IdxCETR_CC").FirstOrDefault() == null)
                    Db.SQL("CREATE INDEX IdxCETR_CC    ON BDB.CETR (CC)");
                if (Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "IdxCETR_CET").FirstOrDefault() == null)
                    Db.SQL("CREATE INDEX IdxCETR_CET   ON BDB.CETR (CET)");
                if (Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "IdxCETR_RH").FirstOrDefault() == null)
                    Db.SQL("CREATE INDEX IdxCETR_RH    ON BDB.CETR (RH)");

                if (Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "IdxRH_Trh").FirstOrDefault() == null)
                    Db.SQL("CREATE INDEX IdxRH_Trh     ON BDB.RH (Trh)");
                //if (Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "IdxRH_CC").FirstOrDefault() == null)
                //    Db.SQL("CREATE INDEX IdxRH_CC      ON BDB.RH (CC)");

                return "IndexCreate: OK";
            });

            Handle.GET("/scExpert/IndexDrop", () =>
            {
                Db.SQL("DROP INDEX IdxPP_Ad       ON BDB.PP");
                Db.SQL("DROP INDEX IdxPP_Rnk      ON BDB.PP");
                Db.SQL("DROP INDEX IdxPP_Sra      ON BDB.PP");

                Db.SQL("DROP INDEX IdxPPGR_PPRnkID ON BDB.PPGR");
                Db.SQL("DROP INDEX IdxPPGR_Rnk     ON BDB.PPGR");
                Db.SQL("DROP INDEX IdxPPGR_RnkID   ON BDB.PPGR");

                Db.SQL("DROP INDEX IdxRH_Trh      ON BDB.RH");
                //Db.SQL("DROP INDEX IdxRH_CC       ON BDB.RH");

                //Db.SQL("DROP INDEX IdxPRH_Trh     ON BDB.PRH");
                //Db.SQL("DROP INDEX IdxPRH_PP_Trh  ON BDB.PRH");

                Db.SQL("DROP INDEX IdxCC_Idx      ON BDB.CC");

                Db.SQL("DROP INDEX IdxCT_CC       ON BDB.CT");

                Db.SQL("DROP INDEX IdxCET_CC_Trh  ON BDB.CET");
                Db.SQL("DROP INDEX IdxCET_hCT     ON BDB.CET");
                Db.SQL("DROP INDEX IdxCET_gCT     ON BDB.CET");

                Db.SQL("DROP INDEX IdxCTP_CC      ON BDB.CTP");
                Db.SQL("DROP INDEX IdxCTP_CT      ON BDB.CTP");

                Db.SQL("DROP INDEX IdxCETP_CET    ON BDB.CETP");

                Db.SQL("DROP INDEX IdxCETR_CC     ON BDB.CETR");
                Db.SQL("DROP INDEX IdxCETR_CET    ON BDB.CETR");
                Db.SQL("DROP INDEX IdxCETR_RH     ON BDB.CETR");

                return "IndexDrop: OK";
            });


        }
    }
}