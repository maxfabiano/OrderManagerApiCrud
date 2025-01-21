using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Database.Domain;
using Microsoft.AspNetCore.Mvc;

namespace CadastroDeProdutosNovo.Helpers
{
    public class ValidarDados
    {
        public bool validarProduto(Produto produto)
        {
            try
            {
                if (produto.itens.Count < 1)
                {
                    throw new Exception("Produto deve ter ao menos um item");
                }

                foreach (var item in produto.itens)
                {

                    if (item.valor <= 0)
                    {
                        throw new Exception("Valor do item deve ser maior que zero");
                    }
                    if (item.quantidade <= 0)
                    {
                        throw new Exception("Quantidade do item deve ser maior que zero");
                    }
                    produto.totalValor(item.valor * item.quantidade);
                }
                return true;
            } catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
