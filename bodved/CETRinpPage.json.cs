using Starcounter;

namespace bodved
{
    partial class CETRinpPage : Json
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

            int s = 0;
            foreach (var src in SR)
            {
                sng = Singles.Add();
                sng.Hide = false;
                sng.Idx = src.Idx;
                sng.oNo = src.oNo.ToString();
                sng.PPAd = src.PPAd;
                sng.S1W = src.S1W;
                sng.S2W = src.S2W;
                sng.S3W = src.S3W;
                sng.S4W = src.S4W;
                sng.S5W = src.S5W;
                if ((s % 2) == 1)
                    sng.Hide = true;

                s++;
            }

            var DR = Db.SQL<BDB.CETR>("select c from CETR c where c.CET = ? and c.SoD = ? order by c.Idx, c.HoG desc", cet, "D");
            DoublesElementJson dbl = null;
            int c = 0, i = 1;
            foreach (var src in DR)
            {
                if ((c % 2) == 0)
                {
                    dbl = Doubles.Add();

                    dbl.Idx = i;
                    dbl.oNo1 = src.oNo.ToString();
                    dbl.PPAd1 = src.PPAd;
                    dbl.S1W = src.S1W;
                    dbl.S2W = src.S2W;
                    dbl.S3W = src.S3W;
                    dbl.S4W = src.S4W;
                    dbl.S5W = src.S5W;
                }
                else
                { 
                    dbl.oNo2 = src.oNo.ToString();
                    dbl.PPAd2 = src.PPAd;
                }
    
                c++;
                if ((c % 4) == 0)
                    i++;

            }

        }
    }
}
