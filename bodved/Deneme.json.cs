using Starcounter;

namespace bodved
{
    partial class Deneme : Json
    {
        protected override void OnData()
        {
            base.OnData();

            Deneme.SinglesElementJson s;

            s = Singles.Add();
            s.oNo = 11;
            s.Idx = 1;
            s.Ad = "Þener DEMÝRAL";
            s.AoY = "A";

            s = Singles.Add();
            s.oNo = 12;
            s.Idx = 2;
            s.Ad = "Ümit ÇETÝNALP";
            s.AoY = "A";

            s = Singles.Add();
            s.oNo = 13;
            s.Idx = 3;
            s.Ad = "Erhan DOÐRU";
            s.AoY = "A";

            s = Singles.Add();
            s.oNo = 14;
            s.Idx = 4;
            s.Ad = "Göksan AKAY";
            s.AoY = "A";

            s = Singles.Add();
            s.oNo = 15;
            s.Idx = 5;
            s.Ad = "Ahmet ACET";
            s.AoY = "A";

            s = Singles.Add();
            s.oNo = 16;
            s.Idx = 6;
            s.Ad = "Yenal EGE";
            s.AoY = "A";

            s = Singles.Add();
            s.oNo = 17;
            s.Idx = 7;
            s.Ad = "Emre ESMER";
            s.AoY = "A";

            s = Singles.Add();
            s.oNo = 18;
            s.Idx = 8;
            s.Ad = "Hakan UÐURLU";
            s.AoY = "A";

            s = Singles.Add();
            s.oNo = 19;
            s.Idx = 9;
            s.Ad = "Celalettin ABUZER";
            s.AoY = "Y";

            // Double da 3 mac yapilacak ise ilk 6 yi oku, ikili olarak yaz c1, c2
            // Sonrasini sadece c1'e yaz.
            Deneme.DoublesElementJson d;

            d = Doubles.Add();
            d.c1.oNo = 11;
            d.c1.Idx = 1;
            d.c1.Ad = "Þener DEMÝRAL";
            d.c2.oNo = 12;
            d.c2.Idx = 1;
            d.c2.Ad = "Ümit ÇETÝNALP";

            d = Doubles.Add();
            d.c1.oNo = 13;
            d.c1.Idx = 2;
            d.c1.Ad = "Erhan DOÐRU";
            d.c2.oNo = 14;
            d.c2.Idx = 2;
            d.c2.Ad = "Göksan AKAY";

            d = Doubles.Add();
            d.c1.oNo = 15;
            d.c1.Idx = 3;
            d.c1.Ad = "Ahmet ACET";
            d.c2.oNo = 16;
            d.c2.Idx = 3;
            d.c2.Ad = "Yenal EGE";

            d = Doubles.Add();
            d.c1.oNo = 17;
            d.c1.Idx = 4;
            d.c1.Ad = "Emre ESMER";
            d = Doubles.Add();
            d.c1.oNo = 18;
            d.c1.Idx = 4;
            d.c1.Ad = "Hakan UÐURLU";

            d = Doubles.Add();
            d.c1.oNo = 19;
            d.c1.Idx = 5;
            d.c1.Ad = "Celalettin ABUZER";
        }

        [Deneme_json.Singles]
        public partial class SinglesElementJson
        {
            void Handle(Input.Idx Action)
            {
                var oo = Action.OldValue;
                var nn = Action.Value;
                var aa = this.Ad;

            }
        }

        void Handle(Input.SaveTrigger Action)
        {
            var aa = this.Singles.Count;
            var bb = this.Doubles.Count;
            this.Singles[0].Ad = "DENEME";
        }

        [Deneme_json.Doubles]
        public partial class DoublesElementJson
        {

        }

        [Deneme_json.Doubles.c1]
        public partial class DoublesElementC1Json
        {
            void Handle(Input.Idx Action)
            {
                var oo = Action.OldValue;
                var nn = Action.Value;
                var aa = this.Ad;
            }
        }

        [Deneme_json.Doubles.c2]
        public partial class DoublesElementC2Json
        {
            void Handle(Input.Idx Action)
            {
                var oo = Action.OldValue;
                var nn = Action.Value;
                var aa = this.Ad;
            }
        }
    }
}
