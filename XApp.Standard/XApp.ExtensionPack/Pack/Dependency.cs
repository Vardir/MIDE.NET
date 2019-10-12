using System;

using Vardirsoft.Shared.Helpers;

namespace Vardirsoft.XApp.ExtensionPack
{
    /// <summary>
    /// An extension pack dependency which corresponds to either application version or another extension
    /// </summary>
    public class Dependency
    {
        /// <summary>
        /// Target ID
        /// </summary>
        public string Target { get; }
        /// <summary>
        /// Required target version
        /// </summary>
        public Version Version { get; }
        /// <summary>
        /// Minimum allowed target version
        /// </summary>
        public Version MinimumVersion { get; }
        /// <summary>
        /// Flag indicating how versions should be handled
        /// </summary>
        public VersionRangeMode VersionRangeMode { get; }

        public Dependency(string target, Version version)
        {
            Target = target;
            Version = version;
            MinimumVersion = version;
        }
        public Dependency(string target, Version version, Version minVersion, VersionRangeMode rangeMode)
        {
            Target = target;
            Version = version;
            MinimumVersion = minVersion;
            VersionRangeMode = rangeMode;
        }

        /// <summary>
        /// Converts dependency object to string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var left = '(';
            var right = ')';

            switch (VersionRangeMode)
            {
                case VersionRangeMode.BothInclusive:
                    left = '['; right = ']'; break;
                case VersionRangeMode.RightExclusive:
                    left = '['; break;
                case VersionRangeMode.LeftExclusive:
                    right = ']'; break;
            }

            return $"{Target}::{left}{MinimumVersion.ToString(3)},{Version.ToString(3)}{right}";
        }

        /// <summary>
        /// Converts the given pair of characters to version range mode
        /// </summary>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static VersionRangeMode From(char l, char r)
        {
            if (l == '[' && r == ']')
                return VersionRangeMode.BothInclusive;
                
            if (l == '(' && r == ')')
                return VersionRangeMode.BothExclusive;
                
            if (l == '(')
                return VersionRangeMode.LeftExclusive;
                
            if (r == ')')
                return VersionRangeMode.RightExclusive;
                
            throw new ArgumentException("Invalid characters given");
        }
        /// <summary>
        /// Converts the given string back to dependency
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Dependency FromString(string str)
        {
            var (target, tail) = str.ExtractUntil(0, ':');
            var left = tail[1];
            var right = tail[tail.Length - 1];
            var (value, _) = tail.ExtractUntil(2, ')', ']');
            var parts = value.Split(',');
            var min = Version.Parse(parts[0]);
            var maj = Version.Parse(parts[1]);
            
            return new Dependency(target, maj, min, From(left, right));
        }
    }
}