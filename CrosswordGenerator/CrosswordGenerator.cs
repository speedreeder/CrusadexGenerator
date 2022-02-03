using CrosswordGenerator.Extensions;

namespace CrosswordGenerator
{
    public class CrosswordGenerator
    {
        private readonly CrosswordGeneratorOptions _options;
        private int TwoLetterWordsGenerated = 0;
        private int ThreeLetterWordsGenerated = 0;
        private int TotalWordsGenerated = 0;

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
                    cellList.Add(new CrosswordCell(j.ToColumnLabel(), i));
                }
            }
            var r = new Random();
            var numWords = r.Next(_options.MinWords, _options.MaxWords + 1);
            var boundaryColumns = new List<string>
            {
                "A", _options.Width.ToColumnLabel()
            };

            var boundaryRows = new List<int>
            {
                1, _options.Height
            };

            var lastWordCells = new List<CrosswordCell>();
            var currentWordCells = new List<CrosswordCell>();

            for (int i = 0; i < numWords; i++) {
                do
                {
                    var startingCell = ChooseWordStartingCell(lastWordCells.Any() ? lastWordCells : cellList);

                    var randomDirection = (DirectionEnum)r.Next(1, 5);

                    switch (randomDirection)
                    {
                        case DirectionEnum.Up:
                            if (startingCell.Row == boundaryRows[0])
                            {
                                randomDirection = DirectionEnum.Down;
                            }
                            break;
                        case DirectionEnum.Right:
                            if (startingCell.Column == boundaryColumns[1])
                            {
                                randomDirection = DirectionEnum.Left;
                            }
                            break;
                        case DirectionEnum.Down:
                            if (startingCell.Row == boundaryRows[1])
                            {
                                randomDirection = DirectionEnum.Up;
                            }
                            break;
                        case DirectionEnum.Left:
                            if (startingCell.Column == boundaryColumns[0])
                            {
                                randomDirection = DirectionEnum.Right;
                            }
                            break;

                    }

                    int maxLength;
                    int? randomLength;
                    switch (randomDirection)
                    {
                        case DirectionEnum.Up:
                            maxLength = startingCell.Row < _options.Height ? startingCell.Row : _options.Height;
                            randomLength = GetWordLength(2, maxLength + 1);
                            currentWordCells = cellList.Where(cell => cell.Column == startingCell.Column
                                                                      && cell.Row <= startingCell.Row
                                                                      && cell.Row > startingCell.Row - randomLength).ToList();
                            foreach (var cell in currentWordCells)
                            {
                                cell.Selected = true;
                            }
                            break;
                        case DirectionEnum.Right:
                            maxLength = _options.Width - startingCell.Column.ToColumnIndex() < _options.Width ? _options.Width - startingCell.Column.ToColumnIndex() : _options.Width;
                            randomLength = GetWordLength(2, maxLength + 1);
                            currentWordCells = cellList.Where(cell => cell.Row == startingCell.Row
                                                                      && cell.Column.ToColumnIndex() >= startingCell.Column.ToColumnIndex()
                                                                      && cell.Column.ToColumnIndex() <= cell.Column.ToColumnIndex() + randomLength).ToList();
                            foreach (var cell in currentWordCells)
                            {
                                cell.Selected = true;
                            }
                            break;
                        case DirectionEnum.Down:
                            maxLength = _options.Height - startingCell.Row < _options.Height ? _options.Height - startingCell.Row : _options.Height;
                            randomLength = GetWordLength(2, maxLength + 1);
                            currentWordCells = cellList.Where(cell => cell.Column == startingCell.Column
                                                                      && cell.Row >= startingCell.Row
                                                                      && cell.Row <= cell.Row + randomLength).ToList();
                            foreach (var cell in currentWordCells)
                            {
                                cell.Selected = true;
                            }
                            break;
                        case DirectionEnum.Left:
                            maxLength = startingCell.Column.ToColumnIndex() < _options.Width ? startingCell.Column.ToColumnIndex() : _options.Width;
                            randomLength = GetWordLength(2, maxLength + 1);
                            if (!randomLength.HasValue) break;
                            currentWordCells = cellList.Where(cell => cell.Row == startingCell.Row
                                                                      && cell.Column.ToColumnIndex() <= startingCell.Column.ToColumnIndex()
                                                                      && cell.Column.ToColumnIndex() > startingCell.Column.ToColumnIndex() - randomLength).ToList();
                            foreach (var cell in currentWordCells)
                            {
                                cell.Selected = true;
                            }
                            break;
                    }
                } while (!currentWordCells.Any());

                lastWordCells = currentWordCells;
            }

            return cellList;
        }

        private int? GetWordLength(int minlength, int maxLength)
        {
            var r = new Random();
            var result = r.Next(2, maxLength + 1);

            if(TwoLetterWordsGenerated == _options.MaxTwoLetterWords && ThreeLetterWordsGenerated == _options.MaxThreeLetterWords)
            {
                result = r.Next(4, maxLength + 1);
            }
            else if(TwoLetterWordsGenerated == _options.MaxTwoLetterWords && ThreeLetterWordsGenerated < _options.MaxThreeLetterWords)
            {
                result = r.Next(3, maxLength + 1);
            }
            else if (TwoLetterWordsGenerated < _options.MaxTwoLetterWords && ThreeLetterWordsGenerated == _options.MaxThreeLetterWords)
            {
                do
                {
                    result = r.Next(2, maxLength + 1);
                } while (result == 3);
            }
            else
            {
                result = r.Next(2, maxLength + 1);
            }

            return result;
        }

        private CrosswordCell ChooseWordStartingCell(List<CrosswordCell> input)
        {
            var r = new Random();
            var randomIndex = r.Next(input.Count);
            return input[randomIndex];
        }

    }
}