namespace JPEG.MatrixExtend
{
    public interface IMatrixExtender<T>
    {
        T[,] Extend(T[,] matrix, int extendIndex);
    }
}