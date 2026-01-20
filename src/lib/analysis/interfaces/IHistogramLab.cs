using Lib.Colors;

namespace Lib.Analysis.Interfaces;

public interface IHistogramLab : IHistogram<EntryLab>
{
     public Dictionary<ColorRgb, PackedLab> Colormap { get; set; }
}
