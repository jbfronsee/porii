namespace Lib.Analysis;

public class Histogram
{
    private const int BucketCount = 256;

    public record Entry : IComparable<Entry>
    {
        private SimpleColor.LabComparer mComparer = new();

        public SimpleColor.PackedLab Bucket { get; set; }
        
        public SimpleColor.PackedLab Average { get; set;} 
        
        public int Count { get; set; }

        public int CompareTo(Entry? other)
        {
            if (other == null)
            {
                return -1;
            }

            return mComparer.Compare(Conversion.Lab.Unpack(Bucket), Conversion.Lab.Unpack(other.Bucket));
        }
    }

    public Entry[] Results { get; } = new Entry[BucketCount];
}