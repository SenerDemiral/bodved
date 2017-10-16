using System;
using Starcounter;

namespace BDB
{
    [Database]
    public class CC    // Competitions
    {
        public ulong oNo => this.GetObjectNo();

        public string Ad { get; set; }
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

        public string Ad { get; set; }
        public string Sex { get; set; }
        public int DgmYil { get; set; }
        public string Tel { get; set; }
        public string eMail { get; set; }
        public DateTime KytTrh { get; set; }

        public string KytTarih => string.Format("{0:s}", KytTrh);

        public PP()
        {
            Ad = "";
            Sex = "E";
            DgmYil = 1960;
            KytTrh = DateTime.Now;
        }
    }
    // Global, Oyuncu oynayabilmesi icin burada tanimlanmalidir.
    // Hangi takimda oynayacagi CTP de belirtilir.

    [Database]
    public class CT    // Competition Teams
    {
        public ulong oNo => this.GetObjectNo();

        public CC CC { get; set; }
        public string Ad { get; set; }
        public PP K1 { get; set; }
        public PP K2 { get; set; }
        public string Adres { get; set; }
        public string Pw { get; set; }

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
        public string Tarih => string.Format("{0:yyyy-MM-dd}", Trh);

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

        public CC     CC  { get; set; }
        public CET    CET { get; set; }
        public CT     CT  { get; set; }
        public string HoG { get; set; }
        public string SoD { get; set; }
        public int    Idx { get; set; }
        public PP     PP  { get; set; }

        // Setlerde kazandigi sayilar
        public int S1W { get; set; }
        public int S2W { get; set; }
        public int S3W { get; set; }
        public int S4W { get; set; }
        public int S5W { get; set; }

        // Mac bittikten sonra hesaplanacak
        public int    SW   { get; set; }     // Set Won
        public int    SL   { get; set; }     // Set Lost
        public int    PW   { get; set; }     // Puan Won
        public string Snc1 { get; set; }    // 11-5, 8-11, 12-10, 11-0
        public string Snc2 { get; set; }    // [3-1] –5,+8,–10,–0    [-]Aldigi sette verdigi sayi, [+]Verdigi sette aldigi sayi

        public string CCAd => CC?.Ad ?? "-";
        public string CTAd => CT?.Ad ?? "-";
        public string PPAd => PP?.Ad ?? "-";
    }
    // Oyuncu siralamasi H&G takimlari tarafindan bitirilip onaylandiktan sonra CETP'den olusturulur.

}