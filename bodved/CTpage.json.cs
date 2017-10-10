using Starcounter;

namespace bodved
{
    partial class CTpage : Json
    {
        protected override void OnData()
        {
            base.OnData();

            if ((Root as MasterPage).Login.LI && (Root as MasterPage).Login.Id == -1)
                this.canMdfy = true;

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

            var cc = Db.FromId<BDB.CC>(ulong.Parse(CCoNo));
            CapHdr = $"{cc.Ad}";


            CTs.Data = Db.SQL<BDB.CT>("select c from CT c where c.CC = ? order by c.Ad", cc);

            //sener.NoR = DateTime.Now.Ticks;
        }

        void Handle(Input.InsertTrigger Action)
        {
            var oo = Action.OldValue;
            var nn = Action.Value;

            if (!string.IsNullOrWhiteSpace(MdfRec.Ad))
            {
                Db.Transact(() =>
                {
                    new BDB.CT()
                    {
                        CC = Db.FromId<BDB.CC>(ulong.Parse(CCoNo)),
                        Ad = this.MdfRec.Ad,
                        //K1 = MdfRec.K1oNo == "" ? null : Db.FromId<BDB.PP>(ulong.Parse(MdfRec.K1oNo)),
                        //K2 = MdfRec.K2oNo == "" ? null : Db.FromId<BDB.PP>(ulong.Parse(MdfRec.K2oNo))
                    };
                });
                MdfRec.oNo = 0;
                CTs.Data = Db.SQL<BDB.CT>("select c from CT c where c.CC = ? order by c.Ad", Db.FromId<BDB.CC>(ulong.Parse(CCoNo)));
            }
        }

        void Handle(Input.UpdateTrigger Action)
        {
            var oo = Action.OldValue;
            var nn = Action.Value;

            if (MdfRec.oNo != 0)
            {
                Db.Transact(() =>
                {
                    var r = Db.FromId<BDB.CT>((ulong)MdfRec.oNo);
                    r.Ad = MdfRec.Ad;
                    r.K1 = MdfRec.K1oNo == "" ? null : Db.FromId<BDB.PP>(ulong.Parse(MdfRec.K1oNo));
                    r.K2 = MdfRec.K2oNo == "" ? null : Db.FromId<BDB.PP>(ulong.Parse(MdfRec.K2oNo));
                });
                MdfRec.oNo = 0;
                CTs.Data = Db.SQL<BDB.CT>("select c from CT c where c.CC = ? order by c.Ad", Db.FromId<BDB.CC>(ulong.Parse(CCoNo)));
            }
        }

        void Handle(Input.DeleteTrigger Action)
        {
            var oo = Action.OldValue;
            var nn = Action.Value;

            if (MdfRec.oNo != 0)
            {
                Db.Transact(() =>
                {
                    if (MdfRec.oNo != 0)
                    {
                        Db.Transact(() =>
                        {
                            var r = Db.FromId<BDB.CT>((ulong)MdfRec.oNo);
                            r.Delete();
                        });
                    }
                });
                MdfRec.oNo = 0;
                CTs.Data = Db.SQL<BDB.CT>("select c from CT c where c.CC = ? order by c.Ad", Db.FromId<BDB.CC>(ulong.Parse(CCoNo)));
            }
        }

        [CTpage_json.CTs]
        public partial class CTsElementJson
        {
            protected override void OnData()
            {
                base.OnData();
            }

            void Handle(Input.MdfTrigger Action)
            {
                var p = this.Parent.Parent as CTpage;
                p.MdfRec.oNo = this.oNo;
                p.MdfRec.Ad = this.Ad;
                p.MdfRec.K1oNo = this.K1oNo.ToString();
                p.MdfRec.K2oNo = this.K2oNo.ToString();
            }
        }
    }
}
