using System.Collections.Generic;

namespace Command
{
    [System.Serializable]
    public abstract class DigitCommand : EffectedTilesCommand
    {
        public List<int> previousGridDigits;
    }
    
    [System.Serializable]
    public class AddDigitCommand : DigitCommand
    {
        public int addedDigit;
    
        public override void Execute()
        {
            CommandManager.instance.AddDigit(effectedIndexes, addedDigit);
        }
    
        public override void Undo()
        {
            CommandManager.instance.RemoveDigits(effectedIndexes);
            CommandManager.instance.AddDigits(effectedIndexes, previousGridDigits);
        }
    }
    
    [System.Serializable]
    public class RemoveDigitCommand : EffectedTilesCommand
    {
        public List<int> previousGridDigits;
    
        public override void Execute()
        {
            CommandManager.instance.RemoveDigits(effectedIndexes);
        }
    
        public override void Undo()
        {
            CommandManager.instance.AddDigits(effectedIndexes, previousGridDigits);
        }
    }
}