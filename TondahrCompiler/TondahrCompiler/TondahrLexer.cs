using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TondahrCompiler
{
    public class TondahrLexer
    {
        List<Token> Tokens = new List<Token>();

        public void GenerateTokens(string expression)
        {
            var a = expression.Split(new char[0]);
            foreach (string d in a)
            {
                var data = Regex.IsMatch(d.ToString(), @"[0-9]+");
                if (data) Tokens.Add(new Token { Symbol = "Integer", Value = d.ToString() });
                data = Regex.IsMatch(d.ToString(), @"[a-zA-Z]");
                if (data && d != "meghee_ya" && d != "mechie_ya" && d != "Kwuo_ihe_no_na" && d != "oghini_bu" && d != "ti_ba") Tokens.Add(new Token { Symbol = "String", Value = d.ToString() });
                if (!data || (d == "meghee_ya" || d == "mechie_ya" || d == "Kwuo_ihe_no_na" || d == "oghini_bu" || d == "ti_ba"))
                {
                    if(d == "+" ) { Tokens.Add(new Token { Symbol = "Add", Value = d}); }
                    if ( d == "-" ) { Tokens.Add(new Token { Symbol = "Sub", Value = d}); }
                    if (d == "/") { Tokens.Add(new Token { Symbol = "Div", Value = d}); }
                    if (d == "*") { Tokens.Add(new Token { Symbol = "Mul", Value = d}); }
                    if (d == "%") { Tokens.Add(new Token { Symbol = "Mod", Value = d}); }
                    if (d == "meghee_ya") { Tokens.Add(new Token { Symbol = "LoopBegin", Value = d}); }
                    if (d == "mechie_ya") { Tokens.Add(new Token { Symbol = "LoopEnd", Value = d}); }
                    if (d == "=") { Tokens.Add(new Token { Symbol = "Equ", Value = d}); }
                    if (d == "Kwuo_ihe_no_na") { Tokens.Add(new Token { Symbol = "Print", Value = d }); }
                    if (d == ":") { Tokens.Add(new Token { Symbol = "MLoop", Value = d }); }
                    if (d == "oghini_bu") { Tokens.Add(new Token { Symbol = "PassExp", Value = d }); }
                    if (d == "?") { Tokens.Add(new Token { Symbol = "Comp", Value = d }); }
                    if (d == "<") { Tokens.Add(new Token { Symbol = "lessThan", Value = d }); }
                    if (d == ">") { Tokens.Add(new Token { Symbol = "grtThan", Value = d }); }
                    if (d == "!="){ Tokens.Add(new Token { Symbol = "ntEquTo", Value = d }); }
                    if (d == "==") { Tokens.Add(new Token { Symbol = "equTo", Value = d }); }
                    if (d == ">=") { Tokens.Add(new Token { Symbol = "gtEquTo", Value = d }); }
                    if (d == "<=") { Tokens.Add(new Token { Symbol = "lsEquTo", Value = d }); }
                    if (d == "ti_ba") { Tokens.Add(new Token { Symbol = "Put", Value = d }); }
                    if (d == "\"") { Tokens.Add(new Token { Symbol = "SLiteral", Value = d }); }
                    if(d == "'") { Tokens.Add(new Token { Symbol = "CLiteral", Value = d }); }
                    // data = Regex.Match(d.ToString(), @"[^A-Za-z]");
                }
            }
        }

        public List<Token> GetTokens()
        {
            return Tokens;
        }
    }

    public class Token
    {
        public string Symbol { get; set; }
        public string Value { get; set; }
    }
}
