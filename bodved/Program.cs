using System.Linq;
using Starcounter;
using System.Timers;
using System;

namespace bodved
{
    class Program
    {
        static Timer tmr;

        static void Main()
        {
            //tmr = new Timer(10000);
            //tmr.AutoReset = true;
            //tmr.Elapsed += Tmr_Elapsed;
            //tmr.Start();


            var HTML = @"<!DOCTYPE html>
				<html>
				<head>
					<meta charset=""utf-8"">
				    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0, minimum-scale=0.50, user-scalable=yes"">
				    <!--meta name=""viewport"" content=""width=device-width""-->
					<title>{0}</title>
					
                    <script src=""/sys/webcomponentsjs/webcomponents.min.js""></script>
					<!--script src=""/sys/document-register-element/build/document-register-element.js""></script-->

                    <script>
                        window.Polymer = {{
                            dom: ""shadow""
                        }};
                    </script>

					<link rel=""import"" href=""/sys/polymer/polymer.html"">
					<link rel=""import"" href=""/sys/starcounter.html"">
					<link rel=""import"" href=""/sys/starcounter-include/starcounter-include.html"">
					<!--link rel=""import"" href=""/sys/starcounter-debug-aid/src/starcounter-debug-aid.html""-->
					
                    <link rel=""stylesheet"" href=""https://fonts.googleapis.com/css?family=Source+Sans+Pro"">
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

            /*
                <body>
                    <template is=""dom-bind"" id=""puppet-root"">
                        <starcounter-include view-model=""{{{{model}}}}""></starcounter-include>
                        <div><template is=""imported-template"" model=""{{{{model}}}}"" href=""/bodved/masterPage.Html""></template></div>
                    </template>
                    <!--puppet-client ref=""puppet-root"" remote-url=""{1}""></puppet-client-->
                    <palindrom-client ref=""puppet-root"" remote-url=""{1}""></palindrom-client>
                    <starcounter-debug-aid></starcounter-debug-aid>
                </body>

            */
            Application.Current.Use(new HtmlFromJsonProvider());
            Application.Current.Use(new PartialToStandaloneHtmlProvider(HTML));

            BDB.H.Write2Log("Start");

            Handle.GET("/bodved/insOtoNotice", () =>
            {
                BDB.H.insOtoNotice();
                return $"OK: insOtoNotice";
            });



            //Handle.GET("/bodved", () => { return Self.GET("/bodved/MainPage"); });
            Handle.GET("/bodved", (Request request) => {
                return Self.GET("/bodved/NoticePage");
            });
            //Handle.GET("/bodved", () => {return new MainPage(); });


            Handle.GET("/bodved/partial/MainPage", () => new MainPage());
            Handle.GET("/bodved/MainPage", () => WrapPage<MainPage>("/bodved/partial/MainPage"));

            Handle.GET("/bodved/partial/NoticePage", () => new NoticePage());
            Handle.GET("/bodved/NoticePage", () => WrapPage<NoticePage>("/bodved/partial/NoticePage"));

            Handle.GET("/bodved/partial/AboutPage", () => new AboutPage());
            Handle.GET("/bodved/AboutPage", () => WrapPage<AboutPage>("/bodved/partial/AboutPage"));

            Handle.GET("/bodved/partial/LoginPage", () => new LoginPage());
            Handle.GET("/bodved/LoginPage", () => WrapPage<LoginPage>("/bodved/partial/LoginPage"));

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

            Handle.GET("/bodved/partial/CETpageCT/{?}", (string CToNo) => new CETpage() { CToNo = $"{CToNo}" });
            Handle.GET("/bodved/CETpageCT/{?}", (string CToNo) => WrapPage<CETpage>($"/bodved/partial/CETpageCT/{CToNo}"));

            Handle.GET("/bodved/partial/CETPpage/{?}/{?}", (string CEToNo, string CToNo) => new CETPpage() { CEToNo = $"{CEToNo}", CToNo = $"{CToNo}" });
            Handle.GET("/bodved/CETPpage/{?}/{?}", (string CEToNo, string CToNo) => WrapPage<CETPpage>($"/bodved/partial/CETPpage/{CEToNo}/{CToNo}"));

            Handle.GET("/bodved/partial/CETRviewPage/{?}", (string CEToNo) => new CETRviewPage() { CEToNo = $"{CEToNo}" });
            Handle.GET("/bodved/CETRviewPage/{?}", (string CEToNo) => WrapPage<CETRviewPage>($"/bodved/partial/CETRviewPage/{CEToNo}"));

            Handle.GET("/bodved/partial/CETRinpPage/{?}", (string CEToNo) => new CETRinpPage() { CEToNo = $"{CEToNo}" });
            Handle.GET("/bodved/CETRinpPage/{?}", (string CEToNo) => WrapPage<CETRinpPage>($"/bodved/partial/CETRinpPage/{CEToNo}"));

            Handle.GET("/bodved/partial/ppMacPage/{?}", (string PPoNo) => new ppMacPage() { PPoNo = $"{PPoNo}" });
            Handle.GET("/bodved/ppMacPage/{?}", (string PPoNo) => WrapPage<ppMacPage>($"/bodved/partial/ppMacPage/{PPoNo}"));


            Handle.GET("/bodved/partial/Deneme/{?}", (string CCoNo) => new Deneme() { CCoNo = $"{CCoNo}" });
            Handle.GET("/bodved/Deneme/{?}", (string CCoNo) => WrapPage<Deneme>($"/bodved/partial/Deneme/{CCoNo}"));

            Handle.GET("/bodved/Stat", () => {
                return $"Toplam Turnuva sayisi: 5\ndfgsdfgsdgf";
            });

            Hook<BDB.CET>.CommitUpdate += (s, obj) =>
            {
                /*
                //var old = Db.FromId<BDB.CET>(obj.GetObjectNo());
                //if (obj.Trh != old.Trh)
                {
                    foreach(var r in Db.SQL<BDB.CETR>("select c from CETR c where c.CET = ?", obj))
                    {
                        if (r.PRH != null)
                        {
                            r.PRH.Trh = obj.Trh;
                        }
                    }
                }
                */
            };
            /*
            Hook<BDB.CT>.CommitInsert += (p, obj) =>
            {
                Session.RunTaskForAll((s, id) => {
                    
                    s.CalculatePatchAndPushOnWebSocket(); });
            };*/
        }

        private static void Tmr_Elapsed(object sender, ElapsedEventArgs e)
        {
            /*
            Session.RunTaskForAll((s, id) => {
                var bb = s.ActiveWebSocket.IsDead();
                var aa = "";

                if (s.ActiveWebSocket.IsDead())
                {
                    aa = "sener";
                }
            });
            */
        }

        public static MasterPage GetMasterPageFromSession()
        {
            var master = Session.Ensure().Store["bodved"] as MasterPage;

            if (master == null)
            {
                master = new MasterPage();
                //Session.Current.Data = master;
                Session.Current.Store["bodved"] = master;

                BDB.H.Write2Log($"Enter: {Session.Current.SessionId}");
            }

            return master;
        }

        private static Json WrapPage<T>(string partialPath) where T : Json
        {
            var master = GetMasterPageFromSession();

            
            if (master.CurrentPage != null && master.CurrentPage.GetType().Equals(typeof(T)))
            {
                return master;
            }
            
            master.CurrentPage = Self.GET(partialPath);

            if (master.CurrentPage.Data == null)
            {
                master.CurrentPage.Data = null; //trick to invoke OnData in partial
            }

            return master;
        }

    }
}