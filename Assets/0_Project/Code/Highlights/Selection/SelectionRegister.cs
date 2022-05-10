using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KWUtils;

namespace KaizerWald
{
    public class SelectionRegister : IHighlightRegister
    {
        public GameObject Prefab { get; set; }
        public Dictionary<int, IHighlightable[]> Records { get; set; }
        
        public SelectionRegister(GameObject prefab, List<Regiment> regiments)
        {
            Prefab = prefab;
            Records = new Dictionary<int, IHighlightable[]>(regiments.Count);
            IHighlightRegister registerInterface = this;
            foreach (Regiment regiment in regiments)
            {
                registerInterface.RegisterNewRegiment<Selection>(regiment);
            }
        }
    }
}
