namespace App.Core;

public class BucketPoint(double saturation, double value, int bins)
{
    public double Saturation { get; set; } = saturation;

    public double Value { get; set; } = value;

    public int Bins { get; set; } = bins;

    public (bool, string) Validate()
    {
        string svError = "{0} must be specified from the range 0 to 1.";
        if (Saturation < 0 || Saturation > 1)
        {
            return (false, string.Format(svError, "Saturation"));
        }
        else if (Value <  0 || Value > 1)
        {
            return (false, string.Format(svError, "Value"));
        }

        return (true, "");
    }
}