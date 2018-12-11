namespace MIDE.Standard.API.Validation
{
    public interface IValidator<T>
    {
        bool IsValid(T value);
    }
}