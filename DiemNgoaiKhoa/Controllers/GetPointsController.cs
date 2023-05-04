using DiemNgoaiKhoa.Helpers;
using DiemNgoaiKhoa.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiemNgoaiKhoa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetPointsController : ControllerBase
    {
        private readonly DataContext _context;

        public GetPointsController(DataContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<List<ActionResult>> GetAll()
        {
                return (List<ActionResult>)(from s in _context.Students
                                            join c in _context.Classes on s.ClassId equals c.Id
                                            join p in _context.Points on s.Id equals p.StudentId
                                            join sem in _context.Semesters on p.SemesterId equals sem.Id
                                            where s.Id == p.StudentId
                                            group new { s.Id, s.Fullname, c.Name, p.PointStudent }
                                            by new { s.Id, s.Fullname, c.Name }
                                            into g
                                            select new
                                            {
                                                g.Key.Id,
                                                g.Key.Fullname,
                                                ClassName = g.Key.Name,
                                                SemesterName = g.Key.Name,
                                                Điểm = g.Sum(x => x.PointStudent)
                                            });
        }
    }
}
