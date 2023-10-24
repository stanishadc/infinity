using Azure;
using InfinityWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace InfinityWeb.Controllers
{
    public class GroupTypeController : Controller
    {
        private readonly RepositoryContext _context;
        public GroupTypeController(RepositoryContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var data = await (from p in _context.GroupTypes
                              select new
                              {
                                  p.GroupTypeId,
                                  p.GroupTypeName
                              }).ToListAsync();
            return View(data);
        }
        [HttpGet]
        public async Task<IActionResult> GetById(Guid GroupTypeId)
        {
            var data = await (from p in _context.GroupTypes
                              select new
                              {
                                  p.GroupTypeId,
                                  p.GroupTypeName
                              }).Where(p => p.GroupTypeId == GroupTypeId).ToListAsync();
            return View(data);
        }
        [HttpPost]
        public IActionResult Insert(GroupType model)
        {
            try
            {
                model.GroupTypeId = Guid.NewGuid();
                model.LastUpdated = DateTime.UtcNow;
                _context.Add(model);
                _context.SaveChanges();
                return View(model);
            }
            catch (Exception ex)
            {
                return View("Index", ex.Message);
            }
        }
        [HttpPut]
        public IActionResult Update(GroupType model)
        {
            try
            {
                var data = _context.GroupTypes.Where(z => z.GroupTypeId == model.GroupTypeId).AsNoTracking().FirstOrDefault();
                if (data == null)
                {
                    return View("Index", "Record not exists!");
                }
                else
                {
                    data.GroupTypeName = model.GroupTypeName;
                    data.LastUpdated = DateTime.UtcNow;
                    _context.Entry(data).State = EntityState.Modified;
                    _context.SaveChanges();
                    return View("Index", "Record Updated Successfully");
                }
            }
            catch (Exception ex)
            {
                return View("Index", ex.Message);
            }
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(Guid Id)
        {
            try
            {
                var data = await _context.GroupTypes.FindAsync(Id);
                if (data == null)
                {
                    return View("Index", "Record not exists!");
                }
                else
                {
                    _context.GroupTypes.Remove(data);
                    await _context.SaveChangesAsync();
                    return View("Index", "Record Deleted!");
                }
            }
            catch (Exception ex)
            {
                return View("Index", "Error in deleting the record!");
            }
        }
    }
}
