using System.IO;
using System.Text;

namespace ShimEx
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                foreach (var file in args)
                {
                    Extract(file);
                }
            }
        }

        static void Extract(string file)
        {
            using (BinaryReader reader = new BinaryReader(new FileStream(file, FileMode.Open)))
            {
                //I don't have any option to check if this is an correct file.
                //I'm trusting the user on this one.

                reader.BaseStream.Position = 4;
                var files = reader.ReadInt32();

                var saved = Path.Combine(Path.GetDirectoryName(file), Path.GetFileName(file.Replace(".", "_")));

                Directory.CreateDirectory(saved);

                for (int i = 0; i < files; i++)
                {
                    reader.BaseStream.Position = 0x10 + i * 0x30;
                    var name = Encoding.UTF8.GetString(reader.ReadBytes(0x20)).Replace("\0", string.Empty).Replace("\\", "_").Replace("/", "_");
                    var offset = reader.ReadInt32();
                    var unknown = reader.ReadInt32();
                    var size = reader.ReadInt32();

                    reader.BaseStream.Position = offset;
                    var shimFile = reader.ReadBytes(size);
                    File.WriteAllBytes(Path.Combine(saved, name), shimFile);
                }
            }
        }
    }
}
