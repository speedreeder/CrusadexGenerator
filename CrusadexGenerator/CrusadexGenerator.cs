using CrusadexGenerator.Extensions;
using CrusadexGenerator.Helpers;
using System.Diagnostics;

namespace CrusadexGenerator
{
    public class CrusadexGenerator
    {
        private readonly CrusadexGeneratorOptions _options;
        private int TwoLetterWordsGenerated = 0;
        private int ThreeLetterWordsGenerated = 0;
        private int TotalCubeJointsGenerated = 0;

        private readonly int TargetNumWords = 0;

        public CrusadexGenerator(CrusadexGeneratorOptions options)
        {
            _options = options;

            var r = new Random();
            TargetNumWords = r.Next(_options.MinWords, _options.MaxWords + 1);
        }

        public List<CrusadexCell> Generate()
        {
            var cellList = new List<CrusadexCell>();
            for (int i = 1; i <= _options.Height; i++)
            {
                for (int j = 1; j <= _options.Width; j++)
                {
                    cellList.Add(new CrusadexCell(j.ToColumnLabel(), i, false));
                }
            }

            var createdWords = new List<DirectionalWord>();
            var lastCreatedWord = new DirectionalWord(true, new List<CrusadexCell>());
            var currentWordCells = new List<CrusadexCell>();
            var attempts = 0;
            var flipped = 0;

            do
            {
                attempts++;

                var isVertical = lastCreatedWord.IsVertical;
                if (lastCreatedWord.Cells.Any())
                {
                    isVertical = !isVertical;
                }
                CrusadexCell startingCell;
                var cubeJointsCreatedForInProgressWord = 0;
                var inProgressWordCells = new List<CrusadexCell>();

                Debug.Print($"Attempt {attempts}.");

                if (attempts > 1 && attempts >= createdWords.Sum(c => c.Cells.Count) * _options.MaxWordLength * 2)
                {
                    if (flipped == 1)
                    {
                        var generatedGrid = CrusadexCellListHelpers.GetHtmlStringTable(cellList);
                        Debug.Print(generatedGrid);

                        throw new CrusadexGeneratorException($"Unable to generate puzzle with options shown after {attempts * 2} attempts. Succesfully generated {CrusadexCellListHelpers.GetWordsFromCellList(cellList).Count}/{TargetNumWords}.",
                            _options,
                            generatedGrid);
                    }

                    isVertical = !isVertical;
                    flipped++;

                    Debug.Print($"Tried {attempts} times using {(!isVertical ? "vertical" : "horizontal")} starting words. Flipping to {(isVertical ? "vertical" : "horizontal")}");
                    attempts = 0;
                }

                if (createdWords.Count == 0)
                {
                    startingCell = ChooseStartingCellFromList(cellList.Where(c => c.Row == Math.Floor(cellList.Select(r => r.Row).Average())
                                                                                  && c.Column.ToColumnIndex() == Math.Floor(cellList.Select(p => p.Column.ToColumnIndex())
                                                                                                                                    .Average())).ToList());
                }
                else
                {
                    startingCell = GetStartingCell(isVertical, createdWords, cellList);
                    if (startingCell == null) { continue; }
                }

                if (isVertical)
                {
                    var randomStartingRow = GetRandomStartingRowOrColumn(
                        startingCell.Row, _options.MaxWordLength, _options.Height);

                    if (!randomStartingRow.HasValue) { continue; }

                    var randomLength = GetRandomLength(startingCell.Row, randomStartingRow.Value, _options.MaxWordLength);

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

                    if (!randomStartingColumn.HasValue) { continue; }

                    var randomLength = GetRandomLength(startingCell.Column.ToColumnIndex(), randomStartingColumn.Value, _options.MaxWordLength);

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

                if (_options.MaxCubeJoints.HasValue)
                {
                    cubeJointsCreatedForInProgressWord = CrusadexCellListHelpers.GetCubeJointsCreatedWithWord(cellList, inProgressWordCells);
                    if (TotalCubeJointsGenerated + cubeJointsCreatedForInProgressWord > _options.MaxCubeJoints)
                    {
                        Debug.Print($"Created illegal number of cube joints: {TotalCubeJointsGenerated + cubeJointsCreatedForInProgressWord}/{_options.MaxCubeJoints}, " +
                        $"{string.Join(", ", inProgressWordCells.Select(w => $"[{w.Row},{w.Column}]"))}");
                        continue;
                    }
                }

                var priorCellList = cellList.Select(c => new CrusadexCell(c.Column, c.Row, c.Selected)).ToList();
                var priorWordList = CrusadexCellListHelpers.GetWordsFromCellList(priorCellList);
                var tempCellList = cellList.Select(c => new CrusadexCell(c.Column, c.Row, c.Selected)).ToList();
                foreach (var cell in inProgressWordCells)
                {
                    var tempCurrentWordCell = tempCellList.Where(c => c.Row == cell.Row && c.Column == cell.Column).First();
                    tempCurrentWordCell.Selected = true;
                }
                Debug.Print(CrusadexCellListHelpers.GetHtmlStringTable(cellList));
                Debug.Print(CrusadexCellListHelpers.GetHtmlStringTable(tempCellList));

                var generatedWords = CrusadexCellListHelpers.GetWordsFromCellList(tempCellList);
                if (generatedWords.Count > TargetNumWords)
                {
                    Debug.Print($"Generated more words than we can use: {generatedWords.Count}/{TargetNumWords}, " +
                        $"{string.Join(", ", inProgressWordCells.Select(w => $"[{w.Row},{w.Column}]"))}");
                    continue;
                }

                var priorWordLengths = priorWordList.GroupBy(p => p.Cells.Count).ToList();
                var generatedWordLengths = generatedWords.GroupBy(g => g.Cells.Count).ToList();

                if (generatedWordLengths.Any(g => g.Key > _options.MaxWordLength) || generatedWordLengths.Any(g => g.Key < _options.MinWordLength))
                {
                    Debug.Print($"Created words of illegal length. " +
                        $"{string.Join(", ", inProgressWordCells.Select(w => $"[{w.Row},{w.Column}]"))}");
                    continue;
                }

                TotalCubeJointsGenerated += cubeJointsCreatedForInProgressWord;

                foreach (var cell in inProgressWordCells)
                {
                    var currentWordCell = cellList.Where(c => c.Row == cell.Row && c.Column == cell.Column).First();
                    currentWordCell.Selected = true;
                }

                lastCreatedWord = new DirectionalWord(isVertical, inProgressWordCells)
                {
                    GeneratedIndex = createdWords.Count + 1
                };
                createdWords.Add(lastCreatedWord);
                Debug.Print($"{lastCreatedWord.GeneratedIndex}: Added {(isVertical ? "vertical" : "horizontal")} word: {string.Join(", ", lastCreatedWord.Cells.Select(w => $"[{w.Row},{w.Column}]"))}");
            } while (CrusadexCellListHelpers.GetWordsFromCellList(cellList).Count < TargetNumWords);

            return cellList;
        }

        private CrusadexCell GetStartingCell(bool isVertical, List<DirectionalWord> createdWords, List<CrusadexCell> cellList)
        {
            var startingWord = ChooseStartingWordFromDirection(!isVertical, createdWords);
            var startingCell = ChooseWordStartingCellFromDirectionalWord(startingWord, cellList);

            return startingCell;
        }

        private int GetRandomLength(int startingColumnOrRow, int currentWordStartingColumnOrRow, int maxLength)
        {
            var r = new Random();
            var distance = Math.Abs(startingColumnOrRow - currentWordStartingColumnOrRow) + 1;
            var result = r.Next(distance, maxLength);
            return result;
        }

        private int? GetRandomStartingRowOrColumn(int current, int maxDistanceFromCurrent, int maxDimension)
        {
            var excludes = new HashSet<int> { current };
            if (_options.MaxTwoLetterWords != null && _options.MaxTwoLetterWords == TwoLetterWordsGenerated)
            {
                excludes.Add(current + 1);
                excludes.Add(current - 1);
            }
            if (_options.MaxThreeLetterWords != null && _options.MaxThreeLetterWords == ThreeLetterWordsGenerated)
            {
                excludes.Add(current + 2);
                excludes.Add(current - 2);
            }

            var range = new HashSet<int>();

            var min = current - maxDistanceFromCurrent + 1 > 1 ? current - maxDistanceFromCurrent + 1 : 1;
            var max = current + maxDistanceFromCurrent - 1 < maxDimension ? current + maxDistanceFromCurrent - 1 : maxDimension;

            for (int i = min; i <= max; i++)
            {
                if (excludes.Contains(i)) { continue; }
                range.Add(i);
            }

            var rand = new Random();
            int index = rand.Next(0, range.Count);
            return range.ElementAt(index);
        }

        private CrusadexCell ChooseStartingCellFromList(List<CrusadexCell> cellList)
        {
            var r = new Random();
            var randomIndex = r.Next(cellList.Count);
            return cellList[randomIndex];
        }

        private CrusadexCell ChooseWordStartingCellFromDirectionalWord(DirectionalWord wordInput, List<CrusadexCell> cellList)
        {
            var range = wordInput.Cells.Select(c => c).ToList();
            var htmlOutput = CrusadexCellListHelpers.GetHtmlStringTable(cellList);

            foreach (var wordCell in wordInput.Cells)
            {
                if (!wordInput.IsVertical && cellList.Any(c => c.Column == wordCell.Column && (c.Row == wordCell.Row + 1 || c.Row == wordCell.Row - 1) && c.Selected))
                {
                    range.Remove(wordCell);
                }
                else if (wordInput.IsVertical && cellList.Any(c => c.Row == wordCell.Row && (c.Column.ToColumnIndex() == wordCell.Column.ToColumnIndex() + 1 || c.Column.ToColumnIndex() == wordCell.Column.ToColumnIndex() - 1) && c.Selected))
                {
                    range.Remove(wordCell);
                }
            }

            if (range.Count == 0) return null;

            var r = new Random();
            var randomIndex = r.Next(range.Count);
            return range[randomIndex];
        }

        private DirectionalWord ChooseStartingWordFromDirection(bool isVertical, List<DirectionalWord> input)
        {
            var directionalList = input.Where(i => i.IsVertical == isVertical).ToList();

            var r = new Random();
            var randomIndex = r.Next(directionalList.Count);
            return directionalList[randomIndex];
        }
    }
}
