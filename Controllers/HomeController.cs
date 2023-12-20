using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LoginRegistration.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace WeddingPlanner.Controllers;

public class HomeController : Controller
{    
    private readonly ILogger<HomeController> _logger;
    // Add a private variable of type MyContext (or whatever you named your context file)
    private MyContext _context;         
    // Here we can "inject" our context service into the constructor 
    // The "logger" was something that was already in our code, we're just adding around it   
    public HomeController(ILogger<HomeController> logger, MyContext context)    
    {        
        _logger = logger;
        // When our HomeController is instantiated, it will fill in _context with context
        // Remember that when context is initialized, it brings in everything we need from DbContext
        // which comes from Entity Framework Core
        _context = context;    
    } 
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost("login")]
    public IActionResult Login(UserLogin user){
        if(ModelState.IsValid){
            UserReg? CurrentUser = _context.Users.FirstOrDefault(e => e.Email == user.LEmail);
            if(CurrentUser == null){
                ModelState.AddModelError("Email", "Invalid Email/Password");
                return View("Index");
            }
            PasswordHasher<UserLogin> hasher = new PasswordHasher<UserLogin> ();
            var result = hasher.VerifyHashedPassword(user, CurrentUser.Password, user.LPassword);
            if(result == 0){
                ModelState.AddModelError("LPassword", "Password invalid");
                return View("Index");
            }
            HttpContext.Session.SetInt32("UserId", CurrentUser.id);
            HttpContext.Session.SetString("UserName", CurrentUser.FirstName);
            ViewBag.username = CurrentUser.FirstName;
            return  RedirectToAction("Profile");
        }
        else{
            return View("Index");
        }
    }


        [HttpPost("register")]
    public IActionResult Register(UserReg user){
        if(ModelState.IsValid){
            PasswordHasher<UserReg> Hasher = new PasswordHasher<UserReg>();
            user.Password = Hasher.HashPassword(user, user.Password);  
            _context.Add(user);
            _context.SaveChanges();
            HttpContext.Session.SetInt32("UserId", user.id);
            HttpContext.Session.SetString("UserName", user.FirstName);
            ViewBag.username = user.FirstName;
            return RedirectToAction("Profile");
        }
        else{
            return View("Index");
        }
    }

    [SessionCheck]
    [HttpGet("Profile")]
    public IActionResult Profile(){
        List<Wedding> AllWeddings = _context.Weddings.Include(e=> e.Associations).ThenInclude(e => e.Guest).ToList();
        return View(AllWeddings);
        
    }


    [SessionCheck]
    [HttpGet("logout")]
    public IActionResult LogOut(){
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }

    [SessionCheck]
    [HttpGet("wedding/register")]
    public IActionResult NewWedding(){
        return View();
    }

    [SessionCheck]
    [HttpPost("wedding/register/post")]
    public IActionResult RegisterWedding(Wedding wedding){
        if(ModelState.IsValid){ 
            _context.Add(wedding);
            _context.SaveChanges();
            return RedirectToAction("Profile");
        }
        else{
            return View("NewWedding");
        }
    }

    [SessionCheck]
    [HttpGet("wedding/delete/{id}")]
    public IActionResult DeleteWedding(int id){
        Wedding weddingToDelete = _context.Weddings.Include(w => w.Associations).FirstOrDefault(e => e.WeddingId == id);
        if(weddingToDelete.CreatorId != HttpContext.Session.GetInt32("UserId")){
            return RedirectToAction("Profile");
        }
        _context.Remove(weddingToDelete);
        _context.SaveChanges();
        return RedirectToAction("Profile");
    }

    [SessionCheck]
    [HttpGet("wedding/enroll/{id}")]
    public IActionResult Enroll(int id){
        if(_context.Associations.Any(e => e.WeddingId == id && e.GuestId == HttpContext.Session.GetInt32("UserId"))){
            return RedirectToAction("Profile");
        }
        Association assoc = new Association();
        assoc.WeddingId = id;
        int? loggedin = HttpContext.Session.GetInt32("UserId");
        assoc.GuestId = loggedin;
        _context.Add(assoc);
        _context.SaveChanges();
        return RedirectToAction("Profile");
    }


    [SessionCheck]
    [HttpGet("wedding/leave/{id}")]
    public IActionResult Leave(int id){
        Association assoc = _context.Associations.FirstOrDefault(e=> e.WeddingId ==id && e.GuestId == HttpContext.Session.GetInt32("UserId"));
        _context.Remove(assoc);
        _context.SaveChanges();
        return RedirectToAction("Profile");
        
    }

    [SessionCheck]
    [HttpGet("wedding/details/{id}")]
    public IActionResult WeddingDetails(int id){
        Wedding showWedding = _context.Weddings.Include(e=> e.Associations).ThenInclude(e => e.Guest).FirstOrDefault(e=> e.WeddingId ==id);
        return View(showWedding);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}




public class SessionCheckAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Find the session, but remember it may be null so we need int?
        int? userId = context.HttpContext.Session.GetInt32("UserId");
        // Check to see if we got back null
        if(userId == null)
        {
            // Redirect to the Index page if there was nothing in session
            // "Home" here is referring to "HomeController", you can use any controller that is appropriate here
            context.Result = new RedirectToActionResult("Index", "Home", null);
        }
    }
}
