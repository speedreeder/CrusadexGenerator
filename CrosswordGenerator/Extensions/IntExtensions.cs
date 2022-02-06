namespace CrosswordGenerator.Extensions
{
    internal static class IntExtensions
    {
        internal static string ToColumnLabel(this int col)
        {
            var dividend = col;
            var columnLabel = string.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnLabel = Convert.ToChar(65 + modulo).ToString() + columnLabel;
                dividend = (dividend - modulo) / 26;
            }

            return columnLabel;
        }
    }
}
