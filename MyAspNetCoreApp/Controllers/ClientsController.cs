using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAspNetCoreApp.Data;
using MyAspNetCoreApp.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace MyAspNetCoreApp.Controllers
{
    [Authorize]
    public class ClientsController : Controller
    {
        private readonly INTSystemDbContext _context;

        public ClientsController(INTSystemDbContext context)
        {
            _context = context;
        }

        // GET: Clients/Create
        public IActionResult Create()
        {
            return View(new Client { Active = true, ExpDate = DateTime.Now.AddYears(1) });
        }

        // POST: Clients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClientName,MainBusiness,Address,Website,ContactPerson,ExpDate,Active")] Client client)
        {
            if (ModelState.IsValid)
            {
                client.Id = Guid.NewGuid().ToString();
                _context.Add(client);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        // GET: Clients
        public async Task<IActionResult> Index(int? pageNumber)
        {
            int pageSize = 5;
            var clients = from c in _context.intClient
                         select c;

            return View(await PaginatedList<Client>.CreateAsync(clients, pageNumber ?? 1, pageSize));
        }

        // GET: Clients/View/5
        public new async Task<IActionResult> View(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var client = await _context.intClient
                .FirstOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var client = await _context.intClient.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }

        // POST: Clients/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,ClientName,MainBusiness,Address,Website,ContactPerson,ExpDate,Active")] Client client)
        {
            if (id != client.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(client);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client.Id))
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
            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var client = await _context.intClient.FindAsync(id);
            if (client != null)
            {
                _context.intClient.Remove(client);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(string id)
        {
            return _context.intClient.Any(e => e.Id == id);
        }
    }
}
