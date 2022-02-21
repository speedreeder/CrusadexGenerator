namespace CrusadexGenerator.Extensions
{
    public static class StringExtensions
    {
        public static int ToColumnIndex(this string colLabel)
        {
            var colIndex = 0;
            for (int ind = 0, pow = colLabel.Count() - 1; ind < colLabel.Count(); ++ind, --pow)
            {
                var cVal = Convert.ToInt32(colLabel[ind]) - 64;
                colIndex += cVal * (int)Math.Pow(26, pow);
            }
            return colIndex;
        }
    }
}
