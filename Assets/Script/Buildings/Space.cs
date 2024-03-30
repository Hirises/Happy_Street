using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Space : MonoBehaviour
{
    private Position position;
    private Structure structure;
    private bool isSubStructure;
    private Structure structureBlueprint;
    private bool isSubStructureBlueprint;

    public void SetPosition(Position pos)
    {
        if(position == null)
        {
            this.position = pos;
        }
    }

    public Position GetPosition()
    {
        return position;
    }

    public void RemoveStructureBluePrint()
    {
        if(structureBlueprint != null)
        {
            if (isSubStructureBlueprint)
            {
                structureBlueprint.GetPosition().GetSpace().RemoveStructureBluePrint();
            }
            else
            {
                for(int index = 1; index < structureBlueprint.GetStructureData().GetLength(); index++)
                {
                    position.Add(index).GetSpace().RemoveSubStructureBluePrint();
                }
                Destroy(structureBlueprint.gameObject);
                structureBlueprint = null;
            }
        }
    }

    public void RemoveSubStructureBluePrint()
    {
        if (structureBlueprint != null && isSubStructureBlueprint)
        {
            structureBlueprint = null;
            isSubStructureBlueprint = false;
        }
    }

    public void SetStructureBluePrint(StructureData data)
    {
        if (!position.Add(data.GetLength() - 1).IsInStreetBoundary())
        {
            return;
        }

        RemoveStructureBluePrint();
        GameObject obj = Instantiate(data.GetPrefab(), transform);
        structureBlueprint = obj.GetComponent<Structure>();
        structureBlueprint.Intialize(position, true);
        bool occupied = HasStructure();
        for (int index = 1; index < structureBlueprint.GetStructureData().GetLength(); index++)
        {
            Space space = position.Add(index).GetSpace();
            space.SetSubStructureBluePrint(structureBlueprint);
            occupied |= space.HasStructure();
        }
        structureBlueprint.SetMaterial(GameManager.GetInstance().GetBlueprintMaterial(occupied));
    }

    public void SetSubStructureBluePrint(Structure data)
    {
        RemoveStructureBluePrint();
        structureBlueprint = data;
    }

    public bool HasStructureBluePrint()
    {
        return structureBlueprint != null;
    }

    public Structure GetStructureBluePrint()
    {
        return structureBlueprint;
    }

    public bool applyStructureBluePrint()
    {
        if (HasStructure() || !HasStructureBluePrint())
        {
            RemoveStructureBluePrint();
            return false;
        }

        SetStructure(GetStructureBluePrint().GetStructureData());
        return true;
    }

    public void RemoveStructure()
    {
        if(structure != null)
        {
            if (isSubStructure)
            {
                structureBlueprint.GetPosition().GetSpace().RemoveStructure();
            }
            else
            {
                for (int index = 1; index < structure.GetStructureData().GetLength(); index++)
                {
                    position.Add(index).GetSpace().RemoveSubStructure();
                }
                Destroy(structure.gameObject);
                structure = null;
            }
        }
    }

    public void RemoveSubStructure()
    {
        if (structure != null && isSubStructure)
        {
            structure = null;
            isSubStructure = false;
        }
    }

    public void SetStructure(StructureData data)
    {
        if (!position.Add(data.GetLength() - 1).IsInStreetBoundary())
        {
            return;
        }

        RemoveStructure();
        GameObject obj = Instantiate(data.GetPrefab(), transform);
        structure = obj.GetComponent<Structure>();
        structure.Intialize(position, true);
        for (int index = 1; index < structureBlueprint.GetStructureData().GetLength(); index++)
        {
            position.Add(index).GetSpace().SetSubStructure(structure);
        }
    }

    public void SetSubStructure(Structure data)
    {
        RemoveStructure();
        structure = data;
    }

    public bool HasStructure()
    {
        return structure != null;
    }

    public Structure GetStructure()
    {
        return structure;
    }
}
