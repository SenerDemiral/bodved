using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Starcounter;
using System.Diagnostics;

namespace BDB
{
    public static class Tanimlar
    {
        public static string deneme1()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            Db.Transact(() => {
                for (int i = 0; i < 1000000; i++)
                {
                    new BDB.PP
                    {
                        Ad = i.ToString(),
                        DgmYil = i
                    };
                }
            });

            watch.Stop();
            //Console.WriteLine($"{watch.ElapsedMilliseconds} msec  {watch.ElapsedTicks} ticks");
            return $"{watch.ElapsedMilliseconds} msec  {watch.ElapsedTicks} ticks";
        }

        public static string deneme3()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

                for (int i = 0; i < 1000000; i++)
                {
                    var rec = Db.SQL<BDB.PP>("select p from PP p where p.ObjectNo = ?", i).FirstOrDefault();
                }

            watch.Stop();
            //Console.WriteLine($"{watch.ElapsedMilliseconds} msec  {watch.ElapsedTicks} ticks");
            return $"{watch.ElapsedMilliseconds} msec  {watch.ElapsedTicks} ticks";
        }

        public static string deneme4()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            var rec = Db.SQL<BDB.PP>("select p from PP p");
            int i = 0;
            int a = 0;
            string s = "";
            foreach (var r in rec)
            {
                s = r.Ad;
                a = r.DgmYil;
                i++;
            }

            watch.Stop();
            //Console.WriteLine($"{watch.ElapsedMilliseconds} msec  {watch.ElapsedTicks} ticks");
            return $"{watch.ElapsedMilliseconds} msec  {watch.ElapsedTicks} ticks {i}";
        }

        public static string deneme2()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            Db.Transact(() =>
            {
                Db.SQL<BDB.PP>("delete from PP");
            });

            watch.Stop();
            //Console.WriteLine($"{watch.ElapsedMilliseconds} msec  {watch.ElapsedTicks} ticks");
            return $"{watch.ElapsedMilliseconds} msec  {watch.ElapsedTicks} ticks";
        }

        public static void popTable()
        {
            Db.Transact(() =>
            {
                // Turnuvalar
                var cc1  = new BDB.CC() { Ad = "1.Lig (2017-18)",  Idx = "17-5", Skl = "T", Grp = "17T" };
                var cc2A = new BDB.CC() { Ad = "2A.Lig (2017-18)", Idx = "17-4", Skl = "T", Grp = "17T" };
                var cc2B = new BDB.CC() { Ad = "2B.Lig (2017-18)", Idx = "17-3", Skl = "T", Grp = "17T" };
                var cc3A = new BDB.CC() { Ad = "3A.Lig (2017-18)", Idx = "17-2", Skl = "T", Grp = "17T" };
                var cc3B = new BDB.CC() { Ad = "3B.Lig (2017-18)", Idx = "17-1", Skl = "T", Grp = "17T" };

                // Turnuva Takimlari
                // 1.Lig
                var df1 = new BDB.CT() { CC = cc1, Ad = "Delfi•1",      Pw = "df1", Adres = "Delfi Arena Bodrum" };
                var dr1 = new BDB.CT() { CC = cc1, Ad = "Dragons•1",    Pw = "dr1", Adres = "Funtown Yahşi Orakent" };
                var em1 = new BDB.CT() { CC = cc1, Ad = "ErkanMotel•1", Pw = "em1", Adres = "Erkan Motel Yahşi" };
                var gm1 = new BDB.CT() { CC = cc1, Ad = "Gümüşlük•1",   Pw = "gm1", Adres = "Avta Tesisleri Turgutreis" };
                var kp1 = new BDB.CT() { CC = cc1, Ad = "KingPong•1",   Pw = "kk1", Adres = "Yahşi Tenis Kulübü" };
                var kr1 = new BDB.CT() { CC = cc1, Ad = "KutayReno•1",  Pw = "kr1", Adres = "Avta Tesisleri Turgutreis" };
                var mk1 = new BDB.CT() { CC = cc1, Ad = "MilasKarya•1", Pw = "mk1", Adres = "Delfi Arena Bodrum" };
                var mt1 = new BDB.CT() { CC = cc1, Ad = "Mitos•1",      Pw = "mt1", Adres = "Avta Turgutreis" };
                var pn1 = new BDB.CT() { CC = cc1, Ad = "Ponpin•1",     Pw = "pn1", Adres = "Delfi Arena Bodrum" };
                var pr1 = new BDB.CT() { CC = cc1, Ad = "Promil•1",     Pw = "pr1", Adres = "Değirmen Cafe OASİS Bodrum" };
                var yk1 = new BDB.CT() { CC = cc1, Ad = "Yalıkavak•1",  Pw = "yk1", Adres = "Çaça Restaurant Yalıkavak" };


                // 1.Lig Fikstur
                new BDB.CET { CC = cc1, hCT = df1, gCT = dr1, Trh = DateTime.Parse("30.10.2017 19:00") };
                new BDB.CET { CC = cc1, hCT = mt1, gCT = kr1, Trh = DateTime.Parse("01.11.2017 19:00") };
                new BDB.CET { CC = cc1, hCT = pn1, gCT = kp1, Trh = DateTime.Parse("01.11.2017 19:00") };
                new BDB.CET { CC = cc1, hCT = pr1, gCT = gm1, Trh = DateTime.Parse("01.11.2017 19:00") };
                new BDB.CET { CC = cc1, hCT = yk1, gCT = em1, Trh = DateTime.Parse("03.11.2017 19:00") };

                new BDB.CET { CC = cc1, hCT = em1, gCT = df1, Trh = DateTime.Parse("06.11.2017 19:00") };
                new BDB.CET { CC = cc1, hCT = gm1, gCT = yk1, Trh = DateTime.Parse("07.11.2017 19:00") };
                new BDB.CET { CC = cc1, hCT = kp1, gCT = pr1, Trh = DateTime.Parse("07.11.2017 19:00") };
                new BDB.CET { CC = cc1, hCT = kr1, gCT = pn1, Trh = DateTime.Parse("08.11.2017 19:00") };
                new BDB.CET { CC = cc1, hCT = mk1, gCT = mt1, Trh = DateTime.Parse("10.11.2017 19:00") };

                new BDB.CET { CC = cc1, hCT = df1, gCT = gm1, Trh = DateTime.Parse("13.11.2017 19:00") };
                new BDB.CET { CC = cc1, hCT = pn1, gCT = mk1, Trh = DateTime.Parse("15.11.2017 19:00") };
                new BDB.CET { CC = cc1, hCT = pr1, gCT = kr1, Trh = DateTime.Parse("15.11.2017 19:00") };
                new BDB.CET { CC = cc1, hCT = dr1, gCT = em1, Trh = DateTime.Parse("16.11.2017 19:00") };
                new BDB.CET { CC = cc1, hCT = yk1, gCT = kp1, Trh = DateTime.Parse("17.11.2017 19:00") };

                new BDB.CET { CC = cc1, hCT = gm1, gCT = dr1, Trh = DateTime.Parse("21.11.2017 19:00") };
                new BDB.CET { CC = cc1, hCT = kp1, gCT = df1, Trh = DateTime.Parse("21.11.2017 19:00") };
                new BDB.CET { CC = cc1, hCT = kr1, gCT = yk1, Trh = DateTime.Parse("22.11.2017 19:00") };
                new BDB.CET { CC = cc1, hCT = mt1, gCT = pn1, Trh = DateTime.Parse("24.11.2017 19:00") };
                new BDB.CET { CC = cc1, hCT = mk1, gCT = pr1, Trh = DateTime.Parse("24.11.2017 19:00") };

                new BDB.CET { CC = cc1, hCT = df1, gCT = kr1, Trh = DateTime.Parse("27.11.2017 19:00") };
                new BDB.CET { CC = cc1, hCT = em1, gCT = gm1, Trh = DateTime.Parse("27.11.2017 19:00") };
                new BDB.CET { CC = cc1, hCT = pr1, gCT = mt1, Trh = DateTime.Parse("29.11.2017 19:00") };
                new BDB.CET { CC = cc1, hCT = dr1, gCT = kp1, Trh = DateTime.Parse("30.11.2017 19:00") };
                new BDB.CET { CC = cc1, hCT = yk1, gCT = mk1, Trh = DateTime.Parse("01.12.2017 19:00") };

                new BDB.CET { CC = cc1, hCT = mt1, gCT = yk1, Trh = DateTime.Parse("04.12.2017 19:00") };
                new BDB.CET { CC = cc1, hCT = kp1, gCT = em1, Trh = DateTime.Parse("05.12.2017 19:00") };
                new BDB.CET { CC = cc1, hCT = kr1, gCT = dr1, Trh = DateTime.Parse("06.12.2017 19:00") };
                new BDB.CET { CC = cc1, hCT = pn1, gCT = pr1, Trh = DateTime.Parse("06.12.2017 19:00") };
                new BDB.CET { CC = cc1, hCT = mk1, gCT = df1, Trh = DateTime.Parse("08.12.2017 19:00") };

                new BDB.CET { CC = cc1, hCT = df1, gCT = mt1, Trh = DateTime.Parse("11.12.2017 19:00") };
                new BDB.CET { CC = cc1, hCT = em1, gCT = kr1, Trh = DateTime.Parse("11.12.2017 19:00") };
                new BDB.CET { CC = cc1, hCT = gm1, gCT = kp1, Trh = DateTime.Parse("12.12.2017 19:00") };
                new BDB.CET { CC = cc1, hCT = dr1, gCT = mk1, Trh = DateTime.Parse("14.12.2017 19:00") };
                new BDB.CET { CC = cc1, hCT = yk1, gCT = pn1, Trh = DateTime.Parse("15.12.2017 19:00") };

                new BDB.CET { CC = cc1, hCT = mt1, gCT = dr1, Trh = DateTime.Parse("18.12.2017 19:00") };
                new BDB.CET { CC = cc1, hCT = kr1, gCT = gm1, Trh = DateTime.Parse("20.12.2017 19:00") };
                new BDB.CET { CC = cc1, hCT = pn1, gCT = df1, Trh = DateTime.Parse("20.12.2017 19:00") };
                new BDB.CET { CC = cc1, hCT = pr1, gCT = yk1, Trh = DateTime.Parse("20.12.2017 19:00") };
                new BDB.CET { CC = cc1, hCT = mk1, gCT = em1, Trh = DateTime.Parse("22.12.2017 19:00") };

                new BDB.CET { CC = cc1, hCT = gm1, gCT = mk1, Trh = DateTime.Parse("02.01.2018 19:00") };
                new BDB.CET { CC = cc1, hCT = kp1, gCT = kr1, Trh = DateTime.Parse("02.01.2018 19:00") };
                new BDB.CET { CC = cc1, hCT = em1, gCT = mt1, Trh = DateTime.Parse("03.01.2018 19:00") };
                new BDB.CET { CC = cc1, hCT = dr1, gCT = pn1, Trh = DateTime.Parse("04.01.2018 19:00") };
                new BDB.CET { CC = cc1, hCT = df1, gCT = pr1, Trh = DateTime.Parse("05.01.2018 19:00") };

                new BDB.CET { CC = cc1, hCT = mt1, gCT = gm1, Trh = DateTime.Parse("08.01.2018 19:00") };
                new BDB.CET { CC = cc1, hCT = pn1, gCT = em1, Trh = DateTime.Parse("10.01.2018 19:00") };
                new BDB.CET { CC = cc1, hCT = pr1, gCT = dr1, Trh = DateTime.Parse("10.01.2018 19:00") };
                new BDB.CET { CC = cc1, hCT = mk1, gCT = kp1, Trh = DateTime.Parse("12.01.2018 19:00") };
                new BDB.CET { CC = cc1, hCT = yk1, gCT = df1, Trh = DateTime.Parse("12.01.2018 19:00") };

                new BDB.CET { CC = cc1, hCT = em1, gCT = pr1, Trh = DateTime.Parse("15.01.2018 19:00") };
                new BDB.CET { CC = cc1, hCT = gm1, gCT = pn1, Trh = DateTime.Parse("16.01.2018 19:00") };
                new BDB.CET { CC = cc1, hCT = kp1, gCT = mt1, Trh = DateTime.Parse("16.01.2018 19:00") };
                new BDB.CET { CC = cc1, hCT = kr1, gCT = mk1, Trh = DateTime.Parse("17.01.2018 19:00") };
                new BDB.CET { CC = cc1, hCT = dr1, gCT = yk1, Trh = DateTime.Parse("18.01.2018 19:00") };

                // 2A.Lig 
                new BDB.CT() { CC = cc2A, Ad = "Bitez•2A" };
                new BDB.CT() { CC = cc2A, Ad = "Bosku•2A" };
                new BDB.CT() { CC = cc2A, Ad = "Delfi•2A" };
                new BDB.CT() { CC = cc2A, Ad = "Dragons•2A" };
                new BDB.CT() { CC = cc2A, Ad = "Gümüşlük•2A" };
                var kk2A = new BDB.CT() { CC = cc2A, Ad = "Kekik•2A" };
                var kr2A = new BDB.CT() { CC = cc2A, Ad = "KutayReno•2A" };
                new BDB.CT() { CC = cc2A, Ad = "MitNarKop•2A" };
                new BDB.CT() { CC = cc2A, Ad = "Nane•2A" };
                new BDB.CT() { CC = cc2A, Ad = "Newpon•2A" };
                new BDB.CT() { CC = cc2A, Ad = "Nomads•2A" };
                new BDB.CT() { CC = cc2A, Ad = "Onbar•2A" };
                new BDB.CT() { CC = cc2A, Ad = "Sağlık•2A" };
                new BDB.CT() { CC = cc2A, Ad = "Yahşi•2A" };
                new BDB.CT() { CC = cc2A, Ad = "Yalıkavak•2A", Pw = "yk2A" };
                // 2B/Lig
                new BDB.CT() { CC = cc2B, Ad = "Delfi•2B" };
                var np2B = new BDB.CT() { CC = cc2B, Ad = "Newpon•2B" };

                // 3A.Lig
                new BDB.CT() { CC = cc3A, Ad = "Bitez•3A" };
                new BDB.CT() { CC = cc3A, Ad = "Bosku•3A" };
                new BDB.CT() { CC = cc3A, Ad = "Delfi•3A" };
                var gm3A = new BDB.CT() { CC = cc3A, Ad = "Gümüşlük•3A" };
                new BDB.CT() { CC = cc3A, Ad = "KingPong•3A" };
                var kr3A = new BDB.CT() { CC = cc3A, Ad = "KutayReno•3A" };
                new BDB.CT() { CC = cc3A, Ad = "MiaMare•3A" };
                new BDB.CT() { CC = cc3A, Ad = "MitNarKop•3A" };
                new BDB.CT() { CC = cc3A, Ad = "Nane•3A" };
                new BDB.CT() { CC = cc3A, Ad = "Onbar•3A" };
                new BDB.CT() { CC = cc3A, Ad = "Waffelhane•3A" };
                new BDB.CT() { CC = cc3A, Ad = "Yahşi•3A" };
                new BDB.CT() { CC = cc3A, Ad = "Yalıkavak•3A" };

                // 3b.Lig
                new BDB.CT() { CC = cc3A, Ad = "Delfi•3B" };

                #region 1.Lig

                #region Delfi-1
                var df11 = new BDB.PP() { Ad = "Ahmet KURŞUNLU" };
                var df12 = new BDB.PP() { Ad = "Erkan ARI" };
                var df13 = new BDB.PP() { Ad = "Mehmet BARUTÇU" };
                var df14 = new BDB.PP() { Ad = "Ayhan BOSTAN" };
                var df15 = new BDB.PP() { Ad = "Ünal PEKTAŞ" };
                var df16 = new BDB.PP() { Ad = "Yücel NİEGO" };
                var df17 = new BDB.PP() { Ad = "Atilla OĞUZ" };
                var df18 = new BDB.PP() { Ad = "Levent BULUT" };
                var df19 = new BDB.PP() { Ad = "Göksal ORAL" };
                var df1A = new BDB.PP() { Ad = "Çetin ÖNCÜ" };
                var df1B = new BDB.PP() { Ad = "Rıza KARAKAYA" };
                var df1C = new BDB.PP() { Ad = "Aytuğ DOĞER" };
                var df1D = new BDB.PP() { Ad = "Temel ÖZENMİŞ" };
                new BDB.CTP() { CC = cc1, CT = df1, PP = df11, Idx = 1 };
                new BDB.CTP() { CC = cc1, CT = df1, PP = df12, Idx = 2 };
                new BDB.CTP() { CC = cc1, CT = df1, PP = df13, Idx = 3 };
                new BDB.CTP() { CC = cc1, CT = df1, PP = df14, Idx = 4 };
                new BDB.CTP() { CC = cc1, CT = df1, PP = df15, Idx = 5 };
                new BDB.CTP() { CC = cc1, CT = df1, PP = df16, Idx = 6 };
                new BDB.CTP() { CC = cc1, CT = df1, PP = df17, Idx = 7 };
                new BDB.CTP() { CC = cc1, CT = df1, PP = df18, Idx = 8 };
                new BDB.CTP() { CC = cc1, CT = df1, PP = df19, Idx = 9 };
                new BDB.CTP() { CC = cc1, CT = df1, PP = df1A, Idx = 10 };
                new BDB.CTP() { CC = cc1, CT = df1, PP = df1B, Idx = 11 };
                new BDB.CTP() { CC = cc1, CT = df1, PP = df1C, Idx = 12 };
                new BDB.CTP() { CC = cc1, CT = df1, PP = df1D, Idx = 13 };
                #endregion Delfi-1

                #region Dragons-1
                var dr11 = new BDB.PP() { Ad = "Nihat ÜMMETOĞLU" };
                var dr12 = new BDB.PP() { Ad = "Tunç HIZAL" };
                var dr13 = new BDB.PP() { Ad = "Vahi GÜNER" };
                var dr14 = new BDB.PP() { Ad = "Çağatay ŞAŞMAZ" };
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
                var dr1L = new BDB.PP() { Ad = "Yaşar KARATAŞ" };
                new BDB.CTP() { CC = cc1, CT = dr1, PP = dr11, Idx = 1 };
                new BDB.CTP() { CC = cc1, CT = dr1, PP = dr12, Idx = 2 };
                new BDB.CTP() { CC = cc1, CT = dr1, PP = dr13, Idx = 3 };
                new BDB.CTP() { CC = cc1, CT = dr1, PP = dr14, Idx = 4 };
                new BDB.CTP() { CC = cc1, CT = dr1, PP = dr15, Idx = 5 };
                new BDB.CTP() { CC = cc1, CT = dr1, PP = dr16, Idx = 6 };
                new BDB.CTP() { CC = cc1, CT = dr1, PP = dr17, Idx = 7 };
                new BDB.CTP() { CC = cc1, CT = dr1, PP = dr18, Idx = 8 };
                new BDB.CTP() { CC = cc1, CT = dr1, PP = dr19, Idx = 9 };
                new BDB.CTP() { CC = cc1, CT = dr1, PP = dr1A, Idx = 10 };
                new BDB.CTP() { CC = cc1, CT = dr1, PP = dr1B, Idx = 11 };
                new BDB.CTP() { CC = cc1, CT = dr1, PP = dr1C, Idx = 12 };
                new BDB.CTP() { CC = cc1, CT = dr1, PP = dr1D, Idx = 13 };
                new BDB.CTP() { CC = cc1, CT = dr1, PP = dr1E, Idx = 14 };
                new BDB.CTP() { CC = cc1, CT = dr1, PP = dr1F, Idx = 15 };
                new BDB.CTP() { CC = cc1, CT = dr1, PP = dr1G, Idx = 16 };
                new BDB.CTP() { CC = cc1, CT = dr1, PP = dr1H, Idx = 17 };
                new BDB.CTP() { CC = cc1, CT = dr1, PP = dr1I, Idx = 18 };
                new BDB.CTP() { CC = cc1, CT = dr1, PP = dr1J, Idx = 19 };
                new BDB.CTP() { CC = cc1, CT = dr1, PP = dr1K, Idx = 20 };
                new BDB.CTP() { CC = cc1, CT = dr1, PP = dr1L, Idx = 20 };
                #endregion Dragons-1

                #region ErkanMotel-1
                var em11 = new BDB.PP() { Ad = "Burak KANAT", Tel = "530-119-0155" };
                var em12 = new BDB.PP() { Ad = "Faruk ULUSOY", Tel = "532-565-7188" };
                var em13 = new BDB.PP() { Ad = "Tarık TURFAN", Tel = "546-274-5584" };
                var em14 = new BDB.PP() { Ad = "Güntaç AKŞEHİRLİ", Tel = "533-447-8283" };
                var em15 = new BDB.PP() { Ad = "Önder KURT", Tel = "542-540-4325" };
                var em16 = new BDB.PP() { Ad = "Mustafa İLKER", Tel = "533-413-9741" };
                var em17 = new BDB.PP() { Ad = "Suavi DEMİRCİOĞLU", Tel = "533-283-6822" };
                var em18 = new BDB.PP() { Ad = "Necip KUTLUAY", Tel = "532-217-3547" };
                var em19 = new BDB.PP() { Ad = "Veysel TOSUN", Tel = "532-426-0099" };
                var em1A = new BDB.PP() { Ad = "Erhan AYAŞLI", Tel = "532-373-5304" };
                var em1B = new BDB.PP() { Ad = "Okyay ARI", Tel = "532-281-8936" };
                var em1C = new BDB.PP() { Ad = "Soner AYDOĞ", Tel = "532-656-2644" };
                var em1D = new BDB.PP() { Ad = "Gamze BAĞCI", Tel = "507-353-1086" };
                var em1E = new BDB.PP() { Ad = "Levent DİGİLİ", Tel = "542-324-1698" };
                var em1F = new BDB.PP() { Ad = "Vedat GÖKALP", Tel = "542-674-2828" };
                var em1G = new BDB.PP() { Ad = "Meral BAHÇETEPE", Tel = "530-927-3627" };
                var em1H = new BDB.PP() { Ad = "Ayhan ARAL", Tel = "532-252-4266" };
                var em1I = new BDB.PP() { Ad = "Hasan DOĞAN" };
                new BDB.CTP() { CC = cc1, CT = em1, PP = em11, Idx = 1 };
                new BDB.CTP() { CC = cc1, CT = em1, PP = em12, Idx = 2 };
                new BDB.CTP() { CC = cc1, CT = em1, PP = em13, Idx = 3 };
                new BDB.CTP() { CC = cc1, CT = em1, PP = em14, Idx = 4 };
                new BDB.CTP() { CC = cc1, CT = em1, PP = em15, Idx = 5 };
                new BDB.CTP() { CC = cc1, CT = em1, PP = em16, Idx = 6 };
                new BDB.CTP() { CC = cc1, CT = em1, PP = em17, Idx = 7 };
                new BDB.CTP() { CC = cc1, CT = em1, PP = em18, Idx = 8 };
                new BDB.CTP() { CC = cc1, CT = em1, PP = em19, Idx = 9 };
                new BDB.CTP() { CC = cc1, CT = em1, PP = em1A, Idx = 10 };
                new BDB.CTP() { CC = cc1, CT = em1, PP = em1B, Idx = 11 };
                new BDB.CTP() { CC = cc1, CT = em1, PP = em1C, Idx = 12 };
                new BDB.CTP() { CC = cc1, CT = em1, PP = em1D, Idx = 13 };
                new BDB.CTP() { CC = cc1, CT = em1, PP = em1E, Idx = 14 };
                new BDB.CTP() { CC = cc1, CT = em1, PP = em1F, Idx = 15 };
                new BDB.CTP() { CC = cc1, CT = em1, PP = em1G, Idx = 16 };
                new BDB.CTP() { CC = cc1, CT = em1, PP = em1H, Idx = 17 };
                new BDB.CTP() { CC = cc1, CT = em1, PP = em1I, Idx = 17 };
                #endregion ErkanMotel-1

                #region Gümüşlük-1
                var gm11 = new BDB.PP() { Ad = "Serdar KAYA" };
                var gm12 = new BDB.PP() { Ad = "Ümit YEKREK" };
                var gm13 = new BDB.PP() { Ad = "İsmail DÜZTAŞ" };
                var gm14 = new BDB.PP() { Ad = "Mustafa ÖZASLAN" };
                var gm15 = new BDB.PP() { Ad = "Levent ALTAN" };
                var gm16 = new BDB.PP() { Ad = "Hakkı ZIRH" };
                var gm17 = new BDB.PP() { Ad = "Behçet AK" };
                var gm18 = new BDB.PP() { Ad = "Hamit MENGÜÇ" };
                var gm19 = new BDB.PP() { Ad = "Serkan İLHAN" };
                var gm1A = new BDB.PP() { Ad = "Ali TÜRKELİ" };
                var gm1B = new BDB.PP() { Ad = "Metin SOYSAL" };
                var gm1C = new BDB.PP() { Ad = "Nicholas KARYDAKİS" };
                var gm1D = new BDB.PP() { Ad = "Fikret YÜCEL" };
                var gm1E = new BDB.PP() { Ad = "Fırat POLAT" };
                var gm1F = new BDB.PP() { Ad = "Erol DANACI" };
                var gm1G = new BDB.PP() { Ad = "Süleyman KARTAL" };
                var gm1H = new BDB.PP() { Ad = "Belen ÜNAL" };
                var gm1I = new BDB.PP() { Ad = "Turgay ÇABALAR" };
                new BDB.CTP() { CC = cc1, CT = gm1, PP = gm11, Idx = 1 };
                new BDB.CTP() { CC = cc1, CT = gm1, PP = gm12, Idx = 2 };
                new BDB.CTP() { CC = cc1, CT = gm1, PP = gm13, Idx = 3 };
                new BDB.CTP() { CC = cc1, CT = gm1, PP = gm14, Idx = 4 };
                new BDB.CTP() { CC = cc1, CT = gm1, PP = gm15, Idx = 5 };
                new BDB.CTP() { CC = cc1, CT = gm1, PP = gm16, Idx = 6 };
                new BDB.CTP() { CC = cc1, CT = gm1, PP = gm17, Idx = 7 };
                new BDB.CTP() { CC = cc1, CT = gm1, PP = gm18, Idx = 8 };
                new BDB.CTP() { CC = cc1, CT = gm1, PP = gm19, Idx = 9 };
                new BDB.CTP() { CC = cc1, CT = gm1, PP = gm1A, Idx = 10 };
                new BDB.CTP() { CC = cc1, CT = gm1, PP = gm1B, Idx = 11 };
                new BDB.CTP() { CC = cc1, CT = gm1, PP = gm1C, Idx = 12 };
                new BDB.CTP() { CC = cc1, CT = gm1, PP = gm1D, Idx = 13 };
                new BDB.CTP() { CC = cc1, CT = gm1, PP = gm1E, Idx = 14 };
                new BDB.CTP() { CC = cc1, CT = gm1, PP = gm1F, Idx = 15 };
                new BDB.CTP() { CC = cc1, CT = gm1, PP = gm1G, Idx = 16 };
                new BDB.CTP() { CC = cc1, CT = gm1, PP = gm1H, Idx = 17 };
                new BDB.CTP() { CC = cc1, CT = gm1, PP = gm1I, Idx = 18 };
                #endregion Gümüşlük-1

                #region KingPong-1
                var kk11 = new BDB.PP() { Ad = "Mahir TUFAN" };
                var kk12 = new BDB.PP() { Ad = "Hünkar YÜCEL" };
                var kk13 = new BDB.PP() { Ad = "Can Deniz" };
                var kk14 = new BDB.PP() { Ad = "Bahadır TULUMCUOĞLU" };
                var kk15 = new BDB.PP() { Ad = "Hakan DEBBAĞ" };
                var kk16 = new BDB.PP() { Ad = "Hasan ACIOLUK" };
                var kk17 = new BDB.PP() { Ad = "Göksun ALAY" };
                var kk18 = new BDB.PP() { Ad = "Murat ÇAKIR" };
                var kk19 = new BDB.PP() { Ad = "Halil ÇALIŞKAN" };
                var kk1A = new BDB.PP() { Ad = "Yaşar BULMUŞ" };
                var kk1B = new BDB.PP() { Ad = "Gürdal DALKILIÇ" };
                var kk1C = new BDB.PP() { Ad = "Ahmet ERDEM" };
                var kk1D = new BDB.PP() { Ad = "Ramazan İZMİRLİ" };
                var kk1E = new BDB.PP() { Ad = "Ahmet ŞAHAN İZMİRLİ" };
                new BDB.CTP() { CC = cc1, CT = kp1, PP = kk11, Idx = 1 };
                new BDB.CTP() { CC = cc1, CT = kp1, PP = kk12, Idx = 2 };
                new BDB.CTP() { CC = cc1, CT = kp1, PP = kk13, Idx = 3 };
                new BDB.CTP() { CC = cc1, CT = kp1, PP = kk14, Idx = 4 };
                new BDB.CTP() { CC = cc1, CT = kp1, PP = kk15, Idx = 5 };
                new BDB.CTP() { CC = cc1, CT = kp1, PP = kk16, Idx = 6 };
                new BDB.CTP() { CC = cc1, CT = kp1, PP = kk17, Idx = 7 };
                new BDB.CTP() { CC = cc1, CT = kp1, PP = kk18, Idx = 8 };
                new BDB.CTP() { CC = cc1, CT = kp1, PP = kk19, Idx = 9 };
                new BDB.CTP() { CC = cc1, CT = kp1, PP = kk1A, Idx = 10 };
                new BDB.CTP() { CC = cc1, CT = kp1, PP = kk1B, Idx = 11 };
                new BDB.CTP() { CC = cc1, CT = kp1, PP = kk1C, Idx = 12 };
                new BDB.CTP() { CC = cc1, CT = kp1, PP = kk1D, Idx = 13 };
                new BDB.CTP() { CC = cc1, CT = kp1, PP = kk1E, Idx = 13 };
                #endregion KingPong-1

                #region KutayReno-1
                var kr11 = new BDB.PP() { Ad = "Sefer NAMRUK" };
                var kr12 = new BDB.PP() { Ad = "Metin ÖZDEMİR" };
                var kr13 = new BDB.PP() { Ad = "Yaşar ASLAN" };
                var kr14 = new BDB.PP() { Ad = "Atalay SÜTCÜ" };
                var kr15 = new BDB.PP() { Ad = "Barış DALDAL" };
                var kr16 = new BDB.PP() { Ad = "Ümit ÜNSAL" };
                var kr17 = new BDB.PP() { Ad = "Doğan İPEK" };
                var kr18 = new BDB.PP() { Ad = "Davut DİRİL" };
                var kr19 = new BDB.PP() { Ad = "Oya KİZİR" };
                var kr1A = new BDB.PP() { Ad = "Koray BAYSAL" };
                var kr1B = new BDB.PP() { Ad = "Hamdi AKDOĞAN" };
                var kr1C = new BDB.PP() { Ad = "Kaya ERGİN" };
                var kr1D = new BDB.PP() { Ad = "Nijat SİREL" };
                var kr1E = new BDB.PP() { Ad = "Kaya BOZDAĞ" };
                new BDB.CTP() { CC = cc1, CT = kr1, PP = kr11, Idx = 1 };
                new BDB.CTP() { CC = cc1, CT = kr1, PP = kr12, Idx = 2 };
                new BDB.CTP() { CC = cc1, CT = kr1, PP = kr13, Idx = 3 };
                new BDB.CTP() { CC = cc1, CT = kr1, PP = kr14, Idx = 4 };
                new BDB.CTP() { CC = cc1, CT = kr1, PP = kr15, Idx = 5 };
                new BDB.CTP() { CC = cc1, CT = kr1, PP = kr16, Idx = 6 };
                new BDB.CTP() { CC = cc1, CT = kr1, PP = kr17, Idx = 7 };
                new BDB.CTP() { CC = cc1, CT = kr1, PP = kr18, Idx = 8 };
                new BDB.CTP() { CC = cc1, CT = kr1, PP = kr19, Idx = 9 };
                new BDB.CTP() { CC = cc1, CT = kr1, PP = kr1A, Idx = 10 };
                new BDB.CTP() { CC = cc1, CT = kr1, PP = kr1B, Idx = 11 };
                new BDB.CTP() { CC = cc1, CT = kr1, PP = kr1C, Idx = 12 };
                new BDB.CTP() { CC = cc1, CT = kr1, PP = kr1D, Idx = 13 };
                new BDB.CTP() { CC = cc1, CT = kr1, PP = kr1E, Idx = 14 };
                #endregion KutayReno-1

                #region MilasKarya-1
                var mk11 = new BDB.PP() { Ad = "İbrahim TOP" };
                var mk12 = new BDB.PP() { Ad = "Çağdaş SAVAŞ" };
                var mk13 = new BDB.PP() { Ad = "Enver VARAN" };
                var mk14 = new BDB.PP() { Ad = "İsmail BOZKURT" };
                var mk15 = new BDB.PP() { Ad = "Alaattin SAGUŞ" };
                var mk16 = new BDB.PP() { Ad = "Fatih BULDAN" };
                var mk17 = new BDB.PP() { Ad = "Mesut ÖZER" };
                var mk18 = new BDB.PP() { Ad = "Erdem ÖZKAN" };
                var mk19 = new BDB.PP() { Ad = "Anıl AYDEMİR" };
                var mk1A = new BDB.PP() { Ad = "Koray ATABEY" };
                var mk1B = new BDB.PP() { Ad = "Cenker ÇETİN" };
                var mk1C = new BDB.PP() { Ad = "Soner ALAGÖZ" };
                var mk1D = new BDB.PP() { Ad = "Emre POLAT" };
                var mk1E = new BDB.PP() { Ad = "Mustafa ALP" };
                var mk1F = new BDB.PP() { Ad = "Ahmet EGE" };
                var mk1G = new BDB.PP() { Ad = "Efkan UYSAL" };
                var mk1H = new BDB.PP() { Ad = "Furkan YİĞİT" };
                new BDB.CTP() { CC = cc1, CT = mk1, PP = mk11, Idx = 1 };
                new BDB.CTP() { CC = cc1, CT = mk1, PP = mk12, Idx = 2 };
                new BDB.CTP() { CC = cc1, CT = mk1, PP = mk13, Idx = 3 };
                new BDB.CTP() { CC = cc1, CT = mk1, PP = mk14, Idx = 4 };
                new BDB.CTP() { CC = cc1, CT = mk1, PP = mk15, Idx = 5 };
                new BDB.CTP() { CC = cc1, CT = mk1, PP = mk16, Idx = 6 };
                new BDB.CTP() { CC = cc1, CT = mk1, PP = mk17, Idx = 7 };
                new BDB.CTP() { CC = cc1, CT = mk1, PP = mk18, Idx = 8 };
                new BDB.CTP() { CC = cc1, CT = mk1, PP = mk19, Idx = 9 };
                new BDB.CTP() { CC = cc1, CT = mk1, PP = mk1A, Idx = 10 };
                new BDB.CTP() { CC = cc1, CT = mk1, PP = mk1B, Idx = 11 };
                new BDB.CTP() { CC = cc1, CT = mk1, PP = mk1C, Idx = 12 };
                new BDB.CTP() { CC = cc1, CT = mk1, PP = mk1D, Idx = 13 };
                new BDB.CTP() { CC = cc1, CT = mk1, PP = mk1E, Idx = 14 };
                new BDB.CTP() { CC = cc1, CT = mk1, PP = mk1F, Idx = 15 };
                new BDB.CTP() { CC = cc1, CT = mk1, PP = mk1G, Idx = 16 };
                new BDB.CTP() { CC = cc1, CT = mk1, PP = mk1H, Idx = 16 };
                #endregion MilasKarya-1

                #region Mitos-1
                var mt11 = new BDB.PP() { Ad = "Volkan NAMRUK" };
                var mt12 = new BDB.PP() { Ad = "Kemal GÖKAL" };
                var mt13 = new BDB.PP() { Ad = "Hüseyin ÖZGÜL" };
                var mt14 = new BDB.PP() { Ad = "Ömer AYTAN" };
                var mt15 = new BDB.PP() { Ad = "Namık SARITUNÇ" };
                var mt16 = new BDB.PP() { Ad = "Selin TÜRKOĞLU" };
                var mt17 = new BDB.PP() { Ad = "Murat GÜVENİN" };
                var mt18 = new BDB.PP() { Ad = "Ferhun ERBİR" };
                var mt19 = new BDB.PP() { Ad = "Dilek ERBİR" };
                var mt1a = new BDB.PP() { Ad = "Şener YILDIRIM" };
                var mt1b = new BDB.PP() { Ad = "Gürkan DOĞANAY" };
                new BDB.CTP() { CC = cc1, CT = mt1, PP = mt11, Idx = 1 };
                new BDB.CTP() { CC = cc1, CT = mt1, PP = mt12, Idx = 2 };
                new BDB.CTP() { CC = cc1, CT = mt1, PP = mt13, Idx = 3 };
                new BDB.CTP() { CC = cc1, CT = mt1, PP = mt14, Idx = 4 };
                new BDB.CTP() { CC = cc1, CT = mt1, PP = mt15, Idx = 5 };
                new BDB.CTP() { CC = cc1, CT = mt1, PP = mt16, Idx = 6 };
                new BDB.CTP() { CC = cc1, CT = mt1, PP = mt17, Idx = 7 };
                new BDB.CTP() { CC = cc1, CT = mt1, PP = mt18, Idx = 8 };
                new BDB.CTP() { CC = cc1, CT = mt1, PP = mt19, Idx = 9 };
                new BDB.CTP() { CC = cc1, CT = mt1, PP = mt1a, Idx = 10 };
                new BDB.CTP() { CC = cc1, CT = mt1, PP = mt1b, Idx = 11 };
                #endregion Mitos-1

                #region Ponpin-1
                var pn11 = new BDB.PP() { Ad = "Cengiz ÇORUMLU" };
                var pn12 = new BDB.PP() { Ad = "Kenan TURSAK" };
                var pn13 = new BDB.PP() { Ad = "Arif ERGUVAN" };
                var pn14 = new BDB.PP() { Ad = "Bülent AKDERE" };
                var pn15 = new BDB.PP() { Ad = "Akif KURTULUŞ" };
                var pn16 = new BDB.PP() { Ad = "İlyas EBREM" };
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
                new BDB.CTP() { CC = cc1, CT = pn1, PP = pn11, Idx = 1 };
                new BDB.CTP() { CC = cc1, CT = pn1, PP = pn12, Idx = 2 };
                new BDB.CTP() { CC = cc1, CT = pn1, PP = pn13, Idx = 3 };
                new BDB.CTP() { CC = cc1, CT = pn1, PP = pn14, Idx = 4 };
                new BDB.CTP() { CC = cc1, CT = pn1, PP = pn15, Idx = 5 };
                new BDB.CTP() { CC = cc1, CT = pn1, PP = pn16, Idx = 6 };
                new BDB.CTP() { CC = cc1, CT = pn1, PP = pn17, Idx = 7 };
                new BDB.CTP() { CC = cc1, CT = pn1, PP = pn18, Idx = 8 };
                new BDB.CTP() { CC = cc1, CT = pn1, PP = pn19, Idx = 9 };
                new BDB.CTP() { CC = cc1, CT = pn1, PP = pn1A, Idx = 10 };
                new BDB.CTP() { CC = cc1, CT = pn1, PP = pn1B, Idx = 11 };
                new BDB.CTP() { CC = cc1, CT = pn1, PP = pn1C, Idx = 12 };
                new BDB.CTP() { CC = cc1, CT = pn1, PP = pn1D, Idx = 13 };
                new BDB.CTP() { CC = cc1, CT = pn1, PP = pn1E, Idx = 14 };
                new BDB.CTP() { CC = cc1, CT = pn1, PP = pn1F, Idx = 15 };
                new BDB.CTP() { CC = cc1, CT = pn1, PP = pn1G, Idx = 16 };
                new BDB.CTP() { CC = cc1, CT = pn1, PP = pn1H, Idx = 17 };
                #endregion Ponpin-1

                #region Promil-1
                var pr11 = new BDB.PP() { Ad = "Erhan DOĞRU" };
                var pr12 = new BDB.PP() { Ad = "Çağdaş DURANSOY" };
                var pr13 = new BDB.PP() { Ad = "Mustafa TUTAM" };
                var pr14 = new BDB.PP() { Ad = "Sedat ÖZÜNEM" };
                var pr15 = new BDB.PP() { Ad = "Yenal EGE" };
                var pr16 = new BDB.PP() { Ad = "Şener DEMİRAL", Tel = "533-271-9797", eMail = "sener.demiral@gmail.com" };
                var pr17 = new BDB.PP() { Ad = "Göksan AKAY" };
                var pr18 = new BDB.PP() { Ad = "Emre ESMER" };
                var pr19 = new BDB.PP() { Ad = "Hakan UĞURLU" };
                var pr1A = new BDB.PP() { Ad = "Şevket TAYHAN" };
                var pr1B = new BDB.PP() { Ad = "Ahmethan ACET " };
                var pr1C = new BDB.PP() { Ad = "Ümit ÇETİNALP" };
                var pr1D = new BDB.PP() { Ad = "Adem AYDIN" };
                var pr1E = new BDB.PP() { Ad = "Serdar KILIÇ" };
                var pr1F = new BDB.PP() { Ad = "Emre UZER" };
                var pr1G = new BDB.PP() { Ad = "Tuncer ÖLMEZ" };
                var pr1H = new BDB.PP() { Ad = "Metehan KARADAN" };
                var pr1I = new BDB.PP() { Ad = "Alptekin ARAT" };
                var pr1J = new BDB.PP() { Ad = "M.Ali ÇİÇEK" };
                var pr1K = new BDB.PP() { Ad = "Tolga TAŞKIN" };
                var pr1L = new BDB.PP() { Ad = "Mehmet KARADAĞ" };
                var pr1M = new BDB.PP() { Ad = "Can SÖNMEZ" };
                new BDB.CTP() { CC = cc1, CT = pr1, PP = pr11, Idx = 1 };
                new BDB.CTP() { CC = cc1, CT = pr1, PP = pr12, Idx = 2 };
                new BDB.CTP() { CC = cc1, CT = pr1, PP = pr13, Idx = 3 };
                new BDB.CTP() { CC = cc1, CT = pr1, PP = pr14, Idx = 4 };
                new BDB.CTP() { CC = cc1, CT = pr1, PP = pr15, Idx = 5 };
                new BDB.CTP() { CC = cc1, CT = pr1, PP = pr16, Idx = 6 };
                new BDB.CTP() { CC = cc1, CT = pr1, PP = pr17, Idx = 7 };
                new BDB.CTP() { CC = cc1, CT = pr1, PP = pr18, Idx = 8 };
                new BDB.CTP() { CC = cc1, CT = pr1, PP = pr19, Idx = 9 };
                new BDB.CTP() { CC = cc1, CT = pr1, PP = pr1A, Idx = 10 };
                new BDB.CTP() { CC = cc1, CT = pr1, PP = pr1B, Idx = 11 };
                new BDB.CTP() { CC = cc1, CT = pr1, PP = pr1C, Idx = 12 };
                new BDB.CTP() { CC = cc1, CT = pr1, PP = pr1D, Idx = 13 };
                new BDB.CTP() { CC = cc1, CT = pr1, PP = pr1E, Idx = 14 };
                new BDB.CTP() { CC = cc1, CT = pr1, PP = pr1F, Idx = 15 };
                new BDB.CTP() { CC = cc1, CT = pr1, PP = pr1G, Idx = 16 };
                new BDB.CTP() { CC = cc1, CT = pr1, PP = pr1H, Idx = 17 };
                new BDB.CTP() { CC = cc1, CT = pr1, PP = pr1I, Idx = 18 };
                new BDB.CTP() { CC = cc1, CT = pr1, PP = pr1J, Idx = 19 };
                new BDB.CTP() { CC = cc1, CT = pr1, PP = pr1K, Idx = 20 };
                new BDB.CTP() { CC = cc1, CT = pr1, PP = pr1L, Idx = 21 };
                new BDB.CTP() { CC = cc1, CT = pr1, PP = pr1M, Idx = 22 };
                #endregion Promil

                #region Yalıkavak-1
                var yk11 = new BDB.PP() { Ad = "Hadis BAYRAKTAR" };
                var yk12 = new BDB.PP() { Ad = "Görkem ONUR" };
                var yk13 = new BDB.PP() { Ad = "Oğuzcan BÜTÜNER" };
                var yk14 = new BDB.PP() { Ad = "Kutluğhan TENGİZ" };
                var yk15 = new BDB.PP() { Ad = "Ahmet KÖK" };
                var yk16 = new BDB.PP() { Ad = "Nadir UYSAL" };
                var yk17 = new BDB.PP() { Ad = "Ali ŞENIŞIK" };
                var yk18 = new BDB.PP() { Ad = "Sinan LİMONCUOĞLU" };
                var yk19 = new BDB.PP() { Ad = "İhsan ÇÖKER" };
                var yk1A = new BDB.PP() { Ad = "Oğuz DEVELİ" };
                var yk1B = new BDB.PP() { Ad = "Tahir ÇELİKKESEN" };
                var yk1C = new BDB.PP() { Ad = "Mustafa TOZAN" };
                var yk1D = new BDB.PP() { Ad = "Hasancan SAĞKAL" };
                var yk1E = new BDB.PP() { Ad = "Aydın TOLLUOĞLU" };
                var yk1F = new BDB.PP() { Ad = "Serdar ÖNER" };
                var yk1G = new BDB.PP() { Ad = "Balamir GÜLER" };
                var yk1H = new BDB.PP() { Ad = "Özgürhan ÖZÜNAL" };
                var yk1I = new BDB.PP() { Ad = "Oğuz KESKİNER" };
                var yk1J = new BDB.PP() { Ad = "Ali KAYAR" };
                new BDB.CTP() { CC = cc1, CT = yk1, PP = yk11, Idx = 1 };
                new BDB.CTP() { CC = cc1, CT = yk1, PP = yk12, Idx = 2 };
                new BDB.CTP() { CC = cc1, CT = yk1, PP = yk13, Idx = 3 };
                new BDB.CTP() { CC = cc1, CT = yk1, PP = yk14, Idx = 4 };
                new BDB.CTP() { CC = cc1, CT = yk1, PP = yk15, Idx = 5 };
                new BDB.CTP() { CC = cc1, CT = yk1, PP = yk16, Idx = 6 };
                new BDB.CTP() { CC = cc1, CT = yk1, PP = yk17, Idx = 7 };
                new BDB.CTP() { CC = cc1, CT = yk1, PP = yk18, Idx = 8 };
                new BDB.CTP() { CC = cc1, CT = yk1, PP = yk19, Idx = 9 };
                new BDB.CTP() { CC = cc1, CT = yk1, PP = yk1A, Idx = 10 };
                new BDB.CTP() { CC = cc1, CT = yk1, PP = yk1B, Idx = 11 };
                new BDB.CTP() { CC = cc1, CT = yk1, PP = yk1C, Idx = 12 };
                new BDB.CTP() { CC = cc1, CT = yk1, PP = yk1D, Idx = 13 };
                new BDB.CTP() { CC = cc1, CT = yk1, PP = yk1E, Idx = 14 };
                new BDB.CTP() { CC = cc1, CT = yk1, PP = yk1F, Idx = 15 };
                new BDB.CTP() { CC = cc1, CT = yk1, PP = yk1G, Idx = 16 };
                new BDB.CTP() { CC = cc1, CT = yk1, PP = yk1H, Idx = 17 };
                new BDB.CTP() { CC = cc1, CT = yk1, PP = yk1I, Idx = 18 };
                new BDB.CTP() { CC = cc1, CT = yk1, PP = yk1J, Idx = 19 };
                #endregion Yalıkavak-1

                #endregion 1.Lig ---------------------------------------------



                // Newpon 2B
                var np2B1 = new BDB.PP() { Ad = "Sayım AKAÇIK" };
                var np2B2 = new BDB.PP() { Ad = "Sadullah TEOMAN" };
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
                new BDB.CTP() { CC = cc2B, CT = np2B, PP = np2B1, Idx = 1 };
                new BDB.CTP() { CC = cc2B, CT = np2B, PP = np2B2, Idx = 2 };
                new BDB.CTP() { CC = cc2B, CT = np2B, PP = np2B3, Idx = 3 };
                new BDB.CTP() { CC = cc2B, CT = np2B, PP = np2B4, Idx = 4 };
                new BDB.CTP() { CC = cc2B, CT = np2B, PP = np2B5, Idx = 5 };
                new BDB.CTP() { CC = cc2B, CT = np2B, PP = np2B6, Idx = 6 };
                new BDB.CTP() { CC = cc2B, CT = np2B, PP = np2B7, Idx = 7 };
                new BDB.CTP() { CC = cc2B, CT = np2B, PP = np2B8, Idx = 8 };
                new BDB.CTP() { CC = cc2B, CT = np2B, PP = np2B9, Idx = 9 };
                new BDB.CTP() { CC = cc2B, CT = np2B, PP = np2BA, Idx = 10 };
                new BDB.CTP() { CC = cc2B, CT = np2B, PP = np2BB, Idx = 11 };
                new BDB.CTP() { CC = cc2B, CT = np2B, PP = np2BC, Idx = 12 };
                new BDB.CTP() { CC = cc2B, CT = np2B, PP = np2BD, Idx = 13 };
                new BDB.CTP() { CC = cc2B, CT = np2B, PP = np2BE, Idx = 14 };

                // Gümüşlük-3A
                var gm3A1 = new BDB.PP() { Ad = "Kemal ÇINGI" };
                var gm3A2 = new BDB.PP() { Ad = "Gül GUZELBEY" };
                var gm3A3 = new BDB.PP() { Ad = "Fahrettin TÜRE" };
                var gm3A5 = new BDB.PP() { Ad = "Melih FİDAN" };
                var gm3A6 = new BDB.PP() { Ad = "Melike FELEKOĞLU" };
                var gm3A7 = new BDB.PP() { Ad = "Ahmet DURUKAN" };
                var gm3A8 = new BDB.PP() { Ad = "Beyhan BAĞDATLI" };
                var gm3A9 = new BDB.PP() { Ad = "Hilmi ACAR" };
                var gm3AA = new BDB.PP() { Ad = "Ufuk TURHAN" };
                var gm3AB = new BDB.PP() { Ad = "Sevinç ERSİN" };
                var gm3AC = new BDB.PP() { Ad = "Umut YANIK" };
                var gm3AD = new BDB.PP() { Ad = "Barlas GÖZE" };
                var gm3AE = new BDB.PP() { Ad = "Neşe ODABAŞI" };
                var gm3AF = new BDB.PP() { Ad = "Derya AYDIN" };
                new BDB.CTP() { CC = cc3A, CT = gm3A, PP = gm3A1, Idx = 1 };
                new BDB.CTP() { CC = cc3A, CT = gm3A, PP = gm3A2, Idx = 2 };
                new BDB.CTP() { CC = cc3A, CT = gm3A, PP = gm3A3, Idx = 3 };
                new BDB.CTP() { CC = cc3A, CT = gm3A, PP = gm1D, Idx = 4 };
                new BDB.CTP() { CC = cc3A, CT = gm3A, PP = gm3A5, Idx = 5 };
                new BDB.CTP() { CC = cc3A, CT = gm3A, PP = gm3A6, Idx = 6 };
                new BDB.CTP() { CC = cc3A, CT = gm3A, PP = gm3A7, Idx = 7 };
                new BDB.CTP() { CC = cc3A, CT = gm3A, PP = gm3A8, Idx = 8 };
                new BDB.CTP() { CC = cc3A, CT = gm3A, PP = gm3A9, Idx = 9 };
                new BDB.CTP() { CC = cc3A, CT = gm3A, PP = gm3AA, Idx = 10 };
                new BDB.CTP() { CC = cc3A, CT = gm3A, PP = gm3AB, Idx = 11 };
                new BDB.CTP() { CC = cc3A, CT = gm3A, PP = gm3AC, Idx = 12 };
                new BDB.CTP() { CC = cc3A, CT = gm3A, PP = gm3AD, Idx = 13 };
                new BDB.CTP() { CC = cc3A, CT = gm3A, PP = gm3AE, Idx = 14 };
                new BDB.CTP() { CC = cc3A, CT = gm3A, PP = gm3AF, Idx = 15 };

                // KutayReno-2A
                var kr2A2 = new BDB.PP() { Ad = "Tamer ÖZÇELİK" };
                var kr2A3 = new BDB.PP() { Ad = "Selim YETİM" };
                var kr2A4 = new BDB.PP() { Ad = "Apo KOLARMAN" };
                var kr2A5 = new BDB.PP() { Ad = "Akın CİM" };
                var kr2A6 = new BDB.PP() { Ad = "Ergin DEMİRTAŞ" };
                var kr2A7 = new BDB.PP() { Ad = "Sefer NAMRUK" };
                new BDB.CTP() { CC = cc2A, CT = kr2A, PP = kr19, Idx = 1 };
                new BDB.CTP() { CC = cc2A, CT = kr2A, PP = kr13, Idx = 2 };
                new BDB.CTP() { CC = cc2A, CT = kr2A, PP = kr14, Idx = 3 };
                new BDB.CTP() { CC = cc2A, CT = kr2A, PP = kr15, Idx = 4 };
                new BDB.CTP() { CC = cc2A, CT = kr2A, PP = kr16, Idx = 5 };
                new BDB.CTP() { CC = cc2A, CT = kr2A, PP = kr17, Idx = 6 };
                new BDB.CTP() { CC = cc2A, CT = kr2A, PP = kr18, Idx = 7 };
                new BDB.CTP() { CC = cc2A, CT = kr2A, PP = kr1A, Idx = 8 };
                new BDB.CTP() { CC = cc2A, CT = kr2A, PP = kr1E, Idx = 9 };
                new BDB.CTP() { CC = cc2A, CT = kr2A, PP = kr1B, Idx = 10 };
                new BDB.CTP() { CC = cc2A, CT = kr2A, PP = kr2A2, Idx = 11 };
                new BDB.CTP() { CC = cc2A, CT = kr2A, PP = kr2A3, Idx = 12 };
                new BDB.CTP() { CC = cc2A, CT = kr2A, PP = kr2A4, Idx = 13 };
                new BDB.CTP() { CC = cc2A, CT = kr2A, PP = kr2A5, Idx = 14 };
                new BDB.CTP() { CC = cc2A, CT = kr2A, PP = kr2A6, Idx = 15 };
                new BDB.CTP() { CC = cc2A, CT = kr2A, PP = kr2A7, Idx = 16 };

                // KutayReno-3A
                var kr3A1 = new BDB.PP() { Ad = "Serdar PAMUKKALE" };
                var kr3A2 = new BDB.PP() { Ad = "Cem DEMİRCAN" };
                var kr3A3 = new BDB.PP() { Ad = "Turgay GÜNGÖR" };
                var kr3A4 = new BDB.PP() { Ad = "Barış NAMRUK" };
                var kr3A5 = new BDB.PP() { Ad = "Melih SAYİP" };
                var kr3A6 = new BDB.PP() { Ad = "Tuncer TELÖREN" };
                var kr3A7 = new BDB.PP() { Ad = "Özlem SÜTÇÜ" };
                var kr3A8 = new BDB.PP() { Ad = "Kadriye BOZDAĞ" };
                var kr3A9 = new BDB.PP() { Ad = "Ayten NAMRUK" };
                var kr3AA = new BDB.PP() { Ad = "Deniz ÖZ" };
                var kr3AB = new BDB.PP() { Ad = "Gamze DİRİL" };
                var kr3AC = new BDB.PP() { Ad = "Yıldıray UZAR" };
                new BDB.CTP() { CC = cc3A, CT = kr3A, PP = kr3A1, Idx = 1 };
                new BDB.CTP() { CC = cc3A, CT = kr3A, PP = kr3A2, Idx = 2 };
                new BDB.CTP() { CC = cc3A, CT = kr3A, PP = kr3A3, Idx = 3 };
                new BDB.CTP() { CC = cc3A, CT = kr3A, PP = kr3A4, Idx = 4 };
                new BDB.CTP() { CC = cc3A, CT = kr3A, PP = kr3A5, Idx = 5 };
                new BDB.CTP() { CC = cc3A, CT = kr3A, PP = kr3A6, Idx = 6 };
                new BDB.CTP() { CC = cc3A, CT = kr3A, PP = kr3A7, Idx = 7 };
                new BDB.CTP() { CC = cc3A, CT = kr3A, PP = kr3A8, Idx = 8 };
                new BDB.CTP() { CC = cc3A, CT = kr3A, PP = kr3A9, Idx = 9 };
                new BDB.CTP() { CC = cc3A, CT = kr3A, PP = kr3AA, Idx = 10 };
                new BDB.CTP() { CC = cc3A, CT = kr3A, PP = kr3AB, Idx = 11 };
                new BDB.CTP() { CC = cc3A, CT = kr3A, PP = kr3AC, Idx = 12 };

                // Kekik-2A
                var kk2A2 = new BDB.PP() { Ad = "Kadir ÇİFTÇİ" };
                var kk2A3 = new BDB.PP() { Ad = "" };
                var kk2A4 = new BDB.PP() { Ad = "İbrahim MOLLA" };
                var kk2A6 = new BDB.PP() { Ad = "M.Emin SEKİN" };
                var kk2A7 = new BDB.PP() { Ad = "Nezih YAĞAR" };
                var kk2A8 = new BDB.PP() { Ad = "Zafer YIKAR" };
                var kk2A9 = new BDB.PP() { Ad = "Selim YAPAR" };
                var kk2Aa = new BDB.PP() { Ad = "Nuran KANSU" };
                var kk2Ab = new BDB.PP() { Ad = "Reyhan KANBER" };
                var kk2Ac = new BDB.PP() { Ad = "Aslı NARİN" };
                var kk2Ad = new BDB.PP() { Ad = "Deniz ER" };
                var kk2Ae = new BDB.PP() { Ad = "Erdinç ER" };
                var kk2Af = new BDB.PP() { Ad = "Tufan GÜVEN" };
                var kk2Ag = new BDB.PP() { Ad = "Cenkmen ERÇAK" };
                var kk2Ah = new BDB.PP() { Ad = "Timuçin BİNDER" };
                var kk2Ai = new BDB.PP() { Ad = "Hulusi ŞAHİN" };
                var kk2Aj = new BDB.PP() { Ad = "Alaattin CENGİZ" };
                var kk2Ak = new BDB.PP() { Ad = "Ufuk ALTINTABAK" };
                var kk2Al = new BDB.PP() { Ad = "Monika MUNZINGER" };
                var kk2Am = new BDB.PP() { Ad = "Şener KURT" };
                var kk2An = new BDB.PP() { Ad = "Mariedo KURT" };
                var kk2Ao = new BDB.PP() { Ad = "Songül YILMAZ" };
                var kk2Ap = new BDB.PP() { Ad = "Belen ÜNAL" };
                new BDB.CTP() { CC = cc2A, CT = kk2A, PP = kk18, Idx = 1 };
                new BDB.CTP() { CC = cc2A, CT = kk2A, PP = kk2A2, Idx = 2 };
                new BDB.CTP() { CC = cc2A, CT = kk2A, PP = kk2A3, Idx = 3 };
                new BDB.CTP() { CC = cc2A, CT = kk2A, PP = kk2A4, Idx = 4 };
                new BDB.CTP() { CC = cc2A, CT = kk2A, PP = kk15, Idx = 5 };
                new BDB.CTP() { CC = cc2A, CT = kk2A, PP = kk2A6, Idx = 6 };
                new BDB.CTP() { CC = cc2A, CT = kk2A, PP = kk2A7, Idx = 7 };
                new BDB.CTP() { CC = cc2A, CT = kk2A, PP = kk2A8, Idx = 8 };
                new BDB.CTP() { CC = cc2A, CT = kk2A, PP = kk2A9, Idx = 9 };
                new BDB.CTP() { CC = cc2A, CT = kk2A, PP = kk2Aa, Idx = 10 };
                new BDB.CTP() { CC = cc2A, CT = kk2A, PP = kk2Ab, Idx = 11 };
                new BDB.CTP() { CC = cc2A, CT = kk2A, PP = kk2Ac, Idx = 12 };
                new BDB.CTP() { CC = cc2A, CT = kk2A, PP = kk2Ad, Idx = 13 };
                new BDB.CTP() { CC = cc2A, CT = kk2A, PP = kk2Ae, Idx = 14 };
                new BDB.CTP() { CC = cc2A, CT = kk2A, PP = kk2Af, Idx = 15 };
                new BDB.CTP() { CC = cc2A, CT = kk2A, PP = kk2Ag, Idx = 16 };
                new BDB.CTP() { CC = cc2A, CT = kk2A, PP = kk2Ah, Idx = 17 };
                new BDB.CTP() { CC = cc2A, CT = kk2A, PP = kk2Ai, Idx = 18 };
                new BDB.CTP() { CC = cc2A, CT = kk2A, PP = kk2Aj, Idx = 19 };
                new BDB.CTP() { CC = cc2A, CT = kk2A, PP = kk2Ak, Idx = 20 };
                new BDB.CTP() { CC = cc2A, CT = kk2A, PP = kk2Al, Idx = 21 };
                new BDB.CTP() { CC = cc2A, CT = kk2A, PP = kk2Am, Idx = 22 };
                new BDB.CTP() { CC = cc2A, CT = kk2A, PP = kk2An, Idx = 23 };
                new BDB.CTP() { CC = cc2A, CT = kk2A, PP = kk2Ao, Idx = 24 };
                new BDB.CTP() { CC = cc2A, CT = kk2A, PP = kk2Ap, Idx = 25 };
            });


        }

    }
}
