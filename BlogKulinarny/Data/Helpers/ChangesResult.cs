namespace BlogKulinarny.Data.Helpers
{
    public class ChangesResult
    {
        public ChangesResult(bool success, string errorMessage)
        {
            Success = success;
            ErrorMessage = errorMessage;
        }

        public bool Success { get; }
        public string ErrorMessage { get; }
    }
}
