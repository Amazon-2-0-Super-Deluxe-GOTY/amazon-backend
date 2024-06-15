
namespace amazon_backend.Services.Random
{
    public class RandomService : IRandomService
    {
        private readonly System.Random random = new System.Random();
        private readonly String symbolsDataSet = "abcdefghijklmnopqrstuvwxyz0123456789";
        private readonly String _safeChars = new String(
            Enumerable.Range(20, 107).Select(x => (char)x).ToArray());
        
        public string RandomNumberUseDate()
        {
            return $"{DateTime.Now.Year}{DateTime.Now.Month}{DateTime.Now.Day}{random.Next(1000, 9999)}";
        }
        public string ConfirmCode(int length)
        {
            string result = "";

            for (int i = 0; i < length; i++)
            {
                int index = random.Next(symbolsDataSet.Length);
                result += symbolsDataSet[index];
            }
            return result;
        }
        public string RandomString(int length)
        {
            char[] chars = new char[length];
            for (int i = 0; i < length; i++)
                chars[i] = _safeChars[random.Next(_safeChars.Length)];
            return new string(chars);
        }
    }
}
