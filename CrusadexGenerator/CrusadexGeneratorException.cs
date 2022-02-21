namespace CrusadexGenerator
{
    public class CrusadexGeneratorException : Exception
    {
        public CrusadexGeneratorException(string message, CrusadexGeneratorOptions options, string generatedGrid = "")
            : base($"{message} [Height {options.Height}][Width {options.Width}][MinWordLength {options.MinWordLength}][MaxWordLength {options.MaxWordLength}][MaxCubeJoints {options.MaxCubeJoints}][MinWords {options.MinWords}][MaxWords {options.MaxWords}]" +
                  $"\r\n{generatedGrid}")
        {
        }
    }
}
