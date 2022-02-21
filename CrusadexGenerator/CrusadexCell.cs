namespace CrusadexGenerator
{
    public class CrusadexCell
    {
        public CrusadexCell(string column, int row, bool selected)
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
