using System;
using Starcounter;

namespace bodved
{
    class Program
    {
        static void Main()
        {
            var HTML = @"<!DOCTYPE html>
				<html>
				<head>
					<meta charset=""utf-8"">
				    <meta name=""viewport"" content=""width=device-width, initial-scale=1, shrink-to-fit=yes"">
					<title>{0}</title>
					
                    <script src=""/sys/webcomponentsjs/webcomponents.min.js""></script>
					<script src=""/sys/document-register-element/build/document-register-element.js""></script>

                    <script>
                        window.Polymer = {{
                            dom: ""shadow""
                        }};
                    </script>

					<link rel=""import"" href=""/sys/polymer/polymer.html"">
					<link rel=""import"" href=""/sys/starcounter.html"">
					<link rel=""import"" href=""/sys/starcounter-include/starcounter-include.html"">
					<!--link rel=""import"" href=""/sys/starcounter-debug-aid/src/starcounter-debug-aid.html""-->
					

                    <script src=""/sys/thenBy.js""></script>
                    <script src=""/sys/redips-drag-min.js""></script>
					<link rel=""stylesheet"" href=""/sys/redips-style.css"">
					
                    <link rel=""stylesheet"" href=""/sys/Bodved.css"">
				
				</head>


                <body>
                    <template is=""dom-bind"" id=""puppet-root"">
                    <starcounter-include view-model=""{{{{model}}}}""></starcounter-include>
                    </template>
                    <puppet-client ref=""puppet-root"" remote-url=""{1}""></puppet-client>
                    <!--starcounter-debug-aid></starcounter-debug-aid-->
                </body>
				</html>";


            Application.Current.Use(new HtmlFromJsonProvider());
            Application.Current.Use(new PartialToStandaloneHtmlProvider(HTML));
            Handle.GET("/bodved/init", () => {
                popTable();
                return "Init: OK";
            });

            Handle.GET("/bodved", () => { return Self.GET("/bodved/MainPage"); });

            Handle.GET("/bodved/partial/MainPage", () => new MainPage());
            Handle.GET("/bodved/MainPage", () => WrapPage<MainPage>("/bodved/partial/MainPage"));

            Handle.GET("/bodved/partial/PPpage", () => new PPpage());
            Handle.GET("/bodved/PPpage", () => WrapPage<PPpage>("/bodved/partial/PPpage"));

            Handle.GET("/bodved/partial/CCpage", () => new CCpage());
            Handle.GET("/bodved/CCpage", () => WrapPage<CCpage>("/bodved/partial/CCpage"));

            Handle.GET("/bodved/partial/CTpage/{?}", (string CCoNo) => new CTpage() { CCoNo = $"{CCoNo}" });
            Handle.GET("/bodved/CTpage/{?}", (string CCoNo) => WrapPage<CTpage>($"/bodved/partial/CTpage/{CCoNo}"));

            Handle.GET("/bodved/partial/CTPpage/{?}", (string CToNo) => new CTPpage() { CToNo = $"{CToNo}" });
            Handle.GET("/bodved/CTPpage/{?}", (string CToNo) => WrapPage<CTPpage>($"/bodved/partial/CTPpage/{CToNo}"));

            Handle.GET("/bodved/partial/CETpage/{?}", (string CCoNo) => new CETpage() { CCoNo = $"{CCoNo}" });
            Handle.GET("/bodved/CETpage/{?}", (string CCoNo) => WrapPage<CETpage>($"/bodved/partial/CETpage/{CCoNo}"));

            Handle.GET("/bodved/partial/CETPpage/{?}/{?}", (string CEToNo, string CToNo) => new CETPpage() { CEToNo = $"{CEToNo}", CToNo = $"{CToNo}" });
            Handle.GET("/bodved/CETPpage/{?}/{?}", (string CEToNo, string CToNo) => WrapPage<CETPpage>($"/bodved/partial/CETPpage/{CEToNo}/{CToNo}"));



            Handle.GET("/bodved/partial/Deneme/{?}", (string CCoNo) => new Deneme() { CCoNo=$"{CCoNo}" });
            Handle.GET("/bodved/Deneme/{?}", (string CCoNo) => WrapPage<Deneme>($"/bodved/partial/Deneme/{CCoNo}"));
        }


        public static MasterPage GetMasterPageFromSession()
        {
            /*
            if (Session.Current == null)
            {
                Session.Current = new Session(Session.Flags.PatchVersioning);
            }
            */
            Session.Ensure();

            //MasterPage master = Session.Current.Data as MasterPage;
            MasterPage master = Session.Current.Store["App"] as MasterPage;

            if (master == null)
            {
                master = new MasterPage();
                //Session.Current.Data = master;
                Session.Current.Store["App"] = master;
            }

            return master;
        }

        private static Json WrapPage<T>(string partialPath) where T : Json
        {
            var master = GetMasterPageFromSession();

            /*
            if (master.CurrentPage != null && master.CurrentPage.GetType().Equals(typeof(T)))
            {
                return master;
            }
            */
            master.CurrentPage = Self.GET(partialPath);

            //if (master.CurrentPage.Data == null)
            {
                master.CurrentPage.Data = null; //trick to invoke OnData in partial
            }

            return master;
        }

        private static void popTable()
        {
            Db.Transact(() =>
            {
                // Turnuvalar
                var cc1 = new BDB.CC() { Ad = "17-18 1.Lig" };
                var cc2 = new BDB.CC() { Ad = "17-18 2.Lig" };
                var cc3 = new BDB.CC() { Ad = "17-18 3.Lig" };

                // Turnuva Takimlari
                // 1.Lig
                new BDB.CT() { CC = cc1, Ad = "Bitez-1" };
                new BDB.CT() { CC = cc1, Ad = "Delfi-1" };
                new BDB.CT() { CC = cc1, Ad = "Dragons-1" };
                new BDB.CT() { CC = cc1, Ad = "Gümüşlük-1" };
                new BDB.CT() { CC = cc1, Ad = "Kekik-1" };
                new BDB.CT() { CC = cc1, Ad = "MitNarKop-1" };
                var pn1 = new BDB.CT() { CC = cc1, Ad = "Ponpin-1" };
                var pr1 = new BDB.CT() { CC = cc1, Ad = "Promil-1" };
                new BDB.CT() { CC = cc1, Ad = "Yahşi-1" };
                new BDB.CT() { CC = cc1, Ad = "Yalıkavak-1" };
                // 2.Lig 
                new BDB.CT() { CC = cc2, Ad = "Bitez-2" };
                new BDB.CT() { CC = cc2, Ad = "Bosku-2" };
                new BDB.CT() { CC = cc2, Ad = "Delfi-2A" };
                new BDB.CT() { CC = cc2, Ad = "Delfi-2B" };
                new BDB.CT() { CC = cc2, Ad = "Dragons-2" };
                new BDB.CT() { CC = cc2, Ad = "Gümüşlük-2" };
                new BDB.CT() { CC = cc2, Ad = "Kekik-2" };
                new BDB.CT() { CC = cc2, Ad = "KutayReno-2" };
                new BDB.CT() { CC = cc2, Ad = "MitNarKop-2" };
                new BDB.CT() { CC = cc2, Ad = "Nane-2" };
                new BDB.CT() { CC = cc2, Ad = "Newpon-2A" };
                var np2B = new BDB.CT() { CC = cc2, Ad = "Newpon-2B" };
                new BDB.CT() { CC = cc2, Ad = "Nomads-2" };
                new BDB.CT() { CC = cc2, Ad = "Onbar-2" };
                new BDB.CT() { CC = cc2, Ad = "Ponpin-2" };
                new BDB.CT() { CC = cc2, Ad = "Promil-2" };
                new BDB.CT() { CC = cc2, Ad = "Sağlık-2" };
                new BDB.CT() { CC = cc2, Ad = "Yahşi-2" };
                new BDB.CT() { CC = cc2, Ad = "Yalıkavak-2" };
                // 3.Lig
                new BDB.CT() { CC = cc3, Ad = "Bitez-3" };
                new BDB.CT() { CC = cc3, Ad = "Bosku-3" };
                new BDB.CT() { CC = cc3, Ad = "Delfi-3A" };
                new BDB.CT() { CC = cc3, Ad = "Delfi-3B" };
                new BDB.CT() { CC = cc3, Ad = "Gümüşlük-3" };
                new BDB.CT() { CC = cc3, Ad = "Kekik-3" };
                new BDB.CT() { CC = cc3, Ad = "MiaMare-3" };
                new BDB.CT() { CC = cc3, Ad = "MitNarKop-3" };
                new BDB.CT() { CC = cc3, Ad = "Nane-3" };
                new BDB.CT() { CC = cc3, Ad = "Onbar-3" };
                new BDB.CT() { CC = cc3, Ad = "Waffelhane-3" };
                new BDB.CT() { CC = cc3, Ad = "Yahşi-3" };
                new BDB.CT() { CC = cc3, Ad = "Yalıkavak-3" };

                // Oyuncular

                // Promil
                var pr11 = new BDB.PP() { Ad = "Şener DEMİRAL" };
                var pr12 = new BDB.PP() { Ad = "Ümit ÇETİNALP" };
                var pr13 = new BDB.PP() { Ad = "Erhan DOĞRU" };
                var pr14 = new BDB.PP() { Ad = "Göksan AKAY K2" };
                var pr15 = new BDB.PP() { Ad = "Ahmethan ACET" };
                var pr16 = new BDB.PP() { Ad = "Yenal EGE" };
                var pr17 = new BDB.PP() { Ad = "Emre ESMER" };
                var pr18 = new BDB.PP() { Ad = "Hakan UĞURLU K1" };
                var pr19 = new BDB.PP() { Ad = "Alptekin ARAT" };
                var pr1A = new BDB.PP() { Ad = "Şevket TAYHAN" };
                var pr1B = new BDB.PP() { Ad = "MehmetAli " };
                var pr1C = new BDB.PP() { Ad = "Mustafa " };
                new BDB.CTP() { CC = cc1, CT = pr1, PP = pr11 };
                new BDB.CTP() { CC = cc1, CT = pr1, PP = pr12 };
                new BDB.CTP() { CC = cc1, CT = pr1, PP = pr13 };
                new BDB.CTP() { CC = cc1, CT = pr1, PP = pr14 };
                new BDB.CTP() { CC = cc1, CT = pr1, PP = pr15 };
                new BDB.CTP() { CC = cc1, CT = pr1, PP = pr16 };
                new BDB.CTP() { CC = cc1, CT = pr1, PP = pr17 };
                new BDB.CTP() { CC = cc1, CT = pr1, PP = pr18 };
                new BDB.CTP() { CC = cc1, CT = pr1, PP = pr19 };
                new BDB.CTP() { CC = cc1, CT = pr1, PP = pr1A };
                new BDB.CTP() { CC = cc1, CT = pr1, PP = pr1B };
                new BDB.CTP() { CC = cc1, CT = pr1, PP = pr1C };

                // Ponpin-1
                var pn11 = new BDB.PP() { Ad = "Cengiz ÇORUMLU K1" };
                var pn12 = new BDB.PP() { Ad = "Kenan TURSAK" };
                var pn13 = new BDB.PP() { Ad = "Arif ERGUVAN" };
                var pn14 = new BDB.PP() { Ad = "Bülent AKDERE" };
                var pn15 = new BDB.PP() { Ad = "Akif KURTULMUŞ" };
                var pn16 = new BDB.PP() { Ad = "İlyas ERBEM K2" };
                var pn17 = new BDB.PP() { Ad = "Ertuğ TUTAL" };
                var pn18 = new BDB.PP() { Ad = "Tümer ERENLER" };
                var pn19 = new BDB.PP() { Ad = "Uğur KESİCİ" };
                var pn1A = new BDB.PP() { Ad = "Tansel ERGUN" };
                var pn1B = new BDB.PP() { Ad = "Ara KAMAR" };
                var pn1C = new BDB.PP() { Ad = "Çağlar ULUDAMAR" };
                var pn1D = new BDB.PP() { Ad = "İbrahim ONURLU" };
                var pn1E = new BDB.PP() { Ad = "Ali HAGİ" };
                var pn1F = new BDB.PP() { Ad = "Turan BARAN" };
                var pn1G = new BDB.PP() { Ad = "Rüştü TEZCAN" };
                var pn1H = new BDB.PP() { Ad = "Şebnem KAPANAKİ" };
                new BDB.CTP() { CC = cc1, CT = pn1, PP = pn11 };
                new BDB.CTP() { CC = cc1, CT = pn1, PP = pn12 };
                new BDB.CTP() { CC = cc1, CT = pn1, PP = pn13 };
                new BDB.CTP() { CC = cc1, CT = pn1, PP = pn14 };
                new BDB.CTP() { CC = cc1, CT = pn1, PP = pn15 };
                new BDB.CTP() { CC = cc1, CT = pn1, PP = pn16 };
                new BDB.CTP() { CC = cc1, CT = pn1, PP = pn17 };
                new BDB.CTP() { CC = cc1, CT = pn1, PP = pn18 };
                new BDB.CTP() { CC = cc1, CT = pn1, PP = pn19 };
                new BDB.CTP() { CC = cc1, CT = pn1, PP = pn1A };
                new BDB.CTP() { CC = cc1, CT = pn1, PP = pn1B };
                new BDB.CTP() { CC = cc1, CT = pn1, PP = pn1C };
                new BDB.CTP() { CC = cc1, CT = pn1, PP = pn1D };
                new BDB.CTP() { CC = cc1, CT = pn1, PP = pn1E };
                new BDB.CTP() { CC = cc1, CT = pn1, PP = pn1F };
                new BDB.CTP() { CC = cc1, CT = pn1, PP = pn1G };
                new BDB.CTP() { CC = cc1, CT = pn1, PP = pn1H };

                // Newpon 2B
                var np2B1 = new BDB.PP() { Ad = "Sayım AKAÇIK K1" };
                var np2B2 = new BDB.PP() { Ad = "Sadullah TEOMAN K2" };
                var np2B3 = new BDB.PP() { Ad = "Mahmut ŞİMŞEK" };
                var np2B4 = new BDB.PP() { Ad = "Selçuk ÜNSAL" };
                var np2B5 = new BDB.PP() { Ad = "Kamil KARATAY" };
                var np2B6 = new BDB.PP() { Ad = "Salim POYRAZ" };
                var np2B7 = new BDB.PP() { Ad = "Bülent KÖPRÜLÜ" };
                var np2B8 = new BDB.PP() { Ad = "Ahmet ARAÇ" };
                var np2B9 = new BDB.PP() { Ad = "Murat KORKUT" };
                var np2BA = new BDB.PP() { Ad = "Derya İREN" };
                var np2BB = new BDB.PP() { Ad = "Uğur ERKMEN" };
                var np2BC = new BDB.PP() { Ad = "Birnur AKAN" };
                var np2BD = new BDB.PP() { Ad = "Sevinç YILDIRIM" };
                var np2BE = new BDB.PP() { Ad = "Serpil POYRAZ" };
                new BDB.CTP() { CC = cc2, CT = np2B, PP = np2B1 };
                new BDB.CTP() { CC = cc2, CT = np2B, PP = np2B2 };
                new BDB.CTP() { CC = cc2, CT = np2B, PP = np2B3 };
                new BDB.CTP() { CC = cc2, CT = np2B, PP = np2B4 };
                new BDB.CTP() { CC = cc2, CT = np2B, PP = np2B5 };
                new BDB.CTP() { CC = cc2, CT = np2B, PP = np2B6 };
                new BDB.CTP() { CC = cc2, CT = np2B, PP = np2B7 };
                new BDB.CTP() { CC = cc2, CT = np2B, PP = np2B8 };
                new BDB.CTP() { CC = cc2, CT = np2B, PP = np2B9 };
                new BDB.CTP() { CC = cc2, CT = np2B, PP = np2BA };
                new BDB.CTP() { CC = cc2, CT = np2B, PP = np2BB };
                new BDB.CTP() { CC = cc2, CT = np2B, PP = np2BC };
                new BDB.CTP() { CC = cc2, CT = np2B, PP = np2BD };
                new BDB.CTP() { CC = cc2, CT = np2B, PP = np2BE };

                // Bitez
                new BDB.PP() { Ad = "Faruk ULUSOY" };
                new BDB.PP() { Ad = "Cem BAŞARGAN" };
                // Bosku
                new BDB.PP() { Ad = "Kemal BERBER" };
                new BDB.PP() { Ad = "Songül MELEK" };
                // Delfi
                new BDB.PP() { Ad = "Mehmet DAĞOĞLU" };
                // Dragons
                new BDB.PP() { Ad = "Nihat ÜMMETOĞLU" };
                new BDB.PP() { Ad = "Çağatay ŞAŞMAZ" };
                // Yahşi
                new BDB.PP() { Ad = "Levent DİĞİLİ" };
                new BDB.PP() { Ad = "Vedat GÖKALP" };
                new BDB.PP() { Ad = "Özgür ÇAM" };
                // Gümüşlük
                new BDB.PP() { Ad = "Behçet AK" };
                new BDB.PP() { Ad = "Levent ALTAN" };
                new BDB.PP() { Ad = "Mustafa FİL" };
                // Kekik
                new BDB.PP() { Ad = "Murat ÇAKIR" };
                new BDB.PP() { Ad = "Bahadır TULUMCUOĞLU" };
                new BDB.PP() { Ad = "Hasan ACIOLUK" };
                // Kopuyos
                new BDB.PP() { Ad = "Tülay ŞAHAN" };
                // KutayReno
                new BDB.PP() { Ad = "Sefer NAMRUK" };
                new BDB.PP() { Ad = "Yaşar ASLAN" };
                // MiaMare
                new BDB.PP() { Ad = "Nizam ATAÇ" };
                // Mitos
                new BDB.PP() { Ad = "Ömer AYTAN" };
                // Nane
                new BDB.PP() { Ad = "Kader ONAY" };
                new BDB.PP() { Ad = "Oktay AKÇA" };
                // Narkos
                new BDB.PP() { Ad = "Hüseyin ÖZGÜL" };
                // Nomads
                new BDB.PP() { Ad = "Şükrü TÖRÜN" };
                new BDB.PP() { Ad = "AliRıza KÖKGİL" };
                // Sağlık
                new BDB.PP() { Ad = "Edip AYDOĞAN" };
                // Wafellhane
                new BDB.PP() { Ad = "Gül DİLBER" };
                // Yalıkavak
                new BDB.PP() { Ad = "Tahir ÇELİKESEN" };
                new BDB.PP() { Ad = "Oğuz DEVELİ" };
                new BDB.PP() { Ad = "Şükran KILIÇASLAN" };
            });
        }

    }
}