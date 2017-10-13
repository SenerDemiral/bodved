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

                var that = this;
                //SaveSingles(that);

                var hCetr = Db.FromId<BDB.CETR>(ulong.Parse(hoNo));
                var gCetr = Db.FromId<BDB.CETR>(ulong.Parse(goNo));
                Db.Transact(() =>
                {
                    hCetr.S1W = (int)Action.Value;
                    gCetr.S1W = (int)gS1W;
                });
                
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

                var hCetr = Db.FromId<BDB.CETR>(ulong.Parse(hoNo));
                var gCetr = Db.FromId<BDB.CETR>(ulong.Parse(goNo));

                Db.Transact(() =>
                {
                    gCetr.S1W = (int)Action.Value;
                    hCetr.S1W = (int)hS1W;
                });
            }
        }

        public void SaveSingles(SinglesElementJson e)
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
