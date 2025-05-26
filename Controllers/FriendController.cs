using Microsoft.AspNetCore.Mvc;
using socialApi.Data;
using socialApi.Dtos;
using socialApi.Models;
using socialApi.Mappers;

namespace socialApi.Controllers
{
    [Route("api/friends")]
    [ApiController]
    public class FriendController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FriendController(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}