using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MSD63AWebApp.Models;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace MSD63AWebApp.DataAccess
{
    public class RedisCacheMenusRepository
    {

        IDatabase myCacheDb;

        public RedisCacheMenusRepository(string connectionstring)
        {
            var cn = ConnectionMultiplexer.Connect(connectionstring);
            myCacheDb = cn.GetDatabase();
        
        }
        public async void AddMenu(Menu m)
        {
            var list = await GetMenus();
            list.Add(m);
            string menus = JsonConvert.SerializeObject(list);
            await myCacheDb.StringSetAsync("menus", menus);
        
        }

        public async Task<List<Menu>> GetMenus()
        {
            string menus = await myCacheDb.StringGetAsync("menus");

            var list =JsonConvert.DeserializeObject<List<Menu>>(menus);
            return list;
        }
    }
}
