public interface ILoad <T> where T : class
{
    T[] Result { get; set; }
}
