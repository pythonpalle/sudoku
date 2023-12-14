using System.Collections.Generic;

namespace Command
{
    [System.Serializable]
    public abstract class MarkCommand : EffectedTilesCommand
    {
        public int enterType;
    }
    
    [System.Serializable]
    public class AddMarkCommand : MarkCommand
    {
        public int number;
        
        public override void Execute()
        {
            CommandManager.instance.AddMark(effectedIndexes, number, enterType);
        }

        public override void Undo()
        {
            CommandManager.instance.RemoveSingleMark(effectedIndexes, number, enterType);
        }
    }
    
    [System.Serializable]
    public class RemoveSingleMarkCommand : MarkCommand
    {
        public int number;
        
        public override void Execute()
        {
            CommandManager.instance.RemoveSingleMark(effectedIndexes, number, enterType);
        }

        public override void Undo()
        {
            CommandManager.instance.AddMark(effectedIndexes, number, enterType);
        }
    }
    
    [System.Serializable]
    public class RemoveAllMarksCommand : MarkCommand
    {
        public List<List<int>> previousMarks;

        public override void Execute()
        {
            CommandManager.instance.RemoveAllMarks(effectedIndexes, enterType);
        }

        public override void Undo()
        {
            CommandManager.instance.AddMarks(effectedIndexes, previousMarks, enterType);
        }
    }
}