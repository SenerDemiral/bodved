using System.Linq;
using Starcounter;

namespace bodved
{
    partial class ppMacPage : Json
    {
        protected override void OnData()
        {
            base.OnData();

            var pp = Db.FromId<BDB.PP>(ulong.Parse(PPoNo));

            SinglesElementJson sng = null;
            var cetrS = Db.SQL<BDB.CETR>("select c from CETR c where c.PP = ? and c.SoD = ?", pp, "S");
            foreach(var k in cetrS)
            {
                // Rakip
                var r = Db.SQL<BDB.CETR>("select c from CETR c where c.CET = ? and c.Idx = ? and c.SoD = ? and c.PP <> ?", k.CET, k.Idx, k.SoD, pp).First;

                sng = Singles.Add();
                sng.Rakip = r.PPAd;
                sng.Sonuc = $"[{k.SW}-{r.SW}] {k.S1W}-{r.S1W} {k.S2W}-{r.S2W} {k.S3W}-{r.S3W} {k.S4W}-{r.S4W} {k.S5W}-{r.S5W}";

                // Esit sonuc sadece 0-0 da olur, bu da oynanmamis demektir
                if (k.SW > r.SW)
                {
                    TMSW++;
                    TSSW += k.SW;
                }
                else if (k.SW < r.SW)
                {
                    TMSL++;
                    TSSL += r.SW;
                }
            }

            DoublesElementJson dbl = null;
            var cetrD = Db.SQL<BDB.CETR>("select c from CETR c where c.PP = ? and c.SoD = ?", pp, "D");
            foreach(var k in cetrD)
            {
                var o = Db.SQL<BDB.CETR>("select c from CETR c where c.CET = ? and c.Idx = ? and c.SoD = ? and c.HoG = ? and c.PP <> ?", k.CET, k.Idx, k.SoD, k.HoG, pp).First;
                
                // Rakip
                var rHoG = k.HoG == "H" ? "G" : "H";
                var r = Db.SQL<BDB.CETR>("select c from CETR c where c.CET = ? and c.Idx = ? and c.SoD = ? and c.HoG = ?", k.CET, k.Idx, k.SoD, rHoG).ToArray();

                dbl = Doubles.Add();
                dbl.Ortak = o.PPAd;

                dbl.Rakip1 = r[0].PPAd;
                dbl.Rakip2 = r[1].PPAd;

                dbl.Sonuc = $"[{k.SW}-{r[0].SW}] {k.S1W}-{r[0].S1W} {k.S2W}-{r[0].S2W} {k.S3W}-{r[0].S3W} {k.S4W}-{r[0].S4W} {k.S5W}-{r[0].S5W}";

                if (k.SW > r[0].SW)
                {
                    TMDW++;
                    TSDW += k.SW;
                }
                else if (k.SW < r[0].SW)
                {
                    TMDL++;
                    TSDL += r[0].SW;
                }
            }

            TMS = TMSW + TMSL;
            TMD = TMDW + TMDL;
        }
    }
}
