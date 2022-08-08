using DynamicExpresso;
using System.Data;
using System.Runtime.InteropServices;
using thinger.DataConvertLib;

namespace ModbusLibNew
{
    public  class baselib
    {
        EvalHelp _EvalHelp = new EvalHelp();
        /// <summary>
        /// 原始数据转换
        /// </summary>
        /// <param name="_SoureByteData">byte[]数据</param>
        /// <param name="datetype">数据转换类型</param>
        /// <param name="length">数据长度，bool用到</param>
        /// <param name="formula">转换公式 row*10</param>
        /// <returns></returns>
        public string DataConversion(byte[] _SoureByteData, DataFormatEnum FormatEnum, int length, int skipCount=0,string formula = "", DataFormat format = DataFormat.ABCD)
        {
            try
            {
                if (_SoureByteData == null) return "";
                ushort[] _SoureData = null;
                string[] listString = null;              
                switch (FormatEnum)
                {
                    case DataFormatEnum.Bool:
                    case DataFormatEnum.Bit:
                        bool[] resbool = BitLib.GetBitArrayFromByteArray(_SoureByteData.Skip(skipCount).Take(length).ToArray(), length);
                        return string.Join(",", resbool.ToArray());
                    case DataFormatEnum.Uint32:
                        _SoureData = UShortLib.GetUShortArrayFromByteArray(_SoureByteData).Skip(skipCount).Take(length).ToArray();                       
                        listString = new string[_SoureData.Length / 2];
                        for (int i = 0; i < listString.Length; i++)
                        {
                            string res = (((UInt32)_SoureData[i * 2] << 16) + _SoureData[i * 2 + 1]).ToString();
                            listString[i] = _EvalHelp.EvalMian(res, formula);
                        }
                        break;
                    default:
                    case DataFormatEnum.Uint16:
                        _SoureData = UShortLib.GetUShortArrayFromByteArray(_SoureByteData).Skip(skipCount).Take(length).ToArray();
                        listString = new string[_SoureData.Length];
                        for (int i = 0; i < _SoureData.Length; i++)
                        {
                            listString[i] = _EvalHelp.EvalMian(_SoureData[i].ToString(), formula);
                        }
                        break;
                        //case DataFormatEnum.Byte:
                        //    for (int i = 0; i < _SoureData.Length; i++)
                        //    {
                        //        listString[i] = _EvalHelp.EvalMian(_SoureData[i].ToString(), formula);
                        //    }
                        //    break;
                        //case DataFormatEnum.Short:
                        //    for (int i = 0; i < _SoureData.Length; i++)
                        //    {
                        //        listString[i] = _EvalHelp.EvalMian(_SoureData[i].ToString(), formula);
                        //    }
                        //    break;
                        //case DataFormatEnum.Int:
                        //    listString = new string[_SoureData.Length / 2];
                        //    for (int i = 0; i < listString.Length; i++)
                        //    {
                        //        string res = (((int)_SoureData[i * 2] << 16) + _SoureData[i * 2 + 1]).ToString();
                        //        listString[i] = _EvalHelp.EvalMian(res, formula);
                        //    }
                        //    break;

                        //case DataFormatEnum.Bit:
                        //    for (int i = 0; i < _SoureData.Length; i++)
                        //    {
                        //        listString[i] = _EvalHelp.EvalMian(_SoureData[i].ToString(), formula);
                        //    }
                        //    break;
                }
                return string.Join(",", listString.ToArray());
            }
            catch (Exception EX)
            {
                Console.WriteLine(EX);
            }
            return "";
        }

        public ushort[] StrToUshortArray(string str)
        {         
           return UShortLib.GetUShortArrayFromString(str);
        }
        public string[] StrToStrArray(string str, string split=",")
        {
            return str.Split(split);
        }
        public static void SwitchEndianness<T>(Span<T> dataset) where T : unmanaged
        {
            var size = Marshal.SizeOf<T>();
            var dataset_bytes = MemoryMarshal.Cast<T, byte>(dataset);
            for (int i = 0; i < dataset_bytes.Length; i += size)
            {
                for (int j = 0; j < size / 2; j++)
                {
                    var i1 = i + j;
                    var i2 = i - j + size - 1;
                    byte tmp = dataset_bytes[i1];
                    dataset_bytes[i1] = dataset_bytes[i2];
                    dataset_bytes[i2] = tmp;
                }
            }
        }
    }
}
