using Starcounter;

namespace bodved
{
    partial class MACpage : Json
    {
        protected override void OnData()
        {
            base.OnData();

            Read();
        }

        protected void Read()
        {
            var mac = Db.FromId<BDB.MAC>(ulong.Parse(MACoNo));

            CCoNo = (long)mac.CC.oNo;
            if (mac.CET != null)
            {
                hCToNo = (long)mac.CET.hCToNo;
                gCToNo = (long)mac.CET.gCToNo;
                hCTAd = mac.CET.hCTAd;
                gCTAd = mac.CET.gCTAd;
                //Rok = cet.Rok;

            }

            //RnkID = cet.CC.RnkID;
            Cap1 = $"Maç Sonuçlarý";
            //Cap1 = $"{cet.CCAd} [{cet.Trh:dd.MM.yy}] Müsabaka Sonuçlarý";
            //Cap2 = $"{cet.hCTAd} <> {cet.gCTAd}"; // Müsabaka Sonuçlarý";

            if (canMdfy)
            {
                hPPsElementJson hps;

                foreach (var t in Db.SQL<BDB.CTP>("select p from CTP p where p.CT = ? order by p.PP.Ad", mac.CET.hCT))
                {
                    hps = hPPs.Add();
                    hps.oNo = t.PP.oNo.ToString();
                    hps.Ad = t.PP.Ad;
                }

                gPPsElementJson gps;

                foreach (var t in Db.SQL<BDB.CTP>("select p from CTP p where p.CT = ? order by p.PP.Ad", mac.CET.gCT))
                {
                    gps = gPPs.Add();
                    gps.oNo = t.PP.oNo.ToString();
                    gps.Ad = t.PP.Ad;
                }
            }

            Tarih = $"{mac.Trh:yyyy-MM-dd}"; // yyyy-MM-dd HH:mm}";
            Saat = $"{mac.Trh:HH:mm}"; // yyyy-MM-dd HH:mm}";
            hDrm = $"{mac.hDrm}";
            gDrm = $"{mac.gDrm}";

            hPP1oNo = $"{mac.hPP1.oNo}"; //(long)mac.hPP1.oNo;
            hPP1Ad = mac.hPP1.Ad;
            gPP1oNo = $"{mac.gPP1.oNo}"; //(long)mac.gPP1.oNo;
            gPP1Ad = mac.gPP1.Ad;

            hPP2oNo = $"{mac.hPP2?.oNo}"; //(long)mac.hPP1.oNo;
            hPP2Ad = mac.hPP2?.Ad;
            gPP2oNo = $"{mac.gPP2?.oNo}"; //(long)mac.gPP1.oNo;
            gPP2Ad = mac.gPP2?.Ad;

            hS1W = mac.hS1W;
            gS1W = mac.gS1W;
            hS2W = mac.hS2W;
            gS2W = mac.gS2W;
            hS3W = mac.hS3W;
            gS3W = mac.gS3W;
            hS4W = mac.hS4W;
            gS4W = mac.gS4W;
            hS5W = mac.hS5W;
            gS5W = mac.gS5W;

            hSW += hS1W > gS1W ? 1 : 0;
            gSW += hS1W < gS1W ? 1 : 0;
            hSW += hS2W > gS2W ? 1 : 0;
            gSW += hS2W < gS2W ? 1 : 0;
            hSW += hS3W > gS3W ? 1 : 0;
            gSW += hS3W < gS3W ? 1 : 0;
            hSW += hS4W > gS4W ? 1 : 0;
            gSW += hS4W < gS4W ? 1 : 0;
            hSW += hS5W > gS5W ? 1 : 0;
            gSW += hS5W < gS5W ? 1 : 0;

        }
    }
}
