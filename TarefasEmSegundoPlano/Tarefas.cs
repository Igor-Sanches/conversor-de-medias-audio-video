using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.UI.Notifications;

namespace TarefasEmSegundoPlano
{
    public sealed class Tarefas : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();
            StorageFolder lOCAL = ApplicationData.Current.LocalFolder;
            var pasta = await lOCAL.CreateFolderAsync("Tarefa", CreationCollisionOption.OpenIfExists);
            var arquivo = await pasta.CreateFileAsync("Tarefa", CreationCollisionOption.OpenIfExists);
            var folderws = await lOCAL.GetFolderAsync("Tarefa");
            var file2s = await folderws.GetFileAsync("Tarefa");

            String ThemeSettings = await FileIO.ReadTextAsync(file2s);

            ChamaTile310x150(ThemeSettings);
            deferral.Complete();

        }
    
        private void ChamaTile310x150(string message)
        {
            //TileWide310x150
            var tile = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150BlockAndText01);

            var tileAtributos = tile.GetElementsByTagName("text");
            tileAtributos[0].AppendChild(tile.CreateTextNode(message));
            var tileNotificar = new TileNotification(tile);

            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotificar);
           
        } 
    }
}
