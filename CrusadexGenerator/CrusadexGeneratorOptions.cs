namespace CrusadexGenerator
{
    public class CrusadexGeneratorOptions
    {
        public int MaxWords { get; set; } = 5;
        public int MinWords { get; set; } = 1;
        public int? MaxTwoLetterWords { get; set; }
        public int? MaxThreeLetterWords { get; set; }
        public int? MaxCubeJoints { get; set; }
        public int Height { get; set; } = 10;
        public int Width { get; set; } = 10;
        public int MinWordLength { get; set; } = 2;
        public int MaxWordLength { get; set; } = 7;
    }
}
