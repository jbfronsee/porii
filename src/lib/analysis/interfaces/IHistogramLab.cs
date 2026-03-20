using Lib.Colors;

namespace Lib.Analysis.Interfaces;

public interface IHistogramLab : IHistogram<EntryLab, VectorLab>
{
     public Dictionary<ColorRgb, PackedLab> Colormap { get; set; }
}
