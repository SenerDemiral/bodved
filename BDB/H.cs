using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Starcounter;
using System.IO;

namespace BDB
{
    public static class H
    {

        // Sonuclari toplayip CET'e yaz
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

        public static void updCTsum(ulong oNo)
        {
            int aP = 0; // Musabakalardan Aldigi Puan Toplami
            int vP = 0; //                Verdigi
            int oE = 0; // Oynadigi Event
            int aE = 0; // Aldigi/Kazandigi Event
            int vE = 0; // Verdigi/Kaybettigi Event

            var ct = Db.FromId<CT>(oNo);
            //Musabaka Soncu Onaylanmislari tara
            foreach (var r in Db.SQL<CET>("select c from CET c where c.hCT = ? and c.Rok = ?", ct, true))
            {
                aP += r.hP;
                vP += r.gP;
                oE++;
                if (r.hP > r.gP)
                    aE++;
                else
                    vE++;

            }
            foreach (var r in Db.SQL<CET>("select c from CET c where c.gCT = ? and c.Rok = ?", ct, true))
            {
                aP += r.gP;
                vP += r.hP;
                oE++;
                if (r.hP < r.gP)
                    aE++;
                else
                    vE++;
            }

            Db.Transact(() =>
            {
                ct.aP = aP;
                ct.vP = vP;
                ct.oE = oE;
                ct.aE = aE;
                ct.vE = vE;
            });

        }

        public static void refreshPRH()
        {
            // ReCalculate Rank of All Players
            var rr = Db.SQL<BDB.PRH>("select p from PRH p order by p.Trh ASC");

            Db.Transact(() =>
            {
                foreach (var r in rr)
                {
                    r.NOPX = r.compNOPX;
                    r.Rnk = r.NOPX + r.prvRnk;
                }
            });
        }
        
        public static int PPprvRnk(ulong PPoNo, DateTime Trh)
        {
            var r = Db.SQL<BDB.PRH>("select p from PRH p where p.PP.ObjectNo = ? and p.Trh < ? order by p.Trh desc", PPoNo, Trh).First;
            return r?.prvRnk ?? Db.FromId<BDB.PP>(PPoNo).RnkBaz;
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
                var g = Db.SQL<BDB.CETR>("select c from CETR c where c.CET = ? and c.Idx = ? and c.SoD = ? and c.HoG <> ?", h.CET, h.Idx, h.SoD, "G").First;
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

        public static void Write2Log(string Msg)
        {
            //StreamWriter sw = null;
            //if (sw == null)
            //    sw = new StreamWriter(@"C:\Starcounter\MyLog\tMaxLogin-Log.txt", true);

            try
            {
                StreamWriter sw = new StreamWriter(@"C:\Starcounter\MyLog\BodVed-Log.txt", true);
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ": " + Msg);
                sw.Flush();
                sw.Close();
            }
            catch
            {
            }
        }
    }
}
