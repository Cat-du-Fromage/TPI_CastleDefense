using System;
using System.Collections;
using System.Collections.Generic;
using KWUtils;
using Unity.Collections;
using UnityEngine;

namespace KaizerWald
{
    /*
    public sealed class PreselectionResponse : HighlightBehaviour
    {
        public IHighlightRegister Register { get; set; }
        protected override Dictionary<Regiment, List<Preselection>> Highlights { get; set; } = new(2);

        private void Start()
        {
            GetComponent<RegimentManager>().Regiments.ForEach(RegisterHighlights);
        }

        public override void OnEnableHighlight(Regiment regiment)
        {
            base.OnEnableHighlight(regiment);
            regiment.TogglePreSelected(true);
        }

        public override void OnDisableHighlight(Regiment regiment)
        {
            base.OnDisableHighlight(regiment);
            regiment.TogglePreSelected(false);
            Register.OnUnRegister(this, regiment);
        }

        public override void OnClearHighlight()
        {
            base.OnClearHighlight();
            Register.PreselectedRegiments.ForEach(regiment => regiment.TogglePreSelected(false));
            Register.OnClearRegister(this);
        }

        //Remove killed Unit
        public void OnUpdate(UnitComponent unit)
        {
            if (Highlights[unit.Regiment].Count <= 1)
            {
                OnRegimentDestroyed(unit);
                return;
            }

            OnUnitDestroyed(unit);
        }

        protected override void OnRegimentDestroyed(UnitComponent unit)
        {
            base.OnRegimentDestroyed(unit);
            Register.PreselectedRegiments.Remove(unit.Regiment);
        }
    }
    */
}