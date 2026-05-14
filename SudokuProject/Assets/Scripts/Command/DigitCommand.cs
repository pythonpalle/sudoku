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
            _commandManager.AddDigit(effectedIndexes, addedDigit);
        }
    
        public override void Undo()
        {
            _commandManager.RemoveDigits(effectedIndexes);
            _commandManager.AddDigits(effectedIndexes, previousGridDigits);
        }
    }
    
    [System.Serializable]
    public class RemoveDigitCommand : EffectedTilesCommand
    {
        public List<int> previousGridDigits;
        
        public override void Execute()
        {
            _commandManager.RemoveDigits(effectedIndexes);
        }
    
        public override void Undo()
        {
            _commandManager.AddDigits(effectedIndexes, previousGridDigits);
        }
    }
}