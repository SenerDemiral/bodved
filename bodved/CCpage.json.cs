using System.Linq;
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
                    new BDB.CC()
                    {
                        PK = H.GEN_ID(),
                        Ad = MdfRec.Ad,
                        Idx = MdfRec.Idx,
                        Skl = MdfRec.Skl,
                        Grp = MdfRec.Grp,
                        Lig = MdfRec.Lig,
                        RnkGrp = MdfRec.RnkGrp
                    };
                });
                MdfRec.oNo = 0;

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
                    var r = Db.FromId<BDB.CC>((ulong)MdfRec.oNo);
                    r.ID = MdfRec.ID;
                    r.Ad = MdfRec.Ad;
                    r.Idx = MdfRec.Idx;
                    r.Skl = MdfRec.Skl;
                    r.Grp = MdfRec.Grp;
                    r.Lig = MdfRec.Lig;
                    r.RnkGrp = MdfRec.RnkGrp;
                });
                MdfRec.oNo = 0;

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
                            var r = Db.FromId<BDB.CC>((ulong)MdfRec.oNo);
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
                p.MdfRec.ID = ID;
                p.MdfRec.Ad = Ad;
                p.MdfRec.Idx = Idx;
                p.MdfRec.Skl = Skl;
                p.MdfRec.Grp = Grp;
                p.MdfRec.Lig = Lig;
                p.MdfRec.RnkGrp = RnkGrp;

                p.DlgOpened = true;
            }
        }

    }
}
