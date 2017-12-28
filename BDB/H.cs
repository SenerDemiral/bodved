using System;
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
            string rS = "";
            if (hR > gR)
                rS = $"◄ {gR}";
            else if(hR < gR)
                rS = $"{hR} ►";
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
                        Link = $"/bodved/CETpage/{c.CCoNo}"
                        
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
                        IdVal = 4230
                    };
                    EntCnt = 4230;
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
            RefreshRH();
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

        // Kullanilmiyor
        public static void ReCalcPPsra()
        {
            // ReCalculate Sira of All Players
            var pps = Db.SQL<BDB.PP>("select p from PP p order by p.Rnk desc, p.RnkBaz desc, p.Ad");

            Db.Transact(() =>
            {
                int sira = 1;
                foreach (var r in pps)
                {
                    //r.Rnk = r.curRnk;
                    r.Sra = sira++;
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

                    //if (r.hWon == 0 || hPP.Ad.StartsWith("∞") || gPP.Ad.StartsWith("∞"))   // Oynanmamis veya Oyunculardan biri diskalifiye ise Rank hesaplama
                    if (r.hWon == 0 || r.hPP.RnkBaz == -1 || r.gPP.RnkBaz == -1)   // Oynanmamis veya Oyunculardan biri diskalifiye ise Rank hesaplama
                        NOPX = 0;
                    else
                        NOPX = compNOPX(r.hWon, hpRnk, gpRnk);

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
                var pp = Db.SQL<BDB.PP>("select p from PP p order by p.Rnk desc");
                int sira = 1;
                foreach (var r in pp)
                {
                    r.Sra = sira++;
                }
            });
            
            watch.Stop();
            Console.WriteLine($"refreshRH {nor}: {watch.ElapsedMilliseconds} msec  {watch.ElapsedTicks} ticks");
        }

        public static PPGR getPPGR(PP pp, string RnkGrp)
        {
            PPGR ppgr = Db.SQL<PPGR>("select p from PPGR p where p.PP = ? and p.RnkGrp = ?", pp, RnkGrp).FirstOrDefault();
            if (ppgr == null)
            {
                ppgr = new PPGR
                {
                    PP = pp,
                    RnkGrp = RnkGrp,
                    RnkBaz = ppgr.PP.RnkBaz,
                    Rnk = ppgr.PP.RnkBaz
                };
            }
            return ppgr;
        }
        public static void RefreshRH4(DateTime basTrh)
        {
            // Kucuk bir tarih verilerek basindan beri yaptirilabilir
            Stopwatch watch = new Stopwatch();
            watch.Start();
            int nor = 0;
            PP pp;

            DateTime d = new DateTime(2017, 12, 1);
            Dictionary<ulong, int> myDic = new Dictionary<ulong, int>();
            /*
            foreach (var p in Db.SQL<BDB.PP>("select p from PP p"))
            {
                myDic[p.oNo] = -1;
            }
            */
            // startDate'e kadarki PP lerin sonRank'ini kaydet.
            foreach (var r in Db.SQL<BDB.RH>("select p from RH p where p.Trh < ? order by p.Trh", basTrh))
            {
                myDic[r.hPP.oNo] = r.hpRnk + r.hNOPX;
                myDic[r.gPP.oNo] = r.gpRnk + r.gNOPX;
            }

            ulong rhPPoNo = 0, rgPPoNo = 0;
            int hpRnk = 0, gpRnk = 0;
            int NOPX = 0;
            Db.Transact(() =>
            {
                foreach (var r in Db.SQL<BDB.RH>("select p from RH p where p.Trh >= ? order by p.Trh", basTrh))
                {
                    nor++;
                    rhPPoNo = r.hPP.GetObjectNo();
                    rgPPoNo = r.gPP.GetObjectNo();

                    //if (myDic[rhPPoNo] == -1)
                    if (!myDic.ContainsKey(rhPPoNo))
                        myDic[rhPPoNo] = r.hPP.RnkBaz;
                    hpRnk = myDic[rhPPoNo];

                    //if (myDic[rgPPoNo] == -1)
                    if (!myDic.ContainsKey(rgPPoNo))
                        myDic[rgPPoNo] = r.gPP.RnkBaz;
                    gpRnk = myDic[rgPPoNo];

                    if (r.hWon == 0 || r.hPP.RnkBaz == -1 || r.gPP.RnkBaz == -1)   // Oynanmamis veya Oyunculardan biri diskalifiye ise Rank hesaplama
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
                // Copy Dict to PP
                foreach (var pair in myDic)
                {
                    pp = Db.FromId<PP>(pair.Key);
                    pp.Rnk = pair.Value;
                }

                // Update PP Sira
                int sira = 1;
                foreach (var r in Db.SQL<BDB.PP>("select p from PP p order by p.Rnk desc"))
                {
                    r.Sra = sira++;
                }

                /*
                var items = from pair in myDic
                            orderby pair.Value descending
                            select pair;

                int sra = 1;
                foreach (var pair in items)
                {
                    pp = Db.FromId<PP>(pair.Key);
                    pp.Rnk = pair.Value;
                    pp.Sra = sra++;

                    //Console.WriteLine("{0}: {1}", pair.Key, pair.Value);
                }
                */
                /*
                var list = myDic.Keys.ToList();
                list.Sort();

                // Loop through keys.
                foreach (var key in list)
                {
                    pp = Db.FromId<PP>(key);
                    pp.Rnk = myDic[key];
                    pp.Sra = sra++;

                    //Console.WriteLine("{0}: {1}", key, myDic[key]);
                }
                */
                /*
                foreach (var pair in myDic)
                {
                    pp = Db.FromId<PP>(pair.Key);
                    pp.Rnk = pair.Value;
                }
                */
            });

            watch.Stop();
            Console.WriteLine($"refreshRH4 {nor}: {watch.ElapsedMilliseconds} msec  {watch.ElapsedTicks} ticks");
        }

        public static void RefreshRH2(ulong CCoNo)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            int nor = 0;
            var cc = Db.FromId<CC>(CCoNo);
            string RnkGrp = cc.RnkGrp;

            PPGR hPP;
            PPGR gPP;
            int NOPX = 0;
            Db.Transact(() =>
            {
                // Init
                // PP deki Rnk son degeri 

                var pps = Db.SQL<BDB.PPGR>("select p from PPGR p where p.RnkGrp = ?", RnkGrp);
                foreach (var p in pps)
                {
                    p.RnkBaz = p.PP.RnkBaz;
                    p.Rnk = p.PP.RnkBaz; // p.RnkBaz;
                    p.aO = 0;
                    p.vO = 0;
                }

                // ReCalculate Rank of RnkGrp Players
                int hpRnk = 0, gpRnk = 0;
                var rhs = Db.SQL<BDB.RH>("select p from RH p where p.CC.RnkGrp = ? order by p.Trh", RnkGrp);
                foreach (var r in rhs)
                {
                    nor++;

                    hPP = getPPGR(r.hPP, RnkGrp);
                    gPP = getPPGR(r.gPP, RnkGrp);

                    hpRnk = hPP.Rnk;
                    gpRnk = gPP.Rnk;

                    //if (r.hWon == 0 || r.hPP.Ad.StartsWith("∞") || r.gPP.Ad.StartsWith("∞"))   // Oynanmamis veya Oyunculardan biri diskalifiye ise Rank hesaplama
                    if (r.hWon == 0 || r.hPP.RnkBaz == -1 || r.gPP.RnkBaz == -1)   // Oynanmamis veya Oyunculardan biri diskalifiye ise Rank hesaplama
                        NOPX = 0;
                    else
                        NOPX = compNOPX(r.hWon, hpRnk, gpRnk);

                    r.hNOPX2 = NOPX;
                    r.gNOPX2 = -NOPX;
                    r.hpRnk2 = hpRnk;
                    r.gpRnk2 = gpRnk;

                    hPP.Rnk = hpRnk + NOPX;
                    gPP.Rnk = gpRnk - NOPX;

                    hPP.aO += r.hWon > 0 ? 1 : 0;
                    hPP.vO += r.hWon < 0 ? 1 : 0;
                    gPP.aO += r.gWon > 0 ? 1 : 0;
                    gPP.vO += r.gWon < 0 ? 1 : 0;
                }

                // Update PP Sira
                var ppgr = Db.SQL<BDB.PPGR>("select p from PPGR p where p.RnkGrp = ? order by p.Rnk desc", RnkGrp);
                int sira = 1;
                foreach (var r in ppgr)
                {
                    r.Sra = sira++;
                }
            });
            watch.Stop();
            Console.WriteLine($"refreshRH2 {nor}: {watch.ElapsedMilliseconds} msec  {watch.ElapsedTicks} ticks");
        }

        public static void RefreshRH3(ulong CCoNo)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            int nor = 0;

            var cc = Db.FromId<CC>(CCoNo);
            string RnkGrp = cc.RnkGrp;
            string Lig = RnkGrp.Substring(2, 1);
            int RnkBaz = 1700;
            if (Lig == "1")
                RnkBaz = 1900;
            else if (Lig == "2")
                RnkBaz = 1800;


            PPGR hPP;
            PPGR gPP;
            int NOPX = 0;
            Db.Transact(() =>
            {
                // Init
                // PP deki Rnk son degeri 
                //Db.SQL("DELETE FROM BDB.PPGR WHERE RnkGrp = ?", RnkGrp);
                
                var pps = Db.SQL<BDB.PPGR>("select p from PPGR p where p.RnkGrp = ?", RnkGrp);
                foreach (var p in pps)
                {
                    p.RnkBaz = p.PP.RnkBaz;
                    p.Rnk = p.PP.RnkBaz; // p.RnkBaz;
                    p.aO = 0;
                    p.vO = 0;
                }

                // ReCalculate Rank of RnkGrp Players
                int hpRnk = 0, gpRnk = 0;
                var rhs = Db.SQL<BDB.RH>("select p from RH p where p.CC.RnkGrp = ? order by p.Trh", RnkGrp);
                foreach (var r in rhs)
                {
                    nor++;

                    hPP = getPPGR(r.hPP, RnkGrp);
                    gPP = getPPGR(r.gPP, RnkGrp);

                    hpRnk = hPP.Rnk;
                    gpRnk = gPP.Rnk;

                    //if (r.hWon == 0 || r.hPP.Ad.StartsWith("∞") || r.gPP.Ad.StartsWith("∞"))   // Oynanmamis veya Oyunculardan biri diskalifiye ise Rank hesaplama
                    if (r.hWon == 0 || r.hPP.RnkBaz == -1 || r.gPP.RnkBaz == -1)   // Oynanmamis veya Oyunculardan biri diskalifiye ise Rank hesaplama
                        NOPX = 0;
                    else
                        NOPX = compNOPX(r.hWon, hpRnk, gpRnk);

                    r.hNOPX = NOPX;
                    r.gNOPX = -NOPX;
                    r.hpRnk = hpRnk;
                    r.gpRnk = gpRnk;

                    hPP.Rnk = hpRnk + NOPX;
                    gPP.Rnk = gpRnk - NOPX;

                    hPP.aO += r.hWon > 0 ? 1 : 0;
                    hPP.vO += r.hWon < 0 ? 1 : 0;
                    gPP.aO += r.gWon > 0 ? 1 : 0;
                    gPP.vO += r.gWon < 0 ? 1 : 0;
                }
                
                // Update PP Sira
                var ppgr = Db.SQL<BDB.PPGR>("select p from PPGR p where p.RnkGrp = ? order by p.Rnk desc", RnkGrp);
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

        // KULLANILMIYOR
        public static void updPPrnk(ulong CCoNo)
        {
            var cc = Db.FromId<BDB.CC>(CCoNo);

            // Sadece Sngl oynadiklarinda Rank hesaplanir
            // Home oyunculari tara, rakibi Guest

            int hRnk = 0,
                gRnk = 0;

            int PS = 0;    // Point Spread between players
            int ER = 0; // ExpectedResult
            int UR = 0; // UpsetResult
            string W = "";  // Winner H/G

            var cetr = Db.SQL<BDB.CETR>("select c from CETR c where c.CC = ? and c.SoD = ? and c.HoG = ? order by c.Trh", cc, "S", "H");
            foreach (var h in cetr)
            {
                hRnk = h.PP.Rnk == 0 ? h.PP.RnkBaz : h.PP.Rnk;
                // Guest/Rakip
                var g = Db.SQL<BDB.CETR>("select c from CETR c where c.CET = ? and c.Idx = ? and c.SoD = ? and c.HoG <> ?", h.CET, h.Idx, h.SoD, "G").FirstOrDefault();
                gRnk = g.PP.Rnk == 0 ? g.PP.RnkBaz : g.PP.Rnk;

                W = h.MW > g.MW ? "H" : "G";

                PS = Math.Abs(hRnk - gRnk);
                ER = 0;
                UR = 0;

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
            }

            if (hRnk >= gRnk)   // Home: Iyi
            {
                if (W == "H")   // Expected: Home kazanmis
                {
                    hRnk += ER;
                    gRnk -= ER;
                }
                else            // Upset: Home kaybetmis
                {
                    hRnk -= UR;
                    gRnk += UR;
                }
            }
            else                // Guest: Iyi
            {
                if (W == "G")   // Expected: Guest kazanmis
                {
                    hRnk -= ER;
                    gRnk += ER;
                }
                else            // Upset: Guest kayetmis
                {
                    hRnk += UR;
                    gRnk -= UR;
                }
            }
        }


        public static void LoadPP()     // Oyuncular
        {
            //sw.WriteLine($"{r.ID},{r.RnkBaz},{r.Sex},{r.DgmYil},{r.Ad},{r.eMail},{r.Tel}");

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
                            ID = ra[0],
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
            //sw.WriteLine($"{cc.ID},{cc.Ad},{cc.Skl},{cc.Grp},{cc.Idx}");

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
                            ID = ra[0],
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


        public static void BackupDB()
        {
            SavePP();
            SaveCC();
            foreach(var cc in Db.SQL<CC>("select r from CC r"))
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
                    sw.WriteLine($"{r.ID},{r.RnkBaz},{r.Sex},{r.DgmYil},{r.Ad},{r.eMail},{r.Tel}");
                }
            }
        }

        public static void SaveCC()                     // Turnuvalar
        {
            /* JSON deneme 
            // Read From DB
            var rec = new RowCC();
            rec.CCs.Data = Db.SQL<CC>("select r from CC r order by r.ID");
            var aaa = rec.ToJson();
            using (StreamWriter sw = new StreamWriter($@"C:\Starcounter\BodVedData\Ydk-CC2.txt", false))
            {
                sw.WriteLine(aaa);
            }
            // Write To DB
            dynamic json = new Json(aaa);
            string ad = json.CCs[0].Ad;
            int iz = json.CCs.Count;
            for (int i = 0; i < iz; i++)
            {

            }
            */
            var ccs = Db.SQL<CC>("select r from CC r order by r.ID");
            using (StreamWriter sw = new StreamWriter($@"C:\Starcounter\BodVedData\Ydk-CC.txt", false))
            {
                foreach (var cc in ccs)
                sw.WriteLine($"{cc.ID},{cc.Ad},{cc.Skl},{cc.Grp},{cc.Idx}");
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

        public static void SaveCTofCC(ulong CCoNo)      // Turnuva Takimlari
        {
            var cc = Db.FromId<CC>(CCoNo);

            using (StreamWriter sw = new StreamWriter($@"C:\Starcounter\BodVedData\Ydk-CT-{cc.ID}.txt", false))
            {
                var recs = Db.SQL<CT>("select r from CT r where r.CC = ? order by r.Ad", cc);
                foreach (var r in recs)
                {
                    sw.WriteLine($"{r.CC.ID},{r.ID},{r.Ad},{r.Adres},{r.Pw},{r.K1?.ID},{r.K2?.ID},{r.K1?.Ad},{r.K2?.Ad}");
                }
            }
        }

        public static void SaveCTPofCC(ulong CCoNo)     // Turnuva Takim Oyunculari
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

        public static void SaveCETofCC(ulong CCoNo)     // Turnuva Fikstur
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

        public static void SaveCETRofCC(ulong CCoNo)    // Turnuva Tum Sonuclari
        {
            var cc = Db.FromId<CC>(CCoNo);
            var recs = Db.SQL<CET>("select r from CET r where r.CC = ? order by r.ID", cc);
            foreach (var r in recs)
                SaveCETRofCET(r.oNo);
        }

        public static void SaveCETRofCET(ulong CEToNo)  // Musabaka Sonuclari
        {
            var cet = Db.FromId<CET>(CEToNo);

            var recs = Db.SQL<CETR>("select r from CETR r where r.CET = ? order by r.SoD desc, r.Idx, r.HoG desc", cet);
            if (recs.FirstOrDefault() == null)
                return;

            using (StreamWriter sw = new StreamWriter($@"C:\Starcounter\BodVedData\Ydk-CETR-{cet.CC.ID}-{cet.ID}.txt", false))
            {
                foreach (var r in recs)
                {
                    sw.WriteLine($"{r.CC.ID},{r.CET.ID},{r.CT.ID},{r.SoD},{r.Idx:D2},{r.HoG},{r.PP.ID},{r.S1W:D2},{r.S2W:D2},{r.S3W:D2},{r.S4W:D2},{r.S5W:D2},{r.SW},{r.SL},{r.MW},{r.ML},{r.PPAd,25}");
                }
            }
        }

        public static void SaveCETPofCC(ulong CCoNo)    // Turnuva OyuncuSiralama
        {
            var cc = Db.FromId<CC>(CCoNo);
            var recs = Db.SQL<CET>("select r from CET r where r.CC = ? order by r.ID", cc);
            foreach (var r in recs)
                SaveCETPofCET(r.oNo);
        }

        public static void SaveCETPofCET(ulong CEToNo)  // Musabaka OyuncuSiralama
        {
            var cet = Db.FromId<CET>(CEToNo);

            var recs = Db.SQL<CETP>("select r from CETP r where r.CET = ? and r.PP is not null order by r.SoD desc, r.Idx, r.HoG desc", cet);
            if (recs.FirstOrDefault() == null)
                return;

            using (StreamWriter sw = new StreamWriter($@"C:\Starcounter\BodVedData\Ydk-CETP-{cet.CC.ID}-{cet.ID}.txt", false))
            {
                foreach (var r in recs)
                {
                    sw.WriteLine($"{r.CC.ID},{r.CET.ID},{r.CT.ID},{r.SoD},{r.Idx:D2},{r.HoG},{r.PP.ID},{r.PPAd,25}");
                }
            }
        }

        
        
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
}
