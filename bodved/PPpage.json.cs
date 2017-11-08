using System.Linq;
using Starcounter;

namespace bodved
{
    partial class PPpage : Json
    {
        protected override void OnData()
        {
            base.OnData();

            var mpLgn = (Root as MasterPage).Login;
            canMdfy = mpLgn.Rl == "ADMIN" && mpLgn.LI ? true : false;

            PPs.Data = Db.SQL<BDB.PP>("select p from PP p order by p.Sra");
            NOP = Db.SQL<BDB.PP>("select p from PP p").Count();
            Cap1 = $"Oyuncular  [{NOP} adet]";

            //sener.NoR = DateTime.Now.Ticks;
        }

        void Handle(Input.InsertTrigger Action)
        {
            if (!string.IsNullOrWhiteSpace(MdfRec.Ad))
            {
                Db.Transact(() =>
                {
                    new BDB.PP()
                    {
                        ID = this.MdfRec.ID,
                        Ad = this.MdfRec.Ad,
                        Sex = this.MdfRec.Sex,
                        DgmYil = (int)this.MdfRec.DgmYil,
                        RnkBaz = (int)this.MdfRec.RnkBaz,
                        Tel = this.MdfRec.Tel,
                        eMail = this.MdfRec.eMail
                    };
                });
                MdfRec.oNo = 0;

                PushChanges();
            }
        }

        void Handle(Input.UpdateTrigger Action)
        {
            if (MdfRec.oNo != 0)
            {
                Db.Transact(() =>
                {
                    var r = Db.FromId<BDB.PP>((ulong)MdfRec.oNo);
                    r.ID = MdfRec.ID;
                    r.Ad = MdfRec.Ad;
                    r.Sex = this.MdfRec.Sex;
                    r.DgmYil = (int)this.MdfRec.DgmYil;
                    r.RnkBaz = (int)this.MdfRec.RnkBaz;
                    r.Tel = this.MdfRec.Tel;
                    r.eMail = this.MdfRec.eMail;
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
                            var r = Db.FromId<BDB.PP>((ulong)MdfRec.oNo);
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
                if (cp is PPpage)
                {
                    (s.Store["bodved"] as MasterPage).CurrentPage.Data = null;
                    s.CalculatePatchAndPushOnWebSocket();
                }
            });
        }

        void Handle(Input.SortAd Action)
        {
            if (Action.Value % 2 == 0)
                PPs.Data = Db.SQL<BDB.PP>("select p from PP p order by p.Ad desc");
            else
                PPs.Data = Db.SQL<BDB.PP>("select p from PP p order by p.Ad");
        }

        void Handle(Input.SortRnk Action)
        {
            if (Action.Value % 2 == 0)
                PPs.Data = Db.SQL<BDB.PP>("select p from PP p order by p.curRnk, p.Ad");
            else
                PPs.Data = Db.SQL<BDB.PP>("select p from PP p order by p.curRnk desc, p.Ad");
        }

        void Handle(Input.SortSra Action)
        {
            if (Action.Value % 2 == 0)
                PPs.Data = Db.SQL<BDB.PP>("select p from PP p order by p.Sra desc");
            else
                PPs.Data = Db.SQL<BDB.PP>("select p from PP p order by p.Sra");
        }

        [PPpage_json.PPs]
        public partial class PPsElementJson
        {
            void Handle(Input.MdfTrigger Action)
            {
                var p = this.Parent.Parent as PPpage;
                p.MdfRec.oNo = this.oNo;
                p.MdfRec.ID = this.ID;
                p.MdfRec.Ad = this.Ad;
                p.MdfRec.Sex = this.Sex;
                p.MdfRec.DgmYil = this.DgmYil;
                p.MdfRec.RnkBaz = RnkBaz;
                p.MdfRec.Tel = this.Tel;
                p.MdfRec.eMail = this.eMail;
            }
        }

    }
}
