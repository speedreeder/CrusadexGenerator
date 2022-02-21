# CrusadexGenerator

Engine for generating clue-less crossword-style puzzle grids called [crusadex, cruzadex, or fill-in crosswords](https://en.wikipedia.org/wiki/Crossword#Fill-in_crosswords).

# Installing via NuGet

    Install-Package 
    
# How To Use

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
generator.Generate();
```
