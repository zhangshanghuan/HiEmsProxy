using DynamicExpresso;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusLibNew
{
    //http://dynamic-expresso.azurewebsites.net/
    public class EvalHelp
    {
        public string EvalMian(string data, string formula)
        {
            if (formula == "" || formula == null) return data;
            var interpreter = new Interpreter();
            string row = formula;
            string res = row.Replace("row", data.ToString());        
            return interpreter.Eval(res).ToString();          
        }
    }
}
