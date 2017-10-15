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
