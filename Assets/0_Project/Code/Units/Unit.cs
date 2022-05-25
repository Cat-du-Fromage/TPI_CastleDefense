using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KaizerWald
{
    public class Unit : MonoBehaviour, IUnit
    {
        public Regiment RegimentAttach { get; set; }
        public int IndexInRegiment { get; set; }
        
        public UnitAnimation Animation { get; private set; }

        public bool IsDead { get; private set; }
        
        //======================================================
        //TEMPORARY: SHOOTMAnAGER
        
        public Unit Target { get; set; }
        
        //======================================================

        private void Awake()
        {
            Animation = GetComponent<UnitAnimation>();
        }

        public void OnDeath()
        {
            if (IsDead) return;
            Animation.SetDead();
            IsDead = true;
        }
    }
}