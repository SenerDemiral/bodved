using System.Collections.Generic;
using System.Linq;
using Starcounter;

namespace bodved
{
    partial class PPRnkGrpPage : Json
    {
        protected override void OnData()
        {
            base.OnData();

            var mpLgn = (Root as MasterPage).Login;
            canMdfy = mpLgn.Rl == "ADMIN" && mpLgn.LI ? true : false;


            BDB.CC cc = null;
            if (CCoNo != 0)
            {
                cc = Db.FromId<BDB.CC>((ulong)CCoNo);
                RnkID = cc.RnkID;
                RnkAd = cc.RnkAd;
            }

            NOP = 0;
            int NOP2 = 0;
            _Sra = 1;
            /*
            //PPs.Data = Db.SQL<BDB.PPGR>("select p from PPGR p where p.RnkID = ? order by p.Sra", RnkID);
            //PPs.Data = Db.SQL<BDB.PPGR>("select p from PPGR p where p.RnkID = ? order by p.Aktif DESC, p.Rnk DESC", RnkID);
            PPs.Data = Db.SQL<BDB.PPGR>("select p from PPGR p where p.RnkID = ?", RnkID)
                .OrderByDescending(x => {
                    var p = x.PPAd;
                    var ctp = Db.SQL<BDB.CTP>("select C from CTP c where c.CC = ? and c.PP = ?", cc, x.PP).FirstOrDefault();
                    if (ctp == null)
                        return -x.Rnk;
                    NOP++;
                    return x.Rnk;
                });
            //NOP = PPs.Count;
            */
            PPsElementJson pps = null;

            Dictionary<ulong, D2> myDic2 = new Dictionary<ulong, D2>(1000);    // Initial capacity 1000 players
            var CCs = Db.SQL<BDB.CC>("select c from CC c where c.RnkID = ?", cc.RnkID);
            foreach (var c in CCs)
            {
                var CTPs = Db.SQL<BDB.CTP>("select c from CTP c where c.CC = ?", c);
                foreach (var ctp in CTPs)
                {
                    var ppgr = Db.SQL<BDB.PPGR>("select p from PPGR p where p.PP = ? and p.RnkID = ?", ctp.PP, RnkID).FirstOrDefault();
                    D2 d2 = new D2
                    {
                        Ad = ctp.PPAd,
                        Rnk = 0,
                        aO = 0,
                        vO = 0
                    };
                    if (ppgr != null)
                    {
                        d2.Rnk = ppgr.Rnk;
                        d2.aO = ppgr.aO;
                        d2.vO = ppgr.vO;
                    }
                    myDic2[ctp.PPoNo] = d2;

                }
            }
            // GrpRnk de var ama TakimOyuncuListesinde(CTP) yoksa -Rnk koy
            foreach (var a in Db.SQL<BDB.PPGR>("select p from PPGR p where p.RnkID = ?", RnkID))
            {
                if (!myDic2.ContainsKey(a.PPoNo))
                {
                    D2 d2 = new D2
                    {
                        Ad = a.PPAd,
                        Rnk = -a.Rnk,
                        aO = a.aO,
                        vO = a.vO
                    };
                    if (a.Rnk == 0)
                        d2.Rnk = -9999;

                    myDic2[a.PPoNo] = d2;
                }
            }


            //PPs.Clear();
            bool bb = false;
            bool bbia = false;

            BDB.PP pp;

            var items = from pair in myDic2
                        orderby pair.Value.Rnk descending, pair.Value.Ad
                        select pair;

            foreach (var s in items)
            {
                pp = Db.FromId<BDB.PP>(s.Key);

                if (!bbia && s.Value.Rnk == 0)
                {
                    pps = PPs.Add();
                    pps.PPAd = "";
                    pps.Sra2 = "";
                    pps.Rnk = "";
                    pps.RnkBaz = "";
                    bbia = true;
                }

                if (!bb && s.Value.Rnk < 0)
                {
                    pps = PPs.Add();
                    pps.PPAd = "";
                    pps.Sra2 = "";
                    pps.Rnk = "";
                    pps.RnkBaz = "";
                    bb = true;
                }

                pps = PPs.Add();
                //pps.oNo = pp.oNo;
                pps.PPoNo = $"{s.Key}";
                pps.PPAd = pp.Ad;
                pps.Rnk = $"{s.Value.Rnk:#}";
                pps.RnkBaz = $"{pp.RnkBaz}";
                pps.aO = $"{s.Value.aO:#}";
                pps.vO = $"{s.Value.vO:#}";
                if (pp.RnkBaz == 0)
                {
                    pps.Rnk = "";
                    pps.RnkBaz = "";
                }
                pps.Sra2 = $"{_Sra++}";

                if (s.Value.Rnk > 0)
                    NOP++;
                if (s.Value.Rnk >= 0)
                    NOP2++;

            }
            /*
            Dictionary<ulong, int> myDic = new Dictionary<ulong, int>(1000);    // Initial capacity 1000 players

            // RankID de olan TakimOyuncuListesi
            var CCs = Db.SQL<BDB.CC>("select c from CC c where c.RnkID = ?", cc.RnkID);
            foreach(var c in CCs)
            {
                var CTPs = Db.SQL<BDB.CTP>("select c from CTP c where c.CC = ?", c );
                foreach(var ctp in CTPs)
                {
                    var ppgr = Db.SQL<BDB.PPGR>("select p from PPGR p where p.PP = ? and p.RnkID = ?", ctp.PP, RnkID).FirstOrDefault();
                    if (ppgr == null)
                        myDic[ctp.PPoNo] = 0;
                    else
                        myDic[ctp.PPoNo] = ppgr.Rnk;

                }
            }

            // GrpRnk de var ama TakimOyuncuListesinde(CTP) yoksa -Rnk koy
            foreach(var a in Db.SQL<BDB.PPGR>("select p from PPGR p where p.RnkID = ?", RnkID))
            {
                if (!myDic.ContainsKey(a.PPoNo))
                {
                    if (a.Rnk == 0)
                       myDic[a.PPoNo] = -1;
                    else
                        myDic[a.PPoNo] = -a.Rnk;
                }
            }

            var sener = myDic.OrderByDescending(x => x.Value);


            foreach (var s in sener)
            {
                if (!bbia && s.Value == 0)
                {
                    pps = PPs.Add();
                    pps.PPAd = "";
                    pps.Sra2 = "";
                    pps.Rnk = "";
                    pps.RnkBaz = "";
                    bbia = true;
                }

                if (!bb && s.Value < 0)
                {
                    pps = PPs.Add();
                    pps.PPAd = "";
                    pps.Sra2 = "";
                    pps.Rnk = "";
                    pps.RnkBaz = "";
                    bb = true;
                }

                pp = Db.FromId<BDB.PP>(s.Key);
                pps = PPs.Add();
                //pps.oNo = pp.oNo;
                pps.PPoNo = $"{s.Key}";
                pps.PPAd = pp.Ad;
                pps.Rnk = $"{s.Value:#}";
                pps.RnkBaz = $"{pp.RnkBaz}";
                pps.Sra2 = $"{_Sra++}";
                if (s.Value > 0)
                    NOP++;
                if (s.Value >= 0)
                    NOP2++;

            }
            */
            Cap1 = $"{RnkAd} Oyuncuları -> Aktif: {NOP}, Kayıtlı: {NOP2}";
        }

        [PPRnkGrpPage_json.PPs]
        public partial class PPsElementJson
        {
            protected override void OnData()
            {
                base.OnData();

                //this.Sra2 = (Parent.Parent as PPRnkGrpPage)._Sra++;
                //if (!PPAd.StartsWith("∞"))
                //    (Parent.Parent as PPRnkGrpPage).NOP++;

            }
        }
    }

    public class D2
    {
        public int Rnk;
        public string Ad;
        public int aO;
        public int vO;
    }
}
