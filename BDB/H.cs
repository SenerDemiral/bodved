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

            foreach (var r in Db.SQL<CET>("select c from CET c where c.hCT = ?", ct))
            {
                aP += r.hP;
                vP += r.gP;
                oE++;
                if (r.hP > r.gP)
                    aE++;
                else
                    vE++;

            }
            foreach (var r in Db.SQL<CET>("select c from CET c where c.gCT = ?", ct))
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
