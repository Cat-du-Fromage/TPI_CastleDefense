using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;

namespace KaizerWald
{
    public interface IHighlightCoordinator
    {
        public List<Regiment> Regiments { get; set; }
        public IHighlightRegister[] Registers { get; set; }
        
        //public PreselectionRegister PreselectionRegister { get; }
        public PreselectionSystem PreselectionSystem { get; }
        public SelectionSystem SelectionSystem { get; }
        public PlacementSystem PlacementSystem { get; }
        public PlayerControls Controls { get; }
        
        //Regiment States
        public List<Regiment> PreselectedRegiments =>  ((PreselectionRegister)PreselectionSystem.Register).PreselectedRegiments;
        public List<Regiment> SelectedRegiments => ((SelectionRegister)SelectionSystem.Register).SelectedRegiments;
        public List<Regiment> MovingRegiments => ((PlacementRegister)PlacementSystem.Register).MovingRegiments;
        
        //Transform Access Array
        public Dictionary<int, TransformAccessArray> PlacementAccessArrays => ((PlacementRegister)PlacementSystem.Register).TransformAccessArrays;
        
        public void OnRegimentKilled(Regiment regiment)
        {
            for (int i = 0; i < Registers.Length; i++)
            {
                Registers[i].Records.Remove(regiment.RegimentID);
            }
            Regiments.Remove(regiment);
        }
        
        public void OnUnitKilled(int regimentID, int unitIndexInRegiment)
        {
            for (int i = 0; i < Registers.Length; i++)
            {
                Registers[i].OnUnitUpdate(regimentID, unitIndexInRegiment);
            }
        }

        public void DispatchEvent(HighlightBehaviour sender)
        {
            if (sender is SelectionRegister)
            {
                ((PreselectionRegister)PreselectionSystem.Register).OnClearHighlight();
                PreselectionSystem.Controller.OnLeftMouseReleased();
            }
            else if (sender is PlacementRegister)
            {
                //((PlacementRegister)PlacementSystem.Register).LeadersDestination[]
                
            }
        }
    }
}