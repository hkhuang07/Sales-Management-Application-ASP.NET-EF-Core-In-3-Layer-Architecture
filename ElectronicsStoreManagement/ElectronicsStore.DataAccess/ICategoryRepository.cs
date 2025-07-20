using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicsStore.DataAccess
{
    public interface ICategoryRepository
    {
        List<Categories> GetAll();
        Categories? GetById(int id);
        void Add(Categories category);
        void Update(Categories category);
        void Delete(Categories category);

    }
}
