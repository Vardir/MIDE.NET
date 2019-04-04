namespace MIDE.API.Components
{
    public interface ICloneable<T>
    {
        T Clone();
        T Clone(string id);
    }
}