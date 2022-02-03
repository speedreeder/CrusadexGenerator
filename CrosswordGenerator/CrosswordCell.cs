namespace CrosswordGenerator
{
    public class CrosswordCell
    {
        public CrosswordCell(string column, int row)
        {
            Column = column;
            Row = row;
        }
        public string Column { get; set; }
        public int Row { get; set; }
        public bool Selected { get; set; }
    }
}
