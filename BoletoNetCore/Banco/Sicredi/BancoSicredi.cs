using System.Collections.Generic;
using BoletoNetCore.Exceptions;
using System;

namespace BoletoNetCore
{
    internal sealed partial class BancoSicredi : BancoFebraban<BancoSicredi>, IBanco
    {
        public BancoSicredi()
        {
            Codigo = 748;
            Nome = "Sicredi";
            Digito = "X";
            IdsRetornoCnab400RegistroDetalhe = new List<string> { "1" };
            RemoveAcentosArquivoRemessa = true;
        }

        public void FormataBeneficiario()
        {
            var contaBancaria = Beneficiario.ContaBancaria;

            string agencia = contaBancaria.Agencia.Trim();
            if (!string.IsNullOrEmpty(contaBancaria.DigitoAgencia))
            {
                agencia = agencia
                    + contaBancaria.DigitoAgencia.Trim();
            }

            string beneficiario = Beneficiario.Codigo.Trim();
            if (!string.IsNullOrEmpty(Beneficiario.CodigoDV))
            {
                beneficiario = beneficiario
                    + Beneficiario.CodigoDV.Trim();
            }
            
            if (!CarteiraFactory<BancoSicredi>.CarteiraEstaImplementada(contaBancaria.CarteiraComVariacaoPadrao))
                throw BoletoNetCoreException.CarteiraNaoImplementada(contaBancaria.CarteiraComVariacaoPadrao);

            contaBancaria.FormatarDados("PAGÁVEL EM QUALQUER BANCO ATÉ O VENCIMENTO.", "", "", 9);

            Beneficiario.CodigoFormatado = $"{agencia.Substring(agencia.Length - 4, 4)}.{contaBancaria.OperacaoConta}.{beneficiario}";
        }

        public override string FormatarNomeArquivoRemessa(int sequencial)
        {
            var agora = DateTime.Now;

            var mes = agora.Month.ToString();
            if (mes == "10") mes = "O";
            if (mes == "11") mes = "N";
            if (mes == "12") mes = "D";
            var dia = agora.Day.ToString().PadLeft(2, '0');

            string beneficiario = Beneficiario.Codigo.Trim();
            if (!string.IsNullOrEmpty(Beneficiario.CodigoDV))
            {
                beneficiario = beneficiario
                    + Beneficiario.CodigoDV.Trim();
            }

            //Caso for gerado mais de um arquivo de remessa alterar a extensão do aquivo para "RM" + o contador do numero do arquivo de remessa gerado no dia
            var nomeArquivoRemessa = string.Format("{0}{1}{2}.{3}", beneficiario, mes, dia, "CRM");

            return nomeArquivoRemessa;
        }
       
    }
}