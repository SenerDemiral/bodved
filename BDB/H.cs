using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Starcounter;

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

    }
}
