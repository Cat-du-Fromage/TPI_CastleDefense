using System.Collections.Generic;
using UnityEngine;

namespace KaizerWald
{
    public interface IHighlightRegister
    {
        public GameObject Prefab { get; }
        public Dictionary<int, IHighlightable[]> Records { get; set; }

        public void PopulateRecords<T>(int regimentID, Transform[] units)
        where T: IHighlightable
        {
            for (int i = 0; i < Records[regimentID].Length; i++)
            {
                Vector3 unitPosition = units[i].position;
                T highlight = Object.Instantiate(Prefab, unitPosition + Vector3.up * 0.05f, Quaternion.identity).GetComponent<T>();
                Records[regimentID][i] = highlight;
            }
        }

        public void RegisterNewRegiment<T>(Regiment regiment)
        where T: IHighlightable
        {
            Records.TryAdd(regiment.RegimentID, new IHighlightable[regiment.Units.Length]);
            PopulateRecords<T>(regiment.RegimentID, regiment.Units);
        }
        
        public void OnUnitUpdate(int regimentID, int unitIndexInRegiment)
        {
            //Placer l'algorithme ici
        }
        
    }
}