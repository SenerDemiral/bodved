using System.Linq;
using System.Collections.Generic;
using Starcounter;

namespace bodved
{
    partial class CETPpage : Json
    {
        protected override void OnData()
        {
            base.OnData();

            var cet = Db.FromId<BDB.CET>(ulong.Parse(CEToNo));

            var mpLgn = (Root as MasterPage).Login;
            canMdfy = mpLgn.Rl == "ADMIN" && mpLgn.LI ? true : false;
            if (!canMdfy && mpLgn.Rl == "TAKIM" && mpLgn.LI)
            {
                if (mpLgn.Id == CToNo)
                    canMdfy = true;
            }
            //canMdfy = true; // DENEME

            if (!canMdfy)
                return;

            // Asagidaki sartlarda buraya girise izin verme
            // Bu takim'in yetkilisi Login olan kullanicisi degilse
            // Sirlama yapilip onaylamis ise
            // Yetkili isterse burada Sirlamayi bitirdigine dair onay verir.
            // Her ikisi de onay vermis ise bu tablo kullanilarak CETR olusturulur.
            // Sirlama yapilip onaylanmadan Mac sonuclari (CETR) girilemez.

            HoG = CToNo == cet.hCToNo.ToString() ? "H" : "G";
            Pok = HoG == "H" ? cet.hPok : cet.gPok;
            if (mpLgn.Rl == "ADMIN")
                Pok = false;    // Admin sirlama OK olsa bile degistirebilsin

            var ct = Db.FromId<BDB.CT>(ulong.Parse(CToNo));     // Bu Takim'in siralamasi yapiliyor.
            Cap1 = $"{cet.CCAd} {cet.Tarih} {ct.Ad} Oyuncu Sıralama";
            Cap2 = $"{ct.Ad} Oyuncu Sıralama";

            var cetps = Db.SQL<BDB.CETP>("select c from CETP c where c.CET = ? and c.CT = ?", cet, ct).FirstOrDefault();
            // Kayit yoksa CTP'den alip yarat
            if (cetps == null)
            {
                var ctps = Db.SQL<BDB.CTP>("select c from CTP c where c.CT = ? order by c.Idx", ct);
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
                            HoG = HoG
                        };

                        new BDB.CETP()
                        {
                            CET = cet,
                            CC = src.CC,
                            CT = src.CT,
                            PP = src.PP,
                            Idx = ix++,
                            SoD = "D",
                            HoG = HoG
                        };
                    });
                }
            }
            else if (!Pok && !cet.Rok) // Yeni oyuncu eklendiyse sonuna ekle
            {
                var ctps = Db.SQL<BDB.CTP>("select c from CTP c where c.CT = ? order by c.Idx", ct);
                int ix = ctps.Count() + 1;

                foreach (var src in ctps)
                {
                    // CETP de yoksa ekle
                    var cetp = Db.SQL<BDB.CETP>("select p from CETP p where p.CT = ? and p.PP = ?", src.CT, src.PP).FirstOrDefault();
                    if (cetp == null)
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
                                HoG = HoG
                            };

                            new BDB.CETP()
                            {
                                CET = cet,
                                CC = src.CC,
                                CT = src.CT,
                                PP = src.PP,
                                Idx = ix++,
                                SoD = "D",
                                HoG = HoG
                            };
                        });
                    }
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
                d.c1.Idx = c/2+1;

                d.c2.oNo = da[c+1].oNo.ToString();
                d.c2.PPAd = da[c+1].PPAd;
                d.c2.Idx = c/2+1;
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
            Save();
        }

        void Handle(Input.SaveOkTrigger Action)
        {
            Save();

            // Siralama OK Onayla
            var cet = Db.FromId<BDB.CET>(ulong.Parse(CEToNo));
            Db.Transact(() =>
            {
                if (HoG == "H")
                    cet.hPok = true;
                else
                    cet.gPok = true;
            });

            // Her iki takim da Sirlamayi onaylamis ise CETR tablosunu yarat
            if (cet.hPok && cet.gPok)
                createCETR();

        }

        void Save()
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

        void createCETR()
        {
            var x = Db.SQL<BDB.CETR>("select c from BDB.CETR c where c.CET.ObjectNo = ?", ulong.Parse(CEToNo)).FirstOrDefault();

            if (x == null)
            {
                createCETR("H", "S");
                createCETR("H", "D");
                createCETR("G", "S");
                createCETR("G", "D");
            }
            /*
            var cet = Db.FromId<BDB.CET>(ulong.Parse(CEToNo));
            // Home/Guest Players Single/Double
            var hPS = Db.SQL<BDB.CETP>("select c from CETP c where c.CET = ? and c.CT = ? and c.SoD = ? order by c.Idx", cet, cet.hCT, "S");
            var hPD = Db.SQL<BDB.CETP>("select c from CETP c where c.CET = ? and c.CT = ? and c.SoD = ? order by c.Idx", cet, cet.hCT, "D");
            var gPS = Db.SQL<BDB.CETP>("select c from CETP c where c.CET = ? and c.CT = ? and c.SoD = ? order by c.Idx", cet, cet.gCT, "S");
            var gPD = Db.SQL<BDB.CETP>("select c from CETP c where c.CET = ? and c.CT = ? and c.SoD = ? order by c.Idx", cet, cet.gCT, "D");

            int c = 0;
            foreach (var src in hPS)
            {
                if (c > 7)
                    break;

                Db.Transact(() =>
                {
                    new BDB.CETR()
                    {
                        CET = cet,
                        CC = src.CC,
                        CT = src.CT,
                        PP = src.PP,
                        Idx = src.Idx,
                        SoD = "S",
                        HoG = "H"
                    };

                });
                c++;
            }
            */
        }

        void createCETR(string hOg, string sOd)
        {
            var cet = Db.FromId<BDB.CET>(ulong.Parse(CEToNo));
            // Home/Guest Players Single/Double
            //QueryResultRows<BDB.CETP> P;
            IEnumerable<BDB.CETP> P;
            if (hOg == "H")
                P = Db.SQL<BDB.CETP>("select c from CETP c where c.CET = ? and c.CT = ? and c.SoD = ? order by c.Idx", cet, cet.hCT, sOd);
            else
                P = Db.SQL<BDB.CETP>("select c from CETP c where c.CET = ? and c.CT = ? and c.SoD = ? order by c.Idx", cet, cet.gCT, sOd);

            int c = 0;
            foreach (var src in P)
            {
                if ((sOd == "S" && c > 7) || (sOd == "D" && c > 5))
                    break;

                Db.Transact(() =>
                {
                    var nCetr = new BDB.CETR()
                    {
                        CET = cet,
                        CC = src.CC,
                        CT = src.CT,
                        PP = src.PP,
                        Idx = src.Idx,
                        SoD = sOd,
                        HoG = hOg,
                        Trh = cet.Trh
                    };
                    if (sOd == "S")
                    {
                        var rp = Db.SQL<BDB.CETP>("select c from CETP c where c.CET = ? and c.SoD = ? and c.Idx = ? and c.HoG <> ?", cet, sOd, src.Idx, hOg).FirstOrDefault();
                        nCetr.PRH = new BDB.PRH()
                        {
                            PP = src.PP,
                            rPP = rp.PP,
                            Trh = cet.Trh,
                            Won = 0
                        };
                    }
                });
                c++;
            }
        }
    }
}
