using Starcounter;

namespace bodved
{
    partial class CTPpage : Json
    {
        protected override void OnData()
        {
            base.OnData();

            var mpLgn = (Root as MasterPage).Login;
            canMdfy = mpLgn.Rl == "ADMIN" && mpLgn.LI ? true : false;

            if (canMdfy)
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

            var pt = Db.FromId<BDB.CT>(ulong.Parse(CToNo));
            CCoNo = pt.CC.GetObjectNo().ToString();
            Cap1 = $"{pt.CCAd} {pt.Ad} Takým Oyuncularý";

            CTPs.Data = Db.SQL<BDB.CTP>("select c from CTP c where c.CT = ? order by c.Idx", Db.FromId<BDB.CT>(ulong.Parse(CToNo)));

            //sener.NoR = DateTime.Now.Ticks;
        }

        void Handle(Input.InsertTrigger Action)
        {
            var oo = Action.OldValue;
            var nn = Action.Value;

            Db.Transact(() =>
            {
                new BDB.CTP()
                {
                    CC = Db.FromId<BDB.CC>(ulong.Parse(CCoNo)),
                    CT = Db.FromId<BDB.CT>(ulong.Parse(CToNo)),
                    PP = MdfRec.PPoNo == "" ? null : Db.FromId<BDB.PP>(ulong.Parse(MdfRec.PPoNo)),
                };
            });
            MdfRec.oNo = 0;
            CTPs.Data = Db.SQL<BDB.CTP>("select c from CTP c where c.CT = ?", Db.FromId<BDB.CT>(ulong.Parse(CToNo)));
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
                });
                MdfRec.oNo = 0;
                CTPs.Data = Db.SQL<BDB.CTP>("select c from CTP c where c.CT = ?", Db.FromId<BDB.CT>(ulong.Parse(CToNo)));
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
                CTPs.Data = Db.SQL<BDB.CTP>("select c from CTP c where c.CT = ?", Db.FromId<BDB.CT>(ulong.Parse(CToNo)));
            }
        }

        [CTPpage_json.CTPs]
        public partial class CTPsElementJson
        {
            void Handle(Input.MdfTrigger Action)
            {
                var p = this.Parent.Parent as CTPpage;
                p.MdfRec.oNo = this.oNo;
                p.MdfRec.PPoNo = this.PPoNo.ToString();
            }
        }
    }
}
