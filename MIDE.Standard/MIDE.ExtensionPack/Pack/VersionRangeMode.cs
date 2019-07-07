namespace MIDE.ExtensionPack
{
    /// <summary>
    /// Dependency version range mode
    /// </summary>
    public enum VersionRangeMode
    {
        /// <summary>
        /// [min,required] Only versions between the min and required are allowed
        /// </summary>
        BothInclusive,
        /// <summary>
        /// (min,required] Only versions between the min and required are allowed excluding min
        /// </summary>
        LeftExclusive,
        /// <summary>
        /// [min,max) Only versions between the min and max are allowed excluding max
        /// </summary> 
        RightExclusive,
        /// <summary>
        /// (man,max) Only versions in-between the min and max are allowed
        /// </summary>
        BothExclusive = LeftExclusive | RightExclusive
    }
}