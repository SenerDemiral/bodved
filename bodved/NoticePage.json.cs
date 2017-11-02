using System;
using System.Linq;
using Starcounter;

namespace bodved
{
    partial class NoticePage : Json
    {
        protected override void OnData()
        {
            base.OnData();

            var mpLgn = (Root as MasterPage).Login;
            canMdfy = mpLgn.Rl == "ADMIN" && mpLgn.LI ? true : false;

            Cap1 = $"Duyurular";

            NTCs.Data = Db.SQL<BDB.NTC>("select c from NTC c order by c.Onc, c.Trh desc");

        }

        void Handle(Input.InsertTrigger Action)
        {
            Db.Transact(() =>
            {
                new BDB.NTC()
                {
                    Onc = MdfRec.Onc,
                    Trh = DateTime.Parse(MdfRec.Tarih),
                    Ad = MdfRec.Ad,
                    Link = MdfRec.Link
                };
            });
            MdfRec.oNo = 0;

            PushChanges();
        }

        void Handle(Input.UpdateTrigger Action)
        {
            if (MdfRec.oNo != 0)
            {
                Db.Transact(() =>
                {
                    var r = Db.FromId<BDB.NTC>((ulong)MdfRec.oNo);

                    r.Onc = MdfRec.Onc;
                    r.Trh = DateTime.Parse(MdfRec.Tarih);
                    r.Ad = MdfRec.Ad;
                    r.Link = MdfRec.Link;
                });

                MdfRec.oNo = 0; // Yanlislikla birdaha bu kayitta birsey yapamasin

                PushChanges();
            }
        }

        void Handle(Input.DeleteTrigger Action)
        {
            if (MdfRec.oNo != 0)
            {
                Db.Transact(() =>
                {
                    if (MdfRec.oNo != 0)
                    {
                        Db.Transact(() =>
                        {
                            var r = Db.FromId<BDB.NTC>((ulong)MdfRec.oNo);
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
                if (cp is NoticePage)
                {
                    (s.Store["bodved"] as MasterPage).CurrentPage.Data = null;
                    s.CalculatePatchAndPushOnWebSocket();
                }
            });
        }

        [NoticePage_json.NTCs]
        public partial class NTCsElementJson
        {
            void Handle(Input.MdfTrigger Action)
            {
                var p = this.Parent.Parent as NoticePage;
                p.MdfRec.oNo = this.oNo;
                p.MdfRec.Onc = Onc;
                p.MdfRec.Tarih = DateTime.Parse(Trh).ToString("yyyy-MM-dd");
                p.MdfRec.Ad = Ad;
                p.MdfRec.Link = Link;
            }
        }


    }
}
