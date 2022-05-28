using System;

namespace KaizerWald
{
    public enum WaypointCardinal : int
    {
        North = 0,
        Est   = 1,
        South = 2,
        West  = 3,
    }

    public static class Cardinals
    {
        private static readonly int numCardinals;
        static Cardinals()
        {
            numCardinals = Enum.GetValues(typeof(WaypointCardinal)).Length;
        }

        public static int NumCardinals => Enum.GetValues(typeof(WaypointCardinal)).Length;
    }
}
