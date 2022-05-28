using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace KaizerWald
{
    public class RegimentManager : MonoBehaviour, IHighlightCoordinator
    {
        //==============================================================================================================
        // NEW : PATHFINDING
        public HPAPathfinder hpaPathfinder { get; private set; }

        public HPAUpdateManager HPAUpdateManager;
        //==============================================================================================================
        
        [field:SerializeField] private GameObject prefabPreselection;
        [field:SerializeField] public GameObject PrefabSelection { get; private set; }
        public PlayerControls Controls { get; private set; }
        
        public List<Regiment> Regiments { get; set; }
        public IHighlightRegister[] Registers { get; set; }

        public PreselectionSystem PreselectionSystem { get; private set; }
        public SelectionSystem SelectionSystem { get; private set; }
        public PlacementSystem PlacementSystem { get; private set; }

        private SquarePreselection preselectionSquare;

        private void Awake()
        {
            //==============================================================================================================
            // NEW : PATHFINDING
            hpaPathfinder = FindObjectOfType<HPAPathfinder>();
            //==============================================================================================================
            
            Controls = new PlayerControls();
            
            Regiments = GetComponent<RegimentFactory>().CreateRegiments();
            
            PreselectionSystem = new PreselectionSystem(this, prefabPreselection);
            SelectionSystem = new SelectionSystem(this, PrefabSelection);
            PlacementSystem = new PlacementSystem(this, null); //null...: not normal, need refactor
        }

        private void Start()
        {
            if (!TryGetComponent(out preselectionSquare)) preselectionSquare = gameObject.AddComponent<SquarePreselection>();
            Registers = new IHighlightRegister[] {PreselectionSystem.Register, SelectionSystem.Register, PlacementSystem.Register};
        }

        private void OnDestroy()
        {
            for (int i = 0; i < Registers.Length; i++)
            {
                ((HighlightBehaviour)Registers[i]).DisposeAllTransformAccessArrays();
            }
        }
    }
}
