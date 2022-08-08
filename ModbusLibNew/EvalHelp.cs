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
            try
            {
                if (formula == "" || formula == null) return data;
                var interpreter = new Interpreter();
                string res = formula.Replace("raw", data.ToString());
                var result = interpreter.Eval(res);
                if (result != null) return result.ToString();
            }
            catch (Exception ex)
            {
            }
            return "";
        }
    }
}
