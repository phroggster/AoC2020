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
    class Program08
    {
        static void Main(string[] args)
        {
            Part1();
            Part2();
        }

        /*
         * --- Day 8: Handheld Halting ---
         * Your flight to the major airline hub reaches cruising altitude without incident. While you consider checking the
         * in-flight menu for one of those drinks that come with a little umbrella, you are interrupted by the kid sitting
         * next to you.
         * 
         * Their handheld game console won't turn on! They ask if you can take a look.
         * 
         * You narrow the problem down to a strange infinite loop in the boot code (your puzzle input) of the device. You
         * should be able to fix it, but first you need to be able to run the code in isolation.
         * 
         * The boot code is represented as a text file with one instruction per line of text. Each instruction consists of
         * an operation (acc, jmp, or nop) and an argument (a signed number like +4 or -20).
         * 
         * acc increases or decreases a single global value called the accumulator by the value given in the argument. For
         *    example, acc +7 would increase the accumulator by 7. The accumulator starts at 0. After an acc instruction,
         *    the instruction immediately below it is executed next.
         * jmp jumps to a new instruction relative to itself. The next instruction to execute is found using the argument
         *    as an offset from the jmp instruction; for example, jmp +2 would skip the next instruction, jmp +1 would
         *    continue to the instruction immediately below it, and jmp -20 would cause the instruction 20 lines above to
         *    be executed next.
         * nop stands for No OPeration - it does nothing. The instruction immediately below it is executed next.
         * 
         * For example, consider the following program:
         * 
         * nop +0
         * acc +1
         * jmp +4
         * acc +3
         * jmp -3
         * acc -99
         * acc +1
         * jmp -4
         * acc +6
         * 
         * These instructions are visited in this order:
         * 
         * nop +0  | 1
         * acc +1  | 2, 8(!)
         * jmp +4  | 3
         * acc +3  | 6
         * jmp -3  | 7
         * acc -99 |
         * acc +1  | 4
         * jmp -4  | 5
         * acc +6  |
         * 
         * First, the nop +0 does nothing. Then, the accumulator is increased from 0 to 1 (acc +1) and jmp +4 sets the next
         * instruction to the other acc +1 near the bottom. After it increases the accumulator from 1 to 2, jmp -4
         * executes, setting the next instruction to the only acc +3. It sets the accumulator to 5, and jmp -3 causes the
         * program to continue back at the first acc +1.
         * 
         * This is an infinite loop: with this sequence of jumps, the program will run forever. The moment the program
         * tries to run any instruction a second time, you know it will never terminate.
         * 
         * Immediately before the program would run an instruction a second time, the value in the accumulator is 5.
         * 
         * Run your copy of the boot code. Immediately before any instruction is executed a second time, what value is in
         * the accumulator?
         */
        static void Part1()
        {
            Computer cmp = null;
            int result = 0;

            {
                cmp = new Computer(Data.Test.Item1);
                bool retval = cmp.Run();
                result = cmp.State.Eax;

                if (retval)
                    throw new ApplicationException("Test program did not run into a loop as it was expected to do! Cannot continue.");
                else if (result != Data.Test.Item2)
                    throw new ApplicationException("Test data was not executed correctly, cannot continue.");

                cmp = null;
                result = 0;
                retval = false;
            }

            cmp = new Computer(Data.Actual);
            cmp.Run();
            Console.WriteLine(cmp.State.Eax);
            // Solution: 1501
        }

        /*
         * --- Part Two ---
         * After some careful analysis, you believe that exactly one instruction is corrupted.
         * 
         * Somewhere in the program, either a jmp is supposed to be a nop, or a nop is supposed to be a jmp. (No acc
         * instructions were harmed in the corruption of this boot code.)
         * 
         * The program is supposed to terminate by attempting to execute an instruction immediately after the last
         * instruction in the file. By changing exactly one jmp or nop, you can repair the boot code and make it terminate
         * correctly.
         * 
         * For example, consider the same program from above:
         * 
         * nop +0
         * acc +1
         * jmp +4
         * acc +3
         * jmp -3
         * acc -99
         * acc +1
         * jmp -4
         * acc +6
         * 
         * If you change the first instruction from nop +0 to jmp +0, it would create a single-instruction infinite loop,
         * never leaving that instruction. If you change almost any of the jmp instructions, the program will still
         * eventually find another jmp instruction and loop forever.
         * 
         * However, if you change the second-to-last instruction (from jmp -4 to nop -4), the program terminates! The
         * instructions are visited in this order:
         * 
         * nop +0  | 1
         * acc +1  | 2
         * jmp +4  | 3
         * acc +3  |
         * jmp -3  |
         * acc -99 |
         * acc +1  | 4
         * nop -4  | 5
         * acc +6  | 6
         * 
         * After the last instruction (acc +6), the program terminates by attempting to run the instruction below the last
         * instruction in the file. With this change, after the program terminates, the accumulator contains the value 8
         * (acc +1, acc +1, acc +6).
         * 
         * Fix the program so that it terminates normally by changing exactly one jmp (to nop) or nop (to jmp). What is the
         * value of the accumulator after the program terminates?
         */
        static void Part2()
        {
            List<Instruction> program = null;
            int result = 0;
            {
                program = Instruction.Parse(Data.Test.Item1);
                result = Part2Solver(program);
                if (result != Data.Test.Item3)
                    throw new ApplicationException("Test program failed to complete.");

                program.Clear();
                result = 0;
            }

            program = Instruction.Parse(Data.Actual);
            result = Part2Solver(program);
            Console.Write(result);
            // Solution: 509
        }

        static int Part2Solver(List<Instruction> instructions)
        {
            var cmp = new Computer(instructions);
            var failedMods = new List<int>((int)(0.66f * instructions.Count));
            int lastMod = -1;

            while (!cmp.Run())
            {
                if (lastMod >= 0)
                {
                    cmp.Program[lastMod].Flip();
                    failedMods.Add(lastMod);
                }

                // TODO: linq over cmp.state.esp wasn't working for some reason.
                // Also, this doesn't check for an unsolvable problem. oops.
                var stack = new List<int>(cmp.State.Esp);
                stack.Reverse();

                foreach(var idx in stack)
                {
                    if (cmp.Program[idx].CanFlip && !failedMods.Contains(idx))
                    {
                        cmp.Program[idx].Flip();
                        lastMod = idx;
                        break;
                    }
                }

                cmp.State.Reset();
            }

            return cmp.State.Eax;
        }
    }

    static class Data
    {
        // Tuple<Program, Accumulator-pt1, Accumulator-pt2>
        public static readonly Tuple<List<string>, int, int> Test = new Tuple<List<string>, int, int>(
        new List<string> {
            "nop +0", // 0: 0
            "acc +1", // 1: 1; **7**
            "jmp +4", // 2: 2
            "acc +3", // 3: 5
            "jmp -3", // 4: 6
            "acc -99",// 5:
            "acc +1", // 6: 3
            "jmp -4", // 7: 4; **nop -4**
            "acc +6"  // 8:
        }, 5, 8);

        public static readonly List<string> Actual = new List<string>
        {
            "jmp +1",
            "acc -15",
            "acc +14",
            "acc +18",
            "jmp +443",
            "jmp +286",
            "acc +27",
            "jmp +522",
            "jmp +1",
            "acc -19",
            "acc +22",
            "acc +37",
            "jmp +111",
            "acc +28",
            "acc +43",
            "acc +18",
            "nop +597",
            "jmp +479",
            "jmp +604",
            "jmp +499",
            "acc +0",
            "acc +22",
            "acc +13",
            "jmp +566",
            "acc -12",
            "acc +0",
            "nop +153",
            "jmp +173",
            "jmp +192",
            "jmp +292",
            "acc +36",
            "acc +7",
            "jmp +440",
            "acc -17",
            "acc +40",
            "acc +24",
            "acc -7",
            "jmp +519",
            "nop +16",
            "acc +15",
            "acc +42",
            "jmp +445",
            "jmp +350",
            "acc +42",
            "acc +12",
            "acc +2",
            "jmp +133",
            "acc +12",
            "acc +3",
            "acc +27",
            "jmp +186",
            "acc +25",
            "acc +46",
            "jmp +285",
            "acc +32",
            "acc -11",
            "acc -6",
            "jmp +565",
            "nop +215",
            "acc +1",
            "acc +35",
            "jmp +1",
            "jmp +502",
            "acc +27",
            "acc +19",
            "acc -8",
            "acc -8",
            "jmp +531",
            "jmp -21",
            "nop +292",
            "acc +8",
            "acc -13",
            "jmp +26",
            "acc +1",
            "acc +45",
            "nop -42",
            "jmp +323",
            "jmp +39",
            "jmp +336",
            "acc +19",
            "jmp -51",
            "acc +45",
            "acc +26",
            "jmp +278",
            "jmp +6",
            "acc +40",
            "nop +271",
            "acc -10",
            "nop -4",
            "jmp +272",
            "nop -61",
            "acc +4",
            "acc -14",
            "acc +27",
            "jmp -70",
            "acc -9",
            "acc +29",
            "jmp +416",
            "acc +25",
            "acc +45",
            "jmp +19",
            "jmp +39",
            "acc -19",
            "acc +7",
            "jmp +248",
            "acc +11",
            "acc +36",
            "jmp +515",
            "acc +45",
            "acc +49",
            "jmp +329",
            "acc +30",
            "acc +31",
            "acc +28",
            "acc +26",
            "jmp +8",
            "jmp +283",
            "acc +32",
            "jmp +127",
            "acc +4",
            "acc +20",
            "jmp +92",
            "jmp +50",
            "jmp +133",
            "acc +5",
            "acc +8",
            "jmp +313",
            "acc +38",
            "acc +34",
            "jmp +395",
            "acc +14",
            "acc +29",
            "jmp +392",
            "nop +246",
            "jmp +374",
            "nop +429",
            "nop +388",
            "acc +3",
            "acc +0",
            "jmp +432",
            "acc -1",
            "acc +35",
            "acc +35",
            "jmp +148",
            "acc +8",
            "acc +11",
            "acc +12",
            "acc -10",
            "jmp +434",
            "acc -19",
            "jmp +330",
            "nop +329",
            "acc +30",
            "jmp +239",
            "acc -6",
            "jmp -136",
            "jmp +418",
            "nop +385",
            "jmp +1",
            "acc +34",
            "acc +9",
            "jmp +410",
            "nop -13",
            "acc +31",
            "acc +15",
            "acc +37",
            "jmp -142",
            "jmp +109",
            "acc -16",
            "nop +405",
            "nop +343",
            "jmp +8",
            "acc +44",
            "acc -15",
            "acc +7",
            "acc +9",
            "jmp +185",
            "acc +6",
            "jmp +35",
            "nop -25",
            "jmp +93",
            "acc +22",
            "acc -17",
            "acc +15",
            "acc +39",
            "jmp +41",
            "nop -123",
            "acc +15",
            "acc +6",
            "jmp -35",
            "acc +48",
            "jmp +422",
            "acc -7",
            "nop +67",
            "nop +66",
            "acc +48",
            "jmp -29",
            "acc -11",
            "acc +16",
            "jmp +92",
            "acc +45",
            "jmp +92",
            "jmp +212",
            "acc -3",
            "acc -18",
            "nop -186",
            "nop +7",
            "jmp -28",
            "nop +292",
            "acc +7",
            "nop -120",
            "acc +46",
            "jmp +48",
            "acc -3",
            "acc -16",
            "acc +50",
            "jmp -44",
            "acc -2",
            "acc -11",
            "jmp +236",
            "jmp +344",
            "acc +33",
            "acc +44",
            "acc +39",
            "nop -45",
            "jmp -53",
            "acc -11",
            "nop +380",
            "acc +35",
            "jmp +113",
            "nop +203",
            "acc +40",
            "jmp +167",
            "acc +44",
            "jmp +394",
            "jmp +229",
            "jmp -167",
            "jmp -204",
            "acc +21",
            "acc +49",
            "jmp +25",
            "acc -19",
            "acc -17",
            "acc +44",
            "jmp -11",
            "acc +40",
            "acc +12",
            "jmp +253",
            "acc +21",
            "jmp +349",
            "jmp +285",
            "acc +0",
            "nop +261",
            "acc +15",
            "acc +38",
            "jmp +10",
            "acc +27",
            "jmp +1",
            "jmp +373",
            "jmp -151",
            "acc +6",
            "jmp -48",
            "acc +14",
            "acc -8",
            "jmp -61",
            "acc +8",
            "acc +20",
            "jmp +1",
            "jmp +1",
            "jmp +208",
            "acc -18",
            "acc +32",
            "jmp +94",
            "jmp +262",
            "acc +0",
            "jmp -156",
            "nop +188",
            "nop +312",
            "acc +21",
            "acc +6",
            "jmp -123",
            "acc +47",
            "jmp +316",
            "acc +25",
            "nop +290",
            "jmp +62",
            "acc -7",
            "acc +36",
            "nop +212",
            "acc +14",
            "jmp +332",
            "jmp +291",
            "jmp +226",
            "acc +30",
            "jmp -161",
            "acc +39",
            "acc +38",
            "jmp +203",
            "nop +63",
            "nop -6",
            "acc -15",
            "nop -56",
            "jmp +72",
            "acc +1",
            "acc +34",
            "acc +22",
            "acc +19",
            "jmp -135",
            "acc +27",
            "jmp -303",
            "acc +1",
            "acc +48",
            "acc -19",
            "jmp +142",
            "acc +50",
            "jmp +298",
            "acc +43",
            "acc +0",
            "acc +50",
            "acc +12",
            "jmp +137",
            "acc +41",
            "nop +252",
            "jmp -310",
            "acc +13",
            "acc +34",
            "acc -15",
            "acc +43",
            "jmp +236",
            "acc +5",
            "acc -8",
            "acc +25",
            "acc +45",
            "jmp +153",
            "acc -12",
            "acc +31",
            "acc -1",
            "jmp +120",
            "jmp +236",
            "acc +38",
            "nop -238",
            "jmp -328",
            "jmp +81",
            "acc +48",
            "acc +15",
            "acc -9",
            "jmp -73",
            "nop -49",
            "jmp -271",
            "acc -17",
            "acc -17",
            "jmp +106",
            "nop +212",
            "jmp -290",
            "acc +36",
            "nop +109",
            "jmp +186",
            "jmp -310",
            "acc +4",
            "acc +16",
            "jmp +117",
            "jmp +1",
            "acc +10",
            "jmp +20",
            "acc +12",
            "jmp -311",
            "acc +12",
            "acc +30",
            "nop +182",
            "jmp -315",
            "acc +25",
            "acc +12",
            "acc +30",
            "jmp +50",
            "acc -19",
            "jmp -333",
            "acc +30",
            "nop +87",
            "jmp -199",
            "acc +8",
            "jmp +112",
            "acc -8",
            "jmp -313",
            "acc +7",
            "acc +32",
            "jmp +1",
            "jmp +230",
            "acc +25",
            "acc +45",
            "acc +20",
            "acc +0",
            "jmp -307",
            "acc +30",
            "nop -253",
            "acc +7",
            "acc +39",
            "jmp -113",
            "acc -12",
            "jmp +209",
            "acc +42",
            "acc +17",
            "acc -19",
            "acc +24",
            "jmp -170",
            "acc +30",
            "acc +9",
            "acc -1",
            "jmp -328",
            "acc +19",
            "acc +45",
            "jmp +132",
            "nop -244",
            "nop +35",
            "jmp +34",
            "acc -10",
            "acc +26",
            "acc +35",
            "nop -238",
            "jmp +54",
            "acc +15",
            "nop -378",
            "acc +42",
            "jmp -43",
            "acc -9",
            "acc -5",
            "acc -11",
            "nop -307",
            "jmp -129",
            "nop -202",
            "acc -9",
            "nop -376",
            "acc +11",
            "jmp -75",
            "jmp +14",
            "acc -1",
            "acc +32",
            "acc -14",
            "acc +16",
            "jmp +39",
            "acc +42",
            "acc +32",
            "jmp -133",
            "acc +1",
            "acc +17",
            "nop +85",
            "acc +35",
            "jmp +83",
            "acc +27",
            "acc +0",
            "acc -12",
            "jmp -93",
            "acc +48",
            "acc +35",
            "nop +154",
            "jmp -287",
            "jmp -347",
            "jmp -348",
            "acc +18",
            "jmp -374",
            "acc -15",
            "jmp +36",
            "jmp -123",
            "acc -11",
            "jmp +55",
            "acc +19",
            "acc +23",
            "jmp -339",
            "nop +5",
            "acc +44",
            "acc +2",
            "jmp +1",
            "jmp -417",
            "acc +23",
            "jmp -253",
            "acc -9",
            "acc -3",
            "jmp -138",
            "jmp -227",
            "acc +12",
            "jmp -437",
            "acc +47",
            "acc +19",
            "acc -6",
            "jmp -245",
            "acc +2",
            "jmp -328",
            "acc -14",
            "acc +25",
            "acc +4",
            "acc -2",
            "jmp -411",
            "jmp -351",
            "jmp -459",
            "acc +3",
            "acc +48",
            "jmp -134",
            "nop +54",
            "acc -14",
            "jmp -298",
            "jmp -401",
            "acc -14",
            "acc +25",
            "nop -55",
            "acc -10",
            "jmp -312",
            "acc -7",
            "acc +45",
            "jmp -74",
            "acc +30",
            "jmp -462",
            "acc +5",
            "acc -8",
            "jmp -355",
            "acc +9",
            "acc +44",
            "acc +44",
            "jmp -150",
            "jmp -484",
            "acc +14",
            "acc +19",
            "acc -6",
            "jmp -474",
            "acc -18",
            "jmp -166",
            "jmp -264",
            "acc -15",
            "acc +17",
            "acc +29",
            "jmp -149",
            "nop -273",
            "acc +31",
            "acc +0",
            "acc -2",
            "jmp -410",
            "jmp -411",
            "acc +47",
            "acc -6",
            "nop -287",
            "jmp -436",
            "acc +4",
            "nop +88",
            "jmp -158",
            "acc +32",
            "jmp +1",
            "acc -15",
            "jmp -319",
            "acc -6",
            "acc -18",
            "acc +49",
            "jmp -256",
            "acc -18",
            "acc +31",
            "acc +27",
            "acc +27",
            "jmp -351",
            "jmp +58",
            "acc +12",
            "jmp +1",
            "acc +32",
            "nop -151",
            "jmp -411",
            "acc +19",
            "acc +7",
            "jmp -287",
            "acc +30",
            "jmp -496",
            "acc -11",
            "acc +5",
            "acc +42",
            "acc +25",
            "jmp -249",
            "acc -1",
            "jmp -243",
            "jmp -190",
            "acc +32",
            "acc +32",
            "acc +14",
            "jmp +12",
            "acc +5",
            "acc +30",
            "acc +34",
            "jmp -46",
            "acc -13",
            "acc +5",
            "acc +45",
            "jmp -271",
            "acc +29",
            "acc +37",
            "jmp -323",
            "nop -18",
            "acc -2",
            "acc +21",
            "acc -12",
            "jmp -453",
            "acc -14",
            "acc +19",
            "nop -173",
            "jmp -411",
            "acc +24",
            "acc -7",
            "nop -136",
            "acc +6",
            "jmp -357",
            "acc -1",
            "acc -1",
            "acc +32",
            "jmp -264",
            "acc +26",
            "jmp -175",
            "acc +10",
            "acc +35",
            "nop -361",
            "jmp -493",
            "acc +14",
            "jmp -206",
            "jmp -138",
            "acc -1",
            "jmp -156",
            "acc +3",
            "acc +11",
            "acc -2",
            "jmp -213",
            "acc +35",
            "acc -13",
            "acc +47",
            "acc +45",
            "jmp -376",
            "jmp -543",
            "jmp -479",
            "acc +29",
            "jmp -532",
            "acc +28",
            "acc +47",
            "acc -11",
            "acc -14",
            "jmp +1"
        };
    }
}
