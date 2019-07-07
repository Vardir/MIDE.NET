namespace MIDE.ExtensionPack
{
    public enum VersionRangeMode
    {
        BothInclusive,
        LeftExclusive,
        RightExclusive,
        BothExclusive = LeftExclusive | RightExclusive
    }
}