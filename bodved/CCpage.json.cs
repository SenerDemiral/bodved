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

            //canMdfy = true; //DENEME
            CCs.Data = Db.SQL<BDB.CC>("select c from CC c order by c.Idx desc");

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
                        Ad = MdfRec.Ad,
                        Idx = MdfRec.Idx,
                        Skl = MdfRec.Skl,
                        Grp = MdfRec.Grp
                    };
                });
                MdfRec.oNo = 0;

                PushChanges();
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
                    r.Idx = MdfRec.Idx;
                    r.Skl = MdfRec.Skl;
                    r.Grp = MdfRec.Grp;
                });
                MdfRec.oNo = 0;

                PushChanges();
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

                PushChanges();
            }
        }

        public void PushChanges()
        {
            Session.ForAll((s, sId) => {
                var cp = (s.Store["bodved"] as MasterPage).CurrentPage;
                if (cp is CCpage)
                {
                    (s.Store["bodved"] as MasterPage).CurrentPage.Data = null;
                    s.CalculatePatchAndPushOnWebSocket();
                }
            });
        }

        [CCpage_json.CCs]
        public partial class CCsElementJson
        {
            void Handle(Input.MdfTrigger Action)
            {
                var p = this.Parent.Parent as CCpage;
                p.MdfRec.oNo = oNo;
                p.MdfRec.Ad = Ad;
                p.MdfRec.Idx = Idx;
                p.MdfRec.Skl = Skl;
                p.MdfRec.Grp = Grp;

                var deneme = this.Root;
            }
        }

    }
}
