using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BeltExam.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeltExam.Controllers 
{
    public class HomeController : Controller 
    {
        private MyContext _context { get; set; }

        public HomeController (MyContext context) 
        {
            _context = context;
        }

        [HttpGet ("")]
        public IActionResult Index () 
        {
            return View ();
        }

        [HttpPost ("login")]
        public IActionResult Login (LoginUser userLogin) 
        {
            if (ModelState.IsValid) 
            {
                var userInDb = _context.Users.FirstOrDefault (log => log.Email == userLogin.LoginEmail);
                if (userInDb == null) 
                {
                    ModelState.AddModelError ("LoginEmail", "Invalid Email/Password");
                    return View ("Index");
                } 
                else 
                {
                    var hasher = new PasswordHasher<LoginUser> ();
                    var result = hasher.VerifyHashedPassword (userLogin, userInDb.Password, userLogin.LoginPassword);
                    if (result == 0) 
                    {
                        ModelState.AddModelError ("LoginEmail", "Invalid Email/Password");
                        return View ("Index");
                    } 
                    else 
                    {
                        HttpContext.Session.SetInt32 ("userId", userInDb.UserId);
                        return Redirect ("/Home");
                    }
                }
            } 
            else 
            {
                return View ("Index");
            }
        }

        [HttpPost ("Register")]
        public IActionResult Register (User user) 
        {
            if (ModelState.IsValid) 
            {
                if (_context.Users.Any (logger => logger.Email == user.Email)) 
                {
                    ModelState.AddModelError ("Email", "Email already in use!");
                    return View ("Index");
                } 
                else 
                {
                    PasswordHasher<User> Hasher = new PasswordHasher<User> ();
                    user.Password = Hasher.HashPassword (user, user.Password);
                    _context.Users.Add (user);
                    _context.SaveChanges ();
                    HttpContext.Session.SetInt32 ("userId", user.UserId);
                    return Redirect ("/Home");
                }
            }
            return View ("Index");
        }

        [HttpGet ("Home")]
        public IActionResult Home () 
        {
            int? userId = HttpContext.Session.GetInt32 ("userId");
            if (userId == null) 
            {
                return RedirectToAction ("Logout");
            } 
            else 
            {
                ViewBag.User = _context.Users.FirstOrDefault (use => use.UserId == userId);
                List<DojoActivity> dojoactivities = _context.DojoActivities.Include (active => active.Coordinator).Include (active => active.CalendarActivities).ThenInclude (calendar => calendar.UserJoin).Where(date => date.StartDate > DateTime.Now).OrderBy(start => start.StartDate).ToList ();
                return View (dojoactivities);
            }
        }

        [HttpGet ("Logout")]
        public IActionResult Logout () 
        {
            HttpContext.Session.Clear ();
            return RedirectToAction ("Index");
        }

        [HttpGet ("New")]
        public IActionResult New () 
        {
            return View ();
        }

        [HttpPost ("Create/Activity")]
        public IActionResult Create (DojoActivity newActivity) 
        {
            if (ModelState.IsValid) 
            {
                int? userId = HttpContext.Session.GetInt32 ("userId");
                if (userId == null) 
                {
                    return RedirectToAction ("Logout");
                }
                newActivity.UserId = (int) userId;
                _context.DojoActivities.Add (newActivity);
                _context.SaveChanges ();
                return Redirect ($"/Show/{newActivity.ActivityId}"); { }
            }
            return View ("New");
        }

        [HttpGet ("Join/{userId}/{actid}")]
        public IActionResult Join (int userId, int actid) 
        {
            CalendarEvent toJoin = new CalendarEvent ();
            toJoin.UserId = userId;
            toJoin.ActivityId = actid;
            _context.CalendarEvents.Add (toJoin);
            _context.SaveChanges ();
            return RedirectToAction ("Home");
        }

        [HttpGet ("Leave/{userId}/{actid}")]
        public IActionResult Leave (int userId, int actid) 
        {
            CalendarEvent toLeave = _context.CalendarEvents.FirstOrDefault (calendaractivity => calendaractivity.ActivityId == actid && calendaractivity.UserId == userId);
            _context.CalendarEvents.Remove (toLeave);
            _context.SaveChanges ();
            return RedirectToAction ("Home");
        }

        [HttpGet ("Cancel/{actid}")]
        public IActionResult Cancel (int actid) 
        {
            DojoActivity toCancel = _context.DojoActivities.FirstOrDefault (act => act.ActivityId == actid);
            _context.DojoActivities.Remove (toCancel);
            _context.SaveChanges ();
            return RedirectToAction ("Home");
        }

        [HttpGet ("Show/{actId}")]
        public IActionResult Show (int actId) 
        {
            int? userId = HttpContext.Session.GetInt32 ("userId");
            if (userId == null) 
            {
                return RedirectToAction ("Logout");
            } 
            else 
            {
                ViewBag.User = _context.Users.FirstOrDefault (use => use.UserId == userId);
                DojoActivity activity = _context.DojoActivities.Include (act => act.Coordinator).Include(active => active.CalendarActivities).ThenInclude (calendar => calendar.UserJoin).FirstOrDefault(act => act.ActivityId == actId);
                return View ("Show", activity);
            }
        }
    }
}