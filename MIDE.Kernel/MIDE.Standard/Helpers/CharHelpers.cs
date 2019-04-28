namespace MIDE.Helpers
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
            int count = last - c;
            if (count <= 0)
                return new[] { c };
            char[] array = new char[count];
            int start = c;
            for (int i = 0; i < count + 1; i++)
            {
                array[i] = (char)(start + i);
            }
            return array;
        }
    }
}