using FireBrowserMultiCore;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace FireBrowserFavorites
{
    public class FavManager
    {
        public void SaveFav(User user, string title, string url)
        {
            if (user is null) return;

            string username = user.Username; // Replace 'Username' with the actual property that holds the username in your User model.
            string favoritesFilePath = Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, username, "Database", "favorites.json");

            List<FavItem> favorites = File.Exists(favoritesFilePath)
                ? JsonSerializer.Deserialize<List<FavItem>>(File.ReadAllText(favoritesFilePath)) ?? new()
                : new();

            favorites.Add(new() { Title = title, Url = url });

            File.WriteAllText(favoritesFilePath, JsonSerializer.Serialize(favorites));
        }


        public List<FavItem> LoadFav(User user)
        {
            if (user is null) return new();

            string username = user.Username; // Replace 'Username' with the actual property that holds the username in your User model.
            string favoritesFilePath = Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, username, "Database", "favorites.json");

            if (File.Exists(favoritesFilePath))
            {
                string json = File.ReadAllText(favoritesFilePath);
                return JsonSerializer.Deserialize<List<FavItem>>(json) ?? new();
            }

            return new();
        }
    }
}