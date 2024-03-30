using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Structure Data", menuName = "Scriptable Object/Sctructure Data", order = 0)]
public class StructureData : ScriptableObject
{
    public enum StructureType
    {
        House,
        Store,
        LandScape
    }

    [SerializeField]
    private GameObject prefab;

    [SerializeField]
    private StructureType type;
    [SerializeField]
    private int length;

    public GameObject GetPrefab()
    {
        return prefab;
    }

    public int GetLength()
    {
        return length;
    }
}
