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
            Cap1 = $"{pp.Ad} [{pp.ID}/{pp.oNo}]"; // №

            SinglesElementJson sng = null;
            var cetrS = Db.SQL<BDB.CETR>("select c from CETR c where c.PP = ? and c.SoD = ? order by c.Trh desc", pp, "S");
            foreach(var k in cetrS)
            {
                
                // Rakip
                var r = Db.SQL<BDB.CETR>("select c from CETR c where c.CET = ? and c.Idx = ? and c.SoD = ? and c.PP <> ?", k.CET, k.Idx, k.SoD, pp).FirstOrDefault();

                sng = Singles.Add();
                sng.oNo = (long)k.oNo;
                sng.Tarih = k.Trh.ToString("yyy-MM-dd");
                sng.Rnk = k.PRH.pRnk;  //prvRnk;
                sng.NOBX = k.PRH.NOPX;
                sng.rRnk = r.PRH.pRnk; //prvRnk;

                sng.rPPoNo = r.PP.oNo.ToString();
                sng.rPPAd = r.PPAd;
                sng.Sonuc = $"[{k.SW}-{r.SW}] {k.S1W}-{r.S1W} {k.S2W}-{r.S2W} {k.S3W}-{r.S3W} {k.S4W}-{r.S4W} {k.S5W}-{r.S5W}";

                sng.SW = $"{k.SW}";
                sng.SL = $"{r.SW}";
                sng.S1 = $"{k.S1W}-{r.S1W}";
                if (k.S1W == 0 && r.S1W == 0)
                    sng.S1 = "";
                sng.S2 = $"{k.S2W}-{r.S2W}";
                if (k.S2W == 0 && r.S2W == 0)
                    sng.S2 = "";
                sng.S3 = $"{k.S3W}-{r.S3W}";
                if (k.S3W == 0 && r.S3W == 0)
                    sng.S3 = "";
                sng.S4 = $"{k.S4W}-{r.S4W}";
                if (k.S4W == 0 && r.S4W == 0)
                    sng.S4 = "";
                sng.S5 = $"{k.S5W}-{r.S5W}";
                if (k.S5W == 0 && r.S5W == 0)
                    sng.S5 = "";

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
            SSo = SSa + SSv;

            DoublesElementJson dbl = null;
            var cetrD = Db.SQL<BDB.CETR>("select c from CETR c where c.PP = ? and c.SoD = ?", pp, "D");
            foreach(var k in cetrD)
            {
                // Ortagi
                var o = Db.SQL<BDB.CETR>("select c from CETR c where c.CET = ? and c.Idx = ? and c.SoD = ? and c.HoG = ? and c.ObjectNo <> ?", k.CET, k.Idx, k.SoD, k.HoG, k.GetObjectNo()).FirstOrDefault();

                // Rakipleri 
                var rHoG = k.HoG == "H" ? "G" : "H";
                var r = Db.SQL<BDB.CETR>("select c from CETR c where c.CET = ? and c.Idx = ? and c.SoD = ? and c.HoG = ?", k.CET, k.Idx, k.SoD, rHoG).ToArray();

                dbl = Doubles.Add();
                dbl.oNo = (long)k.oNo;
                dbl.Tarih = k.Trh.ToString("dd.MM.yy");
                dbl.oPPoNo = o.PP.oNo.ToString();
                dbl.oPPAd = o.PPAd;

                dbl.rPPoNo1 = r[0].PP.oNo.ToString();
                dbl.rPPoNo2 = r[1].PP.oNo.ToString();
                dbl.rPPAd1 = r[0].PPAd;
                dbl.rPPAd2 = r[1].PPAd;

                dbl.Sonuc = $"[{k.SW}-{r[0].SW}] {k.S1W}-{r[0].S1W} {k.S2W}-{r[0].S2W} {k.S3W}-{r[0].S3W} {k.S4W}-{r[0].S4W} {k.S5W}-{r[0].S5W}";

                dbl.SW = $"{k.SW}";
                dbl.SL = $"{r[0].SW}";
                dbl.S1 = $"{k.S1W}-{r[0].S1W}";
                if (k.S1W == 0 && r[0].S1W == 0)
                    dbl.S1 = "";
                dbl.S2 = $"{k.S2W}-{r[0].S2W}";
                if (k.S2W == 0 && r[0].S2W == 0)
                    dbl.S2 = "";
                dbl.S3 = $"{k.S3W}-{r[0].S3W}";
                if (k.S3W == 0 && r[0].S3W == 0)
                    dbl.S3 = "";
                dbl.S4 = $"{k.S4W}-{r[0].S4W}";
                if (k.S4W == 0 && r[0].S4W == 0)
                    dbl.S4 = "";
                dbl.S5 = $"{k.S5W}-{r[0].S5W}";
                if (k.S5W == 0 && r[0].S5W == 0)
                    dbl.S5 = "";

                dbl.WoL = k.SW > r[0].SW ? "W" : "L";

                DMo++;

                DSa += k.SW;
                DSv += r[0].SW;
                if (k.SW > r[0].SW)
                    DMa++;
                else if (k.SW < r[0].SW)
                    DMv++;
            }
            DSo = DSa + DSv;
        }
    }
}
