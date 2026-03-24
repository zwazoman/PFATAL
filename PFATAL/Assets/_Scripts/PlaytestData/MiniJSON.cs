using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public static class MiniJSON
{
    public static object Deserialize(string json)
    {
        return Parser.Parse(json);
    }

    sealed class Parser : IDisposable
    {
        const string WORD_BREAK = "{}[],:\"";

        public static object Parse(string json)
        {
            using (var instance = new Parser(json))
            {
                return instance.ParseValue();
            }
        }

        private StringReader json;

        private Parser(string jsonString)
        {
            json = new StringReader(jsonString);
        }

        public void Dispose()
        {
            json.Dispose();
            json = null;
        }

        private object ParseValue()
        {
            SkipWhitespace();

            if (json.Peek() == '{')
                return ParseObject();
            if (json.Peek() == '[')
                return ParseArray();
            if (json.Peek() == '"')
                return ParseString();
            if (char.IsDigit((char)json.Peek()) || json.Peek() == '-')
                return ParseNumber();

            string word = ParseWord();

            switch (word)
            {
                case "true": return true;
                case "false": return false;
                case "null": return null;
            }

            return null;
        }

        private Dictionary<string, object> ParseObject()
        {
            var table = new Dictionary<string, object>();

            json.Read(); // {

            while (true)
            {
                SkipWhitespace();

                if (json.Peek() == '}')
                {
                    json.Read();
                    break;
                }

                string key = ParseString();

                SkipWhitespace();
                json.Read(); // :

                object value = ParseValue();
                table[key] = value;

                SkipWhitespace();

                if (json.Peek() == ',')
                    json.Read();
            }

            return table;
        }

        private List<object> ParseArray()
        {
            var array = new List<object>();

            json.Read(); // [

            while (true)
            {
                SkipWhitespace();

                if (json.Peek() == ']')
                {
                    json.Read();
                    break;
                }

                object value = ParseValue();
                array.Add(value);

                SkipWhitespace();

                if (json.Peek() == ',')
                    json.Read();
            }

            return array;
        }

        private string ParseString()
        {
            var s = new StringBuilder();
            json.Read(); // "

            while (true)
            {
                if (json.Peek() == '"')
                {
                    json.Read();
                    break;
                }

                s.Append((char)json.Read());
            }

            return s.ToString();
        }

        private object ParseNumber()
        {
            string number = ParseWord();

            try
            {
                if (number.Contains("."))
                    return double.Parse(number, System.Globalization.CultureInfo.InvariantCulture);

                return long.Parse(number, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch
            {
                return number;
            }
        }

        private string ParseWord()
        {
            var word = new StringBuilder();

            while (json.Peek() != -1 && !WORD_BREAK.Contains((char)json.Peek()) && !char.IsWhiteSpace((char)json.Peek()))
            {
                word.Append((char)json.Read());
            }

            return word.ToString();
        }

        private void SkipWhitespace()
        {
            while (json.Peek() != -1 && char.IsWhiteSpace((char)json.Peek()))
            {
                json.Read();
            }
        }
    }
}