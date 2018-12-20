using System.Globalization;
using MIDE.Standard.API.Components;

namespace MIDE.Standard.Helpers
{
    public static class ApplicationComponentHelpers
    {
        private static readonly TextInfo TEXT_INFO = new CultureInfo("en-US", false).TextInfo;

        public static string FormatId(this IApplicationComponent component) => TEXT_INFO.ToTitleCase(component.Id.Replace('-', ' '));
        public static string GetSpec(this IApplicationComponent component) => $"[{component.GetType().Name}] ID:{component.Id}";
    }
}
