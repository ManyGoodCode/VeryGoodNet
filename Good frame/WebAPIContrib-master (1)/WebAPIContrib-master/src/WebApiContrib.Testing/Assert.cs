using System;
using System.Text;

namespace WebApiContrib.Testing
{
    internal static class Assert
    {
        public static void IsTrue(bool actual, string message = null)
        {
            if(actual)
                return;

            string errorMessage = BuildErrorMessage("Expected the value to be true", message);
            throw new AssertionException(errorMessage);
        }

        public static void IsNotNull(object actual, string message = null)
        {
            if(actual != null)
                return;

            string errorMessage = BuildErrorMessage("Expected the value to not be null", message);
            throw new AssertionException(errorMessage);
        }

        public static T InstanceOf<T>(object actual, string message = null)
            where T : class
        {
            T actualT = actual as T;
            if(actualT != null)
                return actualT;

            string expectedType = typeof(T).FullName;
            string actualType = actual == null ? "null" : actual.GetType().FullName;
            string errorMessage = BuildErrorMessage(expectedType, actualType, message);
            throw new AssertionException(errorMessage);
        }

        public static void AreEqual(object expected, object actual, string message = null)
        {
            if(Equals(actual, expected))
                return;

            string errorMessage = BuildErrorMessage(expected, actual, message);
            throw new AssertionException(errorMessage);
        }

        public static void AreSameString(string expected, string actual, string message = null)
        {
            if(string.Equals(expected, actual, StringComparison.OrdinalIgnoreCase))
                return;

            string errorMessage = BuildErrorMessage(expected, actual, message);
            throw new AssertionException(errorMessage);
        }

        private static string BuildErrorMessage(string assertionMessage, string message)
        {
            StringBuilder exceptionMessage = new StringBuilder();
            exceptionMessage.AppendLine(assertionMessage);
            if(message != null)
            {
                exceptionMessage.AppendLine(message);
            }

            return exceptionMessage.ToString();
        }

        private static string BuildErrorMessage(object expected, object actual, string message)
        {
            string actualValue = actual != null ? actual.ToString() : "null";
            string expectedValue = expected != null ? expected.ToString() : "null";

            StringBuilder exceptionMessage = new StringBuilder();
            exceptionMessage.AppendFormat("was {0} but expected {1}", actualValue, expectedValue);
            if(message != null)
            {
                exceptionMessage.AppendLine();
                exceptionMessage.AppendLine(message);
            }

            return exceptionMessage.ToString();
        }
    }
}