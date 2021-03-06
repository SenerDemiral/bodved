﻿using Starcounter;
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

            BDB.CC cc = null;
            BDB.CT ct = null;

            // CT (CToNo) ->
            if (!string.IsNullOrEmpty(CToNo))
            {
                ct = Db.FromId<BDB.CT>(ulong.Parse(CToNo));
                cc = ct.CC;
                Cap1 = $"{cc.Ad} {ct.Ad} Müsabakaları";
            }
            // CET (CCoNo) ->
            if (!string.IsNullOrEmpty(CCoNo))
            {
                cc = Db.FromId<BDB.CC>(ulong.Parse(CCoNo));
                Cap1 = $"{cc.Ad} Müsabakaları";
            }

            if (canMdfy && CTs.Count == 0)
            {
                CTsElementJson ps;

                var cts = Db.SQL<BDB.CT>("select p from CT p where p.CC = ? order by p.Ad", cc);
                foreach (var t in cts)
                {
                    ps = CTs.Add();
                    ps.oNo = t.oNo.ToString();
                    ps.Ad = t.Ad;
                }
            }
            if (!string.IsNullOrEmpty(CCoNo))
                CETs.Data = Db.SQL<BDB.CET>("select c from CET c where c.CC = ? order by c.Trh", cc);
            else if (!string.IsNullOrEmpty(CToNo))
                CETs.Data = Db.SQL<BDB.CET>("select c from CET c where c.CC = ? and (c.hCT = ? or c.gCT = ?) order by c.Trh", cc, ct, ct);

            //sener.NoR = DateTime.Now.Ticks;
        }

        void Handle(Input.InsertTrigger Action)
        {
            Db.Transact(() =>
            {
                new BDB.CET()
                {
                    PK = BDB.H.GEN_ID(),
                    CC = Db.FromId<BDB.CC>(ulong.Parse(CCoNo)),
                    hCT = MdfRec.hCToNo == "" ? null : Db.FromId<BDB.CT>(ulong.Parse(MdfRec.hCToNo)),
                    gCT = MdfRec.gCToNo == "" ? null : Db.FromId<BDB.CT>(ulong.Parse(MdfRec.gCToNo)),
                    Trh = DateTime.Parse(MdfRec.Trh)
                };
            });
            MdfRec.oNo = 0;
            CETs.Data = Db.SQL<BDB.CET>("select c from CET c where c.CC = ? order by c.Trh", Db.FromId<BDB.CC>(ulong.Parse(CCoNo)));

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
                            if (s.RH != null)
                                s.RH.Trh = nTrh;
                        }
                    } 
                });
                
                if (trhChanged)
                    BDB.H.RefreshRH();

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
            Session.RunTaskForAll((s, sId) => {
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
