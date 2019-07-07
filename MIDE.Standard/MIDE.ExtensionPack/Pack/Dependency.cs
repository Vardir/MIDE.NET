using MIDE.Helpers;
using System;

namespace MIDE.ExtensionPack
{
    public class Dependency
    {
        public string Target { get; }
        public Version Version { get; }
        public Version MinimumVersion { get; }
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

        public override string ToString()
        {
            char left = '(';
            char right = ')';
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
            return VersionRangeMode.BothInclusive;
        }
        public static Dependency FromString(string str)
        {
            var (target, tail) = str.ExtractUntil(0, ':');
            char left = tail[1];
            char right = tail[tail.Length - 1];
            var (value, _) = tail.ExtractUntil(2, ')', ']');
            var parts = value.Split(',');
            var min = Version.Parse(parts[0]);
            var maj = Version.Parse(parts[1]);
            return new Dependency(target, maj, min, From(left, right));
        }
    }
}