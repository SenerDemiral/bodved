using Starcounter;

namespace bodved
{
    partial class CTpage : Json
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

            var cc = Db.FromId<BDB.CC>(ulong.Parse(CCoNo));
            Cap1 = $"{cc.Ad} Takımları";
            
            CTs.Data = Db.SQL<BDB.CT>("select c from CT c where c.CC = ? order by c.Ad", cc);

            //sener.NoR = DateTime.Now.Ticks;
        }

        void Handle(Input.InsertTrigger Action)
        {
            if (!string.IsNullOrWhiteSpace(MdfRec.Ad))
            {
                Db.Transact(() =>
                {
                    new BDB.CT()
                    {
                        CC = Db.FromId<BDB.CC>(ulong.Parse(CCoNo)),
                        Ad = MdfRec.Ad,
                        K1 = MdfRec.K1oNo == "" ? null : Db.FromId<BDB.PP>(ulong.Parse(MdfRec.K1oNo)),
                        K2 = MdfRec.K2oNo == "" ? null : Db.FromId<BDB.PP>(ulong.Parse(MdfRec.K2oNo)),
                        Adres = MdfRec.Adres,
                        Pw = MdfRec.Pw 
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
                    r.Adres = MdfRec.Adres;
                    r.Pw = MdfRec.Pw;
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
                p.MdfRec.Adres = this.Adres;
                p.MdfRec.Pw = Pw;
            }
        }
    }
}
