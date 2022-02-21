using CrusadexGenerator.Extensions;
using HtmlTags;

namespace CrusadexGenerator.Helpers
{
    public static class CrusadexCellListHelpers
    {
        public static List<DirectionalWord> GetWordsFromCellList(List<CrusadexCell> cellList)
        {
            var words = new List<DirectionalWord>();
            var checkList = cellList.Where(c => c.Selected).ToList();

            foreach (var selectedCell in checkList)
            {
                var rightCell = cellList.SingleOrDefault(c => c.Row == selectedCell.Row && c.Column.ToColumnIndex() == selectedCell.Column.ToColumnIndex() + 1 && c.Selected);

                if (rightCell != null && !words.Any(w => !w.IsVertical && w.Cells.Contains(rightCell)))
                {
                    var word = new DirectionalWord(false, new List<CrusadexCell>());
                    word.Cells.Add(selectedCell);
                    var column = selectedCell.Column.ToColumnIndex();
                    while (checkList.Any(c => c.Row == selectedCell.Row && c.Column.ToColumnIndex() == column + 1))
                    {
                        column++;
                        word.Cells.Add(checkList.Single(c => c.Row == selectedCell.Row && c.Column.ToColumnIndex() == column));
                    }

                    words.Add(word);
                }

                var downCell = cellList.SingleOrDefault(c => c.Row == selectedCell.Row + 1 && c.Column == selectedCell.Column && c.Selected);
                if (downCell != null && !words.Any(w => w.IsVertical && w.Cells.Contains(downCell)))
                {
                    var word = new DirectionalWord(true, new List<CrusadexCell>());
                    word.Cells.Add(selectedCell);
                    var row = selectedCell.Row;
                    while (checkList.Any(c => c.Column == selectedCell.Column && c.Row == row + 1))
                    {
                        row++;
                        word.Cells.Add(checkList.Single(c => c.Column == selectedCell.Column && c.Row == row));
                    }

                    words.Add(word);
                }
            }

            return words;
        }

        public static int GetCubeJointsCreatedWithWord(List<CrusadexCell> cellList, List<CrusadexCell> inProgressWordCells)
        {
            var copyOfCellList = cellList.Select(c => new CrusadexCell(c.Column, c.Row, c.Selected)).ToList();
            foreach (var cell in inProgressWordCells)
            {
                var currentWordCell = copyOfCellList.Where(c => c.Row == cell.Row && c.Column == cell.Column).First();
                currentWordCell.Selected = true;
            }

            return GetCubeJointsInList(copyOfCellList, inProgressWordCells);
        }

        public static int GetCubeJointsInList(List<CrusadexCell> cellList, List<CrusadexCell> inProgressWordCells)
        {
            var cubeJoints = new List<(CrusadexCell TopLeft, CrusadexCell TopRight, CrusadexCell BottomLeft, CrusadexCell BottomRight)>();
            var checkList = cellList.Where(c => c.Selected).ToList();

            if (inProgressWordCells != null && inProgressWordCells.Any())
            {
                checkList = checkList.Where(c => inProgressWordCells.Any(k => k.Row == c.Row && k.Column == c.Column)).ToList();
            }

            foreach (var cell in checkList)
            {
                var leftCell = cellList.SingleOrDefault(c => c.Row == cell.Row && c.Column.ToColumnIndex() == cell.Column.ToColumnIndex() - 1 && c.Selected);
                var topLeftCell = cellList.SingleOrDefault(c => c.Row == cell.Row - 1 && c.Column.ToColumnIndex() == cell.Column.ToColumnIndex() - 1 && c.Selected);
                var topCell = cellList.SingleOrDefault(c => c.Row == cell.Row - 1 && c.Column == cell.Column && c.Selected);
                var topRightCell = cellList.SingleOrDefault(c => c.Row == cell.Row - 1 && c.Column.ToColumnIndex() == cell.Column.ToColumnIndex() + 1 && c.Selected);
                var rightCell = cellList.SingleOrDefault(c => c.Row == cell.Row && c.Column.ToColumnIndex() == cell.Column.ToColumnIndex() + 1 && c.Selected);
                var bottomRightCell = cellList.SingleOrDefault(c => c.Row == cell.Row + 1 && c.Column.ToColumnIndex() == cell.Column.ToColumnIndex() + 1 && c.Selected);
                var bottomCell = cellList.SingleOrDefault(c => c.Row == cell.Row + 1 && c.Column == cell.Column && c.Selected);
                var bottomLeftCell = cellList.SingleOrDefault(c => c.Row == cell.Row + 1 && c.Column.ToColumnIndex() == cell.Column.ToColumnIndex() - 1 && c.Selected);

                var topLeftCube = leftCell != null && topLeftCell != null && topCell != null;
                var topRightCube = topCell != null && topRightCell != null && rightCell != null;
                var bottomRightCube = rightCell != null && bottomRightCell != null && bottomCell != null;
                var bottomLeftCube = bottomCell != null && bottomLeftCell != null && leftCell != null;

                // always start from lower left and spiral clockwise
                if (topLeftCube)
                {
                    var cube = (leftCell, topLeftCell, topCell, cell);

                    if (!cubeJoints.Any(c => c == cube))
                    {
                        cubeJoints.Add(cube);
                    }
                }
                if (topRightCube)
                {
                    var cube = (cell, topCell, topRightCell, rightCell);

                    if (!cubeJoints.Any(c => c == cube))
                    {
                        cubeJoints.Add(cube);
                    }
                }
                if (bottomRightCube)
                {
                    var cube = (bottomCell, cell, rightCell, bottomRightCell);

                    if (!cubeJoints.Any(c => c == cube))
                    {
                        cubeJoints.Add(cube);
                    }
                }
                if (bottomLeftCube)
                {
                    var cube = (bottomLeftCell, leftCell, cell, bottomCell);

                    if (!cubeJoints.Any(c => c == cube))
                    {
                        cubeJoints.Add(cube);
                    }
                }
            }

            return cubeJoints.Count;
        }

        public static string GetHtmlStringTable(List<CrusadexCell> cellList)
        {
            var height = cellList.Max(s => s.Row);
            var resultHtml = new HtmlTag("table").Style("border-collapse", "collapse").Style("border", "1px solid #FF0000");

            for (var i = 1; i <= height; i++)
            {
                var row = resultHtml.Add("tr");
                foreach (var cell in cellList.Where(r => r.Row == i))
                {
                    var td = row.Add("td").Style("border", "1px solid #FF0000").Style("height", "30px").Style("width", "30px").Attr("bgcolor", "black");
                    if (cell.Selected)
                    {
                        td.Attr("bgcolor", "white");
                    }
                }
            }

            return resultHtml.ToHtmlString();
        }
    }
}
