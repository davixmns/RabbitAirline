using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RabbitAirline.API.Models;
using RabbitAirline.API.Services;

namespace RabbitAirline.API.Controllers;

[ApiController]
[Route("[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IMessageProducer _messageProducer;

    //Banco em memória
    private static readonly List<Booking> Bookings = [];

    public BookingsController(IMessageProducer messageProducer)
    {
        _messageProducer = messageProducer;
    }


    [HttpPost]
    public IActionResult CreateBooking(Booking newBooking)
    {
        try
        {
            //Adiciona a nova reserva ao banco de dados
            Bookings.Add(newBooking);
            //Envia a mensagem para a fila RabbitMQ
            _messageProducer.SendMessage(newBooking);

            Console.WriteLine($"Nova reserva criada: {newBooking.PassengerName}");
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}