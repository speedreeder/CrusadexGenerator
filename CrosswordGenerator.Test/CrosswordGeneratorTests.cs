using NUnit.Framework;

namespace CrosswordGenerator.Test
{
    public class CrosswordGeneratorTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Generate_ResultIsCorrectLength()
        {
            var height = 8;
            var width = 8;

            var sut = new CrosswordGenerator(new CrosswordGeneratorOptions { Height = height, Width = width});
            var result = sut.Generate();

            Assert.AreEqual(height*width, result.Count);
        }
    }
}