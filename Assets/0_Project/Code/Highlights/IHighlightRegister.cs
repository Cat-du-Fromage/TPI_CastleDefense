using System.Collections.Generic;
using UnityEngine;

namespace KaizerWald
{
    public interface IHighlightRegister
    {
        public GameObject Prefab { get; }
        public Dictionary<int, IHighlightable[]> Records { get; set; }

        public void PopulateRecords<T>(Transform[] units)
        where T: IHighlightable
        {
            foreach ((_, IHighlightable[] highlights) in Records)
            {
                for (int i = 0; i < highlights.Length; i++)
                {
                    Vector3 unitPosition = units[i].position;
                    T highlight = GameObject.Instantiate(Prefab, unitPosition + Vector3.up * 0.05f, Quaternion.identity).GetComponent<T>();
                    highlights[i] = highlight;
                }
            }
        }

        public void RegisterNewRegiment<T>(IRegiment regiment)
        where T: IHighlightable
        {
            Records.TryAdd(regiment.RegimentID, new IHighlightable[regiment.Units.Length]);
            PopulateRecords<T>(regiment.Units);
        }
        
        public void OnUnitUpdate(int regimentID, int unitIndexInRegiment)
        {
            //Placer l'algorithme ici
        }
        
    }
}