using System;
using System.Collections;
using System.Collections.Generic;
using KWUtils;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

namespace KaizerWald
{
    public class Regiment : MonoBehaviour, ISelectable
    {
        //Interface
        public bool IsPreselected { get; set; }
        public bool IsSelected { get; set; }
        
        //Properties
        public int RegimentID { get; set; }
        public IHighlightCoordinator HighlightCoordinator { get; set; }

        public Transform[] Units { get; set; }
        public TransformAccessArray UnitsTransformAccessArray { get; private set; }
        
        public int CurrentLineFormation { get; private set; }
        public RegimentClass RegimentClass { get; private set; }
        
        //Shoot behaviour
        public bool IsPlayer { get; set; }
        public float regimentShootRange = 10f;
        public Regiment currentTarget;
        
        //==============================================================================================================
        // Private Setters
        //==============================================================================================================
        
        public void SetRegimentClass(RegimentClass regimentClass)
        {
            RegimentClass = regimentClass;
            CurrentLineFormation = RegimentClass.MinRow;
        }
        public void SetCurrentLineFormation(int unitPerLine) => CurrentLineFormation = unitPerLine;

        //==============================================================================================================
        // Unity Events
        //==============================================================================================================
        
        private void Start()
        {
            UnitsTransformAccessArray = new TransformAccessArray(Units, Units.Length);
        }

        private void Update()
        {
            //throw new NotImplementedException();
        }

        private void FixedUpdate()
        {
            //if (!ShootingBehaviour.IsTargetInRange(this, out RaycastHit hit)) return;
            //Debug.Log("found");
            //Debug.Log(hit.transform.name);
            //currentTarget = hit.transform.GetComponent<Unit>().RegimentAttach;
        }

        private void OnDestroy()
        {
            if(UnitsTransformAccessArray.isCreated) UnitsTransformAccessArray.Dispose();
        }

        //==============================================================================================================

        
        
        //UNIT REARRANGE HERE!
        public void OnUnitKilled(int unitIndexInRegiment)
        {
            //Units.Rearrange(unitIndexInRegiment);
            HighlightCoordinator.OnUnitKilled(RegimentID, unitIndexInRegiment);
            //Faire le ménage ICI
            
            //Placer l'algorithme ici
            
            //Envoyer le message au IScrivener
        }
    }
}
