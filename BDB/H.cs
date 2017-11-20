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
        public static void updCETsumCC(ulong CCoNo)
        {
            var cets = Db.SQL<CET>("select c from CET c where c.CC.ObjectNo = ?", CCoNo);
            foreach (var r in cets)
            {
                if (r.Rok)
                    updCETsum(r.oNo);
            }
        }

        public static void updCETsum(ulong CEToNo)
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

            updCTsum(cet.hCT.oNo);
            updCTsum(cet.gCT.oNo);
        }

        // Sonuclari toplayip CET'e yaz KULLANILMIYOR
        public static void Cetr2Cet(string CEToNo)
        {
            var cet = Db.FromId<BDB.CET>(ulong.Parse(CEToNo));
            var recs = Db.SQL<CETR>("select c from CETR c where c.CET = ? order by c.SoD, c.Idx, c.HoG desc", cet);
            long hMSW = 0,
                 hMDW = 0,
                 gMSW = 0,
                 gMDW = 0;

            foreach (var r in recs)
            {
                if (r.SoD == "S")
                {
                    if (r.HoG == "H")
                        hMSW = r.S1W + r.S2W + r.S3W + r.S4W + r.S5W;
                    else
                        gMSW = r.S1W + r.S2W + r.S3W + r.S4W + r.S5W;
                }
                else
                {
                    if (r.HoG == "H")
                        hMDW = r.S1W + r.S2W + r.S3W + r.S4W + r.S5W;
                    else
                        gMDW = r.S1W + r.S2W + r.S3W + r.S4W + r.S5W;
                }
            }
        }

        public static void updCTsumCC(ulong CCoNo)
        {
            //var cc = Db.SQL<CC>("select r from CC r where r.ID = ?", ccID).FirstOrDefault();
            var cc = Db.FromId<CC>(CCoNo);

            var cts = Db.SQL<CT>("select r from CT r where r.CC = ?", cc);
            
            foreach (var ct in cts)
            {
                updCTsum(ct.oNo);
            }
        }

        public static void updCTsum(ulong CToNo)
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
            });
        }

        public static void reCreatePRHofCC(ulong CCoNo)
        {
            // Rank PRH kayitlarini yarat, Sadece Singles
            // Sonrasinda RefreshPRH gerekir!!!

            var cc = Db.FromId<BDB.CC>(CCoNo);

            // Delete PRHs of CC
            Db.Transact(() =>
            {
                var cetrs = Db.SQL<CETR>("select r from CETR r where r.CC = ?", cc);
                foreach (var cetr in cetrs)
                {
                    if (cetr.PRH != null)
                    {
                        Db.FromId<PRH>(cetr.PRH.GetObjectNo()).Delete();
                    }
                }
            });

            Db.Transact(() =>
            {
                PP pp = null, rpp = null;
                CETR h = null, g = null;
                int Won = 0;
                PRH prh;
                // Sadece Singles
                var cetrs = Db.SQL<CETR>("select r from CETR r where r.CC = ? and r.SoD = ?", cc, "S");
                foreach (var r in cetrs)
                {
                    if (r.HoG == "H")
                    {
                        h = r;
                        pp = r.PP;
                        if (r.MW > r.ML)
                            Won = 1;
                        else if (r.MW < r.ML)
                            Won = -1;
                    }
                    else if (r.HoG == "G")
                    {
                        g = r;
                        rpp = r.PP;

                        prh = new PRH
                        {
                            PP = pp,
                            rPP = rpp,
                            Won = Won,
                            Trh = r.Trh,
                        };
                        h.PRH = prh;

                        prh = new PRH
                        {
                            PP = rpp,
                            rPP = pp,
                            Won = Won * -1,
                            Trh = r.Trh,
                        };
                        g.PRH = prh;
                    }
                }
            });
            // PRH daki herhangi bir degisiklik hepsini etkiler!
            refreshPRH2();
        }

        // Kullanilmiyor Old version
        public static void refreshPRH(DateTime trh)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            // ReCalculate Rank of All Players
            var rr = Db.SQL<BDB.PRH>("select p from PRH p where p.Trh >= ? order by p.Trh", trh);

            int nor = 0;
            Db.Transact(() =>
            {
                foreach (var r in rr)
                {
                    r.NOPX = r.compNOPX;
                    r.Rnk = r.NOPX + r.prvRnk;
                    nor++;
                }
            });
            watch.Stop();
            Console.WriteLine($"refreshPRH {nor}: {watch.ElapsedMilliseconds} msec  {watch.ElapsedTicks} ticks");

            watch.Restart();
            ReCalcPPsra();
            Console.WriteLine($"ReCalcPPsra: {watch.ElapsedMilliseconds} msec  {watch.ElapsedTicks} ticks");
        }

        // Kullanilmiyor Old version
        public static void refreshPRH()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            // ReCalculate Rank of All Players
            var rr = Db.SQL<BDB.PRH>("select p from PRH p order by p.Trh");

            int nor = 0;
            Db.Transact(() =>
            {
                foreach (var r in rr)
                {
                    r.NOPX = r.compNOPX;
                    r.Rnk = r.NOPX + r.prvRnk;
                    nor++;
                }
            });
            watch.Stop();
            Console.WriteLine($"refreshPRH {nor}: {watch.ElapsedMilliseconds} msec  {watch.ElapsedTicks} ticks");

            watch.Restart();
            ReCalcPPsra();
            Console.WriteLine($"ReCalcPPsra: {watch.ElapsedMilliseconds} msec  {watch.ElapsedTicks} ticks");
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

        public static void refreshPRH2()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            int nor = 0;

            var pps = Db.SQL<BDB.PP>("select p from PP p");
            // ReCalculate Rank of All Players
            var rr = Db.SQL<BDB.PRH>("select p from PRH p order by p.Trh");

            int NOPX = 0;
            int pRnk = 0;
            int pRnkRkp = 0;
            PP pp = null;
            PP ppRkp = null;
            Db.Transact(() =>
            {
                // Init
                // PP deki Rnk son degeri 
                foreach (var p in pps)
                {
                    p.Rnk = 0; // p.RnkBaz;
                    nor++;
                }

                foreach (var r in rr)
                {
                    pp = Db.FromId<PP>(r.PP.GetObjectNo());
                    pRnk = pp.Rnk == 0 ? pp.RnkBaz : pp.Rnk;
                    ppRkp = Db.FromId<PP>(r.rPP.GetObjectNo());
                    pRnkRkp = ppRkp.Rnk == 0 ? ppRkp.RnkBaz : ppRkp.Rnk;

                    if (r.Won == 0 || r.PP.ID == "∞" || r.rPP.ID == "∞")   // Oynanmamis veya Oyunculardan biri diskalifiye ise Rank hesaplama
                        NOPX = 0;
                    else
                        NOPX = compNOPX(r.Won, pRnk, pRnkRkp);

                    r.NOPX = NOPX;
                    r.pRnk = pRnk;
                    r.Rnk = NOPX + pRnk;

                    pp.Rnk = r.Rnk;
                }

                // Update PP Sira
                pps = Db.SQL<BDB.PP>("select p from PP p order by p.Rnk desc, p.RnkBaz desc, p.Ad");
                int sira = 1;
                foreach (var r in pps)
                {
                    //r.Rnk = r.curRnk;
                    r.Sra = sira++;
                }

                /*
                foreach (var r in rr)
                {
                    pRnk = dct[r.PP.GetObjectNo()];
                    pRnkRkp = dct[r.rPP.GetObjectNo()];
                    r.NOPX2 = compNOPX(r.Won, pRnk, pRnkRkp);
                    r.Rnk2 = r.NOPX + pRnk;
                    dct[r.PP.GetObjectNo()] = r.Rnk2;
                }
                */
            });

            watch.Stop();
            Console.WriteLine($"refreshPRH2 {nor}: {watch.ElapsedMilliseconds} msec  {watch.ElapsedTicks} ticks");
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
            var r = Db.SQL<BDB.PRH>("select p from PRH p where p.PP.ObjectNo = ? and p.Trh < ? order by p.Trh desc", PPoNo, Trh).FirstOrDefault();
            return r?.Rnk ?? Db.FromId<BDB.PP>(PPoNo).RnkBaz;    // Zaten prev kayit Rnk al, prvRnk degil
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

                updCETsumCC(cc.oNo);
                updCTsumCC(cc.oNo);
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
            PRH prh;
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
                    if (cetr.PRH != null)
                    {
                        prh = Db.FromId<PRH>(cetr.PRH.GetObjectNo());
                        prh.Delete();
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

                updCTsum(cet.hCT.oNo);
                updCTsum(cet.gCT.oNo);

                refreshPRH();
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
            PRH prh;
            Db.Transact(() =>
            {
                var recs = Db.SQL<CETR>("select r from CETR r where r.CC = ?", cc);
                foreach (var cetr in recs)
                {
                    if (cetr.PRH != null)
                    {
                        prh = Db.FromId<PRH>(cetr.PRH.GetObjectNo());
                        prh.Delete();
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
                    if (cetr.PRH != null)
                    {
                        Db.FromId<PRH>(cetr.PRH.GetObjectNo()).Delete();
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

            // Rank PRH kayitlarini yarat, Sadece Singles
            // Sonrasinda RefreshPRH gerekir!!!
            Db.Transact(() =>
            {
                PP pp = null, rpp = null;
                CETR h = null, g = null;
                int Won = 0;
                PRH prh;
                var recs = Db.SQL<CETR>("select r from CETR r where r.CET = ? and r.SoD = ? order by r.Idx, r.HoG desc", cet, "S");
                foreach (var r in recs)
                {
                    if(r.HoG == "H")
                    {
                        h = r;
                        pp = r.PP;
                        if (r.MW > r.ML)
                            Won = 1;
                        else if (r.MW < r.ML)
                            Won = -1;
                    }
                    else if (r.HoG == "G")
                    {
                        g = r;
                        rpp = r.PP;

                        prh = new PRH
                        {
                            PP = pp,
                            rPP = rpp,
                            Won = Won,
                            Trh = cet.Trh,
                        };
                        h.PRH = prh;

                        prh = new PRH
                        {
                            PP = rpp,
                            rPP = pp,
                            Won = Won * -1,
                            Trh = cet.Trh,
                        };
                        g.PRH = prh;
                    }
                }
            });

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
            var ccs = Db.SQL<CC>("select r from CC r order by r.ID");
            using (StreamWriter sw = new StreamWriter($@"C:\Starcounter\BodVedData\Ydk-CC.txt", false))
            {
                foreach (var cc in ccs)
                sw.WriteLine($"{cc.ID},{cc.Ad},{cc.Skl},{cc.Grp},{cc.Idx}");
            }
        }

        public static void BackupCC(string ccID)        // Turnuva Ilgili Kayitlari
        {
            var cc = Db.SQL<CC>("select r from CC r where r.ID = ?", ccID).FirstOrDefault();

            if (cc != null)
            {
                SaveCTofCC(cc.oNo);
                SaveCTPofCC(cc.oNo);
                SaveCETofCC(cc.oNo);
                SaveCETPofCC(cc.oNo);
                SaveCETRofCC(cc.oNo);
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
    }
}
