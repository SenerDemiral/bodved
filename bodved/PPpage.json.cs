using Starcounter;

namespace bodved
{
    partial class PPpage : Json
    {
        protected override void OnData()
        {
            base.OnData();

            //var parent = (MasterPage)this.Parent;
            //var fid = Convert.ToInt32(parent.fID);
            //var std = Convert.ToDateTime(parent.StartDate);
            //fID = parent.fID;
            //StartDate = parent.StartDate;

            //if (!parent.fOnLine)
            //    return;

            if ((Root as MasterPage).Login.LI && (Root as MasterPage).Login.Id == -1)
                this.canMdfy = true;

            PPs.Data = Db.SQL<BDB.PP>("select p from PP p order by p.Ad");

            //sener.NoR = DateTime.Now.Ticks;
        }

        void Handle(Input.InsertTrigger Action)
        {
            var oo = Action.OldValue;
            var nn = Action.Value;

            if (!string.IsNullOrWhiteSpace(PPrec.Ad))
            {
                Db.Transact(() =>
                {
                    new BDB.PP()
                    {
                        Ad = this.PPrec.Ad,
                        Sex = this.PPrec.Sex,
                        DgmYil = (int)this.PPrec.DgmYil,
                        Tel = this.PPrec.Tel,
                        eMail = this.PPrec.eMail
                    };
                });
                PPrec.oNo = 0;
                PPs.Data = Db.SQL<BDB.PP>("select p from PP p order by p.Ad");
            }
        }

        void Handle(Input.UpdateTrigger Action)
        {
            var oo = Action.OldValue;
            var nn = Action.Value;

            if (PPrec.oNo != 0)
            {
                Db.Transact(() =>
                {
                    var r = Db.FromId<BDB.PP>((ulong)PPrec.oNo);
                    r.Ad = PPrec.Ad;
                    r.Sex = this.PPrec.Sex;
                    r.DgmYil = (int)this.PPrec.DgmYil;
                    r.Tel = this.PPrec.Tel;
                    r.eMail = this.PPrec.eMail;
                });
                PPrec.oNo = 0;
                PPs.Data = Db.SQL<BDB.PP>("select p from PP p order by p.Ad");
            }
        }

        void Handle(Input.DeleteTrigger Action)
        {
            var oo = Action.OldValue;
            var nn = Action.Value;

            if (PPrec.oNo != 0)
            {
                Db.Transact(() =>
                {
                    if (PPrec.oNo != 0)
                    {
                        Db.Transact(() =>
                        {
                            var r = Db.FromId<BDB.PP>((ulong)PPrec.oNo);
                            r.Delete();
                        });
                    }
                });
                PPrec.oNo = 0;
                PPs.Data = Db.SQL<BDB.PP>("select p from PP p order by p.Ad");
            }
        }

        [PPpage_json.PPs]
        public partial class PPsElementJson
        {
            void Handle(Input.MdfTrigger Action)
            {
                var rec = this.Parent.Parent as PPpage;
                rec.PPrec.oNo = this.oNo;
                rec.PPrec.Ad = this.Ad;
                rec.PPrec.Sex = this.Sex;
                rec.PPrec.DgmYil = this.DgmYil;
                rec.PPrec.Tel = this.Tel;
                rec.PPrec.eMail = this.eMail;

                var deneme = this.Root;
            }
        }

    }
}
