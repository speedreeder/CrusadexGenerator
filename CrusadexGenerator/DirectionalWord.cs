namespace CrusadexGenerator
{
    internal class DirectionalWord
    {
        public DirectionalWord(bool isVertical, List<CrusadexCell> cells)
        {
            IsVertical = isVertical;
            Cells = cells;
        }
        public bool IsVertical { get; set; }
        public List<CrusadexCell> Cells { get; set; }
        public int? GeneratedIndex { get; set; }
    }
}
