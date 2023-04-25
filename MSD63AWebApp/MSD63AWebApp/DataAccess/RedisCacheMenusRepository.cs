using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Diagnostics.AspNetCore3;
using Microsoft.Extensions.Logging;
using MSD63AWebApp.Models;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace MSD63AWebApp.DataAccess
{
    public class RedisCacheMenusRepository
    {

        IDatabase myCacheDb;
        ILogger _logger;
        public RedisCacheMenusRepository(string connectionstring, ILogger logger)
        {
            try
            {
                _logger = logger;

                var cn = ConnectionMultiplexer.Connect(connectionstring);
                myCacheDb = cn.GetDatabase();
            }
            catch (Exception ex)
            {
                //log


            }
        
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
            try
            {
                _logger.LogInformation("Menus are about to be read from the Cache");
            //    string menus = await myCacheDb.StringGetAsync("menus");

              //  var list = JsonConvert.DeserializeObject<List<Menu>>(menus);
                return new List<Menu>() { };
            }catch (Exception ex)
            { //log
              
            
            }
    return new List<Menu>();
        

        }
    }
}
