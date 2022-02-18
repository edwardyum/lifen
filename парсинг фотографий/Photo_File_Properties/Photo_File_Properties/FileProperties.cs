using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Photo_File_Properties
{
    class FileProperties
    {
        // to do
        // проверить существование файлов в системе папок. вдруг я их уже добавил.
        // протестировать на разные даты, в том числе с нулями в значении

        // папок с датой съёмки может быть несколько. в одну дату моглипроизойти несколько событий, которые уже добавлены
        // в связи с этими создаём папку с названием "дата разобрать" и кладём туда фотографию

        // одно событие может занять несколько дат - это обрбатывается вручную

        // установить прогресс бар. поколичеству файлов по объёму. для изображений и для видео.
        


        string path_general_forled = "";    // путь к папке, содержащей все папки с фотографиями по событиям

        public FileProperties(string Path_general_forled)
        {
            path_general_forled = Path_general_forled;
        }


        public void execute()
        {
            copy();
            //delete();           
        }

        void copy()
        {
            // получаем список файлов
            string[] paths = Files.open_file_dialog_multiple();

            // пробегаемся по списку файлов
            for (int i = 0; i < paths.Length; i++)
            {
                string path = paths[i];
                FileInfo info = new FileInfo(path);
                DateTime date = info.LastWriteTime;

                // копируем файл соответствующую папку
                string path_destination = Path.Combine(path_general_forled, form_directory_name(date, "разобрать"));

                // если папки не существует, создаём её
                if (!Directory.Exists(path_destination))
                    Directory.CreateDirectory(path_destination);

                string file_name = Path.GetFileName(path);
                string new_path_to_file = Path.Combine(path_destination, file_name);

                File.Copy(path, new_path_to_file);
            }
        }

        void delete()
        {
            // проверяем существование всех файлов
            // по названию
            // по размеру

            // если все файлы задублированы, то удаляем файлы в источнике
        }

        string form_directory_name(DateTime date, string s)
        {
            //string dir = date.Year.ToString("g") + date.Month.ToString("g") + date.Day.ToString("g") + " " + s;
            string dir = date.ToString("yyyy.MM.dd") + " " + s;
            return dir;
        }

        FileInfo get_file_properties(string path)
        {
            FileInfo oFileInfo = new FileInfo(path);

            //if (oFileInfo != null || oFileInfo.Length == 0)
            //{
            //    MessageBox.Show("My File's Name: \"" + oFileInfo.Name + "\"");
            //    // For calculating the size of files it holds.
            //    MessageBox.Show("myFile total Size: " + oFileInfo.Length.ToString());
            //}

            return oFileInfo;
        }


    }
}
