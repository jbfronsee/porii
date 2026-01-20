using System.Collections.Concurrent;

using Lib.Analysis.Interfaces;
using Lib.Colors;
using Lib.Colors.Interfaces;

namespace Lib.Analysis;

public abstract class KMeans<T, U> : IKMeans<T>
    where T: class, ICluster<U, T>
    where U: IColorVector<double>
{
    public virtual T[] Clusters { get; set; } = [];

    protected virtual int UpdateBestCluster(T[] clusters, ColorRgb pixel, Dictionary<ColorRgb, int> memoizedClusters)
    {
        U color = UnpackPixel(pixel);

        int bestClusterIndex = 0;
        if (!memoizedClusters.TryGetValue(pixel, out bestClusterIndex))
        {
            double bestDistance = double.MaxValue;
            for (int i = 0; i < clusters.Length; i++)
            {
                T cluster = clusters[i];
                U value = cluster.Cluster;
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
        bestCluster.Mean = ColorMath.CalculateNewMean(bestCluster.Mean, bestCluster.Count, color);
        bestCluster.Count++;

        return bestClusterIndex;
    }

    protected abstract U UnpackPixel(ColorRgb pixel);

    public virtual void Cluster(ColorRgb[] pixels)
    {
        Dictionary<ColorRgb, int> memoizedClusters = [];

        foreach(var pixel in pixels)
        {
            UpdateBestCluster(Clusters, pixel, memoizedClusters);
        }
    }

    public virtual void ClusterParallel(ColorRgb[] pixels)
    {
        ConcurrentBag<T[]> bag = [];
        Parallel.ForEach(
            pixels,
            () => (new Dictionary<ColorRgb, int>(), Clusters.Select(c => c.ParallelSafeCopy()).ToArray()), 
            (pixel, _, threadLocals) =>
            {
                var (memoizedCluster, means) = threadLocals;
                UpdateBestCluster(means, pixel, memoizedCluster);
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
                    total.Mean = ColorMath.AddMeans(total.Mean, total.Count, threadResults.Mean, threadResults.Count);
                    total.Count += threadResults.Count;
                    return total;
                }
            ).ToArray();
        }
    }

    public virtual IEnumerable<int> BestClustersWithProgress(ColorRgb[] pixels, int maxIterations, bool parallelize = false)
    {
        bool finished = false;
        for (int i = 0; (i < maxIterations) && !finished; i++)
        {
            yield return i;

            if (parallelize)
            {
                ClusterParallel(pixels);
            }
            else
            {
                Cluster(pixels);
            }

            finished = true;
            foreach (var cluster in Clusters)
            {
                finished = finished && (ColorMath.CalculateDistance(cluster.Cluster, cluster.Mean) <= 1.0);
                
                cluster.Cluster = cluster.Mean;
            }
        }
    }
}

public class KMeansLab : KMeans<ClusterLab, VectorLab>
{
    public Dictionary<ColorRgb, PackedLab> Colormap { get; set; } = [];

    public KMeansLab() {}

    public KMeansLab(ClusterLab[] clusters, Dictionary<ColorRgb, PackedLab> colormap)
    {
        Clusters = clusters;
        Colormap = colormap;
    }

    protected override VectorLab UnpackPixel(ColorRgb pixel)
    {
        return Colormap[pixel].Unpack();
    }
}
