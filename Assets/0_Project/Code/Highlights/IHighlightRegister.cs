using System.Collections.Generic;
using UnityEngine;

namespace KaizerWald
{
    public interface IHighlightRegister<T>
    where T : IHighlightable
    {
        public GameObject Prefab { get; }
        public Dictionary<int, T[]> Records { get; set; }

        public void PopulateRecords(Transform[] units)
        {
            foreach ((_, T[] highlights) in Records)
            {
                for (int i = 0; i < highlights.Length; i++)
                {
                    Vector3 unitPosition = units[i].position;
                    T highlight = GameObject.Instantiate(Prefab, unitPosition + Vector3.up * 0.05f, Quaternion.identity).GetComponent<T>();
                    highlights[i] = highlight;
                }
            }
        }

        public void RegisterNewRegiment(IRegiment regiment)
        {
            Records.TryAdd(regiment.RegimentID, new T[regiment.Units.Length]);
            PopulateRecords(regiment.Units);
        }
        
        public void OnUnitUpdate(int regimentID, int unitIndexInRegiment)
        {
            //Placer l'algorithme ici
        }
        
    }
}