using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TondahrCompiler
{
    class Program
    {
        static List<Variable> variables = new List<Variable>();
        
        static void Main(string[] args)
        {
            string output = "";
            string[] filenameArray;
            string filename = "C:";
            bool exit = false;
            DateTime time = DateTime.Now;
            StreamReader objReader;
            TondahrLexer tondahrLexer;
            TondahrParser tondahrParser;
            Console.WriteLine($"Tondhar Interpreter version 0.1 DateTime: {time}");
            while (!exit)
            {
                Console.Write(">>> ");
                string exp = Console.ReadLine();
                if(exp.Contains("wetara"))
                {
                    filenameArray = exp.Remove(0, 7).Split('\\');
                    foreach(var d in filenameArray)
                    {
                        filename += "\\" + d;
                    }
                    if (File.Exists(filename))
                    {
                        objReader = new StreamReader(filename);
                        do
                        {
                            exp = objReader.ReadLine() + "\r\n";
                             tondahrLexer = new TondahrLexer();
                            tondahrLexer.GenerateTokens(exp);
                            tondahrParser = new TondahrParser(tondahrLexer, ref variables);
                            tondahrParser.ABST();
                            output = tondahrParser.ExecuteTree();
                        } while (objReader.Peek() != -1);

                        objReader.Close();
                    }
                    else
                    {
                        exp = "0 + 0";
                        
                    }
                }

                if (exp.ToLower() == "kwusi") exit = true;
                tondahrLexer = new TondahrLexer();
                tondahrLexer.GenerateTokens(exp);
                tondahrParser = new TondahrParser(tondahrLexer, ref variables);
                tondahrParser.ABST();
                output = tondahrParser.ExecuteTree();
                if (output != null) Console.WriteLine(output);
                exp = null;
            }
            
        }
    }
}
