using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSEditor.FSData
{
    public class Locale
    {
        public static readonly string EN = "en";
        public static readonly string DE = "de";
        public static readonly string FR = "fr";
        public static readonly string IT = "it";
        public static readonly string JP = "JP";
        public static readonly string ES = "su";
        public static readonly string UK = "uk";

        public static readonly string[] ALL_WITHOUT_UK = new string[] { EN, DE, FR, IT, JP, ES };
    }
}
