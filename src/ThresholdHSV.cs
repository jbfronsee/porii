public struct ThresholdHSV(double hue = 0, double saturation = 0, double value = 0, double valueStart = 0)
{
    public double Hue { get; set; } = hue;

    public double Saturation { get; set; } = saturation;

    public double Value { get; set; } = value;

    public double ValueStart { get; set; } = valueStart;
}