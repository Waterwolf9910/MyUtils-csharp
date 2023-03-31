using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyUtils.Extensions {
    public static class Strings {

        public static string ToUpperFirst(this string str) {
            if (str == null) {
                throw new ArgumentNullException(nameof(str));
            }

            char _char = str[0].ToString().ToUpper().ToCharArray()[0];
            
            return _char + str[1..];
        }
    }
}
