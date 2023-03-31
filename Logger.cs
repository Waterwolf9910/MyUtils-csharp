using System;
using System.Collections;

namespace MyUtils {
    public class LogSettings {
        public string Name = "main";
        public string Path = System.IO.Path.GetFullPath("logs/main.log");

    }
    public class Logger {
        private static string FileSuffix { get; } = DateTime.Now.ToString("MMddyy-hhmmss");
        protected string LogPath {
            get;
        }
        protected LogSettings Settings {
            get;
        }
        protected static bool DebugMode { get; private set; } = false;
        protected string BaseMessage() {
            return $"[{FormatDate()}] [{Settings.Name}]";
        }
        protected Queue<string> queue { get; } = new();
        private static Logger Main { get; } = new(new());

        public static Logger GetMain() {
            return Main;
        }

        public Logger(LogSettings? settings = null) {
            settings ??= new();
            if (!Directory.Exists(Path.GetFullPath("logs"))) {
                Directory.CreateDirectory(Path.GetFullPath("logs"));
            }
            if (!settings.Path.EndsWith(".log")) {
                settings.Path = $"{settings.Path}.log";
            }
            if (!Path.IsPathRooted(settings.Path) && !Path.IsPathFullyQualified(settings.Path) && !settings.Path.Contains("..")) {
                settings.Path = $"logs/{settings.Path}";
            }
            Settings = settings;
            LogPath = Path.GetFullPath(settings.Path.Replace(".log", $"-{FileSuffix}.log"));
            if (!Directory.Exists(Path.GetDirectoryName(LogPath))) {
                throw new ArgumentException($"{Path.GetDirectoryName(LogPath)} does not exist");
            }
            // System.IO.File.CreateText()
        }

        public void Log(params object[]? message) {
            var returnString = GetReturnString(in message, "log");
            Write(returnString);
            Console.WriteLine(returnString);
        }

        public void WriteLog(params object[]? message) {
            Write(GetReturnString(in message, "log"));
        }

        public void Warn(params object[]? message) {
            var returnString = GetReturnString(in message, "warn");
            Write(returnString);
            Console.WriteLine(returnString);
        }

        public void WriteWarn(params object[]? message) {
            Write(GetReturnString(in message, "warn"));
        }

        public void Error(params object[]? message) {
            var returnString = GetReturnString(in message, "error");
            Write(returnString);
            Console.WriteLine(returnString);
        }

        public void WriteError(params object[]? message) {
            Write(GetReturnString(in message, "error"));
        }

        public void Fatal(Exception e, params object[]? message) {
            string returnString = GetReturnString(in message, "fatal");
            returnString = $"{returnString}\n${( returnString != null ? "    " : "" )}{e.Message}: {e.StackTrace}";
            Write(returnString);
            Console.WriteLine(returnString);
        }

        public void WriteFatal(Exception e, params object[]? message) {
            string returnString = GetReturnString(in message, "fatal");
            returnString = $"{returnString}\n${( returnString != null ? "    " : "" )}{e.Message}: {e.StackTrace}";
            Write(returnString);
        }

        public void Debug(params object[]? message) {
            if (!DebugMode) {
                return;
            }
            var returnString = GetReturnString(in message, "debug");
            Write(returnString);
            Console.WriteLine(returnString);
        }

        public void WriteDebug(params object[]? message) {
            if (!DebugMode) {
                return;
            }
            Write(GetReturnString(in message, "debug"));
        }


        public static void SetDebugMode(bool mode) {
            DebugMode = mode;
        }

        protected string GetReturnString(in object[]? message, string type) {
            if (message == null || "".Equals(message[0])) {
                return "";
            }
            string returnString = "";
            var objects = message;
            for (int i = 0; i < objects.Length; i++) {
                object obj = objects[i];

                if (obj is IList list) {
                    LoopArray(list, ref returnString);
                    continue;
                }
                returnString = string.Format($"{{0}}{( returnString == "" ? "" : " " )}{{1}}", returnString, obj);
            }
            if (returnString == "") {
                return "";
            }
            returnString = string.Format("{0} [{1}] {2}", BaseMessage(), type, returnString);
            return returnString;
        }

        protected void Write(string data, int trys = 0) {
            try {
                // queue.
                File.AppendAllText(LogPath, $"{data}\n", System.Text.Encoding.UTF8);
            }
            catch {
                if (trys > 2) {
                    return;
                }
                var timer = new System.Timers.Timer() {
                    Enabled = true,
                    Interval = 50
                };
                timer.Elapsed += (_, _) => {
                    timer.Dispose();
                    Write(data, trys + 1);
                };
            }
        }

        protected static void LoopArray(IList arr, ref string returnString, int depth = 0) {
            for (int i = 0; i < arr.Count; i++) {
                var obj = arr[i];
                if (obj is IList list) {
                    LoopArray(list, ref returnString, depth + 1);
                    continue;
                }
                returnString = string.Format($"{returnString} {( i == 0 ? "[ " : "" )}{{0}}{( i == arr.Count - 1 ? " ]" : "," )}", obj);
            }
            if (depth == 0) {
                returnString = $"{returnString} ]";
            }
        }

        protected static string FormatDate() {
            return DateTime.Now.ToString("dd/MM/yyyy-hh:mm:ss");
            // DateTime date = DateTime.Now;
            // return $"{(date.Day < 10 ? $"0{date.Day}" : date.Day)}/{(date.Month < 10 ? $"0{date.Month}" : date.Month)}/{(date.Year < 10 ? $"0{date.Year}" : date.Year)}-{(date.Hour < 10 ? $"0{date.Hour}" : date.Hour)}:{(date.Minute < 10 ? $"0{date.Minute}" : date.Minute)}:{(date.Second < 10 ? $"0{date.Second}" : date.Second)}";
        }

    }
}
