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

            if (canMdfy && PPs.Count == 0)
            {
                PPsElementJson ps;
                PPs.Clear();
                //PPs.Data = Db.SQL<BDB.PP>("select p from PP p order by p.Ad");  //Cakıyor
                
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
            
            CTs.Data = Db.SQL<BDB.CT>("select c from CT c where c.CC = ? order by c.tP desc, c.fM desc, c.fMP desc", cc);

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
                        ID = MdfRec.ID,
                        K1 = MdfRec.K1oNo == "" ? null : Db.FromId<BDB.PP>(ulong.Parse(MdfRec.K1oNo)),
                        K2 = MdfRec.K2oNo == "" ? null : Db.FromId<BDB.PP>(ulong.Parse(MdfRec.K2oNo)),
                        Adres = MdfRec.Adres,
                        Pw = MdfRec.Pw 
                    };
                });
                MdfRec.oNo = 0;

                var cc = Db.FromId<BDB.CC>(ulong.Parse(CCoNo));
                CTs.Data = Db.SQL<BDB.CT>("select c from CT c where c.CC = ? order by c.aP desc, c.vP, c.Ad", cc);
                //PushChanges();

            }
        }

        void Handle(Input.UpdateTrigger Action)
        {
            if (MdfRec.oNo != 0)
            {
                Db.Transact(() =>
                {
                    var r = Db.FromId<BDB.CT>((ulong)MdfRec.oNo);
                    r.Ad = MdfRec.Ad;
                    r.ID = MdfRec.ID;
                    r.K1 = MdfRec.K1oNo == "" ? null : Db.FromId<BDB.PP>(ulong.Parse(MdfRec.K1oNo));
                    r.K2 = MdfRec.K2oNo == "" ? null : Db.FromId<BDB.PP>(ulong.Parse(MdfRec.K2oNo));
                    r.Adres = MdfRec.Adres;
                    r.Pw = MdfRec.Pw;
                });
                MdfRec.oNo = 0;

                PushChanges();
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
                            var r = Db.FromId<BDB.CT>((ulong)MdfRec.oNo);
                            r.Delete();
                        });
                    }
                });
                MdfRec.oNo = 0;

                PushChanges();
            }
        }

        public void PushChanges()
        {
            Session.ForAll((s, sId) => {
                var cp = (s.Store["bodved"] as MasterPage).CurrentPage;
                if (cp is CTpage && CCoNo == (cp as CTpage).CCoNo)
                {
                    //(s.Store["bodved"] as MasterPage).CurrentPage.Data = null;

                    //var cc = Db.FromId<BDB.CC>(ulong.Parse(CCoNo));
                    //CTs.Data = Db.SQL<BDB.CT>("select c from CT c where c.CC = ? order by c.aP desc, c.vP, c.Ad", cc);

                    s.CalculatePatchAndPushOnWebSocket();
                }
            });
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
                var p = Parent.Parent as CTpage;
                p.MdfRec.oNo = oNo;
                p.MdfRec.Ad = Ad;
                p.MdfRec.ID = ID;
                p.MdfRec.K1oNo = K1oNo.ToString();
                p.MdfRec.K2oNo = K2oNo.ToString();
                p.MdfRec.Adres = Adres;
                p.MdfRec.Pw = Pw;
            }
        }
    }
}
