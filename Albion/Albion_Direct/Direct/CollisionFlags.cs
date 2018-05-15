////////////////////////////////////////////////////////////////////////////////////
// Merlin API for Albion Online v1.11.362.117031-prod
////////////////////////////////////////////////////////////////////////////////////
//------------------------------------------------------------------------------
// <auto-generated>
// This code was generated by a tool.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Albion_Direct
{
    /* Internal type: ai2.a */
    public enum CollisionFlags
    {
        None = 0,
        CollidedSides = 1,
        Sides = 1,
        Above = 2,
        CollidedAbove = 2,
        Below = 4,
        CollidedBelow = 4
    }
    
    public static class CollisionFlagsExtensions
    {
        public static ai2.a ToInternal(this CollisionFlags instance)
        {
            return (ai2.a)instance;
        }
        
        public static CollisionFlags ToWrapped(this ai2.a instance)
        {
            return (CollisionFlags)instance;
        }
    }
}
