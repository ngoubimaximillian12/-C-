using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Petition.Models;
using Petition.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace Petition.Controllers
{
    [Authorize]
    public class PetitionsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<Member> _userManager;

        public PetitionsController(AppDbContext context, UserManager<Member> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var petitions = await _context.Petitions.Include(p => p.petition_Signatures).ToListAsync();
            return View(petitions);
        }
        public async Task<IActionResult> PetitionsList()
        {
            var petitions = await _context.Petitions.Include(p => p.petition_Signatures).ToListAsync();
            return View(petitions);
        }
        public async Task<IActionResult> Details(int id)
        {
            var petition = await _context.Petitions
                .Include(p => p.petition_Signatures)
                .ThenInclude(ps => ps.member)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (petition == null)
            {
                return NotFound();
            }

            return View(petition);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PetitionTable petition, IFormFile petitionBanner)
        {
                if (petitionBanner != null && petitionBanner.Length > 0)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                    Directory.CreateDirectory(uploadsFolder); 

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(petitionBanner.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await petitionBanner.CopyToAsync(fileStream);
                    }

                    petition.Picture = "/uploads/" + uniqueFileName;
                }

                _context.Petitions.Add(petition);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index" , "Petitions");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var petition = await _context.Petitions.FindAsync(id);
            if (petition == null)
            {
                return NotFound();
            }
            return View(petition);
        }

        [HttpGet]
        public async Task<IActionResult> Sign(int id)
        {
            var petition = await _context.Petitions.FindAsync(id);
            if (petition == null)
            {
                return NotFound();
            }

            var model = new SignPetitionViewModel
            {
                PetitionId = petition.Id,
                Title = petition.Title,
                Description = petition.Description,
                Picture = petition.Picture
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sign(SignPetitionViewModel model)
        {
            var userId = _userManager.GetUserId(User); 
            if (userId == null)
            {
                return RedirectToAction("Login", "Members"); 
            }

            // Check if user already signed
            bool alreadySigned = await _context.PetitionSignatures
                .AnyAsync(ps => ps.PetitionId == model.PetitionId && ps.MemberId == userId);

            if (!alreadySigned)
            {
                var signature = new Petition_signature
                {
                    PetitionId = model.PetitionId,
                    MemberId = userId,
                    SignatureText = model.SignatureText,
                    SignedAt = DateTime.Now
                };

                _context.PetitionSignatures.Add(signature);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Petitions");
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var petition = await _context.Petitions.FindAsync(id);
            if (petition != null)
            {
                _context.Petitions.Remove(petition);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
