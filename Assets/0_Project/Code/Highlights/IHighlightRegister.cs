using System.Collections.Generic;
using UnityEngine;

namespace KaizerWald
{
    public interface IHighlightRegister
    {
        public GameObject Prefab { get; }
        public Dictionary<int, IHighlightable[]> Records { get; set; }

        public void PopulateRecords<T>(int regimentID, Transform[] units, GameObject prefab = null)
        where T: IHighlightable
        {
            GameObject prefabUsed = prefab == null ? Prefab : prefab;

            for (int i = 0; i < Records[regimentID].Length; i++)
            {
                Vector3 unitPosition = units[i].position;
                
                T highlight = Object.Instantiate(prefabUsed, unitPosition + Vector3.up * 0.05f, Quaternion.identity).GetComponent<T>();

                Records[regimentID][i] = highlight;
            }
        }

        public void RegisterNewRegiment<T>(Regiment regiment, GameObject prefab = null)
        where T: IHighlightable
        {
            Records.TryAdd(regiment.RegimentID, new IHighlightable[regiment.Units.Length]);
            PopulateRecords<T>(regiment.RegimentID, regiment.Units, prefab);
        }

        public void OnUnitUpdate(int regimentID, int unitIndexInRegiment)
        {
            //Placer l'algorithme ici
        }
        
    }
}