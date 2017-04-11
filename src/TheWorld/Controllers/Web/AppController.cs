using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TheWorld.Models;
using TheWorld.Services;
using TheWorld.ViewModels;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace TheWorld.Controllers.Web
{
    public class AppController : Controller
    {
        private IMailService _mailService;
        private IConfigurationRoot _config;
        private IWorldRepository _repository;
        private ILogger<AppController> _logger;

        // private WorldContext _context;

        public AppController(IMailService mailService, 
            IConfigurationRoot config, 
            IWorldRepository repository,
            ILogger<AppController> logger)
        {
            _mailService = mailService;
            _config = config;
            _repository = repository;
            _logger = logger;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Trips()
        {
            var trips = _repository.GetAllTrips();

            return View(trips);
        }

        public IActionResult Contact()
        {
            //throw new InvalidOperationException("Bad things happen to good developers");
            return View("Contact");
        }

        [HttpPost]
        public IActionResult Contact(ContactViewModel model)
        {

            if(model.Email.Contains("aol.com"))
                ModelState.AddModelError("", "We don't support AOL addresses");

            if (ModelState.IsValid)
            {
                _mailService.SendMail(_config["MailSettings:ToAddress"], model.Email, "From The World", "runtah");

                ModelState.Clear();
                ViewBag.UserMessage = "Message sent!";

            }
           
            return View();
        }

        public IActionResult About()
        {
            return View("About");
        }
          
    }
}
