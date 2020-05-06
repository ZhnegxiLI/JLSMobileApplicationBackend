using JLSDataAccess.Interfaces;
using JLSDataModel.Models.Adress;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using JLSDataModel.Models.User;

namespace JLSDataAccess.Repositories
{
    public class AnalyticsRepository : IAnalyticsReporsitory
    {
        private readonly JlsDbContext db;

        public AnalyticsRepository(JlsDbContext context)
        {
            db = context;
        }

        public async Task<List<dynamic>> GetAdminSalesPerformanceDashboard(string Lang)
        {
           // var riStatusId = db.ReferenceItem.Where(p => p.Code == "OrderStatus_Valid").Select(p => p.Id).FirstOrDefault();
            var result = await (from riYear in db.ReferenceItem
                          join rcYear in db.ReferenceCategory on riYear.ReferenceCategoryId equals rcYear.Id
                          where rcYear.ShortLabel == "Year"
                          orderby riYear.Value descending
                          select new
                          {
                              Year = riYear.Value,
                           
                              Dashboard = (from riMonth in db.ReferenceItem
                                                  join rcMonth in db.ReferenceCategory on riMonth.ReferenceCategoryId equals rcMonth.Id
                                                  where rcMonth.ShortLabel == "Month" 
                                                  select new
                                                  {
                                                      ProductCommentCount = (from pc in db.ProductComment
                                                                        where pc.CreatedOn.Value.Year == int.Parse(riYear.Value) && pc.CreatedOn.Value.Month == int.Parse(riMonth.Value)
                                                                        select pc.Id).Count(),
                                                      Month = riMonth.Value,
                                                      Order = (from o in db.OrderInfo
                                                               where o.CreatedOn.Value.Year == int.Parse(riYear.Value) && o.CreatedOn.Value.Month == int.Parse(riMonth.Value) 
                                                               select new{ 
                                                                    Id = o.Id,
                                                                    UserId = o.UserId,
                                                                    TotalPrice = o.TotalPrice,
                                                                    CreatedOn = o.CreatedOn,
                                                                    OrderStatusId = o.StatusReferenceItemId,
                                                                    OrderStatusCode =db.ReferenceItem.Where(p => p.Id == o.StatusReferenceItemId).Select(p => p.Code).FirstOrDefault(),
                                                                    OrderTypeId = o.OrderTypeId,
                                                                    OrderTypeCode = db.ReferenceItem.Where(p=>p.Id == o.OrderTypeId).Select(p=>p.Code).FirstOrDefault()
                                                               }).ToList()

                                                  }).ToList()
                          }).ToListAsync<dynamic>();
            return result;
        }

        private object await(IQueryable<object> queryable)
        {
            throw new NotImplementedException();
        }
    }
}
