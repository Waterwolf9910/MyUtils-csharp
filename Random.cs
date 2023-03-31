using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyUtils {
    public class MyRandom {

        public static MyRandom Shared { get; private set; } = new();

        private int Length {
            get;
        }

        private Random Ran { get; } = new(Random.Shared.Next(1_000, 50_000));

        private static bool regRefresh = false;
        private static void RefreshShared() {
            var timer = new System.Timers.Timer() {
                Interval = 300_000,
                AutoReset = true,
                Enabled = true,
            };

            timer.Elapsed += (_, _) => {
                Shared = new();
            };
        }

        public MyRandom(int length = 32) {
            if (!regRefresh) {
                RefreshShared();
                regRefresh = true;
            }
            this.Length = length;
        }

        // Hooks into Random.Next
        public int Int(int max = int.MaxValue, int min = 0) {
            return Ran.Next(min, max);
        }

        public long Long(long max = long.MaxValue, long min = 0) {
            return Ran.NextInt64(min, max);
        }

        public void Bytes(byte[] buffer) {
            Ran.NextBytes(buffer);
        }

        public float Float() {
            return Ran.NextSingle();
        }

        public double Double() {
            return Ran.NextDouble();
        }

        public string Alpha(int length, bool caps = false) {
            string returner = "";
            while (returner.Length < length) {
                string chara = Alphabet[Ran.Next(26)];
                if (caps && Ran.Next(3) == Ran.Next(3)) {
                    chara = chara.ToUpper();
                }
                returner = $"{returner}{chara}";
            }
            return returner;
        }

        public string Alpha(bool caps = false) {
            return Alpha(this.Length, caps);
        }

        public string Special(int length) {
            string returner = "";
            while (returner.Length < length) {
                returner = $"{returner}{SpecialChars[Ran.Next(32)]}";
            }
            return returner;
        }

        public string Special() {
            return Special(this.Length);
        }

        public string AlphaNum(bool caps = false) {
            string returner = "";
            while (returner.Length < Length) {
                returner = $"{returner}{( Ran.Next(2) == Ran.Next(2) ? Int(9) : Alpha(1, caps) )}";
            }
            return returner;
        }

        public string AlphaSpecial(bool caps = false) {
            string returner = "";
            while (returner.Length < this.Length) {
                returner = $"{returner}{( Ran.Next(2) == Ran.Next(2) ? Alpha(1, caps) : Special(1) )}";
            }
            return returner;
        }

        public string NumberSpecial() {
            string returner = "";
            while (returner.Length < this.Length) {
                returner = $"{returner}${( Ran.Next(2) == Ran.Next(2) ? Ran.Next(10) : Special(1) )}";
            }
            return returner;
        }

        public string AlphaNumSpecial(int length, bool caps = false) {
            string returner = "";
            while (returner.Length < length) {
                switch (Ran.Next(3)) {
                    case 0: {
                        returner = $"{returner} {Ran.Next(10)}";
                        break;
                    }
                    case 1: {
                        returner = $"{returner} {Alpha(1, caps)}";
                        break;
                    }
                    case 2: {
                        returner = $"{returner} {Special(1)}";
                        break;
                    }
                }
            }

            return returner;
        }

        public string AlphaNumSpecial(bool caps = false) {
            return AlphaNumSpecial(this.Length, caps);
        }

        private static string[] Alphabet {
            get;
        } = {
            "a",
            "b",
            "c",
            "d",
            "e",
            "f",
            "g",
            "h",
            "i",
            "j",
            "k",
            "l",
            "m",
            "n",
            "o",
            "p",
            "q",
            "r",
            "s",
            "t",
            "u",
            "v",
            "w",
            "x",
            "y",
            "z"
        };

        private static string[] SpecialChars {
            get;
        } = {
            "`",
            "~",
            "!",
            "@",
            "#",
            "$",
            "%",
            "^",
            "&",
            "*",
            "(",
            ")",
            "-",
            "_",
            "=",
            "+",
            "[",
            "{",
            "]",
            "}",
            "\\",
            "|",
            ":",
            ";",
            "'",
            "\"",
            ",",
            "<",
            ".",
            ">",
            "/",
            "?"
        };

    }
}
