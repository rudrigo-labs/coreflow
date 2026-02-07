using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace CoreFlow.Shared.Extensions
{
    public static class DecimalExtensions
    {
        /// <summary>
        /// Formata número decimal como moeda brasileira, mantendo todas as casas fornecidas
        /// (sem arredondar além do informado).
        /// </summary>
        public static string ToBrazilianCurrency(this decimal? input, int casas = 2)
        {
            if (!input.HasValue) return string.Empty;

            var cultura = (CultureInfo)CultureInfo.GetCultureInfo("pt-BR").Clone();
            cultura.NumberFormat.CurrencySymbol = "R$";
            cultura.NumberFormat.CurrencyDecimalDigits = casas;

            // Usa "C" + casas em vez de "C2"
            return input.Value.ToString("C" + casas, cultura);
        }

        public static string ToBrazilianCurrency(this decimal? input)
        {
            if (!input.HasValue) return string.Empty;

            var valor = input.Value;

            // Conta quantas casas decimais esse decimal realmente tem
            int casasDecimais = BitConverter.GetBytes(decimal.GetBits(valor)[3])[2];

            // Monta o formato "Ncasas" (sem arredondar, só exibindo)
            var cultura = System.Globalization.CultureInfo.GetCultureInfo("pt-BR");
            string raw = valor.ToString("N" + casasDecimais, cultura);

            return "R$ " + raw;
        }

        public static string ToBrazilianCurrencyRaw(this decimal? input)
        {
            var cultura = CultureInfo.GetCultureInfo("pt-BR");
            return input.HasValue
                ? "R$ " + input.Value.ToString("0.################", cultura)
                : string.Empty;
        }
    }
}

