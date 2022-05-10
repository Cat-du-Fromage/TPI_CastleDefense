using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KaizerWald
{
    [CreateAssetMenu(fileName = "NewRegimentClass", menuName = "RegimentClass", order = 0)]
    public class RegimentClass : ScriptableObject
    {
        [Header("Prefabs")] public GameObject PrefabUnit;
        public GameObject PrefabPlacement;

        [Header("RegimentStats")] public int BaseHealth = 1;
        public int BaseMorale = 1;
        public int Range = 1;
        public int MeleeAttack = 1;
        public float Speed = 1f;
        public Vector3 UnitSize;

        [Header("RegimentStats")] public int BaseNumUnits = 20;
        public int MinRow = 4;
        public int MaxRow = 10;
        public float SpaceBetweenUnits = 0.5f;

        public int MinColumn => BaseNumUnits / MinRow;
        public int MaxColumn => BaseNumUnits / MaxRow;

        private void OnEnable()
        {
            UnitSize = PrefabPlacement.GetComponent<MeshFilter>().sharedMesh.bounds.size;
        }
    }
}