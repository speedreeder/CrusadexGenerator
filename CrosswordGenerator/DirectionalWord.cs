namespace CrosswordGenerator
{
    internal class DirectionalWord
    {
        public DirectionalWord(bool isVertical, List<CrosswordCell> cells)
        {
            IsVertical = isVertical;
            Cells = cells;
        }
        public bool IsVertical { get; set; }
        public List<CrosswordCell> Cells { get; set; }
    }
}
