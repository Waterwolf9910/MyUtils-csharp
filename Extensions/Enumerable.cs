
namespace MyUtils.Extensions {
    public static class Enumerable {
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source, IEqualityComparer<TKey>? comparer = null) where TKey : notnull {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }

            if (source is ICollection<KeyValuePair<TKey, TValue>> collection) {
                if (collection.Count == 0) {
                    return new Dictionary<TKey, TValue>();
                }
                if (collection is KeyValuePair<TKey, TValue>[] array) {
                    var dict = new Dictionary<TKey, TValue>(array.Length, comparer);
                    foreach (var kvp in array) {
                        dict.Add(kvp.Key, kvp.Value);
                    }
                    return dict;
                }
                if (collection is List<KeyValuePair<TKey, TValue>> list) {
                    var dict = new Dictionary<TKey, TValue>(list.Count, comparer);
                    foreach (var kvp in list) {
                        dict.Add(kvp.Key, kvp.Value);
                    }
                    return dict;
                }
            }

            var _dict = new Dictionary<TKey, TValue>(comparer);
            foreach (var kvp in source) {
                _dict.Add(kvp.Key, kvp.Value);
            }
            return _dict;
        }

        /*public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>?> source, IEqualityComparer<TKey>? comparer = null) where TKey : notnull {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }

            if (source is ICollection<KeyValuePair<TKey, TValue>> collection) {
                if (collection.Count == 0) {
                    return new Dictionary<TKey, TValue>();
                }
                if (collection is KeyValuePair<TKey, TValue>[] array) {
                    var dict = new Dictionary<TKey, TValue>(array.Length, comparer);
                    foreach (var kvp in array) {
                        dict.Add(kvp.Key, kvp.Value);
                    }
                    return dict;
                }
                if (collection is List<KeyValuePair<TKey, TValue>> list) {
                    var dict = new Dictionary<TKey, TValue>(list.Count, comparer);
                    foreach (var kvp in list) {
                        dict.Add(kvp.Key, kvp.Value);
                    }
                    return dict;
                }
            }

            var _dict = new Dictionary<TKey, TValue>(comparer);
            foreach (var kvp in source) {
                if (kvp is not null) {
                    _dict.Add(kvp.Value.Key, kvp.Value.Value);
                }
            }
            return _dict;
        }*/

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<(TKey, TValue)> source, IEqualityComparer<TKey>? comparer = null) where TKey : notnull {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }

            if (source is ICollection<(TKey, TValue)> collection) {
                if (collection.Count == 0) {
                    return new Dictionary<TKey, TValue>();
                }
                if (collection is (TKey, TValue)[] array) {
                    var dict = new Dictionary<TKey, TValue>(array.Length, comparer);
                    foreach (var kvp in array) {
                        dict.Add(kvp.Item1, kvp.Item2);
                    }
                    return dict;
                }
                if (collection is List<(TKey, TValue)> list) {
                    var dict = new Dictionary<TKey, TValue>(list.Count, comparer);
                    foreach (var kvp in list) {
                        dict.Add(kvp.Item1, kvp.Item2);
                    }
                    return dict;
                }
            }

            var _dict = new Dictionary<TKey, TValue>(comparer);
            foreach (var kvp in source) {
                _dict.Add(kvp.Item1, kvp.Item2);
            }
            return _dict;
        }

        /*public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<(TKey, TValue)?> source, IEqualityComparer<TKey>? comparer = null) where TKey : notnull {
            if (source == null) {
                throw new ArgumentNullException(nameof(source));
            }

            if (source is ICollection<(TKey, TValue)> collection) {
                if (collection.Count == 0) {
                    return new Dictionary<TKey, TValue>();
                }
                if (collection is (TKey, TValue)[] array) {
                    var dict = new Dictionary<TKey, TValue>(array.Length, comparer);
                    foreach (var kvp in array) {
                        dict.Add(kvp.Item1, kvp.Item2);
                    }
                    return dict;
                }
                if (collection is List<(TKey, TValue)> list) {
                    var dict = new Dictionary<TKey, TValue>(list.Count, comparer);
                    foreach (var kvp in list) {
                        dict.Add(kvp.Item1, kvp.Item2);
                    }
                    return dict;
                }
            }

            var _dict = new Dictionary<TKey, TValue>(comparer);
            foreach (var kvp in source) {
                if (kvp is not null) {
                    _dict.Add(kvp.Value.Item1, kvp.Value.Item2);
                }            
            }
            return _dict;
        }*/

        //https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
        public static int Shuffle<T>(this IList<T> list, int curIndex = 0) {
            if (list == null) {
                throw new ArgumentNullException(nameof(list));
            }

            int newIndex = curIndex;

            int size = list.Count;

            while (size > 1) {
                size--;
                int i = MyRandom.Shared.Int(size + 1);
                if (newIndex == size) {
                    newIndex = i;
                } else if (newIndex == i) {
                    newIndex = size;
                }
                (list[size], list[i]) = (list[i], list[size]);
            }

            return newIndex;
        }
    }
}
