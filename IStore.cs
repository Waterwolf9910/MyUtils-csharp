using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace NFCKeyApp {
    public abstract class IStore<T> {

        public string Filename {
            init;
            get;
        }

        public event Action? StoreEdited;

        protected Dictionary<string, T> Store;

        #pragma warning disable CS8618 // Save and Load causes Store to be set
        public IStore(string Filename) {
        #pragma warning restore CS8618
            this.Filename = Filename;
            if (!File.Exists(Filename)) {
                Store = new();
                Save();
                return;
            }
            Load();
        }

        protected virtual void Save() {
            OnStoreEdited();
            File.WriteAllText(Filename, Convert.ToBase64String(Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(Store))));
        }

        protected virtual void Load() {
            var data = File.ReadAllText(Filename);
            var b64 = Convert.FromBase64String(data);
            var json = Encoding.UTF8.GetString(b64);
            Store = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, T>>(json) ?? new();
            OnStoreEdited();
        }

#if DEBUG
        public void ForceLoad() {
            this.Load();
        }
#endif

        protected virtual void OnStoreEdited() {
            StoreEdited?.Invoke();
        }

        public virtual ImmutableList<T> GetAllKeys() {
            return Store.Values.ToImmutableList();
        }

        public virtual ImmutableDictionary<string, T> GetData() {
            return Store.ToImmutableDictionary();
        }

        public virtual T GetKey(string id) {
            return Store[id];
        }

        public void SetKey(string id, T value) {
            Store[id] = value;
            Save();
        }

        public void Remove(string id) {
            Store.Remove(id);
            Save();
        }

        public void Clear() {
            Store.Clear();
            Save();
        }
    }
}
