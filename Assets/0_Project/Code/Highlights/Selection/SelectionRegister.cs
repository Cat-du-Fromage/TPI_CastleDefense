using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KWUtils;

namespace KaizerWald
{
    public class SelectionRegister : IHighlightRegister<Selection>
    {
        public GameObject Prefab { get; set; }
        public Dictionary<int, Selection[]> Records { get; set; }

        public SelectionRegister(GameObject prefab, List<IRegiment> regiments)
        {
            Prefab = prefab;
            Records = new Dictionary<int, Selection[]>(regiments.Count);
            foreach (IRegiment regiment in regiments)
            {
                (this as IHighlightRegister<Selection>).RegisterNewRegiment(regiment);
            }
        }
    }
}
