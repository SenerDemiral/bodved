using System;
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
            //NOP = Db.SQL<BDB.PP>("select p from PP p").Count();
            NOP = PPs.Count;

            Cap1 = $"Oyuncu sayısı : {NOP-1}";

            //sener.NoR = DateTime.Now.Ticks;
        }

        void Handle(Input.DlgRejectTrigger A)
        {
            DlgOpened = false;
        }

        void Handle(Input.DlgInsertTrigger Action)
        {
            if (!string.IsNullOrWhiteSpace(MdfRec.Ad))
            {
                Db.Transact(() =>
                {
                    var nPP = new BDB.PP()
                    {
                        PK = BDB.H.GEN_ID(),
                        Ad = this.MdfRec.Ad,
                        Sex = this.MdfRec.Sex,
                        DgmYil = (int)this.MdfRec.DgmYil,
                        RnkBaz = (int)this.MdfRec.RnkBaz,
                        Tel = this.MdfRec.Tel,
                        eMail = this.MdfRec.eMail
                    };
                });
                MdfRec.oNo = 0;

                BDB.H.RefreshRH();
                PushChanges();
            }
            DlgOpened = false;
        }

        void Handle(Input.DlgUpdateTrigger Action)
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
                BDB.H.RefreshRH();
                BDB.H.RefreshRH4(DateTime.MinValue);
                PushChanges();
            }
            DlgOpened = false;
        }

        void Handle(Input.DlgDeleteTrigger Action)
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
            DlgOpened = false;
        }

        public void PushChanges()
        {
            Session.RunTaskForAll((s, sId) => {
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
            PPs.Data = Db.SQL<BDB.PP>("select p from PP p order by p.Ad");
        }

        void Handle(Input.SortRnk Action)
        {
            if (Action.Value % 2 == 0)
                PPs.Data = Db.SQL<BDB.PP>("select p from PP p order by p.curRnk, p.Ad");
        }

        void Handle(Input.SortSra Action)
        {
            PPs.Data = Db.SQL<BDB.PP>("select p from PP p order by p.Sra");
        }

        void Handle(Input.SortLTC Action)
        {
            PPs.Data = Db.SQL<BDB.PP>("select p from PP p order by p.LTC desc");
        }

        void Handle(Input.SortL1C Action)
        {
            PPs.Data = Db.SQL<BDB.PP>("select p from PP p order by p.L1C desc");
        }

        void Handle(Input.SortL2C Action)
        {
            PPs.Data = Db.SQL<BDB.PP>("select p from PP p order by p.L2C desc");
        }

        void Handle(Input.SortL3C Action)
        {
            PPs.Data = Db.SQL<BDB.PP>("select p from PP p order by p.L3C desc");
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

                p.DlgOpened = true;
            }
        }

    }
}
