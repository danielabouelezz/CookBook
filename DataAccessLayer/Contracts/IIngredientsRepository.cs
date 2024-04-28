using DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Contracts
{
    public interface IIngredientsRepository
    {
        public Task AddIngredient(Ingredient ingredient);
        public Task<List<Ingredient>> GetIngredient(string? name = "");
        public Task DeleteIngredient(Ingredient ingredient);
        public Task EditIngredient(Ingredient ingredient);
    }
}
