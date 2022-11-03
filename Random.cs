using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyUtils {
    public class MyRandom {

        public static MyRandom Shared { get; private set; } = new();

        private int length {
            get;
        }
        private Random random { get; } = new(Random.Shared.Next(1_000, 50_000));

        private static bool regRefresh = false;
        private static void RefreshShared() {
            var timer = new System.Timers.Timer() {
                Interval = 300_000,
                AutoReset = true,
                Enabled = true,
            };

            timer.Elapsed += (a, b) => {
                Shared = new();
            };
        }

        public MyRandom(int length = 25) {
            if (!regRefresh) {
                RefreshShared();
                regRefresh = true;
            }
            this.length = length;
        }

        // Hooks into Random.Next
        public int Int(int max = int.MaxValue, int min = 0) {
            return random.Next(min, max);
        }

        public long Long(long max = long.MaxValue, long min = 0) {
            return random.NextInt64(min, max);
        }

        public void Bytes(byte[] buffer) {
            random.NextBytes(buffer);
        }

        public float Float() {
            return random.NextSingle();
        }

        public double Double() {
            return random.NextDouble();
        }

        public string Alpha(int length, bool caps = false) {
            string returner = "";
            while (returner.Length < length) {
                string chara = Alphabet[random.Next(26)];
                if (caps && random.Next(3) == random.Next(3)) {
                    chara = chara.ToUpper();
                }
                returner = $"{returner}{chara}";
            }
            return returner;
        }

        public string Alpha(bool caps = false) {
            return Alpha(this.length, caps);
        }

        public string Special(int length) {
            string returner = "";
            while (returner.Length < length) {
                returner = $"{returner}{SpecialChars[random.Next(32)]}";
            }
            return returner;
        }

        public string Special() {
            return Special(this.length);
        }

        public string AlphaNum(bool caps = false) {
            string returner = "";
            while (returner.Length < length) {
                returner = $"{returner}{(random.Next(2) == random.Next(2) ? Int(9) : Alpha(1, caps))}";
            }
            return returner;
        }

        public string AlphaSpecial(bool caps = false) {
            string returner = "";
            while (returner.Length < this.length) {
                returner = $"{returner}{(random.Next(2) == random.Next(2) ? Alpha(1, caps) : Special(1))}";
            }
            return returner;
        }

        public string NumberSpecial() {
            string returner = "";
            while (returner.Length < this.length) {
                returner = $"{returner}${(random.Next(2) == random.Next(2) ? random.Next(10) : Special(1))}";
            }
            return returner;
        }

        public string AlphaNumSpecial(int length, bool caps = false) {
            string returner = "";
            while (returner.Length < length) {
                switch (random.Next(3)) {
                    case 0: {
                        returner = $"{returner} {random.Next(10)}";
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
            return AlphaNumSpecial(this.length, caps);
        }

        private static string[] Alphabet { get; } = {
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

        private static string[] SpecialChars { get; } = {
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
