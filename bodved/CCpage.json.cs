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

            if ((Root as MasterPage).Login.LI && (Root as MasterPage).Login.Id == -1)
                this.canMdfy = true;

            CCs.Data = Db.SQL<BDB.CC>("select c from CC c order by c.Ad");

            //sener.NoR = DateTime.Now.Ticks;
        }

        void Handle(Input.SaveTrigger Action)
        {
            var oo = Action.OldValue;
            var nn = Action.Value;

            Db.Transact(() =>
            {
                if (CCrec.oNo == 0)
                {
                    new BDB.CC()
                    {
                        Ad = this.CCrec.Ad
                    };
                }
                else
                {
                    var ccr = Db.FromId<BDB.CC>((ulong)CCrec.oNo);
                    ccr.Ad = CCrec.Ad;
                }
            });

            CCs.Data = Db.SQL<BDB.CC>("select c from CC c order by c.Ad");
        }

        [CCpage_json.CCs]
        public partial class CCsElementJson
        {
            void Handle(Input.MdfTrigger Action)
            {
                var ccp = this.Parent.Parent as CCpage;
                ccp.CCrec.oNo = this.oNo;
                ccp.CCrec.Ad = this.Ad;

                var deneme = this.Root;
            }

            void Handle(Input.DltTrigger Action)
            {
                var ccp = this.Parent.Parent as CCpage;
                ccp.CCrec.oNo = this.oNo;
                ccp.CCrec.Ad = this.Ad;

                Db.Transact(() =>
                {
                    var ccr = Db.FromId<BDB.CC>((ulong)oNo);
                    ccr.Delete();
                    //Db.SQL<BDB.CC>("delete from CC c where c.ObjectNo = ?", (object)this.oNo);
                });
                (Parent.Parent as CCpage).Data = Db.SQL<BDB.CC>("select c from CC c order by c.Ad");
            }
        }

    }
}
