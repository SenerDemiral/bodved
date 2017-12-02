using System.Linq;
using Starcounter;

namespace bodved
{
    partial class MasterPage : Json
    {
        protected override void OnData()
        {
            var s = Db.SQL<BDB.STAT>("select s from STAT s where s.ID = ?", 1).FirstOrDefault();
            EntCnt = s.IdVal;

            base.OnData();
        }

        void Handle(Input.DlgOpenTrigger Action)
        {
            DlgOpened = true;
        }
        /*
        [MasterPage_json.Login]
        public partial class LoginElementJson
        {

        }
        */
        void Handle(Input.LoginTrigger A)
        {
            var mpLgn = (Root as MasterPage).Login;
            mpLgn.LI = false;
            mpLgn.Nm = "Login";
            mpLgn.Id = Login.Id;

            if (Login.Id == "-1")
            {
                if (Login.Pw == "BoDvEd")
                {
                    Login.Nm = "ADMIN";
                    Login.Rl = "ADMIN";
                    Login.LI = true;
                }

            }
            else if (Login.Id == "-2")
            {
                if (Login.Pw == "modev")
                {
                    Login.Nm = "ADMIN";
                    Login.Rl = "ADMIN";
                    Login.LI = true;
                }

            }
            else
            {
                var ct = Db.FromId<BDB.CT>(ulong.Parse(Login.Id));
                if (Login.Pw == ct.Pw)
                {
                    Login.Nm = ct.Ad;
                    Login.Rl = "TAKIM";
                    Login.LI = true;
                    if (!string.IsNullOrEmpty(Login.nPw))
                    {
                        Db.Transact(() =>
                        {
                            ct.Pw = Login.nPw;
                        });
                    }

                }
            }
            Login.Pw = "";
            Login.nPw = "";

            BDB.H.Write2Log($"Login: {Login.Nm} {Login.Id} {Login.Pw} {Login.nPw} {Login.LI}");

            DlgOpened = false;
            CurrentPage["canMdfy"] = true;
        }
        
    }
}
