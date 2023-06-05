using ExifLibrary;
using Newtonsoft.Json;
using System.Text;
//using static System.Net.Mime.MediaTypeNames;

namespace UpdateMetainfo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var mainPath = args.Length > 0 ? args[0] : "";
            var missedFiles = new List<string>();
            foreach (var ext in new[] { "*.jpg", "*.jpeg", "*.png", "*.gif", "*.bmp", "*.mp4" })
            {
                var files = System.IO.Directory.GetFiles(mainPath, ext);
                for (int i = 0; i < files.Length; i++)
                {
                    var file = files[i];
                    var jsonfile = file + ".json";
                    if (!File.Exists(jsonfile))
                    {
                        missedFiles.Add(file);
                        continue;
                    }
                    try
                    {
                        var meta = JsonConvert.DeserializeObject<Metadata>(File.ReadAllText(jsonfile));
                        if (meta == null) throw new Exception("null time");
                        if (meta.PhotoTime < new DateTime(2012, 1, 1))
                            throw new ArgumentException("wrong time");
                        SetMetadata(file, meta.PhotoTime);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine();
                        Console.WriteLine(file);
                        Console.WriteLine(e);
                        Console.ReadKey();
                        return;
                    }
                    Progress(ext, 6, i + 1, files.Length, 50);
                }
                Console.WriteLine();
            }
            File.WriteAllLines("missedFiles.txt", missedFiles.ToArray());

            Console.WriteLine("missed files:");

            for (int i = 0; i < missedFiles.Count; i++)
            {
                var file = missedFiles[i];
                Console.WriteLine(file);
                Console.Write($"[{i.ToString().PadLeft(missedFiles.Count)}/{missedFiles.Count}] Date: ");
                var strDate = Console.ReadLine()?.Trim() ?? "";
                var split = strDate.Split('.').Select(p => int.Parse(p)).ToArray();
                var date = new DateTime(split[0], split[1], split[2], split[3], split[4], split[5]);
                SetJson(file, date);
                SetMetadata(file, date.ToUniversalTime());
            }
        }

        private static void SetJsonForAll(string path)
        {
            var existPath = Path.Combine(path, "Exist");
            var diffPath = Path.Combine(path, "Diff");

            var files = System.IO.Directory.GetFiles(path);
            for (int i = 0; i < files.Length; i++)
            {
                var file = files[i];
                SetJson(file, File.GetLastWriteTimeUtc(file));
                Progress("files", 6, i + 1, files.Length, 50);
            }
        }

        private static void SetJson(string file, DateTime date)
        {
            var dto = new DateTimeOffset(date, new TimeSpan());
            var timestamp = dto.ToUnixTimeSeconds();
            var metadata = new Metadata(new Metadata.TimeClass(timestamp));
            var jsonfile = file + ".json";
            File.WriteAllText(jsonfile, JsonConvert.SerializeObject(metadata));
        }

        private static void SetMetadata(string file, DateTime date)
        {
            var newDate = date.ToString("yyyy:MM:dd HH:mm:ss");

            var ifile = ImageFile.FromFile(file);

            ifile.Properties.Set(ExifTag.DateTime, newDate);
            if (ifile.Properties.Get(ExifTag.DateTimeDigitized) == null)
                ifile.Properties.Set(ExifTag.DateTimeDigitized, newDate);
            if (ifile.Properties.Get(ExifTag.DateTimeOriginal) == null)
                ifile.Properties.Set(ExifTag.DateTimeOriginal, newDate);
            ifile.Save(file);

            File.SetCreationTimeUtc(file, date);
            File.SetLastWriteTimeUtc(file, date);
        }

        static void Diff(string path)
        {
            var existPath = Path.Combine(path, "Exist");
            var diffPath = Path.Combine(path, "Diff");

            var files = System.IO.Directory.GetFiles(path);
            for (int i = 0; i < files.Length; i++)
            {
                var file = files[i];
                if (!File.Exists(Path.Combine(existPath, Path.GetFileName(file))))
                    File.Copy(file, Path.Combine(diffPath, Path.GetFileName(file)), true);
                Progress("files", 6, i + 1, files.Length, 50);
            }
        }

        static void Progress(string tag, int tagLength, int currentItem, int maxItems, int progLength)
        {
            var position = Console.GetCursorPosition();
            Console.SetCursorPosition(0, position.Top);
            var dots = currentItem * progLength / maxItems;
            Console.Write($"{tag.PadLeft(tagLength)}: [{Line('#', dots)}{Line('.', progLength - dots)}] {currentItem}/{maxItems}");
        }

        static string Line(char c, int length)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < length; i++)
                sb.Append(c);
            return sb.ToString();
        }
    }
}