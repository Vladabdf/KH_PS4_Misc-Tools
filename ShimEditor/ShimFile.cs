using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace ShimEditor
{
    public class ShimFile
    {
        public ObservableCollection<ShimEntry> ShimEntries { get; set; }
        public byte[] BaseFile { get; set; }
        public string FileName { get; }
        private string ExtractDir { get; }
        private string FullPath { get; }

        public ShimFile(string file)
        {
            FullPath = file;
            FileName = Path.GetFileName(file).Replace("\\", "_").Replace("/", "_");
            ExtractDir = Path.Combine(Path.GetTempPath(), "ShimEditor", FileName);
            ShimEntries = new ObservableCollection<ShimEntry>();
            using (BinaryReader reader = new BinaryReader(new FileStream(file, FileMode.Open)))
            {
                //I don't have any option to check if this is an correct file.
                //I'm trusting the user on this one.

                reader.BaseStream.Position = 4;
                var files = reader.ReadInt32();
                for (int i = 0; i < files; i++)
                {
                    ShimEntry shim = new ShimEntry();
                    reader.BaseStream.Position = 0x10 + i * 0x30;
                    shim.Name = Encoding.UTF8.GetString(reader.ReadBytes(0x20)).Replace("\0", string.Empty)/*.Replace("\\", "_").Replace("/", "_")*/;
                    shim.Offset = reader.ReadInt32();
                    shim.UnknownButImportant = reader.ReadInt32();
                    shim.Size = reader.ReadInt32();

                    reader.BaseStream.Position = shim.Offset;
                    shim.Data = reader.ReadBytes(shim.Size);
                    ShimEntries.Add(shim);

                    //while we're at the first shim entry, let's grab the base file
                    if (i == 0)
                    {
                        var shimHeaderLen = 0x10 + files * 0x30;
                        reader.BaseStream.Position = shimHeaderLen;
                        BaseFile = reader.ReadBytes(shim.Offset - shimHeaderLen);
                    }
                }
            }
        }

        public void ExtractAll()
        {
            if (!Directory.Exists(ExtractDir))
                Directory.CreateDirectory(ExtractDir);
            else
                ClearDirectory(ExtractDir);

            foreach (var shim in ShimEntries)
            {
                Extract(shim, Path.Combine(ExtractDir, shim.SafeFileName));
            }
        }
        public void Extract(ShimEntry entry, string path)
        {
            File.WriteAllBytes(path, entry.Data);
        }

        private void ClearDirectory(string dir)
        {
            var info = new DirectoryInfo(dir);
            foreach (var file in info.GetFiles())
            {
                file.Delete();
            }
        }

        public void Save(string file)
        {
            List<byte> shimFilesStored = new List<byte>();
            var header = new byte[(0x30 * ShimEntries.Count) + 0x10];
            using (BinaryWriter writer = new BinaryWriter(new MemoryStream(header)))
            {
                writer.Write(BaseFile.Length);
                writer.Write(ShimEntries.Count);
                writer.BaseStream.Position = 0x10;
                for (int i = 0; i < ShimEntries.Count; i++)
                {
                    shimFilesStored.AddRange(ShimEntries[i].Data);

                    writer.BaseStream.Position = 0x10 + i * 0x30;
                    writer.Write(Encoding.ASCII.GetBytes(ShimEntries[i].Name));

                    writer.Seek(0x20 - (ShimEntries[i].Name.Length), SeekOrigin.Current);

                    writer.Write((int)ShimEntries[i].Offset);
                    writer.Write((int)ShimEntries[i].UnknownButImportant);
                    writer.Write((int)ShimEntries[i].Size);
                }
            }

            List<byte> finishedFile = new List<byte>();
            finishedFile.AddRange(header);
            finishedFile.AddRange(BaseFile);
            finishedFile.AddRange(shimFilesStored);

            File.WriteAllBytes(file, finishedFile.ToArray());
        }
        public void ReplaceEntry(ShimEntry oldEntry, ShimEntry newEntry)
        {
            int index = ShimEntries.IndexOf(oldEntry);
            if (index != -1)
            {
                newEntry.Offset = oldEntry.Offset;
                newEntry.UnknownButImportant = oldEntry.UnknownButImportant;
                ShimEntries[index] = newEntry;
                RebuildOffsets();
            }
        }
        public void DeleteEntry(ShimEntry entry)
        {
            int index = ShimEntries.IndexOf(entry);
            if (index != -1)
            {
                ShimEntries.Remove(entry);
                RebuildOffsets();
            }
        }
        public void ReplaceBase(string path)
        {
            BaseFile = File.ReadAllBytes(path);
            RebuildOffsets();
        }
        private void RebuildOffsets()
        {
            var offset = 0x10 + (0x30 * ShimEntries.Count) + BaseFile.Length;
            foreach(var entry in ShimEntries)
            {
                entry.Offset = offset;
                offset += entry.Size;
            }
        }
        public void AddEntry(ShimEntry en)
        {
            ShimEntries.Add(en);
            RebuildOffsets();
        }
    }

    public class ShimEntry
    {
        public string Name { get; set; }
        public int Offset { get; set; }
        public int Size { get; set; }
        public byte[] Data { get; set; }
        public string SafeFileName
        {
            get
            {
                return Name.Replace("\\", "_").Replace("/", "_");
            }
        }
        public int UnknownButImportant { get; set; }

        public ShimEntry() { }

        public ShimEntry(string filepath, string nameRep = "")
        {
            Data = File.ReadAllBytes(filepath);
            Size = Data.Length;
            if (!string.IsNullOrEmpty(nameRep))
                Name = nameRep;
            else
                Name = "NEWENTRY#";
        }
    }
    public enum EntryType
    {
        GNF, SCD
    }
}
