﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KLTN_E.Data;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authorization;

namespace KLTN_E.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class KhachHangsController : Controller
    {
        private readonly KltnContext _context;
        private readonly IWebHostEnvironment _environment;
        public KhachHangsController(KltnContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [Authorize(Roles = "Admin, NhanVien")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.KhachHangs.ToListAsync());
        }
        [Authorize(Roles = "Admin, NhanVien")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khachHang = await _context.KhachHangs
                .FirstOrDefaultAsync(m => m.MaKh == id);
            if (khachHang == null)
            {
                return NotFound();
            }

            return View(khachHang);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KhachHang khachHang, IFormFile hinhKH)
        {

            var check = await _context.KhachHangs.FirstOrDefaultAsync(p => p.HoTen == khachHang.HoTen);
            if (check != null)
            {
                ModelState.AddModelError("HoTen", "Ten da ton tai");
                return View(khachHang);
            }
            else
            {

                if (hinhKH != null)
                {
                    string dir = Path.Combine(_environment.WebRootPath, "Hinh/KhachHang");
                    string imgName = Guid.NewGuid().ToString() + hinhKH.FileName;
                    string filePath = Path.Combine(dir, imgName);
                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await hinhKH.CopyToAsync(fs);
                    fs.Close();
                    khachHang.Hinh = imgName;
                }
            }
            _context.Add(khachHang);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khachHang = await _context.KhachHangs.FindAsync(id);
            if (khachHang == null)
            {
                return NotFound();
            }
            return View(khachHang);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, KhachHang khachHang, IFormFile hinhKH)
        {
            if (id != khachHang.MaKh)
            {
                return NotFound();
            }

            //if (ModelState.IsValid)
            //{
                try
                {


                    if (hinhKH != null)
                    {
                        string dir = Path.Combine(_environment.WebRootPath, "Hinh/KhachHang");
                        string imgName = Guid.NewGuid().ToString() + hinhKH.FileName;
                        string filePath = Path.Combine(dir, imgName);
                        FileStream fs = new FileStream(filePath, FileMode.Create);
                        await hinhKH.CopyToAsync(fs);
                        fs.Close();
                        khachHang.Hinh = imgName;
                    }

                    _context.Update(khachHang);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KhachHangExists(khachHang.MaKh))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
           // }
          //  return View(khachHang);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khachHang = await _context.KhachHangs
                .FirstOrDefaultAsync(m => m.MaKh == id);
            if (khachHang == null)
            {
                return NotFound();
            }

            return View(khachHang);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var khachHang = await _context.KhachHangs.FindAsync(id);
            if (khachHang != null)
            {
                if (!string.Equals(khachHang.Hinh, "noImg.jpg"))
                {
                    string dir = Path.Combine(_environment.WebRootPath, "Hinh/KhachHang");
                    string oldfileImg = Path.Combine(dir, khachHang.Hinh);
                    if (System.IO.File.Exists(oldfileImg))
                    {
                        System.IO.File.Delete(oldfileImg);
                    }
                }
                _context.KhachHangs.Remove(khachHang);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KhachHangExists(string id)
        {
            return _context.KhachHangs.Any(e => e.MaKh == id);
        }
    }
}
