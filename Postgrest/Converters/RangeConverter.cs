﻿using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Postgrest.Converters
{
    public class RangeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return ParseIntRange(reader.Value.ToString());
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public static Range ParseIntRange(string value)
        {
            //int4range (0,1] , [123,4123], etc. etc.
            string pattern = @"^(\[|\()(\d+),(\d+)(\]|\))$";
            var matches = Regex.Matches(value, pattern);

            if (matches.Count > 0)
            {
                var groups = matches[0].Groups;
                bool isInclusiveLower = groups[1].Value == "[";
                bool isInclusiveUpper = groups[4].Value == "]";
                int value1 = int.Parse(groups[2].Value);
                int value2 = int.Parse(groups[3].Value);

                int start = isInclusiveLower ? value1 : value1 + 1;
                int count = isInclusiveUpper ? value2 : value2 - 1;

                // Edge-case, includes no points
                if (count < start)
                {
                    return new Range();
                }

                return new Range(start, count);
            }

            throw new Exception("Unknown Range format.");
        }
    }
}