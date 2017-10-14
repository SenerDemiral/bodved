using Starcounter;
using BDB;

namespace bodved
{
    partial class CCpage : Json
    {
        protected override void OnData()
        {
            base.OnData();

            var parent = (MasterPage)this.Parent;
            //var fid = Convert.ToInt32(parent.fID);
            //var std = Convert.ToDateTime(parent.StartDate);
            //fID = parent.fID;
            //StartDate = parent.StartDate;

            //if (!parent.fOnLine)
            //    return;
            var mpLgn = (Root as MasterPage).Login;
            canMdfy = mpLgn.Rl == "ADMIN" && mpLgn.LI ? true : false;

            CCs.Data = Db.SQL<BDB.CC>("select c from CC c order by c.Ad");

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
                    new BDB.CC()
                    {
                        Ad = this.MdfRec.Ad,
                    };
                });
                MdfRec.oNo = 0;
                CCs.Data = Db.SQL<BDB.CC>("select c from CC c order by c.Ad");
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
                    var r = Db.FromId<BDB.CC>((ulong)MdfRec.oNo);
                    r.Ad = MdfRec.Ad;
                });
                MdfRec.oNo = 0;
                CCs.Data = Db.SQL<BDB.CC>("select c from CC c order by c.Ad");
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
                            var r = Db.FromId<BDB.CC>((ulong)MdfRec.oNo);
                            r.Delete();
                        });
                    }
                });
                MdfRec.oNo = 0;
                CCs.Data = Db.SQL<BDB.CC>("select c from CC c order by c.Ad");
            }
        }

        [CCpage_json.CCs]
        public partial class CCsElementJson
        {
            void Handle(Input.MdfTrigger Action)
            {
                var p = this.Parent.Parent as CCpage;
                p.MdfRec.oNo = this.oNo;
                p.MdfRec.Ad = this.Ad;

                var deneme = this.Root;
            }
        }

    }
}
