using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command
{
    public abstract class SudokuCommand
    {
        public abstract void Execute();
    
        public abstract void Undo();
    }
    
    public abstract class EffectedTilesCommand : SudokuCommand
    {
        public List<int> effectedIndexes;
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
}



