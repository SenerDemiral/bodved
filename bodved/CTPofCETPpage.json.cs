using Starcounter;
using System.Linq;

namespace bodved
{
    partial class CTPofCETPpage : Json
    {
        protected override void OnData()
        {
            base.OnData();
            // Input: CTPofCETpage/CEToNo/CToNo

            var cet = Db.FromId<BDB.CET>(ulong.Parse(CEToNo));
            var ct = Db.FromId<BDB.CT>(ulong.Parse(CToNo));

            CCoNo = ct.CC.GetObjectNo().ToString();
            Cap1 = $"{ct.CCAd} {ct.Ad} Takım Oyuncuları Maç Sıraları";

            PPsElementJson pps;
            int tkmSra = 1;
            foreach(var ctp in Db.SQL<BDB.CTP>("select c from CTP c where c.CT = ? order by Idx", ct))
            {
                var cetp = Db.SQL<BDB.CETP>("select c from CETP c where c.CET = ? and c.CT = ? and c.PP = ? and SoD = ? and Idx < ?", cet, ct, ctp.PP, "S", 9).FirstOrDefault();
                if (cetp != null)
                {
                    pps = PPs.Add();
                    pps.TkmSra = tkmSra;
                    pps.PPAd = cetp.PPAd;
                    pps.MacSra = cetp.Idx;

                    //if ((tkmSra - 2) >= pps.MacSra && pps.MacSra <= (tkmSra + 2))
                    if (pps.MacSra < (tkmSra - 2) || pps.MacSra > (tkmSra + 2))
                        pps.Hata = "✗";
                    else
                        pps.Hata = "✓";

                    tkmSra++;
                }

            }
        }
    }
}