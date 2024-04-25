using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication1
{
    [Route("api/animals")]
    [ApiController]
    public class AnimalControl : ControllerBase
    {
        private readonly AnimalRepo _animalsRepository;

        public AnimalControl(AnimalRepo animalRepository)
        {
            _animalsRepository = animalRepository ?? throw new ArgumentNullException(nameof(animalRepository));
        }

        [HttpGet]
        public ActionResult<IEnumerable<Animal>> GetAnimals(string orderBy = "name")
        {
            var animals = _animalsRepository.GetRepoAnimals().ToList();

            switch (orderBy.ToLower())
            {
                case "description":
                    animals = animals.OrderBy(a => a.Description, StringComparer.OrdinalIgnoreCase).ToList();
                    break;
                case "category":
                    animals = animals.OrderBy(a => a.Category, StringComparer.OrdinalIgnoreCase).ToList();
                    break;
                case "area":
                    animals = animals.OrderBy(a => a.Area, StringComparer.OrdinalIgnoreCase).ToList();
                    break;
                default:
                    animals = animals.OrderBy(a => a.Name, StringComparer.OrdinalIgnoreCase).ToList();
                    break;
            }

            return animals;
        }

        [HttpPost]
        public ActionResult AddAnimal([FromBody] Animal newAnimal)
        {
            try
            {
                _animalsRepository.AddAnimal(newAnimal);
                return Ok("Zwierzę dodane pomyślnie.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Wystąpił błąd podczas dodawania zwierzęcia: {ex.Message}");
            }
        }

        [HttpPut("{idAnimal}")]
        public ActionResult UpdateAnimal(int idAnimal, [FromBody] Animal updatedAnimal)
        {
            try
            {
                var existingAnimal = _animalsRepository.GetRepoAnimals().FirstOrDefault(a => a.IdAnimal == idAnimal);

                if (existingAnimal == null)
                {
                    return NotFound("Nie znaleziono zwierzęcia o podanym ID.");
                }

                existingAnimal.Name = updatedAnimal.Name;
                existingAnimal.Description = updatedAnimal.Description;
                existingAnimal.Category = updatedAnimal.Category;
                existingAnimal.Area = updatedAnimal.Area;

                _animalsRepository.UpdateAnimal(existingAnimal);

                return Ok("Aktualizacja przebiegła pomyślnnie.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Wystąpił błąd podczas aktualizacji zwierzęcia: {ex.Message}");
            }
        }

        [HttpDelete("{idAnimal}")]
        public ActionResult DeleteAnimal(int idAnimal)
        {
            try
            {
                var existingAnimal = _animalsRepository.GetRepoAnimals().FirstOrDefault(a => a.IdAnimal == idAnimal);

                if (existingAnimal == null)
                {
                    return NotFound("Nie znaleziono zwierzęcia o podanym ID.");
                }

                _animalsRepository.DeleteAnimal(idAnimal);
                return Ok("Zwierzę zostało usunięte.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Wystąpił błąd podczas usuwania zwierzęcia: {ex.Message}");
            }
        }
    }
}
