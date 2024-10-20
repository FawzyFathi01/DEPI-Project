using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineCoursesApp.BLL.Services;
using OnlineCoursesApp.DAL.Models;
using OnlineCoursesApp.Models;
using OnlineCoursesApp.ViewModel.HomePageViewModels;
using System.Diagnostics;

namespace OnlineCoursesApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IService<Course> _courseService;

        public HomeController(ILogger<HomeController> logger, IService<Course>courseService)
        {
            _logger = logger;
            this._courseService = courseService;
        }

        public IActionResult Index(string searchQuery)
        {

            List<Course> courses = _courseService.Query().
                Include(i => i.Students).
                Include(i => i.Instructor).
                Where(i => i.CourseStatus == CourseStatus.Approved).
                ToList();  // filter Home Courses

            // If searchQuery is provided, filter the course list
            List<Course> filteredCourses;
            if (string.IsNullOrEmpty(searchQuery))
            {
                filteredCourses = courses;
            }
            else
            {
                filteredCourses = courses.Where(c => c.Name.Contains(searchQuery))
                                .ToList(); // Search by name
            }

            List<CoursesHomeViewModel> courceList = filteredCourses.Select(e => new CoursesHomeViewModel()
            {
                CourseId = e.CourseId,
                CourseName = e.Name,
                CourseDescription = e.Description,
                InsrUctorName = e.Instructor.Name,
                NumStudent = e.Students.Count
            }).ToList();

            return View(courceList);
            
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
