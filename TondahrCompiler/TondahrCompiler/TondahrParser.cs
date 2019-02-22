using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TondahrCompiler
{
    public class TondahrParser
    {
        TondahrLexer _tondahrLexer;
        List<TokenPrecedence> tokenPrecedences = new List<TokenPrecedence>();
         List<Variable> variables;
        int keywordIndex;
        public TondahrParser(TondahrLexer tondahrLexer, ref List<Variable> _variables)
        {
            _tondahrLexer = tondahrLexer;
            variables = _variables;
        }

        public List<TokenPrecedence> ABST()
        {
            var tokens = _tondahrLexer.GetTokens();
            foreach(var token in tokens)
            {
                if (token.Symbol.Equals("Add")) tokenPrecedences.Add(new TokenPrecedence { Order = 5, Token = token });
                if (token.Symbol.Equals("Sub")) tokenPrecedences.Add(new TokenPrecedence { Order = 6, Token = token });
                if (token.Symbol.Equals("Mul")) tokenPrecedences.Add(new TokenPrecedence { Order = 4, Token = token });
                if (token.Symbol.Equals("Div")) tokenPrecedences.Add(new TokenPrecedence { Order = 3, Token = token });
                if (token.Symbol.Equals("Equ")) tokenPrecedences.Add(new TokenPrecedence { Order = 0, Token = token });
                if (token.Symbol.Equals("Integer")) tokenPrecedences.Add(new TokenPrecedence {Order = 16, Token = token });
                if (token.Symbol.Equals("String")) tokenPrecedences.Add(new TokenPrecedence { Order = 17, Token = token });
                if (token.Symbol.Equals("Print")) tokenPrecedences.Add(new TokenPrecedence { Order = 1, Token = token });
                if (token.Symbol.Equals("LoopBegin")) tokenPrecedences.Add(new TokenPrecedence { Order = 23, Token = token });
                if (token.Symbol.Equals("MLoop")) tokenPrecedences.Add(new TokenPrecedence { Order = 25, Token = token });
                if (token.Symbol.Equals("LoopEnd")) tokenPrecedences.Add(new TokenPrecedence { Order = 24, Token = token });
                if (token.Symbol.Equals("PassExp")) tokenPrecedences.Add(new TokenPrecedence { Order = -1, Token = token });
                if (token.Symbol.Equals("Comp")) tokenPrecedences.Add(new TokenPrecedence { Order = 2, Token = token});
                if (token.Symbol.Equals("lessThan")) tokenPrecedences.Add(new TokenPrecedence { Order = 7, Token = token });
                if (token.Symbol.Equals("grtThan")) tokenPrecedences.Add(new TokenPrecedence { Order = 8, Token = token });
                if (token.Symbol.Equals("ntEquTo")) tokenPrecedences.Add(new TokenPrecedence { Order = 9, Token = token });
                if (token.Symbol.Equals("equTo")) tokenPrecedences.Add(new TokenPrecedence { Order = 10, Token = token });
                if (token.Symbol.Equals("gtEqulTo")) tokenPrecedences.Add(new TokenPrecedence { Order = 11, Token = token });
                if (token.Symbol.Equals("lsEquTo")) tokenPrecedences.Add(new TokenPrecedence { Order = 12, Token = token });
                if (token.Symbol.Equals("Put")) tokenPrecedences.Add(new TokenPrecedence { Order = 13, Token = token });
                if (token.Symbol.Equals("SLiteral")) tokenPrecedences.Add(new TokenPrecedence { Order = 18, Token = token });
            }

            return tokenPrecedences;
        }

        public string ExecuteTree()
        {
            var output = 0;

            if (tokenPrecedences.Exists(e => e.Order == 0))
            {
                if (output >= 0)
                {
                    
                    int operatorIndex = tokenPrecedences.IndexOf(tokenPrecedences.First(e => e.Order == 0));
                    string _var = tokenPrecedences[operatorIndex - 1].Token.Value;
                    tokenPrecedences.RemoveRange(0, 2);
                    if(tokenPrecedences.Count > 2)
                    {
                        output = FullExpressionEval(output, tokenPrecedences);
                    }
                    if (variables.Any(e => e.Name == _var))
                    {
                        var variable = variables.FirstOrDefault(e => e.Name == _var);
                        variable.Value = output.ToString();
                    }
                    else
                    {
                        variables.Add(new Variable
                        {
                            Name = _var,
                            Value = tokenPrecedences[0].Token.Value
                        });
                    }

                }
            }

            if(tokenPrecedences.Exists(e => e.Order == 1))
            {
                try {
                    int operatorIndex = tokenPrecedences.IndexOf(tokenPrecedences.First(e => e.Order == 1));
                    var _symbol = tokenPrecedences[operatorIndex + 1];
                    if(_symbol.Token.Symbol == "SLiteral")
                    {
                        tokenPrecedences.RemoveRange(0, 2);
                        tokenPrecedences.RemoveAt(tokenPrecedences.Count - 1);

                        string _literal = "";
                        foreach(var d in tokenPrecedences)
                        {
                            _literal += d.Token.Value + " ";
                        }

                        return _literal;
                    }
                    var variableValue = variables.FirstOrDefault(e => e.Name == _symbol.Token.Value);

                    if(tokenPrecedences.Count > 2)
                    {
                        tokenPrecedences.RemoveRange(0, 1);
                        return FullExpressionEval(output, tokenPrecedences).ToString();
                    }
                    else return variableValue.Value;
                } catch { };
                
            }

            // Initiates evaluation of bare expression not mapped to a variable  
            if (tokenPrecedences.Exists(e => e.Order == -1)) {
                tokenPrecedences.RemoveAt(0);
                output = FullExpressionEval(output, tokenPrecedences);
                return output.ToString();
            }
            
            return null;

        }

        public int FullExpressionEval(int output, List<TokenPrecedence> tokenPrecedences)
        {
            var operand1 = 0; var operand2 = 0;
            while (tokenPrecedences.Exists(e => e.Order == 5 || e.Order == 4 || e.Order == 3 || e.Order == 6 || e.Order == 24 || e.Order == 23 || e.Order == 25
                                            || e.Order == 7 || e.Order == 8 || e.Order == 9 || e.Order == 10 || e.Order == 11 || e.Order == 12 ))
            {
                if (tokenPrecedences.Any(e => e.Order == 3) && tokenPrecedences.Min(e => e.Order) == 3)
                {
                    int operatorIndex = tokenPrecedences.IndexOf(tokenPrecedences.First(e => e.Order == 3));
                    if (operatorIndex == 1)
                    {
                        if (tokenPrecedences.Count == 3)
                        {
                            bool dynamicVar = variables.Exists(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value);
                            bool dynamicVar2 = variables.Exists(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value);

                            if (dynamicVar && dynamicVar2)
                            {
                                var val1 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value);
                                var val2 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value);


                                output += int.Parse(val1.Value) / int.Parse(val2.Value);
                            }

                            if (dynamicVar && !dynamicVar2)
                            {
                                var val1 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value);
                                operand2 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);

                                output += int.Parse(val1.Value) / operand2;
                            }

                            if (!dynamicVar && dynamicVar2)
                            {
                                var val1 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value);
                                operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);

                                output += operand1 / int.Parse(val1.Value);
                            }

                            if (!(dynamicVar || dynamicVar2))
                            {
                                operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);
                                operand2 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);
                                output += operand1 / operand2;
                            }
                        }

                        if (tokenPrecedences.Count > 3)
                        {
                            string isInteger = tokenPrecedences[operatorIndex + 2].Token.Symbol;
                            if (isInteger == "Integer")
                            {
                                var is_variableInMemory = variables.Exists(e => e.Name == tokenPrecedences[operatorIndex + 2].Token.Value);
                                if (is_variableInMemory)
                                {
                                    operand1 = int.Parse(variables.FirstOrDefault(e => e.Value == tokenPrecedences[operatorIndex - 1].Token.Value).Value);
                                }
                                else operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);

                                output = operand1 / output;
                            }
                            if (isInteger != "Integer")
                            {
                                //operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);
                                //operand2 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value); 

                                //output += operand1 / operand2;

                                bool dynamicVar = variables.Exists(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value);
                                bool dynamicVar2 = variables.Exists(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value);

                                if (dynamicVar && dynamicVar2)
                                {
                                    var val1 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value);
                                    var val2 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value);


                                    output += int.Parse(val1.Value) / int.Parse(val2.Value);
                                }

                                if (dynamicVar && !dynamicVar2)
                                {
                                    var val1 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value);
                                    operand2 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);

                                    output += int.Parse(val1.Value) / operand2;
                                }

                                if (!dynamicVar && dynamicVar2)
                                {
                                    var val1 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value);
                                    operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);

                                    output += operand1 / int.Parse(val1.Value);
                                }

                                if (!(dynamicVar || dynamicVar2))
                                {
                                    operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);
                                    operand2 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);
                                    output += operand1 / operand2;
                                }
                            }
                        }
                    }


                    if (operatorIndex > 1)
                    {
                        //bool isInteger = int.TryParse(tokenPrecedences[operatorIndex - 1].Token.Value, out int a);
                        string isInteger = tokenPrecedences[operatorIndex - 2].Token.Symbol;
                        if (isInteger == "Integer")
                        {
                            var is_operandInMemory = variables.Exists(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value);
                            if (is_operandInMemory)
                            {
                                operand1 = int.Parse(variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value).Value);
                            }
                            else operand1 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);

                            output = output / operand1;
                        }

                        if (isInteger != "Integer")
                        {
                            //int value = tokenPrecedences.IndexOf(tokenPrecedences[operatorIndex - 2]);
                           
                            operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);
                            operand2 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);
                            output += operand1 / operand2;
                        }
                    }

                    tokenPrecedences.RemoveAt(operatorIndex);
                }

                if (tokenPrecedences.Any(e => e.Order == 4) && tokenPrecedences.Min(e=>e.Order) == 4)
                {
                    int operatorIndex = tokenPrecedences.IndexOf(tokenPrecedences.First(e => e.Order == 4));
                    if(operatorIndex == 1)
                    {
                        if(tokenPrecedences.Count == 3)
                        {
                            bool dynamicVar = variables.Exists(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value);
                            bool dynamicVar2 = variables.Exists(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value);

                            if (dynamicVar && dynamicVar2)
                            {
                                var val1 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value);
                                var val2 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value);


                                output += int.Parse(val1.Value) * int.Parse(val2.Value);
                            }

                            if (dynamicVar && !dynamicVar2)
                            {
                                var val1 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value);
                                operand2 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);

                                output += int.Parse(val1.Value) * operand2;
                            }

                            if (!dynamicVar && dynamicVar2)
                            {
                                var val1 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value);
                                operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);

                                output += operand1 * int.Parse(val1.Value);
                            }

                            if (!(dynamicVar || dynamicVar2))
                            {
                                operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);
                                operand2 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);
                                output += operand1 * operand2;
                            }
                        }
                        if(tokenPrecedences.Count > 3)
                        {
                            string isInteger = tokenPrecedences[operatorIndex + 2].Token.Symbol;
                            if (isInteger == "Integer" || isInteger == "String")
                            {
                                bool dynamicVar = variables.Exists(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value);
                                if (dynamicVar)
                                {
                                    operand1 = int.Parse(variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value).Value);
                                }
                                else operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);
                                output = output * operand1;
                            }

                            if (isInteger != "Integer")
                            {
                                bool dynamicVar = variables.Exists(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value);
                                bool dynamicVar2 = variables.Exists(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value);

                                if (dynamicVar && dynamicVar2)
                                {
                                    var val1 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value);
                                    var val2 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value);


                                    output += int.Parse(val1.Value) * int.Parse(val2.Value);
                                }

                                if (dynamicVar && !dynamicVar2)
                                {
                                    var val1 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value);
                                    operand2 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);

                                    output += int.Parse(val1.Value) * operand2;
                                }

                                if (!dynamicVar && dynamicVar2)
                                {
                                    var val1 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value);
                                    operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);

                                    output += operand1 * int.Parse(val1.Value);
                                }

                                if (!(dynamicVar || dynamicVar2))
                                {
                                    operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);
                                    operand2 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);

                                    output += operand1 * operand2;
                                }
                            }

                        }
                    }

                    if(operatorIndex > 1)
                    {
                        //bool isInteger = int.TryParse(tokenPrecedences[operatorIndex - 1].Token.Value, out int a);
                        string isInteger = tokenPrecedences[operatorIndex - 2].Token.Symbol;
                        if (isInteger == "Integer" || isInteger == "String")
                        {
                            bool dynamicVar = variables.Exists(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value);
                            if (dynamicVar)
                            {
                                operand1 = int.Parse(variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value).Value);
                            }
                            operand1 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);
                            output = output * operand1;
                        }

                        if (isInteger != "Integer")
                        {
                            //int value = tokenPrecedences.IndexOf(tokenPrecedences[operatorIndex - 2]);
                            bool dynamicVar = variables.Exists(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value);
                            bool dynamicVar2 = variables.Exists(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value);

                            if (dynamicVar && dynamicVar2)
                            {
                                var val1 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value);
                                var val2 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value);


                                output += int.Parse(val1.Value) * int.Parse(val2.Value);
                            }

                            if (dynamicVar && !dynamicVar2)
                            {
                                var val1 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value);
                                operand2 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);

                                output += int.Parse(val1.Value) * operand2;
                            }

                            if (!dynamicVar && dynamicVar2)
                            {
                                var val1 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value);
                                operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);

                                output += operand1 * int.Parse(val1.Value);
                            }

                            if (!(dynamicVar || dynamicVar2))
                            {
                                operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);
                                operand2 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);

                                output += operand1 * operand2;
                            }
                        }
                    }
                    tokenPrecedences.RemoveAt(operatorIndex);
                }

                if (tokenPrecedences.Any(e => e.Order == 5) && tokenPrecedences.Min(e => e.Order) == 5)
                {
                    int operatorIndex = tokenPrecedences.IndexOf(tokenPrecedences.First(e => e.Order == 5));
                    if (operatorIndex == 1)
                    {
                        if(tokenPrecedences.Count == 3) {

                            bool dynamicVar = variables.Exists(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value);
                            bool dynamicVar2 = variables.Exists(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value);
                            
                            if(dynamicVar && dynamicVar2)
                            {
                                var val1 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value);
                                var val2 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value);
                         

                                output += int.Parse(val1.Value) + int.Parse(val2.Value);
                            }

                            if(dynamicVar && !dynamicVar2)
                            {
                                var val1 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value);
                                operand2 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);

                                output += int.Parse(val1.Value) + operand2;
                            }

                            if(!dynamicVar && dynamicVar2)
                            {
                                var val1 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value);
                                operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);

                                output += operand1 + int.Parse(val1.Value);
                            }

                            if(!(dynamicVar || dynamicVar2))
                            {
                                operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);
                                operand2 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);
                                output += operand1 + operand2;
                            }
                        }

                        if (tokenPrecedences.Count > 3)
                        {
                            string isInteger = tokenPrecedences[operatorIndex + 2].Token.Symbol;
                            if (isInteger == "Integer" || isInteger == "String")
                            {
                                bool dynamicVar = variables.Exists(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value);
                                if (dynamicVar)
                                {
                                    operand1 = int.Parse(variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value).Value);
                                }
                                else operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);
                                output =  operand1 + output;
                            }
                            if (isInteger != "Integer")
                            {
                                //operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);
                                //operand2 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);

                                //output += operand1 + operand2;

                                bool dynamicVar = variables.Exists(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value);
                                bool dynamicVar2 = variables.Exists(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value);

                                if (dynamicVar && dynamicVar2)
                                {
                                    var val1 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value);
                                    var val2 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value);


                                    output += int.Parse(val1.Value) + int.Parse(val2.Value);
                                }

                                if (dynamicVar && !dynamicVar2)
                                {
                                    var val1 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value);
                                    operand2 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);

                                    output += int.Parse(val1.Value) + operand2;
                                }

                                if (!dynamicVar && dynamicVar2)
                                {
                                    var val1 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value);
                                    operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);

                                    output += operand1 + int.Parse(val1.Value);
                                }

                                if (!(dynamicVar || dynamicVar2))
                                {
                                    operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);
                                    operand2 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);
                                    output += operand1 + operand2;
                                }
                            }
                        }
                    }

                    if (operatorIndex > 1)
                    {
                        //bool isInteger = int.TryParse(tokenPrecedences[operatorIndex - 1].Token.Value, out int a);
                        string isInteger = tokenPrecedences[operatorIndex - 2].Token.Symbol;
                        if (isInteger == "Integer" || isInteger == "String") // Check if the token 2 cell ahead is a variable or a numeric character
                        {
                            bool isVariableDynamic = variables.Exists(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value);
                            if (isVariableDynamic)
                            {
                                operand1 = int.Parse(variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value).Value);
                            }
                            else operand1 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);
                            output = output + operand1;
                        }

                        if (isInteger != "Integer")
                        {
                            bool dynamicVar = variables.Exists(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value);
                            bool dynamicVar2 = variables.Exists(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value);

                            if (dynamicVar && dynamicVar2)
                            {
                                var val1 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value);
                                var val2 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value);


                                output += int.Parse(val1.Value) + int.Parse(val2.Value);
                            }

                            if (dynamicVar && !dynamicVar2)
                            {
                                var val1 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value);
                                operand2 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);

                                output += int.Parse(val1.Value) + operand2;
                            }

                            if (!dynamicVar && dynamicVar2)
                            {
                                var val1 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value);
                                operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);

                                output += operand1 + int.Parse(val1.Value);
                            }

                            if (!(dynamicVar || dynamicVar2))
                            {
                                operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);
                                operand2 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);
                                output += operand1 + operand2;
                            }
                        }
                    }
                    tokenPrecedences.RemoveAt(operatorIndex);
                }

                if (tokenPrecedences.Any(e => e.Order == 6) && tokenPrecedences.Min(e => e.Order) == 6)
                {
                    int operatorIndex = tokenPrecedences.IndexOf(tokenPrecedences.First(e => e.Order == 6));
                    if (operatorIndex == 1)
                    {
                        if (tokenPrecedences.Count == 3)
                        {
                            bool dynamicVar = variables.Exists(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value);
                            bool dynamicVar2 = variables.Exists(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value);

                            if (dynamicVar && dynamicVar2)
                            {
                                var val1 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value);
                                var val2 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value);


                                output += int.Parse(val1.Value) - int.Parse(val2.Value);
                            }

                            if (dynamicVar && !dynamicVar2)
                            {
                                var val1 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value);
                                operand2 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);

                                output += int.Parse(val1.Value) - operand2;
                            }

                            if (!dynamicVar && dynamicVar2)
                            {
                                var val1 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value);
                                operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);

                                output += operand1 - int.Parse(val1.Value);
                            }

                            if (!(dynamicVar || dynamicVar2))
                            {
                                operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);
                                operand2 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);
                                output += operand1 - operand2;
                            }
                        }

                        if (tokenPrecedences.Count > 3)
                        {
                            string isInteger = tokenPrecedences[operatorIndex + 2].Token.Symbol;
                            if (isInteger == "Integer" || isInteger == "String")
                            {
                                bool is_InMemory = variables.Exists(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value);
                                if (is_InMemory)
                                {
                                    operand1 = int.Parse(variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value).Value);
                                }
                                else operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);
                                output = operand1 - output;
                            }
                            if (isInteger != "Integer")
                            {
                                bool dynamicVar = variables.Exists(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value);
                                bool dynamicVar2 = variables.Exists(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value);

                                if (dynamicVar && dynamicVar2)
                                {
                                    var val1 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value);
                                    var val2 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value);


                                    output += int.Parse(val1.Value) - int.Parse(val2.Value);
                                }

                                if (dynamicVar && !dynamicVar2)
                                {
                                    var val1 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value);
                                    operand2 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);

                                    output += int.Parse(val1.Value) - operand2;
                                }

                                if (!dynamicVar && dynamicVar2)
                                {
                                    var val1 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value);
                                    operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);

                                    output += operand1 - int.Parse(val1.Value);
                                }

                                if (!(dynamicVar || dynamicVar2))
                                {
                                    operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);
                                    operand2 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);
                                    output += operand1 - operand2;
                                }
                            }
                        }
                    }


                    if (operatorIndex > 1)
                    {
                        //bool isInteger = int.TryParse(tokenPrecedences[operatorIndex - 1].Token.Value, out int a);
                        string isInteger = tokenPrecedences[operatorIndex - 2].Token.Symbol;
                        if (isInteger == "Integer" || isInteger == "String")
                        {
                            bool is_InMemory = variables.Exists(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value);
                            if (is_InMemory)
                            {
                                operand1 = int.Parse(variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value).Value);
                            }
                            else operand1 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);
                            output = output - operand1;
                        }

                        if (isInteger != "Integer")
                        {
                            bool dynamicVar = variables.Exists(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value);
                            bool dynamicVar2 = variables.Exists(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value);

                            if (dynamicVar && dynamicVar2)
                            {
                                var val1 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value);
                                var val2 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value);


                                output += int.Parse(val1.Value) - int.Parse(val2.Value);
                            }

                            if (dynamicVar && !dynamicVar2)
                            {
                                var val1 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value);
                                operand2 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);

                                output += int.Parse(val1.Value) - operand2;
                            }

                            if (!dynamicVar && dynamicVar2)
                            {
                                var val1 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value);
                                operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);

                                output += operand1 - int.Parse(val1.Value);
                            }

                            if (!(dynamicVar || dynamicVar2))
                            {
                                operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);
                                operand2 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);
                                output += operand1 - operand2;
                            }
                        }
                    }

                    tokenPrecedences.RemoveAt(operatorIndex);
                }

                if(tokenPrecedences.Any(e => e.Order == 7) && tokenPrecedences.Min(e => e.Order) == 7)
                {
                    var operatorIndex = tokenPrecedences.IndexOf(tokenPrecedences.First(e => e.Order == 7));
                    if(operatorIndex == 1)
                    {
                        if (tokenPrecedences.Count == 3)
                        {
                            bool dynamicVar = variables.Exists(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value);
                            bool dynamicVar2 = variables.Exists(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value);

                            if (dynamicVar && dynamicVar2)
                            {
                                var val1 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value);
                                var val2 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value);

                                if (int.Parse(val1.Value) < int.Parse(val2.Value))
                                {
                                    output = 1;
                                }
                                else { output = 0; }
                            }

                            if (dynamicVar && !dynamicVar2)
                            {
                                var val1 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value);
                                operand2 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);

                                if (int.Parse(val1.Value) < operand2) output = 1;
                                else output = 0;
                            }

                            if (!dynamicVar && dynamicVar2)
                            {
                                var val1 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value);
                                operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);

                                if (operand1 < int.Parse(val1.Value)) output = 1;
                                else output = 0 ;
                            }

                            if (!(dynamicVar || dynamicVar2))
                            {
                                operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);
                                operand2 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);
                                if (operand1 < operand2) output = 1;
                                else output = 0;
                            }
                        }
                    }

                    tokenPrecedences.RemoveAt(operatorIndex);
                }

                if (tokenPrecedences.Any(e => e.Order == 8) && tokenPrecedences.Min(e => e.Order) == 8)
                {
                    var operatorIndex = tokenPrecedences.IndexOf(tokenPrecedences.First(e => e.Order == 8));
                    if (operatorIndex == 1)
                    {
                        if (tokenPrecedences.Count == 3)
                        {
                            bool dynamicVar = variables.Exists(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value);
                            bool dynamicVar2 = variables.Exists(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value);

                            if (dynamicVar && dynamicVar2)
                            {
                                var val1 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value);
                                var val2 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value);

                                if (int.Parse(val1.Value) > int.Parse(val2.Value))
                                {
                                    output = 1;
                                }
                                else { output = 0; }
                            }

                            if (dynamicVar && !dynamicVar2)
                            {
                                var val1 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value);
                                operand2 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);

                                if (int.Parse(val1.Value) > operand2) output = 1;
                                else output = 0;
                            }

                            if (!dynamicVar && dynamicVar2)
                            {
                                var val1 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value);
                                operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);

                                if (operand1 > int.Parse(val1.Value)) output = 1;
                                else output = 0;
                            }

                            if (!(dynamicVar || dynamicVar2))
                            {
                                operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);
                                operand2 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);
                                if (operand1 > operand2) output = 1;
                                else output = 0;
                            }
                        }
                    }

                    tokenPrecedences.RemoveAt(operatorIndex);
                }

                if (tokenPrecedences.Any(e => e.Order == 9) && tokenPrecedences.Min(e => e.Order) == 9)
                {
                    var operatorIndex = tokenPrecedences.IndexOf(tokenPrecedences.First(e => e.Order == 9));
                    if (operatorIndex == 1)
                    {
                        if (tokenPrecedences.Count == 3)
                        {
                            bool dynamicVar = variables.Exists(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value);
                            bool dynamicVar2 = variables.Exists(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value);

                            if (dynamicVar && dynamicVar2)
                            {
                                var val1 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value);
                                var val2 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value);

                                if (int.Parse(val1.Value) != int.Parse(val2.Value))
                                {
                                    output = 1;
                                }
                                else { output = 0; }
                            }

                            if (dynamicVar && !dynamicVar2)
                            {
                                var val1 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value);
                                operand2 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);

                                if (int.Parse(val1.Value) != operand2) output = 1;
                                else output = 0;
                            }

                            if (!dynamicVar && dynamicVar2)
                            {
                                var val1 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value);
                                operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);

                                if (operand1 != int.Parse(val1.Value)) output = 1;
                                else output = 0;
                            }

                            if (!(dynamicVar || dynamicVar2))
                            {
                                operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);
                                operand2 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);
                                if (operand1 != operand2) output = 1;
                                else output = 0;
                            }
                        }
                    }

                    tokenPrecedences.RemoveAt(operatorIndex);
                }

                if (tokenPrecedences.Any(e => e.Order == 10) && tokenPrecedences.Min(e => e.Order) == 10)
                {
                    var operatorIndex = tokenPrecedences.IndexOf(tokenPrecedences.First(e => e.Order == 10));
                    if (operatorIndex == 1)
                    {
                        if (tokenPrecedences.Count == 3)
                        {
                            bool dynamicVar = variables.Exists(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value);
                            bool dynamicVar2 = variables.Exists(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value);

                            if (dynamicVar && dynamicVar2)
                            {
                                var val1 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value);
                                var val2 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value);

                                if (int.Parse(val1.Value) == int.Parse(val2.Value))
                                {
                                    output = 1;
                                }
                                else { output = 0; }
                            }

                            if (dynamicVar && !dynamicVar2)
                            {
                                var val1 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value);
                                operand2 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);

                                if (int.Parse(val1.Value) == operand2) output = 1;
                                else output = 0;
                            }

                            if (!dynamicVar && dynamicVar2)
                            {
                                var val1 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value);
                                operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);

                                if (operand1 == int.Parse(val1.Value)) output = 1;
                                else output = 0;
                            }

                            if (!(dynamicVar || dynamicVar2))
                            {
                                operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);
                                operand2 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);
                                if (operand1 == operand2) output = 1;
                                else output = 0;
                            }
                        }
                    }

                    tokenPrecedences.RemoveAt(operatorIndex);
                }

                if (tokenPrecedences.Any(e => e.Order == 11) && tokenPrecedences.Min(e => e.Order) == 11)
                {
                    var operatorIndex = tokenPrecedences.IndexOf(tokenPrecedences.First(e => e.Order == 11));
                    if (operatorIndex == 1)
                    {
                        if (tokenPrecedences.Count == 3)
                        {
                            bool dynamicVar = variables.Exists(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value);
                            bool dynamicVar2 = variables.Exists(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value);

                            if (dynamicVar && dynamicVar2)
                            {
                                var val1 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value);
                                var val2 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value);

                                if (int.Parse(val1.Value) >= int.Parse(val2.Value))
                                {
                                    output = 1;
                                }
                                else { output = 0; }
                            }

                            if (dynamicVar && !dynamicVar2)
                            {
                                var val1 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value);
                                operand2 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);

                                if (int.Parse(val1.Value) >= operand2) output = 1;
                                else output = 0;
                            }

                            if (!dynamicVar && dynamicVar2)
                            {
                                var val1 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value);
                                operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);

                                if (operand1 >= int.Parse(val1.Value)) output = 1;
                                else output = 0;
                            }

                            if (!(dynamicVar || dynamicVar2))
                            {
                                operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);
                                operand2 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);
                                if (operand1 >= operand2) output = 1;
                                else output = 0;
                            }
                        }
                    }

                    tokenPrecedences.RemoveAt(operatorIndex);
                }

                if (tokenPrecedences.Any(e => e.Order == 12) && tokenPrecedences.Min(e => e.Order) == 12)
                {
                    var operatorIndex = tokenPrecedences.IndexOf(tokenPrecedences.First(e => e.Order == 12));
                    if (operatorIndex == 1)
                    {
                        if (tokenPrecedences.Count == 3)
                        {
                            bool dynamicVar = variables.Exists(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value);
                            bool dynamicVar2 = variables.Exists(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value);

                            if (dynamicVar && dynamicVar2)
                            {
                                var val1 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value);
                                var val2 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value);

                                if (int.Parse(val1.Value) <= int.Parse(val2.Value))
                                {
                                    output = 1;
                                }
                                else { output = 0; }
                            }

                            if (dynamicVar && !dynamicVar2)
                            {
                                var val1 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex - 1].Token.Value);
                                operand2 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);

                                if (int.Parse(val1.Value) <= operand2) output = 1;
                                else output = 0;
                            }

                            if (!dynamicVar && dynamicVar2)
                            {
                                var val1 = variables.FirstOrDefault(e => e.Name == tokenPrecedences[operatorIndex + 1].Token.Value);
                                operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);

                                if (operand1 <= int.Parse(val1.Value)) output = 1;
                                else output = 0;
                            }

                            if (!(dynamicVar || dynamicVar2))
                            {
                                operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);
                                operand2 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);
                                if (operand1 <= operand2) output = 1;
                                else output = 0;
                            }
                        }
                    }

                    tokenPrecedences.RemoveAt(operatorIndex);
                }

                if (tokenPrecedences.Exists(e => e.Order == 23) && tokenPrecedences.Exists(e => e.Order == 24))
                {
                    try {
                        int operatorIndex = tokenPrecedences.IndexOf(tokenPrecedences.First(e => e.Order == 25));
                        List<int> temporaryStorage = new List<int>();
                        for (int i = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);
                                i <= int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value); i++)
                        {
                            if (tokenPrecedences.Any(e => e.Order == 13))
                            {
                                keywordIndex = tokenPrecedences.IndexOf(tokenPrecedences.First(e => e.Order == 13));
                                operand1 = int.Parse(tokenPrecedences[keywordIndex + 1].Token.Value);

                                temporaryStorage.Add(operand1);
                                tokenPrecedences.RemoveAt(keywordIndex);
                            }
                            else
                            {
                                if (operand1 == 0)
                                {
                                    temporaryStorage.Add(i);
                                }else if(operand1 > 0)
                                {
                                    operatorIndex = tokenPrecedences.IndexOf(tokenPrecedences.First(e => e.Order == 25));
                                    temporaryStorage.Add(operand1);
                                }
                            }

                        }


                        output += temporaryStorage.Sum();
                        tokenPrecedences.RemoveAt(tokenPrecedences.IndexOf(tokenPrecedences.First(e => e.Order == 25))); // remove :
                        tokenPrecedences.RemoveAt(tokenPrecedences.IndexOf(tokenPrecedences.First(e => e.Order == 23))); // remove [
                        tokenPrecedences.RemoveAt(tokenPrecedences.IndexOf(tokenPrecedences.First(e => e.Order == 24))); // remove ]
                    }
                    catch (Exception ex)
                    {
                        
                    }
                  }
            }

            return output;
        }

        public int ArithmeticExpressionEval(int output)
        {
            var operand1 = 0; var operand2 = 0;
            while (tokenPrecedences.Exists(e => e.Order == 5 || e.Order == 4 || e.Order == 3 || e.Order == 6 || e.Order == 24 || e.Order == 23 || e.Order == 25))
            {
                if (tokenPrecedences.Any(e => e.Order == 3) && tokenPrecedences.Min(e => e.Order) == 3)
                {
                    int operatorIndex = tokenPrecedences.IndexOf(tokenPrecedences.First(e => e.Order == 3));
                    if (operatorIndex == 1)
                    {
                        if (tokenPrecedences.Count == 3)
                        {
                            operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);
                            operand2 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);

                            output += operand1 / operand2;
                        }

                        if (tokenPrecedences.Count > 3)
                        {
                            string isInteger = tokenPrecedences[operatorIndex + 2].Token.Symbol;
                            if (isInteger == "Integer")
                            {
                                operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);
                                output = operand1 / output;
                            }
                            if (isInteger != "Integer")
                            {
                                operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);
                                operand2 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);

                                output += operand1 / operand2;
                            }
                        }
                    }


                    if (operatorIndex > 1)
                    {
                        //bool isInteger = int.TryParse(tokenPrecedences[operatorIndex - 1].Token.Value, out int a);
                        string isInteger = tokenPrecedences[operatorIndex - 2].Token.Symbol;
                        if (isInteger == "Integer")
                        {
                            operand1 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);
                            output = output / operand1;
                        }

                        if (isInteger != "Integer")
                        {
                            //int value = tokenPrecedences.IndexOf(tokenPrecedences[operatorIndex - 2]);
                            operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);
                            operand2 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);
                            output += operand1 / operand2;
                        }
                    }

                    tokenPrecedences.RemoveAt(operatorIndex);
                }

                if (tokenPrecedences.Any(e => e.Order == 4) && tokenPrecedences.Min(e => e.Order) == 4)
                {
                    int operatorIndex = tokenPrecedences.IndexOf(tokenPrecedences.First(e => e.Order == 4));
                    if (operatorIndex == 1)
                    {
                        if (tokenPrecedences.Count == 3)
                        {
                            operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);
                            operand2 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);

                            output += operand1 * operand2;
                        }
                        if (tokenPrecedences.Count > 3)
                        {
                            string isInteger = tokenPrecedences[operatorIndex + 2].Token.Symbol;
                            if (isInteger == "Integer")
                            {
                                operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);
                                output = output * operand1;
                            }

                            if (isInteger != "Integer")
                            {
                                operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);
                                operand2 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);

                                output += operand1 * operand2;
                            }

                        }
                    }

                    if (operatorIndex > 1)
                    {
                        //bool isInteger = int.TryParse(tokenPrecedences[operatorIndex - 1].Token.Value, out int a);
                        string isInteger = tokenPrecedences[operatorIndex - 2].Token.Symbol;
                        if (isInteger == "Integer")
                        {
                            operand1 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);
                            output = output * operand1;
                        }

                        if (isInteger != "Integer")
                        {
                            //int value = tokenPrecedences.IndexOf(tokenPrecedences[operatorIndex - 2]);
                            operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);
                            operand2 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);
                            output += operand1 * operand2;
                        }
                    }
                    tokenPrecedences.RemoveAt(operatorIndex);
                }

                if (tokenPrecedences.Any(e => e.Order == 5) && tokenPrecedences.Min(e => e.Order) == 5)
                {
                    int operatorIndex = tokenPrecedences.IndexOf(tokenPrecedences.First(e => e.Order == 5));
                    //operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);
                    //operand2 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);
                    //output += operand1 + operand2;
                    if (operatorIndex == 1)
                    {
                        if (tokenPrecedences.Count == 3)
                        {
                            operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);
                            operand2 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);

                            output += operand1 + operand2;
                        }

                        if (tokenPrecedences.Count > 3)
                        {
                            string isInteger = tokenPrecedences[operatorIndex + 2].Token.Symbol;
                            if (isInteger == "Integer")
                            {
                                operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);
                                output = operand1 + output;
                            }
                            if (isInteger != "Integer")
                            {
                                operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);
                                operand2 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);

                                output += operand1 + operand2;
                            }
                        }
                    }

                    if (operatorIndex > 1)
                    {
                        //bool isInteger = int.TryParse(tokenPrecedences[operatorIndex - 1].Token.Value, out int a);
                        string isInteger = tokenPrecedences[operatorIndex - 2].Token.Symbol;
                        if (isInteger == "Integer")
                        {
                            operand1 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);
                            output = output + operand1;
                        }

                        if (isInteger != "Integer")
                        {
                            //int value = tokenPrecedences.IndexOf(tokenPrecedences[operatorIndex - 2]);
                            operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);
                            operand2 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);
                            output += operand1 + operand2;
                        }
                    }
                    tokenPrecedences.RemoveAt(operatorIndex);
                }

                if (tokenPrecedences.Any(e => e.Order == 6) && tokenPrecedences.Min(e => e.Order) == 6)
                {
                    int operatorIndex = tokenPrecedences.IndexOf(tokenPrecedences.First(e => e.Order == 6));
                    if (operatorIndex == 1)
                    {
                        if (tokenPrecedences.Count == 3)
                        {
                            operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);
                            operand2 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);

                            output += operand1 - operand2;
                        }

                        if (tokenPrecedences.Count > 3)
                        {
                            string isInteger = tokenPrecedences[operatorIndex + 2].Token.Symbol;
                            if (isInteger == "Integer")
                            {
                                operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);
                                output = operand1 - output;
                            }
                            if (isInteger != "Integer")
                            {
                                operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);
                                operand2 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);

                                output += operand1 - operand2;
                            }
                        }
                    }


                    if (operatorIndex > 1)
                    {
                        //bool isInteger = int.TryParse(tokenPrecedences[operatorIndex - 1].Token.Value, out int a);
                        string isInteger = tokenPrecedences[operatorIndex - 2].Token.Symbol;
                        if (isInteger == "Integer")
                        {
                            operand1 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);
                            output = output - operand1;
                        }

                        if (isInteger != "Integer")
                        {
                            //int value = tokenPrecedences.IndexOf(tokenPrecedences[operatorIndex - 2]);
                            operand1 = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);
                            operand2 = int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value);
                            output += operand1 - operand2;
                        }
                    }

                    tokenPrecedences.RemoveAt(operatorIndex);
                }



                if (tokenPrecedences.Exists(e => e.Order == 23) && tokenPrecedences.Exists(e => e.Order == 24))
                {
                    try
                    {
                        int operatorIndex = tokenPrecedences.IndexOf(tokenPrecedences.First(e => e.Order == 25));

                        for (int i = int.Parse(tokenPrecedences[operatorIndex - 1].Token.Value);
                                i <= int.Parse(tokenPrecedences[operatorIndex + 1].Token.Value); i++)
                        {
                            output += i;
                        }

                        tokenPrecedences.RemoveAt(operatorIndex); // remove :
                        tokenPrecedences.RemoveAt(tokenPrecedences.IndexOf(tokenPrecedences.First(e => e.Order == 23))); // remove [
                        tokenPrecedences.RemoveAt(tokenPrecedences.IndexOf(tokenPrecedences.First(e => e.Order == 24))); // remove ]
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }

            return output;
        }
    }

    public class TokenPrecedence
    {
        public Token Token { get; set; }
        public int Order { get; set; }
    }

    public class Variable
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
