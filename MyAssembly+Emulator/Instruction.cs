using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
//using System.Collections.Generic;

namespace projectclass
{
    public abstract class Instruction
    {
        private uint InstructionData => MachineCode;
        public uint MachineCode { get; set; }
        public string pattern { get; }
        public byte OpCode => (byte)(MachineCode >> 24);
        private static byte NthByte(uint number, int bitIndex)
        {
            number >>= (bitIndex * 8);
            number = number & 255;
            return (byte)number;
        }
        public static uint[] Parse(string[] instruction, Dictionary<string, uint> dict)
        {
            Dictionary<string, byte> labeldictionary = new Dictionary<string, byte>();
            for (int i = 0; i < instruction.Length; i++)
            {
                if (instruction[i][instruction[i].Length - 1] == ':')
                {
                    labeldictionary.Add(instruction[i], (byte)i);
                    instruction[i] = "NOP 00 00 00";
                }

            }




            uint[] machinecodes = new uint[instruction.Length];
            for (int i = 0; i < instruction.Length; i++)
            {
                Match[] matches = new Match[]
                {

                 Regex.Match(input: instruction[i], pattern: "(ADD|SUB|DIV|MULT|MOD|AND|OR|XOR|GTE|LTE|EQ) R([0-9a-fA-F]+) R([0-9a-fA-F]+) R([0-9a-fA-F]+)"),
                 Regex.Match(input: instruction[i], pattern: "(COPY|LOAD|STORE|SHR|SHL|NOT) R([0-9a-fA-F]+) R([0-9a-fA-F]+) ([0][0])"),
                 Regex.Match(input: instruction[i], pattern: "(SET) R([0-9a-fA-F]+) ([0-9A-F][0-9A-F]) ([0][0])"),
                 Regex.Match(input: instruction[i], pattern: "(JMPT|JMPZ) (\\S+:) R([0-9a-fA-F]+) ([0][0])"),
                 Regex.Match(input: instruction[i], pattern: "(POP|PUSH) R([0-9a-fA-F]+) ([0][0]) ([0][0])"),
                 Regex.Match(input: instruction[i], pattern: "(JMP) (\\S+:) ([0][0]) ([0][0])"),
                 Regex.Match(input: instruction[i], pattern: "(NOP) ([0][0]) ([0][0]) ([0][0])"),


                 };
                foreach (Match match in matches)
                {
                    if (match.Success)
                    {
                        string[] match1 = new string[4];
                        for (int j = 0; j < match1.Length; j++)
                        {
                            match1[j] = match.Groups[j + 1].Value;
                        }

                        uint program = 0;
                        program += dict[match.Groups[1].Value];

                        if (match.Groups[1].Value == "JMP" || match.Groups[1].Value == "JMPT" || match.Groups[1].Value == "JMPZ")
                        {
                            program <<= 8;
                            program += labeldictionary[match1[1]];
                        }
                        else
                        {
                            program <<= 8;
                            program += uint.Parse(match1[1], System.Globalization.NumberStyles.HexNumber);
                        }
                        for (int k = 2; k < match1.Length; k++)
                        {
                            program <<= 8;
                            program += uint.Parse(match1[k], System.Globalization.NumberStyles.HexNumber);
                        }
                        machinecodes[i] = program;
                    }
                    ;
                }

            }
            return machinecodes;
            throw new Exception("Wrong Syntax");
        }
        public static string[] UnParse(uint[] machinecode, Dictionary<uint, string> dict, Dictionary<string, uint> dict2)
        {
            Dictionary<int, string> labeldictionary = new Dictionary<int, string>();
            string[] words = new string[machinecode.Length];
            int counter = 0;
            for (int k = 0; k < machinecode.Length; k++)
            {
                if (!(words[k] == "" || words[k] == null))
                {
                    continue;
                }
                
                byte[] parts = new byte[4];

                for (int i = 0; i < parts.Length; i++)
                {
                    parts[3 - i] = NthByte(machinecode[k], i);
                }

                string word = dict[parts[0]];
                int startpoint = 1;
                if (word == "JMP" || word == "JMPT" || word == "JMPZ")
                {
                    word += ' ';
                    if (labeldictionary.ContainsKey(parts[1]))
                    {
                        word += labeldictionary[parts[1]];
                    }
                    else
                    {
                        string temp = "";
                      
                        temp += 'L';
                        temp += counter.ToString();
                        temp += ':';
                        word += temp;
                        counter++;
                        words[parts[1]] = temp;
                        labeldictionary.Add(parts[1], temp);
                    }

                    startpoint++;
                }
                for (int i = startpoint; i < dict2[dict[parts[0]]] + 1; i++)
                {
                    word += ' ';
                    word += 'R';
                    word += parts[i].ToString("x2");
                }
                for (int i = (int)dict2[dict[parts[0]]] + 1; i < parts.Length; i++)
                {
                    word += ' ';

                    word += parts[i].ToString("x2");
                }
                words[k] = word;
            }

            return words;
        }
    }
}