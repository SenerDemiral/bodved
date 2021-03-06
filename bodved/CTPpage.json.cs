﻿using Starcounter;
using Starcounter.Templates;
using System.Linq;

namespace bodved
{
    partial class CTPpage : Json
    {
        protected override void OnData()
        {
            base.OnData();
            var mpLgn = (Root as MasterPage).Login;
            canMdfy = mpLgn.Rl == "ADMIN" && mpLgn.LI ? true : false;

            if (canMdfy && PPs.Count == 0)
            {
                PPsElementJson ps;

                var pps = Db.SQL<BDB.PP>("select p from PP p order by p.Ad");
                foreach (var pp in pps)
                {
                    ps = PPs.Add();
                    ps.oNo = pp.oNo.ToString();
                    ps.Ad = pp.Ad;
                }
            }

            var ct = Db.FromId<BDB.CT>(ulong.Parse(CToNo));
            CCoNo = ct.CC.GetObjectNo().ToString();
            Cap1 = $"{ct.CCAd} {ct.Ad} Takım Oyuncuları";
            RnkID = ct.CC.RnkID;
            RnkAd = ct.CC.RnkAd;
            _Sra = 1;

            //CTPs.Data = Db.SQL<BDB.CTP>("select c from CTP c where c.CT = ? order by c.PPRnk desc", Db.FromId<BDB.CT>(ulong.Parse(CToNo)));
            //CTPs.Data = Db.SQL<BDB.CTP>("select c from CTP c where c.CT = ? order by c.PP.Rnk desc", Db.FromId<BDB.CT>(ulong.Parse(CToNo)));
            CTPs.Data = Db.SQL<BDB.CTP>("select c from CTP c where c.CT = ? order by c.Idx", Db.FromId<BDB.CT>(ulong.Parse(CToNo)));

            tRnk = ct.tRnk;

            //sener.NoR = DateTime.Now.Ticks;
        }

        void Handle(Input.InsertTrigger Action)
        {
            Db.Transact(() =>
            {
                new BDB.CTP()
                {
                    CC = Db.FromId<BDB.CC>(ulong.Parse(CCoNo)),
                    CT = Db.FromId<BDB.CT>(ulong.Parse(CToNo)),
                    PP = MdfRec.PPoNo == "" ? null : Db.FromId<BDB.PP>(ulong.Parse(MdfRec.PPoNo)),
                    Idx = (int)MdfRec.Idx,
                };
            });
            MdfRec.oNo = 0;
            var ct = Db.FromId<BDB.CT>(ulong.Parse(CToNo));
            BDB.H.CompCTtRnk(ct.oNo);
            _Sra = 1;
            CTPs.Data = Db.SQL<BDB.CTP>("select c from CTP c where c.CT = ? order by c.Idx", ct);
            tRnk = ct.tRnk;
        }

        void Handle(Input.UpdateTrigger Action)
        {
            var oo = Action.OldValue;
            var nn = Action.Value;

            if (MdfRec.oNo != 0)
            {
                Db.Transact(() =>
                {
                    var r = Db.FromId<BDB.CTP>((ulong)MdfRec.oNo);
                    r.PP = MdfRec.PPoNo == "" ? null : Db.FromId<BDB.PP>(ulong.Parse(MdfRec.PPoNo));
                    r.Idx = (int)MdfRec.Idx;
                });
                MdfRec.oNo = 0;
                var ct = Db.FromId<BDB.CT>(ulong.Parse(CToNo));
                BDB.H.CompCTtRnk(ct.oNo);
                _Sra = 1;
                CTPs.Data = Db.SQL<BDB.CTP>("select c from CTP c where c.CT = ? order by c.Idx", Db.FromId<BDB.CT>(ulong.Parse(CToNo)));
                tRnk = ct.tRnk;
            }
        }

        void Handle(Input.DeleteTrigger Action)
        {
            if (MdfRec.oNo != 0)
            {
                Db.Transact(() =>
                {
                    if (MdfRec.oNo != 0)
                    {
                        Db.Transact(() =>
                        {
                            var r = Db.FromId<BDB.CTP>((ulong)MdfRec.oNo);
                            r.Delete();
                        });
                    }
                });
                MdfRec.oNo = 0;
                var ct = Db.FromId<BDB.CT>(ulong.Parse(CToNo));
                BDB.H.CompCTtRnk(ct.oNo);
                _Sra = 1;
                CTPs.Data = Db.SQL<BDB.CTP>("select c from CTP c where c.CT = ? order by c.Idx", Db.FromId<BDB.CT>(ulong.Parse(CToNo)));
                tRnk = ct.tRnk;
            }
        }

        [CTPpage_json.CTPs]
        public partial class CTPsElementJson
        {
            protected override void OnData()
            {
                base.OnData();
                this.Sra = (Parent.Parent as CTPpage)._Sra++;

                //var rnkID = (Parent.Parent as CTPpage).RnkID;
                //var ppgr = Db.SQL<BDB.PPGR>("select p from PPGR p where p.PP.ObjectNo = ? and p.RnkID = ?", PPoNo, rnkID).FirstOrDefault();
                //if (ppgr != null)
                //    PPRnk2 = ppgr.Rnk;
            }

            void Handle(Input.MdfTrigger Action)
            {
                var p = this.Parent.Parent as CTPpage;
                p.MdfRec.oNo = oNo;
                p.MdfRec.Idx = Idx;
                p.MdfRec.PPoNo = this.PPoNo.ToString();
            }
        }
    }
}
