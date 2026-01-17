namespace Lib.Colors;

public static class ColorMath
{
    public static double UpdateMean(double mean, double count, double newValue)
    {
        double sum = mean * count;
        return (sum + newValue) / (count + 1);
    }

    public static double UpdateMean(double mean, double count, double otherMean, double otherCount)
    {
        double sum1 = mean * count;
        double sum2 = otherMean * otherCount;
        return (sum1 + sum2) / (count + otherCount);
    }

    public static T UpdateMeanColor<T>(T mean, int count, T newColor) where T: IColorVector<double>
    {
        mean.X = UpdateMean(mean.X, count, newColor.X);
        mean.Y = UpdateMean(mean.Y, count, newColor.Y);
        mean.Z = UpdateMean(mean.Z, count, newColor.Z);
        return mean;
    }

    public static T UpdateMeanColor<T>(T mean, int count, T mean2, int count2) where T: IColorVector<double>
    {
        if (count == 0 && count2 == 0)
        {
            return mean;
        }

        mean.X = UpdateMean(mean.X, count, mean2.X, count2);
        mean.Y = UpdateMean(mean.Y, count, mean2.Y, count2);
        mean.Z = UpdateMean(mean.Z, count, mean2.Z, count2);
        return mean;
    }

    /// <summary>
    /// Calculate distance between 2 colors in a Euclidean space.
    /// </summary>
    /// <param name="color1">First Color.</param>
    /// <param name="color2">Second Color.</param>
    /// <returns>The distance between them.</returns>
    public static double CalculateDistance<T>(T color1, T color2) where T: IColorVector<double>
    {
        return Math.Sqrt(CalculateDistanceSquared(color1, color2));
    }

    /// <summary>
    /// Calculate distance between 2 colors in a Euclidean space.
    /// </summary>
    /// <param name="color1">First Color.</param>
    /// <param name="color2">Second Color.</param>
    /// <returns>The distance between them.</returns>
    public static double CalculateDistanceSquared<T>(T color1, T color2) where T: IColorVector<double>
    {
        double deltaX = Math.Abs(color2.X - color1.X);
        double deltaY = Math.Abs(color2.Y - color1.Y);
        double deltaZ = Math.Abs(color2.Z - color1.Z);
        return deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ;
    }
}