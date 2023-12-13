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
    
    public class ImportCommand : EffectedTilesCommand
    {
        public List<int> previousGridDigits;
        public List<int> importedGridDigits;
    
        public override void Execute()
        {
            CommandManager.instance.RemoveDigits(effectedIndexes);
            CommandManager.instance.AddDigits(effectedIndexes, importedGridDigits);
        }
    
        public override void Undo()
        {
            CommandManager.instance.RemoveDigits(effectedIndexes);
            CommandManager.instance.AddDigits(effectedIndexes, previousGridDigits);
        }
    }
}



