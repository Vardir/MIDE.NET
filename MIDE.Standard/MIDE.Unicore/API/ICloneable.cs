namespace MIDE.API
{
    public interface ICloneable<T>
    {
        T Clone();
        T Clone(string id);
    }
}