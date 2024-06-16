using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankLib.Services.Interfaces
{
    public interface IGenericCRUDInterface<T>
    {
        public T Create(T theObject);
        public T Read(int id);
        public T Update(T theObject, int id);
        public T Delete(int id);
        public List<T> GetAll();
    }
}
