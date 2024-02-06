using FireBrowserDataCore.Actions.Contracts;
using FireBrowserDataCore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FireBrowserDataCore.Actions
{
    public class HistoryActions : IHistoryActions
    {
        public HistoryContext HistoryContext { get; set; }

        public HistoryActions(string username)
        {

            HistoryContext = new HistoryContext(username);

        }



        //https://stackoverflow.com/questions/4218566/update-a-record-without-first-querying/74208246#74208246
        //TODO: add error types implicit er core https://www.nuget.org/packages/EntityFrameworkCore.Exceptions.Common
        public async Task InsertHistoryItem(string url, string title, int visitCount, int typedCount, int hidden)
        {
            try
            {
                if (await HistoryContext.Urls.FirstOrDefaultAsync(t => t.url == url) is HistoryItem item)
                {
                    HistoryContext.Urls.Where(x => x.url == url).ExecuteUpdate(y => y.SetProperty(z => z.visit_count, z => z.visit_count + 1));
                }
                else
                {
                    await HistoryContext.Urls.AddAsync(new HistoryItem(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), url, title, visitCount, typedCount, hidden));
                }

                await HistoryContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting history item: {ex.Message}");
            }
        }

        public async Task DeleteHistoryItem(string url)
        {
            try
            {

                await HistoryContext.Urls.Where(x => x.url == url).ExecuteDeleteAsync();
                await HistoryContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting download item: {ex.Message}");
            }
        }
    }

}

