using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Database.Domain;
using Microsoft.AspNetCore.Mvc;

namespace OrderManager.Helpers
{
    public class ValidarDados
    {
        public bool validarPedido(Pedido pedido)
        {
            try
            {
                if (pedido.itens.Count < 1)
                {

                    throw new Exception("Pedido deve ter ao menos um item");
                }


                foreach (var item in pedido.itens)
                {

                    if (item.valor < 1)
                    {

                        throw new Exception("Valor do item deve ser maior que zero");
                    }
                    if (item.quantidade < 1)
                    {

                        throw new Exception("Quantidade do item deve ser maior que zero");

                    }
                    pedido.totalValor(item.valor * item.quantidade);
                }
                return true;
            } catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }
    }
}
