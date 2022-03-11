using System.Collections.Generic;

namespace LegendOfConSol
{
    class SaveData
    {
        //Level One Flags
        public bool HimFlag { get; set; } = false;
        public bool BridgeActive { get; set; } = false;
        public int HimTalkCount { get; set; } = 0;
        public Dictionary<string, int[]> TreeLocations { get; set; } = new Dictionary<string, int[]>();
        public List<string> TreesCut { get; set; } = new List<string>();

        //Level Two Flags
        public Dictionary<string, int[]> RockLocations { get; set; } = new Dictionary<string, int[]>();
        public bool GuardBlock { get; set; } = true;
        public int GuardPoke { get; set; } = 0;
        public bool WatchedLvl2Anim { get; set; } = false;

        //Castle Flags
        public bool KingTalked { get; set; } = false;
    }
}
