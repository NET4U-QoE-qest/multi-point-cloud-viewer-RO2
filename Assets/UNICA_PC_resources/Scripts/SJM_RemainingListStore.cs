using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SJM_RemainingListStore
{
    [Serializable]
    class RemainingState
    {
        public int version = 1;
        public List<string> remainingIds = new();
    }

    // Percorso completo dentro persistentDataPath
    public static string GetPath(string fileName)
    {
        return Path.Combine(Application.persistentDataPath, fileName);
    }

    // --- API: SALVA/LOAD SOLO LISTE DI NOMI ---

    public static void WriteEmpty(string path)
    {
        // Scrive uno stato vuoto: { "version": 1, "remainingIds": [] }
        SaveIdsAtomic(path, System.Array.Empty<string>());
    }

    public static void SaveIdsAtomic(string path, IEnumerable<string> ids)
    {
        try
        {
            var state = new RemainingState { remainingIds = new List<string>(ids) };
            var json = JsonUtility.ToJson(state);

            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);

            var tmp = path + ".tmp";
            var bak = path + ".bak";

            using (var fs = new FileStream(tmp, FileMode.Create, FileAccess.Write, FileShare.None, 4096, FileOptions.WriteThrough))
            using (var sw = new StreamWriter(fs))
            {
                sw.Write(json);
                sw.Flush();
                fs.Flush(true);
            }

            if (File.Exists(path))
            {
                try { File.Copy(path, bak, true); } catch { /* best-effort */ }
            }

            File.Copy(tmp, path, true);
            try { File.Delete(tmp); } catch { /* best-effort */ }
        }
        catch (Exception e)
        {
            Debug.LogError($"[SJM_RemainingListStore] Save error: {e}");
        }
    }

    public static bool TryLoadIds(string path, out List<string> ids)
    {
        ids = null;
        if (TryRead(path, out ids)) return true;
        if (TryRead(path + ".tmp", out ids)) return true;
        if (TryRead(path + ".bak", out ids)) return true;
        return false;
    }

    static bool TryRead(string path, out List<string> ids)
    {
        ids = null;
        try
        {
            if (!File.Exists(path)) return false;
            var json = File.ReadAllText(path);
            if (string.IsNullOrWhiteSpace(json)) return false;
            var state = JsonUtility.FromJson<RemainingState>(json);
            if (state?.remainingIds == null) return false;
            ids = state.remainingIds;
            return true;
        }
        catch { return false; }
    }

    // --- Helper comodi ---

    // Ritorna i nomi (ID) dai prefab, evitando null
    public static List<string> NamesOf(IEnumerable<GameObject> prefabs)
    {
        var list = new List<string>();
        if (prefabs == null) return list;
        var set = new HashSet<string>(StringComparer.Ordinal);
        foreach (var go in prefabs)
        {
            if (go == null) continue;
            if (set.Add(go.name)) list.Add(go.name); // no duplicati di nome
        }
        return list;
    }

    // Ricostruisce la lista di prefab da una lista di nomi (filtra quelli non più presenti)
    public static List<GameObject> RebuildFromIds(IEnumerable<string> ids, IEnumerable<GameObject> sourcePrefabs)
    {
        var rebuilt = new List<GameObject>();
        if (ids == null || sourcePrefabs == null) return rebuilt;

        var map = new Dictionary<string, GameObject>(StringComparer.Ordinal);
        foreach (var go in sourcePrefabs)
        {
            if (go == null) continue;
            if (!map.ContainsKey(go.name)) map.Add(go.name, go);
        }

        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (var id in ids)
        {
            if (id == null) continue;
            if (seen.Add(id) && map.TryGetValue(id, out var prefab) && prefab != null)
            {
                rebuilt.Add(prefab);
            }
        }
        return rebuilt;
    }
}
