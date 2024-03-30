using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position
{
    private int signedPosX;

    private Position(int signedPosX)
    {
        this.signedPosX = signedPosX;
    }

    public static Position FromSignedPosX(int signedPosX)
    {
        return new Position(signedPosX);
    }

    public static Position FromWorldPosX(float worldPosX)
    {
        return new Position(GameManager.SnapToBuildingPosX(worldPosX));
    }

    public static Position FromIndexPosX(int index)
    {
        return new Position(index - GameManager.GetInstance().GetRightStreetLength());
    }

    public int ToSignedPosX()
    {
        return signedPosX;
    }

    public float ToWorldPosX()
    {
        return signedPosX * GameManager.GetInstance().GetBuildingWidthUnit();
    }

    public int ToIndexPosX()
    {
        return signedPosX + GameManager.GetInstance().GetRightStreetLength();
    }

    public bool IsInStreetBoundary()
    {
        if (signedPosX < 0)
        {
            return -signedPosX <= GameManager.GetInstance().GetRightStreetLength();
        }
        else if (signedPosX == 0)
        {
            return true;
        }
        else
        {
            return signedPosX <= GameManager.GetInstance().GetLeftStreetLength();
        }
    }

    public Position Add(int value)
    {
        if(value == 0)
        {
            return this;
        }
        return new Position(signedPosX + value);
    }

    public Position Add(Position pos)
    {
        return new Position(signedPosX + pos.signedPosX);
    }

    public Space GetSpace()
    {
        return GameManager.GetInstance().GetSpace(this);
    }

    public override bool Equals(object obj)
    {
        return obj is Position position &&
               signedPosX == position.signedPosX;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(signedPosX);
    }
}
