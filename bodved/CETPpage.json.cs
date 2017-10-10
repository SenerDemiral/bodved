using Starcounter;

namespace bodved
{
    partial class CETPpage : Json
    {
        protected override void OnData()
        {
            base.OnData();

            if ((Root as MasterPage).Login.LI && (Root as MasterPage).Login.Id == -1)
                this.canMdfy = true;

            // Asagidaki sartlarda buraya girise izin verme
                // Bu takim'in yetkilisi Login olan kullanicisi degilse
                // Sirlama yapilip onaylamis ise
            // Yetkili isterse burada Sirlamayi bitirdigine dair onay verir.
            // Her ikisi de onay vermis ise bu tablo kullanilarak CETR olusturulur.
            // Sirlama yapilip onaylanmadan Mac sonuclari (CETR) girilemez.
            if (canMdfy)
            {
            }

            var cet = Db.FromId<BDB.CET>(ulong.Parse(CEToNo));
            var ct = Db.FromId<BDB.CT>(ulong.Parse(CToNo));     // Bu Takim'in siralamasi yapiliyor.
            CapHdr = $"{cet.Tarih} {ct.Ad}";

            var cetps = Db.SQL<BDB.CETP>("select c from CETP c where c.CET = ? and c.CT = ?", cet, ct);

            // Single lari yerlestir

            // Double lari yerlestir

        }


    }
}
