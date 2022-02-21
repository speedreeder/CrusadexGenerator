using CrosswordGenerator.Helpers;
using NUnit.Framework;
using System.Diagnostics;

namespace CrosswordGenerator.Test
{
    public class CrosswordGeneratorTests
    {
        [Test, Repeat(20)]
        [TestCase(4, 4)]
        [TestCase(8, 8)]
        [TestCase(4, 8)]
        [TestCase(8, 4)]
        [TestCase(10, 10)]
        [TestCase(10, 20)]
        [TestCase(20, 10)]
        public void Generate_ResultGridHasCorrectNumberOfCells(int height, int width)
        {
            var sut = new CrosswordGenerator(new CrosswordGeneratorOptions { Height = height, Width = width});
            var result = sut.Generate();

            var htmlOutput = CellListHelpers.GetHtmlStringTable(result, height);
            Debug.Print(htmlOutput);

            Assert.AreEqual(height*width, result.Count, htmlOutput);
        }

        [Test, Repeat(20)]
        [TestCase(6, 4, 8, 8, 2)]
        [TestCase(8, 5, 8, 8, 2)] //sometimes impossible
        [TestCase(10, 6, 8, 8, 2)] //sometimes impossible
        [TestCase(10, 7, 12, 12, 4)]
        [TestCase(10, 8, 15, 15, 6)]
        public void Generate_ResultGridHasCorrectNumberOfCubeJoints(int numberOfWords, int maxWordLength, int height, int width, int maxCubeJoints)
        {
            var sut = new CrosswordGenerator(new CrosswordGeneratorOptions
            {
                Height = height,
                Width = width,
                MinWords = numberOfWords,
                MaxWords = numberOfWords,
                MaxCubeJoints = maxCubeJoints,
                MaxWordLength = maxWordLength
            });

            var success = false;

            do
            {
                try
                {
                    var result = sut.Generate();
                    success = true;

                    var numCubeJoints = CellListHelpers.GetCubeJointsInList(result, null);
                    var htmlOutput = CellListHelpers.GetHtmlStringTable(result, height);
                    Debug.Print(htmlOutput);

                    Assert.IsTrue(numCubeJoints <= maxCubeJoints, htmlOutput);
                }
                catch (CrosswordGeneratorException ex)
                {
                    Debug.Print(ex.Message);
                }
            } while(!success);
        }

        [Test]
        [TestCase(10, 4, 6, 6, 2)]
        [TestCase(15, 4, 8, 8, 1)]
        public void Generate_ThrowCrosswordGeneratorException(int numberOfWords, int maxWordLength, int height, int width, int maxCubeJoints)
        {
            Assert.Throws<CrosswordGeneratorException>(() =>
            {
                var sut = new CrosswordGenerator(new CrosswordGeneratorOptions
                {
                    Height = height,
                    Width = width,
                    MinWords = numberOfWords,
                    MaxWords = numberOfWords,
                    MaxCubeJoints = maxCubeJoints,
                    MaxWordLength = maxWordLength
                });
                var result = sut.Generate();

                var numCubeJoints = CellListHelpers.GetCubeJointsInList(result, null);
                var htmlOutput = CellListHelpers.GetHtmlStringTable(result, height);
                Debug.Print(htmlOutput);
            });
        }

        [Test, Repeat(20)]
        [TestCase(2, 2)]
        [TestCase(2, 4)]
        [TestCase(8, 8)]
        [TestCase(10, 10)]
        [TestCase(12, 12)]
        public void Generate_ResultGridHasCorrectNumberOfWords(int minNumberOfWords, int maxNumberOfWords)
        {
            var height = 20;
            var width = 20;

            var sut = new CrosswordGenerator(new CrosswordGeneratorOptions { Height = height, Width = width, MinWords = minNumberOfWords, MaxWords = maxNumberOfWords });
            var result = sut.Generate();

            var htmlOutput = CellListHelpers.GetHtmlStringTable(result, height);
            Debug.Print(htmlOutput);
            var wordsInResult = CellListHelpers.GetWordsFromCellList(result);

            Assert.GreaterOrEqual(wordsInResult.Count, minNumberOfWords, htmlOutput);
            Assert.LessOrEqual(wordsInResult.Count, maxNumberOfWords, htmlOutput);
        }

        [Test, Repeat(20)]
        [TestCase(2, 2)]
        [TestCase(2, 4)]
        [TestCase(2, 8)]
        [TestCase(4, 8)]
        [TestCase(5, 10)]
        public void Generate_ResultWordsAreCorrectLength(int minWordLength, int maxWordLength)
        {
            var height = 15;
            var width = 15;

            var success = false;
            do
            {
                try
                {
                    var sut = new CrosswordGenerator(new CrosswordGeneratorOptions { 
                        Height = height, 
                        Width = width,
                        MinWords = 15, 
                        MaxWords = 15, 
                        MinWordLength = minWordLength,
                        MaxWordLength = maxWordLength,
                        MaxCubeJoints = 1 
                    });
                    var result = sut.Generate();
                    success = true;

                    var htmlOutput = CellListHelpers.GetHtmlStringTable(result, height);
                    Debug.Print(htmlOutput);
                    var wordsInResult = CellListHelpers.GetWordsFromCellList(result);

                    Assert.IsTrue(wordsInResult.TrueForAll(w => w.Cells.Count >= minWordLength), htmlOutput);
                    Assert.IsTrue(wordsInResult.TrueForAll(w => w.Cells.Count <= maxWordLength), htmlOutput);
                } catch (CrosswordGeneratorException ex)
                {
                    Debug.Print(ex.Message);
                }
            } while (!success);
            
        }
    }
}