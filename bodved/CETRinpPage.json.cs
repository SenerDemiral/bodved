using System.Linq;
using Starcounter;
using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace bodved
{
    partial class CETRinpPage : Json
    {
        protected override void OnData()
        {
            base.OnData();

            Read();
        }
        
        public void Handle(Input.SaveTrigger Action)
        {
            Save(false);

            PushChanges();
        }

        public void Handle(Input.SaveOkTrigger Action)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            Save(true);

            // Sonuc OK Onayla
            var cet = Db.FromId<BDB.CET>(ulong.Parse(CEToNo));
            Db.Transact(() =>
            {
                cet.Rok = true;
            });

            BDB.H.UpdPPLigMacSay(ulong.Parse(CEToNo));  // RefreshRH dan once gelmeli
            BDB.H.RefreshRH();  // Global Rank

            //BDB.H.RefreshRH4(cet.Trh);  // Global Rank DENEME OK RefreshRH dan 2x daha hizli
            //BDB.H.RefreshRH4(DateTime.MinValue);  // Global Rank DENEME OK RefreshRH dan daha hizli

            BDB.H.RefreshRH2(ulong.Parse(CCoNo));  // RnkGrp Rank
            //BDB.H.RefreshRH24(ulong.Parse(CCoNo));  // RnkGrp Rank DENEME
            BDB.H.UpdCTsum(cet.hCT.oNo);
            BDB.H.UpdCTsum(cet.gCT.oNo);

            //BDB.H.BackupCET(cet.CC.ID, cet.ID); // CETP, CETR yedegi. \Starcounter\BodVedData\Ydk-ccID-cetID.txt

            PushChangesCT();

            watch.Stop();
            Console.WriteLine($"CETR{CEToNo} SaveOkTrigger: {watch.ElapsedMilliseconds} msec  {watch.ElapsedTicks} ticks");

        }

        protected void Read()
        {
            var cet = Db.FromId<BDB.CET>(ulong.Parse(CEToNo));
            CCoNo = cet.CCoNo.ToString();
            hCToNo = (long)cet.hCToNo;
            gCToNo = (long)cet.gCToNo;
            hCTAd = cet.hCTAd;
            gCTAd = cet.gCTAd;
            Rok = cet.Rok;
            Info = cet.Info;
            RnkID = cet.CC.RnkID;

            var mpLgn = (Root as MasterPage).Login;
            canMdfy = mpLgn.Rl == "ADMIN" && mpLgn.LI ? true : false;
            if (!canMdfy)
            {
                if (mpLgn.Rl == "TAKIM")
                    if (mpLgn.LI && (mpLgn.Id == cet.hCToNo.ToString() || mpLgn.Id == cet.gCToNo.ToString()))
                        canMdfy = true;
            }

            //canMdfy = true; //deneme
            if (mpLgn.Rl == "ADMIN")
                Rok = false;    // Admin sonuc OK olsa bile degistirebilsin



            Cap1 = $"{cet.CCAd} [{cet.Tarih}] Müsabaka Sonuçları";
            Cap2 = $"{cet.hCTAd} <> {cet.gCTAd}"; // Müsabaka Sonuçları";

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

            Singles.Clear();
            Doubles.Clear();

            //var SR = Db.SQL<BDB.CETR>("select c from CETR c where c.CET = ? and c.SoD = ? order by c.Idx, c.HoG desc", cet, "S");
            IEnumerable<BDB.CETR> SR = Db.SQL<BDB.CETR>("select c from CETR c where c.CET = ? and c.SoD = ? order by c.Idx, c.HoG desc", cet, "S");

            SinglesElementJson sng = null;

            int s = 0;
            foreach (var src in SR)
            {
                if ((s % 2) == 0)   // Home
                {
                    sng = Singles.Add();
                    sng.Idx = src.Idx;

                    sng.hoNo = src.oNo.ToString();
                    sng.hPPoNo = src.PP?.oNo.ToString();
                    sng.hPPAd = src.PP?.Ad;
                    //sng.hPPrnk = src.PRH.pRnk; //prvRnk;
                    
                    sng.hPPrnk = src.RH.hpRnk2; // grup prvRnk;
                    sng.hS1W = src.S1W; // src.S1W < 0 ? "" : src.S1W.ToString();
                    sng.hS2W = src.S2W;
                    sng.hS3W = src.S3W;
                    sng.hS4W = src.S4W;
                    sng.hS5W = src.S5W;
                    sng.hSW = src.SW;
                }
                if ((s % 2) == 1)   // Guest
                {
                    sng.goNo = src.oNo.ToString();
                    sng.gPPoNo = src.PP?.oNo.ToString();
                    sng.gPPAd = src.PPAd;
                    //sng.gPPrnk = src.PRH.pRnk;  //prvRnk;
                    sng.gPPrnk = src.RH.gpRnk2;  //grup prvRnk;

                    sng.gS1W = src.S1W; // src.S1W < 0 ? "" : src.S1W.ToString();
                    sng.gS2W = src.S2W;
                    sng.gS3W = src.S3W;
                    sng.gS4W = src.S4W;
                    sng.gS5W = src.S5W;
                    sng.gSW = src.SW;

                    sng.hgS1W = BDB.H.MacSetResult(sng.hS1W, sng.gS1W);
                    sng.hgS2W = BDB.H.MacSetResult(sng.hS2W, sng.gS2W);
                    sng.hgS3W = BDB.H.MacSetResult(sng.hS3W, sng.gS3W);
                    sng.hgS4W = BDB.H.MacSetResult(sng.hS4W, sng.gS4W);
                    sng.hgS5W = BDB.H.MacSetResult(sng.hS5W, sng.gS5W);
                    /*
                    if (sng.hS1W == 0 && sng.gS1W == 0)
                        sng.hgS1W = "";
                    else
                        sng.hgS1W = BDB.H.MacSetResult(sng.hS1W, sng.gS1W); //$"{sng.hS1W}-{sng.gS1W}";
                    if (sng.hS2W == 0 && sng.gS2W == 0)
                        sng.hgS2W = "";
                    else
                        sng.hgS2W = $"{sng.hS2W}-{sng.gS2W}";
                    if (sng.hS3W == 0 && sng.gS3W == 0)
                        sng.hgS3W = "";
                    else
                        sng.hgS3W = $"{sng.hS3W}-{sng.gS3W}";
                    if (sng.hS4W == 0 && sng.gS4W == 0)
                        sng.hgS4W = "";
                    else
                        sng.hgS4W = $"{sng.hS4W}-{sng.gS4W}";
                    if (sng.hS5W == 0 && sng.gS5W == 0)
                        sng.hgS5W = "";
                    else
                        sng.hgS5W = $"{sng.hS5W}-{sng.gS5W}";
                        */
                }

                s++;
            }

            var DR = Db.SQL<BDB.CETR>("select c from CETR c where c.CET = ? and c.SoD = ? order by c.Idx, c.HoG desc", cet, "D");
            DoublesElementJson dbl = null;
            int c = 0;
            foreach (var src in DR)
            {
                if ((c % 4) == 0)
                {
                    dbl = Doubles.Add();
                    dbl.Idx = src.Idx;

                    dbl.hoNo1 = src.oNo.ToString();
                    dbl.hPPoNo1 = src.PP?.oNo.ToString();
                    dbl.hPPAd1 = src.PPAd;
                    dbl.hPPrnk1 = BDB.H.PPprvRnk(src.PP.oNo, cet.Trh);
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
                    dbl.hPPoNo2 = src.PP?.oNo.ToString();
                    dbl.hPPAd2 = src.PPAd;
                    dbl.hPPrnk2 = BDB.H.PPprvRnk(src.PP.oNo, cet.Trh);
                }
                if ((c % 4) == 2)
                {
                    dbl.goNo1 = src.oNo.ToString();
                    dbl.gPPoNo1 = src.PP?.oNo.ToString();
                    dbl.gPPAd1 = src.PPAd;
                    dbl.gPPrnk1 = BDB.H.PPprvRnk(src.PP.oNo, cet.Trh);
                    dbl.gS1W = src.S1W;
                    dbl.gS2W = src.S2W;
                    dbl.gS3W = src.S3W;
                    dbl.gS4W = src.S4W;
                    dbl.gS5W = src.S5W;
                    dbl.gDW = src.SW;

                    dbl.hgS1W = BDB.H.MacSetResult(dbl.hS1W, dbl.gS1W);
                    dbl.hgS2W = BDB.H.MacSetResult(dbl.hS2W, dbl.gS2W);
                    dbl.hgS3W = BDB.H.MacSetResult(dbl.hS3W, dbl.gS3W);
                    dbl.hgS4W = BDB.H.MacSetResult(dbl.hS4W, dbl.gS4W);
                    dbl.hgS5W = BDB.H.MacSetResult(dbl.hS5W, dbl.gS5W);
                    /*
                    if (dbl.hS1W == 0 && dbl.gS1W == 0)
                        dbl.hgS1W = "";
                    else
                        dbl.hgS1W = $"{dbl.hS1W}-{dbl.gS1W}";
                    if (dbl.hS2W == 0 && dbl.gS2W == 0)
                        dbl.hgS2W = "";
                    else
                        dbl.hgS2W = $"{dbl.hS2W}-{dbl.gS2W}";
                    if (dbl.hS3W == 0 && dbl.gS3W == 0)
                        dbl.hgS3W = "";
                    else
                        dbl.hgS3W = $"{dbl.hS3W}-{dbl.gS3W}";
                    if (dbl.hS4W == 0 && dbl.gS4W == 0)
                        dbl.hgS4W = "";
                    else
                        dbl.hgS4W = $"{dbl.hS4W}-{dbl.gS4W}";
                    if (dbl.hS5W == 0 && dbl.gS5W == 0)
                        dbl.hgS5W = "";
                    else
                        dbl.hgS5W = $"{dbl.hS5W}-{dbl.gS5W}";*/
                }
                if ((c % 4) == 3)
                {
                    dbl.goNo2 = src.oNo.ToString();
                    dbl.gPPoNo2 = src.PP?.oNo.ToString();
                    dbl.gPPAd2 = src.PPAd;
                    dbl.gPPrnk2 = BDB.H.PPprvRnk(src.PP.oNo, cet.Trh);
                }

                c++;
            }
        }

        public void PushChanges()
        {
            //Read(); // Her bir Seesion icin Data=null yapilarak zaten okunuyor

            //var csId = Session.Current.SessionId;
            // var cscpData = (Session.Current.Store["bodved"] as MasterPage).CurrentPage.Data; Hep Null geliyor

            Session.RunTaskForAll((s, sId) => {
                var cp = (s.Store["bodved"] as MasterPage).CurrentPage;
                // var xx = s.Store["bodved"].Data;  Hep Null geliyor???
                if (cp is CETRinpPage && CEToNo == (cp as CETRinpPage).CEToNo) // && csId != sId)
                {
                    // Session.Current.SessionId ve sId burda ayni oluyor ???

                    // trick to invoke OnData. Null olunca OnData call ediliyor
                    (s.Store["bodved"] as MasterPage).CurrentPage.Data = null; // cscpData;

                    s.CalculatePatchAndPushOnWebSocket();
                }
            });
        }

        public void PushChangesCT()
        {
            var csId = Session.Current.SessionId;
            Session.RunTaskForAll((s, sId) => {
                var cp = (s.Store["bodved"] as MasterPage).CurrentPage;
                if (cp is CTpage && CCoNo == (cp as CTpage).CCoNo && csId != sId)
                {
                    (s.Store["bodved"] as MasterPage).CurrentPage.Data = null;
                    s.CalculatePatchAndPushOnWebSocket();
                }
            });
        }

        protected void Save(bool OK)
        {
            var cet = Db.FromId<BDB.CET>(ulong.Parse(CEToNo));
            var dkPP = Db.SQL<BDB.PP>("select p from PP p where p.Ad like ?", "∞%").FirstOrDefault();
            Db.Transact(() =>
            {
                cet.Info = Info;

                long hA, gA, hMW = 0, gMW = 0, hSW = 0, gSW = 0;
                int hSP = 0, gSP = 0;

                foreach (var src in Singles)
                {
                    var hR = Db.FromId<BDB.CETR>(ulong.Parse(src.hoNo));
                    var gR = Db.FromId<BDB.CETR>(ulong.Parse(src.goNo));

                    hR.S1W = (int)src.hS1W;
                    hR.S2W = (int)src.hS2W;
                    hR.S3W = (int)src.hS3W;
                    hR.S4W = (int)src.hS4W;
                    hR.S5W = (int)src.hS5W;

                    gR.S1W = (int)src.gS1W;
                    gR.S2W = (int)src.gS2W;
                    gR.S3W = (int)src.gS3W;
                    gR.S4W = (int)src.gS4W;
                    gR.S5W = (int)src.gS5W;

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

                    if (src.hS1W == -9 || hR.PP.Ad.StartsWith("∞")) // HomePlyr diskalifiye
                    {
                        hA = 0;
                        gA = 3;
                        hR.PP = dkPP;
                        hR.RH.hPP = dkPP;

                        hR.S1W = 0;
                        hR.S2W = 0;
                        hR.S3W = 0;
                        hR.S4W = 0;
                        hR.S5W = 0;
                        gR.S1W = 11;
                        gR.S2W = 11;
                        gR.S3W = 11;
                        gR.S4W = 0;
                        gR.S5W = 0;
                    }
                    if (src.gS1W == -9 || gR.PP.Ad.StartsWith("∞")) // GuestPlyr diskalifiye
                    {
                        hA = 3;
                        gA = 0;
                        gR.PP = dkPP;
                        gR.RH.gPP = dkPP;

                        hR.S1W = 11;
                        hR.S2W = 11;
                        hR.S3W = 11;
                        hR.S4W = 0;
                        hR.S5W = 0;
                        gR.S1W = 0;
                        gR.S2W = 0;
                        gR.S3W = 0;
                        gR.S4W = 0;
                        gR.S5W = 0;
                    }

                    hR.SW = (int)hA;
                    hR.SL = (int)gA;
                    gR.SW = (int)gA;
                    gR.SL = (int)hA;

                    src.hSW = hA;
                    src.gSW = gA;

                    hR.MW = 0;
                    hR.ML = 0;
                    gR.MW = 0;
                    gR.ML = 0;
                    //if ((hA + gA) > 0)  // Oynandiysa
                    {
                        if (hA == gA)
                        {
                            hR.MW = 0;
                            gR.ML = 0;
                            hR.RH.hWon = 0;
                            gR.RH.gWon = 0;
                            //hSP += 1;
                            //gSP += 1;

                        }
                        else if (hA > gA)
                        {
                            hMW++;
                            hR.MW = 1;
                            gR.ML = 1;

                            hR.RH.hWon = 1;
                            gR.RH.gWon = -1;

                            hSP += 2;
                        }
                        else
                        {
                            gMW++;
                            hR.ML = 1;
                            gR.MW = 1;

                            hR.RH.hWon = -1;
                            gR.RH.gWon = 1;

                            gSP += 2;
                        }
                    }
                    hSW += hA;
                    gSW += gA;
                    //var prh = Db.FromId<BDB.PRH>(hR.PRH.);
                }
                cet.hMSW = (int)hMW;
                cet.gMSW = (int)gMW;
                cet.hSSW = (int)hSW;
                cet.gSSW = (int)gSW;

                hMW = 0;
                gMW = 0;
                hSW = 0;
                gSW = 0;
                foreach (var src in Doubles)
                {
                    var hR1 = Db.FromId<BDB.CETR>(ulong.Parse(src.hoNo1));
                    var hR2 = Db.FromId<BDB.CETR>(ulong.Parse(src.hoNo2));
                    var gR1 = Db.FromId<BDB.CETR>(ulong.Parse(src.goNo1));
                    var gR2 = Db.FromId<BDB.CETR>(ulong.Parse(src.goNo2));

                    hR1.S1W = (int)src.hS1W;
                    hR1.S2W = (int)src.hS2W;
                    hR1.S3W = (int)src.hS3W;
                    hR1.S4W = (int)src.hS4W;
                    hR1.S5W = (int)src.hS5W;

                    hR2.S1W = (int)src.hS1W;
                    hR2.S2W = (int)src.hS2W;
                    hR2.S3W = (int)src.hS3W;
                    hR2.S4W = (int)src.hS4W;
                    hR2.S5W = (int)src.hS5W;

                    gR1.S1W = (int)src.gS1W;
                    gR1.S2W = (int)src.gS2W;
                    gR1.S3W = (int)src.gS3W;
                    gR1.S4W = (int)src.gS4W;
                    gR1.S5W = (int)src.gS5W;

                    gR2.S1W = (int)src.gS1W;
                    gR2.S2W = (int)src.gS2W;
                    gR2.S3W = (int)src.gS3W;
                    gR2.S4W = (int)src.gS4W;
                    gR2.S5W = (int)src.gS5W;

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

                    if (src.hS1W == -9 || hR1.PP.Ad.StartsWith("∞")) // HomePlyrS diskalifiye
                    {
                        hA = 0;
                        gA = 2;
                        hR1.PP = dkPP;
                        hR2.PP = dkPP;

                        hR1.S1W = 0;
                        hR1.S2W = 0;
                        hR1.S3W = 0;
                        hR1.S4W = 0;
                        hR1.S5W = 0;
                        hR2.S1W = 0;
                        hR2.S2W = 0;
                        hR2.S3W = 0;
                        hR2.S4W = 0;
                        hR2.S5W = 0;
                        gR1.S1W = 11;
                        gR1.S2W = 11;
                        gR1.S3W = 11;
                        gR1.S4W = 0;
                        gR1.S5W = 0;
                        gR2.S1W = 11;
                        gR2.S2W = 11;
                        gR2.S3W = 11;
                        gR2.S4W = 0;
                        gR2.S5W = 0;
                    }
                    if (src.gS1W == -9 || gR1.PP.Ad.StartsWith("∞")) // GuestPlyrS diskalifiye
                    {
                        hA = 2;
                        gA = 0;
                        gR1.PP = dkPP;
                        gR2.PP = dkPP;

                        hR1.S1W = 11;
                        hR1.S2W = 11;
                        hR1.S3W = 11;
                        hR1.S4W = 0;
                        hR1.S5W = 0;
                        hR2.S1W = 11;
                        hR2.S2W = 11;
                        hR2.S3W = 11;
                        hR2.S4W = 0;
                        hR2.S5W = 0;
                        gR1.S1W = 0;
                        gR1.S2W = 0;
                        gR1.S3W = 0;
                        gR1.S4W = 0;
                        gR1.S5W = 0;
                        gR2.S1W = 0;
                        gR2.S2W = 0;
                        gR2.S3W = 0;
                        gR2.S4W = 0;
                        gR2.S5W = 0;
                    }
                    src.hDW = hA;
                    src.gDW = gA;

                    hR1.SW = (int)hA;
                    hR1.SL = (int)gA;
                    gR1.SW = (int)gA;
                    gR1.SL = (int)hA;

                    hR2.SW = (int)hA;
                    hR2.SL = (int)gA;
                    gR2.SW = (int)gA;
                    gR2.SL = (int)hA;

                    hR1.MW = 0;
                    hR1.ML = 0;
                    gR1.MW = 0;
                    gR1.ML = 0;
                    hR2.MW = 0;
                    hR2.ML = 0;
                    gR2.MW = 0;
                    gR2.ML = 0;
                    if ((hA + gA) > 0)  // Oynandiysa
                    {
                        if (hA > gA)
                        {
                            hMW++;
                            hR1.MW = 1;
                            hR2.MW = 1;
                            gR1.ML = 1;
                            gR2.ML = 1;
                        }
                        else
                        {
                            gMW++;
                            hR1.ML = 1;
                            hR2.ML = 1;
                            gR1.MW = 1;
                            gR2.MW = 1;
                        }
                    }
                    hSW += hA;
                    gSW += gA;
                }

                cet.hMDW = (int)hMW;
                cet.gMDW = (int)gMW;
                cet.hSDW = (int)hSW;
                cet.gSDW = (int)gSW;


                cet.hPW = (cet.hMSW * 2) + (cet.hMDW * 3);
                cet.gPW = (cet.gMSW * 2) + (cet.gMDW * 3);
                //cet.hPW = hSP + (cet.hMDW * 3);
                //cet.gPW = gSP + (cet.gMDW * 3);

                cet.hP = 0;
                cet.gP = 0;
                if (cet.hPW > cet.gPW)
                {
                    cet.hP = 2;
                    cet.gP = 1;
                }
                else if(cet.hPW < cet.gPW)
                {
                    cet.hP = 1;
                    cet.gP = 2;
                }
            });
        }

        #region InputDialog
        void Handle(Input.DlgRejectTrigger A)
        {
            DlgOpened = false;
        }

        void Handle(Input.DlgSaveTrigger A)
        {
            DlgOpened = false;
            int i = (int)Idx - 1;

            if (SoD == "S")
            {
                Singles[i].hS1W = hS1W;
                Singles[i].hS2W = hS2W;
                Singles[i].hS3W = hS3W;
                Singles[i].hS4W = hS4W;
                Singles[i].hS5W = hS5W;
                Singles[i].gS1W = gS1W;
                Singles[i].gS2W = gS2W;
                Singles[i].gS3W = gS3W;
                Singles[i].gS4W = gS4W;
                Singles[i].gS5W = gS5W;
            }
            else if (SoD == "D")
            {
                Doubles[i].hS1W = hS1W;
                Doubles[i].hS2W = hS2W;
                Doubles[i].hS3W = hS3W;
                Doubles[i].hS4W = hS4W;
                Doubles[i].hS5W = hS5W;
                Doubles[i].gS1W = gS1W;
                Doubles[i].gS2W = gS2W;
                Doubles[i].gS3W = gS3W;
                Doubles[i].gS4W = gS4W;
                Doubles[i].gS5W = gS5W;
            }
            Save(false);

            PushChanges();
        }

        void Handle(Input.hS1W A)
        {
            if (A.Value < -1)
            {
                A.Value = -9;   // Diskalifiye, kayit edilirken (Onayla&Kaydet) Oyuncu "∞" ye cevrilecek ve 0 yapilacak
                gS1W = 11;
            }
            else if (A.Value == -1)
            {
                A.Value = 0;
                gS1W = 11;
            }
            else if (A.Value == 0)
            {
                A.Value = 0;
                gS1W = 0;
            }
            else if (A.Value > 21)
                A.Cancel();
            else
                this.gS1W = A.Value < 10 ? 11 : A.Value + 2;
        }

        void Handle(Input.hS2W A)
        {
            if (A.Value == -1)
            {
                A.Value = 0;
                gS2W = 11;
            }
            else if (A.Value == 0)
            {
                A.Value = 0;
                gS2W = 0;
            }
            else if (A.Value > 21)
                A.Cancel();
            else
                this.gS2W = A.Value < 10 ? 11 : A.Value + 2;
        }

        void Handle(Input.hS3W A)
        {
            if (A.Value == -1)
            {
                A.Value = 0;
                gS3W = 11;
            }
            else if (A.Value == 0)
            {
                A.Value = 0;
                gS3W = 0;
            }
            else if (A.Value > 21)
                A.Cancel();
            else
                this.gS3W = A.Value < 10 ? 11 : A.Value + 2;
        }

        void Handle(Input.hS4W A)
        {
            if (A.Value == -1)
            {
                A.Value = 0;
                gS4W = 11;
            }
            else if (A.Value == 0)
            {
                A.Value = 0;
                gS4W = 0;
            }
            else if (A.Value > 21)
                A.Cancel();
            else
                this.gS4W = A.Value < 10 ? 11 : A.Value + 2;
        }

        void Handle(Input.hS5W A)
        {
            if (A.Value == -1)
            {
                A.Value = 0;
                gS5W = 11;
            }
            else if (A.Value == 0)
            {
                A.Value = 0;
                gS5W = 0;
            }
            else if (A.Value > 21)
                A.Cancel();
            else
                this.gS5W = A.Value < 10 ? 11 : A.Value + 2;
        }

        void Handle(Input.gS1W A)
        {
            if (A.Value < -1)
            {
                A.Value = -9;   // Diskalifiye, kayit edilirken (Onayla&Kaydet) Oyuncu "∞" ye cevrilecek ve 0 yapilacak
                hS1W = 11;
            }
            else if (A.Value == -1)
            {
                A.Value = 0;
                hS1W = 11;
            }
            else if (A.Value == 0)
            {
                A.Value = 0;
                gS1W = 0;
            }
            else if (A.Value > 21)
                A.Cancel();
            else
                this.hS1W = A.Value < 10 ? 11 : A.Value + 2;
        }

        void Handle(Input.gS2W A)
        {
            if (A.Value == -1)
            {
                A.Value = 0;
                hS2W = 11;
            }
            else if (A.Value == 0)
            {
                A.Value = 0;
                hS2W = 0;
            }
            else if (A.Value > 21)
                A.Cancel();
            else
                this.hS2W = A.Value < 10 ? 11 : A.Value + 2;
        }

        void Handle(Input.gS3W A)
        {
            if (A.Value == -1)
            {
                A.Value = 0;
                hS3W = 11;
            }
            else if (A.Value == 0)
            {
                A.Value = 0;
                hS3W = 0;
            }
            else if (A.Value > 21)
                A.Cancel();
            else
                this.hS3W = A.Value < 10 ? 11 : A.Value + 2;
        }

        void Handle(Input.gS4W A)
        {
            if (A.Value == -1)
            {
                A.Value = 0;
                hS4W = 11;
            }
            else if (A.Value == 0)
            {
                A.Value = 0;
                hS4W = 0;
            }
            else if (A.Value > 21)
                A.Cancel();
            else
                this.hS4W = A.Value < 10 ? 11 : A.Value + 2;
        }

        void Handle(Input.gS5W A)
        {
            if (A.Value == -1)
            {
                A.Value = 0;
                hS5W = 11;
            }
            else if (A.Value == 0)
            {
                A.Value = 0;
                hS5W = 0;
            }
            else if (A.Value > 21)
                A.Cancel();
            else
                this.hS5W = A.Value < 10 ? 11 : A.Value + 2;
        }

        #endregion InputDialog

        [CETRinpPage_json.Singles]
        public partial class SinglesElementJson
        {
            void Handle(Input.MdfTrigger A)
            {
                var p = this.Parent.Parent as CETRinpPage;

                p.hPPAd1 = hPPAd;
                p.hPPAd2 = "";
                p.gPPAd1 = gPPAd;
                p.gPPAd2 = "";

                p.SoD = "S";
                p.Idx = Idx;
                p.hS1W = hS1W;
                p.hS2W = hS2W;
                p.hS3W = hS3W;
                p.hS4W = hS4W;
                p.hS5W = hS5W;
                p.gS1W = gS1W;
                p.gS2W = gS2W;
                p.gS3W = gS3W;
                p.gS4W = gS4W;
                p.gS5W = gS5W;

                p.DlgOpened = true;
            }
        }

        [CETRinpPage_json.Doubles]
        public partial class DoublesElementJson
        {
            void Handle(Input.MdfTrigger A)
            {
                var p = this.Parent.Parent as CETRinpPage;

                p.hPPAd1 = hPPAd1;
                p.hPPAd2 = hPPAd2;
                p.gPPAd1 = gPPAd1;
                p.gPPAd2 = gPPAd2;
                p.SoD = "D";
                p.Idx = Idx;
                p.hS1W = hS1W;
                p.hS2W = hS2W;
                p.hS3W = hS3W;
                p.hS4W = hS4W;
                p.hS5W = hS5W;
                p.gS1W = gS1W;
                p.gS2W = gS2W;
                p.gS3W = gS3W;
                p.gS4W = gS4W;
                p.gS5W = gS5W;

                p.DlgOpened = true;
            }
        }
    }
}
