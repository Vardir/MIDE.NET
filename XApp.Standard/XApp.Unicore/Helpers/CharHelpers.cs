namespace XApp.Helpers
{
    public static class CharHelpers
    {
        /// <summary>
        /// Generates a sequence of characters from the given on till the last specified
        /// </summary>
        /// <param name="c"></param>
        /// <param name="last"></param>
        /// <returns></returns>
        public static char[] To(this char c, char last)
        {
            var count = last - c + 1;
            if (count <= 0)
                return new[] { c };

            var array = new char[count];
            var start = c;
            for (int i = 0; i < count; i++)
            {
                array[i] = (char)(start + i);
            }
            
            return array;
        }
    }
}