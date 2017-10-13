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

            hP = cet.hP;
            hPW = cet.hPW;
            hMSW = cet.hMSW;
            hMDW = cet.hMDW;
            hPSW = hMSW * 2;
            hPDW = hMDW * 3;

            gP = cet.gP;
            gPW = cet.gPW;
            gMSW = cet.gMSW;
            gMDW = cet.gMDW;
            gPSW = gMSW * 2;
            gPDW = gMDW * 3;

            hCTAd = cet.hCTAd;
            gCTAd = cet.gCTAd;

            var SR = Db.SQL<BDB.CETR>("select c from CETR c where c.CET = ? and c.SoD = ? order by c.Idx, c.HoG desc", cet, "S");
            SinglesElementJson sng = null;

            int s = 0;
            foreach (var src in SR)
            {
                if ((s % 2) == 0)
                {
                    sng = Singles.Add();
                    sng.Idx = src.Idx;

                    sng.hoNo = src.oNo.ToString();
                    sng.hPPAd = src.PPAd;
                    sng.hS1W = src.S1W; // src.S1W < 0 ? "" : src.S1W.ToString();
                    sng.hS2W = src.S2W;
                    sng.hS3W = src.S3W;
                    sng.hS4W = src.S4W;
                    sng.hS5W = src.S5W;
                }
                if ((s % 2) == 1)
                {
                    sng.goNo = src.oNo.ToString();
                    sng.gPPAd = src.PPAd;
                    sng.gS1W = src.S1W; // src.S1W < 0 ? "" : src.S1W.ToString();
                    sng.gS2W = src.S2W;
                    sng.gS3W = src.S3W;
                    sng.gS4W = src.S4W;
                    sng.gS5W = src.S5W;
                }

                s++;
            }

            var DR = Db.SQL<BDB.CETR>("select c from CETR c where c.CET = ? and c.SoD = ? order by c.Idx, c.HoG desc", cet, "D");
            DoublesElementJson dbl = null;
            int c = 0, i = 0;
            foreach (var src in DR)
            {
                if ((c % 4) == 0)
                {
                    dbl = Doubles.Add();
                    dbl.Idx = src.Idx;

                    dbl.hoNo1 = src.oNo.ToString();
                    dbl.hPPAd1 = src.PPAd;
                    dbl.hS1W = src.S1W;
                    dbl.hS2W = src.S2W;
                    dbl.hS3W = src.S3W;
                    dbl.hS4W = src.S4W;
                    dbl.hS5W = src.S5W;
                }
                if ((c % 4) == 1)
                {
                    dbl.hoNo2 = src.oNo.ToString();
                    dbl.hPPAd2 = src.PPAd;
                }
                if ((c % 4) == 2)
                {
                    dbl.goNo1 = src.oNo.ToString();
                    dbl.gPPAd1 = src.PPAd;
                    dbl.gS1W = src.S1W;
                    dbl.gS2W = src.S2W;
                    dbl.gS3W = src.S3W;
                    dbl.gS4W = src.S4W;
                    dbl.gS5W = src.S5W;
                }
                if ((c % 4) == 3)
                {
                    dbl.goNo2 = src.oNo.ToString();
                    dbl.gPPAd2 = src.PPAd;
                }

                c++;
            }

        }
        
        public void Handle(Input.SaveTrigger Action)
        {
            var cet = Db.FromId<BDB.CET>(ulong.Parse(CEToNo));

            Db.Transact(() =>
            {
                long hA, gA, hMW = 0, gMW = 0;
                foreach (var src in Singles)
                {
                    var hCetr = Db.FromId<BDB.CETR>(ulong.Parse(src.hoNo));
                    var gCetr = Db.FromId<BDB.CETR>(ulong.Parse(src.goNo));

                    hCetr.S1W = (int)src.hS1W;
                    hCetr.S2W = (int)src.hS2W;
                    hCetr.S3W = (int)src.hS3W;
                    hCetr.S4W = (int)src.hS4W;
                    hCetr.S5W = (int)src.hS5W;

                    gCetr.S1W = (int)src.gS1W;
                    gCetr.S2W = (int)src.gS2W;
                    gCetr.S3W = (int)src.gS3W;
                    gCetr.S4W = (int)src.gS4W;
                    gCetr.S5W = (int)src.gS5W;

                    hA = 0;
                    gA = 0;
                    if (src.hS1W > src.gS1W)
                        hA++;
                    else if (src.hS1W < src.gS1W)
                        gA++;
                    if (src.hS2W > src.gS2W)
                        hA++;
                    else if (src.hS2W < src.gS2W)
                        gA++;
                    if (src.hS3W > src.gS3W)
                        hA++;
                    else if (src.hS3W < src.gS3W)
                        gA++;
                    if (src.hS4W > src.gS4W)
                        hA++;
                    else if (src.hS4W < src.gS4W)
                        gA++;
                    if (src.hS5W > src.gS5W)
                        hA++;
                    else if (src.hS5W < src.gS5W)
                        gA++;
                    //(int)(src.hS1W + src.hS2W + src.hS3W + src.hS4W + src.hS5W);
                    if (hA > gA)
                        hMW++;
                    else
                        gMW++;
                }
                cet.hMSW = (int)hMW;
                cet.gMSW = (int)gMW;

                hMW = 0;
                gMW = 0;
                foreach (var src in Doubles)
                {
                    var hCetr1 = Db.FromId<BDB.CETR>(ulong.Parse(src.hoNo1));
                    var hCetr2 = Db.FromId<BDB.CETR>(ulong.Parse(src.hoNo2));
                    var gCetr1 = Db.FromId<BDB.CETR>(ulong.Parse(src.goNo1));
                    var gCetr2 = Db.FromId<BDB.CETR>(ulong.Parse(src.goNo2));

                    hCetr1.S1W = (int)src.hS1W;
                    hCetr1.S2W = (int)src.hS2W;
                    hCetr1.S3W = (int)src.hS3W;
                    hCetr1.S4W = (int)src.hS4W;
                    hCetr1.S5W = (int)src.hS5W;

                    hCetr2.S1W = (int)src.hS1W;
                    hCetr2.S2W = (int)src.hS2W;
                    hCetr2.S3W = (int)src.hS3W;
                    hCetr2.S4W = (int)src.hS4W;
                    hCetr2.S5W = (int)src.hS5W;

                    gCetr1.S1W = (int)src.gS1W;
                    gCetr1.S2W = (int)src.gS2W;
                    gCetr1.S3W = (int)src.gS3W;
                    gCetr1.S4W = (int)src.gS4W;
                    gCetr1.S5W = (int)src.gS5W;

                    gCetr2.S1W = (int)src.gS1W;
                    gCetr2.S2W = (int)src.gS2W;
                    gCetr2.S3W = (int)src.gS3W;
                    gCetr2.S4W = (int)src.gS4W;
                    gCetr2.S5W = (int)src.gS5W;

                    hA = 0;
                    gA = 0;
                    if (src.hS1W > src.gS1W)
                        hA++;
                    else if (src.hS1W < src.gS1W)
                        gA++;
                    if (src.hS2W > src.gS2W)
                        hA++;
                    else if (src.hS2W < src.gS2W)
                        gA++;
                    if (src.hS3W > src.gS3W)
                        hA++;
                    else if (src.hS3W < src.gS3W)
                        gA++;
                    if (src.hS4W > src.gS4W)
                        hA++;
                    else if (src.hS4W < src.gS4W)
                        gA++;
                    if (src.hS5W > src.gS5W)
                        hA++;
                    else if (src.hS5W < src.gS5W)
                        gA++;
                    //(int)(src.hS1W + src.hS2W + src.hS3W + src.hS4W + src.hS5W);
                    if (hA > gA)
                        hMW++;
                    else
                        gMW++;
                }

                cet.hMDW = (int)hMW;
                cet.gMDW = (int)gMW;

                cet.hPW = (cet.hMSW * 2) + (cet.hMDW * 3);
                cet.gPW = (cet.gMSW * 2) + (cet.gMDW * 3);

                if (cet.hPW > cet.gPW)
                {
                    cet.hP = 3;
                    cet.gP = 1;
                }
                else
                {
                    cet.hP = 1;
                    cet.gP = 3;
                }
            });
        }

        [CETRinpPage_json.Singles]
        public partial class SinglesElementJson
        {
            void Handle(Input.hS1W Action)
            {
                if (Action.Value < 0 || Action.Value > 21)
                {
                    Action.Cancel();
                    return;
                }
                else if (Action.Value <= 10)
                    this.gS1W = 11;
                else if (Action.Value > 11)
                    this.gS1W = Action.Value - 2;

                /*
                var hCetr = Db.FromId<BDB.CETR>(ulong.Parse(hoNo));
                var gCetr = Db.FromId<BDB.CETR>(ulong.Parse(goNo));
                Db.Transact(() =>
                {
                    hCetr.S1W = (int)Action.Value;
                    gCetr.S1W = (int)gS1W;
                });*/
            }

            void Handle(Input.gS1W Action)
            {
                if (Action.Value < 0 || Action.Value > 21)
                {
                    Action.Cancel();
                    return;
                }
                else if (Action.Value <= 10)
                    this.hS1W = 11;
                else if (Action.Value > 11)
                    this.hS1W = Action.Value - 2;
                /*
                var hCetr = Db.FromId<BDB.CETR>(ulong.Parse(hoNo));
                var gCetr = Db.FromId<BDB.CETR>(ulong.Parse(goNo));

                Db.Transact(() =>
                {
                    gCetr.S1W = (int)Action.Value;
                    hCetr.S1W = (int)hS1W;
                });*/
            }
        }

        public static void SaveSingles()
        {

           // var aaa = e.hS1W;
          //  var bbb = e.gS1W;
            /*
            if (sV < 0 || sV > 21)
                return;
            else if (sV <= 10)
                this.hS1W = 11;
            else if (sV > 11)
                this.hS1W = sV - 2;

            var hCetr = Db.FromId<BDB.CETR>(ulong.Parse(hoNo));
            var gCetr = Db.FromId<BDB.CETR>(ulong.Parse(goNo));

            Db.Transact(() =>
            {
                gCetr.S1W = (int)Action.Value;
                hCetr.S1W = (int)hS1W;
            });*/
        }
    }
}
