namespace JPEG.MatrixExtend
{
    public interface IMatrixExtender
    {
        T[,] Extend<T>(T[,] matrix, int yExtend, int xExtend);
    }
}