using Starcounter;

namespace bodved
{
    partial class OyuncuMacPage : Json
    {
        protected override void OnData()
        {
            base.OnData();

            Read();
        }

        protected void Read()
        {
            ulong PPoNo = 0;

            SinglesElementJson sng = null;
            foreach (var mac in Db.SQL<BDB.MAC>("select m from MAC m where m.SoD = ? and (m.hPP1.ObjectNo = ? or m.gPP1.ObjectNo = ?) order by m.Trh", "S", PPoNo, PPoNo))
            {
                sng = Singles.Add();
                sng.oNo = (long)mac.oNo;
                sng.Tarih = $"{mac.Trh:dd.MM.yy}";
                sng.CEToNo = $"{mac.CET?.oNo}";

                if (PPoNo == mac.hPP1.oNo)  // Oyuncu = Home
                {
                    SMa += mac.hMW;
                    SMv += mac.gMW;

                    SSa += mac.hSW;
                    SSv += mac.gSW;

                    sng.CToNo = $"{mac.CET?.hCT.oNo}";
                    sng.CTAd = $"{mac.CET?.hCTAd}";

                    sng.Drm = $"{mac.hDrm}";
                    sng.Rnk = mac.hpRnk;
                    sng.NOBX = mac.hNOPX;

                    sng.SW = $"{mac.hSW}";
                    sng.SL = $"{mac.gSW}";
                    sng.WoL = mac.hSW > mac.gSW ? "W" : "L";

                    sng.S1 = BDB.H.MacSetResult(mac.hS1W, mac.gS1W);
                    sng.S2 = BDB.H.MacSetResult(mac.hS2W, mac.gS2W);
                    sng.S3 = BDB.H.MacSetResult(mac.hS3W, mac.gS3W);
                    sng.S4 = BDB.H.MacSetResult(mac.hS4W, mac.gS4W);
                    sng.S5 = BDB.H.MacSetResult(mac.hS5W, mac.gS5W);

                    // Rakip
                    sng.rDrm = $"{mac.gDrm}";
                    sng.rCToNo = $"{mac.CET?.gCT.oNo}";
                    sng.rCTAd = $"{mac.CET?.gCTAd}";
                    sng.rPPoNo = $"{mac.gPP1.oNo}";
                    sng.rPPAd = $"{mac.gPP1.Ad}";
                    sng.rRnk = mac.gpRnk;
                }
                else  // Oyuncu = Guest
                {
                    SMa += mac.gMW;
                    SMv += mac.hMW;

                    SSa += mac.gSW;
                    SSv += mac.hSW;

                    sng.CToNo = $"{mac.CET?.gCT.oNo}";
                    sng.CTAd = $"{mac.CET?.gCTAd}";

                    sng.Drm = $"{mac.gDrm}";
                    sng.Rnk = mac.gpRnk;
                    sng.NOBX = mac.gNOPX;

                    sng.SW = $"{mac.gSW}";
                    sng.SL = $"{mac.hSW}";
                    sng.WoL = mac.gSW > mac.hSW ? "W" : "L";

                    sng.S1 = BDB.H.MacSetResult(mac.gS1W, mac.hS1W);
                    sng.S2 = BDB.H.MacSetResult(mac.gS2W, mac.hS2W);
                    sng.S3 = BDB.H.MacSetResult(mac.gS3W, mac.hS3W);
                    sng.S4 = BDB.H.MacSetResult(mac.gS4W, mac.hS4W);
                    sng.S5 = BDB.H.MacSetResult(mac.gS5W, mac.hS5W);

                    // Rakip
                    sng.rDrm = $"{mac.hDrm}";
                    sng.rCToNo = $"{mac.CET?.hCT.oNo}";
                    sng.rCTAd = $"{mac.CET?.hCTAd}";
                    sng.rPPoNo = $"{mac.hPP1.oNo}";
                    sng.rPPAd = $"{mac.hPP1.Ad}";
                    sng.rRnk = mac.hpRnk;
                }
                SMo = SMa + SMv;
                SSo = SSa + SSv;
            }

            DoublesElementJson dbl = null;
            foreach (var mac in Db.SQL<BDB.MAC>("select m from MAC m where m.SoD = ? and (m.hPP1.ObjectNo = ? or m.gPP1.ObjectNo = ? or m.hPP2.ObjectNo = ? or m.gPP2.ObjectNo = ?) order by m.Trh", "D", PPoNo, PPoNo, PPoNo, PPoNo))
            {
                dbl = Doubles.Add();

                dbl.oNo = (long)mac.oNo;
                dbl.Tarih = $"{mac.Trh:dd.MM.yy}";
                dbl.CEToNo = $"{mac.CET?.oNo}";

                if (PPoNo == mac.hPP1.oNo || PPoNo == mac.hPP2.oNo)  // Oyuncu = Home
                {
                    DMa += mac.hMW;
                    DMv += mac.gMW;

                    DSa += mac.hSW;
                    DSv += mac.gSW;

                    dbl.CToNo = $"{mac.CET?.hCT.oNo}";
                    dbl.CTAd = $"{mac.CET?.hCTAd}";

                    dbl.Drm = $"{mac.hDrm}";

                    dbl.SW = $"{mac.hSW}";
                    dbl.SL = $"{mac.gSW}";
                    dbl.WoL = mac.hSW > mac.gSW ? "W" : "L";

                    dbl.S1 = BDB.H.MacSetResult(mac.hS1W, mac.gS1W);
                    dbl.S2 = BDB.H.MacSetResult(mac.hS2W, mac.gS2W);
                    dbl.S3 = BDB.H.MacSetResult(mac.hS3W, mac.gS3W);
                    dbl.S4 = BDB.H.MacSetResult(mac.hS4W, mac.gS4W);
                    dbl.S5 = BDB.H.MacSetResult(mac.hS5W, mac.gS5W);

                    // Rakip
                    dbl.rDrm = $"{mac.gDrm}";
                    dbl.rCToNo = $"{mac.CET?.gCT.oNo}";
                    dbl.rCTAd = $"{mac.CET?.gCTAd}";
                    dbl.rPP1oNo = $"{mac.gPP1.oNo}";
                    dbl.rPP1Ad = $"{mac.gPP1.Ad}";
                    dbl.rPP2oNo = $"{mac.gPP2.oNo}";
                    dbl.rPP2Ad = $"{mac.gPP2.Ad}";
                }
                else  // Oyuncu = Guest
                {
                    DMa += mac.gMW;
                    DMv += mac.hMW;

                    DSa += mac.gSW;
                    DSv += mac.hSW;

                    dbl.CToNo = $"{mac.CET?.gCT.oNo}";
                    dbl.CTAd = $"{mac.CET?.gCTAd}";

                    dbl.Drm = $"{mac.gDrm}";

                    dbl.SW = $"{mac.gSW}";
                    dbl.SL = $"{mac.hSW}";
                    dbl.WoL = mac.gSW > mac.hSW ? "W" : "L";

                    dbl.S1 = BDB.H.MacSetResult(mac.gS1W, mac.hS1W);
                    dbl.S2 = BDB.H.MacSetResult(mac.gS2W, mac.hS2W);
                    dbl.S3 = BDB.H.MacSetResult(mac.gS3W, mac.hS3W);
                    dbl.S4 = BDB.H.MacSetResult(mac.gS4W, mac.hS4W);
                    dbl.S5 = BDB.H.MacSetResult(mac.gS5W, mac.hS5W);

                    // Rakip
                    dbl.rDrm = $"{mac.hDrm}";
                    dbl.rCToNo = $"{mac.CET?.hCT.oNo}";
                    dbl.rCTAd = $"{mac.CET?.hCTAd}";
                    dbl.rPP1oNo = $"{mac.hPP1.oNo}";
                    dbl.rPP1Ad = $"{mac.hPP1.Ad}";
                    dbl.rPP2oNo = $"{mac.hPP2.oNo}";
                    dbl.rPP2Ad = $"{mac.hPP2.Ad}";
                }
                DMo = DMa + DMv;
                DSo = DSa + DSv;
            }
        }
    }
}
