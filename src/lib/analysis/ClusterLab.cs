
using Lib.Colors;

namespace Lib.Analysis;

public abstract record SafeClusterLab<T> : ICluster<VectorLab, T>, IComparable<SafeClusterLab<T>> where T: ICluster<VectorLab, T>
{
    public SafeClusterLab(VectorLab cluster, VectorLab mean, int count)
    {
        Cluster = cluster;
        Mean = mean;
        Count = count;
    }

    public VectorLab Cluster { get; set; }

    public VectorLab Mean { get; set; }

    public int Count { get; set; }

    public virtual int CompareTo(SafeClusterLab<T>? other)
    {
        if (other == null)
        {
            return -1;
        }

        return Cluster.CompareTo(other.Cluster);
    }

    public abstract T ParallelSafeCopy();
}

public sealed record ClusterLab : SafeClusterLab<ClusterLab>
{
    public ClusterLab(VectorLab cluster, VectorLab mean, int count) : base(cluster, mean, count) { }
    
    public override ClusterLab ParallelSafeCopy()
    {
        return new ClusterLab(Cluster, Mean, Count);
    }
}
