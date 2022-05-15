using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KaizerWald
{
    public class RegimentManager : MonoBehaviour, IHighlightCoordinator
    {
        [field:SerializeField] private GameObject prefabPreselection;
        [field:SerializeField] public GameObject PrefabSelection { get; private set; }
        public List<Regiment> Regiments { get; set; }
        public IHighlightRegister[] Registers { get; set; }
        public PreselectionRegister PreselectionRegister { get; private set; }
        public SelectionSystem SelectionSystem { get; private set; }
        
        public PlacementSystem PlacementSystem { get; private set; }
        public PlayerControls Controls { get; private set; }

        private void Awake()
        {
            Controls = new PlayerControls();
            
            Regiments = GetComponent<RegimentFactory>().CreateRegiments();
            
            PreselectionRegister = new PreselectionRegister(prefabPreselection, Regiments);
            SelectionSystem = new SelectionSystem(this, PrefabSelection);
            PlacementSystem = new PlacementSystem(this, null); //null...: not normal, need refactor
        }

        private void Start()
        {
            Registers = new IHighlightRegister[] {PreselectionRegister, SelectionSystem.Register, PlacementSystem.Register};
        }
    }
}
