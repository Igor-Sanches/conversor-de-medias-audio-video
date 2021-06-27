using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using SQLite.Net;
using System.Threading.Tasks;
//using SQLite.Net.Attributes;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using System.Threading;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace Conversor.Helper
{
    public class ListingSaved
    {
      //  [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public Symbol symbol { get; set; } 
        public string NomeArquivo { get; set; }
        public int Size { get; set; }
        public string Date { get; set; }
        public string Key { get; private set; }
        public string CaminhoSaved { get; set; }
        private static SemaphoreSlim gettingFileProperties = new SemaphoreSlim(1);

      }
}
