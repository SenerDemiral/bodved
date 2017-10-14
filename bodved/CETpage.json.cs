using Starcounter;
using System;

namespace bodved
{
    partial class CETpage : Json
    {
        protected override void OnData()
        {
            base.OnData();

            var mpLgn = (Root as MasterPage).Login;
            canMdfy = mpLgn.Rl == "ADMIN" && mpLgn.LI ? true : false;

            var cc = Db.FromId<BDB.CC>(ulong.Parse(CCoNo));
            Cap1 = $"{cc.Ad} Müsabakalarý";

            if (canMdfy)
            {
                CTsElementJson ps;

                var pps = Db.SQL<BDB.CT>("select p from CT p where p.CC = ? order by p.Ad", cc);
                foreach (var pp in pps)
                {
                    ps = CTs.Add();
                    ps.oNo = pp.oNo.ToString();
                    ps.Ad = pp.Ad;
                }
            }


            CETs.Data = Db.SQL<BDB.CET>("select c from CET c where c.CC = ?", Db.FromId<BDB.CC>(ulong.Parse(CCoNo)));

            //sener.NoR = DateTime.Now.Ticks;
        }

        void Handle(Input.InsertTrigger Action)
        {
            var oo = Action.OldValue;
            var nn = Action.Value;

            Db.Transact(() =>
            {
                new BDB.CET()
                {
                    CC = Db.FromId<BDB.CC>(ulong.Parse(CCoNo)),
                    hCT = MdfRec.hCToNo == "" ? null : Db.FromId<BDB.CT>(ulong.Parse(MdfRec.hCToNo)),
                    gCT = MdfRec.gCToNo == "" ? null : Db.FromId<BDB.CT>(ulong.Parse(MdfRec.gCToNo)),
                    Trh = DateTime.Parse(MdfRec.Tarih)
                };
            });
            MdfRec.oNo = 0;
            CETs.Data = Db.SQL<BDB.CET>("select c from CET c where c.CC = ?", Db.FromId<BDB.CC>(ulong.Parse(CCoNo)));
        }

        void Handle(Input.UpdateTrigger Action)
        {
            var oo = Action.OldValue;
            var nn = Action.Value;

            if (MdfRec.oNo != 0)
            {
                Db.Transact(() =>
                {
                    var r = Db.FromId<BDB.CET>((ulong)MdfRec.oNo);
                    r.hCT = MdfRec.hCToNo == "" ? null : Db.FromId<BDB.CT>(ulong.Parse(MdfRec.hCToNo));
                    r.gCT = MdfRec.gCToNo == "" ? null : Db.FromId<BDB.CT>(ulong.Parse(MdfRec.gCToNo));
                    r.Trh = DateTime.Parse(MdfRec.Tarih);
                });
                MdfRec.oNo = 0;
                CETs.Data = Db.SQL<BDB.CET>("select c from CET c where c.CC = ?", Db.FromId<BDB.CC>(ulong.Parse(CCoNo)));
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
                            var r = Db.FromId<BDB.CET>((ulong)MdfRec.oNo);
                            r.Delete();
                        });
                    }
                });
                MdfRec.oNo = 0;
                CETs.Data = Db.SQL<BDB.CET>("select c from CET c where c.CC = ?", Db.FromId<BDB.CC>(ulong.Parse(CCoNo)));
            }
        }

        [CETpage_json.CETs]
        public partial class CETsElementJson
        {
            void Handle(Input.MdfTrigger Action)
            {
                var p = this.Parent.Parent as CETpage;
                p.MdfRec.oNo = this.oNo;
                p.MdfRec.hCToNo = this.hCToNo.ToString();
                p.MdfRec.gCToNo = this.gCToNo.ToString();
                p.MdfRec.Tarih = this.Tarih.Substring(0, 10);
            }
        }
    }
}
