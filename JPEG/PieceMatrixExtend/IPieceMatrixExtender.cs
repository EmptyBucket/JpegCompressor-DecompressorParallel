namespace JPEG.PieceMatrixExtend
{
    public interface IPieceMatrixExtender<T>
    {
        T[,] Extend(T[,] matrix, int extendIndex);
    }
}