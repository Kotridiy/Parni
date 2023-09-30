using System.Collections.Generic;

namespace Assets.Scripts.Core
{
    public struct ScanInfo
    {
        public GraphPointInfo Point { get; set; }
        public IEnumerable<GameEntity> Entities { get; set; }
    }
}