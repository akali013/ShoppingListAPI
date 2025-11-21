using System.Globalization;

namespace ShoppingListAPI.Helpers
{
    // Custom exception class for throwing app specific exceptions like for validation
    // that can be caught and handled
    public class AppException : Exception
    {
        public AppException() : base() { }
        public AppException(string message) : base(message) { }
        public AppException(string message, params object[] args): base(String.Format(CultureInfo.CurrentCulture, message, args)) { }
    }
}
