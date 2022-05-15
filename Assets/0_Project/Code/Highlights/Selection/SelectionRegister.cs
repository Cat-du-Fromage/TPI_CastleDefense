using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KWUtils;

namespace KaizerWald
{
    public sealed class SelectionRegister : HighlightBehaviour, IHighlightRegister
    {
        private readonly Vector3 yOffset = new Vector3(0, 0.05f, 0); 
        public GameObject Prefab { get; set; }
        public Dictionary<int, IHighlightable[]> Records { get; set; }
        
        public List<Regiment> SelectedRegiments { get; private set; } = new List<Regiment>(2);
        
        public SelectionRegister(GameObject prefab, List<Regiment> regiments)
        {
            Prefab = prefab;
            Records = new Dictionary<int, IHighlightable[]>(regiments.Count);
            
            HighlightRegister = this;
            IHighlightRegister registerInterface = this;
            
            foreach (Regiment regiment in regiments)
            {
                registerInterface.RegisterNewRegiment<Selection>(regiment, Prefab);
            }
        }
        
        public override void OnEnableHighlight(Regiment regiment)
        {
            if (SelectedRegiments.Contains(regiment)) return;
            base.OnEnableHighlight(regiment);
            regiment.IsSelected = true;
            SelectedRegiments.Add(regiment);
        }

        public override void OnDisableHighlight(Regiment regiment)
        {
            base.OnDisableHighlight(regiment);
            regiment.IsSelected = false;
            SelectedRegiments.Remove(regiment);
        }

        public override void OnClearHighlight()
        {
            base.OnClearHighlight();
            for (int i = 0; i < SelectedRegiments.Count; i++)
            {
                SelectedRegiments[i].IsSelected = false;
            }
            SelectedRegiments.Clear();
        }
    }
}
