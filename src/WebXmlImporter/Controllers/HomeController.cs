using BusinessLogic.CQRS.Customers.Commands.Create;
using BusinessLogic.CQRS.Customers.Commands.Delete;
using BusinessLogic.CQRS.Customers.Commands.Update;
using BusinessLogic.CQRS.Customers.Queries.GetDetails;
using BusinessLogic.CQRS.Customers.Queries.GetList;
using BusinessLogic.CQRS.FileUpload.Commands.Import;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebXmlImporter.Models;

namespace WebXmlImporter.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger,
            IMediator mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var customerListVm = await _mediator.Send(new GetCustomerListQueryRequest());
            return View(customerListVm);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var customerQuery = new GetCustomerDetailQueryRequest() { Id = id };
            var vm = await _mediator.Send(customerQuery);
            return View(vm);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerCreateCommandRequest custCreateCmdRequest)
        {
            if (ModelState.IsValid)
            {
                var id = await _mediator.Send(custCreateCmdRequest);
                return RedirectToAction(nameof(Index));
            }

            var vm = new CustomerCreateCommandRequest();

            return View(vm);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            if (id == default)
            {
                return NotFound();
            }

            var customerQuery = new GetCustomerDetailQueryRequest() { Id = id };
            var vm = await _mediator.Send(customerQuery);           

            if (vm == null)
            {
                return NotFound();
            }

            return View(vm);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(Guid id, CustomerUpdateCommandRequest customerUpdateCommandRequest)
        {
            if (id == default)
            {
                return NotFound();
            }

            var customerQuery = new GetCustomerDetailQueryRequest() { Id = id };
            var vm = await _mediator.Send(customerQuery);

            if (vm != null)
            {
                await _mediator.Send(customerUpdateCommandRequest);
                try
                {
                    //save
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    //Log.Error(ex, "Unable to save changes ");
                }
            }
            return View(vm);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == default)
            {
                return NotFound();
            }

            var customerQuery = new GetCustomerDetailQueryRequest() { Id = id };
            var vm = await _mediator.Send(customerQuery);

            if (vm == null)
            {
                return NotFound();
            }

            return View(vm);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var deleteCustomerCommand = new CustomerDeleteCommandRequest() { Id = id };
            await _mediator.Send(deleteCustomerCommand);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UploadXmlAsync([FromForm] FileUploadCommandRequest content)
        {

            if (content.File != null)
            {
                try
                {
                    await _mediator.Send(content);
                }
                catch (Exception ex)
                {
                    //Log
                }
            }
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}