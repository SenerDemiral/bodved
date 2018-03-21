using Starcounter;
using System.Linq;

namespace bodved
{

    partial class CETMpage : Json
    {
        protected override void OnData()
        {
            base.OnData();

            CreateIfEmpty();
            Read();
        }


        protected void CreateIfEmpty()
        {
            var cet = Db.FromId<BDB.CET>(ulong.Parse(CEToNo));

            if (!cet.hPok || !cet.gPok) // Oyuncu siralama 
                return;

            if (null != Db.SQL<BDB.MAC>("select m from MAC m where m.CET = ?", cet).FirstOrDefault())
                return;

            // CreateFrom CETP
            BDB.CETP hCETP, gCETP;

            string SD = "S";    // Singles
            for(int i = 1; i <= 8; i++)
            {
                hCETP = Db.SQL<BDB.CETP>("select t from CETP t where t.CET = ? and t.hCT = ? and t.SoD = ? and t.Idx = ?", cet, cet.hCT, SD, i).FirstOrDefault();
                gCETP = Db.SQL<BDB.CETP>("select t from CETP t where t.CET = ? and t.gCT = ? and t.SoD = ? and t.Idx = ?", cet, cet.gCT, SD, i).FirstOrDefault();

                new BDB.MAC
                {
                    CC = cet.CC,
                    CET = cet,
                    Trh = cet.Trh,
                    SoD = SD,
                    Idx = i,
                    hPP1 = hCETP.PP,
                    gPP1 = gCETP.PP,
                };
            }

            SD = "D";   // Doubles
            for (int i = 1; i <= 3; i++)
            {
                var hh = Db.SQL<BDB.CETP>("select t from CETP t where t.CET = ? and t.hCT = ? and t.SoD = ? and t.Idx = ?", cet, cet.hCT, SD, i).ToArray();
                var gg = Db.SQL<BDB.CETP>("select t from CETP t where t.CET = ? and t.gCT = ? and t.SoD = ? and t.Idx = ?", cet, cet.gCT, SD, i).ToArray();

                var mac = new BDB.MAC
                {
                    CC = cet.CC,
                    CET = cet,
                    Trh = cet.Trh,
                    SoD = SD,
                    Idx = i,
                    hPP1 = hh[0].PP,
                    hPP2 = hh[1].PP,
                    gPP1 = gg[0].PP,
                    gPP2 = gg[1].PP,
                };
            }

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
            //RnkID = cet.CC.RnkID;
            Cap1 = $"{cet.CCAd} [{cet.Trh:dd.MM.yy}] Müsabaka Sonuçlarý";
            Cap2 = $"{cet.hCTAd} <> {cet.gCTAd}"; // Müsabaka Sonuçlarý";

            // Singles
            SinglesElementJson sng = null;
            
            foreach (var m in Db.SQL<BDB.MAC>("select m from MAC m where m.CET = ? and m.SoD = ? order by m.Idx", cet, "S"))
            {
                sng = Singles.Add();

                sng.MACoNo = (long)m.oNo;

                sng.Idx = m.Idx;

                sng.hPP1oNo = m.hPP1?.oNo.ToString();
                sng.hPP1Ad = m.hPP1?.Ad;
                sng.hPP1Rnk = m.hpRnk;
                sng.hS1W = m.hS1W;
                sng.hS2W = m.hS2W;
                sng.hS3W = m.hS3W;
                sng.hS4W = m.hS4W;
                sng.hS5W = m.hS5W;

                sng.gPP1oNo = m.gPP1?.oNo.ToString();
                sng.gPP1Ad = m.gPP1?.Ad;
                sng.gPP1Rnk = m.gpRnk;
                sng.gS1W = m.gS1W;
                sng.gS2W = m.gS2W;
                sng.gS3W = m.gS3W;
                sng.gS4W = m.gS4W;
                sng.gS5W = m.gS5W;

                sng.S1R = BDB.H.MacSetResult(sng.hS1W, sng.gS1W);
                sng.S2R = BDB.H.MacSetResult(sng.hS2W, sng.gS2W);
                sng.S3R = BDB.H.MacSetResult(sng.hS3W, sng.gS3W);
                sng.S4R = BDB.H.MacSetResult(sng.hS4W, sng.gS4W);
                sng.S5R = BDB.H.MacSetResult(sng.hS5W, sng.gS5W);

                sng.hSW = m.hSW;    // Set Win
                sng.gSW = m.gSW;
                sng.hMW = m.hMW;    // Mac Win
                sng.gMW = m.gMW;

                sng.hPW = m.hMW * 2;    // Puan Win
                sng.gPW = m.gMW * 2;

                ShMW += sng.hMW;
                ShPW += sng.hPW;
                SgMW += sng.gMW;
                SgPW += sng.gPW;
            }

            // Doubles
            DoublesElementJson dbl = null;

            foreach (var m in Db.SQL<BDB.MAC>("select m from MAC m where m.CET = ? and m.SoD = ? order by m.Idx", cet, "D"))
            {
                dbl = Doubles.Add();

                dbl.MACoNo = (long)m.oNo;

                dbl.Idx = m.Idx;

                dbl.hPP1oNo = m.hPP1?.oNo.ToString();
                dbl.hPP1Ad = m.hPP1?.Ad;
                dbl.hPP2oNo = m.hPP2?.oNo.ToString();
                dbl.hPP2Ad = m.hPP2?.Ad;
                dbl.hS1W = m.hS1W;
                dbl.hS2W = m.hS2W;
                dbl.hS3W = m.hS3W;
                dbl.hS4W = m.hS4W;
                dbl.hS5W = m.hS5W;
                
                dbl.gPP1oNo = m.gPP1?.oNo.ToString();
                dbl.gPP1Ad = m.gPP1?.Ad;
                dbl.gPP2oNo = m.gPP2?.oNo.ToString();
                dbl.gPP2Ad = m.gPP2?.Ad;
                dbl.gS1W = m.gS1W;
                dbl.gS2W = m.gS2W;
                dbl.gS3W = m.gS3W;
                dbl.gS4W = m.gS4W;
                dbl.gS5W = m.gS5W;
                
                // Set Results h-g
                dbl.S1R = BDB.H.MacSetResult(dbl.hS1W, dbl.gS1W);
                dbl.S2R = BDB.H.MacSetResult(dbl.hS2W, dbl.gS2W);
                dbl.S3R = BDB.H.MacSetResult(dbl.hS3W, dbl.gS3W);
                dbl.S4R = BDB.H.MacSetResult(dbl.hS4W, dbl.gS4W);
                dbl.S5R = BDB.H.MacSetResult(dbl.hS5W, dbl.gS5W);

                dbl.hSW = m.hSW;    // Set Win
                dbl.gSW = m.gSW;
                dbl.hMW = m.hMW;    // Mac Win
                dbl.gMW = m.gMW;

                dbl.hPW = m.hMW * 3;    // Puan Win
                dbl.gPW = m.gMW * 3;

                DhMW += dbl.hMW;
                DhPW += dbl.hPW;
                DgMW += dbl.gMW;
                DgPW += dbl.gPW;

            }

            // Toplam
            ThPW = ShPW + DhPW;
            TgPW = SgPW + DgPW;
        }
    }
}
