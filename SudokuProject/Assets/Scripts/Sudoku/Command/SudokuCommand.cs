using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SudokuCommand
{
    public List<TileBehaviour> tiles;

    public SudokuCommand(List<TileBehaviour> tiles)
    {
        this.tiles = tiles;
    }

    public abstract void Execute();

    public abstract void Undo();
}