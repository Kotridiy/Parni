using Assets.Scripts.Core;
using System.Text;

namespace Assets.Scripts.Entity
{
    public class Castle : GamePlace
    {
        public override string Name => "Замок";

        public override string Description => "Главное убежище людей, здесь они размножаются, развиваются и торгуют";

        public override float GetImportance()
        {
            return 100;
        }
    }
}