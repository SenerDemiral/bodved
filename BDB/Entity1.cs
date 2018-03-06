using System;
using System.Linq;
using Starcounter;
using System.Globalization;

namespace BDB
{
    [Database]
    public class _ID
    {
        public ulong ID { get; set; }

        public _ID()
        {
            ID = 0;
        }

    }

    [Database]
    public class STAT
    {
        public int ID { get; set; }
        public int IdVal { get; set; }

        public STAT()
        {
            ID = 1;
            IdVal = 4230;
        }
    }

    [Database]
    public class NTC
    {
        public ulong oNo => this.GetObjectNo();

        public string Onc { get; set; }     // Oncelik 1-9, < 1 olanlar otomatik 
        public DateTime Trh { get; set; }
        public string Ad { get; set; }
        public string Link { get; set; }
        public string Rtbl { get; set; }    // Referans Table
        public ulong RoNo { get; set; }

        public string Tarih => string.Format(CultureInfo.CreateSpecificCulture("tr-TR"), "{0:dd MMM ddd}", Trh);
    }

    [Database]
    public class CC    // Competitions
    {
        public ulong oNo => this.GetObjectNo();
        public long PK { get; set; }
        public string ID { get; set; }
        public string Ad { get; set; }
        public string Idx { get; set; }     // Siralama icin gerekli
        public string Skl { get; set; }     // Takim/Ferdi
        public string Grp { get; set; }     // Birden cok turnuvada oynayan Oyunculari takip icin
        public string Lig { get; set; }
        public long RnkID { get; set; }
        public string RnkAd { get; set; }
        //public string RnkGrp { get; set; }
        //public string RnkGrpAd { get; set; }
        /*
        public string Lig
        {
            get
            {
                if (Skl == "T")
                    return ID[3].ToString();
                return "-";
            }
        }
        */
        public CC()
        {
            Ad = "";
            Skl = "T";
            Grp = "T17";
        }
    }

    [Database]
    public class PP     // Players
    {
        public ulong oNo => this.GetObjectNo();
        public long PK { get; set; }

        public string ID { get; set; }  //SIL
        public string Ad { get; set; }
        public string Sex { get; set; }
        public int DgmYil { get; set; }
        public string Tel { get; set; }
        public string eMail { get; set; }
        public DateTime KytTrh { get; set; }

        public string KytTarih => KytTrh.ToString("s");

        public int RnkBaz { get; set; }
        public int Rnk { get; set; }
        public int Sra { get; set; }    // Rank'a gore sirasi

        // DROP-----
        /*
        public int SSo { get; set; }    // Single Set Oynadigi
        public int SSa { get; set; }
        public int SSv { get; set; }
        public int SMo { get; set; }    //        Mac Oynadigi
        public int SMa { get; set; }
        public int SMv { get; set; }

        public int DSo { get; set; }
        public int DSa { get; set; }
        public int DSv { get; set; }
        public int DMo { get; set; }
        public int DMa { get; set; }
        public int DMv { get; set; }
        */
        // ----------
        public int L1C { get; set; }
        public int L2C { get; set; }
        public int L3C { get; set; }

        public int LTC => L1C + L2C + L3C;
        public int Lig => Ad.StartsWith("∞") ? 0 : L1C != 0 ? 1 : L2C != 0 ? 2 : L3C != 0 ? 3 : 0;  // Oynadigi en ust lig.

        public string oCTs
        {
            get     // Oynadigi dTakimlar
            {
                string oCTs = "";
                foreach (var r in Db.SQL<CTP>("select c from BDB.CTP c where c.PP = ?", this))
                {
                    oCTs += $"{r.CC.ID}:{r.CT.Ad} ";  //r.CC.ID + "-" + r.CT.ID + " ";
                }

                return oCTs;
            }
        }

        public PP()
        {
            Ad = "";
            Sex = "E";
            DgmYil = 1960;
            KytTrh = DateTime.Now;
            RnkBaz = 1900;
            Rnk = 0;
        }
    }

    /// Dnm(17-18), Grp(T17) veya Lig(1,2,3) bazli Rank yapilabilir. 
    /// Aktif Dnm sonuclari PP de gosterilir, digerleri ayri Page de.
    [Database]
    public class PPGR   // Oyuncu Group Rank
    {
        public ulong oNo => this.GetObjectNo();
        public PP PP { get; set; }
        public long RnkID { get; set; }
        //public string RnkGrp { get; set; } 
        public int RnkBaz { get; set; }
        public int Rnk { get; set; }
        public int RnkANY { get; set; } // Rank Azaldi(-1), Neutral(0), Yukseldi(+1)
        public int Sra { get; set; }
        public int aO { get; set; }     // Aldigi Oyun/Mac
        public int vO { get; set; }

        public ulong PPoNo => PP?.GetObjectNo() ?? 0;
        public string PPAd => PP?.Ad ?? "-";
        public int Aktif => PPAd.StartsWith("∞") ? 0 : 1;


        public PPGR()
        {
            RnkBaz = 0;
            Rnk = 0;
            Sra = 0;
            aO = 0;
            vO = 0;
            RnkANY = 0;
        }
    }


    // Global, Oyuncu oynayabilmesi icin burada tanimlanmalidir.
    // Hangi takimda oynayacagi CTP de belirtilir.

    [Database]
    public class CT    // Competition Teams
    {
        public ulong oNo => this.GetObjectNo();
        public long PK { get; set; }

        public CC CC { get; set; }
        public string ID { get; set; }
        public string Ad { get; set; }
        public PP K1 { get; set; }
        public PP K2 { get; set; }
        public string Adres { get; set; }
        public string Pw { get; set; }

        public int tRnk { get; set; }// Takim Rank
        public int tP { get; set; }  // Takim Puan

        public int oM { get; set; }  // Oynadigi Musabaka
        public int aM { get; set; }  // Aldigi
        public int vM { get; set; }  // Verdigi
        public int fM { get; set; }  // Fark

        public int aMP { get; set; } // Aldigi MusabakaPuani/Skor
        public int vMP { get; set; } // Verdigi
        public int fMP { get; set; } // Fark

        public int aO { get; set; }  // Aldigi Mac/Oyun
        public int vO { get; set; }  // Verdigi
        public int fO { get; set; }  // Fark

        public int aS { get; set; }  // Aldigi Set
        public int vS { get; set; }  // Verdigi Set
        public int fS { get; set; }  // Fark Set
        /*
        public int oE { get; set; }
        public int aE { get; set; }
        public int vE { get; set; }
        public int aP { get; set; }
        public int vP { get; set; }
        */
        public string CCAd => CC?.Ad ?? "-";
        public ulong K1oNo => K1?.GetObjectNo() ?? 0;
        public ulong K2oNo => K2?.GetObjectNo() ?? 0;
        public string K1Ad => K1 == null ? "-" : $"{K1.Ad} ({K1.Tel})";
        public string K2Ad => K2 == null ? "-" : $"{K2.Ad} ({K2.Tel})"; //K2?.Ad ?? "-";

        public long CC_PK => CC?.PK ?? 0;
        public long K1_PK => K1?.PK ?? 0;
        public long K2_PK => K2?.PK ?? 0;

        public CT()
        {
            Ad = "";
            Adres = "";
            Pw = null;
        }
    }
    // CC->CT: Kaptan1 ve Kaptan2 PP'den secilir
    // Her Comp icin Takim tanimi yapilir, birdahaki sene baska bir ad alabilecegi icin

    [Database]
    public class CTP    // Competition Team Players
    {
        public ulong oNo => this.GetObjectNo();

        public CC CC { get; set; }
        public CT CT { get; set; }
        public PP PP { get; set; }
        //public string PPid { get; set; } DROP
        public int Idx { get; set; }

        public ulong PPoNo => PP?.GetObjectNo() ?? 0;
        public ulong CCoNo => CC?.GetObjectNo() ?? 0;
        public ulong CToNo => CT?.GetObjectNo() ?? 0;
        public string CCAd => CC?.Ad ?? "-";
        public string CTAd => CT?.Ad ?? "-";
        public string PPAd => PP?.Ad ?? "-";


        public int L1C => PP?.L1C ?? 0;
        public int L2C => PP?.L2C ?? 0;
        public int L3C => PP?.L3C ?? 0;

        public long CC_PK => CC?.PK ?? 0;
        public long CT_PK => CT.PK;
        public long PP_PK => PP.PK;

        //public int PPRnk => PP?.Rnk == null ? 0 : PP.Rnk == 0 ? PP.RnkBaz : PP.Rnk;   DROP
        public int PPRnk2
        {
            get
            {
                int grpRnk = 0;
                var ppgr = Db.SQL<PPGR>("select p from BDB.PPGR p where p.PP = ? and p.RnkID = ?", PP, CC.RnkID).FirstOrDefault();
                if (ppgr != null)
                    grpRnk = ppgr.Rnk;
                return grpRnk;

            }
        }
        public string oCTs
        {
            get     // Oynadigi diger Takimlar
            {
                string oCTs = "";
                foreach (var r in Db.SQL<CTP>("select c from BDB.CTP c where c.PP = ? and c.CC.Grp = ?", this.PP, this.CC.Grp))
                {
                    if (r.CT.GetObjectNo() != this.CT.GetObjectNo())
                    {
                        oCTs += r.CTAd + " ";
                    }
                }

                return oCTs;
            }
        }
    }
    // CT->CTP gecildikten sonra PP'den secilerek olusturulur
    // Oyuncu baska takima gectiginde burdan silinip digerine eklenir.
    // Bu tablo CETP (Siralama) olusturulmasinda kullanidigi icin, oynamiyanin cikartilmasi gerekir

    /*
    [Database]
    public class CEI    // Competition Event Individual (Ferdi)
    {
        public ulong oNo => this.GetObjectNo();

        public CC CC { get; set; }
        public PP PP { get; set; }
    
        public string CCAd => CC?.Ad ?? "-";
        public string PPAd => PP?.Ad ?? "-";
    }
    */

    [Database]
    public class CET    // Competition Event Teams
    {
        public ulong oNo => this.GetObjectNo();
        public long PK { get; set; }

        public CC CC { get; set; }
        public string ID { get; set; }
        public CT hCT { get; set; }
        public CT gCT { get; set; }
        public DateTime Trh { get; set; }

        public string Info { get; set; }  // Aciklama

        public bool hPok { get; set; }  // Home Players Ok
        public bool gPok { get; set; }
        public bool Rok { get; set; }   // Results Ok. Home/Guest Onaylayabilir

        // H & G Sonuclari onayladiktan sonra hesaplanir
        public int hP { get; set; }    // Home  Musabaka Puan
        public int gP { get; set; }    // Guest 

        public int hPW { get; set; }    // Home Skor/MsbkPuan Win
        public int hMSW { get; set; }   //      Mac Single Win
        public int hMDW { get; set; }   //          Double
        public int hSSW { get; set; }   //      Set Single Win (Single Aldigi Set Sayisi)
        public int hSDW { get; set; }   //          Double Win (Double Aldigi Set Sayisi)

        public int gPW { get; set; }    // Guest Skor/MsbkPuan Win
        public int gMSW { get; set; }   //       Mac Single
        public int gMDW { get; set; }   //           Double
        public int gSSW { get; set; }   //       Set Single Win (Single Aldigi Set Sayisi)
        public int gSDW { get; set; }   //           Double Win (Double Aldigi Set Sayisi)

        public ulong CCoNo => CC?.GetObjectNo() ?? 0;
        public ulong hCToNo => hCT?.GetObjectNo() ?? 0;
        public ulong gCToNo => gCT?.GetObjectNo() ?? 0;

        public string CCAd => CC?.Ad ?? "-";
        public string hCTAd => hCT?.Ad ?? "-";
        public string gCTAd => gCT?.Ad ?? "-";
        public string Tarih => string.Format(CultureInfo.CreateSpecificCulture("tr-TR"), "{0:dd.MMM.ddd}", Trh);
        public string TrhS => Trh.ToString("s");    // Sortable 
        //CultureInfo.CreateSpecificCulture("de-DE")

        public long CC_PK => CC?.PK ?? 0;
        public long hCT_PK => hCT?.PK ?? 0;
        public long gCT_PK => gCT?.PK ?? 0;

        public string TrhWeekEoO
        {
            get
            {
                CultureInfo myCI = new CultureInfo("tr-TR");
                Calendar mc = myCI.Calendar;
                var w = mc.GetWeekOfYear(Trh, myCI.DateTimeFormat.CalendarWeekRule, myCI.DateTimeFormat.FirstDayOfWeek) - 1;
                return w % 2 == 0 ? "E" : "O";
            }
        }

        public string hWL
        {
            get
            {
                if (hP > gP)
                    return "W";
                else if (hP < gP)
                    return "L";
                return "X";
            }
        }
        public string gWL
        {
            get
            {
                if (hP > gP)
                    return "L";
                else if (hP < gP)
                    return "W";
                return "X";
            }
        }
    }

    [Database]
    public class CETP    // Competition Event Team Players (Siralama)
    {
        public ulong oNo => this.GetObjectNo();

        public CC CC { get; set; }
        public CET CET { get; set; }
        public CT CT { get; set; }
        public string HoG { get; set; }
        public string SoD { get; set; }
        public int Idx { get; set; }
        public PP PP { get; set; }

        public string CCAd => CC?.Ad ?? "-";
        public string CTAd => CT?.Ad ?? "-";
        public string PPAd => PP?.Ad ?? "-";

        public long CC_PK => CC?.PK ?? 0;
        public long CET_PK => CET?.PK ?? 0;
        public long CT_PK => CT?.PK ?? 0;
        public long PP_PK => PP?.PK ?? 0;

    }
    // Kayit yoksa H&G takimlari icin CTP'den olusturulur
    // Siralama bitirildikten sonra Onaylanarak kapatilir.
    // Her iki takim da onay (hPok/gPok) verdikten sonra CETR olusturulur


    [Database]
    public class CETR    // CompetitionEventTeam Player Results
    {
        public ulong oNo => this.GetObjectNo();

        public CC CC { get; set; }
        public CET CET { get; set; }
        public CT CT { get; set; }
        public string HoG { get; set; }
        public string SoD { get; set; }
        public int Idx { get; set; }
        public PP PP { get; set; }

        //public PRH PRH { get; set; }
        public RH RH { get; set; }

        public DateTime Trh { get; set; }

        public bool OynDis { get; set; }    // Oyuncu Diskalifiye Rank hesaplanmayacak, KULLANILMIYOR
        // Setlerde kazandigi sayilar
        public int S1W { get; set; }
        public int S2W { get; set; }
        public int S3W { get; set; }
        public int S4W { get; set; }
        public int S5W { get; set; }

        // Mac bittikten sonra hesaplanacak
        public int SW { get; set; }     // Set Won
        public int SL { get; set; }     //     Lost
        public int MW { get; set; }     // Mac Won
        public int ML { get; set; }     //     Lost

        //public int    PW   { get; set; }     // Puan Won
        public string Snc1 { get; set; }    // 11-5, 8-11, 12-10, 11-0
        public string Snc2 { get; set; }    // [3-1] –5,+8,–10,–0    [-]Aldigi sette verdigi sayi, [+]Verdigi sette aldigi sayi

        public string CCAd => CC?.Ad ?? "-";
        public string CTAd => CT?.Ad ?? "-";
        public string PPAd => PP?.Ad ?? "-";
        public string TrhS => Trh.ToString("s");    // Sortable 

        public long CC_PK => CC?.PK ?? 0;
        public long CT_PK => CT?.PK ?? 0;
        public long CET_PK => CET?.PK ?? 0;
        public long PP_PK => PP?.PK ?? 0;

    }
    // Oyuncu siralamasi H&G takimlari tarafindan bitirilip onaylandiktan sonra CETP'den olusturulur.

    // Kullanilmiyor
    /*
    [Database]
    public class PRH     // Player Rank History
    {
        public PP PP { get; set; }     // Target Oyuncu
        public PP rPP { get; set; }    // Rakip

        public DateTime Trh { get; set; }

        public int Won { get; set; }    // -1:Kaybetti, 0:Oynanmadi, +1:Kazandi
        public int pRnk { get; set; }    // Macdan Onceki Rank.
        public int NOPX { get; set; }   // NumberOfPointsExchange between players. -:Kaybetti, 0:Oynanmadi, +:Kazandi
        public int Rnk { get; set; }    // Macdan Sonraki Rank. (Rnk = prvRnk + NOPX)

        public string PPAd => PP?.Ad ?? "-";
        public string rPPAd => rPP?.Ad ?? "-";

        public int prvRnk
        {
            get
            {
                if (PP.Ad.StartsWith("∞"))
                    return 0;
                // Ayni tarihde baska maci olabilir zamani da olmali (Ayni zamanda iki maci olamaz)
                var p = Db.SQL<PRH>("select m from BDB.PRH m where m.PP = ? and m.Trh < ? order by m.Trh desc", this.PP, this.Trh).FirstOrDefault();
                return p?.Rnk ?? PP.RnkBaz;
            }
        }
        public int prvRnkRkp
        {
            get
            {
                if (rPP.Ad.StartsWith("∞"))
                    return 0;
                // Ayni tarihde baska maci olabilir zamani da olmali (Ayni zamanda iki maci olamaz)
                var p = Db.SQL<PRH>("select m from BDB.PRH m where m.PP = ? and m.Trh < ? order by m.Trh desc", this.rPP, this.Trh).FirstOrDefault();
                return p?.Rnk ?? rPP.RnkBaz;
            }
        }

        public int compNOPX
        {
            get
            {
                int NOPX = 0;
                if(PP.Ad.StartsWith("∞") || rPP.Ad.StartsWith("∞"))   // Oyunculardan biri diskalifiye ise Rank hesaplama
                    return NOPX;

                if (this.Won == 0)
                        return NOPX;

                int Rnk = this.prvRnk;
                int RnkRkp = this.prvRnkRkp;

                int PS = 0; // Point Spread between players
                int ER = 0; // ExpectedResult
                int UR = 0; // UpsetResult

                // Compute
                PS = Math.Abs(Rnk - RnkRkp);

                if (PS < 13)
                {
                    ER = 8;
                    UR = 8;
                }
                else if (PS < 38)
                {
                    ER = 7;
                    UR = 10;
                }
                else if (PS < 63)
                {
                    ER = 6;
                    UR = 13;
                }
                else if (PS < 88)
                {
                    ER = 5;
                    UR = 16;
                }
                else if (PS < 113)
                {
                    ER = 4;
                    UR = 20;
                }
                else if (PS < 138)
                {
                    ER = 3;
                    UR = 25;
                }
                else if (PS < 163)
                {
                    ER = 2;
                    UR = 30;
                }
                else if (PS < 188)
                {
                    ER = 2;
                    UR = 35;
                }
                else if (PS < 213)
                {
                    ER = 1;
                    UR = 40;
                }
                else if (PS < 238)
                {
                    ER = 1;
                    UR = 45;
                }
                else
                {
                    ER = 0;
                    UR = 50;
                }
                if (Rnk >= RnkRkp)   // Target: Iyi
                {
                    if (Won > 0)     // Expected: Target kazanmis
                        NOPX += ER;
                    else             // Upset: Target kaybetmis
                        NOPX -= UR;
                }
                else                 // Rakip: Iyi
                {
                    if (Won < 0)     // Expected: Rakip kazanmis
                        NOPX -= ER;
                    else             // Upset: Rakip kaybetmis
                        NOPX += UR;
                }

                return NOPX;
            }
        }

        public PRH()
        {
            Won = 0;
            Rnk = 0;
        }
    }
    // CETR (Veya baska Turnuva) yaratildiginda bu kayit da yaratilacak, Sonuc girildiginde NOPX ve Rnk hesaplanarak buraya yazilacak
    */

    [Database]
    public class RH     // Rank History
    {
        public ulong oNo => this.GetObjectNo();

        public DateTime Trh { get; set; }
        public CC CC { get; set; }

        public PP hPP { get; set; }
        public int hWon { get; set; }       // +1 Kazandi -1 Kaybetti 0 Oynanmadi
        public int hpRnk { get; set; }
        public int hNOPX { get; set; }

        public PP gPP { get; set; }
        public int gWon { get; set; }
        public int gpRnk { get; set; }
        public int gNOPX { get; set; }

        // Grup Rank
        public int hpRnk2 { get; set; }
        public int hNOPX2 { get; set; }
        public int gpRnk2 { get; set; }
        public int gNOPX2 { get; set; }
    }

    [Database]
    public class MAC
    {
        // CETR+RH yerine kullanilacak
        public CC CC { get; set; }
        public CET CET { get; set; }    // Takim Event
        //public CET CEF { get; set; }    // Ferdi Event

        public DateTime Trh { get; set; }
        public string SoD { get; set; } // Single or Double
        public int Idx { get; set; }

        // Home
        public PP hPP1 { get; set; }
        public PP hPP2 { get; set; }    // Double ise 2.Oyuncu
        public int hDrm { get; set; }   // 0:Oynadi, 1:MacaCikmadi, 2:SiralamaHatasi
        public int hS1W { get; set; }   // 1.Sette Alidigi/Won Sayi
        public int hS2W { get; set; }
        public int hS3W { get; set; }
        public int hS4W { get; set; }
        public int hS5W { get; set; }
        public int hSW { get; set; }    // Kazandigi Set Miktari
        public int hMW { get; set; }    // Kazandigi Mac (0/1)
        public int hMP { get; set; }    // Mac Puani (S/D, L/W ve Event'e gore degisir)
        public int hpRnk { get; set; }  // Previous Rank Global
        public int hNOPX { get; set; }  // PointExcahnage

        // Guest
        public PP gPP1 { get; set; }
        public PP gPP2 { get; set; }    // Double ise 2.Oyuncu
        public int gDrm { get; set; }   // 0:Oynadi, 1:MacaCikmadi, 2:SiralamaHatasi
        public int gS1W { get; set; }   // 1.Sette Alidigi/Won Sayi
        public int gS2W { get; set; }
        public int gS3W { get; set; }
        public int gS4W { get; set; }
        public int gS5W { get; set; }
        public int gSW { get; set; }    // Kazandigi Set Miktari
        public int gMW { get; set; }    // Kazandigi Mac (0/1)
        public int gMP { get; set; }    // Mac Puani
        public int gpRnk { get; set; }  // Previous Rank Global
        public int gNOPX { get; set; }  // PointExcahnage
    }

}