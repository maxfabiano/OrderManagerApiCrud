using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Domain
{
    public class Produto
    {
        public int id { get; private set; }
        public string nome { get; set; }
        public string data { get; set; }
        public decimal valorTotal { get; private set; }
        public List<Iten> itens { get; set; }

        public void totalValor(decimal value)
        {
            this.valorTotal += value;
        }
        public void setId(int _id)
        {
            this.id = _id;
        }
    }
    public class Iten
    {
        public int id { get;  private set; }
        public string nome { get; set; }
        public decimal valor { get; set; }
        public int quantidade { get; set; }
        public int produtoId { get; private set; }

    }

}
