using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;

namespace TwitterStream.Core
{
    /// <summary>
    /// Represents a static class combining common validation APIs
    /// </summary>
    public static class Validation
    {
        /// <summary>
        /// Ensures that the given parameter is not null
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown, if the given parameter is null</exception>
        /// <param name="parameter">The parameter to validate</param>
        public static void EnsureNotNull(object parameter)
        {
            EnsureNotNull(parameter, nameof(parameter));
        }

        /// <summary>
        /// Ensures that the given parameter is not null
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown, if the given parameter is null</exception>
        /// <param name="parameter">The parameter to validate</param>
        /// <param name="name">The name of the validating parameter, to put in the exception</param>
        public static void EnsureNotNull(object parameter, string name)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(name);
            }
        }

        /// <summary>
        /// Ensures that the given string parameter is not null or empty
        /// </summary>
        /// <param name="parameter">The parameter to validate</param>
        public static void EnsureNotNullOrEmpty(string parameter)
        {
            EnsureNotNullOrEmpty(parameter, nameof(parameter));
        }

        /// <summary>
        /// Ensures that the given string parameter is not null or empty
        /// </summary>
        /// <param name="parameter">The parameter to validate</param>
        /// <param name="name">The name of the validating parameter, to put in the exception</param>
        public static void EnsureNotNullOrEmpty(string parameter, string name)
        {
            if (string.IsNullOrEmpty(parameter))
            {
                throw new ArgumentException("Parameter cannot be null or empty", name);
            }
        }

        /// <summary>
        /// Ensures that the given string parameter is not null, empty, or white space.
        /// </summary>
        /// <param name="parameter">The parameter to validate</param>
        public static void EnsureNotNullOrWhiteSpace(string parameter)
        {
            EnsureNotNullOrWhiteSpace(parameter, nameof(parameter));
        }

        /// <summary>
        /// Ensures that the given string parameter is not null, empty, or white space.
        /// </summary>
        /// <param name="parameter">The parameter to validate</param>
        /// <param name="name">The name of the validating parameter, to put in the exception</param>
        public static void EnsureNotNullOrWhiteSpace(string parameter, string name)
        {
            if (string.IsNullOrWhiteSpace(parameter))
            {
                throw new ArgumentException("Parameter cannot be null, empty, or white space.", name);
            }
        }

        /// <summary>
        /// Ensures that the given string parameter is valid Guid
        /// </summary>
        /// <param name="parameter">The parameter to validate</param>
        public static void EnsureValidGuidString(string parameter)
        {
            EnsureValidGuidString(parameter, nameof(parameter));
        }

        /// <summary>
        /// Ensures that the given string parameter is valid Guid
        /// </summary>
        /// <param name="parameter">The parameter to validate</param>
        /// <param name="name">The name of the validating parameter, to put in the exception</param>
        public static void EnsureValidGuidString(string parameter, string name)
        {
            if (Guid.TryParse(parameter, out Guid guid) == false)
            {
                throw new ArgumentException("Parameter is not valid Guid.", name);
            }
        }

        /// <summary>
        /// Ensures that the given string is a valid build name in the format {major}.{minor}.{branch}.{timestamp}
        /// </summary>
        /// <param name="buildName">The build name to validate</param>
        public static void EnsureValidBuildName(string buildName)
        {
            EnsureNotNullOrWhiteSpace(buildName, nameof(buildName));

            var regex = new Regex(@"^(\d+\.){2}[\d\w]+\.[\d-]+$");

            if (!regex.IsMatch(buildName))
            {
                throw new ArgumentException($"{buildName} is not a valid build name.");
            }
        }

        /// <summary>
        /// Ensures that the given int parameter is non zero value.
        /// </summary>
        /// <param name="parameter">The parameter to validate</param>
        public static void EnsureGreaterThanZeroValue(int parameter)
        {
            EnsureGreaterThanZeroValue(parameter, nameof(parameter));
        }

        /// <summary>
        /// Ensures that the given int parameter is non zero value.
        /// </summary>
        /// <param name="parameter">The parameter to validate</param>
        /// <param name="name">The name of the validating parameter, to put in the exception</param>
        public static void EnsureGreaterThanZeroValue(int parameter, string name)
        {
            if (parameter <= 0)
            {
                throw new ArgumentException(string.Format("Parameter {1} = {0}.  Cannot be Zero or less", parameter, name));
            }
        }

        /// <summary>
        /// Ensures that the given directory exists. If the directory doesn't exist, the method throws <see cref="DirectoryNotFoundException"/>.
        /// </summary>
        /// <param name="path">The path to the directory.</param>
        /// <exception cref="DirectoryNotFoundException">Thrown, if the directory doesn't exist.</exception>
        public static void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException(string.Format(CultureInfo.InvariantCulture, "Directory '{0}' does not exist.", path));
            }
        }

        /// <summary>
        /// Ensures that the given file exists. If the file doesn't exist. the method throws <see cref="FileNotFoundException"/>.
        /// </summary>
        /// <param name="path">File Path</param>
        /// <exception cref="FileNotFoundException">Thrown, if the file doesn't exist.</exception>
        public static void EnsureFileExists(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(string.Format(CultureInfo.InvariantCulture, "File '{0}' does not exist.", Path.IsPathRooted(path) ? path : Path.GetFullPath(path)));
            }
        }

        /// <summary>
        /// Print out information about the exception and recursively print out the inner exceptions.
        /// </summary>
        /// <param name="trace">The source used to output diagnostic information provided by the exception.</param>
        /// <param name="ex">An exception to be traced.</param>
        /// <param name="message">Optional message information to be printed along with the exception.</param>
        public static void TraceException(TraceSource trace, Exception ex, string message = null)
        {
            Validation.EnsureNotNull(ex, "ex");
            Validation.EnsureNotNull(trace, "trace");

            if (!string.IsNullOrWhiteSpace(message))
            {
                trace.TraceEvent(TraceEventType.Error, 0, message);
            }

            trace.TraceEvent(TraceEventType.Error, 0, ex.Message);
            trace.TraceEvent(TraceEventType.Verbose, 0, ex.StackTrace);

            AggregateException aggregateException = ex as AggregateException;
            if (aggregateException != null)
            {
                foreach (Exception e in aggregateException.InnerExceptions)
                {
                    TraceException(trace, e);
                }
            }
            else if (ex.InnerException != null)
            {
                TraceException(trace, ex.InnerException);
            }
        }

        /// <summary>
        /// Verifies that the specified enumerable is not empty.
        /// </summary>
        /// <param name="enumerable">The enumerable to be verified.</param>
        /// <param name="paramName">The name of the enumerable parameter in the calling method.</param>
        /// <exception cref="ArgumentException">Thrown, if the enumeration contains no items.</exception>
        /// <exception cref="ArgumentNullException">Thrown, if the enumerable is null.</exception>
        public static void EnsureNonEmptyEnumeration(IEnumerable enumerable, string paramName)
        {
            Validation.EnsureNotNull(enumerable, paramName);

            bool isEmpty = true;
            foreach (var item in enumerable)
            {
                isEmpty = false;
                break;
            }

            if (isEmpty)
            {
                throw new ArgumentException("Enumerable is empty", paramName);
            }
        }
    }
}