using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KaizerWald
{
    public interface IUnit
    {
        IRegiment RegimentAttach { get; set; }
        public int IndexInRegiment { get; set; }

        public void OnKilled() => RegimentAttach.OnUnitKilled(IndexInRegiment);
    }
    
    public interface IGeneral
    {
        public List<IRegiment> Regiments { get; set; }
        IHighlightRegister<IHighlightable>[] Registers { get; set; }

        public void OnRegimentUpdate(IRegiment regiment)
        {
            for (int i = 0; i < Registers.Length; i++)
            {
                Registers[i].Records.Remove(regiment.RegimentID);
            }
            Regiments.Remove(regiment);
        }
        
        public void OnUnitUpdate(int regimentID, int unitIndexInRegiment)
        {
            for (int i = 0; i < Registers.Length; i++)
            {
                Registers[i].OnUnitUpdate(regimentID, unitIndexInRegiment);
            }
        }
    }
    
    //Sert a enregistrer les composantes des unités des régiments
    //T peut être "IHighlightable"
    //Donc si Highlight => Dictionary<int, IHighlightable[]>


    public class RegimentManager : MonoBehaviour, IGeneral
    {
        [SerializeField] private GameObject prefabPreselection;
        [SerializeField] private GameObject prefabSelection;
        public List<IRegiment> Regiments { get; set; }
        public List<Regiment> RegimentsNoInterface { get; set; }
        public IHighlightRegister<IHighlightable>[] Registers { get; set; }

        private PreselectionRegister preselectionRegister;
        private SelectionRegister selectionRegister;

        private void Awake()
        {
            RegimentsNoInterface = GetComponent<RegimentFactory>().CreateRegiments();
            Regiments = new List<IRegiment>(RegimentsNoInterface.Count);
            
            //RegimentsNoInterface.ForEach(r => Regiments.Add(r.GetComponent<IRegiment>()));

            foreach (var r in RegimentsNoInterface)
            {
                if (r is IRegiment iRegiment)
                {
                    Regiments.Add(iRegiment);
                }
            }
            //Debug.Log(Regiments.Count);
        }

        private void Start()
        {
            preselectionRegister = new PreselectionRegister(prefabPreselection, Regiments);
            selectionRegister = new SelectionRegister(prefabSelection, Regiments);
            
            Debug.Log(preselectionRegister.Records.Count);
            Debug.Log(selectionRegister.Records.Count);

            Registers = new IHighlightRegister<IHighlightable>[2];
            if (preselectionRegister is IHighlightRegister<IHighlightable> register1) Registers[0] = register1;
            if (selectionRegister is IHighlightRegister<IHighlightable> register2) Registers[1] = register2;
            //Registers = new[] {preselectionRegister.GetRegister, selectionRegister as IHighlightRegister<IHighlightable>};

            for (int i = 0; i < Registers.Length; i++)
            {
                Debug.Log(Registers[i].Records.Count);
            }
        }
    }
}
