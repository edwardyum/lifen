using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Photo_File_Properties
{
    class Files
    {
        public static string open_file_dialog()
        {
            string filename = "";

            OpenFileDialog opfd = new OpenFileDialog();
            opfd.InitialDirectory = @"C:\\";

            if (opfd.ShowDialog() == DialogResult.OK)
                filename = opfd.FileName;

            if (filename == string.Empty & filename == null)
                MessageBox.Show("не удалось получить путь к файлу");

            return filename;
        }

        public static string [] open_file_dialog_multiple()
        {
            string [] filenames = new string [0];

            OpenFileDialog opfd = new OpenFileDialog();
            //opfd.InitialDirectory = @"C:\\";
            opfd.Multiselect = true;

            if (opfd.ShowDialog() == DialogResult.OK)
                filenames = opfd.FileNames;

            if (filenames == null)
                MessageBox.Show("не удалось получить путь к файлу");

            return filenames;
        }


    }
}
