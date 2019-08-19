using Lockstep.Collision2D;
using Lockstep.Math;

namespace Lockstep.Game {
    public static class HashCodeCalculator {
        public static int GetHash(this byte val){return val;}
        public static int GetHash(this short val){return val;}
        public static int GetHash(this int val){return val;}
        public static int GetHash(this long val){return (int) val;}
        public static int GetHash(this sbyte val){return val;}
        public static int GetHash(this ushort val){return val;}
        public static int GetHash(this uint val){return (int)val;}
        public static int GetHash(this ulong val){return (int) val;}
        public static int GetHash(this bool val){return val ? 1 : 0;}
        
        public static int GetHash(this LFloat val){
            return PrimerLUT.GetPrimer(val._val);
        }

        public static int GetHash(this LVector2 val){
            return PrimerLUT.GetPrimer(val._x) + PrimerLUT.GetPrimer(val._y) * 17;
        }

        public static int GetHash(this LVector3 val){
            return PrimerLUT.GetPrimer(val._x)
                   + PrimerLUT.GetPrimer(val._y) * 31
                   + PrimerLUT.GetPrimer(val._z) * 37;
        }
        public static int GetHash(this CTransform2D val){
            return val.Pos3.GetHash() * 13
                   + val.deg.GetHash() * 31;
  
        }
    }
}