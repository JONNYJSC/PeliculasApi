using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculaAPI.Data;
using PeliculaAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeliculaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeliculaController : ControllerBase
    {
        private readonly AppDbContext _db;

        public PeliculaController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<Pelicula>))] // Si esta bien retorna una lista de tipo pelicula
        [ProducesResponseType(400)] // retorna Bad Request
        public async Task<IActionResult> GetPeliculas()
        {
            var lista = await _db.Peliculas.OrderBy(p => p.NombrePelicula).Include(p => p.Categoria).ToListAsync();

            return Ok(lista);
        }

        [HttpGet("{id:int}", Name = "GetPelicula")]
        [ProducesResponseType(200, Type = typeof(Pelicula))] // Si esta bien retorna un objeto de tipo pelicula
        [ProducesResponseType(400)] // retorna Bad Request
        [ProducesResponseType(404)] // Not Found (para cuando no encuentre algun objeto)
        public async Task<IActionResult> GetPelicula(int id)
        {
            var obj = await _db.Peliculas.OrderBy(p => p.Categoria).FirstOrDefaultAsync(p => p.Id == id);

            if (obj == null )
            {
                return NotFound();
            }

            return Ok(obj);
        }

        [HttpPost]
        [ProducesResponseType(201)] //Siempre responde con un status 201
        [ProducesResponseType(400)]
        [ProducesResponseType(500)] //Error interno
        public async Task<IActionResult> CrearPeliculas([FromBody] Pelicula pelicula)
        {
            if (pelicula == null)
            {
                return BadRequest(ModelState);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _db.AddAsync(pelicula);
            await _db.SaveChangesAsync();
            //return Ok(); // No puedo retornar un Ok porque hace referencia al status 200
            return CreatedAtRoute("GetPelicula", new { id = pelicula.Id }, pelicula); // status 201
            // para poder llamar el GetCategoria se le pone el nombre en el metodo [HttpGet("{id:int}", Name = "GetPelicula")]
        }
    }
}
