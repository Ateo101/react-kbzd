using System;
using System.IO;
using UnityEngine;

namespace CrimeEmpire.Save
{
    /// <summary>Local JSON save with a backup copy. Cloud Save can sit on top of this later.</summary>
    public sealed class SaveSystem
    {
        readonly string _path;
        readonly string _backupPath;
        readonly string _tempPath;

        public SaveSystem(string fileName)
        {
            _path = Path.Combine(Application.persistentDataPath, fileName);
            _backupPath = _path + ".bak";
            _tempPath = _path + ".tmp";
        }

        public SaveData Load() => TryRead(_path) ?? TryRead(_backupPath);

        public void Write(SaveData data)
        {
            try
            {
                File.WriteAllText(_tempPath, JsonUtility.ToJson(data));
                if (File.Exists(_path)) File.Copy(_path, _backupPath, true);
                File.Copy(_tempPath, _path, true);
                File.Delete(_tempPath);
            }
            catch (Exception e)
            {
                Debug.LogError($"[Save] Write failed: {e}");
            }
        }

        public void Delete()
        {
            foreach (var path in new[] { _path, _backupPath, _tempPath })
                if (File.Exists(path)) File.Delete(path);
        }

        static SaveData TryRead(string path)
        {
            try
            {
                if (!File.Exists(path)) return null;
                var data = JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
                return data != null && data.version > 0 ? data : null;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[Save] Could not read {path}: {e.Message}");
                return null;
            }
        }
    }
}
