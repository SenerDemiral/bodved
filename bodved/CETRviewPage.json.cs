using Starcounter;

namespace bodved
{
    partial class CETRviewPage : Json
    {
        protected override void OnData()
        {
            base.OnData();

            var cet = Db.FromId<BDB.CET>(ulong.Parse(CEToNo));
            CapHdr = $"{cet.CCAd} {cet.Tarih} Müsabaka";

            hCTAd = cet.hCTAd;
            gCTAd = cet.gCTAd;

            var SR = Db.SQL<BDB.CETR>("select c from CETR c where c.CET = ? and c.SoD = ? order by c.Idx, c.HoG desc", cet, "S");
            SinglesElementJson sng = null;

            foreach(var src in SR)
            {
                if (src.HoG == "H")
                {
                    sng = Singles.Add();
                    sng.hAd = src.PPAd;
                    sng.hSW = src.SW;
                    sng.hSL = src.SL;
                    sng.Idx = src.Idx;
                    sng.hSnc1 = src.Snc1;

                }
                else
                {
                    sng.gAd = src.PPAd;
                }
            }

            var DR = Db.SQL<BDB.CETR>("select c from CETR c where c.CET = ? and c.SoD = ? order by c.Idx, c.HoG desc", cet, "D");
            DoublesElementJson dbl = null;

            int c = 0;
            foreach (var src in DR)
            {
                if (src.HoG == "H")
                {
                    if ((c % 2) == 0)
                    {
                        dbl = Doubles.Add();
                        dbl.hAd = src.PPAd;
                        dbl.hSW = src.SW;
                        dbl.hSL = src.SL;
                        dbl.Idx = src.Idx;
                        dbl.hSnc1 = src.Snc1;
                    }
                    else
                        dbl.hAd += " + " + src.PPAd;
                }
                else
                {
                    if ((c % 2) == 0)
                        dbl.gAd = src.PPAd;
                    else
                        dbl.gAd += " + " + src.PPAd;
                }

                c++;
            }
        }
    }
}
