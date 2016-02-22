namespace JPEG.MatrixToArrayTransform
{
    public interface IMatrixToArrayTransformer
    {
        T[] Transform<T>(T[,] matrix);
    }
}