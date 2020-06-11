namespace AresAdditiveAnalysisPlugin.Database.Filtering
{
    public interface IDataFilterOptions
    {
        double BlueLineStraightnessFactor { get; set; }
        bool FilterBlueLineStraightnessFactor { get; set; }
    }
}
