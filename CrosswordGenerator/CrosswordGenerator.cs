using CrosswordGenerator.Extensions;
using CrosswordGenerator.Helpers;
using System.Diagnostics;

namespace CrosswordGenerator
{
    public class CrosswordGenerator
    {
        private readonly CrosswordGeneratorOptions _options;
        private int TwoLetterWordsGenerated = 0;
        private int ThreeLetterWordsGenerated = 0;
        private int TotalCubeJointsGenerated = 0;

        public CrosswordGenerator(CrosswordGeneratorOptions options)
        {
            _options = options;
        }

        public List<CrosswordCell> Generate()
        {
            var cellList = new List<CrosswordCell>();
            for (int i = 1; i <= _options.Height; i++) { 
                for(int j = 1; j <= _options.Width; j++)
                {
                    cellList.Add(new CrosswordCell(j.ToColumnLabel(), i, false));
                }
            }
            var r = new Random();
            var numWords = r.Next(_options.MinWords, _options.MaxWords + 1);

            var createdWords = new List<DirectionalWord>();
            var lastWordCells = new DirectionalWord(true, new List<CrosswordCell>());
            var currentWordCells = new List<CrosswordCell>();

            do
            {
                var isVertical = lastWordCells.IsVertical;
                if (lastWordCells.Cells.Any())
                {
                    isVertical = !isVertical;
                }

                var cubeJointsCreatedForInProgressWord = 0;
                var inProgressWordCells = new List<CrosswordCell>();
                var generatedMoreWordsThanWeCanUse = false;
                var priorWordLengthsEqual = false;
                var attempts = 0;
                var flipped = 0;
                do
                {
                    attempts++;
                    CrosswordCell startingCell;

                    if(attempts > 1 && attempts >= createdWords.SelectMany(c => c.Cells).Count() * _options.MaxWordLength)
                    {
                        if(flipped == 1)
                        {
                            Debug.Print(CellListHelpers.GetHtmlStringTable(cellList, _options.Height));

                            throw new CrosswordGeneratorException($"Unable to generate puzzle with options shown after {attempts * 2} attempts. Succesfully generated {CellListHelpers.GetWordsFromCellList(cellList).Count}/{numWords}.",
                                _options);
                        }

                        isVertical = !isVertical;
                        flipped++;

                        Debug.Print($"Tried {attempts} times using {(!isVertical ? "vertical" : "horizontal")} starting words. Flipping to {(isVertical ? "vertical" : "horizontal")}");
                        attempts = 0;

                    }

                    if (createdWords.Any(c => c.IsVertical == !isVertical))
                    {
                        var startingWord = ChooseStartingWordFromDirection(!isVertical, createdWords);
                        startingCell = ChooseWordStartingCell(startingWord.Cells);
                    }
                    else
                    {
                        startingCell = ChooseWordStartingCell(cellList);
                    }

                    cubeJointsCreatedForInProgressWord = 0;
                    if (isVertical)
                    {
                        var randomStartingRow = GetRandomStartingRowOrColumn(
                            startingCell.Row, _options.MaxWordLength, _options.Height);

                        if (!randomStartingRow.HasValue) continue; // We hit upon a starting position that cannot generate any legal words


                        var randomLength = GetRandomLength(startingCell.Row, randomStartingRow.Value, _options.MinWordLength, _options.MaxWordLength);

                        //down
                        if (randomStartingRow < startingCell.Row)
                        {
                            inProgressWordCells = cellList.Where(cell => cell.Column == startingCell.Column && cell.Row >= randomStartingRow && cell.Row < randomStartingRow + randomLength)
                                                        .ToList();
                        }
                        //up
                        else
                        {
                            inProgressWordCells = cellList.Where(cell => cell.Column == startingCell.Column && cell.Row <= randomStartingRow && cell.Row > randomStartingRow - randomLength)
                                                        .ToList();
                        }
                    }
                    else
                    {
                        var randomStartingColumn = GetRandomStartingRowOrColumn(
                            startingCell.Column.ToColumnIndex(), _options.MaxWordLength, _options.Width);

                        if (!randomStartingColumn.HasValue) continue; // We hit upon a starting position that cannot generate any legal words

                        var randomLength = GetRandomLength(startingCell.Column.ToColumnIndex(), randomStartingColumn.Value, _options.MinWordLength, _options.MaxWordLength);

                        //right
                        if (randomStartingColumn < startingCell.Column.ToColumnIndex())
                        {
                            inProgressWordCells = cellList.Where(cell => cell.Row == startingCell.Row && cell.Column.ToColumnIndex() >= randomStartingColumn && cell.Column.ToColumnIndex() < randomStartingColumn + randomLength)
                                                        .ToList();
                        }
                        //left
                        else
                        {
                            inProgressWordCells = cellList.Where(cell => cell.Row == startingCell.Row && cell.Column.ToColumnIndex() <= randomStartingColumn && cell.Column.ToColumnIndex() > randomStartingColumn - randomLength)
                                                        .ToList();
                        }
                    }

                    cubeJointsCreatedForInProgressWord = CellListHelpers.GetCubeJointsCreatedWithWord(cellList, inProgressWordCells);

                    var priorCellList = cellList.Select(c => new CrosswordCell(c.Column, c.Row, c.Selected)).ToList();
                    var priorWordList = CellListHelpers.GetWordsFromCellList(priorCellList);
                    var tempCellList = cellList.Select(c => new CrosswordCell(c.Column, c.Row, c.Selected)).ToList();
                    foreach (var cell in inProgressWordCells)
                    {
                        var tempCurrentWordCell = tempCellList.Where(c => c.Row == cell.Row && c.Column == cell.Column).First();
                        tempCurrentWordCell.Selected = true;
                    }
                    var generatedWords = CellListHelpers.GetWordsFromCellList(tempCellList);
                    generatedMoreWordsThanWeCanUse = generatedWords.Count > numWords;

                    var j = priorWordList.Select(w => w.Cells.Count);
                    Debug.Print(CellListHelpers.GetHtmlStringTable(cellList, _options.Height));

                    var f = generatedWords.GetRange(0, priorWordList.Count).Select(w => w.Cells.Count);
                    Debug.Print(CellListHelpers.GetHtmlStringTable(tempCellList, _options.Height));

                    priorWordLengthsEqual = priorWordList.Select(w => w.Cells.Count).SequenceEqual(generatedWords.GetRange(0, priorWordList.Count).Select(w => w.Cells.Count));
                    Debug.Print($"Attempt {attempts}");
                } while (_options.MaxCubeJoints.HasValue && TotalCubeJointsGenerated + cubeJointsCreatedForInProgressWord > _options.MaxCubeJoints
                || cellList.Where(c => c.Selected && inProgressWordCells.Any(p => p.Row == c.Row && p.Column == c.Column)).Count() > 1
                || generatedMoreWordsThanWeCanUse
                || !priorWordLengthsEqual);

                TotalCubeJointsGenerated = TotalCubeJointsGenerated + cubeJointsCreatedForInProgressWord;

                foreach (var cell in inProgressWordCells)
                {
                    var currentWordCell = cellList.Where(c => c.Row == cell.Row && c.Column == cell.Column).First();
                    currentWordCell.Selected = true;
                }

                lastWordCells = new DirectionalWord(isVertical, inProgressWordCells);
                createdWords.Add(lastWordCells);
                Debug.Print($"Added {(isVertical ? "vertical" : "horizontal")} word: {string.Join(", ", lastWordCells.Cells.Select(w => $"[{w.Row},{w.Column}]"))}");
            } while (CellListHelpers.GetWordsFromCellList(cellList).Count < numWords);

            return cellList;
        }

        private int GetRandomLength(int startingColumnOrRow, int currentWordStartingColumnOrRow, int minLength, int maxLength)
        {
            var r = new Random();
            var distance = Math.Abs(startingColumnOrRow - currentWordStartingColumnOrRow);
            var result = r.Next(distance > minLength ? distance > maxLength ? maxLength : distance : minLength,
                                distance > maxLength ? maxLength : distance >= minLength ? distance : minLength);
            return result;
        }

        private int? GetRandomStartingRowOrColumn(int current, int maxDistanceFromCurrent, int maxDimension)
        {
            var excludes = new HashSet<int> { current };
            if(_options.MaxTwoLetterWords != null && _options.MaxTwoLetterWords == TwoLetterWordsGenerated)
            {
                excludes.Add(current + 1);
                excludes.Add(current - 1);
            }
            if (_options.MaxThreeLetterWords != null && _options.MaxThreeLetterWords == ThreeLetterWordsGenerated)
            {
                excludes.Add(current + 2);
                excludes.Add(current - 2);
            }

            var rangeStart = current - maxDistanceFromCurrent < 1 ? 1 : current - maxDistanceFromCurrent;
            var count = maxDistanceFromCurrent * 2;

            //does the count exceed the max dimension?
            if (rangeStart + maxDistanceFromCurrent * 2 > maxDimension)
            {
                count = maxDimension - rangeStart;
            }

            var range = Enumerable.Range(rangeStart, count - 1)
                .Where(i => !excludes.Contains(i));

            if (!range.Any())
            {
                return null;
            }

            var rand = new Random();
            int index = rand.Next(0, range.Count() - excludes.Count);
            return range.ElementAt(index);
        }

        private CrosswordCell ChooseWordStartingCell(List<CrosswordCell> input)
        {
            var r = new Random();
            var randomIndex = r.Next(input.Count);
            return input[randomIndex];
        }

        private DirectionalWord ChooseStartingWordFromDirection(bool isVertical, List<DirectionalWord> input)
        {
            var directionalList = input.Where(i => i.IsVertical == isVertical).ToList();

            var r = new Random();
            var randomIndex = r.Next(directionalList.Count());
            return directionalList[randomIndex];
        }

    }
}