namespace JPEG.QuantificationMatrixProvide
{
    public interface IMatrixProvider<out T>
    {
        T[,] GetMatrix();
    }
}