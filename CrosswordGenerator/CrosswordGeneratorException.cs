namespace CrosswordGenerator
{
    public class CrosswordGeneratorException : Exception
    {
        public CrosswordGeneratorException(string message, CrosswordGeneratorOptions options, string generatedGrid = "")
            : base($"{message} [Height {options.Height}][Width {options.Width}][Width {options.Width}][MinWordLength {options.MinWordLength}][MaxWordLength {options.MaxWordLength}][MaxCubeJoints {options.MaxCubeJoints}][MinWords {options.MinWords}][MaxWords {options.MaxWords}]" +
                  $"\r\n{generatedGrid}")
        {
        }
    }
}
