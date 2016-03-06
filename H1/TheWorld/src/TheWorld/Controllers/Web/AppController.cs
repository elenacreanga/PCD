using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using System.Linq;
using TheWorld.Models;
using TheWorld.Services;
using TheWorld.ViewModels;

namespace TheWorld.Controllers.Web
{
    public class AppController : Controller
    {
        private readonly IMailService mailService;
        private IWorldRepository worldRepository { get; set; }

        public AppController(IMailService mailService, IWorldRepository worldRepository)
        {
            this.mailService = mailService;
            this.worldRepository = worldRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Trips()
        {
            var trips = worldRepository.GetAllTrips().OrderBy(x => x.Name).ToList();
            return View(trips);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Contact(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                var email = Startup.Configuration["AppSettings:SiteEmailAddress"];
                if (string.IsNullOrWhiteSpace(email))
                {
                    ModelState.AddModelError("", "Could not send mail, config missing");
                }
                if (mailService.SendMail(email, model.Email, model.Message, "Subject"))
                {
                    ModelState.Clear();
                    ViewBag.Message = "E-mail sent!";
                }
            }
            return View();
        }
    }
}