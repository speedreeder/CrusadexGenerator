namespace CrosswordGenerator
{
    public class CrosswordCell
    {
        public CrosswordCell(string column, int row, bool selected)
        {
            Column = column;
            Row = row;
            Selected = selected;
        }
        public string Column { get; set; }
        public int Row { get; set; }
        public bool Selected { get; set; }
    }
}
