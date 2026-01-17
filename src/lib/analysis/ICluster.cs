namespace Lib.Analysis;

public interface ICluster<T, U> where U: ICluster<T, U>
{
    public T Cluster { get; set; }
    
    public T Mean { get; set;} 
        
    public int Count { get; set; }

    public U ParallelSafeCopy();
}