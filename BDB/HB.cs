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
        public static void PKset()
        {
            H.GEN_ID_DEL();

            Db.Transact(() =>
            {
                foreach(var r in Db.SQL<CC>("select c from CC c"))
                {
                    r.PK = H.GEN_ID();
                }

                foreach (var r in Db.SQL<PP>("select c from PP c"))
                {
                    r.PK = H.GEN_ID();
                }

                foreach (var r in Db.SQL<CT>("select c from CT c"))
                {
                    r.PK = H.GEN_ID();
                }

                foreach (var r in Db.SQL<CET>("select c from CET c"))
                {
                    r.PK = H.GEN_ID();
                }
            });
        }

        public static void bCC()
        {

            var rec = new RowCC();
            rec._IDs.Data = Db.SQL<_ID>("select r from _ID r");
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
                foreach (var r in rowCC._IDs)
                {
                    new BDB._ID()
                    {
                        ID = (ulong)r.ID
                    };
                }
            });
            Db.Transact(() =>
            {
                foreach (var r in rowCC.CCs)
                {
                    new BDB.CC()
                    {
                        PK = r.PK,
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
                        PK = r.PK,
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
                    var cc   = Db.SQL<CC>("select r from CC r where r.PK = ?", r.CC_PK).FirstOrDefault();
                    var ppK1 = Db.SQL<PP>("select r from PP r where r.PK = ?", r.K1_PK).FirstOrDefault();
                    var ppK2 = Db.SQL<PP>("select r from PP r where r.PK = ?", r.K2_PK).FirstOrDefault();

                    new BDB.CT()
                    {
                        PK = r.PK,
                        CC = cc,
                        K1 = ppK1,
                        K2 = ppK2,
                        Ad = r.Ad,
                        Adres = r.Adres,
                        Pw = r.Pw,
                        tRnk = (int)r.tRnk,
                        tP = (int)r.tP,
                        oM = (int)r.oM,
                        aM = (int)r.aM,
                        vM = (int)r.vM,
                        fM = (int)r.fM,
                        aMP = (int)r.aMP,
                        vMP = (int)r.vMP,
                        fMP = (int)r.fMP,
                        aO = (int)r.aO,
                        vO = (int)r.vO,
                        fO = (int)r.fO,
                        aS = (int)r.aS,
                        vS = (int)r.vS,
                        fS = (int)r.fS
                    };
                }
            });
            Db.Transact(() =>
            {
                foreach (var r in rowCC.CTPs)
                {
                    var cc = Db.SQL<CC>("select r from CC r where r.PK = ?", r.CC_PK).FirstOrDefault();
                    var ct = Db.SQL<CT>("select r from CT r where r.PK = ?", r.CT_PK).FirstOrDefault();
                    var pp = Db.SQL<PP>("select r from PP r where r.PK = ?", r.PP_PK).FirstOrDefault();
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
                    var cc =  Db.SQL<CC>("select r from CC r where r.PK = ?", r.CC_PK).FirstOrDefault();
                    var hct = Db.SQL<CT>("select r from CT r where r.PK = ?", r.hCT_PK).FirstOrDefault();
                    var gct = Db.SQL<CT>("select r from CT r where r.PK = ?", r.gCT_PK).FirstOrDefault();

                    new BDB.CET()
                    {
                        PK = r.PK,
                        CC = cc,
                        hCT = hct,
                        gCT = gct,
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
                        gSDW = (int)r.gSDW,

                        Info = r.Info
                    };
                }
            });
            Db.Transact(() =>
            {
                foreach (var r in rowCC.CETPs)
                {
                    var cc =  Db.SQL<CC>( "select r from CC  r where r.PK = ?", r.CC_PK).FirstOrDefault();
                    var cet = Db.SQL<CET>("select r from CET r where r.PK = ?", r.CET_PK).FirstOrDefault();
                    var ct =  Db.SQL<CT>( "select r from CT  r where r.PK = ?", r.CT_PK).FirstOrDefault();
                    var pp =  Db.SQL<PP>( "select r from PP  r where r.PK = ?", r.PP_PK).FirstOrDefault();

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
                    var cc =  Db.SQL<CC> ("select r from CC  r where r.PK = ?", r.CC_PK).FirstOrDefault();
                    var cet = Db.SQL<CET>("select r from CET r where r.PK = ?", r.CET_PK).FirstOrDefault();
                    var ct =  Db.SQL<CT> ("select r from CT  r where r.PK = ?", r.CT_PK).FirstOrDefault();
                    var pp =  Db.SQL<PP> ("select r from PP  r where r.Pk = ?", r.PP_PK).FirstOrDefault();

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
