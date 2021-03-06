﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Starcounter;
using System.IO;
using System.Diagnostics;

namespace BDB
{
    public static class H
    {
        public static string MacSetResult(long hR, long gR)
        {
            // ← →
            // ◄ ►
            // ◀▶ ◄►
            string rS = "";
            if (hR > gR)
                rS = $"◀-{gR}";
            else if(hR < gR)
                rS = $"{hR}-▶";
            /*
            if (hR > gR)
                rS = $"{hR}-{gR}";
            else if (hR < gR)
                rS = $"{hR}-{gR}";
            */
            return rS;
        }

        public static long GEN_ID()
        {
            ulong id = 0;
            Db.Transact(() =>
            {
                var _id = Db.SQL<_ID>("select i from _ID i").FirstOrDefault();
                if (_id == null)
                {
                    _id = new _ID
                    {
                        ID = 100000
                    };
                }
                id = _id.ID + 1;
                _id.ID = id;

            });
            return (long)id;
        }
        public static void GEN_ID_DEL()
        {
            Db.Transact(() =>
            {
                Db.SQL("DELETE FROM _ID");
            });
        }

        // If Db.FromId<table>(tablePrimaryKey) is for retreiving table unique row/object,
        // Db.FromId should be return null if tablePrimaryKey does not belong to `table` but retuns exception if belongs to another `table`
        // System.InvalidCastException: Unable to cast object of type `anotherTable` to type `table`.
        // `tablePrimaryKey` is not used in any other tables Db.FromId returns null as expected.
        static Random _r = new Random();
        public static string perfTest()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            PP pp = null;
            //var pp = Db.SQL<PP>("select p from PP p where p.ObjectNo = ?", 1);

            pp = Db.FromId<PP>(0);
            pp = Db.FromId<PP>(5);
            pp = Db.FromId<PP>(570);

            int n = _r.Next(5);
            int nor = 0;
            for (int k = 0; k < 1000000; k++)
            {
                /*
                n = _r.Next(5000) + 1;
                try
                {
                    pp = Db.FromId<PP>((ulong)n);
                    oNo = pp.oNo;
                    nor++;
                }
                catch (Exception)
                {
                }
                */
                /*
                //pp = Db.SQL<PP>("select p from PP p where p.ObjectNo = ?", n).FirstOrDefault();
                if (pp != null)
                {
                    oNo = pp.oNo;
                    nor++;

                }
                */
                /*
                pp = Db.SQL<PP>("select p from PP p where p.ObjectNo = ?", n);
                foreach(var p in pp)
                {
                    oNo = p.oNo;
                    nor++;
                }*/

            }
            watch.Stop();
            return $"perfTest 1M: {watch.ElapsedMilliseconds} msec  {watch.ElapsedTicks} ticks {nor}";
        }

        public static void insOtoNotice()
        {
            var ilkGun = DateTime.Today;
            var sonGun = ilkGun.AddDays(5);
            var cets = Db.SQL<CET>("select c from CET c where c.Trh >= ? and c.Trh < ? order by c.Trh", ilkGun, sonGun);
            int onc = 1;
            Db.Transact(() =>
            {
                Db.SQL("DELETE FROM NTC WHERE Onc like ?", "+%");

                foreach (var c in cets)
                {
                    new NTC
                    {
                        Trh = c.Trh,
                        Onc = $"+{onc:D2}",
                        Ad = $"{c.hCTAd} - {c.gCTAd}",
                        Link = $"/bodved/CETpage/{c.CCoNo}",
                        Rtbl = "CET",
                        RoNo = c.GetObjectNo()
                    };
                    onc++;
                }
            });
        }

        public static int UpdEntCnt()
        {
            Write2Log($"Enter: {Session.Current.SessionId}");
            int EntCnt = 0;
            Db.Transact(() =>
            {
                var s = Db.SQL<STAT>("select s from STAT s where s.ID = ?", 1).FirstOrDefault();
                if (s == null)
                {
                    new STAT()
                    {
                        ID = 1,
                        IdVal = 10580
                    };
                    EntCnt = 10580;
                }
                else {
                    s.IdVal += 1;
                    EntCnt = s.IdVal;
                }

            });
            return EntCnt;
        }

        public static void UpdCETsumCC(ulong CCoNo)
        {
            var cc = Db.FromId<CC>(CCoNo);
            var cets = Db.SQL<CET>("select c from CET c where c.CC = ?", cc);
            foreach (var r in cets)
            {
                if (r.Rok)
                    UpdCETsum(r.oNo);
            }
        }

        public static void UpdCETRpp(ulong CETRoNo, ulong newPPoNo)
        {
            var cetr = Db.FromId<BDB.CETR>(CETRoNo);
            var pp = Db.FromId<BDB.PP>(newPPoNo);
            Db.Transact(() =>
            {
                if (cetr.SoD == "S")
                {
                    if (cetr.HoG == "H")
                        cetr.RH.hPP = pp;
                    else
                        cetr.RH.gPP = pp;
                }
                cetr.PP = pp;
            });
            BDB.H.UpdPPLigMacSay(cetr.CET.oNo);  // RefreshRH dan once gelmeli
            BDB.H.RefreshRH();  // Global Rank
            BDB.H.RefreshRH2(cetr.CC.oNo);  // RnkGrp Rank
            BDB.H.UpdCTsum(cetr.CET.hCT.oNo);
            BDB.H.UpdCTsum(cetr.CET.gCT.oNo);
        }

        public static void RefreshRank()
        {
            Dictionary<ulong, GrRnk> PPdic = new Dictionary<ulong, GrRnk>();
            /*
            foreach(var pp in Db.SQL<PP>("select p from PP p"))
            {
                PPdic[pp.oNo] = new GrRnk
                {
                    CCoNo = 0,
                    CToNo = 0,
                    RnkBaz = pp.RnkBaz,
                    Rnk = 0,
                    RnkGrp = 0
                };
            }

            foreach (var cc in Db.SQL<CC>("select c from CC c"))
            {
                // TakimOyunculari
                foreach (var ctp in Db.SQL<CTP>("select c from CTP c where c.CC = ?", cc))
                {
                    PPdic[ctp.PPoNo].CCoNo = cc.oNo;
                    PPdic[ctp.PPoNo].CToNo = ctp.CToNo;
                }
                // OyuncuMaclari
                var rhs = Db.SQL<BDB.RH>("select p from RH p order by p.Trh", RnkID);


                foreach (var rh in Db.SQL<RH>("select c from RH c where c.CC = ?", cc))
                {
                    PPdic[rh.hPP.oNo].CCoNo = cc.oNo;
                    PPdic[ctp.PPoNo].CToNo = ctp.CToNo;
                }

            }
            */
        }

        public static void UpdCETsum(ulong CEToNo)
        {
            var cet = Db.FromId<BDB.CET>(CEToNo);

            if (!cet.Rok)   // Sonuclar onaylanmamis
                return;

            Db.Transact(() =>
            {
                // Musabakaya gelmeyip Hukmen Maglup da CETR kayitlari yok! Rok=true h/g Puani manuel girilmis.
                var cetr = Db.SQL<CETR>("select c from CETR c where c.CET = ?", cet).FirstOrDefault();
                if (cetr == null)
                {
                    // Sonuclar yok sadece TurnuvaPuani var hP/gP
                    // Digerlerini sifirla

                    cet.hPW = 0;
                    cet.hMSW = 0;
                    cet.hMDW = 0;
                    cet.hSSW = 0;
                    cet.hSDW = 0;

                    cet.gPW = 0;
                    cet.gMSW = 0;
                    cet.gMDW = 0;
                    cet.gSSW = 0;
                    cet.gSDW = 0;

                    return;
                }


                // HomeRecs
                int aSetS = 0, aSetD = 0;
                int aMacS = 0, aMacD = 0;
                var hRs = Db.SQL<CETR>("select c from CETR c where c.CET = ? and c.HoG = ?", cet, "H");
                foreach(var r in hRs)
                {
                    if (r.SoD == "S")
                    {
                        aSetS += r.SW;
                        aMacS += r.MW;
                    }
                    else // Double da her kisi icin bir kayit var!!
                    {
                        aSetD += r.SW;
                        aMacD += r.MW;
                    }
                }
                cet.hMSW = aMacS;
                cet.hMDW = aMacD / 2;   // Iki defa sayildigi icin
                cet.hSSW = aSetS;
                cet.hSDW = aSetD / 2;

                // GuestRecs
                aSetS = 0; aSetD = 0;
                aMacS = 0; aMacD = 0;
                var gRs = Db.SQL<CETR>("select c from CETR c where c.CET = ? and c.HoG = ?", cet, "G");
                foreach (var r in gRs)
                {
                    if (r.SoD == "S")
                    {
                        aSetS += r.SW;
                        aMacS += r.MW;
                    }
                    else // Double da her kisi icin bir kayit var!! 
                    {
                        aSetD += r.SW;
                        aMacD += r.MW;
                    }
                }
                cet.gMSW = aMacS;
                cet.gMDW = aMacD / 2;   // Iki defa sayildigi icin
                cet.gSSW = aSetS;
                cet.gSDW = aSetD / 2;

                // Skor
                cet.hPW = (cet.hMSW * 2) + (cet.hMDW * 3);
                cet.gPW = (cet.gMSW * 2) + (cet.gMDW * 3);
                // Puan
                cet.hP = 0;
                cet.gP = 0;
                if (cet.hPW > cet.gPW)
                {
                    cet.hP = 2;
                    cet.gP = 1;
                }
                else if (cet.hPW < cet.gPW)
                {
                    cet.hP = 1;
                    cet.gP = 2;
                }
            });

            UpdCTsum(cet.hCT.oNo);
            UpdCTsum(cet.gCT.oNo);
        }

        public static void UpdCTsumCC(ulong CCoNo)
        {
            //var cc = Db.SQL<CC>("select r from CC r where r.ID = ?", ccID).FirstOrDefault();
            var cc = Db.FromId<CC>(CCoNo);

            var cts = Db.SQL<CT>("select r from CT r where r.CC = ?", cc);
            
            foreach (var ct in cts)
            {
                UpdCTsum(ct.oNo);
            }
        }

        public static void UpdCTsum(ulong CToNo)
        {
            int tP = 0;  // Takim Puan

            int oM = 0;  // Oynadigi Musabaka
            int aM = 0;  // Aldigi 
            int vM = 0;  // Verdigi 
            int fM = 0;  // Fark 

            int aMP = 0; // Aldigi MusabakaPuani/Skor
            int vMP = 0; // Verdigi 
            int fMP = 0; // Fark

            int aO = 0;  // Aldigi Oyun/Mac
            int vO = 0;  // Verdigi 
            int fO = 0;  // Fark

            int aS = 0;  // Aldigi Set sayisi
            int vS = 0;  // Verdigi 
            int fS = 0;  // Fark

            var ct = Db.FromId<CT>(CToNo);

            // Musabaka Soncu Onaylanmislari tara
            // HOME
            foreach (var r in Db.SQL<CET>("select c from CET c where c.hCT = ? and c.Rok = ?", ct, true))
            {
                oM++;
                
                if (r.hP > r.gP)
                    aM++;
                else
                    vM++;
                tP += r.hP;
                aMP += r.hPW;
                vMP += r.gPW;

                aO += r.hMSW + r.hMDW;
                vO += r.gMSW + r.gMDW;

                aS += r.hSSW + r.hSDW;
                vS += r.gSSW + r.gSDW;
            }
            // GUEST
            foreach (var r in Db.SQL<CET>("select c from CET c where c.gCT = ? and c.Rok = ?", ct, true))
            {
                oM++;
                
                if (r.gP > r.hP)
                    aM++;
                else
                    vM++;
                tP += r.gP;
                aMP += r.gPW;
                vMP += r.hPW;

                aO += r.gMSW + r.gMDW;
                vO += r.hMSW + r.hMDW;

                aS += r.gSSW + r.gSDW;
                vS += r.hSSW + r.hSDW;
            }
            fM = aM - vM;
            fMP = aMP - vMP;
            fO = aO - vO;
            fS = aS - vS;

            // Calculate TakimRank Sum(Rnk) / Count(CTP)
            int ppCnt = 0;
            int ppRnkSum = 0;
            foreach (var r in Db.SQL<CTP>("select c from CTP c where c.CT = ?", ct))
            {
                ppCnt++;
                ppRnkSum += r.PP.Rnk == 0 ? r.PP.RnkBaz : r.PP.Rnk;
            }

            Db.Transact(() =>
            {
                ct.tP = tP;
                ct.oM = oM;
                ct.aM = aM;
                ct.vM = vM;
                ct.fM = fM;
                ct.aMP = aMP;
                ct.vMP = vMP;
                ct.fMP = fMP;
                ct.aO = aO;
                ct.vO = vO;
                ct.fO = fO;
                ct.aS = aS;
                ct.vS = vS;
                ct.fS = fS;

                ct.tRnk = ppRnkSum / ppCnt;
            });
        }

        public static void CompCTtRnkOfCC(ulong CCoNo)
        {
            // Calculate TakimRank Sum(Rnk) / Count(CTP)

            var cc = Db.FromId<CC>(CCoNo);
            Db.Transact(() =>
            {
                foreach (var ct in Db.SQL<CT>("select c from CT c where c.CC = ?", cc))
                {
                    int ppCnt = 0;
                    int ppRnkSum = 0;
                    foreach (var r in Db.SQL<CTP>("select c from CTP c where c.CT = ?", ct))
                    {
                        ppCnt++;
                        ppRnkSum += r.PP.Rnk == 0 ? r.PP.RnkBaz : r.PP.Rnk;
                    }

                    ct.tRnk = ppRnkSum / ppCnt;
                }
            });
        }

        public static void CompCTtRnk(ulong CToNo)
        {
            // Calculate TakimRank Sum(Rnk) / Count(CTP)

            var ct = Db.FromId<CT>(CToNo);
            Db.Transact(() =>
            {
                int ppCnt = 0;
                int ppRnkSum = 0;
                foreach (var r in Db.SQL<CTP>("select c from CTP c where c.CT = ?", ct))
                {
                    ppCnt++;
                    ppRnkSum += r.PP.Rnk == 0 ? r.PP.RnkBaz : r.PP.Rnk;
                }

                ct.tRnk = ppRnkSum / ppCnt;
            });
        }

        public static void ReCreateRHofCC(ulong CCoNo)
        {
            // Rank RH kayitlarini yarat, Sadece Singles
            // Sonrasinda RefreshRH gerekir!!!

            var cc = Db.FromId<BDB.CC>(CCoNo);

            // Delete PRHs of CC
            Db.Transact(() =>
            {
                var cetrs = Db.SQL<CETR>("select r from CETR r where r.CC = ?", cc);
                foreach (var cetr in cetrs)
                {
                    if (cetr.RH != null)
                    {
                        Db.FromId<RH>(cetr.RH.GetObjectNo()).Delete();
                        cetr.RH = null;
                    }
                }
            });

            Db.Transact(() =>
            {
                CETR h = null, g = null;
                int Won = 0;
                RH rh;
                // Sadece Singles
                var cetrs = Db.SQL<CETR>("select r from CETR r where r.CC = ? and r.SoD = ? order by r.CET, r.Idx, r.HoG desc", cc, "S");
                
                foreach (var r in cetrs)
                {
                    // Ilk H sonra G gelir
                    if (r.HoG == "H")
                    {
                        h = r;
                        if (r.MW > r.ML)
                            Won = 1;
                        else if (r.MW < r.ML)
                            Won = -1;

                    }
                    else if (r.HoG == "G")
                    {
                        g = r;

                        rh = new RH
                        {
                            CC = r.CC,
                            Trh = r.Trh,

                            hPP = h.PP,
                            hWon = Won,
                            gPP = g.PP,
                            gWon = -Won,
                        };
                        h.RH = rh;
                        g.RH = rh;
                    }
                }
            });
            // RH daki herhangi bir degisiklik hepsini etkiler!
            //refreshRH();
        }

        public static void CreateRHofCET(ulong CEToNo)
        {
            var cet = Db.FromId<BDB.CET>(CEToNo);

            Db.Transact(() =>
            {
                CETR h = null, g = null;
                int Won = 0;
                RH rh;
                // Sadece Singles
                var cetrs = Db.SQL<CETR>("select r from CETR r where r.CET = ? and r.SoD = ? order by r.CET, r.Idx, r.HoG desc", cet, "S");

                foreach (var r in cetrs)
                {
                    // Ilk H sonra G gelir
                    if (r.HoG == "H")
                    {
                        h = r;
                        if (r.MW > r.ML)
                            Won = 1;
                        else if (r.MW < r.ML)
                            Won = -1;

                    }
                    else if (r.HoG == "G")
                    {
                        g = r;

                        rh = new RH
                        {
                            CC = r.CC,
                            Trh = r.Trh,

                            hPP = h.PP,
                            hWon = Won,
                            gPP = g.PP,
                            gWon = -Won,

                        };
                        h.RH = rh;
                        g.RH = rh;
                    }
                }
            });
        }

        public static void UpdPPLigMacSay()
        {
            // 10msec;
            Stopwatch watch = new Stopwatch();
            watch.Start();

            var cetrs = Db.SQL<CETR>("select c from CETR c").GroupBy((x) => new { x.PP.oNo, x.CC.Lig });
            int nor = 0;
            Db.Transact(() =>
            {
                foreach(var c in cetrs)
                {
                    var p = Db.FromId<PP>(c.Key.oNo);

                    if (c.Key.Lig == "1")
                        p.L1C = c.Count();
                    else if (c.Key.Lig == "2")
                        p.L2C = c.Count();
                    else if (c.Key.Lig == "3")
                        p.L3C = c.Count();
                        
                    nor += c.Count();
                }
            });
            watch.Stop();
            Console.WriteLine($"UpdPPLigMacSay {nor}: {watch.ElapsedMilliseconds} msec  {watch.ElapsedTicks} ticks");
        }

        public static void UpdPPLigMacSay(ulong CEToNo)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var cet = Db.FromId<CET>(CEToNo);

            var pps = Db.SQL<CETR>("select c from CETR c where c.CET = ?", cet).Select(x => x.PP).Distinct();  //Distinct((x) => x.PP.  //GroupBy((x) => x.PP.oNo);
            int nor = 0;
            Db.Transact(() =>
            {
            foreach (var p in pps)
            {
                var cetrs = Db.SQL<CETR>("select c from CETR c where c.PP = ?", p).GroupBy( x => x.CC.Lig );
                   
                    foreach (var c in cetrs)
                    {
                        
                        if (c.Key == "1")
                            p.L1C = c.Count();
                        else if (c.Key == "2")
                            p.L2C = c.Count();
                        else if (c.Key == "3")
                            p.L3C = c.Count();
                            
                        nor += c.Count();
                    }
            }
            });

            watch.Stop();
            Console.WriteLine($"UpdPPLigMacSayCET {nor}: {watch.ElapsedMilliseconds} msec  {watch.ElapsedTicks} ticks");
        }

        public static void initBazRanks()
        {
            Db.Transact(() =>
            {
                // BazRnk sifirla
                var pps = Db.SQL<BDB.PP>("select p from PP p");
                foreach (var p in pps)
                {
                    p.RnkBaz = 0;
                }

                // 1.Lig
                var tps1 = Db.SQL<CTP>("select p from CTP p where p.CC.ObjectNo = ?", 185).GroupBy((x) => new { x.PP.oNo }); ;
                foreach (var tp in tps1)
                {
                    var p = Db.FromId<PP>(tp.Key.oNo);
                    p.RnkBaz = 1900;
                }
                // 2.Lig
                var tps2 = Db.SQL<CTP>("select p from CTP p where p.CC.ObjectNo = ? or p.CC.ObjectNo = ?", 186, 187).GroupBy((x) => new { x.PP.oNo }); ;
                foreach (var tp in tps2)
                {
                    var p = Db.FromId<PP>(tp.Key.oNo);
                    if (p.RnkBaz == 0)  // Daha once tanimlanmadiysa
                        p.RnkBaz = 1800;
                }
                // 3.Lig
                var tps3 = Db.SQL<CTP>("select p from CTP p where p.CC.ObjectNo = ? or p.CC.ObjectNo = ?", 188, 189).GroupBy((x) => new { x.PP.oNo }); ;
                foreach (var tp in tps3)
                {
                    var p = Db.FromId<PP>(tp.Key.oNo);
                    if (p.RnkBaz == 0)  // Daha once tanimlanmadiysa
                        p.RnkBaz = 1700;
                }
            });
        }

        public static void CETRtoMAC()
        {
            int s = 0;
            MAC m = null;
            Db.Transact(() =>
            {
                foreach (var cet in Db.SQL<CET>("select c from CET c"))
                {
                    s = 0;
                    foreach (var cetr in Db.SQL<BDB.CETR>("select c from CETR c where c.CET = ? and c.SoD = ? order by c.Idx, c.HoG desc", cet, "S"))
                    {
                        if ((s % 2) == 0)   // Home
                        {
                            m = new MAC();

                            m.CC = cetr.CC;
                            m.CET = cetr.CET;
                            m.SoD = cetr.SoD;
                            m.Trh = cet.Trh;
                            m.Idx = cetr.Idx;

                            m.hPP1 = cetr.PP;
                            m.hDrm = 0;
                            m.hS1W = cetr.S1W;
                            m.hS2W = cetr.S2W;
                            m.hS3W = cetr.S3W;
                            m.hS4W = cetr.S4W;
                            m.hS5W = cetr.S5W;
                            m.hSW = cetr.SW;
                            m.hMW = cetr.MW;
                            //m.hPW = cetr.MW * 2;
                            m.hpRnk = 0;
                            m.hNOPX = 0;
                        }
                        else if ((s % 2) == 1)   // Guest
                        {
                            m.gPP1 = cetr.PP;
                            m.gDrm = 0;
                            m.gS1W = cetr.S1W;
                            m.gS2W = cetr.S2W;
                            m.gS3W = cetr.S3W;
                            m.gS4W = cetr.S4W;
                            m.gS5W = cetr.S5W;
                            m.gSW = cetr.SW;
                            m.gMW = cetr.MW;
                            //m.gPW = cetr.MW * 2;
                            m.gpRnk = 0;
                            m.gNOPX = 0;
                        }

                        s++;
                    }

                    s = 0;
                    foreach (var cetr in Db.SQL<BDB.CETR>("select c from CETR c where c.CET = ? and c.SoD = ? order by c.Idx, c.HoG desc", cet, "D"))
                    {
                        if ((s % 4) == 0)
                        {
                            m = new MAC();

                            m.CC = cetr.CC;
                            m.CET = cetr.CET;
                            m.SoD = cetr.SoD;
                            m.Trh = cet.Trh;
                            m.Idx = cetr.Idx;

                            m.hPP1 = cetr.PP;
                            m.hDrm = 0;
                            m.hS1W = cetr.S1W;
                            m.hS2W = cetr.S2W;
                            m.hS3W = cetr.S3W;
                            m.hS4W = cetr.S4W;
                            m.hS5W = cetr.S5W;
                            m.hSW = cetr.SW;
                            m.hMW = cetr.MW;
                            //m.hPW = cetr.MW * 3;
                        }
                        if ((s % 4) == 1)
                        {
                            m.hPP2 = cetr.PP;
                        }
                        if ((s % 4) == 2)
                        {
                            m.gPP1 = cetr.PP;
                            m.gDrm = 0;
                            m.gS1W = cetr.S1W;
                            m.gS2W = cetr.S2W;
                            m.gS3W = cetr.S3W;
                            m.gS4W = cetr.S4W;
                            m.gS5W = cetr.S5W;
                            m.gSW = cetr.SW;
                            m.gMW = cetr.MW;
                            //m.gPW = cetr.MW * 3;
                        }
                        if ((s % 4) == 3)
                        {
                            m.gPP2 = cetr.PP;
                        }

                        s++;
                    }
                }
            });
        }

        public static void DENEME()
        {
            ulong pp = 103;
            var items = from m in Db.SQL<MAC>("select m from MAC m")
                        where m.hPP1.oNo == pp || m.gPP1.oNo == pp
                        orderby m.hMW descending
                        select m;

            int MSW = 0;
            foreach (var m in Db.SQL<MAC>("select m from MAC m where m.hPP1.oNo = ? or m.gPP1.oNo = ?", pp, pp))
            {
                if (m.hPP1.oNo == pp)
                    MSW = m.hSW;
                else
                    MSW = m.gSW;

            }
        }

        public static void RefreshGlobalRank()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            int nor = 0;

            Dictionary<ulong, int> ppDic = new Dictionary<ulong, int>();    // Players

            Db.Transact(() =>
            {
                //for (int i = 0; i < 100; i++) // SpeedTest 85K/sn R+R+U+R+U
                {
                    foreach (var p in Db.SQL<BDB.PP>("select p from PP p"))
                    {
                        ppDic[p.oNo] = p.RnkBaz;
                    }
                    MAC m;
                    ulong hPPoNo, gPPoNo; 
                    int hpRnk, gpRnk;
                    int NOPX = 0;
                    // Sadece Single Rank uretir
                    //foreach (var mac in Db.SQL<BDB.MAC>("select m from MAC m where m.SoD = ? order by m.Trh", "S"))
                    foreach (var mac in Db.SQL<BDB.MAC>("select m from MAC m where m.SoD = ? order by m.Trh", "S"))
                    {
                        nor++;
                        
                        //m = mac;
                        hPPoNo = mac.hPP1.oNo;
                        gPPoNo = mac.gPP1.oNo;

                        hpRnk = ppDic[hPPoNo];
                        gpRnk = ppDic[gPPoNo];
                        
                        if (mac.hDrm > 0 || mac.gDrm > 0) // 1:Oynamadi/2:SiralamaHatasi ise Rank hesaplma
                            NOPX = 0;
                        else
                            NOPX = compNOPX(mac.hMW == 0 ? -1 : 1, hpRnk, gpRnk);
                        
                        // Update MAC
                        mac.hNOPX = NOPX;
                        mac.gNOPX = -NOPX;
                        mac.hpRnk = hpRnk;
                        mac.gpRnk = gpRnk;
                        
                        // Update dictionary of PP
                        ppDic[hPPoNo] = hpRnk + NOPX;
                        ppDic[gPPoNo] = gpRnk - NOPX;
                    }
                    
                    // Rank'e gore Sira verip PP ye koy
                    var items = from pair in ppDic
                                orderby pair.Value descending
                                select pair;

                    int sira = 1;
                    PP pp;
                    foreach (var pair in items)
                    {
                        pp = Db.FromId<PP>(pair.Key);
                        pp.Rnk = pair.Value;
                        pp.Sra = sira++;
                    }
                }
            });

            watch.Stop();
            Console.WriteLine($"RefreshGlobalRank {nor}: {watch.ElapsedMilliseconds} msec  {watch.ElapsedTicks} ticks");
        }

        public static void RefreshRH()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            int nor = 0;

            PP hPP;
            PP gPP;
            int NOPX = 0;
            Db.Transact(() =>
            {
                // Init
                // PP deki Rnk son degeri 
                var pps = Db.SQL<BDB.PP>("select p from PP p");
                foreach (var p in pps)
                {
                    p.Rnk = 0; // p.RnkBaz;
                }
                
                int hRnk = 0, gRnk = 0;
                int hpRnk = 0, gpRnk = 0;
                int hLig, gLig;
                // ReCalculate Rank of All Players
                //var rhs = Db.SQL<BDB.RH>("select p from RH p").OrderBy(x => x.Trh);
                var rhs = Db.SQL<BDB.RH>("select p from RH p order by p.Trh");
                foreach (var r in rhs)
                {
                    nor++;

                    hPP = Db.FromId<PP>(r.hPP.GetObjectNo());
                    gPP = Db.FromId<PP>(r.gPP.GetObjectNo());

                    hRnk = hPP.Rnk;
                    gRnk = gPP.Rnk;

                    hpRnk = hRnk == 0 ? hPP.RnkBaz : hRnk;
                    gpRnk = gRnk == 0 ? gPP.RnkBaz : gRnk;

                    if (r.oNo == 26625)     // Cagdas-Fatih Maci, Cagdas siralama Hatasi
                        NOPX = 0;
                    else if (r.hWon == 0 && r.gWon == 0) // Ikisi de maca cikmamis
                        NOPX = 0;
                    else
                    {
                        if (r.hWon == 0 || hpRnk == -1 || gpRnk == -1)   // Oynanmamis veya Oyunculardan biri diskalifiye ise Rank hesaplama
                            NOPX = 0;
                        else
                            NOPX = compNOPX(r.hWon, hpRnk, gpRnk);
                    }

                    /*
                    hLig = hPP.L1C != 0 ? 1 : hPP.L2C != 0 ? 2 : hPP.L3C != 0 ? 3 : 0;  // Oynadigi en ust lig.
                    gLig = gPP.L1C != 0 ? 1 : gPP.L2C != 0 ? 2 : gPP.L3C != 0 ? 3 : 0;  // Oynadigi en ust lig.

                    if (hLig != gLig)   // Farkli Lig oyunculari (Hangi ligde mac yaptiklari onemli degil)
                    {
                        if (hLig < gLig && r.hWon == 1) // H olan UstLig kazanmis
                            NOPX = 0;
                        if (gLig < hLig && r.gWon == 1) // G olan UstLig kazanmis
                            NOPX = 0;
                    }
                    */

                    r.hNOPX = NOPX;
                    r.gNOPX = -NOPX;
                    r.hpRnk = hpRnk;
                    r.gpRnk = gpRnk;

                    hPP.Rnk = hpRnk + NOPX;
                    gPP.Rnk = gpRnk - NOPX;
                    
                }

                // Update PP Sira
                var pp = Db.SQL<BDB.PP>("select p from PP p order by p.Rnk desc, p.LTC desc");
                int sira = 1;
                foreach (var r in pp)
                {
                    r.Sra = sira++;
                }
            });
            
            watch.Stop();
            Console.WriteLine($"refreshRH {nor}: {watch.ElapsedMilliseconds} msec  {watch.ElapsedTicks} ticks");
        }

        public static void RefreshRH4(DateTime basTrh)
        {
            // Kucuk bir tarih verilerek basindan beri yaptirilabilir
            // RefreshRH'dan 2x hizli
            Stopwatch watch = new Stopwatch();
            watch.Start();
            int nor = 0;
            PP pp;

            Dictionary<ulong, int> myDic = new Dictionary<ulong, int>(1000);    // Initial capacity 1000 players

            // basTrh'e kadarki PP lerin sonRank'ini kaydet.
            // Oyuncunun bircok degeri var. RH tarih sirali basindan beri okundugu icin en sonu istenen rank
            foreach (var r in Db.SQL<BDB.RH>("select p from RH p where p.Trh < ? order by p.Trh", basTrh))
            {
                myDic[r.hPP.oNo] = r.hpRnk + r.hNOPX;
                myDic[r.gPP.oNo] = r.gpRnk + r.gNOPX;
            }

            ulong rhPPoNo = 0, rgPPoNo = 0;
            int hpRnk = 0, gpRnk = 0, rnkBaz = 0;
            int NOPX = 0;
            Db.Transact(() =>
            {
                foreach (var r in Db.SQL<BDB.RH>("select p from RH p where p.Trh >= ? order by p.Trh", basTrh))
                {
                    nor++;
                    rhPPoNo = r.hPP.GetObjectNo();
                    rgPPoNo = r.gPP.GetObjectNo();

                    if (!myDic.ContainsKey(rhPPoNo))
                    {
                        rnkBaz = r.hPP.RnkBaz;
                        myDic[rhPPoNo] = rnkBaz;
                        hpRnk = rnkBaz;
                    }
                    else
                        hpRnk = myDic[rhPPoNo];

                    if (!myDic.ContainsKey(rgPPoNo))
                    {
                        rnkBaz = r.gPP.RnkBaz;
                        myDic[rgPPoNo] = rnkBaz;
                        gpRnk = rnkBaz;
                    }
                    else
                        gpRnk = myDic[rgPPoNo];

                    //if (r.hWon == 0 || r.hPP.RnkBaz == -1 || r.gPP.RnkBaz == -1)   // Oynanmamis veya Oyunculardan biri diskalifiye ise Rank hesaplama
                    if (r.hWon == 0 || hpRnk < 1 || gpRnk < 1)   // Oynanmamis veya Oyunculardan biri diskalifiye ise Rank hesaplama
                        NOPX = 0;
                    else
                        NOPX = compNOPX(r.hWon, hpRnk, gpRnk);
                    
                    // Update RHs
                    r.hNOPX = NOPX;
                    r.gNOPX = -NOPX;
                    r.hpRnk = hpRnk;
                    r.gpRnk = gpRnk;
                    
                    // Update dictionary of PP
                    myDic[rhPPoNo] = hpRnk + NOPX;
                    myDic[rgPPoNo] = gpRnk - NOPX;

                }

                int sira = 1;
                /*
                // Copy Dict to PP
                foreach (var pair in myDic)
                {
                    pp = Db.FromId<PP>(pair.Key);
                    pp.Rnk = pair.Value;
                }

                // Update PP Sira
                sira = 1;
                foreach (var r in Db.SQL<BDB.PP>("select p from PP p order by p.Rnk desc"))
                {
                    r.Sra = sira++;
                }
                */
                
                var items = from pair in myDic
                            orderby pair.Value descending
                            select pair;
                sira = 1;
                foreach (var pair in items)
                {
                    pp = Db.FromId<PP>(pair.Key);
                    pp.Rnk = pair.Value;
                    pp.Sra = sira++;

                }

            });

            watch.Stop();
            Console.WriteLine($"refreshRH4 {nor}: {watch.ElapsedMilliseconds} msec  {watch.ElapsedTicks} ticks");
        }
 
        public static void RefreshRH24(ulong CCoNo)
        {
            // DENEME
            Stopwatch watch = Stopwatch.StartNew();
            int nor = 0;
            var cc = Db.FromId<CC>(CCoNo);
            long RnkID = cc.RnkID;

            Dictionary<ulong, int> myDic = new Dictionary<ulong, int>(1000);    // Initial capacity 1000 players

            ulong rhPPoNo = 0, rgPPoNo = 0;
            int hpRnk = 0, gpRnk = 0, rnkBaz = 0;
            int NOPX = 0;
            Db.Transact(() =>
            {
                var rhs = Db.SQL<BDB.RH>("select p from RH p where p.CC.RnkID = ? order by p.Trh", RnkID);
                foreach (var r in rhs)
                {
                    nor++;
                    rhPPoNo = r.hPP.GetObjectNo();
                    rgPPoNo = r.gPP.GetObjectNo();

                    if (!myDic.ContainsKey(rhPPoNo))
                    {
                        rnkBaz = r.hPP.RnkBaz;
                        myDic[rhPPoNo] = rnkBaz;
                        hpRnk = rnkBaz;
                    }
                    else
                        hpRnk = myDic[rhPPoNo];

                    if (!myDic.ContainsKey(rgPPoNo))
                    {
                        rnkBaz = r.gPP.RnkBaz;
                        myDic[rgPPoNo] = rnkBaz;
                        gpRnk = rnkBaz;
                    }
                    else
                        gpRnk = myDic[rgPPoNo];

                    if (r.hWon == 0 || hpRnk < 1 || gpRnk < 1)   // Oynanmamis veya Oyunculardan biri diskalifiye ise Rank hesaplama
                        NOPX = 0;
                    else
                        NOPX = compNOPX(r.hWon, hpRnk, gpRnk);

                    // Update RHs2
                    r.hNOPX2 = NOPX;
                    r.gNOPX2 = -NOPX;
                    r.hpRnk2 = hpRnk;
                    r.gpRnk2 = gpRnk;

                    // Update dictionary of PP
                    myDic[rhPPoNo] = hpRnk + NOPX;
                    myDic[rgPPoNo] = gpRnk - NOPX;

                }
                /*
                int sira = 1;

                var items = from pair in myDic
                            orderby pair.Value descending
                            select pair;
                sira = 1;
                PPGR ppgr;
                foreach (var pair in items)
                {
                    ppgr = Db.SQL<PPGR>("select p from PPGR p where p.PP.ObjectNo = ? and p.RnkID = ?", pair.Key, RnkID).FirstOrDefault();
                    if (ppgr == null)
                    {
                        new PPGR()
                        {
                            PP = Db.FromId<PP>(pair.Key),
                            RnkID = RnkID,
                            RnkBaz = 0,
                            Rnk = pair.Value,
                            Sra = sira++,
                        };
                    }
                    else
                    {
                        ppgr.Rnk = pair.Value;
                        ppgr.Sra = sira++;
                    }
                }
                */
            });

            watch.Stop();
            Console.WriteLine($"refreshRH24 {nor}: {watch.ElapsedMilliseconds} msec  {watch.ElapsedTicks} ticks");
        }

        public static PPGR getPPGR(PP pp, long RnkID)
        {
            PPGR ppgr = Db.SQL<PPGR>("select p from PPGR p where p.PP = ? and p.RnkID = ?", pp, RnkID).FirstOrDefault();
            if (ppgr == null)
            {
                ppgr = new PPGR
                {
                    PP = pp,
                    RnkID = RnkID,
                    RnkBaz = pp.RnkBaz,
                    Rnk = pp.RnkBaz
                };
            }
            return ppgr;
        }

        public static void RefreshPpGrpRnk(ulong PPoNo)
        {
            // Oyuncunun oynadigi turnuvalari bul
            var cc = Db.SQL<CETR>("select c from CETR c where c.PP.ObjectNo = ?", PPoNo).Select(x => x.CC).Distinct();
            foreach(var c in cc)
            {
                RefreshRH2(c.oNo);  
            }
        }

        public static void RefreshRH2(ulong CCoNo)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            int nor = 0;
            var cc = Db.FromId<CC>(CCoNo);
            long RnkID = cc.RnkID;

            PPGR hPP;
            PPGR gPP;
            int NOPX = 0;
            Db.Transact(() =>
            {
                // Init
                // PP deki Rnk son degeri 

                var pps = Db.SQL<BDB.PPGR>("select p from PPGR p where p.RnkID = ?", RnkID);
                foreach (var p in pps)
                {
                    p.RnkBaz = p.PP.RnkBaz;
                    p.Rnk = p.PP.RnkBaz; // p.RnkBaz;
                    p.aO = 0;
                    p.vO = 0;
                }

                // ReCalculate Rank of RnkGrp Players
                int hpRnk = 0, gpRnk = 0;
                var rhs = Db.SQL<BDB.RH>("select p from RH p where p.CC.RnkID = ? order by p.Trh", RnkID);
                foreach (var r in rhs)
                {
                    nor++;

                    hPP = getPPGR(r.hPP, RnkID);
                    gPP = getPPGR(r.gPP, RnkID);

                    hpRnk = hPP.Rnk;
                    gpRnk = gPP.Rnk;

                    //if (r.hWon == 0 || r.hPP.Ad.StartsWith("∞") || r.gPP.Ad.StartsWith("∞"))   // Oynanmamis veya Oyunculardan biri diskalifiye ise Rank hesaplama
                    if (r.hWon == 0 && r.gWon == 0) // Ikisi de maca cikmamis
                        NOPX = 0;
                    else
                    {
                        if (r.hWon == 0 || r.hPP.RnkBaz == -1 || r.gPP.RnkBaz == -1)   // Oynanmamis veya Oyunculardan biri diskalifiye ise Rank hesaplama
                            NOPX = 0;
                        else
                            NOPX = compNOPX(r.hWon, hpRnk, gpRnk);
                    }
                    r.hNOPX2 = NOPX;
                    r.gNOPX2 = -NOPX;
                    r.hpRnk2 = hpRnk;
                    r.gpRnk2 = gpRnk;

                    hPP.Rnk = hpRnk + NOPX;
                    gPP.Rnk = gpRnk - NOPX;

                    hPP.RnkANY = NOPX;
                    gPP.RnkANY = -NOPX;

                    hPP.aO += r.hWon > 0 ? 1 : 0;
                    hPP.vO += r.hWon < 0 ? 1 : 0;
                    gPP.aO += r.gWon > 0 ? 1 : 0;
                    gPP.vO += r.gWon < 0 ? 1 : 0;
                }

                // Update PP Sira
                var ppgr = Db.SQL<BDB.PPGR>("select p from PPGR p where p.RnkID = ? order by p.Rnk desc", RnkID);
                int sira = 1;
                foreach (var r in ppgr)
                {
                    r.Sra = sira++;
                }
            });
            watch.Stop();
            Console.WriteLine($"refreshRH2 {nor}: {watch.ElapsedMilliseconds} msec  {watch.ElapsedTicks} ticks");
        }

        public static int compNOPX(int Won, int pRnk, int pRnkRkp)
        {
            int NOPX = 0;

            int PS = 0; // Point Spread between players
            int ER = 0; // ExpectedResult
            int UR = 0; // UpsetResult

            // Compute
            PS = Math.Abs(pRnk - pRnkRkp);

            if (PS < 13)
            {
                ER = 8;
                UR = 8;
            }
            else if (PS < 38)
            {
                ER = 7;
                UR = 10;
            }
            else if (PS < 63)
            {
                ER = 6;
                UR = 13;
            }
            else if (PS < 88)
            {
                ER = 5;
                UR = 16;
            }
            else if (PS < 113)
            {
                ER = 4;
                UR = 20;
            }
            else if (PS < 138)
            {
                ER = 3;
                UR = 25;
            }
            else if (PS < 163)
            {
                ER = 2;
                UR = 30;
            }
            else if (PS < 188)
            {
                ER = 2;
                UR = 35;
            }
            else if (PS < 213)
            {
                ER = 1;
                UR = 40;
            }
            else if (PS < 238)
            {
                ER = 1;
                UR = 45;
            }
            else
            {
                ER = 0;
                UR = 50;
            }
            if (pRnk >= pRnkRkp)   // Target: Iyi
            {
                if (Won > 0)     // Expected: Target kazanmis
                    NOPX += ER;
                else             // Upset: Target kaybetmis
                    NOPX -= UR;
            }
            else                 // Rakip: Iyi
            {
                if (Won < 0)     // Expected: Rakip kazanmis
                    NOPX -= ER;
                else             // Upset: Rakip kaybetmis
                    NOPX += UR;
            }

            return NOPX;
        }
        
        public static int PPprvRnk(ulong PPoNo, DateTime Trh)
        {
            var p = Db.FromId<PP>(PPoNo);
            var r = Db.SQL<BDB.RH>("select p from RH p where (p.hPP = ? or p.gPP = ?) and p.Trh <= ? order by p.Trh desc", p, p, Trh).FirstOrDefault();
            //return r?.Rnk ?? Db.FromId<BDB.PP>(PPoNo).RnkBaz;    // Zaten prev kayit Rnk al, prvRnk degil
            if (r != null)
            {
                if (PPoNo == r.hPP.GetObjectNo())
                    return r.hpRnk;
                else
                    return r.gpRnk;
            }
            else
            {
                return 0;       // Simdilik boyle aslinda BazRank gelebilir
            }
        }

        public static void DeleteCETrelated(ulong CEToNo)  // Delete Musabaka alti
        {
            RH rh;
            var cet = Db.FromId<CET>(CEToNo);
            var cetps = Db.SQL<CETP>("select r from CETP r where r.CET = ?", cet);
            var cetrs = Db.SQL<CETR>("select r from CETR r where r.CET = ?", cet);

            Db.Transact(() =>
            {
                foreach (var cetp in cetps)
                {
                    cetp.Delete();
                }

                foreach (var cetr in cetrs)
                {
                    if (cetr.RH != null)
                    {
                        rh = Db.FromId<RH>(cetr.RH.GetObjectNo());
                        rh.Delete();
                    }
                    cetr.Delete();
                }

                cet.hPok = false;
                cet.gPok = false;
                cet.Rok = false;
                cet.hP = 0;
                cet.hPW = 0;
                cet.hMSW = 0;
                cet.hMDW = 0;
                cet.gP = 0;
                cet.gPW = 0;
                cet.gMSW = 0;
                cet.gMDW = 0;

                RefreshRH();
                UpdCTsum(cet.hCT.oNo);
                UpdCTsum(cet.gCT.oNo);
            });
        }

        //--------------------------------------------

        /*
        public static void updPPsum()
        {
            var pp = Db.SQL<BDB.PP>("select p from PP p");
            foreach(var p in pp)
            {
                updPPsum(p.oNo);
            }
        }

        public static void updPPsum(ulong oNo)
        {
            var pp = Db.FromId<BDB.PP>(oNo);
            int SMo = 0, SMa = 0, SMv = 0;
            int DMo = 0, DMa = 0, DMv = 0;
            int SSo = 0, SSa = 0, SSv = 0;
            int DSo = 0, DSa = 0, DSv = 0;

            var cetr = Db.SQL<BDB.CETR>("select c from CETR c where c.PP = ?", pp);
            foreach(var k in cetr)
            {
                if ((k.SW + k.SL) != 0)
                {
                    if (k.SoD == "S")
                    {
                        SMo++;
                        SMa += k.MW;
                        SMv += k.ML;
                        SSa += k.SW;
                        SSv += k.SL;
                        SSo += SSa + SSv;
                    }
                    else
                    {
                        DMo++;
                        DMa += k.MW;
                        DMv += k.ML;
                        DSa += k.SW;
                        DSv += k.SL;
                        DSo += DSa + DSv;
                    }
                }
            }
            Db.Transact(() =>
            {
                pp.SSo = SSo;
                pp.SSa = SSa;
                pp.SSv = SSv;
                pp.SMo = SMo;
                pp.SMa = SMa;
                pp.SMv = SMv;

                pp.DSo = DSo;
                pp.DSa = DSa;
                pp.DSv = DSv;
                pp.DMo = DMo;
                pp.DMa = DMa;
                pp.DMv = DMv;
            });

        }

        public static void LoadPP()     // Oyuncular
        {
            //sw.WriteLine($"{r.PK},{r.RnkBaz},{r.Sex},{r.DgmYil},{r.Ad},{r.eMail},{r.Tel}");

            using (StreamReader reader = new StreamReader($@"C:\Starcounter\BodVedData\Ydk-PP.txt", System.Text.Encoding.UTF8))
            {
                string line;
                Db.Transact(() =>
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] ra = line.Split(',');

                        new BDB.PP()
                        {
                            PK = long.Parse(ra[0]),
                            RnkBaz = int.Parse(ra[1]),
                            Sex = ra[2],
                            DgmYil = int.Parse(ra[3]),
                            Ad = ra[4],
                            eMail = ra[5],
                            Tel = ra[6],
                        };
                    }
                });
            }
        }
        public static void LoadCC()     // Turnuvalar
        {
            //sw.WriteLine($"{cc.PK},{cc.Ad},{cc.Skl},{cc.Grp},{cc.Idx}");

            using (StreamReader sr = new StreamReader($@"C:\Starcounter\BodVedData\Ydk-CC.txt", System.Text.Encoding.UTF8))
            {
                string line;
                Db.Transact(() =>
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] ra = line.Split(',');

                        new BDB.CC()
                        {
                            PK = long.Parse(ra[0]),
                            Ad = ra[1],
                            Skl = ra[2],
                            Grp = ra[3],
                            Idx = ra[4],
                        };
                    }
                });
            }
        }
        public static void RestoreCC(string ccID)   // Delete&Restore Turnuva alti
        {
            // Eski kalmamali (Mukerrer olur)
            // PP disindaki her tablo etkilenir
            var cc = Db.SQL<CC>("select r from CC r where r.ID = ?", ccID).FirstOrDefault();

            if (cc != null)
            {
                DeleteCCrelated(cc.oNo);

                LoadCTofCC(ccID);
                LoadCTPofCC(ccID);
                LoadCETofCC(ccID);
                LoadCETPofCC(ccID);
                LoadCETRofCC(ccID);

                UpdPPLigMacSay();
                ReCreateRHofCC(cc.oNo);
                RefreshRH();
                RefreshRH2(cc.oNo);  // RnkGrp Rank
                UpdCETsumCC(cc.oNo);    // Musabaka
                UpdCTsumCC(cc.oNo);     // Lig Takim
                CompCTtRnkOfCC(cc.oNo); // Takim Rank Avarage
            }
        }
        public static void DeleteCCrelated(ulong CCoNo)    // Delete Turnuvanin alti
        {
            var cc = Db.FromId<CC>(CCoNo);

            var cets = Db.SQL<CET>("select r from CET r where r.CC = ?", cc);
            Db.Transact(() =>
            {
                foreach (var cet in cets)
                {
                    DeleteCETrelated(cet.oNo);
                }
            });

            var ctps = Db.SQL<CTP>("select r from CTP r where r.CC = ?", cc);
            Db.Transact(() =>
            {
                foreach (var ctp in ctps)
                {
                    ctp.Delete();
                }
            });

            var cts = Db.SQL<CT>("select r from CT r where r.CC = ?", cc);
            Db.Transact(() =>
            {
                foreach (var ct in cts)
                {
                    ct.Delete();
                }
            });

        }

        public static void LoadCTofCC(string ccID)                  // Turnuva Takimlari
        {
            var cc = Db.SQL<CC>("select r from CC r where r.ID = ?", ccID).FirstOrDefault();

            //sw.WriteLine($"{r.CC.ID},{r.ID},{r.Ad},{r.Adres},{r.Pw},{r.K1?.ID},{r.K2?.ID},{r.K1?.Ad,25},{r.K2?.Ad,25}");
            using (StreamReader reader = new StreamReader($@"C:\Starcounter\BodVedData\Ydk-CT-{ccID}.txt", System.Text.Encoding.UTF8))
            {
                string line;

                Db.Transact(() =>
                {
                    while ((line = reader.ReadLine()) != null)
                    {

                        string[] ra = line.Split(',');

                        var ppK1 = Db.SQL<PP>("select r from PP r where r.ID = ?", ra[5]).FirstOrDefault();
                        var ppK2 = Db.SQL<PP>("select r from PP r where r.ID = ?", ra[6]).FirstOrDefault();

                        new BDB.CT()
                        {
                            CC = cc,
                            ID = ra[1],
                            Ad = ra[2],
                            Adres = ra[3],
                            Pw = ra[4],
                            K1 = ppK1,
                            K2 = ppK2,
                        };
                    }
                });
            }
        }
        public static void LoadCTPofCC(string ccID)                 // Turnuva Takim Oyunculari
        {
            using (StreamReader reader = new StreamReader($@"C:\Starcounter\BodVedData\Ydk-CTP-{ccID}.txt"))
            {
                string line;
                var cc = Db.SQL<CC>("select r from CC r where r.ID = ?", ccID).FirstOrDefault();
                int i = 1;
                string pctID = "";

                Db.Transact(() =>
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] ra = line.Split(',');
                        if (pctID != ra[1])
                        {
                            i = 1;
                            pctID = ra[1];
                        }

                        var ct = Db.SQL<CT>("select r from CT r where r.CC = ? and r.ID = ?", cc, ra[1]).FirstOrDefault();
                        var pp = Db.SQL<PP>("select r from PP r where r.ID = ?", ra[2]).FirstOrDefault();

                        new BDB.CTP()
                        {
                            CC = cc,
                            CT = ct,
                            Idx = i++,
                            PP = pp,
                            PPid = ra[2],
                        };
                    }
                });
            }
        }
        public static void LoadCETofCC(string ccID)                 // Turnuva Musabakalari
        {
            string line;
            var cc = Db.SQL<CC>("select r from CC r where r.ID = ?", ccID).FirstOrDefault();
            RH rh;
            Db.Transact(() =>
            {
                var recs = Db.SQL<CETR>("select r from CETR r where r.CC = ?", cc);
                foreach (var cetr in recs)
                {
                    if (cetr.RH != null)
                    {
                        rh = Db.FromId<RH>(cetr.RH.GetObjectNo());
                        rh.Delete();
                    }
                    cetr.Delete();
                }

                Db.SQL("DELETE FROM BDB.CET where CC = ?", cc);
            });

            //sw.WriteLine($"{r.CC.ID},{r.ID},{r.Trh:dd.MM.yyyy HH:mm},{r.hCT.ID},{r.gCT.ID},{r.hPok},{r.gPok},{r.Rok},{r.hP},{r.gP},{r.hPW},{r.hMSW},{r.hMDW},{r.gPW},{r.gMSW},{r.gMDW}");
            using (StreamReader reader = new StreamReader($@"C:\Starcounter\BodVedData\Ydk-CET-{ccID}.txt"))
            {
                Db.Transact(() =>
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] ra = line.Split(',');

                        var hct = Db.SQL<CT>("select r from CT r where r.CC = ? and r.ID = ?", cc, ra[3]).FirstOrDefault();
                        var gct = Db.SQL<CT>("select r from CT r where r.CC = ? and r.ID = ?", cc, ra[4]).FirstOrDefault();

                        new BDB.CET()
                        {
                            CC = cc,
                            ID = ra[1],
                            Trh = DateTime.Parse(ra[2]),
                            hCT = hct,
                            gCT = gct,

                            hPok = bool.Parse(ra[5]),
                            gPok = bool.Parse(ra[6]),
                            Rok = bool.Parse(ra[7]),

                            hP = int.Parse(ra[8]),
                            gP = int.Parse(ra[9]),

                            hPW = int.Parse(ra[10]),
                            hMSW = int.Parse(ra[11]),
                            hMDW = int.Parse(ra[12]),
                            gPW = int.Parse(ra[13]),
                            gMSW = int.Parse(ra[14]),
                            gMDW = int.Parse(ra[15]),
                        };
                    }
                });
            }
        }
        public static void LoadCETRofCC(string ccID)                // Turnuva Sonuclari
        {
            var cc = Db.SQL<CC>("select r from CC r where r.ID = ?", ccID).FirstOrDefault();
            var cets = Db.SQL<CET>("select r from CET r where r.CC = ? order by r.ID", cc);
            foreach (var cet in cets)
            {
                LoadCETRofCET(ccID, cet.ID);
            }
        }
        public static void LoadCETRofCET(string ccID, string cetID) // Musabaka Sonuclari
        {
            string line;
            var cc = Db.SQL<CC>("select r from CC r where r.ID = ?", ccID).FirstOrDefault();
            var cet = Db.SQL<CET>("select r from CET r where r.CC = ? and r.ID = ?", cc, cetID).FirstOrDefault();

            Db.Transact(() =>
            {
                //Db.SQL("DELETE FROM PRH"); // Deneme
                var cetrs = Db.SQL<CETR>("select r from CETR r where r.CET = ?", cet);
                foreach (var cetr in cetrs)
                {
                    if (cetr.RH != null)
                    {
                        Db.FromId<RH>(cetr.RH.GetObjectNo()).Delete();
                    }
                    cetr.Delete();
                }
            });


            //sw.WriteLine($"{r.CC.ID},{r.CET.ID},{r.CT.ID},{r.SoD},{r.Idx},{r.HoG},{r.PP.ID},{r.S1W:D2},{r.S2W:D2},{r.S3W:D2},{r.S4W:D2},{r.S5W:D2},{r.SW},{r.SL},{r.MW},{r.ML},{r.PPAd}");
            //                      0          1         2       3       4       5         6       7          8          9         10         11        12     13     14     15       16      
            if (File.Exists($@"C:\Starcounter\BodVedData\Ydk-CETR-{ccID}-{cetID}.txt"))
            {
                using (StreamReader sr = new StreamReader($@"C:\Starcounter\BodVedData\Ydk-CETR-{ccID}-{cetID}.txt"))
                {
                    Db.Transact(() =>
                    {
                        while ((line = sr.ReadLine()) != null)
                        {
                            string[] ra = line.Split(',');

                            var ct = Db.SQL<CT>("select r from CT r where r.CC = ? and r.ID = ?", cc, ra[2]).FirstOrDefault();
                            var pp = Db.SQL<PP>("select r from PP r where r.ID = ?", ra[6]).FirstOrDefault();

                            var cetr = new CETR()
                            {
                                CC = cc,
                                CET = cet,
                                CT = ct,
                                Trh = cet.Trh,
                                SoD = ra[3],
                                Idx = int.Parse(ra[4]),
                                HoG = ra[5],
                                PP = pp,
                                S1W = int.Parse(ra[7]),
                                S2W = int.Parse(ra[8]),
                                S3W = int.Parse(ra[9]),
                                S4W = int.Parse(ra[10]),
                                S5W = int.Parse(ra[11]),
                                SW = int.Parse(ra[12]),
                                SL = int.Parse(ra[13]),
                                MW = int.Parse(ra[14]),
                                ML = int.Parse(ra[15]),
                            };
                        }
                    });
                }
            }

            // Rank RH kayitlarini yarat, Sadece Singles
            // Sonrasinda RefreshRH gerekir!!!
        }
        public static void LoadCETPofCC(string ccID)                // Turnuva OyuncuSiralama
        {
            var cc = Db.SQL<CC>("select r from CC r where r.ID = ?", ccID).FirstOrDefault();
            var cets = Db.SQL<CET>("select r from CET r where r.CC = ? order by r.ID", cc);
            foreach (var cet in cets)
            {
                LoadCETPofCET(ccID, cet.ID);
            }
        }
        public static void LoadCETPofCET(string ccID, string cetID) // Musabaka OyuncuSiralama
        {
            string line;
            var cc = Db.SQL<CC>("select r from CC r where r.ID = ?", ccID).FirstOrDefault();
            var cet = Db.SQL<CET>("select r from CET r where r.CC = ? and r.ID = ?", cc, cetID).FirstOrDefault();

            Db.Transact(() =>
            {
                var cetps = Db.SQL<CETP>("select r from CETP r where r.CET = ?", cet);
                foreach (var cetp in cetps)
                {
                    cetp.Delete();
                }
            });

            //sw.WriteLine($"{r.CC.ID},{r.CET.ID},{r.CT.ID},{r.SoD},{r.Idx},{r.HoG},{r.PP.ID},{r.PPAd,25}");
            //                      0          1         2       3       4       5         6         
            if (File.Exists($@"C:\Starcounter\BodVedData\Ydk-CETP-{ccID}-{cetID}.txt"))
            {
                using (StreamReader sr = new StreamReader($@"C:\Starcounter\BodVedData\Ydk-CETP-{ccID}-{cetID}.txt"))
                {
                    Db.Transact(() =>
                    {
                        while ((line = sr.ReadLine()) != null)
                        {
                            string[] ra = line.Split(',');

                            var ct = Db.SQL<CT>("select r from CT r where r.CC = ? and r.ID = ?", cc, ra[2]).FirstOrDefault();
                            var pp = Db.SQL<PP>("select r from PP r where r.ID = ?", ra[6]).FirstOrDefault();

                            var cetr = new CETP()
                            {
                                CC = cc,
                                CET = cet,
                                CT = ct,
                                SoD = ra[3],
                                Idx = int.Parse(ra[4]),
                                HoG = ra[5],
                                PP = pp,
                            };
                        }
                    });
                }
            }
        }
        */

        #region Restore

        static char __ = '║';

        public static void Restore()
        {
            Restore_ID();
            RestoreSTAT();
            RestorePP();
            RestoreCC();
            RestoreCT();
            RestoreCTP();
            RestoreCET();
            RestoreCETP();
            RestoreCETR();

            IndexCreate();
            
            UpdPPLigMacSay();
            CreateRH();
            RefreshRH();
            foreach (var cc in Db.SQL<CC>("select c from CC c"))
            {
                RefreshRH2(cc.oNo);     // RnkGrp Rank
                UpdCETsumCC(cc.oNo);    // Musabaka
                UpdCTsumCC(cc.oNo);     // Lig Takim
                CompCTtRnkOfCC(cc.oNo); // Takim Rank Avarage
            }
        }

        public static void Restore_ID()   // PKs
        {
            //sw.WriteLine($"{r.ID}");

            using (StreamReader sr = new StreamReader($@"C:\Starcounter\BodVedData\BDB-ID.txt", System.Text.Encoding.UTF8))
            {
                string line;
                Db.Transact(() =>
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] ra = line.Split(',');

                        new BDB._ID()
                        {
                            ID = ulong.Parse(ra[0]),
                        };
                    }
                });
            }
        }
        public static void RestoreSTAT()  // STAT
        {
            //sw.WriteLine($"{r.ID},{r.IdVal}");

            using (StreamReader sr = new StreamReader($@"C:\Starcounter\BodVedData\BDB-STAT.txt", System.Text.Encoding.UTF8))
            {
                string line;
                Db.Transact(() =>
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] ra = line.Split(',');

                        new BDB.STAT()
                        {
                            ID = int.Parse(ra[0]),
                            IdVal = int.Parse(ra[1]),
                        };
                    }
                });
            }
        }
        public static void RestorePP()    // Oyuncular
        {
            //sw.WriteLine($"{r.PK},{r.RnkBaz},{r.Sex},{r.DgmYil},{r.Ad},{r.eMail},{r.Tel}");

            using (StreamReader reader = new StreamReader($@"C:\Starcounter\BodVedData\BDB-PP.txt", System.Text.Encoding.UTF8))
            {
                string line;
                Db.Transact(() =>
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] ra = line.Split(',');

                        new BDB.PP()
                        {
                            PK = long.Parse(ra[0]),
                            RnkBaz = int.Parse(ra[1]),
                            Sex = ra[2],
                            DgmYil = int.Parse(ra[3]),
                            Ad = ra[4],
                            eMail = ra[5],
                            Tel = ra[6],
                        };
                    }
                });
            }
        }
        public static void RestoreCC()    // Turnuvalar
        {
            //sw.WriteLine($"{r.PK},{r.ID},{r.Ad},{r.Skl},{r.Grp},{r.Idx},{r.Lig},{r.RnkID},{r.RnkAd}");

            using (StreamReader sr = new StreamReader($@"C:\Starcounter\BodVedData\BDB-CC.txt", System.Text.Encoding.UTF8))
            {
                string line;
                Db.Transact(() =>
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] ra = line.Split(',');

                        new BDB.CC()
                        {
                            PK = long.Parse(ra[0]),
                            ID = ra[1],
                            Ad = ra[2],
                            Skl = ra[3],
                            Grp = ra[4],
                            Idx = ra[5],
                            Lig = ra[6],
                            RnkID = int.Parse(ra[7]),
                            RnkAd = ra[8],
                        };
                    }
                });
            }
        }
        public static void RestoreCT()    // TurnuvaTakimlari
        {
            //sw.WriteLine($"{r.CC.PK}║{r.PK}║{r.Ad}║{r.Adres}║{r.Pw}║{r.K1?.PK}║{r.K2?.PK}║{r.K1?.Ad}║{r.K2?.Ad}");
            using (StreamReader reader = new StreamReader($@"C:\Starcounter\BodVedData\BDB-CT.txt", System.Text.Encoding.UTF8))
            {
                string line;
                long ccPK, rPK, ppK1PK, ppK2PK;

                Db.Transact(() =>
                {
                    while ((line = reader.ReadLine()) != null)
                    {

                        string[] ra = line.Split(__);

                        ccPK = long.Parse(ra[0]);
                        rPK = long.Parse(ra[1]);
                        ppK1PK = string.IsNullOrEmpty(ra[5]) ? 0 : long.Parse(ra[5]);
                        ppK2PK = string.IsNullOrEmpty(ra[6]) ? 0 : long.Parse(ra[6]);

                        var cc = Db.SQL<CC>("select r from CC r where r.PK = ?", ccPK).FirstOrDefault();
                        var ppK1 = Db.SQL<PP>("select r from PP r where r.PK = ?", ppK1PK).FirstOrDefault();
                        var ppK2 = Db.SQL<PP>("select r from PP r where r.PK = ?", ppK2PK).FirstOrDefault();

                        new BDB.CT()
                        {
                            CC = cc,
                            PK = rPK,
                            Ad = ra[2],
                            Adres = ra[3],
                            Pw = ra[4],
                            K1 = ppK1,
                            K2 = ppK2,
                        };
                    }
                });
            }
        }
        public static void RestoreCTP()   // TurnuvaTakimOyunculari
        {
            //sw.WriteLine($"{r.CC.PK},{r.CT.PK},{r.PP.PK},{r.Idx}{r.PPAd,25},{r.CTAd,20}");
            using (StreamReader reader = new StreamReader($@"C:\Starcounter\BodVedData\BDB-CTP.txt"))
            {
                string line;
                long ccPK, ctPK, ppPK;

                Db.Transact(() =>
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] ra = line.Split(',');

                        ccPK = long.Parse(ra[0]);
                        ctPK = long.Parse(ra[1]);
                        ppPK = long.Parse(ra[2]);

                        var cc = Db.SQL<CC>("select r from CC r where r.PK = ?", ccPK).FirstOrDefault();
                        var ct = Db.SQL<CT>("select r from CT r where r.PK = ?", ctPK).FirstOrDefault();
                        var pp = Db.SQL<PP>("select r from PP r where r.PK = ?", ppPK).FirstOrDefault();

                        new BDB.CTP()
                        {
                            CC = cc,
                            CT = ct,
                            PP = pp,
                            Idx = int.Parse(ra[3])
                        };
                    }
                });
            }
        }
        public static void RestoreCET()   // TurnuvaFikstur/Event
        {
            //sw.WriteLine($"{r.CC.PK},{r.PK},{r.hCT.PK},{r.gCT.PK},{r.Trh:dd.MM.yyyy HH:mm},{r.hPok},{r.gPok},{r.Rok},{r.hP},{r.gP},{r.hPW},{r.hMSW},{r.hMDW},{r.gPW},{r.gMSW},{r.gMDW}");
            using (StreamReader reader = new StreamReader($@"C:\Starcounter\BodVedData\BDB-CET.txt"))
            {
                string line;
                long ccPK, rPK, hctPK, gctPK;

                Db.Transact(() =>
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] ra = line.Split(',');

                        ccPK = long.Parse(ra[0]);
                        rPK = long.Parse(ra[1]);
                        hctPK = long.Parse(ra[2]);
                        gctPK = long.Parse(ra[3]);

                        var cc = Db.SQL<CC>("select r from CC r where r.PK = ?", ccPK).FirstOrDefault();
                        var hct = Db.SQL<CT>("select r from CT r where r.PK = ?", hctPK).FirstOrDefault();
                        var gct = Db.SQL<CT>("select r from CT r where r.PK = ?", gctPK).FirstOrDefault();

                        new BDB.CET()
                        {
                            CC = cc,
                            PK = rPK,
                            Trh = DateTime.Parse(ra[4]),
                            hCT = hct,
                            gCT = gct,

                            hPok = bool.Parse(ra[5]),
                            gPok = bool.Parse(ra[6]),
                            Rok = bool.Parse(ra[7]),

                            hP = int.Parse(ra[8]),
                            gP = int.Parse(ra[9]),

                            hPW = int.Parse(ra[10]),
                            hMSW = int.Parse(ra[11]),
                            hMDW = int.Parse(ra[12]),
                            gPW = int.Parse(ra[13]),
                            gMSW = int.Parse(ra[14]),
                            gMDW = int.Parse(ra[15]),
                        };
                    }
                });
            }
        }
        public static void RestoreCETP()  // MusabakaOyuncuSiralama
        {
            //sw.WriteLine($"{r.CC.PK},{r.CET.PK},{r.CT.PK},{r.PP.PK},{r.SoD},{r.Idx:D2},{r.HoG},{r.PPAd,25}");
            using (StreamReader sr = new StreamReader($@"C:\Starcounter\BodVedData\BDB-CETP.txt"))
            {
                string line;
                long ccPK, cetPK, ctPK, ppPK;

                Db.Transact(() =>
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] ra = line.Split(',');
                        ccPK = long.Parse(ra[0]);
                        cetPK = long.Parse(ra[1]);
                        ctPK = long.Parse(ra[2]);
                        ppPK = long.Parse(ra[3]);

                        var cc = Db.SQL<CC>("select r from CC r where r.PK = ?", ccPK).FirstOrDefault();
                        var cet = Db.SQL<CET>("select r from CET r where r.PK = ?", cetPK).FirstOrDefault();
                        var ct = Db.SQL<CT>("select r from CT r where r.PK = ?", ctPK).FirstOrDefault();
                        var pp = Db.SQL<PP>("select r from PP r where r.PK = ?", ppPK).FirstOrDefault();

                        var cetr = new CETP()
                        {
                            CC = cc,
                            CET = cet,
                            CT = ct,
                            SoD = ra[4],
                            Idx = int.Parse(ra[5]),
                            HoG = ra[6],
                            PP = pp,
                        };
                    }
                });
            }
        }
        public static void RestoreCETR()  // MusabakaSonuclari
        {
            //sw.WriteLine($"{r.CC.PK},{r.CET.PK},{r.CT.PK},{r.PP.PK},{r.SoD},{r.Idx:D2},{r.HoG},{r.S1W:D2},{r.S2W:D2},{r.S3W:D2},{r.S4W:D2},{r.S5W:D2},{r.SW},{r.SL},{r.MW},{r.ML},{r.PPAd,25}");
            using (StreamReader sr = new StreamReader($@"C:\Starcounter\BodVedData\BDB-CETR.txt"))
            {
                string line;
                long ccPK, cetPK, ctPK, ppPK;

                Db.Transact(() =>
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] ra = line.Split(',');
                        ccPK = long.Parse(ra[0]);
                        cetPK = long.Parse(ra[1]);
                        ctPK = long.Parse(ra[2]);
                        ppPK = long.Parse(ra[3]);

                        var cc = Db.SQL<CC>("select r from CC r where r.PK = ?", ccPK).FirstOrDefault();
                        var cet = Db.SQL<CET>("select r from CET r where r.PK = ?", cetPK).FirstOrDefault();
                        var ct = Db.SQL<CT>("select r from CT r where r.PK = ?", ctPK).FirstOrDefault();
                        var pp = Db.SQL<PP>("select r from PP r where r.PK = ?", ppPK).FirstOrDefault();

                        var cetr = new CETR()
                        {
                            CC = cc,
                            CET = cet,
                            CT = ct,
                            PP = pp,
                            Trh = cet.Trh,
                            SoD = ra[4],
                            Idx = int.Parse(ra[5]),
                            HoG = ra[6],
                            S1W = int.Parse(ra[7]),
                            S2W = int.Parse(ra[8]),
                            S3W = int.Parse(ra[9]),
                            S4W = int.Parse(ra[10]),
                            S5W = int.Parse(ra[11]),
                            SW = int.Parse(ra[12]),
                            SL = int.Parse(ra[13]),
                            MW = int.Parse(ra[14]),
                            ML = int.Parse(ra[15]),
                        };
                    }
                });
            }
        }

        public static void CreateRH()
        {
            // Rank RH kayitlarini yarat, Sadece Singles
            // Sonrasinda RefreshRH gerekir!!!

            Db.Transact(() =>
            {
                CETR h = null, g = null;
                int Won = 0;
                RH rh;
                // Sadece Singles
                var cetrs = Db.SQL<CETR>("select r from CETR r where r.SoD = ? order by r.CET, r.Idx, r.HoG desc", "S");

                foreach (var r in cetrs)
                {
                    // Ilk H sonra G gelir
                    if (r.HoG == "H")
                    {
                        h = r;
                        if (r.MW > r.ML)
                            Won = 1;
                        else if (r.MW < r.ML)
                            Won = -1;
                    }
                    else if (r.HoG == "G")
                    {
                        g = r;

                        rh = new RH
                        {
                            CC = r.CC,
                            Trh = r.Trh,

                            hPP = h.PP,
                            hWon = Won,
                            gPP = g.PP,
                            gWon = -Won,
                        };
                        h.RH = rh;
                        g.RH = rh;
                    }
                }
            });
            // RH daki herhangi bir degisiklik hepsini etkiler!
            //refreshRH();
        }
        public static void IndexCreate()
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

            if (Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "IdxMAC_Trh").FirstOrDefault() == null)
                Db.SQL("CREATE INDEX IdxMAC_Trh     ON BDB.MAC (Trh)");
            //if (Db.SQL("SELECT i FROM Starcounter.Metadata.\"Index\" i WHERE Name = ?", "IdxMAC_SoD").FirstOrDefault() == null)
            //    Db.SQL("CREATE INDEX IdxMAC_SoD    ON BDB.MAC (SoD)");
        }
        public static void IndexDrop()
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

            Db.SQL("DROP INDEX IdxMAC_Trh ON BDB.MAC");
            Db.SQL("DROP INDEX IdxMAC_SoD ON BDB.MAC");
        }


        #endregion Restore

        #region Backup

        public static void Backup()
        {
            Backup_ID();
            BackupSTAT();
            BackupPP();
            BackupCC();
            BackupCT();
            BackupCTP();
            BackupCET();
            BackupCETP();
            BackupCETR();
        }

        public static void Backup_ID()   // PKs
        {
            using (StreamWriter sw = new StreamWriter($@"C:\Starcounter\BodVedData\BDB-ID.txt", false))
            {
                var recs = Db.SQL<_ID>("select r from _ID r");
                foreach (var r in recs)
                {
                    sw.WriteLine($"{r.ID}");
                }
            }
        }
        public static void BackupSTAT()  // STAT
        {
            using (StreamWriter sw = new StreamWriter($@"C:\Starcounter\BodVedData\BDB-STAT.txt", false))
            {
                var recs = Db.SQL<STAT>("select r from STAT r");
                foreach (var r in recs)
                {
                    sw.WriteLine($"{r.ID},{r.IdVal}");
                }
            }
        }
        public static void BackupPP()    // Oyuncular
        {
            using (StreamWriter sw = new StreamWriter($@"C:\Starcounter\BodVedData\BDB-PP.txt", false))
            {
                var recs = Db.SQL<PP>("select r from PP r order by r.Ad");
                foreach (var r in recs)
                {
                    sw.WriteLine($"{r.PK},{r.RnkBaz},{r.Sex},{r.DgmYil},{r.Ad},{r.eMail},{r.Tel}");
                }
            }
        }
        public static void BackupCC()    // Turnuvalar
        {
            using (StreamWriter sw = new StreamWriter($@"C:\Starcounter\BodVedData\BDB-CC.txt", false))
            {
                var recs = Db.SQL<CC>("select r from CC r order by r.PK");
                foreach (var r in recs)
                    sw.WriteLine($"{r.PK},{r.ID},{r.Ad},{r.Skl},{r.Grp},{r.Idx},{r.Lig},{r.RnkID},{r.RnkAd}");
            }
        }
        public static void BackupCT()    // TurnuvaTakimlari
        {
            using (StreamWriter sw = new StreamWriter($@"C:\Starcounter\BodVedData\BDB-CT.txt", false))
            {
                var recs = Db.SQL<CT>("select r from CT r order by r.CC");
                foreach (var r in recs)
                {
                    sw.WriteLine($"{r.CC.PK}{__}{r.PK}{__}{r.Ad}{__}{r.Adres}{__}{r.Pw}{__}{r.K1?.PK}{__}{r.K2?.PK}{__}{r.K1?.Ad}{__}{r.K2?.Ad}");
                }
            }
        }
        public static void BackupCTP()   // TurnuvaTakimOyunculari
        {
            using (StreamWriter sw = new StreamWriter($@"C:\Starcounter\BodVedData\BDB-CTP.txt", false))
            {
                var recs = Db.SQL<CTP>("select r from CTP r order by r.CC, r.CT");
                foreach (var r in recs)
                {
                    sw.WriteLine($"{r.CC.PK},{r.CT.PK},{r.PP.PK},{r.Idx},{r.PPAd,25},{r.CTAd,20}");
                }
            }
        }
        public static void BackupCET()   // TurnuvaFikstur/Event
        {
            using (StreamWriter sw = new StreamWriter($@"C:\Starcounter\BodVedData\BDB-CET.txt", false))
            {
                var recs = Db.SQL<CET>("select r from CET r order by r.CC, r.Trh");
                foreach (var r in recs)
                {
                    sw.WriteLine($"{r.CC.PK},{r.PK},{r.hCT.PK},{r.gCT.PK},{r.Trh:dd.MM.yyyy HH:mm},{r.hPok},{r.gPok},{r.Rok},{r.hP},{r.gP},{r.hPW},{r.hMSW},{r.hMDW},{r.gPW},{r.gMSW},{r.gMDW},{r.hCTAd,25},{r.gCTAd,25}");
                }
            }
        }
        public static void BackupCETP()  // MusabakaOyuncuSiralama
        {
            using (StreamWriter sw = new StreamWriter($@"C:\Starcounter\BodVedData\BDB-CETP.txt", false))
            {
                var recs = Db.SQL<CETP>("select r from CETP r order by r.CC, r.CET, r.HoG desc, r.SoD desc, r.Idx");
                foreach (var r in recs)
                {
                    if (r.PP != null)
                        sw.WriteLine($"{r.CC.PK},{r.CET.PK},{r.CT.PK},{r.PP.PK},{r.SoD},{r.Idx:D2},{r.HoG},{r.PPAd,25}");
                }
            }
        }
        public static void BackupCETR()  // Musabaka Sonuclari
        {
            using (StreamWriter sw = new StreamWriter($@"C:\Starcounter\BodVedData\BDB-CETR.txt", false))
            {
                var recs = Db.SQL<CETR>("select r from CETR r order by r.CC, r.CET, r.SoD desc, r.Idx, r.HoG desc");
                foreach (var r in recs)
                {
                    sw.WriteLine($"{r.CC.PK},{r.CET.PK},{r.CT.PK},{r.PP.PK},{r.SoD},{r.Idx:D2},{r.HoG},{r.S1W:D2},{r.S2W:D2},{r.S3W:D2},{r.S4W:D2},{r.S5W:D2},{r.SW},{r.SL},{r.MW},{r.ML},{r.PPAd,25}");
                }
            }
        }

        #endregion Backup

        /*
        public static void BackupDB()
        {
            SavePP();
            SaveCC();
            foreach (var cc in Db.SQL<CC>("select r from CC r"))
            {
                BackupCC(cc.oNo);
            }
        }

        public static void SavePP()                     // Oyuncular
        {
            using (StreamWriter sw = new StreamWriter($@"C:\Starcounter\BodVedData\Ydk-PP.txt", false))
            {
                var recs = Db.SQL<PP>("select r from PP r order by r.Ad");
                foreach (var r in recs)
                {
                    sw.WriteLine($"{r.PK},{r.RnkBaz},{r.Sex},{r.DgmYil},{r.Ad},{r.eMail},{r.Tel}");
                }
            }
        }
        public static void SaveCC()                     // Turnuvalar
        {
            // JSON deneme 
            // Read From DB
            //var rec = new RowCC();
            //rec.CCs.Data = Db.SQL<CC>("select r from CC r order by r.ID");
            //var aaa = rec.ToJson();
            //using (StreamWriter sw = new StreamWriter($@"C:\Starcounter\BodVedData\Ydk-CC2.txt", false))
            //{
            //    sw.WriteLine(aaa);
            //}
            //// Write To DB
            //dynamic json = new Json(aaa);
            //string ad = json.CCs[0].Ad;
            //int iz = json.CCs.Count;
            //for (int i = 0; i < iz; i++)
            //{
            //
            //}
            var ccs = Db.SQL<CC>("select r from CC r order by r.ID");
            using (StreamWriter sw = new StreamWriter($@"C:\Starcounter\BodVedData\Ydk-CC.txt", false))
            {
                foreach (var cc in ccs)
                sw.WriteLine($"{cc.PK},{cc.Ad},{cc.Skl},{cc.Grp},{cc.Idx}");
            }
        }

        public static void BackupCC(ulong CCoNo)        // Turnuva Ilgili Kayitlari
        {
            var cc = Db.SQL<CC>("select r from CC r where r.ObjectNo = ?", CCoNo).FirstOrDefault();

            if (cc != null)
            {
                SaveCTofCC(CCoNo);
                SaveCTPofCC(CCoNo);
                SaveCETofCC(CCoNo);
                SaveCETPofCC(CCoNo);
                SaveCETRofCC(CCoNo);
            }
        }
        public static void SaveCTofCC(ulong CCoNo)      // Turnuva Takimlari
        {
            var cc = Db.FromId<CC>(CCoNo);

            using (StreamWriter sw = new StreamWriter($@"C:\Starcounter\BodVedData\Ydk-CT-{cc.PK}.txt", false))
            {
                var recs = Db.SQL<CT>("select r from CT r where r.CC = ? order by r.Ad", cc);
                foreach (var r in recs)
                {
                    sw.WriteLine($"{r.CC.PK},{r.PK},{r.Ad},{r.Adres},{r.Pw},{r.K1?.PK},{r.K2?.PK},{r.K1?.Ad},{r.K2?.Ad}");
                }
            }
        }
        public static void SaveCTPofCC(ulong CCoNo)     // Turnuva Takim Oyunculari
        {
            var cc = Db.FromId<CC>(CCoNo);

            using (StreamWriter sw = new StreamWriter($@"C:\Starcounter\BodVedData\Ydk-CTP-{cc.PK}.txt", false))
            {
                var recs = Db.SQL<CTP>("select r from CTP r where r.CC = ? order by r.CT, r.Idx", cc);
                foreach (var r in recs)
                {
                    sw.WriteLine($"{r.CC.PK},{r.CT.PK},{r.PP.PK},{r.PPAd,25},{r.CTAd,20}");
                }
            }
        }
        public static void SaveCETofCC(ulong CCoNo)     // Turnuva Fikstur
        {
            var cc = Db.FromId<CC>(CCoNo);

            using (StreamWriter sw = new StreamWriter($@"C:\Starcounter\BodVedData\Ydk-CET-{cc.PK}.txt", false))
            {
                var recs = Db.SQL<CET>("select r from CET r where r.CC = ? order by r.ID", cc);
                foreach (var r in recs)
                {
                    sw.WriteLine($"{r.CC.PK},{r.PK},{r.Trh:dd.MM.yyyy HH:mm},{r.hCT.PK},{r.gCT.PK},{r.hPok},{r.gPok},{r.Rok},{r.hP},{r.gP},{r.hPW},{r.hMSW},{r.hMDW},{r.gPW},{r.gMSW},{r.gMDW}");
                }
            }
        }   
        public static void SaveCETRofCC(ulong CCoNo)    // Turnuva Tum Sonuclari
        {
            var cc = Db.FromId<CC>(CCoNo);
            var recs = Db.SQL<CET>("select r from CET r where r.CC = ? order by r.Trh", cc);
            foreach (var r in recs)
                SaveCETRofCET(r.oNo);
        }
        public static void SaveCETRofCET(ulong CEToNo)  // Musabaka Sonuclari
        {
            var cet = Db.FromId<CET>(CEToNo);

            var recs = Db.SQL<CETR>("select r from CETR r where r.CET = ? order by r.SoD desc, r.Idx, r.HoG desc", cet);
            if (recs.FirstOrDefault() == null)
                return;

            using (StreamWriter sw = new StreamWriter($@"C:\Starcounter\BodVedData\Ydk-CETR-{cet.CC.PK}-{cet.PK}.txt", false))
            {
                foreach (var r in recs)
                {
                    sw.WriteLine($"{r.CC.PK},{r.CET.PK},{r.CT.PK},{r.SoD},{r.Idx:D2},{r.HoG},{r.PP.ID},{r.S1W:D2},{r.S2W:D2},{r.S3W:D2},{r.S4W:D2},{r.S5W:D2},{r.SW},{r.SL},{r.MW},{r.ML},{r.PPAd,25}");
                }
            }
        }
        public static void SaveCETPofCC(ulong CCoNo)    // Turnuva OyuncuSiralama
        {
            var cc = Db.FromId<CC>(CCoNo);
            var recs = Db.SQL<CET>("select r from CET r where r.CC = ? order by r.Trh", cc);
            foreach (var r in recs)
                SaveCETPofCET(r.oNo);
        }
        public static void SaveCETPofCET(ulong CEToNo)  // Musabaka OyuncuSiralama
        {
            var cet = Db.FromId<CET>(CEToNo);

            var recs = Db.SQL<CETP>("select r from CETP r where r.CET = ? and r.PP is not null order by r.SoD desc, r.Idx, r.HoG desc", cet);
            if (recs.FirstOrDefault() == null)
                return;

            using (StreamWriter sw = new StreamWriter($@"C:\Starcounter\BodVedData\Ydk-CETP-{cet.CC.PK}-{cet.PK}.txt", false))
            {
                foreach (var r in recs)
                {
                    sw.WriteLine($"{r.CC.PK},{r.CET.PK},{r.CT.PK},{r.SoD},{r.Idx:D2},{r.HoG},{r.PP.PK},{r.PPAd,25}");
                }
            }
        }
        public static void BackupCET(string ccID, string cetID)     // Musabaka Ilgili Kayitlari
        {
            var cc = Db.SQL<CC>("select r from CC r where r.ID = ?", ccID).FirstOrDefault();
            var cet = Db.SQL<CET>("select r from CET r where r.CC = ? and r.ID = ?", cc, cetID).FirstOrDefault();

            if (cet != null)
            {
                //SaveCETPofCET(cet.oNo);
                SaveCETRofCET(cet.oNo);
            }
        }
        */



        #region IPTAL
        /*
        public static void BackupPP()
        {
            using (StreamWriter sw = new StreamWriter($@"C:\Starcounter\BodVedData\Ydk-PP.txt", false))
            {
                var recs = Db.SQL<PP>("select r from PP r order by r.Ad");
                foreach (var r in recs)
                {
                    sw.WriteLine($"{r.ID},{r.RnkBaz},{r.Sex},{r.DgmYil},{r.Ad},{r.eMail},{r.Tel}");
                }
            }
        }

        public static void BackupCC()
        {
            BackupPP();

            var recs = Db.SQL<CC>("select r from CC r order by r.ID");
            foreach (var r in recs)
            {
                BackupCC(r.ID);
            }
        }

        public static void BackupCT(ulong CCoNo)
        {
            var cc = Db.FromId<CC>(CCoNo);

            using (StreamWriter sw = new StreamWriter($@"C:\Starcounter\BodVedData\Ydk-CT-{cc.ID}.txt", false))
            {
                var recs = Db.SQL<CT>("select r from CT r where r.CC = ? order by r.Ad", cc);
                foreach (var r in recs)
                {
                    sw.WriteLine($"{r.CC.ID},{r.ID},{r.Ad}");
                }
            }
        }

        public static void BackupCTP(ulong CCoNo)
        {
            var cc = Db.FromId<CC>(CCoNo);

            using (StreamWriter sw = new StreamWriter($@"C:\Starcounter\BodVedData\Ydk-CTP-{cc.ID}.txt", false))
            {
                var recs = Db.SQL<CTP>("select r from CTP r where r.CC = ? order by r.CT, r.Idx", cc);
                foreach (var r in recs)
                {
                    sw.WriteLine($"{r.CC.ID},{r.CT.ID},{r.PP.ID},{r.PPAd,25},{r.CTAd,20}");
                }
            }
        }

        public static void BackupCET(ulong CCoNo)
        {
            var cc = Db.FromId<CC>(CCoNo);

            using (StreamWriter sw = new StreamWriter($@"C:\Starcounter\BodVedData\Ydk-CET-{cc.ID}.txt", false))
            {
                var recs = Db.SQL<CET>("select r from CET r where r.CC = ? order by r.ID", cc);
                foreach (var r in recs)
                {
                    sw.WriteLine($"{r.CC.ID},{r.ID},{r.Trh:dd.MM.yyyy HH:mm},{r.hCT.ID},{r.gCT.ID},{r.hPok},{r.gPok},{r.Rok},{r.hP},{r.gP},{r.hPW},{r.hMSW},{r.hMDW},{r.gPW},{r.gMSW},{r.gMDW}");
                }
            }
        }

        public static void BackupCETR(ulong CCoNo)
        {
            var cc = Db.FromId<CC>(CCoNo);
            var recs = Db.SQL<CET>("select r from CET r where r.CC = ? order by r.ID", cc);
            foreach (var r in recs)
                BackupCETR(CCoNo, r.oNo);
        }

        public static void BackupCETR(ulong CCoNo, ulong CEToNo)
        {
            var cet = Db.FromId<CET>(CEToNo);

            var recs = Db.SQL<CETR>("select r from CETR r where r.CET = ? order by r.SoD desc, r.Idx, r.HoG desc", cet);
            if (recs.FirstOrDefault() == null)
                return;

            using (StreamWriter sw = new StreamWriter($@"C:\Starcounter\BodVedData\Ydk-CETR-{cet.CC.ID}-{cet.ID}.txt", false))
            {
                foreach (var r in recs)
                {
                    sw.WriteLine($"{r.CC.ID},{r.CET.ID},{r.CT.ID},{r.SoD},{r.Idx},{r.HoG},{r.PP.ID},{r.S1W:D2},{r.S2W:D2},{r.S3W:D2},{r.S4W:D2},{r.S5W:D2},{r.SW},{r.SL},{r.MW},{r.ML},{r.PPAd,25}");
                }
            }
        }
        */
        #endregion IPTAL



        public static void Write2Log(string Msg)
        {
            using (StreamWriter sw = new StreamWriter(@"C:\Starcounter\MyLog\BodVed-Log.txt", true))
            {
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ": " + Msg);
            }
            /*
            try
            {
                StreamWriter sw = new StreamWriter(@"C:\Starcounter\MyLog\BodVed-Log.txt", true);
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ": " + Msg);
                sw.Flush();
                sw.Close();
            }
            catch
            {
            }*/
        }
        public static void Write2UserLog(string Msg)
        {
            using (StreamWriter sw = new StreamWriter(@"C:\Starcounter\MyLog\BodVed-UserLog.txt", true))
            {
                sw.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {Msg}");
            }
        }
    }

    public class GrRnk
    {
        public ulong CCoNo;
        public ulong CToNo;
        public int RnkBaz;
        public int Rnk;
        public int RnkGrp;
    }
}
