using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Petition.Models;
using Petition.ViewModels;
using System.Linq;
using System.Threading.Tasks;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly UserManager<Member> _userManager;
    private readonly AppDbContext _context;

    public AdminController(UserManager<Member> userManager, AppDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }
    public async Task<IActionResult> Dashboard()
    {
        var totalMembers = await _userManager.Users.CountAsync();
        var totalPetitions = await _context.Petitions.CountAsync();
        var blockedMembers = await _userManager.Users.Where(m => m.LockoutEnd != null).CountAsync();
        var activePetitions = await _context.Petitions.CountAsync();

        var model = new AdminDashboardViewModel
        {
            TotalMembers = totalMembers,
            TotalPetitions = totalPetitions,
            ActivePetitions = activePetitions,
            BlockedMembers = blockedMembers
        };

        return View(model);
    }
    // Manage Members
    public IActionResult ManageMembers()
    {
        var members = _userManager.Users.ToList();
        return View(members);
    }

    [HttpPost]
    public async Task<IActionResult> BlockMember(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user != null)
        {
            user.IsBlocked = !user.IsBlocked; 
            await _userManager.UpdateAsync(user);
        }
        return RedirectToAction("ManageMembers");
    }

    // Manage Petitions
    public IActionResult ManagePetitions()
    {
        var petitions = _context.Petitions.ToList();
        return View(petitions);
    }

    [HttpPost]
    public IActionResult DeletePetition(int id)
    {
        var petition = _context.Petitions.Find(id);
        if (petition != null)
        {
            _context.Petitions.Remove(petition);
            _context.SaveChanges();
        }
        return RedirectToAction("ManagePetitions");
    }
}
