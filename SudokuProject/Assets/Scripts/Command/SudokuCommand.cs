using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SudokuCommand : MonoBehaviour
{
    public abstract void Execute();

    public abstract void Undo();
}

public abstract class EffectedTilesCommand : MonoBehaviour
{
    public List<int> effectedIndexes;

    public abstract void Execute();

    public abstract void Undo();
}

public class ImportCommand : SudokuCommand
{
    public List<int> previousGridDigits;
    public List<int> importedGridDigits;

    public override void Execute()
    {
        //EventManager.Import(importedGridDigits);
    }

    public override void Undo()
    {
        //EventManager.Import(previousGridDigits);
    }
}

public class AddDigitCommand : EffectedTilesCommand
{
    public List<int> previousGridDigits;
    public int addedDigit;

    public override void Execute()
    {
        ApplyAllDigits(true);
    }

    public override void Undo()
    {
        ApplyAllDigits(false);
    }

    private void ApplyAllDigits(bool execute)
    {
        for (int i = 0; i < effectedIndexes.Count; i++)
        {
            int index = effectedIndexes[i];

            int digit = execute ? addedDigit : previousGridDigits[i];
            //EventManager.ApplyDigit(index, digit);
        }
    }
}


