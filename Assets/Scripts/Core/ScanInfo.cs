using System;
using System.Collections.Generic;

namespace Assets.Scripts.Core
{
    public class ScanInfo
    {
        public GraphPointInfo Point { get; set; }
        public IEnumerable<GameEntity> Entities { get; set; }

        public ScanInfo(GraphPointInfo point, IEnumerable<GameEntity> entities)
        {
            Point = point ?? throw new ArgumentNullException(nameof(point));
            Entities = entities ?? throw new ArgumentNullException(nameof(entities));
        }
    }
}