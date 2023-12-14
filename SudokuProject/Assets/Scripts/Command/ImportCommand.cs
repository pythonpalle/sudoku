using System.Collections.Generic;

namespace Command
{
    [System.Serializable]
    public class ImportCommand : EffectedTilesCommand
    {
        public List<int> previousGridDigits;
        public List<int> importedGridDigits;
    
        public override void Execute()
        {
            //CommandManager.instance.RemoveDigits(effectedIndexes);
            CommandManager.instance.AddDigits(effectedIndexes, importedGridDigits);
        }
    
        public override void Undo()
        {
            //CommandManager.instance.RemoveDigits(effectedIndexes);
            CommandManager.instance.AddDigits(effectedIndexes, previousGridDigits);
        }
    }
}