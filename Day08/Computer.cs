/*
 * Solutions to the Advent Of Code 2020
 * Copyright © 2021 Leland Roach
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Day08
{
    public enum OpCode
    {
        invalid = 0,
        acc,
        jmp,
        nop,
    };

    public class State
    {
        /// <summary>Accumulator.</summary>
        public int Eax { get; set; }
        /// <summary>Instruction pointer, or the index of the instruction that will execute next.</summary>
        public int Eip { get; set; }
        /// <summary>List of instruction indices that have already been executed.</summary>
        public List<int> Esp { get; set; }

        public State(int rax = 0, int rip = 0, List<int> rsp = null)
        {
            Eax = rax;
            Eip = rip;
            Esp = rsp ?? new List<int>();
        }

        public void Reset(int rax = 0, int rip = 0, List<int> rsp = null)
        {
            Eax = rax;
            Eip = rip;
            Esp = rsp ?? new List<int>();
        }
    }

    public class Instruction
    {
        /// <summary>The argument or data for the instruction.</summary>
        public int Argument { get; set; }
        /// <summary>The operation that the instruction will perform.</summary>
        public OpCode Operation { get; set; }

        public bool CanFlip => Operation == OpCode.jmp || Operation == OpCode.nop;

        public Instruction(in Instruction instruction)
        {
            Argument = instruction.Argument;
            Operation = instruction.Operation;
        }

        public Instruction(in string instruction)
        {
            Argument = int.Parse(instruction[4..]);
            Operation = Enum.Parse<OpCode>(instruction[0..3]);
        }

        public bool Flip()
        {
            switch (Operation)
            {
                case OpCode.jmp:
                    Operation = OpCode.nop;
                    return true;

                case OpCode.nop:
                    Operation = OpCode.jmp;
                    return true;

                case OpCode.acc:
                case OpCode.invalid:
                    return false;

                default:
                    throw new NotImplementedException();
            }
        }

        public static List<Instruction> Parse(in List<string> instructions)
        {
            var result = new List<Instruction>(instructions.Count);
            foreach(var inst in instructions)
            {
                result.Add(new Instruction(inst));
            }
            return result;
        }
    };

    public class Computer
    {
        public State State { get; set; }

        /// <summary>The program being executed.</summary>
        public List<Instruction> Program { get; set; }


        public Computer(List<Instruction> program = null, State state = null)
        {
            Program = program ?? new List<Instruction>();
            State = state ?? new State();
        }

        public Computer(List<string> program = null, State state = null)
        {
            Program = new List<Instruction>();
            State = state ?? new State();

            if (program != null && program.Any())
            {
                foreach (var inst in program)
                {
                    Program.Add(new Instruction(inst));
                }
            }
        }


        /// <returns><c>true</c> if the program ran to completion; <c>false</c> if it terminated early.</returns>
        public bool Run()
        {
            while (!State.Esp.Contains(State.Eip) && State.Eip < Program.Count)
            {
                Instruction inst = Program[State.Eip];
                State.Esp.Add(State.Eip);

                switch (inst.Operation)
                {
                    case OpCode.acc:
                        State.Eax += inst.Argument;
                        break;
                    case OpCode.jmp:
                        State.Eip += inst.Argument;
                        continue;
                    case OpCode.nop:
                        break;
                    default:
                        throw new ApplicationException($"Invalid OpCode in Program at instruction {State.Eip}: {inst}");
                }
                State.Eip++;
            }

            return State.Eip == Program.Count;
        }
    }
}
