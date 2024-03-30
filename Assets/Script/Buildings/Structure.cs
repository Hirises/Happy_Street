using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure : MonoBehaviour
{
    [SerializeField]
    private StructureData structureData;
    [SerializeField]
    private SpriteRenderer[] renderers;
    private Position position;
    private bool isBlueprint;

    public void Intialize(Position pos, bool isBlueprint)
    {
        if(position == null)
        {
            position = pos;
            this.isBlueprint = isBlueprint;
        }
    }

    public Position GetPosition()
    {
        return position;
    }

    public void Remove(bool force)
    {
        if (force)
        {
            //애니메이션 재생 X
            if (isBlueprint)
            {
                position.GetSpace().RemoveStructureBluePrint();
            }
            else
            {
                position.GetSpace().RemoveStructure();
            }
        }
    }

    public StructureData GetStructureData()
    {
        return structureData;
    }

    public void SetMaterial(Material mat)
    {
        foreach(SpriteRenderer renderer in renderers)
        {
            renderer.material = mat;
        }
    }
}
