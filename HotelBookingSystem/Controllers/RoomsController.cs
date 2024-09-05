using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotelBookingSystem.Data;
using HotelBookingSystem.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace HotelBookingSystem
{
    public class RoomsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileServices fileServices;
        private readonly IWebHostEnvironment webHost;

        public RoomsController(ApplicationDbContext context,IFileServices fileServices,IWebHostEnvironment webHost)
        {
            _context = context;
            this.fileServices = fileServices;
            this.webHost = webHost;
        }


        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Rooms.Include(r => r.Branch).Include(r => r.RoomType);
            return View(await applicationDbContext.ToListAsync());
        }

    
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms
                .Include(r => r.Branch)
                .Include(r => r.RoomType)
                .FirstOrDefaultAsync(m => m.RoomID == id);
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }


        public IActionResult Create()
        {
            ViewData["BranchID"] = new SelectList(_context.Branches, "BranchID", "BranchName");
            ViewData["RoomTypeID"] = new SelectList(_context.RoomTypes, "RoomTypeID", "TypeName");
            return View();
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoomDTO RoomDTO)
        {
            var isFound = await _context.Rooms.AnyAsync(c => c.RoomNumber == RoomDTO.RoomNumber);
            if(isFound==true)
            {
                ModelState.AddModelError(nameof(RoomDTO.RoomNumber), "Dublicated Room Number");
                return View();
            }
            var ImageName = "";
            if (RoomDTO.file!=null)
            {

                ImageName =  fileServices.SaveFile(RoomDTO.file, Path.Combine(webHost.WebRootPath, "RoomImages"));
            }
            var Room = new Room()
            {

                BranchID= RoomDTO.BranchID,
                Imagepath= Path.Combine("RoomImages", ImageName),
                PricePerNight= RoomDTO.PricePerNight,
                RoomTypeID= RoomDTO.RoomTypeID,
                RoomNumber= RoomDTO.RoomNumber,
                Status= RoomDTO.Status,
                Note= RoomDTO.Note,
            };
            _context.Add(Room);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }
            ViewData["BranchID"] = new SelectList(_context.Branches, "BranchID", "BranchID", room.BranchID);
            ViewData["RoomTypeID"] = new SelectList(_context.RoomTypes, "RoomTypeID", "RoomTypeID", room.RoomTypeID);
            return View(room);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RoomID,RoomNumber,BranchID,RoomTypeID,PricePerNight,Status")] Room room)
        {
            if (id != room.RoomID)
            {
                return NotFound();
            }

           
                try
                {
                    _context.Update(room);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoomExists(room.RoomID))
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


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms
                .Include(r => r.Branch)
                .Include(r => r.RoomType)
                .FirstOrDefaultAsync(m => m.RoomID == id);
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room != null)
            {
                _context.Rooms.Remove(room);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RoomExists(int id)
        {
            return _context.Rooms.Any(e => e.RoomID == id);
        }


        [HttpGet("RoomDetailsForCustomer/{id}")]
        public async Task<IActionResult> RoomDetailsForCustomer([FromRoute]int id)
        {
            var Room=await _context.Rooms.Include(c=>c.Branch).FirstOrDefaultAsync(c=>c.RoomID == id);

            if(Room is null)
                return RedirectToAction(nameof(Index));

            return View(Room);
        }

        public async Task<IActionResult> GetAllBookRequest()
        {
            var BookRequests = await _context.requestForBooks.ToListAsync();
              return Ok(BookRequests);
        }
        [HttpGet]
        public IActionResult AllBookRequest()
        {
              return View();
        }


        [Authorize]
        [HttpGet("BookRoom/{id}")]
        public async Task<IActionResult> BookRoom(int id)
        {
            var Room=await _context.Rooms.FirstOrDefaultAsync(c=>c.RoomID==id);

            if(Room is null)    
                return RedirectToAction(nameof(Index));

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ViewBag.ImagePath = "\\" + Room.Imagepath;
            ViewBag.RoomId = id;
            ViewBag.UserId = userId;

            return View();
        }

        [Authorize]
        [HttpPost("Rooms/BookRoomData")]
        public async Task<IActionResult> BookRoomData([FromBody]RequestForBook requestForBook)
        {
            if(requestForBook == null)
                return RedirectToAction(nameof(Index));



            await _context.requestForBooks.AddAsync(requestForBook);
             var RowsUpdated=   await _context.SaveChangesAsync();
            if (RowsUpdated <= 0)
                return BadRequest();

            return Ok();

        }


        public async Task<IActionResult> GetRoomsWithStatus()
        {
            var rooms = await _context.Rooms.ToListAsync();

            return View(rooms);
        }

        [Authorize(Roles ="Admin")]
        [HttpPost]
        public async Task<IActionResult> RejectRequest(int id)
        {
            var Request = await _context.requestForBooks.FirstOrDefaultAsync(c => c.Id == id);
            if (Request is null)
                return BadRequest();

            _context.requestForBooks.Remove(Request);
           await _context.SaveChangesAsync();

            return Ok();
        }
        [Authorize(Roles ="Admin")]
        [HttpPost]

        public async Task<IActionResult> AcceptRequest(int id)
        {
            var Request = await _context.requestForBooks.FirstOrDefaultAsync(c => c.Id == id);
            if (Request is null)
                return BadRequest();
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomID == Request.RoomId);
            if (room is null)
                return BadRequest();

            room.isBooked = true;
            Request.isAccpted = true;
            _context.Rooms.Update(room);
           _context.requestForBooks.Update(Request);
           await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
