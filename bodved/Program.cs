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
					<link rel=""import"" href=""/sys/starcounter-debug-aid/src/starcounter-debug-aid.html"">
					

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
                    <starcounter-debug-aid></starcounter-debug-aid>
                </body>
				</html>";


            Application.Current.Use(new HtmlFromJsonProvider());
            Application.Current.Use(new PartialToStandaloneHtmlProvider(HTML));
            Handle.GET("/bodved/init", () => {
                popTable();
                return "Init: OK";
            });

            Handle.GET("/bodved", () => { return Self.GET("/bodved/MainPage"); });

            Handle.GET("/bodved/partial/AboutPage", () => new AboutPage());
            Handle.GET("/bodved/AboutPage", () => WrapPage<AboutPage>("/bodved/partial/AboutPage"));

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

            Handle.GET("/bodved/partial/CETRviewPage/{?}", (string CEToNo) => new CETRviewPage() { CEToNo = $"{CEToNo}" });
            Handle.GET("/bodved/CETRviewPage/{?}", (string CEToNo) => WrapPage<CETRviewPage>($"/bodved/partial/CETRviewPage/{CEToNo}"));

            Handle.GET("/bodved/partial/CETRinpPage/{?}", (string CEToNo) => new CETRinpPage() { CEToNo = $"{CEToNo}" });
            Handle.GET("/bodved/CETRinpPage/{?}", (string CEToNo) => WrapPage<CETRinpPage>($"/bodved/partial/CETRinpPage/{CEToNo}"));


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
                var dr1 = new BDB.CT() { CC = cc1, Ad = "Dragons-1" };
                var gm1 = new BDB.CT() { CC = cc1, Ad = "Gümüşlük-1" };
                new BDB.CT() { CC = cc1, Ad = "Kekik-1" };
                new BDB.CT() { CC = cc1, Ad = "MitNarKop-1" };
                var pn1 = new BDB.CT() { CC = cc1, Ad = "Ponpin-1" };
                var pr1 = new BDB.CT() { CC = cc1, Ad = "Promil-1" };
                var yh1 = new BDB.CT() { CC = cc1, Ad = "Yahşi-1" };
                var yk1 = new BDB.CT() { CC = cc1, Ad = "Yalıkavak-1" };
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
                var gm3 = new BDB.CT() { CC = cc3, Ad = "Gümüşlük-3" };
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

                // Dragons-1
                var dr11 = new BDB.PP() { Ad = "Nihat ÜMMETOĞLU" };
                var dr12 = new BDB.PP() { Ad = "Çağatay ŞAŞMAZ" };
                var dr13 = new BDB.PP() { Ad = "Tunç HIZAL" };
                var dr14 = new BDB.PP() { Ad = "Vahi GÜNER" };
                var dr15 = new BDB.PP() { Ad = "Ercan YEŞİÇİMEN" };
                var dr16 = new BDB.PP() { Ad = "Ali BİLGİN" };
                var dr17 = new BDB.PP() { Ad = "Recai ÇAKIR" };
                var dr18 = new BDB.PP() { Ad = "Gökhan HAMURCU" };
                var dr19 = new BDB.PP() { Ad = "Rıdvan GÖKBEL" };
                var dr1A = new BDB.PP() { Ad = "Yasin YILDIRIM" };
                var dr1B = new BDB.PP() { Ad = "Edip AYDOĞAN" };
                var dr1C = new BDB.PP() { Ad = "Ahmet KÜLAHÇIOĞLU" };
                var dr1D = new BDB.PP() { Ad = "Ferit CİLSİN" };
                var dr1E = new BDB.PP() { Ad = "Necip GÖKBEL" };
                var dr1F = new BDB.PP() { Ad = "Melike KOPARAN" };
                var dr1G = new BDB.PP() { Ad = "Kezban ŞAŞMAZ" };
                var dr1H = new BDB.PP() { Ad = "Dora ŞAŞMAZ" };
                var dr1I = new BDB.PP() { Ad = "Faruk GEDİKOĞLU" };
                var dr1J = new BDB.PP() { Ad = "Can DEMİR" };
                var dr1K = new BDB.PP() { Ad = "Halil OCAKLI" };
                new BDB.CTP() { CC = cc1, CT = dr1, PP = dr11 };
                new BDB.CTP() { CC = cc1, CT = dr1, PP = dr12 };
                new BDB.CTP() { CC = cc1, CT = dr1, PP = dr13 };
                new BDB.CTP() { CC = cc1, CT = dr1, PP = dr14 };
                new BDB.CTP() { CC = cc1, CT = dr1, PP = dr15 };
                new BDB.CTP() { CC = cc1, CT = dr1, PP = dr16 };
                new BDB.CTP() { CC = cc1, CT = dr1, PP = dr17 };
                new BDB.CTP() { CC = cc1, CT = dr1, PP = dr18 };
                new BDB.CTP() { CC = cc1, CT = dr1, PP = dr19 };
                new BDB.CTP() { CC = cc1, CT = dr1, PP = dr1A };
                new BDB.CTP() { CC = cc1, CT = dr1, PP = dr1B };
                new BDB.CTP() { CC = cc1, CT = dr1, PP = dr1C };
                new BDB.CTP() { CC = cc1, CT = dr1, PP = dr1D };
                new BDB.CTP() { CC = cc1, CT = dr1, PP = dr1E };
                new BDB.CTP() { CC = cc1, CT = dr1, PP = dr1F };
                new BDB.CTP() { CC = cc1, CT = dr1, PP = dr1G };
                new BDB.CTP() { CC = cc1, CT = dr1, PP = dr1H };
                new BDB.CTP() { CC = cc1, CT = dr1, PP = dr1I };
                new BDB.CTP() { CC = cc1, CT = dr1, PP = dr1J };
                new BDB.CTP() { CC = cc1, CT = dr1, PP = dr1K };

                // Gümüşlük-1
                var gm11 = new BDB.PP() { Ad = "Serdar KAYA" };
                var gm12 = new BDB.PP() { Ad = "Ümit YEKREK" };
                var gm13 = new BDB.PP() { Ad = "İsmail DÜZTAŞ" };
                var gm14 = new BDB.PP() { Ad = "Mustafa ÖZASLAN" };
                var gm15 = new BDB.PP() { Ad = "Levent ALTAN" };
                var gm16 = new BDB.PP() { Ad = "Hakkı ZIRH" };
                var gm17 = new BDB.PP() { Ad = "Behçet AK K1" };
                var gm18 = new BDB.PP() { Ad = "Hamit MENGÜÇ K2" };
                var gm19 = new BDB.PP() { Ad = "Serkan İLHAN" };
                var gm1A = new BDB.PP() { Ad = "Ali TÜRKELİ" };
                var gm1B = new BDB.PP() { Ad = "Metin SOYSAL" };
                var gm1C = new BDB.PP() { Ad = "Nicholas KARYDAKİS" };
                var gm1D = new BDB.PP() { Ad = "Fikret YÜCEL" };
                var gm1E = new BDB.PP() { Ad = "Fırat POLAT" };
                var gm1F = new BDB.PP() { Ad = "Süleyman KARTAL" };
                var gm1G = new BDB.PP() { Ad = "Belen ÜNAL" };
                var gm1H = new BDB.PP() { Ad = "Turgay ÇABALAR" };
                new BDB.CTP() { CC = cc1, CT = gm1, PP = gm11 };
                new BDB.CTP() { CC = cc1, CT = gm1, PP = gm12 };
                new BDB.CTP() { CC = cc1, CT = gm1, PP = gm13 };
                new BDB.CTP() { CC = cc1, CT = gm1, PP = gm14 };
                new BDB.CTP() { CC = cc1, CT = gm1, PP = gm15 };
                new BDB.CTP() { CC = cc1, CT = gm1, PP = gm16 };
                new BDB.CTP() { CC = cc1, CT = gm1, PP = gm17 };
                new BDB.CTP() { CC = cc1, CT = gm1, PP = gm18 };
                new BDB.CTP() { CC = cc1, CT = gm1, PP = gm19 };
                new BDB.CTP() { CC = cc1, CT = gm1, PP = gm1A };
                new BDB.CTP() { CC = cc1, CT = gm1, PP = gm1B };
                new BDB.CTP() { CC = cc1, CT = gm1, PP = gm1C };
                new BDB.CTP() { CC = cc1, CT = gm1, PP = gm1D };
                new BDB.CTP() { CC = cc1, CT = gm1, PP = gm1E };
                new BDB.CTP() { CC = cc1, CT = gm1, PP = gm1F };
                new BDB.CTP() { CC = cc1, CT = gm1, PP = gm1G };
                new BDB.CTP() { CC = cc1, CT = gm1, PP = gm1H };

                // Gümüşlük-3
                var gm31 = new BDB.PP() { Ad = "Kemal ÇINGI" };
                var gm32 = new BDB.PP() { Ad = "Gül GUZELBEY K2" };
                var gm33 = new BDB.PP() { Ad = "Fahrettin TÜRE" };
                var gm34 = new BDB.PP() { Ad = "Fikret YÜCAL" };
                var gm35 = new BDB.PP() { Ad = "Melih FİDAN" };
                var gm36 = new BDB.PP() { Ad = "Melike FELEKOĞLU" };
                var gm37 = new BDB.PP() { Ad = "Ahmet DURUKAN" };
                var gm38 = new BDB.PP() { Ad = "Beyhan BAĞDATLI" };
                var gm39 = new BDB.PP() { Ad = "Hilmi ACAR" };
                var gm3A = new BDB.PP() { Ad = "Ufuk TURHAN" };
                var gm3B = new BDB.PP() { Ad = "Sevinç ERSİN" };
                var gm3C = new BDB.PP() { Ad = "Umut YANIK" };
                var gm3D = new BDB.PP() { Ad = "Barlas GÖZE" };
                var gm3E = new BDB.PP() { Ad = "Neşe ODABAŞI" };
                var gm3F = new BDB.PP() { Ad = "Derya AYDIN" };
                new BDB.CTP() { CC = cc3, CT = gm3, PP = gm31 };
                new BDB.CTP() { CC = cc3, CT = gm3, PP = gm32 };
                new BDB.CTP() { CC = cc3, CT = gm3, PP = gm33 };
                new BDB.CTP() { CC = cc3, CT = gm3, PP = gm34 };
                new BDB.CTP() { CC = cc3, CT = gm3, PP = gm35 };
                new BDB.CTP() { CC = cc3, CT = gm3, PP = gm36 };
                new BDB.CTP() { CC = cc3, CT = gm3, PP = gm37 };
                new BDB.CTP() { CC = cc3, CT = gm3, PP = gm38 };
                new BDB.CTP() { CC = cc3, CT = gm3, PP = gm39 };
                new BDB.CTP() { CC = cc3, CT = gm3, PP = gm3A };
                new BDB.CTP() { CC = cc3, CT = gm3, PP = gm3B };
                new BDB.CTP() { CC = cc3, CT = gm3, PP = gm3C };
                new BDB.CTP() { CC = cc3, CT = gm3, PP = gm3D };
                new BDB.CTP() { CC = cc3, CT = gm3, PP = gm3E };
                new BDB.CTP() { CC = cc3, CT = gm3, PP = gm3F };

                // Yahşi-1
                var yh11 = new BDB.PP() { Ad = "Burak KANAT",       Tel = "530-119-0155" };
                var yh12 = new BDB.PP() { Ad = "Faruk ULUSOY",      Tel = "532-565-7188" };
                var yh13 = new BDB.PP() { Ad = "Tarık TURFAN",      Tel = "546-274-5584" };
                var yh14 = new BDB.PP() { Ad = "Güntaç AKŞEHİRLİ",  Tel = "533-447-8283" };
                var yh15 = new BDB.PP() { Ad = "Önder KURT",        Tel = "542-540-4325" };
                var yh16 = new BDB.PP() { Ad = "Mustafa İLKER",     Tel = "533-413-9741" };
                var yh17 = new BDB.PP() { Ad = "Suavi DEMİRCİOĞLU", Tel = "533-283-6822" };
                var yh18 = new BDB.PP() { Ad = "Necip KUTLUAY",     Tel = "532-217-3547" };
                var yh19 = new BDB.PP() { Ad = "Veysel TOSUN K2",   Tel = "532-426-0099" };
                var yh1A = new BDB.PP() { Ad = "Erhan AYAŞLI",      Tel = "532-373-5304" };
                var yh1B = new BDB.PP() { Ad = "Okyay ARI",         Tel = "532-281-8936" };
                var yh1C = new BDB.PP() { Ad = "Soner AYDOĞ",       Tel = "532-656-2644" };
                var yh1D = new BDB.PP() { Ad = "Gamze BAĞCI",       Tel = "507-353-1086" };
                var yh1E = new BDB.PP() { Ad = "Levent DİGİLİ K1",  Tel = "542-324-1698" };
                var yh1F = new BDB.PP() { Ad = "Vedat GÖKALP",      Tel = "542-674-2828" };
                var yh1G = new BDB.PP() { Ad = "Meral BAHÇETEPE",   Tel = "530-927-3627" };
                var yh1H = new BDB.PP() { Ad = "Ayhan ARAL",        Tel = "532-252-4266" };
                new BDB.CTP() { CC = cc1, CT = yh1, PP = yh11 };
                new BDB.CTP() { CC = cc1, CT = yh1, PP = yh12 };
                new BDB.CTP() { CC = cc1, CT = yh1, PP = yh13 };
                new BDB.CTP() { CC = cc1, CT = yh1, PP = yh14 };
                new BDB.CTP() { CC = cc1, CT = yh1, PP = yh15 };
                new BDB.CTP() { CC = cc1, CT = yh1, PP = yh16 };
                new BDB.CTP() { CC = cc1, CT = yh1, PP = yh17 };
                new BDB.CTP() { CC = cc1, CT = yh1, PP = yh18 };
                new BDB.CTP() { CC = cc1, CT = yh1, PP = yh19 };
                new BDB.CTP() { CC = cc1, CT = yh1, PP = yh1A };
                new BDB.CTP() { CC = cc1, CT = yh1, PP = yh1B };
                new BDB.CTP() { CC = cc1, CT = yh1, PP = yh1C };
                new BDB.CTP() { CC = cc1, CT = yh1, PP = yh1D };
                new BDB.CTP() { CC = cc1, CT = yh1, PP = yh1E };
                new BDB.CTP() { CC = cc1, CT = yh1, PP = yh1F };
                new BDB.CTP() { CC = cc1, CT = yh1, PP = yh1G };
                new BDB.CTP() { CC = cc1, CT = yh1, PP = yh1H };

                // Yalıkavak-1
                var yk11 = new BDB.PP() { Ad = "Tahir ÇELİKESEN" };
                var yk12 = new BDB.PP() { Ad = "Oğuz DEVELİ" };
                var yk13 = new BDB.PP() { Ad = "Şükran KILIÇASLAN" };
                new BDB.CTP() { CC = cc1, CT = yk1, PP = yk11 };
                new BDB.CTP() { CC = cc1, CT = yk1, PP = yk12 };
                new BDB.CTP() { CC = cc1, CT = yk1, PP = yk13 };
            });
        }

    }
}