namespace JPEG.MatrixExtend
{
    public interface IPieceMatrixExtender<T>
    {
        T[,] Extend(T[,] matrix, int extendIndex);
    }
}