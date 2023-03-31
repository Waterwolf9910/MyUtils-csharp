using Microsoft.EntityFrameworkCore.ChangeTracking;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Readers;

namespace MyUtils {
    public static partial class Utils {

        public static bool ExtractFileOrDir(string archivePath, string exportPath, string? fileToExport = null, string? filename = null) {
            using var stream = File.OpenRead(archivePath);
            using var wrapper = ReaderWrapper.GetReader(stream);
            var reader = wrapper.reader;
            if (!Directory.Exists(exportPath)) {
                Directory.CreateDirectory(exportPath);
            }
            if (filename != null && fileToExport == null) {
                throw new ArgumentNullException(nameof(fileToExport), "fileToExport cannot be null if filename is not null");
            }
            if (fileToExport != null) {
                while (reader.MoveToNextEntry()) {
                    if (!reader.Entry.IsDirectory && reader.Entry.Key.EndsWith(fileToExport)) {
                        reader.WriteEntryTo(Path.GetFullPath(filename ?? fileToExport, exportPath));
                        return true;
                    }
                }
                return false;
            } else {
                try {
                    reader.WriteAllToDirectory(Path.GetFullPath(exportPath), new() {
                        ExtractFullPath = true,
                        Overwrite = true
                        // PreserveAttributes = true
                    });
                }
                catch {
                    return false;
                }
            }
            try {
                if (Directory.GetFiles(exportPath).Length < 1 && Directory.GetDirectories(exportPath).Length == 1) {
                    var loneDir = Directory.GetDirectories(exportPath).First();
                    var dirs = Directory.GetDirectories(loneDir);
                    var files = Directory.GetFiles(loneDir);
                    foreach (var dir in dirs) {
                        Directory.Move(dir, Path.GetFullPath(Path.GetFileName(dir), exportPath));
                    }
                    foreach (var file in files) {
                        Directory.Move(file, Path.GetFullPath(Path.GetFileName(file), exportPath));
                    }
                    Directory.Delete(loneDir);
                }
            }
            catch {
                return false;
            }
            return true;
        }

        public static Dictionary<string, bool> ExtractFiles(string archivePath, string exportPath, params (string name, string? outputName)[] filesToExport) {
            using var stream = File.OpenRead(archivePath);
            using var wrapper = ReaderWrapper.GetReader(stream);
            var reader = wrapper.reader;
            var results = new Dictionary<string, bool>();
            if (!Directory.Exists(exportPath)) {
                Directory.CreateDirectory(exportPath);
            }
            foreach (var (name, _) in filesToExport) {
                results.Add(name, false);
            }
            if (filesToExport != null) {
                while (reader.MoveToNextEntry()) {
                    foreach (var (name, outputName) in filesToExport) {
                        if (!reader.Entry.IsDirectory && name == reader.Entry.Key) {
                            reader.WriteEntryTo(Path.GetFullPath(outputName ?? name, exportPath));
                            results.Remove(name);
                            results.Add(name, true);
                        }
                    }
                }
                return results;
            } else {
                try {
                    reader.WriteAllToDirectory(Path.GetFullPath(exportPath), new() {
                        ExtractFullPath = true,
                        Overwrite = true
                        // PreserveAttributes = true
                    });
                }
                catch {
                    return results;
                }
            }
            try {
                if (Directory.GetFiles(exportPath).Length < 1 && Directory.GetDirectories(exportPath).Length == 1) {
                    var loneDir = Directory.GetDirectories(exportPath).First();
                    var dirs = Directory.GetDirectories(loneDir);
                    var files = Directory.GetFiles(loneDir);
                    foreach (var dir in dirs) {
                        Directory.Move(dir, Path.GetFullPath(Path.GetFileName(dir), exportPath));
                    }
                    foreach (var file in files) {
                        Directory.Move(file, Path.GetFullPath(Path.GetFileName(file), exportPath));
                    }
                    Directory.Delete(loneDir);
                }
            }
            catch {
                return results;
            }
            return results;
        }

        public static ValueComparer ListVC<T>() where T : notnull {

            return new ValueComparer<List<T>>(
                (v1, v2) => v1 != null && v2 != null && v1.SequenceEqual(v2),
                list => list.Aggregate(0, (hash, val) => HashCode.Combine(hash, val.GetHashCode())),
                list => list.ToList()
            );
        }
        public static ValueComparer DictVC<TKey, TVal>() where TKey : notnull {
            return new ValueComparer<Dictionary<TKey, TVal>>(
                (v1, v2) => v1 != null && v2 != null && v1.SequenceEqual(v2),
                dict => dict.Aggregate(0, (hash, val) => HashCode.Combine(hash, val.GetHashCode())),
                dict => new Dictionary<TKey, TVal>(dict)
            );
        }

        private class ReaderWrapper : IDisposable {

            public bool Disposed {
                get; private set;
            }
            public IReader reader;

            public void Dispose() {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            private ReaderWrapper(Stream file) {
                try {
                    reader = ReaderFactory.Open(file);
                }
                catch (InvalidOperationException) {
                    reader = SevenZipArchive.Open(file).ExtractAllEntries();
                }
            }

            public static ReaderWrapper GetReader(Stream file) {
                return new ReaderWrapper(file);
            }

            public void Dispose(bool disposing) {
                if (Disposed) {
                    return;
                }
                if (disposing) {
                    reader.Dispose();
                }
                Disposed = true;
            }
        }
    }
}
