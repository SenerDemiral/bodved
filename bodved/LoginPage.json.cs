using Starcounter;

namespace bodved
{
    partial class LoginPage : Json
    {
        void Handle(Input.LoginTrigger A)
        {
            var mpLgn = (Root as MasterPage).Login;
            mpLgn.LI = false;
            mpLgn.Nm = "Login";
            mpLgn.Id = Id;

            if (Id == "-1")
            {
                if (Pw == "CanDilSen")
                {
                    mpLgn.Nm = "ADMIN";
                    mpLgn.Rl = "ADMIN";
                    mpLgn.LI = true;
                }

            }
            else
            {
                var ct = Db.FromId<BDB.CT>(ulong.Parse(Id));
                if (Pw == ct.Pw)
                {
                    mpLgn.Nm = ct.Ad;
                    mpLgn.Rl = "TAKIM";
                    mpLgn.LI = true;
                    if (!string.IsNullOrEmpty(nPw))
                    {
                        Db.Transact(() =>
                        {
                            ct.Pw = nPw;
                        });
                    }

                }
            }
            BDB.H.Write2Log($"Login: {mpLgn.Nm} {mpLgn.Id} {Pw} {nPw} {mpLgn.LI}");

        }
    }
}
