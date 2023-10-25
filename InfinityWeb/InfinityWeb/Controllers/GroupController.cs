using InfinityWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InfinityWeb.Controllers
{
    public class GroupController : Controller
    {
        private readonly RepositoryContext _context;
        public GroupController(RepositoryContext context)
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
            var data = await (from g in _context.Groups
                              join gt in _context.GroupTypes on g.GroupTypeId equals gt.GroupTypeId
                              select new
                              {
                                  g.GroupId,
                                  g.GroupName,
                                  g.Address,
                                  g.IsActive,
                                  g.UpdatedDate,
                                  gt.GroupTypeId,
                                  gt.GroupTypeName
                              }).ToListAsync();
            return View(data);
        }
        [HttpGet]
        public async Task<IActionResult> GetById(Guid GroupTypeId)
        {
            var data = await (from g in _context.Groups
                              join gt in _context.GroupTypes on g.GroupTypeId equals gt.GroupTypeId
                              select new
                              {
                                  g.GroupId,
                                  g.GroupName,
                                  g.Address,
                                  g.IsActive,
                                  g.UpdatedDate,
                                  gt.GroupTypeId,
                                  gt.GroupTypeName
                              }).Where(p => p.GroupTypeId == GroupTypeId).ToListAsync();
            return View(data);
        }
        [HttpPost]
        public IActionResult Insert(Group model)
        {
            try
            {
                model.GroupId = Guid.NewGuid();
                model.CreatedDate = DateTime.UtcNow;
                model.UpdatedDate = DateTime.UtcNow;
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
        public IActionResult Update(Group model)
        {
            try
            {
                var data = _context.Groups.Where(z => z.GroupId == model.GroupId).AsNoTracking().FirstOrDefault();
                if (data == null)
                {
                    return View("Index", "Record not exists!");
                }
                else
                {
                    data.GroupName = model.GroupName;
                    data.UpdatedDate = DateTime.UtcNow;
                    data.GroupTypeId = model.GroupTypeId;
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
                var data = await _context.Groups.FindAsync(Id);
                if (data == null)
                {
                    return View("Index", "Record not exists!");
                }
                else
                {
                    _context.Groups.Remove(data);
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
