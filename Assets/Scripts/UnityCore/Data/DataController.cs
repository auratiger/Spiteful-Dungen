using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace UnityCore.Data
{
    public class DataController : MonoBehaviour
    {
        private string SavePath => $"{Application.persistentDataPath}/save.txt";

#region Public Functions

        [ContextMenu("Save")]
        public void Save()
        {
            var state = LoadFile();
            CaptureState(state);
            SaveFile(state);
        }

        [ContextMenu("Load")]
        public void Load()
        {
            var state = LoadFile();
            RestoreState(state);
        }
        
#endregion

#region Private Funtions

        private void SaveFile(object state)
        {
            using (var stream = File.Open(SavePath, FileMode.Create))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, state);
            }
        }

        private Dictionary<string, object> LoadFile()
        {
            if (!File.Exists(SavePath))
            {
                return new Dictionary<string, object>();
            }

            using (FileStream stream = File.Open(SavePath, FileMode.Open))
            {
                var formatter = new BinaryFormatter();
                return (Dictionary<string, object>) formatter.Deserialize(stream);
            }
        }

        private void CaptureState(Dictionary<string, object> state)
        {
            foreach (var savable in FindObjectsOfType<SavableEntity>())
            {
                state[savable.Id] = savable.CaptureState();
            }
        }

        private void RestoreState(Dictionary<string, object> state)
        {
            foreach (var savable in FindObjectsOfType<SavableEntity>())
            {
                if (state.TryGetValue(savable.Id, out object value))
                {
                    savable.RestoreState(value);
                }
            }
        }
        
#endregion

    }
}
