namespace Instructables.DataModel
{
    public class GroupedItem
    {
        public DataGroup Group { get; set; }
        public int GroupOrdinal { get; set; }
        public InstructableSummary SummaryItem { get; set; }
    }
}
