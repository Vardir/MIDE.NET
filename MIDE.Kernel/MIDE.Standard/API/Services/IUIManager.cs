using MIDE.API.Components;

namespace MIDE.API.Services
{
    public interface IUIManager
    {
        Menu ApplicationMenu { get; }

        TabSection this[string id] { get; }

        void AddSection(TabSection section);
    }
}