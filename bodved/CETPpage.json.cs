using System.Linq;
using Starcounter;

namespace bodved
{
    partial class CETPpage : Json
    {
        protected override void OnData()
        {
            base.OnData();

            if ((Root as MasterPage).Login.LI && (Root as MasterPage).Login.Id == -1)
                this.canMdfy = true;

            // Asagidaki sartlarda buraya girise izin verme
                // Bu takim'in yetkilisi Login olan kullanicisi degilse
                // Sirlama yapilip onaylamis ise
            // Yetkili isterse burada Sirlamayi bitirdigine dair onay verir.
            // Her ikisi de onay vermis ise bu tablo kullanilarak CETR olusturulur.
            // Sirlama yapilip onaylanmadan Mac sonuclari (CETR) girilemez.
            if (canMdfy)
            {
            }


            var cet = Db.FromId<BDB.CET>(ulong.Parse(CEToNo));
            var ct = Db.FromId<BDB.CT>(ulong.Parse(CToNo));     // Bu Takim'in siralamasi yapiliyor.
            CapHdr = $"{cet.Tarih} {ct.Ad}";

            var cetps = Db.SQL<BDB.CETP>("select c from CETP c where c.CET = ? and c.CT = ? order by c.Idx", cet, ct).First;
            // Kayit yoksa CTP'den alip yarat
            if (cetps == null)
            {
                var ctps = Db.SQL<BDB.CTP>("select c from CTP c where c.CT = ?", ct);
                int ix = 1;
                foreach (var src in ctps)
                {
                    Db.Transact(() =>
                    {
                        new BDB.CETP()
                        {
                            CET = cet,
                            CC = src.CC,
                            CT = src.CT,
                            PP = src.PP,
                            Idx = ix++,
                            SoD = "S",
                            HoG = "?"
                        };

                        new BDB.CETP()
                        {
                            CET = cet,
                            CC = src.CC,
                            CT = src.CT,
                            PP = src.PP,
                            Idx = ix++,
                            SoD = "D",
                            HoG = "?"
                        };
                    });
                }
            }


            // Single lari yerlestir
            SinglesElementJson s;
            int i = 1;
            var ss = Db.SQL<BDB.CETP>("select c from CETP c where c.CET = ? and c.CT = ? and c.SoD = ? order by c.Idx", cet, ct, "S");
            foreach (var p in ss)
            {
                s = Singles.Add();
                s.oNo = p.oNo.ToString();
                s.Idx = i;
                s.PPAd = p.PPAd;
                s.AoY = i < 9 ? "A" : "Y";  // Ilk 8 Asil snrakiler Yedek

                i++;
            }

            // Double lari yerlestir
            DoublesElementJson d;
            var ds = Db.SQL<BDB.CETP>("select c from CETP c where c.CET = ? and c.CT = ? and c.SoD = ? order by c.Idx", cet, ct, "D");
            var da = ds.ToArray();
            for (int c = 0; c < 6; c += 2)
            {
                d = Doubles.Add();

                d.c1.oNo = da[c].oNo.ToString();
                d.c1.PPAd = da[c].PPAd;
                d.c1.Idx = c;

                d.c2.oNo = da[c+1].oNo.ToString();
                d.c2.PPAd = da[c+1].PPAd;
                d.c2.Idx = c;
            }
            for (int c = 6; c < da.Length; c++)
            {
                d = Doubles.Add();

                d.c1.oNo = da[c].oNo.ToString();
                d.c1.PPAd = da[c].PPAd;
                d.c1.Idx = c;
            }
        }

        void Handle(Input.SaveTrigger Action)
        {
            for (int i = 0; i < Singles.Count; i++)
            {
                Db.Transact(() => {
                    var r = Db.FromId<BDB.CETP>(ulong.Parse(Singles[i].oNo));
                    r.Idx = (int)Singles[i].Idx;
                });
            }

            for (int i = 0; i < Doubles.Count; i++)
            {
                Db.Transact(() => {
                    var r = Db.FromId<BDB.CETP>(ulong.Parse(Doubles[i].c1.oNo));
                    r.Idx = (int)Doubles[i].c1.Idx;
                    if (Doubles[i].c2.oNo != "")
                    {
                        r = Db.FromId<BDB.CETP>(ulong.Parse(Doubles[i].c2.oNo));
                        r.Idx = (int)Doubles[i].c2.Idx;

                    }
                });
            }

        }

    }
}
