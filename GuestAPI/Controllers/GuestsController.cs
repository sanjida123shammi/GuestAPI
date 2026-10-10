using GuestAPI.Models;
using GuestAPI.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace GuestAPI.Controllers
{
    public class GuestsController : ApiController
    {
        private readonly GuestDbContext db = new GuestDbContext();

        [HttpGet]
        [Authorize(Roles = "Admin, User")]
        public IHttpActionResult GetGuests()
        {
            IEnumerable<Guest> guestList = db.Guests.Include(gf => gf.GuestFacilities).ToList();
            return Ok(guestList);
        }

        [HttpGet]
        [Authorize(Roles = "Admin, User")]
        public IHttpActionResult GetGuestById(int id)
        {
            Guest guest = db.Guests.Include(gf => gf.GuestFacilities).Where(g => g.GuestId == id).FirstOrDefault();
            return Ok(guest);
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult DeleteGuest(int id)
        {
            Guest guest = db.Guests.Find(id);
            var guestFacilities = db.GuestFacilities.Where(f => f.GuestId == id).ToList();

            foreach (var facility in guestFacilities)
            {
                db.GuestFacilities.Remove(facility);
            }
            db.Guests.Remove(guest);
            db.SaveChanges();
            return Ok("Deleted successfully");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult PostGuest(GuestRequest request)
        {
            if (request == null || request.Guest == null)
                return BadRequest("Guest not found");
            Guest obj = request.Guest;

            if (request.ImageFile != null && request.ImageFile.Length > 0)
            {
                obj.ImageUrl = GetImageFile(request.ImageFile);
            }
            db.Guests.Add(obj);
            db.SaveChanges();
            return Ok("Saved successfully");
        }

        [HttpPut]
        public IHttpActionResult PutGuest(int id, GuestRequest request)
        {
            if (request == null || request.Guest == null)
                return BadRequest("Guest not found");

            if (id != request.Guest.GuestId)
                return BadRequest("Id mismatch");

            Guest existingGuest = db.Guests.Include(f => f.GuestFacilities).FirstOrDefault(g => g.GuestId == id);
            if (existingGuest == null)
                return NotFound();

            if (request.ImageFile != null && request.ImageFile.Length > 0)
            {
                request.Guest.ImageUrl = GetImageFile(request.ImageFile);
            }

            existingGuest.GuestName = request.Guest.GuestName;
            existingGuest.Contact = request.Guest.Contact;
            existingGuest.CheckOut = request.Guest.CheckOut;
            existingGuest.IsPaid = request.Guest.IsPaid;

            var existingFacilities = db.GuestFacilities.Where(gf => gf.GuestId == id).ToList();
            db.GuestFacilities.RemoveRange(existingFacilities);

            if (request.Guest.GuestFacilities != null)
            {
                foreach (var facility in request.Guest.GuestFacilities)
                {
                    db.GuestFacilities.Add(new GuestFacility
                    {
                        FacilityId = facility.FacilityId,
                        TotalVisits = facility.TotalVisits,
                        Price = facility.Price,
                        GuestId = id
                    });
                }
            }
            db.SaveChanges();
            return Ok("Updated successfully");
        }

        private string GetImageFile(byte[] imageFile)
        {
            string imageUrl = "";
            if (imageFile != null)
            {
                string fileName = Guid.NewGuid() + ".jpg";
                string path = Path.Combine("/images", fileName);
                File.WriteAllBytes(HttpContext.Current.Server.MapPath(path), imageFile);
                imageUrl = path;
            }
            return imageUrl;
        }
    }
}
