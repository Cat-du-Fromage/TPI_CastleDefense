using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KWUtils;

namespace KaizerWald
{
    public sealed class SelectionRegister : HighlightBehaviour
    {
        private readonly Vector3 yOffset = new Vector3(0, 0.05f, 0);
        public List<Regiment> SelectedRegiments { get; private set; } = new List<Regiment>(2);
        
        public SelectionRegister(GameObject prefab, List<Regiment> regiments)
        {
            Prefab = prefab;
            Records = new Dictionary<int, IHighlightable[]>(regiments.Count);
            
            IHighlightRegister registerInterface = this;
            HighlightRegister = registerInterface;

            foreach (Regiment regiment in regiments)
            {
                registerInterface.RegisterNewRegiment<Selection>(regiment, Prefab);
            }
            SetUpTransformAccess();
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
