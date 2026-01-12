public class ThresholdHsv(double hue = 0, double saturation = 0, double value = 0, double valueStart = 0)
{
    public double Hue { get; set; } = hue;

    public double Saturation { get; set; } = saturation;

    public double Value { get; set; } = value;

    public double ValueStart { get; set; } = valueStart;

    public override string ToString()
    {
        return $"Hue: {Hue} Saturation: {Saturation} Value: {Value} ValueStart: {ValueStart}";
    }

    public (bool, string) Validate()
    {
        string svError = "{0} must be specified from the range 0 to 1.";
        if (Hue < 0 || Hue > 360)
        {
            return (false, "Hue must be specified in degrees from 0 to 360.");
        }
        else if (Saturation < 0 || Saturation > 1)
        {
            return (false, string.Format(svError, "Saturation"));
        }
        else if (Value <  0 || Value > 1)
        {
            return (false, string.Format(svError, "Value"));
        }
        else if (ValueStart < 0 || ValueStart > 1)
        {
            return (false, string.Format(svError, "ValueStart"));
        }

        return (true, "");
    }
}