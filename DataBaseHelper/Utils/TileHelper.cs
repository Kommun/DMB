using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.StartScreen;
using Shared.Model;

namespace Shared.Utils
{
    public static class TileHelper
    {
        /// <summary>
        /// Закрепить иконку на рабочем столе
        /// </summary>
        /// <returns></returns>
        public async static Task Pin(voin soldier)
        {
            var tileToUpdate = (await SecondaryTile.FindAllForPackageAsync()).FirstOrDefault(t => t.TileId == soldier.Id.ToString());

            if (tileToUpdate != null)
                await tileToUpdate.RequestDeleteAsync();

            var secondaryTile = new SecondaryTile()
            {
                TileId = soldier.Id.ToString(),
                DisplayName = $"{ soldier.name} - { soldier.TileTime}",
            Arguments = "args"
            };
            secondaryTile.VisualElements.Square150x150Logo = new Uri("ms-appx:///Assets/Logo.png");
            secondaryTile.VisualElements.ShowNameOnSquare150x150Logo = true;
            await secondaryTile.RequestCreateAsync();
        }

        /// <summary>
        /// Обновить иконки
        /// </summary>
        /// <returns></returns>
        public async static Task Update()
        {
            var tiles = await SecondaryTile.FindAllForPackageAsync();

            foreach (var tile in tiles)
                try
                {
                    var soldier = DataBaseHelper.Connection.Get<voin>(tile.TileId);
                    tile.DisplayName = $"{ soldier.name} - { soldier.TileTime}";
                    await tile.UpdateAsync();
                }
                catch { }
        }
    }
}
