using Saving;
using UnityEngine;

namespace Saving
{
    [CreateAssetMenu(menuName = "Sudoku/Ports/Save Request")]
    public class SaveRequestPort : ScriptableObject
    {
        public SaveRequestLocation Location;
    }
}
