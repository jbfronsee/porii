using Lib.Analysis.Interfaces;
using Lib.Colors;

namespace Lib.Analysis;

public sealed record EntryLab : ClusterLab<EntryLab>, IEntry<VectorLab, EntryLab>
{
    public EntryLab(VectorLab cluster, VectorLab mean, int count) : base(cluster, mean, count) { }
    
    public VectorLab Bucket { get => Cluster; set => Cluster = value; }

    public override EntryLab ParallelSafeCopy()
    {
        return new EntryLab(Cluster, Mean, Count);
    }
}
