using JLSDataAccess;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using System.Linq;
using JLSDataModel.Models;

public class VisitorCounterMiddleware
{
    private readonly RequestDelegate _requestDelegate;

    public VisitorCounterMiddleware(RequestDelegate requestDelegate)
    {
        _requestDelegate = requestDelegate;
    }

    public async Task Invoke(HttpContext context, JlsDbContext db)
    {
        var request = context.Request;

        string visitorId = context.Request.Cookies["VisitorId"];
      if (visitorId == null && CheckAgent(context) == false)
      {
            //don the necessary staffs here to save the count by one
            int Year = DateTime.Now.Year;
            int Month = DateTime.Now.Month;
            int Day = DateTime.Now.Day;
            var visitorCounter = db.VisitorCounter.Where(p => p.Year == Year && p.Month == Month && p.Day == Day).FirstOrDefault();
            if (visitorCounter != null)
            {
                visitorCounter.Counter = visitorCounter.Counter + 1;
            }
            else
            {
                var visitorCounterToCreate = new VisitorCounter();
                visitorCounterToCreate.Year = Year;
                visitorCounterToCreate.Month = Month;
                visitorCounterToCreate.Day = Day;
                visitorCounterToCreate.Counter = 1;
                await db.VisitorCounter.AddAsync(visitorCounterToCreate);
            }
            await db.SaveChangesAsync();
            context.Response.Cookies.Append("VisitorId", Guid.NewGuid().ToString(), new CookieOptions()
            {
                    Path ="/",
                    HttpOnly = true,
                    Secure = false,
            });
       }

      await _requestDelegate(context);
    }

    public static bool CheckAgent(HttpContext context)
    {
        bool flag = false;

        string agent = context.Request.Headers["User-Agent"].ToString();
        string[] keywords = { "Android", "iPhone", "iPod", "iPad", "Windows Phone", "MQQBrowser" };

        //排除 Windows 桌面系统
        if (!agent.Contains("Windows NT") || (agent.Contains("Windows NT") && agent.Contains("compatible; MSIE 9.0;")))
        {
            //排除 苹果桌面系统
            if (!agent.Contains("Windows NT") && !agent.Contains("Macintosh"))
            {
                foreach (string item in keywords)
                {
                    if (agent.Contains(item))
                    {
                        flag = true;
                        break;
                    }
                }
            }
        }

        return flag;
    }
}