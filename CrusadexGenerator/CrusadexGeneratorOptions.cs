namespace CrusadexGenerator
{
    public class CrusadexGeneratorOptions
    {
        public int MaxWords { get; set; }
        public int MinWords { get; set; } = 1;
        public int? MaxTwoLetterWords { get; set; }
        public int? MaxThreeLetterWords { get; set; }
        public int? MaxCubeJoints { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public int MinWordLength { get; set; } = 2;
        public int MaxWordLength { get; set; } = 7;
    }
}
