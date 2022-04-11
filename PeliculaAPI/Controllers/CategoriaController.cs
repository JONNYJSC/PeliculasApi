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
    public class CategoriaController : ControllerBase
    {
        private readonly AppDbContext _db;

        public CategoriaController(AppDbContext db)
        {
            _db = db;
        }

        //Retorna todos los datos
        [HttpGet]
        [ProducesResponseType(200, Type =typeof(List<Categoria>))] // Si esta bien retorna una lista de tipo categoria
        [ProducesResponseType(400)] // retorna Bad Request
        public async Task<ActionResult> GetCategorias()
        {
            var lista = await _db.Categorias.OrderBy(c => c.Nombre).ToListAsync();

            return Ok(lista);
        }

        //Retorna un dato por Id
        [HttpGet("{id:int}", Name = "GetCategoria")] //para diferenciar los get ("{id:int}")
        [ProducesResponseType(200, Type = typeof(Categoria))] // Si esta bien retorna un objeto de tipo categoria
        [ProducesResponseType(400)] // retorna Bad Request
        [ProducesResponseType(404)] // Not Found (para cuando no encuentre algun objeto)
        public async Task<ActionResult> GetCategoria(int id)
        {
            var obj = await _db.Categorias.FirstOrDefaultAsync(c => c.Id == id);

            if (obj == null)
            {
                return NotFound();
            }

            return Ok(obj);
        }

        //Ingresar datos
        [HttpPost]
        [ProducesResponseType(201)] //Siempre responde con un status 201
        [ProducesResponseType(400)]
        [ProducesResponseType(500)] //Error interno
        public async Task<ActionResult> CrearCategoria([FromBody] Categoria categoria)
        {
            //confirmar categoria no este vacio
            if (categoria == null)
            {
                return BadRequest(ModelState);
            }

            //si no es valido
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _db.AddAsync(categoria);
            await _db.SaveChangesAsync();
            //return Ok(); // No puedo retornar un Ok porque hace referencia al status 200
            return CreatedAtRoute("GetCategoria", new { id = categoria.Id }, categoria); // status 201
            // para poder llamar el GetCategoria se le pone el nombre en el metodo [HttpGet("{id:int}", Name = "GetCategoria")]
        }
    }
}
