using JLSDataAccess;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
namespace JLSMobileApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : Controller
    {
        private readonly JlsDbContext db;
        public ValuesController(JlsDbContext jlsDbContext)
        {
            db = jlsDbContext;
        }
        // GET api/values
        [HttpGet]
        public JsonResult  Get()
        {
            var result = db.ReferenceCategory.ToList();
            return Json(result);
        }
    }
}
