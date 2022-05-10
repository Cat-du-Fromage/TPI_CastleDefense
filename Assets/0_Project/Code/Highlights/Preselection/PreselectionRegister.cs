using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KWUtils;

namespace KaizerWald
{
    public class PreselectionRegister : MonoBehaviour, IHighlightRegister
    {
        public GameObject Prefab { get; set; }
        public Dictionary<int, IHighlightable[]> Records { get; set; }

        public void InitializeRegister(GameObject prefab, List<IRegiment> regiments)
        {
            Prefab = prefab;
            Records = new Dictionary<int, IHighlightable[]>(regiments.Count);
            foreach (IRegiment regiment in regiments)
            {
                this.AsInterface<IHighlightRegister>().RegisterNewRegiment<Preselection>(regiment);
            }
        }
    }
}
