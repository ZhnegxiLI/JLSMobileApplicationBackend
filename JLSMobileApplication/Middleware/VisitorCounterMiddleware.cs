using JLSDataAccess;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using System.Linq;
using JLSDataModel.Models;

public class VisitorCounterMiddleware
{
    private readonly RequestDelegate _requestDelegate;
    private readonly JlsDbContext db;

    public VisitorCounterMiddleware(RequestDelegate requestDelegate, JlsDbContext dbContext)
    {
        _requestDelegate = requestDelegate;
        db = dbContext;
    }

    public async Task Invoke(HttpContext context)
    {
      string visitorId = context.Request.Cookies["VisitorId"];
      if (visitorId == null)
      {
            //don the necessary staffs here to save the count by one
            int Year = DateTime.Now.Year;
            int Month = DateTime.Now.Month;
            var visitorCounter = db.VisitorCounter.Where(p => p.Year == Year && p.Month == Month).FirstOrDefault();
            if (visitorCounter!=null)
            {
                visitorCounter.Counter = visitorCounter.Counter + 1;
            }
            else
            {
                var visitorCounterToCreate = new VisitorCounter();
                visitorCounterToCreate.Year = Year;
                visitorCounterToCreate.Year = Year;
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
}