using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace CoreFlow.Shared.Extensions
{
    public static class StringExtensions
    {
        public static string MaskCpfCnpj(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            var digits = new string(input.Where(char.IsDigit).ToArray());

            if (digits.Length == 11)
            {
                // CPF: 000.000.000-00
                return Convert.ToUInt64(digits).ToString(@"000\.000\.000\-00");
            }
            else if (digits.Length == 14)
            {
                // CNPJ: 00.000.000/0000-00
                return Convert.ToUInt64(digits).ToString(@"00\.000\.000\/0000\-00");
            }

            // fora dos padrï¿½es, retorna como veio
            return input;
        }

        public static string MaskPhone(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            var digits = new string(input.Where(char.IsDigit).ToArray());

            if (digits.Length == 10)
            {
                // fixo: (00) 0000-0000
                return Convert.ToUInt64(digits).ToString(@"\(00\) 0000\-0000");
            }
            else if (digits.Length == 11)
            {
                // celular: (00) 00000-0000
                return Convert.ToUInt64(digits).ToString(@"\(00\) 00000\-0000");
            }

            return input;
        }

        public static string MaskCep(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            var digits = new string(input.Where(char.IsDigit).ToArray());

            if (digits.Length == 8)
            {
                // CEP: 00000-000
                return Convert.ToUInt64(digits).ToString(@"00000\-000");
            }

            return input;
        }

        public static string GetFirstWord(this string texto)
        {
            return string.IsNullOrWhiteSpace(texto)
                ? string.Empty
                : texto.Split(' ')[0];
        }

        /// <summary>
        /// Remove todas as tags HTML da string.
        /// </summary>
        public static string RemoveHtmlTags(this string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Remove as tags HTML
            var withoutTags = Regex.Replace(input, "<.*?>", string.Empty);

            // Decodifica entidades HTML (ex: &nbsp;, &amp;, etc.)
            return System.Net.WebUtility.HtmlDecode(withoutTags).Trim();
        }

        public static T ToEnum<T>(this string value) where T : struct
        {
            return !Enum.TryParse<T>(value, out var enumeration) ? default : enumeration;
        }

        public static string ToInitials(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            var builder = new StringBuilder();

            var words = value.Split(" ");
            foreach (var word in words)
            {
                builder.Append(word.Substring(0, 1));
            }

            return builder.ToString().ToUpper();
        }

        public static string TrimStart(this string target, string trimString)
        {
            if (string.IsNullOrEmpty(trimString)) return target;

            string result = target;
            while (result.StartsWith(trimString))
            {
                result = result.Substring(trimString.Length);
            }

            return result;
        }

        public static string ConvertToISO(this string value)
        {

            Encoding isoEncoding = Encoding.GetEncoding("ISO-8859-1");
            Encoding utfEncoding = Encoding.UTF8;

            // Converte os bytes 
            byte[] bytesIso = utfEncoding.GetBytes(value);

            //  Obtï¿½m os bytes da string UTF 
            byte[] bytesUtf = Encoding.Convert(utfEncoding, isoEncoding, bytesIso);

            // Obtï¿½m a string ISO a partir do array de bytes convertido
            string textoISO = utfEncoding.GetString(bytesUtf);

            return textoISO;
        }

        public static string FormatPhone(this string value, bool celular)
        {
            value = value.Trim().Replace("(", "").Replace(")", "").Replace("-", "");
            if (value.Length > 2 && value.Substring(0, 2) == "55")
            {
                value = value.Substring(2);
            }

            if (celular && value.Length > 2)
            {
                if (value.Length > 2 && value.Length < 11)
                {
                    string ddd = value.Substring(0, 2),
                        telefone = value.Substring(2);

                    value = ddd + "9" + telefone;
                }

                return string.Format("{0:(##)#####-####}", Convert.ToInt64(value));

            }
            else if (value.Length == 10)
            {
                return string.Format("{0:(##)####-####}", Convert.ToInt64(value));
            }
            else
            {
                return string.Empty;
            }
        }

        public static bool IsLandLine(this string value)
        {
            var reg = @"[1-9]{2}[2-8]{1}[0-9]{3}[0-9]{4}";
            var ehValido = Regex.IsMatch(value, reg);
            return ehValido;
        }

        public static bool IsMobile(this string value)
        {
            var reg = @"[1-9]{2}[9]{0,1}[6-9]{1}[0-9]{3}[0-9]{4}";
            var ehValido = Regex.IsMatch(value, reg);
            return ehValido;
        }

        public static string FormatMobile(this string num)
        {
            var ddd = num.Substring(0, 2);
            var parte1 = string.Empty;
            var parte2 = string.Empty;

            if (num.Length == 10)
            {
                parte1 = $"9{num.Substring(2, 4)}";
                parte2 = $"{num.Substring(6, 4)}";
            }

            if (num.Length == 11)
            {
                parte1 = $"{num.Substring(2, 5)}";
                parte2 = $"{num.Substring(7, 4)}";
            }

            return $"({ddd}){parte1}-{parte2}";
        }

        public static string FormatLandLine(this string num)
        {
            var ddd = num.Substring(0, 2);
            var parte1 = num.Substring(2, 4);
            var parte2 = num.Substring(6, 4);

            return $"({ddd}){parte1}-{parte2}";
        }

        public static string OnlyNumbers(this string input)
        {
            return new string(input.Where(char.IsDigit).ToArray());
        }

        /// <summary>
        /// Insere espaï¿½o ao final de cada palavra
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string CamelCaseSpacing(this string s)
        {
            // Sourced from https://stackoverflow.com/questions/4488969/split-a-string-by-capital-letters.
            var r = new Regex(@"
            (?<=[A-Z])(?=[A-Z][a-z]) |
             (?<=[^A-Z])(?=[A-Z]) |
             (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);

            return r.Replace(s, " ");
        }

        /// <summary>
        /// Converte uma string no formato "dd/MM/yyyy" para DateTime.
        /// </summary>
        /// <param name="value">A string a ser convertida no formato "dd/MM/yyyy".</param>
        /// <returns>Um DateTime convertido da string.</returns>
        /// <exception cref="ArgumentNullException">Lanï¿½ada quando value ï¿½ null.</exception>
        /// <exception cref="ArgumentException">Lanï¿½ada quando o formato da string ï¿½ invï¿½lido.</exception>
        /// <exception cref="FormatException">Lanï¿½ada quando a string nï¿½o corresponde ao formato esperado.</exception>
        public static DateTime ToDateTime(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value), "Value cannot be null or empty.");

            var result = Regex.IsMatch(value, "([0-9]{2})/([0-9]{2})/([0-9]{4})");

            if (!result)
                throw new FormatException($"Invalid date format. Expected 'dd/MM/yyyy' but got: {value}");

            try
            {
                var day = value.Substring(0, 2);
                var month = value.Substring(3, 2);
                var year = value.Substring(6, 4);

                return new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), Convert.ToInt32(day));
            }
            catch (Exception ex) when (ex is ArgumentException || ex is FormatException || ex is ArgumentOutOfRangeException)
            {
                throw new FormatException($"Invalid date format or value: {value}", ex);
            }
        }

        /// <summary>
        /// Converte uma string no formato "dd/MM/yyyy" com hora para DateTime.
        /// </summary>
        /// <param name="value">A string de data no formato "dd/MM/yyyy".</param>
        /// <param name="time">A string de hora no formato "HH:mm" ou "HH:mm:ss".</param>
        /// <returns>Um DateTime convertido da string com data e hora.</returns>
        /// <exception cref="ArgumentNullException">Lanï¿½ada quando value ou time ï¿½ null.</exception>
        /// <exception cref="ArgumentException">Lanï¿½ada quando o formato da string ï¿½ invï¿½lido.</exception>
        /// <exception cref="FormatException">Lanï¿½ada quando a string nï¿½o corresponde ao formato esperado.</exception>
        public static DateTime ToDateTimeWithHour(this string value, string time)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value), "Value cannot be null or empty.");
            if (string.IsNullOrWhiteSpace(time))
                throw new ArgumentNullException(nameof(time), "Time cannot be null or empty.");

            var result = Regex.IsMatch(value, "([0-9]{2})/([0-9]{2})/([0-9]{4})");

            if (!result)
                throw new FormatException($"Invalid date format. Expected 'dd/MM/yyyy' but got: {value}");

            try
            {
                var day = value.Substring(0, 2);
                var month = value.Substring(3, 2);
                var year = value.Substring(6, 4);
                var hour = time.Substring(0, 2);
                var minute = time.Substring(3, 2);
                var second = time.Length > 5 ? time.Substring(6, 2) : "00";

                return new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), Convert.ToInt32(day), Convert.ToInt32(hour), Convert.ToInt32(minute), Convert.ToInt32(second));
            }
            catch (Exception ex) when (ex is ArgumentException || ex is FormatException || ex is ArgumentOutOfRangeException)
            {
                throw new FormatException($"Invalid date/time format or value. Date: {value}, Time: {time}", ex);
            }
        }

        public static string ToTitleCase(this string title)
        {
            TextInfo textInfo = new CultureInfo("pt-BR", false).TextInfo;

            return textInfo.ToTitleCase(title.ToLower());
        }

        public static string HtmlEncode(this string value)
        {
            // call the normal HtmlEncode first
            char[] chars = HttpUtility.HtmlEncode(value).ToCharArray();
            StringBuilder encodedValue = new StringBuilder();
            foreach (char c in chars)
            {
                if ((int)c > 127) // above normal ASCII
                    encodedValue.Append("&#" + (int)c + ";");
                else
                    encodedValue.Append(c);
            }
            return encodedValue.ToString();
        }

        public static string ToUF(this string value)
        {
            var data = string.Empty;

            if (value != string.Empty || value != null)
            {
                switch (value.ToUpper())
                {
                    case "ACRE":
                        data = "AC";
                        break;
                    case "ALAGOAS":
                        data = "AL";
                        break;
                    case "AMAZONAS":
                        data = "AM";
                        break;
                    case "AMAPï¿½":
                        data = "AP";
                        break;
                    case "BAHIA":
                        data = "BA";
                        break;
                    case "CEARï¿½":
                        data = "CE";
                        break;
                    case "DISTRITO FEDERAL":
                        data = "DF";
                        break;
                    case "ESPï¿½RITO SANTO":
                        data = "ES";
                        break;
                    case "GOIï¿½S":
                        data = "GO";
                        break;
                    case "MARANHï¿½O":
                        data = "MA";
                        break;
                    case "MINAS GERAIS":
                        data = "MG";
                        break;
                    case "MATO GROSSO DO SUL":
                        data = "MS";
                        break;
                    case "MATO GROSSO":
                        data = "MT";
                        break;
                    case "PARï¿½":
                        data = "PA";
                        break;
                    case "PARAï¿½BA":
                        data = "PB";
                        break;
                    case "PERNAMBUCO":
                        data = "PE";
                        break;
                    case "PIAUï¿½":
                        data = "PI";
                        break;
                    case "PARANï¿½":
                        data = "PR";
                        break;
                    case "RIO DE JANEIRO":
                        data = "RJ";
                        break;
                    case "RIO GRANDE DO NORTE":
                        data = "RN";
                        break;
                    case "RONDï¿½NIA":
                        data = "RO";
                        break;
                    case "RORAIMA":
                        data = "RR";
                        break;
                    case "RIO GRANDE DO SUL":
                        data = "RS";
                        break;
                    case "SANTA CATARINA":
                        data = "SC";
                        break;
                    case "SERGIPE":
                        data = "SE";
                        break;
                    case "Sï¿½O PAULO":
                        data = "SP";
                        break;
                    case "TOCANTï¿½NS":
                        data = "TO";
                        break;
                    default:
                        data = "";
                        break;
                }
            }

            return data;
        }

        public static string FormatCnpj(this string cnpj)
        {
            return Convert.ToUInt64(cnpj).ToString(@"00\.000\.000\/0000\-00");
        }

        public static string FormatCpf(this string cpf)
        {
            return Convert.ToUInt64(cpf).ToString(@"000\.000\.000\-00");
        }

        public static string RemoveFormatCpfCnpj(this string codigo)
        {
            return codigo.Replace(".", string.Empty).Replace("-", string.Empty).Replace("/", string.Empty);
        }

        public static string GenerateSlug(this string phrase)
        {
            string str = phrase.RemoveAccent().ToLower();
            // invalid chars           
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            // convert multiple spaces into one space   
            str = Regex.Replace(str, @"\s+", " ").Trim();
            // cut and trim 
            str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim();
            str = Regex.Replace(str, @"\s", "-"); // hyphens   
            return str;
        }

        public static string RemoveAccent(this string txt)
        {
            byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }

        public static string ToSlug(this string value)
        {
            return value.ToLower().Replace(" ", "-");
        }

        public static string RemoveSpecialCharacters(this string text, bool allowSpace = false)
        {
            string ret;

            ret = System.Text.RegularExpressions.Regex.Replace(text, allowSpace ? @"[^0-9a-zA-Zï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½Yï¿½ï¿½ï¿½ï¿½ï¿½ï¿½\s]+?" : @"[^0-9a-zA-Zï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½Yï¿½ï¿½ï¿½ï¿½ï¿½ï¿½]+?", string.Empty);

            return ret;
        }

        public static string RemoveWhiteSpace(this string text)
        {
            return text.Trim();
        }

        public static string RemoveDiacritics(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            StringBuilder sb = new StringBuilder();
            foreach (var t in text)
            {
                sb.Append(t > 255 ? t : s_Diacritics[t]);
            }

            return sb.ToString();
        }

        private static readonly char[] s_Diacritics = GetDiacritics();
        private static char[] GetDiacritics()
        {
            char[] accents = new char[256];

            for (int i = 0; i < 256; i++)
                accents[i] = (char)i;

            // á à â ã ä
            accents[225] = accents[224] = accents[226] = accents[227] = accents[228] = 'a';
            // Á À Â Ã Ä
            accents[193] = accents[192] = accents[194] = accents[195] = accents[196] = 'A';

            // é è ê ë
            accents[233] = accents[232] = accents[234] = accents[235] = 'e';
            // É È Ê Ë
            accents[201] = accents[200] = accents[202] = accents[203] = 'E';

            // í ì î ï
            accents[237] = accents[236] = accents[238] = accents[239] = 'i';
            // Í Ì Î Ï
            accents[205] = accents[204] = accents[206] = accents[207] = 'I';

            // ó ò ô õ ö
            accents[243] = accents[242] = accents[244] = accents[245] = accents[246] = 'o';
            // Ó Ò Ô Õ Ö
            accents[211] = accents[210] = accents[212] = accents[213] = accents[214] = 'O';

            // ú ù û ü
            accents[250] = accents[249] = accents[251] = accents[252] = 'u';
            // Ú Ù Û Ü
            accents[218] = accents[217] = accents[219] = accents[220] = 'U';

            // ç Ç
            accents[231] = 'c';
            accents[199] = 'C';

            // ñ Ñ
            accents[241] = 'n';
            accents[209] = 'N';

            // ý ÿ Ý
            accents[253] = accents[255] = 'y';
            accents[221] = 'Y';

            return accents;
        }

    }
}
