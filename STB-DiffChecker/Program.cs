using System;

//using StbCompareUI;
namespace STBDiffChecker
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            RunWpf();
        }

        static void RunWpf()
        {
            MainWindow mainWindowWPF = new MainWindow();
            if (mainWindowWPF.ShowDialog() != true)
            {
                return;
            }
        }
    }
}
