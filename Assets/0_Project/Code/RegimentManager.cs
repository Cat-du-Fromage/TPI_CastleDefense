using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KaizerWald
{
    public class RegimentManager : MonoBehaviour, ICoordinator
    {
        [SerializeField] private GameObject prefabPreselection;
        [SerializeField] private GameObject prefabSelection;
        public List<Regiment> Regiments { get; set; }
        public IHighlightRegister[] Registers { get; set; }
        public PreselectionRegister PreselectionRegister { get; private set; }
        public SelectionRegister SelectionRegister { get; private set; }

        private void Awake()
        {
            Regiments = GetComponent<RegimentFactory>().CreateRegiments();
            
            PreselectionRegister = new PreselectionRegister(prefabPreselection, Regiments);
            SelectionRegister = new SelectionRegister(prefabSelection, Regiments);
        }

        private void Start()
        {
            Registers = new IHighlightRegister[] {PreselectionRegister, SelectionRegister};
        }
    }
}
