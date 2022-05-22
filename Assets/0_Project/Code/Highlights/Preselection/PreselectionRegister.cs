using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KWUtils;

namespace KaizerWald
{
    public sealed class PreselectionRegister : HighlightBehaviour//, IHighlightRegister
    {
        private readonly IHighlightSystem highlightSystem;
        
        private readonly Vector3 yOffset = new Vector3(0, 0.05f, 0);

        public List<Regiment> PreselectedRegiments { get; private set; } = new List<Regiment>(2);

        public PreselectionRegister(IHighlightSystem system, GameObject prefab, List<Regiment> regiments)
        {
            highlightSystem = system;
            Records = new Dictionary<int, IHighlightable[]>(regiments.Count);
            
            IHighlightRegister registerInterface = this;
            HighlightRegister = registerInterface;
            
            Prefab = prefab;

            foreach (Regiment regiment in regiments)
            {
                registerInterface.RegisterNewRegiment<Preselection>(regiment);
            }
            SetUpTransformAccess();
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
