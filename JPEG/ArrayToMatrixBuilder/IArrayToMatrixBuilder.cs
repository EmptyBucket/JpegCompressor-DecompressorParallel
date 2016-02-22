namespace JPEG.ArrayToMatrixBuilder
{
    public interface IArrayToMatrixBuilder
    {
        T[,] Build<T>(T[] array, int matrixLengthX, int matrixLengthY);
    }
}