using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Data;
using api.Dtos;
using api.Models;
using api.Mappers;

namespace api.Controllers
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