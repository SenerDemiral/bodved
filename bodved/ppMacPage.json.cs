﻿using System.Linq;
using Starcounter;

namespace bodved
{
    partial class ppMacPage : Json
    {
        protected override void OnData()
        {
            base.OnData();
            string ccInf = "";
            var pp = Db.FromId<BDB.PP>(ulong.Parse(PPoNo));
            if (CCoNo != "")
            {
                var cc = Db.FromId<BDB.CC>(ulong.Parse(CCoNo));
                RnkID = cc.RnkID;
                //RnkGrpAd = cc.RnkAd;

                var ppgr = Db.SQL<BDB.PPGR>("select p from BDB.PPGR p where p.PP = ? and p.RnkID = ?", pp, RnkID).FirstOrDefault();
                ccInf = $"Rank: {cc.RnkAd}/{ppgr?.Rnk}/{ppgr?.Sra}";

            }
            //Cap1 = $"{pp.Ad} ♯{pp.Lig}";// [{pp.ID}/{pp.oNo}]"; // №
            Cap1 = $"{pp.Ad} {ccInf}";
            Cap2 = $"Global Rank: {pp.Rnk}/{pp.Sra}"; // №

            SinglesElementJson sng = null;
            var cetrS = Db.SQL<BDB.CETR>("select c from CETR c where c.PP = ? and c.SoD = ? order by c.Trh desc", pp, "S");
            foreach (var k in cetrS)
            {
                if (RnkID == 0 || RnkID == k.CC.RnkID)
                {

                    // Rakip
                    var r = Db.SQL<BDB.CETR>("select c from CETR c where c.CET = ? and c.Idx = ? and c.SoD = ? and c.PP <> ?", k.CET, k.Idx, k.SoD, pp).FirstOrDefault();
                    if (r == null)
                        Cap2 = "Rakip is null";

                    sng = Singles.Add();
                    sng.oNo = (long)k.oNo;
                    sng.Tarih = k.Trh.ToString("dd.MM.yy");
                    sng.CEToNo = $"{k.CET.oNo}";
                    sng.CToNo = $"{k.CT.oNo}";
                    sng.CTAd = $"{k.CTAd}•{k.CC.Grp}";

                    if (k.HoG == "H")
                    {
                        if (RnkID == 0)
                        {
                            sng.Rnk = k.RH.hpRnk;  //prvRnk;
                            sng.NOBX = k.RH.hNOPX;
                            sng.rRnk = r.RH.gpRnk; //prvRnk;
                        }
                        else
                        {
                            sng.Rnk = k.RH.hpRnk2;  //prvRnk;
                            sng.NOBX = k.RH.hNOPX2;
                            sng.rRnk = r.RH.gpRnk2; //prvRnk;
                        }

                    }
                    else
                    {
                        if (RnkID == 0)
                        {
                            sng.Rnk = k.RH.gpRnk;  //prvRnk;
                            sng.NOBX = k.RH.gNOPX;
                            sng.rRnk = r.RH.hpRnk; //prvRnk;
                        }
                        else
                        {
                            sng.Rnk = k.RH.gpRnk2;  //prvRnk;
                            sng.NOBX = k.RH.gNOPX2;
                            sng.rRnk = r.RH.hpRnk2; //prvRnk;
                        }
                    }
                    //sng.Rnk = k.PRH.pRnk;  //prvRnk;
                    //sng.NOBX = k.PRH.NOPX;
                    //sng.rRnk = r.PRH.pRnk; //prvRnk;

                    sng.rPPoNo = r.PP.oNo.ToString();
                    sng.rPPAd = $"{r.PPAd}";
                    //sng.rPPAd = $"{r.PPAd} ♯{r.PP.Lig}";

                    sng.rCToNo = $"{r.CT.oNo}";
                    sng.rCTAd = r.CTAd;
                    sng.Sonuc = $"[{k.SW}-{r.SW}] {k.S1W}-{r.S1W} {k.S2W}-{r.S2W} {k.S3W}-{r.S3W} {k.S4W}-{r.S4W} {k.S5W}-{r.S5W}";

                    sng.SW = $"{k.SW}";
                    sng.SL = $"{r.SW}";

                    sng.S1 = BDB.H.MacSetResult(k.S1W, r.S1W);
                    sng.S2 = BDB.H.MacSetResult(k.S2W, r.S2W);
                    sng.S3 = BDB.H.MacSetResult(k.S3W, r.S3W);
                    sng.S4 = BDB.H.MacSetResult(k.S4W, r.S4W);
                    sng.S5 = BDB.H.MacSetResult(k.S5W, r.S5W);

                    sng.WoL = k.SW > r.SW ? "W" : "L";

                    // Esit sonuc sadece 0-0 da olur, bu da oynanmamis demektir
                    SMo++;

                    SSa += k.SW;
                    SSv += r.SW;
                    if (k.SW > r.SW)
                        SMa++;
                    else if (k.SW < r.SW)
                        SMv++;

                    SSo = SSa + SSv;
                }
            }

            DoublesElementJson dbl = null;
            var cetrD = Db.SQL<BDB.CETR>("select c from CETR c where c.PP = ? and c.SoD = ? order by c.Trh desc", pp, "D");
            foreach (var k in cetrD)
            {
                if (RnkID == 0 || RnkID == k.CC.RnkID)
                {
                    // Ortagi
                    var o = Db.SQL<BDB.CETR>("select c from CETR c where c.CET = ? and c.Idx = ? and c.SoD = ? and c.HoG = ? and c.ObjectNo <> ?", k.CET, k.Idx, k.SoD, k.HoG, k.GetObjectNo()).FirstOrDefault();

                    // Rakipleri 
                    var rHoG = k.HoG == "H" ? "G" : "H";
                    var r = Db.SQL<BDB.CETR>("select c from CETR c where c.CET = ? and c.Idx = ? and c.SoD = ? and c.HoG = ?", k.CET, k.Idx, k.SoD, rHoG).ToArray();

                    dbl = Doubles.Add();
                    dbl.oNo = (long)k.oNo;
                    dbl.Tarih = k.Trh.ToString("dd.MM.yy");
                    dbl.CEToNo = $"{k.CET.oNo}";
                    dbl.CToNo = $"{k.CT.oNo}";
                    dbl.CTAd = $"{k.CTAd}•{k.CC.Grp}";
                    dbl.oPPoNo = o.PP.oNo.ToString();
                    dbl.oPPAd = o.PPAd;

                    dbl.rPPoNo1 = r[0].PP.oNo.ToString();
                    dbl.rPPoNo2 = r[1].PP.oNo.ToString();
                    dbl.rPPAd1 = r[0].PPAd;
                    dbl.rPPAd2 = r[1].PPAd;

                    dbl.rCToNo = $"{r[0].CT.oNo}";
                    dbl.rCTAd = r[0].CTAd;
                    dbl.Sonuc = $"[{k.SW}-{r[0].SW}] {k.S1W}-{r[0].S1W} {k.S2W}-{r[0].S2W} {k.S3W}-{r[0].S3W} {k.S4W}-{r[0].S4W} {k.S5W}-{r[0].S5W}";

                    dbl.SW = $"{k.SW}";
                    dbl.SL = $"{r[0].SW}";

                    dbl.S1 = BDB.H.MacSetResult(k.S1W, r[0].S1W);
                    dbl.S2 = BDB.H.MacSetResult(k.S2W, r[0].S2W);
                    dbl.S3 = BDB.H.MacSetResult(k.S3W, r[0].S3W);
                    dbl.S4 = BDB.H.MacSetResult(k.S4W, r[0].S4W);
                    dbl.S5 = BDB.H.MacSetResult(k.S5W, r[0].S5W);

                    dbl.WoL = k.SW > r[0].SW ? "W" : "L";

                    DMo++;

                    DSa += k.SW;
                    DSv += r[0].SW;
                    if (k.SW > r[0].SW)
                        DMa++;
                    else if (k.SW < r[0].SW)
                        DMv++;

                    DSo = DSa + DSv;
                }
            }
        }
    }
}
