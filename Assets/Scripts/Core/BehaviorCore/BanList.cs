using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Core.BehaviorCore
{
    public class BanList
    {
        private List<BanInfo> banList = new List<BanInfo>();
        private float lastUpdate = 0;

        public bool IsBanned(GameUnit unit)
        {
            UpdateBans();

            return banList.Any(info => info.Unit == unit);
        }

        public void Ban(GameUnit unit, float timeUntil)
        {
            banList.Add(new BanInfo(unit, timeUntil));
        }

        private void UpdateBans()
        {
            if (lastUpdate == Time.time) return;

            for (int i = 0; i < banList.Count; i++)
            {
                if (banList[i].IgnoreTime < Time.time)
                {
                    banList.RemoveAt(i);
                    i--;
                }
            }
            lastUpdate = Time.time;
        }

        private class BanInfo
        {
            public GameUnit Unit { get; private set; }
            public float IgnoreTime { get; private set; }

            public BanInfo(GameUnit unit, float ignoreTime)
            {
                Unit = unit ?? throw new ArgumentNullException(nameof(unit));
                IgnoreTime = ignoreTime > Time.time ? ignoreTime : throw new ArgumentException($"Must be later than now: get {ignoreTime} vs {Time.time}", nameof(ignoreTime));
            }
        }
    }
}
