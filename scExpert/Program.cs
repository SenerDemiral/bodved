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

            Handle.GET("/scExpert/IndexCreate", () =>
            {
                H.IndexCreate();
                return "IndexCreate: OK";
            });

            Handle.GET("/scExpert/IndexDrop", () =>
            {
                H.IndexDrop();
                return "IndexDrop: OK";
            });


        }
    }
}