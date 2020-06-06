using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Utility
{
    public class Utility
    {
        /// <summary>
        /// 取得最常用type
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        static public List<string> GetTypes(string UserId)
        {
            List<string> items = new List<string>();

           
                //如果沒有資料
                items.Add("餐費");
                items.Add("交通費");
                items.Add("娛樂費");
                items.Add("服裝費");

                return items;
        
        }

        static public float GetTodaySpend(string UserId)
        {
           
            var amount =0;
            return (float)amount;
        }

        static public float GetThisMonthSpend(string UserId)
        {
           
            var amount = 0;
            return (float)amount;
        }

        

       
    }
}

