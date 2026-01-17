using Lib.Colors;

namespace Lib.Analysis;

public class Histogram : KMeans<Histogram.Entry, PackedLab, VectorLab>
{
    private const int BucketCount = 256;

    public record Entry : SafeClusterLab<Entry>
    {
        public Entry(VectorLab cluster, VectorLab mean, int count) : base(cluster, mean, count) { }
        
        public VectorLab Bucket { get => Cluster; set => Cluster = value; }

        public override Entry ParallelSafeCopy()
        {
            return new Entry(Cluster, Mean, Count);
        }
    }

    public override Entry[] Clusters { get; set; } = new Entry[BucketCount];

    public Entry[] Results => Clusters;
}