using System.Collections.Concurrent;

using Lib.Colors;

namespace Lib.Analysis;

public abstract class KMeans<T, U, V>
    where T: class, ICluster<V, T>
    where U: IPacked<V>
    where V: IColorVector<double>, IPackable<U>
{
    public virtual T[] Clusters { get; set; } = [];

    protected static void UpdateBestCluster(T[] clusters, SimpleColor.Rgb pixel, Dictionary<SimpleColor.Rgb, U>? colormap, Dictionary<SimpleColor.Rgb, int> memoizedClusters)
    {
        V color;
        if (colormap is null)
        {
            // TODO figure out how to handle this.
            return;
            //color = pixel;
        }
        else
        {
            color = colormap[pixel].Unpack();
        }

        int bestClusterIndex = 0;
        if (!memoizedClusters.TryGetValue(pixel, out bestClusterIndex))
        {
            double bestDistance = double.MaxValue;
            for (int i = 0; i < clusters.Length; i++)
            {
                T cluster = clusters[i];
                V value = cluster.Cluster;
                double distance = ColorMath.CalculateDistanceSquared(value, color);
                if (distance < bestDistance)
                {
                    bestClusterIndex = i;
                    bestDistance = distance;
                }
            }

            memoizedClusters[pixel] = bestClusterIndex;
        }

        T bestCluster = clusters[bestClusterIndex];
        bestCluster.Mean = ColorMath.UpdateMeanColor(bestCluster.Mean, bestCluster.Count, color);
        bestCluster.Count++;
    }

    public virtual void Cluster(SimpleColor.Rgb[] pixels, Dictionary<SimpleColor.Rgb, U> colormap)
    {
        Dictionary<SimpleColor.Rgb, int> memoizedClusters = [];

        foreach(var pixel in pixels)
        {
            UpdateBestCluster(Clusters, pixel, colormap, memoizedClusters);
           // Console.WriteLine($"{bestClusterIndex} Count: {bestCluster.Count}");
        }

        // foreach(var cluster in clusters.Where(c => c.Count > 0))
        // {
        //     Console.WriteLine(cluster);
        // }
    }

    public virtual void ClusterParallel(SimpleColor.Rgb[] pixels, Dictionary<SimpleColor.Rgb, U>? colormap = null)
    {
        //Console.WriteLine("still running this");
        ConcurrentBag<T[]> bag = [];
        Parallel.ForEach(
            pixels,
            () => (new Dictionary<SimpleColor.Rgb, int>(), Clusters.Select(c => c.ParallelSafeCopy()).ToArray()), 
            (pixel, _, threadLocals) =>
            {
                var (memoizedCluster, means) = threadLocals;
                UpdateBestCluster(means, pixel, colormap, memoizedCluster);
                return (memoizedCluster, means);
            }, 
            threadLocals =>
            {
                var (_, means) = threadLocals;
                bag.Add(means);
            }
        );


        foreach(var threadMeans in bag)
        {
            Clusters = Clusters.Zip(threadMeans).Select(z => 
                {
                    var (total, threadResults) = z;
                    total.Mean = ColorMath.UpdateMeanColor(total.Mean, total.Count, threadResults.Mean, threadResults.Count);
                    total.Count += threadResults.Count;
                    return total;
                }
            ).ToArray();
        }
    }
}

public class KMeansLab : KMeans<ClusterLab, PackedLab, VectorLab>
{
    public KMeansLab() {}

    public KMeansLab(ClusterLab[] clusters)
    {
        Clusters = clusters;
    }

}
