using GlobalFests.Services;
using GlobalFests.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GlobalFests.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private readonly ITicketService _ticketService;
        private readonly IEventService _eventService; 

        public TicketsController(ITicketService ticketService, IEventService eventService)
        {
            _ticketService = ticketService;
            _eventService = eventService;
        }

        // GET: Checkout Page
        [HttpGet]
        public async Task<IActionResult> Checkout(int eventId)
        {
            var eventDetails = await _eventService.GetEventByIdAsync(eventId);
            if (eventDetails == null) return NotFound();

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userTicketCount = await _ticketService.GetUserTicketCountForEventAsync(userId, eventId);

            
            int maxAllowed = 4 - userTicketCount;
            if (maxAllowed <= 0)
            {
                TempData["ErrorMessage"] = "You have reached the maximum limit of 4 tickets for this event.";
                return RedirectToAction("Details", "Events", new { id = eventId });
            }

            var model = new CheckoutViewModel
            {
                EventId = eventDetails.Id,
                EventTitle = eventDetails.Title,
                EventPoster = eventDetails.Poster, 
                EventDate = eventDetails.StartDate,
                Venue = $"{eventDetails.City}, {eventDetails.CountryName}",
                PricePerTicket = eventDetails.TicketPrice ?? 0,
                StockAvailable = eventDetails.TicketAmount, 
                ExistingTicketsCount = userTicketCount,
                Quantity = 1 
            };

            return View(model);
        }

        // POST: Process Payment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Purchase(CheckoutViewModel model)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Re-validate limit
            var currentCount = await _ticketService.GetUserTicketCountForEventAsync(userId, model.EventId);
            if (currentCount + model.Quantity > 4)
            {
                TempData["ErrorMessage"] = $"You can only buy {4 - currentCount} more ticket(s).";
                return RedirectToAction(nameof(Checkout), new { eventId = model.EventId });
            }

            var success = await _ticketService.PurchaseTicketsAsync(model.EventId, userId, model.Quantity);

            if (success)
            {
                TempData["SuccessMessage"] = "Tickets purchased successfully!";
                return RedirectToAction(nameof(MyTickets));
            }
            else
            {
                TempData["ErrorMessage"] = "Purchase failed. Tickets might be sold out.";
                return RedirectToAction(nameof(Checkout), new { eventId = model.EventId });
            }
        }

        // GET: My Tickets List
        [HttpGet]
        public async Task<IActionResult> MyTickets()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var tickets = await _ticketService.GetTicketsByUserIdAsync(userId);

            return View(new MyTicketsViewModel { Tickets = tickets });
        }

        // POST: Cancel Ticket
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var success = await _ticketService.CancelTicketAsync(id, userId);

            if (success)
                TempData["SuccessMessage"] = "Ticket cancelled and refunded.";
            else
                TempData["ErrorMessage"] = "Could not cancel ticket.";

            return RedirectToAction(nameof(MyTickets));
        }
    }
}
