﻿using System;
using System.Linq;
using Starcounter;
using System.Globalization;

namespace BDB
{
    [Database]
    public class NTC
    {
        public ulong oNo => this.GetObjectNo();

        public string Onc { get; set; }     // Oncelik 1-9
        public DateTime Trh { get; set; }
        public string Ad { get; set; }
        public string Link { get; set; }

        public string Tarih => string.Format(CultureInfo.CreateSpecificCulture("tr-TR"), "{0:dd MMM ddd}", Trh);
    }

    [Database]
    public class CC    // Competitions
    {
        public ulong oNo => this.GetObjectNo();

        public string ID { get; set; }
        public string Ad { get; set; }
        public string Idx { get; set; }
        public string Skl { get; set; }    // Takim/Ferdi
        public string Grp { get; set; }    // Birden cok turnuvada oynayan Oyunculari takip icin

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

        public string ID { get; set; }
        public string Ad { get; set; }
        public string Sex { get; set; }
        public int DgmYil { get; set; }
        public string Tel { get; set; }
        public string eMail { get; set; }
        public DateTime KytTrh { get; set; }

        public string KytTarih => string.Format("{0:s}", KytTrh);

        public int RnkBaz { get; set; }
        public int Rnk { get; set; }
        public int Sra { get; set; }    // Rank'a gore sirasi

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

        public int curRnk
        {
            get
            {
                var p = Db.SQL<PRH>("select m from BDB.PRH m where m.PP = ? and m.Trh < ? order by m.Trh desc", this, DateTime.MaxValue).FirstOrDefault();
                return p?.Rnk ?? 0; // this.RnkBaz;
            }
        }

        public string oCTs
        {
            get     // Oynadigi dTakimlar
            {
                string oCTs = "";
                foreach (var r in Db.SQL<CTP>("select c from BDB.CTP c where c.PP = ?", this))
                {
                    oCTs += r.CC.ID + "-" + r.CT.ID + " ";
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
    // Global, Oyuncu oynayabilmesi icin burada tanimlanmalidir.
    // Hangi takimda oynayacagi CTP de belirtilir.

    [Database]
    public class CT    // Competition Teams
    {
        public ulong oNo => this.GetObjectNo();

        public CC CC { get; set; }
        public string ID { get; set; }
        public string Ad { get; set; }
        public PP K1 { get; set; }
        public PP K2 { get; set; }
        public string Adres { get; set; }
        public string Pw { get; set; }

        public int tP { get; set; }  // Takim Puan
        public int oM { get; set; }  // Oynadigi Musabaka Sayisi
        public int aMP { get; set; } // Aldigi Musabaka Puani
        public int vMP { get; set; } // Verdigi Musabaka Puani
        public int fMP { get; set; } // Fark Aldigi-Verdigi Musabaka Puani


        public int oE { get; set; }
        public int aE { get; set; }
        public int vE { get; set; }
        public int aP { get; set; }
        public int vP { get; set; }

        public string CCAd => CC?.Ad ?? "-";
        public ulong K1oNo => K1?.GetObjectNo() ?? 0;
        public ulong K2oNo => K2?.GetObjectNo() ?? 0;
        public string K1Ad => K1?.Ad ?? "-";
        public string K2Ad => K2?.Ad ?? "-";


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
        public string PPid { get; set; }
        public int Idx { get; set; }

        public ulong PPoNo => PP?.GetObjectNo() ?? 0;
        public ulong CCoNo => CC?.GetObjectNo() ?? 0;
        public ulong CToNo => CT?.GetObjectNo() ?? 0;
        public string CCAd => CC?.Ad ?? "-";
        public string CTAd => CT?.Ad ?? "-";
        public string PPAd => PP?.Ad ?? "-";

        public string oCTs
        {
            get     // Oynadigi diger Takimlar
            {
                string oCTs = "";
                foreach( var r in Db.SQL<CTP>("select c from BDB.CTP c where c.PP = ? and c.CC.Grp = ?", this.PP, this.CC.Grp))
                {
                    if(r.CT.GetObjectNo() != this.CT.GetObjectNo())
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

        public CC CC { get; set; }
        public string ID { get; set; }
        public CT hCT { get; set; }
        public CT gCT { get; set; }
        public DateTime Trh { get; set; }

        public bool hPok { get; set; }  // Home Players Ok
        public bool gPok { get; set; }
        public bool Rok { get; set; }   // Results Ok. Home/Guest Onaylayabilir

        // H & G Sonuclari onayladiktan sonra hesaplanir
        public int hP { get; set; }    // Home  Musabaka Puan
        public int gP { get; set; }    // Guest 

        public int hPW { get; set; }    // Home Puan Win
        public int hMSW { get; set; }   //      Mac Single Win
        public int hMDW { get; set; }   //          Double

        public int gPW { get; set; }    // Guest Puan Win
        public int gMSW { get; set; }   //       Mac Single
        public int gMDW { get; set; }   //           Double

        public ulong CCoNo => CC?.GetObjectNo() ?? 0;
        public ulong hCToNo => hCT?.GetObjectNo() ?? 0;
        public ulong gCToNo => gCT?.GetObjectNo() ?? 0;

        public string CCAd => CC?.Ad ?? "-";
        public string hCTAd => hCT?.Ad ?? "-";
        public string gCTAd => gCT?.Ad ?? "-";
        public string Tarih => string.Format(CultureInfo.CreateSpecificCulture("tr-TR"), "{0:dd MMM ddd}", Trh);
        //public string Tarih2 => string.Format()
        //CultureInfo.CreateSpecificCulture("de-DE")
    }

    [Database]
    public class CETP    // Competition Event Team Players (Siralama)
    {
        public ulong oNo => this.GetObjectNo();

        public CC     CC  { get; set; }
        public CET    CET { get; set; }
        public CT     CT  { get; set; }
        public string HoG { get; set; }
        public string SoD { get; set; }
        public int    Idx { get; set; }
        public PP     PP  { get; set; }

        public string CCAd => CC?.Ad ?? "-";
        public string CTAd => CT?.Ad ?? "-";
        public string PPAd => PP?.Ad ?? "-";
    }
    // Kayit yoksa H&G takimlari icin CTP'den olusturulur
    // Siralama bitirildikten sonra Onaylanarak kapatilir.
    // Her iki takim da onay (hPok/gPok) verdikten sonra CETR olusturulur

    [Database]
    public class CETR    // Competition Event Team Results
    {
        public ulong oNo => this.GetObjectNo();

        public CC CC  { get; set; }
        public CET CET { get; set; }
        public CT CT  { get; set; }
        public string HoG { get; set; }
        public string SoD { get; set; }
        public int Idx { get; set; }
        public PP PP { get; set; }
        public PRH PRH { get; set; }
        public DateTime Trh { get; set; }

        public bool OynDis { get; set; }    // Oyuncu Diskalifiye Rank hesaplanmayacak
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
    }
    // Oyuncu siralamasi H&G takimlari tarafindan bitirilip onaylandiktan sonra CETP'den olusturulur.

    [Database]
    public class PRH     // Player Rank History
    {
        public PP PP { get; set; }     // Target Oyuncu
        public PP rPP { get; set; }    // Rakip

        public DateTime Trh { get; set; }

        public int Won { get; set; }    // -1:Kaybetti, 0:Oynanmadi, +1:Kazandi
        public int NOPX { get; set; }   // NumberOfPointsExchange between players. -:Kaybetti, 0:Oynanmadi, +:Kazandi

        public int Rnk { get; set; }    // Macdan Sonraki Rank. (Rnk = prvRnk + NOPX)

        public string PPAd => PP?.Ad ?? "-";
        public string rPPAd => rPP?.Ad ?? "-";

        public int prvRnk
        {
            get
            {
                if (PP.ID == "∞")
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
                if (rPP.ID == "∞")
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
                if(PP.ID == "∞" || rPP.ID == "∞")   // Oyunculardan biri diskalifiye ise Rank hesaplama
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

}