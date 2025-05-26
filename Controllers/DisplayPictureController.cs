using Microsoft.AspNetCore.Mvc;
using socialApi.Data;
using socialApi.Dtos;
using socialApi.Models;
using socialApi.Mappers;

namespace socialApi.Controllers
{
    [Route("api/display-pictures")]
    [ApiController]
    public class DisplayPictureController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DisplayPictureController(ApplicationDbContext context)
        {
            _context = context;
        }

    }
}