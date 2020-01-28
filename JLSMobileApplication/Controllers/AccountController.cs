using AutoMapper;
using JLSDataAccess;
using JLSDataModel.Models.Mapping;
using JLSDataModel.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace JLSMobileApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private JlsDbContext db;
        private readonly IMapper _mapper;

        public AccountController(UserManager<User> userManager, JlsDbContext dbContext, IMapper mapper)
        {
            _mapper = mapper;
            _userManager = userManager;
            db = dbContext;
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserRegistrationView model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userIdentity = _mapper.Map<User>(model);
            var result = await _userManager.CreateAsync(userIdentity, model.Password);
            if (!result.Succeeded)  {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return new BadRequestObjectResult(result.Errors);
            }
            //await db.Customers.AddAsync(new Customer { IdentityId = userIdentity.Id, Location = model.Location });
            //await _appDbContext.SaveChangesAsync();

            return new OkObjectResult("Account created");
        }
    }
}
