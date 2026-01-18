namespace Lib.Analysis.Interfaces;

public interface IKMeans<T>
{
    public T[] Clusters { get; set; }

    public void Cluster(ColorRgb[] pixels);

    public void ClusterParallel(ColorRgb[] pixels);
}
