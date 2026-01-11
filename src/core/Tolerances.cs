public class Tolerances(ThresholdHsv darks, ThresholdHsv shadows, ThresholdHsv midtones, ThresholdHsv brights)
{
    public ThresholdHsv Darks { get; set; } = darks;

    public ThresholdHsv Shadows { get; set; } = shadows;

    public ThresholdHsv Midtones { get; set; } = midtones;

    public ThresholdHsv Brights { get; set; } = brights;

    /// <summary>
    /// Get threshold for dark, shadow, midtone, or bright values.
    /// </summary>
    /// 
    /// <param name="value">Value component of color to compare.</param>
    public ThresholdHsv GetThreshold(double value)
    {
        if (value >= Shadows.ValueStart && value < Midtones.ValueStart)
        {
            return Shadows;
        }
        else if (value >= Midtones.ValueStart && value < Brights.ValueStart)
        {
            return Midtones;
        }
        else if (value >= Brights.ValueStart)
        {
            return Brights;
        }

        return Darks;
    }

    public override string ToString()
    {
        return $"Darks: {Darks}\nShadows: {Shadows}\nMidtones: {Midtones}\nBrights: {Brights}";
    }

    public (bool, string) Validate()
    {
        if (
            Darks.ValueStart > Shadows.ValueStart ||
            Shadows.ValueStart > Midtones.ValueStart || 
            Midtones.ValueStart > Brights.ValueStart
        )
        {
            return (false, "Start values for tolerance ranges are invalid.");
        }
        
        string err = "Invalid field value for {0}: {1}";

        (ThresholdHsv, string)[] fields = [(Darks, "Darks"), (Shadows, "Shadows"), (Midtones, "Midtones"), (Brights, "Brights")];

        foreach ((ThresholdHsv thresh, string name) in fields)
        {
            (bool result, string message) = thresh.Validate();
            if (!result)
            {
                return (false, string.Format(err, name, message));
            }   
        }

        return (true, "");
    }
}