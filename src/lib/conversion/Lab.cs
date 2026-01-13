namespace Conversion
{
    public static class Lab
    {
        // Calculated ranges:
        // Min: Lab { L = 0, A = -86.18271462445199, B = -107.86016288933186 }
        // Max: Lab { L = 99.99999999999999, A = 98.23433854246716, B = 94.47796331945983 }

        private const double LMax = 100;

        private const double AMax = AShift + 98.23433854246716;

        private const double BMax = BShift + 94.47796331945983;

        private const double AShift = 86.18271462445199;

        private const double BShift = 107.86016288933186;

        private const double LScale = ushort.MaxValue / LMax;

        private const double AScale = ushort.MaxValue / AMax;

        private const double BScale = ushort.MaxValue / BMax;

        private static double ABPack(double val, double scale, double shift)
        {
            return (val + shift) * scale;
        }

        private static ushort LPack(double l)
        {
            return (ushort)(l * LScale);
        }

        private static ushort APack(double a)
        {
            
            return (ushort)ABPack(a, AScale, AShift);
        }

        private static ushort BPack(double b)
        {
            return (ushort)ABPack(b, BScale, BShift);
        }

        private static double ABUnpack(double val, double scale, double shift)
        {
            
            return (val / scale) - shift;
        }

        private static double LUnpack(ushort l)
        {
            return l / LScale;
        }

        private static double AUnpack(ushort a)
        {
            
            return ABUnpack(a, AScale, AShift);
        }

        private static double BUnpack(ushort b)
        {
            return ABUnpack(b, BScale, BShift);
        }

        /// <summary>
        /// Convert Lab value from double representation to ushort representation.
        /// 
        /// Precision of Lab space is lost during conversion but the calculated
        /// total RGB values that it can map is all 16,777,216 possible values.
        /// </summary>
        /// <param name="l">l</param>
        /// <param name="a">a</param>
        /// <param name="b">b</param>
        /// <returns></returns>
        public static SimpleColor.PackedLab Pack(double l, double a, double b)
        {
            return new SimpleColor.PackedLab(LPack(l), APack(a), BPack(b));
        }

        /// <summary>
        /// Convert Lab value from double representation to byte representation.
        /// 
        /// Precision of Lab space is lost during conversion but the calculated
        /// total RGB values that it can map is all 16,777,216 possible values.
        /// </summary>
        /// <param name="lab">Color to convert.</param>
        /// <returns></returns>
        public static SimpleColor.PackedLab Pack(SimpleColor.Lab lab)
        {
            return Pack(lab.L, lab.A, lab.B);
        }

        /// <summary>
        /// Convert Lab value from ushort representation to double representation.
        /// </summary>
        /// <param name="l">l</param>
        /// <param name="a">a</param>
        /// <param name="b">b</param>
        /// <returns></returns>
        public static SimpleColor.Lab Unpack(ushort l, ushort a, ushort b)
        {
            return new SimpleColor.Lab(LUnpack(l), AUnpack(a), BUnpack(b));
        }

        /// <summary>
        /// Convert Lab value from ushort representation to double representation.
        /// </summary>
        /// <param name="lab">Color to convert.</param>
        /// <returns></returns>
        public static SimpleColor.Lab Unpack(SimpleColor.PackedLab lab)
        {
            return Unpack(lab.L, lab.A, lab.B);
        }
    }
}