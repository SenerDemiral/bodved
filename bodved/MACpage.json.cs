using System.Linq;
using Starcounter;
using System.Collections.Generic;
using System.Diagnostics;
using System;

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
            SoD = mac.SoD;
            if (SoD == "D")
                Dbl = true;

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
            Cap1 = $"Maç Sonuçları";
            //Cap1 = $"{cet.CCAd} [{cet.Trh:dd.MM.yy}] Müsabaka Sonuçları";
            //Cap2 = $"{cet.hCTAd} <> {cet.gCTAd}"; // Müsabaka Sonuçları";

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

            CalcSW();
            /*
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
            */
        }

        protected void Save()
        {
            Db.Transact(() =>
            {
                var mac = Db.FromId<BDB.MAC>(ulong.Parse(MACoNo));
                mac.Trh = DateTime.Parse($"{Tarih} {Saat}");
                mac.hS1W = (int)hS1W;
                mac.gS1W = (int)gS1W;
                mac.hS2W = (int)hS2W;
                mac.gS2W = (int)gS2W;
                mac.hS3W = (int)hS3W;
                mac.gS3W = (int)gS3W;
                mac.hS4W = (int)hS4W;
                mac.gS4W = (int)gS4W;
                mac.hS5W = (int)hS5W;
                mac.gS5W = (int)gS5W;
            });
        }

        public void Handle(Input.UpdateTrigger Action)
        {
            if (string.IsNullOrEmpty(MACoNo))
                return;

            Save();
        }

        private Tuple<bool, long, long> CheckInput(long val, long rakip)
        {
            bool cancel = false;
            if (val < -1)
            {
                val = -9;   // Diskalifiye, kayit edilirken (Onayla&Kaydet) Oyuncu "∞" ye cevrilecek ve 0 yapilacak
                rakip = 11;
            }
            else if (val == 0)
            {
                val = 0;
                rakip = 11;
            }
            else if (val == -1)
            {
                val = 0;
                rakip = 0;
            }
            else if (val > 21)
                cancel = true;
            else
                rakip = val < 10 ? 11 : val + 2;

            return new Tuple<bool, long, long>(cancel, val, rakip);

        }

        private void CalcSW()
        {
            hSW = 0;
            gSW = 0;
            /*
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
            */
            if (hS1W == gS1W)
            {
                hS1R = "X";
                gS1R = "X";
            }
            else if (hS1W > gS1W)
            {
                hSW++;
                hS1R = "W";
                gS1R = "L";
            }
            else
            {
                gSW++;
                hS1R = "L";
                gS1R = "W";
            }

            if (hS2W == gS2W)
            {
                hS2R = "X";
                gS2R = "X";
            }
            else if (hS2W > gS2W)
            {
                hSW++;
                hS2R = "W";
                gS2R = "L";
            }
            else
            {
                gSW++;
                hS2R = "L";
                gS2R = "W";
            }

            if (hS3W == gS3W)
            {
                hS3R = "X";
                gS3R = "X";
            }
            else if (hS3W > gS3W)
            {
                hSW++;
                hS3R = "W";
                gS3R = "L";
            }
            else
            {
                gSW++;
                hS3R = "L";
                gS3R = "W";
            }

            if (hS4W == gS4W)
            {
                hS4R = "X";
                gS4R = "X";
            }
            else if (hS4W > gS4W)
            {
                hSW++;
                hS4R = "W";
                gS4R = "L";
            }
            else
            {
                gSW++;
                hS4R = "L";
                gS4R = "W";
            }

            if (hS5W == gS5W)
            {
                hS5R = "X";
                gS5R = "X";
            }
            else if (hS5W > gS5W)
            {
                hSW++;
                hS5R = "W";
                gS5R = "L";
            }
            else
            {
                gSW++;
                hS5R = "L";
                gS5R = "W";
            }

            hSR = "X";
            gSR = "X";
            if (hSW > gSW)
            {
                hSR = "W";
                gSR = "L";
            }
            else if (hSW < gSW)
            {
                hSR = "L";
                gSR = "W";
            }
            if (hSW > 3)
                hSR = "H";
            if (gSW > 3)
                gSR = "H";
        }

        void Handle(Input.hS1W A)
        {
            Tuple<bool, long, long> snc = CheckInput(A.Value, gS1W);
            A.Cancelled = snc.Item1;
            if (!A.Cancelled)
            {
                A.Value = snc.Item2;
                hS1W = A.Value;
                gS1W = snc.Item3;
                CalcSW();
            }
        }

        void Handle(Input.hS2W A)
        {
            Tuple<bool, long, long> snc = CheckInput(A.Value, gS2W);
            A.Cancelled = snc.Item1;
            if (!A.Cancelled)
            {
                A.Value = snc.Item2;
                hS2W = A.Value;
                gS2W = snc.Item3;
                CalcSW();
            }
        }

        void Handle(Input.hS3W A)
        {
            Tuple<bool, long, long> snc = CheckInput(A.Value, gS3W);
            A.Cancelled = snc.Item1;
            if (!A.Cancelled)
            {
                A.Value = snc.Item2;
                hS3W = A.Value;
                gS3W = snc.Item3;
                CalcSW();
            }
        }

        void Handle(Input.hS4W A)
        {
            Tuple<bool, long, long> snc = CheckInput(A.Value, gS4W);
            A.Cancelled = snc.Item1;
            if (!A.Cancelled)
            {
                A.Value = snc.Item2;
                hS4W = A.Value;
                gS4W = snc.Item3;
                CalcSW();
            }
        }

        void Handle(Input.hS5W A)
        {
            Tuple<bool, long, long> snc = CheckInput(A.Value, gS5W);
            A.Cancelled = snc.Item1;
            if (!A.Cancelled)
            {
                A.Value = snc.Item2;
                hS5W = A.Value;
                gS5W = snc.Item3;
                CalcSW();
            }
        }

        void Handle(Input.gS1W A)
        {
            Tuple<bool, long, long> snc = CheckInput(A.Value, hS1W);
            A.Cancelled = snc.Item1;
            if (!A.Cancelled)
            {
                A.Value = snc.Item2;
                gS1W = A.Value;
                hS1W = snc.Item3;
                CalcSW();
            }
        }

        void Handle(Input.gS2W A)
        {
            Tuple<bool, long, long> snc = CheckInput(A.Value, hS2W);
            A.Cancelled = snc.Item1;
            if (!A.Cancelled)
            {
                A.Value = snc.Item2;
                gS2W = A.Value;
                hS2W = snc.Item3;
                CalcSW();
            }
        }

        void Handle(Input.gS3W A)
        {
            Tuple<bool, long, long> snc = CheckInput(A.Value, hS3W);
            A.Cancelled = snc.Item1;
            if (!A.Cancelled)
            {
                A.Value = snc.Item2;
                gS3W = A.Value;
                hS3W = snc.Item3;
                CalcSW();
            }
        }

        void Handle(Input.gS4W A)
        {
            Tuple<bool, long, long> snc = CheckInput(A.Value, hS4W);
            A.Cancelled = snc.Item1;
            if (!A.Cancelled)
            {
                A.Value = snc.Item2;
                gS4W = A.Value;
                hS4W = snc.Item3;
                CalcSW();
            }
        }

        void Handle(Input.gS5W A)
        {
            Tuple<bool, long, long> snc = CheckInput(A.Value, hS5W);
            A.Cancelled = snc.Item1;
            if (!A.Cancelled)
            {
                A.Value = snc.Item2;
                gS5W = A.Value;
                hS5W = snc.Item3;
                CalcSW();
            }
        }

    }
}
