using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Starcounter;
using System.IO;
using System.Diagnostics;

namespace BDB
{
    public static class HB
    {
        public static void bCC()
        {

            var rec = new RowCC();
            rec.CCs.Data = Db.SQL<CC>("select r from CC r");
            rec.PPs.Data = Db.SQL<PP>("select r from PP r");
            rec.CTs.Data = Db.SQL<CT>("select r from CT r");
            rec.CTPs.Data = Db.SQL<CTP>("select r from CTP r");
            rec.CETs.Data = Db.SQL<CET>("select r from CET r");
            rec.CETPs.Data = Db.SQL<CETP>("select r from CETP r");
            rec.CETRs.Data = Db.SQL<CETR>("select r from CETR r");

            var json = rec.ToJson();

            using (StreamWriter sw = new StreamWriter($@"C:\Starcounter\BodVedData\BodvedBackup.txt", false))
            {
                sw.WriteLine(json);
            }
        }

        public static void rCC()
        {
            Starcounter.XSON.Serializer.NewtonSoftSerializer srz = new Starcounter.XSON.Serializer.NewtonSoftSerializer();
            FileStream strm = File.Open(@"C:\Starcounter\BodVedData\BodvedBackup.txt", FileMode.Open);
            var rowCC = srz.Deserialize<RowCC>(strm);
            Db.Transact(() =>
            {

                foreach (var r in rowCC.CCs)
                {
                    new BDB.CC()
                    {
                        ID = r.ID,
                        Ad = r.Ad,
                        Skl = r.Skl,
                        Grp = r.Grp,
                        Idx = r.Idx,
                    };
                }
            });
            Db.Transact(() =>
            {
                foreach (var r in rowCC.PPs)
                {
                    new BDB.PP()
                    {
                        ID = r.ID,
                        RnkBaz = (int)r.RnkBaz,
                        Rnk = (int)r.Rnk,
                        Sex = r.Sex,
                        Sra = (int)r.Sra,
                        DgmYil = (int)r.DgmYil,
                        Ad = r.Ad,
                        eMail = r.eMail,
                        Tel = r.Tel,
                        L1C = (int)r.L1C,
                        L2C = (int)r.L2C,
                        L3C = (int)r.L3C,
                        KytTrh = DateTime.Parse(r.KytTarih)
                    };
                }
            });
            Db.Transact(() =>
            {
                foreach (var r in rowCC.CTs)
                {
                    var cc = Db.SQL<CC>("select r from CC r where r.ID = ?", r.CC_ID).FirstOrDefault();
                    var ppK1 = Db.SQL<PP>("select r from PP r where r.ID = ?", r.K1_ID).FirstOrDefault();
                    var ppK2 = Db.SQL<PP>("select r from PP r where r.ID = ?", r.K2_ID).FirstOrDefault();

                    new BDB.CT()
                    {
                        CC = cc,
                        ID = r.ID,
                        Ad = r.Ad,
                        Adres = r.Adres,
                        Pw = r.Pw,
                        K1 = ppK1,
                        K2 = ppK2,
                        tRnk = (int)r.tRnk,
                        tP = (int)r.tP,
                        oM = (int)r.oM,
                        aM = (int)r.aM,
                        vM = (int)r.vM,
                        aMP = (int)r.aMP,
                        vMP = (int)r.vMP,
                        aO = (int)r.aO,
                        vO = (int)r.vO,
                        aS = (int)r.aS,
                        vS = (int)r.vS
                    };
                }
            });
            Db.Transact(() =>
            {
                foreach (var r in rowCC.CTPs)
                {
                    var cc = Db.SQL<CC>("select r from CC r where r.ID = ?", r.CC_ID).FirstOrDefault();
                    var ct = Db.SQL<CT>("select r from CT r where r.CC = ? and r.ID = ?", cc, r.CT_ID).FirstOrDefault();
                    var pp = Db.SQL<PP>("select r from PP r where r.ID = ?", r.PP_ID).FirstOrDefault();
                    //var aaa = a.Ad;
                    new BDB.CTP()
                    {
                        CC = cc,
                        CT = ct,
                        PP = pp,
                        Idx = (int)r.Idx,
                    };
                }
            });
            Db.Transact(() =>
            {
                foreach (var r in rowCC.CETs)
                {
                    var cc = Db.SQL<CC>("select r from CC r where r.ID = ?", r.CC_ID).FirstOrDefault();
                    var hct = Db.SQL<CT>("select r from CT r where r.CC = ? and r.ID = ?", cc, r.hCT_ID).FirstOrDefault();
                    var gct = Db.SQL<CT>("select r from CT r where r.CC = ? and r.ID = ?", cc, r.gCT_ID).FirstOrDefault();

                    new BDB.CET()
                    {
                        CC = cc,
                        hCT = hct,
                        gCT = gct,
                        ID = r.ID,
                        Trh = DateTime.Parse(r.TrhS),
                        hPok = r.hPok,
                        gPok = r.gPok,
                        Rok = r.Rok,
                        hP = (int)r.hP,
                        gP = (int)r.gP,
                        hPW = (int)r.hPW,
                        hMSW = (int)r.hMSW,
                        hMDW = (int)r.hMDW,
                        hSSW = (int)r.hSSW,
                        hSDW = (int)r.hSDW,
                        gPW = (int)r.gPW,
                        gMSW = (int)r.gMSW,
                        gMDW = (int)r.gMDW,
                        gSSW = (int)r.gSSW,
                        gSDW = (int)r.gSDW
                    };
                }
            });
            Db.Transact(() =>
            {
                foreach (var r in rowCC.CETPs)
                {
                    var cc = Db.SQL<CC>("select r from CC r where r.ID = ?", r.CC_ID).FirstOrDefault();
                    var cet = Db.SQL<CET>("select r from CT r where r.CC = ? and r.ID = ?", cc, r.CET_ID).FirstOrDefault();
                    var ct = Db.SQL<CT>("select r from CT r where r.CC = ? and r.ID = ?", cc, r.CT_ID).FirstOrDefault();
                    var pp = Db.SQL<PP>("select r from PP r where r.ID = ?", r.PP_ID).FirstOrDefault();

                    new BDB.CETP()
                    {
                        CC = cc,
                        CET = cet,
                        CT = ct,
                        PP = pp,
                        HoG = r.HoG,
                        SoD = r.SoD,
                        Idx = (int)r.Idx
                    };
                }
            });
            Db.Transact(() =>
            {
                foreach (var r in rowCC.CETRs)
                {
                    var cc = Db.SQL<CC>("select r from CC r where r.ID = ?", r.CC_ID).FirstOrDefault();
                    var cet = Db.SQL<CET>("select r from CET r where r.CC = ? and r.ID = ?", cc, r.CET_ID).FirstOrDefault();
                    var ct = Db.SQL<CT>("select r from CT r where r.CC = ? and r.ID = ?", cc, r.CT_ID).FirstOrDefault();
                    var pp = Db.SQL<PP>("select r from PP r where r.ID = ?", r.PP_ID).FirstOrDefault();

                    new BDB.CETR()
                    {
                        CC = cc,
                        CET = cet,
                        CT = ct,
                        PP = pp,
                        HoG = r.HoG,
                        SoD = r.SoD,
                        Trh = DateTime.Parse(r.TrhS),

                        Idx = (int)r.Idx,
                        S1W = (int)r.S1W,
                        S2W = (int)r.S2W,
                        S3W = (int)r.S3W,
                        S4W = (int)r.S4W,
                        S5W = (int)r.S5W,

                        SW = (int)r.SW,
                        SL = (int)r.SL,
                        MW = (int)r.MW,
                        ML = (int)r.ML
                    };
                }
            });

            foreach(var cc in Db.SQL<CC>("select c from CC c"))
                H.ReCreateRHofCC(cc.oNo);

            H.RefreshRH();

        }
    }
}
