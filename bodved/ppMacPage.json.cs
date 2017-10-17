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
            Cap1 = $"{pp.Ad} №{pp.oNo}";

            SinglesElementJson sng = null;
            var cetrS = Db.SQL<BDB.CETR>("select c from CETR c where c.PP = ? and c.SoD = ?", pp, "S");
            foreach(var k in cetrS)
            {
                
                // Rakip
                var r = Db.SQL<BDB.CETR>("select c from CETR c where c.CET = ? and c.Idx = ? and c.SoD = ? and c.PP <> ?", k.CET, k.Idx, k.SoD, pp).First;

                sng = Singles.Add();
                sng.oNo = (long)k.oNo;
                sng.Rakip = r.PPAd;
                sng.Sonuc = $"[{k.SW}-{r.SW}] {k.S1W}-{r.S1W} {k.S2W}-{r.S2W} {k.S3W}-{r.S3W} {k.S4W}-{r.S4W} {k.S5W}-{r.S5W}";

                sng.SW = $"{k.SW}";
                sng.SL = $"{r.SW}";
                sng.S1 = $"{k.S1W}-{r.S1W}";
                sng.S2 = $"{k.S2W}-{r.S2W}";
                sng.S3 = $"{k.S3W}-{r.S3W}";
                sng.S4 = $"{k.S4W}-{r.S4W}";
                sng.S5 = $"{k.S5W}-{r.S5W}";

                sng.WoL = k.SW > r.SW ? "W" : "L";

                // Esit sonuc sadece 0-0 da olur, bu da oynanmamis demektir
                SMo++;
                SSa += k.SW;
                SSv += r.SW;
                if (k.SW > r.SW)
                    SMa++;
                else if (k.SW < r.SW)
                    SMv++;
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
                dbl.oNo = (long)k.oNo;
                dbl.Ortak = o.PPAd;

                dbl.Rakip1 = r[0].PPAd;
                dbl.Rakip2 = r[1].PPAd;

                dbl.Sonuc = $"[{k.SW}-{r[0].SW}] {k.S1W}-{r[0].S1W} {k.S2W}-{r[0].S2W} {k.S3W}-{r[0].S3W} {k.S4W}-{r[0].S4W} {k.S5W}-{r[0].S5W}";

                dbl.SW = $"{k.SW}";
                dbl.SL = $"{r[0].SW}";
                dbl.S1 = $"{k.S1W}-{r[0].S1W}";
                dbl.S2 = $"{k.S2W}-{r[0].S2W}";
                dbl.S3 = $"{k.S3W}-{r[0].S3W}";
                dbl.S4 = $"{k.S4W}-{r[0].S4W}";
                dbl.S5 = $"{k.S5W}-{r[0].S5W}";

                dbl.WoL = k.SW > r[0].SW ? "W" : "L";

                DMo++;
                DSa += k.SW;
                DSv += r[0].SW;
                if (k.SW > r[0].SW)
                    DMa++;
                else if (k.SW < r[0].SW)
                    DMv++;
            }
        }
    }
}
