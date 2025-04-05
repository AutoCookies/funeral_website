using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DietDoHongTran.Models;
using System.Threading.Tasks;

namespace DietDoHongTran.Areas.Admin.Controllers
{
    [Area("Admin")] // BẮT BUỘC
    [Authorize(Roles = "Admin")]
    public class AdminInvoiceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminInvoiceController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hiển thị tất cả hóa đơn
        public async Task<IActionResult> Index()
        {
            var invoices = await _context.Invoices
                .Include(i => i.InvoiceDetails)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            var productNames = await _context.Products
                .ToDictionaryAsync(p => p.Id, p => p.Name);

            ViewBag.ProductNames = productNames;

            return View(invoices);
        }

        // Xem chi tiết hóa đơn
        public async Task<IActionResult> Details(int id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.InvoiceDetails)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (invoice == null)
                return NotFound();

            var productNames = await _context.Products
                .ToDictionaryAsync(p => p.Id, p => p.Name);

            ViewBag.ProductNames = productNames;

            return View(invoice);
        }
        // GET: Admin/AdminInvoice/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
                return NotFound();

            return View(invoice);
        }

        // POST: Admin/AdminInvoice/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Invoice updatedInvoice)
        {
            if (id != updatedInvoice.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(updatedInvoice);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Invoices.Any(i => i.Id == updatedInvoice.Id))
                        return NotFound();

                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(updatedInvoice);
        }
        // GET: Admin/AdminInvoice/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.InvoiceDetails)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (invoice == null)
                return NotFound();

            return View(invoice);
        }

        // POST: Admin/AdminInvoice/DeleteConfirmed/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.InvoiceDetails)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (invoice != null)
            {
                _context.InvoiceDetails.RemoveRange(invoice.InvoiceDetails);
                _context.Invoices.Remove(invoice);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
