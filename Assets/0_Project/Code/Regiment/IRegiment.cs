using UnityEngine;
using UnityEngine.Jobs;

namespace KaizerWald
{
    public interface ISelectable
    {
        public bool IsPreselected { get; set; }
        public bool IsSelected { get; set;}
    }

    public abstract class RegimentBehaviour : MonoBehaviour, ISelectable
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

        //==================================================
        //IPreselectable Interface
        //==================================================
        public bool IsPreselected { get; set; }
        public bool IsSelected { get; set; }
        
    }
}