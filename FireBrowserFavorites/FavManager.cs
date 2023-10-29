using FireBrowserMultiCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace FireBrowserFavorites;
public class FavManager
{
    public void SaveFav(User user, string title, string url)
    {
        if (user != null)
        {
            string username = user.Username; // Replace 'Username' with the actual property that holds the username in your User model.
            string userFolderPath = Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, username);
            string databaseFolderPath = Path.Combine(userFolderPath, "Database");
            string favoritesFilePath = Path.Combine(databaseFolderPath, "favorites.json");

            // Initialize the favorites list or load existing favorites
            List<FavItem> favorites;
            if (File.Exists(favoritesFilePath))
            {
                string json = File.ReadAllText(favoritesFilePath);
                favorites = JsonConvert.DeserializeObject<List<FavItem>>(json);
            }
            else
            {
                favorites = new List<FavItem>();
            }

            // Create a new favorite item and add it to the list
            FavItem newFavorite = new FavItem { Title = title, Url = url };
            favorites.Add(newFavorite);

            // Serialize and save the updated favorites list to a JSON file
            string jsonFavorites = JsonConvert.SerializeObject(favorites);
            File.WriteAllText(favoritesFilePath, jsonFavorites);
        }
        else
        {
            // Handle the case where there is no authenticated user (user is null)
            // You may want to log an error or display a message to the user.
        }
    }


    public List<FavItem> LoadFav(User user)
    {
        if (user == null)
        {
            // Handle the case where there is no authenticated user (user is null)
            // You may want to log an error or display a message to the user.
            return new List<FavItem>();
        }

        string username = user.Username; // Replace 'Username' with the actual property that holds the username in your User model.
        string userFolderPath = Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, username);
        string databaseFolderPath = Path.Combine(userFolderPath, "Database");
        string favoritesFilePath = Path.Combine(databaseFolderPath, "favorites.json");

        List<FavItem> favorites = new List<FavItem>();

        if (File.Exists(favoritesFilePath))
        {
            string json = File.ReadAllText(favoritesFilePath);
            favorites = JsonConvert.DeserializeObject<List<FavItem>>(json);
        }

        return favorites;
    }

}
