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
            Handle.GET("/bodved/CETPpage/{?}", (string CEToNo, string CToNo) => WrapPage<CETPpage>($"/bodved/partial/CETPpage/{CEToNo}/{CToNo}"));



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

    }
}