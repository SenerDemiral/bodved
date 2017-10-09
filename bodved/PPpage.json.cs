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

            if (!string.IsNullOrWhiteSpace(MdfRec.Ad))
            {
                Db.Transact(() =>
                {
                    new BDB.PP()
                    {
                        Ad = this.MdfRec.Ad,
                        Sex = this.MdfRec.Sex,
                        DgmYil = (int)this.MdfRec.DgmYil,
                        Tel = this.MdfRec.Tel,
                        eMail = this.MdfRec.eMail
                    };
                });
                MdfRec.oNo = 0;
                PPs.Data = Db.SQL<BDB.PP>("select p from PP p order by p.Ad");
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
                    var r = Db.FromId<BDB.PP>((ulong)MdfRec.oNo);
                    r.Ad = MdfRec.Ad;
                    r.Sex = this.MdfRec.Sex;
                    r.DgmYil = (int)this.MdfRec.DgmYil;
                    r.Tel = this.MdfRec.Tel;
                    r.eMail = this.MdfRec.eMail;
                });
                MdfRec.oNo = 0;
                PPs.Data = Db.SQL<BDB.PP>("select p from PP p order by p.Ad");
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
                            var r = Db.FromId<BDB.PP>((ulong)MdfRec.oNo);
                            r.Delete();
                        });
                    }
                });
                MdfRec.oNo = 0;
                PPs.Data = Db.SQL<BDB.PP>("select p from PP p order by p.Ad");
            }
        }

        [PPpage_json.PPs]
        public partial class PPsElementJson
        {
            void Handle(Input.MdfTrigger Action)
            {
                var p = this.Parent.Parent as PPpage;
                p.MdfRec.oNo = this.oNo;
                p.MdfRec.Ad = this.Ad;
                p.MdfRec.Sex = this.Sex;
                p.MdfRec.DgmYil = this.DgmYil;
                p.MdfRec.Tel = this.Tel;
                p.MdfRec.eMail = this.eMail;

                var deneme = this.Root;
            }
        }

    }
}
