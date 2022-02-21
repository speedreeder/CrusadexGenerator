# CrusadexGenerator

Engine for generating clue-less crossword-style puzzle grids called [crusadex, cruzadex, or fill-in crosswords](https://en.wikipedia.org/wiki/Crossword#Fill-in_crosswords).

# Installing via NuGet

    Install-Package CrusadexGenerator
    
# How To Use

## Generation

This will give you a `List<CrusadexCell>`. Each `CrusadexCell` has three properties:
- **Column** (string) The column of the cell ("A", "B" ... "AA", "AB", etc.)
- **Row** (int) The row of the cell (1, 2 .... 99, 100, etc.)
- **Selected** (boolean) Whether or not this cell is selected in the puzzle

```csharp
var options = new CrusadexGeneratorOptions
{
    // The length of the y-axis
    Height = 10,    // default: 10

    // The length of the x-axis
    Width = 10,     // default: 10

    // The number of 2x2 squares in the puzzle
    // If null/unset no constraint will be used
    MaxCubeJoints = 2,  //default: null

    // Constraint for specific word lengths
    // If null/unset no constraint will be used
    MaxTwoLetterWords = 2,      // default: null
    MaxThreeLetterWords = 2,    // default: null

    // General word length constraints
    MaxWordLength = 7,  // default: 7
    MinWordLength = 2,  // default: 2

    // Constraints for the total number of words in the puzzle
    MaxWords = 5,   // default: 5
    MinWords = 2    // default: 2
};

var generator = new CrusadexGenerator(options);
var result = generator.Generate();
```

## Checking

You can use the `CrusadexCellListHelpers.GetHtmlStringTable(cellList)` class to check your output:

```csharp
var options = new CrusadexGeneratorOptions();
var generator = new CrusadexGenerator(options);
var cellList = generator.Generate();

var htmlOutput = CrusadexCellListHelpers.GetHtmlStringTable(cellList);
```

That will give you an HTML string that you can save as an HTML file or plug into any HTML visualizer. Here are some examples (using https://html.onlineviewer.net/):

![10x10](https://raw.githubusercontent.com/speedreeder/CrusadexGenerator/main/Examples/10x10.png)

![15x15](https://raw.githubusercontent.com/speedreeder/CrusadexGenerator/main/Examples/15x15.png)

![8x8](https://raw.githubusercontent.com/speedreeder/CrusadexGenerator/main/Examples/8x8cj.png)



