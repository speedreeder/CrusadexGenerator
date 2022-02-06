namespace CrosswordGenerator
{
    public class CrosswordGeneratorException : Exception
    {
        public CrosswordGeneratorException(string message, CrosswordGeneratorOptions options)
            : base($"{message} [Height {options.Height}][Width {options.Width}][Width {options.Width}][MinWordLength {options.MinWordLength}][MaxWordLength {options.MaxWordLength}][MaxCubeJoints {options.MaxCubeJoints}][MinWords {options.MinWords}][MaxWords {options.MaxWords}]")
        {
        }
    }
}
