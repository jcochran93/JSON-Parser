using System;
using System.IO;

class Program
{
    static void Main()
    {
        string filePath = "example.json"; // Replace with your file path

        try
        {
            // Read the entire file into a string using StreamReader
            using (StreamReader reader = new StreamReader(filePath))
            {
                string fileContents = reader.ReadToEnd();
                Console.WriteLine("File Contents:");
                // Console.WriteLine(fileContents);
                var json = new JsonDeserialized();

                var parsed = json.parse(fileContents);

                var dict = (Dictionary<string, object>)parsed;
                var locs = new Locations();
                foreach (var item in dict)
                {
                    if (item.Key == "Sources")
                    {
                        locs.Sources.AddRange(((List<object>)item.Value).OfType<string>());
                    }
                    if (item.Key == "Destinations")
                    {
                        locs.Destinations.AddRange(((List<object>)item.Value).OfType<string>());
                    }
                }
                locs.Sources.ForEach(s => Console.WriteLine(s));
                locs.Destinations.ForEach(s => Console.WriteLine(s));
            }
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("The specified file was not found.");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"An error occurred while reading the file: {ex.Message}");
        }
    }
}

public class Locations
{
    public List<string> Sources = new List<string>();
    public List<string> Destinations = new List<string>();
}

public class JsonDeserialized
{
    string originalJson;

    public object parse(string str)
    {
        var i = 0;
        Dictionary<string, object?> obj = new Dictionary<string, object?>();

        parseObject();

        return obj;

        void parseObject()
        {
            if (str[i] == '{')
            {
                i++;
                skipWhitespace();

                var initial = true;
                while (str[i] != '}')
                {
                    if (!initial)
                    {
                        skipComma();
                        skipWhitespace();
                    }
                    var key = parseString();
                    Console.WriteLine(key);

                    skipWhitespace();
                    skipColon();
                    skipWhitespace();
                    var value = parseValue();
                    skipWhitespace();
                    obj.Add(key, value);
                    initial = false;
                }
            }
            i++;
        }

        void skipWhitespace()
        {
            while (str[i] == ' ' || str[i] == '\t' || str[i] == '\n' || str[i] == '\r')
            {
                i++;
            }
        }
        void skipComma()
        {
            if (str[i] != ',')
            {
                throw new Exception("Expected , at " + i + ": str[i]= " + str[i]);
            }
            i++;
        }
        void skipColon()
        {
            if (str[i] != ':')
            {
                throw new Exception("Expected :");
            }
            i++;
        }
        string parseString()
        {
            if (str[i] != '"')
            {
                throw new Exception("Invalid string");
            }

            var val = "";

            i++;
            while (str[i] != '"')
            {
                val += str[i];
                i++;
            }
            i++;

            return val;
        }
        List<object> parseArray()
        {
            var arr = new List<object>();

            skipWhitespace();
            if (str[i] == ',')
            {
                skipComma();
            }
            skipWhitespace();
            while (str[i] != ']')
            {
                var value = parseValue();
                arr.Add(value);
                skipWhitespace();
                if (str[i] != ']')
                    skipComma();
            }
            i++;
            return arr;
        }
        object? parseValue()
        {
            object? value = null;

            if (str[i] == '"')
            {
                value = parseString();
                return value;
            }
            if (str[i] == '[')
            {
                i++;
                value = parseArray();
                return value;
            }

            return value;
        }
    }
}
