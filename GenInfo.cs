using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ASCII_gen
{
    class GenInfo(int charWidth = 11, int charHeight = 24, string gradient = " .:!/r(l1Z4H9W8$@", int normalWidth = 44, int normalHeight = 24)
    {
        [JsonInclude]
        public int charWidth = charWidth;
        [JsonInclude]
        public int charHeight = charHeight;
        [JsonInclude]
        public string gradient = gradient;

        [JsonInclude]
        public int normalWidth = normalWidth;
        [JsonInclude]
        public int normalHeight = normalHeight;
    }
}
