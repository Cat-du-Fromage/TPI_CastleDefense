using UnityEngine;

namespace KaizerWald
{
    public interface IRegiment
    {
        public Transform[] Units { get; set; }
        public IGeneral General { get; set; }
        public int RegimentID { get; set; }

        //UNIT REARRANGE HERE!
        public void OnUnitKilled(int unitIndexInRegiment)
        {
            //Units.Rearrange(unitIndexInRegiment);
            General.OnUnitUpdate(RegimentID, unitIndexInRegiment);
            //Faire le ménage ICI
            
            //Placer l'algorithme ici
            
            //Envoyer le message au IScrivener
        }
    }
}