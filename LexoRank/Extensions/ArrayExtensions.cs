namespace LexoAlgorithm.Extensions
{
    internal static class ArrayExtensions
    {
        public static void Fill<T>(this T[] originalArray, T with)
        {
            for (var i = 0; i < originalArray.Length; i++) originalArray[i] = with;
        }
    }
}