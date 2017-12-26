using System.Linq;
using Starcounter;

namespace bodved
{
    partial class PPRnkGrpPage : Json
    {
        protected override void OnData()
        {
            base.OnData();

            var mpLgn = (Root as MasterPage).Login;
            canMdfy = mpLgn.Rl == "ADMIN" && mpLgn.LI ? true : false;


            BDB.CC cc = null;
            if (CCoNo != 0)
            {
                cc = Db.FromId<BDB.CC>((ulong)CCoNo);
                RnkGrp = cc.RnkGrp;
                RnkGrpAd = cc.RnkGrpAd;
            }

            PPs.Data = Db.SQL<BDB.PPGR>("select p from PPGR p where p.RnkGrp = ? order by p.Sra", RnkGrp);
            NOP = PPs.Count;
            Cap1 = $"{RnkGrpAd} Oynayan sayýsý : {NOP - 1}";
        }
    }
}
