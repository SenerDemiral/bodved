using Starcounter;

namespace bodved
{
    partial class CETRinpPage : Json
    {
        protected override void OnData()
        {
            base.OnData();

            var cet = Db.FromId<BDB.CET>(ulong.Parse(CEToNo));
            hCTAd = cet.hCTAd;
            gCTAd = cet.gCTAd;
            Rok = cet.Rok;

            var mpLgn = (Root as MasterPage).Login;
            canMdfy = mpLgn.Rl == "ADMIN" && mpLgn.LI ? true : false;
            if (!canMdfy)
            {
                if (mpLgn.Rl == "TAKIM")
                    if (mpLgn.LI && (mpLgn.Id == cet.hCToNo.ToString() || mpLgn.Id == cet.gCToNo.ToString()))
                        canMdfy = true;
            }
            if (mpLgn.Rl == "ADMIN")
                Rok = false;    // Admin sonuc OK olsa bile degistirebilsin


            Cap1 = $"{cet.CCAd}";
            Cap2 = $"{cet.Tarih} {cet.hCTAd} <> {cet.gCTAd} Müsabaka Sonuçlarý";

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
                    sng.hPPrnk = src.PRH.prvRnk;
                    sng.hS1W = src.S1W; // src.S1W < 0 ? "" : src.S1W.ToString();
                    sng.hS2W = src.S2W;
                    sng.hS3W = src.S3W;
                    sng.hS4W = src.S4W;
                    sng.hS5W = src.S5W;
                    sng.hSW = src.SW;
                }
                if ((s % 2) == 1)
                {
                    sng.goNo = src.oNo.ToString();
                    sng.gPPAd = src.PPAd;
                    sng.gPPrnk = src.PRH.prvRnk;

                    sng.gS1W = src.S1W; // src.S1W < 0 ? "" : src.S1W.ToString();
                    sng.gS2W = src.S2W;
                    sng.gS3W = src.S3W;
                    sng.gS4W = src.S4W;
                    sng.gS5W = src.S5W;
                    sng.gSW = src.SW;
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
                    dbl.hDW = src.SW;
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
                    dbl.gDW = src.SW;
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
            Save();
        }

        public void Handle(Input.SaveOkTrigger Action)
        {
            Save();

            // Sonuc OK Onayla
            var cet = Db.FromId<BDB.CET>(ulong.Parse(CEToNo));
            Db.Transact(() =>
            {
                cet.Rok = true;
            });

            BDB.H.updCTsum(cet.hCT.oNo);
        }

        protected void Save()
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

                    hCetr.SW = (int)hA;
                    hCetr.SL = (int)gA;
                    gCetr.SW = (int)gA;
                    gCetr.SL = (int)hA;

                    src.hSW = hA;
                    src.gSW = gA;

                    hCetr.MW = 0;
                    hCetr.ML = 0;
                    gCetr.MW = 0;
                    gCetr.ML = 0;
                    if ((hA + gA) > 0)  // Oynandiysa
                    {
                        if (hA > gA)
                        {
                            hMW++;
                            hCetr.MW = 1;
                            gCetr.ML = 1;

                            hCetr.PRH.Won = 1;
                            gCetr.PRH.Won = -1;

                            hCetr.PRH.NOPX = hCetr.PRH.compNOPX;
                            hCetr.PRH.Rnk = hCetr.PRH.NOPX + hCetr.PRH.prvRnk;
                            gCetr.PRH.NOPX = gCetr.PRH.compNOPX;
                            gCetr.PRH.Rnk = gCetr.PRH.NOPX + gCetr.PRH.prvRnk;
                        }
                        else
                        {
                            gMW++;
                            hCetr.ML = 1;
                            gCetr.MW = 1;

                            hCetr.PRH.Won = -1;
                            gCetr.PRH.Won = 1;

                            hCetr.PRH.NOPX = hCetr.PRH.compNOPX;
                            hCetr.PRH.Rnk = hCetr.PRH.NOPX + hCetr.PRH.prvRnk;
                            gCetr.PRH.NOPX = gCetr.PRH.compNOPX;
                            gCetr.PRH.Rnk = gCetr.PRH.NOPX + gCetr.PRH.prvRnk;
                        }
                    }

                    //var prh = Db.FromId<BDB.PRH>(hCetr.PRH.);
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

                    src.hDW = hA;
                    src.gDW = gA;

                    hCetr1.SW = (int)hA;
                    hCetr1.SL = (int)gA;
                    gCetr1.SW = (int)gA;
                    gCetr1.SL = (int)hA;

                    hCetr2.SW = (int)hA;
                    hCetr2.SL = (int)gA;
                    gCetr2.SW = (int)gA;
                    gCetr2.SL = (int)hA;

                    hCetr1.MW = 0;
                    hCetr1.ML = 0;
                    gCetr1.MW = 0;
                    gCetr1.ML = 0;
                    hCetr2.MW = 0;
                    hCetr2.ML = 0;
                    gCetr2.MW = 0;
                    gCetr2.ML = 0;
                    if ((hA + gA) > 0)  // Oynandiysa
                    {
                        if (hA > gA)
                        {
                            hMW++;
                            hCetr1.MW = 1;
                            hCetr2.MW = 1;
                            gCetr1.ML = 1;
                            gCetr2.ML = 1;
                        }
                        else
                        {
                            gMW++;
                            hCetr1.ML = 1;
                            hCetr2.ML = 1;
                            gCetr1.MW = 1;
                            gCetr2.MW = 1;
                        }
                    }
                }

                cet.hMDW = (int)hMW;
                cet.gMDW = (int)gMW;

                cet.hPW = (cet.hMSW * 2) + (cet.hMDW * 3);
                cet.gPW = (cet.gMSW * 2) + (cet.gMDW * 3);

                cet.hP = 0;
                cet.gP = 0;
                if (cet.hPW > cet.gPW)
                {
                    cet.hP = 3;
                    cet.gP = 1;
                }
                else if(cet.hPW < cet.gPW)
                {
                    cet.hP = 1;
                    cet.gP = 3;
                }
            });
            hP = cet.hP;
            gP = cet.gP;
            hPW = cet.hPW;
            gPW = cet.gPW;
            //hPSW = cet.hPSW;
            //gPSW = cet.gPSW;
            //hPDW = cet.hPDW;
            //gPDW = cet.gPDW;
            hMSW = cet.hMSW;
            gMSW = cet.gMSW;
            hMDW = cet.hMDW;
            gMDW = cet.gMDW;

        }

        [CETRinpPage_json.Singles]
        public partial class SinglesElementJson
        {
            void Handle(Input.hS1W A)
            {
                if (A.Value < 0 || A.Value > 21)
                    A.Cancel();
                else
                    this.gS1W = A.Value < 10 ? 11 : A.Value + 2;
            }

            void Handle(Input.hS2W A)
            {
                if (A.Value < 0 || A.Value > 21)
                    A.Cancel();
                else
                    this.gS2W = A.Value < 10 ? 11 : A.Value + 2;
            }

            void Handle(Input.hS3W A)
            {
                if (A.Value < 0 || A.Value > 21)
                    A.Cancel();
                else
                    this.gS3W = A.Value < 10 ? 11 : A.Value + 2;
            }

            void Handle(Input.hS4W A)
            {
                if (A.Value < 0 || A.Value > 21)
                    A.Cancel();
                else
                    this.gS4W = A.Value < 10 ? 11 : A.Value + 2;
            }

            void Handle(Input.hS5W A)
            {
                if (A.Value < 0 || A.Value > 21)
                    A.Cancel();
                else
                    this.gS5W = A.Value < 10 ? 11 : A.Value + 2;
            }

            void Handle(Input.gS1W A)
            {
                if (A.Value < 0 || A.Value > 21)
                    A.Cancel();
                else
                    this.hS1W = A.Value < 10 ? 11 : A.Value + 2;
            }

            void Handle(Input.gS2W A)
            {
                if (A.Value < 0 || A.Value > 21)
                    A.Cancel();
                else
                    this.hS2W = A.Value < 10 ? 11 : A.Value + 2;
            }

            void Handle(Input.gS3W A)
            {
                if (A.Value < 0 || A.Value > 21)
                    A.Cancel();
                else
                    this.hS3W = A.Value < 10 ? 11 : A.Value + 2;
            }

            void Handle(Input.gS4W A)
            {
                if (A.Value < 0 || A.Value > 21)
                    A.Cancel();
                else
                    this.hS4W = A.Value < 10 ? 11 : A.Value + 2;
            }

            void Handle(Input.gS5W A)
            {
                if (A.Value < 0 || A.Value > 21)
                    A.Cancel();
                else
                    this.hS5W = A.Value < 10 ? 11 : A.Value + 2;
            }
        }

        [CETRinpPage_json.Doubles]
        public partial class DoublesElementJson
        {
            void Handle(Input.hS1W A)
            {
                if (A.Value < 0 || A.Value > 21)
                    A.Cancel();
                else
                    this.gS1W = A.Value < 10 ? 11 : A.Value + 2;
            }

            void Handle(Input.hS2W A)
            {
                if (A.Value < 0 || A.Value > 21)
                    A.Cancel();
                else
                    this.gS2W = A.Value < 10 ? 11 : A.Value + 2;
            }

            void Handle(Input.hS3W A)
            {
                if (A.Value < 0 || A.Value > 21)
                    A.Cancel();
                else
                    this.gS3W = A.Value < 10 ? 11 : A.Value + 2;
            }

            void Handle(Input.hS4W A)
            {
                if (A.Value < 0 || A.Value > 21)
                    A.Cancel();
                else
                    this.gS4W = A.Value < 10 ? 11 : A.Value + 2;
            }

            void Handle(Input.hS5W A)
            {
                if (A.Value < 0 || A.Value > 21)
                    A.Cancel();
                else
                    this.gS5W = A.Value < 10 ? 11 : A.Value + 2;
            }

            void Handle(Input.gS1W A)
            {
                if (A.Value < 0 || A.Value > 21)
                    A.Cancel();
                else
                    this.hS1W = A.Value < 10 ? 11 : A.Value + 2;
            }

            void Handle(Input.gS2W A)
            {
                if (A.Value < 0 || A.Value > 21)
                    A.Cancel();
                else
                    this.hS2W = A.Value < 10 ? 11 : A.Value + 2;
            }

            void Handle(Input.gS3W A)
            {
                if (A.Value < 0 || A.Value > 21)
                    A.Cancel();
                else
                    this.hS3W = A.Value < 10 ? 11 : A.Value + 2;
            }

            void Handle(Input.gS4W A)
            {
                if (A.Value < 0 || A.Value > 21)
                    A.Cancel();
                else
                    this.hS4W = A.Value < 10 ? 11 : A.Value + 2;
            }

            void Handle(Input.gS5W A)
            {
                if (A.Value < 0 || A.Value > 21)
                    A.Cancel();
                else
                    this.hS5W = A.Value < 10 ? 11 : A.Value + 2;
            }
        }
    }
}
