using System.Windows.Forms;

namespace ShimEditor
{
    public class IO
    {
        public static string OpenFileDialog()
        {
            using (OpenFileDialog opf = new System.Windows.Forms.OpenFileDialog())
            {
                if (opf.ShowDialog() == DialogResult.OK)
                {
                    return opf.FileName;
                }
                else return string.Empty;
            }
        }

        public static string SaveFileDialog(string filename = "")
        {
            using (SaveFileDialog opf = new System.Windows.Forms.SaveFileDialog())
            {
                opf.FileName = filename;
                if (opf.ShowDialog() == DialogResult.OK)
                {
                    return opf.FileName;
                }
                else return string.Empty;
            }
        }
    }
}
