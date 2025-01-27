using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
namespace projectclass
{
    public class Animal
    {
        
        public Animal()
        {
            Type type = GetType();
            Type type2 = typeof(Animal);
            if(type.IsSubclassOf(type2) && type.GetMethods(System.Reflection.BindingFlags.Public|System.Reflection.BindingFlags.Instance| System.Reflection.BindingFlags.DeclaredOnly).Length<2)
            {
                throw new Exception("Not Enough Methods");
            }
            
        }
    }

    public class Cat : Animal
    {
        public int Age;
        public string Name;

        public void PrintName()
        {
            Console.WriteLine(Name);
        }
        public void PrintAge()
        {
            Console.WriteLine(Age);
        }
    }


    class LLN
    {
        public int value { get; set; }
        public LLN Next { get; set; }
        public static LLN Empty = new LLN(0);
        public LLN(int Value, LLN next = null)
        {
            value = Value;
            Next = next;
        }
    }
    class Program
    {
        enum OpCodes : uint
        {
            //Memory
            POP = 0x45,
            PUSH = 0x44,
            STORE = 0x43,
            LOAD = 0x42,
            COPY = 0x41,
            SET = 0x40,
            //Control
            JMPZ = 0x32,
            JMPT = 0x31,
            JMP = 0x30,
            //Logic
            EQ = 0x26,
            LSEQ = 0x25,
            GREQ = 0x24,
            NOT = 0x23,
            XOR = 0x22,
            OR = 0x21,
            AND = 0x20,
            // CS Math
            SFTL = 0x16,
            SFTR = 0x15,
            MOD = 0x14,
            DIV = 0x13,
            MLT = 0x12,
            SUB = 0x11,
            ADD = 0x10,

            //NULL
            NOP = 0x00,
        }

        static byte[] Registers = new byte[32];
        static byte[] memory = new byte[256];
        static int[] Instructions = new int[10];

        static int instrutionpointer = 31;
        static int Stackpointer = 31;
        static Stack<byte> stack = new Stack<byte>();
        public static Dictionary<string, uint> adictionary = new Dictionary<string, uint>()
        {
            ["NOP"] = 0x00,
            ["ADD"] = 0x10,
            ["SUB"] = 0x11,
            ["MULT"] =0x12,
            ["DIV"]  =0x13,
            ["MOD"]  =0x14,
            ["SHR"]  =0x15,
            ["SHL"]  =0x16,
            ["AND"]  =0x20,
            ["OR"]   =0x21,
            ["XOR"] = 0x22,
            ["NOT"] = 0x23,
            ["GTE"] = 0x24,
            ["LTE"] = 0x25,
            ["EQ"]  = 0x26,
            ["JMP"] = 0X30,
            ["JMPT"] = 0X31,
            ["JMPZ"] = 0X32,
            ["SET"] = 0x40,
            ["COPY"] =0x41,
            ["LOAD"] =0x42,
            ["STORE"]=0X43,
            ["PUSH"] = 0X44,
            ["POP"] = 0X45,
        };
        public static Dictionary<string, uint> registercountdictionary = new Dictionary<string, uint>()
        {
            ["NOP"] = 0,
            ["ADD"] = 3,
            ["SUB"] = 3,
            ["MULT"] =3,
            ["DIV"] = 3,
            ["MOD"] = 3,
            ["SHR"] = 2,
            ["SHL"] = 2,
            ["AND"] = 3,
            ["OR"] =  3,
            ["XOR"] = 3,
            ["NOT"] = 3,
            ["GTE"] = 3,
            ["LTE"] = 3,
            ["EQ"] = 3,
            ["JMP"] = 1,
            ["JMPT"] = 2,
            ["JMPZ"] = 2,
            ["SET"] = 1,
            ["COPY"] = 2,
            ["LOAD"] = 2,
            ["STORE"] = 2,
            ["PUSH"] = 1,
            ["POP"] = 1,
        };
        public static Dictionary<uint, string> ddictionary = new Dictionary<uint, string>();
        

        Instruction[] GetInstructions()
        {
            Type type = typeof(Instruction);

            // Instruction[] instructions =
            return null;
        }
        static string[] intarrtostringarr(uint[] array)
        {
            string[] temp = new string[array.Length];
            for(int i=0; i<array.Length;i++)
            {
                temp[i] = array[i].ToString("x8");
            }
            return temp;
        }
        static void Main(string[] args)
        {
            foreach (KeyValuePair<string, uint> i in adictionary)
            {
                ddictionary.Add(i.Value,i.Key);
            }
            string[] instructions = LoadAssemblyProgram();
            uint[] assemblycode = new uint[instructions.Length];
            string[] instructions2 = new string[assemblycode.Length];
          
                assemblycode = Instruction.Parse(instructions,adictionary);
           
            ;
            
                instructions2 = Instruction.UnParse(assemblycode, ddictionary,registercountdictionary);

            ;
            File.WriteAllLines("starttext.txt", intarrtostringarr(assemblycode));
             assemblycode = Instruction.Parse(instructions2, adictionary);
            File.WriteAllLines("finaltext.txt", intarrtostringarr(assemblycode));
            ;
            Registers[instrutionpointer] = 0;
            while (Registers[instrutionpointer] < assemblycode.Length)
            {
                DoWork(assemblycode[Registers[instrutionpointer]]);
                Registers[instrutionpointer]++;
            }
            ;
















            Console.WriteLine();
        }
        string disassembler(uint mcode)
        {
            StringBuilder newstring = new StringBuilder();
            if ((mcode >> 24) == 0x10)
            {
                newstring.Append("ADD ");
                for (int i = 0; i < 3; i++)
                {
                    newstring.Append("R");
                    newstring.Append(mcode >> (8 * (2 - i)));
                    newstring.Append(" ");
                }
            }
            return newstring.ToString();
        }
        //Assembler----------------------------
       
        static string[] LoadAssemblyProgram()
        {
            return System.IO.File.ReadAllLines("Program.asm");

        }
        //Assembler----------------------------
        static void Print(LLN head)
        {
            while (head != null)
            {
                Console.Write(head.value);
                Console.Write("->");
                head = head.Next;
            }
            Console.WriteLine();
        }
        static unsafe void LoopThrough(int[] array, int start, int end)
        {
            int counter = 0;
            fixed (int* ptr = array)
            {
                while (counter < end)
                {
                    Console.WriteLine(*(ptr + start + counter));
                    counter++;
                }
            }
        }
        static void Subtraction(int val1, int val2)
        {
            val2 = (~val2) + 1;
            val1 += val2;
        }
        static LLN ADDLL(LLN head1, LLN head2)
        {
            LLN newhead = LLN.Empty;
            LLN nexttemp = newhead;
            int carry = 0;
            while (!(head1 == LLN.Empty && head2 == LLN.Empty) || carry != 0)
            {
                int temp = 0;
                if (head1 == LLN.Empty && head2 == LLN.Empty)
                {
                    temp = carry;
                }
                else if (head1 == LLN.Empty)
                {
                    temp = head2.value + carry;
                    head2 = head2.Next;
                }
                else if (head2 == LLN.Empty)
                {
                    temp = head1.value + carry;
                    head1 = head1.Next;
                }
                else
                {
                    temp = head2.value + head1.value + carry;
                    head1 = head1.Next;
                    head2 = head2.Next;
                }
                if (temp >= 2)
                {
                    temp -= 2;
                    carry = 1;
                }
                else
                {
                    carry = 0;
                }
                nexttemp.value = temp;
                nexttemp.Next = LLN.Empty;
                nexttemp = nexttemp.Next;


            }
            nexttemp = null;
            return newhead;

        }
        static unsafe void Bubble(int[] array, int start, int end)
        {
            bool SortDone = false;

            fixed (int* ptr = array)
            {
                while (!SortDone)
                {
                    SortDone = true;
                    int counter = 1;
                    while (counter < end)
                    {
                        if (ptr[start + counter] < ptr[start + counter - 1])
                        {
                            int temp = ptr[start + counter];
                            ptr[start + counter] = ptr[start + counter - 1];
                            ptr[start + counter - 1] = temp;
                            SortDone = false;
                        }
                        counter++;
                    }
                }

            }
        }
        static bool PowerofTwo(int number)
        {

            bool temp = true;
            while (number > 1)
            {
                if (number != number >> 1 << 1)
                {
                    temp = false;
                }
                number >>= 1;
            }
            return temp;
        }
        static int ClosestPoweroftwo(int number)
        {
            return 2;
        }
        static bool IsNthBitOne(int number, int bitIndex)
        {
            number >>= bitIndex;
            number = number & 1;
            return number == 1;
        }
        static byte NthByte(uint number, int bitIndex)
        {
            number >>= (bitIndex * 8);
            number = number & 255;
            return (byte)number;
        }
        static int NthFlip(int number, int byteindex)
        {
            if (IsNthBitOne(number, byteindex))
            {
                return number - (int)Math.Pow(2, byteindex);
            }
            return number + (int)Math.Pow(2, byteindex);
        }
        static void DoWork(uint input)
        {
            uint opperator = NthByte(input, 3);
            byte val1 = NthByte(input, 2);
            byte val2 = NthByte(input, 1);
            byte val3 = NthByte(input, 0);

            switch ((OpCodes)opperator)
            {
                case OpCodes.POP:
                    Registers[val1]=stack.Pop();
                    break;
                case OpCodes.PUSH:
                    stack.Push(Registers[val1]);
                    break;
                case OpCodes.STORE:
                    memory[Registers[val2]] = Registers[val1];
                    break;
                case OpCodes.LOAD:
                    Registers[val1]=memory[val2];
                    break;
                case OpCodes.COPY:
                    Registers[val2] = Registers[val1];
                    break;
                case OpCodes.SET:
                    Registers[val1] = (byte)val2;
                    break;
                case OpCodes.JMPZ:
                    if (Registers[val2] == 0)
                    {
                        Registers[instrutionpointer] = val1;
                    }
                    break;
                case OpCodes.JMPT:
                    if (Registers[val2] == 1)
                    {
                        Registers[instrutionpointer] = val1;
                    }
                    break;
                case OpCodes.JMP:
                    Registers[instrutionpointer] = val1;
                    break;
                case OpCodes.EQ:
                    if (Registers[val1] == Registers[val2])
                    {
                        Registers[val3] = 1;
                    }
                    else
                    {
                        Registers[val3] = 0;
                    }
                    break;
                case OpCodes.LSEQ:
                    if (Registers[val1] <= Registers[val2])
                    {
                        Registers[val3] = 1;
                    }
                    else
                    {
                        Registers[val3] = 0;
                    }
                    break;
                case OpCodes.GREQ:
                    if (Registers[val1] >= Registers[val2])
                    {
                        Registers[val3] = 1;
                    }
                    else
                    {
                        Registers[val3] = 0;
                    }
                    break;
                case OpCodes.NOT:
                    if (Registers[val1] != 1 )
                    {
                        Registers[val2] = 1;
                    }
                    break;
                case OpCodes.XOR:
                    if (Registers[val2] != 1 && Registers[val1] != 1)
                    {
                        Registers[val3] = 1;
                    }
                    break;
                case OpCodes.OR:
                    if (Registers[val2] == 1 || Registers[val1] == 1)
                    {
                        Registers[val3] = 1;
                    }
                    break;
                case OpCodes.AND:
                    if (Registers[val2] ==1&& Registers[val1]==1)
                    {
                        Registers[val3] = 1;
                    }
                    break;
                case OpCodes.SFTL:
                    Registers[val2] = Registers[val1] <<= 1;
                    break;
                case OpCodes.SFTR:
                    Registers[val2] = Registers[val1] >>= 1;
                    break;
                case OpCodes.MOD:
                    Registers[val3] = (byte)(Registers[val1] % Registers[val2]);
                    break;
                case OpCodes.DIV:
                    Registers[val3] = (byte)(Registers[val1]/Registers[val2]);
                    break;
                case OpCodes.MLT:
                    int counter = 0;
                Loop:
                    Registers[val3] = (byte)(Registers[val1] + Registers[val3]);
                    counter++;
                    if (counter < val2)
                    {
                        goto Loop;
                    }
                    break;
                case OpCodes.SUB:
                    Registers[val3] = (byte)(Registers[val1] - (byte)Registers[2]);
                    break;
                case OpCodes.ADD:
                    Registers[val3] = (byte)(Registers[val1] + (byte)Registers[val2]);
                    break;
                case OpCodes.NOP:
                    break;
            }
        }
        static void Loop(int times)
        {
            int X = 0;
        ReLoop:
            if (X != times)
            {
                X++;
                goto ReLoop;
            }

        }
        /*
         * SET R1 0 
         * SET R2 1
         * Lb1:
         * ADD R1 R2 R1
         * JMP Lb1
         * 0x40000000
         * 0x40010001
         * 
         * 0x10000100
         * 0x30FF0002
         * Destination,Source,Source
        */

    }
}
