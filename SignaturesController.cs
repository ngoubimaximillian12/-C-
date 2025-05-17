using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Petition.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Petition.Controllers
{
    [Authorize]
    public class SignaturesController : Controller
    {
        private readonly AppDbContext _context;

        public SignaturesController(AppDbContext context)
        {
            _context = context;
        }

        // POST: Signatures/Sign
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sign(int petitionId)
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Members");
            }

            // Ensure no duplicate signatures
            bool alreadySigned = await _context.PetitionSignatures
                .AnyAsync(ps => ps.PetitionId == petitionId && ps.MemberId == userId);

            if (!alreadySigned)
            {
                var signature = new Petition_signature
                {
                    PetitionId = petitionId,
                    MemberId = userId,
                    SignatureText = "Signed",  // You can change this value as needed
                    SignedAt = DateTime.Now
                };

                _context.PetitionSignatures.Add(signature);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", "Petitions", new { id = petitionId });
        }

        public async Task<IActionResult> List(int petitionId)
        {
            var signatures = await _context.PetitionSignatures
                .Include(ps => ps.member)
                .Where(ps => ps.PetitionId == petitionId)
                .ToListAsync();

            return View(signatures);
        }
    }
}
