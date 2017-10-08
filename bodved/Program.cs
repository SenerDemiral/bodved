﻿using System;
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

            Handle.GET("/bodved/partial/TurnuvalarPage", () => new TurnuvalarPage());
            Handle.GET("/bodved/TurnuvalarPage", () => WrapPage<TurnuvalarPage>("/bodved/partial/TurnuvalarPage"));

            Handle.GET("/bodved/partial/OyuncularPage", () => new OyuncularPage());
            Handle.GET("/bodved/OyuncularPage", () => WrapPage<OyuncularPage>("/bodved/partial/OyuncularPage"));

            Handle.GET("/bodved/partial/Deneme", () => new Deneme());
            Handle.GET("/bodved/Deneme", () => WrapPage<Deneme>("/bodved/partial/Deneme"));
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