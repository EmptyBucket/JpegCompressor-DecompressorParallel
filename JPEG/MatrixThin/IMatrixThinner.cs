namespace JPEG.MatrixThin
{
    public interface IMatrixThinner<T>
    {
        T[,] Thin(T[,] array, int thinIndex);
    }
}