using System;
using System.Windows.Forms;

namespace UserManager
{
    public class Program
    {
        private static AppInitializer _initializer;

        public static void Main(string[] args)
        {
            bool isStorageInit = false;
            try
            {
                _initializer = new AppInitializer();
                var initTuple = _initializer.Init();

                isStorageInit = initTuple.Item1;
                if (!isStorageInit)
                {
                    MessageBox.Show("Не удалось инициализировать БД карт", "Ошибка", MessageBoxButtons.OK);
                }
                else if (!initTuple.Item2)
                {
                    MessageBox.Show("Считыватель карт не найден", "Внимание", MessageBoxButtons.OK);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unhandled exception: " + ex.Message);
                Console.WriteLine("Stack trace:");
                Console.WriteLine(ex.StackTrace);
            }

            if (isStorageInit)
            {
                var main = _initializer.MainForm;
                Application.Run(main);
            }
        }
    }
}
