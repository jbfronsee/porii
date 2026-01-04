public class Tolerances
{
    public const double DEF_DARK_H = 360;

    public const double DEF_DARK_S = 1.0;

    public const double DEF_DARK_V = .3;

    public const double DEF_DARK_START = 0;

    public const double DEF_SHADOW_H = 14.4;

    public const double DEF_SHADOW_S = .8;

    public const double DEF_SHADOW_V = .3;

    public const double DEF_SHADOW_START = .2;

    public const double DEF_MID_H = 7.2;

    public const double DEF_MID_S = .6;

    public const double DEF_MID_V = .3;

    public const double DEF_MID_START = .4;

    public const double DEF_BRIGHT_H = 7.2;

    public const double DEF_BRIGHT_S = .3;

    public const double DEF_BRIGHT_V = .3;

    public const double DEF_BRIGHT_START = .6;

    public Tolerances()
    {
        Darks = new(DEF_DARK_H, DEF_DARK_S, DEF_DARK_V, DEF_DARK_START);
        Shadows = new(DEF_SHADOW_H, DEF_SHADOW_S, DEF_SHADOW_V, DEF_SHADOW_START);
        Midtones = new(DEF_MID_H, DEF_MID_S, DEF_MID_V, DEF_MID_START);
        Brights = new(DEF_BRIGHT_H, DEF_BRIGHT_S, DEF_BRIGHT_V, DEF_BRIGHT_START);
    }

    public ThresholdHSV Darks { get; set; }

    public ThresholdHSV Shadows { get; set; }

    public ThresholdHSV Midtones { get; set; }

    public ThresholdHSV Brights { get; set; }

    /// <summary>
    /// Get threshold for dark, shadow, midtone, or bright values.
    /// </summary>
    /// 
    /// <param name="value">Value component of color to compare.</param>
    public ThresholdHSV GetThreshold(double value)
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
}