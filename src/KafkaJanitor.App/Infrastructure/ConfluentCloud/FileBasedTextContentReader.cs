using System.Text;

namespace KafkaJanitor.App.Infrastructure.ConfluentCloud;

public class FileBasedTextContentReader : ITextContentReader
{
    private readonly FileInfo _file;

    public FileBasedTextContentReader(FileInfo file)
    {
        _file = file;
    }

    public async Task<string[]> ReadAllLines()
    {
        if (!_file.Exists)
        {
            throw new Exception($"File \"{_file.FullName}\" does not exist!");
        }

        return await File.ReadAllLinesAsync(_file.FullName, Encoding.UTF8);
    }
}