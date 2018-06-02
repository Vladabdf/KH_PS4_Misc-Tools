using System.Windows.Forms;

namespace GnfHelper
{
    public class IO
    {
        public static string[] OpenFileDialog()
        {
            using (OpenFileDialog opf = new OpenFileDialog())
            {
                opf.Multiselect = true;
                if (opf.ShowDialog() == DialogResult.OK)
                {
                    return opf.FileNames;
                }
                else return new string[0];
            }
        }

        public static string SaveFileDialog(string filename = "")
        {
            using (SaveFileDialog opf = new SaveFileDialog())
            {
                opf.FileName = filename;
                if (opf.ShowDialog() == DialogResult.OK)
                {
                    return opf.FileName;
                }
                else return string.Empty;
            }
        }

        public static string SaveFolder()
        {
            using (FolderBrowserDialog sfd = new FolderBrowserDialog())
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    return sfd.SelectedPath;
                }
                else return string.Empty;
            }
        }
    }
}
