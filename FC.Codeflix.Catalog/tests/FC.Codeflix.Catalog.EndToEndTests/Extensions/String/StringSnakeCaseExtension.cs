using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FC.Codeflix.Catalog.EndToEndTests.Extensions.String
{
    public static class StringSnakeCaseExtension
    {
        private readonly static NamingStrategy _snakeCaseNamingStrategy = new SnakeCaseNamingStrategy();

        public static string ToSnakeCase(this string stringToConvert)
        {
            ArgumentNullException.ThrowIfNull(stringToConvert, nameof(stringToConvert));
            return _snakeCaseNamingStrategy.GetPropertyName(stringToConvert, false);
        }
    }
}
