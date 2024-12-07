using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SareeDesigns.Data;
using SareeDesigns.Models;
using SareeDesigns.Services;

namespace SareeDesigns.Controllers
{
    public class SareesController : Controller
    {
        private readonly SareeDesignsContext _context;
        private readonly ICloudStorageService _cloudStorageService;

        public SareesController(SareeDesignsContext context, ICloudStorageService cloudStorageService)
        {
            _context = context;
            _cloudStorageService = cloudStorageService;
        }

        // GET: Sarees
        public async Task<IActionResult> Index()
        {
            var sarees = await _context.Saree.ToListAsync();
            foreach (var saree in sarees)
            {
                await GenerateSignedUrl(saree);
            }
            return View(sarees);
        }

        // GET: Sarees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var saree = await _context.Saree
                .FirstOrDefaultAsync(m => m.Id == id);
            if (saree == null)
            {
                return NotFound();
            }
            await GenerateSignedUrl(saree);

            return View(saree);
        }

        private async Task GenerateSignedUrl(Saree saree)
        {
            //Get Signed URL only when Saved File Name is available
            if (!string.IsNullOrWhiteSpace(saree.SavedFileName))
                saree.SignedUrl = await _cloudStorageService.GetSignedUrlAsync(saree.SavedFileName);
        }

        // GET: Sarees/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Sarees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,Photo,SavedUrl,SavedFileName")] Saree saree)
        {
            if (ModelState.IsValid)
            {
                if (saree.Photo != null)
                {
                    saree.SavedFileName = GenerateFileNameToSave(saree.Photo.FileName);
                    saree.SavedUrl= await _cloudStorageService.UploadFileAsync(saree.Photo, saree.SavedFileName);
                }
                _context.Add(saree);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(saree);
        }

        private string? GenerateFileNameToSave(string incomingFileName)
        {
            var fileName = Path.GetFileNameWithoutExtension(incomingFileName);
            var extension = Path.GetExtension(incomingFileName);
            return $"{fileName}-{DateTime.Now.ToUniversalTime().ToString("yyyyMMddHHmmss")}{extension}";

        }

        // GET: Sarees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var saree = await _context.Saree.FindAsync(id);
            if (saree == null)
            {
                return NotFound();
            }
            await GenerateSignedUrl(saree);
            return View(saree);
        }

        // POST: Sarees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,Photo,SavedUrl,SavedFileName")] Saree saree)
        {
            if (id != saree.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await ReplacePhoto(saree);
                    _context.Update(saree);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SareeExists(saree.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(saree);
        }

        private async Task ReplacePhoto(Saree saree)
        {
            if (saree.Photo != null)
            {
                // replace the file by deleting animal.SavedFileName file and then uploading new saree.Photo
                if (!String.IsNullOrEmpty(saree.SavedFileName))
                {
                    await _cloudStorageService.DeleteFileAsync(saree.SavedFileName);
                }
                saree.SavedFileName = GenerateFileNameToSave(saree.Photo.FileName);
                saree.SavedUrl = await _cloudStorageService.UploadFileAsync(saree.Photo, saree.SavedFileName);
            }
        }

        // GET: Sarees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var saree = await _context.Saree
                .FirstOrDefaultAsync(m => m.Id == id);
            if (saree == null)
            {
                return NotFound();
            }
            
            await GenerateSignedUrl(saree);
            return View(saree);
        }

        // POST: Sarees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var saree = await _context.Saree.FindAsync(id);
            if (saree != null)
            {
                if (!String.IsNullOrWhiteSpace(saree.SavedFileName))
                {
                    await _cloudStorageService.DeleteFileAsync(saree.SavedFileName);
                    saree.SavedFileName = String.Empty;
                    saree.SavedUrl = String.Empty;
                }
                _context.Saree.Remove(saree);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SareeExists(int id)
        {
            return _context.Saree.Any(e => e.Id == id);
        }
    }
}
