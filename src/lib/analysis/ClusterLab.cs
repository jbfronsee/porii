using Lib.Analysis.Interfaces;
using Lib.Colors;

namespace Lib.Analysis;

public abstract record ClusterLab<T> : ICluster<VectorLab, T>, IComparable<ClusterLab<T>>
    where T: ICluster<VectorLab, T>
{
    public ClusterLab(VectorLab cluster, VectorLab mean, int count)
    {
        Cluster = cluster;
        Mean = mean;
        Count = count;
    }

    public VectorLab Cluster { get; set; }

    public VectorLab Mean { get; set; }

    public int Count { get; set; }

    public virtual int CompareTo(ClusterLab<T>? other)
    {
        if (other == null)
        {
            return -1;
        }

        return Cluster.CompareTo(other.Cluster);
    }

    public abstract T ParallelSafeCopy();
}

public sealed record SafeClusterLab : ClusterLab<SafeClusterLab>
{
    public SafeClusterLab(VectorLab cluster, VectorLab mean, int count) : base(cluster, mean, count) { }
    
    public override SafeClusterLab ParallelSafeCopy()
    {
        return new SafeClusterLab(Cluster, Mean, Count);
    }
}
