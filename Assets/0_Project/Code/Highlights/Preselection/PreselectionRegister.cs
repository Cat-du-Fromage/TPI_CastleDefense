using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KWUtils;

namespace KaizerWald
{
    public sealed class PreselectionRegister : HighlightBehaviour, IHighlightRegister
    {
        private readonly Vector3 yOffset = new Vector3(0, 0.05f, 0); 
        public GameObject Prefab { get; set; }
        public Dictionary<int, IHighlightable[]> Records { get; set; }

        public List<Regiment> PreselectedRegiments { get; private set; } = new List<Regiment>(2);

        public PreselectionRegister(GameObject prefab, List<Regiment> regiments)
        {
            Prefab = prefab;
            Records = new Dictionary<int, IHighlightable[]>(regiments.Count);
            HighlightRegister = this;
            IHighlightRegister registerInterface = this;
            foreach (Regiment regiment in regiments)
            {
                registerInterface.RegisterNewRegiment<Preselection>(regiment);
            }
        }
        
        public override void OnEnableHighlight(Regiment regiment)
        {
            if (PreselectedRegiments.Contains(regiment)) return;
            base.OnEnableHighlight(regiment);
            regiment.IsPreselected = true;
            PreselectedRegiments.Add(regiment);
        }

        public override void OnDisableHighlight(Regiment regiment)
        {
            base.OnDisableHighlight(regiment);
            regiment.IsPreselected = false;
            PreselectedRegiments.Remove(regiment);
        }

        public override void OnClearHighlight()
        {
            base.OnClearHighlight();
            for (int i = 0; i < PreselectedRegiments.Count; i++)
            {
                PreselectedRegiments[i].IsPreselected = false;
            }
            PreselectedRegiments.Clear();
        }
    }
}
