namespace MenteBacata.ScivoloCharacterController.Internal
{
    /*
     * Surface used to determine the resulting movement when moving onto it. It's type is determined by the angle that the 
     * surface normal forms with the upDirection.
     */
    public enum MovementSurface
    {
        Floor,         // It faces up and the slope angle is less than max floor angle.
        SteepSlope,    // It faces up and it's steeper than floor but not as vertical as a wall.
        Wall,          // Nearly vertical surface.
        SlopedCeiling, // It faces down and is sloped.
        FlatCeiling    // It faces down and is nearly flat.
    }
}
