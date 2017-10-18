using Starcounter;

namespace bodved
{
    partial class ppStatPage : Json
    {
        protected override void OnData()
        {
            base.OnData();

            var mpLgn = (Root as MasterPage).Login;
            canMdfy = mpLgn.Rl == "ADMIN" && mpLgn.LI ? true : false;

            PPs.Data = Db.SQL<BDB.PP>("select p from PP p order by p.Ad");

            //sener.NoR = DateTime.Now.Ticks;
        }

    }
}
