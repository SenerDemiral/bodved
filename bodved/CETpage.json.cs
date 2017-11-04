using Starcounter;
using System;
using System.Linq;
using System.Diagnostics;

namespace bodved
{
    partial class CETpage : Json
    {
        protected override void OnData()
        {
            base.OnData();

            var mpLgn = (Root as MasterPage).Login;
            canMdfy = mpLgn.Rl == "ADMIN" && mpLgn.LI ? true : false;

            var cc = Db.FromId<BDB.CC>(ulong.Parse(CCoNo));
            Cap1 = $"{cc.Ad} Müsabakaları";

            if (canMdfy && CTs.Count == 0)
            {
                CTsElementJson ps;

                var cts = Db.SQL<BDB.CT>("select p from CT p where p.CC = ? order by p.Ad", cc);
                foreach (var ct in cts)
                {
                    ps = CTs.Add();
                    ps.oNo = ct.oNo.ToString();
                    ps.Ad = ct.Ad;
                }
            }

            CETs.Data = Db.SQL<BDB.CET>("select c from CET c where c.CC = ? order by c.ID, c.Trh", Db.FromId<BDB.CC>(ulong.Parse(CCoNo)));

            //sener.NoR = DateTime.Now.Ticks;
        }

        void Handle(Input.InsertTrigger Action)
        {
            Db.Transact(() =>
            {
                new BDB.CET()
                {
                    CC = Db.FromId<BDB.CC>(ulong.Parse(CCoNo)),
                    ID = MdfRec.ID,
                    hCT = MdfRec.hCToNo == "" ? null : Db.FromId<BDB.CT>(ulong.Parse(MdfRec.hCToNo)),
                    gCT = MdfRec.gCToNo == "" ? null : Db.FromId<BDB.CT>(ulong.Parse(MdfRec.gCToNo)),
                    Trh = DateTime.Parse(MdfRec.Trh)
                };
            });
            MdfRec.oNo = 0;
            CETs.Data = Db.SQL<BDB.CET>("select c from CET c where c.CC = ? order by c.ID, c.Trh", Db.FromId<BDB.CC>(ulong.Parse(CCoNo)));

            //PushChanges();
        }

        void Handle(Input.UpdateTrigger Action)
        {
            if (MdfRec.oNo != 0)
            {
                bool trhChanged = false;
                //Stopwatch watch = new Stopwatch();
                //watch.Start();

                Db.Transact(() =>
                {
                    var r = Db.FromId<BDB.CET>((ulong)MdfRec.oNo);

                    // Oyuncular varsa talimlar degisemez, ama tarih degisebilir
                    var cetps = Db.SQL<BDB.CETP>("select c from CETP c where c.CET = ?", r).FirstOrDefault();
                    if (cetps == null)
                    {
                        r.hCT = MdfRec.hCToNo == "" ? null : Db.FromId<BDB.CT>(ulong.Parse(MdfRec.hCToNo));
                        r.gCT = MdfRec.gCToNo == "" ? null : Db.FromId<BDB.CT>(ulong.Parse(MdfRec.gCToNo));
                    }

                    var nTrh = DateTime.Parse(MdfRec.Trh);
                    r.ID = MdfRec.ID;
                    r.hP = (int)MdfRec.hP;
                    r.gP = (int)MdfRec.gP;

                    if (r.Trh != nTrh)
                    {
                        trhChanged = true;
                        r.Trh = nTrh;

                        foreach (var s in Db.SQL<BDB.CETR>("select c from CETR c where c.CET = ?", r))
                        {
                            s.Trh = nTrh;
                            // Oyuncu Rank/Mac tarihini de degistir
                            if (s.PRH != null)
                                s.PRH.Trh = nTrh;
                        }
                    } 
                });
                
                if (trhChanged)
                    BDB.H.refreshPRH();

                //watch.Stop();
                //Console.WriteLine($"{watch.ElapsedMilliseconds} msec  {watch.ElapsedTicks} ticks");

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
                            var r = Db.FromId<BDB.CET>((ulong)MdfRec.oNo);
                            r.Delete();
                        });
                    }
                });
                MdfRec.oNo = 0;
                CETs.Data = Db.SQL<BDB.CET>("select c from CET c where c.CC = ? order by c.ID, c.Trh", Db.FromId<BDB.CC>(ulong.Parse(CCoNo)));

                //PushChanges();
            }
        }

        public void PushChanges()
        {
            Session.ForAll((s, sId) => {
                var cp = (s.Store["bodved"] as MasterPage).CurrentPage;
                if (cp is CETpage && CCoNo == (cp as CETpage).CCoNo)
                {
                    //(s.Store["bodved"] as MasterPage).CurrentPage.Data = null;
                    s.CalculatePatchAndPushOnWebSocket();
                }
            });
        }

        [CETpage_json.CETs]
        public partial class CETsElementJson
        {
            void Handle(Input.MdfTrigger Action)
            {
                var p = this.Parent.Parent as CETpage;
                p.MdfRec.oNo = this.oNo;
                p.MdfRec.ID = this.ID;
                p.MdfRec.hCToNo = this.hCToNo.ToString();
                p.MdfRec.gCToNo = this.gCToNo.ToString();
                p.MdfRec.Trh = DateTime.Parse(Trh).ToString("yyyy-MM-dd");
                p.MdfRec.hP = hP;
                p.MdfRec.gP = gP;
            }
        }
    }
}
