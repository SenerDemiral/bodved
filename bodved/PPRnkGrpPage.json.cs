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

            PPs.Data = Db.SQL<BDB.PPGR>("select p from PPGR p where p.RnkGrp = ? order by p.Sra", RnkGrp);
            //NOP = Db.SQL<BDB.PP>("select p from PP p").Count();
            NOP = PPs.Count;

            Cap1 = $"{RnkGrp} Oyuncu sayýsý : {NOP - 1}";
        }
    }
}
