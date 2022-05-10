using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KWUtils;

namespace KaizerWald
{
    public class PreselectionRegister : IHighlightRegister<Preselection>
    {
        public IHighlightRegister<IHighlightable> GetRegister => (this as IHighlightRegister<IHighlightable>);
        public GameObject Prefab { get; set; }
        public Dictionary<int, Preselection[]> Records { get; set; }

        public PreselectionRegister(GameObject prefab, List<IRegiment> regiments)
        {
            Prefab = prefab;
            Records = new Dictionary<int, Preselection[]>(regiments.Count);
            Debug.Log(Records.Count);
            foreach (IRegiment regiment in regiments)
            {
                (this as IHighlightRegister<Preselection>).RegisterNewRegiment(regiment);
            }
        }
    }
}
