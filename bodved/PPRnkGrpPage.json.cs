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
                RnkID = cc.RnkID;
                RnkAd = cc.RnkAd;
            }

            PPs.Data = Db.SQL<BDB.PPGR>("select p from PPGR p where p.RnkID = ? order by p.Sra", RnkID);
            NOP = PPs.Count;
            Cap1 = $"{RnkAd} Oynayan say�s� : {NOP - 1}";
        }
    }
}
