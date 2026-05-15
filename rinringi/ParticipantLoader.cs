using System.Text;

namespace Rinringi;

static class ParticipantLoader
{
    public static List<Person> Load(string csvPath, OpenAIClient api)
    {
        var people = new List<Person>();

        using var reader = new StreamReader(csvPath, Encoding.UTF8);
        reader.ReadLine(); // ヘッダー行をスキップ

        while (!reader.EndOfStream)
        {
            string? line = reader.ReadLine();
            if (string.IsNullOrWhiteSpace(line)) continue;

            var fields = ParseCsvLine(line);
            if (fields.Count < 6) continue;

            people.Add(new Person(
                api,
                name:        fields[0],
                age:         int.Parse(fields[1]),
                background:  fields[2],
                speakingStyle: fields[3],
                tier:        int.Parse(fields[4]),
                isApprover:  bool.Parse(fields[5])
            ));
        }

        return people;
    }

    private static List<string> ParseCsvLine(string line)
    {
        var fields = new List<string>();
        var current = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    current.Append('"');
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                fields.Add(current.ToString());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }

        fields.Add(current.ToString());
        return fields;
    }
}
