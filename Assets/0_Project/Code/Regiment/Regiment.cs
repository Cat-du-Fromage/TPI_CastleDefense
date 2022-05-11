using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;

namespace KaizerWald
{
    public class Regiment : MonoBehaviour, ISelectable
    {
        public int RegimentID { get; set; }
        public ICoordinator Coordinator { get; set; }
        public Transform[] Units { get; set; }

        public TransformAccessArray UnitsTransformAccessArray { get; protected set; }
        
        public RegimentClass RegimentClass { get; private set; }
        public void SetRegimentClass(RegimentClass regimentClass) => RegimentClass = regimentClass;
        
        

        //UNIT REARRANGE HERE!
        public void OnUnitKilled(int unitIndexInRegiment)
        {
            //Units.Rearrange(unitIndexInRegiment);
            Coordinator.OnUnitKilled(RegimentID, unitIndexInRegiment);
            //Faire le ménage ICI
            
            //Placer l'algorithme ici
            
            //Envoyer le message au IScrivener
        }
        private void Start()
        {
            UnitsTransformAccessArray = new TransformAccessArray(Units, Units.Length);
        }

        private void OnDestroy()
        {
            if(UnitsTransformAccessArray.isCreated) UnitsTransformAccessArray.Dispose();
        }

        public bool IsPreselected { get; set; }
        public bool IsSelected { get; set; }
    }
}
