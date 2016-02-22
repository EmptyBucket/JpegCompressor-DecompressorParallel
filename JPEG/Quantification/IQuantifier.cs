namespace JPEG.Quantification
{
    public interface IQuantifier
    {
        double[,] Quantification(double[,] matrix, double[,] matrixQuantification);
        double[,] UnQuantification(double[,] matrix, double[,] matrixQuantification);
    }
}